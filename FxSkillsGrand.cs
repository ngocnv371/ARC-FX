using Godot;

namespace Arc.Fx;

/// <summary>
/// The fifth set of skills, and the finer pieces the last hundred are built from: ornate marks
/// on the ground (fx_mark: a lotus, a star, wings, a dial, a snowflake, a skull), small pictures
/// in the air (fx_glyph: glints, petals, feathers, butterflies, skulls, wraiths) and the ways
/// they are scattered. This part holds the summoner's skills: ghostly minions, a raging aura
/// of spirits, and skulls that fly screaming at what is ahead.
/// </summary>
internal sealed partial class FxSkills
{
    private static readonly Tone Spirit = new(new(0.4f, 1f, 0.85f), new(0.92f, 1f, 0.98f), new(0.02f, 0.22f, 0.22f));
    private static readonly Tone Bone = new(new(0.85f, 0.82f, 0.68f), new(1f, 1f, 0.92f), new(0.25f, 0.2f, 0.12f));
    private static readonly Tone Stellar = new(new(1f, 0.9f, 0.55f), new(1f, 1f, 0.95f), new(0.35f, 0.22f, 0.5f));
    private static readonly Tone Nebula = new(new(0.95f, 0.4f, 0.85f), new(1f, 0.88f, 1f), new(0.15f, 0.1f, 0.5f));
    private static readonly Tone Amethyst = new(new(0.7f, 0.42f, 1f), new(0.96f, 0.88f, 1f), new(0.2f, 0.06f, 0.4f));
    private static readonly Tone Pearl = new(new(0.92f, 0.88f, 1f), new(1f, 1f, 1f), new(0.35f, 0.3f, 0.5f));
    private static readonly Tone Dawn = new(new(1f, 0.72f, 0.45f), new(1f, 0.96f, 0.8f), new(0.5f, 0.15f, 0.2f));
    private static readonly Tone Honey = new(new(1f, 0.72f, 0.12f), new(1f, 0.95f, 0.6f), new(0.45f, 0.2f, 0f));
    private static readonly Tone Mint = new(new(0.5f, 1f, 0.78f), new(0.94f, 1f, 0.96f), new(0.05f, 0.3f, 0.25f));
    private static readonly Tone Aurora = new(new(0.3f, 1f, 0.7f), new(0.85f, 0.9f, 1f), new(0.25f, 0.1f, 0.55f));

    // fx_mark styles.
    private const int Lotus = 0, Pentacle = 1, Rays = 2, Ripples = 3, Wings = 4, Dial = 5, Frostflake = 6, Cobweb = 7,
        Galaxy = 8, Comb = 9, Corona = 10, Caustics = 11, Deathmark = 12, Filigree = 13, Reticle = 14, Sunburst = 15;
    // fx_glyph styles.
    private const int Glint = 0, Leaflet = 1, Bubble = 2, Skull = 3, Wraith = 4, Plume = 5, Heart = 6, Puff = 7,
        Flutter = 8, Sickle = 9, Gem = 10, Snow = 11, Eye = 12, Crow = 13, Wisp = 14, Coin = 15;

    private void Grand(FxSkill skill, Cast c, float r, float range)
    {
        switch (skill)
        {
            case FxSkill.GhostLegion: GhostLegion(c, r); break;
            case FxSkill.RagingSpirits: RagingSpirits(c, r); break;
            case FxSkill.WrathAura: WrathAura(c, r); break;
            case FxSkill.SkullBarrage: SkullBarrage(c, range); break;
            case FxSkill.GraveCall: GraveCall(c, r); break;
            case FxSkill.SpiritGuard: SpiritGuard(c, r); break;
            case FxSkill.PhantomHost: PhantomHost(c, r, range); break;
            case FxSkill.CrowMurder: CrowMurder(c, r); break;
            case FxSkill.HauntedGround: HauntedGround(c, r); break;
            case FxSkill.BansheeWail: BansheeWail(c, r); break;
            case FxSkill.SoulBomber: SoulBomber(c, r); break;
            case FxSkill.DeathsJaw: DeathsJaw(c, r); break;
            case FxSkill.ReaperShade: ReaperShade(c, r); break;
            case FxSkill.SkullRain: SkullRain(c, r); break;
            case FxSkill.Possession: Possession(c, r); break;
            case FxSkill.NetherGate: NetherGate(c, r); break;
            default: Grace(skill, c, r, range); break;
        }
    }

    // ---- Pieces.

    /// <summary>A basis that stands a disc or a mark upright facing <paramref name="facing"/>, its +Z up.</summary>
    private static Basis Stand(Vector3 facing)
    {
        var f = FxKit.Flat(facing);
        f = f.LengthSquared() < 1e-6f ? Vector3.Back : f.Normalized();
        return new Basis(f.Cross(Vector3.Up), f, Vector3.Up);
    }

    /// <summary>
    /// An ornament (fx_mark) at <paramref name="at"/>, lying flat unless <paramref name="basis"/>
    /// stands it up, <paramref name="size"/> metres round, drawn out over a third of a second
    /// unless <paramref name="progress"/> says otherwise.
    /// </summary>
    private void Mark(Cast c, float start, float life, int style, Tone tone, Vector3 at, Func<float, float> size, Func<float, float> alpha,
        Func<float, float>? progress = null, float count = 8f, float spin = 1f, Basis? basis = null, float gain = 1f)
    {
        var turn = basis ?? Basis.Identity;
        var lift = basis == null ? Vector3.Up * Lift : Vector3.Zero;
        Mark(c, start, life, style, tone, k => new Transform3D(turn * Basis.FromScale(Vector3.One * Mathf.Max(size(k), 0.05f)), at + lift), alpha, progress, count, spin, gain);
    }

    /// <summary>An ornament placed every frame: a unit of <paramref name="place"/>'s scale is a metre of radius.</summary>
    private void Mark(Cast c, float start, float life, int style, Tone tone, Func<float, Transform3D> place, Func<float, float> alpha,
        Func<float, float>? progress = null, float count = 8f, float spin = 1f, float gain = 1f)
    {
        var material = Material("mark", style, tone, gain, ("uCount", count), ("uSpin", spin));
        Add(c, Kit.Ground, material, start, life, (n, k, t) =>
        {
            n.Transform = place(k);
            Look(n, alpha(k), progress?.Invoke(k) ?? Mathf.Min(t / 0.35f, 1f));
        });
    }

