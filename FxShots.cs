using Godot;

namespace Arc.Fx;

/// <summary>What a projectile looks like: an arrow (plain or elemental), a thrown axe, or a spell.</summary>
public enum FxShotLook { Arrow, FireBolt, Fireball, IceSpear, Spark, Axe, LightningArrow, PoisonArrow, PhantomArrow, FireArrow, IceArrow, ColdWave, Frostbolt, Tornado, SteelWave, StormTornado, FireTornado, IceTornado }

/// <summary>
/// A projectile's look, placed by the game every frame along its flight. ARC FX turns it to face
/// its velocity (an axe spins flat instead, a tornado stands upright with its debris whirling),
/// lights the ground under a spell (a storm tornado also calls down bolts that strike it), and shows the parts it has for being stuck in the ground.
/// Release it when the flight is over; the handle goes inert once released.
/// </summary>
public readonly struct FxShot
{
    private readonly FxHandle _handle;

    internal FxShot(FxHandle handle) => _handle = handle;

    private FxShots.Shot? Live => _handle.Live as FxShots.Shot;

    public bool Alive => _handle.Alive;

    /// <summary>
    /// Puts it at <paramref name="position"/> heading along <paramref name="velocity"/>, drawn at
    /// <paramref name="size"/> (and lighting that far round a spell) times <paramref name="grow"/>
    /// (for one swelling in or dying away). A <paramref name="lit"/> spell lights the ground under it.
    /// </summary>
    public void Place(Vector3 position, Vector3 velocity, float size = 1f, float grow = 1f, bool lit = true)
    {
        if (Live is { } shot)
            FxShots.Place(shot, position, velocity, size, grow, lit);
    }

    /// <summary>Shows or hides it (a shot waiting its turn to drop is hidden).</summary>
    public void Show(bool visible)
    {
        if (Live is { } shot)
            shot.Node.Visible = visible;
    }

    /// <summary>Swaps the parts shown in flight for those shown once stuck in the ground, or back.</summary>
    public void Stick(bool stuck)
    {
        if (Live is { } shot)
            FxShots.ShowStuck(shot, stuck);
    }

    /// <summary>For a fading shot or a wave: how much of its flight is left, 1 at launch to 0 at the end (it dims, or widens).</summary>
    public void Strength(float left) => Live?.Fading?.SetInstanceShaderParameter("uStrength", left);

    /// <summary>Hides it and gives its node back to the pool.</summary>
    public void Release() => _handle.Stop();
}

/// <summary>
/// Projectile looks, pooled by look. Arrows are the arrow model (<see cref="FxModels.Arrow"/>)
/// with what an elemental arrow carries (fx_missile flame, rime, lightning or venom, an ice-crusted
/// head, or the arrow itself as phantasmal light); the axe is a spectral great axe
/// (<see cref="FxModels.Axe"/>, fx_spectral); spells are fire bolts and fireballs, sparks
/// (fx_spark), ice spears (fx_shard), frostbolts (fx_orb), cold and steel waves (fx_wave,
/// fx_steel_wave) and tornados (fx_tornado with whirling debris: dust, a storm struck by
/// lightning again and again, fire or ice).
/// </summary>
internal sealed class FxShots(FxKit kit)
{
    /// <summary>How high above the ground shots fly; looks that touch the ground (waves, tornados) reach down this far.</summary>
    public const float Height = 1.1f;
    private const float AxeSpin = 18f; // radians per second
    private const float AxeLength = 1.75f;
    private static readonly Color SpectralAxe = new(0.6f, 0.82f, 1f);
    /// <summary>How far ahead of its centre the 0.9 m arrow's point is.</summary>
    private const float ArrowTip = 0.4f;
    private const float SparkSize = 1.3f;
    private static readonly Color FireColor = new(1f, 0.36f, 0.05f);

