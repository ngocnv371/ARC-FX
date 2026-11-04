using Godot;

namespace Arc.Fx;

/// <summary>
/// How effects light the scene around them. ARC's shaders are unlit, so an effect cannot cast
/// real light; instead it reports a pool of light each frame and the game's ground and model
/// shaders add it. A game without such pools leaves <see cref="Fx.Lighting"/> null and the
/// effects still look right on their own.
/// </summary>
public interface IFxLighting
{
    /// <summary>A pool of light this frame: centre, how far it reaches, its colour and strength (0 to about 1.5).</summary>
    void Light(Vector3 at, float reach, Color color, float strength);
}
