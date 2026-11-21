using Godot;

namespace Arc.Fx;

/// <summary>
/// The fifth set's graceful half: blades that leave petals, stars and butterflies behind them,
/// the living things of the woods, the light of heaven, and five blessings.
/// </summary>
internal sealed partial class FxSkills
{
    private void Grace(FxSkill skill, Cast c, float r, float range)
    {
        switch (skill)
        {
            case FxSkill.LotusFlurry: LotusFlurry(c, r); break;
            case FxSkill.StarlitCrescent: StarlitCrescent(c, r); break;
            case FxSkill.ButterflyWaltz: ButterflyWaltz(c, r); break;
            case FxSkill.TwinMoons: TwinMoons(c, r); break;
            case FxSkill.FeatherFan: FeatherFan(c, r); break;
            case FxSkill.IaidoBloom: IaidoBloom(c, r); break;
            case FxSkill.RibbonDance: RibbonDance(c, r); break;
            case FxSkill.PrismaticEdge: PrismaticEdge(c, r); break;
            case FxSkill.DiamondLance: DiamondLance(c, r); break;
            case FxSkill.BlossomRain: BlossomRain(c, r); break;
            case FxSkill.ButterflySwarm: ButterflySwarm(c, r); break;
            case FxSkill.FairyRing: FairyRing(c, r); break;
            case FxSkill.LeafCyclone: LeafCyclone(c, r); break;
            case FxSkill.WispCompanions: WispCompanions(c, r); break;
            case FxSkill.DandelionDrift: DandelionDrift(c, r); break;
            case FxSkill.ThornCrown: ThornCrown(c, r); break;
            case FxSkill.SpiritBloom: SpiritBloom(c, r); break;
            case FxSkill.BloomBurst: BloomBurst(c, r); break;
            case FxSkill.WingsOfDawn: WingsOfDawn(c, r); break;
            case FxSkill.HaloDescent: HaloDescent(c, r); break;
            case FxSkill.FeatherStorm: FeatherStorm(c, r); break;
            case FxSkill.DawnRays: DawnRays(c, r); break;
            case FxSkill.SacredLotus: SacredLotus(c, r); break;
            case FxSkill.StarOfGrace: StarOfGrace(c, r); break;
            case FxSkill.LanternsOfHope: LanternsOfHope(c, r); break;
            case FxSkill.ChoirOfRings: ChoirOfRings(c, r); break;
            case FxSkill.GuardianSpirit: GuardianSpirit(c, r); break;
            case FxSkill.HeartSurge: HeartSurge(c, r); break;
            case FxSkill.StarBlessing: StarBlessing(c, r); break;
            case FxSkill.ButterflyRenewal: ButterflyRenewal(c, r); break;
            case FxSkill.MoonlitVeil: MoonlitVeil(c, r); break;
            case FxSkill.CrystalAegis: CrystalAegis(c, r); break;
            default: Glory(skill, c, r, range); break;
        }
    }

    /// <summary>A random point in a disc of <paramref name="r"/> round <paramref name="centre"/>, between two heights.</summary>
    private static Vector3 Within(Vector3 centre, float r, float low = 0f, float high = 0f) =>
        centre + Around(Rand(0f, Mathf.Tau)) * r * Mathf.Sqrt(Rand(0f, 1f)) + Vector3.Up * Rand(low, high);

    // ---- Blades.

    private void LotusFlurry(Cast c, float r)
    {
        const int Cuts = 6;
        const float Every = 0.09f, End = Cuts * Every + 0.1f;
        var spot = new Vector3(0f, 0f, r * 0.35f);
        Mark(c, 0f, End + 0.7f, Lotus, Petal, spot, _ => r * 0.7f, k => Fade(k, 0.05f, 0.4f), count: 8f, gain: 1.3f);
        for (var i = 0; i < Cuts; i++)
        {
            var plane = new Basis(Vector3.Up, Rand(-0.6f, 0.6f)) * new Basis(Vector3.Back, Mathf.DegToRad(Rand(-70f, 70f)));
            Crescent(c, i * Every, 0.26f, new Transform3D(plane, new Vector3(0f, 1.1f, r * 0.05f)), r * 0.75f, 150f, i % 2 == 0 ? Petal : Pearl, thick: 0.22f, sweep: 0.4f, gain: 1.5f);
        }
        Drift(c, 44, 0.05f, End, 1.1f, Leaflet, [Petal, Rose, Pearl], () => Within(spot, r * 0.45f, 0.5f, 1.8f), () => Around(Rand(0f, Mathf.Tau)) * Rand(0.5f, 2f) + Vector3.Up * Rand(0f, 1.5f), 0.36f, gravity: 2f, slim: 0.5f);
        Hit(c, End, Rose, spot + Vector3.Up * 0.8f, r * 0.6f);
        Mark(c, End, 0.6f, Lotus, Rose, spot, k => r * 0.7f * (1f + 0.25f * Out(k)), k => 1f - k, _ => 1f, count: 8f, gain: 1.6f);
        Twinkle(c, 16, 0f, End + 0.3f, Pearl, () => Within(spot, r * 0.6f, 0.3f, 2f), 0.6f);
        Shine(c, spot + Vector3.Up, r * 1.4f, Petal.Main, age => 0.8f + 1.4f * Bump(age, End, 0.5f));
    }

