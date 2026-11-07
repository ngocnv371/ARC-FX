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

/// <summary>How an aura's sigil is drawn.</summary>
public enum FxSigilLook
{
    /// <summary>A whole disc with the emblem in the middle: best for an aura on its own.</summary>
    Disc,
    /// <summary>
    /// Stacking: every aura on one character shares a single ring of runes round an empty middle
    /// where they stand, and each adds a small badge with its emblem just outside it, at that
    /// sigil's own angle, so any set of auras sits side by side.
    /// </summary>
    Ring,
}

/// <summary>
/// Pooled aura sigils (fx_sigil). A disc is a disc of light at someone's feet, its rings and runes
/// slowly turning round the emblem of what the aura does. Ring sigils are drawn as one shared
/// ring per character (a halo) and a badge per aura round it, all turning together. Each rides
/// on a node and is renewed by calling again, as an aura is; it swells in, and fades out once
/// nothing renews it, and a halo goes with the last of its badges.
/// </summary>
internal sealed class FxSigils(FxKit kit)
{
    /// <summary>How long a sigil takes to swell in and to fade out.</summary>
    private const float In = 0.3f;
    private const float Out = 0.35f;
    /// <summary>Just above patches of ground, so a sigil shows over burning ground or frost.</summary>
    private const float Lift = FxKit.GroundLift + 0.01f;

    /// <summary>fx_sigil.gdshader's modes.</summary>
    private const int DiscMode = 0, RingMode = 1, BadgeMode = 2;
    /// <summary>A badge's radius as a share of the ring's, and how far out from the ring's edge its centre sits, in badge radii.</summary>
    private const float BadgeShare = 0.27f;
    private const float BadgeOut = 1.1f;
    /// <summary>Where the ring's outer line is, as a share of its radius (fx_sigil's ring()).</summary>
    private const float RingEdge = 0.965f;
    /// <summary>Radians a second a ring and its badges turn.</summary>
    private const float RingSpin = 0.15f;
    /// <summary>The shared ring's colour: pale, so the badges carry the colour.</summary>
    private static readonly Color RingColor = new(0.9f, 0.88f, 0.8f);

    private sealed class Mark : FxItem
    {
        public required MeshInstance3D Disc;
        public Node3D? Follow;
        public FxSigil Sigil;
        public FxSigilLook Look;
        public float Radius;
        public float Alpha;
        public Color Color;
    }

    /// <summary>The ring shared by every ring sigil on one node.</summary>
    private sealed class Halo
    {
        public required MeshInstance3D Disc;
        public Node3D? Follow;
        public float Turn;
        public float Radius;
        public bool Active;
    }

    private readonly List<Mark> _marks = [];
    private readonly List<Halo> _halos = [];
    private readonly Dictionary<(FxSigil, Color, int), ShaderMaterial> _materials = [];
    private readonly Dictionary<Node3D, (float Alpha, float Radius)> _ringed = [];

    /// <summary>
    /// Where round its ring a sigil's badge sits, in radians from the front (toward the camera).
    /// Ten slots a tenth of a turn apart, stepped three at a time so that sigils next to each other
    /// in the list (fire and frost, say) land well apart.
    /// </summary>
    public static float SlotOf(FxSigil sigil) => (int)sigil * 3 % 10 * Mathf.Tau / 10f;