    /// <summary>
    /// A small picture (fx_glyph) square on to the camera, <paramref name="size"/> tall and
    /// <paramref name="slim"/> of that wide, its top toward <paramref name="up"/> as the camera sees it.
    /// </summary>
    private void Glyph(Cast c, float start, float life, int style, Tone tone, Func<float, Vector3> at, Func<float, float> size, Func<float, float> alpha,
        Func<float, Vector3>? up = null, float gain = 1.3f, float slim = 1f)
    {
        var material = Material("glyph", style, tone, gain);
        var seed = Rand(0f, 1f);
        Add(c, Kit.Quad, material, start, life, (n, k, _) =>
        {
            var s = Mathf.Max(size(k), 1e-3f);
            var y = up?.Invoke(k) ?? Vector3.Up;
            y = y.LengthSquared() < 1e-8f ? Vector3.Up : y.Normalized();
            n.Transform = new Transform3D(Along(y * s, s * slim, 1e-3f), at(k));
            Look(n, alpha(k), k);
            n.SetInstanceShaderParameter("uSeed", seed);
        });
    }

    /// <summary>A picture that pops up, swells a little and fades.</summary>
    private void Glyph(Cast c, float start, float life, int style, Tone tone, Vector3 at, float size, float gain = 1.4f) =>
        Glyph(c, start, life, style, tone, _ => at, k => size * (0.5f + 0.5f * Overshoot(Mathf.Min(k * 3f, 1f))), k => Fade(k, 0f, 0.5f), gain: gain);

    /// <summary><paramref name="count"/> glints twinkling one after another between <paramref name="from"/> and <paramref name="to"/>, each where <paramref name="where"/> puts it.</summary>
    private void Twinkle(Cast c, int count, float from, float to, Tone tone, Func<Vector3> where, float size = 0.5f, float each = 0.45f)
    {
        for (var i = 0; i < count; i++)
        {
            var at = where();
            var s = size * Rand(0.6f, 1.3f);
            Glyph(c, Rand(from, Mathf.Max(from, to)), each * Rand(0.7f, 1.3f), Glint, tone, _ => at, k => s * Mathf.Sin(k * Mathf.Pi), _ => 1f, gain: 1.6f);
        }
    }

    /// <summary>
    /// <paramref name="count"/> pictures set adrift between <paramref name="from"/> and
    /// <paramref name="to"/>: each starts <paramref name="where"/>, goes off at
    /// <paramref name="velocity"/>, falls by <paramref name="gravity"/>, sways, and (a petal, a
    /// feather) tumbles as it goes.
    /// </summary>
    private void Drift(Cast c, int count, float from, float to, float life, int style, Tone[] tones, Func<Vector3> where, Func<Vector3> velocity, float size,
        float gravity = 0f, float sway = 0.3f, float slim = 1f, bool tumble = true, float gain = 1.4f)
    {
        for (var i = 0; i < count; i++)
        {
            var p0 = where();
            var v = velocity();
            var phase = Rand(0f, Mathf.Tau);
            var side = Around(Rand(0f, Mathf.Tau));
            var s = size * Rand(0.7f, 1.3f);
            var roll = Rand(2f, 5f) * (i % 2 == 0 ? 1f : -1f);
            var mine = life * Rand(0.8f, 1.2f);
            Func<float, Vector3> at = k =>
            {
                var t = k * mine;
                return p0 + v * t + Vector3.Down * 0.5f * gravity * t * t + side * sway * Mathf.Sin(t * 4f + phase);
            };
            Func<float, Vector3>? up = tumble ? k => side * Mathf.Sin(phase + k * mine * roll) + Vector3.Up * Mathf.Cos(phase + k * mine * roll) : null;
            Glyph(c, Rand(from, Mathf.Max(from, to)), mine, style, tones[i % tones.Length], at, _ => s, k => Fade(k, 0.15f, 0.3f), up, gain, slim);
        }
    }

    /// <summary>Sparks thrown out all round from a point.</summary>
    private void Spray(Cast c, float at, Tone tone, Vector3 from, int count, float speed, float up = 4f, float life = 0.5f, float width = 0.06f)
    {
        for (var i = 0; i < count; i++)
            Spark(c, at, Rand(life * 0.7f, life * 1.3f), tone, from, Around(Rand(0f, Mathf.Tau)) * Rand(speed * 0.4f, speed) + Vector3.Up * Rand(up * 0.5f, up), Gravity, width, 0.035f);
    }

    /// <summary>A blow landing: a ring, a sunburst on the ground, a flash and sparks.</summary>
    private void Hit(Cast c, float at, Tone tone, Vector3 spot, float r)
    {
        var ground = FxKit.Flat(spot);
        Disc(c, at, 0.4f, Shock, tone, ground, k => r * 1.2f * Out(k), k => 1f - k, band: 0.14f, gain: 1.4f);
        Mark(c, at, 0.38f, Rays, tone, ground, _ => r * 1.15f, k => (1f - k) * (1f - k), k => Out(Mathf.Min(k * 2.5f, 1f)), count: 14f, gain: 1.3f);
        Flare(c, at, 0.25f, Star, tone, spot + Vector3.Up * 0.3f, r * 1.6f);
        Spray(c, at, tone, spot + Vector3.Up * 0.2f, 8 + (int)(r * 6f), 2f + r * 2f);
    }

    /// <summary>A ribbon streaming off a moving point: the last <paramref name="tail"/> of its window's path.</summary>
    private void Trail(Cast c, float start, float life, Tone tone, Func<float, Vector3> at, float width, float tail = 0.25f, int style = 0, float gain = 1.4f) =>
        Ribbon(c, start, life, style, tone, at, k => (Mathf.Max(0f, k - tail), k), width, k => Fade(k, 0f, 0.1f), gain);

    /// <summary>A point on the curve from <paramref name="a"/> to <paramref name="d"/> pulled toward <paramref name="b"/> then <paramref name="e"/>.</summary>
    private static Vector3 Curve(Vector3 a, Vector3 b, Vector3 e, Vector3 d, float k)
    {
        var u = 1f - k;
        return a * (u * u * u) + b * (3f * u * u * k) + e * (3f * u * k * k) + d * (k * k * k);
    }

    /// <summary>A skull flying along <paramref name="at"/>, leaning into its flight, fire streaming off it.</summary>
    private void FlyingSkull(Cast c, float start, float life, Tone tone, Func<float, Vector3> at, float size, float tail = 0.3f)
    {
        Trail(c, start, life, tone, at, size * 1.1f, tail, style: 1, gain: 1.5f);
        Trail(c, start, life, tone with { Main = tone.Hot }, at, size * 0.4f, tail * 0.7f, gain: 1.2f);
        Flare(c, start, life, Mote, tone, at, _ => size * 1.9f, k => Fade(k, 0.1f, 0.1f) * 0.55f, gain: 1f);
        Glyph(c, start, life, Skull, tone, at, k => size * Out(Mathf.Min(k * life / 0.12f, 1f)), k => Fade(k, 0f, 0.05f), k => Vector3.Up + Heading(at, k, 0.6f), gain: 1.7f);
    }

