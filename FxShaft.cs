using Godot;

namespace Arc.Fx;

/// <summary>
/// A standing column of light from the sky down onto something that must read from across the
/// screen (a red one onto an elite, a gold one round a chest): fx_beam.gdshader's look held at
/// full strength, past its fade in and never fading out. The game owns and places the node.
/// </summary>
public static class FxShaft
{
    public const float Height = 30f;

    private static readonly CylinderMesh Mesh = new() { TopRadius = 1f, BottomRadius = 1f, Height = 1f, RadialSegments = 16, Rings = 1, CapTop = false, CapBottom = false };
    public static readonly Color Gold = new(1f, 0.82f, 0.4f);
    public static readonly Color Red = new(1f, 0.28f, 0.22f);
    private static readonly Dictionary<Color, ShaderMaterial> Materials = [];

    public static MeshInstance3D Create(Color color) =>
        new() { Mesh = Mesh, MaterialOverride = Material(color), CastShadow = GeometryInstance3D.ShadowCastingSetting.Off };

    /// <summary>Stands <paramref name="shaft"/> on <paramref name="foot"/>, <paramref name="radius"/> metres round, reaching up out of view.</summary>
    public static void Place(Node3D shaft, Vector3 foot, float radius)
    {
        shaft.Position = foot + Vector3.Up * (Height / 2f);
        shaft.Scale = new Vector3(radius, Height, radius);
    }

    private static ShaderMaterial Material(Color color)
    {
        if (Materials.TryGetValue(color, out var cached))
            return cached;
        var material = new ShaderMaterial { Shader = FxShaders.Get("beam") };
        material.SetShaderParameter("uColor", new Vector3(color.R, color.G, color.B));
        material.SetShaderParameter("uAge", 1f);
        material.SetShaderParameter("uLife", 1e9f);
        return Materials[color] = material;
    }
}
