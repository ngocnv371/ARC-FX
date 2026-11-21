namespace Arc.Fx;

/// <summary>
/// When effects that unfold over time reach things, so a game's hits can land as the picture
/// says they do. Effects only draw; these numbers are the contract between the two.
/// </summary>
public static class FxTiming
{
    /// <summary>A nova's rings reach their full radius after this long (easing out), and are gone after <see cref="NovaLife"/>.</summary>
    public const float NovaSpread = 0.25f;
    public const float NovaLife = 0.7f;
    /// <summary>How deep the ring of lightning at a nova's edge is, in metres.</summary>
    public const float NovaBand = 1.2f;
    /// <summary>Where a nova's inner ring of sparks stops, as a share of its radius.</summary>
    public const float NovaInner = 0.55f;

    /// <summary>How far a nova's rings have come after <paramref name="age"/> seconds.</summary>
    public static float NovaFront(float age, float radius)
    {
        var spread = Math.Min(age / NovaSpread, 1f);
        return radius * (1f - (1f - spread) * (1f - spread));
    }

    /// <summary>Divine Ire: how long light gathers before the beam fires, then how long the beam lingers.</summary>
    public const float IreWindup = 0.6f;
    public const float IreLinger = 0.5f;
    /// <summary>How far ahead of the caster the beam starts.</summary>
    public const float IreLead = 0.6f;

    /// <summary>An arc's jump (<see cref="Fx.Arc"/>): how long its bolt shows, and the beat between one jump and the next.</summary>
    public const float ArcLife = 0.22f;
    public const float ArcLeap = 0.07f;

    /// <summary>A shield ring reaches its radius after this long (easing out), and is gone after <see cref="ShieldLife"/>.</summary>
    public const float ShieldSpread = 0.3f;
    public const float ShieldLife = 0.6f;

    /// <summary>How far a shield ring's shields have come after <paramref name="age"/> seconds.</summary>
    public static float ShieldRingRadius(float age, float radius)
    {
        var spread = Math.Min(1f, age / ShieldSpread);
        return 0.9f + (radius - 0.9f) * (1f - (1f - spread) * (1f - spread));
    }

    /// <summary>A smite lands this long after it starts, then lingers fading.</summary>
    public const float SmiteFall = 0.32f;
    public const float SmiteLinger = 0.45f;

    /// <summary>A meteor lands (and its blast goes off) this long after it is called.</summary>
    public const float MeteorFall = 0.9f;

    /// <summary>
    /// A bladefall's blades: each takes <see cref="BladefallDrop"/> to fall, and they start over
    /// <see cref="BladefallSpread"/>, so they land from BladefallDrop after the call until
    /// BladefallDrop + BladefallSpread.
    /// </summary>
    public const float BladefallDrop = 0.22f;
    public const float BladefallSpread = 0.7f;

    /// <summary>A taken shrine's light reaches whoever took it this long after <see cref="FxShrine.Take"/>: the moment to give the boon.</summary>
    public const float ShrineReach = 0.55f;

    /// <summary>A rupture's spikes race along the ground at this many metres a second.</summary>
    public const float RuptureSpeed = 26f;

