using Godot;

namespace Arc.Fx;

/// <summary>How a ram looks: a spectral shield held out in front, or a ball tumbling over (with dust and a tail of wind).</summary>
public enum FxRam { Shield, Stone }

/// <summary>
/// Something running a line through the crowd. A shield charge holds a spectral shield
/// (fx_spectral) out in front at chest height, tipped back so the camera sees its face. A
/// rolling stone throws up dust behind it and trails a few pale strands of wind off the path it
/// has just rolled, which drain away once it stops. Each follows a position the game gives
/// (a function, called every frame) until that gives null or its time is up.
/// </summary>
internal sealed class FxRams(FxKit kit, FxWorld world)
{
    private const float DustEvery = 0.07f;
    private const float Fade = 0.08f;
    /// <summary>How far the shield's face is tipped up (rise over run): far enough to show its face even running away from the camera.</summary>
    private const float ShieldTip = 1.5f;
    /// <summary>How far ahead of the one charging the shield is held, in metres at size 1.</summary>
    private const float ShieldReach = 1.35f;
    public static readonly Color ShieldColor = new(1f, 0.8f, 0.42f);
    /// <summary>A stone's wind tail: how long a point of it lasts, how many strands twist round its path, their colour.</summary>
    private const float TailTime = 0.22f;
    private const int TailStrands = 3;
    private const int TailSamples = 28; // along each strand, so it curves smoothly whatever the frame rate
    private const float TailTwist = 4.5f; // radians a strand turns round the path from the ball to its end
    private static readonly Color TailColor = new(0.85f, 0.95f, 1f);

    private sealed class Ram : FxItem
    {
        public required Func<Vector3?> At;
        public Node3D? Shield;
        public GeometryInstance3D[] Parts = [];
        public FxRam Look;
        public Vector3 Direction;
        public float Size;
        public float NextDust;
        /// <summary>A stone's wind tail: where the ball has been lately (on the ground) and how long ago.</summary>
        public MeshInstance3D? Tail;
        public readonly List<(Vector3 At, float Age)> Trail = [];
        /// <summary>Still under way; once over, it stays active only while its tail fades.</summary>
        public bool Running;
    }

    private readonly List<Ram> _pool = [];
    private StandardMaterial3D? _tailMaterial;

    public FxHandle Spawn(FxRam look, Func<Vector3?> at, Vector3 direction, float life, float size)
    {
        var ram = _pool.Find(r => !r.Active && r.Look == look);
        if (ram == null)
        {
            _pool.Add(ram = new Ram { At = at, Look = look });
            if (look == FxRam.Shield)
                MakeShield(ram);
            else
                MakeTail(ram);
        }
        ram.Begin(life);
        ram.Running = true;
        ram.Trail.Clear();
        ram.At = at;
        ram.Direction = FxKit.Flat(direction).LengthSquared() > 1e-4f ? FxKit.Flat(direction).Normalized() : Vector3.Forward;
        ram.Size = size;
        ram.NextDust = 0f;
        if (ram.Shield != null)
            ram.Shield.Visible = true;
        Tick(ram, 0f);
        return new FxHandle(ram);
    }

    public void Tick(float dt)
    {
        foreach (var ram in _pool)
        {
            if (ram.Active)
                Tick(ram, dt);
        }
    }

    private void Tick(Ram ram, float dt)
    {
        ram.Age += dt;
        var at = ram.Running && ram.Age <= ram.Life ? ram.At() : null;
        if (at == null && ram.Running)
        {
            ram.Running = false;
            if (ram.Shield != null)
                ram.Shield.Visible = false;
        }
        if (ram.Tail != null)
            DrawTail(ram, dt, at);
        if (at is not { } now)
        {
            ram.Active = ram.Trail.Count > 0;
            return;
        }
        now = FxKit.Flat(now);

        if (ram.Shield != null)
        {
            // Held well out in front at chest height (clear of the body even tipped back), its face
            // forward and tipped back so the camera sees it. The model's face is on its +Z side, so it
            // is turned round after LookAt aims -Z.
            var shield = ram.Shield;
            shield.Position = now + ram.Direction * ShieldReach * ram.Size + Vector3.Up * 1.1f * ram.Size;
            shield.LookAt(shield.Position + ram.Direction + Vector3.Up * ShieldTip, Vector3.Up);
            shield.RotateObjectLocal(Vector3.Up, Mathf.Pi);
            shield.Scale = Vector3.One * ram.Size;
            var alpha = Mathf.Min(1f, ram.Age / Fade) * Mathf.Min(1f, (ram.Life - ram.Age) / Fade);
            foreach (var part in ram.Parts)
                part.SetInstanceShaderParameter("uAlpha", alpha);
        }
        else if ((ram.NextDust -= dt) <= 0f)
        {
            // A trail of dust thrown up behind the tumbling ball.
            ram.NextDust = DustEvery;
            world.Patch(now - ram.Direction * 0.4f * ram.Size, 0.55f * ram.Size, 0.45f, FxSlams.DustColor);
        }
    }