    /// <summary>A wave's half-width at launch and at the end of its range; fx_wave.gdshader draws the same.</summary>
    private const float WaveStart = 0.8f;
    private const float WaveEnd = 1.8f;
    /// <summary>The same for a steel wave, which is wider (fx_steel_wave.gdshader's uStart and uEnd).</summary>
    private const float SteelStart = 1.3f;
    private const float SteelEnd = 2.4f;
    /// <summary>A wave's quad, across and along the flight, in metres, and how far its front is ahead of the node (fx_wave.gdshader's uSize and uLead).</summary>
    private static readonly Vector2 WaveQuad = new(4.2f, 4.6f);
    /// <summary>A steel wave's quad (fx_steel_wave.gdshader's uSize), wide enough for its horns.</summary>
    private static readonly Vector2 SteelQuad = new(5.6f, 4.6f);
    private const float WaveLead = 0.5f;

    /// <summary>A tornado's funnel: its height, and how wide it is at the foot and the top.</summary>
    private const float FunnelHeight = 3.6f;
    private const float FunnelFoot = 0.25f;
    private const float FunnelTop = 1.3f;

    /// <summary>Names of a shot's parts shown only in flight, or only once stuck in the ground.</summary>
    private const string InFlight = "InFlight";
    private const string Stuck = "Stuck";
    /// <summary>The name of a shot's part that dims or widens with its flight (uStrength).</summary>
    private const string FadingPart = "Fading";
    /// <summary>The name of a tornado's rings of debris, spun as it flies.</summary>
    private const string SpinPart = "Spin";
    /// <summary>The names of a tornado's funnel layers, which a storm tornado's strikes light up (uFlash).</summary>
    private const string FunnelPart = "Funnel";
    /// <summary>How long between bolts striking a storm tornado, shortest and longest, and how fast its flash dies (per second).</summary>
    private const float StrikeMin = 0.25f, StrikeMax = 0.7f, FlashFade = 6f;
    private static readonly Color StormGlow = new(0.6f, 0.72f, 1f);

    internal sealed class Shot : FxItem
    {
        public required Node3D Node;
        public required FxShotLook Look;
        public GeometryInstance3D? Fading;
        public Node3D? Spin;
        public GeometryInstance3D[] Funnels = [];
        public Vector3 Position;
        public float NextStrike;
        public float Flash;
        public float Size = 1f;
        public bool Lit;

        public override void Stop()
        {
            Active = false;
            Node.Visible = false;
        }
    }

    private readonly List<Shot> _pool = [];
    private readonly Dictionary<FxShotLook, Node3D> _prototypes = [];

    /// <summary>A wave's half-width (how wide it hits) with <paramref name="left"/> of its flight still to go.</summary>
    public static float WaveHalfWidth(FxShotLook look, float left) => look == FxShotLook.SteelWave
        ? Mathf.Lerp(SteelStart, SteelEnd, 1f - left)
        : Mathf.Lerp(WaveStart, WaveEnd, 1f - left);

    public FxShot Spawn(FxShotLook look)
    {
        var shot = _pool.Find(s => !s.Active && s.Look == look) ?? Make(look);
        shot.Begin(float.MaxValue);
        shot.Lit = false;
        shot.NextStrike = 0.15f;
        shot.Flash = 0f;
        shot.Node.Visible = true;
        ShowStuck(shot, false);
        return new FxShot(new FxHandle(shot));
    }

    public void Tick(float dt)
    {
        foreach (var shot in _pool)
        {
            if (!shot.Active)
                continue;
            shot.Age += dt;
            if (shot.Look == FxShotLook.StormTornado && shot.Node.Visible)
                Storm(shot, dt);
            if (shot.Lit && shot.Node.Visible)
                Shine(shot);
        }
    }

    public static bool IsTornado(FxShotLook look) =>
        look is FxShotLook.Tornado or FxShotLook.StormTornado or FxShotLook.FireTornado or FxShotLook.IceTornado;

