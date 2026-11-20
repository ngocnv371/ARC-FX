using Godot;

namespace Arc.Fx;

/// <summary>
/// The case-study skills (docs/hero-case-study.md): our own versions of the mechanics of classic
/// hero kits, built from the same pieces as the rest, plus walls, ribbons, cone marks and props.
/// </summary>
internal sealed partial class FxSkills
{
    private static readonly Tone Wind = new(new(0.55f, 0.95f, 0.92f), new(0.95f, 1f, 1f), new(0.08f, 0.32f, 0.35f));
    private static readonly Tone Fey = new(new(0.72f, 0.55f, 1f), new(1f, 0.92f, 1f), new(0.25f, 0.15f, 0.5f));
    private static readonly Tone Rose = new(new(1f, 0.4f, 0.7f), new(1f, 0.88f, 0.95f), new(0.4f, 0.05f, 0.2f));
    private static readonly Tone Doom = new(new(0.3f, 0.9f, 0.35f), new(0.85f, 1f, 0.8f), new(0.02f, 0.12f, 0.04f));
    private static readonly Tone Soul = new(new(0.35f, 1f, 0.65f), new(0.9f, 1f, 0.95f), new(0.02f, 0.25f, 0.12f));
    private static readonly Tone Cyan = new(new(0.25f, 0.95f, 1f), new(0.9f, 1f, 1f), new(0.02f, 0.25f, 0.4f));
    private static readonly Tone Smoke = new(new(0.45f, 0.42f, 0.4f), new(0.85f, 0.8f, 0.75f), new(0.12f, 0.1f, 0.08f));
    private static readonly Tone Ember = new(new(1f, 0.25f, 0.05f), new(1f, 0.8f, 0.5f), new(0.3f, 0f, 0f));
    private static readonly Tone Verdict = new(new(0.9f, 0.05f, 0.05f), new(1f, 0.85f, 0.8f), new(0.12f, 0f, 0f));

    private ArrayMesh? _totem, _trap, _rocket, _lantern, _shard, _sword;
    private ArrayMesh[]? _crags;
    private ShaderMaterial? _iceBlock, _metal, _rocketPaint, _lanternBody, _totemLight, _verdictBlade;
    private readonly BoxMesh _block = new() { Size = Vector3.One };

    private ArrayMesh TotemMesh => _totem ??= FxProps.Totem();
    private ArrayMesh TrapMesh => _trap ??= FxProps.Trap();
    private ArrayMesh RocketMesh => _rocket ??= FxProps.Rocket();
    private ArrayMesh LanternMesh => _lantern ??= FxProps.Lantern();
    private ArrayMesh ShardMesh => _shard ??= FxCrystalMesh.Shard(6);
    private ArrayMesh SwordMesh => _sword ??= FxProps.Sword();
    private ArrayMesh[] Crags => _crags ??= [.. Enumerable.Range(0, FxProps.RockSpikeLooks).Select(FxProps.RockSpike)];
    private ShaderMaterial IceBlock => _iceBlock ??= FxKit.Crystal(new Color(0.55f, 0.82f, 1f), new Color(0.92f, 0.98f, 1f), rim: 0.8f, glow: 0.15f);
    private ShaderMaterial Metal => _metal ??= FxKit.Crystal(new Color(0.3f, 0.3f, 0.34f), new Color(1f, 0.6f, 0.25f), rim: 0.5f, glow: 0.05f);
    private ShaderMaterial RocketPaint => _rocketPaint ??= FxKit.Crystal(new Color(0.75f, 0.18f, 0.15f), new Color(1f, 0.85f, 0.4f), rim: 0.5f, glow: 0.1f);
    private ShaderMaterial LanternBody => _lanternBody ??= FxKit.Crystal(new Color(0.15f, 0.25f, 0.2f), new Color(0.5f, 1f, 0.7f), rim: 0.6f, glow: 0.1f);
    private ShaderMaterial TotemLight
    {
        get
        {
            if (_totemLight != null)
                return _totemLight;
            _totemLight = FxKit.Spectral(Verdant.Main);
            _totemLight.SetShaderParameter("uGain", 1.8f);
            return _totemLight;
        }
    }

    private ShaderMaterial VerdictBlade
    {
        get
        {
            if (_verdictBlade != null)
                return _verdictBlade;
            _verdictBlade = FxKit.Spectral(Gold.Main);
            _verdictBlade.SetShaderParameter("uGain", 1.3f);
            _verdictBlade.SetShaderParameter("uRunes", 1.8f);
            return _verdictBlade;
        }
    }

    private void Study(FxSkill skill, Cast c, float r, float range)
    {
        switch (skill)
        {
            case FxSkill.CinderWave: CinderWave(c, range); break;
            case FxSkill.PyreStrike: PyreStrike(c, r); break;
            case FxSkill.CrimsonVerdict: CrimsonVerdict(c, range); break;
            case FxSkill.RimeBloom: RimeBloom(c, r); break;
            case FxSkill.RimeShackle: RimeShackle(c); break;
            case FxSkill.FrozenExpanse: FrozenExpanse(c, r); break;
            case FxSkill.Skybolt: Skybolt(c); break;
            case FxSkill.HeavensWrath: HeavensWrath(c, r); break;
            case FxSkill.SolarBrand: SolarBrand(c, r); break;
            case FxSkill.RollingCinderstone: RollingCinderstone(c, r, range); break;
            case FxSkill.ConcussiveWave: ConcussiveWave(c, range); break;
            case FxSkill.RimeRampart: RimeRampart(c, r); break;
            case FxSkill.SteelCyclone: SteelCyclone(c); break;
            case FxSkill.MendingTotem: MendingTotem(c, r); break;
            case FxSkill.ThousandSteps: ThousandSteps(c, r); break;
            case FxSkill.RewindStep: RewindStep(c, range); break;
            case FxSkill.StilledMoment: StilledMoment(c); break;
            case FxSkill.StoppedHour: StoppedHour(c, r); break;
            case FxSkill.RivenRidge: RivenRidge(c, range); break;
            case FxSkill.TremorRing: TremorRing(c); break;
            case FxSkill.ResoundingSlam: ResoundingSlam(c, r); break;
            case FxSkill.DriftingMote: DriftingMote(c, range); break;
            case FxSkill.FeyRift: FeyRift(c, range); break;
            case FxSkill.Dreamtether: Dreamtether(c, r); break;
            case FxSkill.Thornline: Thornline(c, range); break;
            case FxSkill.PolymorphPuff: PolymorphPuff(c); break;
            case FxSkill.SunderingFinger: SunderingFinger(c, range); break;
            case FxSkill.FirewingDive: FirewingDive(c, range); break;
            case FxSkill.EmberWisps: EmberWisps(c, range); break;
            case FxSkill.SolarLance: SolarLance(c, range); break;
            case FxSkill.RebornStar: RebornStar(c, r); break;
            case FxSkill.GaleThrust: GaleThrust(c, range); break;
            case FxSkill.GaleBarrier: GaleBarrier(c, r); break;
            case FxSkill.GaleStep: GaleStep(c, range); break;
            case FxSkill.SkywardCut: SkywardCut(c); break;
            case FxSkill.ReturningOrb: ReturningOrb(c, range); break;
            case FxSkill.SpiritWisps: SpiritWisps(c, r); break;
            case FxSkill.BeguilingMote: BeguilingMote(c, range); break;
            case FxSkill.SpiritSteps: SpiritSteps(c, range); break;
            case FxSkill.BindOfDawn: BindOfDawn(c, range); break;
            case FxSkill.PrismWard: PrismWard(c, range); break;
            case FxSkill.LingeringStar: LingeringStar(c, r, range); break;
            case FxSkill.RadiantLance: RadiantLance(c, range); break;
            case FxSkill.ShockLine: ShockLine(c, range); break;
            case FxSkill.SnapTraps: SnapTraps(c, range); break;
            case FxSkill.GrandRocket: GrandRocket(c, r, range); break;
            case FxSkill.ReapingHook: ReapingHook(c, range); break;
            case FxSkill.LanternToss: LanternToss(c, range); break;
            case FxSkill.ChainSweep: ChainSweep(c, r); break;
            case FxSkill.SpectralCage: SpectralCage(c, r); break;
            case FxSkill.ArcaneBolt: ArcaneBolt(c, range); break;
            case FxSkill.ShiftShot: ShiftShot(c, range); break;
            case FxSkill.LongshotBarrage: LongshotBarrage(c, range); break;
            case FxSkill.LandslideCharge: LandslideCharge(c, r, range); break;
            case FxSkill.FlareFan: FlareFan(c, r); break;
            case FxSkill.EmberShroud: EmberShroud(c); break;
            case FxSkill.GuardiansFall: GuardiansFall(c, r); break;
            case FxSkill.SwordOfVerdict: SwordOfVerdict(c, r); break;
            case FxSkill.UmbralBind: UmbralBind(c, range); break;
            case FxSkill.BlightPool: BlightPool(c, r); break;
            case FxSkill.UmbralWard: UmbralWard(c); break;
            default: SoulFetters(c, r); break;
        }
    }

    /// <summary><paramref name="count"/> points spread round the spot, between half and all of <paramref name="r"/> out, evenly round with a little jitter.</summary>
    private static Vector3[] Spread(int count, float r)
    {
        var turn = Rand(0f, Mathf.Tau);
        return [.. Enumerable.Range(0, count).Select(i => Around(turn + i * Mathf.Tau / count + Rand(-0.3f, 0.3f)) * r * Rand(0.55f, 1f))];
    }

    /// <summary>A flat transform for a disc at <paramref name="at"/>, <paramref name="radius"/> metres round, turned by <paramref name="turn"/>.</summary>
    private static Transform3D Lay(Vector3 at, float radius, Basis? turn = null) =>
        new((turn ?? Basis.Identity) * Basis.FromScale(Vector3.One * Mathf.Max(radius, 1e-3f)), at + Vector3.Up * Lift);

    // ---- Fire.

