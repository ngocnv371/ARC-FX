using Godot;

namespace Arc.Fx;

/// <summary>
/// The sixth set of skills, built on more marks (yin and yang, a compass, a gear, a maze, a
/// whorl, scales, chain, a rose window, a triad, a chequerboard) and more pictures (notes,
/// stars, crosses, claws, cards, hourglasses, masks, swords, arrows, shields, crowns, daisies).
/// This part holds the martial skills, more of the summoner's dead, and five blessings.
/// </summary>
internal sealed partial class FxSkills
{
    // More fx_mark styles.
    private const int YinYang = 16, Compass = 17, Gear = 18, Maze = 19, Whorl = 20, Scales = 21, Links = 22, Window = 23, Triad = 24, Checker = 25;
    // More fx_glyph styles.
    private const int Note = 16, Starlet = 17, Rood = 18, Claw = 19, Card = 20, Sandglass = 21, Mask = 22, Rapier = 23, Shaft = 24, Buckler = 25, Tiara = 26, Daisy = 27;

    private ArrayMesh? _shieldMesh;
    private ShaderMaterial? _aegisSteel, _aegisGlow;

    private ArrayMesh ShieldMesh => _shieldMesh ??= FxProps.Shield();
    private ShaderMaterial AegisSteel => _aegisSteel ??= FxKit.Crystal(new Color(0.55f, 0.62f, 0.76f), new Color(1f, 0.86f, 0.5f), rim: 0.6f, glow: 0.14f);
    private ShaderMaterial AegisGlow => _aegisGlow ??= FxKit.Spectral(Gold.Main);

    /// <summary>
    /// A shield: the model, steel in a shell of golden light, its face turned the way the skill
    /// goes. <paramref name="tall"/> is its height, <paramref name="at"/> its middle.
    /// </summary>
    private void Shield3(Cast c, float start, float life, Func<float, Vector3> at, Func<float, float> tall, Func<float, float> alpha)
    {
        // The model is 1.3 tall and faces -Z.
        Transform3D Place(float k, float grow) => new(new Basis(Vector3.Up, Mathf.Pi) * Basis.FromScale(Vector3.One * Mathf.Max(tall(k), 0.01f) / 1.3f * grow), at(k));
        Add(c, ShieldMesh, AegisSteel, start, life, (n, k, _) =>
        {
            n.Transform = Place(k, 1f);
            n.SetInstanceShaderParameter("uFade", Mathf.Clamp(alpha(k) * 2f, 0f, 1f));
        });
        Add(c, ShieldMesh, AegisGlow, start, life, (n, k, _) =>
        {
            n.Transform = Place(k, 1.12f);
            n.SetInstanceShaderParameter("uAlpha", Mathf.Clamp(alpha(k), 0f, 1f) * 0.8f);
            n.SetInstanceShaderParameter("uImpact", 0f);
            n.SetInstanceShaderParameter("uDissolve", 0f);
        });
    }

    private ArrayMesh? _maskMesh;
    private readonly Dictionary<Tone, ShaderMaterial> _maskSolid = [];

    private ArrayMesh MaskMesh => _maskMesh ??= FxProps.Mask();

    /// <summary>
    /// A great horned mask: the model, solid in the tone's colour inside a shell of its light,
    /// its eyes burning, its face turned the way the skill goes. <paramref name="size"/> is about
    /// its height, horns and all, and <paramref name="at"/> the middle of its face.
    /// </summary>
    private void Mask3(Cast c, float start, float life, Tone tone, Func<float, Vector3> at, Func<float, float> size, Func<float, float> alpha)
    {
        if (!_maskSolid.TryGetValue(tone, out var solid))
            _maskSolid[tone] = solid = FxKit.Crystal(tone.Deep.Lerp(tone.Main, 0.6f), tone.Hot, rim: 0.7f, glow: 0.3f);
        // The model faces -Z and is about 2 tall. It is tipped back, face half to the sky, so it shows
        // to a camera looking down as well as to whoever stands before it.
        Transform3D Place(float k, float grow) => new(new Basis(Vector3.Up, Mathf.Pi) * new Basis(Vector3.Right, 0.5f) * Basis.FromScale(Vector3.One * Mathf.Max(size(k), 0.01f) / 2f * grow), at(k));
        Add(c, MaskMesh, solid, start, life, (n, k, _) =>
        {
            n.Transform = Place(k, 1f);
            n.SetInstanceShaderParameter("uFade", Mathf.Clamp(alpha(k) * 2f, 0f, 1f));
        });
        Add(c, MaskMesh, SpectralOf(tone), start, life, (n, k, _) =>
        {
            n.Transform = Place(k, 1.06f);
            n.SetInstanceShaderParameter("uAlpha", Mathf.Clamp(alpha(k), 0f, 1f) * 0.5f);
            n.SetInstanceShaderParameter("uImpact", 0f);
            n.SetInstanceShaderParameter("uDissolve", 0f);
        });
        foreach (var side in new[] { -1f, 1f })
            Flare(c, start, life, Star, tone with { Main = tone.Hot }, k => Place(k, 1f) * new Vector3(side * 0.23f, 0.11f, -0.26f), k => size(k) * 0.2f * (1f + 0.15f * Mathf.Sin(k * life * 23f + side)), alpha, gain: 1.6f);
    }

    private void Legend(FxSkill skill, Cast c, float r, float range)
    {
        switch (skill)
        {
            case FxSkill.TigerClaws: TigerClaws(c, r); break;
            case FxSkill.SwordSaint: SwordSaint(c, r); break;
            case FxSkill.FlickerCuts: FlickerCuts(c, r); break;
            case FxSkill.CrescentKick: CrescentKick(c, r); break;
            case FxSkill.YinYangPalm: YinYangPalm(c, r); break;
            case FxSkill.ArrowStorm: ArrowStorm(c, r); break;
            case FxSkill.PiercingArrow: PiercingArrow(c, range); break;
            case FxSkill.BladeLotus: BladeLotus(c, r); break;
            case FxSkill.ShieldBash: ShieldBash(c, r); break;
            case FxSkill.ChakramReturn: ChakramReturn(c, r); break;
            case FxSkill.SwordHalo: SwordHalo(c, r); break;
            case FxSkill.MaskOfDread: MaskOfDread(c, r); break;
            case FxSkill.SpecterKnights: SpecterKnights(c, r); break;
            case FxSkill.SoulChains: SoulChains(c, r); break;
            case FxSkill.Wraithstorm: Wraithstorm(c, r); break;
            case FxSkill.SkullPyre: SkullPyre(c, r); break;
            case FxSkill.PhantomArchers: PhantomArchers(c, r); break;
            case FxSkill.SpiritPack: SpiritPack(c, r); break;
            case FxSkill.DarkCrown: DarkCrown(c, r); break;
            case FxSkill.NightmareEyes: NightmareEyes(c, r); break;
            case FxSkill.BoneWhorl: BoneWhorl(c, r); break;
            case FxSkill.DeathKnell: DeathKnell(c, r); break;
            case FxSkill.ShadowPuppets: ShadowPuppets(c, r); break;
            case FxSkill.LichForm: LichForm(c, r); break;
            case FxSkill.CrownOfVictory: CrownOfVictory(c, r); break;
            case FxSkill.BattleHymn: BattleHymn(c, r); break;
            case FxSkill.LuckyStar: LuckyStar(c, r); break;
            case FxSkill.AegisOath: AegisOath(c, r); break;
            case FxSkill.TimeSlip: TimeSlip(c, r); break;
            default: Lore(skill, c, r, range); break;
        }
    }

    /// <summary>A thing thrown from <paramref name="from"/> to land on <paramref name="to"/> after <paramref name="flight"/> seconds: where it is <paramref name="k"/> of the way.</summary>
    private static Vector3 Lob(Vector3 from, Vector3 to, float flight, float k)
    {
        var v = (to - from) / flight + Vector3.Up * 0.5f * Gravity * flight;
        var t = k * flight;
        return from + v * t + Vector3.Down * 0.5f * Gravity * t * t;
    }

    // ---- Martial.

    private void TigerClaws(Cast c, float r)
    {
        const float Second = 0.16f, Rake = 0.22f;
        Mark(c, 0f, 1f, Triad, Crit, new Vector3(0f, 0f, r * 0.45f), _ => r * 0.6f, k => Fade(k, 0.05f, 0.5f), spin: 3f, gain: 1.2f);
        for (var i = 0; i < 2; i++)
        {
            var side = i == 0 ? 1f : -1f;
            var start = i * Second;
            // One paw: its three claws cut along three ellipses about the same centre (the
            // shoulder), so the cuts fan out from one place instead of running side by side.
            var hub = new Vector3(side * 0.45f, 1.5f, -0.2f);
            var plane = new Basis(Vector3.Back, Mathf.DegToRad(side * 52f)) * new Basis(Vector3.Right, Mathf.DegToRad(-10f));
            for (var claw = 0; claw < 3; claw++)
            {
                var reach = r * (0.6f + 0.17f * claw);
                // Mirrored for the second paw, so both rake downward and across.
                var oval = plane * Basis.FromScale(new Vector3(side, 1f, 1.2f + 0.1f * claw));
                var when = start + claw * 0.012f;
                Crescent(c, when, Rake + 0.16f, new Transform3D(oval, hub), reach, 120f, Crit, thick: 0.13f, sweep: 0.45f, gain: 1.8f);
                Crescent(c, when + 0.02f, Rake + 0.16f, new Transform3D(oval, hub), reach * 0.985f, 120f, Sun, thick: 0.05f, sweep: 0.5f, gain: 1.4f);
                Glyph(c, when + Rake * 0.7f, 0.3f, Glint, Sun, hub + oval * (Around(Mathf.DegToRad(-52f)) * reach), 1f, 1.7f);
            }
            var land = hub + plane * (Vector3.Back * r * 1.05f);
            Hit(c, start + Rake * 0.6f, Crit, land, r * 0.45f);
            Disc(c, start + Rake * 0.6f, 1f, Cracks, Crit, FxKit.Flat(land), _ => r * 0.5f, k => Fade(k, 0.02f, 0.5f), k => k, stretch: new Vector3(0.5f, 1f, 1.2f), basis: new Basis(Vector3.Up, -side * 0.7f));
            for (var j = 0; j < 12; j++)
                Spark(c, start + Rand(Rake * 0.3f, Rake), Rand(0.3f, 0.6f), j % 2 == 0 ? Crit : Sun, land + FxKit.Jitter(0.5f), FxKit.Jitter(4f) + new Vector3(-side * 3f, -1f, 2f), Gravity * 0.6f, 0.07f, 0.04f);
        }
        Shine(c, new Vector3(0f, 1f, r * 0.5f), r * 1.4f, Crit.Main, age => 2f * Bump(age, Rake * 0.6f, 0.6f));
    }