    /// <summary>
    /// When a skill (<see cref="Fx.Skill"/>) lands, in seconds after the call: the second blow of
    /// a cross slash, a dash's arrival, a blink's reappearance, a burst after its gathering, a
    /// pillar after its telegraph, Judgment's strike, Thunder Call's first bolt, a singularity's
    /// collapse, a recall's flash, a thrown or fired thing's arrival and the first of a volley's
    /// landings. Skills that land at once, and those that last (Whirlwind, Stasis Dome, Sanctuary,
    /// Bloodrage, Blizzard...), give 0.
    /// </summary>
    public static float SkillHit(FxSkill skill) => skill switch
    {
        FxSkill.CrimsonSlash => 0.05f,
        FxSkill.CrossSlash => 0.17f,
        FxSkill.RisingCleave => 0.06f,
        FxSkill.PhantomDash => 0.18f,
        FxSkill.ArcaneBurst => 0.45f,
        FxSkill.Singularity => 2.3f,
        FxSkill.FlamePillar => 0.5f,
        FxSkill.Judgment => 0.6f,
        FxSkill.ThunderCall => 0.25f,
        FxSkill.ShadowBlink => 0.12f,
        FxSkill.Recall => 2.4f,
        FxSkill.Execution => 0.1f,
        FxSkill.Lunge => 0.07f,
        FxSkill.Grapple => 0.22f,
        FxSkill.LeapSlam => 0.35f,
        FxSkill.BoulderToss => 0.5f,
        FxSkill.DragonBreath => 0.1f,
        FxSkill.FireRain => 0.4f,
        FxSkill.BlazingDash => 0.2f,
        FxSkill.Combustion => 0.55f,
        FxSkill.EmberMark => 1.5f,
        FxSkill.FrostLance => 0.32f,
        FxSkill.Thunderclap => 0.3f,
        FxSkill.Thornbind => 0.15f,
        FxSkill.VenomSpit => 0.5f,
        FxSkill.ArcaneMissiles => 0.4f,
        FxSkill.RiftBeam => 0.08f,
        FxSkill.SacredLances => 0.2f,
        FxSkill.ShadowClaws => 0.05f,
        FxSkill.DeathCoil => 0.4f,
        FxSkill.ShadeClones => 0.3f,
        FxSkill.PyreStrike => 0.35f,
        FxSkill.CrimsonVerdict => 0.15f,
        FxSkill.Skybolt => 0.35f,
        FxSkill.HeavensWrath => 0.6f,
        FxSkill.SolarBrand => 1.4f,
        FxSkill.RollingCinderstone => 0.5f,
        FxSkill.ThousandSteps => 0.12f,
        FxSkill.RewindStep => 0.2f,
        FxSkill.FeyRift => 0.1f,
        FxSkill.SunderingFinger => 0.2f,
        FxSkill.EmberWisps => 1.55f,
        FxSkill.SolarLance => 0.1f,
        FxSkill.RebornStar => 2.2f,
        FxSkill.GaleThrust => 0.07f,
        FxSkill.GaleStep => 0.18f,
        FxSkill.SkywardCut => 0.25f,
        FxSkill.BeguilingMote => 0.6f,
        FxSkill.SpiritSteps => 0.12f,
        FxSkill.BindOfDawn => 0.4f,
        FxSkill.PrismWard => 0.5f,
        FxSkill.LingeringStar => 2f,
        FxSkill.RadiantLance => 1f,
        FxSkill.ShockLine => 0.35f,
        FxSkill.SnapTraps => 1.2f,
        FxSkill.GrandRocket => 0.9f,
        FxSkill.ReapingHook => 0.22f,
        FxSkill.LanternToss => 0.5f,
        FxSkill.ChainSweep => 0.05f,
        FxSkill.ArcaneBolt => 0.25f,
        FxSkill.ShiftShot => 0.08f,
        FxSkill.LongshotBarrage => 0.8f,
        FxSkill.LandslideCharge => 0.6f,
        FxSkill.FlareFan => 0.05f,
        FxSkill.GuardiansFall => 0.5f,
        FxSkill.SwordOfVerdict => 0.45f,
        FxSkill.UmbralBind => 0.45f,
        FxSkill.SoulFetters => 2.4f,
        FxSkill.Dreamtether => 2.2f,
        FxSkill.RimeShackle => 1.6f,
        FxSkill.MoonfallCut => 0.18f,
        FxSkill.BladeRain => 0.25f,
        FxSkill.Monolith => 0.4f,
        FxSkill.StoneshotFan => 0.4f,
        FxSkill.SkyfireMeteor => 1.0f,
        FxSkill.FlameSerpent => 0.9f,
        FxSkill.EmberVolley => 0.45f,
        FxSkill.NapalmFlask => 0.5f,
        FxSkill.IcicleBarrage => 0.3f,
        FxSkill.GlacialComet => 0.8f,
        FxSkill.StormLance => 0.45f,
        FxSkill.TeslaCoil => 0.4f,
        FxSkill.TidalWave => 0.5f,
        FxSkill.Geyser => 0.6f,
        FxSkill.BubblePrison => 0.15f,
        FxSkill.PetalStorm => 1.6f,
        FxSkill.CreepingRoots => 0.6f,
        FxSkill.ThornVolley => 0.7f,
        FxSkill.MirrorImages => 0.25f,
        FxSkill.AstralSigils => 0.55f,
        FxSkill.RiftTear => 1.6f,
        FxSkill.SealOfLight => 1.2f,
        FxSkill.ShadowTendrils => 0.3f,
        FxSkill.SoulHarvest => 0.15f,
        FxSkill.GhostLegion => 1.7f,
        FxSkill.RagingSpirits => 1.1f,
        FxSkill.SkullBarrage => 0.5f,
        FxSkill.GraveCall => 0.5f,
        FxSkill.PhantomHost => 0.8f,
        FxSkill.CrowMurder => 0.5f,
        FxSkill.HauntedGround => 0.2f,
        FxSkill.BansheeWail => 0.35f,
        FxSkill.SoulBomber => 0.9f,
        FxSkill.DeathsJaw => 1.0f,
        FxSkill.ReaperShade => 0.5f,
        FxSkill.SkullRain => 0.45f,
        FxSkill.Possession => 0.55f,
        FxSkill.NetherGate => 1.0f,
        FxSkill.LotusFlurry => 0.64f,
        FxSkill.StarlitCrescent => 0.1f,
        FxSkill.ButterflyWaltz => 0.15f,
        FxSkill.TwinMoons => 0.26f,
        FxSkill.FeatherFan => 0.38f,
        FxSkill.IaidoBloom => 0.38f,
        FxSkill.PrismaticEdge => 0.3f,
        FxSkill.DiamondLance => 0.1f,
        FxSkill.ButterflySwarm => 0.85f,
        FxSkill.WispCompanions => 0.8f,
        FxSkill.DandelionDrift => 0.3f,
        FxSkill.ThornCrown => 0.3f,
        FxSkill.SpiritBloom => 0.6f,
        FxSkill.BloomBurst => 0.45f,
        FxSkill.HaloDescent => 0.8f,
        FxSkill.DawnRays => 0.15f,
        FxSkill.StarOfGrace => 0.7f,
        FxSkill.PhoenixRise => 0.3f,
        FxSkill.SunFlare => 0.9f,
        FxSkill.CandleVigil => 1.29f,
        FxSkill.EmberButterflies => 0.85f,
        FxSkill.FireworkBloom => 0.45f,
        FxSkill.LavaLotus => 1.1f,
        FxSkill.DragonSpiral => 0.9f,
        FxSkill.CinderFall => 0.4f,
        FxSkill.SnowfallSanctum => 0.2f,
        FxSkill.FrostLotus => 0.5f,
        FxSkill.ChandelierDrop => 1.02f,
        FxSkill.MirrorShards => 0.9f,
        FxSkill.FrozenStar => 0.9f,
        FxSkill.FrostHeart => 2.2f,
        FxSkill.StarCall => 0.4f,
        FxSkill.VoltLattice => 0.35f,
        FxSkill.PlasmaBloom => 1.5f,
        FxSkill.BallLightningTrio => 1.1f,
        FxSkill.TempestCrown => 0.85f,
        FxSkill.PearlBarrage => 0.55f,
        FxSkill.MoonTide => 1.1f,
        FxSkill.BubbleStream => 0.6f,
        FxSkill.CrystalGarden => 0.3f,
        FxSkill.GeodeBurst => 0.7f,
        FxSkill.JadePillars => 0.35f,
        FxSkill.GoldenFissure => 0.5f,
        FxSkill.DuneRoller => 1.2f,
        FxSkill.AmberTomb => 0.35f,
        FxSkill.StarMap => 1.5f,
        FxSkill.PrismRay => 0.4f,
        FxSkill.GravityLens => 1.8f,
        FxSkill.EyeOfInsight => 0.4f,
        FxSkill.CharmHeart => 0.75f,
        FxSkill.RuneCascade => 1.15f,
        FxSkill.NebulaBurst => 1.4f,
        FxSkill.UmbralEclipse => 1.8f,
        FxSkill.TimeShatter => 1.35f,
        FxSkill.WishingStars => 1.9f,
        _ => 0f,
    };
}
