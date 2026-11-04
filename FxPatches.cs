using Godot;

namespace Arc.Fx;

/// <summary>What a patch of ground looks like. The values are fx_ground.gdshader's styles.</summary>
public enum FxGround
{
    /// <summary>A soft glow in the patch's colour.</summary>
    Glow = 0,
    /// <summary>Burning ground: flickering embers over a scorch.</summary>
    Fire = 1,
    /// <summary>A bubbling pool.</summary>
    Poison = 2,
    /// <summary>Hoarfrost.</summary>
    Frost = 3,
    /// <summary>A blow about to land: a filling red disc.</summary>
    Warning = 8,
    /// <summary>A ring of flame under a slowly turning bubble of fire.</summary>
    Inferno = 9,
    /// <summary>A poison cloud: a bubbling pool under a low dome of drifting green vapour.</summary>
    Miasma = 10,
}

/// <summary>
/// A handle on a patch of ground that is still showing. Lets the game move it, keep it longer or
/// end it; does nothing once the patch has gone, even if its slot was reused.
/// </summary>
public readonly struct FxPatch
{
    private readonly FxPatches.Patch? _patch;
    private readonly int _generation;

    internal FxPatch(FxPatches.Patch patch)
    {
        _patch = patch;
        _generation = patch.Generation;
    }

    private FxPatches.Patch? Live => _patch is { Active: true } p && p.Generation == _generation ? p : null;

    /// <summary>Still on screen.</summary>
    public bool Alive => Live != null;

    /// <summary>Its centre on the ground (only X and Z count). Ignored while it follows a node.</summary>
    public Vector3 Position
    {
        get => Live?.Position ?? Vector3.Zero;
        set
        {
            if (Live is { } p)
                p.Position = value with { Y = 0f };
        }
    }

    public float Radius
    {
        get => Live?.Radius ?? 0f;
        set
        {
            if (Live is { } p)
                p.Radius = value;
        }
    }

    /// <summary>Shows for <paramref name="seconds"/> more from now.</summary>
    public void Keep(float seconds)
    {
        if (Live is { } p)
            p.Life = p.Age + seconds;
    }

    /// <summary>Fades out now, over the shader's own fade.</summary>
    public void Stop() => Keep(0.3f);
}

/// <summary>
/// Pooled patches of ground (fx_ground): burning ground, poison pools, hoarfrost, glows and warnings,
/// which may ride on a node (an aura). An Inferno patch also raises a bubble of fire (fx_inferno),
/// a Miasma patch a low dome of poison vapour (fx_miasma) over a poison pool.
/// Each lights what is around it while it shows.
/// </summary>
internal sealed class FxPatches(Node3D parent)
{
    private const float GroundLift = 0.05f;
    /// <summary>An Inferno bubble's height as a share of its radius, and how long it takes to swell in or die away.</summary>
    private const float BubbleHeight = 0.6f;
    private const float BubbleFade = 0.35f;
    /// <summary>A Miasma dome sits lower and rolls in and thins away more slowly.</summary>
    private const float MiasmaHeight = 0.45f;
    private const float MiasmaFade = 0.6f;

    internal sealed class Patch
    {
        public required MeshInstance3D Ground;
        public required ShaderMaterial GroundMaterial;
        public MeshInstance3D? Bubble;
        public ShaderMaterial? BubbleMaterial;
        public Vector3 Position;
        public Node3D? Follow;
        public float Radius;
        public float Age;
        public float Life;
        public Color Color;
        public FxGround Look;
        public int Generation;
        public bool Active;
    }

    private readonly List<Patch> _pool = [];
    private readonly PlaneMesh _groundMesh = new() { Size = new Vector2(2f, 2f) };
    private readonly SphereMesh _globeMesh = new() { Radius = 1f, Height = 2f, RadialSegments = 32, Rings = 16 };

    public static FxGround GroundOf(FxElement element) => element switch
    {
        FxElement.Fire => FxGround.Fire,
        FxElement.Frost => FxGround.Frost,
        FxElement.Poison => FxGround.Poison,
        _ => FxGround.Glow,
    };

    public FxPatch Spawn(Vector3 at, float radius, float life, Color color, FxGround look) =>
        new(Take(at, radius, life, color, look));

    /// <summary>The patch riding on <paramref name="follow"/> with this look, kept <paramref name="keep"/> more seconds; made if there is none.</summary>
    public FxPatch Aura(Node3D follow, float radius, float keep, Color color, FxGround look)
    {
        var patch = _pool.Find(p => p.Active && p.Follow == follow && p.Look == look) ?? Take(follow.Position, radius, keep, color, look);
        if (HasBubble(look))
            ShowBubble(patch, color);
        patch.Follow = follow;
        patch.Position = follow.Position with { Y = 0f };
        patch.Radius = radius;
        patch.Life = patch.Age + keep;
        Draw(patch);
        return new FxPatch(patch);
    }

