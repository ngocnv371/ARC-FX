using Godot;

namespace Arc.Fx;

/// <summary>
/// Models a few effects borrow from the game: a game hands over its props here at startup (a
/// fresh instance per call). Left null, each effect falls back to a shape of its own that reads
/// the same from the game camera: a built prop for the shield, hammer and rock spike
/// (<see cref="FxProps"/>), a plain one for the rest.
/// </summary>
public static class FxModels
{
    /// <summary>A shield, for spectral shields (a shield ring, a shield charge). Fallback: a heater shield with a rim, a cross and a boss.</summary>
    public static Func<Node3D?>? Shield { get; set; }

    /// <summary>A rock spike, any of several (picked by the game per call). Fallback: a jagged crag of faceted rock, shards and rubble (a few looks).</summary>
    public static Func<Node3D?>? RockSpike { get; set; }

    /// <summary>A large hammer, for the spectral hammer that smites. Fallback: a war maul with a banded, studded head.</summary>
    public static Func<Node3D?>? Hammer { get; set; }

    /// <summary>An arrow, any length (it is laid along its longest side, head at its +Z end, and scaled to 0.9 m). Fallback: a stick.</summary>
    public static Func<Node3D?>? Arrow { get; set; }

    /// <summary>A great axe, for the spectral thrown axe (scaled to 1.75 m along its haft). Fallback: a bar.</summary>
    public static Func<Node3D?>? Axe { get; set; }

    internal static Node3D ShieldOrFallback() =>
        Shield?.Invoke() ?? FxKit.Wrap(new MeshInstance3D { Mesh = FxProps.Shield() });

    internal static Node3D RockSpikeOrFallback() =>
        RockSpike?.Invoke() ?? FxKit.Wrap(new MeshInstance3D
        {
            Mesh = FxProps.RockSpike(GD.RandRange(0, FxProps.RockSpikeLooks - 1)),
            // Flat facets lit by fx_crystal's fixed key light, so the jagged cut reads.
            MaterialOverride = FxKit.Crystal(new Color(0.44f, 0.38f, 0.32f), new Color(0.7f, 0.6f, 0.48f), rim: 0.25f, glow: 0f),
        });
}
