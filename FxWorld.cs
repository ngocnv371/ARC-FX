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
    private FxSkills? _skills;
    private FxShrines? _shrines;
    private FxAudio? _audio;

    /// <summary>Advance in <c>_Process</c>; false: the game calls <see cref="Tick"/>.</summary>
    public bool AutoTick { get; set; } = true;

    private FxKit Kit => _kit ??= new FxKit(this);
    private FxExplosions Explosions => _explosions ??= new FxExplosions(this);
    private FxPatches Patches => _patches ??= new FxPatches(this);
    private FxLines Lines => _lines ??= new FxLines(Kit);
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
    private FxSkills Skills => _skills ??= new FxSkills(Kit, this);
    private FxShrines Shrines => _shrines ??= new FxShrines(Kit, this);
    private FxAudio Audio => _audio ??= new FxAudio(this);

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
        _depth++;
        try
        {
            Advance(dt);
        }
        finally
        {
            _depth--;
        }
    }

    private void Advance(float dt)
    {
        _audio?.Tick(dt);
        // Things that spawn others (a storm's crackles, a smite's blast, a bomb's burst, a ram's
        // dust, a meteor's flame and blast, a bladefall's glows, a skill's cues, a shrine's sigil, burst and heal) go first, so what they spawn starts this frame.
        _shrines?.Tick(dt);
        _casts?.Tick(dt);
        _skills?.Tick(dt);
        _bladefalls?.Tick(dt);
        _shots?.Tick(dt);
        _storms?.Tick(dt);
        _ires?.Tick(dt);
        _slams?.Tick(dt);
        _rams?.Tick(dt);
        _beams?.Tick(dt);
        _vortices?.Tick(dt);
        _lines?.Tick(dt);
        _patches?.Tick(dt);
        _sigils?.Tick(dt);
        _explosions?.Tick(dt);
    }

    /// <inheritdoc cref="Fx.Explosion(Vector3, float, Color, FxBurst)"/>
    public void Explosion(Vector3 at, float radius, Color color, FxBurst burst = FxBurst.Dome)
    {
        Play(FxSounds.Blast(color, burst), at, FxSounds.Size(radius));
        Inside(() => Explosions.Spawn(at, radius, color, burst));
    }

    /// <inheritdoc cref="Fx.Patch(Vector3, float, float, Color, FxGround)"/>
    public FxPatch Patch(Vector3 at, float radius, float life, Color color, FxGround look = FxGround.Glow)
    {
        Play(FxSounds.Ground(look), at, 0.6f * FxSounds.Size(radius));
        return Inside(() => Patches.Spawn(at, radius, life, color, look));
    }

    /// <inheritdoc cref="Fx.Aura(Node3D, float, float, Color, FxGround)"/>
    public FxPatch Aura(Node3D follow, float radius, float keep, Color color, FxGround look = FxGround.Glow)
    {
        var aura = Inside(() => Patches.Aura(follow, radius, keep, color, look));
        // Renewed every tick: only its arrival sounds.
        if (aura.Age == 0f)
            Play(FxSounds.Ground(look) ?? FxSound.Aura, follow.Position, 0.6f);
        return aura;
    }

    /// <inheritdoc cref="Fx.Sigil(Node3D, FxSigil, float, float, Color, FxSigilLook)"/>
    public FxHandle Sigil(Node3D follow, FxSigil sigil, float radius, float keep, Color color, FxSigilLook look = FxSigilLook.Disc)
    {
        var mark = Inside(() => Sigils.Spawn(follow, sigil, radius, keep, color, look));
        if (mark.Age == 0f)
            Play(FxSound.Aura, follow.Position, 0.6f);
        return mark;
    }

    /// <inheritdoc cref="Fx.Lightning"/>
    public FxHandle Lightning((Vector3 A, Vector3 B)[] segments, float life, int tendrils = 0, float thickness = 1f)
    {
        if (segments.Length > 0)
            Play(FxSound.LightningHit, segments[^1].B, Math.Clamp(0.6f + 0.4f * thickness, 0.6f, 1.1f));
        return Inside(() => Lines.Spawn(segments, life, tendrils, thickness));
    }

    /// <inheritdoc cref="Fx.BladeVortex(Node3D, int, float, float, FxVortexLook)"/>
    public FxHandle BladeVortex(Node3D follow, int blades, float radius, float keep, FxVortexLook look = FxVortexLook.Steel)
    {
        var vortex = Inside(() => Vortices.Spawn(follow, blades, radius, keep, look));
        if (vortex.Age == 0f)
            Play(FxSound.Whirl, follow.Position, 0.8f);
        return vortex;
    }

    /// <inheritdoc cref="Fx.Beam"/>
    public FxHandle Beam(Vector3 at, float radius, float life, Color color)
    {
        Play(FxSound.Beam, at, FxSounds.Size(radius * 2f));
        return Inside(() => Beams.Spawn(at, radius, life, color));
    }

    /// <inheritdoc cref="Fx.StormOrb"/>
    public FxOrb StormOrb(Vector3 centre, float radius, float life)
    {
        Play(FxSound.LightningCast, centre, FxSounds.Size(radius));
        return Inside(() => Storms.StormOrb(centre, radius, life));
    }

    /// <inheritdoc cref="Fx.FrostBomb"/>
    public FxHandle FrostBomb(Vector3 centre, float radius, float fuse)
    {
        var bomb = Inside(() => Storms.FrostBomb(centre, radius, fuse));
        Play(FxSound.FrostCast, centre, 0.8f);
        // It bursts as the fuse runs out: sound just before, while it is still there to be defused.
        Play(FxSound.Glass, centre, FxSounds.Size(radius), Math.Max(fuse - 0.02f, 0f), bomb);
        return bomb;
    }

    /// <inheritdoc cref="Fx.Nova"/>
    public FxHandle Nova(Vector3 centre, float radius)
    {
        Play(FxSound.LightningCast, centre, FxSounds.Size(radius));
        return Inside(() => Storms.Nova(centre, radius));
    }

    /// <inheritdoc cref="Fx.DivineIre"/>
    public FxIreCast DivineIre(Vector3 caster, Vector3 direction, float length, float width)
    {
        var ire = Inside(() => Ires.Spawn(caster, direction, length, width));
        Play(FxSound.HolyCast, caster);
        Play(FxSound.Beam, caster, 1f, FxTiming.IreWindup - 0.02f, ire.Handle);
        return ire;
    }

    /// <inheritdoc cref="Fx.Slam"/>
    public void Slam(Vector3 centre, float radius, FxSpike look)
    {
        Play(FxSound.Slam, centre, FxSounds.Size(radius));
        if (look == FxSpike.Ice)
            Play(FxSound.FrostHit, centre, FxSounds.Size(radius));
        Inside(() => Slams.Slam(centre, radius, look));
    }

    /// <inheritdoc cref="Fx.SpikeRing"/>
    public void SpikeRing(Vector3 centre, float radius, FxSpike look)
    {
        Play(look == FxSpike.Ice ? FxSound.FrostHit : FxSound.Rumble, centre, FxSounds.Size(radius));
        Inside(() => Slams.SpikeRing(centre, radius, look));
    }

    /// <inheritdoc cref="Fx.Spike"/>
    public FxHandle Spike(Vector3 at, Vector3 lean, float height, FxSpike look, float delay = 0f)
    {
        var spike = Inside(() => Slams.Jut(at, lean, height, delay, look));
        Play(look == FxSpike.Ice ? FxSound.FrostHit : FxSound.Rumble, at, Math.Clamp(height / 2f, 0.4f, 1f), delay, spike);
        return spike;
    }

    /// <inheritdoc cref="Fx.Rupture"/>
    public FxHandle Rupture(Vector3 from, Vector3 direction, float length, float width)
    {
        Play(FxSound.Rumble, from, FxSounds.Size(length / 2f));
        return Inside(() => Slams.Rupture(from, direction, length, width));
    }

    /// <inheritdoc cref="Fx.ShieldRing"/>
    public FxHandle ShieldRing(Vector3 centre, int count, float radius)
    {
        Play(FxSound.Plate, centre, FxSounds.Size(radius));
        return Inside(() => Slams.ShieldRing(centre, count, radius));
    }

    /// <inheritdoc cref="Fx.Smite"/>
    public FxHandle Smite(FxSmite look, Vector3 centre, Vector3 facing, float radius)
    {
        var smite = Inside(() => Slams.Smite(look, centre, facing, radius));
        Play(FxSound.Whirl, centre, 0.8f);
        Play(FxSound.Slam, centre, FxSounds.Size(radius), FxTiming.SmiteFall, smite);
        if (look == FxSmite.Hammer)
            Play(FxSound.LightningHit, centre, FxSounds.Size(radius), FxTiming.SmiteFall, smite);
        return smite;
    }

    /// <inheritdoc cref="Fx.Ram"/>
    public FxHandle Ram(FxRam look, Func<Vector3?> at, Vector3 direction, float life, float size = 1f)
    {
        Play(look == FxRam.Stone ? FxSound.Rumble : FxSound.Dodge, at() ?? Vector3.Zero, size);
        return Inside(() => Rams.Spawn(look, at, direction, life, size));
    }

    /// <inheritdoc cref="Fx.Meteor"/>
    public FxHandle Meteor(Vector3 at, float radius)
    {
        var meteor = Inside(() => Casts.SpawnMeteor(at, radius));
        Play(FxSound.FireCast, at, 0.8f);
        Play(FxSound.Boom, at, FxSounds.Size(radius) * 1.1f, FxTiming.MeteorFall, meteor);
        return meteor;
    }

    /// <inheritdoc cref="Fx.Bladefall(Vector3, float, Color)"/>
    public FxHandle Bladefall(Vector3 at, float radius, Color color)
    {
        var fall = Inside(() => Bladefalls.Spawn(at, radius, color));
        Play(FxSound.Swing, at, 0.8f);
        // The swords land over a spread: a few strikes stand for the volley.
        for (var i = 0; i < 4; i++)
            Play(FxSound.Chop, at, 0.8f, FxTiming.BladefallDrop + FxTiming.BladefallSpread * i / 3f, fall);
        return fall;
    }

    /// <inheritdoc cref="Fx.Skill"/>
    public FxHandle Skill(FxSkill skill, Vector3 at, Vector3 facing, float radius, Node3D? follow = null)
    {
        var cast = Inside(() => Skills.Spawn(skill, at, facing, radius, follow));
        var (start, hit) = FxSounds.Of(skill);
        var volume = FxSounds.Size(radius);
        Play(start, at, volume);
        Play(hit, at, volume, FxTiming.SkillHit(skill), cast);
        return cast;
    }

    /// <inheritdoc cref="Fx.Heal"/>
    public FxHandle Heal(Node3D target, Color color, float size = 1f)
    {
        Play(FxSound.Heal, target.Position, size);
        return Inside(() => Casts.SpawnHeal(target, target.Position, color, size));
    }

    /// <inheritdoc cref="Fx.Barrier"/>
    public FxBarrier Barrier(Node3D target, float radius, float life, Color color)
    {
        Play(FxSound.Aura, target.Position, 0.8f);
        return Inside(() => Casts.SpawnBarrier(target, target.Position, radius, life, color));
    }

    /// <inheritdoc cref="Fx.Shrine(Vector3, FxSigil, Color, float)"/>
    public FxShrine Shrine(Vector3 at, FxSigil sigil, Color color, float size = 1f) =>
        Inside(() => Shrines.Spawn(at, sigil, color, size));

    /// <inheritdoc cref="Fx.Shot"/>
    public FxShot Shot(FxShotLook look)
    {
        var asked = _depth == 0;
        var shot = Inside(() => Shots.Spawn(look));
        // Where it leaves from is known only once the game places it: it sounds then.
        if (asked)
            shot.Launch(FxSounds.Launch(look));
        return shot;
    }

    /// <inheritdoc cref="Fx.Sound"/>
    public void Sound(FxSound sound, Vector3 at, float volume = 1f, float delay = 0f) => Audio.Play(sound, at, volume, delay);

    /// <summary>
    /// How deep we are in making effects: above 0, an effect (or the tick) is making its own
    /// parts, which stay quiet, as the effect the game asked for has made its sounds already.
    /// </summary>
    private int _depth;

    private T Inside<T>(Func<T> spawn)
    {
        _depth++;
        try
        {
            return spawn();
        }
        finally
        {
            _depth--;
        }
    }

    private void Inside(Action spawn) => Inside(() =>
    {
        spawn();
        return 0;
    });

    /// <summary>An effect's sound, if the game asked for the effect; one for later is dropped if <paramref name="whileAlive"/> ends first.</summary>
    private void Play(FxSound? sound, Vector3 at, float volume = 1f, float delay = 0f, FxHandle? whileAlive = null)
    {
        if (sound is { } s && _depth == 0)
            Audio.Play(s, at, volume, delay, whileAlive);
    }

    /// <summary>A sound an effect makes when the game acts on it later (a barrier struck, a shrine taken, a shot first placed).</summary>
    internal void Later(FxSound sound, Vector3 at, float volume = 1f, float delay = 0f) => Audio.Play(sound, at, volume, delay);

    /// <summary>The world an effect's node is drawn under (its sounds go on that world's clock), or null once it has gone.</summary>
    internal static FxWorld? Of(Node node) => node.GetParent() as FxWorld;

    // The fx shaders take display colours and decode them themselves.
    internal static Vector3 Rgb(Color c) => new(c.R, c.G, c.B);
}
