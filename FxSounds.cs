using Godot;

namespace Arc.Fx;

/// <summary>
/// The sounds effects make, shared: many effects play the same one (every fire spell leaves the
/// hand with <see cref="FireCast"/>). Each is a few takes in <c>audio/</c>, played at random a touch
/// above or below pitch so repeats don't drone.
/// </summary>
public enum FxSound
{
    Swing, Chop, Hit, Plate, Glass, Bow, Dodge, Whirl, Throw, Lob,
    FireCast, FrostCast, LightningCast, PoisonCast, HolyCast, ShadowCast, ArcaneCast, WaterCast, NatureCast,
    FireHit, FrostHit, LightningHit, PoisonHit, HolyHit, ShadowHit, ArcaneHit, WaterHit,
    Boom, Slam, Rumble, Blink, Beam, Aura, Heal, LevelUp,
}

/// <summary>
/// What every effect sounds like, and how loud. Effects called from the game play their own sounds
/// (a meteor's rush, then its blast as it lands; a skill's cast, then its hit at
/// <see cref="FxTiming.SkillHit"/>); what an effect spawns as part of itself stays quiet, so one
/// call is one sound or one cast-and-hit. A game that has its own sounds for some effects turns
/// these off with <see cref="On"/>, or plays them itself with <see cref="Fx.Sound"/>.
/// </summary>
public static class FxSounds
{
    /// <summary>Where the sounds live (see <see cref="FxShaders.Root"/>).</summary>
    public const string Root = "res://addons/arc_fx/audio/";

    /// <summary>False: effects are silent (<see cref="Fx.Sound"/> too).</summary>
    public static bool On { get; set; } = true;

    /// <summary>The audio bus they play on.</summary>
    public static StringName Bus { get; set; } = "Master";

    /// <summary>Raises or lowers them all, in decibels.</summary>
    public static float VolumeDb { get; set; }

    /// <summary>Each sound's takes (<c>name_1..takes</c>, or just <c>name</c> for one) and how loud it is against the rest.</summary>
    internal static (string Name, int Takes, string Ext, float Gain) File(FxSound sound) => sound switch
    {
        FxSound.Swing => ("swing", 3, "ogg", 0.9f),
        FxSound.Chop => ("chop", 3, "ogg", 0.8f),
        FxSound.Hit => ("hit_physical", 3, "ogg", 0.9f),
        FxSound.Plate => ("plate", 3, "ogg", 0.7f),
        FxSound.Glass => ("glass", 3, "ogg", 0.7f),
        FxSound.Bow => ("bow", 4, "ogg", 0.9f),
        FxSound.Dodge => ("dodge", 2, "ogg", 0.9f),
        FxSound.FireCast => ("cast_fire", 3, "ogg", 0.9f),
        FxSound.FrostCast => ("cast_cold", 3, "ogg", 0.9f),
        FxSound.LightningCast => ("cast_lightning", 3, "ogg", 0.9f),
        FxSound.FrostHit => ("hit_cold", 3, "ogg", 0.8f),
        FxSound.HolyHit => ("hit_holy", 3, "ogg", 0.6f),
        FxSound.Whirl => ("whirl", 1, "wav", 0.9f),
        FxSound.Throw => ("throw", 1, "wav", 0.9f),
        FxSound.Lob => ("lob", 1, "wav", 0.9f),
        FxSound.PoisonCast => ("cast_poison", 1, "wav", 0.9f),
        FxSound.HolyCast => ("cast_holy", 1, "wav", 0.8f),
        FxSound.ShadowCast => ("cast_shadow", 1, "wav", 0.9f),
        FxSound.ArcaneCast => ("cast_arcane", 1, "wav", 0.8f),
        FxSound.WaterCast => ("cast_water", 1, "wav", 0.9f),
        FxSound.NatureCast => ("cast_nature", 1, "wav", 0.9f),
        FxSound.FireHit => ("hit_fire", 1, "wav", 1f),
        FxSound.LightningHit => ("hit_lightning", 1, "wav", 0.9f),
        FxSound.PoisonHit => ("hit_poison", 1, "wav", 1f),
        FxSound.ShadowHit => ("hit_shadow", 1, "wav", 1f),
        FxSound.ArcaneHit => ("hit_arcane", 1, "wav", 0.9f),
        FxSound.WaterHit => ("hit_water", 1, "wav", 1f),
        FxSound.Boom => ("boom", 1, "wav", 0.9f),
        FxSound.Slam => ("slam", 1, "wav", 0.9f),
        FxSound.Rumble => ("rumble", 1, "wav", 0.9f),
        FxSound.Blink => ("blink", 1, "wav", 0.9f),
        FxSound.Beam => ("beam", 1, "wav", 0.85f),
        FxSound.Aura => ("aura", 1, "wav", 0.7f),
        FxSound.Heal => ("heal", 1, "wav", 0.8f),
        _ => ("level_up", 1, "wav", 0.8f),
    };