    /// <summary>
    /// A storm tornado: every so often a bolt comes down from the sky into the top of its funnel
    /// and on down through its core to the ground, and the funnel flashes as it does.
    /// </summary>
    private static void Storm(Shot shot, float dt)
    {
        shot.Flash = Mathf.Max(shot.Flash - dt * FlashFade, 0f);
        if (shot.Age >= shot.NextStrike)
        {
            shot.NextStrike = shot.Age + (float)GD.RandRange(StrikeMin, StrikeMax);
            shot.Flash = 1f;
            var scale = shot.Node.Scale.Y;
            var foot = shot.Node.Position + Vector3.Down * Height * scale;
            Vector3 Off(float reach) => Vector3.Forward.Rotated(Vector3.Up, GD.Randf() * Mathf.Tau) * reach * GD.Randf();
            var top = foot + Vector3.Up * FunnelHeight * 0.85f * scale + Off(FunnelTop * 0.5f * scale);
            var mid = foot + Vector3.Up * FunnelHeight * 0.45f * scale + Off(FunnelFoot * scale);
            var sky = top + Vector3.Up * 9f + Off(1.5f);
            // Lines are drawn LineLift above their ends: this keeps the bolt where it is meant to be.
            var lift = Vector3.Up * Fx.LineLift;
            Fx.Lightning([(sky - lift, top - lift), (top - lift, mid - lift), (mid - lift, foot + Vector3.Up * 0.1f - lift)], 0.12f, 1);
        }
        foreach (var funnel in shot.Funnels)
            funnel.SetInstanceShaderParameter("uFlash", shot.Flash);
    }

    /// <summary>Frees the prototypes: they are never in the tree, so the tree will not free them.</summary>
    public void Release()
    {
        foreach (var prototype in _prototypes.Values)
            prototype.Free();
        _prototypes.Clear();
    }

    internal static void Place(Shot shot, Vector3 position, Vector3 velocity, float size, float grow, bool lit)
    {
        shot.Position = position;
        shot.Size = size;
        shot.Lit = lit;
        shot.Node.Position = position;
        // A thrown axe spins flat, whichever way it flies.
        if (shot.Look == FxShotLook.Axe)
            shot.Node.Rotation = new Vector3(0f, -shot.Age * AxeSpin, 0f);
        else if (IsTornado(shot.Look))
        {
            shot.Node.Rotation = Vector3.Zero;
            if (shot.Spin != null)
                for (var i = 0; i < shot.Spin.GetChildCount(); i++)
                    shot.Spin.GetChild<Node3D>(i).Rotation = new Vector3(0f, shot.Age * (7f - 1.5f * i), 0f);
        }
        else if (velocity.LengthSquared() > 1e-8f)
            shot.Node.LookAt(position + velocity, Mathf.Abs(velocity.Normalized().Y) > 0.99f ? Vector3.Right : Vector3.Up);
        shot.Node.Scale = Vector3.One * Mathf.Max(size * grow, 0.01f);
    }

    /// <summary>Spells light the ground they fly over; plain arrows and axes do not.</summary>
    private static void Shine(Shot shot)
    {
        var (color, reach, strength) = shot.Look switch
        {
            FxShotLook.Fireball => (new Color(1f, 0.5f, 0.15f), 4.5f, 0.8f),
            FxShotLook.FireBolt or FxShotLook.FireArrow => (new Color(1f, 0.5f, 0.15f), 3.2f, 0.6f),
            FxShotLook.Spark or FxShotLook.LightningArrow => (new Color(0.55f, 0.7f, 1f), 3f, 0.55f),
            FxShotLook.IceSpear or FxShotLook.IceArrow or FxShotLook.Frostbolt or FxShotLook.ColdWave => (new Color(0.5f, 0.8f, 1f), 3f, 0.2f),
            FxShotLook.PoisonArrow => (new Color(0.5f, 1f, 0.3f), 2.5f, 0.3f),
            FxShotLook.PhantomArrow => (new Color(0.6f, 0.75f, 1f), 2.5f, 0.3f),
            FxShotLook.FireTornado => (new Color(1f, 0.5f, 0.15f), 5f, 0.9f),
            FxShotLook.IceTornado => (new Color(0.5f, 0.8f, 1f), 3.5f, 0.25f),
            FxShotLook.StormTornado => (StormGlow, 5f, 0.15f + shot.Flash * 1.6f),
            _ => (Colors.Black, 0f, 0f),
        };
        if (strength > 0f)
            Fx.Light(shot.Position, reach * shot.Size, color, strength);
    }

