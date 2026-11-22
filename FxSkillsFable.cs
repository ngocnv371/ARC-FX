using Godot;

namespace Arc.Fx;

/// <summary>
/// The sixth set's last third: the small lives of the woods, the arcane at play (cards, chess,
/// masks, music, mazes, stained glass) and the light of heaven in crosses, crowns and windows.
/// </summary>
internal sealed partial class FxSkills
{
    /// <summary>Stained glass: rose through gold to blue, for a window that should not be one colour.</summary>
    private static readonly Tone Glass = new(new(1f, 0.35f, 0.55f), new(1f, 0.9f, 0.4f), new(0.15f, 0.35f, 1f));

    private void Fable(FxSkill skill, Cast c, float r, float range)
    {
        switch (skill)
        {
            case FxSkill.DaisyChain: DaisyChain(c, r); break;
            case FxSkill.PawPrints: PawPrints(c, r); break;
            case FxSkill.OwlWatch: OwlWatch(c, r); break;
            case FxSkill.ThornMaze: ThornMaze(c, r); break;
            case FxSkill.MapleGust: MapleGust(c, r); break;
            case FxSkill.SporeCloud: SporeCloud(c, r); break;
            case FxSkill.WidowsWeb: WidowsWeb(c, r); break;
            case FxSkill.BeeSwarm: BeeSwarm(c, r); break;
            case FxSkill.FernUnfurl: FernUnfurl(c, r); break;
            case FxSkill.CardTrick: CardTrick(c, r); break;
            case FxSkill.RoyalFlush: RoyalFlush(c, r); break;
            case FxSkill.HourglassTurn: HourglassTurn(c, r); break;
            case FxSkill.Crescendo: Crescendo(c, r); break;
            case FxSkill.Lullaby: Lullaby(c, r); break;
            case FxSkill.PowerChord: PowerChord(c, r); break;
            case FxSkill.ChessGambit: ChessGambit(c, r); break;
            case FxSkill.MaskedRevel: MaskedRevel(c, r); break;
            case FxSkill.CompassRose: CompassRose(c, r); break;
            case FxSkill.Labyrinth: Labyrinth(c, r); break;
            case FxSkill.StainedGlass: StainedGlass(c, r); break;
            case FxSkill.Triquetra: Triquetra(c, r); break;
            case FxSkill.ArcaneWhorl: ArcaneWhorl(c, r); break;
            case FxSkill.StarCrown: StarCrown(c, r); break;
            case FxSkill.FortuneWheel: FortuneWheel(c, r); break;
            case FxSkill.HolyCross: HolyCross(c, r); break;
            case FxSkill.CrownOfLight: CrownOfLight(c, r); break;
            case FxSkill.RoseWindow: RoseWindow(c, r); break;
            case FxSkill.AngelChoir: AngelChoir(c, r); break;
            case FxSkill.SacredShield: SacredShield(c, r); break;
            case FxSkill.PilgrimStar: PilgrimStar(c, r); break;
            case FxSkill.BlessedRain: BlessedRain(c, r); break;
            case FxSkill.Aureole: Aureole(c, r); break;
            default: HallowedCross(c, r); break;
        }
    }

    // ---- The woods.

    private void DaisyChain(Cast c, float r)
    {
        const int Flowers = 12;
        const float Draw = 1f, Life = 3f;
        var ring = r * 0.75f;
        Mark(c, 0f, Life, Lotus, Leaf, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.05f, 0.2f), count: 6f, spin: 2f, gain: 1.2f);
        Tone[] petals = [Pearl, Honey, Petal, Pearl];
        for (var i = 0; i < Flowers; i++)
        {
            var a0 = i * Mathf.Tau / Flowers;
            var a1 = (i + 1) * Mathf.Tau / Flowers;
            var start = i * Draw / Flowers;
            var life = Life - start;
            var bob = (float)i;
            Func<float, Vector3> at = k => Around(a0 + k * life * 0.35f) * ring + Vector3.Up * (0.7f + 0.12f * Mathf.Sin(k * life * 3f + bob));
            Func<float, Vector3> next = k => Around(a1 + k * life * 0.35f) * ring + Vector3.Up * (0.7f + 0.12f * Mathf.Sin(k * life * 3f + bob + 1f));
            Glyph(c, start, life, Daisy, petals[i % petals.Length], at, k => 0.95f * Overshoot(Mathf.Min(k * life / 0.25f, 1f)), k => Fade(k, 0.02f, 0.12f), gain: 1.8f);
            Tube(c, start + 0.1f, life - 0.1f, Beam, Leaf, k => (at(k), at(k).Lerp(next(k), Mathf.Min(k * (life - 0.1f) / (Draw / Flowers), 1f))), _ => 0.03f, k => Fade(k, 0f, 0.15f), speed: 1.5f, gain: 1.5f);
            Glyph(c, start, 0.3f, Glint, Honey, at(0f), 1f, 1.6f);
        }
        Drift(c, 26, Draw, Life - 1.2f, 1.2f, Leaflet, [Pearl, Petal, Honey], () => Around(Rand(0f, Mathf.Tau)) * ring + Vector3.Up * Rand(0.5f, 1f), () => new Vector3(Rand(-0.4f, 0.4f), Rand(0.3f, 1f), Rand(-0.4f, 0.4f)), 0.28f, sway: 0.4f, slim: 0.5f);
        Disc(c, Draw, 0.5f, Shock, Leaf, Vector3.Zero, k => r * Out(k), k => (1f - k) * 0.8f, band: 0.1f);
        Twinkle(c, 24, 0.2f, Life - 0.4f, Pearl, () => Within(Vector3.Zero, r * 0.9f, 0.3f, 1.6f), 0.45f);
        Shine(c, Vector3.Up, r * 1.2f, Leaf.Main, age => 0.8f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void PawPrints(Cast c, float r)
    {
        const int Steps = 9;
        const float Run = 0.9f;
        var reach = r * 1.7f;
        Func<float, Vector3> path = s => new Vector3(0.5f * Mathf.Sin(s * 6f), 0f, 0.5f + (reach - 0.5f) * s);
        // Something unseen runs ahead: only its prints and a glow show.
        for (var i = 0; i < Steps; i++)
        {
            var s = (i + 0.5f) / Steps;
            var side = i % 2 == 0 ? 0.3f : -0.3f;
            var pad = path(s) + Vector3.Right * side;
            var when = s * Run;
            Disc(c, when, 1.2f, Glow, Honey, pad, _ => 0.26f, k => Fade(k, 0.03f, 0.5f), gain: 1.7f);
            for (var toe = 0; toe < 4; toe++)
            {
                var off = Around((toe - 1.5f) * 0.42f) * 0.36f;
                Disc(c, when, 1.2f, Glow, Gold, pad + off, _ => 0.11f, k => Fade(k, 0.03f, 0.5f), gain: 1.8f);
            }
            Mark(c, when, 0.5f, Ripples, Honey, pad, k => 0.3f + 0.7f * k, k => 1f - k, count: 2f, spin: 0f);
            for (var j = 0; j < 3; j++)
                Spark(c, when, 0.35f, Leaf, pad + Vector3.Up * 0.1f, FxKit.Jitter(1.5f) + Vector3.Up * 2.5f, Gravity, 0.05f, 0.03f);
        }
        Func<float, Vector3> beast = k => path(k) + Vector3.Up * (0.7f + 0.25f * Mathf.Abs(Mathf.Sin(k * Steps * Mathf.Pi)));
        Flare(c, 0f, Run, Mote, Honey, beast, _ => 1.8f, k => Fade(k, 0.1f, 0.1f) * 0.7f);
        Trail(c, 0f, Run, Honey, beast, 0.8f, 0.3f, gain: 1.3f);
        Glyph(c, 0f, Run, Eye, Gold, k => beast(k) + new Vector3(0f, 0.25f, 0.3f), _ => 0.6f, k => Fade(k, 0.1f, 0.1f), k => Vector3.Up, gain: 1.8f);
        var end = path(1f) + Vector3.Up * 0.9f;
        Glyph(c, Run - 0.05f, 0.45f, Claw, Honey, end, 2.6f, 2f);
        Glyph(c, Run + 0.03f, 0.4f, Claw, Gold, k => end, k => 2f + 0.6f * k, k => 1f - k, _ => new Vector3(0.6f, 1f, 0f), gain: 1.6f);
        Hit(c, Run, Honey, end, r * 0.6f);
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Honey.Main, age => 0.9f + 1.4f * Bump(age, Run, 0.5f));
    }

    private void OwlWatch(Cast c, float r)
    {
        const float Open = 0.4f, Life = 3f;
        var perch = new Vector3(0f, 3.2f, -0.4f);
        Mark(c, 0f, Life, Reticle, Honey, Vector3.Zero, _ => r, k => Fade(k, 0.04f, 0.2f), count: 12f, spin: 0.8f, gain: 1.2f);
        // Two great eyes open in the dark above, and turn together.
        Func<float, float> look = k => 0.5f * Mathf.Sin(k * Life * 1.6f);
        foreach (var side in new[] { -1f, 1f })
        {
            Glyph(c, 0f, Life, Eye, Honey, k => perch + new Vector3(side * 0.75f + 0.2f * look(k), 0f, 0f), k => 1.5f * Overshoot(Mathf.Min(k * Life / Open, 1f)), k => Fade(k, 0.03f, 0.1f) * (Mathf.Abs(k * Life % 1.4f - 1.3f) < 0.06f ? 0.1f : 1f),
                _ => new Vector3(-side * 0.25f, 1f, 0f), gain: 1.9f);
            Flare(c, 0f, Life, Mote, Honey, _ => perch + Vector3.Right * side * 0.75f, _ => 1.8f, k => Fade(k, 0.05f, 0.1f) * 0.5f, gain: 1f);
        }
        // Its gaze: a patch of pale light that sweeps the ground.
        Func<float, Vector3> gaze = k => new Vector3(r * 0.7f * look(k) * 2f, 0f, r * (0.25f + 0.35f * Mathf.Sin(k * Life * 2.3f)));
        Tube(c, Open, Life - Open, Light, Honey, k => (perch + Vector3.Down * 0.3f, gaze(k)), _ => 0.55f, k => Fade(k, 0.05f, 0.12f) * 0.35f, speed: 1f);
        Disc(c, Open, Life - Open, Glow, Honey, k => Lay(gaze(k), 1.2f), k => Fade(k, 0.05f, 0.12f) * 0.9f, gain: 1.4f);
        Mark(c, Open, Life - Open, Reticle, Gold, k => Lay(gaze(k), 0.9f, new Basis(Vector3.Up, k * 8f)), k => Fade(k, 0.05f, 0.12f), _ => 1f, count: 6f, gain: 1.3f);
        Drift(c, 14, 0.2f, Life - 1.4f, 1.4f, Plume, [Bark, Honey, Pearl], () => perch + new Vector3(Rand(-1.5f, 1.5f), Rand(-0.3f, 0.6f), 0f), () => new Vector3(0f, Rand(-1.6f, -1f), 0.3f), 0.45f, sway: 0.4f, slim: 0.42f);
        Shine(c, Vector3.Up, r * 1.2f, Honey.Main, age => 0.7f * Fade(age / Life, 0.06f, 0.12f));
    }