    /// <summary>How loud something <paramref name="radius"/> across is: small things quieter, huge ones a little louder.</summary>
    internal static float Size(float radius) => Math.Clamp(0.5f + radius / 6f, 0.6f, 1.15f);

    /// <summary>
    /// A blast: fire booms, frost shatters, lightning cracks; any other is told by its colour
    /// (green splashes as poison, gold rings as holy, violet thumps as shadow, a strong blue
    /// chimes as arcane, the rest booms as dust).
    /// </summary>
    internal static FxSound Blast(Color color, FxBurst burst) => burst switch
    {
        FxBurst.Fire => FxSound.Boom,
        FxBurst.Frost => FxSound.Glass,
        FxBurst.Lightning => FxSound.LightningHit,
        _ when color.S < 0.3f => FxSound.Boom,
        _ => (color.H * 360f) switch
        {
            < 30f or >= 335f => FxSound.Boom,
            < 65f => FxSound.HolyHit,
            < 170f => FxSound.PoisonHit,
            < 250f => FxSound.ArcaneHit,
            _ => FxSound.ShadowHit,
        },
    };

    /// <summary>A patch of ground or an aura appearing; null for a plain glow or a warning, which are marks, not happenings.</summary>
    internal static FxSound? Ground(FxGround look) => look switch
    {
        FxGround.Fire or FxGround.Inferno => FxSound.FireCast,
        FxGround.Frost => FxSound.FrostCast,
        FxGround.Poison or FxGround.Miasma => FxSound.PoisonCast,
        _ => null,
    };

    /// <summary>A projectile leaving the hand.</summary>
    internal static FxSound Launch(FxShotLook look) => look switch
    {
        FxShotLook.Arrow or FxShotLook.LightningArrow or FxShotLook.PoisonArrow or FxShotLook.PhantomArrow
            or FxShotLook.FireArrow or FxShotLook.IceArrow => FxSound.Bow,
        FxShotLook.Axe or FxShotLook.BoneSpear => FxSound.Throw,
        FxShotLook.FireBolt or FxShotLook.Fireball => FxSound.FireCast,
        FxShotLook.IceSpear or FxShotLook.Frostbolt or FxShotLook.ColdWave => FxSound.FrostCast,
        FxShotLook.Spark => FxSound.LightningCast,
        FxShotLook.SteelWave => FxSound.Swing,
        FxShotLook.SkySplit => FxSound.HolyCast,
        _ => FxSound.Whirl,
    };

