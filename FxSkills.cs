using Godot;

namespace Arc.Fx;

/// <summary>
/// A hero's skill, in the bold, saturated look of a top-down hero brawler: big shapes that read
/// at a glance from the game camera, white-hot cores over strong colour. See <see cref="Fx.Skill"/>.
/// </summary>
public enum FxSkill
{
    /// <summary>A huge blood-red crescent swept right to left, layered, throwing off drops of blood.</summary>
    CrimsonSlash,
    /// <summary>Two azure crescents slashed across each other in front, an X with a flash where they cross.</summary>
    CrossSlash,
    /// <summary>A golden crescent cleaving up out of the ground in front, sparks flying up off a crack.</summary>
    RisingCleave,
    /// <summary>Spinning steel crescents round the caster over a swirl of dust (follows them).</summary>
    Whirlwind,
    /// <summary>A violet dash along <c>facing</c>: speed lines, fading afterimages and a slash where it ends.</summary>
    PhantomDash,
    /// <summary>A shockwave of dust rolling out over glowing cracks, throwing up rubble.</summary>
    Shockwave,
    /// <summary>Arcane light gathering into an orb over a rune circle, then bursting.</summary>
    ArcaneBurst,
    /// <summary>A black hole hanging over a swirling disc, drawing streaks in, then collapsing with a blast.</summary>
    Singularity,
    /// <summary>A dome of hexagons over a rune circle, motes held still inside it.</summary>
    StasisDome,
    /// <summary>A ring of frost racing out, ice spikes thrusting up across it, hoarfrost left behind.</summary>
    GlacialBurst,
    /// <summary>A telegraph filling, then a pillar of flame erupting from it with a fire blast.</summary>
    FlamePillar,
    /// <summary>A golden rune circle drawn under motes falling from the sky, then a pillar of holy light.</summary>
    Judgment,
    /// <summary>A storm circle, then bolt after bolt striking across it, each cracking the ground.</summary>
    ThunderCall,
    /// <summary>A thick green ring of toxin rolling out, puffs of gas and a bubbling pool behind it.</summary>
    ToxicNova,
    /// <summary>A shadow blink along <c>facing</c>: a void swallows them, a thin streak, and one spits them out.</summary>
    ShadowBlink,
    /// <summary>A green rune circle with a curtain of light round its edge, motes rising all through it.</summary>
    Sanctuary,
    /// <summary>A golden spiral rising round the caster, a star at their head and rings rolling out (follows them).</summary>
    LevelUp,
    /// <summary>A blue rune circle and a spiral winding up round the caster, ending in a column of light (follows them).</summary>
    Recall,
    /// <summary>A red burst, then flames licking round the caster and embers rising (follows them).</summary>
    Bloodrage,
    /// <summary>A star flare with spikes radiating out of it, a quick slash and sparks: a critical hit.</summary>
    CriticalStrike,

    // The skills from here on that go somewhere (thrown, fired, dashed or cast along a line) go
    // facing's length, or 3 radii when facing is a bare direction.

    /// <summary>A flurry of small steel slashes in every direction round a spot in front, ending in a flash.</summary>
    ThousandCuts,
    /// <summary>A gilded red cleave chopped straight down in front, cracking the ground with a blast.</summary>
    Execution,
    /// <summary>Three jade crescents slashed one after another, each a step further forward.</summary>
    BladeDance,
    /// <summary>A golden spear-thrust straight ahead, a ring of air bursting off its point.</summary>
    Lunge,
    /// <summary>An upright azure crescent sent skimming along the ground, cracking it as it goes.</summary>
    GroundWave,
    /// <summary>A golden arc of parry in front, a flash and sparks bouncing off it.</summary>
    Riposte,
    /// <summary>A leap along facing on an arc of dust, landing in a shockwave, cracks and stone spikes.</summary>
    LeapSlam,
    /// <summary>An iron chain hurled ahead and yanked back, flashing where it hooks.</summary>
    Grapple,
    /// <summary>A wall of rock spikes thrusting up across the way in front.</summary>
    EarthenWall,
    /// <summary>A glowing fissure of magma tearing open along facing, flames spitting from it.</summary>
    MagmaFissure,
    /// <summary>Three shockwaves, each wider, over cracks that spread with them.</summary>
    Quake,
    /// <summary>A boulder heaved along facing, smashing into a cloud of dust and rubble.</summary>
    BoulderToss,
    /// <summary>Rocks orbiting the caster, flung out at the end (follows them).</summary>
    StoneArmor,
    /// <summary>A ring of flame columns round the edge over burning ground.</summary>
    InfernoRing,
    /// <summary>A cone of billowing flame breathed along facing, scorching the ground.</summary>
    DragonBreath,
    /// <summary>Fireballs raining across the area, each landing as a fire blast.</summary>
    FireRain,
    /// <summary>A dash along facing leaving burning ground behind, bursting into flame where it ends.</summary>
    BlazingDash,
    /// <summary>A fire orb swelling over a rune circle, then exploding with flame columns round it.</summary>
    Combustion,
    /// <summary>A burning mark over the head pulsing three times, then exploding.</summary>
    EmberMark,
    /// <summary>Ice shards hailing down over a swirl of snow and hoarfrost.</summary>
    Blizzard,
    /// <summary>Ice spikes leaning in round a spot under a dome of frost: a cage.</summary>
    IcePrison,
    /// <summary>An ice spear thrown along facing, bursting into a frost blast.</summary>
    FrostLance,
    /// <summary>A path of hoarfrost and ice spikes spreading along facing.</summary>
    GlacierPath,
    /// <summary>A slow orb of ice drifting along facing, spitting shards all round, bursting at the end.</summary>
    FrozenOrb,
    /// <summary>Lightning leaping from target to target along facing, a burst at each.</summary>
    ChainLightning,
    /// <summary>A dome of crackling hexagons, bolts snapping across it.</summary>
    StaticField,
    /// <summary>A ball of lightning drifting along facing, striking the ground under it.</summary>
    BallLightning,
    /// <summary>Light gathering, then one huge bolt from the sky and a nova.</summary>
    Thunderclap,
    /// <summary>A tornado whirling on the spot over a swirl of dust and leaves.</summary>
    Tempest,
    /// <summary>Thorny vines spiralling up out of cracked ground round a spot.</summary>
    Thornbind,
    /// <summary>A gob of venom spat along facing, splashing into a bubbling pool.</summary>
    VenomSpit,
    /// <summary>A cloud of plague with motes rising and flies buzzing through it.</summary>
    PlagueCloud,
    /// <summary>Leaves spiralling up round the caster, a green ring rolling out (follows them).</summary>
    Regrowth,
    /// <summary>A buzzing swarm surging along facing.</summary>
    Swarm,
    /// <summary>Five arcane missiles curving in along facing, popping one after another.</summary>
    ArcaneMissiles,
    /// <summary>A dome of arcane hexagons round the caster, motes orbiting (follows them).</summary>
    ManaShield,
    /// <summary>A beam of arcane light fired along facing from an upright rune circle.</summary>
    RiftBeam,
    /// <summary>Rings closing in over a swirling disc, motes dragged down into it.</summary>
    GravityWell,
    /// <summary>Two upright portals, here and along facing, motes streaming from one to the other.</summary>
    Portal,
    /// <summary>A cone of streaks in every colour sprayed along facing.</summary>
    PrismaticSpray,
    /// <summary>A golden dome of hexagons round the caster, light rising inside (follows them).</summary>
    DivineShield,
    /// <summary>Holy ground: a rune circle, rays of light rising all through it.</summary>
    Consecration,
    /// <summary>Golden rings and healing motes bursting out from a column of light.</summary>
    HolyNova,
    /// <summary>Wings of golden light spreading and beating behind the caster (follows them).</summary>
    SeraphWings,
    /// <summary>Spears of light falling from the sky across the area.</summary>
    SacredLances,
    /// <summary>Three parallel violet claw marks raked across in front.</summary>
    ShadowClaws,
    /// <summary>A shadow tether along facing, motes of life flowing back to the caster.</summary>
    SoulDrain,
    /// <summary>A coil of darkness hurled along facing, bursting where it lands.</summary>
    DeathCoil,
    /// <summary>Three shades of the caster dashing out and slashing.</summary>
    ShadeClones,
    /// <summary>A crimson-violet rune circle under a dark sigil, darkness dripping round it.</summary>
    Curse,

    // The case-study set (docs/hero-case-study.md): our own versions of the mechanics of classic
    // hero kits. Skills that reach several things spread their own points over the radius.