    private void SwordSaint(Cast c, float r)
    {
        const int Swords = 8;
        const float Draw = 0.5f, Flight = 0.22f, After = 0.5f;
        Mark(c, 0f, Draw + Flight + After, Compass, Steel, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.04f, 0.4f), spin: 2f, gain: 1.3f);
        for (var i = 0; i < Swords; i++)
        {
            var way = Around(i * Mathf.Tau / Swords);
            var start = i * 0.04f;
            var hang = way * 0.9f + Vector3.Up * 1.5f;
            var end = way * r * 0.95f + Vector3.Up * 0.6f;
            var life = Draw - start + Flight;
            Func<float, Vector3> at = k =>
            {
                var t = k * life;
                return t < Draw - start ? hang + Vector3.Up * 0.08f * Mathf.Sin(t * 9f) : hang.Lerp(end, Mathf.Pow((t - (Draw - start)) / Flight, 2f));
            };
            // Point up while they hang, then swung down to point the way they fly.
            var dive = (way * 2f + Vector3.Down * 0.9f).Normalized();
            Func<float, Vector3> up = k => Vector3.Up.Lerp(dive, Mathf.SmoothStep(0f, 1f, Mathf.Clamp((k * life - (Draw - start) + 0.2f) / 0.24f, 0f, 1f)));
            Sword3(c, start, life, i % 2 == 0 ? Steel : Pearl, at, k => 1.7f * Out(Mathf.Min(k * life / 0.15f, 1f)), k => Mathf.Min(k / 0.03f, 1f), up);
            Glyph(c, start, 0.25f, Glint, Pearl, hang + Vector3.Up * 0.7f, 1.1f, 1.7f);
            Line(c, Draw, Flight, Steel, k => hang.Lerp(end, k * k * 0.6f), _ => way * 1.6f, 0.1f, k => k);
            Hit(c, Draw + Flight, Steel, end, 0.7f);
            // Stuck where it landed, still pointing the way it flew.
            Sword3(c, Draw + Flight, After, i % 2 == 0 ? Steel : Pearl, _ => end, _ => 1.7f, k => 1f - k * k, _ => dive);
        }
        Disc(c, Draw, 0.45f, Shock, Steel, Vector3.Zero, k => r * 1.2f * Out(k), k => 1f - k, band: 0.12f, gain: 1.4f);
        Flare(c, Draw - 0.05f, 0.3f, Star, Pearl, Vector3.Up * 1.4f, 3.4f);
        Shine(c, Vector3.Up, r * 1.5f, Steel.Main, age => age < Draw ? 0.6f : 2.2f * Mathf.Max(0f, 1f - (age - Draw) / 0.6f));
    }

    private void FlickerCuts(Cast c, float r)
    {
        const float Flurry = 0.6f, Pause = 0.2f;
        var heart = new Vector3(0f, 1.1f, r * 0.3f);
        var burst = Flurry + Pause;
        Mark(c, 0f, burst + 0.6f, Triad, Steel, FxKit.Flat(heart), _ => r * 0.6f, k => Fade(k, 0.04f, 0.4f), spin: 4f, gain: 1.2f);
        var cuts = new (Vector3 At, Vector3 Way)[30];
        for (var i = 0; i < cuts.Length; i++)
        {
            var way = FxKit.RandomDirection();
            way = new Vector3(way.X, way.Y * 0.5f, way.Z).Normalized();
            var at = heart + FxKit.Jitter(0.5f);
            cuts[i] = (at, way);
            var start = i * Flurry / cuts.Length;
            Line(c, start, 0.1f, Pearl, _ => at, k => way * 3.4f * Out(k), 0.14f, k => 1f - k * k);
            Line(c, start, burst - start, Steel, _ => at, _ => way * 3.4f, 0.02f, k => Fade(k, 0f, 0.1f) * 0.8f);
        }
        // Stillness, then every cut opens at once.
        foreach (var (at, way) in cuts)
        {
            Line(c, burst, 0.25f, Pearl, _ => at, k => way * (3.4f + 1.5f * k), 0.2f, k => (1f - k) * (1f - k));
            Glyph(c, burst, 0.3f, Glint, Pearl, at + way * Rand(-1.6f, 1.6f), 1f, 1.7f);
        }
        Hit(c, burst, Steel, heart, r * 0.7f);
        Globe(c, "energy", Shell, burst, 0.3f, Steel, _ => heart, k => 1.2f + 1.2f * Out(k), k => (1f - k) * 0.5f);
        for (var i = 0; i < 30; i++)
            Spark(c, burst, Rand(0.3f, 0.6f), i % 3 == 0 ? Crimson : Steel, heart, FxKit.RandomDirection() * Rand(3f, 8f), Gravity * 0.6f, 0.06f, 0.04f);
        Shine(c, heart, r * 1.4f, Steel.Main, age => age < burst ? 0.6f : 2.4f * Mathf.Max(0f, 1f - (age - burst) / 0.5f));
    }

    private void CrescentKick(Cast c, float r)
    {
        const float Life = 0.36f;
        Mark(c, 0f, 1.1f, YinYang, Pearl, Vector3.Zero, _ => r * 0.55f, k => Fade(k, 0.05f, 0.5f), spin: 9f, gain: 1.2f);
        Crescent(c, 0f, Life, new Transform3D(new Basis(Vector3.Back, Mathf.DegToRad(8f)), Vector3.Up * 0.6f), r, 250f, Sun, thick: 0.4f, sweep: 0.36f, gain: 1.6f);
        Crescent(c, 0.03f, Life, new Transform3D(new Basis(Vector3.Back, Mathf.DegToRad(8f)), Vector3.Up * 0.75f), r * 0.84f, 250f, Pearl, thick: 0.16f, sweep: 0.4f, gain: 1.3f);
        Disc(c, 0.05f, 0.45f, Shock, Sun, Vector3.Zero, k => r * 1.15f * Out(k), k => 1f - k, band: 0.12f, gain: 1.4f);
        Drift(c, 16, 0.05f, 0.2f, 0.7f, Puff, [Dust], () => Around(Rand(-2f, 2f)) * r * Rand(0.6f, 0.9f) + Vector3.Up * 0.3f, () => Vector3.Up * 0.6f, 1.2f, tumble: false, gain: 0.6f);
        for (var i = 0; i < 24; i++)
        {
            var way = Around(Rand(-2.1f, 2.1f));
            Spark(c, Rand(0.02f, 0.16f), Rand(0.3f, 0.5f), Sun, way * r * 0.9f + Vector3.Up * 0.6f, way * Rand(3f, 6f) + Vector3.Up * Rand(1f, 4f), Gravity * 0.6f, 0.06f, 0.04f);
        }
        Twinkle(c, 10, 0.05f, 0.3f, Pearl, () => Around(Rand(-2f, 2f)) * r * Rand(0.7f, 1f) + Vector3.Up * Rand(0.4f, 1f), 0.6f, 0.3f);
        Shine(c, Vector3.Up, r * 1.4f, Sun.Main, age => 1.8f * Mathf.Max(0f, 1f - age / 0.6f));
    }

    private void YinYangPalm(Cast c, float r)
    {
        const float Gather = 0.6f, Flight = 0.35f;
        var reach = r * 1.5f;
        var hands = new Vector3(0f, 1.1f, 0.7f);
        var foe = new Vector3(0f, 1.1f, reach);
        Mark(c, 0f, Gather + Flight + 0.5f, YinYang, Pearl, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.04f, 0.3f), spin: 6f, gain: 1.3f);
        // Light and dark circle each other between the hands, closing.
        for (var i = 0; i < 2; i++)
        {
            var phase = i * Mathf.Pi;
            Func<float, Vector3> at = k => hands + new Vector3(Mathf.Cos(phase + k * 14f), Mathf.Sin(phase + k * 14f), 0f) * 0.6f * (1f - k * 0.9f);
            if (i == 0)
                Globe(c, "energy", Orb, 0f, Gather, Pearl, at, _ => 0.2f, _ => 1f, gain: 1.7f);
            else
                Globe(c, "void", 0, 0f, Gather, Void, at, _ => 0.22f, _ => 1f);
            Trail(c, 0f, Gather, i == 0 ? Pearl : Shadow, at, 0.3f, 0.2f, gain: 1.4f);
        }
        Func<float, Vector3> ball = k => hands.Lerp(foe, k);
        Globe(c, "energy", Orb, Gather, Flight, Pearl, ball, _ => 0.4f, _ => 1f, gain: 1.7f);
        Mark(c, Gather, Flight, YinYang, Pearl, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * 0.75f), ball(k) + Vector3.Back * 0.42f), _ => 1f, _ => 1f, spin: 20f, gain: 1.8f);
        Trail(c, Gather, Flight, Pearl, ball, 0.8f, 0.4f, gain: 1.5f);
        for (var i = 0; i < 5; i++)
        {
            var ring = ball(i / 5f);
            Mark(c, Gather + i * 0.06f, 0.4f, Ripples, i % 2 == 0 ? Pearl : Shadow, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (0.5f + 1.4f * k)), ring), k => 1f - k, count: 1.5f, spin: 0f, gain: 1.5f);
        }
        Disc(c, Gather, 0.4f, Shock, Pearl, Vector3.Zero, k => 2.2f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Hit(c, Gather + Flight, Pearl, foe, r * 0.7f);
        Mark(c, Gather + Flight, 0.6f, YinYang, Pearl, FxKit.Flat(foe), k => r * 0.6f * (1f + 0.3f * Out(k)), k => 1f - k, _ => 1f, spin: 12f, gain: 1.6f);
        Cue(c, Gather + Flight, () => Stage.Explosion(At(c, foe), r * 0.5f, Pearl.Main, FxBurst.Dome));
        Shine(c, hands.Lerp(foe, 0.5f), reach, Pearl.Main, age => 0.8f + 1.6f * Bump(age, Gather + Flight, 0.5f));
    }

    private void ArrowStorm(Cast c, float r)
    {
        const int Arrows = 18;
        const float Flight = 0.5f, Stuck = 0.7f;
        var bow = new Vector3(0f, 1.5f, 0.3f);
        var zone = new Vector3(0f, 0f, r * 1.2f);
        Mark(c, 0f, Flight + 0.9f, Reticle, Gold, zone, _ => r * 0.6f, k => Fade(k, 0.04f, 0.3f), count: 10f, spin: 2f, gain: 1.3f);
        Flare(c, 0f, 0.25f, Star, Gold, bow, 2f);
        for (var i = 0; i < Arrows; i++)
        {
            var start = Rand(0f, 0.3f);
            var land = Within(zone, r * 0.55f);
            // Loosed almost straight up: a tall arc into the sky and down onto the mark.
            var apex = Rand(6.5f, 8.5f);
            Func<float, Vector3> at = k => bow.Lerp(land, k * k * (3f - 2f * k)) + Vector3.Up * apex * 4f * k * (1f - k);
            Glyph(c, start, Flight, Shaft, i % 3 == 0 ? Pearl : Gold, at, _ => 1.3f, k => Fade(k, 0.05f, 0f), k => Heading(at, k, 1f), gain: 1.8f, slim: 0.5f);
            Line(c, start, Flight, Gold, k => at(Mathf.Max(k - 0.06f, 0f)), k => Heading(at, k, 0.9f), 0.05f, _ => 0.6f);
            var dive = Heading(at, 1f, 1f);
            // Buried a quarter to half its length: only the tail end stands out of the ground, the
            // picture set so that where it is cut off sits exactly on the spot it struck.
            var sunk = Rand(0.25f, 0.5f);
            Glyph(c, start + Flight, Stuck, Shaft, Gold, _ => land - dive * 1.3f * (0.5f - sunk), _ => 1.3f, k => 1f - k * k, _ => dive, gain: 1.5f, slim: 0.5f, sink: sunk);
            Disc(c, start + Flight, 0.3f, Shock, Gold, land, k => 0.7f * Out(k), k => 1f - k, band: 0.1f);
            Spray(c, start + Flight, Gold, land + Vector3.Up * 0.1f, 3, 2f, 3f, 0.35f);
        }
        Shine(c, zone + Vector3.Up, r, Gold.Main, age => 1.2f * Fade(age / (Flight + 0.8f), 0.3f, 0.3f));
    }

    private void PiercingArrow(Cast c, float range)
    {
        const float Draw = 0.4f, Flight = 0.3f;
        var length = Mathf.Max(range, 2f);
        var bow = new Vector3(0f, 1.2f, 0.5f);
        var end = new Vector3(0f, 1.2f, length + 2f);
        Glyph(c, 0f, Draw, Shaft, Gold, k => bow + Vector3.Forward * 0.4f * k, _ => 2.2f, k => 0.4f + 0.6f * k, _ => Vector3.Back, gain: 1.8f, slim: 0.5f);
        Flare(c, 0f, Draw, Star, Gold, _ => bow + Vector3.Back * 1.1f, k => 0.5f + 2f * k, k => k);
        Mark(c, 0f, Draw + 0.1f, Reticle, Gold, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (1.3f - 0.6f * k)), bow + Vector3.Back * 1.1f), k => Fade(k, 0.1f, 0.1f), count: 8f, spin: 6f, gain: 1.6f);
        Func<float, Vector3> at = k => bow.Lerp(end, k);
        // The arrow's point rides the path; its picture is centred half its length behind.
        Glyph(c, Draw, Flight, Shaft, Pearl, k => at(k) + Vector3.Forward * 1.3f, _ => 2.6f, _ => 1f, _ => Vector3.Back, gain: 2f, slim: 0.5f);
        Trail(c, Draw, Flight, Gold, k => at(k) + Vector3.Forward * 2.5f, 0.4f, 0.5f, gain: 1.6f);
        Line(c, Draw, Flight + 0.25f, Pearl, _ => bow.Lerp(end, 0.5f), _ => (end - bow), 0.12f, k => (1f - k) * (1f - k));
        for (var i = 0; i < 7; i++)
        {
            var s = (i + 0.5f) / 7f;
            Mark(c, Draw + s * Flight, 0.45f, Ripples, Gold, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (0.3f + 1.5f * k)), at(s)), k => 1f - k, count: 1.5f, spin: 0f, gain: 1.5f);
            Disc(c, Draw + s * Flight, 0.4f, Shock, Gold, FxKit.Flat(at(s)), k => 1.2f * Out(k), k => (1f - k) * 0.7f, band: 0.1f);
        }
        Hit(c, Draw + Flight * (length / (length + 2f)), Gold, new Vector3(0f, 1.2f, length), 1.1f);
        Shine(c, bow.Lerp(end, 0.5f), length, Gold.Main, age => age < Draw ? 0.5f * age / Draw : 2f * Mathf.Max(0f, 1f - (age - Draw) / 0.6f));
    }

    private void BladeLotus(Cast c, float r)
    {
        const float Open = 0.5f, Spin = 1.1f, Life = 1.5f;
        const int Blades = 8;
        Mark(c, 0f, Life + 0.4f, Lotus, Steel, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.04f, 0.3f), count: 8f, spin: 2f, gain: 1.3f);
        for (var layer = 0; layer < 2; layer++)
        {
            for (var i = 0; i < Blades; i++)
            {
                var a0 = (i + layer * 0.5f) * Mathf.Tau / Blades;
                var long1 = r * (layer == 0 ? 0.95f : 0.65f);
                var high = 0.5f + layer * 0.5f;
                var turn = layer == 0 ? 1f : -1f;
                // Opens like a flower, then the whole bloom spins and flies apart.
                Func<float, float> angle = k => a0 + turn * 5f * Mathf.Pow(Mathf.Max(0f, k * Life - Open) / (Life - Open), 2f);
                Func<float, float> far = k => 0.55f * Overshoot(Mathf.Min(k * Life / Open, 1f)) + 0.9f * Mathf.Pow(Mathf.Max(0f, (k * Life - Spin) / (Life - Spin)), 2f);
                Sword3(c, layer * 0.1f, Life - layer * 0.1f, layer == 0 ? Steel : Pearl, k => Around(angle(k)) * long1 * far(k) + Vector3.Up * high, _ => long1 * 1.15f, k => Fade(k, 0.05f, 0.12f),
                    k => Around(angle(k)) + Vector3.Up * 0.25f * layer);
            }
        }
        Flare(c, 0f, Life, Star, Pearl, _ => Vector3.Up * 0.8f, k => 1.4f + 0.4f * Mathf.Sin(k * 30f), k => Fade(k, 0.05f, 0.15f));
        Disc(c, Open * 0.6f, 0.45f, Shock, Steel, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.1f, gain: 1.2f);
        Disc(c, Spin, 0.5f, Shock, Steel, Vector3.Zero, k => r * 1.5f * Out(k), k => 1f - k, band: 0.12f, gain: 1.5f);
        Spray(c, Spin, Steel, Vector3.Up * 0.6f, 36, 8f, 3f, 0.5f);
        Twinkle(c, 24, 0.2f, Life, Pearl, () => Within(Vector3.Zero, r, 0.4f, 1.6f), 0.6f);
        Shine(c, Vector3.Up, r * 1.5f, Steel.Main, age => 1f + 1.2f * Bump(age, Spin, 0.5f));
    }

    private void ShieldBash(Cast c, float r)
    {
        const float Rush = 0.18f, After = 0.9f;
        var from = new Vector3(0f, 1.1f, 0.5f);
        var to = new Vector3(0f, 1.1f, r * 0.95f);
        Func<float, Vector3> at = k => from.Lerp(to, Out(Mathf.Min(k * (Rush + 0.3f) / Rush, 1f)));
        Shield3(c, 0f, Rush + 0.3f, at, k => 2.2f + 0.5f * Mathf.Min(k * 3f, 1f), k => Fade(k, 0.05f, 0.4f));
        for (var i = 0; i < 10; i++)
        {
            var off = new Vector3(Rand(-0.9f, 0.9f), Rand(-0.7f, 0.9f), 0f);
            Line(c, 0f, Rush + 0.1f, Steel, k => at(k * 0.9f) + off + Vector3.Forward * 0.9f, _ => Vector3.Back * 1.6f, 0.05f, k => Fade(k, 0.1f, 0.4f));
        }
        Hit(c, Rush, Steel, to, r * 0.7f);
        Mark(c, Rush, 0.5f, Rays, Gold, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * 2.6f), to + Vector3.Back * 0.2f), k => 1f - k, k => Out(k), count: 14f, gain: 1.6f);
        // Stars wheel over whoever was struck.
        var dazed = to + new Vector3(0f, 1.2f, 0.7f);
        Disc(c, Rush + 0.1f, After, Band, Gold, k => Lay(dazed, 0.6f), k => Fade(k, 0.1f, 0.3f) * 0.6f, band: 0.1f);
        for (var i = 0; i < 4; i++)
        {
            var phase = i * Mathf.Tau / 4f;
            Glyph(c, Rush + 0.1f, After, Starlet, Gold, k => dazed + Around(phase + k * After * 6f) * 0.6f, _ => 0.5f, k => Fade(k, 0.1f, 0.3f), k => new Vector3(Mathf.Sin(k * 9f + phase), Mathf.Cos(k * 9f + phase), 0f), gain: 1.9f);
        }
        Shine(c, to, r * 1.3f, Steel.Main, age => 2f * Bump(age, Rush, 0.5f));
    }

    private void ChakramReturn(Cast c, float r)
    {
        const float Life = 1.6f;
        var reach = r * 2.5f;
        // A long narrow loop: out along one side, round the far end, and home along the other.
        Func<float, Vector3> at = k => new Vector3(1.7f * Mathf.Sin(k * Mathf.Tau), 1f, 0.6f + (reach - 0.6f) * (0.5f - 0.5f * Mathf.Cos(k * Mathf.Tau)));
        Mark(c, 0f, Life, Sunburst, Gold, k => Lay(at(k), 0.95f, new Basis(Vector3.Up, k * Life * 34f)), k => Fade(k, 0.04f, 0.06f), _ => 1f, count: 10f, gain: 1.9f);
        Disc(c, 0f, Life, Band, Pearl, k => Lay(at(k), 0.6f), k => Fade(k, 0.04f, 0.06f), band: 0.16f, gain: 1.8f);
        Flare(c, 0f, Life, Mote, Gold, at, _ => 1.6f, k => Fade(k, 0.04f, 0.06f) * 0.6f);
        Trail(c, 0f, Life, Gold, at, 1.3f, 0.13f, gain: 1.5f);
        Trail(c, 0f, Life, Pearl, at, 0.4f, 0.18f, gain: 1.3f);
        Disc(c, 0f, Life, Glow, Gold, k => Lay(FxKit.Flat(at(k)), 1.1f), _ => 0.5f);
        for (var i = 0; i < 9; i++)
        {
            var k0 = 0.12f + i * 0.09f;
            var spot = at(k0);
            Glyph(c, k0 * Life, 0.3f, Glint, Pearl, spot + FxKit.Jitter(0.3f), 1.3f, 1.7f);
            Crescent(c, k0 * Life, 0.2f, new Transform3D(new Basis(Vector3.Up, Rand(0f, Mathf.Tau)), spot), 1.1f, 140f, Gold, thick: 0.3f, sweep: 0.4f, gain: 1.3f);
            for (var j = 0; j < 3; j++)
                Spark(c, k0 * Life, 0.35f, Gold, spot, FxKit.Jitter(3f) + Vector3.Up * 1.5f, Gravity * 0.5f, 0.05f, 0.04f);
        }
        Hit(c, Life * 0.5f, Gold, at(0.5f), r * 0.5f);
        Flare(c, Life - 0.05f, 0.25f, Star, Gold, at(1f), 2f);
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Gold.Main, age => 1.2f * Fade(age / Life, 0.05f, 0.2f));
    }

    private void SwordHalo(Cast c, float r)
    {
        const int Swords = 6;
        const float Life = 3f, Fling = 0.3f;
        var ring = r * 0.5f;
        Mark(c, 0f, Life, Compass, Steel, Vector3.Zero, _ => ring * 1.5f, k => Fade(k, 0.05f, 0.15f), spin: 4f, gain: 1.2f);
        Disc(c, 0f, Life, Band, Steel, k => Lay(Vector3.Up * 1.1f, ring * Out(Mathf.Min(k * Life / 0.3f, 1f))), k => Fade(k, 0.05f, 0.15f) * 0.6f, band: 0.08f, gain: 1.3f);
        for (var i = 0; i < Swords; i++)
        {
            var phase = i * Mathf.Tau / Swords;
            Func<float, float> angle = k => phase + k * Life * 2.4f;
            Func<float, Vector3> at = k => Around(angle(k)) * ring * Out(Mathf.Min(k * Life / 0.3f, 1f)) + Vector3.Up * 1.1f;
            Sword3(c, 0f, Life, i % 2 == 0 ? Steel : Pearl, at, _ => 1.5f, k => Fade(k, 0.03f, 0.03f), k => Around(angle(k)));
            Trail(c, 0f, Life, Steel, at, 0.3f, 0.05f, gain: 1.1f);
            var way = Around(angle(1f));
            var last = at(1f);
            Sword3(c, Life, Fling, Pearl, k => last + way * r * 0.9f * k, _ => 1.5f, k => 1f - k, _ => way);
            Line(c, Life, Fling, Steel, k => last + way * (r * 0.9f * k - 0.6f), _ => way * 1.6f, 0.08f, k => 1f - k);
            Glyph(c, Life + Fling, 0.3f, Glint, Pearl, last + way * r * 0.9f, 1.3f, 1.7f);
        }
        Disc(c, Life, 0.4f, Shock, Steel, Vector3.Zero, k => r * 1.3f * Out(k), k => 1f - k, band: 0.12f, gain: 1.4f);
        Twinkle(c, 20, 0f, Life, Pearl, () => Around(Rand(0f, Mathf.Tau)) * ring * Rand(0.9f, 1.4f) + Vector3.Up * Rand(0.8f, 1.5f), 0.5f, 0.3f);
        Shine(c, Vector3.Up, r, Steel.Main, age => 0.9f * Fade(age / (Life + Fling), 0.05f, 0.1f));
    }

    // ---- The dead.

    private void MaskOfDread(Cast c, float r)
    {
        const float Loom = 0.4f, Stare = 1.2f;
        var reach = r * 1.25f;
        var face = new Vector3(0f, 2f, 0.7f);
        Mark(c, 0f, Loom + Stare, Links, Hex, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.05f, 0.2f), count: 6f, spin: 3f, gain: 1.2f);
        Mask3(c, 0f, Loom + Stare, Hex, k => face + Vector3.Up * 0.1f * Mathf.Sin(k * 20f), k => 3.6f * Overshoot(Mathf.Min(k * (Loom + Stare) / Loom, 1f)), k => Fade(k, 0.03f, 0.2f));
        for (var i = 0; i < 30; i++)
        {
            var way = Around(Rand(-0.55f, 0.55f));
            var high = Rand(0.3f, 2f);
            Line(c, Loom + Rand(0f, Stare - 0.4f), 0.4f, i % 2 == 0 ? Hex : Shadow, k => way * reach * (0.2f + 0.8f * k) + Vector3.Up * high, _ => way * 1.2f, 0.05f, k => Fade(k, 0.2f, 0.4f));
        }
        // Those caught in it: an eye opens over each.
        for (var i = 0; i < 4; i++)
        {
            var spot = Around(-0.45f + i * 0.3f) * reach * Rand(0.6f, 0.9f);
            var when = Loom + 0.15f + i * 0.08f;
            Glyph(c, when, Stare - 0.3f, Eye, Hex, _ => spot + Vector3.Up * 2f, k => 0.9f * Overshoot(Mathf.Min(k * 5f, 1f)), k => Fade(k, 0.05f, 0.3f), gain: 1.7f);
            Mark(c, when, Stare - 0.3f, Links, Hex, spot, _ => 0.6f, k => Fade(k, 0.05f, 0.3f), count: 4f, spin: 5f);
        }
        Wisps(c, 16, 0f, Loom + Stare - 0.6f, Hex, new Vector3(0f, 0f, 0.6f), 1.2f, 2f);
        Shine(c, new Vector3(0f, 1f, reach * 0.4f), reach, Hex.Main, age => 1.2f * Fade(age / (Loom + Stare), 0.1f, 0.3f));
    }

    private void SpecterKnights(Cast c, float r)
    {
        const float Rise = 0.4f, Charge = 0.7f, Life = Rise + Charge;
        var reach = r * 1.6f;
        for (var i = 0; i < 3; i++)
        {
            var x = (i - 1) * r * 0.55f;
            var start = Mathf.Abs(i - 1) * 0.1f;
            var from = new Vector3(x, 0f, 0.5f);
            var to = new Vector3(x, 0f, reach - Mathf.Abs(i - 1) * 0.5f);
            var life = Life - start;
            Func<float, float> go = k => Mathf.Pow(Mathf.Clamp((k * life - Rise) / (life - Rise), 0f, 1f), 1.6f);
            Func<float, Vector3> feet = k => from.Lerp(to, go(k));
            Mark(c, start, Rise + 0.5f, Triad, Spirit, from, _ => 0.7f, k => Fade(k, 0.1f, 0.5f), spin: 3f, gain: 1.3f);
            Column(c, start, Rise, Light, Spirit, from, k => new Vector2(0.4f, 3f * Out(k)), k => 1f - k, gain: 1.5f, speed: 2f);
            Ghost(c, start, life, Spirit, feet, 2.4f, gain: 1.7f);
            // Sword held upright as it rises, then levelled for the charge.
            Sword3(c, start, life, Pearl, k => feet(k) + new Vector3(0.45f, 1.5f, 0.3f + 0.7f * go(k)), k => 1.9f * Out(Mathf.Min(k * life / 0.3f, 1f)), k => Fade(k, 0.05f, 0.08f),
                k => Vector3.Up.Lerp(Vector3.Back * 2f + Vector3.Up * 0.2f, Mathf.Clamp((k * life - Rise + 0.15f) / 0.2f, 0f, 1f)));
            Trail(c, start + Rise, life - Rise, Spirit, k => feet((Rise + k * (life - Rise)) / life) + Vector3.Up * 0.8f, 1.1f, 0.4f, gain: 1.2f);
            Crescent(c, start + life - 0.1f, 0.3f, new Transform3D(new Basis(Vector3.Forward, Mathf.DegToRad(i == 1 ? 80f : i == 0 ? 30f : -30f)), to + new Vector3(0f, 1.2f, -0.5f)), 1.6f, 150f, Spirit, thick: 0.32f, sweep: 0.35f, gain: 1.6f);
            Hit(c, start + life, Spirit, to + Vector3.Up * 0.8f, 0.9f);
        }
        Wisps(c, 20, 0f, Life - 0.5f, Spirit, new Vector3(0f, 0f, reach * 0.5f), r * 0.9f);
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Spirit.Main, age => age < Life ? 0.9f : 2f * Mathf.Max(0f, 1f - (age - Life) / 0.4f));
    }

    private void SoulChains(Cast c, float r)
    {
        const float Bind = 0.4f, Drag = 1.4f, Life = Bind + Drag;
        const int Souls = 5;
        var heart = new Vector3(0f, 1.2f, r * 0.15f);
        var ground = FxKit.Flat(heart);
        Mark(c, 0f, Life + 0.5f, Links, Doom, ground, _ => r * 0.95f, k => Fade(k, 0.04f, 0.3f), count: 8f, spin: 3f, gain: 1.4f);
        Globe(c, "energy", Orb, 0f, Life, Doom, _ => heart, k => 0.25f + 0.3f * k, _ => 1f, gain: 1.6f);
        Flare(c, 0f, Life, Star, Soul, _ => heart, k => 1.2f + k, _ => 1f);
        for (var i = 0; i < Souls; i++)
        {
            var angle = i * Mathf.Tau / Souls + 0.3f;
            var far = Around(angle) * r * 0.85f;
            var start = i * 0.06f;
            var life = Life - start;
            var mine = (float)i;
            // Each struggles on its chain as it is hauled in.
            Func<float, Vector3> feet = k => ground + far * (1f - 0.8f * Mathf.Pow(Mathf.Clamp((k * life - Bind) / (life - Bind), 0f, 1f), 2f)) + Around(angle + 1.57f) * 0.25f * Mathf.Sin(k * 26f + mine);
            Ghost(c, start, life, i % 2 == 0 ? Spirit : Soul, feet, 1.7f);
            Tube(c, start + 0.1f, life - 0.1f, Spiral, Doom, k => (heart, feet((0.1f + k * (life - 0.1f)) / life) + Vector3.Up * 0.9f), _ => 0.07f, k => Fade(k, 0.1f, 0.02f), twist: 8f, speed: -3f, gain: 1.7f);
            Mark(c, start, Bind + 0.3f, Links, Soul, ground + far, _ => 0.55f, k => Fade(k, 0.1f, 0.5f), count: 4f, spin: 6f, gain: 1.3f);
            Glyph(c, start, 0.3f, Glint, Soul, ground + far + Vector3.Up * 0.9f, 1.2f, 1.6f);
        }
        Hit(c, Life, Doom, heart, r * 0.8f);
        Cue(c, Life, () => Stage.Explosion(At(c, heart), r * 0.6f, Soul.Main, FxBurst.Dome));
        Glyph(c, Life, 0.5f, Skull, Soul, heart + Vector3.Up * 0.3f, 2.4f, 1.8f);
        Wisps(c, 16, Life, Life + 0.2f, Soul, ground, r * 0.5f, 2.2f);
        Shine(c, heart, r * 1.5f, Doom.Main, age => age < Life ? 0.8f : 2.2f * Mathf.Max(0f, 1f - (age - Life) / 0.5f));
    }

    private void Wraithstorm(Cast c, float r)
    {
        const float Life = 3.2f;
        var reach = r * 0.7f;
        Mark(c, 0f, Life, Whorl, Spirit, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.15f), count: 8f, spin: -14f, gain: 1.3f);
        Disc(c, 0f, Life, Swirl, Soul, Vector3.Zero, _ => r, k => Fade(k, 0.08f, 0.15f) * 0.7f, spin: 7f);
        Column(c, 0f, Life, Spiral, Spirit, Vector3.Zero, k => new Vector2(reach * (0.5f + 0.1f * Mathf.Sin(k * 20f)), 5f * Out(Mathf.Min(k * Life / 0.4f, 1f))), k => Fade(k, 0.05f, 0.15f) * 0.6f, twist: 2.5f, speed: 3.5f, gain: 1.3f);
        Column(c, 0f, Life, Fire, Soul, Vector3.Zero, _ => new Vector2(reach * 0.3f, 4f), k => Fade(k, 0.06f, 0.15f) * 0.7f, gain: 1.3f, speed: 2.5f);
        for (var i = 0; i < 26; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var start = i * (Life - 1.2f) / 26f;
            var turn = Rand(6f, 8f);
            var far = reach * Rand(0.6f, 1.15f);
            var low = Rand(0f, 1.2f);
            Func<float, Vector3> at = k => Around(phase + k * turn) * far * (0.7f + 0.5f * k) + Vector3.Up * (low + 3.2f * k);
            Glyph(c, start, 1.2f, i % 5 == 0 ? Skull : Wraith, i % 2 == 0 ? Spirit : Soul, at, k => 1.1f + 0.5f * k, k => Fade(k, 0.2f, 0.35f), k => Vector3.Up * 0.5f - Heading(at, k, 1f), gain: 1.6f, slim: 0.8f);
            if (i % 2 == 0)
                Trail(c, start, 1.2f, Spirit, at, 0.45f, 0.25f, gain: 1f);
        }
        for (var i = 0; i < 36; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var high = Rand(0.2f, 3.6f);
            Func<float, Vector3> at = k => Around(phase + k * 5f) * reach * 1.15f + Vector3.Up * (high + 0.6f * k);
            Line(c, Rand(0f, Life - 0.5f), 0.5f, Spirit, at, k => Heading(at, k, 1f), 0.05f, k => Fade(k, 0.2f, 0.3f) * 0.8f);
        }
        Cue(c, 0.2f, () => Stage.Patch(At(c, Vector3.Zero), r * 0.9f, Life - 0.2f, Soul.Main, FxGround.Miasma));
        Shine(c, Vector3.Up * 1.5f, r * 1.5f, Spirit.Main, age => (1.1f + 0.2f * Mathf.Sin(age * 13f)) * Fade(age / Life, 0.06f, 0.15f));
    }

    private void SkullPyre(Cast c, float r)
    {
        const float Stack = 0.5f, Burn = 1.6f, Blow = Stack + Burn;
        var spot = new Vector3(0f, 0f, r * 0.15f);
        Mark(c, 0f, Blow + 0.6f, Triad, Soul, spot, _ => r * 0.75f, k => Fade(k, 0.04f, 0.3f), spin: 1.5f, gain: 1.3f);
        Mark(c, 0f, Blow + 0.6f, Filigree, Doom, spot, _ => r * 0.85f, k => Fade(k, 0.04f, 0.3f) * 0.7f, count: 9f, spin: -1f);
        // A pyramid of skulls dropped into place: three, two, one.
        var places = new[] { new Vector3(-0.75f, 0.45f, 0f), new Vector3(0f, 0.45f, 0.15f), new Vector3(0.75f, 0.45f, 0f), new Vector3(-0.38f, 1.15f, 0.05f), new Vector3(0.38f, 1.15f, 0.05f), new Vector3(0f, 1.85f, 0.05f) };
        for (var i = 0; i < places.Length; i++)
        {
            var at = spot + places[i];
            var start = i * Stack / places.Length;
            var life = Blow - start;
            Glyph(c, start, life, Skull, i == 5 ? Spirit : Bone, k => at + Vector3.Up * 2.5f * Mathf.Pow(Mathf.Max(0f, 1f - k * life / 0.15f), 2f), _ => 0.95f, k => Fade(k, 0.03f, 0.02f), gain: 1.7f);
            Disc(c, start + 0.15f, 0.25f, Shock, Bone, FxKit.Flat(at), k => 0.7f * Out(k), k => 1f - k, band: 0.1f);
            var way = (places[i] - new Vector3(0f, 0.6f, 0f)).Normalized();
            Func<float, Vector3> fly = k => at + (way * 5f + Vector3.Up * 3f) * k * 0.6f + Vector3.Down * 0.5f * Gravity * k * k * 0.36f;
            FlyingSkull(c, Blow, 0.6f, Soul, fly, 0.9f, 0.5f);
        }
        Column(c, Stack, Burn, Fire, Soul, spot, k => new Vector2(1.1f, 2.6f + 1.6f * k), k => Fade(k, 0.1f, 0.05f), gain: 1.6f, speed: 2.4f);
        Column(c, Stack, Burn, Fire, Doom, spot, k => new Vector2(0.6f, 3.6f + 2f * k), k => Fade(k, 0.1f, 0.05f) * 0.8f, gain: 1.4f, speed: 3f);
        Disc(c, Stack, Burn, Glow, Soul, spot, _ => r * 0.7f, k => Fade(k, 0.1f, 0.1f) * 0.7f);
        Wisps(c, 26, Stack, Blow - 0.6f, Soul, spot, 1.2f, 3f);
        Hit(c, Blow, Soul, spot + Vector3.Up, r * 0.9f);
        Cue(c, Blow, () => Stage.Explosion(At(c, spot + Vector3.Up), r * 0.7f, Soul.Main, FxBurst.Dome));
        Shine(c, spot + Vector3.Up, r * 1.6f, Soul.Main, age => age < Blow ? 1.2f * Mathf.Min(age / Stack, 1f) : 2.6f * Mathf.Max(0f, 1f - (age - Blow) / 0.6f));
    }

    private void PhantomArchers(Cast c, float r)
    {
        const int Archers = 4;
        const float Rise = 0.4f, Flight = 0.3f, Life = 1.9f;
        var reach = r * 1.7f;
        for (var i = 0; i < Archers; i++)
        {
            var x = (i - (Archers - 1) * 0.5f) * r * 0.42f;
            var feet = new Vector3(x, 0f, 0.3f);
            var start = i * 0.07f;
            Mark(c, start, Life - start, Reticle, Spirit, feet, _ => 0.55f, k => Fade(k, 0.05f, 0.2f), count: 6f, spin: 3f, gain: 1.2f);
            Ghost(c, start, Life - start, i % 2 == 0 ? Spirit : Soul, _ => feet, 2f);
            var bow = feet + new Vector3(0f, 1.3f, 0.4f);
            // The bow: a standing arc of light, drawn and loosed three times.
            Crescent(c, start + Rise * 0.5f, Life - start - Rise * 0.5f - 0.2f, new Transform3D(new Basis(Vector3.Forward, Mathf.Pi * 0.5f), bow + Vector3.Forward * 0.5f), 0.9f, 130f, Spirit, thick: 0.12f, sweep: 0.05f, gain: 1.5f);
            for (var shot = 0; shot < 3; shot++)
            {
                var when = start + Rise + shot * 0.36f;
                var to = new Vector3(x * 1.3f + Rand(-0.4f, 0.4f), 1f, reach * Rand(0.85f, 1f));
                Func<float, Vector3> at = k => bow.Lerp(to, k);
                Glyph(c, when, Flight, Shaft, Pearl, at, _ => 1.5f, _ => 1f, _ => to - bow, gain: 1.9f, slim: 0.5f);
                Trail(c, when, Flight, Spirit, at, 0.3f, 0.5f, gain: 1.4f);
                Flare(c, when, 0.15f, Star, Spirit, bow, 1.3f);
                Glyph(c, when + Flight, 0.3f, Glint, Spirit, to, 1.4f, 1.7f);
                Disc(c, when + Flight, 0.3f, Shock, Spirit, FxKit.Flat(to), k => 0.9f * Out(k), k => 1f - k, band: 0.1f);
                Spray(c, when + Flight, Spirit, to, 4, 2.5f, 2f, 0.35f);
            }
        }
        Wisps(c, 14, 0f, Life - 0.8f, Spirit, new Vector3(0f, 0f, 0.3f), r * 0.7f);
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Spirit.Main, age => 1f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void SpiritPack(Cast c, float r)
    {
        const float Run = 0.75f;
        var reach = r * 1.7f;
        for (var i = 0; i < 4; i++)
        {
            var x = (i - 1.5f) * r * 0.36f;
            var start = i % 2 * 0.1f + Rand(0f, 0.05f);
            var weave = Rand(0.4f, 0.8f) * (i % 2 == 0 ? 1f : -1f);
            var mine = (float)i;
            // A low, long shape bounding forward: the body streams back from the head.
            Func<float, Vector3> at = k => new Vector3(x + weave * Mathf.Sin(k * 7f + mine), 0.55f + 0.3f * Mathf.Abs(Mathf.Sin(k * 14f + mine)), 0.4f + (reach - 0.4f) * k);
            Glyph(c, start, Run, Wraith, i % 2 == 0 ? Spirit : Soul, k => at(k) - Heading(at, k, 0.7f), _ => 2.2f, k => Fade(k, 0.1f, 0.08f), k => Heading(at, k, 1f), gain: 1.8f, slim: 0.5f);
            Flare(c, start, Run, Mote, Spirit, at, _ => 1f, k => Fade(k, 0.1f, 0.1f) * 0.6f);
            Trail(c, start, Run, Spirit, at, 0.7f, 0.3f, gain: 1.2f);
            for (var j = 0; j < 5; j++)
            {
                var k0 = (j + 0.5f) / 5f;
                Mark(c, start + k0 * Run, 0.5f, Ripples, Spirit, FxKit.Flat(at(k0)), k => 0.2f + 0.5f * k, k => 1f - k, count: 2f, spin: 0f);
            }
            var end = at(1f);
            Glyph(c, start + Run - 0.05f, 0.4f, Claw, Spirit, end + Vector3.Up * 0.5f, 2f, 1.9f);
            Hit(c, start + Run, Spirit, end, 0.8f);
        }
        Wisps(c, 14, 0f, Run, Spirit, new Vector3(0f, 0f, reach * 0.5f), r * 0.8f);
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Spirit.Main, age => 1f + 1.2f * Bump(age, Run, 0.5f));
    }

    private void DarkCrown(Cast c, float r)
    {
        const float Life = 3.2f;
        var reach = r * 0.45f;
        Mark(c, 0f, Life, Links, Shadow, Vector3.Zero, _ => reach * 1.5f, k => Fade(k, 0.05f, 0.15f), count: 7f, spin: 2f, gain: 1.3f);
        Disc(c, 0f, Life, Swirl, Void, Vector3.Zero, _ => reach * 1.6f, k => Fade(k, 0.08f, 0.15f) * 0.7f, spin: -4f);
        Crown(c, 0f, Life, Amethyst, k => Vector3.Up * (2.25f + 1.5f * Mathf.Pow(Mathf.Max(0f, 1f - k * Life / 0.5f), 2f) + 0.06f * Mathf.Sin(k * Life * 4f)));
        Column(c, 0f, Life, Fire, Shadow, Vector3.Zero, _ => new Vector2(0.7f, 2.3f), k => Fade(k, 0.08f, 0.15f) * 0.6f, gain: 1.3f, speed: 2f);
        for (var i = 0; i < 3; i++)
        {
            var phase = i * Mathf.Tau / 3f;
            Func<float, Vector3> at = k => Around(phase - k * Life * 2f) * reach + Vector3.Up * (1.2f + 0.3f * Mathf.Sin(k * Life * 3f + phase));
            Glyph(c, 0.3f, Life - 0.3f, Skull, Shadow, at, _ => 0.6f, k => Fade(k, 0.08f, 0.1f), gain: 1.7f);
            Trail(c, 0.3f, Life - 0.3f, Shadow, at, 0.3f, 0.08f, gain: 1.2f);
        }
        Disc(c, 0.5f, 0.5f, Shock, Amethyst, Vector3.Zero, k => r * 0.9f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Twinkle(c, 24, 0.5f, Life - 0.4f, Amethyst, () => Vector3.Up * 2.5f + new Vector3(Rand(-0.8f, 0.8f), Rand(-0.4f, 0.8f), Rand(-0.3f, 0.3f)), 0.5f);
        Wisps(c, 16, 0f, Life - 0.9f, Shadow, Vector3.Zero, reach * 1.2f, 2f);
        Shine(c, Vector3.Up, r, Amethyst.Main, age => 0.9f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void NightmareEyes(Cast c, float r)
    {
        const float Life = 2.6f, Blow = 2.1f;
        var heart = new Vector3(0f, 1f, r * 0.1f);
        var ground = FxKit.Flat(heart);
        Disc(c, 0f, Life, Swirl, Void, ground, _ => r, k => Fade(k, 0.08f, 0.15f), spin: 2f, gain: 1.2f);
        Mark(c, 0f, Life, Maze, Hex, ground, _ => r, k => Fade(k, 0.05f, 0.2f) * 0.9f, spin: 3f, gain: 1.2f);
        Cue(c, 0.2f, () => Stage.Patch(At(c, ground), r * 0.9f, Life - 0.2f, Shadow.Main, FxGround.Miasma));
        for (var i = 0; i < 12; i++)
        {
            var at = ground + Around(i * 2.4f) * r * Rand(0.45f, 0.95f) + Vector3.Up * Rand(0.8f, 2.6f);
            var start = Rand(0.1f, 1.2f);
            var size = Rand(0.9f, 1.6f);
            var life = Blow - start + 0.1f;
            // Each opens, blinks once, and fixes on the middle.
            Glyph(c, start, life, Eye, i % 3 == 0 ? Amethyst : Hex, _ => at, k => size * Overshoot(Mathf.Min(k * life / 0.2f, 1f)), k => Fade(k, 0.02f, 0.05f) * (Mathf.Abs(k - 0.45f) < 0.04f ? 0.1f : 1f), gain: 1.9f);
            Line(c, start + 0.5f * life, 0.5f * life, Hex, _ => (at + heart) * 0.5f, _ => heart - at, 0.03f, k => Fade(k, 0.2f, 0.05f) * 0.8f);
            Line(c, Blow, 0.2f, Pearl, _ => (at + heart) * 0.5f, _ => heart - at, 0.1f, k => 1f - k);
        }
        Globe(c, "void", 0, 0.6f, Blow - 0.6f, Void, _ => heart, k => 0.2f + 0.5f * k, _ => 1f);
        Hit(c, Blow, Hex, heart, r * 0.9f);
        Cue(c, Blow, () => Stage.Explosion(At(c, heart), r * 0.6f, Hex.Main, FxBurst.Dome));
        Wisps(c, 20, 0f, Life - 0.9f, Shadow, ground, r * 0.9f);
        Shine(c, heart, r * 1.5f, Hex.Main, age => age < Blow ? 0.7f : 2.2f * Mathf.Max(0f, 1f - (age - Blow) / 0.5f));
    }

    private void BoneWhorl(Cast c, float r)
    {
        const float Life = 2.4f;
        Mark(c, 0f, Life, Whorl, Bone, Vector3.Zero, _ => r, k => Fade(k, 0.04f, 0.2f), count: 7f, spin: 10f, gain: 1.5f);
        Mark(c, 0f, Life, Deathmark, Bone, Vector3.Zero, _ => r * 0.35f, k => Fade(k, 0.04f, 0.2f), gain: 1.3f);
        Disc(c, 0f, Life, Glow, Soul, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.2f) * 0.3f);
        for (var i = 0; i < 10; i++)
        {
            var phase = i * Mathf.Tau / 10f;
            var start = i * 0.12f;
            const float Ride = 1.1f;
            // Skulls ride the spiral out from the middle.
            Func<float, Vector3> at = k => Around(phase + k * 6.5f) * r * (0.1f + 0.9f * k) + Vector3.Up * (0.6f + 0.3f * Mathf.Sin(k * 9f));
            FlyingSkull(c, start, Ride, i % 2 == 0 ? Bone : Soul, at, 0.7f, 0.2f);
            var end = at(1f);
            Glyph(c, start + Ride, 0.3f, Glint, Bone, end, 1.3f, 1.7f);
            Spray(c, start + Ride, Bone, end, 5, 3f, 3f, 0.4f, 0.08f);
        }
        for (var i = 0; i < 40; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            Func<float, Vector3> at = k => Around(phase + k * 5f) * r * (0.1f + 0.9f * k) + Vector3.Up * 0.15f;
            Line(c, Rand(0f, Life - 0.7f), 0.7f, Bone, at, k => Heading(at, k, 0.5f), 0.07f, k => Fade(k, 0.2f, 0.3f));
        }
        Disc(c, 0f, 0.45f, Shock, Bone, Vector3.Zero, k => r * 1.1f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Shine(c, Vector3.Up, r * 1.4f, Bone.Main, age => 1f * Fade(age / Life, 0.06f, 0.2f));
    }

    private void DeathKnell(Cast c, float r)
    {
        const float Toll = 0.6f;
        const int Tolls = 3;
        var bell = new Vector3(0f, 3f, 0f);
        var last = (Tolls - 1) * Toll + 0.15f;
        Mark(c, 0f, last + 1f, Dial, Doom, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), spin: 1f, gain: 1.3f);
        Glyph(c, 0f, last + 0.6f, Note, Doom, k => bell + new Vector3(0.25f * Mathf.Sin(k * (last + 0.6f) / Toll * Mathf.Tau), 0f, 0f), k => 2f * Overshoot(Mathf.Min(k * 6f, 1f)), k => Fade(k, 0.03f, 0.2f),
            k => new Vector3(0.3f * Mathf.Sin(k * (last + 0.6f) / Toll * Mathf.Tau), 1f, 0f), gain: 1.9f);
        for (var i = 0; i < Tolls; i++)
        {
            var when = 0.15f + i * Toll;
            var big = i == Tolls - 1;
            Disc(c, when, 0.7f, Shock, Doom, Vector3.Zero, k => r * (big ? 1.6f : 1.1f) * Out(k), k => 1f - k, band: 0.12f, gain: big ? 1.6f : 1.2f);
            Mark(c, when, 0.8f, Ripples, Soul, Vector3.Zero, k => r * (0.3f + 0.9f * k), k => (1f - k) * 0.9f, count: 3f, spin: 0f, gain: 1.3f);
            Glyph(c, when, 0.5f, Bubble, Doom, k => bell, k => 1f + 5f * k, k => (1f - k) * 0.8f, gain: 1.4f);
            Glyph(c, when, 0.5f, Skull, Soul, Vector3.Up * 1.3f, big ? 3f : 1.4f + 0.5f * i, big ? 1.7f : 1.3f);
            Flare(c, when, 0.2f, Star, Doom, bell, 2f);
        }
        Hit(c, last, Soul, Vector3.Up * 0.5f, r);
        Mark(c, last, 0.9f, Deathmark, Soul, Vector3.Zero, _ => r * 0.8f, k => Fade(k, 0.02f, 0.6f), _ => 1f, gain: 1.5f);
        Wisps(c, 24, last, last + 0.3f, Soul, Vector3.Zero, r * 0.8f, 2.4f);
        Shine(c, Vector3.Up, r * 1.6f, Doom.Main, age => 0.6f + 1.4f * Bump(age, 0.15f, 0.4f) + 1.4f * Bump(age, 0.15f + Toll, 0.4f) + 2.2f * Bump(age, last, 0.6f));
    }

    private void ShadowPuppets(Cast c, float r)
    {
        const float Dance = 1.6f, Fall = 0.2f;
        Mark(c, 0f, Dance + Fall + 0.6f, Checker, Hex, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), count: 8f, spin: 1f, gain: 1.2f);
        for (var i = 0; i < 3; i++)
        {
            var x = (i - 1) * r * 0.55f;
            var z = r * (i == 1 ? 0.35f : 0.1f);
            var start = i * 0.1f;
            var phase = i * 2.1f;
            var life = Dance - start;
            Func<float, Vector3> hang = k => new Vector3(x + 0.35f * Mathf.Sin(k * life * 4f + phase), 2.1f + 0.3f * Mathf.Abs(Mathf.Sin(k * life * 6f + phase)), z);
            var tone = i == 1 ? Hex : Shadow;
            Glyph(c, start, life, Mask, tone, hang, k => 1.5f * Overshoot(Mathf.Min(k * life / 0.25f, 1f)), k => Fade(k, 0.03f, 0.02f), k => new Vector3(0.25f * Mathf.Sin(k * life * 4f + phase), 1f, 0f), gain: 1.9f);
            // The body dangles under it; strings run up out of sight.
            Glyph(c, start, life, Wraith, tone, k => hang(k) + Vector3.Down * 1.2f, _ => 1.6f, k => Fade(k, 0.05f, 0.02f) * 0.8f, k => new Vector3(-0.2f * Mathf.Sin(k * life * 4f + phase), 1f, 0f), gain: 1.3f, slim: 0.7f);
            foreach (var side in new[] { -0.4f, 0f, 0.4f })
                Line(c, start, life, Pearl, k => hang(k) + new Vector3(side * 0.5f, 3f, 0f), k => new Vector3(side, 6f, 0f), 0.015f, k => Fade(k, 0.1f, 0.02f) * 0.6f);
            // The strings are cut: it drops and bursts.
            var top = hang(1f);
            var land = new Vector3(top.X, 0.4f, top.Z);
            Glyph(c, Dance, Fall, Mask, tone, k => top.Lerp(land, k * k), _ => 1.5f, _ => 1f, gain: 1.9f);
            Glyph(c, Dance - 0.05f, 0.25f, Glint, Pearl, top + Vector3.Up * 0.9f, 1.4f, 1.7f);
            Hit(c, Dance + Fall, tone, land, 0.9f);
            Wisps(c, 5, Dance + Fall, Dance + Fall + 0.1f, tone, FxKit.Flat(land), 0.5f, 1.6f);
        }
        Shine(c, Vector3.Up, r * 1.4f, Hex.Main, age => 0.8f + 1.4f * Bump(age, Dance + Fall, 0.5f));
    }

    private void LichForm(Cast c, float r)
    {
        const float Life = 3.4f;
        var reach = r * 0.45f;
        Mark(c, 0f, Life, Deathmark, Doom, Vector3.Zero, _ => reach * 1.4f, k => Fade(k, 0.05f, 0.15f), gain: 1.3f);
        Mark(c, 0f, Life, Links, Soul, Vector3.Zero, _ => reach * 2f, k => Fade(k, 0.05f, 0.15f) * 0.8f, count: 8f, spin: -2f);
        Glyph(c, 0f, Life, Wraith, Doom, k => new Vector3(0f, 1.9f, -0.5f), k => 4.2f * Out(Mathf.Min(k * Life / 0.4f, 1f)) * (1f + 0.03f * Mathf.Sin(k * Life * 5f)), k => Fade(k, 0.04f, 0.12f) * 0.8f, gain: 1.5f, slim: 0.8f);
        Column(c, 0f, Life, Fire, Doom, Vector3.Zero, _ => new Vector2(0.75f, 2.6f), k => Fade(k, 0.06f, 0.12f) * 0.7f, gain: 1.4f, speed: 2.2f);
        Glyph(c, 0.2f, Life - 0.2f, Skull, Soul, k => Vector3.Up * (2.75f + 0.06f * Mathf.Sin(k * Life * 4f)), k => 0.95f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.04f, 0.12f), gain: 1.7f);
        Crown(c, 0.35f, Life - 0.35f, Gold, k => Vector3.Up * (3.3f + 0.06f * Mathf.Sin(k * Life * 4f)), 0.34f);
        // Phylacteries: three gems circling, each tied to the lich by a thread.
        for (var i = 0; i < 3; i++)
        {
            var phase = i * Mathf.Tau / 3f;
            Func<float, Vector3> at = k => Around(phase + k * Life * 1.8f) * reach * 1.3f + Vector3.Up * (1.3f + 0.3f * Mathf.Sin(k * Life * 2.6f + phase));
            Glyph(c, 0.3f, Life - 0.3f, Gem, Soul, at, _ => 0.8f, k => Fade(k, 0.08f, 0.1f), gain: 1.8f);
            Tube(c, 0.4f, Life - 0.4f, Beam, Soul, k => (at(k), Vector3.Up * 1.4f), _ => 0.03f, k => Fade(k, 0.1f, 0.15f) * 0.8f, speed: 3f, gain: 1.4f);
        }
        for (var t = 0.2f; t < Life - 0.5f; t += 0.7f)
            Disc(c, t, 0.6f, Shock, Doom, Vector3.Zero, k => r * 0.9f * Out(k), k => (1f - k) * 0.8f, band: 0.1f);
        Wisps(c, 24, 0f, Life - 0.9f, Soul, Vector3.Zero, reach * 1.4f, 2.4f);
        Shine(c, Vector3.Up, r * 1.2f, Doom.Main, age => (1.1f + 0.2f * Mathf.Sin(age * 9f)) * Fade(age / Life, 0.06f, 0.12f));
    }

    // ---- Blessings.

    private void CrownOfVictory(Cast c, float r)
    {
        const float Life = 3f;
        var reach = r * 0.42f;
        Mark(c, 0f, Life, Sunburst, Gold, Vector3.Zero, _ => reach * 1.5f, k => Fade(k, 0.05f, 0.15f), count: 12f, spin: 2f, gain: 1.3f);
        Func<float, float> high = k => 2.5f + 2.5f * Mathf.Pow(Mathf.Max(0f, 1f - k * Life / 0.6f), 2f) + 0.05f * Mathf.Sin(k * Life * 4f);
        Crown(c, 0f, Life, Gold, k => Vector3.Up * (high(k) - 0.3f), gain: 2f);
        Mark(c, 0.5f, Life - 0.5f, Rays, Gold, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * 1.8f), new Vector3(0f, high(k), -0.05f)), k => Fade(k, 0.1f, 0.2f) * 0.7f, k => Out(Mathf.Min(k * 4f, 1f)), count: 16f, spin: 2f, gain: 1.3f);
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, _ => new Vector2(reach * 0.8f, 7f), k => Fade(k, 0.08f, 0.2f) * 0.5f, gain: 1.3f, speed: -1.5f);
        Flare(c, 0.55f, 0.35f, Star, Gold, Vector3.Up * 2.5f, 3.6f);
        Disc(c, 0.55f, 0.5f, Shock, Gold, Vector3.Zero, k => r * 0.9f * Out(k), k => 1f - k, band: 0.12f, gain: 1.4f);
        Drift(c, 22, 0.5f, Life - 1.2f, 1.1f, Starlet, [Gold, Pearl], () => Within(Vector3.Zero, reach * 1.3f, 0.2f, 0.6f), () => Vector3.Up * Rand(1.2f, 2.2f), 0.4f, sway: 0.2f, gain: 1.8f);
        Twinkle(c, 24, 0.5f, Life - 0.4f, Pearl, () => Vector3.Up * 2.5f + new Vector3(Rand(-0.9f, 0.9f), Rand(-0.4f, 0.8f), Rand(-0.3f, 0.3f)), 0.5f);
        Shine(c, Vector3.Up * 1.5f, r, Gold.Main, age => 1.1f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void BattleHymn(Cast c, float r)
    {
        const float Life = 3f;
        var reach = r * 0.5f;
        Mark(c, 0f, Life, Ripples, Gold, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.08f, 0.2f) * 0.8f, count: 4f, spin: 1.2f, gain: 1.2f);
        Mark(c, 0f, Life, Filigree, Rage, Vector3.Zero, _ => reach * 1.2f, k => Fade(k, 0.05f, 0.15f), count: 8f, spin: 3f, gain: 1.2f);
        // Notes wind up round the singer on a stave of five lines.
        for (var line = 0; line < 5; line++)
        {
            var lift = line * 0.14f;
            Func<float, Vector3> path = s => Around(s * 11f) * reach * 1.05f + Vector3.Up * (0.3f + lift + 2.2f * s);
            Ribbon(c, 0f, Life, 0, Gold, path, k => (Mathf.Max(0f, k - 0.3f), k), 0.035f, k => Fade(k, 0.03f, 0.12f) * 0.8f, gain: 1.4f);
        }
        for (var i = 0; i < 20; i++)
        {
            var s0 = Rand(0f, 0.25f);
            var start = i * (Life - 0.9f) / 20f;
            var lift = Rand(0f, 0.56f);
            var size = Rand(0.5f, 0.75f);
            Func<float, Vector3> at = k => Around((start / Life + k * 0.3f - s0 * 0.2f) * 11f) * reach * 1.05f + Vector3.Up * (0.3f + lift + 2.2f * (start / Life + k * 0.3f));
            Glyph(c, start, 0.9f, Note, i % 3 == 0 ? Rage : i % 3 == 1 ? Gold : Pearl, at, _ => size, k => Fade(k, 0.15f, 0.3f), gain: 1.9f);
        }
        for (var t = 0f; t < Life - 0.4f; t += 0.5f)
        {
            Disc(c, t, 0.5f, Shock, (int)(t / 0.5f) % 2 == 0 ? Gold : Rage, Vector3.Zero, k => r * Out(k), k => (1f - k) * 0.8f, band: 0.1f, gain: 1.2f);
            Disc(c, t, 0.5f, Band, Gold, k => Lay(Vector3.Up * (0.2f + 2.4f * k), reach * (0.8f + 0.5f * k)), k => Fade(k, 0.1f, 0.5f) * 0.6f, band: 0.1f);
        }
        Shine(c, Vector3.Up, r * 1.1f, Gold.Main, age => (1f + 0.3f * Mathf.Sin(age * Mathf.Tau * 2f)) * Fade(age / Life, 0.06f, 0.15f));
    }

    private void LuckyStar(Cast c, float r)
    {
        const float Life = 2.8f;
        var reach = r * 0.45f;
        Mark(c, 0f, Life, Galaxy, Gold, Vector3.Zero, _ => r * 0.7f, k => Fade(k, 0.08f, 0.2f), count: 8f, spin: 3f, gain: 1.3f);
        Mark(c, 0f, Life, Pentacle, Gold, Vector3.Zero, _ => reach, k => Fade(k, 0.05f, 0.2f), count: 5f, spin: 3f, gain: 1.3f);
        Glyph(c, 0f, Life, Starlet, Gold, k => Vector3.Up * (2.7f + 0.1f * Mathf.Sin(k * Life * 5f)), k => 1.4f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.04f, 0.12f),
            k => new Vector3(Mathf.Sin(k * Life * 3f), Mathf.Cos(k * Life * 3f), 0f), gain: 2.1f);
        Flare(c, 0f, Life, Mote, Gold, _ => Vector3.Up * 2.7f, _ => 2.6f, k => Fade(k, 0.05f, 0.15f) * 0.5f, gain: 1f);
        // Coins shower down from it and bounce.
        for (var i = 0; i < 26; i++)
        {
            var land = Within(Vector3.Zero, reach * 1.6f);
            var start = Rand(0.2f, Life - 1.2f);
            var size = Rand(0.25f, 0.38f);
            var from = Vector3.Up * 2.6f;
            Func<float, Vector3> fall = k => Lob(from, land + Vector3.Up * 0.15f, 0.55f, k);
            Glyph(c, start, 0.55f, Coin, i % 3 == 0 ? Honey : Gold, fall, _ => size, k => Fade(k, 0.1f, 0f), gain: 2f);
            Func<float, Vector3> hop = k => land + Vector3.Up * (0.15f + 0.5f * Mathf.Sin(k * Mathf.Pi)) + FxKit.Flat(land).Normalized() * 0.3f * k;
            Glyph(c, start + 0.55f, 0.35f, Coin, Gold, hop, _ => size, k => 1f - k * k, gain: 1.9f);
            Glyph(c, start + 0.55f, 0.25f, Glint, Pearl, land + Vector3.Up * 0.2f, 0.8f, 1.6f);
        }
        Twinkle(c, 24, 0f, Life - 0.4f, Pearl, () => Within(Vector3.Zero, reach * 1.4f, 0.4f, 3f), 0.5f);
        Shine(c, Vector3.Up, r, Gold.Main, age => 1f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void AegisOath(Cast c, float r)
    {
        const float Life = 3f;
        var ward = new Vector3(0f, 1.25f, 0.95f);
        Mark(c, 0f, Life, Scales, Steel, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.05f, 0.15f), count: 9f, spin: 1f, gain: 1.3f);
        // A real shield held out in front, facing the way the caster does: steel, in a shell of
        // golden light that flares with every blow. It sways a little, so its shape shows.
        const float First = 0.6f, Every = 0.45f;
        Transform3D Held(float t, float size) => new(new Basis(Vector3.Up, Mathf.Pi + 0.28f * Mathf.Sin(t * 1.5f)) * new Basis(Vector3.Right, 0.1f) * Basis.FromScale(Vector3.One * size * Overshoot(Mathf.Min(t / 0.25f, 1f))),
            ward + Vector3.Up * 0.05f * Mathf.Sin(t * 3f));
        Add(c, ShieldMesh, AegisSteel, 0f, Life, (n, k, t) =>
        {
            n.Transform = Held(t, 1.75f);
            n.SetInstanceShaderParameter("uFade", Mathf.Clamp((1f - k) / 0.12f, 0f, 1f));
        });
        Add(c, ShieldMesh, AegisGlow, 0f, Life, (n, k, t) =>
        {
            n.Transform = Held(t, 1.95f);
            var blow = t < First || t > Life - 0.4f ? 0f : Mathf.Max(0f, 1f - (t - First) % Every / 0.25f);
            n.SetInstanceShaderParameter("uAlpha", Fade(k, 0.03f, 0.1f) * 0.75f);
            n.SetInstanceShaderParameter("uImpact", blow);
            n.SetInstanceShaderParameter("uDissolve", 0f);
        });
        Globe(c, "energy", Hexes, 0.1f, Life - 0.1f, Steel, _ => Vector3.Up * 1f, k => 1.3f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.03f, 0.1f) * 0.6f, gain: 1.2f);
        Flare(c, 0f, 0.35f, Star, Gold, ward, 3.6f);
        Disc(c, 0f, 0.5f, Shock, Steel, Vector3.Zero, k => r * 0.9f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        // Blows glancing off it.
        for (var t = First; t < Life - 0.4f; t += Every)
        {
            var at = ward + new Vector3(Rand(-0.6f, 0.6f), Rand(-0.6f, 0.7f), 0.25f);
            Glyph(c, t, 0.3f, Glint, Pearl, at, 1.5f, 1.8f);
            Mark(c, t, 0.35f, Ripples, Gold, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (0.2f + 0.9f * k)), at), k => 1f - k, count: 1.5f, spin: 0f, gain: 1.4f);
            for (var j = 0; j < 4; j++)
                Spark(c, t, 0.3f, Gold, at, FxKit.Jitter(3f) + Vector3.Back * 3f, Gravity * 0.5f, 0.05f, 0.04f);
        }
        Shine(c, ward, r, Steel.Main, age => 0.9f * Fade(age / Life, 0.05f, 0.1f));
    }

    private void TimeSlip(Cast c, float r)
    {
        const float Life = 3f;
        var reach = r * 0.45f;
        Mark(c, 0f, Life, Dial, Chrono, Vector3.Zero, _ => reach * 1.6f, k => Fade(k, 0.05f, 0.15f), spin: 14f, gain: 1.4f);
        Glyph(c, 0f, Life, Sandglass, Gold, k => Vector3.Up * (2.8f + 0.08f * Mathf.Sin(k * Life * 4f)), k => 1.4f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.04f, 0.12f),
            k => new Vector3(Mathf.Sin(Mathf.Floor(k * Life / 1f) * Mathf.Pi + Mathf.SmoothStep(0f, 1f, Mathf.Clamp(k * Life % 1f / 0.25f, 0f, 1f)) * Mathf.Pi), Mathf.Cos(Mathf.Floor(k * Life / 1f) * Mathf.Pi + Mathf.SmoothStep(0f, 1f, Mathf.Clamp(k * Life % 1f / 0.25f, 0f, 1f)) * Mathf.Pi), 0f), gain: 2f);
        Column(c, 0f, Life, Spiral, Chrono, Vector3.Zero, _ => new Vector2(reach, 2.5f), k => Fade(k, 0.06f, 0.15f) * 0.6f, twist: 2f, speed: 5f, gain: 1.2f);
        // Afterimages streaming off to either side: they are moving faster than they can be seen.
        for (var i = 0; i < 8; i++)
        {
            var side = i % 2 == 0 ? 1f : -1f;
            var start = 0.2f + i * 0.32f;
            Globe(c, "energy", Shell, start, 0.45f, i % 3 == 0 ? Gold : Chrono, k => new Vector3(side * 1.6f * Out(k), 0.85f, -0.3f * k), _ => 1f, k => (1f - k) * 0.8f, mesh: _ghost, gain: 1.2f);
        }
        for (var i = 0; i < 30; i++)
        {
            var high = Rand(0.2f, 2.2f);
            var side = i % 2 == 0 ? 1f : -1f;
            Line(c, Rand(0f, Life - 0.3f), 0.3f, i % 2 == 0 ? Chrono : Gold, k => new Vector3(side * (0.4f + 2.2f * k), high, 0f), _ => Vector3.Right * side * 0.9f, 0.04f, k => Fade(k, 0.2f, 0.4f));
        }
        Twinkle(c, 18, 0f, Life - 0.4f, Gold, () => Within(Vector3.Zero, reach * 1.3f, 0.3f, 3f), 0.45f);
        Shine(c, Vector3.Up, r, Chrono.Main, age => 0.9f * Fade(age / Life, 0.06f, 0.15f));
    }
}