    /// <summary>
    /// A skill's sounds: one as it is cast, and one as it lands (<see cref="FxTiming.SkillHit"/>),
    /// either null. Most go by their theme; what is drawn, thrown or done by the body
    /// (a bow, a dash, a blink, a whirl, a boulder, a heal) has its own.
    /// </summary>
    internal static (FxSound? Cast, FxSound? Hit) Of(FxSkill skill)
    {
        var (cast, hit) = FxSkillThemes.Of(skill) switch
        {
            FxSkillTheme.Blade => (FxSound.Swing, FxSound.Chop),
            FxSkillTheme.Earth => (FxSound.Rumble, FxSound.Slam),
            FxSkillTheme.Fire => (FxSound.FireCast, FxSound.FireHit),
            FxSkillTheme.Frost => (FxSound.FrostCast, FxSound.FrostHit),
            FxSkillTheme.Storm => (FxSound.LightningCast, FxSound.LightningHit),
            FxSkillTheme.Water => (FxSound.WaterCast, FxSound.WaterHit),
            FxSkillTheme.Nature => (FxSound.NatureCast, (FxSound?)null),
            FxSkillTheme.Arcane => (FxSound.ArcaneCast, FxSound.ArcaneHit),
            FxSkillTheme.Holy => (FxSound.HolyCast, FxSound.HolyHit),
            FxSkillTheme.Shadow => (FxSound.ShadowCast, FxSound.ShadowHit),
            _ => (FxSound.Aura, (FxSound?)null),
        };
        return (CastOf(skill) ?? cast, HitOf(skill) ?? hit);
    }

    private static FxSound? CastOf(FxSkill skill) => skill switch
    {
        FxSkill.PhantomDash or FxSkill.GaleStep or FxSkill.LandslideCharge or FxSkill.LeapSlam or FxSkill.BlazingDash
            or FxSkill.RewindStep or FxSkill.SpiritSteps or FxSkill.FirewingDive or FxSkill.SnowOwlDive => FxSound.Dodge,
        FxSkill.ShadowBlink or FxSkill.FeyRift or FxSkill.ShiftShot or FxSkill.ThousandSteps or FxSkill.Portal
            or FxSkill.NetherGate or FxSkill.RiftTear or FxSkill.TimeSlip => FxSound.Blink,
        FxSkill.ArrowStorm or FxSkill.PiercingArrow or FxSkill.LongshotBarrage or FxSkill.PhantomArchers
            or FxSkill.SkyArrows or FxSkill.EmberVolley or FxSkill.ThornVolley => FxSound.Bow,
        FxSkill.BoulderToss or FxSkill.ChakramReturn or FxSkill.DiamondLance or FxSkill.FeatherFan or FxSkill.FrostLance
            or FxSkill.Grapple or FxSkill.ReapingHook or FxSkill.ObsidianShards or FxSkill.StoneshotFan
            or FxSkill.PhoenixFeathers or FxSkill.ChainSweep => FxSound.Throw,
        FxSkill.NapalmFlask or FxSkill.LanternToss or FxSkill.VenomSpit or FxSkill.SoulBomber => FxSound.Lob,
        FxSkill.Whirlwind or FxSkill.SteelCyclone or FxSkill.Tempest or FxSkill.CycloneWhorl or FxSkill.SandVortex
            or FxSkill.LeafCyclone or FxSkill.Whirlpool or FxSkill.TidalWhorl or FxSkill.BoneWhorl or FxSkill.PetalStorm
            or FxSkill.MapleGust or FxSkill.GearStorm or FxSkill.Wraithstorm or FxSkill.ButterflyWaltz
            or FxSkill.RibbonDance or FxSkill.SwordHalo or FxSkill.HaloCut or FxSkill.EyeOfTheStorm
            or FxSkill.ZephyrRibbons => FxSound.Whirl,
        FxSkill.ToxicNova or FxSkill.PlagueCloud or FxSkill.SporeCloud or FxSkill.Swarm or FxSkill.BeeSwarm
            or FxSkill.BlightPool => FxSound.PoisonCast,
        FxSkill.Regrowth or FxSkill.MendingTotem or FxSkill.FountainOfLife or FxSkill.Benediction or FxSkill.SpiritBloom
            or FxSkill.ButterflyRenewal or FxSkill.HeartSurge or FxSkill.Sanctuary or FxSkill.LanternsOfHope
            or FxSkill.BlessedRain or FxSkill.RainDance => FxSound.Heal,
        FxSkill.LevelUp or FxSkill.CrownOfVictory or FxSkill.StarBlessing or FxSkill.LuckyStar => FxSound.LevelUp,
        FxSkill.Bloodrage or FxSkill.WrathAura => FxSound.FireCast,
        FxSkill.DarkCrown or FxSkill.LichForm or FxSkill.UmbralWard => FxSound.ShadowCast,
        FxSkill.AegisOath or FxSkill.CrystalAegis or FxSkill.ScaleShield or FxSkill.StoneArmor or FxSkill.PebbleOrbit => FxSound.Plate,
        FxSkill.ManaShield or FxSkill.MoonlitVeil or FxSkill.MirrorImages => FxSound.ArcaneCast,
        _ => null,
    };