    /// <summary>A front of flame sweeping out along a widening cone, scorching it.</summary>
    CinderWave,
    /// <summary>A tight warning ring, then a squat eruption of fire inside it.</summary>
    PyreStrike,
    /// <summary>A thick, jagged red-and-black bolt from the hand, a hard flash where it lands.</summary>
    CrimsonVerdict,
    /// <summary>A burst of frost with a snowflake rune left in the hoarfrost.</summary>
    RimeBloom,
    /// <summary>Ice crystals growing round a spot, holding, then shattering.</summary>
    RimeShackle,
    /// <summary>A snowstorm round the caster, frost bursting at random inside it (follows them).</summary>
    FrozenExpanse,
    /// <summary>A cloud swirling overhead, then a single bolt from it.</summary>
    Skybolt,
    /// <summary>A charge-up round the caster, then bolts from the sky on several points at once.</summary>
    HeavensWrath,
    /// <summary>A faint gold ring narrowing for a second and a half, then a needle of sunlight.</summary>
    SolarBrand,
    /// <summary>A burning boulder dropping in, then rolling on along facing, leaving fire behind.</summary>
    RollingCinderstone,
    /// <summary>A broad pale wave of force rolling out along facing, kicking up dust.</summary>
    ConcussiveWave,
    /// <summary>A rampart of ice blocks thrusting up across the way in front.</summary>
    RimeRampart,
    /// <summary>A whirling column of steel round the caster, sparks flung off it (follows them).</summary>
    SteelCyclone,
    /// <summary>A spectral totem rising out of the ground, green pulses rolling out of it.</summary>
    MendingTotem,
    /// <summary>A string of blinks between points round the spot, a slash and a shade at each.</summary>
    ThousandSteps,
    /// <summary>A dash along facing with a trail of shades and clock marks at both ends.</summary>
    RewindStep,
    /// <summary>A flash, then a clock ring standing still round the spot.</summary>
    StilledMoment,
    /// <summary>A wide dome over a clock face whose hands turn, motes stopped mid-air inside.</summary>
    StoppedHour,
    /// <summary>A ridge of rock tearing up along facing, standing, then crumbling.</summary>
    RivenRidge,
    /// <summary>A quick ring of dust and cracks at the feet.</summary>
    TremorRing,
    /// <summary>A slam, then echoes racing out to points round it, each ringing out again.</summary>
    ResoundingSlam,
    /// <summary>A slow pastel orb drifting along facing on a long glittering trail.</summary>
    DriftingMote,
    /// <summary>A blink along facing in pastel sparkles, a hush ring at each end.</summary>
    FeyRift,
    /// <summary>A coil at the spot, tethered to points round it, the tethers snapping at the end.</summary>
    Dreamtether,
    /// <summary>A line of stone spikes erupting one after another along facing, growing as they go.</summary>
    Thornline,
    /// <summary>A puff of pastel smoke and sparkles.</summary>
    PolymorphPuff,
    /// <summary>A forked dark-green beam from the hand, spectral claws closing where it lands.</summary>
    SunderingFinger,
    /// <summary>A swooping dive out along facing and back, leaving a ribbon of fire.</summary>
    FirewingDive,
    /// <summary>Four fire wisps orbiting the caster, launched one by one along facing.</summary>
    EmberWisps,
    /// <summary>A long held beam of sunlight along facing, sweeping a little as it burns.</summary>
    SolarLance,
    /// <summary>A sun swelling over the spot, rings turning round it, then a great fire blast.</summary>
    RebornStar,
    /// <summary>A thrust of wind along facing that sends a tornado on.</summary>
    GaleThrust,
    /// <summary>A curved wall of streaming wind standing in front.</summary>
    GaleBarrier,
    /// <summary>A short dash along facing on a ribbon of wind.</summary>
    GaleStep,
    /// <summary>A gust lifting from the spot, a flurry of slashes overhead, a cut down to finish.</summary>
    SkywardCut,
    /// <summary>An orb thrown along facing and back again, blue going out and white coming back.</summary>
    ReturningOrb,
    /// <summary>Three wisps orbiting the caster, then darting off ahead (follows them).</summary>
    SpiritWisps,
    /// <summary>A rose-pink orb with a ring glyph sent along facing, bursting where it lands.</summary>
    BeguilingMote,
    /// <summary>Three short dashes along facing, bolts fired from each stop.</summary>
    SpiritSteps,
    /// <summary>An orb of light along facing, then rings of light closing round where it lands.</summary>
    BindOfDawn,
    /// <summary>A spinning prism thrown along facing and back, a shield flashing at its turn.</summary>
    PrismWard,
    /// <summary>A star thrown along facing, glowing in a pulsing ring, then bursting.</summary>
    LingeringStar,
    /// <summary>A thin aiming line along facing, then a huge beam of white-gold light.</summary>
    RadiantLance,
    /// <summary>A charge at the hand, then a crackling cyan line shot along facing.</summary>
    ShockLine,
    /// <summary>Three traps tossed along facing, snapping shut in bursts of fire.</summary>
    SnapTraps,
    /// <summary>A rocket flying along facing on smoke and fire, landing in a great blast.</summary>
    GrandRocket,
    /// <summary>A ghostly green chain hurled along facing, hooking and pulled back.</summary>
    ReapingHook,
    /// <summary>A lantern tossed along facing, landing aglow.</summary>
    LanternToss,
    /// <summary>A green lash arcing across in front.</summary>
    ChainSweep,
    /// <summary>A pentagon of spectral green walls standing round the spot.</summary>
    SpectralCage,
    /// <summary>A quick gold bolt along facing.</summary>
    ArcaneBolt,
    /// <summary>A blink along facing, then a bolt fired on from where it lands.</summary>
    ShiftShot,
    /// <summary>A charge, then a wide band of golden light rolling a long way along facing.</summary>
    LongshotBarrage,
    /// <summary>A charge along facing in the form of a rolling boulder, erupting into stone where it ends.</summary>
    LandslideCharge,
    /// <summary>A burst of fire in a cone in front, the wedge left scorched.</summary>
    FlareFan,
    /// <summary>Ribbons of fire swirling round the caster (follows them).</summary>
    EmberShroud,
    /// <summary>Something heavy falling from the sky into a fire blast over a rune circle.</summary>
    GuardiansFall,
    /// <summary>A giant spectral sword falling into the spot, a pillar of gold where it lands.</summary>
    SwordOfVerdict,
    /// <summary>A dark orb along facing, then dark chains wrapping where it lands.</summary>
    UmbralBind,
    /// <summary>A dark violet pool, spirits drifting over it.</summary>
    BlightPool,
    /// <summary>A dark dome of runes round the caster (follows them).</summary>
    UmbralWard,
    /// <summary>Dark tethers to points round the spot, brightening, then snapping in bursts.</summary>
    SoulFetters,
    /// <summary>A pale crescent swept from overhead down into the ground in front, cutting a line into it.</summary>
    MoonfallCut,
    /// <summary>Spectral swords falling one after another round the spot over a rune circle, each stabbing in.</summary>
    BladeRain,
    /// <summary>Two rings of steel and gold spun out at waist height round the caster (follows them).</summary>
    HaloCut,
    /// <summary>A swirling pit of sand, streams of grit spiralling down into it.</summary>
    SandVortex,
    /// <summary>A warning ring, then a great crag thrusting up out of it, stone spikes round it.</summary>
    Monolith,
    /// <summary>A cone of rocks flung out in front, each landing in a puff of dust.</summary>
    StoneshotFan,
    /// <summary>A warning ring, then a burning meteor plunging from the sky into a great fire blast.</summary>
    SkyfireMeteor,
    /// <summary>A serpent of fire slithering along facing on a ribbon of flame, rearing up in a burst at the end.</summary>
    FlameSerpent,
    /// <summary>Three fireballs fanned out along facing, each bursting where it lands.</summary>
    EmberVolley,
    /// <summary>A flask lobbed along facing that shatters into a spreading pool of fire.</summary>
    NapalmFlask,
    /// <summary>A volley of icicles shot along facing, shattering where they strike.</summary>
    IcicleBarrage,
    /// <summary>A great ice crystal plunging from the sky, ice spikes and shards bursting out where it lands.</summary>
    GlacialComet,
    /// <summary>Ice spikes erupting outward along three spiral arms over a swirl of frost.</summary>
    PermafrostSpiral,
    /// <summary>A charge, then a lance of lightning thrown along facing, the bolt leaping on to three more.</summary>
    StormLance,
    /// <summary>An orb of lightning on a stalk over a rune circle, zapping the ground round it.</summary>
    TeslaCoil,
    /// <summary>Four tornadoes circling the caster, lightning leaping between them (follows them).</summary>
    EyeOfTheStorm,
    /// <summary>A wall of water rising and rolling along facing, crashing down in spray at the end.</summary>
    TidalWave,
    /// <summary>Bubbling in a ring, then a spout of water bursting high, spray raining down.</summary>
    Geyser,
    /// <summary>A swirling whirlpool, rings and streams of foam drawn into its middle.</summary>
    Whirlpool,
    /// <summary>A wobbling bubble of water round the spot, popping in a splash.</summary>
    BubblePrison,
    /// <summary>Petals swirling up round the spot over a rune circle, then bursting outward.</summary>
    PetalStorm,
    /// <summary>Roots creeping out across the ground to points round the spot, coiling up where they reach.</summary>
    CreepingRoots,
    /// <summary>A volley of thorns lobbed up and raining down into the area in front, left stuck in the ground.</summary>
    ThornVolley,
    /// <summary>A flash, then four shimmering copies of the caster sliding out round them and shattering (follows them).</summary>
    MirrorImages,
    /// <summary>Three upright rune circles over the caster, each firing a beam into one spot in front.</summary>
    AstralSigils,
    /// <summary>A tear in the air in front, motes drawn into it, snapping shut in a blast.</summary>
    RiftTear,
    /// <summary>A beam of light on the caster, a halo over their head and feathers drifting down (follows them).</summary>
    Benediction,
    /// <summary>Six pillars of light round the spot joined into a hexagram, then a flash of light.</summary>
    SealOfLight,
    /// <summary>Dark tendrils rising out of a pool of shadow in front, lashing.</summary>
    ShadowTendrils,
    /// <summary>A sweep of a green scythe round the caster, then souls drawn in from all round.</summary>
    SoulHarvest,
    /// <summary>Five ghosts rising out of seals in a row and gliding forward to cut at what is ahead.</summary>
    GhostLegion,
    /// <summary>Three burning skulls bursting from the caster, hanging a moment, then diving screaming at foes ahead.</summary>
    RagingSpirits,
    /// <summary>A raging aura of spirits: ghostly fire round the caster, howling faces whirling up it under a skull (follows them).</summary>
    WrathAura,
    /// <summary>Five green skulls fired one after another down the line, weaving, each bursting.</summary>
    SkullBarrage,
    /// <summary>Three graves cracking open in front, a ghost standing up out of each in a pillar of light.</summary>
    GraveCall,
    /// <summary>Four ghostly skulls circling the caster with fire streaming off them, flung outward at the end (follows them).</summary>
    SpiritGuard,
    /// <summary>A rank of ghosts sweeping forward together through a veil of mist.</summary>
    PhantomHost,
    /// <summary>A flock of spectral crows bursting from the caster in a cone, feathers falling behind them.</summary>
    CrowMurder,
    /// <summary>A haunted circle: a skull mark in a ring of filigree, ghosts pacing round it, wisps rising.</summary>
    HauntedGround,
    /// <summary>A great wraith rearing up and screaming: rings of sound rolling out along a cone.</summary>
    BansheeWail,
    /// <summary>A ghost rising, rushing at a spot ahead as it swells and flickers, and blowing up in a skull of light.</summary>
    SoulBomber,
    /// <summary>A skull mark drawn in front, then a huge skull heaving up out of it and snapping shut.</summary>
    DeathsJaw,
    /// <summary>A towering shade looming behind the caster and sweeping a scythe round, souls drawn up to it.</summary>
    ReaperShade,
    /// <summary>Skulls falling one after another onto a skull mark, each trailing fire and cracking the ground.</summary>
    SkullRain,
    /// <summary>A wraith flying into a foe ahead, who then burns with ghostly fire under a staring eye.</summary>
    Possession,
    /// <summary>A gate of filigree and shadow standing open at the caster, skulls and wraiths pouring out of it.</summary>
    NetherGate,
    /// <summary>Six quick pink and white cuts over a lotus drawn on the ground, petals flying, then the lotus flaring.</summary>
    LotusFlurry,
    /// <summary>A wide golden crescent that leaves stars hanging along its arc over a field of them.</summary>
    StarlitCrescent,
    /// <summary>Three turning rings of violet and rose round the caster, butterflies spiralling up out of them (follows them).</summary>
    ButterflyWaltz,
    /// <summary>Two pale crescents crossed in front under a sickle moon, a small moon left where each ends.</summary>
    TwinMoons,
    /// <summary>Nine golden feathers thrown in a fan, each sticking in the ground where it lands.</summary>
    FeatherFan,
    /// <summary>One hairline cut across everything in front, a beat of stillness, then blossom bursting all along it.</summary>
    IaidoBloom,
    /// <summary>Three silk ribbons of rose, violet and gold winding round the caster, sparkling (follows them).</summary>
    RibbonDance,
    /// <summary>Six crescents in the colours of the rainbow fanned one after another in front.</summary>
    PrismaticEdge,
    /// <summary>A thrust of steel light, gems flashing along it and rings opening round it, bursting at its tip.</summary>
    DiamondLance,
    /// <summary>Petals drifting down over a great lotus on the ground, ripples where they settle.</summary>
    BlossomRain,
    /// <summary>A stream of butterflies in four colours flying out at what is ahead, leaving sparkles.</summary>
    ButterflySwarm,
    /// <summary>A ring of small lights, each a flame over a tiny lotus, fairies looping round it trailing dust.</summary>
    FairyRing,
    /// <summary>Leaves of green and gold spiralling up round the caster in the wind (follows them).</summary>
    LeafCyclone,
    /// <summary>Three wisps circling the caster, each firing motes at a marked spot in front.</summary>
    WispCompanions,
    /// <summary>A puff of dandelion seeds, each a tiny star on a hair, floating up and away on the wind.</summary>
    DandelionDrift,
    /// <summary>A crown of thorns thrusting up and outward round the spot, rose petals thrown from it.</summary>
    ThornCrown,
    /// <summary>A giant flower opening on the ground: two rings of petals round a heart of light, pollen rising.</summary>
    SpiritBloom,
    /// <summary>A seed lobbed at a spot, vines winding out from it, a flower opening at the end of each.</summary>
    BloomBurst,
    /// <summary>A halo of filigree turning as it comes down out of the sky onto a spot, landing in a pillar of light.</summary>
    HaloDescent,
    /// <summary>A whirl of golden feathers round the spot, glints striking through it.</summary>
    FeatherStorm,
    /// <summary>Shafts of dawn light slanting down one after another onto a great sun on the ground.</summary>
    DawnRays,
    /// <summary>A golden lotus on the ground, smaller ones rising above it through a pillar of light.</summary>
    SacredLotus,
    /// <summary>A star falling onto an eight-pointed star drawn on the ground, breaking into eight.</summary>
    StarOfGrace,
    /// <summary>A dozen lanterns lifting off the ground one after another and floating up.</summary>
    LanternsOfHope,
    /// <summary>Halos lifting off the ground round the caster one after another, widening as they rise (follows them).</summary>
    ChoirOfRings,
    /// <summary>A tall winged spirit of light standing behind the caster under a halo, a shell of light round them (follows them).</summary>
    GuardianSpirit,
    /// <summary>Hearts rising round the caster under one large beating heart (follows them).</summary>
    HeartSurge,
    /// <summary>Five stars circling up round the caster over a field of stars, more falling (follows them).</summary>
    StarBlessing,
    /// <summary>Mint butterflies spiralling up round the caster (follows them).</summary>
    ButterflyRenewal,
    /// <summary>A sickle moon over the caster, a pale veil round them and a corona at their feet (follows them).</summary>
    MoonlitVeil,
    /// <summary>Six cut gems circling the caster inside a dome of hexagons (follows them).</summary>
    CrystalAegis,
    /// <summary>A phoenix rising out of an eruption: wings of flame opening as it climbs, feathers falling.</summary>
    PhoenixRise,
    /// <summary>A small sun swelling in a flickering corona and bursting in rays.</summary>
    SunFlare,
    /// <summary>Nine candle flames lit one after another in a ring, all flaring up into pillars together.</summary>
    CandleVigil,
    /// <summary>A swarm of burning butterflies flying ahead, each going out in a small pop of flame.</summary>
    EmberButterflies,
    /// <summary>Five rockets climbing and bursting into balls of coloured stars.</summary>
    FireworkBloom,
    /// <summary>A lotus of molten petals opening on the ground, then erupting from every petal tip.</summary>
    LavaLotus,
    /// <summary>Three small comets circling the caster, fire streaming off them (follows them).</summary>
    CometDance,
    /// <summary>Two ribbons of fire twisting round each other down a line through opening rings, bursting at the end.</summary>
    DragonSpiral,
    /// <summary>Glowing cinders drifting down over a smouldering circle, small flames catching where they land.</summary>
    CinderFall,
    /// <summary>Snowflakes falling over a great snowflake drawn on the ground, frost glinting.</summary>
    SnowfallSanctum,
    /// <summary>A lotus of frost drawn on the ground, ice thrusting up along every petal.</summary>
    FrostLotus,
    /// <summary>Icicles growing down from a ring in the air, glinting, then dropping and shattering.</summary>
    ChandelierDrop,
    /// <summary>Three curtains of aurora waving across in green, violet and mint.</summary>
    AuroraVeil,
    /// <summary>A glittering cloud of ice dust, gems and snowflakes turning round the caster (follows them).</summary>
    DiamondDust,
    /// <summary>Six mirrors of ice round a spot, light thrown between them and then through the middle.</summary>
    MirrorShards,
    /// <summary>Two ribbons of snow winding round the caster, each led by a snowflake (follows them).</summary>
    WinterWaltz,
    /// <summary>A huge snowflake forming in the air, then dropping flat onto the spot and bursting into ice.</summary>
    FrozenStar,
    /// <summary>A heart of ice beating inside a dome of hexagons, gems circling it, then bursting.</summary>
    FrostHeart,
    /// <summary>Stars falling one after another onto marked spots across a field of stars.</summary>
    StarCall,
    /// <summary>Two tilted rings of lightning turning round the caster, bolts leaping out from them (follows them).</summary>
    ThunderHalo,
    /// <summary>A honeycomb of light with seven glowing nodes, lightning jumping between them.</summary>
    VoltLattice,
    /// <summary>Wings of storm light unfurling behind the caster, bolts striking down from their tips (follows them).</summary>
    StormWings,
    /// <summary>A ball of plasma swelling over a lotus, arcs reaching to every petal, then bursting.</summary>
    PlasmaBloom,
    /// <summary>Three balls of lightning rolling forward side by side, arcs jumping between them.</summary>
    BallLightningTrio,
    /// <summary>Four ribbons of wind winding up round the caster, leaves riding on them (follows them).</summary>
    ZephyrRibbons,
    /// <summary>Six bolts striking round a hexagram one after another, then one great bolt in the middle.</summary>
    TempestCrown,
    /// <summary>A pond of shifting light with lotuses opening on it, ripples spreading.</summary>
    LotusPond,
    /// <summary>Slanting rain over a circle, a ripple where each drop lands.</summary>
    RainDance,
    /// <summary>Two koi of light swimming a circle round the caster, ripples in their wake (follows them).</summary>
    KoiStream,
    /// <summary>Seven pearls lobbed ahead in bubbles, each bursting in a splash and a ring.</summary>
    PearlBarrage,
    /// <summary>A bank of mist rolling slowly over a circle, glints in it.</summary>
    MistVeil,
    /// <summary>A sickle moon overhead drawing the tide in to the middle, then a wave rolling back out.</summary>
    MoonTide,
    /// <summary>A stream of bubbles blown ahead, each popping in a glint.</summary>
    BubbleStream,
    /// <summary>A fountain: a spout in the middle, eight arcs of water falling from it to the rim.</summary>
    FountainOfLife,
    /// <summary>Violet crystals growing out of the ground, the tallest in the middle, glinting.</summary>
    CrystalGarden,
    /// <summary>A boulder shaking as light breaks through it, then splitting in a burst of cut gems.</summary>
    GeodeBurst,
    /// <summary>Five pillars of jade rising in a ring, tied to each other by threads of light.</summary>
    JadePillars,
    /// <summary>A seam of gold running ahead through the ground, light and coins thrown up along it.</summary>
    GoldenFissure,
    /// <summary>A dune of sand rolling forward, grit flying from its crest.</summary>
    DuneRoller,
    /// <summary>Eight stones circling the caster at three heights, flung outward at the end (follows them).</summary>
    PebbleOrbit,
    /// <summary>Drops of resin flying in and setting into a block of amber round a spot, then cracking apart.</summary>
    AmberTomb,
    /// <summary>A constellation lit star by star over the ground and joined by threads, then every star falling.</summary>
    StarMap,
    /// <summary>Clock faces spinning backwards under and behind the caster, echoes of them stepping back in (follows them).</summary>
    ClockworkRewind,
    /// <summary>A white ray through a gem, split into six colours braiding down the line.</summary>
    PrismRay,
    /// <summary>Rings of bent light closing on a dark heart, everything spiralling in, then flung back out.</summary>
    GravityLens,
    /// <summary>A great eye opening overhead, its gaze sweeping the circle and marking what it finds.</summary>
    EyeOfInsight,
    /// <summary>A blown heart flying down the line, leaving the one it reaches under a turning heart.</summary>
    CharmHeart,
    /// <summary>A tower of rune circles stacking upward, each smaller, then a beam down through all of them.</summary>
    RuneCascade,
    /// <summary>Clouds of coloured gas and stars drawing together round a point, then a nova.</summary>
    NebulaBurst,
    /// <summary>A moon sliding across a sun overhead, darkness, then light bursting back from its edge.</summary>
    UmbralEclipse,
    /// <summary>A dial ticking round a spot, a white standstill, then everything breaking like glass.</summary>
    TimeShatter,
    /// <summary>Stars lifting off the ground and gathering overhead, then bursting and raining down as light.</summary>
    WishingStars,
    /// <summary>Great wings of light unfurling behind the caster under a halo, feathers falling (follows them).</summary>
    WingsOfDawn,
}

