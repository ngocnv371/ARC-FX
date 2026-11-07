using Godot;

namespace Arc.Fx;

/// <summary>What an aura does, shown as the emblem in the middle of its sigil. The values are fx_sigil.gdshader's styles.</summary>
public enum FxSigil
{
    /// <summary>Lightning made stronger: a flickering bolt.</summary>
    Lightning = 0,
    /// <summary>Fire made stronger: three licking flames.</summary>
    Fire = 1,
    /// <summary>Frost made stronger: a slowly turning snowflake.</summary>
    Frost = 2,
    /// <summary>Poison made stronger: a drop of venom with bubbles rising past it.</summary>
    Poison = 3,
    /// <summary>Life (more of it, or regeneration): a beating cross.</summary>
    Life = 4,
    /// <summary>Might (more damage): crossed swords with a gleam running along them.</summary>
    Might = 5,
    /// <summary>Guard (armour, less damage taken): a kite shield with a sheen sweeping across.</summary>
    Guard = 6,
    /// <summary>Haste (speed): three chevrons lit one after another.</summary>
    Haste = 7,
    /// <summary>Holy, or a blessing: a sun in a turning ring of rays.</summary>
    Holy = 8,
    /// <summary>Arcane (mana, spell power): a four-pointed star round a smaller one, a mote circling.</summary>
    Arcane = 9,
}

/// <summary>
/// Pooled aura sigils (fx_sigil): a disc of light at someone's feet, its rings and runes slowly
/// turning round the emblem of what the aura does. Each rides on a node and is renewed by
/// calling again, as an aura is; it swells in, and fades out once nothing renews it.
/// </summary>
internal sealed class FxSigils(FxKit kit)
{
    /// <summary>How long a sigil takes to swell in and to fade out.</summary>
    private const float In = 0.3f;
    private const float Out = 0.35f;
    /// <summary>Just above patches of ground, so a sigil shows over burning ground or frost.</summary>
    private const float Lift = FxKit.GroundLift + 0.01f;

    private sealed class Mark : FxItem
    {
        public required MeshInstance3D Disc;
        public Node3D? Follow;
        public FxSigil Sigil;
        public float Radius;
        public Color Color;
    }

    private readonly List<Mark> _marks = [];
    private readonly Dictionary<(FxSigil, Color), ShaderMaterial> _materials = [];

    /// <summary>The sigil of this kind riding on <paramref name="follow"/>, kept <paramref name="keep"/> more seconds; made if there is none.</summary>
    public FxHandle Spawn(Node3D follow, FxSigil sigil, float radius, float keep, Color color)
    {
        var mark = _marks.Find(m => m.Active && m.Follow == follow && m.Sigil == sigil);
        if (mark == null)
        {
            mark = _marks.Find(m => !m.Active) ?? Make();
            mark.Begin(keep);
            mark.Follow = follow;
            mark.Sigil = sigil;
            mark.Disc.SetInstanceShaderParameter("uPhase", GD.Randf());
            mark.Disc.Visible = true;
        }
        else if (mark.Age > mark.Life)
        {
            // Renewed while fading out: come back from where the fade had got to.
            var left = 1f - (mark.Age - mark.Life) / Out;
            mark.Age = In * left;
        }
        mark.Life = mark.Age + keep;
        mark.Radius = radius;
        if (mark.Color != color || mark.Disc.MaterialOverride == null)
        {
            mark.Color = color;
            mark.Disc.MaterialOverride = Material(sigil, color);
        }
        Tick(mark, 0f);
        return new FxHandle(mark);
    }

    public void Tick(float dt)
    {
        foreach (var mark in _marks)
        {
            if (mark.Active)
                Tick(mark, dt);
        }
    }

    private void Tick(Mark mark, float dt)
    {
        mark.Age += dt;
        var gone = mark.Follow == null || !GodotObject.IsInstanceValid(mark.Follow);
        var fade = mark.Age > mark.Life ? 1f - (mark.Age - mark.Life) / Out : 1f;
        if (fade <= 0f || gone)
        {
            mark.Active = false;
            mark.Disc.Visible = false;
            mark.Follow = null;
            return;
        }
        var swell = Mathf.Clamp(mark.Age / In, 0f, 1f);
        var ease = 1f - (1f - swell) * (1f - swell);
        mark.Disc.Position = (mark.Follow!.Position with { Y = 0f }) + Vector3.Up * Lift;
        var size = mark.Radius * (0.8f + 0.2f * ease) * (0.95f + 0.05f * fade);
        mark.Disc.Scale = new Vector3(size, 1f, size);
        mark.Disc.SetInstanceShaderParameter("uAlpha", ease * fade);
        Fx.Light(mark.Disc.Position + Vector3.Up * 0.3f, mark.Radius * 1.4f, mark.Color, 0.35f * ease * fade);
    }

    private ShaderMaterial Material(FxSigil sigil, Color color)
    {
        if (_materials.TryGetValue((sigil, color), out var cached))
            return cached;
        var material = FxKit.Shader("sigil");
        material.SetShaderParameter("uStyle", (int)sigil);
        material.SetShaderParameter("uColor", FxKit.Rgb(color));
        return _materials[(sigil, color)] = material;
    }

    private Mark Make()
    {
        var disc = new MeshInstance3D { Mesh = kit.Ground, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off, Visible = false };
        kit.Parent.AddChild(disc);
        var mark = new Mark { Disc = disc, Color = Colors.Transparent };
        _marks.Add(mark);
        return mark;
    }
}
