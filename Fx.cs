using Godot;

namespace Arc.Fx;

/// <summary>
/// The front door: spawn an effect by name and forget it. Every call draws only; what an
/// effect does to the game (damage, statuses, knockback) is the caller's business.
/// Calls go to the <see cref="FxWorld"/> in the tree; with none, they do nothing.
/// <code>
/// Fx.Explosion(at, radius: 3f, FxElement.Fire);
/// </code>
/// </summary>
public static class Fx
{
    /// <summary>The scene's effects, set when an <see cref="FxWorld"/> enters the tree.</summary>
    public static FxWorld? World { get; internal set; }

    /// <summary>Where effects report the light they give off; null for none.</summary>
    public static IFxLighting? Lighting { get; set; }

    /// <summary>
    /// A blast at <paramref name="at"/> reaching <paramref name="radius"/>. Fire swells into a
    /// globe of flame that pops into a rolling ring and leaves a scorch; frost into a crystal globe
    /// that shatters into a ring of ice over hoarfrost; lightning into a crackling globe over a
    /// small nova. Anything else is a churning dome over a shockwave in the element's colour.
    /// </summary>
    public static void Explosion(Vector3 at, float radius, FxElement element) =>
        World?.Explosion(at, radius, FxTheme.Of(element), FxExplosions.BurstOf(element));

    /// <summary>An explosion in any colour, shaped as <paramref name="burst"/>.</summary>
    public static void Explosion(Vector3 at, float radius, Color color, FxBurst burst = FxBurst.Dome) =>
        World?.Explosion(at, radius, color, burst);

    /// <summary>
    /// A patch of ground at <paramref name="at"/> for <paramref name="life"/> seconds: burning
    /// ground for fire, hoarfrost for frost, a bubbling pool for poison, otherwise a glow in the
    /// element's colour. The handle moves, keeps or ends it.
    /// </summary>
    public static FxPatch Patch(Vector3 at, float radius, float life, FxElement element) =>
        World?.Patch(at, radius, life, FxTheme.Of(element), FxPatches.GroundOf(element)) ?? default;

    /// <summary>A patch of ground in any colour and look.</summary>
    public static FxPatch Patch(Vector3 at, float radius, float life, Color color, FxGround look = FxGround.Glow) =>
        World?.Patch(at, radius, life, color, look) ?? default;

    /// <summary>
    /// A patch that rides on <paramref name="follow"/> for <paramref name="keep"/> more seconds.
    /// Calling again for the same node and look refreshes that one, so an aura can be renewed
    /// every time it ticks.
    /// </summary>
    public static FxPatch Aura(Node3D follow, float radius, float keep, FxElement element) =>
        World?.Aura(follow, radius, keep, FxTheme.Of(element), FxPatches.GroundOf(element)) ?? default;

    /// <summary>An aura in any colour and look; <see cref="FxGround.Inferno"/> raises a bubble of fire over it.</summary>
    public static FxPatch Aura(Node3D follow, float radius, float keep, Color color, FxGround look = FxGround.Glow) =>
        World?.Aura(follow, radius, keep, color, look) ?? default;

    internal static void Light(Vector3 at, float reach, Color color, float strength) =>
        Lighting?.Light(at, reach, color, strength);
}
