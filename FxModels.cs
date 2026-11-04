using Godot;

namespace Arc.Fx;

/// <summary>
/// Models a few effects borrow from the game: ARC FX ships no art of its own, so a game hands
/// over its props here at startup (a fresh instance per call). Left null, each effect falls back
/// to a plain shape that reads the same from the game camera.
/// </summary>
public static class FxModels
{
    /// <summary>A shield, for spectral shields (a shield ring, a shield charge). Fallback: a slab.</summary>
    public static Func<Node3D?>? Shield { get; set; }

    /// <summary>A rock spike, any of several (picked by the game per call). Fallback: a five-sided cone.</summary>
    public static Func<Node3D?>? RockSpike { get; set; }

    /// <summary>A large hammer, for the spectral hammer that smites. Fallback: a haft and a head.</summary>
    public static Func<Node3D?>? Hammer { get; set; }

    /// <summary>An arrow, any length (it is laid along its longest side, head at its +Z end, and scaled to 0.9 m). Fallback: a stick.</summary>
    public static Func<Node3D?>? Arrow { get; set; }

    /// <summary>A great axe, for the spectral thrown axe (scaled to 1.75 m along its haft). Fallback: a bar.</summary>
    public static Func<Node3D?>? Axe { get; set; }

    internal static Node3D ShieldOrFallback() =>
        Shield?.Invoke() ?? FxKit.Wrap(new MeshInstance3D { Mesh = new BoxMesh { Size = new Vector3(1f, 1.4f, 0.12f) } });

    internal static Node3D RockSpikeOrFallback() =>
        RockSpike?.Invoke() ?? FxKit.Wrap(new MeshInstance3D
        {
            Mesh = new CylinderMesh { TopRadius = 0f, BottomRadius = 0.3f, Height = 1f, RadialSegments = 5, Rings = 1 },
            MaterialOverride = new StandardMaterial3D { AlbedoColor = new Color(0.5f, 0.45f, 0.4f) },
        });
}
