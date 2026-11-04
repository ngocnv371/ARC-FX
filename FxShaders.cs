using Godot;

namespace Arc.Fx;

/// <summary>
/// Where ARC FX's shaders live. Every load goes through here, so the folder can be mounted
/// anywhere by changing <see cref="Root"/> alone (shaders include each other by relative path).
/// </summary>
public static class FxShaders
{
    public const string Root = "res://addons/arc_fx/shaders/";

    private static readonly Dictionary<string, Shader> Cache = [];

    /// <summary>The shader <c>fx_&lt;name&gt;.gdshader</c>, loaded once.</summary>
    public static Shader Get(string name)
    {
        if (!Cache.TryGetValue(name, out var shader))
            Cache[name] = shader = GD.Load<Shader>($"{Root}fx_{name}.gdshader");
        return shader;
    }
}