    /// <summary>The sigil of this kind and look riding on <paramref name="follow"/>, kept <paramref name="keep"/> more seconds; made if there is none.</summary>
    public FxHandle Spawn(Node3D follow, FxSigil sigil, float radius, float keep, Color color, FxSigilLook look)
    {
        var mark = _marks.Find(m => m.Active && m.Follow == follow && m.Sigil == sigil && m.Look == look);
        if (mark == null)
        {
            mark = _marks.Find(m => !m.Active) ?? Make();
            mark.Begin(keep);
            mark.Follow = follow;
            mark.Sigil = sigil;
            mark.Look = look;
            mark.Color = Colors.Transparent;
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
        if (mark.Color != color)
        {
            mark.Color = color;
            mark.Disc.MaterialOverride = Material(sigil, color, look == FxSigilLook.Ring ? BadgeMode : DiscMode);
        }
        Tick(0f);
        return new FxHandle(mark);
    }

    public void Tick(float dt)
    {
        // Age every sigil, and note each node wearing ring sigils: how strongly, and how wide.
        _ringed.Clear();
        foreach (var mark in _marks)
        {
            if (!mark.Active || !Age(mark, dt))
                continue;
            if (mark.Look == FxSigilLook.Disc)
                PlaceDisc(mark);
            else
            {
                var had = _ringed.GetValueOrDefault(mark.Follow!);
                _ringed[mark.Follow!] = (Mathf.Max(had.Alpha, mark.Alpha), Mathf.Max(had.Radius, mark.Radius));
            }
        }

        // One ring under each of them, turning; a ring nobody wears any more goes.
        foreach (var halo in _halos)
        {
            if (halo.Active && (halo.Follow == null || !_ringed.ContainsKey(halo.Follow)))
            {
                halo.Active = false;
                halo.Disc.Visible = false;
                halo.Follow = null;
            }
        }
        foreach (var (follow, (alpha, radius)) in _ringed)
        {
            var halo = _halos.Find(h => h.Active && h.Follow == follow);
            if (halo == null)
            {
                halo = _halos.Find(h => !h.Active) ?? MakeHalo();
                halo.Active = true;
                halo.Follow = follow;
                halo.Turn = GD.Randf() * Mathf.Tau;
                halo.Disc.Visible = true;
            }
            halo.Turn += dt * RingSpin;
            halo.Radius = radius;
            halo.Disc.Position = (follow.Position with { Y = 0f }) + Vector3.Up * Lift;
            var size = radius * (0.85f + 0.15f * alpha);
            halo.Disc.Scale = new Vector3(size, 1f, size);
            halo.Disc.SetInstanceShaderParameter("uAlpha", alpha);
            halo.Disc.SetInstanceShaderParameter("uTurn", halo.Turn);
        }

        // Each badge round its ring, at its own angle, turning with it.
        foreach (var mark in _marks)
        {
            if (mark.Active && mark.Look == FxSigilLook.Ring)
                PlaceBadge(mark, _halos.Find(h => h.Active && h.Follow == mark.Follow)!);
        }
    }

    /// <summary>Ages a sigil and sets how strongly it shows; false once it has gone.</summary>
    private static bool Age(Mark mark, float dt)
    {
        mark.Age += dt;
        var gone = mark.Follow == null || !GodotObject.IsInstanceValid(mark.Follow);
        var fade = mark.Age > mark.Life ? 1f - (mark.Age - mark.Life) / Out : 1f;
        if (fade <= 0f || gone)
        {
            mark.Active = false;
            mark.Disc.Visible = false;
            mark.Follow = null;
            return false;
        }
        var swell = Mathf.Clamp(mark.Age / In, 0f, 1f);
        mark.Alpha = (1f - (1f - swell) * (1f - swell)) * fade;
        mark.Disc.SetInstanceShaderParameter("uAlpha", mark.Alpha);
        return true;
    }

    private static void PlaceDisc(Mark mark)
    {
        mark.Disc.Position = (mark.Follow!.Position with { Y = 0f }) + Vector3.Up * Lift;
        var size = mark.Radius * (0.8f + 0.2f * mark.Alpha);
        mark.Disc.Scale = new Vector3(size, 1f, size);
        Fx.Light(mark.Disc.Position + Vector3.Up * 0.3f, mark.Radius * 1.4f, mark.Color, 0.35f * mark.Alpha);
    }

    private static void PlaceBadge(Mark mark, Halo halo)
    {
        var badge = halo.Radius * BadgeShare;
        var reach = halo.Radius * RingEdge + badge * BadgeOut;
        // From the front, turning the same way as the ring's runes (fx_sigil turns them by uTurn).
        var angle = SlotOf(mark.Sigil) - halo.Turn;
        var way = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
        mark.Disc.Position = (mark.Follow!.Position with { Y = 0f }) + way * reach + Vector3.Up * (Lift + 0.002f);
        var size = badge * (0.6f + 0.4f * mark.Alpha);
        mark.Disc.Scale = new Vector3(size, 1f, size);
        Fx.Light(mark.Disc.Position + Vector3.Up * 0.3f, badge * 3f, mark.Color, 0.25f * mark.Alpha);
    }

    private ShaderMaterial Material(FxSigil sigil, Color color, int mode)
    {
        if (_materials.TryGetValue((sigil, color, mode), out var cached))
            return cached;
        var material = FxKit.Shader("sigil");
        material.SetShaderParameter("uStyle", (int)sigil);
        material.SetShaderParameter("uMode", mode);
        material.SetShaderParameter("uColor", FxKit.Rgb(color));
        return _materials[(sigil, color, mode)] = material;
    }

    private Mark Make()
    {
        var disc = new MeshInstance3D { Mesh = kit.Ground, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off, Visible = false };
        kit.Parent.AddChild(disc);
        var mark = new Mark { Disc = disc, Color = Colors.Transparent };
        _marks.Add(mark);
        return mark;
    }

    private Halo MakeHalo()
    {
        var disc = new MeshInstance3D
        {
            Mesh = kit.Ground,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            MaterialOverride = Material(FxSigil.Lightning, RingColor, RingMode),
            Visible = false,
        };
        kit.Parent.AddChild(disc);
        var halo = new Halo { Disc = disc };
        _halos.Add(halo);
        return halo;
    }
}
