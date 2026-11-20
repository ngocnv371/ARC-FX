using Godot;

namespace Arc.Fx;

/// <summary>
/// The second set of skills, built from the same pieces as the first (see FxSkills.cs), grouped
/// by look: blades, earth, fire, frost, storm, nature, arcane, holy and shadow.
/// </summary>
internal sealed partial class FxSkills
{
    private static readonly Tone Jade = new(new(0.2f, 0.95f, 0.7f), new(0.88f, 1f, 0.95f), new(0f, 0.3f, 0.25f));
    private static readonly Tone Gilded = new(new(1f, 0.28f, 0.1f), new(1f, 0.9f, 0.6f), new(0.4f, 0.02f, 0f));
    private static readonly Tone Iron = new(new(0.72f, 0.76f, 0.82f), new(1f, 1f, 1f), new(0.2f, 0.22f, 0.28f));
    private static readonly Tone Magma = new(new(1f, 0.35f, 0.05f), new(1f, 0.85f, 0.4f), new(0.35f, 0.03f, 0f));
    private static readonly Tone Leaf = new(new(0.45f, 0.85f, 0.2f), new(0.85f, 1f, 0.6f), new(0.08f, 0.25f, 0.03f));
    private static readonly Tone Hive = new(new(0.4f, 0.55f, 0.12f), new(0.85f, 1f, 0.45f), new(0.05f, 0.1f, 0f));
    private static readonly Tone Shadow = new(new(0.5f, 0.18f, 0.8f), new(0.85f, 0.7f, 1f), new(0.08f, 0.01f, 0.15f));
    private static readonly Tone Hex = new(new(0.8f, 0.12f, 0.45f), new(1f, 0.6f, 0.85f), new(0.15f, 0f, 0.1f));
    private static readonly Tone[] Prism = [.. Enumerable.Range(0, 6).Select(i =>
        new Tone(Color.FromHsv(i / 6f, 0.8f, 1f), Color.FromHsv(i / 6f, 0.25f, 1f), Color.FromHsv(i / 6f, 1f, 0.35f)))];

    private static Color FireColor => FxTheme.Of(FxElement.Fire);
    private static Color FrostColor => FxTheme.Of(FxElement.Frost);

    private void More(FxSkill skill, Cast c, float r, float range)
    {
        switch (skill)
        {
            case FxSkill.ThousandCuts: ThousandCuts(c, r); break;
            case FxSkill.Execution: Execution(c, r); break;
            case FxSkill.BladeDance: BladeDance(c, r); break;
            case FxSkill.Lunge: Lunge(c, range); break;
            case FxSkill.GroundWave: GroundWave(c, range); break;
            case FxSkill.Riposte: Riposte(c, r); break;
            case FxSkill.LeapSlam: LeapSlam(c, r, range); break;
            case FxSkill.Grapple: Grapple(c, range); break;
            case FxSkill.EarthenWall: EarthenWall(c, r); break;
            case FxSkill.MagmaFissure: MagmaFissure(c, range); break;
            case FxSkill.Quake: Quake(c, r); break;
            case FxSkill.BoulderToss: BoulderToss(c, r, range); break;
            case FxSkill.StoneArmor: StoneArmor(c); break;
            case FxSkill.InfernoRing: InfernoRing(c, r); break;
            case FxSkill.DragonBreath: DragonBreath(c, range); break;
            case FxSkill.FireRain: FireRain(c, r); break;
            case FxSkill.BlazingDash: BlazingDash(c, range); break;
            case FxSkill.Combustion: Combustion(c, r); break;
            case FxSkill.EmberMark: EmberMark(c, r); break;
            case FxSkill.Blizzard: Blizzard(c, r); break;
            case FxSkill.IcePrison: IcePrison(c, r); break;
            case FxSkill.FrostLance: FrostLance(c, r, range); break;
            case FxSkill.GlacierPath: GlacierPath(c, range); break;
            case FxSkill.FrozenOrb: FrozenOrb(c, r, range); break;
            case FxSkill.ChainLightning: ChainLightning(c, range); break;
            case FxSkill.StaticField: StaticField(c, r); break;
            case FxSkill.BallLightning: BallLightning(c, range); break;
            case FxSkill.Thunderclap: Thunderclap(c, r); break;
            case FxSkill.Tempest: Tempest(c, r); break;
            case FxSkill.Thornbind: Thornbind(c); break;
            case FxSkill.VenomSpit: VenomSpit(c, r, range); break;
            case FxSkill.PlagueCloud: PlagueCloud(c, r); break;
            case FxSkill.Regrowth: Regrowth(c, r); break;
            case FxSkill.Swarm: Swarm(c, range); break;
            case FxSkill.ArcaneMissiles: ArcaneMissiles(c, range); break;
            case FxSkill.ManaShield: ManaShield(c); break;
            case FxSkill.RiftBeam: RiftBeam(c, range); break;
            case FxSkill.GravityWell: GravityWell(c, r); break;
            case FxSkill.Portal: Portal(c, range); break;
            case FxSkill.PrismaticSpray: PrismaticSpray(c, range); break;
            case FxSkill.DivineShield: DivineShield(c); break;
            case FxSkill.Consecration: Consecration(c, r); break;
            case FxSkill.HolyNova: HolyNova(c, r); break;
            case FxSkill.SeraphWings: SeraphWings(c); break;
            case FxSkill.SacredLances: SacredLances(c, r); break;
            case FxSkill.ShadowClaws: ShadowClaws(c, r); break;
            case FxSkill.SoulDrain: SoulDrain(c, range); break;
            case FxSkill.DeathCoil: DeathCoil(c, range); break;
            case FxSkill.ShadeClones: ShadeClones(c); break;
            case FxSkill.Curse: Curse(c); break;
            default: Study(skill, c, r, range); break;
        }
    }

    // ---- Blades.

    private void ThousandCuts(Cast c, float r)
    {
        const int Cuts = 16;
        const float Every = 0.04f;
        var spot = new Vector3(0f, 1f, r * 0.4f);
        for (var i = 0; i < Cuts; i++)
        {
            var when = i * Every;
            var basis = new Basis(Vector3.Up, Rand(0f, Mathf.Tau)) * new Basis(Vector3.Back, Rand(-1.1f, 1.1f));
            if (i % 2 == 1)
                basis *= Basis.FromScale(new Vector3(-1f, 1f, 1f));
            var size = Rand(1.2f, 1.8f) * r / 3f;
            var through = spot + FxKit.Jitter(0.35f);
            Crescent(c, when, 0.32f, new Transform3D(basis, through - basis * Vector3.Back * size), size, 120f, Steel, thick: 0.3f, sweep: 0.3f, gain: 1.4f);
            for (var j = 0; j < 3; j++)
                Spark(c, when + 0.03f, Rand(0.15f, 0.25f), Steel, through, FxKit.RandomDirection() * Rand(5f, 9f), Gravity * 0.5f, 0.05f, 0.025f);
        }
        var end = Cuts * Every;
        Flare(c, end, 0.25f, Star, Steel, spot, 3f);
        Disc(c, end, 0.35f, Shock, Steel, FxKit.Flat(spot), k => r * 0.8f * Out(k), k => 1f - k, band: 0.15f);
        Shine(c, spot, r * 1.3f, Steel.Main, age => age < end ? 0.5f : 1.4f * Mathf.Max(0f, 1f - (age - end) / 0.3f));
    }

    private void Execution(Cast c, float r)
    {
        const float Chop = 0.1f;
        // Upright along the facing, starting overhead and chopping down in front.
        var swing = new Transform3D(new Basis(Vector3.Back, Mathf.Pi / 2f), new Vector3(0f, 1.3f, 0.2f));
        Crescent(c, 0f, 0.42f, swing, r * 0.95f, 160f, Gilded, thick: 0.45f, sweep: 0.25f, gain: 1.4f);
        Crescent(c, 0.02f, 0.36f, swing, r * 0.75f, 140f, Gilded with { Main = new Color(1f, 0.6f, 0.2f) }, thick: 0.22f, sweep: 0.28f);
        var hit = new Vector3(0f, 0f, r * 0.75f);
        Cue(c, Chop, () => Stage.Explosion(At(c, hit + Vector3.Up * 0.2f), r * 0.45f, Gilded.Main, FxBurst.Dome));
        Disc(c, Chop, 1.4f, Cracks, Gilded, hit, k => r * 0.6f, k => Fade(k, 0.02f, 0.5f), k => k, stretch: new Vector3(0.45f, 1f, 1f));
        Disc(c, Chop, 0.4f, Shock, Gilded, hit, k => r * 0.8f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        Flare(c, Chop, 0.25f, Star, Gilded, hit + Vector3.Up * 0.4f, 3f);
        for (var i = 0; i < 18; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Chop, Rand(0.4f, 0.7f), Rubble, hit + way * 0.3f, way * Rand(2f, 5f) + Vector3.Up * Rand(4f, 8f), Gravity * 1.2f, 0.13f, 0.02f);
        }
        Shine(c, hit + Vector3.Up, r * 1.5f, Gilded.Main, age => 1.6f * Bump(age, Chop, 0.4f));
    }