    internal static void ShowStuck(Shot shot, bool stuck)
    {
        if (shot.Node.GetNodeOrNull<Node3D>(InFlight) is { } flight)
            flight.Visible = !stuck;
        if (shot.Node.GetNodeOrNull<Node3D>(Stuck) is { } still)
            still.Visible = stuck;
    }

    private Shot Make(FxShotLook look)
    {
        if (!_prototypes.TryGetValue(look, out var prototype))
            _prototypes[look] = prototype = look switch
            {
                FxShotLook.Arrow or FxShotLook.LightningArrow or FxShotLook.PoisonArrow or FxShotLook.PhantomArrow
                    or FxShotLook.FireArrow or FxShotLook.IceArrow => BuildArrow(look),
                FxShotLook.Axe => BuildAxe(),
                _ => BuildSpell(look),
            };
        var node = (Node3D)prototype.Duplicate();
        kit.Parent.AddChild(node);
        var shot = new Shot
        {
            Node = node, Look = look, Fading = node.GetNodeOrNull<GeometryInstance3D>(FadingPart), Spin = node.GetNodeOrNull<Node3D>(SpinPart),
            Funnels = node.FindChildren(FunnelPart + "*", nameof(MeshInstance3D), recursive: false, owned: false).Cast<GeometryInstance3D>().ToArray(),
        };
        _pool.Add(shot);
        return shot;
    }

    /// <summary>
    /// A spectral copy of a great axe lying flat, its haft's middle on the origin, 1.75 m long:
    /// see-through steel-blue light (fx_spectral.gdshader), its edges glowing.
    /// </summary>
    private static Node3D BuildAxe()
    {
        var holder = new Node3D { Name = "Axe" };
        if (FxModels.Axe?.Invoke() is not { } model)
        {
            holder.AddChild(new MeshInstance3D { Mesh = new BoxMesh { Size = new Vector3(0.1f, 0.1f, 1.3f) } });
            return Spectral(holder);
        }
        var bounds = FxKit.Bounds(model);
        var size = bounds.Size;
        // Haft (the longest side) along X, the blade's breadth along Z and its thin side up.
        var order = new[] { 0, 1, 2 }.OrderByDescending(i => size[i]).ToArray();
        var columns = new Vector3[3];
        columns[order[0]] = Vector3.Right;
        columns[order[1]] = Vector3.Back;
        columns[order[2]] = Vector3.Up;
        var basis = new Basis(columns[0], columns[1], columns[2]);
        if (basis.Determinant() < 0f)
            columns[order[1]] = Vector3.Forward;
        basis = new Basis(columns[0], columns[1], columns[2]);
        // Drawn a little larger than an axe in hand, so the see-through copy reads in flight.
        var orient = new Node3D { Basis = basis.Scaled(Vector3.One * (AxeLength / Mathf.Max(size[order[0]], 1e-4f))) };
        model.Position = -bounds.GetCenter();
        orient.AddChild(model);
        holder.AddChild(orient);
        return Spectral(holder);
    }

    /// <summary>Draws every mesh under <paramref name="holder"/> as spectral light.</summary>
    private static Node3D Spectral(Node3D holder)
    {
        var material = FxKit.Spectral(SpectralAxe);
        material.SetShaderParameter("uGain", 2.2f);
        FxKit.Paint(holder, material);
        return holder;
    }