    private Patch Take(Vector3 at, float radius, float life, Color color, FxGround look)
    {
        var patch = _pool.Find(p => !p.Active) ?? Make();
        patch.Active = true;
        patch.Generation++;
        patch.Age = 0f;
        patch.Follow = null;
        patch.Position = at with { Y = 0f };
        patch.Radius = radius;
        patch.Life = life;
        patch.Look = look;
        patch.Color = color;
        // A Miasma's ground is the poison pool; the vapour is its dome.
        patch.GroundMaterial.SetShaderParameter("uStyle", (int)(look == FxGround.Miasma ? FxGround.Poison : look));
        patch.GroundMaterial.SetShaderParameter("uColor", FxWorld.Rgb(color));
        patch.Ground.Visible = true;
        if (HasBubble(look))
            ShowBubble(patch, color);
        else if (patch.Bubble != null)
            patch.Bubble.Visible = false;
        Draw(patch);
        return patch;
    }

    public void Tick(float dt)
    {
        foreach (var patch in _pool)
        {
            if (!patch.Active)
                continue;
            patch.Age += dt;
            if (patch.Age >= patch.Life)
            {
                patch.Active = false;
                patch.Follow = null;
                patch.Ground.Visible = false;
                if (patch.Bubble != null)
                    patch.Bubble.Visible = false;
                continue;
            }
            if (patch.Follow != null)
            {
                if (GodotObject.IsInstanceValid(patch.Follow))
                    patch.Position = patch.Follow.Position with { Y = 0f };
                else
                    patch.Follow = null;
            }
            Draw(patch);
            Light(patch);
        }
    }

    private Patch Make()
    {
        // Drawn before every other effect: its soot darkens whatever is already there, and a
        // flame sorted behind it would show its quad's edges as dark steps.
        var material = new ShaderMaterial { Shader = FxShaders.Get("ground"), RenderPriority = -1 };
        var ground = new MeshInstance3D
        {
            Mesh = _groundMesh,
            MaterialOverride = material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            Visible = false,
        };
        parent.AddChild(ground);
        var patch = new Patch { Ground = ground, GroundMaterial = material };
        _pool.Add(patch);
        return patch;
    }

    private static bool HasBubble(FxGround look) => look is FxGround.Inferno or FxGround.Miasma;

    private void ShowBubble(Patch patch, Color color)
    {
        if (patch.Bubble == null)
        {
            patch.BubbleMaterial = new ShaderMaterial();
            patch.Bubble = new MeshInstance3D
            {
                Mesh = _globeMesh,
                MaterialOverride = patch.BubbleMaterial,
                CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            };
            parent.AddChild(patch.Bubble);
        }
        // A pooled patch can be fire one time and poison the next.
        patch.BubbleMaterial!.Shader = FxShaders.Get(patch.Look == FxGround.Miasma ? "miasma" : "inferno");
        patch.BubbleMaterial.SetShaderParameter("uColor", FxWorld.Rgb(color));
        patch.Bubble.Visible = true;
    }

    private static void Draw(Patch patch)
    {
        // The shader fades the ground in and out and animates it; this places it.
        // An Inferno ring is padded, so its outer edge lands on the radius, under the bubble's base.
        var extent = patch.Look == FxGround.Inferno ? patch.Radius * FxExplosions.BurstPad : patch.Radius;
        patch.Ground.Position = patch.Position + Vector3.Up * GroundLift;
        patch.Ground.Scale = new Vector3(extent, 1f, extent);
        patch.GroundMaterial.SetShaderParameter("uExtent", new Vector2(extent, extent));
        patch.GroundMaterial.SetShaderParameter("uAge", patch.Age);
        patch.GroundMaterial.SetShaderParameter("uLife", patch.Life);
        if (patch.Bubble is { Visible: true } bubble)
        {
            // Centred on the ground, so only its upper half shows: a dome over the hero.
            var (height, fade) = patch.Look == FxGround.Miasma ? (MiasmaHeight, MiasmaFade) : (BubbleHeight, BubbleFade);
            bubble.Position = patch.Position;
            bubble.Scale = new Vector3(patch.Radius, patch.Radius * height, patch.Radius);
            patch.BubbleMaterial!.SetShaderParameter("uBurn", Mathf.Clamp(Mathf.Min(patch.Age, patch.Life - patch.Age) / fade, 0f, 1f));
        }
    }

    /// <summary>Burning ground flickers; the rest glow softly.</summary>
    private static void Light(Patch patch)
    {
        var fade = Mathf.Clamp(Mathf.Min(patch.Age / 0.15f, (patch.Life - patch.Age) / 0.5f), 0f, 1f);
        var (reach, strength) = patch.Look switch
        {
            FxGround.Fire => (1.7f, 0.8f),
            FxGround.Inferno => (1.9f, 0.9f),
            FxGround.Warning => (1.2f, 0.3f),
            // Pale light on pale ground (snow) clips to white, so cold and poison stay faint.
            FxGround.Poison or FxGround.Frost => (1.3f, 0.15f),
            FxGround.Miasma => (1.4f, 0.2f),
            _ => (1.4f, 0.35f),
        };
        if (patch.Look is FxGround.Fire or FxGround.Inferno)
            strength *= 0.85f + 0.15f * Mathf.Sin(patch.Age * 13f + patch.Position.X * 3f) * Mathf.Sin(patch.Age * 7.3f);
        Fx.Light(patch.Position + Vector3.Up * 0.8f, patch.Radius * reach + 0.5f, patch.Color, strength * fade);
    }
}
