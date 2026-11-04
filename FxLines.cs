using Godot;

namespace Arc.Fx;

/// <summary>
/// Lines along segments: lightning (jittering strands of a white core in a blue sheath, with
/// optional tendrils, reshaped every 0.06 s) and fire walls (ragged rows of flame tongues,
/// fx_flame, over strips of burning ground). Both are drawn <see cref="Lift"/> metres above
/// each end, so a line along the ground stands at chest height and one from the sky down to a
/// spot on the ground is a strike.
/// </summary>
internal sealed class FxLines(FxKit kit)
{
    /// <summary>How far above each end a line is drawn.</summary>
    public const float Lift = 0.9f;
    private const int BoltKinks = 4;
    /// <summary>Each decorative tendril off a bolt is a twig of this many pieces.</summary>
    private const int TendrilPieces = 2;
    private const float FlameSpacing = 0.3f;
    private const float FireBedRadius = 0.6f;
    private const float Reshape = 0.06f;
    private const int FireStyle = 1;

    private sealed class Line : FxItem
    {
        public required MeshInstance3D[] Strands;
        /// <summary>A fire wall's burning ground, one per segment (none for lightning).</summary>
        public required MeshInstance3D[] Beds;
        public bool Fire;
        public int Tendrils;
        public (Vector3 A, Vector3 B)[] Segments = [];
        public int[] FirstStrand = [];
        public float NextShape;
    }

    private readonly List<Line> _pool = [];
    private readonly ShaderMaterial _flameMaterial = FxKit.Shader("flame");

    public FxHandle Spawn((Vector3 A, Vector3 B)[] segments, float life, bool fire, int tendrils)
    {
        if (fire)
            tendrils = 0;
        var first = new int[segments.Length];
        var count = 0;
        for (var i = 0; i < segments.Length; i++)
        {
            first[i] = count;
            count += fire
                ? Mathf.Max(2, Mathf.CeilToInt(FxKit.Flat(segments[i].A).DistanceTo(FxKit.Flat(segments[i].B)) / FlameSpacing) + 1)
                : BoltKinks + tendrils * TendrilPieces;
        }
        var beds = fire ? segments.Length : 0;
        var line = _pool.Find(l => !l.Active && l.Fire == fire && l.Strands.Length == count && l.Beds.Length == beds) ?? Make(count, beds, fire);
        line.Begin(life);
        line.Segments = segments;
        line.FirstStrand = first;
        line.Tendrils = tendrils;
        line.NextShape = 0f;
        foreach (var strand in line.Strands)
            strand.Visible = true;
        for (var i = 0; i < beds; i++)
            LayBed(line.Beds[i], FxKit.Flat(segments[i].A), FxKit.Flat(segments[i].B), life);
        Tick(line, 0f);
        return new FxHandle(line);
    }

    public void Tick(float dt)
    {
        foreach (var line in _pool)
        {
            if (line.Active)
                Tick(line, dt);
        }
    }

    private static void Tick(Line line, float dt)
    {
        line.Age += dt;
        if (line.Age >= line.Life)
        {
            line.Active = false;
            foreach (var strand in line.Strands)
                strand.Visible = false;
            foreach (var bed in line.Beds)
                bed.Visible = false;
            return;
        }
        foreach (var bed in line.Beds)
            ((ShaderMaterial)bed.MaterialOverride).SetShaderParameter("uAge", line.Age);
        Light(line);
        if (line.Age < line.NextShape)
            return;
        line.NextShape += Reshape;
        for (var i = 0; i < line.Segments.Length; i++)
        {
            var (a, b) = line.Segments[i];
            var end = i + 1 < line.FirstStrand.Length ? line.FirstStrand[i + 1] : line.Strands.Length;
            if (line.Fire)
                ShapeFlames(line.Strands, line.FirstStrand[i], end, a, b, line.Age, line.Life);
            else
                ShapeBolt(line.Strands, line.FirstStrand[i], a, b, line.Tendrils);
        }
    }

    /// <summary>A fire wall glows along its length; lightning flickers bright and dies fast.</summary>
    private static void Light(Line line)
    {
        if (line.Segments.Length == 0)
            return;
        var (lo, hi) = (line.Segments[0].A, line.Segments[0].A);
        foreach (var (a, b) in line.Segments)
        {
            lo = lo.Min(a).Min(b);
            hi = hi.Max(a).Max(b);
        }
        var centre = FxKit.Flat((lo + hi) * 0.5f) + Vector3.Up * 0.9f;
        var reach = FxKit.Flat(hi - lo).Length() * 0.5f + 1.8f;
        var fade = Mathf.Clamp(Mathf.Min(line.Age / 0.1f, (line.Life - line.Age) / 0.3f), 0f, 1f);
        if (line.Fire)
            Fx.Light(centre, reach, FxTheme.FireLight, (0.8f + 0.15f * Mathf.Sin(line.Age * 11f)) * fade);
        else
            Fx.Light(centre, reach, new Color(0.55f, 0.7f, 1f), (float)GD.RandRange(0.7, 1.2) * fade);
    }