    private void BladeDance(Cast c, float r)
    {
        const int Steps = 3;
        const float Every = 0.16f;
        for (var i = 0; i < Steps; i++)
        {
            var side = i % 2 == 0 ? 1f : -1f;
            var basis = new Basis(Vector3.Up, side * 0.4f) * new Basis(Vector3.Back, Mathf.DegToRad(-20f * side));
            if (side < 0f)
                basis *= Basis.FromScale(new Vector3(-1f, 1f, 1f));
            var hub = new Vector3(0f, 1f, r * (0.1f + 0.3f * i));
            Crescent(c, i * Every, 0.36f, new Transform3D(basis, hub), r * 0.7f, 150f, Jade, thick: 0.4f, sweep: 0.3f, gain: 1.3f);
            var tip = hub + basis * Vector3.Back * r * 0.7f;
            for (var j = 0; j < 8; j++)
                Spark(c, i * Every + 0.05f, Rand(0.2f, 0.35f), Jade, tip, FxKit.RandomDirection() * Rand(4f, 8f), Gravity * 0.5f, 0.06f, 0.03f);
        }
        var end = Steps * Every;
        Disc(c, end - 0.05f, 0.4f, Shock, Jade, new Vector3(0f, 0f, r * 0.7f), k => r * 0.8f * Out(k), k => 1f - k, band: 0.15f);
        Shine(c, new Vector3(0f, 1f, r * 0.5f), r * 1.5f, Jade.Main, age => 0.9f * Fade(age / (end + 0.3f), 0.05f, 0.3f));
    }

    private void Lunge(Cast c, float range)
    {
        const float Thrust = 0.07f;
        var from = new Vector3(0f, 1.05f, 0.3f);
        var length = Mathf.Max(range - 0.3f, 1f);
        var tip = from + Vector3.Back * length;
        Func<float, float> reach = k => Out(Mathf.Min(k * 0.26f / Thrust, 1f));
        Line(c, 0f, 0.26f, Sun, k => from + Vector3.Back * length * reach(k) * 0.5f, k => Vector3.Back * length * reach(k), 0.22f, k => 1f - k * k);
        Line(c, 0f, 0.2f, Sun with { Main = Sun.Hot }, k => from + Vector3.Back * length * reach(k) * 0.5f, k => Vector3.Back * length * reach(k), 0.08f, k => 1f - k);
        for (var i = 0; i < 8; i++)
        {
            var off = new Vector3(Rand(-0.5f, 0.5f), Rand(-0.4f, 0.5f), 0f);
            var len = Rand(1f, 2.2f);
            var start = Rand(0f, 0.05f);
            var z = Rand(0.3f, 0.9f) * length;
            Line(c, start, 0.22f, Sun, k => from + off + Vector3.Back * (z + k * 0.8f), k => Vector3.Back * len * (1f - k), 0.04f, k => 1f - k);
        }
        Flare(c, Thrust, 0.22f, Star, Sun, tip, 2.4f);
        Disc(c, Thrust, 0.3f, Shock, Sun, tip, k => 1.4f * Out(k), k => 1f - k, basis: new Basis(Vector3.Right, Mathf.Pi / 2f), band: 0.18f, gain: 1.3f);
        for (var i = 0; i < 12; i++)
        {
            var way = (Vector3.Back + new Vector3(Rand(-0.6f, 0.6f), Rand(-0.4f, 0.6f), 0f)).Normalized();
            Spark(c, Thrust, Rand(0.2f, 0.35f), Sun, tip, way * Rand(6f, 11f), Gravity * 0.5f, 0.06f, 0.03f);
        }
        Shine(c, tip, 3f, Sun.Main, age => 1.2f * Bump(age, Thrust, 0.3f));
    }