    /// <summary>
    /// The arrow model, turned so its shaft runs along -Z (Godot's forward) and scaled to 0.9 m,
    /// with what an elemental arrow carries: flame pouring off its head, an ice-crusted head
    /// trailing rime, lightning crawling over it, venom streaming off it, or (phantasmal) the
    /// arrow itself as green light trailing wisps.
    /// </summary>
    private static Node3D BuildArrow(FxShotLook look)
    {
        var holder = BuildArrowModel();
        switch (look)
        {
            case FxShotLook.FireArrow:
                // The flame's head sits on the arrowhead and pours back over the shaft.
                var flame = Missile(style: 0, FireColor, length: 1.7f, width: 0.6f, head: 0.1f, radius: 0.09f);
                flame.Position = Vector3.Forward * ArrowTip;
                holder.AddChild(flame);
                break;
            case FxShotLook.IceArrow:
                // An ice-crusted head trailing rime.
                var tip = new Node3D { Position = Vector3.Forward * (ArrowTip - 0.1f) };
                AddShard(tip, 0.2f, 0.08f, 0.045f, new Color(0.12f, 0.38f, 0.75f), new Color(0.7f, 0.92f, 1f), Colors.White,
                    new Color(0.6f, 0.85f, 1f));
                holder.AddChild(tip);
                holder.AddChild(Missile(style: 1, new Color(0.55f, 0.85f, 1f), length: 2.4f, width: 0.4f, head: 0.2f, gap: -0.3f));
                break;
            case FxShotLook.LightningArrow:
                var arcs = Missile(style: 2, new Color(0.45f, 0.4f, 1f), length: 1.8f, width: 0.7f, head: 0.3f, radius: 0.45f);
                arcs.Name = InFlight;
                holder.AddChild(arcs);
                // Stuck in the ground (a post), it crackles in a clump instead.
                holder.AddChild(new MeshInstance3D
                {
                    Name = Stuck,
                    Mesh = new QuadMesh { Size = Vector2.One * 1.1f },
                    CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
                    MaterialOverride = FxKit.Shader("spark"),
                    Visible = false,
                });
                break;
            case FxShotLook.PoisonArrow:
                holder.AddChild(Missile(style: 4, new Color(0.6f, 0.9f, 0.15f), length: 2.4f, width: 0.6f, head: 0.22f, radius: 0.45f));
                break;
            case FxShotLook.PhantomArrow:
                var ghost = FxKit.Shader("ghost");
                foreach (var mesh in holder.FindChildren("*", nameof(MeshInstance3D), owned: false).Cast<MeshInstance3D>())
                    mesh.MaterialOverride = ghost;
                holder.AddChild(Missile(style: 3, new Color(0.25f, 1f, 0.5f), length: 2.8f, width: 0.7f, head: 0.2f, radius: 0.45f));
                break;
        }
        return holder;
    }

    private static Node3D BuildArrowModel()
    {
        var holder = new Node3D { Name = "Arrow" };
        if (FxModels.Arrow?.Invoke() is not { } model)
        {
            // A pale shaft with a dark head, enough to read from the game camera.
            var wood = new StandardMaterial3D { AlbedoColor = new Color(0.85f, 0.75f, 0.55f), ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded };
            var iron = new StandardMaterial3D { AlbedoColor = new Color(0.3f, 0.32f, 0.36f), ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded };
            holder.AddChild(new MeshInstance3D { Mesh = new BoxMesh { Size = new Vector3(0.05f, 0.05f, 0.75f) }, MaterialOverride = wood, Position = Vector3.Back * 0.05f });
            holder.AddChild(new MeshInstance3D
            {
                Mesh = new CylinderMesh { TopRadius = 0f, BottomRadius = 0.06f, Height = 0.15f, RadialSegments = 4, Rings = 1 },
                MaterialOverride = iron,
                Rotation = new Vector3(-Mathf.Pi / 2f, 0f, 0f),
                Position = Vector3.Forward * 0.37f,
            });
            return holder;
        }

        var bounds = FxKit.Bounds(model);
        var size = bounds.Size;
        var axis = size.X >= size.Y && size.X >= size.Z ? Vector3.Axis.X : size.Y >= size.Z ? Vector3.Axis.Y : Vector3.Axis.Z;
        var length = size[(int)axis];
        var basis = axis switch
        {
            Vector3.Axis.X => new Basis(Vector3.Up, Mathf.Pi / 2f),
            Vector3.Axis.Y => new Basis(Vector3.Right, -Mathf.Pi / 2f),
            _ => Basis.Identity,
        };
        var scale = 0.9f / Mathf.Max(length, 1e-4f);
        // The model's head is at its +Z end once laid along Z; a half turn puts it in front.
        basis = new Basis(Vector3.Up, Mathf.Pi) * basis;
        var orient = new Node3D { Basis = basis.Scaled(Vector3.One * scale) };
        model.Position = -bounds.GetCenter();
        orient.AddChild(model);
        holder.AddChild(orient);
        return holder;
    }

