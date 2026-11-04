using Godot;

namespace Arc.Fx;

/// <summary>
/// The node that holds and animates a scene's effects. Add one to the scene; it registers
/// itself as <see cref="Fx.World"/> while in the tree. By default it advances in
/// <c>_Process</c>; a game that runs its own clock (hit-stop, slow motion, a fixed tick order)
/// sets <see cref="AutoTick"/> false and calls <see cref="Tick"/> itself.
/// </summary>
public partial class FxWorld : Node3D
{
    private FxExplosions? _explosions;
    private FxPatches? _patches;

    /// <summary>Advance in <c>_Process</c>; false: the game calls <see cref="Tick"/>.</summary>
    public bool AutoTick { get; set; } = true;

    private FxExplosions Explosions => _explosions ??= new FxExplosions(this);
    private FxPatches Patches => _patches ??= new FxPatches(this);

    public override void _EnterTree() => Fx.World = this;

    public override void _ExitTree()
    {
        if (Fx.World == this)
            Fx.World = null;
    }

    public override void _Process(double delta)
    {
        if (AutoTick)
            Tick((float)delta);
    }

    public void Tick(float dt)
    {
        _patches?.Tick(dt);
        _explosions?.Tick(dt);
    }

    /// <inheritdoc cref="Fx.Explosion(Vector3, float, Color, FxBurst)"/>
    public void Explosion(Vector3 at, float radius, Color color, FxBurst burst = FxBurst.Dome) =>
        Explosions.Spawn(at, radius, color, burst);

    /// <inheritdoc cref="Fx.Patch(Vector3, float, float, Color, FxGround)"/>
    public FxPatch Patch(Vector3 at, float radius, float life, Color color, FxGround look = FxGround.Glow) =>
        Patches.Spawn(at, radius, life, color, look);

    /// <inheritdoc cref="Fx.Aura(Node3D, float, float, Color, FxGround)"/>
    public FxPatch Aura(Node3D follow, float radius, float keep, Color color, FxGround look = FxGround.Glow) =>
        Patches.Aura(follow, radius, keep, color, look);

    // The fx shaders take display colours and decode them themselves.
    internal static Vector3 Rgb(Color c) => new(c.R, c.G, c.B);
}