    /// <summary>A ghost standing on <paramref name="feet"/>, <paramref name="tall"/> high, rising out of the ground as it comes, a glow under it.</summary>
    private void Ghost(Cast c, float start, float life, Tone tone, Func<float, Vector3> feet, float tall, float gain = 1.6f)
    {
        Func<float, float> grown = k => tall * Out(Mathf.Min(k * life / 0.3f, 1f));
        Glyph(c, start, life, Wraith, tone, k => feet(k) + Vector3.Up * (grown(k) * 0.5f + 0.1f), grown, k => Fade(k, 0.04f, 0.12f),
            k => Vector3.Up * 2f + Heading(feet, k, 0.5f), gain, slim: 0.75f);
        Disc(c, start, life, Glow, tone, k => Lay(FxKit.Flat(feet(k)), tall * 0.45f), k => Fade(k, 0.1f, 0.15f) * 0.6f);
    }

    /// <summary>Wisps of spirit rising off the ground round <paramref name="centre"/>.</summary>
    private void Wisps(Cast c, int count, float from, float to, Tone tone, Vector3 centre, float r, float rise = 1.6f)
    {
        for (var i = 0; i < count; i++)
        {
            var spot = centre + Around(Rand(0f, Mathf.Tau)) * r * Mathf.Sqrt(Rand(0f, 1f));
            var high = rise * Rand(0.6f, 1.2f);
            var s = Rand(0.3f, 0.5f);
            Glyph(c, Rand(from, Mathf.Max(from, to)), Rand(0.7f, 1f), Wisp, tone, k => spot + Vector3.Up * (0.15f + high * k), k => s * (1f - 0.4f * k), k => Fade(k, 0.2f, 0.4f), gain: 1.4f);
        }
    }

    // ---- The summoner.

