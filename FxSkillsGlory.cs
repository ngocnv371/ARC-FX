using Godot;

namespace Arc.Fx;

/// <summary>
/// The fifth set's elements: fire with wings and fireworks, frost in snowflakes and
/// chandeliers, storms of stars, quiet water, crystal and gold out of the earth, and the
/// arcane (star maps, dials, prisms, an eclipse).
/// </summary>
internal sealed partial class FxSkills
{
    private ShaderMaterial? _amethystBlock, _jadeBlock, _amberBlock;
    private ArrayMesh[]? _boulders;
    private ArrayMesh? _geodeHalf;

    /// <summary>Lumps of rock: the first is the one a geode splits from.</summary>
    private ArrayMesh[] Boulders => _boulders ??= [FxProps.Boulder(0), FxProps.Boulder(1), FxProps.Boulder(2)];
    private ArrayMesh GeodeHalf => _geodeHalf ??= FxProps.Boulder(0, half: true);

    private ShaderMaterial AmethystBlock => _amethystBlock ??= FxKit.Crystal(new Color(0.55f, 0.3f, 0.9f), new Color(0.95f, 0.8f, 1f), rim: 0.8f, glow: 0.2f);
    private ShaderMaterial JadeBlock => _jadeBlock ??= FxKit.Crystal(new Color(0.15f, 0.7f, 0.5f), new Color(0.8f, 1f, 0.9f), rim: 0.7f, glow: 0.15f);
    private ShaderMaterial AmberBlock => _amberBlock ??= FxKit.Crystal(new Color(0.95f, 0.6f, 0.1f), new Color(1f, 0.95f, 0.6f), rim: 0.8f, glow: 0.25f);

    private void Glory(FxSkill skill, Cast c, float r, float range)
    {
        switch (skill)
        {
            case FxSkill.PhoenixRise: PhoenixRise(c, r); break;
            case FxSkill.SunFlare: SunFlare(c, r); break;
            case FxSkill.CandleVigil: CandleVigil(c, r); break;
            case FxSkill.EmberButterflies: EmberButterflies(c, r); break;
            case FxSkill.FireworkBloom: FireworkBloom(c, r); break;
            case FxSkill.LavaLotus: LavaLotus(c, r); break;
            case FxSkill.CometDance: CometDance(c, r); break;
            case FxSkill.DragonSpiral: DragonSpiral(c, r); break;
            case FxSkill.CinderFall: CinderFall(c, r); break;
            case FxSkill.SnowfallSanctum: SnowfallSanctum(c, r); break;
            case FxSkill.FrostLotus: FrostLotus(c, r); break;
            case FxSkill.ChandelierDrop: ChandelierDrop(c, r); break;
            case FxSkill.AuroraVeil: AuroraVeil(c, r); break;
            case FxSkill.DiamondDust: DiamondDust(c, r); break;
            case FxSkill.MirrorShards: MirrorShards(c, r); break;
            case FxSkill.WinterWaltz: WinterWaltz(c, r); break;
            case FxSkill.FrozenStar: FrozenStar(c, r); break;
            case FxSkill.FrostHeart: FrostHeart(c, r); break;
            case FxSkill.StarCall: StarCall(c, r); break;
            case FxSkill.ThunderHalo: ThunderHalo(c, r); break;
            case FxSkill.VoltLattice: VoltLattice(c, r); break;
            case FxSkill.StormWings: StormWings(c, r); break;
            case FxSkill.PlasmaBloom: PlasmaBloom(c, r); break;
            case FxSkill.BallLightningTrio: BallLightningTrio(c, r); break;
            case FxSkill.ZephyrRibbons: ZephyrRibbons(c, r); break;
            case FxSkill.TempestCrown: TempestCrown(c, r); break;
            case FxSkill.LotusPond: LotusPond(c, r); break;
            case FxSkill.RainDance: RainDance(c, r); break;
            case FxSkill.KoiStream: KoiStream(c, r); break;
            case FxSkill.PearlBarrage: PearlBarrage(c, r); break;
            case FxSkill.MistVeil: MistVeil(c, r); break;
            case FxSkill.MoonTide: MoonTide(c, r); break;
            case FxSkill.BubbleStream: BubbleStream(c, r); break;
            case FxSkill.FountainOfLife: FountainOfLife(c, r); break;
            case FxSkill.CrystalGarden: CrystalGarden(c, r); break;
            case FxSkill.GeodeBurst: GeodeBurst(c, r); break;
            case FxSkill.JadePillars: JadePillars(c, r); break;
            case FxSkill.GoldenFissure: GoldenFissure(c, r); break;
            case FxSkill.DuneRoller: DuneRoller(c, r); break;
            case FxSkill.PebbleOrbit: PebbleOrbit(c, r); break;
            case FxSkill.AmberTomb: AmberTomb(c, r); break;
            case FxSkill.StarMap: StarMap(c, r); break;
            case FxSkill.ClockworkRewind: ClockworkRewind(c, r); break;
            case FxSkill.PrismRay: PrismRay(c, range); break;
            case FxSkill.GravityLens: GravityLens(c, r); break;
            case FxSkill.EyeOfInsight: EyeOfInsight(c, r); break;
            case FxSkill.CharmHeart: CharmHeart(c, range); break;
            case FxSkill.RuneCascade: RuneCascade(c, r); break;
            case FxSkill.NebulaBurst: NebulaBurst(c, r); break;
            case FxSkill.UmbralEclipse: UmbralEclipse(c, r); break;
            case FxSkill.TimeShatter: TimeShatter(c, r); break;
            case FxSkill.WishingStars: WishingStars(c, r); break;
            default: Legend(skill, c, r, range); break;
        }
    }

    // ---- Fire.