    private static FxSound? HitOf(FxSkill skill) => skill switch
    {
        FxSkill.Singularity or FxSkill.Combustion or FxSkill.SkyfireMeteor or FxSkill.GlacialComet or FxSkill.GrandRocket
            or FxSkill.RebornStar or FxSkill.SoulBomber or FxSkill.NebulaBurst or FxSkill.EmberMark or FxSkill.FlamePillar
            or FxSkill.PyreStar or FxSkill.SunFlare or FxSkill.FireworkBloom or FxSkill.Thunderclap
            or FxSkill.NapalmFlask => FxSound.Boom,
        FxSkill.Execution or FxSkill.RollingCinderstone or FxSkill.Avalanche or FxSkill.GuardiansFall
            or FxSkill.SnowOwlDive or FxSkill.ChandelierDrop or FxSkill.DuneRoller => FxSound.Slam,
        FxSkill.MirrorShards or FxSkill.RimeShackle or FxSkill.TimeShatter or FxSkill.FrostHeart or FxSkill.FrostLance
            or FxSkill.IcePrison or FxSkill.FrozenRose or FxSkill.FrostWindow or FxSkill.GeodeBurst
            or FxSkill.AmberTomb => FxSound.Glass,
        FxSkill.Judgment or FxSkill.SolarBrand or FxSkill.RiftBeam or FxSkill.PrismRay or FxSkill.SolarLance
            or FxSkill.RadiantLance or FxSkill.HaloDescent or FxSkill.Recall or FxSkill.SunderingFinger
            or FxSkill.DawnRays or FxSkill.SealOfLight => FxSound.Beam,
        FxSkill.ShadowBlink or FxSkill.FeyRift or FxSkill.ShiftShot or FxSkill.Portal => FxSound.Blink,
        FxSkill.ArrowStorm or FxSkill.PiercingArrow or FxSkill.PhantomArchers or FxSkill.CrescentKick
            or FxSkill.YinYangPalm or FxSkill.ChakramReturn => FxSound.Hit,
        FxSkill.ShieldBash or FxSkill.Riposte or FxSkill.SacredShield => FxSound.Plate,
        FxSkill.ToxicNova or FxSkill.VenomSpit or FxSkill.Swarm or FxSkill.BeeSwarm => FxSound.PoisonHit,
        FxSkill.Thornbind or FxSkill.ThornVolley or FxSkill.ThornCrown or FxSkill.ThornMaze or FxSkill.CreepingRoots
            or FxSkill.Thornline or FxSkill.WidowsWeb => FxSound.Chop,
        FxSkill.HolyNova => FxSound.Heal,
        _ => null,
    };
}