/// <summary>
/// Skills: each a short script of pieces (crescents, ground marks, columns, spheres and sparks
/// drawn by fx_crescent, fx_disc, fx_column, fx_energy, fx_void, fx_streak, fx_mark and fx_glyph) under one root
/// node standing where it is cast and facing the way it goes, with cues that call the library's
/// other effects (blasts, patches, spikes, lightning) as it unfolds. Each piece shows over its
/// own window of the skill's life and is posed every frame from how far through that it is.
/// </summary>
internal sealed partial class FxSkills(FxKit kit, FxWorld world)
{
    private FxKit Kit => kit;
    private FxWorld Stage => world;
    private const float Gravity = 14f;
    private const float Lift = FxKit.GroundLift;

    /// <summary>A skill's colours: its body, its white-hot core and its deep shade.</summary>
    private readonly record struct Tone(Color Main, Color Hot, Color Deep);

    private static readonly Tone Crimson = new(new(0.92f, 0.04f, 0.07f), new(1f, 0.5f, 0.42f), new(0.38f, 0f, 0.03f));
    private static readonly Tone Blood = new(new(0.75f, 0.02f, 0.04f), new(1f, 0.35f, 0.3f), new(0.25f, 0f, 0.01f));
    private static readonly Tone Azure = new(new(0.25f, 0.65f, 1f), new(0.9f, 0.97f, 1f), new(0.04f, 0.15f, 0.5f));
    private static readonly Tone Sun = new(new(1f, 0.6f, 0.12f), new(1f, 0.95f, 0.72f), new(0.5f, 0.14f, 0f));
    private static readonly Tone Steel = new(new(0.68f, 0.8f, 1f), new(1f, 1f, 1f), new(0.18f, 0.26f, 0.5f));
    private static readonly Tone Phantom = new(new(0.62f, 0.35f, 1f), new(0.95f, 0.86f, 1f), new(0.2f, 0.05f, 0.45f));
    private static readonly Tone Dust = new(new(0.85f, 0.64f, 0.38f), new(1f, 0.92f, 0.75f), new(0.32f, 0.2f, 0.09f));
    private static readonly Tone Rubble = new(new(0.5f, 0.36f, 0.22f), new(0.85f, 0.7f, 0.5f), new(0.2f, 0.12f, 0.05f));
    private static readonly Tone Arcane = new(new(0.8f, 0.3f, 1f), new(1f, 0.86f, 1f), new(0.3f, 0.04f, 0.5f));
    private static readonly Tone Void = new(new(0.62f, 0.3f, 1f), new(0.92f, 0.78f, 1f), new(0.03f, 0f, 0.07f));
    private static readonly Tone Chrono = new(new(0.55f, 0.62f, 1f), new(1f, 0.9f, 0.55f), new(0.1f, 0.12f, 0.42f));
    private static readonly Tone Ice = new(new(0.55f, 0.85f, 1f), new(0.92f, 0.98f, 1f), new(0.08f, 0.28f, 0.6f));
    private static readonly Tone Flame = new(new(1f, 0.42f, 0.08f), new(1f, 0.9f, 0.55f), new(0.45f, 0.05f, 0f));
    private static readonly Tone Gold = new(new(1f, 0.78f, 0.3f), new(1f, 0.97f, 0.82f), new(0.5f, 0.28f, 0.04f));
    private static readonly Tone Storm = new(new(0.45f, 0.65f, 1f), new(0.92f, 0.96f, 1f), new(0.08f, 0.14f, 0.5f));
    private static readonly Tone Toxic = new(new(0.42f, 0.95f, 0.2f), new(0.86f, 1f, 0.6f), new(0.07f, 0.3f, 0.02f));
    private static readonly Tone Verdant = new(new(0.45f, 1f, 0.55f), new(0.95f, 1f, 0.82f), new(0.04f, 0.35f, 0.14f));
    private static readonly Tone Tide = new(new(0.3f, 0.65f, 1f), new(0.86f, 0.95f, 1f), new(0.04f, 0.14f, 0.45f));
    private static readonly Tone Rage = new(new(1f, 0.16f, 0.08f), new(1f, 0.55f, 0.35f), new(0.4f, 0f, 0f));
    private static readonly Tone Crit = new(new(1f, 0.5f, 0.12f), new(1f, 0.97f, 0.82f), new(0.5f, 0.07f, 0f));

    // fx_disc, fx_column, fx_energy and fx_streak styles.
    private const int Shock = 0, Runes = 1, Cracks = 2, Glow = 3, Swirl = 4, Telegraph = 5, Cone = 6, Band = 7;
    private const int Light = 0, Fire = 1, Spiral = 2, Beam = 3;
    private const int Orb = 0, Hexes = 1, Shell = 2;
    private const int Streak = 0, Mote = 1, Star = 2;
    private const float ShockFront = 0.8f;

    /// <summary>Poses a piece <paramref name="k"/> (0..1) through its window, <paramref name="t"/> seconds in.</summary>
    private delegate void Pose(GeometryInstance3D node, float k, float t);

    private sealed class Piece
    {
        public required GeometryInstance3D Node;
        public required Pose Pose;
        public float Start;
        public float Life;
    }

    private sealed class Cast : FxItem
    {
        public required Node3D Root;
        public readonly List<Piece> Pieces = [];
        public readonly List<(float At, Action Do)> Cues = [];
        public readonly List<Action<float>> Each = [];
        public readonly List<Action> OnEnd = [];
        public int NextCue;
        public Node3D? Follow;
    }