    private void PhoenixRise(Cast c, float r)
    {
        const float Burst = 0.3f, Life = 2.4f;
        var spot = new Vector3(0f, 0f, r * 0.15f);
        Mark(c, 0f, Life, Sunburst, Flame, spot, _ => r * 0.8f, k => Fade(k, 0.04f, 0.3f), count: 16f, spin: 2f, gain: 1.3f);
        Column(c, 0f, Burst + 0.5f, Fire, Flame, spot, k => new Vector2(r * 0.3f, 5f * Out(Mathf.Min(k * 3f, 1f))), k => 1f - k * k, gain: 1.6f, speed: 2.5f);
        Cue(c, Burst, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.5f), r * 0.7f, FireColor, FxBurst.Fire));
        // The bird: wings of flame opening as it climbs, a bright body and a streaming tail.
        Func<float, Vector3> body = k => spot + Vector3.Up * (1f + 2.6f * Out(k));
        var face = Stand(Vector3.Back);
        var life = Life - Burst;
        Mark(c, Burst, life, Wings, Flame, k => new Transform3D(face * Basis.FromScale(Vector3.One * 3.3f), body(k) + Vector3.Up * 0.5f), k => Fade(k, 0.02f, 0.25f),
            k => Overshoot(Mathf.Min(k * life / 0.4f, 1f)) * (0.93f + 0.07f * Mathf.Sin(k * life * 7f)), count: 9f, gain: 1.8f);
        Mark(c, Burst, life, Wings, Gold, k => new Transform3D(face * Basis.FromScale(Vector3.One * 2.3f), body(k) + new Vector3(0f, 0.4f, 0.05f)), k => Fade(k, 0.02f, 0.25f) * 0.8f,
            k => Out(Mathf.Min(k * life / 0.5f, 1f)), count: 6f, gain: 1.5f);
        Globe(c, "energy", Orb, Burst, life, Sun, body, _ => 0.4f, k => Fade(k, 0.02f, 0.25f), gain: 1.7f);
        Flare(c, Burst, life, Star, Sun, k => body(k) + Vector3.Up * 0.5f, _ => 2.2f, k => Fade(k, 0.02f, 0.3f));
        Tube(c, Burst, life, Fire, Flame, k => (body(k) + Vector3.Down * 2.2f, body(k)), _ => 0.3f, k => Fade(k, 0.05f, 0.3f), speed: -2f, gain: 1.5f);
        Drift(c, 34, Burst, Life - 1.2f, 1.2f, Plume, [Flame, Gold, Ember], () => spot + new Vector3(Rand(-2.6f, 2.6f), Rand(2f, 4.2f), Rand(-0.4f, 0.4f)), () => new Vector3(0f, Rand(-1.8f, -1f), 0f), 0.5f, sway: 0.4f, slim: 0.42f, gain: 1.7f);
        Spray(c, Burst, Flame, spot + Vector3.Up * 0.5f, 30, 6f, 8f, 0.7f);
        Disc(c, Burst, 0.5f, Shock, Flame, spot, k => r * 1.3f * Out(k), k => 1f - k, band: 0.14f, gain: 1.5f);
        Shine(c, spot + Vector3.Up * 2f, r * 1.8f, Flame.Main, age => 1.8f * Fade(age / Life, 0.1f, 0.3f));
    }

    private void SunFlare(Cast c, float r)
    {
        const float Swell = 0.9f, After = 0.8f;
        var sun = new Vector3(0f, 1.6f, r * 0.2f);
        var ground = FxKit.Flat(sun);
        Mark(c, 0f, Swell + After, Corona, Sun, ground, k => r * (0.8f + 0.5f * k), k => Fade(k, 0.08f, 0.3f), spin: 3f, count: 12f, gain: 1.4f);
        Mark(c, 0f, Swell, Sunburst, Sun, ground, _ => r * 0.5f, k => Fade(k, 0.08f, 0.1f), count: 12f, spin: 4f, gain: 1.2f);
        Globe(c, "energy", Orb, 0f, Swell, Sun, _ => sun, k => 0.25f + 0.55f * k * k, _ => 1f, gain: 1.7f);
        Mark(c, 0f, Swell, Corona, Flame, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (0.6f + 1.2f * k * k) * 1.9f), sun), k => Fade(k, 0.1f, 0.05f), spin: 4f, count: 10f, gain: 1.6f);
        for (var i = 0; i < 24; i++)
        {
            var from = sun + FxKit.RandomDirection() * Rand(2f, 3.4f);
            Flare(c, Rand(0f, Swell - 0.35f), 0.35f, Mote, Sun, k => from.Lerp(sun, k * k), k => 0.3f * (1f - 0.5f * k), _ => 1f);
        }
        Cue(c, Swell, () => Stage.Explosion(At(c, sun), r * 0.9f, Sun.Main, FxBurst.Fire));
        Hit(c, Swell, Sun, sun, r);
        Mark(c, Swell, After, Rays, Sun, ground, _ => r * 1.7f, k => 1f - k, k => Out(Mathf.Min(k * 3f, 1f)), count: 20f, spin: 1f, gain: 1.6f);
        Mark(c, Swell, 0.5f, Rays, Gold, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * 4f), sun), k => 1f - k, k => Out(k), count: 16f, gain: 1.6f);
        Twinkle(c, 20, Swell, Swell + 0.4f, Gold, () => sun + FxKit.RandomDirection() * Rand(0.6f, 2.6f), 0.7f);
        Shine(c, sun, r * 2f, Sun.Main, age => age < Swell ? 0.5f + 1f * age / Swell : 3f * Mathf.Max(0f, 1f - (age - Swell) / After));
    }

    private void CandleVigil(Cast c, float r)
    {
        const int Candles = 9;
        const float Light1 = 0.11f, Flare1 = Candles * Light1 + 0.3f, After = 0.9f;
        var ring = r * 0.7f;
        Mark(c, 0f, Flare1 + After, Filigree, Flame, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), count: 9f, spin: 1f, gain: 1.2f);
        Mark(c, 0f, Flare1 + After, Pentacle, Ember, Vector3.Zero, _ => ring, k => Fade(k, 0.04f, 0.3f) * 0.8f, k => Mathf.Min(k * (Flare1 + After) / Flare1, 1f), count: 9f, spin: 0.5f);
        for (var i = 0; i < Candles; i++)
        {
            var spot = Around(i * Mathf.Tau / Candles) * ring;
            var start = i * Light1;
            var life = Flare1 - start;
            Glyph(c, start, life, Wisp, Flame, k => spot + Vector3.Up * 0.55f, k => 0.9f * Overshoot(Mathf.Min(k * life / 0.15f, 1f)), _ => 1f, gain: 1.8f);
            Flare(c, start, life, Mote, Flame, _ => spot + Vector3.Up * 0.4f, _ => 1f, _ => 0.6f, gain: 1.1f);
            Glyph(c, start, 0.25f, Glint, Gold, spot + Vector3.Up * 0.6f, 1.1f, 1.6f);
            Disc(c, start, life + After, Glow, Flame, spot, _ => 0.55f, k => Fade(k, 0.05f, 0.3f) * 0.8f);
            Column(c, Flare1, After, Fire, Flame, spot, k => new Vector2(0.34f, 3.2f * Out(Mathf.Min(k * 5f, 1f))), k => 1f - k * k, gain: 1.6f, speed: 2.4f);
        }
        Cue(c, Flare1, () => Stage.Patch(At(c, Vector3.Zero), ring * 1.1f, 2f, FireColor, FxGround.Fire));
        Disc(c, Flare1, 0.5f, Shock, Flame, Vector3.Zero, k => r * 1.3f * Out(k), k => 1f - k, band: 0.14f, gain: 1.5f);
        Flare(c, Flare1, 0.3f, Star, Flame, Vector3.Up * 0.8f, 4f);
        Spray(c, Flare1, Flame, Vector3.Up * 0.3f, 30, 5f, 7f, 0.6f);
        Shine(c, Vector3.Up, r * 1.6f, Flame.Main, age => age < Flare1 ? 0.3f + 0.9f * age / Flare1 : 2.4f * Mathf.Max(0f, 1f - (age - Flare1) / After));
    }

    private void EmberButterflies(Cast c, float r)
    {
        const float Life = 1.5f;
        var reach = r * 1.6f;
        var from = new Vector3(0f, 1.1f, 0.3f);
        Tone[] wings = [Flame, Ember, Gold];
        Flare(c, 0f, 0.3f, Star, Flame, from, 2.4f);
        for (var i = 0; i < 30; i++)
        {
            var to = new Vector3(Rand(-1f, 1f) * r * 0.55f, Rand(0.4f, 1.8f), reach * Rand(0.7f, 1.1f));
            var bend = new Vector3(Rand(-1.4f, 1.4f), Rand(-0.2f, 1.2f), 0f);
            var start = Rand(0f, 0.55f);
            var size = Rand(0.45f, 0.75f);
            var flutter = Rand(0f, Mathf.Tau);
            var tone = wings[i % wings.Length];
            Func<float, Vector3> at = k => from.Lerp(to, k) + bend * Mathf.Sin(k * Mathf.Pi) + Vector3.Up * 0.18f * Mathf.Sin(k * 22f + flutter);
            Glyph(c, start, 0.85f, Flutter, tone, at, _ => size, k => Fade(k, 0.12f, 0.1f), k => Vector3.Up + Heading(at, k, 0.5f), gain: 1.9f);
            Trail(c, start, 0.85f, tone, at, 0.2f, 0.25f, style: 1, gain: 1.3f);
            // Each burns out in a small pop where it lands.
            var pop = start + 0.85f;
            Glyph(c, pop, 0.3f, Glint, Gold, to, 1.3f, 1.7f);
            Disc(c, pop, 0.3f, Shock, Flame, FxKit.Flat(to), k => 0.7f * Out(k), k => 1f - k, band: 0.1f);
            for (var j = 0; j < 3; j++)
                Spark(c, pop, 0.35f, Flame, to, FxKit.Jitter(2.5f) + Vector3.Up * 2f, Gravity * 0.6f, 0.05f, 0.04f);
        }
        Cue(c, 0.8f, () => Stage.Patch(At(c, new Vector3(0f, 0f, reach * 0.9f)), r * 0.55f, 1.6f, FireColor, FxGround.Fire));
        Shine(c, new Vector3(0f, 1f, reach * 0.6f), reach, Flame.Main, age => 1.3f * Fade(age / Life, 0.1f, 0.3f));
    }

    private void FireworkBloom(Cast c, float r)
    {
        const int Rockets = 5;
        const float Climb = 0.45f;
        var spots = Spread(Rockets, r * 0.75f);
        for (var i = 0; i < Rockets; i++)
        {
            var tone = Prism[(i * 2 + 1) % Prism.Length];
            var foot = spots[i];
            var top = foot + new Vector3(Rand(-0.4f, 0.4f), Rand(2.6f, 4.2f), Rand(-0.4f, 0.4f));
            var start = i * 0.28f;
            Func<float, Vector3> at = k => foot.Lerp(top, Out(k));
            Flare(c, start, Climb, Star, Gold, at, _ => 0.7f, _ => 1f);
            Trail(c, start, Climb, Gold, at, 0.16f, 0.5f);
            Disc(c, start, 0.3f, Shock, Gold, foot, k => 0.6f * Out(k), k => 1f - k, band: 0.1f);
            var pop = start + Climb;
            Flare(c, pop, 0.3f, Star, tone, top, 3.4f);
            // A ball of stars thrown out evenly, sagging as they fade.
            for (var j = 0; j < 30; j++)
            {
                var way = FxKit.RandomDirection();
                var speed = Rand(3.2f, 4f);
                var life = Rand(0.8f, 1.1f);
                Func<float, Vector3> star = k => top + way * speed * life * Out(k) * 0.6f + Vector3.Down * 1.2f * k * k;
                Line(c, pop, life, tone, star, k => Heading(star, k, 0.5f * (1f - k) + 0.1f), 0.06f, k => 1f - k * k);
                if (j % 3 == 0)
                    Glyph(c, pop + life * 0.55f, 0.4f, Glint, j % 2 == 0 ? Pearl : tone, star(0.8f), 0.9f, 1.8f);
            }
            Mark(c, pop, 0.6f, Rays, tone, FxKit.Flat(top), _ => 2.2f, k => (1f - k) * 0.7f, k => Out(k), count: 14f);
            Shine(c, top, r, tone.Main, age => 2f * Bump(age, pop, 0.7f));
        }
    }

    private void LavaLotus(Cast c, float r)
    {
        const float Open = 0.6f, Erupt = 1.1f, After = 1f;
        const int Petals = 8;
        Mark(c, 0f, Erupt + After, Lotus, Magma, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.04f, 0.3f), k => Mathf.Min(k * (Erupt + After) / Open, 1f), count: 8f, spin: 0.6f, gain: 1.5f);
        Disc(c, 0f, Erupt + After, Cracks, Magma, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.04f, 0.4f), k => k * 0.7f);
        Disc(c, 0f, Erupt, Glow, Magma, Vector3.Zero, _ => r * 0.8f, k => 0.2f + 0.6f * k);
        for (var i = 0; i < Petals; i++)
        {
            var way = Around(i * Mathf.Tau / Petals);
            var life = Erupt + After - i * 0.03f;
            Func<float, float> open = k => Overshoot(Mathf.Min(k * life / Open, 1f));
            Glyph(c, i * 0.03f, life, Leaflet, i % 2 == 0 ? Magma : Flame, k => way * r * 0.42f * open(k) + Vector3.Up * 0.35f, k => r * 0.72f * open(k), k => Fade(k, 0.03f, 0.25f), _ => way, gain: 1.6f, slim: 0.55f);
            var tip = way * r * 0.82f;
            Column(c, Erupt + i * 0.03f, After, Fire, Flame, tip, k => new Vector2(0.36f, 3f * Out(Mathf.Min(k * 5f, 1f))), k => 1f - k * k, gain: 1.6f, speed: 2.4f);
            Disc(c, Erupt + i * 0.03f, 0.35f, Shock, Flame, tip, k => 1f * Out(k), k => 1f - k, band: 0.1f);
        }
        Column(c, Erupt, After, Fire, Magma, Vector3.Zero, k => new Vector2(r * 0.3f, 5f * Out(Mathf.Min(k * 4f, 1f))), k => 1f - k * k, gain: 1.7f, speed: 2.6f);
        Cue(c, Erupt, () => Stage.Explosion(At(c, Vector3.Up * 0.4f), r * 0.7f, FireColor, FxBurst.Fire));
        Cue(c, Erupt, () => Stage.Patch(At(c, Vector3.Zero), r * 0.85f, 2.5f, FireColor, FxGround.Inferno));
        Spray(c, Erupt, Magma, Vector3.Up * 0.3f, 40, 6f, 9f, 0.8f, 0.1f);
        Shine(c, Vector3.Up, r * 1.8f, Flame.Main, age => age < Erupt ? 0.4f + 0.8f * age / Erupt : 2.6f * Mathf.Max(0f, 1f - (age - Erupt) / After));
    }

    private void CometDance(Cast c, float r)
    {
        const float Life = 2.8f;
        const int Comets = 3;
        var ring = r * 0.55f;
        Mark(c, 0f, Life, Filigree, Flame, Vector3.Zero, _ => ring * 1.25f, k => Fade(k, 0.05f, 0.15f), count: 9f, spin: 4f, gain: 1.2f);
        Disc(c, 0f, Life, Glow, Ember, Vector3.Zero, _ => ring * 1.2f, k => Fade(k, 0.08f, 0.15f) * 0.4f);
        for (var i = 0; i < Comets; i++)
        {
            var phase = i * Mathf.Tau / Comets;
            Func<float, Vector3> at = k => Around(phase + k * Life * 3.6f) * ring * (0.9f + 0.15f * Mathf.Sin(k * Life * 2.2f + phase)) + Vector3.Up * (1f + 0.45f * Mathf.Sin(k * Life * 3.3f + phase));
            Globe(c, "energy", Orb, 0f, Life, Flame, at, k => 0.22f * Overshoot(Mathf.Min(k * Life / 0.25f, 1f)), k => Fade(k, 0.03f, 0.1f), gain: 1.7f);
            Flare(c, 0f, Life, Star, Flame, at, _ => 1.2f, k => Fade(k, 0.03f, 0.1f));
            Trail(c, 0f, Life, Flame, at, 0.75f, 0.12f, style: 1, gain: 1.6f);
            Trail(c, 0f, Life, Gold, at, 0.2f, 0.09f, gain: 1.3f);
            for (var j = 0; j < 14; j++)
            {
                var t = Rand(0.2f, Life - 0.3f);
                Spark(c, t, Rand(0.3f, 0.5f), Ember, at(t / Life), FxKit.Jitter(1.2f) + Vector3.Up, Gravity * 0.4f, 0.05f, 0.04f);
            }
        }
        Shine(c, Vector3.Up, r * 1.2f, Flame.Main, age => 1.1f * Fade(age / Life, 0.05f, 0.1f));
    }

    private void DragonSpiral(Cast c, float r)
    {
        const float Flight = 0.9f, Tail = 0.3f;
        var reach = r * 1.7f;
        Func<float, Vector3> spine = s => new Vector3(0f, 1.1f, 0.4f + (reach - 0.4f) * s);
        for (var i = 0; i < 2; i++)
        {
            var phase = i * Mathf.Pi;
            Func<float, Vector3> path = s => spine(s) + new Vector3(Mathf.Cos(s * 14f + phase), Mathf.Sin(s * 14f + phase) * 0.7f, 0f) * (0.75f - 0.3f * s);
            Func<float, (float, float)> span = k =>
            {
                var head = Mathf.Min(k * (Flight + Tail) / Flight, 1f);
                return (Mathf.Max(0f, head - 0.4f), head);
            };
            Ribbon(c, 0f, Flight + Tail, 1, i == 0 ? Flame : Ember, path, span, 0.9f, _ => 1f, gain: 1.7f);
            Ribbon(c, 0f, Flight + Tail, 0, Gold, path, span, 0.25f, _ => 1f, gain: 1.4f);
            Flare(c, 0f, Flight, Star, Flame, path, _ => 1.2f, _ => 1f);
        }
        Globe(c, "energy", Orb, 0f, Flight, Sun, spine, _ => 0.3f, _ => 1f, gain: 1.7f);
        for (var i = 0; i < 7; i++)
        {
            var s = (i + 0.5f) / 7f;
            Mark(c, s * Flight, 0.4f, Ripples, Flame, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (0.6f + 1.2f * k)), spine(s)), k => 1f - k, count: 1.5f, spin: 0f, gain: 1.4f);
            var spot = FxKit.Flat(spine(s));
            Cue(c, s * Flight, () => Stage.Patch(At(c, spot), 0.8f, 1.5f, FireColor, FxGround.Fire));
        }
        var end = spine(1f);
        Cue(c, Flight, () => Stage.Explosion(At(c, end), r * 0.6f, FireColor, FxBurst.Fire));
        Hit(c, Flight, Flame, end, r * 0.7f);
        Spray(c, Flight, Flame, end, 30, 6f, 6f, 0.6f);
        Shine(c, spine(0.5f), reach, Flame.Main, age => 1.3f + 1.4f * Bump(age, Flight, 0.5f));
    }

    private void CinderFall(Cast c, float r)
    {
        const float Life = 3f;
        Mark(c, 0f, Life, Corona, Ember, Vector3.Zero, _ => r, k => Fade(k, 0.08f, 0.2f) * 0.8f, spin: 1f, gain: 1.1f);
        Disc(c, 0f, Life, Glow, Ember, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.2f) * 0.35f);
        Cue(c, 0.4f, () => Stage.Patch(At(c, Vector3.Zero), r * 0.9f, Life - 0.4f, FireColor, FxGround.Fire));
        Drift(c, 90, 0f, Life - 1.3f, 1.3f, Glint, [Ember, Flame, Gold], () => Within(Vector3.Zero, r * 0.95f, 2.6f, 4f), () => new Vector3(Rand(-0.3f, 0.3f), Rand(-2.6f, -1.6f), Rand(-0.3f, 0.3f)), 0.4f, sway: 0.3f, tumble: false, gain: 1.7f);
        Drift(c, 26, 0f, Life - 1.4f, 1.4f, Puff, [Smoke], () => Within(Vector3.Zero, r * 0.9f, 0.2f, 0.8f), () => new Vector3(0.2f, Rand(0.5f, 1.1f), 0f), 1.3f, tumble: false, gain: 0.55f);
        for (var i = 0; i < 20; i++)
        {
            var spot = Within(Vector3.Zero, r * 0.9f);
            var when = Rand(0.5f, Life - 0.6f);
            Glyph(c, when, 0.5f, Wisp, Flame, _ => spot + Vector3.Up * 0.3f, k => 0.7f * (1f - 0.4f * k), k => Fade(k, 0.2f, 0.5f), gain: 1.6f);
            Disc(c, when, 0.35f, Shock, Ember, spot, k => 0.5f * Out(k), k => 1f - k, band: 0.1f);
        }
        Shine(c, Vector3.Up, r * 1.4f, Ember.Main, age => 0.9f * Fade(age / Life, 0.1f, 0.2f));
    }

    // ---- Frost.

    private void SnowfallSanctum(Cast c, float r)
    {
        const float Life = 3.2f;
        Mark(c, 0f, Life, Frostflake, Ice, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.05f, 0.2f), spin: 2f, gain: 1.4f);
        Mark(c, 0f, Life, Filigree, Ice, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.2f) * 0.6f, count: 12f, spin: -1f);
        Cue(c, 0.2f, () => Stage.Patch(At(c, Vector3.Zero), r * 0.95f, Life - 0.2f, FrostColor, FxGround.Frost));
        Drift(c, 80, 0f, Life - 1.5f, 1.5f, Snow, [Ice, Pearl], () => Within(Vector3.Zero, r * 0.95f, 2.6f, 4f), () => new Vector3(Rand(-0.3f, 0.3f), Rand(-2.2f, -1.4f), Rand(-0.3f, 0.3f)), 0.4f, sway: 0.35f, tumble: false, gain: 1.6f);
        Twinkle(c, 40, 0.2f, Life - 0.5f, Pearl, () => Within(Vector3.Zero, r * 0.95f, 0.05f, 0.3f), 0.5f);
        Column(c, 0f, Life, Light, Ice, Vector3.Zero, _ => new Vector2(r * 0.9f, 4f), k => Fade(k, 0.1f, 0.2f) * 0.2f, speed: 1f);
        Shine(c, Vector3.Up, r * 1.4f, Ice.Main, age => 0.8f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void FrostLotus(Cast c, float r)
    {
        const float Open = 0.5f, After = 1.6f;
        const int Petals = 8;
        Mark(c, 0f, Open + After, Lotus, Ice, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.04f, 0.3f), k => Mathf.Min(k * (Open + After) / Open, 1f), count: 8f, spin: 0.6f, gain: 1.5f);
        Disc(c, 0f, Open, Glow, Ice, Vector3.Zero, _ => r * 0.8f, k => 0.2f + 0.5f * k);
        for (var i = 0; i < Petals; i++)
        {
            var way = Around(i * Mathf.Tau / Petals);
            var tip = way * r * 0.8f;
            var mid = way * r * 0.45f;
            Cue(c, Open, () => Stage.Spike(At(c, tip), c.Root.Basis * way, 1.5f, FxSpike.Ice, 0f));
            Cue(c, Open - 0.12f, () => Stage.Spike(At(c, mid), c.Root.Basis * way, 0.9f, FxSpike.Ice, 0f));
            Glyph(c, Open, 0.4f, Glint, Ice, tip + Vector3.Up * 1.2f, 1.3f, 1.7f);
        }
        Cue(c, Open, () => Stage.Explosion(At(c, Vector3.Up * 0.5f), r * 0.55f, FrostColor, FxBurst.Frost));
        Cue(c, Open, () => Stage.Patch(At(c, Vector3.Zero), r, 2.5f, FrostColor, FxGround.Frost));
        Disc(c, Open, 0.5f, Shock, Ice, Vector3.Zero, k => r * 1.4f * Out(k), k => 1f - k, band: 0.14f, gain: 1.5f);
        Flare(c, Open, 0.35f, Star, Ice, Vector3.Up * 0.8f, 4.4f);
        Drift(c, 30, Open, Open + 0.3f, 1.2f, Snow, [Ice, Pearl], () => Within(Vector3.Zero, r * 0.8f, 0.6f, 2f), () => Around(Rand(0f, Mathf.Tau)) * Rand(0.3f, 1.2f) + Vector3.Up * Rand(0.2f, 1f), 0.4f, gravity: 1.2f, tumble: false, gain: 1.6f);
        Twinkle(c, 26, Open, Open + After - 0.4f, Pearl, () => Within(Vector3.Zero, r * 0.9f, 0.2f, 2f), 0.55f);
        Shine(c, Vector3.Up, r * 1.6f, Ice.Main, age => age < Open ? 0.4f : 2.2f * Mathf.Max(0f, 1f - (age - Open) / 0.9f));
    }

    private void ChandelierDrop(Cast c, float r)
    {
        const int Shards = 7;
        const float Hang = 0.8f, Fall = 0.22f;
        var spot = new Vector3(0f, 0f, r * 0.2f);
        var top = spot + Vector3.Up * 4.6f;
        Mark(c, 0f, Hang + Fall + 0.8f, Frostflake, Ice, spot, _ => r * 0.8f, k => Fade(k, 0.04f, 0.35f), gain: 1.3f);
        Mark(c, 0f, Hang + 0.1f, Filigree, Ice, k => Lay(top, r * 0.55f, new Basis(Vector3.Up, k * 1.5f)), k => Fade(k, 0.1f, 0.1f), count: 7f, gain: 1.6f);
        Flare(c, 0f, Hang, Star, Ice, _ => top, k => 1.4f + 0.8f * k, k => Fade(k, 0.1f, 0.1f));
        for (var i = 0; i < Shards; i++)
        {
            var off = i == 0 ? Vector3.Zero : Around(i * Mathf.Tau / (Shards - 1)) * r * 0.5f;
            var long1 = i == 0 ? 1.7f : 1.1f;
            var high = top + off + Vector3.Down * (i == 0 ? 0.3f : 0.9f);
            var land = spot + off;
            var drop = Hang + (i == 0 ? 0.12f : i * 0.02f);
            Prop(c, i * 0.05f, drop - i * 0.05f + Fall, ShardMesh, IceBlock, k =>
            {
                var t = k * (drop - i * 0.05f + Fall) + i * 0.05f;
                var fall = Mathf.Clamp((t - drop) / Fall, 0f, 1f);
                var grow = Out(Mathf.Min((t - i * 0.05f) / 0.3f, 1f));
                return new Transform3D(new Basis(Vector3.Right, Mathf.Pi) * Basis.FromScale(new Vector3(1.1f, long1 * grow / 0.41f, 1.1f)), high.Lerp(land + Vector3.Up * long1 * 0.5f, fall * fall) + Vector3.Up * 0.04f * Mathf.Sin(t * 5f + i));
            });
            Line(c, 0f, drop, Ice, _ => (top + off * 0.3f + high) * 0.5f + Vector3.Up * 0.3f, _ => high + Vector3.Up * 0.6f - (top + off * 0.3f), 0.03f, k => Fade(k, 0.2f, 0.05f) * 0.7f);
            Glyph(c, Rand(0.1f, Hang - 0.3f), 0.4f, Glint, Pearl, high + Vector3.Down * long1 * 0.4f, 1f, 1.7f);
            var hit = drop + Fall;
            Hit(c, hit, Ice, land + Vector3.Up * 0.2f, i == 0 ? 1.1f : 0.6f);
            Disc(c, hit, 0.9f, Cracks, Ice, land, _ => 0.7f, k => Fade(k, 0.02f, 0.5f), k => k);
            for (var j = 0; j < 5; j++)
            {
                var v = Around(Rand(0f, Mathf.Tau)) * Rand(1.5f, 4f) + Vector3.Up * Rand(3f, 6f);
                var spin = FxKit.RandomDirection();
                Prop(c, hit, 0.6f, ShardMesh, IceBlock, k => new Transform3D(new Basis(spin, k * 6f) * Basis.FromScale(new Vector3(0.4f, 0.7f, 0.4f) * (1f - k * 0.6f)), land + v * k * 0.6f + Vector3.Down * 0.5f * Gravity * k * k * 0.36f));
            }
        }
        Cue(c, Hang + Fall + 0.12f, () => Stage.Patch(At(c, spot), r * 0.7f, 2f, FrostColor, FxGround.Frost));
        Shine(c, spot + Vector3.Up, r * 1.5f, Ice.Main, age => age < Hang ? 0.5f : 0.5f + 1.8f * Bump(age, Hang + Fall, 0.6f));
    }

    private void AuroraVeil(Cast c, float r)
    {
        const float Life = 3f;
        var half = r * 1.1f;
        Tone[] veils = [Aurora, Fey, Mint];
        for (var i = 0; i < veils.Length; i++)
        {
            var z = r * (0.25f + 0.4f * i);
            var phase = i * 2.1f;
            var tall = 3.6f - 0.5f * i;
            Wall(c, i * 0.2f, Life - i * 0.2f, 0, veils[i], k => (new Vector3(-half, 0f, z + 0.35f * Mathf.Sin(k * 5f + phase)), new Vector3(half, 0f, z - 0.35f * Mathf.Sin(k * 4f + phase))),
                k => tall * Out(Mathf.Min(k * Life / 0.5f, 1f)) * (1f + 0.08f * Mathf.Sin(k * 9f + phase)), k => Fade(k, 0.1f, 0.3f) * 0.9f, speed: 0.6f, gain: 1.3f);
            Disc(c, i * 0.2f, Life - i * 0.2f, Glow, veils[i], new Vector3(0f, 0f, z), _ => 1f, k => Fade(k, 0.1f, 0.3f) * 0.45f, stretch: new Vector3(half, 1f, 0.6f));
        }
        Twinkle(c, 50, 0.2f, Life - 0.5f, Pearl, () => new Vector3(Rand(-half, half), Rand(0.3f, 3.4f), r * Rand(0.2f, 1.1f)), 0.5f);
        Drift(c, 26, 0.2f, Life - 1.4f, 1.4f, Snow, [Aurora, Pearl], () => new Vector3(Rand(-half, half), Rand(2.4f, 3.6f), r * Rand(0.2f, 1.1f)), () => Vector3.Down * Rand(0.8f, 1.4f), 0.36f, tumble: false, gain: 1.5f);
        Shine(c, new Vector3(0f, 1f, r * 0.6f), r * 1.8f, Aurora.Main, age => 1f * Fade(age / Life, 0.1f, 0.25f));
    }

    private void DiamondDust(Cast c, float r)
    {
        const float Life = 3f;
        var reach = r * 0.6f;
        Disc(c, 0f, Life, Swirl, Ice, Vector3.Zero, _ => reach * 1.5f, k => Fade(k, 0.08f, 0.15f) * 0.5f, spin: 3f);
        Mark(c, 0f, Life, Frostflake, Ice, Vector3.Zero, _ => reach * 1.1f, k => Fade(k, 0.05f, 0.15f), spin: 4f, gain: 1.2f);
        for (var i = 0; i < 90; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var far = reach * Rand(0.5f, 1.4f);
            var high = Rand(0.2f, 2.6f);
            var turn = Rand(1.5f, 3f);
            var size = Rand(0.3f, 0.7f);
            Func<float, Vector3> at = k => Around(phase + k * turn) * far + Vector3.Up * (high + 0.2f * Mathf.Sin(k * 6f + phase));
            Glyph(c, Rand(0f, Life - 0.6f), Rand(0.4f, 0.8f), i % 5 == 0 ? Gem : i % 5 == 1 ? Snow : Glint, i % 3 == 0 ? Pearl : Ice, at, k => size * Mathf.Sin(k * Mathf.Pi), _ => 1f, gain: 1.7f);
        }
        Globe(c, "energy", Shell, 0f, Life, Ice, _ => Vector3.Up * 1.2f, _ => reach * 1.3f, k => Fade(k, 0.1f, 0.15f) * 0.3f);
        Shine(c, Vector3.Up, r, Ice.Main, age => 0.9f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void MirrorShards(Cast c, float r)
    {
        const int Mirrors = 6;
        const float Form = 0.5f, Fire1 = 0.9f, After = 0.6f;
        var spot = new Vector3(0f, 0f, r * 0.15f);
        var heart = spot + Vector3.Up * 1.1f;
        Mark(c, 0f, Fire1 + After, Pentacle, Ice, spot, _ => r * 0.8f, k => Fade(k, 0.04f, 0.3f), k => Mathf.Min(k * (Fire1 + After) / Form, 1f), count: 6f, spin: 1f, gain: 1.3f);
        var places = Enumerable.Range(0, Mirrors).Select(i => heart + Around(i * Mathf.Tau / Mirrors) * r * 0.72f).ToArray();
        for (var i = 0; i < Mirrors; i++)
        {
            var at = places[i];
            var start = i * 0.06f;
            var life = Fire1 + After - start;
            Glyph(c, start, life, Gem, i % 2 == 0 ? Ice : Pearl, k => at + Vector3.Up * 0.1f * Mathf.Sin(k * life * 4f + start * 30f), k => 1.5f * Overshoot(Mathf.Min(k * life / 0.25f, 1f)), k => Fade(k, 0.02f, 0.15f), gain: 1.8f);
            Glyph(c, start, 0.3f, Glint, Pearl, at, 1.4f, 1.7f);
            // Light thrown from each mirror to the one opposite, through the middle.
            var other = places[(i + Mirrors / 2) % Mirrors];
            if (i < Mirrors / 2)
                Tube(c, Fire1 + i * 0.05f, After, Beam, Ice, _ => (at, other), k => 0.09f * (1f - k * 0.6f), k => Fade(k, 0f, 0.6f), speed: 5f, gain: 1.8f);
            // And round the ring to the next.
            var next = places[(i + 1) % Mirrors];
            Tube(c, Form + i * 0.06f, Fire1 + After - Form - i * 0.06f, Beam, Azure, k => (at, at.Lerp(next, Mathf.Min(k * 6f, 1f))), _ => 0.035f, k => Fade(k, 0f, 0.3f), speed: 3f, gain: 1.5f);
        }
        Hit(c, Fire1, Ice, heart, r * 0.8f);
        Cue(c, Fire1, () => Stage.Explosion(At(c, heart), r * 0.5f, FrostColor, FxBurst.Frost));
        Glyph(c, Fire1, 0.45f, Gem, Pearl, heart, 2.6f, 1.9f);
        Twinkle(c, 24, Form, Fire1 + After - 0.3f, Pearl, () => Within(spot, r * 0.8f, 0.4f, 2f), 0.55f);
        Shine(c, heart, r * 1.5f, Ice.Main, age => age < Fire1 ? 0.6f : 2.2f * Mathf.Max(0f, 1f - (age - Fire1) / After));
    }

    private void WinterWaltz(Cast c, float r)
    {
        const float Life = 2.8f;
        var reach = r * 0.5f;
        Mark(c, 0f, Life, Frostflake, Ice, Vector3.Zero, _ => reach * 1.3f, k => Fade(k, 0.05f, 0.15f), spin: 5f, gain: 1.3f);
        for (var i = 0; i < 2; i++)
        {
            var phase = i * Mathf.Pi;
            Func<float, Vector3> path = s => Around(phase + s * 14f) * reach * (0.75f + 0.35f * Mathf.Sin(s * 5f)) + Vector3.Up * (0.3f + 2.4f * (0.5f - 0.5f * Mathf.Cos(s * 9f)));
            Ribbon(c, 0f, Life, 0, i == 0 ? Ice : Pearl, path, k => (Mathf.Max(0f, k - 0.14f), k), 0.5f, k => Fade(k, 0.02f, 0.1f), gain: 1.6f);
            Glyph(c, 0f, Life, Snow, Pearl, path, _ => 0.9f, k => Fade(k, 0.03f, 0.1f), gain: 1.8f);
            Flare(c, 0f, Life, Mote, Ice, path, _ => 1f, k => Fade(k, 0.03f, 0.1f) * 0.6f);
        }
        Drift(c, 44, 0f, Life - 1.2f, 1.2f, Snow, [Ice, Pearl], () => Within(Vector3.Zero, reach * 1.3f, 1.4f, 3f), () => new Vector3(Rand(-0.4f, 0.4f), Rand(-1.6f, -0.8f), Rand(-0.4f, 0.4f)), 0.34f, sway: 0.35f, tumble: false, gain: 1.6f);
        Twinkle(c, 24, 0f, Life - 0.4f, Pearl, () => Within(Vector3.Zero, reach * 1.2f, 0.2f, 2.6f), 0.45f);
        Cue(c, 0.2f, () => Stage.Patch(At(c, Vector3.Zero), reach * 1.3f, Life - 0.2f, FrostColor, FxGround.Frost));
        Shine(c, Vector3.Up, r, Ice.Main, age => 0.9f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void FrozenStar(Cast c, float r)
    {
        const float Form = 0.7f, Fall = 0.2f, After = 1f;
        var spot = new Vector3(0f, 0f, r * 0.2f);
        var high = spot + Vector3.Up * 3.4f;
        var land = Form + Fall;
        Mark(c, 0f, land + After, Frostflake, Ice, spot, _ => r * 0.9f, k => Fade(k, 0.04f, 0.35f), k => Mathf.Min(k * (land + After) / Form, 1f), gain: 1.4f);
        Func<float, Vector3> at = k =>
        {
            var t = k * land;
            return t < Form ? high + Vector3.Up * 0.1f * Mathf.Sin(t * 8f) : high.Lerp(spot + Vector3.Up * 0.6f, Mathf.Pow((t - Form) / Fall, 2f));
        };
        Glyph(c, 0f, land, Snow, Ice, at, k => 3.2f * Overshoot(Mathf.Min(k * land / Form, 1f)), _ => 1f, gain: 1.9f);
        Glyph(c, 0f, land, Snow, Pearl, at, k => 1.8f * Out(Mathf.Min(k * land / Form, 1f)), _ => 0.9f, gain: 1.6f);
        Flare(c, 0f, land, Star, Ice, at, k => 1.6f + k, _ => 0.9f);
        for (var i = 0; i < 24; i++)
        {
            var from = high + FxKit.RandomDirection() * Rand(2f, 3.2f);
            Glyph(c, Rand(0f, Form - 0.35f), 0.35f, Glint, Ice, k => from.Lerp(high, k * k), k => 0.5f * (1f - 0.4f * k), _ => 1f, gain: 1.6f);
        }
        Hit(c, land, Ice, spot + Vector3.Up * 0.3f, r * 0.9f);
        Cue(c, land, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.4f), r * 0.7f, FrostColor, FxBurst.Frost));
        Cue(c, land, () => Stage.Patch(At(c, spot), r * 0.9f, 2.5f, FrostColor, FxGround.Frost));
        for (var i = 0; i < 6; i++)
        {
            var way = Around(i * Mathf.Tau / 6f);
            Cue(c, land, () => Stage.Spike(At(c, spot + way * r * 0.55f), c.Root.Basis * way, 1.4f, FxSpike.Ice, 0.03f));
            Line(c, land, 0.5f, Ice, k => spot + way * r * 0.45f * Out(k) + Vector3.Up * 0.1f, k => way * r * 0.9f * Out(k), 0.3f, k => (1f - k) * (1f - k));
        }
        Drift(c, 30, land, land + 0.2f, 1.1f, Snow, [Ice, Pearl], () => Within(spot, r * 0.5f, 0.5f, 1.5f), () => Around(Rand(0f, Mathf.Tau)) * Rand(1f, 3f) + Vector3.Up * Rand(1f, 3f), 0.4f, gravity: 3f, tumble: false, gain: 1.6f);
        Shine(c, spot + Vector3.Up, r * 1.7f, Ice.Main, age => age < land ? 0.4f + 0.5f * age / land : 2.6f * Mathf.Max(0f, 1f - (age - land) / After));
    }

    private void FrostHeart(Cast c, float r)
    {
        const float Beat = 2.2f, After = 0.7f;
        var heart = new Vector3(0f, 1.3f, r * 0.15f);
        var ground = FxKit.Flat(heart);
        Mark(c, 0f, Beat + After, Frostflake, Ice, ground, _ => r * 0.85f, k => Fade(k, 0.04f, 0.3f), spin: 1.5f, gain: 1.3f);
        Func<float, float> pulse = k => 1f + 0.12f * Mathf.Pow(Mathf.Max(0f, Mathf.Sin(k * Beat * 7f)), 3f);
        Prop(c, 0f, Beat, ShardMesh, IceBlock, k => new Transform3D(new Basis(Vector3.Up, k * 3f) * Basis.FromScale(new Vector3(2.6f, 3.4f, 2.6f) * Overshoot(Mathf.Min(k * Beat / 0.3f, 1f)) * pulse(k)), heart));
        Globe(c, "energy", Hexes, 0f, Beat, Ice, _ => heart, k => 1.25f * Out(Mathf.Min(k * Beat / 0.3f, 1f)) * pulse(k), k => Fade(k, 0.03f, 0.03f) * 0.8f, gain: 1.3f);
        Flare(c, 0f, Beat, Star, Ice, _ => heart, k => 1.6f * pulse(k) * pulse(k), k => Fade(k, 0.05f, 0.03f));
        for (var t = 0.2f; t < Beat; t += Mathf.Tau / 7f)
            Disc(c, t, 0.6f, Shock, Ice, ground, k => r * 1.1f * Out(k), k => (1f - k) * 0.8f, band: 0.1f, gain: 1.2f);
        for (var i = 0; i < 5; i++)
        {
            var phase = i * Mathf.Tau / 5f;
            Func<float, Vector3> at = k => heart + Around(phase + k * Beat * 2.5f) * r * 0.55f + Vector3.Up * 0.4f * Mathf.Sin(k * Beat * 3f + phase);
            Glyph(c, 0f, Beat, Gem, i % 2 == 0 ? Ice : Pearl, at, _ => 0.7f, k => Fade(k, 0.05f, 0.05f), gain: 1.7f);
        }
        Cue(c, 0.2f, () => Stage.Patch(At(c, ground), r * 0.8f, Beat + After, FrostColor, FxGround.Frost));
        Cue(c, Beat, () => Stage.Explosion(At(c, heart), r * 0.8f, FrostColor, FxBurst.Frost));
        Hit(c, Beat, Ice, heart, r);
        Glyph(c, Beat, 0.5f, Snow, Pearl, heart, 4f, 1.8f);
        Twinkle(c, 30, 0.1f, Beat + 0.3f, Pearl, () => heart + FxKit.RandomDirection() * Rand(0.6f, r * 0.8f), 0.5f);
        Shine(c, heart, r * 1.6f, Ice.Main, age => age < Beat ? 0.7f + 0.3f * Mathf.Sin(age * 7f) : 2.6f * Mathf.Max(0f, 1f - (age - Beat) / After));
    }

    // ---- Storm.

    private void StarCall(Cast c, float r)
    {
        const int Stars = 8;
        const float Fall = 0.4f, Every = 0.13f;
        Mark(c, 0f, Stars * Every + Fall + 0.6f, Galaxy, Storm, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.3f), count: 9f, spin: 1.5f, gain: 1.4f);
        var spots = Spread(Stars, r * 0.85f);
        for (var i = 0; i < Stars; i++)
        {
            var spot = spots[i] + Vector3.Up * 0.25f;
            var sky = spot + new Vector3(Rand(1f, 3f), 11f, -4f);
            var start = i * Every;
            var tone = i % 3 == 0 ? Stellar : Storm;
            Func<float, Vector3> at = k => sky.Lerp(spot, k * k);
            Mark(c, start, Fall, Reticle, tone, FxKit.Flat(spot), _ => 0.75f, _ => 0.8f, count: 6f, spin: 4f);
            Flare(c, start, Fall, Star, tone, at, _ => 1.6f, _ => 1f, gain: 1.6f);
            Trail(c, start, Fall, tone, at, 0.4f, 0.45f, gain: 1.5f);
            Trail(c, start, Fall, Pearl, at, 0.12f, 0.6f, gain: 1.2f);
            Hit(c, start + Fall, tone, spot, 0.9f);
            Glyph(c, start + Fall, 0.45f, Glint, Pearl, spot + Vector3.Up * 0.4f, 2.2f, 1.8f);
            Disc(c, start + Fall, 0.8f, Cracks, tone, FxKit.Flat(spot), _ => 0.6f, k => Fade(k, 0.02f, 0.5f), k => k);
        }
        Twinkle(c, 30, 0f, Stars * Every + Fall, Pearl, () => Within(Vector3.Zero, r, 0.5f, 5f), 0.5f);
        Shine(c, Vector3.Up, r * 1.6f, Storm.Main, age => 1.3f * Fade(age / (Stars * Every + Fall + 0.4f), 0.1f, 0.3f));
    }

    private void ThunderHalo(Cast c, float r)
    {
        const float Life = 3f;
        var ring = r * 0.5f;
        Mark(c, 0f, Life, Reticle, Cyan, Vector3.Zero, _ => ring * 1.5f, k => Fade(k, 0.05f, 0.15f), count: 12f, spin: 3f, gain: 1.2f);
        for (var i = 0; i < 2; i++)
        {
            var tilt = i == 0 ? 0.3f : -0.3f;
            var way = i == 0 ? 1f : -1f;
            Disc(c, 0f, Life, Band, i == 0 ? Storm : Cyan, k => new Transform3D(new Basis(Vector3.Up, k * Life * 3f * way) * new Basis(Vector3.Right, tilt) * Basis.FromScale(Vector3.One * ring * 1.15f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f))), Vector3.Up * 1.1f),
                k => Fade(k, 0.03f, 0.1f), band: 0.16f, gain: 1.8f);
        }
        for (var t = 0.3f; t < Life - 0.3f; t += 0.22f)
        {
            var angle = Rand(0f, Mathf.Tau);
            var from = Around(angle) * ring + Vector3.Up * 1.1f;
            var to = Around(angle + Rand(-0.4f, 0.4f)) * r * Rand(0.8f, 1f);
            Cue(c, t, () => Stage.Lightning([(At(c, from) - Vector3.Up * Fx.LineLift, At(c, to) - Vector3.Up * Fx.LineLift)], 0.14f, 6, 0.5f));
            Glyph(c, t, 0.3f, Glint, Cyan, to + Vector3.Up * 0.2f, 1.3f, 1.7f);
            Disc(c, t, 0.3f, Shock, Cyan, to, k => 0.8f * Out(k), k => 1f - k, band: 0.1f);
        }
        for (var i = 0; i < 4; i++)
            Bolt(c, Rand(0f, Life - 0.3f), 0.3f, Cyan, Around(Rand(0f, Mathf.Tau)) * ring + Vector3.Up * 1.1f, Around(Rand(0f, Mathf.Tau)) * ring + Vector3.Up * 1.1f, 0.05f, 5, 0.2f);
        Twinkle(c, 20, 0f, Life - 0.4f, Cyan, () => Around(Rand(0f, Mathf.Tau)) * ring * 1.1f + Vector3.Up * Rand(0.7f, 1.5f), 0.5f, 0.3f);
        Shine(c, Vector3.Up, r * 1.2f, Cyan.Main, age => (1f + 0.3f * Mathf.Sin(age * 23f)) * Fade(age / Life, 0.05f, 0.1f));
    }

    private void VoltLattice(Cast c, float r)
    {
        const float Life = 2.8f;
        const int Nodes = 6;
        Mark(c, 0f, Life, Comb, Cyan, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.2f), count: 8f, spin: 1.5f, gain: 1.3f);
        Disc(c, 0f, Life, Glow, Cyan, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.2f) * 0.25f);
        var nodes = Enumerable.Range(0, Nodes).Select(i => Around(i * Mathf.Tau / Nodes) * r * 0.72f + Vector3.Up * 0.9f).Append(Vector3.Up * 1.3f).ToArray();
        for (var i = 0; i < nodes.Length; i++)
        {
            var at = nodes[i];
            var start = i * 0.05f;
            Globe(c, "energy", Orb, start, Life - start, Cyan, _ => at, k => (i == Nodes ? 0.3f : 0.17f) * Overshoot(Mathf.Min(k * 8f, 1f)), k => Fade(k, 0.02f, 0.1f), gain: 1.7f);
            Flare(c, start, Life - start, Star, Cyan, _ => at, k => 0.9f + 0.2f * Mathf.Sin(k * 60f + start * 50f), k => Fade(k, 0.02f, 0.1f));
            Column(c, start, Life - start, Beam, Cyan, FxKit.Flat(at), _ => new Vector2(0.03f, at.Y), k => Fade(k, 0.05f, 0.1f) * 0.6f, speed: 3f);
        }
        for (var t = 0.35f; t < Life - 0.3f; t += 0.16f)
        {
            var a = (int)(GD.Randi() % nodes.Length);
            var b = (a + 1 + (int)(GD.Randi() % (nodes.Length - 1))) % nodes.Length;
            var (p, q) = (nodes[a], nodes[b]);
            Cue(c, t, () => Stage.Lightning([(At(c, p) - Vector3.Up * Fx.LineLift, At(c, q) - Vector3.Up * Fx.LineLift)], 0.13f, 5, 0.5f));
            Glyph(c, t, 0.25f, Glint, Pearl, q, 1.2f, 1.7f);
        }
        Shine(c, Vector3.Up, r * 1.5f, Cyan.Main, age => (1f + 0.25f * Mathf.Sin(age * 31f)) * Fade(age / Life, 0.05f, 0.15f));
    }

    private void StormWings(Cast c, float r)
    {
        const float Life = 2.8f;
        WingPair(c, 0f, Life, Storm, new Vector3(0f, 2.1f, -0.25f), 2.9f, 1.7f);
        Mark(c, 0f, Life, Reticle, Storm, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.05f, 0.15f), count: 10f, spin: 3f, gain: 1.2f);
        Flare(c, 0f, 0.4f, Star, Storm, Vector3.Up * 1.6f, 4f);
        Disc(c, 0f, 0.5f, Shock, Storm, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        for (var t = 0.4f; t < Life - 0.3f; t += 0.3f)
        {
            var side = (int)(t / 0.3f) % 2 == 0 ? 1f : -1f;
            var tip = new Vector3(side * Rand(1.2f, 2.6f), Rand(2.2f, 3.6f), -0.25f);
            var land = new Vector3(side * Rand(1f, r * 0.8f), 0f, Rand(-0.5f, 1f));
            Cue(c, t, () => Stage.Lightning([(At(c, tip) - Vector3.Up * Fx.LineLift, At(c, land))], 0.15f, 6, 0.6f));
            Disc(c, t, 0.3f, Shock, Storm, land, k => 0.9f * Out(k), k => 1f - k, band: 0.1f);
            Glyph(c, t, 0.3f, Glint, Pearl, land + Vector3.Up * 0.2f, 1.4f, 1.7f);
        }
        for (var i = 0; i < 26; i++)
        {
            var side = i % 2 == 0 ? 1f : -1f;
            var from = new Vector3(side * Rand(0.3f, 2.6f), Rand(1.2f, 3.6f), -0.2f);
            Line(c, Rand(0f, Life - 0.5f), 0.5f, i % 3 == 0 ? Pearl : Storm, k => from + new Vector3(side * 1.2f, -0.4f, 1f) * k, _ => new Vector3(side * 0.6f, -0.2f, 0.5f), 0.05f, k => Fade(k, 0.2f, 0.4f));
        }
        Twinkle(c, 22, 0.1f, Life - 0.5f, Pearl, () => new Vector3(Rand(-2.6f, 2.6f), Rand(0.6f, 3.6f), Rand(-0.4f, 0.4f)), 0.5f);
        Shine(c, Vector3.Up * 1.5f, r * 1.4f, Storm.Main, age => (1.2f + 0.3f * Mathf.Sin(age * 27f)) * Fade(age / Life, 0.06f, 0.15f));
    }

    private void PlasmaBloom(Cast c, float r)
    {
        const float Swell = 1.5f, After = 0.6f;
        const int Petals = 8;
        var heart = new Vector3(0f, 1.2f, r * 0.15f);
        var ground = FxKit.Flat(heart);
        Mark(c, 0f, Swell + After, Lotus, Cyan, ground, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), count: 8f, spin: 1.5f, gain: 1.4f);
        Globe(c, "energy", Orb, 0f, Swell, Cyan, _ => heart, k => 0.25f + 0.4f * k, _ => 1f, gain: 1.7f);
        Globe(c, "energy", Shell, 0f, Swell, Fey, _ => heart, k => 0.4f + 0.6f * k, _ => 0.6f, gain: 1.2f);
        Flare(c, 0f, Swell, Star, Cyan, _ => heart, k => 1.5f + k + 0.3f * Mathf.Sin(k * 80f), _ => 1f);
        for (var i = 0; i < Petals; i++)
        {
            var tip = ground + Around(i * Mathf.Tau / Petals) * r * 0.85f + Vector3.Up * 0.1f;
            for (var j = 0; j < 4; j++)
                Bolt(c, 0.15f + j * 0.32f + i * 0.04f, 0.22f, j % 2 == 0 ? Cyan : Fey, heart, tip, 0.06f, 6, 0.3f);
            Glyph(c, 0.15f + i * 0.04f, Swell - 0.2f, Glint, Cyan, _ => tip, k => 0.9f + 0.3f * Mathf.Sin(k * 40f + i), k => Fade(k, 0.1f, 0.1f), gain: 1.6f);
        }
        Cue(c, Swell, () => Stage.Explosion(At(c, heart), r * 0.8f, FxTheme.Of(FxElement.Lightning), FxBurst.Lightning));
        Hit(c, Swell, Cyan, heart, r);
        Mark(c, Swell, After, Lotus, Pearl, ground, k => r * 0.9f * (1f + 0.25f * Out(k)), k => 1f - k, _ => 1f, count: 8f, gain: 1.6f);
        Shine(c, heart, r * 1.6f, Cyan.Main, age => age < Swell ? 0.8f + 0.5f * age / Swell : 2.6f * Mathf.Max(0f, 1f - (age - Swell) / After));
    }

    private void BallLightningTrio(Cast c, float r)
    {
        const float Roll = 1.1f;
        var reach = r * 1.6f;
        Func<int, float, Vector3> ball = (i, k) => new Vector3((i - 1) * 1.1f + 0.5f * Mathf.Sin(k * 9f + i * 2.1f), 0.9f + 0.2f * Mathf.Sin(k * 14f + i), 0.5f + (reach - 0.5f) * k);
        for (var i = 0; i < 3; i++)
        {
            var mine = i;
            Func<float, Vector3> at = k => ball(mine, k);
            Globe(c, "energy", Orb, 0f, Roll, Storm, at, _ => 0.36f, k => Fade(k, 0.05f, 0.02f), gain: 1.7f);
            Globe(c, "energy", Hexes, 0f, Roll, Cyan, at, _ => 0.5f, k => Fade(k, 0.05f, 0.02f) * 0.7f);
            Flare(c, 0f, Roll, Star, Storm, at, k => 1.5f + 0.3f * Mathf.Sin(k * 70f), _ => 1f);
            Trail(c, 0f, Roll, Storm, at, 0.5f, 0.2f, gain: 1.3f);
            Disc(c, 0f, Roll, Glow, Storm, k => Lay(FxKit.Flat(at(k)), 0.9f), _ => 0.6f);
            var end = at(1f);
            Cue(c, Roll, () => Stage.Explosion(At(c, end), 1f, FxTheme.Of(FxElement.Lightning), FxBurst.Lightning));
            Hit(c, Roll, Storm, end, 0.9f);
        }
        for (var t = 0.1f; t < Roll - 0.1f; t += 0.11f)
        {
            var a = (int)(t / 0.11f) % 3;
            var b = (a + 1) % 3;
            var k = t / Roll;
            Bolt(c, t, 0.12f, Cyan, ball(a, k), ball(b, k), 0.06f, 5, 0.2f);
        }
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Storm.Main, age => 1.2f + 1.4f * Bump(age, Roll, 0.5f));
    }

    private void ZephyrRibbons(Cast c, float r)
    {
        const float Life = 2.6f;
        var reach = r * 0.5f;
        Tone[] winds = [Wind, Mint, Pearl, Wind];
        Mark(c, 0f, Life, Ripples, Wind, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.08f, 0.2f) * 0.7f, count: 3f, spin: 1.2f);
        Disc(c, 0f, Life, Swirl, Wind, Vector3.Zero, _ => reach * 1.6f, k => Fade(k, 0.08f, 0.15f) * 0.6f, spin: 8f);
        for (var i = 0; i < winds.Length; i++)
        {
            var phase = i * Mathf.Tau / winds.Length;
            var start = i * 0.32f;
            // Each winds once from the ground to overhead, one starting after another.
            Func<float, Vector3> path = s => Around(phase + s * 11f) * reach * (0.6f + 0.5f * s) + Vector3.Up * (0.2f + 2.8f * s);
            Ribbon(c, start, Life - start, 0, winds[i], path, k => (Mathf.Max(0f, k - 0.3f), k), 0.42f, k => Fade(k, 0.02f, 0.15f), gain: 1.4f);
        }
        Drift(c, 24, 0f, Life - 1f, 1f, Leaflet, [Leaf, Mint, Honey], () => Within(Vector3.Zero, reach * 1.2f, 0.2f, 1.2f), () => Around(Rand(0f, Mathf.Tau)) * 1.2f + Vector3.Up * Rand(1f, 2f), 0.3f, sway: 0.5f, slim: 0.5f);
        for (var i = 0; i < 24; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var high = Rand(0.2f, 2.6f);
            Func<float, Vector3> at = k => Around(phase + k * 4f) * reach * 1.2f + Vector3.Up * (high + 0.5f * k);
            Line(c, Rand(0f, Life - 0.5f), 0.5f, Wind, at, k => Heading(at, k, 0.9f), 0.05f, k => Fade(k, 0.2f, 0.3f) * 0.8f);
        }
        Shine(c, Vector3.Up, r, Wind.Main, age => 0.8f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void TempestCrown(Cast c, float r)
    {
        const int Points = 6;
        const float Every = 0.1f, Final = Points * Every + 0.25f;
        Mark(c, 0f, Final + 0.8f, Pentacle, Storm, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.03f, 0.35f), k => Mathf.Min(k * (Final + 0.8f) / 0.5f, 1f), count: 6f, spin: 1f, gain: 1.4f);
        for (var i = 0; i < Points; i++)
        {
            var spot = Around(i * Mathf.Tau / Points) * r * 0.8f;
            var when = i * Every;
            var sky = spot + new Vector3(Rand(-1f, 1f), 12f, Rand(-1f, 1f));
            Cue(c, when, () => Stage.Lightning([(At(c, sky), At(c, spot))], 0.2f, 8, 1f));
            Hit(c, when, Storm, spot + Vector3.Up * 0.2f, 0.9f);
            Column(c, when, Final - when + 0.5f, Beam, Storm, spot, _ => new Vector2(0.06f, 2.2f), k => Fade(k, 0.02f, 0.3f), speed: 4f, gain: 1.5f);
            Glyph(c, when, Final - when + 0.5f, Glint, Pearl, _ => spot + Vector3.Up * 2.2f, k => 1f + 0.3f * Mathf.Sin(k * 50f), k => Fade(k, 0.02f, 0.3f), gain: 1.7f);
            Disc(c, when, 1f, Cracks, Storm, spot, _ => 0.7f, k => Fade(k, 0.02f, 0.5f), k => k);
        }
        var sky0 = new Vector3(0f, 14f, 0f);
        Cue(c, Final, () => Stage.Lightning([(At(c, sky0), At(c, Vector3.Zero))], 0.3f, 12, 1.8f));
        Cue(c, Final, () => Stage.Explosion(At(c, Vector3.Up * 0.5f), r * 0.7f, FxTheme.Of(FxElement.Lightning), FxBurst.Lightning));
        Hit(c, Final, Storm, Vector3.Up * 0.3f, r * 1.1f);
        Column(c, Final, 0.6f, Light, Storm, Vector3.Zero, _ => new Vector2(r * 0.3f, 12f), k => (1f - k) * (1f - k), gain: 1.8f, speed: -3f);
        Shine(c, Vector3.Up, r * 1.8f, Storm.Main, age => age < Final ? 1.2f : 3f * Mathf.Max(0f, 1f - (age - Final) / 0.6f));
    }

    // ---- Water.

    private void LotusPond(Cast c, float r)
    {
        const float Life = 3.4f;
        Mark(c, 0f, Life, Caustics, Sea, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.2f), count: 7f, spin: 1f, gain: 1.3f);
        Disc(c, 0f, Life, Glow, Sea, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.2f) * 0.45f);
        Mark(c, 0f, Life, Ripples, Foam, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.2f) * 0.5f, count: 3f, spin: 0.4f);
        var pads = Spread(6, r * 0.75f).Append(Vector3.Zero).ToArray();
        for (var i = 0; i < pads.Length; i++)
        {
            var pad = pads[i];
            var start = 0.2f + i * 0.12f;
            var life = Life - start;
            var size = i == pads.Length - 1 ? 0.9f : Rand(0.45f, 0.65f);
            var tone = i % 2 == 0 ? Petal : Pearl;
            Mark(c, start, life, Lotus, tone, k => Lay(pad + Vector3.Up * 0.06f, size * Overshoot(Mathf.Min(k * life / 0.4f, 1f)), new Basis(Vector3.Up, k * 0.6f)), k => Fade(k, 0.02f, 0.2f), _ => 1f, count: 7f, gain: 1.6f);
            Flare(c, start, life, Mote, Honey, _ => pad + Vector3.Up * 0.15f, _ => size * 0.9f, k => Fade(k, 0.1f, 0.2f) * 0.7f);
            Mark(c, start, 0.7f, Ripples, Foam, pad, k => size * (1f + 1.4f * k), k => 1f - k, count: 2f, spin: 0f);
        }
        Drift(c, 30, 0.4f, Life - 1.3f, 1.3f, Glint, [Foam, Pearl, Petal], () => Within(Vector3.Zero, r * 0.9f, 0.1f, 0.3f), () => Vector3.Up * Rand(0.5f, 1.2f), 0.4f, tumble: false, gain: 1.6f);
        Shine(c, Vector3.Up, r * 1.4f, Sea.Main, age => 0.8f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void RainDance(Cast c, float r)
    {
        const float Life = 3f;
        Mark(c, 0f, Life, Caustics, Sea, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.25f) * 0.7f, count: 6f, spin: 1f);
        Disc(c, 0f, Life, Glow, Sea, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.25f) * 0.3f);
        var slant = new Vector3(0.25f, -1f, 0.1f).Normalized();
        for (var i = 0; i < 90; i++)
        {
            var spot = Within(Vector3.Zero, r * 0.95f);
            var when = Rand(0f, Life - 0.5f);
            const float Fall = 0.22f;
            var from = spot - slant * 5f;
            Line(c, when, Fall, i % 3 == 0 ? Foam : Sea, k => from.Lerp(spot, k) - slant * 0.4f, _ => slant * 0.9f, 0.035f, _ => 0.9f);
            if (i % 2 == 0)
            {
                Mark(c, when + Fall, 0.45f, Ripples, Foam, spot, k => 0.15f + 0.5f * k, k => 1f - k, count: 2f, spin: 0f, gain: 1.2f);
                Spark(c, when + Fall, 0.25f, Foam, spot, new Vector3(Rand(-0.6f, 0.6f), 2.2f, Rand(-0.6f, 0.6f)), Gravity, 0.04f, 0.03f);
            }
        }
        Twinkle(c, 20, 0.2f, Life - 0.5f, Foam, () => Within(Vector3.Zero, r * 0.9f, 0.05f, 0.2f), 0.4f);
        Shine(c, Vector3.Up, r * 1.3f, Sea.Main, age => 0.7f * Fade(age / Life, 0.1f, 0.25f));
    }

    private void KoiStream(Cast c, float r)
    {
        const float Life = 3.2f;
        var ring = r * 0.5f;
        Mark(c, 0f, Life, Caustics, Sea, Vector3.Zero, _ => ring * 1.7f, k => Fade(k, 0.08f, 0.15f), count: 6f, spin: 1.5f, gain: 1.2f);
        Mark(c, 0f, Life, Ripples, Foam, Vector3.Zero, _ => ring * 1.7f, k => Fade(k, 0.08f, 0.15f) * 0.5f, count: 3f, spin: 0.6f);
        Tone[] koi = [Dawn, Pearl];
        for (var i = 0; i < koi.Length; i++)
        {
            var phase = i * Mathf.Pi;
            // Each swims a circle, its body flexing, a tail-flick wider than its head.
            Func<float, Vector3> path = s => Around(phase + s * 13f) * ring * (1f + 0.12f * Mathf.Sin(s * 60f + phase)) + Vector3.Up * (0.5f + 0.15f * Mathf.Sin(s * 9f + phase));
            Ribbon(c, 0f, Life, 0, koi[i], path, k => (Mathf.Max(0f, k - 0.085f), k), 0.75f, k => Fade(k, 0.02f, 0.08f), gain: 1.7f);
            Ribbon(c, 0f, Life, 0, i == 0 ? Gold : Rose, path, k => (Mathf.Max(0f, k - 0.05f), Mathf.Max(0f, k - 0.012f)), 0.4f, k => Fade(k, 0.02f, 0.08f), gain: 1.4f);
            Globe(c, "energy", Orb, 0f, Life, koi[i], path, _ => 0.2f, k => Fade(k, 0.02f, 0.08f), gain: 1.6f);
            Glyph(c, 0f, Life, Leaflet, koi[i], k => path(Mathf.Max(0f, k - 0.095f)), _ => 0.7f, k => Fade(k, 0.04f, 0.08f), k => -Heading(path, Mathf.Max(0f, k - 0.095f), 1f), gain: 1.4f, slim: 0.8f);
            for (var j = 0; j < 12; j++)
            {
                var s = (j + 0.5f) / 12f;
                Mark(c, s * Life, 0.6f, Ripples, Foam, FxKit.Flat(path(s)), k => 0.25f + 0.6f * k, k => (1f - k) * 0.8f, count: 2f, spin: 0f);
            }
        }
        Drift(c, 22, 0.2f, Life - 1.1f, 1.1f, Bubble, [Foam], () => Within(Vector3.Zero, ring * 1.3f, 0.1f, 0.5f), () => Vector3.Up * Rand(0.6f, 1.2f), 0.2f, tumble: false, gain: 1.4f);
        Shine(c, Vector3.Up, r, Sea.Main, age => 0.8f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void PearlBarrage(Cast c, float r)
    {
        const int Pearls = 7;
        const float Flight = 0.55f, Every = 0.08f;
        var reach = r * 1.5f;
        var from = new Vector3(0f, 1.3f, 0.3f);
        for (var i = 0; i < Pearls; i++)
        {
            var land = new Vector3(Rand(-1f, 1f) * r * 0.5f, 0.2f, reach * Rand(0.65f, 1.05f));
            var start = i * Every;
            Func<float, Vector3> at = k => from.Lerp(land, k) + Vector3.Up * 2f * Mathf.Sin(k * Mathf.Pi);
            Globe(c, "energy", Orb, start, Flight, Pearl, at, _ => 0.2f, _ => 1f, gain: 1.7f);
            Glyph(c, start, Flight, Bubble, Foam, at, _ => 0.7f, _ => 1f, gain: 1.5f);
            Trail(c, start, Flight, Foam, at, 0.24f, 0.35f);
            var pop = start + Flight;
            var ground = FxKit.Flat(land);
            Glyph(c, pop, 0.35f, Bubble, Foam, k => land + Vector3.Up * 0.3f, k => 0.6f + 2.2f * Out(k), k => 1f - k, gain: 1.6f);
            Mark(c, pop, 0.7f, Ripples, Foam, ground, k => 0.4f + 1.2f * k, k => 1f - k, count: 2.5f, spin: 0f, gain: 1.3f);
            Disc(c, pop, 0.35f, Shock, Sea, ground, k => 1.2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.2f);
            Glyph(c, pop, 0.3f, Glint, Pearl, land + Vector3.Up * 0.3f, 1.4f, 1.7f);
            for (var j = 0; j < 7; j++)
                Spark(c, pop, Rand(0.35f, 0.55f), j % 2 == 0 ? Foam : Sea, land, Around(Rand(0f, Mathf.Tau)) * Rand(1f, 3f) + Vector3.Up * Rand(3f, 5f), Gravity, 0.07f, 0.04f);
        }
        Disc(c, Flight, Pearls * Every + 1f, Glow, Sea, new Vector3(0f, 0f, reach * 0.85f), _ => r * 0.7f, k => Fade(k, 0.1f, 0.5f) * 0.45f);
        Shine(c, new Vector3(0f, 1f, reach * 0.7f), reach, Sea.Main, age => 1f * Fade(age / (Pearls * Every + Flight + 0.4f), 0.1f, 0.3f));
    }

    private void MistVeil(Cast c, float r)
    {
        const float Life = 3.2f;
        Mark(c, 0f, Life, Ripples, Foam, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.25f) * 0.5f, count: 3f, spin: 0.3f);
        Disc(c, 0f, Life, Glow, Foam, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.25f) * 0.35f);
        for (var i = 0; i < 46; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var far = r * Mathf.Sqrt(Rand(0.02f, 1f)) * 0.95f;
            var high = Rand(0.3f, 1.5f);
            var size = Rand(1.6f, 2.6f);
            var life = Rand(1.4f, 2.2f);
            Glyph(c, Rand(0f, Life - life), life, Puff, i % 3 == 0 ? Sea : Foam, k => Around(phase + k * 0.6f) * far + Vector3.Up * (high + 0.3f * k), k => size * (0.8f + 0.3f * k), k => Fade(k, 0.3f, 0.4f) * 0.55f, gain: 0.8f);
        }
        Twinkle(c, 36, 0.2f, Life - 0.5f, Pearl, () => Within(Vector3.Zero, r * 0.9f, 0.3f, 2f), 0.4f);
        Shine(c, Vector3.Up, r * 1.2f, Foam.Main, age => 0.6f * Fade(age / Life, 0.1f, 0.25f));
    }

    private void MoonTide(Cast c, float r)
    {
        const float Draw = 1.1f, After = 0.9f;
        var moon = Vector3.Up * 3.6f;
        Mark(c, 0f, Draw + After, Caustics, Sea, Vector3.Zero, _ => r, k => Fade(k, 0.08f, 0.3f), count: 6f, spin: 1.5f, gain: 1.2f);
        Glyph(c, 0f, Draw + After, Sickle, Moon, k => moon + Vector3.Up * 0.1f * Mathf.Sin(k * 9f), k => 1.6f * Overshoot(Mathf.Min(k * (Draw + After) / 0.35f, 1f)), k => Fade(k, 0.05f, 0.25f), gain: 1.9f);
        Flare(c, 0f, Draw + After, Mote, Moon, _ => moon, _ => 3f, k => Fade(k, 0.1f, 0.25f) * 0.5f, gain: 1f);
        Column(c, 0f, Draw + After, Light, Moon, Vector3.Zero, _ => new Vector2(r * 0.35f, 3.6f), k => Fade(k, 0.1f, 0.3f) * 0.4f, speed: -1f, gain: 1.2f);
        // The tide draws in to the middle under the moon, then rolls back out as a wave.
        for (var i = 0; i < 5; i++)
            Disc(c, i * 0.2f, 0.6f, Shock, Foam, Vector3.Zero, k => r * (1.05f - Out(k) * 0.9f), k => Fade(k, 0.2f, 0.3f) * 0.8f, band: 0.1f);
        for (var i = 0; i < 30; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Line(c, Rand(0f, Draw - 0.4f), 0.4f, i % 2 == 0 ? Foam : Sea, k => way * r * (1f - 0.8f * k) + Vector3.Up * 0.1f, _ => -way * 0.8f, 0.06f, k => Fade(k, 0.2f, 0.2f));
        }
        Column(c, Draw - 0.1f, 0.8f, Spiral, Sea, Vector3.Zero, k => new Vector2(r * 0.22f, 3.4f * Out(Mathf.Min(k * 4f, 1f))), k => 1f - k, twist: 1.5f, speed: 3f, gain: 1.5f);
        Hit(c, Draw, Foam, Vector3.Up * 0.3f, r * 0.9f);
        Disc(c, Draw + 0.15f, 0.6f, Shock, Sea, Vector3.Zero, k => r * 1.5f * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
        Cue(c, Draw, () => Stage.Explosion(At(c, Vector3.Up * 0.4f), r * 0.5f, WaterColor, FxBurst.Dome));
        Spray(c, Draw, Foam, Vector3.Up * 0.3f, 36, 5f, 7f, 0.7f, 0.08f);
        Twinkle(c, 20, 0f, Draw + After - 0.4f, Moon, () => Within(Vector3.Zero, r * 0.8f, 0.5f, 3.6f), 0.5f);
        Shine(c, Vector3.Up, r * 1.5f, Moon.Main, age => age < Draw ? 0.7f : 0.7f + 1.4f * Bump(age, Draw, 0.6f));
    }

    private void BubbleStream(Cast c, float r)
    {
        const float Life = 1.6f;
        var reach = r * 1.6f;
        var from = new Vector3(0f, 1.1f, 0.4f);
        Flare(c, 0f, 0.3f, Star, Foam, from, 2f);
        for (var i = 0; i < 40; i++)
        {
            var to = new Vector3(Rand(-1f, 1f) * r * 0.45f, Rand(0.3f, 2.2f), reach * Rand(0.6f, 1.05f));
            var start = Rand(0f, 0.8f);
            var life = Rand(0.6f, 0.8f);
            var size = Rand(0.3f, 0.8f);
            var wobble = Rand(0f, Mathf.Tau);
            Func<float, Vector3> at = k => from.Lerp(to, Out(k)) + new Vector3(Mathf.Sin(k * 12f + wobble), Mathf.Cos(k * 9f + wobble), 0f) * 0.15f;
            Glyph(c, start, life, Bubble, i % 4 == 0 ? Sea : Foam, at, k => size * Mathf.Min(1f, 0.4f + k * 2f), k => Fade(k, 0.1f, 0.02f), gain: 1.5f);
            // It pops: a quick wider ring and a few drops.
            Glyph(c, start + life, 0.18f, Bubble, Pearl, k => to, k => size * (1f + 1.2f * k), k => 1f - k, gain: 1.6f);
            if (i % 2 == 0)
                Glyph(c, start + life, 0.25f, Glint, Pearl, to, size * 1.4f, 1.6f);
            for (var j = 0; j < 2; j++)
                Spark(c, start + life, 0.3f, Foam, to, FxKit.Jitter(1.5f), Gravity * 0.6f, 0.04f, 0.03f);
        }
        Disc(c, 0f, Life, Cone, Sea, k => Lay(Vector3.Zero, reach), k => Fade(k, 0.05f, 0.4f) * 0.5f, k => Out(Mathf.Min(k * 2f, 1f)), cone: 0.35f);
        Mark(c, 0.3f, Life - 0.3f, Caustics, Sea, new Vector3(0f, 0f, reach * 0.8f), _ => r * 0.6f, k => Fade(k, 0.1f, 0.4f), count: 6f, spin: 2f);
        Shine(c, new Vector3(0f, 1f, reach * 0.6f), reach, Sea.Main, age => 0.9f * Fade(age / Life, 0.1f, 0.3f));
    }

    private void FountainOfLife(Cast c, float r)
    {
        const float Life = 3f;
        const int Jets = 8;
        Mark(c, 0f, Life, Caustics, Sea, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.08f, 0.2f), count: 7f, spin: 1f, gain: 1.2f);
        Mark(c, 0f, Life, Filigree, Foam, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.2f) * 0.7f, count: 8f, spin: 1f);
        Column(c, 0f, Life, Spiral, Sea, Vector3.Zero, k => new Vector2(0.4f, 3.4f * Out(Mathf.Min(k * Life / 0.3f, 1f))), k => Fade(k, 0.03f, 0.15f), twist: 2f, speed: 3.5f, gain: 1.5f);
        Column(c, 0f, Life, Light, Foam, Vector3.Zero, k => new Vector2(0.25f, 3.8f * Out(Mathf.Min(k * Life / 0.3f, 1f))), k => Fade(k, 0.03f, 0.15f) * 0.7f, speed: 3f, gain: 1.5f);
        var top = Vector3.Up * 3.3f;
        for (var i = 0; i < Jets; i++)
        {
            var land = Around(i * Mathf.Tau / Jets) * r * 0.75f;
            // An arc of water from the head of the column out and down to the rim, always flowing.
            Func<float, Vector3> arc = s => top.Lerp(land, s) + Vector3.Up * (1.2f * Mathf.Sin(s * Mathf.Pi) - 0f) * (1f - s * 0.3f);
            const float Flow = 0.5f;
            for (var t = 0.25f; t < Life - Flow - 0.1f; t += 0.22f)
                Ribbon(c, t + i * 0.02f, Flow, 0, i % 2 == 0 ? Sea : Foam, arc, k => (Mathf.Max(0f, k * 1.3f - 0.5f), Mathf.Min(k * 1.3f, 1f)), 0.3f, k => Fade(k, 0.1f, 0.2f), gain: 1.4f);
            Mark(c, 0.6f, Life - 0.8f, Ripples, Foam, land, _ => 0.8f, k => Fade(k, 0.1f, 0.2f), count: 2.5f, spin: 2.5f, gain: 1.2f);
            for (var j = 0; j < 8; j++)
                Spark(c, Rand(0.7f, Life - 0.5f), 0.35f, Foam, land + Vector3.Up * 0.1f, FxKit.Jitter(1.2f) + Vector3.Up * 2.5f, Gravity, 0.05f, 0.03f);
        }
        Drift(c, 30, 0.4f, Life - 1.2f, 1.2f, Glint, [Foam, Pearl, Mint], () => Within(Vector3.Zero, r * 0.8f, 0.2f, 0.6f), () => Vector3.Up * Rand(0.8f, 1.6f), 0.4f, tumble: false, gain: 1.6f);
        Shine(c, Vector3.Up * 1.5f, r * 1.4f, Sea.Main, age => 1f * Fade(age / Life, 0.06f, 0.15f));
    }

    // ---- Earth.

    /// <summary>A crystal growing out of the ground at <paramref name="foot"/>, leaning, and fading away at the end.</summary>
    private void Crystal(Cast c, float start, float life, Material material, Vector3 foot, Vector3 lean, float tall, float wide)
    {
        var turn = Along(lean, 1f, 1f);
        var way = lean.Normalized();
        Add(c, ShardMesh, material, start, life, (n, k, t) =>
        {
            var grown = Overshoot(Mathf.Min(t / 0.18f, 1f));
            // The shard is 0.41 tall and 0.22 across.
            n.Transform = new Transform3D(turn * Basis.FromScale(new Vector3(wide / 0.22f, tall * Mathf.Max(grown, 0.01f) / 0.41f, wide / 0.22f)), foot + way * tall * 0.46f * grown);
            n.SetInstanceShaderParameter("uFade", 1f - Mathf.Clamp((t - (life - 0.45f)) / 0.45f, 0f, 1f));
        });
    }

    private void CrystalGarden(Cast c, float r)
    {
        const float Life = 2.6f;
        var spot = new Vector3(0f, 0f, r * 0.1f);
        Mark(c, 0f, Life, Pentacle, Amethyst, spot, _ => r * 0.9f, k => Fade(k, 0.04f, 0.25f), count: 7f, spin: 0.8f, gain: 1.2f);
        Disc(c, 0f, Life, Cracks, Amethyst, spot, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), k => k * 0.6f);
        Disc(c, 0f, Life, Glow, Amethyst, spot, _ => r * 0.9f, k => Fade(k, 0.1f, 0.25f) * 0.35f);
        for (var i = 0; i < 13; i++)
        {
            var far = i == 0 ? 0f : r * (i < 5 ? 0.35f : 0.75f);
            var way = Around(i * 2.4f);
            var foot = spot + way * far;
            var start = 0.1f + far / r * 0.5f + Rand(0f, 0.06f);
            var tall = i == 0 ? 2.8f : Rand(1f, 1.9f) * (i < 5 ? 1.2f : 0.8f);
            var lean = (Vector3.Up + way * (i == 0 ? 0f : Rand(0.3f, 0.7f))).Normalized();
            Crystal(c, start, Life - start, AmethystBlock, foot, lean, tall, tall * 0.2f);
            Disc(c, start, 0.3f, Shock, Amethyst, foot, k => 0.9f * Out(k), k => 1f - k, band: 0.1f);
            Glyph(c, start + 0.12f, 0.4f, Glint, Pearl, foot + lean * tall * 0.9f, 1.5f, 1.8f);
            for (var j = 0; j < 4; j++)
                Spark(c, start, Rand(0.3f, 0.5f), Amethyst, foot + Vector3.Up * 0.2f, FxKit.Jitter(2f) + Vector3.Up * 4f, Gravity, 0.06f, 0.03f);
        }
        Twinkle(c, 44, 0.5f, Life - 0.5f, Pearl, () => Within(spot, r * 0.8f, 0.3f, 2.4f), 0.6f);
        Shine(c, spot + Vector3.Up, r * 1.5f, Amethyst.Main, age => 1.1f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void GeodeBurst(Cast c, float r)
    {
        const float Shake = 0.7f, After = 1.1f;
        const float Size = 1.05f;
        var at = new Vector3(0f, 0.95f, r * 0.2f);
        var ground = FxKit.Flat(at);
        Disc(c, 0f, Shake, Telegraph, Amethyst, ground, _ => r * 0.7f, _ => 0.6f, k => k);
        // A boulder heaves up and shakes harder and harder.
        var axis = new Vector3(0.3f, 1f, 0.2f).Normalized();
        Prop(c, 0f, Shake, Boulders[0], _stone, k => new Transform3D(new Basis(axis, 0.22f * Mathf.Sin(k * k * 60f)) * Basis.FromScale(Vector3.One * Size * Overshoot(Mathf.Min(k * 5f, 1f))), at + FxKit.Jitter(0.04f * k)));
        Disc(c, 0f, 0.4f, Shock, Dust, ground, k => 1.8f * Out(k), k => 1f - k, band: 0.12f);
        Disc(c, 0f, Shake + After, Cracks, Rubble, ground, _ => 1.5f, k => Fade(k, 0.03f, 0.3f), k => k * 0.6f);
        // Light shows along the seam where it will part, and breaks out in rays.
        Disc(c, Shake * 0.3f, Shake * 0.7f, Band, Amethyst, k => new Transform3D(Stand(Vector3.Right) * Basis.FromScale(Vector3.One * Size * 1.2f), at), k => k, band: 0.1f, gain: 1.9f);
        for (var i = 0; i < 9; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var way = new Vector3(Rand(-0.25f, 0.25f), Mathf.Cos(angle), Mathf.Sin(angle)).Normalized();
            var start = Shake * (0.3f + 0.07f * i);
            Line(c, start, Shake - start, Amethyst, k => at + way * (0.9f + 0.8f * k), k => way * (0.5f + 1.6f * k), 0.1f, _ => 1f);
        }
        Flare(c, 0f, Shake, Star, Amethyst, _ => at, k => 0.5f + 2.4f * k * k, k => k);
        Hit(c, Shake, Amethyst, at, r * 0.9f);
        Cue(c, Shake, () => Stage.Explosion(At(c, at), r * 0.5f, Amethyst.Main, FxBurst.Dome));
        // It splits: the two halves fall open, each lined with crystal.
        foreach (var side in new[] { 1f, -1f })
        {
            var turn = side > 0f ? Basis.Identity : new Basis(Vector3.Up, Mathf.Pi);
            Func<float, Transform3D> half = k =>
            {
                var open = Out(Mathf.Min(k * 2.5f, 1f));
                var gone = 1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp((k - 0.75f) / 0.25f, 0f, 1f));
                // Tipped outward onto its back, so the cut face and its crystals turn up to the sky.
                var tip = new Basis(Vector3.Back, -1.15f * open);
                return new Transform3D(turn * tip * Basis.FromScale(Vector3.One * Size * gone), at + Vector3.Right * side * (0.2f + 1.05f * open) + Vector3.Down * 0.4f * open);
            };
            Prop(c, Shake, After, GeodeHalf, _stone, half);
            for (var i = 0; i < 7; i++)
            {
                var spot = i == 0 ? Vector2.Zero : new Vector2(Mathf.Cos(i * Mathf.Tau / 6f), Mathf.Sin(i * Mathf.Tau / 6f)) * 0.48f;
                var tall = i == 0 ? 0.75f : Rand(0.35f, 0.6f);
                var lean = (Vector3.Left + new Vector3(0f, spot.X, spot.Y) * 0.5f).Normalized();
                var shard = new Transform3D(Along(lean, 1f, 1f) * Basis.FromScale(new Vector3(1.3f, tall / 0.41f, 1.3f)), new Vector3(-tall * 0.3f, spot.X * 0.8f, spot.Y));
                Prop(c, Shake, After, ShardMesh, AmethystBlock, k => half(k) * shard);
            }
            Flare(c, Shake, After, Mote, Amethyst, k => half(k) * new Vector3(-0.3f, 0f, 0f), _ => 2.2f, k => Fade(k, 0.02f, 0.3f) * 0.7f, gain: 1.1f);
            Glyph(c, Shake + 0.35f, After - 0.5f, Glint, Pearl, k => half((0.35f + k * (After - 0.5f)) / After) * new Vector3(-0.6f, 0.1f, 0f), k => 1.2f + 0.3f * Mathf.Sin(k * 30f), k => Fade(k, 0.2f, 0.3f), gain: 1.8f);
        }
        for (var i = 0; i < 26; i++)
        {
            var v = FxKit.RandomDirection() * Rand(2.5f, 6f) + Vector3.Up * Rand(1f, 4f);
            var size = Rand(0.45f, 1f);
            var spin = Rand(0f, Mathf.Tau);
            Func<float, Vector3> gem = k => at + v * k * 0.7f + Vector3.Down * 0.5f * Gravity * k * k * 0.3f;
            Glyph(c, Shake, 0.75f, Gem, i % 3 == 0 ? Pearl : i % 3 == 1 ? Amethyst : Fey, gem, k => size * (1f - 0.3f * k), k => 1f - k * k, k => new Vector3(Mathf.Sin(spin + k * 8f), Mathf.Cos(spin + k * 8f), 0f), gain: 1.8f);
        }
        for (var i = 0; i < 12; i++)
        {
            var v = Around(Rand(0f, Mathf.Tau)) * Rand(2f, 5f) + Vector3.Up * Rand(3f, 7f);
            var spin = FxKit.RandomDirection();
            var size = Rand(0.12f, 0.26f);
            var chunk = Boulders[1 + i % 2];
            Prop(c, Shake, 0.8f, chunk, _stone, k => new Transform3D(new Basis(spin, k * 7f) * Basis.FromScale(Vector3.One * size), at + v * k * 0.8f + Vector3.Down * 0.5f * Gravity * k * k * 0.64f));
        }
        Mark(c, Shake, After, Pentacle, Amethyst, ground, _ => r * 0.7f, k => Fade(k, 0.02f, 0.6f), _ => 1f, count: 7f, gain: 1.3f);
        Twinkle(c, 24, Shake, Shake + After - 0.3f, Pearl, () => at + FxKit.RandomDirection() * Rand(0.5f, 2.6f), 0.7f);
        Shine(c, at, r * 1.6f, Amethyst.Main, age => age < Shake ? 0.3f + 0.9f * age / Shake : 2.6f * Mathf.Max(0f, 1f - (age - Shake) / After));
    }

    private void JadePillars(Cast c, float r)
    {
        const int Pillars = 5;
        const float Rise = 0.35f, Stand1 = 2.2f;
        Mark(c, 0f, Rise + Stand1, Filigree, Jade, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.04f, 0.2f), count: 10f, spin: 1f, gain: 1.3f);
        Mark(c, 0f, Rise + Stand1, Pentacle, Jade, Vector3.Zero, _ => r * 0.72f, k => Fade(k, 0.04f, 0.2f) * 0.8f, count: 5f, spin: 0f);
        for (var i = 0; i < Pillars; i++)
        {
            var foot = Around(i * Mathf.Tau / Pillars) * r * 0.72f;
            var start = Rise + i * 0.08f;
            var life = Stand1 - i * 0.08f;
            var turn = new Basis(Vector3.Up, Rand(0f, Mathf.Tau));
            var size = new Vector3(0.7f, 2.6f, 0.7f);
            Disc(c, i * 0.08f, Rise, Telegraph, Jade, foot, _ => 0.6f, _ => 0.7f, k => k);
            Prop(c, start, life, Crags[i % Crags.Length], JadeBlock, k =>
            {
                var t = k * life;
                var up = Out(Mathf.Min(t / 0.12f, 1f)) - Mathf.Clamp((t - (life - 0.4f)) / 0.4f, 0f, 1f);
                return new Transform3D(turn * Basis.FromScale(size), foot + Vector3.Up * size.Y * (up - 1f));
            });
            Disc(c, start, 0.4f, Shock, Jade, foot, k => 1.4f * Out(k), k => 1f - k, band: 0.1f, gain: 1.3f);
            Glyph(c, start + 0.1f, 0.4f, Glint, Pearl, foot + Vector3.Up * 2.5f, 1.6f, 1.8f);
            Spray(c, start, Jade, foot + Vector3.Up * 0.2f, 8, 3f, 6f);
            // Each pillar is tied to the next by a thread of light.
            var next = Around((i + 1) * Mathf.Tau / Pillars) * r * 0.72f + Vector3.Up * 2f;
            var top = foot + Vector3.Up * 2f;
            Tube(c, Rise + 0.5f + i * 0.06f, Stand1 - 1f, Beam, Jade, k => (top, top.Lerp(next, Mathf.Min(k * 5f, 1f))), _ => 0.04f, k => Fade(k, 0f, 0.3f), speed: 2f, gain: 1.6f);
        }
        Twinkle(c, 24, Rise + 0.3f, Rise + Stand1 - 0.5f, Jade, () => Within(Vector3.Zero, r * 0.8f, 0.5f, 2.6f), 0.5f);
        Shine(c, Vector3.Up, r * 1.5f, Jade.Main, age => 1.1f * Fade(age / (Rise + Stand1), 0.1f, 0.2f));
    }

    private void GoldenFissure(Cast c, float r)
    {
        const float Run = 0.5f, Hold = 1.2f;
        var reach = r * 1.7f;
        Func<float, Vector3> path = s => new Vector3(0.35f * Mathf.Sin(s * 11f), 0.06f, 0.4f + (reach - 0.4f) * s);
        // The seam is widest in its middle and runs out to a point at either end: two ribbons
        // laid head to head (a ribbon is fullest at its head and thins to nothing at its tail).
        const float Mid = 0.55f;
        Func<float, Vector3> near = s => path(s * Mid);
        Func<float, Vector3> far = s => path(1f - s * (1f - Mid));
        Func<float, float> run = k => Mathf.Min(k * (Run + Hold) / Run, 1f);
        foreach (var (tone, wide, gain) in new[] { (Gold, 0.7f, 1.7f), (Pearl, 0.18f, 1.5f) })
        {
            Ribbon(c, 0f, Run + Hold, 0, tone, near, k => (0f, Mathf.Min(run(k) / Mid, 1f)), wide, k => Fade(k, 0f, 0.3f), gain);
            Ribbon(c, Run * Mid, Run + Hold - Run * Mid, 0, tone, far, k => (1f - Mathf.Min(k * (Run + Hold - Run * Mid) / (Run * (1f - Mid)), 1f), 1f), wide, k => Fade(k, 0f, 0.3f), gain);
        }
        for (var i = 0; i < 9; i++)
        {
            var s = (i + 0.5f) / 9f;
            var at = FxKit.Flat(path(s));
            var when = s * Run;
            Disc(c, when, Hold, Cracks, Gold, at, _ => 0.9f, k => Fade(k, 0.02f, 0.4f), k => k * 0.8f);
            Column(c, when, 0.6f, Light, Gold, at, k => new Vector2(0.3f, 2.6f * Out(Mathf.Min(k * 4f, 1f))), k => 1f - k, gain: 1.6f, speed: 3f);
            Disc(c, when, 0.3f, Shock, Gold, at, k => 1f * Out(k), k => 1f - k, band: 0.1f);
            // Coins thrown up out of the seam, flipping.
            for (var j = 0; j < 5; j++)
            {
                var v = new Vector3(Rand(-2f, 2f), Rand(4f, 7f), Rand(-1f, 1f));
                var size = Rand(0.25f, 0.4f);
                Func<float, Vector3> coin = k => at + Vector3.Up * 0.2f + v * k * 0.8f + Vector3.Down * 0.5f * Gravity * k * k * 0.64f;
                Glyph(c, when + Rand(0f, 0.1f), 0.8f, Coin, j % 2 == 0 ? Gold : Honey, coin, _ => size, k => 1f - k * k * k, gain: 1.9f);
            }
            Spray(c, when, Gold, at + Vector3.Up * 0.1f, 5, 2f, 6f, 0.5f, 0.05f);
        }
        Twinkle(c, 40, 0.1f, Run + Hold - 0.4f, Gold, () => path(Rand(0f, 1f)) + new Vector3(Rand(-0.8f, 0.8f), Rand(0.2f, 2f), 0f), 0.6f);
        Hit(c, Run, Gold, path(1f) + Vector3.Up * 0.3f, r * 0.5f);
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Gold.Main, age => 1.5f * Fade(age / (Run + Hold), 0.1f, 0.4f));
    }

    private void DuneRoller(Cast c, float r)
    {
        const float Life = 1.4f;
        var reach = r * 1.8f;
        var half = r * 0.75f;
        Func<float, float> front = k => 0.5f + (reach - 0.5f) * Mathf.SmoothStep(0f, 1f, k);
        // It rises as it sets off and then keeps its height: at the end it thins away, not sinks.
        Func<float, float> tall = k => 3f * Out(Mathf.Min(k / 0.15f, 1f));
        Wall(c, 0f, Life, 1, Dust, k => (new Vector3(-half, 0f, front(k)), new Vector3(half, 0f, front(k))), tall, k => Fade(k, 0.05f, 0.35f), speed: 2.5f, gain: 2.2f);
        Wall(c, 0f, Life, 0, Dust, k => (new Vector3(-half, 0f, front(k) + 0.05f), new Vector3(half, 0f, front(k) + 0.05f)), k => tall(k) * 0.9f, k => Fade(k, 0.05f, 0.35f) * 0.5f, gain: 1.2f);
        Wall(c, 0f, Life, 1, Rubble, k => (new Vector3(-half * 0.9f, 0f, front(k) - 0.45f), new Vector3(half * 0.9f, 0f, front(k) - 0.45f)), k => tall(k) * 0.6f, k => Fade(k, 0.05f, 0.35f) * 0.8f, speed: 2f, gain: 1.2f);
        Drift(c, 34, 0f, Life - 0.5f, 0.9f, Puff, [Dust, Smoke], () => new Vector3(Rand(-half, half), Rand(0.2f, 1.2f), front(Rand(0f, 0.85f)) - 0.5f), () => new Vector3(0f, 0.7f, -0.6f), 1.5f, tumble: false, gain: 0.7f);
        for (var i = 0; i < 40; i++)
        {
            var x = Rand(-half, half);
            var k0 = Rand(0.05f, 0.85f);
            Spark(c, k0 * Life, Rand(0.3f, 0.5f), i % 2 == 0 ? Dust : Rubble, new Vector3(x, tall(k0) * Rand(0.3f, 1f), front(k0)), new Vector3(Rand(-1f, 1f), Rand(1f, 3f), Rand(2f, 5f)), Gravity * 0.8f, 0.07f, 0.04f);
        }
        Disc(c, 0f, Life + 0.6f, Glow, Dust, new Vector3(0f, 0f, reach * 0.5f), _ => 1f, k => Fade(k, 0.1f, 0.5f) * 0.4f, stretch: new Vector3(half, 1f, reach * 0.5f));
        Disc(c, Life - 0.15f, 0.5f, Shock, Dust, new Vector3(0f, 0f, reach), k => r * 1.1f * Out(k), k => 1f - k, band: 0.16f, gain: 1.2f);
    }

    private void PebbleOrbit(Cast c, float r)
    {
        const float Life = 3f;
        const int Stones = 8;
        var ring = r * 0.5f;
        Mark(c, 0f, Life, Reticle, Dust, Vector3.Zero, _ => ring * 1.4f, k => Fade(k, 0.05f, 0.15f), count: 8f, spin: 2f, gain: 1.1f);
        Disc(c, 0f, 0.4f, Shock, Dust, Vector3.Zero, k => ring * 1.6f * Out(k), k => 1f - k, band: 0.16f);
        for (var i = 0; i < Stones; i++)
        {
            var phase = i * Mathf.Tau / Stones;
            var size = Rand(0.16f, 0.3f);
            var axis = FxKit.RandomDirection();
            var high = 0.8f + 0.5f * (i % 3);
            var far = ring * (0.85f + 0.12f * (i % 2));
            Func<float, Vector3> at = k => Around(phase + k * Life * 2.6f) * far * Out(Mathf.Min(k * Life / 0.35f, 1f)) + Vector3.Up * (high * Out(Mathf.Min(k * Life / 0.35f, 1f)) + 0.1f * Mathf.Sin(k * Life * 4f + phase));
            Prop(c, 0f, Life, _rock, _stone, k => new Transform3D(new Basis(axis, k * Life * 3f) * Basis.FromScale(Vector3.One * size), at(k)));
            Trail(c, 0f, Life, Dust, at, size * 1.6f, 0.07f, gain: 0.9f);
            var last = at(1f);
            var away = FxKit.Flat(last).Normalized();
            Prop(c, Life, 0.3f, _rock, _stone, k => new Transform3D(Basis.FromScale(Vector3.One * size), last + away * 5f * k + Vector3.Down * k * k));
            Line(c, Life, 0.3f, Dust, k => last + away * (5f * k - 0.5f), _ => away * 1.2f, 0.1f, k => 1f - k);
        }
        Disc(c, Life, 0.4f, Shock, Dust, Vector3.Zero, k => r * 1.2f * Out(k), k => 1f - k, band: 0.14f, gain: 1.2f);
        Drift(c, 14, 0f, Life - 1f, 1f, Puff, [Dust], () => Within(Vector3.Zero, ring, 0.1f, 0.4f), () => Vector3.Up * 0.5f, 1f, tumble: false, gain: 0.5f);
        Shine(c, Vector3.Up, r, Dust.Main, age => 0.5f * Fade(age / Life, 0.05f, 0.1f));
    }

    private void AmberTomb(Cast c, float r)
    {
        const float Seal = 0.35f, Hold = 2f, Crack = 0.4f;
        var spot = new Vector3(0f, 0f, r * 0.2f);
        var heart = spot + Vector3.Up * 1.3f;
        var life = Seal + Hold;
        Mark(c, 0f, life + Crack, Comb, Honey, spot, _ => r * 0.6f, k => Fade(k, 0.04f, 0.25f), count: 8f, spin: 0.8f, gain: 1.3f);
        // Drops of resin fly in and set.
        for (var i = 0; i < 16; i++)
        {
            var from = heart + FxKit.RandomDirection() * Rand(2f, 3f);
            Func<float, Vector3> at = k => from.Lerp(heart, k * k);
            Glyph(c, Rand(0f, Seal * 0.6f), Seal * 0.7f, Glint, Honey, at, _ => 0.6f, _ => 1f, gain: 1.6f);
        }
        Prop(c, Seal * 0.5f, life - Seal * 0.5f, ShardMesh, AmberBlock, k => new Transform3D(new Basis(Vector3.Up, 0.4f) * Basis.FromScale(new Vector3(4.6f, 6.6f, 4.6f) * Overshoot(Mathf.Min(k * life / Seal, 1f))), heart + Vector3.Up * 0.1f));
        Globe(c, "energy", Shell, Seal * 0.5f, life - Seal * 0.5f, Honey, _ => heart, _ => 1.5f, k => Fade(k, 0.1f, 0.05f) * (0.5f + 0.15f * Mathf.Sin(k * 20f)), gain: 1.2f);
        Disc(c, Seal, 0.4f, Shock, Honey, spot, k => r * 0.9f * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
        Flare(c, Seal, 0.3f, Star, Honey, heart, 3.4f);
        Disc(c, Seal, Hold, Glow, Honey, spot, _ => r * 0.6f, k => Fade(k, 0.05f, 0.1f) * 0.6f);
        Twinkle(c, 30, Seal, life - 0.2f, Pearl, () => heart + new Vector3(Rand(-0.7f, 0.7f), Rand(-1.2f, 1.4f), Rand(0.2f, 0.8f)), 0.55f);
        Hit(c, life, Honey, heart, r * 0.7f);
        for (var i = 0; i < 18; i++)
        {
            var v = FxKit.RandomDirection() * Rand(2f, 5f) + Vector3.Up * Rand(1f, 3f);
            var spin = FxKit.RandomDirection();
            Prop(c, life, Crack + 0.2f, ShardMesh, AmberBlock, k => new Transform3D(new Basis(spin, k * 7f) * Basis.FromScale(new Vector3(0.6f, 1f, 0.6f) * (1f - 0.6f * k)), heart + v * k * 0.6f + Vector3.Down * 0.5f * Gravity * k * k * 0.36f));
        }
        Shine(c, heart, r * 1.3f, Honey.Main, age => age < life ? 0.9f * Mathf.Min(age / Seal, 1f) : 2f * Mathf.Max(0f, 1f - (age - life) / 0.5f));
    }

    // ---- Arcane.

    private void StarMap(Cast c, float r)
    {
        const float Draw = 1f, Strike = 1.35f, After = 0.7f;
        Mark(c, 0f, Strike + After, Galaxy, Azure, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.3f), count: 10f, spin: 1f, gain: 1.4f);
        Mark(c, 0f, Strike + After, Filigree, Stellar, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.3f) * 0.6f, count: 12f, spin: -1f);
        // A constellation hung over the ground: stars lit in turn, joined by threads, then all fall.
        Vector2[] plan = [new(-0.75f, 0.2f), new(-0.4f, -0.35f), new(0f, 0.1f), new(0.35f, -0.4f), new(0.75f, -0.1f), new(0.5f, 0.55f), new(-0.15f, 0.65f)];
        var stars = plan.Select(p => new Vector3(p.X * r, 2.2f + 0.5f * p.Y, p.Y * r)).ToArray();
        (int, int)[] links = [(0, 1), (1, 2), (2, 3), (3, 4), (4, 5), (5, 6), (6, 2), (6, 0)];
        const float Each = Draw / 8f;
        for (var i = 0; i < stars.Length; i++)
        {
            var at = stars[i];
            var start = i * Each;
            Flare(c, start, Strike - start, Star, Stellar, _ => at, k => 1.1f + 0.25f * Mathf.Sin(k * 30f + i), _ => 1f, gain: 1.6f);
            Glyph(c, start, 0.3f, Glint, Pearl, at, 2f, 1.8f);
            Column(c, start, Strike - start, Beam, Azure, FxKit.Flat(at), _ => new Vector2(0.02f, at.Y), _ => 0.35f, speed: 2f);
            var land = FxKit.Flat(at);
            Func<float, Vector3> fall = k => at.Lerp(land + Vector3.Up * 0.2f, k * k);
            Flare(c, Strike, 0.16f, Star, Stellar, fall, _ => 1.4f, _ => 1f);
            Trail(c, Strike, 0.16f, Stellar, fall, 0.3f, 0.7f, gain: 1.5f);
            Hit(c, Strike + 0.16f, Stellar, land + Vector3.Up * 0.2f, 0.9f);
        }
        for (var i = 0; i < links.Length; i++)
        {
            var (a, b) = (stars[links[i].Item1], stars[links[i].Item2]);
            var start = (i + 0.5f) * Each;
            var life = Strike - start;
            Tube(c, start, life, Beam, Azure, k => (a, a.Lerp(b, Mathf.Min(k * life / Each, 1f))), _ => 0.03f, k => Fade(k, 0f, 0.1f), speed: 2f, gain: 1.6f);
        }
        Cue(c, Strike + 0.16f, () => Stage.Explosion(At(c, Vector3.Up * 0.4f), r * 0.6f, Azure.Main, FxBurst.Dome));
        Twinkle(c, 30, 0f, Strike, Pearl, () => Within(Vector3.Zero, r, 0.3f, 3f), 0.45f);
        Shine(c, Vector3.Up, r * 1.6f, Azure.Main, age => age < Strike ? 0.8f : 2.4f * Mathf.Max(0f, 1f - (age - Strike) / After));
    }

    private void ClockworkRewind(Cast c, float r)
    {
        const float Life = 2.6f;
        var reach = r * 0.6f;
        Mark(c, 0f, Life, Dial, Chrono, Vector3.Zero, _ => reach * 1.4f, k => Fade(k, 0.05f, 0.15f), spin: -9f, gain: 1.5f);
        Mark(c, 0.15f, Life - 0.15f, Dial, Gold, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * 1.5f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f))), new Vector3(0f, 2.2f, -0.5f)), k => Fade(k, 0.05f, 0.15f) * 0.9f, spin: -14f, gain: 1.6f);
        Column(c, 0f, Life, Spiral, Chrono, Vector3.Zero, _ => new Vector2(reach * 0.8f, 2.6f), k => Fade(k, 0.06f, 0.15f) * 0.6f, twist: 1.5f, speed: -3f, gain: 1.2f);
        // Echoes of the caster stepping back along where they came from.
        for (var i = 0; i < 5; i++)
        {
            var start = 0.2f + i * 0.4f;
            Func<float, Vector3> at = k => new Vector3(0f, 0.85f, 0f) + Around(2.6f + i * 0.5f) * (2.4f - i * 0.35f) * (1f - Out(k));
            Globe(c, "energy", Shell, start, 0.6f, i % 2 == 0 ? Chrono : Gold, at, _ => 1f, k => (1f - k) * 0.8f, mesh: _ghost, gain: 1.2f);
        }
        // Sparks run backwards: falling up and inward.
        for (var i = 0; i < 40; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            var far = Rand(1f, r * 0.9f);
            var high = Rand(0.2f, 2.4f);
            Line(c, Rand(0f, Life - 0.5f), 0.5f, i % 2 == 0 ? Chrono : Gold, k => way * far * (1f - 0.8f * k * k) + Vector3.Up * (high * (1f - k) + 1f * k), _ => -way * 0.5f + Vector3.Up * 0.2f, 0.05f, k => Fade(k, 0.2f, 0.2f));
        }
        for (var t = 0.3f; t < Life - 0.5f; t += 0.5f)
            Disc(c, t, 0.5f, Shock, Chrono, Vector3.Zero, k => r * (1f - 0.8f * Out(k)), k => Fade(k, 0.2f, 0.3f) * 0.8f, band: 0.1f);
        Twinkle(c, 20, 0f, Life - 0.4f, Gold, () => Within(Vector3.Zero, reach * 1.3f, 0.3f, 2.6f), 0.45f);
        Flare(c, Life - 0.3f, 0.3f, Star, Gold, Vector3.Up, 3f);
        Shine(c, Vector3.Up, r, Chrono.Main, age => 1f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void PrismRay(Cast c, float range)
    {
        const float Charge = 0.3f, Beam1 = 0.8f;
        var length = Mathf.Max(range, 2f);
        var hand = new Vector3(0f, 1.2f, 0.6f);
        var end = new Vector3(0f, 1.2f, length);
        Glyph(c, 0f, Charge + Beam1, Gem, Pearl, _ => hand, k => 1.2f * Overshoot(Mathf.Min(k * (Charge + Beam1) / Charge, 1f)), k => Fade(k, 0.02f, 0.15f), k => new Vector3(Mathf.Sin(k * 6f), Mathf.Cos(k * 6f), 0f), gain: 1.9f);
        Flare(c, 0f, Charge, Star, Pearl, _ => hand, k => 0.6f + 1.8f * k, k => k);
        Tube(c, Charge - 0.05f, 0.2f, Beam, Pearl, _ => (hand + Vector3.Forward * 2f, hand), _ => 0.07f, k => 1f - k, speed: 6f, gain: 1.7f);
        // The white ray splits into six colours that fan apart and braid on down the line.
        for (var i = 0; i < Prism.Length; i++)
        {
            var phase = i * Mathf.Tau / Prism.Length;
            Func<float, Vector3> path = s => hand.Lerp(end, s) + new Vector3(Mathf.Cos(s * 9f + phase), Mathf.Sin(s * 9f + phase), 0f) * 0.42f * Mathf.Sin(Mathf.Min(s * 4f, 1f) * Mathf.Pi * 0.5f);
            Ribbon(c, Charge, Beam1, 0, Prism[i], path, k => (0f, Mathf.Min(k * 6f, 1f)), 0.24f, k => Fade(k, 0f, 0.35f), gain: 1.7f);
        }
        Tube(c, Charge, Beam1, Beam, Pearl, _ => (hand, end), k => 0.1f * (1f - 0.5f * k), k => Fade(k, 0f, 0.35f), speed: 6f, gain: 1.6f);
        Hit(c, Charge + 0.1f, Pearl, end, 1.1f);
        for (var i = 0; i < 36; i++)
            Spark(c, Charge + Rand(0.1f, Beam1 - 0.3f), Rand(0.3f, 0.5f), Prism[i % Prism.Length], end, FxKit.RandomDirection() * Rand(2f, 6f), Gravity * 0.5f, 0.06f, 0.04f);
        Mark(c, Charge + 0.1f, Beam1, Sunburst, Pearl, FxKit.Flat(end), _ => 1.5f, k => Fade(k, 0.05f, 0.5f), count: 12f, spin: 4f, gain: 1.3f);
        Twinkle(c, 24, Charge, Charge + Beam1 - 0.3f, Pearl, () => hand.Lerp(end, Rand(0f, 1f)) + FxKit.Jitter(0.5f), 0.5f, 0.3f);
        Shine(c, hand.Lerp(end, 0.5f), length, Pearl.Main, age => age < Charge ? 0.5f : 1.8f * Fade((age - Charge) / Beam1, 0f, 0.4f));
    }

    private void GravityLens(Cast c, float r)
    {
        const float Pull = 1.8f, After = 0.6f;
        var heart = new Vector3(0f, 1.2f, r * 0.15f);
        var ground = FxKit.Flat(heart);
        Mark(c, 0f, Pull, Ripples, Amethyst, ground, _ => r, k => Fade(k, 0.08f, 0.05f), count: 5f, spin: -1.6f, gain: 1.3f);
        Mark(c, 0f, Pull, Galaxy, Void, ground, k => r * (1f - 0.4f * k), k => Fade(k, 0.08f, 0.05f), count: 9f, spin: 5f, gain: 1.3f);
        Globe(c, "void", 0, 0f, Pull, Void, _ => heart, k => 0.3f + 0.35f * k, _ => 1f);
        Mark(c, 0f, Pull, Corona, Amethyst, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (0.6f + 0.7f * k) * 2f), heart), k => Fade(k, 0.1f, 0.05f), spin: 3f, count: 10f, gain: 1.5f);
        // Rings of bent light closing in on it.
        for (var i = 0; i < 7; i++)
            Glyph(c, i * 0.24f, 0.5f, Bubble, i % 2 == 0 ? Amethyst : Fey, _ => heart, k => 4.4f * (1f - k) + 0.9f, k => Fade(k, 0.3f, 0.2f) * 0.8f, gain: 1.4f);
        for (var i = 0; i < 50; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var far = Rand(1.6f, r);
            var high = Rand(-0.6f, 1.2f);
            Func<float, Vector3> at = k => heart + Around(phase + k * 4f) * far * (1f - k) * (1f - k) + Vector3.Up * high * (1f - k);
            Line(c, Rand(0f, Pull - 0.6f), 0.6f, i % 3 == 0 ? Pearl : Amethyst, at, k => Heading(at, k, 0.5f + k), 0.05f, k => Fade(k, 0.2f, 0.1f));
        }
        Hit(c, Pull, Amethyst, heart, r);
        Cue(c, Pull, () => Stage.Explosion(At(c, heart), r * 0.7f, Amethyst.Main, FxBurst.Dome));
        Glyph(c, Pull, 0.5f, Bubble, Pearl, _ => heart, k => 0.6f + 6f * Out(k), k => 1f - k, gain: 1.7f);
        Mark(c, Pull, After, Rays, Fey, ground, _ => r * 1.5f, k => 1f - k, k => Out(Mathf.Min(k * 3f, 1f)), count: 18f, gain: 1.4f);
        Twinkle(c, 20, Pull, Pull + 0.3f, Pearl, () => heart + FxKit.RandomDirection() * Rand(0.5f, 2.6f), 0.6f);
        Shine(c, heart, r * 1.6f, Amethyst.Main, age => age < Pull ? 0.7f : 2.6f * Mathf.Max(0f, 1f - (age - Pull) / After));
    }

    private void EyeOfInsight(Cast c, float r)
    {
        const float Open = 0.4f, Life = 3f;
        var eye = new Vector3(0f, 3.3f, 0f);
        Mark(c, 0f, Life, Reticle, Azure, Vector3.Zero, _ => r, k => Fade(k, 0.04f, 0.2f), count: 16f, spin: 1f, gain: 1.3f);
        Mark(c, 0f, Life, Ripples, Azure, Vector3.Zero, _ => r, k => Fade(k, 0.08f, 0.2f) * 0.4f, count: 4f, spin: 0.7f);
        Glyph(c, 0f, Life, Eye, Azure, k => eye + Vector3.Up * 0.1f * Mathf.Sin(k * Life * 3f), k => 2.6f * Overshoot(Mathf.Min(k * Life / Open, 1f)), k => Fade(k, 0.03f, 0.12f), gain: 1.9f, slim: 1f);
        Mark(c, 0f, Life, Rays, Azure, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * 2.6f), eye + Vector3.Forward * 0.05f), k => Fade(k, 0.1f, 0.15f) * 0.6f, k => Out(Mathf.Min(k * 4f, 1f)), count: 18f, spin: 1.5f);
        // Its gaze sweeps round the circle, lighting what it finds.
        Func<float, Vector3> gaze = k => Around(k * Life * 2.2f) * r * (0.35f + 0.5f * (0.5f + 0.5f * Mathf.Sin(k * Life * 1.3f)));
        Tube(c, Open, Life - Open, Beam, Azure, k => (eye + Vector3.Down * 0.3f, gaze(k)), _ => 0.12f, k => Fade(k, 0.05f, 0.15f) * 0.8f, speed: 5f, gain: 1.4f);
        Disc(c, Open, Life - Open, Glow, Azure, k => Lay(gaze(k), 1f), k => Fade(k, 0.05f, 0.15f) * 0.9f, gain: 1.4f);
        Mark(c, Open, Life - Open, Reticle, Pearl, k => Lay(gaze(k), 0.7f, new Basis(Vector3.Up, k * 9f)), k => Fade(k, 0.05f, 0.15f), _ => 1f, count: 6f, gain: 1.4f);
        for (var i = 0; i < 6; i++)
        {
            var k0 = (i + 1f) / 7.5f;
            var spot = gaze(k0);
            var when = Open + k0 * (Life - Open);
            Glyph(c, when, 0.9f, Eye, Pearl, _ => spot + Vector3.Up * 1.5f, k => 0.8f * Overshoot(Mathf.Min(k * 4f, 1f)), k => Fade(k, 0.05f, 0.5f), gain: 1.6f);
            Mark(c, when, 0.9f, Reticle, Azure, spot, _ => 0.6f, k => Fade(k, 0.05f, 0.5f), count: 4f, spin: 4f);
        }
        Twinkle(c, 20, 0f, Life - 0.4f, Azure, () => Within(Vector3.Zero, r, 0.3f, 3f), 0.45f);
        Shine(c, Vector3.Up, r * 1.4f, Azure.Main, age => 0.9f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void CharmHeart(Cast c, float range)
    {
        const float Kiss = 0.2f, Flight = 0.55f, Hold = 1.2f;
        var length = Mathf.Max(range, 2f);
        var hand = new Vector3(0f, 1.3f, 0.5f);
        var foe = new Vector3(0f, 1.2f, length);
        Glyph(c, 0f, Kiss + 0.1f, Heart, Rose, _ => hand, k => 0.4f + 0.7f * k, _ => 1f, gain: 1.8f);
        Flare(c, 0f, Kiss, Star, Rose, _ => hand, k => 0.6f + 1.2f * k, _ => 0.8f);
        Func<float, Vector3> at = k => hand.Lerp(foe, k) + new Vector3(0.35f * Mathf.Sin(k * 14f), 0.25f * Mathf.Sin(k * Mathf.Pi), 0f);
        Glyph(c, Kiss, Flight, Heart, Rose, at, k => 1.1f + 0.12f * Mathf.Sin(k * 40f), _ => 1f, gain: 1.9f);
        Flare(c, Kiss, Flight, Mote, Rose, at, _ => 1.8f, _ => 0.6f, gain: 1f);
        Trail(c, Kiss, Flight, Rose, at, 0.7f, 0.3f, gain: 1.5f);
        Trail(c, Kiss, Flight, Pearl, at, 0.2f, 0.4f, gain: 1.2f);
        for (var i = 0; i < 12; i++)
        {
            var s = Rand(0.05f, 0.95f);
            var p = at(s);
            Glyph(c, Kiss + s * Flight, 0.6f, Heart, i % 2 == 0 ? Petal : Pearl, k => p + Vector3.Up * 0.5f * k + FxKit.Jitter(0.01f), k => 0.36f * (1f - 0.5f * k), k => 1f - k, gain: 1.6f);
        }
        var hit = Kiss + Flight;
        var ground = FxKit.Flat(foe);
        Hit(c, hit, Rose, foe, 1.1f);
        Glyph(c, hit, 0.5f, Heart, Rose, foe, 2.4f, 1.6f);
        Mark(c, hit, Hold, Lotus, Rose, ground, _ => 1.4f, k => Fade(k, 0.03f, 0.3f), count: 6f, spin: 3f, gain: 1.5f);
        // Charmed: a heart turning overhead, small ones drifting up.
        Glyph(c, hit, Hold, Heart, Rose, k => foe + Vector3.Up * (1.5f + 0.1f * Mathf.Sin(k * Hold * 6f)), k => 0.9f * Overshoot(Mathf.Min(k * Hold / 0.25f, 1f)), k => Fade(k, 0.03f, 0.2f), gain: 1.8f);
        Drift(c, 16, hit, hit + Hold - 0.8f, 0.9f, Heart, [Rose, Petal, Pearl], () => Within(ground, 0.8f, 0.4f, 1.2f), () => new Vector3(Rand(-0.2f, 0.2f), Rand(0.9f, 1.6f), 0f), 0.4f, sway: 0.2f, tumble: false, gain: 1.7f);
        Column(c, hit, Hold, Spiral, Rose, ground, _ => new Vector2(0.7f, 2.2f), k => Fade(k, 0.05f, 0.2f) * 0.6f, twist: 1.5f, speed: 2f, gain: 1.2f);
        Twinkle(c, 16, hit, hit + Hold - 0.4f, Pearl, () => Within(ground, 1.2f, 0.3f, 2.4f), 0.45f);
        Shine(c, foe, 4f, Rose.Main, age => age < hit ? 0.5f : 0.9f + 1.4f * Bump(age, hit, 0.5f));
    }

    private void RuneCascade(Cast c, float r)
    {
        const int Rings = 5;
        const float Every = 0.16f, Fire1 = Rings * Every + 0.35f, After = 0.8f;
        var spot = new Vector3(0f, 0f, r * 0.2f);
        Mark(c, 0f, Fire1 + After, Filigree, Arcane, spot, _ => r * 0.85f, k => Fade(k, 0.03f, 0.3f), count: 12f, spin: 1f, gain: 1.3f);
        // A tower of circles, each smaller and higher, turning against the one below.
        for (var i = 0; i < Rings; i++)
        {
            var high = 0.9f + i * 0.95f;
            var size = r * (0.62f - 0.09f * i);
            var way = i % 2 == 0 ? 1f : -1f;
            var start = i * Every;
            var life = Fire1 + After * 0.5f - start;
            var style = i % 2 == 0 ? Pentacle : Filigree;
            Mark(c, start, life, style, i % 2 == 0 ? Arcane : Fey, k => Lay(spot + Vector3.Up * high, size * Overshoot(Mathf.Min(k * life / 0.25f, 1f)), new Basis(Vector3.Up, k * life * 1.2f * way)), k => Fade(k, 0.02f, 0.2f),
                count: i % 2 == 0 ? 6f : 9f, gain: 1.6f);
            Disc(c, start, life, Band, Arcane, k => Lay(spot + Vector3.Up * high, size * 1.02f), k => Fade(k, 0.02f, 0.2f) * 0.5f, band: 0.1f, gain: 1.3f);
            Glyph(c, start, 0.3f, Glint, Pearl, spot + Vector3.Up * high, 1.6f, 1.7f);
        }
        var crown = spot + Vector3.Up * (0.9f + Rings * 0.95f);
        Flare(c, Rings * Every, Fire1 - Rings * Every, Star, Arcane, _ => crown, k => 1f + 2.4f * k, _ => 1f);
        Tube(c, Fire1, After, Beam, Arcane, _ => (crown, spot), k => 0.5f * (1f - k * 0.7f), k => Fade(k, 0f, 0.5f), speed: 6f, gain: 1.8f);
        Tube(c, Fire1, After, Beam, Pearl, _ => (crown, spot), k => 0.18f * (1f - k * 0.7f), k => Fade(k, 0f, 0.5f), speed: 6f, gain: 1.6f);
        Hit(c, Fire1, Arcane, spot + Vector3.Up * 0.3f, r * 0.9f);
        Cue(c, Fire1, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.4f), r * 0.6f, Arcane.Main, FxBurst.Dome));
        Mark(c, Fire1, After, Pentacle, Pearl, spot, k => r * 0.85f * (1f + 0.2f * Out(k)), k => 1f - k, _ => 1f, count: 6f, gain: 1.5f);
        Twinkle(c, 30, 0f, Fire1 + 0.3f, Pearl, () => Within(spot, r * 0.6f, 0.5f, 5.5f), 0.5f);
        Shine(c, spot + Vector3.Up * 2f, r * 1.7f, Arcane.Main, age => age < Fire1 ? 0.4f + 0.8f * age / Fire1 : 2.6f * Mathf.Max(0f, 1f - (age - Fire1) / After));
    }

    private void NebulaBurst(Cast c, float r)
    {
        const float Gather = 1.4f, After = 0.8f;
        var heart = new Vector3(0f, 1.5f, r * 0.15f);
        var ground = FxKit.Flat(heart);
        Tone[] gases = [Nebula, Amethyst, Azure];
        Mark(c, 0f, Gather + After, Galaxy, Nebula, ground, _ => r, k => Fade(k, 0.05f, 0.3f), count: 10f, spin: 3f, gain: 1.5f);
        for (var i = 0; i < 34; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var far = Rand(0.6f, 1f) * r * 0.75f;
            var high = Rand(-0.8f, 1f);
            var size = Rand(1.4f, 2.4f);
            var start = Rand(0f, 0.5f);
            var life = Gather - start;
            Glyph(c, start, life, Puff, gases[i % gases.Length], k => heart + Around(phase + k * 2.5f) * far * (1f - 0.75f * k * k) + Vector3.Up * high * (1f - k), k => size * (1f - 0.4f * k), k => Fade(k, 0.25f, 0.1f) * 0.75f, gain: 1f);
        }
        Drift(c, 60, 0f, Gather - 0.5f, 0.5f, Glint, [Pearl, Nebula, Azure], () => heart + FxKit.RandomDirection() * Rand(0.4f, r * 0.75f), () => Vector3.Zero, 0.5f, sway: 0.05f, tumble: false, gain: 1.8f);
        Globe(c, "energy", Orb, 0.3f, Gather - 0.3f, Nebula, _ => heart, k => 0.15f + 0.4f * k * k, _ => 1f, gain: 1.6f);
        Flare(c, 0.3f, Gather - 0.3f, Star, Pearl, _ => heart, k => 0.6f + 2.2f * k * k, _ => 1f);
        Hit(c, Gather, Nebula, heart, r);
        Cue(c, Gather, () => Stage.Explosion(At(c, heart), r * 0.85f, Nebula.Main, FxBurst.Dome));
        Mark(c, Gather, After, Rays, Pearl, ground, _ => r * 1.6f, k => 1f - k, k => Out(Mathf.Min(k * 3f, 1f)), count: 20f, gain: 1.5f);
        for (var i = 0; i < 40; i++)
        {
            var way = FxKit.RandomDirection();
            var speed = Rand(3f, 7f);
            Func<float, Vector3> at = k => heart + way * speed * 0.7f * Out(k);
            Glyph(c, Gather, Rand(0.5f, 0.8f), Glint, gases[i % gases.Length], at, k => 0.9f * (1f - k), _ => 1f, gain: 1.8f);
        }
        Shine(c, heart, r * 1.8f, Nebula.Main, age => age < Gather ? 0.5f + 0.6f * age / Gather : 2.8f * Mathf.Max(0f, 1f - (age - Gather) / After));
    }

    private void UmbralEclipse(Cast c, float r)
    {
        const float Cover = 1.2f, Dark = 0.6f, After = 0.8f;
        var sun = new Vector3(0f, 3f, r * 0.1f);
        var ground = FxKit.Flat(sun);
        var face = Stand(Vector3.Back);
        var total = Cover + Dark;
        Mark(c, 0f, total + After, Sunburst, Sun, ground, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f) * (k * (total + After) > Cover && k * (total + After) < total ? 0.35f : 1f), count: 16f, spin: 1f, gain: 1.2f);
        Mark(c, 0f, total, Corona, Sun, k => new Transform3D(face * Basis.FromScale(Vector3.One * 2.4f), sun), k => Fade(k, 0.08f, 0.02f), spin: 2f, count: 12f, gain: 1.8f);
        Globe(c, "energy", Orb, 0f, Cover, Sun, _ => sun, _ => 1.05f, k => 1f - k * 0.9f, gain: 1.4f);
        // The moon slides across the sun's face.
        Globe(c, "void", 0, 0f, total, Void, k => sun + new Vector3(2.6f * (1f - Mathf.SmoothStep(0f, 1f, Mathf.Min(k * total / Cover, 1f))), 0f, 0.35f), _ => 1.12f, k => Fade(k, 0.1f, 0.02f));
        Disc(c, Cover, Dark + After, Swirl, Void, ground, _ => r, k => Fade(k, 0.1f, 0.4f) * 0.8f, spin: 2f);
        Mark(c, Cover, Dark, Corona, Shadow, ground, _ => r * 1.1f, k => Fade(k, 0.2f, 0.2f), spin: 2f, count: 10f, gain: 1.3f);
        Twinkle(c, 24, Cover, total, Pearl, () => Within(ground, r, 0.4f, 4f), 0.5f);
        // The diamond ring: light bursts back from one edge.
        var bead = sun + new Vector3(-0.85f, 0.85f, 0.4f);
        Glyph(c, total - 0.05f, 0.7f, Glint, Pearl, bead, 4.4f, 2f);
        Flare(c, total, 0.5f, Star, Sun, bead, 5f);
        Column(c, total, After, Light, Sun, ground, _ => new Vector2(r * 0.5f, 10f), k => (1f - k) * (1f - k), gain: 1.8f, speed: -2f);
        Hit(c, total, Sun, ground + Vector3.Up * 0.3f, r * 1.1f);
        Cue(c, total, () => Stage.Explosion(At(c, ground + Vector3.Up * 0.5f), r * 0.7f, Sun.Main, FxBurst.Dome));
        Mark(c, total, After, Rays, Sun, ground, _ => r * 1.6f, k => 1f - k, k => Out(Mathf.Min(k * 3f, 1f)), count: 20f, gain: 1.5f);
        Shine(c, ground + Vector3.Up, r * 1.8f, Sun.Main, age => age < Cover ? 1.2f * (1f - 0.8f * age / Cover) : age < total ? 0.2f : 3f * Mathf.Max(0f, 1f - (age - total) / After));
    }

    private void TimeShatter(Cast c, float r)
    {
        const float Tick = 1f, Freeze = 0.35f, After = 0.9f;
        var spot = new Vector3(0f, 0f, r * 0.15f);
        var heart = spot + Vector3.Up * 1.2f;
        var crack = Tick + Freeze;
        Mark(c, 0f, crack, Dial, Chrono, spot, _ => r * 0.9f, k => Fade(k, 0.04f, 0.02f), spin: 6f, gain: 1.5f);
        Mark(c, 0f, Tick, Dial, Gold, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * 1.3f), heart + Vector3.Up * 1.2f), k => Fade(k, 0.1f, 0.05f), spin: 12f, gain: 1.6f);
        Globe(c, "energy", Hexes, 0f, crack, Chrono, _ => heart, k => 1.5f * Out(Mathf.Min(k * crack / 0.3f, 1f)), k => Fade(k, 0.05f, 0.02f) * 0.7f, gain: 1.2f);
        for (var t = 0f; t < Tick; t += 0.2f)
        {
            Disc(c, t, 0.2f, Shock, Chrono, spot, k => r * 0.9f * Out(k), k => (1f - k) * 0.6f, band: 0.08f);
            Glyph(c, t, 0.2f, Glint, Gold, heart + Vector3.Up * 1.2f, 1f, 1.6f);
        }
        // It stops; everything hangs white for a breath; then it breaks like glass.
        Flare(c, Tick, Freeze, Star, Pearl, _ => heart, k => 2.2f + 0.6f * k, _ => 1f);
        Globe(c, "energy", Shell, Tick, Freeze, Pearl, _ => heart, _ => 1.5f, _ => 1f, gain: 1.5f);
        Bolt(c, Tick, Freeze, Pearl, heart + new Vector3(-1.3f, 1f, 0.2f), heart + new Vector3(1.1f, -1f, 0.2f), 0.06f, 7, 0.3f);
        Bolt(c, Tick + 0.1f, Freeze - 0.1f, Pearl, heart + new Vector3(0.9f, 1.2f, 0.2f), heart + new Vector3(-0.6f, -1.1f, 0.2f), 0.05f, 6, 0.3f);
        Hit(c, crack, Chrono, heart, r);
        Disc(c, crack, After, Cracks, Chrono, spot, _ => r * 0.9f, k => Fade(k, 0.02f, 0.5f), k => k);
        for (var i = 0; i < 40; i++)
        {
            var way = FxKit.RandomDirection();
            var v = way * Rand(3f, 7f) + Vector3.Up * Rand(0f, 3f);
            var size = Rand(0.4f, 1f);
            var spin = Rand(0f, Mathf.Tau);
            var turn = Rand(-9f, 9f);
            Func<float, Vector3> at = k => heart + way * 1.3f + v * k * 0.7f + Vector3.Down * 0.5f * Gravity * k * k * 0.3f;
            Glyph(c, crack, Rand(0.6f, 0.9f), Gem, i % 3 == 0 ? Gold : i % 3 == 1 ? Chrono : Pearl, at, k => size * (1f - 0.4f * k), k => 1f - k * k, k => new Vector3(Mathf.Sin(spin + k * turn), Mathf.Cos(spin + k * turn), 0f), gain: 1.8f, slim: 0.7f);
        }
        Twinkle(c, 20, crack, crack + 0.5f, Pearl, () => heart + FxKit.RandomDirection() * Rand(0.5f, 3f), 0.7f);
        Shine(c, heart, r * 1.7f, Chrono.Main, age => age < Tick ? 0.7f : age < crack ? 1.6f : 2.6f * Mathf.Max(0f, 1f - (age - crack) / After));
    }

    private void WishingStars(Cast c, float r)
    {
        const float Rise = 1.5f, Burst = 1.9f, After = 1.1f;
        var wish = new Vector3(0f, 4f, 0f);
        Mark(c, 0f, Burst + After, Galaxy, Stellar, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.3f), count: 10f, spin: 1.5f, gain: 1.5f);
        Mark(c, 0f, Burst + After, Pentacle, Stellar, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.05f, 0.3f), count: 5f, spin: 2f, gain: 1.2f);
        // Stars lift off the ground one by one and gather overhead.
        for (var i = 0; i < 18; i++)
        {
            var from = Within(Vector3.Zero, r * 0.9f, 0.1f, 0.2f);
            var start = Rand(0f, Rise - 0.7f);
            var side = Around(Rand(0f, Mathf.Tau)) * Rand(0.5f, 1.5f);
            var life = Burst - start;
            Func<float, Vector3> at = k => from.Lerp(wish, Mathf.SmoothStep(0f, 1f, Mathf.Min(k * life / 0.9f, 1f))) + side * Mathf.Sin(Mathf.Min(k * life / 0.9f, 1f) * Mathf.Pi);
            Flare(c, start, life, Star, i % 3 == 0 ? Fey : Stellar, at, k => 0.8f, _ => 1f, gain: 1.5f);
            Trail(c, start, 0.9f, Stellar, k => at(k * 0.9f / life), 0.16f, 0.35f, gain: 1.3f);
            Mark(c, start, 0.5f, Ripples, Stellar, from, k => 0.2f + 0.6f * k, k => 1f - k, count: 2f, spin: 0f);
        }
        Flare(c, 0.6f, Burst - 0.6f, Star, Pearl, _ => wish, k => 1f + 3f * k * k, _ => 1f);
        Globe(c, "energy", Orb, 0.6f, Burst - 0.6f, Stellar, _ => wish, k => 0.2f + 0.4f * k, _ => 1f, gain: 1.7f);
        // Then it bursts, and they come down everywhere as a slow rain of light.
        Glyph(c, Burst, 0.5f, Glint, Pearl, wish, 6f, 2f);
        Drift(c, 70, Burst, Burst + 0.15f, After, Glint, [Stellar, Pearl, Fey], () => wish + FxKit.RandomDirection() * 0.3f, () => Around(Rand(0f, Mathf.Tau)) * Rand(0.5f, r * 0.9f) + Vector3.Down * Rand(2.6f, 3.6f), 0.6f, sway: 0.1f, tumble: false, gain: 1.9f);
        Mark(c, Burst + 0.5f, After - 0.5f, Rays, Stellar, Vector3.Zero, _ => r * 1.3f, k => (1f - k) * 0.8f, k => Out(k), count: 18f);
        Disc(c, Burst + 0.6f, 0.5f, Shock, Stellar, Vector3.Zero, k => r * 1.2f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Shine(c, Vector3.Up * 2f, r * 1.8f, Stellar.Main, age => age < Burst ? 0.5f + 0.6f * age / Burst : 2.4f * Mathf.Max(0f, 1f - (age - Burst) / After));
    }
}