    private void CinderWave(Cast c, float range)
    {
        const float Spread1 = 0.38f, Travel = 0.55f;
        var length = Mathf.Max(range, 2f);
        Disc(c, 0f, 1.6f, Cone, Flame, k => Lay(Vector3.Zero, length), k => Fade(k, 0.02f, 0.5f), k => Out(Mathf.Min(k * 1.6f / Travel, 1f)), cone: Spread1, gain: 1.3f);
        // The front: a fan's rim at the cone's reach, scaled out as it goes.
        Crescent(c, 0f, Travel + 0.15f, k => new Transform3D(Basis.FromScale(Vector3.One * Mathf.Max(0.05f, Out(Mathf.Min(k * (Travel + 0.15f) / Travel, 1f)))), Vector3.Up * 0.35f),
            length, Mathf.RadToDeg(Spread1 * 2f), Flame, thick: 0.28f, sweep: 0.06f, gain: 1.6f);
        for (var i = 0; i < 14; i++)
        {
            var s = Rand(0.25f, 1f);
            var at = Around(Rand(-Spread1, Spread1) * 0.9f) * length * s;
            var tall = Rand(0.8f, 1.6f);
            Column(c, s * Travel, 0.5f, Fire, Flame, at, k => new Vector2(0.3f, tall * Out(Mathf.Min(k * 6f, 1f))), k => 1f - k, gain: 1.4f, speed: 2f);
            Spark(c, s * Travel, Rand(0.4f, 0.7f), Flame, at + Vector3.Up * 0.3f, Vector3.Up * Rand(3f, 6f) + FxKit.Jitter(1f), Gravity * 0.4f, 0.06f, 0.04f);
        }
        Cue(c, Travel * 0.6f, () => Stage.Patch(At(c, new Vector3(0f, 0f, length * 0.6f)), length * 0.3f, 2f, FireColor, FxGround.Fire));
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Flame.Main, age => 1.5f * Fade(age / 1.2f, 0.05f, 0.5f));
    }

    private void PyreStrike(Cast c, float r)
    {
        const float Erupt = 0.35f;
        var reach = r * 0.7f;
        Disc(c, 0f, Erupt + 0.04f, Telegraph, Flame, Vector3.Zero, k => reach, k => 0.8f, k => k * 1.1f);
        Cue(c, Erupt, () => Stage.Explosion(At(c, Vector3.Up * 0.3f), reach * 1.1f, FireColor, FxBurst.Fire));
        Column(c, Erupt, 0.55f, Fire, Flame, Vector3.Zero, k => new Vector2(reach * (0.75f + 0.2f * k), 2.4f * Out(Mathf.Min(k * 6f, 1f))), k => 1f - k * k, k => k * 0.9f, gain: 1.6f, speed: 2.4f);
        Disc(c, Erupt, 0.4f, Shock, Flame, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.14f, gain: 1.4f);
        Flare(c, Erupt, 0.25f, Star, Flame, Vector3.Up * 0.8f, 3.2f);
        for (var i = 0; i < 20; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Erupt, Rand(0.5f, 0.9f), Flame, way * Rand(0f, reach), way * Rand(1f, 3f) + Vector3.Up * Rand(6f, 11f), Gravity * 0.7f, 0.07f, 0.035f);
        }
        Shine(c, Vector3.Up, r * 1.6f, Flame.Main, age => age < Erupt ? 0.2f : 2f * Mathf.Max(0f, 1f - (age - Erupt) / 0.6f));
    }

    private void CrimsonVerdict(Cast c, float range)
    {
        const float Charge = 0.15f, Life = 0.4f;
        var length = Mathf.Max(range, 2f);
        var hand = new Vector3(0f, 1.3f, 0.5f);
        var foe = new Vector3(0f, 1.1f, length);
        Flare(c, 0f, Charge + 0.05f, Star, Verdict, k => hand, k => 0.6f + 2f * k, k => 1f);
        for (var i = 0; i < 10; i++)
        {
            var from = hand + FxKit.RandomDirection() * 1.2f;
            Flare(c, 0f, Charge, Mote, Verdict, k => from.Lerp(hand, k * k), k => 0.2f, k => 1f);
        }
        Bolt(c, Charge, Life, Verdict, hand, foe, 0.32f, 10, 0.45f);
        Bolt(c, Charge, Life * 0.8f, Verdict with { Main = Verdict.Hot }, hand, foe, 0.1f, 10, 0.3f);
        Tube(c, Charge, Life * 0.6f, Beam, Verdict, k => (hand, foe), k => 0.12f * (1f - k), k => 1f, speed: 3f, gain: 1.4f);
        Cue(c, Charge, () => Stage.Explosion(At(c, foe), 1.3f, Verdict.Main, FxBurst.Dome));
        Flare(c, Charge, 0.3f, Star, Verdict, foe, 3.6f);
        Disc(c, Charge, 0.35f, Shock, Verdict, FxKit.Flat(foe), k => 2.2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.4f);
        for (var i = 0; i < 18; i++)
            Spark(c, Charge, Rand(0.3f, 0.6f), Ember, foe, FxKit.RandomDirection() * Rand(5f, 10f), Gravity * 0.6f, 0.06f, 0.035f);
        Shine(c, foe, 4f, Verdict.Main, age => 2f * Bump(age, Charge, 0.4f));
    }

    private void RollingCinderstone(Cast c, float r, float range)
    {
        const float Fall = 0.5f, Roll = 1.4f;
        var length = Mathf.Max(range, 2f);
        var size = 0.55f * r / 3f;
        var land = new Vector3(0f, size, 0.8f);
        var sky = land + new Vector3(-1.5f, 9f, -3f);
        Func<float, Vector3> at = t => t < Fall ? sky.Lerp(land, t / Fall * (t / Fall)) : land + Vector3.Back * (length - 0.8f) * Mathf.SmoothStep(0f, 1f, (t - Fall) / Roll);
        var life = Fall + Roll;
        var spin = Vector3.Right;
        Prop(c, 0f, life, _rock, _stone, k => new Transform3D(new Basis(spin, k * life * 6f) * Basis.FromScale(Vector3.One * size), at(k * life)));
        Globe(c, "energy", Shell, 0f, life, Magma, k => at(k * life), k => size * 1.35f, k => 1f, gain: 1.3f);
        Line(c, 0f, Fall, Magma, k => at(k * Fall), k => (sky - land).Normalized() * 2.2f, 0.5f, k => 1f);
        Cue(c, Fall, () => Stage.Explosion(At(c, land), r * 0.5f, FireColor, FxBurst.Fire));
        var patches = Mathf.Max(2, (int)((length - 0.8f) / 1.2f));
        for (var i = 1; i <= patches; i++)
        {
            var s = (float)i / patches;
            var spot = FxKit.Flat(land) + Vector3.Back * (length - 0.8f) * s;
            Cue(c, Fall + Roll * Mathf.Lerp(0.15f, 0.9f, s), () => Stage.Patch(At(c, spot), 0.8f, 2f, FireColor, FxGround.Fire));
        }
        for (var i = 0; i < 30; i++)
        {
            var t = Rand(Fall, life);
            Spark(c, t, Rand(0.3f, 0.6f), Flame, at(t), new Vector3(Rand(-2f, 2f), Rand(2f, 5f), Rand(-3f, 0f)), Gravity, 0.07f, 0.035f);
        }
        Shine(c, Vector3.Up, length, Flame.Main, age => age < Fall ? 0.4f : 1.2f * Fade((age - Fall) / Roll, 0f, 0.2f));
    }

    private void FirewingDive(Cast c, float range)
    {
        const float Life = 1f, Trail = 0.32f;
        var length = Mathf.Max(range, 2f);
        Func<float, Vector3> path = s => new Vector3(1.8f * Mathf.Sin(s * Mathf.Tau), 0.9f + 0.7f * Mathf.Sin(s * Mathf.Pi), length * Mathf.Sin(s * Mathf.Pi));
        Ribbon(c, 0f, Life + Trail, 1, Flame, path, k => { var head = Mathf.Min(k * (Life + Trail) / Life, 1f); return (Mathf.Max(0f, head - Trail), head); }, 1.3f, k => 1f, gain: 1.6f);
        Globe(c, "energy", Orb, 0f, Life, Flame, k => path(k), k => 0.4f, k => 1f, gain: 1.4f);
        Flare(c, 0f, Life, Star, Flame, k => path(k), k => 1.4f, k => 1f);
        for (var i = 0; i < 6; i++)
        {
            var s = (i + 0.5f) / 6f;
            var spot = FxKit.Flat(path(s));
            Cue(c, s * Life, () => Stage.Patch(At(c, spot), 0.7f, 1.6f, FireColor, FxGround.Fire));
        }
        for (var i = 0; i < 24; i++)
        {
            var s = Rand(0f, 1f);
            Spark(c, s * Life, Rand(0.4f, 0.7f), Flame, path(s), FxKit.Jitter(2f) + Vector3.Up * 2f, Gravity * 0.5f, 0.06f, 0.04f);
        }
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Flame.Main, age => Fade(age / (Life + Trail), 0.05f, 0.3f) * 1.2f);
    }

    private void EmberWisps(Cast c, float range)
    {
        const int Wisps = 4;
        const float Orbit = 1.2f, Flight = 0.35f, Every = 0.15f;
        var length = Mathf.Max(range, 2f);
        for (var i = 0; i < Wisps; i++)
        {
            var phase = i * Mathf.Tau / Wisps;
            var launch = Orbit + i * Every;
            var target = new Vector3((i - 1.5f) * 0.9f, 0.2f, length);
            Func<float, Vector3> orbit = t => Around(phase + t * 4f) * 1.1f + Vector3.Up * 1.3f;
            var from = orbit(launch);
            Func<float, Vector3> arc = s => from.Lerp(target, s) + Vector3.Up * 2.5f * s * (1f - s) * 2f;
            Func<float, Vector3> at = k =>
            {
                var t = k * (launch + Flight);
                return t < launch ? orbit(t) : arc((t - launch) / Flight);
            };
            var life = launch + Flight;
            Globe(c, "energy", Orb, 0f, life, Flame, at, k => 0.2f * Out(Mathf.Min(k * life / 0.2f, 1f)), k => 1f, gain: 1.4f);
            Flare(c, 0f, life, Mote, Flame, at, k => 0.8f, k => 0.8f);
            Line(c, launch, Flight, Flame, k => arc(Mathf.Max(k - 0.08f, 0f)), k => arc(k) - arc(Mathf.Max(k - 0.25f, 0f)), 0.2f, k => 1f);
            Cue(c, life, () => Stage.Explosion(At(c, target), 1.1f, FireColor, FxBurst.Fire));
        }
        Shine(c, Vector3.Up * 1.3f, 3f, Flame.Main, age => 0.6f);
    }

    private void SolarLance(Cast c, float range)
    {
        const float Life = 2.4f;
        var length = Mathf.Max(range, 3f);
        var hand = new Vector3(0f, 1.3f, 0.5f);
        Func<float, Vector3> end = k => hand + new Vector3(Mathf.Sin(Mathf.Sin(k * 3f) * 0.18f), -0.2f, Mathf.Cos(Mathf.Sin(k * 3f) * 0.18f)) * (length - 0.5f);
        Func<float, float> open = k => Out(Mathf.Min(k * Life / 0.15f, 1f)) * (1f - Mathf.SmoothStep(0f, 1f, (k - 0.88f) / 0.12f)) * (1f + 0.07f * Mathf.Sin(k * 120f));
        Tube(c, 0f, Life, Beam, Flame, k => (hand, end(k)), k => 0.55f * open(k), k => 1f, speed: 2f, gain: 1.4f);
        Tube(c, 0f, Life, Beam, Flame with { Main = Flame.Hot }, k => (hand, end(k)), k => 0.2f * open(k), k => 1f, speed: 3f, gain: 1.6f);
        Flare(c, 0f, Life, Star, Flame, k => hand, k => 2.6f * open(k) + 0.3f, k => 1f);
        Flare(c, 0f, Life, Star, Flame, end, k => 2f * open(k), k => 1f);
        for (var i = 0; i < 40; i++)
        {
            var t = Rand(0.1f, Life - 0.3f);
            Spark(c, t, 0.35f, Flame, end(t / Life), FxKit.RandomDirection() * Rand(3f, 7f) + Vector3.Up * 2f, Gravity * 0.5f, 0.06f, 0.035f);
        }
        for (var i = 0; i < 4; i++)
        {
            var when = 0.2f + i * 0.5f;
            Cue(c, when, () => Stage.Patch(At(c, FxKit.Flat(end(when / Life))), 0.9f, 1.5f, FireColor, FxGround.Fire));
        }
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Flame.Main, age => 1.6f * open(Mathf.Min(age / Life, 1f)));
    }

    private void RebornStar(Cast c, float r)
    {
        const float Blow = 2.2f;
        var heart = Vector3.Up * 1.5f;
        Globe(c, "energy", Orb, 0f, Blow, Flame, k => heart, k => (0.5f + 0.6f * k) * (1f + 0.05f * Mathf.Sin(k * k * 80f)), k => 1f, gain: 1.5f);
        Globe(c, "energy", Shell, 0f, Blow, Ember, k => heart, k => (0.7f + 0.8f * k), k => 0.7f, gain: 1.2f);
        for (var i = 0; i < 3; i++)
        {
            var tilt = new Basis(Vector3.Back, i * 1.05f + 0.4f) * new Basis(Vector3.Right, Mathf.Pi / 2f);
            var speed = 2f + i * 0.7f;
            Disc(c, 0.05f, Blow - 0.05f, Runes, Flame, k => new Transform3D(new Basis(Vector3.Up, k * Blow * speed) * tilt * Basis.FromScale(Vector3.One * (1.2f + 0.6f * k)), heart),
                k => Fade(k, 0.05f, 0f), k => k * Blow / 0.4f, gain: 1.2f);
        }
        Disc(c, 0f, Blow, Glow, Flame, Vector3.Zero, k => r * 0.8f, k => 0.3f + 0.5f * k);
        Cue(c, Blow, () => Stage.Explosion(At(c, heart), r, FireColor, FxBurst.Fire));
        Flare(c, Blow, 0.4f, Star, Flame, heart, 5f);
        Disc(c, Blow, 0.6f, Shock, Flame, Vector3.Zero, k => r * 1.3f * Out(k), k => 1f - k, band: 0.14f, gain: 1.5f);
        for (var i = 0; i < 30; i++)
            Spark(c, Blow, Rand(0.5f, 0.9f), Flame, heart, FxKit.RandomDirection() * Rand(6f, 12f), Gravity * 0.4f, 0.08f, 0.04f);
        Shine(c, heart, r * 2f, Flame.Main, age => age < Blow ? 0.5f + 0.7f * age / Blow : 2.4f * Mathf.Max(0f, 1f - (age - Blow) / 0.6f));
    }

    private void SnapTraps(Cast c, float range)
    {
        const float Toss = 0.35f, Armed = 1.2f, Every = 0.15f;
        var length = Mathf.Max(range, 2f);
        var hand = new Vector3(0f, 1.1f, 0.4f);
        for (var i = 0; i < 3; i++)
        {
            var spot = new Vector3((i - 1) * 1.3f, 0f, length * (0.65f + 0.15f * (i % 2)));
            var snap = Armed + i * Every;
            Func<float, Vector3> arc = s => hand.Lerp(spot, s) + Vector3.Up * 2f * s * (1f - s) * 2f;
            var spin = Rand(0f, Mathf.Tau);
            Prop(c, 0f, snap, TrapMesh, Metal, k =>
            {
                var t = k * snap;
                var at = t < Toss ? arc(t / Toss) : spot;
                var turn = t < Toss ? new Basis(Vector3.Right, t * 14f) : Basis.Identity;
                return new Transform3D(new Basis(Vector3.Up, spin) * turn * Basis.FromScale(Vector3.One * 1.4f), at);
            });
            Disc(c, Toss, snap - Toss, Glow, Ember, spot, k => 0.8f, k => 0.4f + 0.3f * Mathf.Sin(k * 40f));
            Cue(c, snap, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.2f), 1f, FireColor, FxBurst.Fire));
            Flare(c, snap, 0.2f, Star, Ember, spot + Vector3.Up * 0.3f, 2f);
        }
    }

    private void GrandRocket(Cast c, float r, float range)
    {
        const float Flight = 0.9f;
        var length = Mathf.Max(range, 3f);
        var from = new Vector3(0f, 1.4f, 0.5f);
        var to = new Vector3(0f, 0.6f, length);
        Func<float, float> pace = k => k * k * 0.6f + k * 0.4f;
        Func<float, Vector3> at = k => from.Lerp(to, pace(k)) + Vector3.Up * 1.2f * Mathf.Sin(pace(k) * Mathf.Pi);
        Prop(c, 0f, Flight, RocketMesh, RocketPaint, k =>
        {
            var way = (at(Mathf.Min(k + 0.02f, 1f)) - at(Mathf.Max(k - 0.02f, 0f))).Normalized();
            return new Transform3D(Along(way, 1f, 1f) * Basis.FromScale(Vector3.One * 1.1f), at(k));
        });
        Line(c, 0f, Flight, Ember, k => at(k) - (at(Mathf.Min(k + 0.02f, 1f)) - at(k)).Normalized() * 0.9f, k => (at(Mathf.Min(k + 0.02f, 1f)) - at(k)).Normalized() * 1.4f, 0.35f, k => 1f);
        for (var i = 0; i < 26; i++)
        {
            var s = (i + 0.5f) / 26f;
            var puff = at(s);
            Flare(c, s * Flight, 1f, Mote, Smoke, k => puff + Vector3.Up * k * 0.6f, k => 0.5f + 0.9f * k, k => (1f - k) * 0.5f, gain: 0.8f);
        }
        Cue(c, Flight, () => Stage.Explosion(At(c, to), r, FireColor, FxBurst.Fire));
        Disc(c, Flight, 0.6f, Shock, Ember, FxKit.Flat(to), k => r * 1.4f * Out(k), k => 1f - k, band: 0.14f, gain: 1.4f);
        Disc(c, Flight, 1.6f, Cracks, Ember, FxKit.Flat(to), k => r * 0.7f, k => Fade(k, 0.02f, 0.4f), k => k);
        for (var i = 0; i < 24; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Flight, Rand(0.5f, 0.9f), Rubble, FxKit.Flat(to) + Vector3.Up * 0.4f, way * Rand(3f, 7f) + Vector3.Up * Rand(5f, 9f), Gravity * 1.2f, 0.12f, 0.02f);
        }
        Shine(c, to + Vector3.Up, r * 2f, Ember.Main, age => age < Flight ? 0.4f : 2.4f * Mathf.Max(0f, 1f - (age - Flight) / 0.6f));
    }

    private void FlareFan(Cast c, float r)
    {
        const float Spread1 = 0.6f;
        var reach = r * 1.3f;
        Disc(c, 0f, 1.4f, Cone, Flame, k => Lay(Vector3.Zero, reach), k => Fade(k, 0.02f, 0.6f), k => Out(Mathf.Min(k * 1.4f / 0.2f, 1f)), cone: Spread1, gain: 1.4f);
        var mouth = new Vector3(0f, 1f, 0.4f);
        for (var i = 0; i < 40; i++)
        {
            var way = new Vector3(Mathf.Sin(Rand(-Spread1, Spread1)), Rand(-0.25f, 0.05f), 1f).Normalized();
            var far = reach * Rand(0.6f, 1f);
            var tone = i % 3 == 0 ? Magma : Flame;
            Flare(c, Rand(0f, 0.08f), Rand(0.3f, 0.45f), Mote, tone, k => mouth + way * far * Out(k), k => 0.5f + 1.6f * k, k => (1f - k) * 0.8f, gain: 1.2f);
        }
        Flare(c, 0f, 0.2f, Star, Flame, mouth, 2.8f);
        Cue(c, 0.15f, () => Stage.Patch(At(c, new Vector3(0f, 0f, reach * 0.55f)), reach * 0.4f, 2f, FireColor, FxGround.Fire));
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach * 1.5f, Flame.Main, age => 1.8f * Mathf.Max(0f, 1f - age / 0.5f));
    }

    private void EmberShroud(Cast c)
    {
        const float Life = 2.6f;
        for (var i = 0; i < 3; i++)
        {
            var phase = i * Mathf.Tau / 3f;
            Func<float, Vector3> path = s => Around(phase + s * 4.5f) * 0.95f + Vector3.Up * (1f + 0.55f * Mathf.Sin(s * 2.3f + phase));
            Ribbon(c, 0f, Life, 1, Flame, path, k => { var head = k * Life * 2.4f; return (Mathf.Max(0f, head - 1.3f), head); }, 0.55f, k => Fade(k, 0.05f, 0.12f), gain: 1.5f);
        }
        Disc(c, 0f, Life, Glow, Flame, Vector3.Zero, k => 1.4f, k => Fade(k, 0.05f, 0.12f) * 0.6f);
        for (var i = 0; i < 24; i++)
        {
            var from = Around(Rand(0f, Mathf.Tau)) * Rand(0.5f, 1f) + Vector3.Up * Rand(0.3f, 1.4f);
            Spark(c, Rand(0f, Life - 0.6f), 0.6f, Flame, from, Vector3.Up * Rand(2f, 4f), -1f, 0.05f, 0.06f);
        }
        Shine(c, Vector3.Up, 3f, Flame.Main, age => 0.8f * Fade(age / Life, 0.05f, 0.12f));
    }

    private void GuardiansFall(Cast c, float r)
    {
        const float Land = 0.5f;
        Disc(c, 0f, Land + 0.6f, Runes, Flame, Vector3.Zero, k => r * 0.7f, k => Fade(k, 0.02f, 0.4f), k => k * (Land + 0.6f) / 0.3f, gain: 1.3f);
        var sky = new Vector3(0.6f, 14f, -0.6f);
        Line(c, 0.1f, Land - 0.1f, Flame, k => sky.Lerp(Vector3.Up * 0.8f, k * k), k => -sky.Normalized() * 3.5f, 0.8f, k => 1f);
        Globe(c, "energy", Orb, 0.1f, Land - 0.1f, Flame, k => sky.Lerp(Vector3.Up * 0.8f, k * k), k => 0.55f, k => 1f, gain: 1.4f);
        Cue(c, Land, () =>
        {
            Stage.Explosion(At(c, Vector3.Up * 0.3f), r, FireColor, FxBurst.Fire);
            Stage.Patch(At(c, Vector3.Zero), r * 0.8f, 2.5f, FireColor, FxGround.Fire);
        });
        Disc(c, Land, 0.5f, Shock, Flame, Vector3.Zero, k => r * 1.3f * Out(k), k => 1f - k, band: 0.14f, gain: 1.4f);
        Disc(c, Land, 1.6f, Cracks, Flame, Vector3.Zero, k => r * 0.65f, k => Fade(k, 0.02f, 0.4f), k => k);
        Flare(c, Land, 0.3f, Star, Flame, Vector3.Up, 4f);
        Shine(c, Vector3.Up, r * 1.8f, Flame.Main, age => age < Land ? 0.3f : 2.2f * Mathf.Max(0f, 1f - (age - Land) / 0.6f));
    }

    // ---- Frost.

    private void RimeBloom(Cast c, float r)
    {
        Disc(c, 0f, 0.5f, Shock, Ice, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.18f, gain: 1.5f);
        Cue(c, 0f, () => Stage.Patch(At(c, Vector3.Zero), r, 2.5f, FrostColor, FxGround.Frost));
        Disc(c, 0f, 2.4f, Runes, Ice, Vector3.Zero, k => r * 0.9f, k => Fade(k, 0.02f, 0.4f), k => k * 2.4f / 0.25f, gain: 1.3f);
        Flare(c, 0f, 0.3f, Star, Ice, Vector3.Up * 0.6f, 3.4f);
        for (var i = 0; i < 26; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Rand(0f, 0.08f), Rand(0.4f, 0.7f), Ice, way * 0.4f + Vector3.Up * 0.3f, way * Rand(4f, 9f) + Vector3.Up * Rand(3f, 6f), Gravity, 0.07f, 0.035f);
        }
        for (var i = 0; i < 18; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * r * Mathf.Sqrt(Rand(0f, 1f)) + Vector3.Up * Rand(0.4f, 1.8f);
            Flare(c, Rand(0.05f, 0.4f), 1.2f, Mote, Ice, k => at + Vector3.Down * k * 0.5f, k => 0.18f, k => Fade(k, 0.2f, 0.4f));
        }
        Shine(c, Vector3.Up, r * 1.5f, Ice.Main, age => 1.3f * Mathf.Max(0f, 1f - age / 0.6f));
    }

    private void RimeShackle(Cast c)
    {
        const float Grow = 0.18f, Shatter = 1.6f;
        for (var i = 0; i < 9; i++)
        {
            var way = Around(i * Mathf.Tau / 9f + Rand(-0.2f, 0.2f));
            var foot = way * Rand(0.35f, 0.6f);
            var tall = Rand(2.6f, 4f);
            var lean = new Basis(Vector3.Up.Cross(-way).Normalized(), Rand(0.15f, 0.4f));
            var delay = i * 0.012f;
            Prop(c, delay, Shatter - delay, ShardMesh, IceBlock, k =>
            {
                var t = k * (Shatter - delay);
                var grow = Out(Mathf.Min(t / Grow, 1f));
                return new Transform3D(lean * Basis.FromScale(new Vector3(1.4f, tall, 1.4f) * Mathf.Max(grow, 0.02f)), foot + Vector3.Up * 0.2f * tall * grow);
            });
        }
        Cue(c, 0f, () => Stage.Patch(At(c, Vector3.Zero), 1.5f, Shatter + 0.8f, FrostColor, FxGround.Frost));
        Flare(c, 0f, 0.25f, Star, Ice, Vector3.Up, 2.6f);
        Cue(c, Shatter, () => Stage.Explosion(At(c, Vector3.Up * 0.8f), 1.4f, FrostColor, FxBurst.Frost));
        for (var i = 0; i < 24; i++)
            Spark(c, Shatter, Rand(0.4f, 0.7f), Ice, Vector3.Up * Rand(0.3f, 1.5f), FxKit.RandomDirection() * Rand(4f, 8f) + Vector3.Up * 3f, Gravity, 0.08f, 0.03f);
    }

    private void FrozenExpanse(Cast c, float r)
    {
        const float Life = 3f;
        Globe(c, "energy", Shell, 0f, Life, Ice, k => Vector3.Zero, k => r * Out(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.02f, 0.1f) * 0.45f);
        Disc(c, 0f, Life, Swirl, Ice, Vector3.Zero, k => r, k => Fade(k, 0.05f, 0.1f) * 0.5f, spin: -2f);
        if (c.Follow != null)
        {
            var follow = c.Follow;
            Cue(c, 0f, () => Stage.Aura(follow, r, Life, FrostColor, FxGround.Frost));
        }
        else
            Cue(c, 0f, () => Stage.Patch(At(c, Vector3.Zero), r, Life, FrostColor, FxGround.Frost));
        var n = 0;
        for (var t = 0.15f; t < Life - 0.3f; t += 0.11f, n++)
        {
            var spot = Around(Rand(0f, Mathf.Tau)) * r * Rand(0.35f, 0.95f);
            Disc(c, t, 0.35f, Shock, Ice, spot, k => 1.1f * Out(k), k => 1f - k, band: 0.2f, gain: 1.4f);
            Flare(c, t, 0.22f, Star, Ice, spot + Vector3.Up * 0.4f, 1.6f);
            for (var j = 0; j < 4; j++)
                Spark(c, t, Rand(0.3f, 0.5f), Ice, spot + Vector3.Up * 0.2f, FxKit.Jitter(2f) + Vector3.Up * Rand(3f, 5f), Gravity, 0.06f, 0.03f);
            if (n % 5 == 0)
                Cue(c, t, () => Stage.Explosion(At(c, spot), 1f, FrostColor, FxBurst.Frost));
        }
        for (var i = 0; i < 40; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * r * Mathf.Sqrt(Rand(0f, 1f)) + Vector3.Up * Rand(0.5f, 3f);
            var fall = new Vector3(Rand(0.3f, 0.8f), -Rand(0.6f, 1.2f), Rand(-0.3f, 0.3f));
            Flare(c, Rand(0f, Life - 1f), 1f, Mote, Ice, k => at + fall * k, k => 0.16f, k => Fade(k, 0.2f, 0.3f));
        }
        Shine(c, Vector3.Up, r * 1.3f, Ice.Main, age => 0.8f * Fade(age / Life, 0.05f, 0.1f) * (0.8f + 0.2f * Mathf.Sin(age * 35f)));
    }

    private void RimeRampart(Cast c, float r)
    {
        const float Stand = 3f;
        var across = new Vector3(0f, 0f, r * 0.7f);
        const int Blocks = 7;
        for (var i = 0; i < Blocks; i++)
        {
            var x = (i - (Blocks - 1) / 2f) * r * 0.32f;
            var at = across + Vector3.Right * x + Vector3.Back * Rand(-0.15f, 0.15f);
            var size = new Vector3(Rand(0.75f, 0.95f), Rand(1.4f, 1.9f), Rand(0.6f, 0.8f)) * r / 3f;
            var delay = Mathf.Abs(i - (Blocks - 1) / 2f) * 0.05f;
            var turn = new Basis(Vector3.Up, Rand(-0.2f, 0.2f)) * new Basis(Vector3.Back, Rand(-0.08f, 0.08f));
            Prop(c, delay, Stand - delay, _block, IceBlock, k =>
            {
                var t = k * (Stand - delay);
                var rise = Out(Mathf.Min(t / 0.18f, 1f));
                var sink = Mathf.Max(0f, (t - (Stand - delay - 0.4f)) / 0.4f);
                return new Transform3D(turn * Basis.FromScale(size * (1f - sink * 0.9f)), at + Vector3.Up * size.Y * (rise - 0.5f));
            });
            Spark(c, delay, 0.4f, Ice, at + Vector3.Up * 0.2f, new Vector3(Rand(-2f, 2f), Rand(3f, 6f), Rand(-2f, 2f)), Gravity, 0.08f, 0.03f);
        }
        Disc(c, 0f, Stand, Glow, Ice, across, k => 1f, k => Fade(k, 0.05f, 0.15f) * 0.6f, stretch: new Vector3(r * 1.25f, 1f, 0.9f));
        Cue(c, 0f, () => Stage.Patch(At(c, across), r * 0.6f, Stand, FrostColor, FxGround.Frost));
        Cue(c, Stand - 0.4f, () => Stage.Explosion(At(c, across + Vector3.Up * 0.5f), r * 0.4f, FrostColor, FxBurst.Frost));
    }

    // ---- Storm.

    private void Skybolt(Cast c)
    {
        const float Strike = 0.35f;
        var cloud = Vector3.Up * 7f;
        Disc(c, 0f, Strike + 0.3f, Swirl, Storm, k => new Transform3D(Basis.FromScale(Vector3.One * 1.8f * Out(Mathf.Min(k * 3f, 1f))), cloud), k => Fade(k, 0.1f, 0.4f), spin: 6f, gain: 1.2f);
        Disc(c, 0f, Strike, Glow, Storm, Vector3.Zero, k => 1.2f, k => k * 0.6f);
        Cue(c, Strike, () => Stage.Lightning([(At(c, cloud), At(c, Vector3.Zero))], 0.2f, 10, 0.8f));
        Flare(c, Strike, 0.22f, Star, Storm, Vector3.Up * 0.3f, 2.6f);
        Disc(c, Strike, 0.35f, Shock, Storm, Vector3.Zero, k => 1.8f * Out(k), k => 1f - k, band: 0.18f, gain: 1.3f);
        Disc(c, Strike, 1f, Cracks, Storm, Vector3.Zero, k => 0.9f, k => Fade(k, 0.02f, 0.5f), k => k);
        Shine(c, Vector3.Up, 4f, Storm.Main, age => 1.8f * Bump(age, Strike, 0.3f));
    }

    private void HeavensWrath(Cast c, float r)
    {
        const float Charge = 0.6f;
        Column(c, 0f, Charge + 0.1f, Light, Storm, Vector3.Zero, k => new Vector2(0.7f, 3.2f), k => k * 0.9f, gain: 1.3f, speed: 2f);
        for (var i = 0; i < 18; i++)
        {
            var from = Around(Rand(0f, Mathf.Tau)) * Rand(0.4f, 0.9f) + Vector3.Up * Rand(0.1f, 0.5f);
            Spark(c, Rand(0f, Charge - 0.2f), 0.4f, Storm, from, Vector3.Up * Rand(4f, 7f), 0f, 0.05f, 0.05f);
        }
        Flare(c, Charge, 0.25f, Star, Storm, Vector3.Up * 1.4f, 3f);
        foreach (var spot in Spread(5, r * 1.6f))
        {
            Cue(c, Charge, () => Stage.Lightning([(At(c, spot) + Vector3.Up * 14f, At(c, spot))], 0.25f, 12, 1f));
            Disc(c, Charge, 0.35f, Shock, Storm, spot, k => 1.6f * Out(k), k => 1f - k, band: 0.18f, gain: 1.3f);
            Disc(c, Charge, 1f, Cracks, Storm, spot, k => 0.9f, k => Fade(k, 0.02f, 0.5f), k => k);
            Flare(c, Charge, 0.22f, Star, Storm, spot + Vector3.Up * 0.3f, 2.2f);
        }
        Shine(c, Vector3.Up, r * 2.5f, Storm.Main, age => age < Charge ? age / Charge * 0.8f : 2.4f * Mathf.Max(0f, 1f - (age - Charge) / 0.4f));
    }

    private void ShockLine(Cast c, float range)
    {
        const float Charge = 0.35f;
        var length = Mathf.Max(range, 2f);
        var hand = new Vector3(0f, 1.2f, 0.5f);
        var end = new Vector3(0f, 1.1f, length);
        Flare(c, 0f, Charge + 0.05f, Star, Cyan, k => hand, k => 0.4f + 1.8f * k * k, k => 1f);
        for (var i = 0; i < 12; i++)
        {
            var from = hand + FxKit.RandomDirection() * 1f;
            var start = Rand(0f, 0.15f);
            Line(c, start, Charge - start, Cyan, k => from.Lerp(hand, k * k), k => (hand - from).Normalized() * 0.4f, 0.04f, k => 1f);
        }
        Tube(c, Charge, 0.3f, Beam, Cyan, k => (hand, end), k => 0.18f * (1f - k), k => 1f, speed: 4f, gain: 1.6f);
        Bolt(c, Charge, 0.3f, Cyan, hand, end, 0.12f, 9, 0.35f);
        Cue(c, Charge + 0.08f, () => Stage.Explosion(At(c, end), 1.2f, FxTheme.Of(FxElement.Lightning), FxBurst.Lightning));
        Flare(c, Charge, 0.25f, Star, Cyan, end, 2.4f);
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Cyan.Main, age => 1.6f * Bump(age, Charge, 0.35f));
    }

    private void GaleBarrier(Cast c, float r)
    {
        const float Stand = 3f;
        const int Panels = 6;
        var bend = r * 0.8f;
        Func<float, Vector3> arc = a => new Vector3(Mathf.Sin(a) * bend, 0f, Mathf.Cos(a) * bend - bend * 0.4f);
        for (var i = 0; i < Panels; i++)
        {
            var (a, b) = (arc(-0.75f + 1.5f * i / Panels), arc(-0.75f + 1.5f * (i + 1) / Panels));
            Wall(c, 0f, Stand, 1, Wind, a, b, k => 2.4f * Out(Mathf.Min(k * Stand / 0.25f, 1f)), k => Fade(k, 0.02f, 0.06f),
                k => Mathf.Max(0f, (k * Stand - (Stand - 0.4f)) / 0.4f), speed: 1.5f, gain: 1.5f);
            Wall(c, 0f, Stand, 0, Wind, a, b, k => 2.4f * Out(Mathf.Min(k * Stand / 0.25f, 1f)), k => Fade(k, 0.02f, 0.06f) * 0.35f,
                k => Mathf.Max(0f, (k * Stand - (Stand - 0.4f)) / 0.4f));
        }
        for (var i = 0; i < 24; i++)
        {
            var at = arc(Rand(-0.75f, 0.75f));
            Spark(c, Rand(0f, Stand - 0.5f), 0.5f, Dust, at + Vector3.Up * 0.1f, new Vector3(Rand(-3f, 3f), Rand(1f, 3f), Rand(-1f, 1f)), Gravity * 0.4f, 0.08f, 0.05f);
        }
    }

    // ---- Blades.

    private void SteelCyclone(Cast c)
    {
        const float Life = 2.6f;
        Column(c, 0f, Life, Spiral, Steel, Vector3.Zero, k => new Vector2(1.15f, 2.3f), k => Fade(k, 0.05f, 0.1f), twist: 2.5f, speed: 3.5f, gain: 1.4f);
        Column(c, 0f, Life, Light, Steel, Vector3.Zero, k => new Vector2(1.05f, 2.2f), k => Fade(k, 0.05f, 0.1f) * 0.35f, speed: 2f);
        Disc(c, 0f, Life, Swirl, Dust, Vector3.Zero, k => 1.8f, k => Fade(k, 0.05f, 0.1f) * 0.7f, spin: -8f);
        for (var i = 0; i < 48; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var reach = Rand(0.6f, 1.35f);
            var high = Rand(0.2f, 2.1f);
            Func<float, Vector3> at = k => Around(angle - k * 6f) * reach + Vector3.Up * high;
            Line(c, Rand(0f, Life - 0.3f), 0.3f, Steel, at, k => (at(Mathf.Min(k + 0.05f, 1f)) - at(k)).Normalized() * 0.7f, 0.06f, k => Fade(k, 0.2f, 0.3f));
        }
        for (var i = 0; i < 24; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Rand(0f, Life - 0.3f), 0.3f, Steel, way * 1.15f + Vector3.Up * Rand(0.3f, 1.8f), new Vector3(-way.Z, 0f, way.X) * 8f + way * 3f, Gravity * 0.5f, 0.05f, 0.03f);
        }
        Shine(c, Vector3.Up, 3f, Steel.Main, age => 0.7f * Fade(age / Life, 0.05f, 0.1f));
    }

    private void ThousandSteps(Cast c, float r)
    {
        const float Every = 0.12f;
        var stops = Spread(6, r);
        var previous = Vector3.Zero;
        for (var i = 0; i < stops.Length; i++)
        {
            var at = stops[i];
            var when = Every * (i + 1);
            var from = previous + Vector3.Up * 0.9f;
            var to = at + Vector3.Up * 0.9f;
            Line(c, when - 0.06f, 0.18f, Steel, k => from.Lerp(to, 0.5f), k => (to - from) * (1f - k * 0.5f), 0.08f, k => 1f - k);
            Globe(c, "energy", Shell, when, 0.4f, Steel, k => to, k => 1f, k => (1f - k) * 0.8f, mesh: _ghost);
            var turn = new Basis(Vector3.Up, Rand(0f, Mathf.Tau)) * new Basis(Vector3.Back, Rand(-0.8f, 0.8f));
            Crescent(c, when, 0.28f, new Transform3D(turn, to - turn * Vector3.Back * 1.2f), 1.3f, 120f, Steel, thick: 0.3f, sweep: 0.3f, gain: 1.4f);
            Flare(c, when, 0.2f, Star, Steel, to, 1.6f);
            previous = at;
        }
        var end = Every * (stops.Length + 1);
        var chest = Vector3.Up * 1.3f;
        foreach (var roll in new[] { 45f, -45f })
        {
            var basis = roll > 0f ? new Basis(Vector3.Back, Mathf.DegToRad(roll)) : new Basis(Vector3.Back, Mathf.DegToRad(roll)) * Basis.FromScale(new Vector3(-1f, 1f, 1f));
            Crescent(c, end + (roll > 0f ? 0f : 0.06f), 0.35f, new Transform3D(basis, chest), r * 0.6f, 130f, Steel, thick: 0.3f, sweep: 0.25f, gain: 1.5f);
        }
        Disc(c, end, 0.4f, Shock, Steel, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.14f);
        Shine(c, Vector3.Up, r * 1.5f, Steel.Main, age => 0.6f + Bump(age, end, 0.4f));
    }

    private void GaleThrust(Cast c, float range)
    {
        const float Thrust = 0.07f;
        var length = Mathf.Max(range, 2f);
        var from = new Vector3(0f, 1.05f, 0.3f);
        var reachTo = Mathf.Min(length * 0.4f, 3f);
        Func<float, float> reach = k => Out(Mathf.Min(k * 0.26f / Thrust, 1f));
        Line(c, 0f, 0.26f, Wind, k => from + Vector3.Back * reachTo * reach(k) * 0.5f, k => Vector3.Back * reachTo * reach(k), 0.22f, k => 1f - k * k);
        Flare(c, Thrust, 0.2f, Star, Wind, from + Vector3.Back * reachTo, 2f);
        Disc(c, Thrust, 0.3f, Shock, Wind, from + Vector3.Back * reachTo, k => 1.2f * Out(k), k => 1f - k, basis: new Basis(Vector3.Right, Mathf.Pi / 2f), band: 0.18f, gain: 1.3f);
        Fly(c, 0.12f, 0.9f, FxShotLook.Tornado, k => new Vector3(0f, Fx.ShotHeight, 0.8f + (length - 0.8f) * k), 0.75f,
            k => Mathf.SmoothStep(0f, 1f, Mathf.Clamp(Mathf.Min(k * 0.9f / 0.15f, (1f - k) * 0.9f / 0.25f), 0f, 1f)));
    }

    private void GaleStep(Cast c, float range)
    {
        const float Arrive = 0.18f;
        var length = Mathf.Max(range, 1f);
        Func<float, Vector3> path = s => new Vector3(0f, 0.9f, length * s);
        Ribbon(c, 0f, Arrive + 0.3f, 0, Wind, path, k => { var head = Mathf.Min(k * (Arrive + 0.3f) / Arrive, 1f); return (Mathf.Max(0f, head - 0.6f) + Mathf.Max(0f, k - 0.6f) * 1f, head); }, 1.4f, k => 1f - k * k, gain: 1.5f);
        for (var i = 0; i < 12; i++)
        {
            var at = new Vector3(Rand(-0.6f, 0.6f), Rand(0.3f, 1.7f), Rand(0.3f, length));
            var len = Rand(1.2f, 2.6f);
            Line(c, at.Z / length * Arrive, 0.25f, Wind, k => at + Vector3.Back * k * 0.5f, k => Vector3.Back * len * (1f - k * 0.5f), 0.05f, k => 1f - k);
        }
        foreach (var (at, when) in new[] { (Vector3.Zero, 0f), (new Vector3(0f, 0f, length), Arrive) })
        {
            Disc(c, when, 0.3f, Shock, Wind, at, k => 1.5f * Out(k), k => 1f - k, band: 0.2f, gain: 1.2f);
            for (var j = 0; j < 6; j++)
                Flare(c, when, 0.5f, Mote, Dust, k => at + Vector3.Up * (0.2f + k * 0.4f), k => 0.6f + k, k => (1f - k) * 0.35f, gain: 0.7f);
        }
    }

    private void SkywardCut(Cast c)
    {
        var high = Vector3.Up * 2.3f;
        Column(c, 0f, 0.5f, Spiral, Wind, Vector3.Zero, k => new Vector2(0.9f, 3f * Out(Mathf.Min(k * 4f, 1f))), k => 1f - k, twist: 1.6f, speed: 3f, gain: 1.4f);
        Disc(c, 0f, 0.4f, Shock, Wind, Vector3.Zero, k => 2f * Out(k), k => 1f - k, band: 0.18f);
        for (var i = 0; i < 6; i++)
        {
            var when = 0.22f + i * 0.07f;
            var turn = new Basis(Vector3.Up, Rand(0f, Mathf.Tau)) * new Basis(Vector3.Back, Rand(-1f, 1f));
            if (i % 2 == 1)
                turn *= Basis.FromScale(new Vector3(-1f, 1f, 1f));
            Crescent(c, when, 0.25f, new Transform3D(turn, high + FxKit.Jitter(0.25f) - turn * Vector3.Back * 1.1f), 1.2f, 130f, Wind, thick: 0.3f, sweep: 0.3f, gain: 1.4f);
            Flare(c, when + 0.03f, 0.15f, Star, Wind, high + FxKit.Jitter(0.4f), 1.3f);
        }
        const float Finish = 0.7f;
        var chop = new Transform3D(new Basis(Vector3.Up, Mathf.Pi / 2f) * new Basis(Vector3.Back, Mathf.Pi / 2f), Vector3.Up * 1.8f + Vector3.Forward * 0.2f);
        Crescent(c, Finish, 0.35f, chop, 2.2f, 160f, Wind, thick: 0.4f, sweep: 0.25f, gain: 1.5f);
        Disc(c, Finish + 0.06f, 0.4f, Shock, Wind, Vector3.Zero, k => 2.4f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        Disc(c, Finish + 0.06f, 1f, Cracks, Wind, Vector3.Zero, k => 1.2f, k => Fade(k, 0.02f, 0.5f), k => k);
        Shine(c, high, 4f, Wind.Main, age => 0.5f + Bump(age, Finish, 0.4f));
    }

    private void ChainSweep(Cast c, float r)
    {
        var swing = new Transform3D(new Basis(Vector3.Forward, Mathf.DegToRad(-8f)), new Vector3(0f, 1f, -r * 0.1f));
        Crescent(c, 0f, 0.4f, swing, r * 0.95f, 150f, Soul, thick: 0.14f, sweep: 0.25f, gain: 1.6f);
        Crescent(c, 0.02f, 0.35f, swing, r * 0.88f, 140f, Soul with { Main = Soul.Hot }, thick: 0.05f, sweep: 0.27f, gain: 1.4f);
        for (var i = 0; i < 12; i++)
        {
            var angle = Mathf.DegToRad(70f - 140f * i / 11f);
            var at = swing * (Around(angle) * r * 0.95f);
            Flare(c, 0.4f * 0.25f * i / 11f, 0.25f, Star, Soul, at, 0.7f);
        }
        Shine(c, new Vector3(0f, 1f, r * 0.6f), r * 1.5f, Soul.Main, age => 1.2f * Mathf.Max(0f, 1f - age / 0.4f));
    }

    // ---- Earth.

    private void RivenRidge(Cast c, float range)
    {
        const float Stand = 3f;
        var length = Mathf.Max(range, 2f);
        var rocks = Mathf.Max(4, (int)(length / 0.55f));
        for (var i = 0; i < rocks; i++)
        {
            var z = 0.8f + (length - 0.8f) * i / (rocks - 1);
            var at = new Vector3(Rand(-0.2f, 0.2f), 0f, z);
            var size = new Vector3(Rand(1.3f, 1.7f), Rand(1.3f, 2f), Rand(1.3f, 1.7f));
            var delay = z / length * 0.3f;
            var tilt = new Basis(Vector3.Up, Rand(0f, Mathf.Tau)) * new Basis(Vector3.Right, Rand(-0.15f, 0.15f));
            var crag = Crags[i % Crags.Length];
            // Thrusting up out of the ground, then sinking back into it.
            Prop(c, delay, Stand - delay, crag, _stone, k =>
            {
                var t = k * (Stand - delay);
                var rise = Out(Mathf.Min(t / 0.15f, 1f));
                var sink = Mathf.Max(0f, (t - (Stand - delay - 0.5f)) / 0.5f);
                return new Transform3D(tilt * Basis.FromScale(size), at + Vector3.Up * size.Y * (rise - 1f - sink * 0.9f));
            });
            Spark(c, delay, Rand(0.4f, 0.7f), Rubble, at + Vector3.Up * 0.2f, new Vector3(Rand(-2f, 2f), Rand(4f, 7f), Rand(-1f, 1f)), Gravity * 1.2f, 0.12f, 0.02f);
            Flare(c, delay, 0.8f, Mote, Dust, k => at + Vector3.Up * (0.3f + k * 0.6f), k => 0.8f + k, k => (1f - k) * 0.4f, gain: 0.7f);
        }
        Disc(c, 0f, Stand, Cracks, Dust, new Vector3(0f, 0f, (0.8f + length) / 2f), k => 1f, k => Fade(k, 0.03f, 0.2f), k => k * 0.6f,
            stretch: new Vector3(0.9f, 1f, (length - 0.8f) / 2f + 0.5f));
    }

    private void TremorRing(Cast c)
    {
        Disc(c, 0f, 0.4f, Shock, Dust, Vector3.Zero, k => 1.9f * Out(k), k => 1f - k, band: 0.2f, gain: 1.3f);
        Disc(c, 0f, 0.9f, Cracks, Dust, Vector3.Zero, k => 1.1f, k => Fade(k, 0.02f, 0.5f), k => k);
        for (var i = 0; i < 8; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Flare(c, 0f, 0.6f, Mote, Dust, k => way * 1.3f * Out(k) + Vector3.Up * 0.25f, k => 0.6f + 0.6f * k, k => (1f - k) * 0.4f, gain: 0.7f);
        }
    }

    private void ResoundingSlam(Cast c, float r)
    {
        Disc(c, 0f, 0.5f, Shock, Dust, Vector3.Zero, k => r * 0.7f * Out(k), k => 1f - k, band: 0.18f, gain: 1.4f);
        Disc(c, 0f, 1.6f, Cracks, Sun, Vector3.Zero, k => r * 0.5f, k => Fade(k, 0.02f, 0.4f), k => k);
        Flare(c, 0f, 0.2f, Star, Dust, Vector3.Up * 0.4f, 2.6f);
        for (var i = 0; i < 14; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, 0f, Rand(0.4f, 0.7f), Rubble, way * 0.4f, way * Rand(2f, 5f) + Vector3.Up * Rand(4f, 7f), Gravity * 1.2f, 0.13f, 0.02f);
        }
        var stops = Spread(5, r * 1.4f);
        for (var i = 0; i < stops.Length; i++)
        {
            var spot = stops[i];
            var arrive = 0.15f + spot.Length() / (r * 1.4f) * 0.2f;
            Line(c, 0.05f, arrive - 0.05f, Sun, k => spot * k + Vector3.Up * 0.3f, k => spot.Normalized() * 1.2f, 0.14f, k => 1f);
            Disc(c, arrive, 0.35f, Shock, Sun, spot, k => 1.4f * Out(k), k => 1f - k, band: 0.2f, gain: 1.3f);
            Disc(c, arrive + 0.15f, 0.35f, Shock, Dust, spot, k => 2f * Out(k), k => (1f - k) * 0.7f, band: 0.2f);
            Flare(c, arrive, 0.2f, Star, Sun, spot + Vector3.Up * 0.4f, 1.8f);
        }
        Shine(c, Vector3.Up * 0.5f, r * 1.8f, Dust.Main, age => 1.3f * Mathf.Max(0f, 1f - age / 0.6f));
    }

    private void Thornline(Cast c, float range)
    {
        var length = Mathf.Max(range, 2f);
        var spikes = Mathf.Max(4, (int)(length / 0.7f));
        for (var i = 0; i < spikes; i++)
        {
            var s = (float)i / (spikes - 1);
            var at = new Vector3(Rand(-0.35f, 0.35f), 0f, 0.8f + (length - 0.8f) * s);
            var delay = at.Z / FxTiming.RuptureSpeed;
            var height = Mathf.Lerp(0.6f, 1.7f, s) * Rand(0.85f, 1.1f);
            Cue(c, 0f, () => Stage.Spike(At(c, at), c.Root.Basis * Vector3.Back, height, FxSpike.Stone, delay));
            Flare(c, delay, 0.7f, Mote, Dust, k => at + Vector3.Up * (0.3f + k * 0.5f), k => 0.6f + k * 0.8f, k => (1f - k) * 0.4f, gain: 0.7f);
            Spark(c, delay, 0.5f, Rubble, at + Vector3.Up * 0.2f, new Vector3(Rand(-2f, 2f), Rand(3f, 6f), Rand(0f, 2f)), Gravity * 1.2f, 0.11f, 0.02f);
        }
        Disc(c, 0f, 1.4f, Cracks, Dust, new Vector3(0f, 0f, (0.8f + length) / 2f), k => 1f, k => Fade(k, 0.03f, 0.4f), k => k,
            stretch: new Vector3(0.8f, 1f, (length - 0.8f) / 2f + 0.4f));
    }

    private void LandslideCharge(Cast c, float r, float range)
    {
        const float Arrive = 0.6f;
        var length = Mathf.Max(range, 1.5f);
        var size = 0.75f;
        Func<float, Vector3> at = k => new Vector3(0f, size, length * k);
        Prop(c, 0f, Arrive, _rock, _stone, k => new Transform3D(new Basis(Vector3.Right, k * Arrive * 14f) * Basis.FromScale(Vector3.One * size), at(k)));
        Globe(c, "energy", Shell, 0f, Arrive, Dust, at, k => size * 1.3f, k => 0.8f);
        for (var i = 0; i < 14; i++)
        {
            var s = (i + 0.5f) / 14f;
            var spot = FxKit.Flat(at(s)) + Vector3.Right * Rand(-0.5f, 0.5f);
            Flare(c, s * Arrive, 0.8f, Mote, Dust, k => spot + Vector3.Up * (0.3f + k * 0.6f), k => 0.7f + k, k => (1f - k) * 0.45f, gain: 0.7f);
        }
        var end = new Vector3(0f, 0f, length);
        Cue(c, Arrive, () => Stage.Explosion(At(c, end + Vector3.Up * 0.4f), r * 0.6f, Dust.Main, FxBurst.Dome));
        for (var i = 0; i < 7; i++)
        {
            var way = Around(i * Mathf.Tau / 7f + Rand(-0.2f, 0.2f));
            Cue(c, Arrive, () => Stage.Spike(At(c, end + way * r * 0.5f), c.Root.Basis * way, Rand(0.9f, 1.4f), FxSpike.Stone, 0.02f));
        }
        Disc(c, Arrive, 0.5f, Shock, Dust, end, k => r * Out(k), k => 1f - k, band: 0.18f, gain: 1.4f);
        for (var i = 0; i < 20; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Arrive, Rand(0.5f, 0.8f), Rubble, end + way * 0.4f, way * Rand(2f, 5f) + Vector3.Up * Rand(5f, 9f), Gravity * 1.2f, 0.14f, 0.02f);
        }
    }

    // ---- Arcane, time and the fey.

    private void ConcussiveWave(Cast c, float range)
    {
        const float Life = 0.75f;
        var length = Mathf.Max(range, 2f);
        var force = new Tone(new(0.6f, 0.78f, 1f), new(0.95f, 0.98f, 1f), new(0.1f, 0.18f, 0.45f));
        Crescent(c, 0f, Life, k => new Transform3D(Basis.Identity, new Vector3(0f, 0.35f, -2.2f + (length - 0.5f) * Out(k))), 3f, 110f, force, thick: 0.35f, sweep: 0.08f, gain: 1.5f);
        Crescent(c, 0.04f, Life - 0.04f, k => new Transform3D(Basis.Identity, new Vector3(0f, 0.9f, -2.5f + (length - 0.5f) * Out(k))), 3f, 100f, force, thick: 0.18f, sweep: 0.08f, gain: 1.2f);
        for (var i = 0; i < 24; i++)
        {
            var s = Rand(0.1f, 1f);
            var at = new Vector3(Rand(-1.6f, 1.6f), 0.2f, 0.8f + (length - 0.8f) * Out(s));
            Spark(c, s * Life * 0.6f, 0.45f, Dust, at, new Vector3(Rand(-1f, 1f), Rand(2f, 4f), Rand(3f, 6f)), Gravity * 0.6f, 0.1f, 0.04f);
        }
    }

    private void MendingTotem(Cast c, float r)
    {
        const float Life = 3.6f;
        Prop(c, 0f, Life, TotemMesh, TotemLight, k =>
        {
            var t = k * Life;
            var rise = Out(Mathf.Min(t / 0.3f, 1f));
            var sink = Mathf.Max(0f, (t - (Life - 0.3f)) / 0.3f);
            return new Transform3D(Basis.FromScale(Vector3.One * 1.8f), Vector3.Up * (-1.8f + 1.8f * rise - 1.8f * sink));
        });
        Disc(c, 0f, Life, Glow, Verdant, Vector3.Zero, k => 1.2f, k => Fade(k, 0.05f, 0.1f) * 0.7f);
        for (var p = 0.4f; p < Life - 0.3f; p += 0.8f)
        {
            Disc(c, p, 0.7f, Shock, Verdant, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
            Flare(c, p, 0.25f, Star, Verdant, Vector3.Up * 1.62f, 1.8f);
        }
        for (var i = 0; i < 24; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * Rand(0.2f, r * 0.8f);
            Flare(c, Rand(0.3f, Life - 1f), 0.9f, Mote, Verdant, k => at + Vector3.Up * (0.2f + k * 1.6f), k => 0.2f, k => Fade(k, 0.2f, 0.4f));
        }
    }

    private void RewindStep(Cast c, float range)
    {
        const float Arrive = 0.2f;
        var length = Mathf.Max(range, 1f);
        for (var i = 0; i < 5; i++)
        {
            var at = new Vector3(0f, 0.85f, length * (i + 1) / 6f);
            Globe(c, "energy", Shell, at.Z / length * Arrive, 0.55f, Chrono, k => at, k => 1f, k => (1f - k) * 0.85f, mesh: _ghost);
        }
        Ribbon(c, 0f, Arrive + 0.35f, 0, Chrono, s => new Vector3(0f, 0.9f, length * s), k => { var head = Mathf.Min(k * (Arrive + 0.35f) / Arrive, 1f); return (Mathf.Max(0f, k - 0.5f) * 2f, head); }, 1f, k => 1f - k, gain: 1.3f);
        foreach (var (at, when) in new[] { (Vector3.Zero, 0f), (new Vector3(0f, 0f, length), Arrive) })
        {
            Disc(c, when, 0.6f, Runes, Chrono, at, k => 1.2f, k => 1f - k, k => k * 3f, gain: 1.3f);
            for (var h = 0; h < 2; h++)
            {
                var hand = h;
                Line(c, when, 0.6f, Chrono, k =>
                {
                    var a = (hand == 0 ? -k * 9f : -k * 2f);
                    return at + Vector3.Up * 0.1f + Around(a) * (hand == 0 ? 0.45f : 0.3f);
                }, k => Around(hand == 0 ? -k * 9f : -k * 2f) * (hand == 0 ? 0.9f : 0.6f), 0.07f, k => 1f - k);
            }
        }
    }

    private void StilledMoment(Cast c)
    {
        var chest = Vector3.Up;
        Flare(c, 0f, 0.25f, Star, Chrono, chest, 3f);
        Disc(c, 0.05f, 1.4f, Runes, Chrono, k => new Transform3D(Basis.FromScale(Vector3.One * 1f), chest), k => Fade(k, 0.05f, 0.2f), k => k * 1.4f / 0.2f, gain: 1.4f);
        Disc(c, 0.05f, 1.4f, Band, Chrono, k => new Transform3D(Basis.FromScale(Vector3.One * 1.15f), chest + Vector3.Up * 0.5f), k => Fade(k, 0.05f, 0.2f), gain: 1.2f);
        Disc(c, 0.05f, 1.4f, Band, Chrono, k => new Transform3D(Basis.FromScale(Vector3.One * 1.15f), chest - Vector3.Up * 0.5f), k => Fade(k, 0.05f, 0.2f), gain: 1.2f);
        Globe(c, "energy", Shell, 0.05f, 1.4f, Chrono, k => Vector3.Up * 0.85f, k => 1.05f, k => Fade(k, 0.05f, 0.2f) * 0.6f, mesh: _ghost);
        for (var i = 0; i < 14; i++)
        {
            var at = chest + FxKit.RandomDirection() * Rand(0.5f, 1.1f);
            Flare(c, 0.05f, 1.4f, Mote, Chrono, k => at, k => 0.16f, k => Fade(k, 0.1f, 0.2f));
        }
    }

    private void StoppedHour(Cast c, float r)
    {
        const float Life = 3.5f;
        var wide = r * 1.3f;
        Globe(c, "energy", Hexes, 0f, Life, Chrono, k => Vector3.Zero, k => wide * Overshoot(Mathf.Min(k * Life / 0.22f, 1f)), k => Fade(k, 0.02f, 0.08f), gain: 1.1f);
        Disc(c, 0f, Life, Runes, Chrono, Vector3.Zero, k => wide, k => Fade(k, 0.02f, 0.08f), k => k * Life / 0.35f, gain: 1.2f);
        Disc(c, 0f, 0.3f, Shock, Chrono, Vector3.Zero, k => wide * Out(k), k => 1f - k, band: 0.1f, gain: 1.4f);
        for (var h = 0; h < 2; h++)
        {
            var hand = h;
            var reach = wide * (hand == 0 ? 0.5f : 0.35f);
            Func<float, Vector3> way = k => Around(-k * Life * (hand == 0 ? 0.9f : 0.15f));
            Line(c, 0.1f, Life - 0.2f, Chrono, k => Vector3.Up * 0.12f + way(k) * reach * 0.5f, k => way(k) * reach, hand == 0 ? 0.1f : 0.16f, k => Fade(k, 0.05f, 0.08f));
        }
        for (var i = 0; i < 30; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * wide * 0.85f * Mathf.Sqrt(Rand(0f, 1f)) + Vector3.Up * Rand(0.4f, wide * 0.6f);
            var star = i % 4 == 0;
            Flare(c, Rand(0.1f, 0.4f), Life - 0.5f, star ? Star : Mote, Chrono, k => at, k => star ? 0.6f : 0.2f, k => Fade(k, 0.05f, 0.1f));
        }
        Shine(c, Vector3.Up, wide * 1.3f, Chrono.Main, age => 0.6f * Fade(age / Life, 0.05f, 0.08f));
    }

    private void DriftingMote(Cast c, float range)
    {
        const float Life = 1.8f;
        var length = Mathf.Max(range, 2f);
        Func<float, Vector3> path = s => new Vector3(Mathf.Sin(s * 9f) * 0.15f, 1.2f + Mathf.Sin(s * 6f) * 0.1f, 0.5f + (length - 0.5f) * s);
        Globe(c, "energy", Orb, 0f, Life, Fey, path, k => 0.32f * Out(Mathf.Min(k * 10f, 1f)), k => 1f, gain: 1.3f);
        Flare(c, 0f, Life, Star, Fey, path, k => 1.2f + 0.2f * Mathf.Sin(k * 40f), k => 1f);
        Ribbon(c, 0f, Life, 0, Fey, path, k => (Mathf.Max(0f, k - 0.45f), k), 0.5f, k => 1f, gain: 1.3f);
        for (var i = 0; i < 26; i++)
        {
            var s = (i + 0.5f) / 26f;
            var at = path(s) + FxKit.Jitter(0.2f);
            Flare(c, s * Life, 0.7f, i % 3 == 0 ? Star : Mote, Fey, k => at + Vector3.Down * k * 0.3f, k => 0.25f, k => 1f - k);
        }
        Flare(c, Life, 0.3f, Star, Fey, path(1f), 2.4f);
    }

    private void FeyRift(Cast c, float range)
    {
        const float Appear = 0.1f;
        var length = Mathf.Max(range, 1f);
        foreach (var (at, when) in new[] { (Vector3.Zero, 0f), (new Vector3(0f, 0f, length), Appear) })
        {
            Disc(c, when, 0.45f, Runes, Fey, at, k => 2f, k => 1f - k, k => k * 4f, gain: 1.3f);
            Disc(c, when, 0.35f, Shock, Fey, at, k => 2.2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.2f);
            for (var i = 0; i < 18; i++)
            {
                var push = FxKit.RandomDirection() with { Y = Rand(0.2f, 1f) };
                var tone = Prism[i % Prism.Length] with { Main = Prism[i % Prism.Length].Main.Lerp(new Color(1f, 1f, 1f), 0.4f) };
                Flare(c, when, Rand(0.4f, 0.6f), i % 2 == 0 ? Star : Mote, tone, k => at + Vector3.Up * 0.9f + push.Normalized() * 1.6f * Out(k), k => 0.35f, k => 1f - k);
            }
        }
    }

    private void Dreamtether(Cast c, float r)
    {
        const float Hold = 2.2f;
        var heart = Vector3.Up * 1f;
        Globe(c, "energy", Orb, 0f, Hold, Fey, k => heart, k => 0.35f * Out(Mathf.Min(k * 8f, 1f)), k => 1f, gain: 1.3f);
        Column(c, 0f, Hold, Spiral, Fey, Vector3.Zero, k => new Vector2(0.5f, 1.6f), k => Fade(k, 0.05f, 0.1f), twist: 2f, speed: 2f, gain: 1.3f);
        Disc(c, 0f, Hold, Glow, Fey, Vector3.Zero, k => r, k => Fade(k, 0.05f, 0.1f) * 0.35f);
        foreach (var spot in Spread(4, r))
        {
            var end = spot + Vector3.Up * 1f;
            Tube(c, 0.05f, Hold - 0.05f, Spiral, Fey, k => (heart, end), k => 0.1f * (1f + 0.25f * Mathf.Sin(k * 60f)), k => Fade(k, 0.05f, 0f), twist: 6f, speed: 2f, gain: 1.4f);
            Flare(c, 0.05f, Hold - 0.05f, Mote, Fey, k => end, k => 0.5f, k => 1f);
            Flare(c, Hold, 0.25f, Star, Fey, end, 2f);
            for (var j = 0; j < 6; j++)
                Spark(c, Hold, 0.35f, Fey, end, FxKit.RandomDirection() * Rand(3f, 6f), 2f, 0.05f, 0.03f);
        }
        Disc(c, Hold, 0.35f, Shock, Fey, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
    }

    private void PolymorphPuff(Cast c)
    {
        for (var i = 0; i < 16; i++)
        {
            var push = FxKit.RandomDirection() with { Y = Rand(0f, 1f) };
            var tone = Prism[i % Prism.Length] with { Main = Prism[i % Prism.Length].Main.Lerp(new Color(1f, 1f, 1f), 0.5f) };
            var size = Rand(0.8f, 1.3f);
            Flare(c, Rand(0f, 0.05f), Rand(0.5f, 0.8f), Mote, tone, k => Vector3.Up * 0.9f + push.Normalized() * 0.8f * Out(k), k => size * (0.6f + 0.7f * k), k => (1f - k) * 0.7f, gain: 1f);
        }
        for (var i = 0; i < 12; i++)
        {
            var push = FxKit.RandomDirection();
            Flare(c, Rand(0.05f, 0.25f), 0.4f, Star, Fey, k => Vector3.Up * 0.9f + push * 1.3f * Out(k), k => 0.5f, k => Mathf.Sin(k * Mathf.Pi));
        }
        Disc(c, 0f, 0.35f, Shock, Fey, Vector3.Zero, k => 1.6f * Out(k), k => 1f - k, band: 0.2f);
    }

    private void ReturningOrb(Cast c, float range)
    {
        const float Life = 1.2f;
        var length = Mathf.Max(range, 2f);
        Func<float, Vector3> path = s => new Vector3(0.8f * Mathf.Sin(s * Mathf.Tau), 1.2f, 0.5f + (length - 0.5f) * Mathf.Sin(s * Mathf.Pi));
        var back = Azure with { Main = new Color(0.85f, 0.95f, 1f) };
        Globe(c, "energy", Orb, 0f, Life * 0.5f, Azure, k => path(k * 0.5f), k => 0.4f, k => 1f, gain: 1.4f);
        Globe(c, "energy", Orb, Life * 0.5f, Life * 0.5f, back, k => path(0.5f + k * 0.5f), k => 0.4f, k => 1f, gain: 1.5f);
        Flare(c, 0f, Life, Star, Azure, k => path(k), k => 1.4f, k => 1f);
        Ribbon(c, 0f, Life, 0, Azure, path, k => (Mathf.Max(0f, k - 0.3f), k), 0.6f, k => 1f, gain: 1.4f);
        Flare(c, Life * 0.5f, 0.25f, Star, back, path(0.5f), 2.4f);
    }

    private void SpiritWisps(Cast c, float r)
    {
        const float Orbit = 0.8f, Dart = 0.3f;
        var fox = new Tone(new(0.55f, 0.75f, 1f), new(1f, 1f, 1f), new(0.12f, 0.15f, 0.5f));
        var targets = Spread(3, r);
        for (var i = 0; i < 3; i++)
        {
            var phase = i * Mathf.Tau / 3f;
            var target = targets[i] + Vector3.Up * 1f;
            Func<float, Vector3> orbit = t => Around(phase + t * 6f) * 0.9f + Vector3.Up * 1.3f;
            var from = orbit(Orbit);
            Func<float, Vector3> at = k =>
            {
                var t = k * (Orbit + Dart);
                if (t < Orbit)
                    return orbit(t);
                var s = (t - Orbit) / Dart;
                return from.Lerp(target, s * s) + Vector3.Up * Mathf.Sin(s * Mathf.Pi) * 0.8f;
            };
            Globe(c, "energy", Orb, 0f, Orbit + Dart, fox, at, k => 0.18f, k => 1f, gain: 1.4f);
            Flare(c, 0f, Orbit + Dart, Mote, fox, at, k => 0.7f, k => 1f);
            Ribbon(c, 0f, Orbit + Dart, 0, fox, s => at(Mathf.Clamp(s, 0f, 1f)), k => (Mathf.Max(0f, k - 0.18f), k), 0.3f, k => 1f, gain: 1.2f);
            Flare(c, Orbit + Dart, 0.22f, Star, fox, target, 1.8f);
        }
    }

    private void BeguilingMote(Cast c, float range)
    {
        const float Flight = 0.6f;
        var length = Mathf.Max(range, 2f);
        Func<float, Vector3> at = k => new Vector3(0f, 1.2f, 0.5f + (length - 0.5f) * k);
        Globe(c, "energy", Orb, 0f, Flight, Rose, at, k => 0.3f, k => 1f, gain: 1.4f);
        Disc(c, 0f, Flight, Runes, Rose, k => new Transform3D(new Basis(Vector3.Up, k * 8f) * Basis.FromScale(Vector3.One * 0.6f), at(k)), k => 1f, k => 1f, gain: 1.3f);
        for (var i = 0; i < 14; i++)
        {
            var s = (i + 0.5f) / 14f;
            var spot = at(s);
            Flare(c, s * Flight, 0.5f, Mote, Rose, k => spot + Vector3.Down * k * 0.3f, k => 0.22f, k => 1f - k);
        }
        var end = at(1f);
        Flare(c, Flight, 0.3f, Star, Rose, end, 2.6f);
        Disc(c, Flight, 0.4f, Shock, Rose, FxKit.Flat(end), k => 1.8f * Out(k), k => 1f - k, band: 0.18f, gain: 1.3f);
        for (var i = 0; i < 12; i++)
        {
            var push = FxKit.RandomDirection();
            Flare(c, Flight, 0.5f, Mote, Rose, k => end + push * 1.2f * Out(k), k => 0.3f, k => 1f - k);
        }
    }

    private void SpiritSteps(Cast c, float range)
    {
        const float Dash = 0.12f, Every = 0.35f;
        var length = Mathf.Max(range, 1.5f);
        var fox = new Tone(new(0.55f, 0.75f, 1f), new(1f, 1f, 1f), new(0.12f, 0.15f, 0.5f));
        for (var i = 0; i < 3; i++)
        {
            var (z0, z1) = (length * i / 3f, length * (i + 1) / 3f);
            var start = i * Every;
            var swerve = (i - 1) * 0.6f;
            Func<float, Vector3> path = s => new Vector3(swerve * Mathf.Sin(s * Mathf.Pi), 0.9f, Mathf.Lerp(z0, z1, s));
            Ribbon(c, start, Dash + 0.25f, 0, fox, path, k => { var head = Mathf.Min(k * (Dash + 0.25f) / Dash, 1f); return (Mathf.Max(0f, k - 0.45f) * 1.8f, head); }, 1f, k => 1f - k, gain: 1.4f);
            var stop = path(1f);
            foreach (var target in Spread(3, 3f))
            {
                var to = stop + Vector3.Back * 2.5f + target * 0.8f;
                Flare(c, start + Dash, 0.3f, Mote, fox, k => stop.Lerp(to, k), k => 0.6f, k => 1f, gain: 1.6f);
                Line(c, start + Dash, 0.3f, fox, k => stop.Lerp(to, Mathf.Max(k - 0.1f, 0f)), k => (to - stop).Normalized() * 1.1f, 0.14f, k => 1f);
                Flare(c, start + Dash + 0.3f, 0.2f, Star, fox, to, 1.8f);
            }
            Disc(c, start + Dash, 0.3f, Shock, fox, FxKit.Flat(stop), k => 1.3f * Out(k), k => 1f - k, band: 0.2f);
        }
    }

    private void ArcaneBolt(Cast c, float range)
    {
        const float Flight = 0.25f;
        var length = Mathf.Max(range, 2f);
        var from = new Vector3(0f, 1.2f, 0.5f);
        var to = new Vector3(0f, 1.1f, length);
        Flare(c, 0f, Flight, Mote, Gold, k => from.Lerp(to, k), k => 0.9f, k => 1f, gain: 1.8f);
        Flare(c, 0f, Flight, Star, Gold, k => from.Lerp(to, k), k => 1.2f, k => 1f);
        Line(c, 0f, Flight, Gold, k => from.Lerp(to, Mathf.Max(k - 0.06f, 0f)), k => (to - from).Normalized() * 2f, 0.24f, k => 1f);
        Flare(c, 0f, 0.12f, Star, Gold, from, 1.4f);
        Flare(c, Flight, 0.22f, Star, Gold, to, 2f);
        for (var i = 0; i < 10; i++)
            Spark(c, Flight, Rand(0.2f, 0.35f), Gold, to, FxKit.RandomDirection() * Rand(4f, 7f), 3f, 0.05f, 0.03f);
    }

    private void ShiftShot(Cast c, float range)
    {
        const float Appear = 0.08f, Flight = 0.3f;
        var length = Mathf.Max(range, 1f);
        var land = new Vector3(0f, 0f, length);
        foreach (var (at, when) in new[] { (Vector3.Zero, 0f), (land, Appear) })
        {
            Flare(c, when, 0.25f, Star, Gold, at + Vector3.Up * 0.9f, 2.2f);
            Disc(c, when, 0.3f, Shock, Gold, at, k => 1.4f * Out(k), k => 1f - k, band: 0.2f, gain: 1.2f);
            for (var i = 0; i < 10; i++)
            {
                var push = FxKit.RandomDirection();
                Flare(c, when, 0.4f, Mote, Gold, k => at + Vector3.Up * 0.9f + push * 0.9f * Out(k), k => 0.25f, k => 1f - k);
            }
        }
        var from = land + Vector3.Up * 1.2f;
        var to = land + new Vector3(1.5f, 1f, 3f);
        var bend = from + new Vector3(-1.5f, 1.2f, 1.5f);
        Func<float, Vector3> arc = s => from * (1f - s) * (1f - s) + bend * 2f * s * (1f - s) + to * s * s;
        Flare(c, Appear + 0.05f, Flight, Mote, Gold, arc, k => 0.5f, k => 1f, gain: 1.6f);
        Ribbon(c, Appear + 0.05f, Flight + 0.15f, 0, Gold, arc, k => { var head = Mathf.Min(k * (Flight + 0.15f) / Flight, 1f); return (Mathf.Max(0f, head - 0.35f), head); }, 0.35f, k => 1f, gain: 1.4f);
        Flare(c, Appear + 0.05f + Flight, 0.22f, Star, Gold, to, 1.8f);
    }

    // ---- Holy.

    private void SolarBrand(Cast c, float r)
    {
        const float Narrow = 1.4f;
        Disc(c, 0f, Narrow, Band, Gold, k => Lay(Vector3.Zero, Mathf.Lerp(r * 0.7f, r * 0.3f, k * k) / 0.85f), k => 0.35f + 0.4f * k, gain: 1.1f);
        Disc(c, 0f, Narrow, Glow, Gold, Vector3.Zero, k => r * 0.4f, k => k * 0.4f);
        Column(c, Narrow, 0.55f, Light, Gold, Vector3.Zero, k => new Vector2(r * 0.12f * (1f + k), 18f), k => (1f - k) * (1f - k), gain: 2.4f, speed: -3f);
        Column(c, Narrow, 0.4f, Light, Gold with { Main = Gold.Hot }, Vector3.Zero, k => new Vector2(r * 0.05f, 18f), k => 1f - k, gain: 2.4f, speed: -4f);
        Flare(c, Narrow, 0.35f, Star, Gold, Vector3.Up * 0.5f, 4f);
        Disc(c, Narrow, 0.45f, Shock, Gold, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.14f, gain: 1.5f);
        Disc(c, Narrow, 1.4f, Cracks, Gold, Vector3.Zero, k => r * 0.45f, k => Fade(k, 0.02f, 0.4f), k => k);
        for (var i = 0; i < 18; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Narrow, Rand(0.4f, 0.7f), Gold, way * 0.2f, way * Rand(1f, 3f) + Vector3.Up * Rand(7f, 12f), Gravity, 0.06f, 0.035f);
        }
        Shine(c, Vector3.Up, r * 1.6f, Gold.Main, age => age < Narrow ? 0.15f : 2.4f * Mathf.Max(0f, 1f - (age - Narrow) / 0.5f));
    }

    private void BindOfDawn(Cast c, float range)
    {
        const float Flight = 0.4f, Hold = 1.4f;
        var length = Mathf.Max(range, 2f);
        var dawn = Gold with { Main = new Color(1f, 0.92f, 0.6f) };
        var from = new Vector3(0f, 1.2f, 0.5f);
        var to = new Vector3(0f, 1f, length);
        Globe(c, "energy", Orb, 0f, Flight, dawn, k => from.Lerp(to, k), k => 0.32f, k => 1f, gain: 1.4f);
        Flare(c, 0f, Flight, Star, dawn, k => from.Lerp(to, k), k => 1.2f, k => 1f);
        Line(c, 0f, Flight, dawn, k => from.Lerp(to, Mathf.Max(k - 0.08f, 0f)), k => Vector3.Back * 1.6f, 0.2f, k => 1f);
        Flare(c, Flight, 0.25f, Star, dawn, to, 2.6f);
        foreach (var high in new[] { -0.55f, 0f, 0.55f })
        {
            var around = to + Vector3.Up * high;
            Disc(c, Flight, Hold, Band, dawn, k => new Transform3D(Basis.FromScale(Vector3.One * Mathf.Lerp(1.4f, 0.6f, Out(Mathf.Min(k * Hold / 0.2f, 1f)))), around),
                k => Fade(k, 0.02f, 0.2f), gain: 1.5f);
        }
        Column(c, Flight, Hold, Light, dawn, FxKit.Flat(to), k => new Vector2(0.6f, 2.2f), k => Fade(k, 0.05f, 0.2f) * 0.5f);
    }

    private void PrismWard(Cast c, float range)
    {
        const float Life = 1f;
        var length = Mathf.Max(range, 2f);
        Func<float, Vector3> path = s => new Vector3(0f, 1.2f, 0.5f + (length - 0.5f) * Mathf.Sin(s * Mathf.Pi));
        Flare(c, 0f, Life, Star, Prism[0] with { Main = new Color(1f, 1f, 1f) }, path, k => 1.3f, k => 1f);
        for (var i = 0; i < 30; i++)
        {
            var s = (i + 0.5f) / 30f;
            var spot = path(s) + FxKit.Jitter(0.15f);
            Flare(c, s * Life, 0.45f, Mote, Prism[i % Prism.Length], k => spot, k => 0.3f, k => 1f - k, gain: 1.4f);
        }
        var turn = path(0.5f);
        Globe(c, "energy", Hexes, Life * 0.5f, 0.6f, Gold, k => FxKit.Flat(turn), k => 1.2f * Out(Mathf.Min(k * 5f, 1f)), k => 1f - k);
        Globe(c, "energy", Hexes, Life, 0.8f, Gold, k => Vector3.Zero, k => 1.3f * Out(Mathf.Min(k * 5f, 1f)), k => 1f - k);
        Flare(c, Life * 0.5f, 0.25f, Star, Gold, turn, 2.2f);
    }

    private void LingeringStar(Cast c, float r, float range)
    {
        const float Toss = 0.4f, Burst = 2f;
        var length = Mathf.Max(range, 2f);
        var land = new Vector3(0f, 0f, length * 0.8f);
        var from = new Vector3(0f, 1.3f, 0.5f);
        Func<float, Vector3> arc = s => from.Lerp(land + Vector3.Up * 0.3f, s) + Vector3.Up * 2f * s * (1f - s) * 2f;
        Flare(c, 0f, Toss, Star, Gold, arc, k => 1.4f, k => 1f);
        Globe(c, "energy", Orb, 0f, Burst, Gold, k => k * Burst < Toss ? arc(k * Burst / Toss) : land + Vector3.Up * 0.3f, k => 0.25f, k => 1f, gain: 1.4f);
        var zone = 1.9f;
        Disc(c, Toss, Burst - Toss, Glow, Gold, land, k => zone, k => 0.5f + 0.2f * Mathf.Sin(k * 30f));
        Disc(c, Toss, Burst - Toss, Band, Gold, k => Lay(land, zone / 0.85f), k => 0.8f + 0.2f * Mathf.Sin(k * 30f), gain: 1.3f);
        for (var i = 0; i < 16; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var reach = Rand(0.4f, zone);
            Flare(c, Toss + Rand(0f, 0.6f), Burst - Toss - 0.6f, Mote, Gold, k => land + Around(angle + k * 3f) * reach + Vector3.Up * 0.3f, k => 0.2f, k => Fade(k, 0.1f, 0.1f));
        }
        Cue(c, Burst, () => Stage.Explosion(At(c, land + Vector3.Up * 0.3f), zone, FxTheme.Of(FxElement.Holy), FxBurst.Dome));
        Flare(c, Burst, 0.3f, Star, Gold, land + Vector3.Up * 0.5f, 3.4f);
        Disc(c, Burst, 0.45f, Shock, Gold, land, k => zone * 1.4f * Out(k), k => 1f - k, band: 0.14f, gain: 1.4f);
        for (var i = 0; i < 16; i++)
            Spark(c, Burst, Rand(0.4f, 0.6f), Gold, land + Vector3.Up * 0.3f, FxKit.RandomDirection() * Rand(5f, 9f) + Vector3.Up * 2f, Gravity * 0.6f, 0.06f, 0.035f);
        Shine(c, land + Vector3.Up, zone * 2f, Gold.Main, age => age < Burst ? 0.5f : 2f * Mathf.Max(0f, 1f - (age - Burst) / 0.4f));
    }

    private void RadiantLance(Cast c, float range)
    {
        const float Aim = 1f, Fire1 = 0.65f;
        var length = Mathf.Max(range, 3f);
        var hand = new Vector3(0f, 1.2f, 0.5f);
        var end = new Vector3(0f, 1.2f, length);
        var radiant = Gold with { Main = new Color(1f, 0.93f, 0.7f) };
        Line(c, 0f, Aim, radiant, k => (hand + end) * 0.5f, k => end - hand, 0.05f, k => 0.25f + 0.5f * k);
        Flare(c, 0f, Aim, Star, radiant, k => hand, k => 0.4f + 1.6f * k * k, k => 1f);
        Func<float, float> open = k => Out(Mathf.Min(k * Fire1 / 0.06f, 1f)) * (1f - Mathf.SmoothStep(0f, 1f, (k - 0.6f) / 0.4f));
        Tube(c, Aim, Fire1, Beam, radiant with { Main = new Color(1f, 0.8f, 0.4f) }, k => (hand, end), k => 0.7f * open(k), k => 1f, speed: 3f, gain: 0.55f);
        Tube(c, Aim, Fire1, Beam, radiant with { Main = new Color(1f, 1f, 1f) }, k => (hand, end), k => 0.22f * open(k), k => 1f, speed: 4f, gain: 1.2f);
        Flare(c, Aim, Fire1, Star, radiant, k => hand, k => 3.4f * open(k), k => 1f);
        for (var i = 0; i < 40; i++)
        {
            var at = hand.Lerp(end, Rand(0f, 1f)) + FxKit.Jitter(0.8f);
            var tone = Prism[i % Prism.Length];
            Flare(c, Aim + Rand(0f, Fire1 * 0.6f), 0.35f, Star, tone, k => at, k => 0.5f, k => Mathf.Sin(k * Mathf.Pi));
        }
        Disc(c, Aim, Fire1 + 1f, Glow, radiant, new Vector3(0f, 0f, (hand.Z + length) / 2f), k => 1f, k => Fade(k, 0.02f, 0.5f) * 0.7f,
            stretch: new Vector3(1.4f, 1f, (length - hand.Z) / 2f));
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, radiant.Main, age => age < Aim ? 0.2f : 2.4f * open(Mathf.Min((age - Aim) / Fire1, 1f)));
    }

    private void LongshotBarrage(Cast c, float range)
    {
        const float Charge = 0.8f, Travel = 0.7f;
        var length = Mathf.Max(range, 3f);
        var hand = new Vector3(0f, 1.2f, 0.5f);
        Flare(c, 0f, Charge, Star, Gold, k => hand, k => 0.5f + 2f * k * k, k => 1f);
        Column(c, 0f, Charge, Light, Gold, Vector3.Zero, k => new Vector2(0.8f, 2.6f), k => k * 0.7f, gain: 1.3f);
        for (var i = 0; i < 16; i++)
        {
            var from = hand + FxKit.RandomDirection() * 1.6f;
            var start = Rand(0f, 0.4f);
            Flare(c, start, Charge - start, Mote, Gold, k => from.Lerp(hand, k * k), k => 0.22f, k => 1f);
        }
        Func<float, float> z = k => 0.5f + (length + 2f) * k;
        Disc(c, Charge, Travel, Glow, Gold, k => new Transform3D(Basis.FromScale(new Vector3(2.4f, 1f, 1.2f)), new Vector3(0f, 0.6f, z(k))), k => Fade(k, 0.05f, 0.3f), gain: 1.6f);
        Crescent(c, Charge, Travel, k => new Transform3D(Basis.Identity, new Vector3(0f, 0.9f, z(k) - 2.8f)), 3.2f, 70f, Gold, thick: 0.25f, sweep: 0.05f, gain: 1.6f);
        for (var i = 0; i < 30; i++)
        {
            var off = new Vector3(Rand(-2f, 2f), Rand(0.3f, 1.8f), Rand(-1f, 0.5f));
            var len = Rand(1.5f, 3f);
            Line(c, Charge, Travel, Gold, k => off + Vector3.Back * z(k), k => Vector3.Back * len, 0.07f, k => Fade(k, 0.05f, 0.3f));
        }
        Disc(c, Charge, Travel + 0.6f, Glow, Gold, new Vector3(0f, 0f, (0.5f + length) / 2f), k => 1f, k => Fade(k, 0.02f, 0.6f) * 0.4f,
            stretch: new Vector3(2.2f, 1f, length / 2f));
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Gold.Main, age => age < Charge ? age / Charge : 2f * Mathf.Max(0f, 1f - (age - Charge) / Travel));
    }

    private void SwordOfVerdict(Cast c, float r)
    {
        const float Fall = 0.45f, Stand = 0.25f, Dissolve = 0.6f;
        const float Size = 4.8f;
        var high = Vector3.Up * 16f;
        var buried = Vector3.Down * Size * 0.15f;
        Add(c, SwordMesh, VerdictBlade, 0f, Fall + Stand + Dissolve, (n, k, t) =>
        {
            var fall = Mathf.Min(t / Fall, 1f);
            n.Transform = new Transform3D(new Basis(Vector3.Up, 0.6f) * Basis.FromScale(Vector3.One * Size), high.Lerp(buried, fall * fall));
            n.SetInstanceShaderParameter("uAlpha", Mathf.Min(1f, t / 0.08f));
            n.SetInstanceShaderParameter("uImpact", t < Fall ? 0f : Mathf.Max(0f, 1f - (t - Fall) / 0.2f));
            n.SetInstanceShaderParameter("uDissolve", Mathf.Clamp((t - Fall - Stand) / Dissolve, 0f, 1f));
        });
        Disc(c, 0f, Fall, Telegraph, Gold, Vector3.Zero, k => r * 0.6f, k => 0.7f, k => k);
        Column(c, Fall, 0.6f, Light, Gold, Vector3.Zero, k => new Vector2(r * 0.4f, 12f), k => (1f - k) * (1f - k), gain: 1.8f, speed: -2f);
        Cue(c, Fall, () => Stage.Explosion(At(c, Vector3.Up * 0.3f), r * 0.8f, FxTheme.Of(FxElement.Holy), FxBurst.Dome));
        Disc(c, Fall, 0.5f, Shock, Gold, Vector3.Zero, k => r * 1.3f * Out(k), k => 1f - k, band: 0.14f, gain: 1.5f);
        Disc(c, Fall, 1.6f, Cracks, Gold, Vector3.Zero, k => r * 0.6f, k => Fade(k, 0.02f, 0.4f), k => k);
        Flare(c, Fall, 0.3f, Star, Gold, Vector3.Up * 0.6f, 4.2f);
        for (var i = 0; i < 20; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Fall, Rand(0.5f, 0.8f), Rubble, way * 0.4f, way * Rand(3f, 6f) + Vector3.Up * Rand(4f, 8f), Gravity * 1.2f, 0.12f, 0.02f);
        }
        Shine(c, Vector3.Up, r * 1.8f, Gold.Main, age => age < Fall ? 0.3f : 2.2f * Mathf.Max(0f, 1f - (age - Fall) / 0.6f));
    }

    // ---- Shadow and soul.

    private void SunderingFinger(Cast c, float range)
    {
        const float Charge = 0.2f, Life = 0.4f;
        var length = Mathf.Max(range, 2f);
        var hand = new Vector3(0f, 1.3f, 0.5f);
        var foe = new Vector3(0f, 1.1f, length);
        Flare(c, 0f, Charge + 0.05f, Star, Doom, k => hand, k => 0.5f + 1.8f * k, k => 1f);
        Bolt(c, Charge, Life, Doom, hand, foe, 0.28f, 10, 0.5f);
        Bolt(c, Charge, Life * 0.8f, Doom with { Main = Doom.Hot }, hand, foe, 0.09f, 10, 0.3f);
        foreach (var side in new[] { -1f, 1f })
        {
            var fork = hand.Lerp(foe, 0.45f) + Vector3.Right * side * 0.6f + Vector3.Up * 0.3f;
            Bolt(c, Charge + 0.03f, Life * 0.7f, Doom, hand.Lerp(foe, 0.3f), fork, 0.12f, 4, 0.25f);
            Bolt(c, Charge + 0.03f, Life * 0.7f, Doom, fork, foe, 0.12f, 5, 0.25f);
        }
        for (var i = 0; i < 4; i++)
        {
            var angle = i * Mathf.Tau / 4f + 0.4f;
            var turn = new Basis(Vector3.Up, angle) * new Basis(Vector3.Back, Mathf.Pi / 2f);
            Crescent(c, Charge + 0.05f, 0.35f, new Transform3D(turn, foe + Around(angle) * 0.9f + Vector3.Up * 0.6f), 1.2f, 70f, Doom, thick: 0.3f, sweep: 0.3f, gain: 1.4f);
        }
        Cue(c, Charge + 0.1f, () => Stage.Explosion(At(c, foe), 1.6f, Doom.Main, FxBurst.Dome));
        Flare(c, Charge, 0.3f, Star, Doom, foe, 3.4f);
        Disc(c, Charge + 0.1f, 0.4f, Shock, Doom, FxKit.Flat(foe), k => 2.4f * Out(k), k => 1f - k, band: 0.18f, gain: 1.4f);
        Shine(c, foe, 4f, Doom.Main, age => 2f * Bump(age, Charge, 0.45f));
    }

    private void ReapingHook(Cast c, float range)
    {
        const float Out1 = 0.22f, Hold = 0.06f, Back1 = 0.25f, Life = Out1 + Hold + Back1;
        var hand = new Vector3(0f, 1.1f, 0.3f);
        var length = Mathf.Max(range - 0.3f, 1f);
        Func<float, float> reach = k =>
        {
            var t = k * Life;
            return t < Out1 ? Out(t / Out1) : t < Out1 + Hold ? 1f : 1f - Mathf.SmoothStep(0f, 1f, (t - Out1 - Hold) / Back1);
        };
        Func<float, Vector3> tip = k => hand + Vector3.Back * length * reach(k);
        Tube(c, 0f, Life, Spiral, Soul, k => (hand, tip(k) + Vector3.Back * 0.01f), k => 0.1f, k => 1f, twist: 14f, speed: 0.6f, gain: 1.5f);
        Tube(c, 0f, Life, Beam, Soul, k => (hand, tip(k) + Vector3.Back * 0.01f), k => 0.2f, k => 0.5f, speed: 2f, gain: 1f);
        var hook = new Basis(Vector3.Back, Mathf.Pi) * new Basis(Vector3.Right, Mathf.Pi / 2f);
        Crescent(c, Out1 - 0.05f, 0.3f, k => new Transform3D(hook, tip(Mathf.Min((Out1 - 0.05f + k * 0.3f) / Life, 1f)) + Vector3.Down * 0.4f), 0.8f, 160f, Soul, thick: 0.35f, sweep: 0.2f, gain: 1.5f);
        Flare(c, Out1, 0.25f, Star, Soul, hand + Vector3.Back * length, 2.4f);
        for (var i = 0; i < 10; i++)
            Spark(c, Out1, Rand(0.2f, 0.35f), Soul, hand + Vector3.Back * length, FxKit.RandomDirection() * Rand(4f, 8f), Gravity * 0.5f, 0.05f, 0.03f);
    }

    private void LanternToss(Cast c, float range)
    {
        const float Toss = 0.5f, Life = 3.2f;
        var length = Mathf.Max(range, 2f);
        var land = new Vector3(0f, 0f, length * 0.8f);
        var from = new Vector3(0f, 1.3f, 0.5f);
        Func<float, Vector3> arc = s => from.Lerp(land, s) + Vector3.Up * 2.4f * s * (1f - s) * 2f;
        Func<float, Vector3> at = k => k * Life < Toss ? arc(k * Life / Toss) : land;
        Prop(c, 0f, Life, LanternMesh, LanternBody, k => new Transform3D(new Basis(Vector3.Up, k * Life < Toss ? k * Life * 10f : 0f) * Basis.FromScale(Vector3.One * 1.5f), at(k)));
        Globe(c, "energy", Orb, 0f, Life, Soul, k => at(k) + Vector3.Up * 0.32f, k => 0.16f * (1f + 0.1f * Mathf.Sin(k * 60f)), k => Fade(k, 0f, 0.08f), gain: 1.6f);
        Flare(c, 0f, Life, Mote, Soul, k => at(k) + Vector3.Up * 0.32f, k => 1f, k => Fade(k, 0f, 0.08f));
        Column(c, Toss, Life - Toss, Light, Soul, land, k => new Vector2(0.6f, 2f), k => Fade(k, 0.05f, 0.1f) * 0.5f);
        Disc(c, Toss, Life - Toss, Glow, Soul, land, k => 1.4f, k => Fade(k, 0.05f, 0.1f) * 0.7f);
        Disc(c, Toss, 0.4f, Shock, Soul, land, k => 1.8f * Out(k), k => 1f - k, band: 0.18f, gain: 1.3f);
        for (var i = 0; i < 18; i++)
        {
            var spot = land + Around(Rand(0f, Mathf.Tau)) * Rand(0.2f, 1f);
            Flare(c, Rand(Toss, Life - 1f), 0.9f, Mote, Soul, k => spot + Vector3.Up * (0.2f + k * 1.5f), k => 0.18f, k => Fade(k, 0.2f, 0.4f));
        }
        Shine(c, land + Vector3.Up * 0.6f, 3f, Soul.Main, age => 0.7f * Fade(age / Life, 0.05f, 0.08f));
    }

    private void SpectralCage(Cast c, float r)
    {
        const float Stand = 3f;
        var corners = Enumerable.Range(0, 5).Select(i => Around(i * Mathf.Tau / 5f + Mathf.Pi / 5f) * r * 0.65f).ToArray();
        for (var i = 0; i < 5; i++)
        {
            var (a, b) = (corners[i], corners[(i + 1) % 5]);
            var delay = i * 0.03f;
            Wall(c, delay, Stand - delay, 0, Soul, a, b, k => 1.8f * Out(Mathf.Min(k * (Stand - delay) / 0.2f, 1f)), k => Fade(k, 0.02f, 0.05f),
                k => Mathf.Max(0f, (k * (Stand - delay) - (Stand - delay - 0.45f)) / 0.45f), speed: 1.2f, gain: 1.5f);
            Flare(c, delay, Stand - delay, Star, Soul, k => a + Vector3.Up * 1.8f * Out(Mathf.Min(k * (Stand - delay) / 0.2f, 1f)), k => 0.9f, k => Fade(k, 0.02f, 0.1f));
        }
        Disc(c, 0f, Stand, Glow, Soul, Vector3.Zero, k => r * 0.65f, k => Fade(k, 0.05f, 0.1f) * 0.4f);
        Disc(c, 0f, 0.35f, Shock, Soul, Vector3.Zero, k => r * 0.8f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
    }

    private void UmbralBind(Cast c, float range)
    {
        const float Flight = 0.45f, Hold = 1.5f;
        var length = Mathf.Max(range, 2f);
        var from = new Vector3(0f, 1.2f, 0.5f);
        var foe = new Vector3(0f, 1f, length);
        Globe(c, "void", 0, 0f, Flight, Void, k => from.Lerp(foe, k), k => 0.3f, k => 1f);
        Globe(c, "energy", Shell, 0f, Flight, Shadow, k => from.Lerp(foe, k), k => 0.45f, k => 1f);
        Line(c, 0f, Flight, Shadow, k => from.Lerp(foe, Mathf.Max(k - 0.08f, 0f)), k => Vector3.Back * 1.4f, 0.22f, k => 1f);
        Flare(c, Flight, 0.25f, Star, Shadow, foe, 2.4f);
        foreach (var angle in new[] { 0.3f, 2.4f, 4.5f })
        {
            var foot = FxKit.Flat(foe) + Around(angle) * 1.1f;
            Tube(c, Flight, Hold, Spiral, Shadow, k => (foot, foe + Vector3.Up * 0.2f), k => 0.09f * Out(Mathf.Min(k * Hold / 0.15f, 1f)), k => Fade(k, 0f, 0.15f), twist: 10f, speed: 0.6f, gain: 1.5f);
        }
        foreach (var high in new[] { -0.4f, 0.3f })
            Disc(c, Flight, Hold, Band, Shadow, k => new Transform3D(Basis.FromScale(Vector3.One * Mathf.Lerp(1.2f, 0.6f, Out(Mathf.Min(k * Hold / 0.15f, 1f)))), foe + Vector3.Up * high),
                k => Fade(k, 0.02f, 0.15f), gain: 1.5f);
        Disc(c, Flight, Hold, Runes, Hex, FxKit.Flat(foe), k => 1.2f, k => Fade(k, 0.02f, 0.15f), k => k * Hold / 0.25f, gain: 1.2f);
    }

    private void BlightPool(Cast c, float r)
    {
        const float Life = 3.2f;
        var pool = r * 0.8f;
        Disc(c, 0f, Life, Swirl, Shadow, Vector3.Zero, k => pool, k => Fade(k, 0.05f, 0.1f), spin: 1.5f, gain: 1.2f);
        Disc(c, 0f, Life, Glow, Shadow, Vector3.Zero, k => pool, k => Fade(k, 0.05f, 0.1f) * 0.7f);
        Disc(c, 0f, Life, Cracks, Hex, Vector3.Zero, k => pool * 0.9f, k => Fade(k, 0.05f, 0.1f) * 0.7f, k => 0.3f);
        for (var i = 0; i < 5; i++)
        {
            var home = Around(Rand(0f, Mathf.Tau)) * Rand(0.2f, pool * 0.7f);
            var phase = Rand(0f, Mathf.Tau);
            Globe(c, "energy", Shell, Rand(0f, 0.6f), Life - 0.6f, Shadow,
                k => home + new Vector3(Mathf.Sin(k * 7f + phase), 0f, Mathf.Cos(k * 5f + phase)) * 0.4f + Vector3.Up * (0.5f + 0.2f * Mathf.Sin(k * 11f + phase)),
                k => 0.6f, k => Fade(k, 0.15f, 0.15f) * 0.7f, mesh: _ghost);
        }
        for (var i = 0; i < 30; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * pool * Mathf.Sqrt(Rand(0f, 1f));
            Flare(c, Rand(0f, Life - 1f), 1f, Mote, Shadow, k => at + Vector3.Up * (0.1f + k * 1.4f), k => 0.2f, k => Fade(k, 0.2f, 0.4f));
        }
    }

    private void UmbralWard(Cast c)
    {
        const float Life = 2.6f;
        Globe(c, "energy", Hexes, 0f, Life, Shadow, k => Vector3.Zero, k => 1.25f * Overshoot(Mathf.Min(k * Life / 0.2f, 1f)), k => Fade(k, 0.02f, 0.1f), gain: 2f);
        Disc(c, 0f, Life, Runes, Shadow, Vector3.Zero, k => 1.35f, k => Fade(k, 0.02f, 0.1f), k => k * Life / 0.3f, gain: 1.2f);
        Disc(c, 0f, 0.35f, Shock, Shadow, Vector3.Zero, k => 2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        for (var i = 0; i < 8; i++)
        {
            var angle = i * Mathf.Tau / 8f;
            var high = 0.4f + 0.2f * (i % 4);
            Flare(c, 0.1f, Life - 0.2f, Mote, Shadow, k => Around(angle - k * Life * 1.5f) * 1.05f + Vector3.Up * high, k => 0.22f, k => Fade(k, 0.1f, 0.1f));
        }
    }

    private void SoulFetters(Cast c, float r)
    {
        const float Snap = 2.4f;
        var chest = Vector3.Up * 1.2f;
        Globe(c, "energy", Orb, 0f, Snap, Shadow, k => chest, k => 0.25f, k => 1f, gain: 1.3f);
        Disc(c, 0f, Snap, Runes, Shadow, Vector3.Zero, k => 1.2f, k => Fade(k, 0.02f, 0.05f), k => k * Snap / 0.3f, gain: 1.2f);
        foreach (var spot in Spread(4, r))
        {
            var end = spot + Vector3.Up * 1f;
            Tube(c, 0.05f, Snap - 0.05f, Spiral, Shadow, k => (chest, end), k => 0.08f + 0.06f * k, k => 0.35f + 0.65f * k, twist: 8f, speed: 1.5f, gain: 1.5f);
            Globe(c, "void", 0, 0.05f, Snap - 0.05f, Void, k => end, k => 0.2f + 0.1f * k, k => 1f);
            Globe(c, "void", 0, Snap, 0.3f, Void, k => end, k => 0.3f + 0.8f * Out(k), k => 1f - k);
            Flare(c, Snap, 0.25f, Star, Shadow, end, 2.2f);
            Disc(c, Snap, 0.35f, Shock, Shadow, spot, k => 1.4f * Out(k), k => 1f - k, band: 0.18f, gain: 1.3f);
        }
        Shine(c, chest, r * 1.4f, Shadow.Main, age => age < Snap ? 0.3f + 0.4f * age / Snap : 1.6f * Mathf.Max(0f, 1f - (age - Snap) / 0.35f));
    }
}