    /// <summary>
    /// The stone's tail of wind: a few pale strands streaming off the path the ball has just
    /// rolled, waving from side to side, brightest at the ball, thinning and spreading out as they trail off.
    /// Once the ball stops (<paramref name="at"/> null) the tail drains away behind it.
    /// </summary>
    private void DrawTail(Ram ram, float dt, Vector3? at)
    {
        var trail = ram.Trail;
        for (var i = 0; i < trail.Count; i++)
            trail[i] = (trail[i].At, trail[i].Age + dt);
        trail.RemoveAll(p => p.Age >= TailTime);
        if (at is { } point)
            trail.Insert(0, (FxKit.Flat(point), 0f));
        var mesh = (ImmediateMesh)ram.Tail!.Mesh;
        mesh.ClearSurfaces();
        ram.Tail.Visible = trail.Count > 1;
        if (trail.Count < 2)
            return;

        var eye = kit.Parent.GetViewport()?.GetCamera3D()?.GlobalPosition ?? Vector3.Up * 20f;
        var middle = Vector3.Up * 0.55f * ram.Size;
        var side = Vector3.Up.Cross(ram.Direction);
        var length = trail[^1].Age;
        for (var strand = 0; strand < TailStrands; strand++)
        {
            var phase = Mathf.Tau * strand / TailStrands + ram.Age * 6f;
            mesh.SurfaceBegin(Mesh.PrimitiveType.TriangleStrip, _tailMaterial);
            for (var j = 0; j <= TailSamples; j++)
            {
                var age = length * j / TailSamples;
                var k = age / TailTime;
                // Each strand at its own height, waving from side to side and opening out as it trails off
                // (a sideways wave, not a coil, which the high camera would see as jagged cusps).
                var height = Mathf.Lerp(-0.45f, 0.45f, TailStrands > 1 ? strand / (TailStrands - 1f) : 0.5f) * ram.Size;
                var wave = side * Mathf.Sin(phase - k * TailTwist) * ram.Size * Mathf.Lerp(0.15f, 0.6f, k);
                var spot = TrailAt(trail, age) + middle + Vector3.Up * height + wave;
                // Flat across the roll rather than the strand, so the ribbon never twists edge-on into a kink.
                var across = ram.Direction.Cross(eye - spot);
                across = across.LengthSquared() > 1e-6f ? across.Normalized() : side;
                // Thin at the ball, fullest a little behind it, tapering to nothing at the end.
                var width = 0.11f * ram.Size * Mathf.Sin(Mathf.Pi * Mathf.Pow(Mathf.Min(k, 1f), 0.6f));
                var alpha = 0.8f * (1f - k) * Mathf.Min(1f, j / 3f);
                mesh.SurfaceSetColor(TailColor with { A = alpha });
                mesh.SurfaceAddVertex(spot - across * width);
                mesh.SurfaceAddVertex(spot + across * width);
            }
            mesh.SurfaceEnd();
        }
    }

    /// <summary>Where the ball was <paramref name="age"/> seconds ago, between the trail's points (newest first).</summary>
    private static Vector3 TrailAt(List<(Vector3 At, float Age)> trail, float age)
    {
        for (var i = 1; i < trail.Count; i++)
        {
            if (trail[i].Age >= age)
            {
                var span = trail[i].Age - trail[i - 1].Age;
                return trail[i - 1].At.Lerp(trail[i].At, span > 1e-6f ? (age - trail[i - 1].Age) / span : 0f);
            }
        }
        return trail[^1].At;
    }

    private void MakeTail(Ram ram)
    {
        _tailMaterial ??= new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            BlendMode = BaseMaterial3D.BlendModeEnum.Add,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
            VertexColorUseAsAlbedo = true,
            NoDepthTest = false,
        };
        ram.Tail = new MeshInstance3D { Mesh = new ImmediateMesh(), CastShadow = GeometryInstance3D.ShadowCastingSetting.Off, Visible = false };
        kit.Parent.AddChild(ram.Tail);
    }

    private void MakeShield(Ram ram)
    {
        var shield = new Node3D { Visible = false };
        shield.AddChild(FxKit.Facing(FxModels.ShieldOrFallback(), 1.5f));
        ram.Parts = FxKit.Paint(shield, FxKit.Spectral(ShieldColor));
        kit.Parent.AddChild(shield);
        ram.Shield = shield;
    }
}
