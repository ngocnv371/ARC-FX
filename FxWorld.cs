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
    private FxKit? _kit;
    private FxExplosions? _explosions;
    private FxPatches? _patches;
    private FxLines? _lines;
    private FxSlashes? _slashes;
    private FxBeams? _beams;
    private FxStorms? _storms;
    private FxIres? _ires;
    private FxSlams? _slams;
    private FxRams? _rams;
    private FxShots? _shots;
    private FxCasts? _casts;
    private FxSigils? _sigils;
    private FxVortices? _vortices;
    private FxBladefalls? _bladefalls;

    /// <summary>Advance in <c>_Process</c>; false: the game calls <see cref="Tick"/>.</summary>
    public bool AutoTick { get; set; } = true;

    private FxKit Kit => _kit ??= new FxKit(this);
    private FxExplosions Explosions => _explosions ??= new FxExplosions(this);
    private FxPatches Patches => _patches ??= new FxPatches(this);
    private FxLines Lines => _lines ??= new FxLines(Kit);
    private FxSlashes Slashes => _slashes ??= new FxSlashes(Kit);
    private FxBeams Beams => _beams ??= new FxBeams(Kit);
    private FxStorms Storms => _storms ??= new FxStorms(Kit, this);
    private FxIres Ires => _ires ??= new FxIres(Kit);
    private FxSlams Slams => _slams ??= new FxSlams(Kit, this);
    private FxRams Rams => _rams ??= new FxRams(Kit, this);
    private FxShots Shots => _shots ??= new FxShots(Kit);
    private FxCasts Casts => _casts ??= new FxCasts(Kit, this);
    private FxSigils Sigils => _sigils ??= new FxSigils(Kit);
    private FxVortices Vortices => _vortices ??= new FxVortices(Kit);
    private FxBladefalls Bladefalls => _bladefalls ??= new FxBladefalls(Kit, this);

    public override void _EnterTree() => Fx.World = this;

    public override void _ExitTree()
    {
        if (Fx.World == this)
            Fx.World = null;
        _shots?.Release();
    }

    public override void _Process(double delta)
    {
        if (AutoTick)
            Tick((float)delta);
    }

    /// <summary>Advances every effect by <paramref name="dt"/> seconds.</summary>
    public void Tick(float dt)
    {
        // Things that spawn others (a storm's crackles, a smite's blast, a bomb's burst, a ram's
        // dust, a meteor's flame and blast, a bladefall's glows) go first, so what they spawn starts this frame.
        _casts?.Tick(dt);
        _bladefalls?.Tick(dt);
        _shots?.Tick(dt);
        _storms?.Tick(dt);
        _ires?.Tick(dt);
        _slams?.Tick(dt);
        _rams?.Tick(dt);
        _beams?.Tick(dt);
        _slashes?.Tick(dt);
        _vortices?.Tick(dt);
        _lines?.Tick(dt);
        _patches?.Tick(dt);
        _sigils?.Tick(dt);
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

    /// <inheritdoc cref="Fx.Sigil(Node3D, FxSigil, float, float, Color, FxSigilLook)"/>
    public FxHandle Sigil(Node3D follow, FxSigil sigil, float radius, float keep, Color color, FxSigilLook look = FxSigilLook.Disc) =>
        Sigils.Spawn(follow, sigil, radius, keep, color, look);

    /// <inheritdoc cref="Fx.Lightning"/>
    public FxHandle Lightning((Vector3 A, Vector3 B)[] segments, float life, int tendrils = 0, float thickness = 1f) =>
        Lines.Spawn(segments, life, fire: false, tendrils, thickness);

    /// <inheritdoc cref="Fx.FireWall"/>
    public FxHandle FireWall((Vector3 A, Vector3 B)[] segments, float life) => Lines.Spawn(segments, life, fire: true, 0);

    /// <inheritdoc cref="Fx.Slash"/>
    public FxHandle Slash(Vector3 centre, Vector3 forward, float radius, float arc, Color color, Node3D? follow = null) =>
        Slashes.Spawn(centre, forward, radius, arc, color, follow);

    /// <inheritdoc cref="Fx.BladeVortex(Node3D, int, float, float, FxVortexLook)"/>
    public FxHandle BladeVortex(Node3D follow, int blades, float radius, float keep, FxVortexLook look = FxVortexLook.Steel) =>
        Vortices.Spawn(follow, blades, radius, keep, look);

    /// <inheritdoc cref="Fx.Beam"/>
    public FxHandle Beam(Vector3 at, float radius, float life, Color color) => Beams.Spawn(at, radius, life, color);

    /// <inheritdoc cref="Fx.StormOrb"/>
    public FxOrb StormOrb(Vector3 centre, float radius, float life) => Storms.StormOrb(centre, radius, life);

    /// <inheritdoc cref="Fx.FrostBomb"/>
    public FxHandle FrostBomb(Vector3 centre, float radius, float fuse) => Storms.FrostBomb(centre, radius, fuse);

    /// <inheritdoc cref="Fx.Nova"/>
    public FxHandle Nova(Vector3 centre, float radius) => Storms.Nova(centre, radius);

    /// <inheritdoc cref="Fx.DivineIre"/>
    public FxIreCast DivineIre(Vector3 caster, Vector3 direction, float length, float width) =>
        Ires.Spawn(caster, direction, length, width);

    /// <inheritdoc cref="Fx.Slam"/>
    public void Slam(Vector3 centre, float radius, FxSpike look) => Slams.Slam(centre, radius, look);

    /// <inheritdoc cref="Fx.SpikeRing"/>
    public void SpikeRing(Vector3 centre, float radius, FxSpike look) => Slams.SpikeRing(centre, radius, look);

    /// <inheritdoc cref="Fx.Spike"/>
    public FxHandle Spike(Vector3 at, Vector3 lean, float height, FxSpike look, float delay = 0f) =>
        Slams.Jut(at, lean, height, delay, look);

    /// <inheritdoc cref="Fx.Rupture"/>
    public FxHandle Rupture(Vector3 from, Vector3 direction, float length, float width) => Slams.Rupture(from, direction, length, width);

    /// <inheritdoc cref="Fx.ShieldRing"/>
    public FxHandle ShieldRing(Vector3 centre, int count, float radius) => Slams.ShieldRing(centre, count, radius);

    /// <inheritdoc cref="Fx.Smite"/>
    public FxHandle Smite(FxSmite look, Vector3 centre, Vector3 facing, float radius) => Slams.Smite(look, centre, facing, radius);

    /// <inheritdoc cref="Fx.Ram"/>
    public FxHandle Ram(FxRam look, Func<Vector3?> at, Vector3 direction, float life, float size = 1f) =>
        Rams.Spawn(look, at, direction, life, size);

    /// <inheritdoc cref="Fx.Meteor"/>
    public FxHandle Meteor(Vector3 at, float radius) => Casts.SpawnMeteor(at, radius);

    /// <inheritdoc cref="Fx.Bladefall(Vector3, float, Color)"/>
    public FxHandle Bladefall(Vector3 at, float radius, Color color) => Bladefalls.Spawn(at, radius, color);

    /// <inheritdoc cref="Fx.Heal"/>
    public FxHandle Heal(Node3D target, Color color, float size = 1f) => Casts.SpawnHeal(target, target.Position, color, size);

    /// <inheritdoc cref="Fx.Barrier"/>
    public FxBarrier Barrier(Node3D target, float radius, float life, Color color) =>
        Casts.SpawnBarrier(target, target.Position, radius, life, color);

    /// <inheritdoc cref="Fx.Shot"/>
    public FxShot Shot(FxShotLook look) => Shots.Spawn(look);

    // The fx shaders take display colours and decode them themselves.
    internal static Vector3 Rgb(Color c) => new(c.R, c.G, c.B);
}