    private void ThornMaze(Cast c, float r)
    {
        const float Grow = 0.6f, Life = 3f;
        Mark(c, 0f, Life, Maze, Leaf, Vector3.Zero, _ => r, k => Fade(k, 0.04f, 0.2f), k => Mathf.Min(k * Life / Grow, 1f), spin: 0f, gain: 1.5f);
        // Briars stand up along the rings.
        for (var ring = 1; ring <= 4; ring++)
        {
            var far = r * ring / 5.2f * 1.2f;
            var count = 6 + ring * 5;
            for (var i = 0; i < count; i++)
            {
                if (Rand(0f, 1f) < 0.25f)
                    continue;
                var angle = i * Mathf.Tau / count + Rand(-0.05f, 0.05f);
                var foot = Around(angle) * far;
                var lean = (Vector3.Up + Around(angle + Rand(-1.5f, 1.5f)) * 0.5f).Normalized();
                var tall = Rand(0.5f, 1.1f);
                var start = Grow * ring / 4f + Rand(0f, 0.15f);
                var life = Life - start;
                Func<float, float> up = k => Out(Mathf.Min(k * life / 0.15f, 1f)) * (1f - Mathf.Clamp((k * life - life + 0.3f) / 0.3f, 0f, 1f));
                Line(c, start, life, Bark, k => foot + lean * tall * 0.5f * up(k), k => lean * tall * up(k), 0.2f, k => Fade(k, 0f, 0.08f));
                if (i % 4 == 0)
                    Glyph(c, start + 0.2f, life - 0.5f, Daisy, i % 8 == 0 ? Crimson : Rose, _ => foot + lean * tall, k => 0.45f * Overshoot(Mathf.Min(k * 6f, 1f)), k => Fade(k, 0.03f, 0.15f), gain: 1.7f);
            }
            Disc(c, Grow * ring / 4f, 0.4f, Shock, Leaf, Vector3.Zero, k => far * 1.15f * Out(k), k => (1f - k) * 0.6f, band: 0.06f);
        }
        Drift(c, 20, Grow, Life - 1.2f, 1.2f, Leaflet, [Rose, Crimson], () => Within(Vector3.Zero, r * 0.9f, 0.6f, 1.2f), () => new Vector3(Rand(-0.4f, 0.4f), Rand(-0.6f, 0.4f), Rand(-0.4f, 0.4f)), 0.26f, sway: 0.4f, slim: 0.55f);
        Cue(c, Grow, () => Stage.Patch(At(c, Vector3.Zero), r * 0.9f, Life - Grow, Leaf.Main, FxGround.Glow));
        Shine(c, Vector3.Up, r * 1.3f, Leaf.Main, age => 0.8f * Fade(age / Life, 0.1f, 0.15f));
    }

