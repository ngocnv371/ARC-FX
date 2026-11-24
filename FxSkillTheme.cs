namespace Arc.Fx;

/// <summary>What a skill is made of, by look: the gallery groups skills by it and <see cref="FxSounds"/> picks their sounds by it.</summary>
public enum FxSkillTheme { Blade, Earth, Fire, Frost, Storm, Water, Nature, Arcane, Holy, Shadow, Buff }

public static class FxSkillThemes
{
    /// <summary>What <paramref name="skill"/> is made of.</summary>
    public static FxSkillTheme Of(FxSkill skill) => skill switch
    {
        FxSkill.CrimsonSlash or FxSkill.CrossSlash or FxSkill.RisingCleave or FxSkill.Whirlwind or
            FxSkill.CriticalStrike or FxSkill.ThousandCuts or FxSkill.Execution or
            FxSkill.BladeDance or FxSkill.Lunge or FxSkill.GroundWave or FxSkill.Riposte or
            FxSkill.Grapple or FxSkill.SteelCyclone or FxSkill.ThousandSteps or FxSkill.GaleThrust or
            FxSkill.GaleStep or FxSkill.SkywardCut or FxSkill.MoonfallCut or FxSkill.BladeRain or
            FxSkill.HaloCut or FxSkill.LotusFlurry or FxSkill.StarlitCrescent or
            FxSkill.ButterflyWaltz or FxSkill.TwinMoons or FxSkill.FeatherFan or FxSkill.IaidoBloom or
            FxSkill.RibbonDance or FxSkill.PrismaticEdge or FxSkill.DiamondLance or
            FxSkill.TigerClaws or FxSkill.SwordSaint or FxSkill.FlickerCuts or FxSkill.CrescentKick or
            FxSkill.YinYangPalm or FxSkill.ArrowStorm or FxSkill.PiercingArrow or FxSkill.BladeLotus or
            FxSkill.ShieldBash or FxSkill.ChakramReturn or FxSkill.SwordHalo
            => FxSkillTheme.Blade,
        FxSkill.Shockwave or FxSkill.LeapSlam or FxSkill.EarthenWall or FxSkill.Quake or
            FxSkill.BoulderToss or FxSkill.StoneArmor or FxSkill.RivenRidge or FxSkill.TremorRing or
            FxSkill.ResoundingSlam or FxSkill.Thornline or FxSkill.LandslideCharge or
            FxSkill.SandVortex or FxSkill.Monolith or FxSkill.StoneshotFan or FxSkill.CrystalGarden or
            FxSkill.GeodeBurst or FxSkill.JadePillars or FxSkill.GoldenFissure or FxSkill.DuneRoller or
            FxSkill.PebbleOrbit or FxSkill.AmberTomb or FxSkill.StoneCircle or FxSkill.StoneMaze or
            FxSkill.SandClock or FxSkill.RubyRain or FxSkill.EmeraldSpire or FxSkill.Quicksand or
            FxSkill.ObsidianShards
            => FxSkillTheme.Earth,
        FxSkill.FlamePillar or FxSkill.MagmaFissure or FxSkill.InfernoRing or FxSkill.DragonBreath or
            FxSkill.FireRain or FxSkill.BlazingDash or FxSkill.Combustion or FxSkill.EmberMark or
            FxSkill.CinderWave or FxSkill.PyreStrike or FxSkill.CrimsonVerdict or
            FxSkill.RollingCinderstone or FxSkill.FirewingDive or FxSkill.EmberWisps or
            FxSkill.SolarLance or FxSkill.RebornStar or FxSkill.SnapTraps or FxSkill.GrandRocket or
            FxSkill.FlareFan or FxSkill.EmberShroud or FxSkill.GuardiansFall or
            FxSkill.SkyfireMeteor or FxSkill.FlameSerpent or FxSkill.EmberVolley or
            FxSkill.NapalmFlask or FxSkill.PhoenixRise or FxSkill.SunFlare or FxSkill.CandleVigil or
            FxSkill.EmberButterflies or FxSkill.FireworkBloom or FxSkill.LavaLotus or
            FxSkill.CometDance or FxSkill.DragonSpiral or FxSkill.CinderFall or FxSkill.FlameCrown or
            FxSkill.EmberGarden or FxSkill.PhoenixFeathers or FxSkill.SearingBrand or
            FxSkill.MoltenChains or FxSkill.PyreStar or FxSkill.FlameMask or FxSkill.SunWheel
            => FxSkillTheme.Fire,
        FxSkill.GlacialBurst or FxSkill.Blizzard or FxSkill.IcePrison or FxSkill.FrostLance or
            FxSkill.GlacierPath or FxSkill.FrozenOrb or FxSkill.RimeBloom or FxSkill.RimeShackle or
            FxSkill.FrozenExpanse or FxSkill.RimeRampart or FxSkill.IcicleBarrage or
            FxSkill.GlacialComet or FxSkill.PermafrostSpiral or FxSkill.SnowfallSanctum or
            FxSkill.FrostLotus or FxSkill.ChandelierDrop or FxSkill.AuroraVeil or
            FxSkill.DiamondDust or FxSkill.MirrorShards or FxSkill.WinterWaltz or FxSkill.FrozenStar or
            FxSkill.FrostHeart or FxSkill.IceCrown or FxSkill.FrozenRose or FxSkill.SnowOwlDive or
            FxSkill.FrostBars or FxSkill.FrostCompass or FxSkill.CrystalRain or FxSkill.Avalanche or
            FxSkill.FrostWindow
            => FxSkillTheme.Frost,
        FxSkill.ThunderCall or FxSkill.ChainLightning or FxSkill.StaticField or
            FxSkill.BallLightning or FxSkill.Thunderclap or FxSkill.Tempest or FxSkill.Skybolt or
            FxSkill.HeavensWrath or FxSkill.GaleBarrier or FxSkill.ShockLine or FxSkill.StormLance or
            FxSkill.TeslaCoil or FxSkill.EyeOfTheStorm or FxSkill.StarCall or FxSkill.ThunderHalo or
            FxSkill.VoltLattice or FxSkill.StormWings or FxSkill.PlasmaBloom or
            FxSkill.BallLightningTrio or FxSkill.ZephyrRibbons or FxSkill.TempestCrown or
            FxSkill.GearStorm or FxSkill.StormCompass or FxSkill.VoltChains or FxSkill.SkyArrows or
            FxSkill.TeslaGate or FxSkill.CycloneWhorl or FxSkill.ShockTiles
            => FxSkillTheme.Storm,
        FxSkill.TidalWave or FxSkill.Geyser or FxSkill.Whirlpool or FxSkill.BubblePrison or
            FxSkill.LotusPond or FxSkill.RainDance or FxSkill.KoiStream or FxSkill.PearlBarrage or
            FxSkill.MistVeil or FxSkill.MoonTide or FxSkill.BubbleStream or FxSkill.FountainOfLife or
            FxSkill.ScaleShield or FxSkill.TidalWhorl or FxSkill.SilverShoal or FxSkill.RainbowMist or
            FxSkill.SongOfTheDeep or FxSkill.TearDrop or FxSkill.WaterMirror or FxSkill.Riptide
            => FxSkillTheme.Water,
        FxSkill.ToxicNova or FxSkill.Thornbind or FxSkill.VenomSpit or FxSkill.PlagueCloud or
            FxSkill.Regrowth or FxSkill.Swarm or FxSkill.MendingTotem or FxSkill.PetalStorm or
            FxSkill.CreepingRoots or FxSkill.ThornVolley or FxSkill.BlossomRain or
            FxSkill.ButterflySwarm or FxSkill.FairyRing or FxSkill.LeafCyclone or
            FxSkill.WispCompanions or FxSkill.DandelionDrift or FxSkill.ThornCrown or
            FxSkill.SpiritBloom or FxSkill.BloomBurst or FxSkill.DaisyChain or FxSkill.PawPrints or
            FxSkill.OwlWatch or FxSkill.ThornMaze or FxSkill.MapleGust or FxSkill.SporeCloud or
            FxSkill.WidowsWeb or FxSkill.BeeSwarm or FxSkill.FernUnfurl
            => FxSkillTheme.Nature,
        FxSkill.ArcaneBurst or FxSkill.Singularity or FxSkill.StasisDome or FxSkill.ArcaneMissiles or
            FxSkill.ManaShield or FxSkill.RiftBeam or FxSkill.GravityWell or FxSkill.Portal or
            FxSkill.PrismaticSpray or FxSkill.ConcussiveWave or FxSkill.RewindStep or
            FxSkill.StilledMoment or FxSkill.StoppedHour or FxSkill.DriftingMote or FxSkill.FeyRift or
            FxSkill.Dreamtether or FxSkill.PolymorphPuff or FxSkill.ReturningOrb or
            FxSkill.SpiritWisps or FxSkill.BeguilingMote or FxSkill.SpiritSteps or
            FxSkill.ArcaneBolt or FxSkill.ShiftShot or FxSkill.MirrorImages or FxSkill.AstralSigils or
            FxSkill.RiftTear or FxSkill.StarMap or FxSkill.ClockworkRewind or FxSkill.PrismRay or
            FxSkill.GravityLens or FxSkill.EyeOfInsight or FxSkill.CharmHeart or FxSkill.RuneCascade or
            FxSkill.NebulaBurst or FxSkill.UmbralEclipse or FxSkill.TimeShatter or
            FxSkill.WishingStars or FxSkill.CardTrick or FxSkill.RoyalFlush or FxSkill.HourglassTurn or
            FxSkill.Crescendo or FxSkill.Lullaby or FxSkill.PowerChord or FxSkill.ChessGambit or
            FxSkill.MaskedRevel or FxSkill.CompassRose or FxSkill.Labyrinth or FxSkill.StainedGlass or
            FxSkill.Triquetra or FxSkill.ArcaneWhorl or FxSkill.StarCrown or FxSkill.FortuneWheel
            => FxSkillTheme.Arcane,
        FxSkill.Judgment or FxSkill.Sanctuary or FxSkill.DivineShield or FxSkill.Consecration or
            FxSkill.HolyNova or FxSkill.SeraphWings or FxSkill.SacredLances or FxSkill.SolarBrand or
            FxSkill.BindOfDawn or FxSkill.PrismWard or FxSkill.LingeringStar or FxSkill.RadiantLance or
            FxSkill.LongshotBarrage or FxSkill.SwordOfVerdict or FxSkill.Benediction or
            FxSkill.SealOfLight or FxSkill.HaloDescent or FxSkill.FeatherStorm or FxSkill.DawnRays or
            FxSkill.SacredLotus or FxSkill.StarOfGrace or FxSkill.LanternsOfHope or
            FxSkill.ChoirOfRings or FxSkill.GuardianSpirit or FxSkill.WingsOfDawn or
            FxSkill.HolyCross or FxSkill.CrownOfLight or FxSkill.RoseWindow or FxSkill.AngelChoir or
            FxSkill.SacredShield or FxSkill.PilgrimStar or FxSkill.BlessedRain or FxSkill.Aureole or
            FxSkill.HallowedCross
            => FxSkillTheme.Holy,
        FxSkill.PhantomDash or FxSkill.ShadowBlink or FxSkill.ShadowClaws or FxSkill.SoulDrain or
            FxSkill.DeathCoil or FxSkill.ShadeClones or FxSkill.Curse or FxSkill.SunderingFinger or
            FxSkill.ReapingHook or FxSkill.LanternToss or FxSkill.ChainSweep or FxSkill.SpectralCage or
            FxSkill.UmbralBind or FxSkill.BlightPool or FxSkill.UmbralWard or FxSkill.SoulFetters or
            FxSkill.ShadowTendrils or FxSkill.SoulHarvest or FxSkill.GhostLegion or
            FxSkill.RagingSpirits or FxSkill.SkullBarrage or FxSkill.GraveCall or
            FxSkill.PhantomHost or FxSkill.CrowMurder or FxSkill.HauntedGround or
            FxSkill.BansheeWail or FxSkill.SoulBomber or FxSkill.DeathsJaw or FxSkill.ReaperShade or
            FxSkill.SkullRain or FxSkill.Possession or FxSkill.NetherGate or FxSkill.MaskOfDread or
            FxSkill.SpecterKnights or FxSkill.SoulChains or FxSkill.Wraithstorm or FxSkill.SkullPyre or
            FxSkill.PhantomArchers or FxSkill.SpiritPack or FxSkill.NightmareEyes or
            FxSkill.BoneWhorl or FxSkill.DeathKnell or FxSkill.ShadowPuppets
            => FxSkillTheme.Shadow,
        FxSkill.LevelUp or FxSkill.Recall or FxSkill.Bloodrage or FxSkill.WrathAura or
            FxSkill.SpiritGuard or FxSkill.HeartSurge or FxSkill.StarBlessing or
            FxSkill.ButterflyRenewal or FxSkill.MoonlitVeil or FxSkill.CrystalAegis or
            FxSkill.DarkCrown or FxSkill.LichForm or FxSkill.CrownOfVictory or FxSkill.BattleHymn or
            FxSkill.LuckyStar or FxSkill.AegisOath or FxSkill.TimeSlip
            => FxSkillTheme.Buff,
        _ => FxSkillTheme.Shadow,
    };
}