    private void GroundWave(Cast c, float range)
    {
        const float Life = 0.6f;
        var length = Mathf.Max(range, 2f);
        // Lying just off the ground and bowed the way it goes, skimming along: it reads from above.
        Crescent(c, 0f, Life, k => new Transform3D(Basis.Identity, new Vector3(0f, 0.35f, -1.2f + (length - 0.5f) * k)), 1.9f, 120f, Azure, thick: 0.45f, sweep: 0.12f, gain: 1.5f);
        Crescent(c, 0.03f, Life - 0.03f, k => new Transform3D(Basis.Identity, new Vector3(0f, 0.5f, -1.2f + (length - 0.5f) * k)), 1.6f, 100f, Azure with { Main = Azure.Hot }, thick: 0.2f, sweep: 0.12f);
        const int Marks = 6;
        for (var i = 0; i < Marks; i++)
        {
            var s = (i + 0.5f) / Marks;
            var at = new Vector3(0f, 0f, 0.5f + (length - 0.5f) * s);
            Disc(c, s * Life, 0.9f, Cracks, Azure, at, k => 0.75f, k => Fade(k, 0.05f, 0.6f), k => k, stretch: new Vector3(0.6f, 1f, 1f));
            for (var j = 0; j < 3; j++)
                Spark(c, s * Life, Rand(0.3f, 0.5f), Dust, at, new Vector3(Rand(-2f, 2f), Rand(2f, 5f), Rand(-1f, 1f)), Gravity, 0.1f, 0.03f);
        }
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Azure.Main, age => 0.8f * Mathf.Max(0f, 1f - age / Life));
    }

    private void Riposte(Cast c, float r)
    {
        var guard = new Vector3(0f, 1.2f, 0.9f);
        var upright = new Basis(Vector3.Back, Mathf.Pi) * new Basis(Vector3.Right, Mathf.Pi / 2f);
        Crescent(c, 0f, 0.38f, new Transform3D(upright, new Vector3(0f, 0.1f, 0.7f)), 1.5f, 130f, Gold, thick: 0.25f, sweep: 0.15f, gain: 1.4f);
        Flare(c, 0f, 0.3f, Star, Gold, guard, 3.2f);
        Flare(c, 0f, 0.12f, Mote, Gold, guard, 1.4f);
        Disc(c, 0f, 0.3f, Shock, Gold, guard, k => 1.6f * Out(k), k => 1f - k, basis: new Basis(Vector3.Right, Mathf.Pi / 2f), band: 0.16f, gain: 1.2f);
        for (var i = 0; i < 22; i++)
        {
            var way = (Vector3.Back * 0.6f + FxKit.RandomDirection()).Normalized();
            Spark(c, 0f, Rand(0.3f, 0.55f), Gold, guard, way * Rand(5f, 10f), Gravity, 0.06f, 0.03f);
        }
        Shine(c, guard, r * 1.2f, Gold.Main, age => 1.4f * Mathf.Max(0f, 1f - age / 0.3f));
    }

    private void LeapSlam(Cast c, float r, float range)
    {
        const float Land = 0.35f;
        var length = Mathf.Max(range, 1f);
        Func<float, Vector3> arc = s => new Vector3(0f, 0.9f + 10f * s * (1f - s), length * s);
        for (var i = 0; i < 12; i++)
        {
            var s = (i + 0.5f) / 12f;
            var at = arc(s);
            Flare(c, s * Land, 0.4f, Mote, Dust, k => at + Vector3.Down * k * 0.3f, k => 0.5f + 0.4f * k, k => (1f - k) * 0.6f, gain: 0.8f);
        }
        Line(c, 0f, Land, Dust, k => arc(k), k => (arc(Mathf.Min(k + 0.05f, 1f)) - arc(Mathf.Max(k - 0.1f, 0f))) * 1.2f, 0.18f, k => 0.8f);
        var land = new Vector3(0f, 0f, length);
        Disc(c, Land, 0.55f, Shock, Dust, land, k => r * Out(k), k => 1f - k * k, band: 0.16f, gain: 1.4f);
        Disc(c, Land, 1.5f, Cracks, Sun, land, k => r * 0.6f, k => Fade(k, 0.03f, 0.4f), k => k);
        Flare(c, Land, 0.2f, Star, Dust, land + Vector3.Up * 0.4f, 2.8f);
        for (var i = 0; i < 6; i++)
        {
            var way = Around(i * Mathf.Tau / 6f + Rand(-0.2f, 0.2f));
            Cue(c, Land, () => Stage.Spike(At(c, land + way * r * 0.55f), c.Root.Basis * way, Rand(0.7f, 1.1f), FxSpike.Stone, 0.02f));
        }
        for (var i = 0; i < 18; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Land, Rand(0.5f, 0.8f), Rubble, land + way * 0.4f, way * Rand(3f, 6f) + Vector3.Up * Rand(4f, 8f), Gravity * 1.2f, 0.13f, 0.02f);
        }
        Shine(c, land + Vector3.Up * 0.5f, r * 1.5f, Dust.Main, age => 1.3f * Bump(age, Land, 0.4f));
    }

    private void Grapple(Cast c, float range)
    {
        const float Out1 = 0.22f, Hold = 0.06f, Back1 = 0.22f, Life = Out1 + Hold + Back1;
        var hand = new Vector3(0f, 1.1f, 0.3f);
        var length = Mathf.Max(range - 0.3f, 1f);
        Func<float, float> reach = k =>
        {
            var t = k * Life;
            return t < Out1 ? Out(t / Out1) : t < Out1 + Hold ? 1f : 1f - Mathf.SmoothStep(0f, 1f, (t - Out1 - Hold) / Back1);
        };
        Func<float, Vector3> tip = k => hand + Vector3.Back * length * reach(k);
        Tube(c, 0f, Life, Spiral, Iron, k => (hand, tip(k) + Vector3.Back * 0.01f), k => 0.09f, k => 1f, twist: 14f, speed: 0.4f, gain: 1.4f);
        Flare(c, 0f, Life, Star, Iron, tip, k => 0.7f, k => 1f);
        Flare(c, Out1, 0.25f, Star, Sun, hand + Vector3.Back * length, 2.4f);
        for (var i = 0; i < 10; i++)
            Spark(c, Out1, Rand(0.2f, 0.35f), Sun, hand + Vector3.Back * length, FxKit.RandomDirection() * Rand(4f, 8f), Gravity * 0.5f, 0.05f, 0.03f);
        Shine(c, hand + Vector3.Back * length, 3f, Sun.Main, age => Bump(age, Out1, 0.3f));
    }

    // ---- Earth.

    private void EarthenWall(Cast c, float r)
    {
        var line = new Vector3(0f, 0f, r * 0.7f);
        for (var i = -4; i <= 4; i++)
        {
            var at = line + Vector3.Right * i * r / 4.5f + Vector3.Back * Rand(-0.15f, 0.15f);
            var delay = Mathf.Abs(i) * 0.03f;
            Cue(c, 0f, () => Stage.Spike(At(c, at), c.Root.Basis * Vector3.Back, Rand(1.3f, 1.9f), FxSpike.Stone, delay));
        }
        Disc(c, 0f, 1.4f, Cracks, Dust, line, k => 1f, k => Fade(k, 0.03f, 0.4f), k => k, stretch: new Vector3(r * 1.1f, 1f, 0.6f));
        Disc(c, 0f, 0.8f, Glow, Dust, line, k => 1f, k => Fade(k, 0.05f, 0.6f) * 0.6f, stretch: new Vector3(r * 1.2f, 1f, 0.9f));
        for (var i = 0; i < 20; i++)
        {
            var at = line + Vector3.Right * Rand(-r, r);
            Spark(c, Rand(0f, 0.15f), Rand(0.4f, 0.7f), Rubble, at, new Vector3(Rand(-1f, 1f), Rand(4f, 7f), Rand(-2f, 2f)), Gravity * 1.2f, 0.12f, 0.02f);
        }
        for (var i = 0; i < 8; i++)
        {
            var at = line + Vector3.Right * Rand(-r, r);
            var size = Rand(0.8f, 1.3f);
            Flare(c, Rand(0f, 0.15f), 0.8f, Mote, Dust, k => at + Vector3.Up * (0.3f + k * 0.6f), k => size * (0.6f + k), k => (1f - k) * 0.4f, gain: 0.7f);
        }
    }

    private void MagmaFissure(Cast c, float range)
    {
        var length = Mathf.Max(range, 2f);
        const int Segments = 7;
        for (var i = 0; i < Segments; i++)
        {
            var z = 0.8f + (length - 0.8f) * i / (Segments - 1);
            var at = new Vector3(Rand(-0.15f, 0.15f), 0f, z);
            var start = i * 0.06f;
            Disc(c, start, 2f, Cracks, Magma, at, k => 1f, k => Fade(k, 0.03f, 0.4f), k => k * 0.8f, stretch: new Vector3(0.6f, 1f, 1f), gain: 1.4f);
            var tall = Rand(1.2f, 2f);
            Column(c, start, 0.75f, Fire, Magma, at, k => new Vector2(0.32f, tall * Out(Mathf.Min(k * 5f, 1f))), k => Fade(k, 0.03f, 0.5f), gain: 1.4f, speed: 1.6f);
            if (i % 2 == 0)
                Cue(c, start, () => Stage.Patch(At(c, at), 0.9f, 2f, FireColor, FxGround.Fire));
            for (var j = 0; j < 4; j++)
                Spark(c, start + Rand(0f, 0.3f), Rand(0.5f, 0.8f), Magma, at + Vector3.Up * 0.2f, new Vector3(Rand(-1.5f, 1.5f), Rand(4f, 8f), Rand(-1f, 1f)), Gravity * 0.6f, 0.06f, 0.035f);
        }
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Magma.Main, age => 1.2f * Fade(age / 2.2f, 0.05f, 0.5f));
    }

    private void Quake(Cast c, float r)
    {
        for (var p = 0; p < 3; p++)
        {
            var when = p * 0.4f;
            var reach = r * (0.5f + 0.25f * p);
            var cracks = r * (0.35f + 0.25f * p);
            Disc(c, when, 0.5f, Shock, Dust, Vector3.Zero, k => reach * Out(k), k => 1f - k, band: 0.18f, gain: 1.3f);
            Disc(c, when, 1.6f - p * 0.4f, Cracks, Sun, Vector3.Zero, k => cracks, k => Fade(k, 0.03f, 0.4f), k => k);
            for (var i = 0; i < 9; i++)
            {
                var way = Around(Rand(0f, Mathf.Tau));
                Spark(c, when, Rand(0.4f, 0.65f), Rubble, way * Rand(0.3f, reach * 0.8f), way * Rand(1f, 3f) + Vector3.Up * Rand(3f, 6f), Gravity * 1.2f, 0.12f, 0.02f);
            }
            Flare(c, when, 0.2f, Star, Dust, Vector3.Up * 0.3f, 2f);
        }
        Shine(c, Vector3.Up * 0.5f, r * 1.5f, Dust.Main, age => Bump(age, 0f, 0.3f) + Bump(age, 0.4f, 0.3f) + Bump(age, 0.8f, 0.3f));
    }

    private void BoulderToss(Cast c, float r, float range)
    {
        const float Flight = 0.5f;
        var length = Mathf.Max(range, 1.5f);
        var size = r / 3f;
        Func<float, Vector3> arc = s => new Vector3(0f, Mathf.Lerp(1.6f, 0.5f * size, s) + 10f * s * (1f - s), 0.5f + (length - 0.5f) * s);
        var spin = FxKit.RandomDirection();
        Prop(c, 0f, Flight, _rock, _stone, k => new Transform3D(new Basis(spin, k * 7f) * Basis.FromScale(new Vector3(0.75f, 0.6f, 0.7f) * size), arc(k)));
        for (var i = 0; i < 8; i++)
        {
            var s = (i + 0.5f) / 8f;
            var at = arc(s);
            Flare(c, s * Flight, 0.45f, Mote, Dust, k => at, k => 0.4f + 0.5f * k, k => (1f - k) * 0.45f, gain: 0.7f);
        }
        var land = new Vector3(0f, 0f, length);
        Cue(c, Flight, () => Stage.Explosion(At(c, land + Vector3.Up * 0.3f), r * 0.55f, Dust.Main, FxBurst.Dome));
        Disc(c, Flight, 0.5f, Shock, Dust, land, k => r * 0.9f * Out(k), k => 1f - k, band: 0.18f, gain: 1.3f);
        Disc(c, Flight, 1.4f, Cracks, Rubble, land, k => r * 0.5f, k => Fade(k, 0.03f, 0.4f), k => k);
        for (var i = 0; i < 22; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Flight, Rand(0.5f, 0.8f), Rubble, land + way * 0.3f + Vector3.Up * 0.3f, way * Rand(3f, 7f) + Vector3.Up * Rand(4f, 8f), Gravity * 1.2f, 0.16f, 0.015f);
        }
        Shine(c, land + Vector3.Up, r * 1.4f, Dust.Main, age => 1.3f * Bump(age, Flight, 0.4f));
    }

    private void StoneArmor(Cast c)
    {
        const float Life = 2.6f, Fling = 2.3f;
        const int Rocks = 6;
        for (var i = 0; i < Rocks; i++)
        {
            var angle = i * Mathf.Tau / Rocks;
            var high = 0.5f + 0.45f * (i % 3);
            var spin = FxKit.RandomDirection();
            var size = Rand(0.3f, 0.42f);
            Prop(c, 0f, Life, _rock, _stone, k =>
            {
                var t = k * Life;
                var rise = Out(Mathf.Min(t / 0.35f, 1f));
                var flung = t > Fling ? (t - Fling) / (Life - Fling) : 0f;
                var at = Around(angle + t * 2.2f) * (1.05f + flung * 3f) + Vector3.Up * high * rise;
                return new Transform3D(new Basis(spin, t * 3f) * Basis.FromScale(Vector3.One * size * Mathf.Max(rise, 0.05f) * (1f - flung * 0.8f)), at);
            });
        }
        Disc(c, 0f, Life, Swirl, Dust, Vector3.Zero, k => 1.7f, k => Fade(k, 0.08f, 0.15f) * 0.7f, spin: 4f);
        Disc(c, 0f, 0.45f, Shock, Dust, Vector3.Zero, k => 2.2f * Out(k), k => 1f - k, band: 0.16f);
        for (var i = 0; i < 16; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Rand(0f, Life - 0.5f), 0.4f, Dust, way * 1.1f + Vector3.Up * 0.1f, new Vector3(way.Z, 0f, -way.X) * 3f + Vector3.Up * 1.5f, Gravity * 0.3f, 0.1f, 0.05f, 0.8f);
        }
    }

    // ---- Fire.

    private void InfernoRing(Cast c, float r)
    {
        const float Life = 2.2f;
        const int Flames = 12;
        for (var i = 0; i < Flames; i++)
        {
            var at = Around(i * Mathf.Tau / Flames) * r;
            var tall = Rand(1.8f, 2.6f);
            Column(c, i * 0.02f, Life - i * 0.02f, Fire, Flame, at, k => new Vector2(0.45f, tall * Out(Mathf.Min(k * 6f, 1f))), k => Fade(k, 0.02f, 0.2f), gain: 1.4f, speed: 1.8f);
        }
        Cue(c, 0f, () => Stage.Patch(At(c, Vector3.Zero), r * 1.05f, Life, FireColor, FxGround.Fire));
        Disc(c, 0f, 0.45f, Shock, Flame, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.14f, gain: 1.4f);
        for (var i = 0; i < 30; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * r + Vector3.Up * Rand(0.3f, 1.5f);
            Spark(c, Rand(0f, Life - 0.6f), Rand(0.5f, 0.8f), Flame, at, Vector3.Up * Rand(3f, 6f) + FxKit.Jitter(1f), -1f, 0.06f, 0.05f);
        }
        Shine(c, Vector3.Up, r * 1.6f, Flame.Main, age => 1.2f * Fade(age / Life, 0.05f, 0.15f));
    }

    private void DragonBreath(Cast c, float range)
    {
        var length = Mathf.Max(range, 2f);
        var mouth = new Vector3(0f, 1.3f, 0.4f);
        const int Puffs = 70;
        for (var i = 0; i < Puffs; i++)
        {
            var way = new Vector3(Mathf.Sin(Rand(-0.42f, 0.42f)), Rand(-0.12f, 0.05f), 1f).Normalized();
            var tone = i % 3 == 0 ? Magma : Flame;
            Flare(c, i * 0.011f, 0.5f, Mote, tone, k => mouth + way * length * Out(k) + Vector3.Up * k * 0.4f, k => 0.6f + 2.4f * k,
                k => Fade(k, 0.1f, 0.6f) * 0.75f, gain: 1.1f);
        }
        for (var i = 0; i < 20; i++)
        {
            var way = new Vector3(Mathf.Sin(Rand(-0.5f, 0.5f)), Rand(-0.1f, 0.2f), 1f).Normalized();
            Spark(c, Rand(0f, 0.7f), Rand(0.35f, 0.5f), Flame, mouth, way * length * Rand(1.8f, 2.6f), Gravity * 0.3f, 0.06f, 0.03f);
        }
        Cue(c, 0.3f, () => Stage.Patch(At(c, new Vector3(0f, 0f, length * 0.6f)), length * 0.35f, 2f, FireColor, FxGround.Fire));
        Disc(c, 0f, 1.1f, Glow, Flame, new Vector3(0f, 0f, length * 0.55f), k => 1f, k => Fade(k, 0.1f, 0.4f) * 0.7f, stretch: new Vector3(length * 0.3f, 1f, length * 0.5f));
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Flame.Main, age => 1.4f * Fade(age / 1.2f, 0.05f, 0.4f));
    }

    private void FireRain(Cast c, float r)
    {
        const int Balls = 10;
        const float Fall = 0.4f, Every = 0.12f;
        Disc(c, 0f, Balls * Every + Fall + 0.2f, Glow, Flame, Vector3.Zero, k => r, k => Fade(k, 0.1f, 0.2f) * 0.45f);
        var slant = new Vector3(1.5f, 9f, -2.5f);
        for (var i = 0; i < Balls; i++)
        {
            var spot = Around(Rand(0f, Mathf.Tau)) * r * 0.9f * Mathf.Sqrt(Rand(0.05f, 1f));
            var start = i * Every;
            Fly(c, start, Fall, FxShotLook.Fireball, k => (spot + slant).Lerp(spot + Vector3.Up * 0.3f, k));
            Cue(c, start + Fall, () => Stage.Explosion(At(c, spot), 1.2f, FireColor, FxBurst.Fire));
        }
    }

    private void BlazingDash(Cast c, float range)
    {
        const float Arrive = 0.2f;
        var length = Mathf.Max(range, 1f);
        for (var i = 0; i < 5; i++)
        {
            var z = length * (i + 0.5f) / 5f;
            var at = new Vector3(0f, 0f, z);
            var when = z / length * Arrive;
            Cue(c, when, () => Stage.Patch(At(c, at), 0.8f, 1.6f, FireColor, FxGround.Fire));
            Column(c, when, 0.6f, Fire, Flame, at, k => new Vector2(0.4f, 1.6f * (1f - k * 0.5f)), k => (1f - k), gain: 1.3f, speed: 2f);
        }
        for (var i = 0; i < 12; i++)
        {
            var at = new Vector3(Rand(-0.5f, 0.5f), Rand(0.3f, 1.6f), Rand(0.5f, length));
            var len = Rand(1.4f, 3f);
            Line(c, at.Z / length * Arrive, 0.3f, Flame, k => at + Vector3.Back * k * 0.5f, k => Vector3.Back * len * (1f - k * 0.5f), 0.06f, k => 1f - k);
        }
        Cue(c, Arrive, () => Stage.Explosion(At(c, new Vector3(0f, 0.2f, length)), 1.6f, FireColor, FxBurst.Fire));
    }

    private void Combustion(Cast c, float r)
    {
        const float Blow = 0.55f;
        var heart = Vector3.Up * 1.1f;
        Disc(c, 0f, Blow + 0.1f, Runes, Flame, Vector3.Zero, k => r * 0.6f, k => 0.9f, k => k * 1.8f, gain: 1.2f);
        Globe(c, "energy", Orb, 0f, Blow, Flame, k => heart, k => (0.15f + 0.45f * k) * (1f + 0.08f * Mathf.Sin(k * k * 60f)), k => 0.7f + 0.5f * k);
        for (var i = 0; i < 14; i++)
        {
            var from = heart + FxKit.RandomDirection() * r * 0.7f;
            var start = Rand(0f, 0.25f);
            Flare(c, start, Blow - start, Mote, Flame, k => from.Lerp(heart, k * k), k => 0.25f, k => Fade(k, 0.2f, 0f));
        }
        Cue(c, Blow, () => Stage.Explosion(At(c, heart), r, FireColor, FxBurst.Fire));
        for (var i = 0; i < 6; i++)
        {
            var at = Around(i * Mathf.Tau / 6f) * r * 0.55f;
            Column(c, Blow, 0.6f, Fire, Flame, at, k => new Vector2(0.4f, 2.6f * Out(Mathf.Min(k * 5f, 1f))), k => 1f - k, k => k * 0.8f, gain: 1.4f, speed: 2f);
        }
        Shine(c, heart, r * 1.6f, Flame.Main, age => age < Blow ? age / Blow * 0.6f : 1.8f * Mathf.Max(0f, 1f - (age - Blow) / 0.5f));
    }

    private void EmberMark(Cast c, float r)
    {
        const float Blow = 1.5f;
        var mark = Vector3.Up * 2.4f;
        Flare(c, 0f, Blow, Star, Flame, k => mark, k => 1.2f + 0.35f * Mathf.Sin(k * Blow * 14f), k => Fade(k, 0.05f, 0f));
        Globe(c, "energy", Orb, 0f, Blow, Flame, k => mark, k => 0.22f + 0.04f * Mathf.Sin(k * Blow * 14f), k => 1f);
        Disc(c, 0f, Blow, Runes, Flame, Vector3.Zero, k => 1.2f, k => Fade(k, 0.03f, 0.03f), k => k * Blow / 0.4f);
        foreach (var when in new[] { 0.3f, 0.7f, 1.1f })
        {
            Disc(c, when, 0.35f, Shock, Flame, Vector3.Zero, k => 1.5f * Out(k), k => 1f - k, band: 0.15f, gain: 1.2f);
            Flare(c, when, 0.2f, Star, Flame, mark, 2f);
        }
        for (var i = 0; i < 18; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * Rand(0.2f, 0.8f) + Vector3.Up * Rand(0.2f, 1.5f);
            Spark(c, Rand(0f, Blow - 0.5f), 0.6f, Flame, at, Vector3.Up * Rand(2f, 4f), -1f, 0.05f, 0.06f);
        }
        Cue(c, Blow, () => Stage.Explosion(At(c, Vector3.Up), r, FireColor, FxBurst.Fire));
        Flare(c, Blow, 0.3f, Star, Flame, mark, 3.5f);
    }

    // ---- Frost.

    private void Blizzard(Cast c, float r)
    {
        const float Life = 3f;
        Disc(c, 0f, Life, Swirl, Ice, Vector3.Zero, k => r, k => Fade(k, 0.08f, 0.15f) * 0.6f, spin: -2f);
        Cue(c, 0f, () => Stage.Patch(At(c, Vector3.Zero), r, Life, FrostColor, FxGround.Frost));
        var drift = new Vector3(0.8f, 6f, 0.5f);
        for (var i = 0; i < 60; i++)
        {
            var spot = Around(Rand(0f, Mathf.Tau)) * r * Mathf.Sqrt(Rand(0f, 1f));
            var start = Rand(0f, Life - 0.6f);
            Line(c, start, 0.35f, Ice, k => (spot + drift).Lerp(spot, k), k => -drift.Normalized() * 0.9f, 0.06f, k => 1f);
            Flare(c, start + 0.35f, 0.2f, Mote, Ice, spot + Vector3.Up * 0.1f, 0.5f);
        }
        for (var i = 0; i < 40; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * r * Mathf.Sqrt(Rand(0f, 1f)) + Vector3.Up * Rand(0.5f, 3f);
            var fall = new Vector3(Rand(0.3f, 0.8f), -Rand(0.5f, 1f), Rand(-0.3f, 0.3f));
            Flare(c, Rand(0f, Life - 1f), 1f, Mote, Ice, k => at + fall * k, k => 0.16f, k => Fade(k, 0.2f, 0.3f));
        }
        Shine(c, Vector3.Up, r * 1.4f, Ice.Main, age => 0.6f * Fade(age / Life, 0.05f, 0.15f));
    }

    private void IcePrison(Cast c, float r)
    {
        var cage = Mathf.Clamp(r * 0.45f, 0.9f, 1.6f);
        const int Bars = 9;
        for (var i = 0; i < Bars; i++)
        {
            var way = Around(i * Mathf.Tau / Bars);
            var delay = i * 0.015f;
            Cue(c, 0f, () => Stage.Spike(At(c, way * cage), c.Root.Basis * -way, 2.2f, FxSpike.Ice, delay));
        }
        Globe(c, "energy", Hexes, 0.05f, 2.1f, Ice, k => Vector3.Zero, k => cage * 0.95f * Overshoot(Mathf.Min(k * 2.1f / 0.2f, 1f)), k => Fade(k, 0.02f, 0.15f) * 0.8f);
        Cue(c, 0f, () => Stage.Patch(At(c, Vector3.Zero), cage * 1.6f, 2.2f, FrostColor, FxGround.Frost));
        Disc(c, 0f, 0.35f, Shock, Ice, Vector3.Zero, k => cage * 1.8f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        Flare(c, 0f, 0.25f, Star, Ice, Vector3.Up * 1.1f, 2.6f);
    }

    private void FrostLance(Cast c, float r, float range)
    {
        const float Flight = 0.32f;
        var length = Mathf.Max(range, 2f);
        var from = new Vector3(0f, 1.1f, 0.5f);
        var to = new Vector3(0f, 1f, length);
        Fly(c, 0f, Flight, FxShotLook.IceSpear, k => from.Lerp(to, k));
        for (var i = 0; i < 12; i++)
        {
            var s = (i + 0.5f) / 12f;
            var at = from.Lerp(to, s) + FxKit.Jitter(0.15f);
            Flare(c, s * Flight, 0.6f, Mote, Ice, k => at + Vector3.Down * k * 0.4f, k => 0.18f, k => 1f - k);
        }
        Cue(c, Flight, () => Stage.Explosion(At(c, to), r * 0.5f, FrostColor, FxBurst.Frost));
    }

    private void GlacierPath(Cast c, float range)
    {
        var length = Mathf.Max(range, 2f);
        const int Steps = 6;
        for (var i = 0; i < Steps; i++)
        {
            var z = 0.8f + (length - 0.8f) * i / (Steps - 1);
            var at = new Vector3(0f, 0f, z);
            var when = i * 0.07f;
            Cue(c, when, () => Stage.Patch(At(c, at), 1.1f, 2.5f, FrostColor, FxGround.Frost));
            foreach (var side in new[] { -1f, 1f })
            {
                var spot = at + Vector3.Right * side * Rand(0.6f, 0.9f);
                Cue(c, when, () => Stage.Spike(At(c, spot), c.Root.Basis * (Vector3.Right * side), Rand(0.6f, 1f), FxSpike.Ice, 0f));
            }
            Disc(c, when, 0.3f, Shock, Ice, at, k => 1.2f * Out(k), k => 1f - k, band: 0.2f);
            for (var j = 0; j < 3; j++)
            {
                var mist = at + new Vector3(Rand(-0.6f, 0.6f), 0.3f, Rand(-0.4f, 0.4f));
                Flare(c, when, 1f, Mote, Ice, k => mist + Vector3.Up * k * 0.5f, k => 0.6f + k * 0.6f, k => (1f - k) * 0.35f, gain: 0.7f);
            }
        }
    }

    private void FrozenOrb(Cast c, float r, float range)
    {
        const float Life = 1.6f;
        var length = Mathf.Max(range, 2f);
        Func<float, Vector3> at = k => new Vector3(0f, 1.2f, 0.5f + (length - 0.5f) * k);
        Globe(c, "energy", Orb, 0f, Life, Ice, at, k => 0.45f * Out(Mathf.Min(k * 8f, 1f)), k => 1f);
        Globe(c, "energy", Shell, 0f, Life, Ice, at, k => 0.7f, k => 0.6f);
        for (var i = 0; i < 30; i++)
        {
            var when = i * 0.05f;
            var way = Around(i * 0.9f) + Vector3.Down * 0.1f;
            Spark(c, when, 0.45f, Ice, at(when / Life), way * 9f, 2f, 0.1f, 0.05f, 1.6f);
        }
        var end = at(1f);
        for (var i = 0; i < 16; i++)
            Spark(c, Life, Rand(0.3f, 0.5f), Ice, end, FxKit.RandomDirection() * Rand(6f, 10f), Gravity * 0.5f, 0.07f, 0.03f);
        Cue(c, Life, () => Stage.Explosion(At(c, end), r * 0.4f, FrostColor, FxBurst.Frost));
        Flare(c, Life, 0.25f, Star, Ice, end, 2.6f);
    }

    // ---- Storm.

    private void ChainLightning(Cast c, float range)
    {
        var length = Mathf.Max(range, 3f);
        Vector3[] hops = [new(0f, 0f, 0.5f), new(1.2f, 0f, length * 0.35f), new(-1f, 0f, length * 0.6f), new(0.8f, 0f, length * 0.85f), new(-0.3f, 0f, length)];
        for (var i = 0; i < hops.Length - 1; i++)
        {
            var (a, b) = (hops[i], hops[i + 1]);
            var when = i * FxTiming.ArcLeap;
            Cue(c, when, () =>
            {
                Stage.Lightning([(At(c, a), At(c, b))], FxTiming.ArcLife + 0.1f, 8, 0.75f);
                Stage.Explosion(At(c, b + Vector3.Up * 0.6f), 0.8f, FxTheme.Of(FxElement.Lightning), FxBurst.Lightning);
            });
            Flare(c, when, 0.2f, Star, Storm, b + Vector3.Up * Fx.LineLift, 1.6f);
        }
    }

    private void StaticField(Cast c, float r)
    {
        const float Life = 2.4f;
        Globe(c, "energy", Hexes, 0f, Life, Storm, k => Vector3.Zero, k => r * Overshoot(Mathf.Min(k * Life / 0.2f, 1f)), k => Fade(k, 0.02f, 0.1f) * 0.75f);
        Disc(c, 0f, Life, Glow, Storm, Vector3.Zero, k => r, k => Fade(k, 0.05f, 0.1f) * 0.4f);
        for (var t = 0.1f; t < Life - 0.15f; t += 0.1f)
        {
            var a = FxKit.RandomDirection() with { Y = Rand(0.15f, 1f) };
            a = a.Normalized() * r;
            var b = (a + FxKit.RandomDirection() * r * 0.6f) with { Y = Mathf.Abs(a.Y + Rand(-0.5f, 0.5f)) + 0.1f };
            b = b.Normalized() * r;
            Cue(c, t, () => Stage.Lightning([(At(c, a) - Vector3.Up * Fx.LineLift, At(c, b) - Vector3.Up * Fx.LineLift)], 0.12f, 3, 0.4f));
            Flare(c, t, 0.15f, Star, Storm, a, 1f);
        }
        Shine(c, Vector3.Up, r * 1.3f, Storm.Main, age => 0.7f * Fade(age / Life, 0.05f, 0.1f) * (0.7f + 0.3f * Mathf.Sin(age * 50f)));
    }

    private void BallLightning(Cast c, float range)
    {
        const float Life = 1.3f;
        var length = Mathf.Max(range, 2f);
        Func<float, Vector3> at = k => new Vector3(0f, 1.3f, 0.5f + (length - 0.5f) * k);
        Fly(c, 0f, Life, FxShotLook.Spark, at, 1.6f);
        for (var t = 0.12f; t < Life; t += 0.15f)
        {
            var ball = at(t / Life);
            var spot = FxKit.Flat(ball) + Around(Rand(0f, Mathf.Tau)) * Rand(0.6f, 1.5f);
            Cue(c, t, () => Stage.Lightning([(At(c, ball) - Vector3.Up * Fx.LineLift, At(c, spot))], 0.14f));
            Disc(c, t, 0.3f, Shock, Storm, spot, k => 0.9f * Out(k), k => 1f - k, band: 0.2f, gain: 1.2f);
            Disc(c, t, 0.8f, Cracks, Storm, spot, k => 0.6f, k => Fade(k, 0.02f, 0.5f), k => k);
        }
    }

    private void Thunderclap(Cast c, float r)
    {
        const float Strike = 0.3f;
        Disc(c, 0f, Strike, Glow, Storm, Vector3.Zero, k => r * 0.8f, k => k * 0.8f);
        for (var i = 0; i < 14; i++)
        {
            var from = Around(Rand(0f, Mathf.Tau)) * Rand(0.5f, r) + Vector3.Up * Rand(4f, 7f);
            var start = Rand(0f, 0.1f);
            Line(c, start, Strike - start, Storm, k => from.Lerp(Vector3.Up * 0.5f, k * k), k => (Vector3.Up * 0.5f - from).Normalized() * 1f, 0.06f, k => Fade(k, 0.2f, 0f));
        }
        Cue(c, Strike, () =>
        {
            Stage.Lightning([(At(c, Vector3.Up * 14f), At(c, Vector3.Zero))], 0.3f, 16, 1.6f);
            Stage.Nova(At(c, Vector3.Zero), r);
        });
        Disc(c, Strike, 1.3f, Cracks, Storm, Vector3.Zero, k => r * 0.6f, k => Fade(k, 0.02f, 0.5f), k => k);
        Flare(c, Strike, 0.3f, Star, Storm, Vector3.Up * 0.8f, 4f);
        Shine(c, Vector3.Up, r * 2f, Storm.Main, age => age < Strike ? age / Strike * 0.5f : 2.2f * Mathf.Max(0f, 1f - (age - Strike) / 0.4f));
    }

    private void Tempest(Cast c, float r)
    {
        const float Life = 2.6f;
        Fly(c, 0f, Life, FxShotLook.Tornado, k => new Vector3(0f, Fx.ShotHeight, k * 0.6f), r / 2.5f,
            k => Mathf.SmoothStep(0f, 1f, Mathf.Clamp(Mathf.Min(k * Life / 0.3f, (1f - k) * Life / 0.4f), 0f, 1f)));
        Disc(c, 0f, Life, Swirl, Dust, Vector3.Zero, k => r, k => Fade(k, 0.1f, 0.15f) * 0.6f, spin: 6f);
        for (var i = 0; i < 30; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var reach = Rand(0.6f, r * 0.9f);
            var high = Rand(0.2f, 2.5f);
            var tone = i % 3 == 0 ? Leaf : Dust;
            Func<float, Vector3> at = k => Around(angle + k * 7f) * reach + Vector3.Up * (high + k * 0.8f);
            Line(c, Rand(0f, Life - 0.6f), 0.6f, tone, at, k => (at(Mathf.Min(k + 0.04f, 1f)) - at(k)).Normalized() * 0.6f, 0.07f, k => Fade(k, 0.2f, 0.3f));
        }
    }

    // ---- Nature.

    private void Thornbind(Cast c)
    {
        const float Life = 1.9f;
        const int Vines = 5, Segments = 12;
        for (var v = 0; v < Vines; v++)
        {
            var angle = v * Mathf.Tau / Vines + Rand(-0.2f, 0.2f);
            Func<float, Vector3> along = s => Around(angle + s * 4.5f) * (0.8f - 0.3f * s) + Vector3.Up * s * 2.2f;
            for (var j = 0; j < Segments; j++)
            {
                var (a, b) = (along((float)j / Segments), along((j + 1f) / Segments));
                var start = j * 0.025f;
                Line(c, start, Life - start, Leaf, k => (a + b) * 0.5f, k => b - a, 0.12f - 0.006f * j, k => Fade(k, 0.05f, 0.25f));
                if (j % 3 == 1)
                    Flare(c, start + 0.05f, Life - start - 0.05f, Star, Leaf, k => b, k => 0.35f, k => Fade(k, 0.1f, 0.25f));
            }
        }
        Disc(c, 0f, Life, Cracks, Leaf, Vector3.Zero, k => 1.5f, k => Fade(k, 0.03f, 0.3f), k => k * 0.6f);
        Disc(c, 0f, 0.4f, Shock, Leaf, Vector3.Zero, k => 2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.2f);
        for (var i = 0; i < 14; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * Rand(0.4f, 1.2f) + Vector3.Up * Rand(0.3f, 2f);
            Flare(c, Rand(0.1f, 1f), 0.8f, Mote, Leaf, k => at + new Vector3(Mathf.Sin(k * 9f) * 0.2f, -k * 0.6f, 0f), k => 0.2f, k => Fade(k, 0.2f, 0.4f));
        }
    }

    private void VenomSpit(Cast c, float r, float range)
    {
        const float Flight = 0.5f;
        var length = Mathf.Max(range, 1.5f);
        Func<float, Vector3> arc = s => new Vector3(0f, 1.3f - 1.1f * s + 8f * s * (1f - s), 0.4f + (length - 0.4f) * s);
        Globe(c, "energy", Orb, 0f, Flight, Toxic, arc, k => 0.3f, k => 1f);
        for (var i = 0; i < 10; i++)
        {
            var s = (i + 0.5f) / 10f;
            Spark(c, s * Flight, 0.4f, Toxic, arc(s), new Vector3(Rand(-0.5f, 0.5f), -1f, 1f), Gravity, 0.07f, 0.03f);
        }
        var land = new Vector3(0f, 0f, length);
        Cue(c, Flight, () => Stage.Patch(At(c, land), r * 0.55f, 3f, FxTheme.Of(FxElement.Poison), FxGround.Poison));
        Disc(c, Flight, 0.4f, Shock, Toxic, land, k => r * 0.7f * Out(k), k => 1f - k, band: 0.25f, gain: 1.3f);
        Flare(c, Flight, 0.22f, Star, Toxic, land + Vector3.Up * 0.3f, 2.2f);
        for (var i = 0; i < 16; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Flight, Rand(0.35f, 0.6f), Toxic, land + Vector3.Up * 0.2f, way * Rand(2f, 5f) + Vector3.Up * Rand(3f, 6f), Gravity, 0.09f, 0.03f);
        }
    }

    private void PlagueCloud(Cast c, float r)
    {
        const float Life = 3.2f;
        Cue(c, 0f, () => Stage.Patch(At(c, Vector3.Zero), r, Life, FxTheme.Of(FxElement.Poison), FxGround.Miasma));
        for (var i = 0; i < 28; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * r * 0.9f * Mathf.Sqrt(Rand(0f, 1f));
            Flare(c, Rand(0f, Life - 1.2f), 1.2f, Mote, Toxic, k => at + Vector3.Up * (0.2f + k * 2f), k => 0.25f, k => Fade(k, 0.2f, 0.4f));
        }
        for (var i = 0; i < 20; i++)
        {
            var home = Around(Rand(0f, Mathf.Tau)) * r * 0.7f * Mathf.Sqrt(Rand(0f, 1f)) + Vector3.Up * Rand(0.6f, 1.6f);
            var phase = Rand(0f, Mathf.Tau);
            Flare(c, Rand(0f, 0.5f), Life - 0.6f, Mote, Hive, k =>
                home + new Vector3(Mathf.Sin(k * 37f + phase), Mathf.Sin(k * 53f + phase) * 0.5f, Mathf.Cos(k * 29f + phase)) * 0.35f,
                k => 0.1f, k => Fade(k, 0.05f, 0.1f), gain: 1.6f);
        }
    }

    private void Regrowth(Cast c, float r)
    {
        Column(c, 0f, 1.6f, Spiral, Leaf, Vector3.Zero, k => new Vector2(0.8f, 2.6f * Out(Mathf.Min(k / 0.3f, 1f))), k => Fade(k, 0.05f, 0.4f) * 0.8f, twist: 1.4f, speed: 1.2f, gain: 1.2f);
        Disc(c, 0f, 1.4f, Glow, Verdant, Vector3.Zero, k => 1.5f, k => Fade(k, 0.05f, 0.5f) * 0.8f);
        Disc(c, 0.05f, 0.6f, Shock, Verdant, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
        for (var i = 0; i < 30; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var rise = Rand(1.8f, 2.8f);
            var tone = i % 2 == 0 ? Leaf : Verdant;
            Flare(c, Rand(0f, 0.6f), Rand(0.8f, 1.1f), Mote, tone, k => Around(angle + k * 3.5f) * (0.95f - 0.4f * k) + Vector3.Up * rise * Out(k), k => 0.24f, k => Fade(k, 0.1f, 0.4f));
        }
        for (var i = 0; i < 8; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * Rand(0.4f, 1f) + Vector3.Up * Rand(0.8f, 2.2f);
            Flare(c, Rand(0.1f, 0.8f), 0.5f, Star, Verdant, k => at, k => 0.6f, k => Mathf.Sin(k * Mathf.Pi));
        }
    }

    private void Swarm(Cast c, float range)
    {
        const float Life = 1.6f;
        var length = Mathf.Max(range, 2f);
        Func<float, Vector3> centre = k => new Vector3(0f, 1.1f, 0.5f + (length - 0.5f) * Mathf.SmoothStep(0f, 1f, k));
        for (var i = 0; i < 46; i++)
        {
            var offset = FxKit.RandomDirection() * Rand(0.2f, 0.9f);
            var phase = Rand(0f, Mathf.Tau);
            Flare(c, Rand(0f, 0.15f), Life - 0.15f, Mote, Hive, k =>
                centre(k) + offset * (1f + k * 0.8f) + new Vector3(Mathf.Sin(k * 45f + phase), Mathf.Sin(k * 61f + phase), Mathf.Cos(k * 38f + phase)) * 0.18f,
                k => 0.18f, k => Fade(k, 0.05f, 0.25f), gain: 2.2f);
        }
        Disc(c, 0f, Life, Glow, Hive, Vector3.Zero, k => 1f, k => Fade(k, 0.1f, 0.25f) * 0.5f, stretch: new Vector3(1.1f, 1f, 1.1f));
    }

    // ---- Arcane.

    private void ArcaneMissiles(Cast c, float range)
    {
        const int Missiles = 5;
        const float Flight = 0.4f, Every = 0.09f;
        var length = Mathf.Max(range, 2f);
        var from = new Vector3(0f, 1.3f, 0.3f);
        var to = new Vector3(0f, 1.1f, length);
        for (var i = 0; i < Missiles; i++)
        {
            var bend = new Vector3((i - (Missiles - 1) / 2f) * 1.2f, 2.4f + (i % 2) * 0.6f, length * 0.4f);
            Func<float, Vector3> at = s => from * (1f - s) * (1f - s) + bend * 2f * s * (1f - s) + to * s * s;
            var start = i * Every;
            Flare(c, start, Flight, Mote, Arcane, at, k => 0.45f, k => 1f);
            Line(c, start, Flight, Arcane, k => at(Mathf.Max(k - 0.06f, 0f)), k => at(k) - at(Mathf.Max(k - 0.2f, 0f)), 0.12f, k => 0.9f);
            Flare(c, start + Flight, 0.2f, Star, Arcane, to, 1.6f);
            Disc(c, start + Flight, 0.3f, Shock, Arcane, FxKit.Flat(to), k => 1.2f * Out(k), k => (1f - k) * 0.7f, band: 0.18f);
            for (var j = 0; j < 6; j++)
                Spark(c, start + Flight, Rand(0.2f, 0.35f), Arcane, to, FxKit.RandomDirection() * Rand(4f, 7f), 3f, 0.05f, 0.03f);
        }
    }

    private void ManaShield(Cast c)
    {
        const float Life = 3f;
        Globe(c, "energy", Hexes, 0f, Life, Arcane, k => Vector3.Zero, k => 1.35f * Overshoot(Mathf.Min(k * Life / 0.2f, 1f)), k => Fade(k, 0.02f, 0.1f), gain: 1.1f);
        Disc(c, 0f, Life, Runes, Arcane, Vector3.Zero, k => 1.45f, k => Fade(k, 0.02f, 0.1f), k => k * Life / 0.3f);
        for (var i = 0; i < 10; i++)
        {
            var angle = i * Mathf.Tau / 10f;
            var high = 0.5f + 0.15f * (i % 4);
            Flare(c, 0.1f, Life - 0.2f, Mote, Arcane, k => Around(angle + k * Life * 1.8f) * 1.15f + Vector3.Up * high, k => 0.2f, k => Fade(k, 0.1f, 0.1f));
        }
    }

    private void RiftBeam(Cast c, float range)
    {
        const float Life = 1.2f;
        var length = Mathf.Max(range, 2f);
        var muzzle = new Vector3(0f, 1.2f, 0.7f);
        var end = new Vector3(0f, 1.2f, length);
        Func<float, float> open = k => Out(Mathf.Min(k * Life / 0.08f, 1f)) * (1f - Mathf.SmoothStep(0f, 1f, (k - 0.8f) / 0.2f)) * (1f + 0.08f * Mathf.Sin(k * 90f));
        Tube(c, 0f, Life, 3, Arcane, k => (muzzle, end), k => 0.42f * open(k), k => 1f, speed: 1.5f, gain: 1.3f);
        Tube(c, 0f, Life, 3, Arcane with { Main = Arcane.Hot }, k => (muzzle, end), k => 0.14f * open(k), k => 1f, speed: 2f, gain: 1.5f);
        var upright = new Basis(Vector3.Right, Mathf.Pi / 2f);
        Disc(c, 0f, Life, Runes, Arcane, muzzle, k => 1f, k => Fade(k, 0.02f, 0.2f), k => k * Life / 0.15f, basis: upright, gain: 1.3f);
        Flare(c, 0.05f, Life - 0.1f, Star, Arcane, k => end, k => 2f + 0.5f * Mathf.Sin(k * 50f), k => Fade(k, 0.05f, 0.2f));
        for (var i = 0; i < 24; i++)
            Spark(c, Rand(0.05f, Life - 0.3f), 0.3f, Arcane, end, FxKit.RandomDirection() * Rand(4f, 8f), 3f, 0.05f, 0.03f);
        Disc(c, 0f, Life, Glow, Arcane, new Vector3(0f, 0f, (muzzle.Z + length) / 2f), k => 1f, k => open(k) * 0.5f, stretch: new Vector3(0.8f, 1f, (length - muzzle.Z) / 2f));
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Arcane.Main, age => 1.2f * open(Mathf.Min(age / Life, 1f)));
    }

    private void GravityWell(Cast c, float r)
    {
        const float Life = 2.2f;
        Disc(c, 0f, Life, Swirl, Void, Vector3.Zero, k => r, k => Fade(k, 0.08f, 0.1f), spin: -5f, gain: 1.2f);
        for (var i = 0; i < 5; i++)
            Disc(c, i * 0.4f, 0.5f, Shock, Void, Vector3.Zero, k => r * (1f - k * 0.9f), k => Mathf.Sin(k * Mathf.Pi), band: 0.12f, gain: 1.3f);
        Globe(c, "void", 0, 0f, Life, Void, k => Vector3.Up * 0.15f, k => 0.4f * Fade(k, 0.05f, 0.1f), k => 1f);
        for (var i = 0; i < 30; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var from = r * Rand(0.6f, 1f);
            var high = Rand(1f, 2.5f);
            Flare(c, Rand(0f, Life - 0.6f), 0.6f, Mote, Void, k => Around(angle - k * 2f) * from * (1f - k * k) + Vector3.Up * high * (1f - k), k => 0.22f, k => Fade(k, 0.2f, 0.1f));
        }
    }

    private void Portal(Cast c, float range)
    {
        const float Life = 2.2f;
        var length = Mathf.Max(range, 2f);
        var upright = new Basis(Vector3.Right, Mathf.Pi / 2f);
        var gates = new[] { new Vector3(0f, 1.2f, 0.8f), new Vector3(0f, 1.2f, length) };
        foreach (var gate in gates)
        {
            Disc(c, 0f, Life, Runes, Tide, gate, k => 1.1f, k => Fade(k, 0.02f, 0.1f), k => k * Life / 0.3f, basis: upright, gain: 1.3f);
            Disc(c, 0f, Life, Swirl, Phantom, gate, k => 1f, k => Fade(k, 0.05f, 0.1f), basis: upright, spin: 5f, gain: 1.2f);
            Flare(c, 0f, 0.3f, Star, Tide, gate, 2.6f);
        }
        for (var i = 0; i < 26; i++)
        {
            var off = new Vector3(Rand(-0.5f, 0.5f), Rand(-0.5f, 0.5f), 0f);
            Flare(c, 0.2f + i * 0.06f, 0.5f, Mote, Tide, k => gates[0].Lerp(gates[1], k) + off + Vector3.Up * Mathf.Sin(k * Mathf.Pi) * 0.8f, k => 0.22f, k => Fade(k, 0.15f, 0.15f));
        }
    }

    private void PrismaticSpray(Cast c, float range)
    {
        var length = Mathf.Max(range, 2f);
        var mouth = new Vector3(0f, 1.1f, 0.4f);
        for (var i = 0; i < 36; i++)
        {
            var way = new Vector3(Mathf.Sin(Rand(-0.5f, 0.5f)), Rand(-0.15f, 0.15f), 1f).Normalized();
            var tone = Prism[i % Prism.Length];
            var reach = length * Rand(0.7f, 1f);
            var start = Rand(0f, 0.12f);
            Line(c, start, Rand(0.3f, 0.45f), tone, k => mouth + way * reach * Out(k), k => way * 1.4f * (1f - k * 0.5f), 0.1f, k => 1f - k * k);
            if (i % 3 == 0)
                Flare(c, start + 0.25f, 0.2f, Star, tone, mouth + way * reach, 0.9f);
        }
        Flare(c, 0f, 0.2f, Star, Prism[2] with { Main = new Color(1f, 1f, 1f) }, mouth, 2f);
    }

    // ---- Holy.

    private void DivineShield(Cast c)
    {
        const float Life = 3f;
        Globe(c, "energy", Hexes, 0f, Life, Gold, k => Vector3.Zero, k => 1.35f * Overshoot(Mathf.Min(k * Life / 0.2f, 1f)), k => Fade(k, 0.02f, 0.1f), gain: 1.2f);
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, k => new Vector2(1.25f, 2.6f), k => Fade(k, 0.05f, 0.1f) * 0.35f, speed: 0.8f);
        Disc(c, 0f, Life, Glow, Gold, Vector3.Zero, k => 1.5f, k => Fade(k, 0.05f, 0.1f) * 0.6f);
        Disc(c, 0f, 0.4f, Shock, Gold, Vector3.Zero, k => 2.4f * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
        for (var i = 0; i < 10; i++)
        {
            var angle = i * Mathf.Tau / 10f;
            var high = 0.4f + 0.2f * (i % 5);
            Flare(c, 0.1f, Life - 0.2f, Star, Gold, k => Around(angle - k * Life * 1.2f) * 1.1f + Vector3.Up * high, k => 0.5f, k => Fade(k, 0.1f, 0.1f) * (0.6f + 0.4f * Mathf.Sin(k * 30f + angle)));
        }
    }

    private void Consecration(Cast c, float r)
    {
        const float Life = 3.2f;
        Disc(c, 0f, Life, Glow, Gold, Vector3.Zero, k => r, k => Fade(k, 0.05f, 0.1f) * (0.5f + 0.15f * Mathf.Sin(k * 30f)));
        Disc(c, 0f, Life, Runes, Gold, Vector3.Zero, k => r, k => Fade(k, 0.02f, 0.1f), k => k * Life / 0.4f, gain: 1.2f);
        Disc(c, 0f, 0.45f, Shock, Gold, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        for (var i = 0; i < 28; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * r * 0.9f * Mathf.Sqrt(Rand(0f, 1f));
            Line(c, Rand(0f, Life - 0.9f), 0.8f, Gold, k => at + Vector3.Up * (0.5f + k * 2f), k => Vector3.Up * 1.4f, 0.08f, k => Mathf.Sin(k * Mathf.Pi));
        }
        Shine(c, Vector3.Up, r * 1.3f, Gold.Main, age => 0.6f * Fade(age / Life, 0.05f, 0.1f));
    }

    private void HolyNova(Cast c, float r)
    {
        Disc(c, 0f, 0.55f, Shock, Gold, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.14f, gain: 1.5f);
        Disc(c, 0.08f, 0.5f, Shock, Gold, Vector3.Zero, k => r * 0.7f * Out(k), k => (1f - k) * 0.7f, band: 0.14f);
        Column(c, 0f, 0.5f, Light, Gold, Vector3.Zero, k => new Vector2(1f, 4f), k => (1f - k) * (1f - k), gain: 1.5f);
        Flare(c, 0f, 0.3f, Star, Gold, Vector3.Up, 3.5f);
        for (var i = 0; i < 22; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            var tone = i % 2 == 0 ? Gold : Verdant;
            var far = r * Rand(0.6f, 1f);
            Flare(c, Rand(0f, 0.1f), Rand(0.6f, 0.9f), Mote, tone, k => way * far * Out(k) + Vector3.Up * (0.6f + k * 1.2f), k => 0.25f, k => 1f - k);
        }
        Shine(c, Vector3.Up, r * 1.5f, Gold.Main, age => 1.5f * Mathf.Max(0f, 1f - age / 0.6f));
    }

    private void SeraphWings(Cast c)
    {
        const float Life = 2f;
        foreach (var side in new[] { -1f, 1f })
        {
            for (var f = 0; f < 3; f++)
            {
                // Each wing reaches out to its side and a little up; its feathers fan down below it.
                var spread = Mathf.DegToRad(side < 0f ? -110f : 110f) - side * f * Mathf.DegToRad(22f);
                var hub = new Vector3(side * 0.15f, 1.5f, -0.3f);
                var size = 2.4f - f * 0.35f;
                var start = f * 0.04f;
                Crescent(c, start, Life - start, k =>
                {
                    var beat = Mathf.Sin(k * Life * 7f) * 0.12f * side;
                    var basis = new Basis(Vector3.Back, spread + beat) * new Basis(Vector3.Right, Mathf.Pi / 2f);
                    if (side > 0f)
                        basis *= Basis.FromScale(new Vector3(-1f, 1f, 1f));
                    return new Transform3D(basis, hub);
                }, size, 40f, Gold, thick: 0.85f, sweep: 0.1f, gain: 0.9f);
            }
        }
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, k => new Vector2(0.7f, 3f), k => Fade(k, 0.1f, 0.3f) * 0.5f);
        for (var i = 0; i < 16; i++)
        {
            var at = new Vector3(Rand(-1.6f, 1.6f), Rand(1.2f, 2.6f), Rand(-0.6f, 0f));
            Flare(c, Rand(0.1f, Life - 0.8f), 0.8f, Mote, Gold, k => at + new Vector3(Mathf.Sin(k * 8f) * 0.15f, -k * 1f, 0f), k => 0.18f, k => Fade(k, 0.2f, 0.4f));
        }
    }

    private void SacredLances(Cast c, float r)
    {
        const int Lances = 8;
        const float Fall = 0.2f, Every = 0.08f;
        var drop = new Vector3(0.6f, 10f, -0.4f);
        var down = -drop.Normalized();
        for (var i = 0; i < Lances; i++)
        {
            var spot = Around(Rand(0f, Mathf.Tau)) * r * 0.85f * Mathf.Sqrt(Rand(0.05f, 1f));
            var start = i * Every;
            Line(c, start, Fall, Gold, k => (spot + drop).Lerp(spot + Vector3.Up * 1.2f, k * k), k => down * 2.8f, 0.24f, k => 1f);
            Line(c, start + Fall, 0.6f, Gold, k => spot + Vector3.Up * 1.1f, k => down * 2.4f, 0.2f, k => (1f - k) * (1f - k));
            Flare(c, start + Fall, 0.22f, Star, Gold, spot + Vector3.Up * 0.2f, 2.2f);
            Disc(c, start + Fall, 0.35f, Shock, Gold, spot, k => 1.3f * Out(k), k => 1f - k, band: 0.18f, gain: 1.3f);
            Column(c, start + Fall, 0.3f, Light, Gold, spot, k => new Vector2(0.3f, 3f), k => 1f - k, gain: 1.4f);
        }
        Shine(c, Vector3.Up, r * 1.5f, Gold.Main, age => age < Fall ? 0f : 1.2f * Fade((age - Fall) / (Lances * Every + 0.4f), 0f, 0.4f));
    }

    // ---- Shadow.

    private void ShadowClaws(Cast c, float r)
    {
        // Three arcs of one swing round one centre, at three reaches: parallel claw marks.
        var chest = Vector3.Up * 1.3f;
        var basis = new Basis(Vector3.Back, Mathf.DegToRad(50f));
        for (var i = 0; i < 3; i++)
            Crescent(c, i * 0.02f, 0.4f, new Transform3D(basis, chest), r * (0.68f + 0.11f * i), 120f, Shadow, thick: 0.08f, sweep: 0.25f, gain: 1.6f);
        var cut = chest + Vector3.Back * r * 0.8f;
        Flare(c, 0.05f, 0.22f, Star, Shadow, cut, 2.4f);
        for (var i = 0; i < 14; i++)
            Spark(c, 0.05f, Rand(0.25f, 0.4f), Shadow, cut, FxKit.RandomDirection() * Rand(4f, 8f), Gravity * 0.4f, 0.06f, 0.03f);
        Shine(c, cut, 3f, Shadow.Main, age => Bump(age, 0.05f, 0.35f));
    }

    private void SoulDrain(Cast c, float range)
    {
        const float Life = 1.6f;
        var length = Mathf.Max(range, 2f);
        var hand = new Vector3(0f, 1.2f, 0.4f);
        var foe = new Vector3(0f, 1.1f, length);
        Tube(c, 0f, Life, Spiral, Shadow, k => (foe, hand), k => 0.2f * (1f + 0.15f * Mathf.Sin(k * 70f)), k => Fade(k, 0.05f, 0.15f), twist: 8f, speed: 1.5f, gain: 1.4f);
        Tube(c, 0f, Life, 3, Shadow, k => (foe, hand), k => 0.08f, k => Fade(k, 0.05f, 0.15f), speed: 1.5f, gain: 1.2f);
        for (var i = 0; i < 30; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            Flare(c, i * 0.042f, 0.35f, Mote, Hex, k => foe.Lerp(hand, k) + new Vector3(Mathf.Sin(k * 12f + phase), Mathf.Cos(k * 9f + phase), 0f) * 0.25f, k => 0.22f, k => Fade(k, 0.2f, 0.2f));
        }
        Globe(c, "void", 0, 0f, Life, Void, k => foe, k => 0.35f * Fade(k, 0.05f, 0.15f) * (1f + 0.1f * Mathf.Sin(k * 40f)), k => 1f);
        Globe(c, "energy", Orb, 0f, Life, Shadow, k => hand, k => 0.25f * Fade(k, 0.1f, 0.15f), k => 1f);
    }

    private void DeathCoil(Cast c, float range)
    {
        const float Flight = 0.4f;
        var length = Mathf.Max(range, 2f);
        var from = new Vector3(0f, 1.2f, 0.5f);
        var to = new Vector3(0f, 1.1f, length);
        Func<float, Vector3> at = k => from.Lerp(to, k);
        Globe(c, "void", 0, 0f, Flight, Void, at, k => 0.32f, k => 1f);
        Globe(c, "energy", Shell, 0f, Flight, Shadow, at, k => 0.5f, k => 0.9f);
        for (var i = 0; i < 16; i++)
        {
            var s = (i + 0.5f) / 16f;
            var spot = at(s) + new Vector3(Mathf.Sin(s * 25f), Mathf.Cos(s * 25f), 0f) * 0.3f;
            Flare(c, s * Flight, 0.45f, Mote, Shadow, k => spot + Vector3.Up * k * 0.3f, k => 0.25f * (1f - k), k => 1f - k);
        }
        Globe(c, "void", 0, Flight, 0.35f, Void, k => to, k => 0.3f + 0.9f * Out(k), k => 1f - k);
        Globe(c, "energy", Shell, Flight, 0.35f, Shadow, k => to, k => 0.5f + 1.2f * Out(k), k => 1f - k);
        Disc(c, Flight, 0.4f, Shock, Shadow, FxKit.Flat(to), k => 2f * Out(k), k => 1f - k, band: 0.18f, gain: 1.3f);
        Flare(c, Flight, 0.22f, Star, Shadow, to, 2.4f);
        for (var i = 0; i < 12; i++)
        {
            var push = FxKit.RandomDirection() * Rand(1f, 2f);
            Flare(c, Flight, 0.5f, Mote, Shadow, k => to + push * Out(k), k => 0.3f, k => 1f - k);
        }
    }

    private void ShadeClones(Cast c)
    {
        const float Dash = 0.3f;
        Globe(c, "void", 0, 0f, 0.18f, Void, k => Vector3.Up * 0.9f, k => 0.7f * (1f - k), k => 1f);
        Disc(c, 0f, 0.35f, Shock, Shadow, Vector3.Zero, k => 1.8f * Out(k), k => 1f - k, band: 0.2f);
        foreach (var turn in new[] { -0.9f, 0f, 0.9f })
        {
            var way = Around(turn);
            var end = way * 3.5f + Vector3.Up * 0.85f;
            Globe(c, "energy", Shell, 0f, Dash, Shadow, k => Vector3.Up * 0.85f + way * 3.5f * Out(k), k => 1f, k => 0.9f, mesh: _ghost);
            Globe(c, "energy", Shell, Dash, 0.35f, Shadow, k => end, k => 1f, k => 0.9f * (1f - k), mesh: _ghost);
            for (var j = 0; j < 4; j++)
            {
                var off = new Vector3(Rand(-0.3f, 0.3f), Rand(-0.5f, 0.6f), Rand(-0.3f, 0.3f));
                Line(c, Rand(0f, 0.1f), 0.3f, Shadow, k => Vector3.Up * 0.85f + off + way * 3.5f * Out(k) * 0.7f, k => way * 1.5f * (1f - k), 0.04f, k => 1f - k);
            }
            var slash = new Basis(Vector3.Up, turn) * new Basis(Vector3.Back, Mathf.DegToRad(-15f));
            Crescent(c, Dash, 0.3f, new Transform3D(slash, end - slash * Vector3.Back * 0.2f), 1.3f, 200f, Shadow, thick: 0.35f, sweep: 0.3f, gain: 1.3f);
        }
    }

    private void Curse(Cast c)
    {
        const float Life = 2.2f;
        var sigil = Vector3.Up * 2.5f;
        Disc(c, 0f, Life, Runes, Hex, Vector3.Zero, k => 1.6f, k => Fade(k, 0.02f, 0.1f), k => k * Life / 0.35f, gain: 1.3f);
        Disc(c, 0f, Life, Glow, Hex, Vector3.Zero, k => 1.6f, k => Fade(k, 0.05f, 0.1f) * 0.5f);
        Disc(c, 0f, 0.4f, Shock, Hex, Vector3.Zero, k => 2.2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        Globe(c, "void", 0, 0f, Life, Void with { Main = Hex.Main }, k => sigil, k => 0.28f * Fade(k, 0.05f, 0.1f) * (1f + 0.1f * Mathf.Sin(k * 30f)), k => 1f);
        Flare(c, 0f, Life, Star, Hex, k => sigil, k => 1.3f + 0.3f * Mathf.Sin(k * 30f), k => Fade(k, 0.05f, 0.1f));
        for (var i = 0; i < 26; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * Rand(0.2f, 0.8f) + Vector3.Up * Rand(2f, 2.4f);
            Line(c, Rand(0f, Life - 0.6f), 0.55f, Hex, k => at + Vector3.Down * at.Y * k * k, k => Vector3.Down * 0.6f, 0.05f, k => Fade(k, 0.1f, 0.2f));
        }
    }
}