/// <summary>
/// Plays the sounds, from a few pooled 3D voices under the world, on the world's clock: a sound
/// for later waits for its moment and, if tied to an effect, is dropped should that end first.
/// </summary>
internal sealed class FxAudio(Node3D parent)
{
    private const int Voices = 24;
    /// <summary>The same sound starting again within this long is one sound (a chain of arcs, a crowd of blasts).</summary>
    private const float Merge = 0.045f;
    private const float Jitter = 0.06f;

    private readonly List<AudioStreamPlayer3D> _voices = [];
    private readonly List<(float At, FxSound Sound, Vector3 Where, float Volume, FxHandle? While)> _later = [];
    private readonly Dictionary<FxSound, AudioStream[]> _takes = [];
    private readonly Dictionary<FxSound, (float At, int Take)> _last = [];
    private float _clock;

    public void Play(FxSound sound, Vector3 at, float volume = 1f, float delay = 0f, FxHandle? whileAlive = null)
    {
        if (!FxSounds.On)
            return;
        if (delay > 0f)
            _later.Add((_clock + delay, sound, at, volume, whileAlive));
        else if (whileAlive is not { Alive: false })
            Start(sound, at, volume);
    }

    public void Tick(float dt)
    {
        _clock += dt;
        for (var i = _later.Count - 1; i >= 0; i--)
        {
            var (when, sound, at, volume, whileAlive) = _later[i];
            if (when > _clock)
                continue;
            _later.RemoveAt(i);
            if (FxSounds.On && whileAlive is not { Alive: false })
                Start(sound, at, volume);
        }
    }

    private void Start(FxSound sound, Vector3 at, float volume)
    {
        var takes = Takes(sound);
        if (takes.Length == 0)
            return;
        var take = GD.RandRange(0, takes.Length - 1);
        if (_last.TryGetValue(sound, out var last))
        {
            if (_clock - last.At < Merge)
                return;
            if (take == last.Take && takes.Length > 1)
                take = (take + 1) % takes.Length;
        }
        _last[sound] = (_clock, take);

        var voice = Voice();
        voice.Stream = takes[take];
        voice.Position = at;
        voice.Bus = FxSounds.Bus;
        voice.VolumeDb = FxSounds.VolumeDb + Mathf.LinearToDb(Math.Max(volume * FxSounds.File(sound).Gain, 1e-4f));
        voice.PitchScale = 1f + (float)GD.RandRange(-Jitter, Jitter);
        voice.Play();
    }

    /// <summary>A free voice, or the one furthest through its sound.</summary>
    private AudioStreamPlayer3D Voice()
    {
        AudioStreamPlayer3D? oldest = null;
        foreach (var voice in _voices)
        {
            if (!voice.Playing)
                return voice;
            if (oldest == null || voice.GetPlaybackPosition() > oldest.GetPlaybackPosition())
                oldest = voice;
        }
        if (_voices.Count >= Voices && oldest != null)
            return oldest;
        // Heard over the whole battlefield from a camera high above it: a gentle fall-off, no
        // muffling with distance, and only a little panning.
        var made = new AudioStreamPlayer3D
        {
            UnitSize = 24f,
            MaxDb = 3f,
            AttenuationFilterDb = 0f,
            PanningStrength = 0.5f,
        };
        parent.AddChild(made);
        _voices.Add(made);
        return made;
    }

    private AudioStream[] Takes(FxSound sound)
    {
        if (_takes.TryGetValue(sound, out var takes))
            return takes;
        var (name, count, ext, _) = FxSounds.File(sound);
        var paths = count == 1
            ? [$"{FxSounds.Root}{name}.{ext}"]
            : Enumerable.Range(1, count).Select(i => $"{FxSounds.Root}{name}_{i}.{ext}");
        takes = paths.Where(path => ResourceLoader.Exists(path)).Select(path => GD.Load<AudioStream>(path)).Where(s => s != null).ToArray();
        if (takes.Length == 0)
            GD.PushWarning($"ARC FX: no sound for {sound} under {FxSounds.Root}");
        return _takes[sound] = takes;
    }
}