    private readonly List<Cast> _casts = [];
    private readonly Dictionary<string, ShaderMaterial> _materials = [];
    private readonly Dictionary<int, ArrayMesh> _fans = [];
    private readonly CylinderMesh _tube = new() { TopRadius = 1f, BottomRadius = 1f, Height = 1f, RadialSegments = 32, Rings = 4, CapTop = false, CapBottom = false };
    private readonly CapsuleMesh _ghost = new() { Radius = 0.32f, Height = 1.7f, RadialSegments = 16, Rings = 6 };
    // A boulder: a sphere cut coarse enough that fx_crystal's facets read as rock.
    private readonly SphereMesh _rock = new() { Radius = 1f, Height = 2f, RadialSegments = 7, Rings = 4 };
    private readonly ShaderMaterial _stone = FxKit.Crystal(new Color(0.44f, 0.38f, 0.32f), new Color(0.7f, 0.6f, 0.48f), rim: 0.25f, glow: 0f);

    public FxHandle Spawn(FxSkill skill, Vector3 at, Vector3 facing, float radius, Node3D? follow)
    {
        var flat = FxKit.Flat(facing);
        var reach = flat.Length();
        var forward = reach > 1e-4f ? flat / reach : Vector3.Back;
        var root = new Node3D { Transform = new Transform3D(new Basis(Vector3.Up, Mathf.Atan2(forward.X, forward.Z)), FxKit.Flat(at)) };
        kit.Parent.AddChild(root);
        var cast = new Cast { Root = root, Follow = follow };
        radius = Mathf.Max(radius, 0.2f);
        // How far the skills that go somewhere go: facing's length, or 3 radii for a bare direction.
        var range = reach > 1f ? reach : radius * 3f;
        switch (skill)
        {
            case FxSkill.CrimsonSlash: CrimsonSlash(cast, radius); break;
            case FxSkill.CrossSlash: CrossSlash(cast, radius); break;
            case FxSkill.RisingCleave: RisingCleave(cast, radius); break;
            case FxSkill.Whirlwind: Whirlwind(cast, radius); break;
            case FxSkill.PhantomDash: PhantomDash(cast, radius, reach); break;
            case FxSkill.Shockwave: Shockwave(cast, radius); break;
            case FxSkill.ArcaneBurst: ArcaneBurst(cast, radius); break;
            case FxSkill.Singularity: Singularity(cast, radius); break;
            case FxSkill.StasisDome: StasisDome(cast, radius); break;
            case FxSkill.GlacialBurst: GlacialBurst(cast, radius); break;
            case FxSkill.FlamePillar: FlamePillar(cast, radius); break;
            case FxSkill.Judgment: Judgment(cast, radius); break;
            case FxSkill.ThunderCall: ThunderCall(cast, radius); break;
            case FxSkill.ToxicNova: ToxicNova(cast, radius); break;
            case FxSkill.ShadowBlink: ShadowBlink(cast, reach); break;
            case FxSkill.Sanctuary: Sanctuary(cast, radius); break;
            case FxSkill.LevelUp: LevelUp(cast, radius); break;
            case FxSkill.Recall: Recall(cast, radius); break;
            case FxSkill.Bloodrage: Bloodrage(cast, radius); break;
            case FxSkill.CriticalStrike: CriticalStrike(cast, radius); break;
            default: More(skill, cast, radius, range); break;
        }
        cast.Cues.Sort((a, b) => a.At.CompareTo(b.At));
        var life = 0f;
        foreach (var piece in cast.Pieces)
            life = Mathf.Max(life, piece.Start + piece.Life);
        foreach (var cue in cast.Cues)
            life = Mathf.Max(life, cue.At + 0.01f);
        cast.Begin(life);
        _casts.Add(cast);
        Tick(cast, 0f);
        return new FxHandle(cast);
    }

    public void Tick(float dt)
    {
        for (var i = _casts.Count - 1; i >= 0; i--)
        {
            var cast = _casts[i];
            if (!Tick(cast, dt))
            {
                cast.Active = false;
                foreach (var end in cast.OnEnd)
                    end();
                cast.Root.QueueFree();
                _casts.RemoveAt(i);
            }
        }
    }

    private static bool Tick(Cast cast, float dt)
    {
        cast.Age += dt;
        if (!cast.Active || cast.Age >= cast.Life)
            return false;
        if (cast.Follow != null)
        {
            if (GodotObject.IsInstanceValid(cast.Follow))
                cast.Root.Position = FxKit.Flat(cast.Follow.Position);
            else
                cast.Follow = null;
        }
        while (cast.NextCue < cast.Cues.Count && cast.Cues[cast.NextCue].At <= cast.Age)
            cast.Cues[cast.NextCue++].Do();
        foreach (var piece in cast.Pieces)
        {
            var t = cast.Age - piece.Start;
            var on = t >= 0f && t < piece.Life;
            piece.Node.Visible = on;
            if (on)
                piece.Pose(piece.Node, t / piece.Life, t);
        }
        foreach (var each in cast.Each)
            each(cast.Age);
        return true;
    }

    // ---- The skills. Each is built in its root's space: +Z the way it faces, Y up, the ground at 0.

    private void CrimsonSlash(Cast c, float r)
    {
        var swing = new Transform3D(new Basis(Vector3.Forward, Mathf.DegToRad(-14f)), new Vector3(0f, 0.95f, -r * 0.15f));
        var inner = new Transform3D(new Basis(Vector3.Up, Mathf.DegToRad(8f)) * swing.Basis, swing.Origin + Vector3.Up * 0.12f);
        Crescent(c, 0f, 0.5f, swing, r * 1.12f, 170f, Blood, thick: 0.16f, sweep: 0.2f, gain: 0.7f);
        Crescent(c, 0f, 0.42f, swing, r, 160f, Crimson, thick: 0.5f, sweep: 0.22f, gain: 1.3f);
        Crescent(c, 0.03f, 0.36f, inner, r * 0.78f, 140f, Crimson with { Main = new Color(1f, 0.28f, 0.16f) }, thick: 0.28f, sweep: 0.26f);
        const int Drops = 18;
        for (var i = 0; i < Drops; i++)
        {
            var k = (i + Rand(0f, 1f)) / Drops;
            var angle = Mathf.DegToRad(80f - 160f * k);
            var way = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            var from = swing * (way * r * Rand(0.75f, 1f));
            var velocity = way * Rand(3f, 7f) + new Vector3(-way.Z, 0f, way.X) * Rand(1f, 3f) + Vector3.Up * Rand(1f, 4f);
            Spark(c, 0.42f * 0.22f * k, Rand(0.35f, 0.6f), Blood, from, velocity, Gravity, Rand(0.06f, 0.11f), 0.05f, 1.4f);
        }
        Flare(c, 0.05f, 0.2f, Star, Crimson, new Vector3(0f, 0.95f, r * 0.8f), 2.4f);
        Disc(c, 0.04f, 0.6f, Glow, Crimson, new Vector3(0f, 0f, r * 0.45f), k => r * 0.75f, k => Fade(k, 0.1f, 0.7f) * 0.5f);
        Shine(c, new Vector3(0f, 1f, r * 0.6f), r * 1.5f, Crimson.Main, age => 1.2f * Mathf.Max(0f, 1f - age / 0.4f));
    }

    private void CrossSlash(Cast c, float r)
    {
        // Two diagonal swings round the same centre at the chest, one tilted each way about the
        // facing: arcs of two circles sharing that centre, so they cross straight in front. Each
        // starts high on its side and cuts down across the other.
        var chest = Vector3.Up * 1.3f;
        var reach = r * 0.85f;
        var cross = chest + Vector3.Back * reach;
        for (var i = 0; i < 2; i++)
        {
            var basis = i == 0
                ? new Basis(Vector3.Back, Mathf.DegToRad(45f))
                : new Basis(Vector3.Back, Mathf.DegToRad(-45f)) * Basis.FromScale(new Vector3(-1f, 1f, 1f));
            Crescent(c, i * 0.12f, 0.4f, new Transform3D(basis, chest), reach, 130f, Azure, thick: 0.3f, sweep: 0.28f, gain: 1.3f);
        }
        Flare(c, 0.17f, 0.26f, Star, Azure, cross, 3f);
        Flare(c, 0.03f, 0.14f, Mote, Azure, cross, 1.2f);
        Disc(c, 0.17f, 0.3f, Shock, Azure, cross, k => 0.5f + r * 0.35f * Out(k), k => (1f - k) * 0.6f, basis: new Basis(Vector3.Right, Mathf.Pi / 2f), band: 0.18f);
        for (var i = 0; i < 16; i++)
        {
            var way = new Vector3(Rand(-1f, 1f), Rand(-0.6f, 1f), Rand(-0.2f, 0.6f)).Normalized();
            Spark(c, 0.17f, Rand(0.2f, 0.35f), Azure, cross, way * Rand(8f, 14f), Gravity * 0.4f, 0.07f, 0.03f);
        }
        Shine(c, cross, r * 1.5f, Azure.Main, age => 1.2f * Bump(age, 0.18f, 0.3f));
    }

    private void RisingCleave(Cast c, float r)
    {
        // Upright along the facing: it starts low in front and sweeps up and over.
        var swing = new Transform3D(new Basis(Vector3.Back, -Mathf.Pi / 2f), new Vector3(0f, 0.3f, r * 0.15f));
        Crescent(c, 0f, 0.45f, swing, r * 0.9f, 150f, Sun, thick: 0.45f, sweep: 0.25f, gain: 1.3f);
        Crescent(c, 0.02f, 0.38f, swing, r * 0.7f, 130f, Sun with { Main = new Color(1f, 0.85f, 0.35f) }, thick: 0.25f, sweep: 0.28f);
        var front = new Vector3(0f, 0f, r * 0.6f);
        Disc(c, 0.03f, 1.2f, Cracks, Sun, front, k => r * 0.55f, k => Fade(k, 0.05f, 0.5f), k => k);
        Column(c, 0.04f, 0.35f, Light, Sun, front, k => new Vector2(0.4f * (1f + k), 3.5f), k => (1f - k) * (1f - k), gain: 1.5f);
        for (var i = 0; i < 16; i++)
        {
            var from = front + new Vector3(Rand(-0.5f, 0.5f), 0.1f, Rand(-r * 0.4f, r * 0.3f));
            Spark(c, Rand(0.03f, 0.12f), Rand(0.4f, 0.65f), Sun, from, new Vector3(Rand(-1.5f, 1.5f), Rand(6f, 10f), Rand(0.5f, 3f)), Gravity, 0.07f, 0.04f);
        }
        Flare(c, 0.05f, 0.22f, Star, Sun, new Vector3(0f, 1.6f, r * 0.55f), 2.4f);
        Shine(c, front + Vector3.Up, r * 1.4f, Sun.Main, age => 1.3f * Bump(age, 0.06f, 0.4f));
    }

    private void Whirlwind(Cast c, float r)
    {
        const float Spins = 8, Every = 0.2f;
        for (var i = 0; i < Spins; i++)
        {
            var tilt = new Basis(Vector3.Right, Mathf.DegToRad(i % 2 == 0 ? 7f : -7f)) * new Basis(Vector3.Up, i * 2.4f);
            Crescent(c, i * Every, 0.32f, new Transform3D(tilt, Vector3.Up * (0.8f + 0.12f * (i % 3))), r, 360f, Steel, thick: 0.32f, sweep: 0.5f, gain: 1.1f);
        }
        var life = Spins * Every + 0.15f;
        Disc(c, 0f, life, Swirl, Dust, Vector3.Zero, k => r * 1.05f, k => Fade(k, 0.08f, 0.15f) * 0.75f, spin: -6f);
        Disc(c, 0f, life, Glow, Steel, Vector3.Zero, k => r * 0.9f, k => Fade(k, 0.08f, 0.15f) * 0.3f);
        for (var i = 0; i < 26; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var way = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            var spin = new Vector3(way.Z, 0f, -way.X);
            Spark(c, Rand(0f, life - 0.4f), 0.4f, Dust, way * r * 0.9f + Vector3.Up * 0.15f, spin * 5f + way * 2f + Vector3.Up * 1.5f, Gravity * 0.3f, 0.12f, 0.06f, 0.8f);
        }
        Shine(c, Vector3.Up, r * 1.4f, Steel.Main, age => 0.6f * Fade(age / life, 0.05f, 0.2f));
    }