    /// <summary>
    /// A jagged bolt from a to b: kinks pushed sideways and up at random, one thin box per piece,
    /// each wrapped in a blue sheath a fixed rim wider (a child, so it follows the box). Then
    /// <paramref name="tendrils"/> tiny forked twigs sprouting off it at random; they grow anew
    /// with every reshape, so they flicker.
    /// </summary>
    private static void ShapeBolt(MeshInstance3D[] strands, int first, Vector3 a, Vector3 b, int tendrils)
    {
        var along = b - a;
        var side = Mathf.Abs(along.Normalized().Y) > 0.95f ? Vector3.Right : along.Cross(Vector3.Up).Normalized();
        var lift = Vector3.Up * Lift;
        Span<Vector3> points = stackalloc Vector3[BoltKinks + 1];
        points[0] = a + lift;
        for (var k = 1; k <= BoltKinks; k++)
        {
            var point = a.Lerp(b, (float)k / BoltKinks) + lift;
            if (k < BoltKinks)
                point += side * (float)GD.RandRange(-0.35, 0.35) + Vector3.Up * (float)GD.RandRange(-0.2, 0.25);
            FxKit.Lay(strands[first + k - 1], points[k - 1], point, (float)GD.RandRange(0.02, 0.035));
            points[k] = point;
        }

        var forward = along.Normalized();
        for (var t = 0; t < tendrils; t++)
        {
            var piece = GD.RandRange(0, BoltKinks - 1);
            var root = points[piece].Lerp(points[piece + 1], GD.Randf());
            // Off to either side, leaning along the bolt a little and mostly drooping.
            var dir = (side * (GD.Randf() < 0.5f ? -1f : 1f) + forward * (float)GD.RandRange(-0.7, 0.7)
                + Vector3.Up * (float)GD.RandRange(-0.7, 0.3)).Normalized();
            // Mostly tiny, now and then one reaching further: cubing skews the spread short.
            var reach = 0.04f + 0.4f * Mathf.Pow(GD.Randf(), 3f);
            var kink = root + dir * reach * 0.5f + FxKit.Jitter(reach * 0.25f);
            var tip = root + dir * reach + FxKit.Jitter(reach * 0.3f);
            var width = (float)GD.RandRange(0.004, 0.008);
            var strand = first + BoltKinks + t * TendrilPieces;
            FxKit.Lay(strands[strand], root, kink, width);
            FxKit.Lay(strands[strand + 1], kink, tip, width * 0.7f);
        }
    }

    /// <summary>
    /// A ragged row of flame tongues along a–b on the ground, rising as the wall lights and
    /// sinking as it runs out. Each keeps its own height, mostly middling with the odd tall one,
    /// and stands a little off the line, so the wall is uneven and deep rather than a fence of
    /// spikes; fx_flame.gdshader does the flickering.
    /// </summary>
    private static void ShapeFlames(MeshInstance3D[] strands, int first, int end, Vector3 a, Vector3 b, float age, float life)
    {
        var (from, to) = (FxKit.Flat(a), FxKit.Flat(b));
        var across = (to - from).Cross(Vector3.Up).Normalized();
        var step = from.DistanceTo(to) / Mathf.Max(1, end - first - 1);
        var fade = Mathf.Min(1f, Mathf.Min(age / 0.2f, (life - age) / 0.3f));
        for (var k = first; k < end; k++)
        {
            var t = end - first > 1 ? (float)(k - first) / (end - first - 1) : 0.5f;
            var tall = FxKit.Hash(k * 12.9898f);
            var height = Mathf.Max(0.01f, (1f + 0.75f * tall * tall) * fade);
            var width = 1f + 0.5f * FxKit.Hash(k * 4.1414f);
            var strand = strands[k];
            var spot = from.Lerp(to, t) + (to - from).Normalized() * step * (FxKit.Hash(k * 7.13f) - 0.5f) * 0.8f
                + across * (FxKit.Hash(k * 3.7f) - 0.5f) * 0.4f;
            // The quad faces the tilted camera, so its foot dips; this keeps the foot on the ground.
            strand.Position = spot + new Vector3(0f, height * 0.3f, 0f);
            strand.Scale = new Vector3(width, height, 1f);
        }
    }

    /// <summary>Lays a fire wall's strip of burning ground under a–b.</summary>
    private static void LayBed(MeshInstance3D bed, Vector3 a, Vector3 b, float life)
    {
        var along = b - a;
        var half = along.Length() * 0.5f + FireBedRadius;
        var yaw = Mathf.Atan2(-along.Z, along.X);
        bed.Transform = new Transform3D(new Basis(Vector3.Up, yaw) * Basis.FromScale(new Vector3(half, 1f, FireBedRadius)),
            (a + b) * 0.5f + Vector3.Up * FxKit.GroundLift);
        var material = (ShaderMaterial)bed.MaterialOverride;
        material.SetShaderParameter("uExtent", new Vector2(half, FireBedRadius));
        material.SetShaderParameter("uLife", life);
        material.SetShaderParameter("uAge", 0f);
        bed.Visible = true;
    }

    private Line Make(int count, int beds, bool fire)
    {
        var strands = new MeshInstance3D[count];
        for (var i = 0; i < strands.Length; i++)
        {
            strands[i] = fire ? FxKit.Mesh(kit.Quad, _flameMaterial) : kit.BoltPiece();
            kit.Parent.AddChild(strands[i]);
        }
        var bedNodes = new MeshInstance3D[beds];
        for (var i = 0; i < beds; i++)
            bedNodes[i] = kit.Plane(FxKit.GroundMaterial(FireStyle, FxTheme.FireLight));
        var line = new Line { Strands = strands, Beds = bedNodes, Fire = fire };
        _pool.Add(line);
        return line;
    }
}
