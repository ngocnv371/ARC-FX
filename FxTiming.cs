namespace Arc.Fx;

/// <summary>
/// When effects that unfold over time reach things, so a game's hits can land as the picture
/// says they do. Effects only draw; these numbers are the contract between the two.
/// </summary>
public static class FxTiming
{
    /// <summary>A nova's rings reach their full radius after this long (easing out), and are gone after <see cref="NovaLife"/>.</summary>
    public const float NovaSpread = 0.25f;
    public const float NovaLife = 0.7f;
    /// <summary>How deep the ring of lightning at a nova's edge is, in metres.</summary>
    public const float NovaBand = 1.2f;
    /// <summary>Where a nova's inner ring of sparks stops, as a share of its radius.</summary>
    public const float NovaInner = 0.55f;

    /// <summary>How far a nova's rings have come after <paramref name="age"/> seconds.</summary>
    public static float NovaFront(float age, float radius)
    {
        var spread = Math.Min(age / NovaSpread, 1f);
        return radius * (1f - (1f - spread) * (1f - spread));
    }

    /// <summary>Divine Ire: how long light gathers before the beam fires, then how long the beam lingers.</summary>
    public const float IreWindup = 0.6f;
    public const float IreLinger = 0.5f;
    /// <summary>How far ahead of the caster the beam starts.</summary>
    public const float IreLead = 0.6f;

    /// <summary>An arc's jump (<see cref="Fx.Arc"/>): how long its bolt shows, and the beat between one jump and the next.</summary>
    public const float ArcLife = 0.22f;
    public const float ArcLeap = 0.07f;

    /// <summary>A shield ring reaches its radius after this long (easing out), and is gone after <see cref="ShieldLife"/>.</summary>
    public const float ShieldSpread = 0.3f;
    public const float ShieldLife = 0.6f;

    /// <summary>How far a shield ring's shields have come after <paramref name="age"/> seconds.</summary>
    public static float ShieldRingRadius(float age, float radius)
    {
        var spread = Math.Min(1f, age / ShieldSpread);
        return 0.9f + (radius - 0.9f) * (1f - (1f - spread) * (1f - spread));
    }

    /// <summary>A smite lands this long after it starts, then lingers fading.</summary>
    public const float SmiteFall = 0.32f;
    public const float SmiteLinger = 0.45f;

    /// <summary>A meteor lands (and its blast goes off) this long after it is called.</summary>
    public const float MeteorFall = 0.9f;

    /// <summary>A rupture's spikes race along the ground at this many metres a second.</summary>
    public const float RuptureSpeed = 26f;
}