    /// <summary>
    /// A spell: a fire bolt or fireball (a hot head trailing flame and embers), a spark's clump
    /// of lightning, or an ice shard with its point along -Z trailing frost. The fire, frost and
    /// lightning are camera-facing quads the fx shaders draw; the shard is a mesh. A cold wave is
    /// a crescent of frost on a flat quad skimming the ground (fx_wave; a steel wave is a wider
    /// crescent of polished steel, fx_steel_wave); a frostbolt a glassy orb of swirling cold
    /// (fx_orb) trailing frost.
    /// </summary>
    private static Node3D BuildSpell(FxShotLook look)
    {
        var holder = new Node3D { Name = look.ToString() };
        switch (look)
        {
            case FxShotLook.Spark:
                // A clump of crackling lightning; fx_spark.gdshader turns it to the camera and draws it.
                holder.AddChild(new MeshInstance3D
                {
                    Mesh = new QuadMesh { Size = Vector2.One * SparkSize },
                    CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
                    MaterialOverride = FxKit.Shader("spark"),
                });
                break;
            case FxShotLook.ColdWave or FxShotLook.SteelWave:
                // Flat on the ground below the flight height, its crescent's front just ahead of the node.
                var steel = look == FxShotLook.SteelWave;
                var quad = steel ? SteelQuad : WaveQuad;
                holder.AddChild(new MeshInstance3D
                {
                    Name = FadingPart,
                    Mesh = new QuadMesh { Size = quad },
                    MaterialOverride = FxKit.Shader(steel ? "steel_wave" : "wave"),
                    CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
                    Rotation = new Vector3(-Mathf.Pi / 2f, 0f, 0f),
                    Position = new Vector3(0f, 0.35f - Height, quad.Y * 0.5f - WaveLead),
                });
                break;
            case FxShotLook.Frostbolt:
                holder.AddChild(new MeshInstance3D
                {
                    Name = FadingPart,
                    Mesh = new QuadMesh { Size = Vector2.One * 1.5f },
                    CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
                    MaterialOverride = FxKit.Shader("orb"),
                });
                holder.AddChild(Missile(style: 1, new Color(0.55f, 0.85f, 1f), length: 2.2f, width: 0.8f, head: 0.1f, gap: 0.35f));
                break;
            case FxShotLook.Tornado or FxShotLook.StormTornado or FxShotLook.FireTornado or FxShotLook.IceTornado:
                BuildTornado(holder, look);
                break;
            case FxShotLook.Fireball:
                holder.AddChild(Missile(style: 0, FireColor, length: 3f, width: 1.8f, head: 0.15f, radius: 0.32f));
                break;
            case FxShotLook.IceSpear:
                AddShard(holder, 0.85f, 0.28f, 0.11f, new Color(0.12f, 0.38f, 0.75f), new Color(0.7f, 0.92f, 1f), Colors.White,
                    new Color(0.6f, 0.85f, 1f));
                holder.AddChild(Missile(style: 1, new Color(0.55f, 0.85f, 1f), length: 2.6f, width: 0.45f, head: 0.12f, gap: 0.45f));
                break;
            default:
                // A charred bolt, its edges smouldering, inside the flame.
                holder.AddChild(Missile(style: 0, FireColor, length: 1.8f, width: 0.7f, head: 0.1f, radius: 0.12f));
                AddShard(holder, 0.42f, 0.14f, 0.045f, new Color(0.02f, 0.015f, 0.015f), new Color(0.1f, 0.05f, 0.04f), new Color(0.5f, 0.12f, 0.02f),
                    new Color(1f, 0.5f, 0.1f));
                break;
        }
        return holder;
    }