    private void PhantomDash(Cast c, float width, float length)
    {
        const float Arrive = 0.18f;
        length = Mathf.Max(length, 1f);
        var w = Mathf.Max(width, 0.6f);
        var end = new Vector3(0f, 0f, length);
        Disc(c, 0f, 0.35f, Shock, Phantom, Vector3.Zero, k => w * 1.3f * Out(k), k => 1f - k, band: 0.2f);
        Disc(c, 0f, 0.7f, Glow, Phantom, new Vector3(0f, 0f, length / 2f), k => 1f, k => Fade(k, 0.15f, 0.6f) * 0.8f, stretch: new Vector3(w * 0.45f, 1f, length / 2f));
        for (var i = 0; i < 16; i++)
        {
            var at = new Vector3(Rand(-w / 2f, w / 2f), Rand(0.3f, 1.8f), Rand(0.5f, length));
            var len = Rand(1.5f, 3.5f);
            var start = at.Z / length * Arrive;
            Line(c, start, 0.3f, Phantom, k => at + Vector3.Back * k * 0.6f, k => Vector3.Back * len * (1f - k * 0.5f), 0.05f, k => 1f - k);
        }
        const int Ghosts = 4;
        for (var i = 0; i < Ghosts; i++)
        {
            var at = new Vector3(0f, 0.85f, length * (i + 1) / (Ghosts + 1));
            Globe(c, "energy", Shell, at.Z / length * Arrive, 0.45f, Phantom, k => at, k => 1f, k => (1f - k) * 0.9f, mesh: _ghost);
        }
        Crescent(c, Arrive, 0.36f, new Transform3D(new Basis(Vector3.Forward, Mathf.DegToRad(10f)), end + Vector3.Up * 0.9f), w * 1.5f + 0.8f, 230f, Phantom, thick: 0.35f, sweep: 0.28f, gain: 1.2f);
        Disc(c, Arrive, 0.35f, Shock, Phantom, end, k => (w * 1.5f + 0.8f) * Out(k), k => 1f - k, band: 0.15f);
        Flare(c, Arrive, 0.22f, Star, Phantom, end + Vector3.Up * 0.9f, 2.2f);
        Shine(c, end + Vector3.Up, 4f, Phantom.Main, age => Bump(age, Arrive, 0.35f));
    }

