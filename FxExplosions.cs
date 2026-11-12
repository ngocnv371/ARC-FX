using Godot;

namespace Arc.Fx;

/// <summary>An explosion's shape.</summary>
public enum FxBurst
{
    /// <summary>A churning dome that burns away over a shockwave and its afterglow.</summary>
    Dome,
    /// <summary>A globe of flame swelling over a ring of fire that rolls out with it, leaving a scorch.</summary>
    Fire,
    /// <summary>A crystal globe swelling over a ring of ice that rolls out with it, then shattering, leaving hoarfrost.</summary>
    Frost,
    /// <summary>A globe of crackling lightning that pops as a small nova's rings land.</summary>
    Lightning,
}

/// <summary>
/// Pooled explosions: a ground plane (fx_ground: shockwave, ring of fire or ice, or nova) under
/// a dome (fx_blast) or a globe (fx_fireball, fx_iceball, fx_sparkball) that swells and pops.
/// </summary>
internal sealed class FxExplosions(Node3D parent)
{
    /// <summary>How long a dome churns, then how long its shockwave's afterglow lingers.</summary>
    public const float DomeTime = 0.3f;
    public const float AfterglowTime = 0.6f;
    /// <summary>A fire or frost blast's ring and what it leaves on the ground, until that has faded.</summary>
    public const float BurstLife = 1.8f;
    /// <summary>How long a lightning blast's nova lasts, and how long its rings take to reach the radius.</summary>
    public const float NovaLife = 0.7f;
    public const float NovaSpread = 0.25f;
    /// <summary>Where a nova's inner ring of sparks stops, as a share of the radius.</summary>
    private const float NovaInner = 0.55f;

    /// <summary>How far a burst's ground reaches past its radius, for flame tongues and crystals (BLAST_PAD in fx_ground).</summary>
    public const float BurstPad = 1.3f;
    private const float GroundLift = 0.05f;

    /// <summary>fx_ground.gdshader's styles for explosions.</summary>
    private const int ShockStyle = 4, NovaStyle = 5, FireStyle = 6, FrostStyle = 7;

    /// <summary>How long a fire or frost globe swells and pops (riding its ring out), and when in that it starts to pop.</summary>
    private const float GlobeTime = 0.45f, GlobePop = 0.6f;
    /// <summary>
    /// A lightning globe is quicker: it rides the nova's outer ring out (see <see cref="FxTiming.NovaFront"/>)
    /// and pops as the rings land.
    /// </summary>
    private const float SparkTime = 0.32f, SparkPop = 0.75f;
    /// <summary>A globe is raised by this share of its size, so it meets the ground a little inside its size.</summary>
    private const float GlobeLift = 0.3f;

    private sealed class Blast
    {
        public required MeshInstance3D Ground;
        public required ShaderMaterial GroundMaterial;
        public required MeshInstance3D Dome;
        public required ShaderMaterial DomeMaterial;
        /// <summary>A burst's globe material for the dome node, remade when the shape changes.</summary>
        public ShaderMaterial? GlobeMaterial;
        public Vector3 Position;
        public float Radius;
        public float Age;
        public float Life;
        public Color Color;
        public FxBurst Burst;
        public bool Active;
    }

    private readonly List<Blast> _pool = [];
    private readonly PlaneMesh _groundMesh = new() { Size = new Vector2(2f, 2f) };
    // Fine enough for fx_blast's billows to bend its outline.
    private readonly SphereMesh _domeMesh = new() { Radius = 1f, Height = 1f, RadialSegments = 48, Rings = 24 };
    private readonly SphereMesh _globeMesh = new() { Radius = 1f, Height = 2f, RadialSegments = 32, Rings = 16 };
    // Few faces, so the shader's flat shading makes it read as cut crystal.
    private readonly SphereMesh _crystalMesh = new() { Radius = 1f, Height = 2f, RadialSegments = 12, Rings = 6 };

    public static FxBurst BurstOf(FxElement element) => element switch
    {
        FxElement.Fire => FxBurst.Fire,
        FxElement.Frost => FxBurst.Frost,
        FxElement.Lightning => FxBurst.Lightning,
        _ => FxBurst.Dome,
    };

    /// <summary>How long an explosion of this shape is on screen.</summary>
    public static float LifeOf(FxBurst burst) => burst switch
    {
        FxBurst.Dome => DomeTime + AfterglowTime,
        FxBurst.Lightning => NovaLife,
        _ => BurstLife,
    };

    public void Spawn(Vector3 at, float radius, Color color, FxBurst burst)
    {
        var blast = _pool.Find(b => !b.Active) ?? Make();
        blast.Active = true;
        blast.Age = 0f;
        blast.Position = at with { Y = 0f };
        blast.Radius = radius;
        blast.Color = color;
        blast.Burst = burst;
        blast.Life = LifeOf(burst);

        var ground = blast.GroundMaterial;
        ground.SetShaderParameter("uColor", FxWorld.Rgb(color));
        blast.DomeMaterial.SetShaderParameter("uColor", FxWorld.Rgb(color));
        switch (burst)
        {
            case FxBurst.Dome:
                ground.SetShaderParameter("uStyle", ShockStyle);
                break;
            case FxBurst.Lightning:
                // The ground is a small nova: sparks inside, crackling lightning round the edge.
                ground.SetShaderParameter("uStyle", NovaStyle);
                ground.SetShaderParameter("uSpread", NovaSpread);
                ground.SetShaderParameter("uRings", new Vector3(NovaInner / BurstPad, 1f / BurstPad, 0.3f / BurstPad));
                break;
            default:
                ground.SetShaderParameter("uStyle", burst == FxBurst.Frost ? FrostStyle : FireStyle);
                break;
        }

        if (burst != FxBurst.Dome)
        {
            var shader = FxShaders.Get(burst switch { FxBurst.Lightning => "sparkball", FxBurst.Frost => "iceball", _ => "fireball" });
            if (blast.GlobeMaterial?.Shader != shader)
                blast.GlobeMaterial = new ShaderMaterial { Shader = shader };
            blast.GlobeMaterial.SetShaderParameter("uColor", FxWorld.Rgb(color));
        }
        // A burst's dome node draws the globe instead.
        blast.Dome.Mesh = burst switch { FxBurst.Dome => _domeMesh, FxBurst.Frost => _crystalMesh, _ => _globeMesh };
        blast.Dome.MaterialOverride = burst == FxBurst.Dome ? blast.DomeMaterial : blast.GlobeMaterial;
        blast.Ground.Visible = true;
        blast.Dome.Visible = true;
        Draw(blast);
    }