    private void GhostLegion(Cast c, float r)
    {
        const int Ghosts = 5;
        const float Rise = 0.5f, March = 1.2f, Life = Rise + March;
        var reach = r * 1.5f;
        Mark(c, 0f, Life, Deathmark, Spirit, new Vector3(0f, 0f, 0.4f), _ => r * 0.6f, k => Fade(k, 0.05f, 0.4f), spin: 1f, gain: 1.2f);
        for (var i = 0; i < Ghosts; i++)
        {
            var x = (i - (Ghosts - 1) * 0.5f) * r * 0.42f;
            var start = Mathf.Abs(i - (Ghosts - 1) * 0.5f) * 0.07f;
            var from = new Vector3(x, 0f, 0.5f - Mathf.Abs(x) * 0.25f);
            var to = new Vector3(x * 1.15f, 0f, reach - Mathf.Abs(x) * 0.25f);
            var life = Life - start;
            Func<float, Vector3> feet = k => from.Lerp(to, Mathf.SmoothStep(0f, 1f, Mathf.Clamp((k * life - Rise) / (life - Rise), 0f, 1f)));
            Mark(c, start, Rise + 0.5f, Filigree, Spirit, from, _ => 0.6f, k => Fade(k, 0.1f, 0.5f), count: 6f, gain: 1.3f);
            Column(c, start, Rise, Light, Spirit, from, k => new Vector2(0.32f, 2.6f * Out(k)), k => 1f - k, gain: 1.5f, speed: 2f);
            Ghost(c, start, life, Spirit, feet, 1.9f);
            Trail(c, start + Rise, life - Rise, Spirit, k => feet((Rise + k * (life - Rise)) / life) + Vector3.Up * 0.5f, 0.7f, 0.35f, gain: 1.1f);
            Crescent(c, start + life - 0.14f, 0.3f, new Transform3D(new Basis(Vector3.Forward, Mathf.DegToRad(i % 2 == 0 ? 35f : -35f)), to + new Vector3(0f, 1f, -0.6f)), 1.3f, 140f, Spirit, thick: 0.3f, sweep: 0.35f, gain: 1.5f);
            Hit(c, Life, Spirit, to + Vector3.Up * 0.5f, 0.7f);
        }
        Wisps(c, 24, 0f, Life - 0.6f, Spirit, new Vector3(0f, 0f, reach * 0.5f), r * 0.9f);
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), r * 1.8f, Spirit.Main, age => age < Life ? 0.9f * Mathf.Min(age / 0.3f, 1f) : 2f * Mathf.Max(0f, 1f - (age - Life) / 0.4f));
    }

    private void RagingSpirits(Cast c, float r)
    {
        const int Skulls = 3;
        const float Hang = 0.35f, Flight = 0.75f;
        var chest = Vector3.Up * 1.3f;
        Mark(c, 0f, 1.3f, Pentacle, Rage, Vector3.Zero, _ => r * 0.42f, k => Fade(k, 0.05f, 0.5f), count: 5f, spin: 2f, gain: 1.4f);
        Column(c, 0f, 0.6f, Fire, Ember, Vector3.Zero, k => new Vector2(0.7f, 2.4f * Out(Mathf.Min(k * 4f, 1f))), k => 1f - k, gain: 1.4f, speed: 2.5f);
        Flare(c, 0f, 0.3f, Star, Rage, chest, 2.8f);
        for (var i = 0; i < Skulls; i++)
        {
            var side = i - (Skulls - 1) * 0.5f;
            var start = i * 0.12f;
            var perch = new Vector3(side * 1.5f, 2.3f + Mathf.Abs(side) * 0.3f, -0.3f);
            var foe = new Vector3(side * 1.1f, 0.9f, r * 1.25f);
            var life = Hang + Flight;
            Func<float, Vector3> at = k =>
            {
                var t = k * life;
                if (t < Hang)
                    return chest.Lerp(perch, Out(t / Hang)) + Vector3.Up * 0.08f * Mathf.Sin(t * 30f);
                var s = (t - Hang) / Flight;
                return Curve(perch, perch + new Vector3(side * 1.6f, 0.8f, 0.6f), foe + new Vector3(side * 0.8f, 1.2f, -1.6f), foe, s * s * (3f - 2f * s));
            };
            FlyingSkull(c, start, life, Ember, at, 0.95f);
            for (var j = 0; j < 8; j++)
            {
                var t = Rand(Hang, life);
                Spark(c, start + t, Rand(0.3f, 0.5f), Flame, at(t / life), FxKit.Jitter(1.5f) + Vector3.Up * 1.5f, Gravity * 0.3f, 0.06f, 0.04f);
            }
            var land = start + life;
            Hit(c, land, Ember, foe, 0.9f);
            Cue(c, land, () => Stage.Explosion(At(c, foe), 0.9f, Ember.Main, FxBurst.Fire));
            Glyph(c, land, 0.4f, Skull, Rage, foe + Vector3.Up * 0.5f, 1.8f);
        }
        Shine(c, new Vector3(0f, 1.2f, r * 0.6f), r * 1.8f, Ember.Main, age => 1.1f + 1.2f * Bump(age, Hang + Flight, 0.5f));
    }

    private void WrathAura(Cast c, float r)
    {
        const float Life = 3.2f;
        var reach = r * 0.5f;
        Mark(c, 0f, Life, Deathmark, Spirit, Vector3.Zero, _ => reach * 1.15f, k => Fade(k, 0.05f, 0.15f), gain: 1.2f);
        Disc(c, 0f, Life, Swirl, Spirit, Vector3.Zero, _ => reach * 1.5f, k => Fade(k, 0.08f, 0.15f) * 0.5f, spin: -6f);
        Column(c, 0f, Life, Fire, Spirit, Vector3.Zero, k => new Vector2(0.75f + 0.06f * Mathf.Sin(k * Life * 11f), 2.7f), k => Fade(k, 0.06f, 0.12f) * 0.85f, gain: 1.5f, speed: 2.4f);
        Column(c, 0f, Life, Spiral, Soul, Vector3.Zero, _ => new Vector2(1f, 2.4f), k => Fade(k, 0.06f, 0.12f) * 0.6f, twist: 2f, speed: 2.5f, gain: 1.2f);
        // Howling faces whirl up round the caster, one after another.
        for (var i = 0; i < 16; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var start = i * (Life - 1f) / 16f;
            var turn = Rand(4f, 6f) * (i % 2 == 0 ? 1f : -1f);
            var far = Rand(0.8f, 1.25f);
            Func<float, Vector3> at = k => Around(phase + k * turn) * far * (1f - 0.3f * k) + Vector3.Up * (0.3f + 2.9f * k);
            Glyph(c, start, 1f, i % 4 == 0 ? Skull : Wraith, i % 2 == 0 ? Spirit : Soul, at, k => 0.95f + 0.4f * k, k => Fade(k, 0.2f, 0.4f), k => Vector3.Up + Heading(at, k, 0.8f), gain: 1.5f, slim: 0.8f);
            Trail(c, start, 1f, Spirit, at, 0.4f, 0.3f, gain: 1f);
        }
        for (var t = 0.1f; t < Life - 0.4f; t += 0.55f)
            Disc(c, t, 0.5f, Shock, Spirit, Vector3.Zero, k => reach * 2f * Out(k), k => (1f - k) * 0.8f, band: 0.12f, gain: 1.2f);
        Wisps(c, 26, 0f, Life - 0.9f, Spirit, Vector3.Zero, reach * 1.3f, 2.2f);
        Glyph(c, 0f, Life, Skull, Spirit, k => Vector3.Up * (3.1f + 0.1f * Mathf.Sin(k * Life * 5f)), k => 1.25f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f)) * (1f + 0.07f * Mathf.Sin(k * Life * 13f)),
            k => Fade(k, 0.05f, 0.1f) * 0.9f, gain: 1.6f);
        Shine(c, Vector3.Up, r * 1.2f, Spirit.Main, age => (1.1f + 0.3f * Mathf.Sin(age * 11f)) * Fade(age / Life, 0.06f, 0.12f));
    }

    private void SkullBarrage(Cast c, float range)
    {
        const int Skulls = 5;
        const float Flight = 0.5f, Every = 0.11f;
        var length = Mathf.Max(range, 2f);
        var from = new Vector3(0f, 1.3f, 0.4f);
        Mark(c, 0f, Skulls * Every + 0.5f, Pentacle, Soul, new Vector3(0f, 1.3f, 0.25f), _ => 0.95f, k => Fade(k, 0.08f, 0.3f), count: 5f, spin: 4f, basis: Stand(Vector3.Back), gain: 1.5f);
        for (var i = 0; i < Skulls; i++)
        {
            var start = i * Every;
            var bend = (i % 2 == 0 ? 1f : -1f) * Rand(0.5f, 1.1f);
            var to = new Vector3(Rand(-0.5f, 0.5f), 1f, length);
            var o = from + new Vector3(Rand(-0.3f, 0.3f), Rand(-0.2f, 0.3f), 0f);
            Func<float, Vector3> at = k => o.Lerp(to, k) + new Vector3(bend, 0.4f, 0f) * Mathf.Sin(k * Mathf.Pi);
            FlyingSkull(c, start, Flight, Soul, at, 0.8f);
            Flare(c, start, 0.15f, Star, Soul, o, 1.4f);
            Hit(c, start + Flight, Soul, to, 0.8f);
            Glyph(c, start + Flight, 0.3f, Skull, Doom, to + Vector3.Up * 0.3f, 1.3f);
        }
        var end = new Vector3(0f, 0f, length);
        Cue(c, (Skulls - 1) * Every + Flight, () => Stage.Explosion(At(c, end + Vector3.Up), 1.1f, Soul.Main, FxBurst.Dome));
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Soul.Main, age => 1.1f * Fade(age / (Skulls * Every + Flight + 0.3f), 0.05f, 0.3f));
    }

    private void GraveCall(Cast c, float r)
    {
        const float Dig = 0.5f, Stand1 = 1.5f, Gone = 0.35f;
        var graves = new[] { new Vector3(-r * 0.5f, 0f, r * 0.35f), new Vector3(0f, 0f, r * 0.75f), new Vector3(r * 0.5f, 0f, r * 0.35f) };
        Mark(c, 0f, Dig + Stand1, Filigree, Bone, new Vector3(0f, 0f, r * 0.5f), _ => r * 0.85f, k => Fade(k, 0.05f, 0.3f) * 0.8f, count: 12f);
        for (var i = 0; i < graves.Length; i++)
        {
            var grave = graves[i];
            var start = i * 0.12f;
            Mark(c, start, Dig + Stand1, Deathmark, Bone, grave, _ => 0.85f, k => Fade(k, 0.05f, 0.3f), gain: 1.3f);
            Disc(c, start, Dig + 0.8f, Cracks, Spirit, grave, _ => 0.9f, k => Fade(k, 0.05f, 0.4f), k => k);
            for (var j = 0; j < 8; j++)
                Spark(c, start + Rand(0.1f, Dig), Rand(0.4f, 0.6f), Rubble, grave + Around(Rand(0f, Mathf.Tau)) * 0.3f, FxKit.Jitter(1.5f) + Vector3.Up * Rand(3f, 5f), Gravity, 0.1f, 0.02f);
            Column(c, start + Dig, 0.6f, Light, Spirit, grave, k => new Vector2(0.45f, 4f * Out(Mathf.Min(k * 4f, 1f))), k => 1f - k, gain: 1.6f, speed: 3f);
            Disc(c, start + Dig, 0.4f, Shock, Spirit, grave, k => 1.5f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
            Ghost(c, start + Dig, Stand1, i == 1 ? Spirit : Soul, _ => grave, i == 1 ? 2.3f : 1.9f);
            Glyph(c, start + Dig + Stand1 - 0.05f, Gone, Wraith, Spirit, k => grave + Vector3.Up * (1.1f + 1.5f * k), k => 2f + k, k => (1f - k) * 0.7f, slim: 0.75f);
            Spray(c, start + Dig + Stand1, Spirit, grave + Vector3.Up, 10, 2f, 3f);
        }
        Wisps(c, 20, Dig, Dig + Stand1 - 0.6f, Spirit, new Vector3(0f, 0f, r * 0.5f), r * 0.8f);
        Shine(c, new Vector3(0f, 1f, r * 0.5f), r * 1.6f, Spirit.Main, age => 1.1f * Fade(age / (Dig + Stand1 + 0.4f), 0.2f, 0.2f));
    }

    private void SpiritGuard(Cast c, float r)
    {
        const int Skulls = 4;
        const float Life = 3.2f, Turn = 3.2f;
        var ring = r * 0.5f;
        Mark(c, 0f, Life, Filigree, Spirit, Vector3.Zero, _ => ring * 1.25f, k => Fade(k, 0.05f, 0.15f), count: 8f, spin: 3f, gain: 1.2f);
        Disc(c, 0f, Life, Glow, Spirit, Vector3.Zero, _ => ring * 1.2f, k => Fade(k, 0.08f, 0.15f) * 0.35f);
        for (var i = 0; i < Skulls; i++)
        {
            var phase = i * Mathf.Tau / Skulls;
            Func<float, Vector3> at = k => Around(phase + k * Life * Turn) * ring * Out(Mathf.Min(k * Life / 0.3f, 1f)) + Vector3.Up * (1.2f + 0.2f * Mathf.Sin(k * Life * 4f + phase));
            FlyingSkull(c, 0f, Life, Spirit, at, 0.75f, 0.09f);
            var last = at(1f);
            var away = FxKit.Flat(last).Normalized();
            Glyph(c, Life, 0.35f, Skull, Spirit, k => last + away * 2.5f * Out(k), k => 0.75f + 0.6f * k, k => 1f - k, gain: 1.6f);
            Spray(c, Life, Spirit, last, 8, 3f, 2f);
        }
        Disc(c, Life, 0.4f, Shock, Spirit, Vector3.Zero, k => r * 0.9f * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
        Wisps(c, 14, 0f, Life - 0.9f, Spirit, Vector3.Zero, ring, 1.4f);
        Shine(c, Vector3.Up, r, Spirit.Main, age => 0.9f * Fade(age / Life, 0.06f, 0.1f));
    }

    private void PhantomHost(Cast c, float r, float range)
    {
        const int Ghosts = 7;
        const float Life = 1.5f;
        var length = Mathf.Clamp(range, 3f, r * 2.5f);
        var half = r * 0.8f;
        Func<float, float> front = k => 0.4f + (length - 0.4f) * Mathf.SmoothStep(0f, 1f, k);
        Wall(c, 0f, Life, 0, Spirit, k => (new Vector3(-half, 0f, front(k) - 0.4f), new Vector3(half, 0f, front(k) - 0.4f)), _ => 1.5f, k => Fade(k, 0.1f, 0.2f) * 0.35f, gain: 1.1f);
        for (var i = 0; i < Ghosts; i++)
        {
            var x = (i - (Ghosts - 1) * 0.5f) / ((Ghosts - 1) * 0.5f) * half;
            var lag = Rand(0f, 0.5f);
            var wob = (float)i;
            var tall = Rand(1.8f, 2.4f);
            Func<float, Vector3> feet = k => new Vector3(x + 0.15f * Mathf.Sin(k * 9f + wob), 0f, front(k) - lag);
            Ghost(c, Rand(0f, 0.1f), Life, i % 3 == 1 ? Soul : Spirit, feet, tall);
            Trail(c, 0f, Life, Spirit, k => feet(k) + Vector3.Up * 0.6f, 0.8f, 0.25f, gain: 0.9f);
        }
        Drift(c, 26, 0f, Life - 0.5f, 0.8f, Puff, [Spirit, Smoke], () => new Vector3(Rand(-half, half), Rand(0.2f, 0.8f), front(Rand(0f, 0.8f)) - 0.6f), () => new Vector3(0f, 0.6f, -0.5f), 1.3f, tumble: false, gain: 0.7f);
        Disc(c, 0f, Life + 0.4f, Glow, Spirit, new Vector3(0f, 0f, length * 0.5f), _ => 1f, k => Fade(k, 0.1f, 0.5f) * 0.45f, stretch: new Vector3(half, 1f, length * 0.5f));
        var end = new Vector3(0f, 0f, length);
        Disc(c, Life - 0.1f, 0.5f, Shock, Spirit, end, k => r * 1.1f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        Wisps(c, 16, Life - 0.2f, Life + 0.1f, Spirit, end, half);
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Spirit.Main, age => 1f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void CrowMurder(Cast c, float r)
    {
        const float Life = 1.3f;
        var reach = r * 1.7f;
        var from = new Vector3(0f, 1.2f, 0.2f);
        Disc(c, 0f, 0.9f, Cone, Shadow, k => Lay(Vector3.Zero, reach), k => Fade(k, 0.02f, 0.5f) * 0.7f, k => Out(Mathf.Min(k * 2f, 1f)), cone: 0.55f, gain: 1.1f);
        Flare(c, 0f, 0.3f, Star, Shadow, from, 2.6f);
        Disc(c, 0f, 0.4f, Shock, Shadow, Vector3.Zero, k => 1.6f * Out(k), k => 1f - k, band: 0.16f);
        for (var i = 0; i < 22; i++)
        {
            var to = Around(Rand(-0.5f, 0.5f)) * reach * Rand(0.75f, 1.1f) + Vector3.Up * Rand(0.6f, 2.4f);
            var bend = new Vector3(Rand(-1.2f, 1.2f), Rand(0.2f, 1.4f), 0f);
            var start = Rand(0f, 0.35f);
            var life = Rand(0.6f, 0.85f);
            var size = Rand(0.7f, 1.1f);
            Func<float, Vector3> at = k => from.Lerp(to, k) + bend * Mathf.Sin(k * Mathf.Pi);
            Glyph(c, start, life, Crow, i % 3 == 0 ? Void : Shadow, at, _ => size, k => Fade(k, 0.1f, 0.25f), gain: 1.7f);
            Line(c, start, life, Shadow, k => at(Mathf.Max(k - 0.07f, 0f)), k => Heading(at, k, 0.9f), 0.07f, k => Fade(k, 0.1f, 0.3f) * 0.8f);
        }
        Drift(c, 24, 0.2f, Life - 0.5f, 0.9f, Plume, [Shadow, Void], () => Around(Rand(-0.5f, 0.5f)) * Rand(1f, reach) + Vector3.Up * Rand(1f, 2.4f), () => new Vector3(0f, -1.2f, 0.6f), 0.5f, slim: 0.45f);
        Hit(c, 0.55f, Shadow, new Vector3(0f, 0.8f, reach * 0.8f), 1.2f);
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Shadow.Main, age => 1.1f * Fade(age / Life, 0.05f, 0.4f));
    }

    private void HauntedGround(Cast c, float r)
    {
        const float Life = 3.4f;
        const int Ghosts = 6;
        Mark(c, 0f, Life, Deathmark, Soul, Vector3.Zero, _ => r * 0.6f, k => Fade(k, 0.05f, 0.15f), gain: 1.2f);
        Mark(c, 0f, Life, Filigree, Spirit, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.15f), count: 14f, spin: 2f);
        Disc(c, 0f, Life, Swirl, Void, Vector3.Zero, _ => r, k => Fade(k, 0.08f, 0.15f) * 0.6f, spin: 2.5f);
        Cue(c, 0.2f, () => Stage.Patch(At(c, Vector3.Zero), r * 0.95f, Life - 0.2f, Soul.Main, FxGround.Miasma));
        for (var i = 0; i < Ghosts; i++)
        {
            var phase = i * Mathf.Tau / Ghosts;
            var far = r * (i % 2 == 0 ? 0.78f : 0.5f);
            var turn = i % 2 == 0 ? 1.1f : -1.5f;
            Func<float, Vector3> feet = k => Around(phase + k * Life * turn) * far;
            Ghost(c, i * 0.08f, Life - i * 0.08f, i % 2 == 0 ? Spirit : Soul, feet, i % 2 == 0 ? 2f : 1.6f);
        }
        Wisps(c, 40, 0f, Life - 0.9f, Spirit, Vector3.Zero, r * 0.95f);
        Twinkle(c, 16, 0.2f, Life - 0.5f, Spirit, () => Around(Rand(0f, Mathf.Tau)) * Rand(0f, r) + Vector3.Up * Rand(0.3f, 2f), 0.5f);
        Shine(c, Vector3.Up, r * 1.4f, Soul.Main, age => 0.9f * Fade(age / Life, 0.08f, 0.15f));
    }

    private void BansheeWail(Cast c, float r)
    {
        const float Draw = 0.35f, Wail = 1.1f;
        var reach = r * 1.7f;
        var mouth = new Vector3(0f, 1.5f, 0.6f);
        Glyph(c, 0f, Draw + Wail, Wraith, Spirit, k => mouth + Vector3.Up * (0.25f + 0.1f * Mathf.Sin(k * 30f)), k => 3f * Overshoot(Mathf.Min(k * (Draw + Wail) / Draw, 1f)), k => Fade(k, 0.05f, 0.2f), gain: 1.8f, slim: 0.8f);
        Flare(c, 0f, Draw, Mote, Spirit, _ => mouth, k => 2f - 1.4f * k, k => k);
        Disc(c, Draw, Wail, Cone, Spirit, k => Lay(Vector3.Zero, reach), k => Fade(k, 0.02f, 0.4f) * 0.8f, k => Out(Mathf.Min(k * 2.5f, 1f)), cone: 0.5f, gain: 1.2f);
        // Rings of sound roll out along the cone, widening.
        for (var i = 0; i < 7; i++)
        {
            var start = Draw + i * 0.12f;
            Mark(c, start, 0.55f, Ripples, i % 2 == 0 ? Spirit : Soul, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * (0.5f + 1.6f * k)), mouth + Vector3.Back * reach * 0.9f * k + Vector3.Down * 0.5f * k),
                k => Fade(k, 0.1f, 0.5f), count: 2f, spin: 0f, gain: 1.5f);
            Glyph(c, start, 0.55f, Bubble, Spirit, k => mouth + Vector3.Back * reach * 0.9f * k + Vector3.Down * 0.5f * k, k => 1f + 3.4f * k, k => Fade(k, 0.1f, 0.5f) * 0.8f);
        }
        for (var i = 0; i < 30; i++)
        {
            var way = Around(Rand(-0.45f, 0.45f));
            var high = Rand(0.4f, 2f);
            Line(c, Draw + Rand(0f, Wail - 0.4f), 0.4f, Spirit, k => way * reach * (0.15f + 0.85f * k) + Vector3.Up * high, _ => way * 1.3f, 0.06f, k => Fade(k, 0.2f, 0.4f));
        }
        Disc(c, Draw, 0.4f, Shock, Spirit, Vector3.Zero, k => 2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        Wisps(c, 14, Draw, Draw + Wail - 0.5f, Spirit, new Vector3(0f, 0f, reach * 0.6f), reach * 0.4f);
        Shine(c, new Vector3(0f, 1f, reach * 0.4f), reach, Spirit.Main, age => age < Draw ? 0.5f : 1.5f * Fade((age - Draw) / Wail, 0f, 0.5f));
    }

    private void SoulBomber(Cast c, float r)
    {
        const float Rise = 0.3f, Run = 0.6f, Blow = Rise + Run;
        var from = new Vector3(0f, 0f, 0.6f);
        var to = new Vector3(0f, 0f, r * 1.15f);
        Func<float, Vector3> feet = k => from.Lerp(to, Mathf.Clamp((k * Blow - Rise) / Run, 0f, 1f) * Mathf.Clamp((k * Blow - Rise) / Run, 0f, 1f));
        Mark(c, 0f, Rise + 0.4f, Filigree, Soul, from, _ => 0.7f, k => Fade(k, 0.1f, 0.5f), count: 6f, gain: 1.3f);
        Disc(c, 0f, Blow, Telegraph, Soul, to, _ => r * 0.6f, _ => 0.7f, k => k);
        Ghost(c, 0f, Blow, Soul, feet, 1.7f);
        Trail(c, Rise, Run, Soul, k => feet((Rise + k * Run) / Blow) + Vector3.Up * 0.6f, 1f, 0.5f, gain: 1.3f);
        // It swells and flickers faster as it nears the spot.
        Globe(c, "energy", Orb, 0f, Blow, Soul, k => feet(k) + Vector3.Up * 0.9f, k => 0.25f + 0.3f * k * k, k => 0.4f + 0.6f * (0.5f + 0.5f * Mathf.Sin(k * k * 60f)), gain: 1.5f);
        var heart = to + Vector3.Up * 0.8f;
        Cue(c, Blow, () => Stage.Explosion(At(c, heart), r * 0.7f, Soul.Main, FxBurst.Dome));
        Hit(c, Blow, Soul, heart, r * 0.75f);
        Mark(c, Blow, 1.2f, Deathmark, Soul, to, _ => r * 0.55f, k => Fade(k, 0.02f, 0.6f), _ => 1f, gain: 1.3f);
        Glyph(c, Blow, 0.7f, Skull, Spirit, k => heart + Vector3.Up * (0.4f + 1.6f * Out(k)), k => 1.6f + 1.6f * Out(k), k => (1f - k) * (1f - k), gain: 1.7f);
        Wisps(c, 18, Blow, Blow + 0.3f, Soul, to, r * 0.6f, 2f);
        Shine(c, heart, r * 1.6f, Soul.Main, age => age < Blow ? 0.5f + 0.5f * age / Blow : 2.4f * Mathf.Max(0f, 1f - (age - Blow) / 0.6f));
    }

    private void DeathsJaw(Cast c, float r)
    {
        const float Warn = 0.55f, Bite = 0.45f, After = 0.8f;
        var spot = new Vector3(0f, 0f, r * 0.3f);
        Mark(c, 0f, Warn + Bite + After, Deathmark, Doom, spot, _ => r * 0.75f, k => Fade(k, 0.03f, 0.3f), k => Mathf.Min(k * (Warn + Bite + After) / Warn, 1f), gain: 1.3f);
        Disc(c, 0f, Warn + Bite, Glow, Doom, spot, _ => r * 0.75f, k => 0.25f + 0.5f * k);
        Wisps(c, 14, 0f, Warn, Doom, spot, r * 0.7f, 1f);
        // The skull heaves up out of the mark, hangs, and drops as its jaws shut.
        Glyph(c, Warn, Bite + 0.25f, Skull, Soul, k =>
        {
            var t = k * (Bite + 0.25f);
            var up = t < Bite ? Out(t / Bite) : 1f - (t - Bite) / 0.25f * 0.5f;
            return spot + Vector3.Up * (-0.6f + 2.4f * up);
        }, k => r * 1.25f * Mathf.Min(1f, 0.5f + k * 2f), k => Fade(k, 0.08f, 0.2f), gain: 1.9f);
        Column(c, Warn, Bite + 0.3f, Fire, Doom, spot, k => new Vector2(r * 0.55f, 3.4f * Out(Mathf.Min(k * 3f, 1f))), k => (1f - k) * 0.9f, gain: 1.3f, speed: 2.2f);
        var snap = Warn + Bite;
        Hit(c, snap, Soul, spot + Vector3.Up * 0.6f, r * 0.8f);
        Disc(c, snap, After, Cracks, Doom, spot, _ => r * 0.8f, k => Fade(k, 0.02f, 0.5f), k => k);
        Cue(c, snap, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.5f), r * 0.5f, Soul.Main, FxBurst.Dome));
        for (var i = 0; i < 26; i++)
            Spark(c, snap, Rand(0.5f, 0.8f), i % 2 == 0 ? Bone : Soul, spot + Vector3.Up * 0.5f, Around(Rand(0f, Mathf.Tau)) * Rand(2f, 6f) + Vector3.Up * Rand(3f, 8f), Gravity, 0.1f, 0.03f);
        Shine(c, spot + Vector3.Up, r * 1.6f, Soul.Main, age => age < snap ? 0.4f + 0.8f * age / snap : 2.4f * Mathf.Max(0f, 1f - (age - snap) / 0.6f));
    }

    private void ReaperShade(Cast c, float r)
    {
        const float Loom = 0.45f, Swing = 0.3f, Life = Loom + Swing + 0.5f;
        var back = new Vector3(0f, 0f, -0.9f);
        Mark(c, 0f, Life, Filigree, Doom, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.05f, 0.3f), count: 10f, gain: 1.2f);
        Ghost(c, 0f, Life, Doom, _ => back, 3.8f, gain: 1.8f);
        Glyph(c, 0f, Life, Skull, Soul, _ => back + Vector3.Up * 3.05f, k => 0.8f * Out(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.1f, 0.15f), gain: 1.6f);
        Wisps(c, 16, 0f, Loom, Doom, back, 1f, 2.4f);
        // Its scythe: a long shaft swung round with the crescent of the blade.
        Crescent(c, Loom, Swing + 0.14f, new Transform3D(new Basis(Vector3.Right, Mathf.DegToRad(-10f)), Vector3.Up * 1.1f), r, 240f, Doom, thick: 0.34f, sweep: 0.4f, gain: 1.7f);
        Crescent(c, Loom + 0.03f, Swing + 0.14f, new Transform3D(new Basis(Vector3.Right, Mathf.DegToRad(-10f)), Vector3.Up * 1.25f), r * 0.86f, 240f, Soul, thick: 0.16f, sweep: 0.45f, gain: 1.2f);
        Disc(c, Loom + 0.08f, 0.5f, Shock, Doom, Vector3.Zero, k => r * 1.15f * Out(k), k => 1f - k, band: 0.14f, gain: 1.4f);
        for (var i = 0; i < 9; i++)
        {
            var soul = Around(-1.8f + i * 0.45f) * r * 0.85f + Vector3.Up * 0.9f;
            var start = Loom + 0.1f + i * 0.02f;
            Func<float, Vector3> at = k => soul.Lerp(back + Vector3.Up * 2.6f, k * k) + Vector3.Up * Mathf.Sin(k * Mathf.Pi) * 0.8f;
            Glyph(c, start, 0.6f, Wisp, Soul, at, _ => 0.6f, k => Fade(k, 0.1f, 0.2f), gain: 1.6f);
            Trail(c, start, 0.6f, Soul, at, 0.3f, 0.3f);
            Flare(c, start - 0.05f, 0.25f, Star, Soul, soul, 1.2f);
        }
        Shine(c, Vector3.Up, r * 1.5f, Doom.Main, age => age < Loom ? 0.7f * age / Loom : 0.7f + 1.3f * Bump(age, Loom, 0.5f));
    }

    private void SkullRain(Cast c, float r)
    {
        const int Skulls = 8;
        const float Fall = 0.45f, Every = 0.1f;
        var centre = new Vector3(0f, 0f, r * 0.4f);
        Mark(c, 0f, Skulls * Every + Fall + 0.5f, Deathmark, Soul, centre, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f) * 0.9f, gain: 1.1f);
        var spots = Spread(Skulls, r * 0.8f);
        for (var i = 0; i < Skulls; i++)
        {
            var spot = centre + spots[i] + Vector3.Up * 0.4f;
            var sky = spot + new Vector3(Rand(-2f, 2f), 10f, -3f);
            var start = i * Every;
            Func<float, Vector3> at = k => sky.Lerp(spot, k * k);
            Disc(c, start, Fall, Telegraph, Soul, FxKit.Flat(spot), _ => 0.7f, _ => 0.7f, k => k);
            FlyingSkull(c, start, Fall, i % 2 == 0 ? Soul : Spirit, at, 0.9f, 0.4f);
            Hit(c, start + Fall, Soul, spot, 0.8f);
            Disc(c, start + Fall, 0.9f, Cracks, Soul, FxKit.Flat(spot), _ => 0.7f, k => Fade(k, 0.02f, 0.5f), k => k);
            Wisps(c, 3, start + Fall, start + Fall + 0.1f, Spirit, FxKit.Flat(spot), 0.4f, 1.4f);
        }
        Shine(c, centre + Vector3.Up, r * 1.6f, Soul.Main, age => 1.2f * Fade(age / (Skulls * Every + Fall + 0.4f), 0.1f, 0.3f));
    }

    private void Possession(Cast c, float r)
    {
        const float Flight = 0.55f, Hold = 1.5f;
        var from = new Vector3(0f, 1.2f, 0.3f);
        var foe = new Vector3(0f, 0f, r * 1.1f);
        var chest = foe + Vector3.Up * 1.1f;
        Func<float, Vector3> at = k => from.Lerp(chest, k) + new Vector3(0.9f * Mathf.Sin(k * Mathf.Tau), 0.7f * Mathf.Sin(k * Mathf.Pi), 0f);
        Flare(c, 0f, 0.25f, Star, Spirit, from, 2f);
        Glyph(c, 0f, Flight, Wraith, Spirit, at, k => 1.7f - 0.5f * k, k => Fade(k, 0.1f, 0.05f), k => Vector3.Up * 0.6f - Heading(at, k, 1f), gain: 1.7f, slim: 0.75f);
        Trail(c, 0f, Flight, Spirit, at, 0.8f, 0.4f, gain: 1.3f);
        Flare(c, Flight, 0.3f, Star, Spirit, chest, 3f);
        Disc(c, Flight, 0.4f, Shock, Spirit, foe, k => 2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        // The one possessed: a ghost flickering in and out over them, eyes burning above a seal.
        Mark(c, Flight, Hold, Pentacle, Spirit, foe, _ => 1.2f, k => Fade(k, 0.05f, 0.2f), count: 5f, spin: -3f, gain: 1.4f);
        Column(c, Flight, Hold, Fire, Spirit, foe, _ => new Vector2(0.6f, 2.5f), k => Fade(k, 0.08f, 0.2f) * 0.8f, gain: 1.4f, speed: 2f);
        Glyph(c, Flight, Hold, Wraith, Soul, k => chest + Vector3.Up * 0.25f, k => 2.5f + 0.2f * Mathf.Sin(k * Hold * 9f), k => Fade(k, 0.08f, 0.2f) * (0.55f + 0.45f * Mathf.Sin(k * Hold * 23f)), gain: 1.6f, slim: 0.75f);
        Glyph(c, Flight, Hold, Eye, Spirit, _ => chest + Vector3.Up * 1.6f, k => 0.9f * Overshoot(Mathf.Min(k * Hold / 0.25f, 1f)), k => Fade(k, 0.05f, 0.2f), gain: 1.6f);
        Wisps(c, 16, Flight, Flight + Hold - 0.8f, Spirit, foe, 0.9f, 2.2f);
        Shine(c, chest, r, Spirit.Main, age => age < Flight ? 0.5f : 1.3f * Fade((age - Flight) / Hold, 0.02f, 0.3f));
    }

    private void NetherGate(Cast c, float r)
    {
        const float Open = 0.4f, Pour = 1.5f, Life = Open + Pour + 0.3f;
        var gate = new Vector3(0f, 1.6f, 0.2f);
        var face = Stand(Vector3.Back);
        Func<float, float> wide = k => 1.6f * Overshoot(Mathf.Min(k * Life / Open, 1f)) * Mathf.Min(1f, (1f - k) * Life / 0.25f);
        Mark(c, 0f, Life, Filigree, Amethyst, k => new Transform3D(face * Basis.FromScale(Vector3.One * wide(k)), gate), k => Fade(k, 0.03f, 0.05f), count: 10f, spin: 5f, gain: 1.6f);
        Disc(c, 0f, Life, Swirl, Void, k => new Transform3D(face * Basis.FromScale(Vector3.One * wide(k) * 0.9f), gate + Vector3.Back * 0.02f), k => Fade(k, 0.03f, 0.05f), spin: 7f, gain: 1.5f);
        Disc(c, 0f, Life, Glow, Spirit, k => new Transform3D(face * Basis.FromScale(Vector3.One * wide(k) * 0.7f), gate + Vector3.Back * 0.04f), k => Fade(k, 0.03f, 0.05f) * 0.6f);
        Disc(c, 0f, Life, Glow, Amethyst, new Vector3(0f, 0f, 0.6f), _ => 1.8f, k => Fade(k, 0.1f, 0.1f) * 0.5f);
        for (var i = 0; i < 6; i++)
        {
            var start = Open + i * 0.22f;
            var foe = new Vector3((i % 3 - 1) * r * 0.5f + Rand(-0.3f, 0.3f), 0.9f, r * Rand(1.2f, 1.6f));
            var bend = new Vector3((i % 2 == 0 ? 1f : -1f) * Rand(0.6f, 1.4f), Rand(0.5f, 1.2f), 0f);
            Func<float, Vector3> at = k => gate.Lerp(foe, k) + bend * Mathf.Sin(k * Mathf.Pi);
            if (i % 2 == 0)
                FlyingSkull(c, start, 0.6f, Spirit, at, 0.85f);
            else
            {
                Glyph(c, start, 0.6f, Wraith, Soul, at, k => 1.3f + 0.4f * k, k => Fade(k, 0.1f, 0.1f), k => Vector3.Up * 0.6f - Heading(at, k, 1f), gain: 1.7f, slim: 0.75f);
                Trail(c, start, 0.6f, Soul, at, 0.7f, 0.35f);
            }
            Flare(c, start, 0.2f, Star, Amethyst, gate, 1.6f);
            Hit(c, start + 0.6f, i % 2 == 0 ? Spirit : Soul, foe, 0.8f);
        }
        Twinkle(c, 20, 0f, Life - 0.4f, Amethyst, () => gate + new Vector3(Rand(-1.7f, 1.7f), Rand(-1.5f, 1.7f), Rand(0f, 0.4f)), 0.5f);
        Spray(c, Life - 0.05f, Amethyst, gate, 16, 4f, 3f);
        Shine(c, gate + Vector3.Back, r * 1.5f, Amethyst.Main, age => 1.2f * Fade(age / Life, 0.1f, 0.1f));
    }
}