    private void MapleGust(Cast c, float r)
    {
        const float Life = 1.6f;
        var reach = r * 1.8f;
        var half = r * 0.7f;
        Tone[] autumn = [Ember, Gold, Crimson, Honey, Sun];
        for (var i = 0; i < 70; i++)
        {
            var x = Rand(-half, half);
            var high = Rand(0.3f, 2.4f);
            var start = Rand(0f, 0.7f);
            var life = Rand(0.6f, 0.9f);
            var swirl = Rand(0f, Mathf.Tau);
            var size = Rand(0.35f, 0.6f);
            var spin = Rand(4f, 10f) * (i % 2 == 0 ? 1f : -1f);
            Func<float, Vector3> at = k => new Vector3(x * (0.3f + 0.7f * k) + 0.4f * Mathf.Sin(k * 8f + swirl), high + 0.4f * Mathf.Cos(k * 7f + swirl), 0.4f + (reach - 0.4f) * k);
            Glyph(c, start, life, Leaflet, autumn[i % autumn.Length], at, _ => size, k => Fade(k, 0.1f, 0.25f), k => new Vector3(Mathf.Sin(swirl + k * spin), Mathf.Cos(swirl + k * spin), 0.3f), gain: 1.7f, slim: 0.75f);
        }
        for (var i = 0; i < 40; i++)
        {
            var x = Rand(-half, half);
            var high = Rand(0.2f, 2.4f);
            Line(c, Rand(0f, Life - 0.4f), 0.4f, i % 3 == 0 ? Pearl : Wind, k => new Vector3(x * (0.4f + 0.6f * k), high, 0.4f + (reach - 0.4f) * k), _ => Vector3.Back * 1.6f, 0.05f, k => Fade(k, 0.2f, 0.4f) * 0.8f);
        }
        Flare(c, 0f, 0.3f, Star, Honey, new Vector3(0f, 1.1f, 0.4f), 2f);
        Disc(c, 0f, 0.4f, Shock, Wind, Vector3.Zero, k => 2f * Out(k), k => 1f - k, band: 0.12f);
        Disc(c, 0.7f, 0.5f, Shock, Honey, new Vector3(0f, 0f, reach), k => r * Out(k), k => 1f - k, band: 0.12f, gain: 1.2f);
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Honey.Main, age => 0.9f * Fade(age / Life, 0.1f, 0.3f));
    }

    private void SporeCloud(Cast c, float r)
    {
        const float Puff1 = 0.3f, Life = 3.2f;
        Disc(c, 0f, Puff1, Telegraph, Toxic, Vector3.Zero, _ => r * 0.9f, _ => 0.6f, k => k);
        Cue(c, Puff1, () => Stage.Patch(At(c, Vector3.Zero), r * 0.9f, Life - Puff1, FxTheme.Of(FxElement.Poison), FxGround.Poison));
        Mark(c, Puff1, Life - Puff1, Comb, Hive, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.05f, 0.25f) * 0.7f, count: 12f, spin: 0.6f);
        // Puffballs go off round the ring, each throwing up a cloud.
        for (var i = 0; i < 7; i++)
        {
            var foot = i == 0 ? Vector3.Zero : Around(i * Mathf.Tau / 6f) * r * 0.6f;
            var when = Puff1 + i * 0.12f;
            Globe(c, "energy", Orb, 0f, when, Hive, _ => foot + Vector3.Up * 0.3f, k => 0.3f * Overshoot(Mathf.Min(k * 4f, 1f)) * (1f + 0.3f * k * k), _ => 1f, gain: 1.4f);
            Disc(c, when, 0.4f, Shock, Toxic, foot, k => 1.5f * Out(k), k => 1f - k, band: 0.12f, gain: 1.2f);
            for (var j = 0; j < 5; j++)
            {
                var way = Around(Rand(0f, Mathf.Tau)) * Rand(0.2f, 1.2f);
                var high = Rand(0.6f, 2f);
                var size = Rand(1.4f, 2.2f);
                var life = Rand(1.6f, 2.4f);
                Glyph(c, when, life, Puff, j % 2 == 0 ? Toxic : Hive, k => foot + way * Out(k) + Vector3.Up * (0.3f + high * Out(k)), k => size * (0.5f + 0.6f * Out(k)), k => Fade(k, 0.1f, 0.5f) * 0.6f, gain: 0.85f);
            }
        }
        Drift(c, 70, Puff1, Life - 1.3f, 1.3f, Glint, [Toxic, Honey, Verdant], () => Within(Vector3.Zero, r * 0.9f, 0.2f, 2f), () => new Vector3(Rand(-0.3f, 0.3f), Rand(0.1f, 0.6f), Rand(-0.3f, 0.3f)), 0.3f, sway: 0.4f, tumble: false, gain: 1.5f);
        Shine(c, Vector3.Up, r * 1.3f, Toxic.Main, age => 0.8f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void WidowsWeb(Cast c, float r)
    {
        const float Spin1 = 0.7f, Hang = 1.6f, Bite = Spin1 + Hang;
        Mark(c, 0f, Bite + 0.8f, Cobweb, Pearl, Vector3.Zero, _ => r, k => Fade(k, 0.03f, 0.25f), k => Mathf.Min(k * (Bite + 0.8f) / Spin1, 1f), count: 10f, gain: 1.6f);
        Disc(c, 0f, Bite + 0.8f, Glow, Shadow, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.25f) * 0.3f);
        // Dew on the threads.
        Twinkle(c, 50, Spin1 * 0.5f, Bite, Pearl, () => Within(Vector3.Zero, r * 0.9f, 0.05f, 0.15f), 0.4f, 0.6f);
        // Anchor threads run up and away from the rim.
        for (var i = 0; i < 5; i++)
        {
            var foot = Around(i * Mathf.Tau / 5f + 0.3f) * r * 0.95f;
            var top = foot * 1.5f + Vector3.Up * 5f;
            Line(c, i * 0.08f, Bite + 0.5f, Pearl, _ => (foot + top) * 0.5f, k => (top - foot) * Out(Mathf.Min(k * 5f, 1f)), 0.02f, k => Fade(k, 0f, 0.2f) * 0.7f);
        }
        // The spider lets herself down on a line, waits, and strikes.
        var high = Vector3.Up * 5f;
        Func<float, Vector3> body = k =>
        {
            var t = k * Bite;
            return t < Spin1 + 0.6f ? high.Lerp(Vector3.Up * 1.6f, Mathf.SmoothStep(0f, 1f, Mathf.Clamp((t - Spin1 * 0.5f) / 0.9f, 0f, 1f))) + Vector3.Up * 0.08f * Mathf.Sin(t * 5f)
                : t < Bite - 0.12f ? Vector3.Up * (1.6f + 0.08f * Mathf.Sin(t * 5f)) : Vector3.Up * Mathf.Lerp(1.6f, 0.3f, (t - Bite + 0.12f) / 0.12f);
        };
        Line(c, Spin1 * 0.5f, Bite - Spin1 * 0.5f, Pearl, k => (body((Spin1 * 0.5f + k * (Bite - Spin1 * 0.5f)) / Bite) + Vector3.Up * 6f) * 0.5f, k => Vector3.Up * (6f - body((Spin1 * 0.5f + k * (Bite - Spin1 * 0.5f)) / Bite).Y), 0.02f, _ => 0.8f);
        Globe(c, "void", 0, Spin1 * 0.5f, Bite - Spin1 * 0.5f, Void, k => body((Spin1 * 0.5f + k * (Bite - Spin1 * 0.5f)) / Bite), _ => 0.34f, _ => 1f);
        Glyph(c, Spin1 * 0.5f, Bite - Spin1 * 0.5f, Sandglass, Crimson, k => body((Spin1 * 0.5f + k * (Bite - Spin1 * 0.5f)) / Bite) + Vector3.Back * 0.36f, _ => 0.3f, _ => 1f, gain: 1.9f);
        for (var leg = 0; leg < 8; leg++)
        {
            var side = leg < 4 ? 1f : -1f;
            var lift = (leg % 4 - 1.5f) * 0.22f;
            Line(c, Spin1 * 0.5f, Bite - Spin1 * 0.5f, Shadow, k => body((Spin1 * 0.5f + k * (Bite - Spin1 * 0.5f)) / Bite) + new Vector3(side * 0.4f, lift, 0f), _ => new Vector3(side * 0.6f, lift * 1.2f - 0.15f, 0f), 0.05f, _ => 1f);
        }
        Hit(c, Bite, Toxic, Vector3.Up * 0.3f, r * 0.7f);
        Cue(c, Bite, () => Stage.Patch(At(c, Vector3.Zero), r * 0.6f, 2f, FxTheme.Of(FxElement.Poison), FxGround.Poison));
        Glyph(c, Bite, 0.4f, Claw, Toxic, Vector3.Up * 0.8f, 2.2f, 1.9f);
        Mark(c, Bite, 0.6f, Cobweb, Toxic, Vector3.Zero, k => r * (1f + 0.15f * Out(k)), k => 1f - k, _ => 1f, count: 10f, gain: 1.6f);
        Shine(c, Vector3.Up, r * 1.3f, Pearl.Main, age => 0.5f + 1.6f * Bump(age, Bite, 0.5f));
    }

    private void BeeSwarm(Cast c, float r)
    {
        const float Life = 1.8f;
        var reach = r * 1.5f;
        var hive = new Vector3(0f, 1.2f, 0.4f);
        var foe = new Vector3(0f, 1f, reach);
        Mark(c, 0f, Life, Comb, Honey, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * 0.9f * Overshoot(Mathf.Min(k * Life / 0.25f, 1f))), hive + Vector3.Forward * 0.1f), k => Fade(k, 0.03f, 0.2f), count: 7f, gain: 1.7f);
        Mark(c, 0.4f, Life - 0.4f, Comb, Honey, FxKit.Flat(foe), _ => r * 0.55f, k => Fade(k, 0.1f, 0.3f), count: 7f, gain: 1.2f);
        for (var i = 0; i < 40; i++)
        {
            var start = Rand(0f, 0.5f);
            var life = Life - start - Rand(0f, 0.2f);
            var wob = new Vector3(Rand(0f, Mathf.Tau), Rand(0f, Mathf.Tau), Rand(0f, Mathf.Tau));
            var rate = new Vector3(Rand(14f, 26f), Rand(14f, 26f), Rand(10f, 18f));
            var cloud = Rand(0.5f, 1.3f);
            // Out along the line in the first third, then round and round what they have found.
            Func<float, Vector3> at = k => hive.Lerp(foe, Mathf.SmoothStep(0f, 1f, Mathf.Min(k * 3f, 1f)))
                + new Vector3(Mathf.Sin(k * rate.X + wob.X), Mathf.Sin(k * rate.Y + wob.Y) * 0.7f, Mathf.Sin(k * rate.Z + wob.Z)) * cloud * Mathf.Min(1f, 0.3f + k * 2f);
            Flare(c, start, life, Mote, i % 4 == 0 ? Bark : Honey, at, _ => 0.2f, k => Fade(k, 0.05f, 0.1f), gain: 1.9f);
            Line(c, start, life, Honey, at, k => Heading(at, k, 0.35f), 0.05f, k => Fade(k, 0.05f, 0.1f) * 0.8f);
        }
        for (var t = 0.7f; t < Life - 0.2f; t += 0.09f)
        {
            var sting = foe + FxKit.Jitter(0.6f);
            Glyph(c, t, 0.2f, Glint, Honey, sting, 0.9f, 1.7f);
        }
        Drift(c, 12, 0.7f, Life - 0.8f, 0.8f, Glint, [Honey], () => foe + FxKit.Jitter(0.6f), () => Vector3.Down * 1.2f, 0.25f, tumble: false, gain: 1.5f);
        Shine(c, hive.Lerp(foe, 0.6f), reach, Honey.Main, age => 0.9f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void FernUnfurl(Cast c, float r)
    {
        const float Life = 3.2f, Unfurl = 0.9f;
        Mark(c, 0f, Life, Filigree, Leaf, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.05f, 0.2f) * 0.8f, count: 10f, spin: 0.8f);
        Disc(c, 0f, Life, Glow, Verdant, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.1f, 0.2f) * 0.3f);
        var spots = Spread(5, r * 0.7f).Append(Vector3.Zero).ToArray();
        for (var i = 0; i < spots.Length; i++)
        {
            var foot = spots[i];
            var start = i * 0.18f;
            var life = Life - start;
            var size = i == spots.Length - 1 ? r * 0.38f : r * Rand(0.2f, 0.28f);
            var way = i % 2 == 0 ? 1f : -1f;
            // A fiddlehead: a tight coil that loosens as it grows.
            Mark(c, start, life, Whorl, i % 2 == 0 ? Leaf : Verdant, k => Lay(foot, size * Out(Mathf.Min(k * life / Unfurl, 1f)), new Basis(Vector3.Up, way * (2f - 2f * Out(Mathf.Min(k * life / Unfurl, 1f))))), k => Fade(k, 0.03f, 0.2f),
                k => Mathf.Min(k * life / Unfurl, 1f), count: 5f, spin: 0f, gain: 1.7f);
            Column(c, start, life, Spiral, Leaf, foot, k => new Vector2(size * 0.35f, 1.8f * Out(Mathf.Min(k * life / Unfurl, 1f))), k => Fade(k, 0.05f, 0.25f) * 0.7f, twist: 2f, speed: 1.2f, gain: 1.3f);
            Glyph(c, start + Unfurl * 0.8f, 0.35f, Glint, Verdant, foot + Vector3.Up * 1.6f, 1.2f, 1.6f);
            Drift(c, 6, start + Unfurl * 0.5f, life - 1.1f, 1f, Leaflet, [Leaf, Verdant, Mint], () => Within(foot, size, 0.2f, 1.2f), () => new Vector3(Rand(-0.3f, 0.3f), Rand(0.5f, 1.2f), Rand(-0.3f, 0.3f)), 0.3f, sway: 0.3f, slim: 0.45f);
        }
        foreach (var at in new[] { 0.5f, 1.4f, 2.3f })
            Disc(c, at, 0.7f, Shock, Verdant, Vector3.Zero, k => r * Out(k), k => (1f - k) * 0.7f, band: 0.08f);
        Twinkle(c, 26, 0.3f, Life - 0.5f, Mint, () => Within(Vector3.Zero, r * 0.9f, 0.2f, 2f), 0.45f);
        Shine(c, Vector3.Up, r * 1.3f, Verdant.Main, age => 0.8f * Fade(age / Life, 0.1f, 0.2f));
    }

    // ---- The arcane at play.

    private void CardTrick(Cast c, float r)
    {
        const int Cards = 7;
        const float Flight = 0.4f, Stuck = 0.6f;
        var reach = r * 1.6f;
        var hand = new Vector3(0f, 1.3f, 0.4f);
        Flare(c, 0f, 0.25f, Star, Pearl, hand, 2f);
        for (var i = 0; i < Cards; i++)
        {
            var angle = (i - (Cards - 1) * 0.5f) * 0.17f;
            var to = Around(angle) * reach + Vector3.Up * 0.9f;
            var start = i * 0.035f;
            var spin = Rand(0f, Mathf.Tau);
            var tone = i % 2 == 0 ? Pearl : Crimson;
            Func<float, Vector3> at = k => hand.Lerp(to, k) + Vector3.Up * 0.3f * Mathf.Sin(k * Mathf.Pi);
            Glyph(c, start, Flight, Card, tone, at, _ => 1.1f, k => Fade(k, 0.05f, 0f), gain: 1.9f, slim: 0.7f,
                // Thrown flat and spinning about its own middle, tipped a little toward the way it flies.
                turn: k => new Basis(Vector3.Up, spin + k * 16f) * new Basis(Vector3.Right, -1.25f));
            Trail(c, start, Flight, tone, at, 0.25f, 0.35f, gain: 1.3f);
            var way = (to - hand).Normalized();
            Glyph(c, start + Flight, Stuck, Card, tone, _ => to + Vector3.Down * 0.4f, _ => 1.1f, k => 1f - k * k, gain: 1.7f, slim: 0.7f, sink: 0.25f,
                // Stuck in edge first, a corner buried, its faces to either side of the way it came.
                turn: _ => Face(-way * 0.6f + Vector3.Up, way.Cross(Vector3.Up)));
            Glyph(c, start + Flight, 0.3f, Glint, Pearl, to, 1.4f, 1.7f);
            Disc(c, start + Flight, 0.3f, Shock, tone, FxKit.Flat(to), k => 0.8f * Out(k), k => 1f - k, band: 0.1f);
            Spray(c, start + Flight, tone, to, 4, 2.5f, 2f, 0.35f);
        }
        Shine(c, new Vector3(0f, 1f, reach * 0.6f), reach, Pearl.Main, age => 1.4f * Bump(age, 0f, 0.9f));
    }

    private void RoyalFlush(Cast c, float r)
    {
        const int Cards = 5;
        const float Deal = 0.14f, Show = Cards * Deal + 0.45f, Fall = 0.16f, After = 0.8f;
        var spot = new Vector3(0f, 0f, r * 0.15f);
        Mark(c, 0f, Show + Cards * 0.08f + After, Checker, Gold, spot, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), count: 8f, spin: 1.5f, gain: 1.2f);
        for (var i = 0; i < Cards; i++)
        {
            var fan = (i - (Cards - 1) * 0.5f) * 0.28f;
            var high = spot + new Vector3(Mathf.Sin(fan) * 3f, 2.4f + Mathf.Cos(fan) * 1.2f, 0f);
            var land = spot + new Vector3((i - (Cards - 1) * 0.5f) * r * 0.36f, 0.6f, 0.3f * Mathf.Cos(fan * 2f));
            var start = i * Deal;
            var drop = Show + i * 0.08f;
            var tilt = new Vector3(Mathf.Sin(fan), Mathf.Cos(fan), 0f);
            // Dealt face up in a fan, each with a flash; then they come down one after another.
            Glyph(c, start, drop - start + Fall, Card, i % 2 == 0 ? Gold : Pearl, k =>
            {
                var t = k * (drop - start + Fall);
                return t < drop - start ? high + Vector3.Up * 0.06f * Mathf.Sin(t * 7f + fan * 9f) : high.Lerp(land, Mathf.Pow((t - (drop - start)) / Fall, 2f));
            }, k => 1.6f * Overshoot(Mathf.Min(k * (drop - start + Fall) / 0.18f, 1f)), _ => 1f, _ => tilt, gain: 2f, slim: 0.7f);
            Glyph(c, start, 0.3f, Glint, Pearl, high, 1.5f, 1.8f);
            Glyph(c, start + 0.1f, drop - start - 0.1f, i == Cards / 2 ? Tiara : Starlet, Gold, _ => high + Vector3.Back * 0.05f, _ => 0.6f, k => Fade(k, 0.1f, 0.05f), gain: 2f, turn: _ => Face(tilt, Vector3.Back));
            Line(c, drop, Fall, Gold, k => high.Lerp(land, k * k) + Vector3.Up * 0.8f, _ => Vector3.Up * 1.6f, 0.2f, _ => 0.7f);
            Hit(c, drop + Fall, Gold, land, 0.9f);
            Disc(c, drop + Fall, After, Cracks, Gold, FxKit.Flat(land), _ => 0.7f, k => Fade(k, 0.02f, 0.5f), k => k);
        }
        var final = Show + Cards * 0.08f + Fall;
        Cue(c, final, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.5f), r * 0.6f, Gold.Main, FxBurst.Dome));
        Column(c, final, After, Light, Gold, spot, _ => new Vector2(r * 0.4f, 9f), k => (1f - k) * (1f - k), gain: 1.7f, speed: -2f);
        for (var i = 0; i < 16; i++)
        {
            var to = Within(spot, r * 0.9f) + Vector3.Up * 0.1f;
            var size = Rand(0.25f, 0.38f);
            Func<float, Vector3> coin = k => Lob(spot + Vector3.Up * 0.6f, to, 0.7f, k);
            Glyph(c, final, 0.7f, Coin, i % 3 == 0 ? Honey : Gold, coin, _ => size, k => 1f - k * k * k, gain: 2f);
        }
        Shine(c, spot + Vector3.Up, r * 1.6f, Gold.Main, age => age < final ? 0.8f : 2.6f * Mathf.Max(0f, 1f - (age - final) / After));
    }

    private void HourglassTurn(Cast c, float r)
    {
        const float Life = 3.4f;
        var heart = new Vector3(0f, 1.1f, r * 0.1f);
        var ground = FxKit.Flat(heart);
        Mark(c, 0f, Life, Dial, Chrono, ground, _ => r, k => Fade(k, 0.04f, 0.15f), spin: 0.4f, gain: 1.4f);
        Globe(c, "energy", Hexes, 0f, Life, Chrono, _ => heart, k => r * 0.75f * Overshoot(Mathf.Min(k * Life / 0.35f, 1f)), k => Fade(k, 0.03f, 0.1f) * 0.55f, gain: 1.1f);
        Glyph(c, 0f, Life, Sandglass, Gold, k => heart + Vector3.Up * (2.4f + 0.06f * Mathf.Sin(k * Life * 2f)), k => 1.6f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.03f, 0.1f), gain: 2f);
        // Inside, time has all but stopped: grains of sand hang where they were, barely drifting.
        for (var i = 0; i < 60; i++)
        {
            var at = heart + FxKit.RandomDirection() * r * 0.7f * Mathf.Pow(Rand(0f, 1f), 0.4f);
            at = new Vector3(at.X, Mathf.Max(at.Y, 0.15f), at.Z);
            var size = Rand(0.25f, 0.5f);
            var start = Rand(0f, 0.6f);
            Glyph(c, start, Life - start, Glint, i % 3 == 0 ? Chrono : Gold, k => at + Vector3.Down * 0.15f * k, _ => size, k => Fade(k, 0.1f, 0.1f) * 0.9f, gain: 1.5f);
        }
        // Outside it the rings still come and go at speed, and slow as they cross the edge.
        for (var t = 0.2f; t < Life - 0.8f; t += 0.45f)
        {
            Disc(c, t, 0.9f, Shock, Chrono, ground, k => r * (1.3f - 1.25f * (1f - Mathf.Pow(1f - k, 3f))), k => Fade(k, 0.15f, 0.2f) * 0.8f, band: 0.06f);
            Mark(c, t, 0.9f, Ripples, Gold, ground, k => r * (1.2f - 0.9f * (1f - Mathf.Pow(1f - k, 3f))), k => Fade(k, 0.15f, 0.3f) * 0.6f, count: 2f, spin: 0f);
        }
        Flare(c, Life - 0.15f, 0.3f, Star, Gold, heart, 3.6f);
        Disc(c, Life - 0.15f, 0.5f, Shock, Gold, ground, k => r * 1.4f * Out(k), k => 1f - k, band: 0.12f, gain: 1.4f);
        Shine(c, heart, r * 1.3f, Chrono.Main, age => 0.9f * Fade(age / Life, 0.06f, 0.1f));
    }

    private void Crescendo(Cast c, float r)
    {
        const float Build = 2f, After = 0.7f;
        var reach = r * 0.5f;
        Mark(c, 0f, Build + After, Ripples, Fey, Vector3.Zero, k => r * (0.5f + 0.5f * Mathf.Min(k * (Build + After) / Build, 1f)), k => Fade(k, 0.05f, 0.3f), count: 4f, spin: 1.5f, gain: 1.3f);
        Tone[] voices = [Fey, Rose, Gold, Azure];
        // Notes climb round the caster, each larger and higher than the last.
        for (var i = 0; i < 22; i++)
        {
            var s = i / 21f;
            var start = s * (Build - 0.5f);
            var angle = i * 1.1f;
            var size = 0.4f + 0.9f * s;
            var far = reach * (0.8f + 0.5f * s);
            Func<float, Vector3> at = k => Around(angle + k * 2f) * far + Vector3.Up * (0.4f + 2.6f * s + 0.8f * k);
            Glyph(c, start, 0.7f, Note, voices[i % voices.Length], at, k => size * Overshoot(Mathf.Min(k * 4f, 1f)), k => Fade(k, 0.05f, 0.4f), gain: 1.9f);
            Disc(c, start, 0.45f, Shock, voices[i % voices.Length], Vector3.Zero, k => r * (0.3f + 0.6f * s) * Out(k), k => (1f - k) * (0.4f + 0.5f * s), band: 0.08f);
        }
        for (var i = 0; i < 5; i++)
        {
            var start = i * Build / 5f;
            Disc(c, start, Build - start, Band, voices[i % voices.Length], k => Lay(Vector3.Up * (0.3f + 0.6f * i), reach * (0.7f + 0.15f * i) * (1f + 0.04f * Mathf.Sin(k * 60f))), k => Fade(k, 0.1f, 0.05f) * 0.8f, band: 0.1f, gain: 1.5f);
        }
        // The last chord.
        Hit(c, Build, Gold, Vector3.Up * 1.2f, r);
        for (var i = 0; i < 4; i++)
            Disc(c, Build + i * 0.07f, 0.6f, Shock, voices[i], Vector3.Zero, k => r * (1.3f + 0.15f * i) * Out(k), k => 1f - k, band: 0.1f, gain: 1.5f);
        Glyph(c, Build, 0.6f, Note, Gold, Vector3.Up * 3.2f, 3f, 2f);
        Column(c, Build, After, Light, Gold, Vector3.Zero, _ => new Vector2(reach, 9f), k => (1f - k) * (1f - k), gain: 1.7f, speed: -2f);
        Twinkle(c, 24, Build, Build + 0.4f, Pearl, () => Within(Vector3.Zero, r, 0.4f, 3.4f), 0.6f);
        Shine(c, Vector3.Up, r * 1.5f, Fey.Main, age => age < Build ? 0.4f + 0.9f * age / Build : 2.6f * Mathf.Max(0f, 1f - (age - Build) / After));
    }

    private void Lullaby(Cast c, float r)
    {
        const float Life = 3.6f;
        Mark(c, 0f, Life, Ripples, Fey, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.25f) * 0.7f, count: 3f, spin: 0.3f, gain: 1.2f);
        Mark(c, 0f, Life, Galaxy, Moon, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.25f), count: 9f, spin: 0.6f, gain: 1.2f);
        Glyph(c, 0f, Life, Sickle, Moon, k => new Vector3(0f, 3.2f, 0f) + Vector3.Right * 0.3f * Mathf.Sin(k * Life * 1.2f), k => 1.6f * Overshoot(Mathf.Min(k * Life / 0.4f, 1f)), k => Fade(k, 0.05f, 0.15f),
            k => new Vector3(0.25f * Mathf.Sin(k * Life * 1.2f), 1f, 0f), gain: 1.9f);
        // Notes rock down like falling feathers; where they settle, sleep.
        for (var i = 0; i < 20; i++)
        {
            var from = Within(Vector3.Up * 3f, r * 0.8f);
            var side = Around(Rand(0f, Mathf.Tau));
            var phase = Rand(0f, Mathf.Tau);
            var start = Rand(0f, Life - 1.9f);
            var size = Rand(0.5f, 0.75f);
            Func<float, Vector3> at = k => from + Vector3.Down * 2.8f * k + side * 0.5f * Mathf.Sin(k * 7f + phase);
            Glyph(c, start, 1.9f, Note, i % 3 == 0 ? Moon : Fey, at, _ => size, k => Fade(k, 0.15f, 0.2f), k => side * 0.4f * Mathf.Cos(k * 7f + phase) + Vector3.Up, gain: 1.7f);
            var land = FxKit.Flat(at(1f));
            Mark(c, start + 1.8f, 0.8f, Ripples, Fey, land, k => 0.2f + 0.7f * k, k => (1f - k) * 0.8f, count: 2f, spin: 0f);
        }
        for (var i = 0; i < 16; i++)
        {
            var at = Within(Vector3.Zero, r * 0.85f, 0.3f, 1.2f);
            var size = Rand(1.4f, 2.2f);
            Glyph(c, Rand(0f, Life - 2f), 2f, Puff, i % 2 == 0 ? Fey : Moon, k => at + Vector3.Up * 0.3f * k, k => size, k => Fade(k, 0.3f, 0.4f) * 0.4f, gain: 0.8f);
        }
        Twinkle(c, 30, 0.3f, Life - 0.6f, Moon, () => Within(Vector3.Zero, r * 0.9f, 0.3f, 3f), 0.45f, 0.7f);
        Shine(c, Vector3.Up, r * 1.2f, Moon.Main, age => 0.6f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void PowerChord(Cast c, float r)
    {
        const float Strum = 0.25f;
        var reach = r * 1.7f;
        var mouth = new Vector3(0f, 1.2f, 0.6f);
        Flare(c, 0f, Strum, Star, Rage, _ => mouth, k => 0.6f + 2.4f * k, k => k);
        // Three walls of sound, one behind another, with jagged edges.
        Tone[] strings = [Rage, Gold, Pearl];
        for (var i = 0; i < 3; i++)
        {
            var start = Strum + i * 0.12f;
            Func<float, Vector3> at = k => mouth + Vector3.Back * reach * 0.95f * Out(k);
            Mark(c, start, 0.6f, Ripples, strings[i], k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (0.7f + 2f * k)), at(k)), k => Fade(k, 0.05f, 0.5f), count: 2f, spin: 0f, gain: 1.7f);
            Mark(c, start, 0.6f, Sunburst, strings[i], k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (0.6f + 1.8f * k)), at(k) + Vector3.Back * 0.05f), k => Fade(k, 0.05f, 0.5f) * 0.8f, _ => 1f, count: 16f, spin: 6f, gain: 1.4f);
            Disc(c, start, 0.5f, Shock, strings[i], Vector3.Zero, k => reach * 0.8f * Out(k), k => (1f - k) * 0.7f, band: 0.08f);
        }
        for (var i = 0; i < 16; i++)
        {
            var to = Around(Rand(-0.45f, 0.45f)) * reach * Rand(0.7f, 1f) + Vector3.Up * Rand(0.5f, 2.2f);
            var start = Strum + Rand(0f, 0.3f);
            var size = Rand(0.6f, 1f);
            var spin = Rand(-3f, 3f);
            Func<float, Vector3> at = k => mouth.Lerp(to, Out(k));
            Glyph(c, start, 0.6f, Note, strings[i % 3], at, _ => size, k => Fade(k, 0.05f, 0.4f), k => new Vector3(Mathf.Sin(k * spin), Mathf.Cos(k * spin), 0f), gain: 1.9f);
        }
        for (var i = 0; i < 6; i++)
            Bolt(c, Strum + i * 0.05f, 0.2f, i % 2 == 0 ? Rage : Gold, mouth, Around(Rand(-0.45f, 0.45f)) * reach * 0.9f + Vector3.Up * Rand(0.3f, 1.8f), 0.06f, 7, 0.35f);
        Hit(c, Strum, Rage, mouth, 1.2f);
        Shine(c, new Vector3(0f, 1f, reach * 0.4f), reach, Rage.Main, age => 2f * Bump(age, Strum, 0.8f));
    }

    private void ChessGambit(Cast c, float r)
    {
        const float Set = 0.7f, Move = 1.5f, After = 0.8f;
        var step = r / 4f;
        var diag = new Basis(Vector3.Up, -Mathf.Pi * 0.25f);
        Func<int, int, Vector3> square = (x, z) => diag * (new Vector3(x + 0.5f, 0f, z + 0.5f) * step);
        Mark(c, 0f, Move + After, Checker, Pearl, Vector3.Zero, _ => r, k => Fade(k, 0.04f, 0.25f), count: 8f, spin: 0.4f, gain: 1.4f);
        // White's pieces stand on their squares; the dark king waits across the board.
        (int Style, int X, int Z, float Tall)[] white = [(Tiara, -2, -2, 1.5f), (Buckler, -3, -1, 1.2f), (Rood, -1, -3, 1.3f), (Starlet, -2, 0, 1f), (Starlet, 0, -2, 1f)];
        for (var i = 0; i < white.Length; i++)
        {
            var (style, x, z, tall) = white[i];
            var foot = square(x, z);
            var start = i * 0.1f;
            var life = Move + After - start;
            Column(c, start, 0.5f, Light, Pearl, foot, k => new Vector2(step * 0.4f, 3f * Out(k)), k => 1f - k, gain: 1.5f, speed: 2f);
            Glyph(c, start, life, style, Pearl, k => foot + Vector3.Up * (tall * 0.6f + 0.05f * Mathf.Sin(k * life * 3f + i)), k => tall * Overshoot(Mathf.Min(k * life / 0.2f, 1f)), k => Fade(k, 0.02f, 0.15f), gain: 1.9f);
            Disc(c, start, life, Glow, Pearl, foot, _ => step * 0.55f, k => Fade(k, 0.05f, 0.15f) * 0.9f, gain: 1.3f);
        }
        var king = square(1, 1);
        Glyph(c, 0.3f, Move - 0.3f, Tiara, Shadow, k => king + Vector3.Up * (0.9f + 0.05f * Mathf.Sin(k * 9f)), k => 1.5f * Overshoot(Mathf.Min(k * 6f, 1f)), k => Fade(k, 0.05f, 0.02f), gain: 1.9f);
        Disc(c, 0.3f, Move - 0.3f, Glow, Shadow, king, _ => step * 0.55f, _ => 0.9f, gain: 1.3f);
        // A knight's move: a white piece leaps the corner and takes it.
        var from = square(-2, 0);
        var corner = square(0, 0);
        Func<float, Vector3> leap = k => (k < 0.6f ? from.Lerp(corner, k / 0.6f) : corner.Lerp(king, (k - 0.6f) / 0.4f)) + Vector3.Up * (0.6f + 1.6f * Mathf.Sin(k * Mathf.Pi));
        Sword3(c, Set, Move - Set, Gold, leap, _ => 1.6f, k => Fade(k, 0.1f, 0f), k => Heading(leap, k, 1f) + Vector3.Up * 0.5f);
        Trail(c, Set, Move - Set, Gold, leap, 0.4f, 0.3f, gain: 1.5f);
        Line(c, Set, Move - Set, Gold, _ => (from + corner) * 0.5f + Vector3.Up * 0.1f, k => (corner - from) * Mathf.Min(k / 0.6f, 1f), 0.12f, k => Fade(k, 0f, 0.2f));
        Line(c, Set + (Move - Set) * 0.6f, (Move - Set) * 0.4f, Gold, _ => (corner + king) * 0.5f + Vector3.Up * 0.1f, k => (king - corner) * k, 0.12f, _ => 1f);
        Hit(c, Move, Gold, king + Vector3.Up * 0.6f, r * 0.6f);
        Cue(c, Move, () => Stage.Explosion(At(c, king + Vector3.Up * 0.6f), r * 0.4f, Gold.Main, FxBurst.Dome));
        Glyph(c, Move, 0.6f, Tiara, Shadow, k => king + Vector3.Up * (0.9f - 0.5f * k) + Vector3.Right * 0.8f * k, k => 1.5f, k => 1f - k, k => new Vector3(Mathf.Sin(k * 2f), Mathf.Cos(k * 2f), 0f), gain: 1.8f);
        Shine(c, Vector3.Up, r * 1.4f, Pearl.Main, age => 0.8f + 1.6f * Bump(age, Move, 0.5f));
    }

    private void MaskedRevel(Cast c, float r)
    {
        const float Life = 3.2f;
        var ring = r * 0.55f;
        Tone[] guests = [Fey, Rose, Gold, Mint, Azure];
        Mark(c, 0f, Life, Checker, Fey, Vector3.Zero, _ => ring * 1.5f, k => Fade(k, 0.05f, 0.15f), count: 8f, spin: 2f, gain: 1.2f);
        for (var i = 0; i < guests.Length; i++)
        {
            var phase = i * Mathf.Tau / guests.Length;
            var tone = guests[i];
            var bob = (float)i;
            Func<float, Vector3> at = k => Around(phase + k * Life * 1.5f) * ring * Out(Mathf.Min(k * Life / 0.3f, 1f)) + Vector3.Up * (1.5f + 0.3f * Mathf.Sin(k * Life * 4f + bob * 2f));
            Glyph(c, 0f, Life, Mask, tone, at, k => 1.1f, k => Fade(k, 0.04f, 0.1f), gain: 1.9f,
                // Each looks outward from the ring, rocking from side to side.
                turn: k => Face(new Vector3(0f, 1f, 0f) + FxKit.Flat(at(k)).Normalized().Cross(Vector3.Up) * 0.3f * Mathf.Sin(k * Life * 4f + bob), FxKit.Flat(at(k))));
            // A streamer trails from each.
            Trail(c, 0f, Life, tone, k => at(k) + Vector3.Down * 0.5f, 0.3f, 0.12f, gain: 1.4f);
        }
        // Confetti.
        Tone[] paper = [Fey, Rose, Gold, Mint, Azure, Pearl];
        Drift(c, 90, 0f, Life - 1.3f, 1.3f, Gem, paper, () => Within(Vector3.Up * 2.8f, ring * 1.4f), () => new Vector3(Rand(-0.6f, 0.6f), Rand(-2.2f, -1.2f), Rand(-0.6f, 0.6f)), 0.22f, sway: 0.4f, slim: 0.7f, gain: 1.7f);
        for (var t = 0.3f; t < Life - 0.5f; t += 0.6f)
        {
            var at = Within(Vector3.Up * 2.4f, ring);
            Glyph(c, t, 0.35f, Glint, Pearl, at, 1.8f, 1.8f);
            for (var j = 0; j < 8; j++)
                Spark(c, t, 0.5f, paper[j % paper.Length], at, FxKit.RandomDirection() * Rand(2f, 4f), Gravity * 0.5f, 0.06f, 0.04f);
        }
        Shine(c, Vector3.Up, r, Fey.Main, age => 0.9f * Fade(age / Life, 0.05f, 0.12f));
    }

    private void CompassRose(Cast c, float r)
    {
        const float Draw = 0.5f, After = 1f;
        Mark(c, 0f, Draw + After + 0.3f, Compass, Arcane, Vector3.Zero, _ => r, k => Fade(k, 0.03f, 0.3f), spin: 0f, gain: 1.6f);
        Globe(c, "energy", Orb, 0f, Draw + After, Arcane, _ => Vector3.Up * 0.5f, k => 0.25f + 0.1f * Mathf.Sin(k * 40f), k => Fade(k, 0.05f, 0.1f), gain: 1.6f);
        // A lance of light down each long point, then each short.
        for (var i = 0; i < 8; i++)
        {
            var way = Around(i * Mathf.Tau / 8f);
            var long1 = i % 2 == 0;
            var when = Draw + (long1 ? 0f : 0.22f);
            var far = r * (long1 ? 1.5f : 0.95f);
            var from = way * 0.3f + Vector3.Up * 0.5f;
            var to = way * far + Vector3.Up * 0.5f;
            Tube(c, when, 0.6f, Beam, long1 ? Arcane : Fey, k => (from, from.Lerp(to, Out(Mathf.Min(k * 4f, 1f)))), k => (long1 ? 0.2f : 0.12f) * (1f - k * 0.7f), k => Fade(k, 0f, 0.5f), speed: 6f, gain: 1.8f);
            Line(c, when, 0.9f, Arcane, _ => (from + to) * 0.5f + Vector3.Down * 0.4f, _ => to - from, 0.3f, k => (1f - k) * 0.7f);
            Glyph(c, when + 0.12f, 0.35f, Glint, Pearl, to, long1 ? 2f : 1.3f, 1.8f);
            Mark(c, when + 0.12f, 0.4f, Rays, Arcane, FxKit.Flat(to), _ => long1 ? 1.5f : 1f, k => (1f - k) * (1f - k), k => Out(Mathf.Min(k * 2.5f, 1f)), count: 12f, gain: 1.3f);
            Spray(c, when + 0.12f, long1 ? Arcane : Fey, to, 5, 3f, 2f, 0.4f);
        }
        Flare(c, Draw, 0.35f, Star, Arcane, Vector3.Up * 0.6f, 4f);
        Disc(c, Draw, 0.5f, Shock, Arcane, Vector3.Zero, k => r * 1.5f * Out(k), k => (1f - k) * 0.6f, band: 0.06f, gain: 1.2f);
        Shine(c, Vector3.Up, r * 1.8f, Arcane.Main, age => age < Draw ? 0.6f : 2.4f * Mathf.Max(0f, 1f - (age - Draw) / After));
    }

    private void Labyrinth(Cast c, float r)
    {
        const float Draw = 0.6f, Walk = 2f, After = 0.7f;
        Mark(c, 0f, Draw + Walk + After, Maze, Arcane, Vector3.Zero, _ => r, k => Fade(k, 0.03f, 0.25f), k => Mathf.Min(k * (Draw + Walk + After) / Draw, 1f), spin: 0f, gain: 1.6f);
        Disc(c, 0f, Draw + Walk + After, Glow, Arcane, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.25f) * 0.25f);
        // A thread of light finds its way in: round a ring, a step inward, round the next.
        Func<float, Vector3> path = s =>
        {
            const int Rings = 4;
            var f = s * Rings;
            var ring = Mathf.Min((int)f, Rings - 1);
            var u = f - ring;
            var outer = r * (0.86f - 0.19f * ring);
            var inner = r * (0.86f - 0.19f * (ring + 1));
            var dir = ring % 2 == 0 ? 1f : -1f;
            var sweep = Mathf.Min(u / 0.8f, 1f) * 4.6f * dir + ring * 1.9f;
            var far = u < 0.8f ? outer : Mathf.Lerp(outer, inner, (u - 0.8f) / 0.2f);
            return Around(sweep) * far + Vector3.Up * 0.25f;
        };
        Ribbon(c, Draw, Walk + After, 0, Gold, path, k => (0f, Mathf.Min(k * (Walk + After) / Walk, 1f)), 0.16f, k => Fade(k, 0f, 0.25f), gain: 1.8f);
        Flare(c, Draw, Walk, Star, Gold, k => path(k), _ => 1.1f, _ => 1f);
        Glyph(c, Draw, Walk, Glint, Pearl, k => path(k) + Vector3.Up * 0.3f, k => 1f + 0.2f * Mathf.Sin(k * 60f), _ => 1f, gain: 1.7f);
        for (var i = 0; i < 12; i++)
        {
            var s = Rand(0.05f, 0.95f);
            Spark(c, Draw + s * Walk, 0.4f, Gold, path(s), FxKit.Jitter(1f) + Vector3.Up * 2f, Gravity * 0.6f, 0.04f, 0.03f);
        }
        // It reaches the middle.
        var found = Draw + Walk;
        Column(c, found, After, Light, Gold, Vector3.Zero, _ => new Vector2(r * 0.2f, 9f), k => (1f - k) * (1f - k), gain: 1.8f, speed: -2f);
        Hit(c, found, Gold, Vector3.Up * 0.3f, r * 0.7f);
        Mark(c, found, After, Maze, Gold, Vector3.Zero, k => r * (1f + 0.12f * Out(k)), k => 1f - k, _ => 1f, spin: 0f, gain: 1.7f);
        Twinkle(c, 20, 0.3f, found, Arcane, () => Within(Vector3.Zero, r * 0.9f, 0.2f, 1.5f), 0.45f);
        Shine(c, Vector3.Up, r * 1.5f, Arcane.Main, age => age < found ? 0.7f : 2.2f * Mathf.Max(0f, 1f - (age - found) / After));
    }

    private void StainedGlass(Cast c, float r)
    {
        const float Form = 0.6f, Life = 3.2f;
        const float Size = 2.3f;
        var at = new Vector3(0f, Size + 0.2f, -r * 0.1f);
        var face = Stand(Vector3.Back);
        Mark(c, 0f, Life, Window, Glass, k => new Transform3D(face * Basis.FromScale(Vector3.One * Size), at), k => Fade(k, 0.02f, 0.12f), k => Mathf.Min(k * Life / Form, 1f), count: 10f, spin: 0.3f, gain: 2f);
        Mark(c, 0f, Life, Rays, Gold, k => new Transform3D(face * Basis.FromScale(Vector3.One * Size * 1.5f), at + Vector3.Forward * 0.1f), k => Fade(k, 0.1f, 0.15f) * 0.4f, k => Out(Mathf.Min(k * 3f, 1f)), count: 20f, spin: 0.6f);
        // Light falls through each pane and lies on the ground in its colour.
        Tone[] panes = [Rose, Gold, Azure, Mint, Fey, Sun, Cyan, Petal, Honey, Amethyst];
        for (var i = 0; i < panes.Length; i++)
        {
            var angle = i * Mathf.Tau / panes.Length + 0.3f;
            var pane = at + new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f) * Size * 0.7f;
            var pool = new Vector3(pane.X * 1.3f, 0f, r * 0.25f + (pane.Y - 0.2f) * 0.55f);
            var start = Form + i * 0.05f;
            var life = Life - start;
            Tube(c, start, life, Light, panes[i], _ => (pane, pool), _ => 0.34f, k => Fade(k, 0.1f, 0.2f) * 0.4f, speed: 0.5f);
            Disc(c, start, life, Glow, panes[i], pool, _ => 0.85f, k => Fade(k, 0.1f, 0.2f) * 0.9f, gain: 1.4f, stretch: new Vector3(1f, 1f, 1.5f));
        }
        Mark(c, Form, Life - Form, Window, Glass, new Vector3(0f, 0f, r * 0.25f + Size * 0.55f), _ => Size * 1.25f, k => Fade(k, 0.1f, 0.2f) * 0.6f, _ => 1f, count: 10f, spin: 0.3f);
        Twinkle(c, 40, 0.2f, Life - 0.4f, Pearl, () => at + new Vector3(Rand(-Size, Size) * 0.7f, Rand(-Size, Size) * 0.7f, 0.1f), 0.5f);
        Drift(c, 30, Form, Life - 1.4f, 1.4f, Glint, panes, () => at + new Vector3(Rand(-Size, Size) * 0.6f, Rand(-Size, Size) * 0.5f, 0.2f), () => new Vector3(0f, -1.2f, 1f), 0.35f, tumble: false, gain: 1.5f);
        Shine(c, new Vector3(0f, 1f, r * 0.4f), r * 1.5f, Gold.Main, age => 0.9f * Fade(age / Life, 0.1f, 0.15f));
    }

    private void Triquetra(Cast c, float r)
    {
        const float Turn = 1.5f, After = 0.8f;
        var spot = new Vector3(0f, 0f, r * 0.1f);
        Mark(c, 0f, Turn + After, Triad, Arcane, spot, _ => r * 0.95f, k => Fade(k, 0.03f, 0.3f), spin: 4f, gain: 1.6f);
        Tone[] three = [Arcane, Azure, Gold];
        // Three orbs start at the corners and wind in to the middle, rising.
        for (var i = 0; i < 3; i++)
        {
            var phase = i * Mathf.Tau / 3f;
            var tone = three[i];
            Func<float, Vector3> at = k => spot + Around(phase + k * k * 9f) * r * 0.82f * (1f - k * k) + Vector3.Up * (0.5f + 1.3f * k);
            Globe(c, "energy", Orb, 0f, Turn, tone, at, k => 0.22f + 0.1f * k, _ => 1f, gain: 1.7f);
            Flare(c, 0f, Turn, Star, tone, at, _ => 1.2f, _ => 1f);
            Trail(c, 0f, Turn, tone, at, 0.4f, 0.25f, gain: 1.5f);
            Column(c, 0f, 0.5f, Light, tone, spot + Around(phase) * r * 0.82f, k => new Vector2(0.3f, 2.4f * Out(k)), k => 1f - k, gain: 1.5f, speed: 2f);
            // Each leaves its own arc of the knot drawn on the ground.
            Func<float, Vector3> arc = s => spot + Around(phase + s * s * 9f) * r * 0.82f * (1f - s * s) + Vector3.Up * 0.08f;
            Ribbon(c, 0f, Turn + After, 0, tone, arc, k => (0f, Mathf.Min(k * (Turn + After) / Turn, 1f)), 0.22f, k => Fade(k, 0f, 0.3f), gain: 1.5f);
        }
        var heart = spot + Vector3.Up * 1.8f;
        Hit(c, Turn, Pearl, heart, r * 0.9f);
        Cue(c, Turn, () => Stage.Explosion(At(c, heart), r * 0.6f, Arcane.Main, FxBurst.Dome));
        Tube(c, Turn, After, Beam, Pearl, _ => (spot, spot + Vector3.Up * 10f), k => 0.4f * (1f - 0.7f * k), k => Fade(k, 0f, 0.5f), speed: 6f, gain: 1.8f);
        for (var i = 0; i < 3; i++)
        {
            var foot = spot + Around(i * 2.1f) * 0.3f;
            Tube(c, Turn, After, Beam, three[i], _ => (foot, foot + Vector3.Up * 10f), k => 0.14f * (1f - 0.7f * k), k => Fade(k, 0f, 0.5f), speed: 6f, gain: 1.6f);
        }
        Mark(c, Turn, After, Triad, Pearl, spot, k => r * 0.95f * (1f + 0.2f * Out(k)), k => 1f - k, _ => 1f, spin: 4f, gain: 1.6f);
        Shine(c, heart, r * 1.7f, Arcane.Main, age => age < Turn ? 0.6f + 0.6f * age / Turn : 2.6f * Mathf.Max(0f, 1f - (age - Turn) / After));
    }

    private void ArcaneWhorl(Cast c, float r)
    {
        const float Life = 2.4f;
        Mark(c, 0f, Life, Whorl, Arcane, Vector3.Zero, _ => r, k => Fade(k, 0.04f, 0.2f), count: 8f, spin: 12f, gain: 1.6f);
        Mark(c, 0f, Life, Pentacle, Fey, Vector3.Zero, _ => r * 0.3f, k => Fade(k, 0.04f, 0.2f), count: 5f, spin: -6f, gain: 1.3f);
        Globe(c, "energy", Orb, 0f, Life, Arcane, _ => Vector3.Up * 0.8f, k => 0.3f + 0.05f * Mathf.Sin(k * 60f), k => Fade(k, 0.05f, 0.1f), gain: 1.6f);
        // Bolts thrown off the heart ride the spiral out, lift off its end and come down on the rim.
        for (var i = 0; i < 12; i++)
        {
            var phase = i * Mathf.Tau / 12f * 5f;
            var start = i * 0.13f;
            const float Ride = 0.8f;
            Func<float, Vector3> at = k => Around(phase + k * 6f) * r * (0.1f + 0.85f * k) + Vector3.Up * (0.8f - 0.5f * Mathf.Sin(k * Mathf.Pi) + 1.4f * Mathf.Pow(Mathf.Max(0f, k - 0.5f) * 2f, 2f) * (1f - k) * 4f * 0.4f);
            Flare(c, start, Ride, Star, i % 2 == 0 ? Arcane : Fey, at, _ => 0.9f, _ => 1f, gain: 1.6f);
            Trail(c, start, Ride, i % 2 == 0 ? Arcane : Fey, at, 0.3f, 0.35f, gain: 1.5f);
            var end = at(1f);
            Hit(c, start + Ride, Arcane, end, 0.7f);
            Mark(c, start + Ride, 0.5f, Pentacle, Fey, FxKit.Flat(end), _ => 0.6f, k => 1f - k, _ => 1f, count: 5f, gain: 1.4f);
        }
        Twinkle(c, 24, 0.2f, Life - 0.3f, Pearl, () => Within(Vector3.Zero, r * 0.9f, 0.2f, 1.6f), 0.45f);
        Shine(c, Vector3.Up, r * 1.5f, Arcane.Main, age => 1.2f * Fade(age / Life, 0.06f, 0.2f));
    }

    private void StarCrown(Cast c, float r)
    {
        const float Life = 3.2f;
        const int Stars = 7;
        var head = Vector3.Up * 2.5f;
        var tilt = new Basis(Vector3.Right, 0.3f);
        Mark(c, 0f, Life, Galaxy, Stellar, Vector3.Zero, _ => r * 0.6f, k => Fade(k, 0.08f, 0.2f), count: 8f, spin: 2f, gain: 1.3f);
        Disc(c, 0.2f, Life - 0.2f, Band, Stellar, k => new Transform3D(tilt * Basis.FromScale(Vector3.One * 0.62f * Out(Mathf.Min(k * Life / 0.3f, 1f))), head), k => Fade(k, 0.05f, 0.12f) * 0.8f, band: 0.1f, gain: 1.6f);
        for (var i = 0; i < Stars; i++)
        {
            var phase = i * Mathf.Tau / Stars;
            // Each flies up from the ground to its place in the ring.
            var from = Around(phase * 2.3f) * r * 0.5f + Vector3.Up * 0.2f;
            var start = i * 0.08f;
            var life = Life - start;
            Func<float, Vector3> at = k =>
            {
                var seat = head + tilt * (Around(phase + k * life * 1.3f) * 0.62f);
                return from.Lerp(seat, Out(Mathf.Min(k * life / 0.5f, 1f)));
            };
            Glyph(c, start, life, Starlet, i % 3 == 0 ? Pearl : Stellar, at, k => 0.55f + 0.08f * Mathf.Sin(k * life * 6f + phase), k => Fade(k, 0.03f, 0.1f), k => new Vector3(Mathf.Sin(k * life * 2f + phase), Mathf.Cos(k * life * 2f + phase), 0f), gain: 2f);
            Trail(c, start, 0.5f, Stellar, k => at(k * 0.5f / life), 0.2f, 0.4f, gain: 1.4f);
            Glyph(c, start + 0.5f, 0.3f, Glint, Pearl, head + tilt * (Around(phase + 0.5f * 1.3f) * 0.62f), 1.2f, 1.7f);
        }
        Column(c, 0f, Life, Light, Stellar, Vector3.Zero, _ => new Vector2(0.6f, 5f), k => Fade(k, 0.08f, 0.2f) * 0.35f, speed: -1.2f);
        Drift(c, 24, 0.6f, Life - 1.2f, 1.2f, Glint, [Stellar, Pearl], () => head + new Vector3(Rand(-0.7f, 0.7f), Rand(-0.1f, 0.3f), Rand(-0.7f, 0.7f)), () => Vector3.Down * Rand(0.8f, 1.6f), 0.4f, sway: 0.1f, tumble: false, gain: 1.7f);
        Shine(c, Vector3.Up * 1.5f, r, Stellar.Main, age => 0.9f * Fade(age / Life, 0.06f, 0.12f));
    }

    private void FortuneWheel(Cast c, float r)
    {
        const float Spin1 = 1.7f, After = 1.1f;
        const float Size = 2f;
        var hub = new Vector3(0f, Size + 0.3f, -r * 0.1f);
        var face = Stand(Vector3.Back);
        // The wheel turns fast, slows, and stops.
        Func<float, float> turned = t => 14f * (1f - Mathf.Pow(1f - Mathf.Min(t / Spin1, 1f), 3f));
        var life = Spin1 + After;
        Mark(c, 0f, life, Compass, Gold, k => new Transform3D(face * new Basis(Vector3.Up, turned(k * life)) * Basis.FromScale(Vector3.One * Size * Overshoot(Mathf.Min(k * life / 0.25f, 1f))), hub), k => Fade(k, 0.02f, 0.15f), _ => 1f, spin: 0f, gain: 1.9f);
        Mark(c, 0f, life, Gear, Honey, k => new Transform3D(face * new Basis(Vector3.Up, turned(k * life)) * Basis.FromScale(Vector3.One * Size * 1.12f), hub + Vector3.Forward * 0.04f), k => Fade(k, 0.02f, 0.15f) * 0.8f, _ => 1f, count: 16f, spin: 0f, gain: 1.5f);
        Mark(c, 0f, life, Checker, Gold, Vector3.Zero, _ => r * 0.8f, k => Fade(k, 0.05f, 0.2f), count: 8f, spin: 1f, gain: 1.1f);
        // The pointer at the top, flicked by every peg as it passes.
        Glyph(c, 0f, life, Leaflet, Crimson, k => hub + Vector3.Up * (Size + 0.25f), _ => 0.7f, k => Fade(k, 0.03f, 0.15f), k => new Vector3(0.5f * Mathf.Sin(turned(k * life) * 8f) * (1f - Mathf.Min(k * life / Spin1, 1f)), -1f, 0f), gain: 2f, slim: 0.6f);
        for (var t = 0.1f; t < Spin1; t += 0.06f + 0.12f * t / Spin1)
            Glyph(c, t, 0.15f, Glint, Pearl, hub + Vector3.Up * Size, 0.8f, 1.6f);
        // It lands on the prize.
        Flare(c, Spin1, 0.4f, Star, Gold, hub + Vector3.Up * Size, 4f);
        Mark(c, Spin1, After, Rays, Gold, k => new Transform3D(face * Basis.FromScale(Vector3.One * Size * 2f), hub + Vector3.Forward * 0.08f), k => 1f - k, k => Out(Mathf.Min(k * 3f, 1f)), count: 20f, spin: 1f, gain: 1.6f);
        Disc(c, Spin1, 0.5f, Shock, Gold, Vector3.Zero, k => r * 1.3f * Out(k), k => 1f - k, band: 0.12f, gain: 1.4f);
        for (var i = 0; i < 40; i++)
        {
            var to = Within(Vector3.Zero, r) + Vector3.Up * 0.1f;
            var size = Rand(0.25f, 0.4f);
            var start = Spin1 + Rand(0f, 0.4f);
            Func<float, Vector3> coin = k => Lob(hub, to, 0.7f, k);
            Glyph(c, start, 0.7f, Coin, i % 3 == 0 ? Honey : Gold, coin, _ => size, k => Fade(k, 0.05f, 0f), gain: 2f);
            Glyph(c, start + 0.7f, 0.25f, Glint, Pearl, to + Vector3.Up * 0.1f, 0.8f, 1.6f);
        }
        Twinkle(c, 24, 0f, life - 0.3f, Gold, () => hub + new Vector3(Rand(-Size, Size), Rand(-Size, Size), 0.1f), 0.5f);
        Shine(c, new Vector3(0f, 1f, 0.5f), r * 1.5f, Gold.Main, age => age < Spin1 ? 0.8f : 0.8f + 1.8f * Bump(age, Spin1, 0.7f));
    }

    // ---- Heaven.

    private void HolyCross(Cast c, float r)
    {
        const float Fall = 0.7f, After = 1.1f;
        var spot = new Vector3(0f, 0f, r * 0.15f);
        Func<float, float> high = k => 2.2f + 8f * (1f - k) * (1f - k);
        Mark(c, 0f, Fall + After, Sunburst, Gold, spot, _ => r * 0.8f, k => Fade(k, 0.04f, 0.4f), k => Mathf.Min(k * (Fall + After) / Fall, 1f), count: 16f, gain: 1.2f);
        Glyph(c, 0f, Fall + After, Rood, Gold, k => spot + Vector3.Up * high(Mathf.Min(k * (Fall + After) / Fall, 1f)), k => 4.2f, k => Fade(k, 0.08f, 0.35f), gain: 2f);
        Glyph(c, 0f, Fall + After, Rood, Pearl, k => spot + Vector3.Up * high(Mathf.Min(k * (Fall + After) / Fall, 1f)) + Vector3.Back * 0.05f, k => 3.4f, k => Fade(k, 0.08f, 0.35f) * 0.7f, gain: 1.6f);
        Column(c, 0f, Fall, Light, Gold, spot, k => new Vector2(0.6f, high(k) + 1f), k => 0.2f + 0.4f * k, gain: 1.3f, speed: -2f);
        // It lands, and its shape is thrown across the ground in light.
        Hit(c, Fall, Gold, spot + Vector3.Up * 0.3f, r * 0.9f);
        Cue(c, Fall, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.4f), r * 0.6f, FxTheme.Of(FxElement.Holy), FxBurst.Dome));
        Disc(c, Fall, After, Glow, Gold, spot + Vector3.Back * r * 0.15f, _ => 1f, k => (1f - k) * (1f - k), gain: 2f, stretch: new Vector3(0.4f, 1f, r * 1.1f));
        Disc(c, Fall, After, Glow, Gold, spot + Vector3.Back * r * 0.35f, _ => 1f, k => (1f - k) * (1f - k), gain: 2f, stretch: new Vector3(r * 0.75f, 1f, 0.4f));
        Column(c, Fall, After, Light, Gold, spot, _ => new Vector2(r * 0.3f, 11f), k => (1f - k) * (1f - k), gain: 1.8f, speed: -2.5f);
        Mark(c, Fall, 0.6f, Rays, Pearl, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * 4.4f), spot + Vector3.Up * 2.6f + Vector3.Forward * 0.1f), k => 1f - k, k => Out(k), count: 20f, gain: 1.6f);
        Twinkle(c, 24, 0f, Fall + 0.5f, Pearl, () => Within(spot, r * 0.7f, 0.5f, 5f), 0.55f);
        Shine(c, spot + Vector3.Up, r * 1.8f, Gold.Main, age => age < Fall ? 0.4f + 0.7f * age / Fall : 2.8f * Mathf.Max(0f, 1f - (age - Fall) / After));
    }

    private void CrownOfLight(Cast c, float r)
    {
        const float Life = 3f;
        var head = Vector3.Up * 2.2f;
        Mark(c, 0f, Life, Filigree, Gold, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.05f, 0.15f), count: 9f, spin: 2f, gain: 1.2f);
        Crown(c, 0f, Life, Gold, _ => head, gain: 2f);
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, _ => new Vector2(0.55f, 6f), k => Fade(k, 0.08f, 0.2f) * 0.45f, gain: 1.3f, speed: -1.5f);
        Drift(c, 26, 0.2f, Life - 1.2f, 1.2f, Glint, [Gold, Pearl], () => head + new Vector3(Rand(-0.7f, 0.7f), Rand(0f, 0.8f), Rand(-0.7f, 0.7f)), () => Vector3.Down * Rand(0.8f, 1.5f), 0.4f, sway: 0.1f, tumble: false, gain: 1.7f);
        Disc(c, 0.2f, 0.5f, Shock, Gold, Vector3.Zero, k => r * 0.9f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Shine(c, Vector3.Up * 1.5f, r, Gold.Main, age => 1.1f * Fade(age / Life, 0.05f, 0.12f));
    }

    private void RoseWindow(Cast c, float r)
    {
        const float Draw = 0.6f, Life = 3.2f;
        Mark(c, 0f, Life, Window, Gold, Vector3.Zero, _ => r, k => Fade(k, 0.03f, 0.2f), k => Mathf.Min(k * Life / Draw, 1f), count: 12f, spin: 0.5f, gain: 1.8f);
        Mark(c, Draw, Life - Draw, Window, Glass, k => Lay(Vector3.Up * 0.05f, r * 0.98f, new Basis(Vector3.Up, -k * 0.8f)), k => Fade(k, 0.1f, 0.25f) * 0.7f, _ => 1f, count: 12f, spin: 0f, gain: 1.3f);
        Column(c, Draw, Life - Draw, Light, Gold, Vector3.Zero, _ => new Vector2(r * 0.9f, 9f), k => Fade(k, 0.1f, 0.25f) * 0.3f, gain: 1.2f, speed: -1f);
        // Light stands up out of each pane in turn round the rose.
        Tone[] panes = [Gold, Rose, Dawn, Pearl];
        for (var i = 0; i < 12; i++)
        {
            var foot = Around((i + 0.5f) * Mathf.Tau / 12f) * r * 0.7f;
            var start = Draw + i * 0.07f;
            Column(c, start, Life - start, Light, panes[i % panes.Length], foot, k => new Vector2(r * 0.14f, 3.4f * Out(Mathf.Min(k * 6f, 1f))), k => Fade(k, 0.03f, 0.3f) * 0.8f, gain: 1.5f, speed: -2f);
            Glyph(c, start, 0.35f, Glint, Pearl, foot + Vector3.Up * 0.4f, 1.3f, 1.7f);
        }
        Disc(c, Draw, 0.6f, Shock, Gold, Vector3.Zero, k => r * 1.3f * Out(k), k => 1f - k, band: 0.1f, gain: 1.4f);
        Drift(c, 36, Draw, Life - 1.3f, 1.3f, Leaflet, [Gold, Rose, Pearl], () => Within(Vector3.Zero, r * 0.85f, 0.1f, 0.3f), () => new Vector3(Rand(-0.2f, 0.2f), Rand(0.9f, 1.9f), Rand(-0.2f, 0.2f)), 0.3f, sway: 0.3f, slim: 0.5f);
        Twinkle(c, 30, Draw, Life - 0.5f, Pearl, () => Within(Vector3.Zero, r * 0.9f, 0.2f, 3f), 0.5f);
        Shine(c, Vector3.Up, r * 1.5f, Gold.Main, age => 1.2f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void AngelChoir(Cast c, float r)
    {
        const float Life = 3.4f;
        const int Angels = 4;
        var ring = r * 0.6f;
        Mark(c, 0f, Life, Filigree, Gold, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.05f, 0.2f), count: 12f, spin: 1f, gain: 1.2f);
        Mark(c, 0f, Life, Ripples, Gold, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.08f, 0.2f) * 0.6f, count: 3f, spin: 0.8f);
        for (var i = 0; i < Angels; i++)
        {
            var phase = i * Mathf.Tau / Angels;
            var start = i * 0.12f;
            var life = Life - start;
            var bob = (float)i;
            Func<float, Vector3> at = k => Around(phase + k * life * 0.6f) * ring + Vector3.Up * (2.4f + 0.15f * Mathf.Sin(k * life * 2.5f + bob));
            // Each a small robed figure of light, winged and haloed, singing.
            Glyph(c, start, life, Wraith, Gold, at, k => 1.5f * Out(Mathf.Min(k * life / 0.3f, 1f)), k => Fade(k, 0.04f, 0.12f), gain: 1.8f, slim: 0.7f);
            // Turned to the camera with the figure, so the wings sit on its back from any side.
            FacingMark(c, start, life, Wings, Pearl, k => at(k) + Vector3.Up * 0.3f, _ => 1.15f, k => Fade(k, 0.04f, 0.12f),
                k => Out(Mathf.Min(k * life / 0.4f, 1f)) * (0.95f + 0.05f * Mathf.Sin(k * life * 6f + bob)), count: 6f, gain: 1.7f);
            Strands(c, start, life, Gold, at, 1.5f, 4);
            Disc(c, start + 0.2f, life - 0.2f, Band, Gold, k => Lay(at(k) + Vector3.Up * 0.75f, 0.2f), k => Fade(k, 0.05f, 0.12f), band: 0.2f, gain: 1.9f);
            Tube(c, start, life, Light, Gold, k => (at(k), FxKit.Flat(at(k))), _ => 0.3f, k => Fade(k, 0.1f, 0.2f) * 0.3f, speed: -1f);
            for (var n = 0; n < 6; n++)
            {
                var when = start + 0.5f + n * 0.4f;
                if (when > Life - 0.9f)
                    break;
                var from = at((when - start) / life) + Vector3.Up * 0.3f;
                var drift = new Vector3(Rand(-0.4f, 0.4f), Rand(0.7f, 1.2f), Rand(-0.2f, 0.4f));
                Glyph(c, when, 0.9f, Note, n % 2 == 0 ? Gold : Pearl, k => from + drift * k, k => 0.5f + 0.2f * k, k => Fade(k, 0.1f, 0.4f), gain: 1.8f);
            }
        }
        foreach (var at in new[] { 0.4f, 1.3f, 2.2f })
            Disc(c, at, 0.8f, Shock, Gold, Vector3.Zero, k => r * Out(k), k => (1f - k) * 0.8f, band: 0.08f, gain: 1.2f);
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, _ => new Vector2(ring * 0.5f, 8f), k => Fade(k, 0.1f, 0.2f) * 0.35f, speed: -1f);
        Twinkle(c, 26, 0.2f, Life - 0.5f, Pearl, () => Within(Vector3.Zero, r * 0.8f, 0.4f, 3.4f), 0.45f);
        Shine(c, Vector3.Up * 1.5f, r * 1.4f, Gold.Main, age => 1f * Fade(age / Life, 0.08f, 0.15f));
    }

    private void SacredShield(Cast c, float r)
    {
        const float Life = 3f;
        var ward = new Vector3(0f, 1.7f, r * 0.35f);
        var half = r * 0.8f;
        Mark(c, 0f, Life, Scales, Gold, new Vector3(0f, 0f, r * 0.35f), _ => half, k => Fade(k, 0.05f, 0.15f), count: 9f, spin: 1f, gain: 1.2f);
        Wall(c, 0.1f, Life - 0.1f, 0, Gold, new Vector3(-half, 0f, ward.Z), new Vector3(half, 0f, ward.Z), k => 3.2f * Out(Mathf.Min(k * 6f, 1f)), k => Fade(k, 0.03f, 0.12f) * 0.7f, k => Mathf.Max(0f, (k - 0.9f) / 0.1f), gain: 1.3f);
        Shield3(c, 0f, Life, k => ward + Vector3.Back * 0.1f + Vector3.Up * 0.04f * Mathf.Sin(k * Life * 3f), k => 3.2f * Overshoot(Mathf.Min(k * Life / 0.25f, 1f)), k => Fade(k, 0.03f, 0.1f));
        Mark(c, 0.2f, Life - 0.2f, Rays, Gold, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * 2.6f), ward), k => Fade(k, 0.1f, 0.15f) * 0.5f, k => Out(Mathf.Min(k * 4f, 1f)), count: 18f, spin: 1f);
        Flare(c, 0f, 0.35f, Star, Gold, ward, 4f);
        Disc(c, 0f, 0.5f, Shock, Gold, FxKit.Flat(ward), k => r * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f, stretch: new Vector3(1f, 1f, 0.5f));
        // What strikes it breaks on it.
        for (var t = 0.6f; t < Life - 0.4f; t += 0.4f)
        {
            var at = ward + new Vector3(Rand(-half, half) * 0.8f, Rand(-1.2f, 1.2f), 0.15f);
            Line(c, t - 0.12f, 0.12f, Crimson, k => at + Vector3.Back * 2.5f * (1f - k), _ => Vector3.Forward * 1.2f, 0.08f, _ => 0.9f);
            Glyph(c, t, 0.3f, Glint, Pearl, at, 1.6f, 1.8f);
            Mark(c, t, 0.4f, Ripples, Gold, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (0.3f + 1.1f * k)), at), k => 1f - k, count: 1.5f, spin: 0f, gain: 1.5f);
            for (var j = 0; j < 4; j++)
                Spark(c, t, 0.3f, Gold, at, FxKit.Jitter(3f) + Vector3.Back * 3f, Gravity * 0.5f, 0.05f, 0.04f);
        }
        Line(c, 0.1f, Life - 0.1f, Gold, _ => new Vector3(0f, 0.08f, ward.Z), _ => Vector3.Right * half * 2f, 0.4f, k => Fade(k, 0.05f, 0.12f) * 0.8f);
        Shine(c, ward, r * 1.4f, Gold.Main, age => 1f * Fade(age / Life, 0.05f, 0.1f));
    }

    private void PilgrimStar(Cast c, float r)
    {
        const float Rise = 0.6f, Life = 3.2f;
        var spot = new Vector3(0f, 0f, r * 0.45f);
        var star = spot + Vector3.Up * 4.4f;
        Mark(c, 0f, Life, Compass, Gold, spot, _ => r * 0.6f, k => Fade(k, 0.05f, 0.2f), spin: 1f, gain: 1.3f);
        // A star climbs and hangs over the place.
        Func<float, Vector3> at = k => spot.Lerp(star, Out(Mathf.Min(k * Life / Rise, 1f))) + Vector3.Up * 0.3f;
        Glyph(c, 0f, Life, Starlet, Stellar, at, k => 1.3f + 0.12f * Mathf.Sin(k * Life * 6f), k => Fade(k, 0.03f, 0.15f), k => new Vector3(Mathf.Sin(k * Life * 0.8f), Mathf.Cos(k * Life * 0.8f), 0f), gain: 2.1f);
        Glyph(c, 0f, Life, Glint, Pearl, at, k => 2.6f + 0.5f * Mathf.Sin(k * Life * 9f), k => Fade(k, 0.03f, 0.15f), gain: 1.7f);
        Flare(c, 0f, Life, Mote, Stellar, at, _ => 3f, k => Fade(k, 0.05f, 0.15f) * 0.45f, gain: 1f);
        Trail(c, 0f, Rise, Stellar, k => at(k * Rise / Life), 0.3f, 0.5f, gain: 1.4f);
        Tube(c, Rise, Life - Rise, Light, Stellar, _ => (star, spot), _ => 0.45f, k => Fade(k, 0.1f, 0.2f) * 0.5f, speed: 1f, gain: 1.3f);
        Disc(c, Rise, Life - Rise, Glow, Stellar, spot, _ => 1.4f, k => Fade(k, 0.1f, 0.2f) * 0.9f, gain: 1.4f);
        // Lights along the way to it, lit one after another from the pilgrim's feet.
        for (var i = 0; i < 7; i++)
        {
            var s = i / 7f;
            var step = new Vector3(0.25f * Mathf.Sin(i * 1.7f), 0f, -r * 0.5f + (spot.Z + r * 0.5f) * s);
            var start = Rise + i * 0.14f;
            var life = Life - start;
            Disc(c, start, life, Glow, Gold, step, _ => 0.3f, k => Fade(k, 0.05f, 0.25f), gain: 1.6f);
            Glyph(c, start, life, Wisp, Gold, k => step + Vector3.Up * 0.35f, k => 0.5f * Overshoot(Mathf.Min(k * 6f, 1f)), k => Fade(k, 0.03f, 0.25f), gain: 1.7f);
            Mark(c, start, 0.5f, Ripples, Gold, step, k => 0.2f + 0.5f * k, k => 1f - k, count: 2f, spin: 0f);
        }
        Twinkle(c, 24, Rise, Life - 0.5f, Pearl, () => spot.Lerp(star, Rand(0f, 1f)) + FxKit.Jitter(0.5f), 0.45f);
        Shine(c, spot + Vector3.Up, r * 1.4f, Stellar.Main, age => 1f * Fade(age / Life, 0.1f, 0.15f));
    }

    private void BlessedRain(Cast c, float r)
    {
        const float Life = 3.2f;
        Mark(c, 0f, Life, Sunburst, Gold, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.05f, 0.2f), count: 16f, spin: 0.8f, gain: 1.1f);
        Disc(c, 0f, Life, Glow, Gold, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.2f) * 0.3f);
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, _ => new Vector2(r * 0.9f, 6f), k => Fade(k, 0.1f, 0.2f) * 0.2f, speed: -1f);
        // Drops of light fall straight and slow; every so often one is a small cross.
        for (var i = 0; i < 80; i++)
        {
            var spot = Within(Vector3.Zero, r * 0.95f);
            var when = Rand(0f, Life - 0.9f);
            const float Fall = 0.5f;
            var holy = i % 6 == 0;
            Func<float, Vector3> at = k => spot + Vector3.Up * (0.2f + 4f * (1f - k));
            if (holy)
                Glyph(c, when, Fall, Rood, Pearl, at, _ => 0.6f, k => Fade(k, 0.15f, 0.05f), gain: 1.9f);
            else
                Line(c, when, Fall, i % 2 == 0 ? Gold : Pearl, at, _ => Vector3.Up * 0.5f, 0.05f, k => Fade(k, 0.15f, 0.05f) * 0.9f);
            Mark(c, when + Fall, 0.5f, Ripples, Gold, spot, k => (holy ? 0.3f : 0.15f) + (holy ? 0.9f : 0.5f) * k, k => 1f - k, count: 2f, spin: 0f, gain: holy ? 1.5f : 1.1f);
            if (holy)
            {
                Glyph(c, when + Fall, 0.3f, Glint, Pearl, spot + Vector3.Up * 0.2f, 1.2f, 1.7f);
                Column(c, when + Fall, 0.5f, Light, Gold, spot, k => new Vector2(0.2f, 1.8f * Out(k)), k => 1f - k, gain: 1.4f, speed: 2f);
            }
        }
        Shine(c, Vector3.Up, r * 1.4f, Gold.Main, age => 0.9f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void Aureole(Cast c, float r)
    {
        const float Life = 3.2f;
        var back = new Vector3(0f, 1.9f, -0.35f);
        var face = Stand(Vector3.Back);
        Mark(c, 0f, Life, Filigree, Gold, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.05f, 0.15f), count: 9f, spin: 2f, gain: 1.2f);
        // A sun of light standing behind the head, its rays turning slowly.
        Func<float, float> open = k => Overshoot(Mathf.Min(k * Life / 0.35f, 1f));
        Mark(c, 0f, Life, Sunburst, Gold, k => new Transform3D(face * Basis.FromScale(Vector3.One * 1.5f * open(k)), back), k => Fade(k, 0.03f, 0.12f), _ => 1f, count: 16f, spin: 2f, gain: 1.9f);
        Mark(c, 0f, Life, Rays, Pearl, k => new Transform3D(face * Basis.FromScale(Vector3.One * 2.6f * open(k)), back + Vector3.Forward * 0.05f), k => Fade(k, 0.05f, 0.15f) * 0.6f, k => Mathf.Min(k * Life / 0.5f, 1f), count: 24f, spin: 0.8f, gain: 1.3f);
        Mark(c, 0.15f, Life - 0.15f, Filigree, Gold, k => new Transform3D(face * new Basis(Vector3.Up, -k * 2f) * Basis.FromScale(Vector3.One * 0.95f), back + Vector3.Back * 0.04f), k => Fade(k, 0.05f, 0.12f), _ => 1f, count: 8f, spin: 0f, gain: 1.7f);
        Disc(c, 0f, Life, Glow, Gold, k => new Transform3D(face * Basis.FromScale(Vector3.One * 1.3f), back + Vector3.Forward * 0.08f), k => Fade(k, 0.05f, 0.12f) * 0.5f);
        Flare(c, 0f, 0.4f, Star, Gold, back, 4f);
        Disc(c, 0f, 0.5f, Shock, Gold, Vector3.Zero, k => r * 0.9f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, _ => new Vector2(0.6f, 5f), k => Fade(k, 0.08f, 0.2f) * 0.3f, speed: -1.2f);
        Twinkle(c, 30, 0.1f, Life - 0.4f, Pearl, () => back + new Vector3(Rand(-1.6f, 1.6f), Rand(-1.3f, 1.6f), 0.15f), 0.5f);
        Shine(c, Vector3.Up * 1.5f, r, Gold.Main, age => 1.1f * Fade(age / Life, 0.05f, 0.12f));
    }

    private void HallowedCross(Cast c, float r)
    {
        const float Draw = 0.5f, Life = 3.2f;
        var arm = r * 0.85f;
        Mark(c, 0f, Life, Filigree, Gold, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.04f, 0.2f), count: 8f, spin: 1.5f, gain: 1.3f);
        // Two bars of holy ground, drawn out from the middle, low walls of light along them.
        foreach (var along in new[] { Vector3.Right, Vector3.Back })
        {
            var across = along.Cross(Vector3.Up);
            Func<float, float> reach = k => arm * Out(Mathf.Min(k * Life / Draw, 1f));
            Disc(c, 0f, Life, Glow, Gold, k => new Transform3D(new Basis(across * 0.55f, Vector3.Up, along * Mathf.Max(reach(k), 0.05f)), Vector3.Up * Lift), k => Fade(k, 0.03f, 0.2f), gain: 1.9f);
            foreach (var side in new[] { -1f, 1f })
            {
                var edge = across * 0.5f * side;
                Wall(c, 0f, Life, 0, Gold, k => (edge - along * reach(k), edge + along * reach(k)), _ => 0.9f, k => Fade(k, 0.03f, 0.2f) * 0.8f, gain: 1.3f);
                Line(c, 0f, Life, Pearl, _ => edge + Vector3.Up * 0.08f, k => along * reach(k) * 2f, 0.1f, k => Fade(k, 0.03f, 0.2f));
            }
            // A cross stands up at the end of each arm.
            foreach (var end in new[] { -1f, 1f })
            {
                var tip = along * arm * end;
                Glyph(c, Draw, Life - Draw, Rood, Gold, k => tip + Vector3.Up * (0.9f + 0.05f * Mathf.Sin(k * 9f + end)), k => 1.4f * Overshoot(Mathf.Min(k * 6f, 1f)), k => Fade(k, 0.03f, 0.2f), gain: 1.9f);
                Disc(c, Draw, 0.4f, Shock, Gold, tip, k => 1.3f * Out(k), k => 1f - k, band: 0.1f, gain: 1.3f);
                Column(c, Draw, Life - Draw, Light, Gold, tip, _ => new Vector2(0.3f, 5f), k => Fade(k, 0.05f, 0.3f) * 0.5f, gain: 1.3f, speed: -1.5f);
            }
        }
        Column(c, Draw, Life - Draw, Light, Gold, Vector3.Zero, _ => new Vector2(0.6f, 10f), k => Fade(k, 0.05f, 0.3f) * 0.8f, gain: 1.6f, speed: -2f);
        Flare(c, Draw, 0.35f, Star, Gold, Vector3.Up * 0.6f, 4f);
        Disc(c, Draw, 0.5f, Shock, Gold, Vector3.Zero, k => r * 1.3f * Out(k), k => 1f - k, band: 0.12f, gain: 1.4f);
        Drift(c, 40, Draw, Life - 1.2f, 1.2f, Glint, [Gold, Pearl], () => (Rand(0f, 1f) < 0.5f ? Vector3.Right : Vector3.Back) * Rand(-arm, arm) + Vector3.Up * 0.2f, () => Vector3.Up * Rand(0.8f, 1.8f), 0.4f, tumble: false, gain: 1.7f);
        Shine(c, Vector3.Up, r * 1.6f, Gold.Main, age => 1.2f * Fade(age / Life, 0.08f, 0.2f));
    }
}