    public void Tick(float dt)
    {
        foreach (var blast in _pool)
        {
            if (!blast.Active)
                continue;
            blast.Age += dt;
            if (blast.Age >= blast.Life)
            {
                blast.Active = false;
                blast.Ground.Visible = false;
                blast.Dome.Visible = false;
                continue;
            }
            Draw(blast);
            Light(blast);
        }
    }

    private Blast Make()
    {
        // Drawn before every other effect: its soot darkens whatever is already there, and a
        // flame sorted behind it would show its quad's edges as dark steps.
        var groundMaterial = new ShaderMaterial { Shader = FxShaders.Get("ground"), RenderPriority = -1 };
        var ground = new MeshInstance3D
        {
            Mesh = _groundMesh,
            MaterialOverride = groundMaterial,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            Visible = false,
        };
        var domeMaterial = new ShaderMaterial { Shader = FxShaders.Get("blast") };
        var dome = new MeshInstance3D
        {
            Mesh = _domeMesh,
            MaterialOverride = domeMaterial,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            Visible = false,
        };
        parent.AddChild(ground);
        parent.AddChild(dome);
        var blast = new Blast { Ground = ground, GroundMaterial = groundMaterial, Dome = dome, DomeMaterial = domeMaterial };
        _pool.Add(blast);
        return blast;
    }

    private void Draw(Blast blast)
    {
        // The shader fades the ground in and out and animates it; this places it.
        var extent = blast.Burst != FxBurst.Dome ? blast.Radius * BurstPad : blast.Radius;
        blast.Ground.Position = blast.Position + Vector3.Up * GroundLift;
        blast.Ground.Scale = new Vector3(extent, 1f, extent);
        blast.GroundMaterial.SetShaderParameter("uExtent", new Vector2(extent, extent));
        blast.GroundMaterial.SetShaderParameter("uAge", blast.Age);
        blast.GroundMaterial.SetShaderParameter("uLife", blast.Life);

        if (blast.Burst != FxBurst.Dome && blast.GlobeMaterial != null)
        {
            PlaceGlobe(blast.Dome, blast.GlobeMaterial, blast.Position, blast.Radius, blast.Age, blast.Burst);
            return;
        }

        var u = blast.Age / DomeTime;
        blast.Dome.Visible = u < 1f;
        if (u >= 1f)
            return;
        var grow = 0.4f + 0.6f * (1f - (1f - u) * (1f - u));
        blast.Dome.Position = blast.Position;
        blast.Dome.Scale = new Vector3(blast.Radius * grow, blast.Radius * grow * 0.8f, blast.Radius * grow);
        blast.DomeMaterial.SetShaderParameter("uProgress", u);
    }

    /// <summary>
    /// A globe swells fast and slows, then gives a last puff outward as it pops. Shared with
    /// effects that pop a globe of their own (a nova's, as <see cref="FxBurst.Lightning"/>).
    /// </summary>
    public static void PlaceGlobe(MeshInstance3D node, ShaderMaterial material, Vector3 centre, float radius, float age, FxBurst shape)
    {
        var spark = shape == FxBurst.Lightning;
        var (time, pop) = spark ? (SparkTime, SparkPop) : (GlobeTime, GlobePop);
        var u = age / time;
        node.Visible = u < 1f;
        if (u >= 1f)
            return;
        var puff = Mathf.Max(u - pop, 0f) / (1f - pop) * 0.2f;
        var swell = 1f - Mathf.Pow(1f - Mathf.Min(u / pop, 1f), 3f);
        // Meets the ground where its ring's front is (the nova's outer ring, or the fire or ice
        // ring, on fx_ground's blast_front, the same ease as the swell), so the two read as one
        // burst.
        var front = spark ? FxTiming.NovaFront(age, radius) : radius * swell;
        var footprint = Mathf.Max(front, radius * 0.15f);
        var size = footprint / Mathf.Sqrt(1f - GlobeLift * GlobeLift) * (1f + puff);
        // Sunk a little into the ground, as if it burst out of it.
        node.Position = centre + Vector3.Up * size * GlobeLift;
        node.Scale = Vector3.One * size;
        material.SetShaderParameter("uProgress", u);
    }

    /// <summary>A blast flashes as it goes off and dies down with its ring.</summary>
    private static void Light(Blast blast)
    {
        var fade = Mathf.Clamp(Mathf.Min(blast.Age / 0.15f, (blast.Life - blast.Age) / 0.5f), 0f, 1f);
        var color = blast.Burst == FxBurst.Fire ? FxTheme.FireLight : blast.Color;
        Fx.Light(blast.Position + Vector3.Up * 0.8f, blast.Radius * 2.2f + 1f, color, 1.5f * Mathf.Exp(-blast.Age * 3.5f) * fade);
    }
}