    private void Shockwave(Cast c, float r)
    {
        Disc(c, 0f, 0.55f, Shock, Dust, Vector3.Zero, k => r * Out(k), k => 1f - k * k, band: 0.16f, gain: 1.4f);
        Disc(c, 0.08f, 0.5f, Shock, Dust, Vector3.Zero, k => r * 0.75f * Out(k), k => (1f - k) * 0.7f, band: 0.2f);
        Disc(c, 0f, 1.6f, Cracks, Sun, Vector3.Zero, k => r * 0.7f, k => Fade(k, 0.03f, 0.4f), k => k);
        Flare(c, 0f, 0.2f, Star, Dust, Vector3.Up * 0.3f, 2.6f);
        for (var i = 0; i < 22; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Rand(0f, 0.08f), Rand(0.5f, 0.8f), Rubble, way * Rand(0.2f, 0.8f), way * Rand(3f, 7f) + Vector3.Up * Rand(4f, 8f), Gravity * 1.2f, Rand(0.1f, 0.16f), 0.02f);
        }
        for (var i = 0; i < 10; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            var far = Rand(0.5f, 0.9f) * r;
            var size = Rand(0.9f, 1.5f);
            Flare(c, Rand(0.02f, 0.1f), Rand(0.6f, 0.9f), Mote, Dust, k => way * far * Out(k) + Vector3.Up * (0.3f + k * 0.5f), k => size * (0.6f + k), k => (1f - k) * 0.45f);
        }
        Shine(c, Vector3.Up * 0.5f, r * 1.5f, Dust.Main, age => Mathf.Max(0f, 1f - age / 0.4f));
    }

    private void ArcaneBurst(Cast c, float r)
    {
        const float Gather = 0.45f;
        var heart = Vector3.Up * 1.2f;
        Disc(c, 0f, 1.2f, Runes, Arcane, Vector3.Zero, k => r * 0.8f, k => Fade(k, 0.05f, 0.3f), k => k * 2.5f);
        Globe(c, "energy", Orb, 0f, Gather + 0.03f, Arcane, k => heart, k => 0.15f + 0.45f * k * k, k => 0.6f + 0.6f * k);
        for (var i = 0; i < 18; i++)
        {
            var from = heart + FxKit.RandomDirection() * r * Rand(0.8f, 1.2f);
            var start = Rand(0f, 0.2f);
            Flare(c, start, Gather - start, Mote, Arcane, k => from.Lerp(heart, k * k), k => 0.3f * (1f - k * 0.5f), k => Fade(k, 0.2f, 0f));
        }
        Globe(c, "energy", Shell, Gather, 0.35f, Arcane, k => heart, k => 0.5f + r * 0.85f * Out(k), k => (1f - k) * (1f - k) * 1.4f);
        Disc(c, Gather, 0.45f, Shock, Arcane, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.15f, gain: 1.3f);
        Flare(c, Gather, 0.28f, Star, Arcane, heart, 3.6f);
        for (var i = 0; i < 22; i++)
            Spark(c, Gather, Rand(0.3f, 0.5f), Arcane, heart, FxKit.RandomDirection() * Rand(8f, 14f), 3f, 0.07f, 0.03f);
        Shine(c, heart, r * 1.6f, Arcane.Main, age => age < Gather ? 0.4f * age / Gather : 1.6f * Mathf.Max(0f, 1f - (age - Gather) / 0.4f));
    }

    private void Singularity(Cast c, float r)
    {
        const float Collapse = 2.1f, Gone = 2.3f;
        var heart = Vector3.Up * 1.2f;
        Func<float, float> size = k =>
        {
            var t = k * Gone;
            var grow = Out(Mathf.Min(t / 0.25f, 1f));
            var shrink = t > Collapse ? 1f - (t - Collapse) / (Gone - Collapse) : 1f;
            return 0.6f * grow * shrink * (1f + 0.06f * Mathf.Sin(t * 14f));
        };
        Globe(c, "energy", Orb, 0f, Gone, Void, k => heart, k => size(k) * 1.35f, k => 0.5f);
        Globe(c, "void", 0, 0f, Gone, Void, k => heart, size, k => 1f);
        Disc(c, 0f, Gone, Swirl, Void, Vector3.Zero, k => r, k => Fade(k, 0.08f, 0.06f), spin: 4f, gain: 1.2f);
        const int Streaks = 44;
        for (var i = 0; i < Streaks; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var start = i * (Collapse - 0.5f) / Streaks;
            var from = r * Rand(0.8f, 1.05f);
            Func<float, Vector3> at = k =>
            {
                var a = angle + k * 2.2f;
                var reach = from * (1f - k * k);
                return Around(a) * reach + Vector3.Up * Mathf.Lerp(0.2f, heart.Y, k * k);
            };
            Line(c, start, 0.55f, Void, at, k => (at(Mathf.Min(k + 0.06f, 1f)) - at(k)).Normalized() * (0.4f + k * 0.8f), 0.06f, k => Fade(k, 0.2f, 0.15f));
        }
        Cue(c, Gone, () => world.Explosion(At(c, heart), r * 0.6f, Void.Main, FxBurst.Dome));
        Flare(c, Gone - 0.05f, 0.3f, Star, Void, heart, 3.6f);
        Disc(c, Gone, 0.4f, Shock, Void, Vector3.Zero, k => r * 1.1f * Out(k), k => 1f - k, band: 0.15f);
        Shine(c, heart, r * 1.4f, Void.Main, age => age < Gone ? 0.35f : 1.5f * Mathf.Max(0f, 1f - (age - Gone) / 0.3f));
    }

    private void StasisDome(Cast c, float r)
    {
        const float Life = 3f;
        Globe(c, "energy", Hexes, 0f, Life, Chrono, k => Vector3.Zero, k => r * Overshoot(Mathf.Min(k * Life / 0.22f, 1f)), k => Fade(k, 0.02f, 0.1f), gain: 1.1f);
        Disc(c, 0f, Life, Runes, Chrono, Vector3.Zero, k => r, k => Fade(k, 0.02f, 0.1f), k => k * Life / 0.35f);
        Disc(c, 0f, Life, Glow, Chrono, Vector3.Zero, k => r, k => Fade(k, 0.02f, 0.1f) * 0.4f);
        Disc(c, 0f, 0.3f, Shock, Chrono, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.1f, gain: 1.4f);
        for (var i = 0; i < 26; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * r * 0.85f * Mathf.Sqrt(Rand(0f, 1f));
            at.Y = Rand(0.3f, r * 0.6f);
            var drift = FxKit.RandomDirection() * 0.08f;
            var phase = Rand(0f, Mathf.Tau);
            var star = i % 3 == 0;
            Flare(c, Rand(0.1f, 0.4f), Life - 0.45f, star ? Star : Mote, Chrono, k => at + drift * k, k => star ? 0.7f : 0.22f,
                k => Fade(k, 0.1f, 0.1f) * (0.6f + 0.4f * Mathf.Sin(k * 20f + phase)));
        }
        Shine(c, Vector3.Up, r * 1.3f, Chrono.Main, age => 0.5f * Fade(age / Life, 0.05f, 0.1f));
    }

    private void GlacialBurst(Cast c, float r)
    {
        Disc(c, 0f, 0.5f, Shock, Ice, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.2f, gain: 1.4f);
        Disc(c, 0.06f, 0.5f, Shock, Ice, Vector3.Zero, k => r * 0.7f * Out(k), k => (1f - k) * 0.7f, band: 0.2f);
        Cue(c, 0f, () => world.Patch(At(c, Vector3.Zero), r * 1.05f, 2.5f, FxTheme.Of(FxElement.Frost), FxGround.Frost));
        for (var ring = 0; ring < 2; ring++)
        {
            var count = ring == 0 ? 11 : 5;
            var far = r * (ring == 0 ? 0.8f : 0.38f);
            var turn = Rand(0f, Mathf.Tau);
            for (var i = 0; i < count; i++)
            {
                var way = Around(turn + i * Mathf.Tau / count + Rand(-0.15f, 0.15f));
                var delay = far / r * 0.25f;
                var height = Rand(0.9f, 1.5f) * (ring == 0 ? 1f : 1.2f);
                Cue(c, 0f, () => world.Spike(At(c, way * far), c.Root.Basis * way, height, FxSpike.Ice, delay));
            }
        }
        for (var i = 0; i < 24; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Rand(0f, 0.1f), Rand(0.4f, 0.7f), Ice, way * 0.3f + Vector3.Up * 0.4f, way * Rand(5f, 10f) + Vector3.Up * Rand(3f, 7f), Gravity, 0.07f, 0.035f);
        }
        for (var i = 0; i < 16; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * r * Mathf.Sqrt(Rand(0f, 1f)) + Vector3.Up * Rand(0.5f, 2f);
            Flare(c, Rand(0.1f, 0.5f), 1f, Mote, Ice, k => at + Vector3.Down * k * 0.6f, k => 0.18f, k => Fade(k, 0.2f, 0.4f));
        }
        Flare(c, 0f, 0.25f, Star, Ice, Vector3.Up * 0.6f, 3f);
        Shine(c, Vector3.Up, r * 1.5f, Ice.Main, age => 1.2f * Mathf.Max(0f, 1f - age / 0.6f));
    }

    private void FlamePillar(Cast c, float r)
    {
        const float Erupt = 0.5f;
        Disc(c, 0f, Erupt + 0.05f, Telegraph, Flame, Vector3.Zero, k => r, k => 0.9f, k => k * 1.05f);
        Column(c, Erupt, 1f, Fire, Flame, Vector3.Zero, k => new Vector2(r * (0.42f + 0.12f * k), 7f * Out(Mathf.Min(k / 0.12f, 1f))),
            k => Fade(k, 0.02f, 0.45f), k => Mathf.Max(0f, (k - 0.55f) * 1.6f), gain: 1.6f, speed: 1.8f);
        Column(c, Erupt, 0.45f, Light, Flame, Vector3.Zero, k => new Vector2(r * 0.25f, 9f), k => (1f - k) * (1f - k), gain: 1.4f);
        Cue(c, Erupt, () =>
        {
            world.Explosion(At(c, Vector3.Zero), r, FxTheme.Of(FxElement.Fire), FxBurst.Fire);
            world.Patch(At(c, Vector3.Zero), r * 0.8f, 2.5f, FxTheme.Of(FxElement.Fire), FxGround.Fire);
        });
        for (var i = 0; i < 26; i++)
        {
            var from = Around(Rand(0f, Mathf.Tau)) * r * 0.5f * Mathf.Sqrt(Rand(0f, 1f)) + Vector3.Up * Rand(0.2f, 2f);
            var way = FxKit.Flat(from).Normalized();
            Spark(c, Erupt + Rand(0f, 0.4f), Rand(0.6f, 1f), Flame, from, way * Rand(1f, 3f) + Vector3.Up * Rand(5f, 10f), Gravity * 0.4f, 0.07f, 0.035f);
        }
        Shine(c, Vector3.Up * 2f, r * 2f, Flame.Main, age => age < Erupt ? 0.15f : 1.8f * Mathf.Max(0f, 1f - (age - Erupt) / 1f));
    }

    private void Judgment(Cast c, float r)
    {
        const float Strike = 0.6f;
        Disc(c, 0f, 1.6f, Runes, Gold, Vector3.Zero, k => r, k => Fade(k, 0.03f, 0.3f), k => k * 1.6f / 0.5f, gain: 1.2f);
        Disc(c, 0f, Strike + 0.1f, Glow, Gold, Vector3.Zero, k => r * 0.9f, k => k * 0.6f);
        for (var i = 0; i < 18; i++)
        {
            var land = Around(Rand(0f, Mathf.Tau)) * r * 0.8f * Mathf.Sqrt(Rand(0f, 1f));
            var high = Rand(5f, 8f);
            var start = Rand(0f, 0.25f);
            Line(c, start, Strike - start, Gold, k => land + Vector3.Up * high * (1f - k * k), k => Vector3.Down * 1.2f, 0.07f, k => Fade(k, 0.2f, 0.05f));
        }
        Column(c, Strike, 0.7f, Light, Gold, Vector3.Zero, k => new Vector2(r * Mathf.Lerp(0.6f, 0.3f, k), 16f), k => Fade(k, 0.02f, 0.6f), gain: 1.8f, speed: -1.5f);
        Cue(c, Strike, () => world.Explosion(At(c, Vector3.Up * 0.3f), r * 0.8f, FxTheme.Of(FxElement.Holy), FxBurst.Dome));
        Disc(c, Strike, 0.5f, Shock, Gold, Vector3.Zero, k => r * 1.2f * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
        Flare(c, Strike, 0.3f, Star, Gold, Vector3.Up * 1f, 4f);
        for (var i = 0; i < 18; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Strike, Rand(0.4f, 0.7f), Gold, way * Rand(0f, r * 0.4f) + Vector3.Up * 0.2f, way * Rand(2f, 5f) + Vector3.Up * Rand(6f, 10f), Gravity, 0.07f, 0.035f);
        }
        Shine(c, Vector3.Up * 1.5f, r * 2f, Gold.Main, age => age < Strike ? 0.4f * age / Strike : 2f * Mathf.Max(0f, 1f - (age - Strike) / 0.6f));
    }

    private void ThunderCall(Cast c, float r)
    {
        const int Bolts = 7;
        Disc(c, 0f, 1.7f, Runes, Storm, Vector3.Zero, k => r, k => Fade(k, 0.03f, 0.2f), k => k * 1.7f / 0.25f);
        Disc(c, 0f, 1.7f, Glow, Storm, Vector3.Zero, k => r, k => Fade(k, 0.1f, 0.2f) * 0.35f);
        var times = new float[Bolts];
        var spots = new Vector3[Bolts];
        for (var i = 0; i < Bolts; i++)
        {
            var when = 0.25f + i * 0.17f + Rand(-0.03f, 0.03f);
            var spot = Around(Rand(0f, Mathf.Tau)) * r * 0.85f * Mathf.Sqrt((i + Rand(0.2f, 0.8f)) / Bolts);
            times[i] = when;
            spots[i] = spot;
            Cue(c, when, () => world.Lightning([(At(c, spot) + Vector3.Up * 12f, At(c, spot))], 0.18f, 10, 0.8f));
            Disc(c, when, 0.3f, Shock, Storm, spot, k => 1.4f * Out(k), k => 1f - k, band: 0.18f, gain: 1.3f);
            Disc(c, when, 1f, Cracks, Storm, spot, k => 0.9f, k => Fade(k, 0.02f, 0.5f), k => k);
            Flare(c, when, 0.18f, Star, Storm, spot + Vector3.Up * 0.3f, 1.8f);
            for (var j = 0; j < 6; j++)
            {
                var way = Around(Rand(0f, Mathf.Tau));
                Spark(c, when, Rand(0.2f, 0.35f), Storm, spot + Vector3.Up * 0.1f, way * Rand(3f, 6f) + Vector3.Up * Rand(3f, 6f), Gravity, 0.05f, 0.03f);
            }
        }
        Shine(c, Vector3.Up, r * 1.4f, Storm.Main, age =>
        {
            var lit = 0.2f;
            foreach (var when in times)
                lit += 1.6f * Bump(age, when, 0.2f);
            return lit;
        });
    }

    private void ToxicNova(Cast c, float r)
    {
        Disc(c, 0f, 0.7f, Shock, Toxic, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.3f, gain: 1.5f);
        Disc(c, 0.1f, 0.6f, Shock, Toxic, Vector3.Zero, k => r * 0.7f * Out(k), k => (1f - k) * 0.6f, band: 0.3f);
        Cue(c, 0.05f, () => world.Patch(At(c, Vector3.Zero), r * 0.9f, 3f, FxTheme.Of(FxElement.Poison), FxGround.Poison));
        for (var i = 0; i < 20; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            var far = r * Rand(0.55f, 1f);
            var size = Rand(0.8f, 1.4f);
            Flare(c, Rand(0f, 0.1f), Rand(0.8f, 1.2f), Mote, Toxic, k => way * far * Out(k) + Vector3.Up * (0.4f + 0.5f * k), k => size * (0.7f + 0.6f * k), k => (1f - k) * 0.55f, gain: 0.8f);
        }
        for (var i = 0; i < 18; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Rand(0f, 0.1f), Rand(0.4f, 0.65f), Toxic, way * 0.3f + Vector3.Up * 0.3f, way * Rand(4f, 8f) + Vector3.Up * Rand(3f, 6f), Gravity, 0.09f, 0.03f);
        }
        Flare(c, 0f, 0.22f, Star, Toxic, Vector3.Up * 0.5f, 2.6f);
        Shine(c, Vector3.Up, r * 1.5f, Toxic.Main, age => 1.2f * Mathf.Max(0f, 1f - age / 0.7f));
    }

    private void ShadowBlink(Cast c, float length)
    {
        const float Appear = 0.12f;
        length = Mathf.Max(length, 1f);
        var from = Vector3.Up * 0.9f;
        var to = new Vector3(0f, 0.9f, length);
        Globe(c, "void", 0, 0f, 0.16f, Void, k => from, k => 0.75f * (1f - k * k), k => 1f);
        Globe(c, "energy", Shell, 0f, 0.3f, Phantom, k => from, k => 0.9f * (1f - k * 0.6f), k => 1f - k);
        Globe(c, "void", 0, Appear, 0.3f, Void, k => to, k => 0.85f * Out(Mathf.Min(k * 2.5f, 1f)) * (1f - k * 0.3f), k => 1f - Mathf.Max(0f, k - 0.4f) / 0.6f);
        Globe(c, "energy", Shell, Appear, 0.35f, Phantom, k => to, k => 0.6f + 0.8f * Out(k), k => 1f - k);
        foreach (var (at, start) in new[] { (Vector3.Zero, 0f), (FxKit.Flat(to), Appear) })
        {
            Disc(c, start, 0.4f, Shock, Phantom, at, k => 1.8f * Out(k), k => 1f - k, band: 0.2f, gain: 1.3f);
            for (var i = 0; i < 14; i++)
            {
                var way = FxKit.RandomDirection() with { Y = Rand(0.1f, 1f) };
                var spot = at + Vector3.Up * 0.9f;
                var push = way.Normalized() * Rand(1f, 2f);
                var size = Rand(0.25f, 0.5f);
                Flare(c, start, Rand(0.4f, 0.6f), Mote, Phantom, k => spot + push * Out(k), k => size * (1f - k * 0.5f), k => 1f - k);
            }
        }
        for (var i = 0; i < 8; i++)
        {
            var side = new Vector3(Rand(-0.3f, 0.3f), Rand(-0.4f, 0.4f), 0f);
            Line(c, 0.04f, 0.16f, Phantom, k => from + side + Vector3.Back * length * (0.2f + 0.6f * k), k => Vector3.Back * length * 0.35f, 0.03f, k => 1f - k);
        }
        Flare(c, Appear, 0.2f, Star, Phantom, to, 1.8f);
        Shine(c, to, 3f, Phantom.Main, age => Bump(age, Appear, 0.3f));
    }

    private void Sanctuary(Cast c, float r)
    {
        const float Life = 3.2f;
        Disc(c, 0f, Life, Runes, Verdant, Vector3.Zero, k => r, k => Fade(k, 0.02f, 0.1f), k => k * Life / 0.45f);
        Disc(c, 0f, Life, Glow, Verdant, Vector3.Zero, k => r, k => Fade(k, 0.05f, 0.1f) * (0.45f + 0.1f * Mathf.Sin(k * 25f)));
        Disc(c, 0f, 0.45f, Shock, Verdant, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.12f, gain: 1.2f);
        Column(c, 0.05f, Life - 0.05f, Light, Verdant, Vector3.Zero, k => new Vector2(r, 2.2f), k => Fade(k, 0.08f, 0.12f) * 0.55f, speed: 0.6f);
        for (var i = 0; i < 40; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * r * 0.95f * Mathf.Sqrt(Rand(0f, 1f));
            var rise = Rand(1.8f, 3f);
            var sway = Rand(0f, Mathf.Tau);
            var star = i % 4 == 0;
            Flare(c, Rand(0f, Life - 1.3f), Rand(0.9f, 1.3f), star ? Star : Mote, Verdant,
                k => at + Vector3.Up * (0.1f + rise * k) + new Vector3(Mathf.Sin(sway + k * 6f), 0f, Mathf.Cos(sway + k * 6f)) * 0.15f,
                k => star ? 0.55f : 0.22f, k => Fade(k, 0.2f, 0.4f));
        }
        Shine(c, Vector3.Up, r * 1.3f, Verdant.Main, age => 0.6f * Fade(age / Life, 0.05f, 0.1f));
    }

    private void LevelUp(Cast c, float r)
    {
        var head = Vector3.Up * 2.2f;
        Column(c, 0f, 1.4f, Spiral, Gold, Vector3.Zero, k => new Vector2(0.85f + 0.2f * k, 3.4f * Out(Mathf.Min(k / 0.35f, 1f))), k => Fade(k, 0.05f, 0.45f), twist: 1.6f, speed: 1.4f, gain: 1.5f);
        Column(c, 0f, 0.8f, Light, Gold, Vector3.Zero, k => new Vector2(0.6f, 4.5f), k => Fade(k, 0.05f, 0.7f) * 0.9f, gain: 1.3f);
        Disc(c, 0f, 1.2f, Glow, Gold, Vector3.Zero, k => 1.5f, k => Fade(k, 0.05f, 0.6f) * 0.8f);
        Disc(c, 0.05f, 0.6f, Shock, Gold, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.12f, gain: 1.4f);
        Disc(c, 0.2f, 0.6f, Shock, Gold, Vector3.Zero, k => r * 0.7f * Out(k), k => (1f - k) * 0.7f, band: 0.12f);
        Flare(c, 0.1f, 0.5f, Star, Gold, k => head, k => 3.8f * (1f - k * 0.4f), k => Fade(k, 0.1f, 0.6f));
        for (var i = 0; i < 26; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var rise = Rand(2.2f, 3.4f);
            Flare(c, Rand(0f, 0.5f), Rand(0.7f, 1f), Mote, Gold, k => Around(angle + k * 4f) * (0.9f - 0.3f * k) + Vector3.Up * rise * Out(k), k => 0.24f, k => Fade(k, 0.1f, 0.4f));
        }
        for (var i = 0; i < 10; i++)
        {
            var way = Around(i * Mathf.Tau / 10f) with { Y = Rand(-0.2f, 0.4f) };
            Flare(c, 0.15f, 0.6f, Star, Gold, k => head + way * 1.8f * Out(k), k => 0.8f, k => 1f - k);
        }
        Shine(c, head, 5f, Gold.Main, age => 1.4f * Mathf.Max(0f, 1f - age / 1.2f));
    }

    private void Recall(Cast c, float r)
    {
        const float Flash = 2.4f;
        Disc(c, 0f, Flash + 0.1f, Runes, Tide, Vector3.Zero, k => 1.5f, k => Fade(k, 0.02f, 0.03f), k => k * (Flash + 0.1f) / 0.6f, gain: 1.2f);
        Disc(c, 0f, Flash + 0.1f, Swirl, Tide, Vector3.Zero, k => 1.6f, k => 0.2f + 0.8f * k, spin: 3f);
        Column(c, 0f, Flash + 0.05f, Spiral, Tide, Vector3.Zero, k => new Vector2(0.9f - 0.15f * k, 2.6f), k => 0.25f + 0.85f * k * k, twist: 1.3f, speed: 1.2f);
        const int Motes = 32;
        for (var i = 0; i < Motes; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            Flare(c, i * (Flash - 0.6f) / Motes, 0.7f, Mote, Tide, k => Around(angle + k * 3f) * 1.3f * (1f - 0.6f * k) + Vector3.Up * 2.4f * k * k, k => 0.22f, k => Fade(k, 0.2f, 0.3f));
        }
        Column(c, Flash, 0.4f, Light, Tide, Vector3.Zero, k => new Vector2(0.9f * (1f - k * 0.5f), 14f), k => (1f - k) * (1f - k), gain: 2f, speed: -2f);
        Flare(c, Flash, 0.3f, Star, Tide, Vector3.Up * 1.2f, 4f);
        Disc(c, Flash, 0.45f, Shock, Tide, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
        Shine(c, Vector3.Up, 4f, Tide.Main, age => age < Flash ? 0.2f + 0.5f * age / Flash : 2f * Mathf.Max(0f, 1f - (age - Flash) / 0.4f));
    }

    private void Bloodrage(Cast c, float r)
    {
        const float Life = 2.6f;
        Disc(c, 0f, 0.45f, Shock, Rage, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.16f, gain: 1.5f);
        Disc(c, 0f, 1.2f, Cracks, Rage, Vector3.Zero, k => 1.7f, k => Fade(k, 0.02f, 0.5f), k => k);
        Flare(c, 0f, 0.25f, Star, Rage, Vector3.Up * 1f, 3f);
        Column(c, 0.05f, Life - 0.05f, Fire, Rage, Vector3.Zero, k => new Vector2(0.75f, 2.4f * Out(Mathf.Min(k * 8f, 1f))), k => Fade(k, 0.03f, 0.15f), gain: 0.9f, speed: 1.6f);
        Disc(c, 0f, Life, Glow, Rage, Vector3.Zero, k => 1.5f, k => Fade(k, 0.03f, 0.15f) * (0.55f + 0.2f * Mathf.Sin(k * 30f)));
        for (var i = 0; i < 34; i++)
        {
            var from = Around(Rand(0f, Mathf.Tau)) * Rand(0.3f, 0.9f) + Vector3.Up * Rand(0.1f, 0.6f);
            Spark(c, Rand(0f, Life - 0.6f), Rand(0.5f, 0.8f), Rage, from, FxKit.Flat(from) * 0.6f + Vector3.Up * Rand(2.5f, 5f), -1f, 0.06f, 0.06f);
        }
        Shine(c, Vector3.Up, 3.5f, Rage.Main, age => age < 0.3f ? 1.5f : 0.6f * Fade(age / Life, 0f, 0.15f));
    }

    private void CriticalStrike(Cast c, float r)
    {
        var at = new Vector3(0f, 1f, r * 0.3f);
        Flare(c, 0f, 0.26f, Star, Crit, k => at, k => 3.4f * (0.7f + 0.3f * Out(k)), k => 1f - k * k);
        Flare(c, 0f, 0.12f, Mote, Crit, at, 1.6f);
        for (var i = 0; i < 14; i++)
        {
            var way = new Vector3(Rand(-1f, 1f), Rand(-0.7f, 1f), Rand(0f, 0.5f)).Normalized();
            var len = Rand(1.2f, 2.2f);
            Line(c, 0f, Rand(0.16f, 0.24f), Crit, k => at + way * (0.3f + 2.2f * Out(k)), k => way * len * (1f - k * 0.8f), 0.14f, k => 1f - k);
        }
        var slash = new Basis(Vector3.Back, Mathf.DegToRad(-30f)) * new Basis(Vector3.Right, Mathf.Pi / 2f);
        Crescent(c, 0f, 0.26f, new Transform3D(slash, at - slash * Vector3.Back * 1.8f), 1.8f, 90f, Crit, thick: 0.18f, sweep: 0.25f, gain: 1.3f);
        Disc(c, 0f, 0.3f, Shock, Crit, at, k => 2f * Out(k), k => 1f - k, basis: new Basis(Vector3.Right, Mathf.Pi / 2f), band: 0.16f, gain: 1.2f);
        Disc(c, 0.02f, 0.4f, Shock, Crit, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.14f);
        for (var i = 0; i < 16; i++)
            Spark(c, 0f, Rand(0.3f, 0.5f), Crit, at, new Vector3(Rand(-1f, 1f), Rand(0f, 1f), Rand(-0.3f, 0.8f)).Normalized() * Rand(6f, 11f), Gravity, 0.06f, 0.03f);
        Shine(c, at, 4f, Crit.Main, age => 1.8f * Mathf.Max(0f, 1f - age / 0.3f));
    }

    // ---- Pieces.

    /// <summary>A crescent sweeping over its window: a fan of <paramref name="arc"/> degrees and <paramref name="radius"/>, its hub and plane placed by <paramref name="place"/>.</summary>
    private void Crescent(Cast c, float start, float life, Transform3D place, float radius, float arc, Tone tone, float thick, float sweep, float gain = 1f) =>
        Crescent(c, start, life, _ => place, radius, arc, tone, thick, sweep, gain);

    /// <summary>A crescent whose hub and plane move over its window.</summary>
    private void Crescent(Cast c, float start, float life, Func<float, Transform3D> place, float radius, float arc, Tone tone, float thick, float sweep, float gain = 1f)
    {
        var material = Material("crescent", 0, tone, gain, ("uThick", thick), ("uSweep", sweep));
        var scale = Basis.FromScale(Vector3.One * radius);
        Add(c, Fan(Mathf.RoundToInt(Mathf.Min(arc, 360f))), material, start, life, (n, k, _) =>
        {
            var p = place(k);
            n.Transform = new Transform3D(p.Basis * scale, p.Origin);
            Look(n, 1f, k);
        });
    }

    /// <summary>A column (fx_column) laid from one end to the other: a beam, a chain, a tether.</summary>
    private void Tube(Cast c, float start, float life, int style, Tone tone, Func<float, (Vector3 From, Vector3 To)> ends, Func<float, float> radius,
        Func<float, float> alpha, float twist = 1f, float speed = 1f, float gain = 1f)
    {
        var material = Material("column", style, tone, gain, ("uTwist", twist), ("uSpeed", speed));
        Add(c, _tube, material, start, life, (n, k, _) =>
        {
            var (a, b) = ends(k);
            n.Transform = new Transform3D(Along(b - a, Mathf.Max(radius(k), 1e-3f), 1e-3f), (a + b) * 0.5f);
            Look(n, alpha(k), 0f);
        });
    }

    /// <summary>A solid thing (a boulder) placed every frame.</summary>
    private void Prop(Cast c, float start, float life, Mesh mesh, Material material, Func<float, Transform3D> place) =>
        Add(c, mesh, material, start, life, (n, k, _) => n.Transform = place(k));

    /// <summary>
    /// One of the library's projectiles (<see cref="FxShotLook"/>) flown along <paramref name="at"/>
    /// over its window, facing the way it goes, and released at the end (or when the skill ends).
    /// </summary>
    private void Fly(Cast c, float start, float life, FxShotLook look, Func<float, Vector3> at, float size = 1f, Func<float, float>? grow = null)
    {
        FxShot shot = default;
        var live = false;
        void Place(float k)
        {
            const float E = 0.02f;
            var from = Mathf.Min(k, 1f - E);
            var velocity = c.Root.Basis * ((at(from + E) - at(from)) / (E * life));
            shot.Place(At(c, at(k)), velocity, size, grow?.Invoke(k) ?? 1f);
        }
        Cue(c, start, () =>
        {
            shot = world.Shot(look);
            live = true;
            Place(0f);
        });
        Cue(c, start + life, () =>
        {
            if (live)
                shot.Release();
            live = false;
        });
        c.Each.Add(age =>
        {
            if (live)
                Place(Mathf.Clamp((age - start) / life, 0f, 1f));
        });
        c.OnEnd.Add(() =>
        {
            if (live)
                shot.Release();
            live = false;
        });
    }

    /// <summary>A disc (fx_disc) at <paramref name="at"/>, lying flat unless <paramref name="basis"/> stands it up, <paramref name="size"/> metres round.</summary>
    private void Disc(Cast c, float start, float life, int style, Tone tone, Vector3 at, Func<float, float> size, Func<float, float> alpha,
        Func<float, float>? progress = null, Basis? basis = null, Vector3? stretch = null, float band = 0.12f, float spin = 3f, float gain = 1f)
    {
        var material = Material("disc", style, tone, gain, ("uBand", band), ("uSpin", spin));
        var turn = basis ?? Basis.Identity;
        var scale = stretch ?? Vector3.One;
        var lift = basis == null ? Vector3.Up * Lift : Vector3.Zero;
        Add(c, kit.Ground, material, start, life, (n, k, _) =>
        {
            // A shockwave's ring is drawn at ShockFront of its plane, so the plane reaches past the ring's soft front.
            var s = Mathf.Max(size(k), 0.05f) / (style == Shock ? ShockFront : 1f);
            n.Transform = new Transform3D(turn * Basis.FromScale(scale * s), at + lift);
            Look(n, alpha(k), style == Shock ? ShockFront : progress?.Invoke(k) ?? k);
        });
    }

    /// <summary>A disc (fx_disc) placed every frame: <paramref name="place"/>'s basis turns and sizes it (a unit of scale is a metre of radius).</summary>
    private void Disc(Cast c, float start, float life, int style, Tone tone, Func<float, Transform3D> place, Func<float, float> alpha,
        Func<float, float>? progress = null, float band = 0.12f, float spin = 3f, float cone = 0.5f, float gain = 1f)
    {
        var material = Material("disc", style, tone, gain, ("uBand", band), ("uSpin", spin), ("uCone", cone));
        Add(c, kit.Ground, material, start, life, (n, k, _) =>
        {
            n.Transform = place(k);
            Look(n, alpha(k), progress?.Invoke(k) ?? k);
        });
    }

    /// <summary>
    /// A wall (fx_wall) standing on the ground from <paramref name="a"/> to <paramref name="b"/>,
    /// <paramref name="height"/> tall; <paramref name="crumble"/> above 0 breaks it away.
    /// </summary>
    private void Wall(Cast c, float start, float life, int style, Tone tone, Vector3 a, Vector3 b, Func<float, float> height, Func<float, float> alpha,
        Func<float, float>? crumble = null, float speed = 1f, float gain = 1f)
    {
        var material = Material("wall", style, tone, gain, ("uSpeed", speed));
        var along = FxKit.Flat(b - a);
        var length = Mathf.Max(along.Length(), 0.05f);
        var x = along / length;
        var foot = FxKit.Flat((a + b) * 0.5f);
        Add(c, kit.Quad, material, start, life, (n, k, _) =>
        {
            var h = Mathf.Max(height(k), 1e-3f);
            n.Transform = new Transform3D(new Basis(x * length, Vector3.Up * h, x.Cross(Vector3.Up)), foot + Vector3.Up * h * 0.5f);
            Look(n, alpha(k), crumble?.Invoke(k) ?? 0f);
            n.SetInstanceShaderParameter("uAspect", length / h);
        });
    }

    /// <summary>
    /// A ribbon (fx_ribbon) drawn along <paramref name="path"/> from <paramref name="span"/>'s
    /// tail to its head (in the path's own units), <paramref name="width"/> wide at the head and
    /// narrowing to its tail, lying across the way it goes.
    /// </summary>
    private void Ribbon(Cast c, float start, float life, int style, Tone tone, Func<float, Vector3> path, Func<float, (float Tail, float Head)> span,
        float width, Func<float, float> alpha, float gain = 1f)
    {
        var material = Material("ribbon", style, tone, gain);
        var mesh = new ImmediateMesh();
        Add(c, mesh, material, start, life, (n, k, _) =>
        {
            mesh.ClearSurfaces();
            var (tail, head) = span(k);
            if (head - tail < 1e-4f)
                return;
            const int Steps = 28;
            var e = (head - tail) / Steps * 0.5f;
            mesh.SurfaceBegin(Mesh.PrimitiveType.TriangleStrip);
            for (var i = 0; i <= Steps; i++)
            {
                var u = (float)i / Steps;
                var s = tail + (head - tail) * u;
                var at = path(s);
                var side = (path(s + e) - path(s - e)).Cross(Vector3.Up);
                side = side.LengthSquared() < 1e-8f ? Vector3.Right : side.Normalized();
                side *= width * 0.5f * (0.25f + 0.75f * u);
                mesh.SurfaceSetUV(new Vector2(u, 0f));
                mesh.SurfaceAddVertex(at - side);
                mesh.SurfaceSetUV(new Vector2(u, 1f));
                mesh.SurfaceAddVertex(at + side);
            }
            mesh.SurfaceEnd();
            Look(n, alpha(k), k);
        });
    }

    /// <summary>
    /// A jagged bolt from <paramref name="a"/> to <paramref name="b"/> drawn with streaks in any
    /// colour (Fx.Lightning is always blue-white): three shapes, flicking from one to the next.
    /// </summary>
    private void Bolt(Cast c, float start, float life, Tone tone, Vector3 a, Vector3 b, float width, int kinks = 8, float jag = 0.3f)
    {
        const int Shapes = 3;
        var side = (b - a).Cross(Vector3.Up);
        side = side.LengthSquared() < 1e-4f ? Vector3.Right : side.Normalized();
        for (var v = 0; v < Shapes; v++)
        {
            var points = new Vector3[kinks + 1];
            for (var k = 0; k <= kinks; k++)
            {
                points[k] = a.Lerp(b, (float)k / kinks);
                if (k > 0 && k < kinks)
                    points[k] += side * Rand(-jag, jag) + Vector3.Up * Rand(-jag, jag) * 0.6f;
            }
            var shape = v;
            for (var j = 0; j < kinks; j++)
            {
                var (p, q) = (points[j], points[j + 1]);
                Line(c, start, life, tone, k => (p + q) * 0.5f, k => (q - p) * 1.15f, width,
                    k => (int)(k * life * 30f) % Shapes == shape ? Fade(k, 0f, 0.35f) : 0f);
            }
        }
    }

    /// <summary>A column (fx_column) standing on <paramref name="foot"/>, <paramref name="size"/> giving its radius and height.</summary>
    private void Column(Cast c, float start, float life, int style, Tone tone, Vector3 foot, Func<float, Vector2> size, Func<float, float> alpha,
        Func<float, float>? erode = null, float twist = 1f, float speed = 1f, float gain = 1f)
    {
        var material = Material("column", style, tone, gain, ("uTwist", twist), ("uSpeed", speed));
        Add(c, _tube, material, start, life, (n, k, _) =>
        {
            var s = size(k);
            n.Transform = new Transform3D(Basis.FromScale(new Vector3(Mathf.Max(s.X, 1e-3f), Mathf.Max(s.Y, 1e-3f), Mathf.Max(s.X, 1e-3f))), foot + Vector3.Up * s.Y * 0.5f);
            Look(n, alpha(k), erode?.Invoke(k) ?? 0f);
        });
    }

    /// <summary>A sphere (fx_energy or fx_void, or <paramref name="mesh"/>) at <paramref name="centre"/>, <paramref name="radius"/> round.</summary>
    private void Globe(Cast c, string shader, int style, float start, float life, Tone tone, Func<float, Vector3> centre, Func<float, float> radius,
        Func<float, float> alpha, Func<float, float>? erode = null, float gain = 1f, Mesh? mesh = null)
    {
        var material = Material(shader, style, tone, gain);
        Add(c, mesh ?? kit.Globe, material, start, life, (n, k, _) =>
        {
            n.Transform = new Transform3D(Basis.FromScale(Vector3.One * Mathf.Max(radius(k), 1e-3f)), centre(k));
            Look(n, alpha(k), erode?.Invoke(k) ?? 0f);
        });
    }

    /// <summary>A spark thrown from <paramref name="from"/> at <paramref name="velocity"/>, falling, drawn out along its flight by <paramref name="stretch"/> seconds of it.</summary>
    private void Spark(Cast c, float start, float life, Tone tone, Vector3 from, Vector3 velocity, float gravity, float width, float stretch, float gain = 1.2f)
    {
        var material = Material("streak", Streak, tone, gain);
        Add(c, kit.Quad, material, start, life, (n, k, t) =>
        {
            var v = velocity + Vector3.Down * gravity * t;
            n.Transform = new Transform3D(Along(v * stretch, width, width * 2f), from + velocity * t + Vector3.Down * 0.5f * gravity * t * t);
            Look(n, 1f - k * k, k);
        });
    }

    /// <summary>A streak posed every frame: where its middle is and the way and length it lies.</summary>
    private void Line(Cast c, float start, float life, Tone tone, Func<float, Vector3> at, Func<float, Vector3> axis, float width, Func<float, float> alpha)
    {
        var material = Material("streak", Streak, tone, 1.2f);
        Add(c, kit.Quad, material, start, life, (n, k, _) =>
        {
            n.Transform = new Transform3D(Along(axis(k), width, width), at(k));
            Look(n, alpha(k), k);
        });
    }

    /// <summary>A mote or star (fx_streak style 1 or 2) square on to the camera.</summary>
    private void Flare(Cast c, float start, float life, int style, Tone tone, Func<float, Vector3> at, Func<float, float> size, Func<float, float> alpha, float gain = 1.3f)
    {
        var material = Material("streak", style, tone, gain);
        Add(c, kit.Quad, material, start, life, (n, k, _) =>
        {
            n.Transform = new Transform3D(Basis.FromScale(Vector3.One * Mathf.Max(size(k), 1e-3f)), at(k));
            Look(n, alpha(k), k);
        });
    }

    /// <summary>A flash that swells a little and fades.</summary>
    private void Flare(Cast c, float start, float life, int style, Tone tone, Vector3 at, float size) =>
        Flare(c, start, life, style, tone, _ => at, k => size * (0.6f + 0.6f * Out(Mathf.Min(k * 3f, 1f))), k => (1f - k) * (1f - k));

    /// <summary>Lights the ground round <paramref name="at"/> every frame by <paramref name="strength"/> of the skill's age.</summary>
    private static void Shine(Cast c, Vector3 at, float reach, Color color, Func<float, float> strength) =>
        c.Each.Add(age => Fx.Light(c.Root.Transform * at, reach, color, strength(age)));

    private static void Cue(Cast c, float at, Action action) => c.Cues.Add((at, action));

    /// <summary>A point in the skill's root space, in the space every effect's parent shares.</summary>
    private static Vector3 At(Cast c, Vector3 local) => c.Root.Transform * local;

    private static void Add(Cast c, Mesh mesh, Material material, float start, float life, Pose pose)
    {
        var node = new MeshInstance3D
        {
            Mesh = mesh,
            MaterialOverride = material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            Visible = false,
            ExtraCullMargin = 4f,
        };
        c.Root.AddChild(node);
        c.Pieces.Add(new Piece { Node = node, Pose = pose, Start = start, Life = Mathf.Max(life, 0.01f) });
    }

    private static void Look(GeometryInstance3D node, float alpha, float progress)
    {
        node.SetInstanceShaderParameter("uAlpha", Mathf.Max(alpha, 0f));
        node.SetInstanceShaderParameter("uProgress", progress);
    }

    /// <summary>One material per shader, style, colours and settings, shared by every skill that asks for it.</summary>
    private ShaderMaterial Material(string shader, int style, Tone tone, float gain, params (string Name, float Value)[] extra)
    {
        var key = $"{shader}/{style}/{tone}/{gain}";
        foreach (var (name, value) in extra)
            key += $"/{name}={value}";
        if (_materials.TryGetValue(key, out var cached))
            return cached;
        var material = FxKit.Shader(shader);
        material.SetShaderParameter("uStyle", style);
        material.SetShaderParameter("uColor", FxKit.Rgb(tone.Main));
        material.SetShaderParameter("uHot", FxKit.Rgb(tone.Hot));
        material.SetShaderParameter("uDeep", FxKit.Rgb(tone.Deep));
        material.SetShaderParameter("uGain", gain);
        foreach (var (name, value) in extra)
            material.SetShaderParameter(name, value);
        return _materials[key] = material;
    }

    /// <summary>
    /// A flat fan of <paramref name="degrees"/> centred on +Z with radius 1, from a small hub out:
    /// UV.x runs along the arc in the swing's direction (right to left), UV.y from the hub to the rim.
    /// </summary>
    private ArrayMesh Fan(int degrees)
    {
        if (_fans.TryGetValue(degrees, out var cached))
            return cached;
        const float Hub = 0.12f;
        var steps = Mathf.Max(12, degrees / 4);
        var vertices = new Vector3[(steps + 1) * 2];
        var uvs = new Vector2[vertices.Length];
        var indices = new int[steps * 6];
        var half = Mathf.DegToRad(degrees) * 0.5f;
        for (var k = 0; k <= steps; k++)
        {
            var t = (float)k / steps;
            var angle = half - t * half * 2f;
            var dir = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            vertices[k * 2] = dir * Hub;
            vertices[k * 2 + 1] = dir;
            uvs[k * 2] = new Vector2(t, 0f);
            uvs[k * 2 + 1] = new Vector2(t, 1f);
            if (k == steps)
                continue;
            var (o, v) = (k * 6, k * 2);
            (indices[o], indices[o + 1], indices[o + 2]) = (v, v + 1, v + 2);
            (indices[o + 3], indices[o + 4], indices[o + 5]) = (v + 1, v + 3, v + 2);
        }
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = vertices;
        arrays[(int)Mesh.ArrayType.TexUV] = uvs;
        arrays[(int)Mesh.ArrayType.Index] = indices;
        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        return _fans[degrees] = mesh;
    }

    /// <summary>A basis whose Y is <paramref name="axis"/> (at least <paramref name="least"/> long) and whose X and Z are <paramref name="width"/> long: what fx_streak reads.</summary>
    private static Basis Along(Vector3 axis, float width, float least)
    {
        var length = axis.Length();
        var y = length > 1e-4f ? axis / length : Vector3.Up;
        var x = (Mathf.Abs(y.Y) < 0.95f ? Vector3.Up : Vector3.Right).Cross(y).Normalized();
        return new Basis(x * width, y * Mathf.Max(length, least), x.Cross(y) * width);
    }

    private static Vector3 Around(float angle) => new(Mathf.Sin(angle), 0f, Mathf.Cos(angle));

    private static float Rand(float from, float to) => (float)GD.RandRange(from, to);

    private static float Out(float k) => 1f - (1f - k) * (1f - k);

    /// <summary>Eases out past 1 and settles back.</summary>
    private static float Overshoot(float k)
    {
        const float S = 1.8f;
        var x = k - 1f;
        return 1f + x * x * ((S + 1f) * x + S);
    }

    /// <summary>1 in the middle of a window, fading in over its first <paramref name="fadeIn"/> and out over its last <paramref name="fadeOut"/>.</summary>
    private static float Fade(float k, float fadeIn, float fadeOut) =>
        Mathf.Clamp(Mathf.Min(fadeIn > 0f ? k / fadeIn : 1f, fadeOut > 0f ? (1f - k) / fadeOut : 1f), 0f, 1f);

    /// <summary>A flash at <paramref name="at"/> seconds, dying away over <paramref name="over"/>.</summary>
    private static float Bump(float age, float at, float over) =>
        age < at ? 0f : Mathf.Max(0f, 1f - (age - at) / over);
}