    /// <summary>
    /// A tornado standing on the ground under the node: a funnel of dust and wind (fx_tornado.gdshader)
    /// round a denser, darker core, over a swirl of dust on the ground, with rings of debris
    /// (clods, stones and splinters) whirling round it, the low ones fastest. A storm tornado is
    /// slate cloud with lightning in its core and dark stones; a fire tornado flame round a
    /// white-hot core, whirling embers; an ice tornado frost and snow, whirling chunks of ice.
    /// </summary>
    private static void BuildTornado(Node3D holder, FxShotLook look)
    {
        var shader = FxShaders.Get("tornado");
        // fx_tornado's style, its dust and dirt (outer funnel and core), what glows, and the debris.
        var (style, dust, dirt, glow, colors) = look switch
        {
            FxShotLook.StormTornado => (1, new Color(0.4f, 0.44f, 0.54f), new Color(0.13f, 0.14f, 0.2f), StormGlow,
                new[] { new Color(0.3f, 0.3f, 0.34f), new Color(0.45f, 0.46f, 0.5f), new Color(0.22f, 0.22f, 0.26f), new Color(0.38f, 0.36f, 0.34f) }),
            FxShotLook.FireTornado => (2, new Color(0.3f, 0.16f, 0.09f), new Color(0.12f, 0.07f, 0.05f), FireColor,
                new[] { new Color(1f, 0.55f, 0.15f), new Color(1f, 0.8f, 0.35f), new Color(0.9f, 0.3f, 0.05f), new Color(0.2f, 0.1f, 0.06f) }),
            FxShotLook.IceTornado => (3, new Color(0.8f, 0.9f, 0.98f), new Color(0.32f, 0.52f, 0.78f), new Color(0.5f, 0.8f, 1f),
                new[] { new Color(0.75f, 0.9f, 1f), new Color(0.55f, 0.78f, 0.98f), new Color(0.92f, 0.97f, 1f), new Color(0.45f, 0.68f, 0.92f) }),
            _ => (0, new Color(0.72f, 0.64f, 0.52f), new Color(0.34f, 0.27f, 0.19f), Colors.White,
                new[] { new Color(0.42f, 0.31f, 0.2f), new Color(0.58f, 0.55f, 0.5f), new Color(0.62f, 0.45f, 0.26f), new Color(0.35f, 0.42f, 0.2f) }),
        };
        ShaderMaterial Layer(int layer)
        {
            // The ground swirl first, then the core, then the outer funnel over both.
            var material = new ShaderMaterial { Shader = shader, RenderPriority = layer == 2 ? -1 : layer == 1 ? 0 : 1 };
            material.SetShaderParameter("uLayer", layer);
            material.SetShaderParameter("uHeight", FunnelHeight);
            material.SetShaderParameter("uStyle", style);
            material.SetShaderParameter("uDust", FxKit.Rgb(dust));
            material.SetShaderParameter("uDirt", FxKit.Rgb(dirt));
            material.SetShaderParameter("uGlow", FxKit.Rgb(glow));
            return material;
        }
        const float foot = -Height;
        holder.AddChild(new MeshInstance3D
        {
            Mesh = new QuadMesh { Size = Vector2.One * 3.4f },
            MaterialOverride = Layer(2),
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            Rotation = new Vector3(-Mathf.Pi / 2f, 0f, 0f),
            Position = new Vector3(0f, foot + 0.05f, 0f),
        });
        foreach (var (layer, widen) in new[] { (1, 0.55f), (0, 1f) })
            holder.AddChild(new MeshInstance3D
            {
                Name = FunnelPart + layer,
                Mesh = new CylinderMesh
                {
                    TopRadius = FunnelTop * widen, BottomRadius = FunnelFoot * widen, Height = FunnelHeight,
                    RadialSegments = 32, Rings = 16, CapTop = false, CapBottom = false,
                },
                MaterialOverride = Layer(layer),
                CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
                Position = new Vector3(0f, foot + FunnelHeight * 0.5f, 0f),
                // The shader bends it about; keep it drawn when the bend takes it off its bounds.
                ExtraCullMargin = 1f,
            });

        var spin = new Node3D { Name = SpinPart };
        holder.AddChild(spin);
        var rng = new RandomNumberGenerator { Seed = 7 };
        for (var r = 0; r < 3; r++)
        {
            var ring = new Node3D();
            spin.AddChild(ring);
            var height = 0.35f + r * 1.05f;
            var radius = Mathf.Lerp(FunnelFoot, FunnelTop, height / FunnelHeight) + 0.25f;
            const int pieces = 4;
            for (var i = 0; i < pieces; i++)
            {
                var angle = Mathf.Tau * (i + rng.Randf() * 0.5f) / pieces;
                var size = rng.RandfRange(0.1f, 0.2f);
                ring.AddChild(new MeshInstance3D
                {
                    Mesh = new BoxMesh { Size = new Vector3(size, size * rng.RandfRange(0.4f, 0.9f), size * rng.RandfRange(0.6f, 1.6f)) },
                    MaterialOverride = new StandardMaterial3D { AlbedoColor = colors[(r + i) % colors.Length], ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded },
                    CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
                    Position = new Vector3(Mathf.Cos(angle) * radius, foot + height + rng.RandfRange(-0.25f, 0.25f), Mathf.Sin(angle) * radius),
                    Rotation = new Vector3(rng.Randf() * Mathf.Tau, rng.Randf() * Mathf.Tau, rng.Randf() * Mathf.Tau),
                });
            }
        }
    }

