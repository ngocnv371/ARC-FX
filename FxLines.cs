using Godot;

namespace Arc.Fx;

/// <summary>
/// Lightning along segments: jittering strands of a white core in a blue sheath, with optional
/// tendrils, reshaped every 0.06 s. Drawn <see cref="Lift"/> metres above each end, so a line along the ground stands at chest height and one from the sky down to a
/// spot on the ground is a strike.
/// </summary>
internal sealed class FxLines(FxKit kit)
{
    /// <summary>How far above each end a line is drawn.</summary>
    public const float Lift = 0.9f;
    /// <summary>A bolt bends about once every this many metres, at least BoltKinks and at most MaxKinks times, so a long one (a strike from the sky) is as jagged as a short one rather than a few long rods.</summary>
    private const int BoltKinks = 4, MaxKinks = 12;
    private const float KinkSpacing = 1.5f;
    /// <summary>Each decorative tendril off a bolt is a twig of this many pieces.</summary>
    private const int TendrilPieces = 2;
    private const float Reshape = 0.06f;

    private sealed class Line : FxItem
    {
        public required MeshInstance3D[] Strands;
        public int Tendrils;
        public float Thickness = 1f;
        public (Vector3 A, Vector3 B)[] Segments = [];
        public int[] FirstStrand = [];
        public float NextShape;
    }

    private readonly List<Line> _pool = [];

    public FxHandle Spawn((Vector3 A, Vector3 B)[] segments, float life, int tendrils, float thickness = 1f)
    {
        var first = new int[segments.Length];
        var count = 0;
        for (var i = 0; i < segments.Length; i++)
        {
            first[i] = count;
            count += KinksOf(segments[i].A, segments[i].B) + tendrils * TendrilPieces;
        }
        var line = _pool.Find(l => !l.Active && l.Strands.Length == count) ?? Make(count);
        line.Begin(life);
        line.Segments = segments;
        line.FirstStrand = first;
        line.Tendrils = tendrils;
        line.Thickness = Mathf.Max(0.05f, thickness);
        line.NextShape = 0f;
        foreach (var strand in line.Strands)
            strand.Visible = true;
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
            return;
        }
        Light(line);
        if (line.Age < line.NextShape)
            return;
        line.NextShape += Reshape;
        for (var i = 0; i < line.Segments.Length; i++)
        {
            var (a, b) = line.Segments[i];
            ShapeBolt(line.Strands, line.FirstStrand[i], a, b, line.Tendrils, line.Thickness);
        }
    }

    /// <summary>Lightning flickers bright and dies fast.</summary>
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
        Fx.Light(centre, reach, new Color(0.55f, 0.7f, 1f), (float)GD.RandRange(0.7, 1.2) * fade);
    }

    /// <summary>
    /// A jagged bolt from a to b: kinks pushed sideways and up at random, one thin box per piece,
    /// each wrapped in a blue sheath a fixed rim wider (a child, so it follows the box). Then
    /// <paramref name="tendrils"/> tiny forked twigs sprouting off it at random; they grow anew
    /// with every reshape, so they flicker. Their core and sheath are about half the bolt's, so they read as finer
    /// offshoots of it. <paramref name="thickness"/> scales the bolt and twigs, sheaths and all.
    /// </summary>
    private static int KinksOf(Vector3 a, Vector3 b) =>
        Math.Clamp(Mathf.CeilToInt(a.DistanceTo(b) / KinkSpacing), BoltKinks, MaxKinks);

    private static void ShapeBolt(MeshInstance3D[] strands, int first, Vector3 a, Vector3 b, int tendrils, float thickness)
    {
        var along = b - a;
        var side = Mathf.Abs(along.Normalized().Y) > 0.95f ? Vector3.Right : along.Cross(Vector3.Up).Normalized();
        var lift = Vector3.Up * Lift;
        var kinks = KinksOf(a, b);
        Span<Vector3> points = stackalloc Vector3[MaxKinks + 1];
        points[0] = a + lift;
        for (var k = 1; k <= kinks; k++)
        {
            var point = a.Lerp(b, (float)k / kinks) + lift;
            if (k < kinks)
                point += side * (float)GD.RandRange(-0.35, 0.35) + Vector3.Up * (float)GD.RandRange(-0.2, 0.25);
            FxKit.Lay(strands[first + k - 1], points[k - 1], point, (float)GD.RandRange(0.02, 0.035) * thickness, FxKit.SheathRim * thickness);
            points[k] = point;
        }

        var forward = along.Normalized();
        for (var t = 0; t < tendrils; t++)
        {
            var piece = GD.RandRange(0, kinks - 1);
            var root = points[piece].Lerp(points[piece + 1], GD.Randf());
            // Off to either side, leaning along the bolt a little and mostly drooping.
            var dir = (side * (GD.Randf() < 0.5f ? -1f : 1f) + forward * (float)GD.RandRange(-0.7, 0.7)
                + Vector3.Up * (float)GD.RandRange(-0.7, 0.3)).Normalized();
            // Mostly short, now and then one reaching further: squaring skews the spread short.
            // A long bolt's pieces are long, and its twigs grow with them, up to 2.5 times.
            var grow = Mathf.Clamp(points[piece].DistanceTo(points[piece + 1]) / 1.5f, 1f, 2.5f);
            var reach = (0.08f + 0.45f * Mathf.Pow(GD.Randf(), 2f)) * grow;
            var kink = root + dir * reach * 0.5f + FxKit.Jitter(reach * 0.25f);
            var tip = root + dir * reach + FxKit.Jitter(reach * 0.3f);
            var width = (float)GD.RandRange(0.006, 0.012) * thickness;
            var rim = FxKit.SheathRim * 0.6f * thickness;
            var strand = first + kinks + t * TendrilPieces;
            FxKit.Lay(strands[strand], root, kink, width, rim);
            FxKit.Lay(strands[strand + 1], kink, tip, width * 0.7f, rim * 0.7f);
        }
    }

    private Line Make(int count)
    {
        var strands = new MeshInstance3D[count];
        for (var i = 0; i < strands.Length; i++)
        {
            strands[i] = kit.BoltPiece();
            kit.Parent.AddChild(strands[i]);
        }
        var line = new Line { Strands = strands };
        _pool.Add(line);
        return line;
    }
}