    private void StarlitCrescent(Cast c, float r)
    {
        const float Life = 0.36f;
        Mark(c, 0f, 1.2f, Galaxy, Stellar, Vector3.Zero, _ => r * 1.05f, k => Fade(k, 0.1f, 0.6f), count: 9f, spin: 2f, gain: 1.3f);
        Crescent(c, 0f, Life, new Transform3D(new Basis(Vector3.Back, Mathf.DegToRad(-6f)), Vector3.Up * 1f), r, 190f, Stellar, thick: 0.34f, sweep: 0.32f, gain: 1.6f);
        Crescent(c, 0.04f, Life, new Transform3D(new Basis(Vector3.Back, Mathf.DegToRad(-6f)), Vector3.Up * 1.15f), r * 0.86f, 190f, Fey, thick: 0.18f, sweep: 0.36f, gain: 1.2f);
        // Stars left hanging along the arc of the cut.
        for (var i = 0; i < 26; i++)
        {
            var angle = Mathf.DegToRad(95f) - i * Mathf.DegToRad(190f) / 25f;
            var at = Around(angle) * r * Rand(0.72f, 1f) + Vector3.Up * Rand(0.7f, 1.4f);
            var s = Rand(0.4f, 0.9f);
            Glyph(c, i * 0.006f + 0.03f, Rand(0.5f, 0.9f), Glint, i % 3 == 0 ? Fey : Stellar, k => at + Vector3.Down * 0.4f * k, k => s * Mathf.Sin(Mathf.Min(k * 1.4f, 1f) * Mathf.Pi), _ => 1f, gain: 1.7f);
        }
        Disc(c, 0.04f, 0.45f, Shock, Stellar, Vector3.Zero, k => r * 1.1f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        for (var i = 0; i < 22; i++)
        {
            var way = Around(Rand(-1.6f, 1.6f));
            Spark(c, Rand(0.02f, 0.14f), Rand(0.4f, 0.6f), Stellar, way * r * 0.9f + Vector3.Up, way * Rand(2f, 5f) + Vector3.Up * Rand(0f, 3f), Gravity * 0.4f, 0.05f, 0.04f);
        }
        Shine(c, Vector3.Up, r * 1.5f, Stellar.Main, age => 1.8f * Mathf.Max(0f, 1f - age / 0.6f));
    }

    private void ButterflyWaltz(Cast c, float r)
    {
        const float Life = 0.9f;
        Mark(c, 0f, Life + 0.6f, Lotus, Fey, Vector3.Zero, _ => r * 0.8f, k => Fade(k, 0.05f, 0.4f), count: 6f, spin: 3f, gain: 1.2f);
        for (var i = 0; i < 3; i++)
        {
            var start = i * 0.22f;
            var twist = i * 2.1f;
            var high = 0.8f + i * 0.3f;
            var tilt = new Basis(Vector3.Right, Mathf.DegToRad(i % 2 == 0 ? 7f : -7f));
            Crescent(c, start, 0.34f, k => new Transform3D(tilt * new Basis(Vector3.Up, k * 2.6f + twist) * Basis.FromScale(Vector3.One * (0.4f + 0.6f * Out(k))), Vector3.Up * high),
                r, 360f, i % 2 == 0 ? Fey : Rose, thick: 0.18f, sweep: 0.55f, gain: 1.5f);
        }
        for (var i = 0; i < 28; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var far = Rand(0.5f, 1f) * r;
            var start = Rand(0f, 0.5f);
            var high = Rand(0.6f, 1.2f);
            var size = Rand(0.4f, 0.65f);
            Func<float, Vector3> at = k => Around(phase + k * 2.4f) * far * (0.35f + 0.75f * Out(k)) + Vector3.Up * (high + 1.4f * k + 0.15f * Mathf.Sin(k * 20f + phase));
            Glyph(c, start, 1f, Flutter, i % 3 == 0 ? Rose : i % 3 == 1 ? Fey : Pearl, at, _ => size, k => Fade(k, 0.15f, 0.3f), gain: 1.7f);
        }
        Twinkle(c, 18, 0.1f, Life, Pearl, () => Within(Vector3.Zero, r, 0.4f, 2.2f), 0.5f);
        Disc(c, 0f, 0.45f, Shock, Fey, Vector3.Zero, k => r * 1.1f * Out(k), k => 1f - k, band: 0.12f, gain: 1.2f);
        Shine(c, Vector3.Up, r * 1.4f, Fey.Main, age => 1.2f * Fade(age / (Life + 0.4f), 0.05f, 0.5f));
    }

    private void TwinMoons(Cast c, float r)
    {
        const float Second = 0.16f, Life = 0.34f;
        var hub = new Vector3(0f, 1.1f, 0f);
        Mark(c, 0f, 1.1f, Corona, Moon, new Vector3(0f, 0f, r * 0.3f), _ => r * 0.9f, k => Fade(k, 0.1f, 0.6f), spin: 2f, gain: 1.2f);
        for (var i = 0; i < 2; i++)
        {
            var roll = new Basis(Vector3.Back, Mathf.DegToRad(i == 0 ? 42f : -42f));
            var start = i * Second;
            Crescent(c, start, Life, new Transform3D(roll, hub), r, 170f, Moon, thick: 0.36f, sweep: 0.3f, gain: 1.6f);
            Crescent(c, start + 0.03f, Life, new Transform3D(roll, hub + Vector3.Up * 0.12f), r * 0.85f, 170f, Phantom, thick: 0.16f, sweep: 0.36f, gain: 1.1f);
            // A sickle moon hangs where each cut ends.
            var tip = hub + roll * Around(Mathf.DegToRad(-80f)) * r * 0.9f;
            Glyph(c, start + 0.1f, 0.7f, Sickle, Moon, tip, 1.1f, 1.6f);
        }
        var cross = hub + Vector3.Back * r * 0.75f;
        Hit(c, Second + 0.1f, Moon, cross, r * 0.5f);
        Glyph(c, 0f, 1f, Sickle, Moon, k => Vector3.Up * (3f + 0.3f * k), k => 1.5f * Overshoot(Mathf.Min(k * 4f, 1f)), k => Fade(k, 0.1f, 0.5f), gain: 1.6f);
        Twinkle(c, 18, 0.05f, 0.6f, Moon, () => hub + Around(Rand(-1.5f, 1.5f)) * r * Rand(0.6f, 1f) + Vector3.Up * Rand(-0.6f, 0.9f), 0.55f);
        Shine(c, cross, r * 1.5f, Moon.Main, age => 1.8f * Mathf.Max(0f, 1f - age / 0.7f));
    }

    private void FeatherFan(Cast c, float r)
    {
        const int Feathers = 9;
        const float Flight = 0.38f;
        var reach = r * 1.5f;
        var hand = new Vector3(0f, 1.2f, 0.4f);
        Flare(c, 0f, 0.25f, Star, Gold, hand, 2.2f);
        Crescent(c, 0f, 0.26f, new Transform3D(Basis.Identity, Vector3.Up * 1.1f), 1.6f, 150f, Gold, thick: 0.3f, sweep: 0.4f, gain: 1.4f);
        for (var i = 0; i < Feathers; i++)
        {
            var angle = (i - (Feathers - 1) * 0.5f) * 0.14f;
            var to = Around(angle) * reach + Vector3.Up * 0.9f;
            var start = Mathf.Abs(i - (Feathers - 1) * 0.5f) * 0.015f;
            var way = (to - hand).Normalized();
            Func<float, Vector3> at = k => hand.Lerp(to, k);
            Glyph(c, start, Flight, Plume, i % 2 == 0 ? Gold : Pearl, at, _ => 1.5f, k => Fade(k, 0.05f, 0.05f), _ => way, gain: 1.8f, slim: 0.36f);
            Line(c, start, Flight, Gold, k => at(Mathf.Max(k - 0.12f, 0f)), _ => way * 1.8f, 0.08f, _ => 0.8f);
            Glyph(c, start + Flight, 0.3f, Glint, Gold, to, 1.4f, 1.7f);
            Disc(c, start + Flight, 0.3f, Shock, Gold, FxKit.Flat(to), k => 0.8f * Out(k), k => 1f - k, band: 0.1f);
            // It stays stuck in the ground a moment.
            Glyph(c, start + Flight, 0.6f, Plume, Gold, _ => FxKit.Flat(to) + Vector3.Up * 0.45f - way * 0.2f, _ => 1.1f, k => 1f - k * k, _ => -way + Vector3.Up * 0.8f, gain: 1.4f, slim: 0.36f);
        }
        Drift(c, 14, Flight, Flight + 0.2f, 0.9f, Plume, [Gold, Pearl], () => Around(Rand(-0.6f, 0.6f)) * reach * Rand(0.8f, 1f) + Vector3.Up * Rand(0.6f, 1.6f), () => new Vector3(0f, -0.8f, 0f), 0.45f, slim: 0.4f);
        Shine(c, new Vector3(0f, 1f, reach * 0.6f), reach, Gold.Main, age => 1.5f * Bump(age, 0f, 0.8f));
    }

    private void IaidoBloom(Cast c, float r)
    {
        const float Pause = 0.38f;
        var mid = new Vector3(0f, 1f, r * 0.3f);
        var half = r * 1.05f;
        // The cut itself: one hairline across everything in front, gone almost at once.
        Line(c, 0f, 0.14f, Pearl, _ => mid, k => Vector3.Right * half * 2f * Out(Mathf.Min(k * 3f, 1f)), 0.22f, k => 1f - k * k);
        Line(c, 0f, Pause + 0.2f, Steel, _ => mid, _ => Vector3.Right * half * 2f, 0.035f, k => Fade(k, 0f, 0.3f));
        Glyph(c, 0f, 0.2f, Glint, Pearl, mid + Vector3.Left * half, 2.2f, 1.8f);
        // A beat later it opens: blossom all along the line.
        for (var i = 0; i < 9; i++)
        {
            var at = mid + Vector3.Right * (i - 4) * half / 4.5f;
            var when = Pause + Mathf.Abs(i - 4) * 0.025f;
            Crescent(c, when, 0.28f, new Transform3D(new Basis(Vector3.Up, Rand(0f, Mathf.Tau)) * new Basis(Vector3.Back, Rand(-1.2f, 1.2f)), at), 0.95f, 150f, i % 2 == 0 ? Petal : Rose, thick: 0.3f, sweep: 0.35f, gain: 1.5f);
            Glyph(c, when, 0.3f, Glint, Pearl, at, 1.6f, 1.6f);
        }
        Drift(c, 60, Pause, Pause + 0.12f, 1.3f, Leaflet, [Petal, Rose, Pearl], () => mid + Vector3.Right * Rand(-half, half) + Vector3.Up * Rand(-0.3f, 0.3f),
            () => new Vector3(Rand(-1f, 1f), Rand(0.5f, 3.5f), Rand(-1.5f, 1.5f)), 0.36f, gravity: 3f, slim: 0.5f);
        Mark(c, Pause, 0.9f, Lotus, Petal, FxKit.Flat(mid), _ => r * 0.75f, k => Fade(k, 0.02f, 0.6f), count: 8f, gain: 1.3f);
        Disc(c, Pause, 0.45f, Shock, Petal, FxKit.Flat(mid), k => r * 1.2f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f, stretch: new Vector3(1f, 1f, 0.5f));
        Shine(c, mid, r * 1.5f, Petal.Main, age => age < Pause ? 0.4f : 2f * Mathf.Max(0f, 1f - (age - Pause) / 0.6f));
    }

    private void RibbonDance(Cast c, float r)
    {
        const float Life = 2.4f;
        Tone[] silks = [Rose, Fey, Gold];
        Mark(c, 0f, Life, Filigree, Rose, Vector3.Zero, _ => r * 0.6f, k => Fade(k, 0.05f, 0.2f), count: 9f, spin: 3f, gain: 1.2f);
        for (var i = 0; i < silks.Length; i++)
        {
            var phase = i * Mathf.Tau / silks.Length;
            var far = r * (0.42f + 0.08f * i);
            Func<float, Vector3> path = s => Around(phase + s * 15f) * far * (0.8f + 0.2f * Mathf.Sin(s * 9f + phase)) + Vector3.Up * (1.2f + 0.8f * Mathf.Sin(s * 11f + phase));
            Ribbon(c, 0f, Life, 0, silks[i], path, k => (Mathf.Max(0f, k - 0.16f), k), 0.5f, k => Fade(k, 0.02f, 0.12f), gain: 1.6f);
            Ribbon(c, 0f, Life, 0, Pearl, path, k => (Mathf.Max(0f, k - 0.07f), k), 0.16f, k => Fade(k, 0.02f, 0.12f), gain: 1.2f);
            Flare(c, 0f, Life, Star, silks[i], path, _ => 0.9f, k => Fade(k, 0.03f, 0.1f));
        }
        Twinkle(c, 40, 0f, Life - 0.4f, Pearl, () => Within(Vector3.Zero, r * 0.6f, 0.3f, 2.3f), 0.45f);
        for (var t = 0.3f; t < Life - 0.4f; t += 0.5f)
            Disc(c, t, 0.5f, Shock, Rose, Vector3.Zero, k => r * 0.8f * Out(k), k => (1f - k) * 0.7f, band: 0.1f);
        Shine(c, Vector3.Up, r, Rose.Main, age => 0.9f * Fade(age / Life, 0.05f, 0.15f));
    }

    private void PrismaticEdge(Cast c, float r)
    {
        const float Every = 0.05f, Life = 0.34f;
        Mark(c, 0f, 1f, Sunburst, Pearl, Vector3.Zero, _ => r * 0.6f, k => Fade(k, 0.05f, 0.6f), count: 12f, spin: 3f, gain: 1.1f);
        for (var i = 0; i < Prism.Length; i++)
        {
            var roll = new Basis(Vector3.Back, Mathf.DegToRad(-50f + i * 20f));
            Crescent(c, i * Every, Life, new Transform3D(roll, Vector3.Up * 1.1f), r * (0.82f + 0.035f * i), 150f, Prism[i], thick: 0.22f, sweep: 0.34f, gain: 1.5f);
            var tip = Vector3.Up * 1.1f + roll * Vector3.Back * r * 0.85f;
            Glyph(c, i * Every + 0.08f, 0.4f, Glint, Prism[i], tip, 1.3f, 1.7f);
        }
        var end = Prism.Length * Every;
        Hit(c, end, Pearl, new Vector3(0f, 1f, r * 0.8f), r * 0.55f);
        for (var i = 0; i < 30; i++)
        {
            var way = Around(Rand(-1.2f, 1.2f));
            Spark(c, Rand(0.05f, end), Rand(0.4f, 0.6f), Prism[i % Prism.Length], way * r * 0.8f + Vector3.Up * Rand(0.4f, 1.8f), way * Rand(2f, 5f) + Vector3.Up * Rand(0f, 3f), Gravity * 0.5f, 0.06f, 0.04f);
        }
        Twinkle(c, 14, 0f, end + 0.2f, Pearl, () => Around(Rand(-1.2f, 1.2f)) * r * Rand(0.5f, 1f) + Vector3.Up * Rand(0.3f, 2f), 0.6f);
        Shine(c, new Vector3(0f, 1f, r * 0.5f), r * 1.5f, Pearl.Main, age => 1.9f * Mathf.Max(0f, 1f - age / 0.8f));
    }

    private void DiamondLance(Cast c, float r)
    {
        const float Thrust = 0.1f, Hold = 0.3f;
        var reach = r * 1.5f;
        var hand = new Vector3(0f, 1.1f, 0.4f);
        var tip = new Vector3(0f, 1.1f, reach);
        Tube(c, 0f, Thrust + Hold, Beam, Steel, k => (hand, hand.Lerp(tip, Out(Mathf.Min(k * (Thrust + Hold) / Thrust, 1f)))), k => 0.14f * (1f - k * 0.7f), k => Fade(k, 0f, 0.5f), speed: 5f, gain: 1.7f);
        Line(c, 0f, Thrust + Hold, Pearl, k => hand.Lerp(tip, 0.5f), _ => (tip - hand) * 1.05f, 0.3f, k => (1f - k) * (1f - k));
        for (var i = 0; i < 7; i++)
        {
            var s = (i + 1f) / 7f;
            var at = hand.Lerp(tip, s);
            var when = Thrust * s;
            Glyph(c, when, 0.5f, Gem, i % 2 == 0 ? Steel : Azure, k => at + Vector3.Up * 0.3f * k, k => (0.5f + 0.5f * s) * (1f - 0.3f * k), k => Fade(k, 0f, 0.5f), k => new Vector3(Mathf.Sin(k * 3f + s * 5f), 1f, 0f), gain: 1.7f);
            Mark(c, when, 0.35f, Ripples, Steel, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (0.3f + (0.5f + s) * k)), at), k => 1f - k, count: 1.5f, spin: 0f, gain: 1.4f);
        }
        Hit(c, Thrust, Steel, tip, r * 0.5f);
        Glyph(c, Thrust, 0.5f, Gem, Pearl, tip, 1.8f, 1.8f);
        for (var i = 0; i < 20; i++)
            Spark(c, Thrust, Rand(0.3f, 0.5f), i % 2 == 0 ? Steel : Azure, tip, FxKit.RandomDirection() * Rand(2f, 6f) + Vector3.Back * 3f, Gravity * 0.5f, 0.06f, 0.04f);
        Twinkle(c, 12, Thrust, Thrust + Hold, Pearl, () => hand.Lerp(tip, Rand(0.2f, 1f)) + FxKit.Jitter(0.4f), 0.5f, 0.35f);
        Shine(c, tip, r, Steel.Main, age => 2f * Bump(age, Thrust, 0.5f));
    }

    // ---- The woods.

    private void BlossomRain(Cast c, float r)
    {
        const float Life = 3f;
        Mark(c, 0f, Life, Lotus, Petal, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.05f, 0.2f), count: 10f, spin: 1.5f, gain: 1.1f);
        Disc(c, 0f, Life, Glow, Petal, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.2f) * 0.3f);
        Drift(c, 110, 0f, Life - 1.2f, 1.5f, Leaflet, [Petal, Rose, Pearl, Petal], () => Within(Vector3.Zero, r * 0.95f, 2.6f, 4f), () => new Vector3(Rand(-0.4f, 0.4f), Rand(-2.4f, -1.6f), Rand(-0.4f, 0.4f)), 0.34f, sway: 0.4f, slim: 0.5f);
        for (var i = 0; i < 16; i++)
        {
            var spot = Within(Vector3.Zero, r * 0.9f);
            Mark(c, Rand(0.4f, Life - 0.8f), 0.7f, Ripples, Petal, spot, k => 0.3f + 0.6f * k, k => (1f - k) * 0.8f, count: 2f, spin: 0f);
        }
        Twinkle(c, 30, 0.2f, Life - 0.5f, Pearl, () => Within(Vector3.Zero, r * 0.9f, 0.2f, 2.5f), 0.5f);
        Shine(c, Vector3.Up, r * 1.4f, Petal.Main, age => 0.8f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void ButterflySwarm(Cast c, float r)
    {
        const float Life = 1.5f;
        var reach = r * 1.6f;
        var from = new Vector3(0f, 1.1f, 0.3f);
        Tone[] wings = [Fey, Rose, Honey, Mint];
        Flare(c, 0f, 0.3f, Star, Fey, from, 2.2f);
        for (var i = 0; i < 34; i++)
        {
            var to = new Vector3(Rand(-1f, 1f) * r * 0.5f, Rand(0.5f, 2f), reach * Rand(0.8f, 1.1f));
            var bend = new Vector3(Rand(-1.5f, 1.5f), Rand(-0.3f, 1.2f), 0f);
            var start = Rand(0f, 0.6f);
            var size = Rand(0.4f, 0.7f);
            var flutter = Rand(0f, Mathf.Tau);
            var tone = wings[i % wings.Length];
            Func<float, Vector3> at = k => from.Lerp(to, k) + bend * Mathf.Sin(k * Mathf.Pi) + Vector3.Up * 0.18f * Mathf.Sin(k * 22f + flutter);
            Glyph(c, start, 0.85f, Flutter, tone, at, _ => size, k => Fade(k, 0.12f, 0.2f), k => Vector3.Up + Heading(at, k, 0.5f), gain: 1.8f);
            if (i % 2 == 0)
                Trail(c, start, 0.85f, tone, at, 0.14f, 0.22f, gain: 1f);
            if (i % 3 == 0)
                Glyph(c, start + 0.85f, 0.3f, Glint, tone, to, 1f, 1.6f);
        }
        Twinkle(c, 40, 0.1f, Life - 0.3f, Pearl, () => from.Lerp(new Vector3(0f, 1.2f, reach), Rand(0f, 1f)) + FxKit.Jitter(0.9f), 0.4f);
        Mark(c, 0.5f, 0.9f, Lotus, Fey, new Vector3(0f, 0f, reach), _ => r * 0.55f, k => Fade(k, 0.1f, 0.5f), count: 6f, gain: 1.3f);
        Shine(c, new Vector3(0f, 1f, reach * 0.6f), reach, Fey.Main, age => 1f * Fade(age / Life, 0.1f, 0.3f));
    }

    private void FairyRing(Cast c, float r)
    {
        const float Life = 3.2f;
        const int Lights = 10;
        var ring = r * 0.75f;
        Mark(c, 0f, Life, Lotus, Mint, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.05f, 0.2f), count: 6f, spin: 2f, gain: 1.2f);
        Mark(c, 0f, Life, Filigree, Mint, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.05f, 0.2f) * 0.8f, count: 10f, spin: -1f);
        for (var i = 0; i < Lights; i++)
        {
            var spot = Around(i * Mathf.Tau / Lights) * ring;
            var start = i * 0.05f;
            var life = Life - start;
            var tone = i % 2 == 0 ? Mint : Honey;
            Mark(c, start, life, Lotus, tone, spot, _ => 0.34f, k => Fade(k, 0.05f, 0.2f), count: 5f, gain: 1.4f);
            Glyph(c, start, life, Wisp, tone, k => spot + Vector3.Up * (0.45f + 0.06f * Mathf.Sin(k * life * 5f + start * 40f)), k => 0.7f * Overshoot(Mathf.Min(k * life / 0.3f, 1f)), k => Fade(k, 0.03f, 0.12f), gain: 1.6f);
            Flare(c, start, life, Mote, tone, _ => spot + Vector3.Up * 0.35f, _ => 0.9f, k => Fade(k, 0.05f, 0.15f) * 0.5f, gain: 1f);
        }
        // Fairies: bright motes looping round the ring, leaving dust.
        for (var i = 0; i < 4; i++)
        {
            var phase = i * Mathf.Tau / 4f;
            var way = i % 2 == 0 ? 1f : -1f;
            Func<float, Vector3> at = k => Around(phase + k * 7f * way) * ring * (0.85f + 0.2f * Mathf.Sin(k * 23f + phase)) + Vector3.Up * (0.9f + 0.4f * Mathf.Sin(k * 17f + phase));
            Flare(c, 0f, Life, Star, Pearl, at, _ => 0.6f, k => Fade(k, 0.05f, 0.1f));
            Trail(c, 0f, Life, i % 2 == 0 ? Mint : Fey, at, 0.2f, 0.06f, gain: 1.4f);
        }
        Twinkle(c, 44, 0.1f, Life - 0.5f, Pearl, () => Within(Vector3.Zero, r * 0.9f, 0.2f, 1.8f), 0.4f);
        Shine(c, Vector3.Up, r * 1.3f, Mint.Main, age => 0.8f * Fade(age / Life, 0.08f, 0.15f));
    }

    private void LeafCyclone(Cast c, float r)
    {
        const float Life = 2.6f;
        var reach = r * 0.55f;
        Disc(c, 0f, Life, Swirl, Leaf, Vector3.Zero, _ => reach * 1.6f, k => Fade(k, 0.08f, 0.15f) * 0.7f, spin: 7f);
        Column(c, 0f, Life, Spiral, Leaf, Vector3.Zero, _ => new Vector2(reach * 0.95f, 3f), k => Fade(k, 0.06f, 0.15f) * 0.5f, twist: 1.5f, speed: 2.5f, gain: 1.1f);
        Tone[] leaves = [Leaf, Honey, Bark, Mint];
        for (var i = 0; i < 70; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var far = reach * Rand(0.6f, 1.3f);
            var turn = Rand(5f, 8f);
            var low = Rand(0.1f, 1.4f);
            var size = Rand(0.3f, 0.5f);
            Func<float, Vector3> at = k => Around(phase + k * turn) * far * (1f + 0.25f * k) + Vector3.Up * (low + 1.8f * k);
            Glyph(c, Rand(0f, Life - 0.9f), 0.9f, Leaflet, leaves[i % leaves.Length], at, _ => size, k => Fade(k, 0.15f, 0.3f), k => Heading(at, k, 1f), gain: 1.6f, slim: 0.5f);
        }
        for (var i = 0; i < 20; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var high = Rand(0.3f, 2.4f);
            Func<float, Vector3> at = k => Around(phase + k * 5f) * reach * 1.1f + Vector3.Up * (high + 0.5f * k);
            Line(c, Rand(0f, Life - 0.5f), 0.5f, Wind, at, k => Heading(at, k, 0.9f), 0.05f, k => Fade(k, 0.2f, 0.3f) * 0.8f);
        }
        Shine(c, Vector3.Up, r, Leaf.Main, age => 0.8f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void WispCompanions(Cast c, float r)
    {
        const float Life = 2.4f, First = 0.5f, Flight = 0.3f;
        var foe = new Vector3(0f, 0.8f, r * 0.75f);
        Tone[] kinds = [Mint, Fey, Honey];
        Mark(c, 0f, Life, Filigree, Mint, Vector3.Zero, _ => r * 0.42f, k => Fade(k, 0.05f, 0.2f), count: 6f, spin: 3f, gain: 1.2f);
        Mark(c, First, Life - First, Reticle, Mint, FxKit.Flat(foe), _ => 0.9f, k => Fade(k, 0.05f, 0.2f) * 0.8f, count: 8f, spin: 2f);
        for (var i = 0; i < kinds.Length; i++)
        {
            var tone = kinds[i];
            var phase = i * Mathf.Tau / kinds.Length;
            Func<float, Vector3> at = k => Around(phase + k * Life * 2.4f) * r * 0.36f + Vector3.Up * (1.5f + 0.25f * Mathf.Sin(k * Life * 5f + phase));
            Globe(c, "energy", Orb, 0f, Life, tone, at, k => 0.17f * Overshoot(Mathf.Min(k * Life / 0.25f, 1f)), k => Fade(k, 0.03f, 0.1f), gain: 1.6f);
            Glyph(c, 0f, Life, Wisp, tone, k => at(k) + Vector3.Up * 0.2f, _ => 0.75f, k => Fade(k, 0.03f, 0.1f), gain: 1.5f);
            Trail(c, 0f, Life, tone, at, 0.22f, 0.08f);
            for (var shot = 0; shot < 4; shot++)
            {
                var when = First + shot * 0.42f + i * 0.14f;
                var o = at(when / Life);
                Func<float, Vector3> bolt = k => o.Lerp(foe, k) + Vector3.Up * 0.5f * Mathf.Sin(k * Mathf.Pi);
                Flare(c, when, Flight, Star, tone, bolt, _ => 0.8f, _ => 1f);
                Trail(c, when, Flight, tone, bolt, 0.22f, 0.5f);
                Glyph(c, when + Flight, 0.3f, Glint, tone, foe + FxKit.Jitter(0.3f), 1.3f, 1.7f);
                Disc(c, when + Flight, 0.3f, Shock, tone, FxKit.Flat(foe), k => 0.9f * Out(k), k => 1f - k, band: 0.1f);
            }
        }
        Twinkle(c, 16, 0f, Life - 0.4f, Pearl, () => Within(Vector3.Zero, r * 0.45f, 0.8f, 2.2f), 0.4f);
        Shine(c, Vector3.Up, r, Mint.Main, age => 0.8f * Fade(age / Life, 0.05f, 0.1f));
    }

    private void DandelionDrift(Cast c, float r)
    {
        const float Puff1 = 0.3f, Life = 2.8f;
        var head = Vector3.Up * 1.2f;
        Globe(c, "energy", Shell, 0f, Puff1, Pearl, _ => head, k => 0.5f + 0.2f * k, k => 0.5f + 0.5f * k, gain: 1.4f);
        Glyph(c, 0f, Puff1, Snow, Pearl, _ => head, k => 1.1f + 0.3f * k, _ => 1f, gain: 1.6f);
        Flare(c, Puff1, 0.3f, Star, Pearl, head, 2.6f);
        Disc(c, Puff1, 0.5f, Shock, Mint, Vector3.Zero, k => r * Out(k), k => (1f - k) * 0.7f, band: 0.1f);
        Mark(c, Puff1, Life - Puff1, Ripples, Mint, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.1f, 0.4f) * 0.5f, count: 3f, spin: 0.4f);
        // Seeds: each a tiny star on a hair, floating up and away on the wind.
        for (var i = 0; i < 46; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            var far = Rand(0.8f, r);
            var rise = Rand(0.4f, 2.2f);
            var sway = Rand(0f, Mathf.Tau);
            var life = Rand(1.6f, Life - Puff1);
            var size = Rand(0.3f, 0.5f);
            Func<float, Vector3> at = k => head + way * far * Out(k) + Vector3.Up * (rise * k + 0.15f * Mathf.Sin(k * 8f + sway)) + new Vector3(0.6f, 0f, 0.2f) * k * k;
            Glyph(c, Puff1 + Rand(0f, 0.12f), life, Snow, i % 4 == 0 ? Mint : Pearl, at, _ => size, k => Fade(k, 0.05f, 0.4f), gain: 1.5f);
            Line(c, Puff1, life, Pearl, k => at(k) + Vector3.Down * 0.14f, _ => Vector3.Up * 0.26f, 0.02f, k => Fade(k, 0.05f, 0.4f) * 0.7f);
        }
        Twinkle(c, 24, Puff1, Life - 0.6f, Pearl, () => Within(head, r * 0.8f, -0.5f, 2f), 0.4f);
        Shine(c, head, r * 1.2f, Pearl.Main, age => 0.5f + 1.2f * Bump(age, Puff1, 0.6f));
    }

    private void ThornCrown(Cast c, float r)
    {
        const int Thorns = 14;
        const float Burst = 0.3f, Hold = 1.2f;
        var ring = r * 0.6f;
        Mark(c, 0f, Burst + Hold, Sunburst, Leaf, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.05f, 0.3f), k => Mathf.Min(k * (Burst + Hold) / Burst, 1f), count: 14f, gain: 1.2f);
        Disc(c, 0f, Burst, Telegraph, Leaf, Vector3.Zero, _ => ring, _ => 0.6f, k => k);
        for (var i = 0; i < Thorns; i++)
        {
            var way = Around(i * Mathf.Tau / Thorns);
            var foot = way * ring * 0.85f;
            var lean = (way * 0.75f + Vector3.Up).Normalized();
            var tall = i % 2 == 0 ? 2f : 1.3f;
            var start = Burst + i * 0.012f;
            Func<float, float> out1 = k => Out(Mathf.Min(k * Hold / 0.1f, 1f)) * (1f - Mathf.Max(0f, (k * Hold - Hold + 0.3f) / 0.3f));
            Line(c, start, Hold, Bark, k => foot + lean * tall * 0.5f * out1(k), k => lean * tall * out1(k), 0.34f, k => Fade(k, 0f, 0.1f));
            Line(c, start, Hold, Leaf, k => foot + lean * tall * 0.5f * out1(k), k => lean * tall * out1(k), 0.12f, k => Fade(k, 0f, 0.1f));
            Glyph(c, start + 0.1f, 0.4f, Glint, Leaf, foot + lean * tall, 0.9f, 1.6f);
        }
        Disc(c, Burst, 0.45f, Shock, Leaf, Vector3.Zero, k => r * 1.2f * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
        Disc(c, Burst, Hold, Cracks, Bark, Vector3.Zero, _ => r * 0.75f, k => Fade(k, 0.02f, 0.4f), k => k);
        Drift(c, 30, Burst, Burst + 0.2f, 1.1f, Leaflet, [Rose, Crimson, Petal], () => Within(Vector3.Zero, ring, 0.5f, 1.6f), () => Around(Rand(0f, Mathf.Tau)) * Rand(0.5f, 2.5f) + Vector3.Up * Rand(1f, 3f), 0.32f, gravity: 4f, slim: 0.55f);
        Spray(c, Burst, Leaf, Vector3.Up * 0.3f, 24, 5f, 6f, 0.6f);
        Shine(c, Vector3.Up, r * 1.4f, Leaf.Main, age => age < Burst ? 0.4f : 1.6f * Mathf.Max(0f, 1f - (age - Burst) / 0.8f));
    }

    private void SpiritBloom(Cast c, float r)
    {
        const float Open = 0.6f, Life = 2.6f;
        const int Petals = 8;
        var heart = Vector3.Up * 0.5f;
        Mark(c, 0f, Life, Lotus, Rose, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.05f, 0.25f), count: 8f, spin: 1f, gain: 1.2f);
        Disc(c, 0f, Life, Glow, Rose, Vector3.Zero, _ => r * 0.8f, k => Fade(k, 0.1f, 0.25f) * 0.4f);
        // Two rings of great petals laid outward round a heart of light, opening.
        for (var layer = 0; layer < 2; layer++)
        {
            for (var i = 0; i < Petals; i++)
            {
                var way = Around((i + layer * 0.5f) * Mathf.Tau / Petals);
                var long1 = r * (layer == 0 ? 0.78f : 0.5f);
                var delay = layer * 0.15f + i * 0.02f;
                var life = Life - delay;
                var sway = (float)i;
                Func<float, float> open = k => Overshoot(Mathf.Min(k * life / Open, 1f));
                Glyph(c, delay, life, Leaflet, layer == 0 ? Rose : Petal, k => heart + way * long1 * 0.55f * open(k) + Vector3.Up * (layer * 0.25f + 0.1f * Mathf.Sin(k * life * 2f + sway)),
                    k => long1 * open(k), k => Fade(k, 0.03f, 0.2f), _ => way + Vector3.Up * (layer * 0.4f), gain: 1.5f, slim: 0.55f);
            }
        }
        Globe(c, "energy", Orb, 0f, Life, Honey, _ => heart + Vector3.Up * 0.2f, k => r * 0.13f * Overshoot(Mathf.Min(k * Life / Open, 1f)), k => Fade(k, 0.03f, 0.15f), gain: 1.6f);
        Flare(c, 0f, Life, Star, Honey, _ => heart + Vector3.Up * 0.3f, k => 1.6f + 0.3f * Mathf.Sin(k * Life * 6f), k => Fade(k, 0.05f, 0.2f));
        // Pollen rising from the heart.
        Drift(c, 40, Open, Life - 1f, 1.1f, Glint, [Honey, Pearl], () => Within(heart, r * 0.25f, 0.2f, 0.5f), () => new Vector3(Rand(-0.5f, 0.5f), Rand(0.8f, 1.8f), Rand(-0.5f, 0.5f)), 0.4f, tumble: false, gain: 1.7f);
        Disc(c, Open * 0.6f, 0.5f, Shock, Rose, Vector3.Zero, k => r * 1.1f * Out(k), k => (1f - k) * 0.8f, band: 0.1f);
        Shine(c, Vector3.Up, r * 1.3f, Rose.Main, age => 1f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void BloomBurst(Cast c, float r)
    {
        const float Flight = 0.45f, Grow = 0.5f, Hold = 1.3f;
        var from = new Vector3(0f, 1.3f, 0.3f);
        var spot = new Vector3(0f, 0f, r * 0.45f);
        Func<float, Vector3> seed = k => from.Lerp(spot + Vector3.Up * 0.15f, k) + Vector3.Up * 1.6f * Mathf.Sin(k * Mathf.Pi);
        Globe(c, "energy", Orb, 0f, Flight, Leaf, seed, _ => 0.16f, _ => 1f, gain: 1.6f);
        Trail(c, 0f, Flight, Leaf, seed, 0.3f, 0.4f);
        Hit(c, Flight, Leaf, spot + Vector3.Up * 0.2f, r * 0.45f);
        Mark(c, Flight, Grow + Hold, Lotus, Leaf, spot, _ => r * 0.4f, k => Fade(k, 0.03f, 0.3f), count: 6f, gain: 1.3f);
        Tone[] flowers = [Rose, Honey, Fey, Petal, Pearl, Rose];
        for (var i = 0; i < 6; i++)
        {
            var tip = Around(i * Mathf.Tau / 6f + Rand(-0.2f, 0.2f)) * r * Rand(0.6f, 0.8f);
            var side = tip.Cross(Vector3.Up).Normalized() * Rand(0.25f, 0.45f) * (i % 2 == 0 ? 1f : -1f);
            Func<float, Vector3> path = s => spot + tip * s + side * Mathf.Sin(s * Mathf.Tau) + Vector3.Up * 0.06f;
            Ribbon(c, Flight, Grow + Hold, 0, Leaf, path, k => (0f, Mathf.Min(k * (Grow + Hold) / Grow, 1f)), 0.36f, k => Fade(k, 0f, 0.25f), gain: 1.3f);
            // A flower opens at the end of each vine.
            var end = path(1f);
            var when = Flight + Grow;
            var tone = flowers[i];
            Mark(c, when, Hold, Lotus, tone, FxKit.Flat(end), k => 0.55f * Overshoot(Mathf.Min(k * Hold / 0.25f, 1f)), k => Fade(k, 0f, 0.3f), _ => 1f, count: 5f, spin: 2f, gain: 1.6f);
            Glyph(c, when, 0.35f, Glint, tone, end + Vector3.Up * 0.3f, 1.2f, 1.6f);
            Drift(c, 5, when, when + 0.2f, 0.9f, Leaflet, [tone], () => end + Vector3.Up * 0.3f, () => FxKit.Jitter(0.8f) + Vector3.Up * 1.4f, 0.28f, gravity: 2f, slim: 0.5f);
        }
        Twinkle(c, 18, Flight + Grow, Flight + Grow + Hold - 0.4f, Pearl, () => Within(spot, r * 0.8f, 0.2f, 1.2f), 0.45f);
        Shine(c, spot + Vector3.Up, r * 1.3f, Leaf.Main, age => age < Flight ? 0.3f : 1.1f * Fade((age - Flight) / (Grow + Hold), 0.02f, 0.3f));
    }

    // ---- Heaven.

    /// <summary>A pair of wings of light standing behind a point, unfurling.</summary>
    private void WingPair(Cast c, float start, float life, Tone tone, Vector3 at, float size, float gain = 1.6f)
    {
        var face = Stand(Vector3.Back);
        Mark(c, start, life, Wings, tone, k => new Transform3D(face * Basis.FromScale(Vector3.One * size * (1f + 0.03f * Mathf.Sin(k * life * 4f))), at), k => Fade(k, 0.02f, 0.15f),
            k => Overshoot(Mathf.Min(k * life / 0.45f, 1f)) * (0.96f + 0.04f * Mathf.Sin(k * life * 4f)), count: 9f, gain: gain);
        Mark(c, start, life, Wings, tone with { Main = tone.Hot }, k => new Transform3D(face * Basis.FromScale(Vector3.One * size * 0.72f), at + Vector3.Back * 0.05f), k => Fade(k, 0.02f, 0.15f) * 0.6f,
            k => Out(Mathf.Min(k * life / 0.6f, 1f)), count: 6f, gain: gain * 0.8f);
    }

    private void WingsOfDawn(Cast c, float r)
    {
        const float Life = 3f;
        WingPair(c, 0f, Life, Gold, new Vector3(0f, 2.1f, -0.25f), 2.9f);
        Mark(c, 0f, Life, Sunburst, Gold, Vector3.Zero, _ => r * 0.6f, k => Fade(k, 0.05f, 0.15f), count: 16f, spin: 1.5f, gain: 1.2f);
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, _ => new Vector2(0.75f, 9f), k => Fade(k, 0.08f, 0.2f) * 0.5f, gain: 1.3f, speed: -1.2f);
        Disc(c, 0.15f, Life - 0.15f, Band, Gold, k => Lay(Vector3.Up * 2.35f, 0.4f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f))), k => Fade(k, 0.05f, 0.15f), band: 0.1f, gain: 1.9f);
        Flare(c, 0f, 0.4f, Star, Gold, Vector3.Up * 1.6f, 4f);
        Disc(c, 0f, 0.5f, Shock, Gold, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Drift(c, 30, 0.3f, Life - 1.4f, 1.4f, Plume, [Gold, Pearl], () => new Vector3(Rand(-2.4f, 2.4f), Rand(2.4f, 3.8f), Rand(-0.6f, 0.6f)), () => new Vector3(0f, Rand(-1.6f, -1f), 0f), 0.5f, sway: 0.45f, slim: 0.42f);
        Twinkle(c, 26, 0.1f, Life - 0.5f, Pearl, () => new Vector3(Rand(-2.6f, 2.6f), Rand(0.6f, 3.6f), Rand(-0.4f, 0.4f)), 0.5f);
        Shine(c, Vector3.Up * 1.5f, r * 1.4f, Gold.Main, age => 1.3f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void HaloDescent(Cast c, float r)
    {
        const float Fall = 0.8f, After = 0.9f;
        var spot = new Vector3(0f, 0f, r * 0.3f);
        Func<float, float> high = k => 7f * (1f - k) * (1f - k) + 0.1f;
        Mark(c, 0f, Fall + After, Sunburst, Gold, spot, _ => r * 0.7f, k => Fade(k, 0.05f, 0.4f), k => Mathf.Min(k * (Fall + After) / Fall, 1f), count: 12f, gain: 1.2f);
        Mark(c, 0f, Fall, Filigree, Gold, k => Lay(spot + Vector3.Up * high(k), r * 0.6f, new Basis(Vector3.Up, k * 4f)), k => Mathf.Min(k * 4f, 1f), _ => 1f, count: 10f, gain: 1.7f);
        Disc(c, 0f, Fall, Band, Gold, k => Lay(spot + Vector3.Up * high(k), r * 0.62f), k => Mathf.Min(k * 4f, 1f), band: 0.16f, gain: 1.6f);
        Column(c, 0f, Fall, Light, Gold, spot, k => new Vector2(r * 0.5f, high(k) + 0.5f), k => 0.25f + 0.3f * k, gain: 1.2f, speed: -2f);
        Twinkle(c, 20, 0f, Fall, Pearl, () => Within(spot, r * 0.6f, 0.5f, 5f), 0.5f);
        Column(c, Fall, After, Light, Gold, spot, _ => new Vector2(r * 0.62f, 11f), k => (1f - k) * (1f - k), gain: 1.8f, speed: -2.5f);
        Hit(c, Fall, Gold, spot + Vector3.Up * 0.4f, r * 0.75f);
        Mark(c, Fall, After, Filigree, Gold, spot, k => r * 0.62f * (1f + 0.3f * Out(k)), k => 1f - k, _ => 1f, count: 10f, gain: 1.6f);
        Cue(c, Fall, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.4f), r * 0.5f, FxTheme.Of(FxElement.Holy), FxBurst.Dome));
        Drift(c, 14, Fall, Fall + 0.2f, 1f, Plume, [Gold, Pearl], () => Within(spot, r * 0.5f, 1f, 2.6f), () => new Vector3(0f, -1.2f, 0f), 0.45f, slim: 0.42f);
        Shine(c, spot + Vector3.Up, r * 1.6f, Gold.Main, age => age < Fall ? 0.3f + 0.7f * age / Fall : 2.4f * Mathf.Max(0f, 1f - (age - Fall) / After));
    }

    private void FeatherStorm(Cast c, float r)
    {
        const float Life = 2.6f;
        Disc(c, 0f, Life, Swirl, Gold, Vector3.Zero, _ => r, k => Fade(k, 0.08f, 0.15f) * 0.6f, spin: 5f);
        Mark(c, 0f, Life, Filigree, Gold, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.15f) * 0.8f, count: 12f, spin: 2f);
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, _ => new Vector2(r * 0.25f, 8f), k => Fade(k, 0.08f, 0.2f) * 0.5f, gain: 1.3f, speed: -1.5f);
        for (var i = 0; i < 72; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var far = r * Rand(0.25f, 0.95f);
            var turn = Rand(3.5f, 5.5f);
            var high = Rand(0.3f, 2.4f);
            var size = Rand(0.55f, 0.85f);
            Func<float, Vector3> at = k => Around(phase + k * turn) * far + Vector3.Up * (high + 0.3f * Mathf.Sin(k * 9f + phase));
            Glyph(c, Rand(0f, Life - 1f), 1f, Plume, i % 3 == 0 ? Pearl : Gold, at, _ => size, k => Fade(k, 0.15f, 0.25f), k => Heading(at, k, 1f) + Vector3.Up * 0.3f, gain: 1.7f, slim: 0.4f);
        }
        for (var t = 0.2f; t < Life - 0.5f; t += 0.45f)
        {
            var spot = Within(Vector3.Zero, r * 0.8f);
            Glyph(c, t, 0.35f, Glint, Pearl, spot + Vector3.Up, 1.6f, 1.7f);
            Disc(c, t, 0.35f, Shock, Gold, spot, k => 1f * Out(k), k => 1f - k, band: 0.16f);
        }
        Twinkle(c, 24, 0f, Life - 0.4f, Pearl, () => Within(Vector3.Zero, r, 0.3f, 2.6f), 0.45f);
        Shine(c, Vector3.Up, r * 1.4f, Gold.Main, age => 1f * Fade(age / Life, 0.08f, 0.15f));
    }

    private void DawnRays(Cast c, float r)
    {
        const float Life = 2.2f;
        const int Shafts = 7;
        var sun = new Vector3(-3f, 12f, -6f);
        Mark(c, 0f, Life, Sunburst, Dawn, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.05f, 0.2f), count: 16f, spin: 1f, gain: 1.2f);
        Mark(c, 0f, Life, Rays, Dawn, Vector3.Zero, _ => r * 1.1f, k => Fade(k, 0.1f, 0.3f) * 0.5f, k => Mathf.Min(k * 3f, 1f), count: 18f, spin: 0.6f);
        for (var i = 0; i < Shafts; i++)
        {
            var spot = i == 0 ? Vector3.Zero : Around(i * Mathf.Tau / (Shafts - 1)) * r * 0.62f;
            var start = i * 0.13f;
            var life = Life - start;
            var top = spot + (sun - spot).Normalized() * 11f;
            var wide = i == 0 ? 0.3f : 0.17f;
            Tube(c, start, life, Beam, Dawn, _ => (top, spot), k => wide * Out(Mathf.Min(k * life / 0.2f, 1f)), k => Fade(k, 0f, 0.35f) * 0.6f, speed: -1.5f, gain: 1.2f);
            Disc(c, start, life, Glow, Dawn, spot, _ => 0.9f, k => Fade(k, 0.05f, 0.35f) * 0.8f, gain: 1.3f);
            Mark(c, start, 0.4f, Rays, Gold, spot, _ => 1.1f, k => 1f - k, k => Out(k), count: 12f, gain: 1.4f);
            Glyph(c, start, 0.35f, Glint, Pearl, spot + Vector3.Up * 0.3f, 1.5f, 1.7f);
        }
        Drift(c, 34, 0.2f, Life - 1.1f, 1.1f, Glint, [Dawn, Pearl, Gold], () => Within(Vector3.Zero, r * 0.9f, 0.1f, 0.4f), () => (sun.Normalized() * Rand(1f, 2.4f)), 0.4f, tumble: false, gain: 1.6f);
        Shine(c, Vector3.Up, r * 1.6f, Dawn.Main, age => 1.3f * Fade(age / Life, 0.08f, 0.25f));
    }

    private void SacredLotus(Cast c, float r)
    {
        const float Life = 2.8f;
        Mark(c, 0f, Life, Lotus, Gold, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.05f, 0.2f), count: 12f, spin: 1.5f, gain: 1.4f);
        Mark(c, 0.2f, Life - 0.2f, Lotus, Pearl, k => Lay(Vector3.Up * (0.25f + 0.6f * k), r * 0.5f, new Basis(Vector3.Up, -k * 1.5f)), k => Fade(k, 0.05f, 0.3f) * 0.8f, count: 8f, gain: 1.3f);
        Mark(c, 0.4f, Life - 0.4f, Lotus, Gold, k => Lay(Vector3.Up * (0.4f + 1.3f * k), r * 0.28f, new Basis(Vector3.Up, k * 2f)), k => Fade(k, 0.05f, 0.3f) * 0.8f, count: 6f, gain: 1.4f);
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, _ => new Vector2(r * 0.3f, 9f), k => Fade(k, 0.08f, 0.25f) * 0.8f, gain: 1.5f, speed: -1.5f);
        Disc(c, 0f, Life, Glow, Gold, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.1f, 0.2f) * 0.35f);
        Drift(c, 40, 0.2f, Life - 1.3f, 1.3f, Leaflet, [Gold, Pearl, Dawn], () => Within(Vector3.Zero, r * 0.8f, 0.1f, 0.3f), () => new Vector3(Rand(-0.3f, 0.3f), Rand(1f, 2.2f), Rand(-0.3f, 0.3f)), 0.36f, sway: 0.35f, slim: 0.5f);
        foreach (var at in new[] { 0.1f, 0.9f, 1.7f })
            Disc(c, at, 0.7f, Shock, Gold, Vector3.Zero, k => r * Out(k), k => (1f - k) * 0.8f, band: 0.1f, gain: 1.2f);
        Twinkle(c, 30, 0.2f, Life - 0.5f, Pearl, () => Within(Vector3.Zero, r * 0.85f, 0.2f, 2.8f), 0.5f);
        Shine(c, Vector3.Up, r * 1.4f, Gold.Main, age => 1.2f * Fade(age / Life, 0.08f, 0.2f));
    }

    private void StarOfGrace(Cast c, float r)
    {
        const float Fall = 0.7f, After = 1f;
        var spot = new Vector3(0f, 0f, r * 0.3f);
        var sky = spot + new Vector3(1.5f, 13f, -5f);
        Func<float, Vector3> at = k => sky.Lerp(spot + Vector3.Up * 0.3f, k * k);
        Mark(c, 0f, Fall + After, Pentacle, Stellar, spot, _ => r * 0.75f, k => Fade(k, 0.04f, 0.4f), k => Mathf.Min(k * (Fall + After) / Fall, 1f), count: 8f, spin: 2f, gain: 1.3f);
        Flare(c, 0f, Fall, Star, Stellar, at, k => 2.4f + 1.2f * k, _ => 1f, gain: 1.6f);
        Globe(c, "energy", Orb, 0f, Fall, Stellar, at, _ => 0.3f, _ => 1f, gain: 1.7f);
        Trail(c, 0f, Fall, Stellar, at, 0.9f, 0.4f, gain: 1.5f);
        Trail(c, 0f, Fall, Pearl, at, 0.3f, 0.55f, gain: 1.3f);
        for (var i = 0; i < 20; i++)
        {
            var s = Rand(0.15f, 1f);
            var p = at(s);
            Glyph(c, s * Fall, Rand(0.4f, 0.7f), Glint, Stellar, k => p + FxKit.Flat(sky - spot).Normalized() * 0.3f * k + Vector3.Down * 0.4f * k, k => 0.6f * Mathf.Sin(k * Mathf.Pi), _ => 1f, gain: 1.7f);
        }
        Hit(c, Fall, Stellar, spot + Vector3.Up * 0.3f, r * 0.85f);
        Cue(c, Fall, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.3f), r * 0.6f, Stellar.Main, FxBurst.Dome));
        Column(c, Fall, After, Light, Stellar, spot, _ => new Vector2(r * 0.3f, 10f), k => (1f - k) * (1f - k), gain: 1.8f, speed: -2f);
        Mark(c, Fall, After, Rays, Stellar, spot, _ => r * 1.3f, k => 1f - k, k => Out(Mathf.Min(k * 3f, 1f)), count: 16f, spin: 1f, gain: 1.4f);
        for (var i = 0; i < 8; i++)
        {
            var way = Around(i * Mathf.Tau / 8f);
            Func<float, Vector3> shard = k => spot + way * r * 0.95f * Out(k) + Vector3.Up * (0.3f + 1.2f * Mathf.Sin(k * Mathf.Pi));
            Flare(c, Fall, 0.6f, Star, Stellar, shard, k => 1f - 0.5f * k, k => 1f - k * k);
            Trail(c, Fall, 0.6f, Stellar, shard, 0.2f, 0.4f);
        }
        Shine(c, spot + Vector3.Up, r * 1.8f, Stellar.Main, age => age < Fall ? 0.3f + 0.6f * age / Fall : 2.6f * Mathf.Max(0f, 1f - (age - Fall) / After));
    }

    private void LanternsOfHope(Cast c, float r)
    {
        const float Life = 3.4f;
        const int Lanterns = 12;
        Mark(c, 0f, Life, Filigree, Dawn, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.05f, 0.2f), count: 12f, spin: 1f, gain: 1.1f);
        Disc(c, 0f, Life, Glow, Dawn, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.1f, 0.2f) * 0.3f);
        for (var i = 0; i < Lanterns; i++)
        {
            var spot = Around(i * Mathf.Tau / Lanterns + Rand(-0.15f, 0.15f)) * r * (i % 2 == 0 ? 0.75f : 0.4f);
            var start = Rand(0f, 0.9f);
            var life = Life - start - Rand(0f, 0.3f);
            var rise = Rand(2.6f, 3.6f);
            var sway = Rand(0f, Mathf.Tau);
            var tone = i % 3 == 0 ? Gold : Dawn;
            Func<float, Vector3> at = k => spot + Vector3.Up * (0.3f + rise * k * (0.4f + 0.6f * k)) + new Vector3(Mathf.Sin(k * 4f + sway), 0f, Mathf.Cos(k * 3f + sway)) * 0.2f;
            Globe(c, "energy", Shell, start, life, tone, at, _ => 0.3f, k => Fade(k, 0.08f, 0.25f), gain: 1.4f);
            Glyph(c, start, life, Wisp, tone, k => at(k) + Vector3.Up * 0.04f, _ => 0.5f, k => Fade(k, 0.08f, 0.25f), gain: 1.7f);
            Flare(c, start, life, Mote, tone, at, _ => 1.1f, k => Fade(k, 0.08f, 0.25f) * 0.55f, gain: 1.1f);
            Disc(c, start, life * 0.6f, Glow, tone, spot, k => 0.6f * (1f - k), k => (1f - k) * 0.8f);
            Mark(c, start, 0.5f, Ripples, tone, spot, k => 0.3f + 0.6f * k, k => 1f - k, count: 2f, spin: 0f);
        }
        Twinkle(c, 34, 0.3f, Life - 0.5f, Pearl, () => Within(Vector3.Zero, r * 0.9f, 0.4f, 3.6f), 0.4f);
        Shine(c, Vector3.Up, r * 1.4f, Dawn.Main, age => 1f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void ChoirOfRings(Cast c, float r)
    {
        const float Life = 2.8f;
        var reach = r * 0.45f;
        Mark(c, 0f, Life, Ripples, Gold, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.08f, 0.2f) * 0.8f, count: 4f, spin: 0.8f, gain: 1.2f);
        Mark(c, 0f, Life, Sunburst, Gold, Vector3.Zero, _ => reach * 1.1f, k => Fade(k, 0.05f, 0.2f), count: 12f, spin: 2f, gain: 1.2f);
        // Halos lifting off the ground one after another and widening as they rise.
        for (var i = 0; i < 9; i++)
        {
            var start = i * 0.26f;
            var tone = i % 3 == 0 ? Pearl : Gold;
            Disc(c, start, 0.9f, Band, tone, k => Lay(Vector3.Up * (0.1f + 2.8f * k), reach * (0.7f + 0.9f * k)), k => Fade(k, 0.1f, 0.5f), band: 0.14f, gain: 1.7f);
            Mark(c, start, 0.9f, Filigree, tone, k => Lay(Vector3.Up * (0.1f + 2.8f * k), reach * (0.7f + 0.9f * k) * 1.15f, new Basis(Vector3.Up, k * 2f)), k => Fade(k, 0.1f, 0.5f) * 0.7f, _ => 1f, count: 8f, gain: 1.3f);
        }
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, _ => new Vector2(reach * 0.5f, 7f), k => Fade(k, 0.08f, 0.2f) * 0.5f, gain: 1.3f, speed: -1.5f);
        Drift(c, 30, 0f, Life - 1.1f, 1.1f, Glint, [Gold, Pearl], () => Within(Vector3.Zero, reach * 1.2f, 0.2f, 0.6f), () => Vector3.Up * Rand(1.4f, 2.6f), 0.45f, tumble: false, gain: 1.7f);
        Shine(c, Vector3.Up, r * 1.2f, Gold.Main, age => (1f + 0.2f * Mathf.Sin(age * 12f)) * Fade(age / Life, 0.08f, 0.15f));
    }

    private void GuardianSpirit(Cast c, float r)
    {
        const float Life = 3f;
        var behind = new Vector3(0f, 0f, -0.7f);
        Mark(c, 0f, Life, Filigree, Gold, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.05f, 0.15f), count: 8f, spin: 2f, gain: 1.2f);
        WingPair(c, 0.15f, Life - 0.15f, Gold, behind + Vector3.Up * 2.6f, 2.6f, 1.4f);
        Ghost(c, 0f, Life, Gold, _ => behind, 3.3f, gain: 1.7f);
        Disc(c, 0.3f, Life - 0.3f, Band, Gold, k => Lay(behind + Vector3.Up * 3.5f, 0.38f), k => Fade(k, 0.05f, 0.15f), band: 0.1f, gain: 1.9f);
        Globe(c, "energy", Hexes, 0.2f, Life - 0.2f, Gold, _ => Vector3.Up * 1f, k => 1.25f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.03f, 0.1f) * 0.7f, gain: 1.2f);
        Flare(c, 0f, 0.4f, Star, Gold, Vector3.Up * 1.5f, 3.6f);
        Disc(c, 0f, 0.5f, Shock, Gold, Vector3.Zero, k => r * 0.9f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Drift(c, 18, 0.3f, Life - 1.3f, 1.3f, Plume, [Gold, Pearl], () => new Vector3(Rand(-2f, 2f), Rand(2.4f, 3.8f), Rand(-1f, 0f)), () => new Vector3(0f, Rand(-1.4f, -0.9f), 0f), 0.45f, sway: 0.4f, slim: 0.42f);
        Twinkle(c, 20, 0.1f, Life - 0.5f, Pearl, () => Within(Vector3.Zero, r * 0.6f, 0.4f, 3.4f), 0.45f);
        Shine(c, Vector3.Up * 1.5f, r * 1.3f, Gold.Main, age => 1.2f * Fade(age / Life, 0.06f, 0.15f));
    }

    // ---- Blessings.

    private void HeartSurge(Cast c, float r)
    {
        const float Life = 2.4f;
        var reach = r * 0.4f;
        Mark(c, 0f, Life, Lotus, Rose, Vector3.Zero, _ => reach * 1.3f, k => Fade(k, 0.05f, 0.2f), count: 6f, spin: 2f, gain: 1.3f);
        Mark(c, 0f, Life, Ripples, Rose, Vector3.Zero, _ => r * 0.8f, k => Fade(k, 0.08f, 0.2f) * 0.6f, count: 3f, spin: 1f);
        Glyph(c, 0f, Life, Heart, Rose, k => Vector3.Up * (2.6f + 0.1f * Mathf.Sin(k * Life * 6f)), k => 1.1f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.05f, 0.15f), gain: 1.8f);
        Drift(c, 26, 0f, Life - 1.1f, 1.1f, Heart, [Rose, Petal, Pearl], () => Within(Vector3.Zero, reach * 1.2f, 0.2f, 0.8f), () => new Vector3(Rand(-0.3f, 0.3f), Rand(1.2f, 2.2f), Rand(-0.3f, 0.3f)), 0.42f, sway: 0.25f, tumble: false, gain: 1.7f);
        Column(c, 0f, Life, Spiral, Rose, Vector3.Zero, _ => new Vector2(reach, 2.4f), k => Fade(k, 0.06f, 0.15f) * 0.6f, twist: 1.5f, speed: 2f, gain: 1.2f);
        foreach (var at in new[] { 0f, 0.8f, 1.6f })
            Disc(c, at, 0.6f, Shock, Rose, Vector3.Zero, k => r * 0.8f * Out(k), k => (1f - k) * 0.8f, band: 0.1f);
        Twinkle(c, 18, 0f, Life - 0.4f, Pearl, () => Within(Vector3.Zero, reach * 1.4f, 0.4f, 2.6f), 0.45f);
        Shine(c, Vector3.Up, r, Rose.Main, age => (1f + 0.25f * Mathf.Sin(age * 8f)) * Fade(age / Life, 0.06f, 0.15f));
    }

    private void StarBlessing(Cast c, float r)
    {
        const float Life = 2.8f;
        var reach = r * 0.45f;
        Mark(c, 0f, Life, Galaxy, Stellar, Vector3.Zero, _ => r * 0.8f, k => Fade(k, 0.08f, 0.2f), count: 8f, spin: 2f, gain: 1.4f);
        Mark(c, 0f, Life, Pentacle, Stellar, Vector3.Zero, _ => reach, k => Fade(k, 0.05f, 0.2f), count: 5f, spin: 2f, gain: 1.3f);
        for (var i = 0; i < 5; i++)
        {
            var phase = i * Mathf.Tau / 5f;
            Func<float, Vector3> at = k => Around(phase + k * Life * 2.6f) * reach * 1.05f + Vector3.Up * (0.6f + 1.6f * k + 0.2f * Mathf.Sin(k * 14f + phase));
            Flare(c, 0f, Life, Star, Stellar, at, _ => 1f, k => Fade(k, 0.05f, 0.1f), gain: 1.6f);
            Trail(c, 0f, Life, Stellar, at, 0.22f, 0.1f, gain: 1.4f);
            var last = at(1f);
            Glyph(c, Life, 0.4f, Glint, Stellar, last, 1.8f, 1.8f);
        }
        Drift(c, 34, 0f, Life - 1.2f, 1.2f, Glint, [Stellar, Pearl, Fey], () => Within(Vector3.Zero, reach * 1.3f, 2.4f, 3.4f), () => Vector3.Down * Rand(1f, 2f), 0.5f, sway: 0.1f, tumble: false, gain: 1.8f);
        Column(c, 0f, Life, Light, Stellar, Vector3.Zero, _ => new Vector2(reach * 0.7f, 6f), k => Fade(k, 0.08f, 0.2f) * 0.4f, gain: 1.2f, speed: -1.2f);
        Shine(c, Vector3.Up, r, Stellar.Main, age => 1f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void ButterflyRenewal(Cast c, float r)
    {
        const float Life = 2.6f;
        var reach = r * 0.42f;
        Mark(c, 0f, Life, Lotus, Mint, Vector3.Zero, _ => reach * 1.4f, k => Fade(k, 0.05f, 0.2f), count: 8f, spin: 2f, gain: 1.3f);
        Column(c, 0f, Life, Spiral, Mint, Vector3.Zero, _ => new Vector2(reach * 0.9f, 2.6f), k => Fade(k, 0.06f, 0.15f) * 0.7f, twist: 1.5f, speed: 2f, gain: 1.3f);
        for (var i = 0; i < 22; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var far = reach * Rand(0.7f, 1.3f);
            var start = Rand(0f, Life - 1.2f);
            var size = Rand(0.4f, 0.6f);
            var turn = Rand(3f, 5f);
            Func<float, Vector3> at = k => Around(phase + k * turn) * far + Vector3.Up * (0.2f + 2.6f * k + 0.12f * Mathf.Sin(k * 25f + phase));
            Glyph(c, start, 1.2f, Flutter, i % 3 == 0 ? Pearl : i % 3 == 1 ? Mint : Verdant, at, _ => size, k => Fade(k, 0.15f, 0.3f), gain: 1.8f);
            if (i % 2 == 0)
                Trail(c, start, 1.2f, Mint, at, 0.12f, 0.2f, gain: 1f);
        }
        foreach (var at in new[] { 0f, 0.9f, 1.8f })
            Disc(c, at, 0.6f, Shock, Mint, Vector3.Zero, k => r * 0.75f * Out(k), k => (1f - k) * 0.8f, band: 0.1f);
        Twinkle(c, 22, 0f, Life - 0.4f, Pearl, () => Within(Vector3.Zero, reach * 1.3f, 0.3f, 2.8f), 0.45f);
        Shine(c, Vector3.Up, r, Mint.Main, age => 1f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void MoonlitVeil(Cast c, float r)
    {
        const float Life = 3f;
        var reach = r * 0.45f;
        Mark(c, 0f, Life, Corona, Moon, Vector3.Zero, _ => r * 0.85f, k => Fade(k, 0.08f, 0.2f), spin: 1.5f, gain: 1.2f);
        Mark(c, 0f, Life, Filigree, Moon, Vector3.Zero, _ => reach * 1.1f, k => Fade(k, 0.05f, 0.2f), count: 8f, spin: -2f, gain: 1.1f);
        Glyph(c, 0f, Life, Sickle, Moon, k => Vector3.Up * (2.9f + 0.1f * Mathf.Sin(k * Life * 3f)), k => 1.2f * Overshoot(Mathf.Min(k * Life / 0.35f, 1f)), k => Fade(k, 0.05f, 0.15f), gain: 1.8f);
        Globe(c, "energy", Shell, 0f, Life, Moon, _ => Vector3.Up * 1.05f, k => 1.2f * Out(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.05f, 0.15f) * (0.55f + 0.15f * Mathf.Sin(k * Life * 4f)), gain: 1.1f);
        Column(c, 0f, Life, Light, Moon, Vector3.Zero, _ => new Vector2(reach * 0.9f, 3.2f), k => Fade(k, 0.08f, 0.2f) * 0.45f, gain: 1.2f, speed: -1f);
        Drift(c, 30, 0f, Life - 1.3f, 1.3f, Glint, [Moon, Pearl], () => Within(Vector3.Up * 2.8f, reach, -0.2f, 0.2f), () => new Vector3(Rand(-0.3f, 0.3f), Rand(-1.8f, -1f), Rand(-0.3f, 0.3f)), 0.45f, sway: 0.15f, tumble: false, gain: 1.7f);
        Shine(c, Vector3.Up, r, Moon.Main, age => 0.9f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void CrystalAegis(Cast c, float r)
    {
        const float Life = 3f;
        const int Gems = 6;
        var ring = r * 0.42f;
        Mark(c, 0f, Life, Comb, Azure, Vector3.Zero, _ => ring * 1.5f, k => Fade(k, 0.05f, 0.15f), count: 9f, gain: 1.2f);
        Globe(c, "energy", Hexes, 0f, Life, Azure, _ => Vector3.Up * 1f, k => 1.3f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.03f, 0.1f) * 0.8f, gain: 1.3f);
        Disc(c, 0f, 0.45f, Shock, Azure, Vector3.Zero, k => r * 0.9f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        for (var i = 0; i < Gems; i++)
        {
            var phase = i * Mathf.Tau / Gems;
            Func<float, Vector3> at = k => Around(phase + k * Life * 1.8f) * ring * Out(Mathf.Min(k * Life / 0.3f, 1f)) + Vector3.Up * (1.1f + 0.35f * Mathf.Sin(k * Life * 3f + phase * 2f));
            Glyph(c, 0f, Life, Gem, i % 2 == 0 ? Azure : Ice, at, _ => 0.85f, k => Fade(k, 0.03f, 0.08f), gain: 1.8f);
            Flare(c, 0f, Life, Mote, Azure, at, _ => 0.9f, k => Fade(k, 0.05f, 0.1f) * 0.5f, gain: 1f);
            var last = at(1f);
            Spray(c, Life, Ice, last, 6, 3f, 2f);
            Glyph(c, Life, 0.3f, Glint, Ice, last, 1.5f, 1.7f);
        }
        Twinkle(c, 26, 0f, Life - 0.4f, Ice, () => Vector3.Up + FxKit.RandomDirection() * 1.35f, 0.5f);
        Shine(c, Vector3.Up, r, Azure.Main, age => 0.9f * Fade(age / Life, 0.05f, 0.1f));
    }
}