    /// <summary>A quad fx_missile.gdshader stretches along the flight, its head on the node.</summary>
    private static MeshInstance3D Missile(int style, Color color, float length, float width, float head, float gap = 0f, float radius = 0.1f)
    {
        var material = FxKit.Shader("missile");
        material.SetShaderParameter("uStyle", style);
        material.SetShaderParameter("uColor", FxKit.Rgb(color));
        material.SetShaderParameter("uLength", length);
        material.SetShaderParameter("uWidth", width);
        material.SetShaderParameter("uHead", head);
        material.SetShaderParameter("uGap", gap);
        material.SetShaderParameter("uRadius", radius);
        return new MeshInstance3D
        {
            Mesh = new QuadMesh { Size = Vector2.One },
            MaterialOverride = material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            // The shader moves the corners well off the unit quad the bounds are taken from.
            ExtraCullMargin = length,
        };
    }

    /// <summary>
    /// A shard drawn by fx_shard.gdshader: a long five-sided point ahead and a short one behind,
    /// base to base. Drawn after the light around it, so it stays solid in the middle of it.
    /// </summary>
    private static void AddShard(Node3D holder, float front, float back, float radius, Color deep, Color pale, Color rim, Color glint)
    {
        var material = new ShaderMaterial { Shader = FxShaders.Get("shard"), RenderPriority = 1 };
        material.SetShaderParameter("uDeep", FxKit.Rgb(deep));
        material.SetShaderParameter("uPale", FxKit.Rgb(pale));
        material.SetShaderParameter("uRim", FxKit.Rgb(rim));
        material.SetShaderParameter("uGlint", FxKit.Rgb(glint));
        // The cones stand along +Y with their points up; lay the front one along -Z, the back one along +Z.
        holder.AddChild(new MeshInstance3D
        {
            Mesh = new CylinderMesh { TopRadius = 0f, BottomRadius = radius, Height = front, RadialSegments = 5, Rings = 0 },
            MaterialOverride = material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            Rotation = new Vector3(-Mathf.Pi / 2f, 0f, 0f),
        });
        holder.AddChild(new MeshInstance3D
        {
            Mesh = new CylinderMesh { TopRadius = 0f, BottomRadius = radius, Height = back, RadialSegments = 5, Rings = 0 },
            MaterialOverride = material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            Rotation = new Vector3(Mathf.Pi / 2f, 0f, 0f),
            Position = new Vector3(0f, 0f, (front + back) * 0.5f),
        });
    }
}
