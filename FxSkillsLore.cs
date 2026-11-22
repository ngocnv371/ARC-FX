using Godot;

namespace Arc.Fx;

/// <summary>
/// The sixth set's elements: standing stones and gems, crowns and wheels of fire, frost in
/// windows and cages, gears and compasses of lightning, and water in whorls, scales and song.
/// </summary>
internal sealed partial class FxSkills
{
    private ShaderMaterial? _obsidianBlock, _rubyBlock;

    private ShaderMaterial ObsidianBlock => _obsidianBlock ??= FxKit.Crystal(new Color(0.1f, 0.08f, 0.16f), new Color(0.75f, 0.45f, 1f), rim: 0.9f, glow: 0.1f);
    private ShaderMaterial RubyBlock => _rubyBlock ??= FxKit.Crystal(new Color(0.85f, 0.08f, 0.15f), new Color(1f, 0.7f, 0.7f), rim: 0.8f, glow: 0.25f);

    private void Lore(FxSkill skill, Cast c, float r, float range)
    {
        switch (skill)
        {
            case FxSkill.StoneCircle: StoneCircle(c, r); break;
            case FxSkill.StoneMaze: StoneMaze(c, r); break;
            case FxSkill.SandClock: SandClock(c, r); break;
            case FxSkill.RubyRain: RubyRain(c, r); break;
            case FxSkill.EmeraldSpire: EmeraldSpire(c, r); break;
            case FxSkill.Quicksand: Quicksand(c, r); break;
            case FxSkill.ObsidianShards: ObsidianShards(c, r); break;
            case FxSkill.FlameCrown: FlameCrown(c, r); break;
            case FxSkill.EmberGarden: EmberGarden(c, r); break;
            case FxSkill.PhoenixFeathers: PhoenixFeathers(c, r); break;
            case FxSkill.SearingBrand: SearingBrand(c, r); break;
            case FxSkill.MoltenChains: MoltenChains(c, r); break;
            case FxSkill.PyreStar: PyreStar(c, r); break;
            case FxSkill.FlameMask: FlameMask(c, r); break;
            case FxSkill.SunWheel: SunWheel(c, r); break;
            case FxSkill.IceCrown: IceCrown(c, r); break;
            case FxSkill.FrozenRose: FrozenRose(c, r); break;
            case FxSkill.SnowOwlDive: SnowOwlDive(c, r); break;
            case FxSkill.FrostBars: FrostBars(c, r); break;
            case FxSkill.FrostCompass: FrostCompass(c, r); break;
            case FxSkill.CrystalRain: CrystalRain(c, r); break;
            case FxSkill.Avalanche: Avalanche(c, r); break;
            case FxSkill.FrostWindow: FrostWindow(c, r); break;
            case FxSkill.GearStorm: GearStorm(c, r); break;
            case FxSkill.StormCompass: StormCompass(c, r); break;
            case FxSkill.VoltChains: VoltChains(c, r); break;
            case FxSkill.SkyArrows: SkyArrows(c, r); break;
            case FxSkill.TeslaGate: TeslaGate(c, r); break;
            case FxSkill.CycloneWhorl: CycloneWhorl(c, r); break;
            case FxSkill.ShockTiles: ShockTiles(c, r); break;
            case FxSkill.ScaleShield: ScaleShield(c, r); break;
            case FxSkill.TidalWhorl: TidalWhorl(c, r); break;
            case FxSkill.SilverShoal: SilverShoal(c, r); break;
            case FxSkill.RainbowMist: RainbowMist(c, r); break;
            case FxSkill.SongOfTheDeep: SongOfTheDeep(c, r); break;
            case FxSkill.TearDrop: TearDrop(c, r); break;
            case FxSkill.WaterMirror: WaterMirror(c, r); break;
            case FxSkill.Riptide: Riptide(c, r); break;
            default: Fable(skill, c, r, range); break;
        }
    }

    private ArrayMesh[]? _menhirs, _blocks;

    private ArrayMesh[] Menhirs => _menhirs ??= [.. Enumerable.Range(0, 4).Select(FxProps.Menhir)];
    private ArrayMesh[] Blocks => _blocks ??= [.. Enumerable.Range(0, 4).Select(FxProps.Block)];

    /// <summary>
    /// A stone (a unit tall, its foot at 0) thrust up out of the ground at <paramref name="foot"/>
    /// and sinking back at the end; <paramref name="shape"/> turns and sizes it.
    /// </summary>
    private void Raise(Cast c, float start, float life, Mesh mesh, Vector3 foot, Basis shape)
    {
        var tall = shape.Y.Length();
        Prop(c, start, life, mesh, _stone, k =>
        {
            var t = k * life;
            var up = Out(Mathf.Min(t / 0.14f, 1f)) - Mathf.Clamp((t - (life - 0.4f)) / 0.4f, 0f, 1f);
            return new Transform3D(shape, foot + Vector3.Up * tall * 1.15f * (up - 1f));
        });
    }

    // ---- Earth.

    private void StoneCircle(Cast c, float r)
    {
        const int Stones = 8;
        const float Rise = 0.3f, Stand1 = 2.4f;
        Mark(c, 0f, Rise + Stand1, Compass, Dust, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.04f, 0.2f), spin: 1f, gain: 1.3f);
        Disc(c, 0f, Rise + Stand1, Glow, Gold, Vector3.Zero, _ => r * 0.5f, k => Fade(k, 0.2f, 0.2f) * 0.5f);
        for (var i = 0; i < Stones; i++)
        {
            var angle = i * Mathf.Tau / Stones;
            var foot = Around(angle) * r * 0.8f;
            var start = Rise + i * 0.06f;
            var tall = i % 2 == 0 ? 2.7f : 2.1f;
            // A slab about a metre broad, its broad face turned to the middle of the ring.
            var shape = new Basis(Vector3.Up, angle + Rand(-0.2f, 0.2f)) * new Basis(Vector3.Right, Rand(-0.06f, 0.06f)) * Basis.FromScale(new Vector3(1.7f, tall, 1.5f));
            Raise(c, start, Stand1 - i * 0.06f, Menhirs[i % Menhirs.Length], foot, shape);
            Disc(c, start, 0.4f, Shock, Dust, foot, k => 1.2f * Out(k), k => 1f - k, band: 0.1f, gain: 1.2f);
            Disc(c, start, 1.2f, Cracks, Rubble, foot, _ => 0.8f, k => Fade(k, 0.02f, 0.5f), k => k);
            Spray(c, start, Rubble, foot + Vector3.Up * 0.2f, 6, 3f, 6f, 0.5f, 0.1f);
            // A rune lights on each stone's face.
            Glyph(c, start + 0.3f, Stand1 - 1f, Glint, Gold, _ => foot * 0.88f + Vector3.Up * tall * 0.6f, k => 0.8f + 0.2f * Mathf.Sin(k * 20f + i), k => Fade(k, 0.1f, 0.2f), gain: 1.7f);
        }
        Column(c, Rise + 0.6f, Stand1 - 1.2f, Light, Gold, Vector3.Zero, _ => new Vector2(r * 0.25f, 7f), k => Fade(k, 0.15f, 0.3f) * 0.6f, gain: 1.3f, speed: -1.5f);
        Drift(c, 16, Rise, Rise + 0.5f, 1f, Puff, [Dust], () => Around(Rand(0f, Mathf.Tau)) * r * 0.8f + Vector3.Up * 0.3f, () => Vector3.Up * 0.5f, 1.2f, tumble: false, gain: 0.55f);
        Shine(c, Vector3.Up, r * 1.5f, Gold.Main, age => 0.9f * Fade(age / (Rise + Stand1), 0.1f, 0.2f));
    }

    private void StoneMaze(Cast c, float r)
    {
        const float Rise = 0.4f, Stand1 = 2.4f;
        Mark(c, 0f, Rise + Stand1, Maze, Dust, Vector3.Zero, _ => r, k => Fade(k, 0.04f, 0.2f), k => Mathf.Min(k * (Rise + Stand1) / 0.6f, 1f), spin: 0f, gain: 1.3f);
        Disc(c, Rise, Stand1, Cracks, Rubble, Vector3.Zero, _ => r, k => Fade(k, 0.03f, 0.3f) * 0.7f, k => k * 0.6f);
        // Dry-stone walls thrown up along the rings, block by block, leaving gaps to walk through.
        for (var ring = 0; ring < 3; ring++)
        {
            var far = r * (0.38f + 0.27f * ring);
            var count = 5 + ring * 3;
            var skip = (int)(GD.Randi() % count);
            for (var i = 0; i < count; i++)
            {
                if (i == skip || i == (skip + count / 2) % count)
                    continue;
                var a0 = i * Mathf.Tau / count;
                var a1 = (i + 0.94f) * Mathf.Tau / count;
                var tall = 1.05f + 0.2f * ring;
                const int Parts = 2;
                for (var part = 0; part < Parts; part++)
                {
                    var p0 = Around(Mathf.Lerp(a0, a1, part / (float)Parts)) * far;
                    var p1 = Around(Mathf.Lerp(a0, a1, (part + 1f) / Parts)) * far;
                    var along = p1 - p0;
                    var x = along.Normalized();
                    var start = Rise + ring * 0.12f + i * 0.02f + part * 0.03f;
                    // A block is 0.7 long and 0.32 through.
                    var shape = new Basis(x * along.Length() / 0.7f * 1.06f, Vector3.Up * tall * Rand(0.8f, 1.15f), x.Cross(Vector3.Up) * Rand(1.1f, 1.4f));
                    Raise(c, start, Rise + Stand1 - start, Blocks[(i + part + ring) % Blocks.Length], (p0 + p1) * 0.5f, shape);
                    Spray(c, start, Rubble, (p0 + p1) * 0.5f + Vector3.Up * 0.1f, 3, 2f, 4f, 0.4f, 0.09f);
                }
                var mid = Around((a0 + a1) * 0.5f) * far;
                Glyph(c, Rise + ring * 0.12f + i * 0.02f, 0.9f, Puff, i % 2 == 0 ? Dust : Smoke, k => mid + Vector3.Up * (0.3f + 0.5f * k), k => 1.1f + 0.6f * k, k => Fade(k, 0.15f, 0.5f) * 0.6f, gain: 0.7f);
            }
            Disc(c, Rise + ring * 0.12f, 0.4f, Shock, Dust, Vector3.Zero, k => far * 1.2f * Out(k), k => (1f - k) * 0.7f, band: 0.08f);
        }
        // A light wanders the paths.
        Func<float, Vector3> light = k => Around(k * 9f) * r * (0.2f + 0.6f * (0.5f + 0.5f * Mathf.Sin(k * 5f))) + Vector3.Up * 0.5f;
        Flare(c, Rise, Stand1, Star, Gold, light, _ => 0.9f, k => Fade(k, 0.05f, 0.15f));
        Trail(c, Rise, Stand1, Gold, light, 0.2f, 0.1f, gain: 1.4f);
        Shine(c, Vector3.Up, r * 1.4f, Dust.Main, age => 0.8f * Fade(age / (Rise + Stand1), 0.1f, 0.2f));
    }

    private void SandClock(Cast c, float r)
    {
        const float Run = 2f, Turn = 0.4f;
        var glass = new Vector3(0f, 2.4f, r * 0.1f);
        var ground = FxKit.Flat(glass);
        Mark(c, 0f, Run + Turn + 0.6f, Whorl, Dust, ground, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), count: 6f, spin: 4f, gain: 1.3f);
        Mark(c, 0f, Run + Turn + 0.6f, Dial, Gold, ground, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f) * 0.7f, spin: 3f);
        Glyph(c, 0f, Run, Sandglass, Gold, k => glass + Vector3.Up * 0.08f * Mathf.Sin(k * 9f), k => 3.2f * Overshoot(Mathf.Min(k * Run / 0.3f, 1f)), k => Fade(k, 0.03f, 0.02f), gain: 2f);
        // Sand runs out of it and heaps on the ground.
        Line(c, 0.3f, Run - 0.3f, Dust, _ => (glass + Vector3.Down * 1.5f + ground) * 0.5f, _ => Vector3.Down * (glass.Y - 1.5f), 0.07f, k => Fade(k, 0.05f, 0.05f) * 0.9f);
        Disc(c, 0.4f, Run - 0.4f, Glow, Dust, ground, k => 0.3f + r * 0.5f * k, k => 0.5f + 0.4f * k, gain: 1.3f);
        for (var i = 0; i < 40; i++)
            Spark(c, Rand(0.4f, Run), Rand(0.3f, 0.5f), i % 2 == 0 ? Dust : Gold, ground + Vector3.Up * 0.1f, Around(Rand(0f, Mathf.Tau)) * Rand(0.5f, 2f) + Vector3.Up * Rand(1f, 2.5f), Gravity, 0.05f, 0.03f);
        // The last grain falls and it turns over.
        Glyph(c, Run, Turn, Sandglass, Gold, _ => glass, _ => 3.2f, k => 1f - k * 0.5f, k => new Vector3(Mathf.Sin(k * Mathf.Pi), Mathf.Cos(k * Mathf.Pi), 0f), gain: 2f);
        Hit(c, Run + Turn, Gold, ground + Vector3.Up * 0.3f, r * 0.9f);
        Mark(c, Run + Turn, 0.6f, Dial, Gold, ground, k => r * 0.9f * (1f + 0.3f * Out(k)), k => 1f - k, _ => 1f, spin: 3f, gain: 1.6f);
        Disc(c, Run + Turn, 0.6f, Swirl, Dust, ground, k => r * 1.2f, k => (1f - k) * 0.8f, spin: 12f);
        // Dust hangs over the heap as it grows, and is thrown wide when the glass turns.
        Drift(c, 44, 0.4f, Run - 0.5f, 1f, Puff, [Dust, Smoke, Dust], () => Within(ground, r * 0.55f, 0.2f, 1f), () => Around(Rand(0f, Mathf.Tau)) * 0.8f + Vector3.Up * 0.6f, 1.8f, tumble: false, gain: 1f);
        Drift(c, 20, Run + Turn, Run + Turn + 0.08f, 0.9f, Puff, [Dust, Smoke], () => Within(ground, r * 0.5f, 0.2f, 1f), () => Around(Rand(0f, Mathf.Tau)) * 2f + Vector3.Up * 0.5f, 1.6f, tumble: false, gain: 0.7f);
        for (var i = 0; i < 30; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Run + Turn, Rand(0.3f, 0.5f), i % 2 == 0 ? Dust : Rubble, ground + Vector3.Up * 0.2f, way * Rand(3f, 6f) + Vector3.Up * Rand(1f, 3f), Gravity * 0.8f, 0.07f, 0.04f);
        }
        Twinkle(c, 20, 0.2f, Run, Gold, () => glass + new Vector3(Rand(-0.9f, 0.9f), Rand(-1.5f, 1.5f), 0.2f), 0.5f);
        Shine(c, glass, r * 1.5f, Gold.Main, age => age < Run + Turn ? 0.8f : 2.2f * Mathf.Max(0f, 1f - (age - Run - Turn) / 0.5f));
    }

    private void RubyRain(Cast c, float r)
    {
        const int Rubies = 10;
        const float Fall = 0.3f, Every = 0.1f;
        var centre = new Vector3(0f, 0f, r * 0.15f);
        Mark(c, 0f, Rubies * Every + Fall + 0.7f, Triad, Crimson, centre, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), spin: 1.5f, gain: 1.3f);
        var spots = Spread(Rubies, r * 0.8f);
        for (var i = 0; i < Rubies; i++)
        {
            var spot = centre + spots[i];
            var sky = spot + new Vector3(-1.5f, 9f, -2f);
            var start = i * Every;
            var way = (spot - sky).Normalized();
            Prop(c, start, Fall + 0.9f, ShardMesh, RubyBlock, k =>
            {
                var t = k * (Fall + 0.9f);
                var fall = Mathf.Min(t / Fall, 1f);
                var sink = Mathf.Clamp((t - Fall - 0.6f) / 0.3f, 0f, 1f);
                return new Transform3D(Along(-way, 1f, 1f) * Basis.FromScale(new Vector3(1.6f, 2.6f, 1.6f) * (1f - sink)), sky.Lerp(spot + Vector3.Up * 0.35f, fall * fall));
            });
            Line(c, start, Fall, Crimson, k => sky.Lerp(spot, k * k) - way * 1.2f, _ => way * 2.2f, 0.1f, _ => 0.8f);
            Hit(c, start + Fall, Crimson, spot + Vector3.Up * 0.2f, 0.7f);
            Glyph(c, start + Fall, 0.4f, Gem, Rose, spot + Vector3.Up * 0.9f, 1.3f, 1.8f);
            Disc(c, start + Fall, 0.9f, Cracks, Crimson, spot, _ => 0.6f, k => Fade(k, 0.02f, 0.5f), k => k);
        }
        Twinkle(c, 30, 0.2f, Rubies * Every + Fall + 0.4f, Rose, () => Within(centre, r * 0.8f, 0.2f, 1.6f), 0.55f);
        Shine(c, centre + Vector3.Up, r * 1.5f, Crimson.Main, age => 1.2f * Fade(age / (Rubies * Every + Fall + 0.6f), 0.1f, 0.3f));
    }

    private void EmeraldSpire(Cast c, float r)
    {
        const float Rise = 0.35f, Stand1 = 2.2f;
        var spot = new Vector3(0f, 0f, r * 0.1f);
        Mark(c, 0f, Rise + Stand1, Compass, Jade, spot, _ => r * 0.85f, k => Fade(k, 0.04f, 0.2f), spin: 1.5f, gain: 1.3f);
        Disc(c, 0f, Rise, Telegraph, Jade, spot, _ => r * 0.4f, _ => 0.7f, k => k);
        Crystal(c, Rise, Stand1, JadeBlock, spot, Vector3.Up, 4.6f, 1f);
        for (var i = 0; i < 6; i++)
        {
            var way = Around(i * Mathf.Tau / 6f + 0.3f);
            Crystal(c, Rise + 0.05f + i * 0.03f, Stand1 - 0.1f - i * 0.03f, JadeBlock, spot + way * 0.7f, (Vector3.Up + way * 0.8f).Normalized(), Rand(1.2f, 1.9f), 0.4f);
        }
        Disc(c, Rise, 0.5f, Shock, Jade, spot, k => r * 1.3f * Out(k), k => 1f - k, band: 0.14f, gain: 1.4f);
        Disc(c, Rise, Stand1, Cracks, Jade, spot, _ => r * 0.8f, k => Fade(k, 0.02f, 0.3f), k => k * 0.7f);
        Flare(c, Rise + 0.1f, Stand1 - 0.4f, Star, Jade, _ => spot + Vector3.Up * 4.3f, k => 1.6f + 0.4f * Mathf.Sin(k * 30f), k => Fade(k, 0.1f, 0.2f));
        Column(c, Rise + 0.15f, Stand1 - 0.5f, Beam, Jade, spot + Vector3.Up * 4.2f, _ => new Vector2(0.08f, 7f), k => Fade(k, 0.1f, 0.3f), speed: 4f, gain: 1.5f);
        Spray(c, Rise, Jade, spot + Vector3.Up * 0.3f, 30, 5f, 9f, 0.7f, 0.08f);
        for (var t = Rise + 0.5f; t < Rise + Stand1 - 0.5f; t += 0.5f)
            Disc(c, t, 0.6f, Shock, Jade, spot, k => r * Out(k), k => (1f - k) * 0.7f, band: 0.08f);
        Twinkle(c, 40, Rise + 0.2f, Rise + Stand1 - 0.4f, Pearl, () => spot + new Vector3(Rand(-0.9f, 0.9f), Rand(0.3f, 4.4f), Rand(0f, 0.6f)), 0.6f);
        Shine(c, spot + Vector3.Up * 2f, r * 1.6f, Jade.Main, age => 1.2f * Fade(age / (Rise + Stand1), 0.12f, 0.2f));
    }

    private void Quicksand(Cast c, float r)
    {
        const float Life = 3f;
        Mark(c, 0f, Life, Whorl, Dust, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.15f), count: 9f, spin: -9f, gain: 1.5f);
        Disc(c, 0f, Life, Swirl, Rubble, Vector3.Zero, _ => r, k => Fade(k, 0.08f, 0.15f) * 0.8f, spin: 4f, gain: 1.2f);
        Disc(c, 0f, Life, Glow, Dust, Vector3.Zero, _ => r * 0.35f, k => Fade(k, 0.08f, 0.15f) * 0.6f);
        for (var i = 0; i < 6; i++)
            Disc(c, i * 0.45f, 0.9f, Shock, Dust, Vector3.Zero, k => r * (1f - 0.85f * Out(k)), k => Fade(k, 0.2f, 0.3f) * 0.7f, band: 0.08f);
        for (var i = 0; i < 26; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var size = Rand(0.9f, 1.5f);
            Glyph(c, Rand(0f, Life - 1.2f), 1.2f, Puff, i % 3 == 0 ? Rubble : Dust, k => Around(phase + k * 3.5f) * r * (0.95f - 0.85f * k) + Vector3.Up * (0.3f - 0.2f * k), k => size * (1f - 0.6f * k), k => Fade(k, 0.2f, 0.3f) * 0.7f, gain: 0.8f);
        }
        // Things going under: stones dragged round and down.
        for (var i = 0; i < 8; i++)
        {
            var phase = i * Mathf.Tau / 8f;
            var start = Rand(0f, Life - 1.6f);
            var size = Rand(0.14f, 0.26f);
            Prop(c, start, 1.6f, Boulders[i % Boulders.Length], _stone, k => new Transform3D(Basis.FromScale(Vector3.One * size * (1f - 0.5f * k)), Around(phase + k * 5f) * r * (0.85f - 0.8f * k) + Vector3.Up * (0.15f - 0.3f * k)));
        }
        Shine(c, Vector3.Up, r * 1.2f, Dust.Main, age => 0.5f * Fade(age / Life, 0.08f, 0.15f));
    }

    private void ObsidianShards(Cast c, float r)
    {
        const float Stand1 = 1.4f;
        var reach = r * 1.7f;
        for (var row = 0; row < 5; row++)
        {
            var far = 0.9f + (reach - 0.9f) * row / 4f;
            var count = 1 + row;
            for (var i = 0; i < count; i++)
            {
                var angle = count == 1 ? 0f : (i / (count - 1f) - 0.5f) * 0.8f;
                var way = Around(angle);
                var foot = way * far;
                var start = row * 0.07f + Rand(0f, 0.03f);
                var tall = Rand(1.3f, 2.1f) + row * 0.25f;
                Crystal(c, start, Stand1 - start, ObsidianBlock, foot, (way * 0.9f + Vector3.Up).Normalized(), tall, tall * 0.2f);
                Disc(c, start, 0.3f, Shock, Amethyst, foot, k => 0.9f * Out(k), k => 1f - k, band: 0.1f);
                Glyph(c, start + 0.12f, 0.35f, Glint, Amethyst, foot + (way * 0.9f + Vector3.Up).Normalized() * tall * 0.9f, 1.2f, 1.7f);
                Disc(c, start, Stand1 - start, Cracks, Amethyst, foot, _ => 0.6f, k => Fade(k, 0.02f, 0.4f), k => k);
                for (var j = 0; j < 3; j++)
                    Spark(c, start, 0.4f, Amethyst, foot + Vector3.Up * 0.2f, FxKit.Jitter(2f) + Vector3.Up * 4f + way * 2f, Gravity, 0.06f, 0.03f);
            }
        }
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Amethyst.Main, age => 1.2f * Fade(age / Stand1, 0.05f, 0.4f));
    }

    // ---- Fire.

    private void FlameCrown(Cast c, float r)
    {
        const float Life = 3f;
        const int Tongues = 8;
        var head = Vector3.Up * 2.35f;
        Mark(c, 0f, Life, Sunburst, Flame, Vector3.Zero, _ => r * 0.55f, k => Fade(k, 0.05f, 0.15f), count: 12f, spin: 3f, gain: 1.3f);
        Crown(c, 0f, Life, Flame, k => head + Vector3.Down * 0.3f + Vector3.Up * 0.05f * Mathf.Sin(k * Life * 5f), gain: 2f);
        for (var i = 0; i < Tongues; i++)
        {
            var phase = i * Mathf.Tau / Tongues;
            Func<float, Vector3> at = k => head + Around(phase + k * Life * 1.5f) * 0.52f + Vector3.Up * 0.1f;
            Glyph(c, i * 0.04f, Life - i * 0.04f, Wisp, i % 2 == 0 ? Flame : Gold, at, k => 0.75f + 0.15f * Mathf.Sin(k * 40f + phase), k => Fade(k, 0.04f, 0.12f), gain: 1.8f);
        }
        Column(c, 0f, Life, Fire, Flame, Vector3.Zero, _ => new Vector2(0.6f, 1.6f), k => Fade(k, 0.08f, 0.15f) * 0.5f, gain: 1.3f, speed: 2f);
        for (var i = 0; i < 40; i++)
            Spark(c, Rand(0f, Life - 0.5f), Rand(0.4f, 0.7f), i % 2 == 0 ? Flame : Gold, head + Around(Rand(0f, Mathf.Tau)) * 0.5f, FxKit.Jitter(0.8f) + Vector3.Up * Rand(1.5f, 3f), -2f, 0.05f, 0.04f);
        for (var t = 0f; t < Life - 0.5f; t += 0.75f)
            Disc(c, t, 0.6f, Shock, Flame, Vector3.Zero, k => r * 0.9f * Out(k), k => (1f - k) * 0.8f, band: 0.1f, gain: 1.2f);
        Shine(c, Vector3.Up * 1.5f, r * 1.2f, Flame.Main, age => (1.2f + 0.2f * Mathf.Sin(age * 15f)) * Fade(age / Life, 0.05f, 0.12f));
    }

    private void EmberGarden(Cast c, float r)
    {
        const float Grow = 0.5f, Bloom = 1.4f, After = 0.8f;
        const int Flowers = 7;
        Mark(c, 0f, Bloom + After, Lotus, Ember, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), count: 7f, spin: 1f, gain: 1.2f);
        var spots = Spread(Flowers - 1, r * 0.75f).Append(Vector3.Zero).ToArray();
        Tone[] petals = [Flame, Gold, Ember, Sun];
        for (var i = 0; i < Flowers; i++)
        {
            var foot = spots[i];
            var start = i * 0.09f;
            var tall = i == Flowers - 1 ? 1.9f : Rand(1f, 1.5f);
            var head = foot + Vector3.Up * tall;
            var tone = petals[i % petals.Length];
            var life = Bloom - start;
            var wide = i == Flowers - 1 ? 1.6f : 1.1f;
            // A stem of fire climbs, and a flower of flame opens at its head.
            Tube(c, start, life, Fire, Ember, k => (foot, foot + Vector3.Up * tall * Out(Mathf.Min(k * life / Grow, 1f))), _ => 0.07f, k => Fade(k, 0f, 0.1f), speed: 2f, gain: 1.5f);
            Glyph(c, start + Grow * 0.7f, life - Grow * 0.7f, Daisy, tone, _ => head, k => wide * Overshoot(Mathf.Min(k * 4f, 1f)), k => Fade(k, 0.03f, 0.05f), gain: 1.9f);
            Disc(c, start, life, Glow, Ember, foot, _ => 0.5f, k => Fade(k, 0.1f, 0.1f) * 0.7f);
            Glyph(c, start + Grow * 0.7f, 0.3f, Glint, Gold, head, 1.3f, 1.7f);
            // Each seeds a burst of flame.
            Column(c, Bloom + i * 0.05f, After, Fire, tone, foot, k => new Vector2(0.4f, (tall + 1.2f) * Out(Mathf.Min(k * 5f, 1f))), k => 1f - k * k, gain: 1.6f, speed: 2.4f);
            Disc(c, Bloom + i * 0.05f, 0.35f, Shock, Flame, foot, k => 1.2f * Out(k), k => 1f - k, band: 0.1f, gain: 1.2f);
            Drift(c, 5, Bloom, Bloom + 0.2f, 0.8f, Leaflet, [tone, Gold], () => head, () => FxKit.Jitter(1.5f) + Vector3.Up * 2f, 0.3f, gravity: 3f, slim: 0.5f);
        }
        Cue(c, Bloom, () => Stage.Patch(At(c, Vector3.Zero), r * 0.8f, 2f, FireColor, FxGround.Fire));
        Shine(c, Vector3.Up, r * 1.5f, Flame.Main, age => age < Bloom ? 0.9f * Mathf.Min(age / Grow, 1f) : 2.2f * Mathf.Max(0f, 1f - (age - Bloom) / After));
    }

    private void PhoenixFeathers(Cast c, float r)
    {
        const int Feathers = 9;
        const float Flight = 0.42f;
        var reach = r * 1.6f;
        var hand = new Vector3(0f, 1.3f, 0.4f);
        Flare(c, 0f, 0.3f, Star, Flame, hand, 2.6f);
        Mark(c, 0f, 0.6f, Wings, Flame, k => new Transform3D(Stand(Vector3.Back) * Basis.FromScale(Vector3.One * 2.2f), hand + new Vector3(0f, 0.6f, -0.3f)), k => 1f - k, k => Out(Mathf.Min(k * 3f, 1f)), count: 8f, gain: 1.6f);
        for (var i = 0; i < Feathers; i++)
        {
            var angle = (i - (Feathers - 1) * 0.5f) * 0.15f;
            var to = Around(angle) * reach + Vector3.Up * 0.8f;
            var start = Mathf.Abs(i - (Feathers - 1) * 0.5f) * 0.02f;
            var way = (to - hand).Normalized();
            Func<float, Vector3> at = k => hand.Lerp(to, k) + Vector3.Up * 0.5f * Mathf.Sin(k * Mathf.Pi);
            Glyph(c, start, Flight, Plume, i % 2 == 0 ? Flame : Gold, at, _ => 1.6f, k => Fade(k, 0.05f, 0.05f), k => Heading(at, k, 1f), gain: 1.9f, slim: 0.38f);
            Trail(c, start, Flight, Flame, at, 0.4f, 0.4f, style: 1, gain: 1.5f);
            var ground = FxKit.Flat(to);
            Column(c, start + Flight, 0.7f, Fire, Flame, ground, k => new Vector2(0.3f, 2f * Out(Mathf.Min(k * 5f, 1f))), k => 1f - k * k, gain: 1.5f, speed: 2.4f);
            Glyph(c, start + Flight, 0.3f, Glint, Gold, to, 1.4f, 1.7f);
            Disc(c, start + Flight, 0.3f, Shock, Flame, ground, k => 0.9f * Out(k), k => 1f - k, band: 0.1f);
            Cue(c, start + Flight, () => Stage.Patch(At(c, ground), 0.7f, 1.4f, FireColor, FxGround.Fire));
        }
        Drift(c, 16, 0.1f, Flight, 1f, Plume, [Flame, Gold, Ember], () => Around(Rand(-0.6f, 0.6f)) * reach * Rand(0.3f, 0.9f) + Vector3.Up * Rand(0.8f, 1.8f), () => new Vector3(0f, -0.9f, 0f), 0.45f, slim: 0.42f, gain: 1.6f);
        Shine(c, new Vector3(0f, 1f, reach * 0.6f), reach, Flame.Main, age => 1.6f * Fade(age / (Flight + 0.7f), 0.05f, 0.4f));
    }

    private void SearingBrand(Cast c, float r)
    {
        const float Draw = 0.9f, After = 1f;
        var spot = new Vector3(0f, 0f, r * 0.15f);
        var corners = Enumerable.Range(0, 3).Select(i => spot + Around(i * Mathf.Tau / 3f) * r * 0.78f + Vector3.Up * 0.08f).ToArray();
        Mark(c, 0f, Draw + After, Triad, Ember, spot, _ => r * 0.9f, k => Fade(k, 0.03f, 0.3f), k => Mathf.Min(k * (Draw + After) / Draw, 1f), spin: 0f, gain: 1.6f);
        Disc(c, 0f, Draw, Glow, Ember, spot, _ => r * 0.8f, k => 0.2f + 0.5f * k);
        // A point of white heat traces the triangle, leaving it burning.
        for (var i = 0; i < 3; i++)
        {
            var (a, b) = (corners[i], corners[(i + 1) % 3]);
            var start = i * Draw / 3f;
            var life = Draw + After - start;
            Tube(c, start, life, Fire, Flame, k => (a, a.Lerp(b, Mathf.Min(k * life / (Draw / 3f), 1f))), _ => 0.14f, k => Fade(k, 0f, 0.25f), speed: 2f, gain: 1.7f);
            Flare(c, start, Draw / 3f, Star, Gold, k => a.Lerp(b, k), _ => 1.3f, _ => 1f);
            for (var j = 0; j < 8; j++)
            {
                var s = Rand(0f, 1f);
                Spark(c, start + s * Draw / 3f, Rand(0.3f, 0.5f), Flame, a.Lerp(b, s), FxKit.Jitter(1.5f) + Vector3.Up * 3f, Gravity, 0.05f, 0.03f);
            }
            Column(c, Draw + i * 0.05f, After, Fire, Flame, FxKit.Flat(a), k => new Vector2(0.45f, 3.6f * Out(Mathf.Min(k * 5f, 1f))), k => 1f - k * k, gain: 1.7f, speed: 2.4f);
        }
        Cue(c, Draw, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.4f), r * 0.6f, FireColor, FxBurst.Fire));
        Cue(c, Draw, () => Stage.Patch(At(c, spot), r * 0.7f, 2.4f, FireColor, FxGround.Inferno));
        Hit(c, Draw, Flame, spot + Vector3.Up * 0.3f, r * 0.9f);
        Shine(c, spot + Vector3.Up, r * 1.6f, Flame.Main, age => age < Draw ? 0.4f + 0.8f * age / Draw : 2.6f * Mathf.Max(0f, 1f - (age - Draw) / After));
    }

    private void MoltenChains(Cast c, float r)
    {
        const float Lash = 0.35f, Hold = 1.8f, Life = Lash + Hold;
        var heart = new Vector3(0f, 1.3f, r * 0.15f);
        var ground = FxKit.Flat(heart);
        Mark(c, 0f, Life + 0.4f, Links, Magma, ground, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), count: 8f, spin: 2f, gain: 1.5f);
        Disc(c, 0f, Life, Cracks, Magma, ground, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), k => k * 0.6f);
        for (var i = 0; i < 4; i++)
        {
            var anchor = ground + Around(i * Mathf.Tau / 4f + 0.785f) * r * 0.8f;
            var start = i * 0.05f;
            var life = Life - start;
            Func<float, float> taut = k => Out(Mathf.Min(k * life / Lash, 1f));
            Tube(c, start, life, Spiral, Magma, k => (anchor, anchor.Lerp(heart, taut(k))), k => 0.1f + 0.015f * Mathf.Sin(k * 50f), k => Fade(k, 0f, 0.1f), twist: 9f, speed: 0.6f, gain: 1.8f);
            Tube(c, start, life, Beam, Flame, k => (anchor, anchor.Lerp(heart, taut(k))), _ => 0.035f, k => Fade(k, 0f, 0.1f), speed: 3f, gain: 1.5f);
            Column(c, start, 0.6f, Fire, Magma, anchor, k => new Vector2(0.35f, 2f * Out(Mathf.Min(k * 5f, 1f))), k => 1f - k, gain: 1.5f, speed: 2.4f);
            Disc(c, start, life, Glow, Magma, anchor, _ => 0.7f, k => Fade(k, 0.05f, 0.15f) * 0.8f);
            for (var j = 0; j < 10; j++)
            {
                var s = Rand(0.1f, 0.9f);
                Spark(c, start + Rand(Lash, life), 0.5f, Flame, anchor.Lerp(heart, s), new Vector3(Rand(-0.5f, 0.5f), 0f, Rand(-0.5f, 0.5f)), Gravity, 0.05f, 0.03f);
            }
        }
        Globe(c, "energy", Shell, Lash, Hold, Magma, _ => heart, k => 0.9f + 0.08f * Mathf.Sin(k * Hold * 14f), k => Fade(k, 0.05f, 0.05f) * 0.8f, gain: 1.4f);
        Flare(c, Lash, 0.3f, Star, Flame, heart, 3f);
        Disc(c, Lash, 0.4f, Shock, Flame, ground, k => r * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Hit(c, Life, Flame, heart, r * 0.7f);
        Cue(c, Life, () => Stage.Explosion(At(c, heart), r * 0.5f, FireColor, FxBurst.Fire));
        Shine(c, heart, r * 1.4f, Flame.Main, age => age < Life ? 1f : 2.2f * Mathf.Max(0f, 1f - (age - Life) / 0.5f));
    }

    private void PyreStar(Cast c, float r)
    {
        const float Each = 0.14f, Blow = 5 * Each + 0.3f, After = 0.9f;
        Mark(c, 0f, Blow + After, Pentacle, Flame, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.03f, 0.3f), k => Mathf.Min(k * (Blow + After) / (5 * Each), 1f), count: 5f, spin: 0f, gain: 1.6f);
        Disc(c, 0f, Blow, Glow, Ember, Vector3.Zero, _ => r * 0.8f, k => 0.2f + 0.5f * k);
        for (var i = 0; i < 5; i++)
        {
            // In the order the star is drawn: every second point.
            var point = Around(i * 2 * Mathf.Tau / 5f) * r * 0.86f;
            var when = i * Each;
            Column(c, when, Blow + After - when, Fire, Flame, point, k => new Vector2(0.35f, 2.4f * Out(Mathf.Min(k * 8f, 1f))), k => Fade(k, 0f, 0.3f), gain: 1.6f, speed: 2.2f);
            Glyph(c, when, 0.3f, Glint, Gold, point + Vector3.Up * 0.5f, 1.6f, 1.8f);
            Disc(c, when, 0.35f, Shock, Flame, point, k => 1.3f * Out(k), k => 1f - k, band: 0.1f, gain: 1.3f);
            Spray(c, when, Flame, point + Vector3.Up * 0.2f, 6, 2.5f, 5f);
            Func<float, Vector3> streak = k => (point + Vector3.Up * 1.4f).Lerp(Vector3.Up * 1.2f, k * k);
            Flare(c, Blow - 0.25f, 0.25f, Star, Flame, streak, _ => 1.2f, _ => 1f);
            Trail(c, Blow - 0.25f, 0.25f, Flame, streak, 0.4f, 0.6f, style: 1, gain: 1.5f);
        }
        Cue(c, Blow, () => Stage.Explosion(At(c, Vector3.Up * 0.6f), r * 0.9f, FireColor, FxBurst.Fire));
        Cue(c, Blow, () => Stage.Patch(At(c, Vector3.Zero), r * 0.85f, 2.4f, FireColor, FxGround.Inferno));
        Hit(c, Blow, Flame, Vector3.Up * 0.5f, r * 1.1f);
        Column(c, Blow, After, Fire, Flame, Vector3.Zero, k => new Vector2(r * 0.35f, 6f * Out(Mathf.Min(k * 4f, 1f))), k => 1f - k * k, gain: 1.8f, speed: 2.6f);
        Glyph(c, Blow, 0.6f, Starlet, Gold, Vector3.Up * 2.4f, 3.4f, 1.9f);
        Shine(c, Vector3.Up, r * 1.8f, Flame.Main, age => age < Blow ? 0.5f + 0.9f * age / Blow : 3f * Mathf.Max(0f, 1f - (age - Blow) / After));
    }

    private void FlameMask(Cast c, float r)
    {
        const float Loom = 0.35f, Breath = 1.1f;
        var reach = r * 1.25f;
        var face = new Vector3(0f, 1.9f, 0.5f);
        Mask3(c, 0f, Loom + Breath, Flame, k => face + Vector3.Up * 0.06f * Mathf.Sin(k * 30f), k => 3.2f * Overshoot(Mathf.Min(k * (Loom + Breath) / Loom, 1f)), k => Fade(k, 0.03f, 0.2f));
        var mouth = face + new Vector3(0f, -0.75f, 0.55f);
        for (var i = 0; i < 5; i++)
        {
            var angle = (i - 2) * 0.18f;
            var end = Around(angle) * reach + Vector3.Up * 0.5f;
            var phase = i * 1.3f;
            Func<float, Vector3> path = s => mouth.Lerp(end, s) + new Vector3(0.25f * Mathf.Sin(s * 12f + phase), 0.2f * Mathf.Sin(s * 9f + phase), 0f) * s;
            for (var t = 0f; t < Breath - 0.35f; t += 0.22f)
                Ribbon(c, Loom + t + i * 0.03f, 0.45f, 1, i % 2 == 0 ? Flame : Ember, path, k => (Mathf.Max(0f, k * 1.4f - 0.5f), Mathf.Min(k * 1.4f, 1f)), 1.1f, k => Fade(k, 0.05f, 0.3f), gain: 1.6f, head: 0.45f);
        }
        for (var i = 0; i < 30; i++)
        {
            var way = Around(Rand(-0.4f, 0.4f));
            Spark(c, Loom + Rand(0f, Breath - 0.3f), Rand(0.4f, 0.6f), Flame, mouth, way * Rand(6f, 12f) + Vector3.Up * Rand(-1f, 2f), Gravity * 0.3f, 0.06f, 0.05f);
        }
        Cue(c, Loom + 0.3f, () => Stage.Patch(At(c, new Vector3(0f, 0f, reach * 0.6f)), reach * 0.33f, 2f, FireColor, FxGround.Fire));
        Shine(c, new Vector3(0f, 1f, reach * 0.4f), reach, Flame.Main, age => age < Loom ? 0.6f : 1.8f * Fade((age - Loom) / Breath, 0f, 0.4f));
    }

    private void SunWheel(Cast c, float r)
    {
        const float Roll = 1.1f;
        const float Size = 1.3f;
        var reach = r * 1.8f;
        var side = Stand(Vector3.Right);
        Func<float, Vector3> hub = k => new Vector3(0f, Size, 0.6f + (reach - 0.6f) * k);
        Func<float, float> turned = k => (reach - 0.6f) * k / Size;
        Mark(c, 0f, Roll, Gear, Sun, k => new Transform3D(side * new Basis(Vector3.Up, -turned(k)) * Basis.FromScale(Vector3.One * Size), hub(k)), k => Fade(k, 0.06f, 0.03f), _ => 1f, count: 12f, spin: 0f, gain: 2f);
        Mark(c, 0f, Roll, Sunburst, Flame, k => new Transform3D(side * new Basis(Vector3.Up, -turned(k)) * Basis.FromScale(Vector3.One * Size * 0.75f), hub(k) + Vector3.Right * 0.03f), k => Fade(k, 0.06f, 0.03f), _ => 1f, count: 12f, spin: 0f, gain: 1.7f);
        Globe(c, "energy", Orb, 0f, Roll, Sun, hub, _ => 0.28f, k => Fade(k, 0.06f, 0.03f), gain: 1.7f);
        Flare(c, 0f, Roll, Mote, Flame, hub, _ => Size * 2.6f, k => Fade(k, 0.06f, 0.03f) * 0.45f, gain: 1f);
        // Fire thrown off the rim behind it, and a burning track.
        for (var i = 0; i < 44; i++)
        {
            var k0 = Rand(0.03f, 0.97f);
            var a0 = Rand(0f, Mathf.Tau);
            var rim = hub(k0) + new Vector3(0f, Mathf.Cos(a0), Mathf.Sin(a0)) * Size;
            Spark(c, k0 * Roll, Rand(0.3f, 0.5f), i % 2 == 0 ? Flame : Sun, rim, new Vector3(Rand(-1f, 1f), Mathf.Sin(a0) * 4f + 2f, -Mathf.Cos(a0) * 4f - 2f), Gravity * 0.7f, 0.06f, 0.04f);
        }
        Func<float, Vector3> track = s => new Vector3(0f, 0.08f, 0.6f + (reach - 0.6f) * s);
        Ribbon(c, 0f, Roll + 0.8f, 1, Flame, track, k => (Mathf.Max(0f, k * (Roll + 0.8f) / Roll - 0.6f), Mathf.Min(k * (Roll + 0.8f) / Roll, 1f)), 0.9f, k => Fade(k, 0f, 0.3f), gain: 1.5f);
        for (var i = 0; i < 6; i++)
        {
            var s = (i + 0.5f) / 6f;
            var spot = FxKit.Flat(track(s));
            Cue(c, s * Roll, () => Stage.Patch(At(c, spot), 0.7f, 1.5f, FireColor, FxGround.Fire));
        }
        var end = hub(1f);
        Cue(c, Roll, () => Stage.Explosion(At(c, end), r * 0.6f, Sun.Main, FxBurst.Fire));
        Hit(c, Roll, Sun, end, r * 0.7f);
        Shine(c, hub(0.5f), reach, Sun.Main, age => 1.3f + 1.4f * Bump(age, Roll, 0.5f));
    }

    // ---- Frost.

    private void IceCrown(Cast c, float r)
    {
        const float Life = 3f;
        var head = Vector3.Up * 2.35f;
        Mark(c, 0f, Life, Frostflake, Ice, Vector3.Zero, _ => r * 0.55f, k => Fade(k, 0.05f, 0.15f), spin: 3f, gain: 1.3f);
        Crown(c, 0f, Life, Ice, k => head + Vector3.Down * 0.3f + Vector3.Up * 0.05f * Mathf.Sin(k * Life * 4f), gain: 2f);
        for (var i = 0; i < 6; i++)
        {
            var phase = i * Mathf.Tau / 6f;
            Func<float, Vector3> at = k => head + Around(phase - k * Life * 1.2f) * 0.55f + Vector3.Up * (0.15f + 0.08f * Mathf.Sin(k * Life * 3f + phase));
            Glyph(c, i * 0.05f, Life - i * 0.05f, Gem, i % 2 == 0 ? Ice : Pearl, at, _ => 0.6f, k => Fade(k, 0.04f, 0.1f), gain: 1.8f);
        }
        Globe(c, "energy", Shell, 0f, Life, Ice, _ => Vector3.Up * 1f, _ => 1.15f, k => Fade(k, 0.08f, 0.15f) * 0.35f);
        Drift(c, 30, 0f, Life - 1.2f, 1.2f, Snow, [Ice, Pearl], () => head + new Vector3(Rand(-0.7f, 0.7f), Rand(-0.2f, 0.4f), Rand(-0.7f, 0.7f)), () => new Vector3(Rand(-0.3f, 0.3f), Rand(-1.6f, -0.9f), Rand(-0.3f, 0.3f)), 0.3f, tumble: false, gain: 1.6f);
        Cue(c, 0.2f, () => Stage.Patch(At(c, Vector3.Zero), r * 0.5f, Life - 0.2f, FrostColor, FxGround.Frost));
        Twinkle(c, 20, 0f, Life - 0.4f, Pearl, () => head + new Vector3(Rand(-0.8f, 0.8f), Rand(-0.4f, 0.7f), Rand(-0.2f, 0.2f)), 0.5f);
        Shine(c, Vector3.Up * 1.5f, r, Ice.Main, age => 0.9f * Fade(age / Life, 0.05f, 0.12f));
    }

    private void FrozenRose(Cast c, float r)
    {
        const float Open = 0.6f, Hold = 1.4f, After = 0.6f;
        const int Petals = 7;
        var heart = new Vector3(0f, 0.6f, r * 0.1f);
        var ground = FxKit.Flat(heart);
        var life = Open + Hold;
        Mark(c, 0f, life + After, Lotus, Ice, ground, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), count: 7f, spin: 0.8f, gain: 1.3f);
        Cue(c, 0.2f, () => Stage.Patch(At(c, ground), r * 0.8f, life + After, FrostColor, FxGround.Frost));
        // Three rings of petals, the inner ones standing more upright: a rose rather than a daisy.
        for (var layer = 0; layer < 3; layer++)
        {
            for (var i = 0; i < Petals; i++)
            {
                var way = Around((i + layer * 0.37f) * Mathf.Tau / Petals);
                var long1 = r * (0.7f - 0.2f * layer);
                var delay = layer * 0.14f + i * 0.015f;
                var mine = life - delay;
                var lift = layer * 0.45f;
                Func<float, float> open = k => Overshoot(Mathf.Min(k * mine / Open, 1f));
                Glyph(c, delay, mine, Leaflet, layer == 1 ? Pearl : Ice, k => heart + way * long1 * 0.5f * open(k) + Vector3.Up * lift, k => long1 * open(k), k => Fade(k, 0.03f, 0.02f), gain: 1.6f, slim: 0.62f, turn: _ => Face(way + Vector3.Up * (0.3f + 0.7f * layer), Vector3.Up));
            }
        }
        Crystal(c, Open * 0.5f, life - Open * 0.5f, IceBlock, ground, Vector3.Up, 1.7f, 0.5f);
        Flare(c, Open, Hold, Star, Pearl, _ => heart + Vector3.Up * 1.1f, k => 1.2f + 0.3f * Mathf.Sin(k * 20f), k => Fade(k, 0.1f, 0.1f));
        Twinkle(c, 30, Open, life, Pearl, () => Within(heart, r * 0.7f, -0.2f, 1.6f), 0.55f);
        // It shatters.
        Hit(c, life, Ice, heart, r * 0.8f);
        Cue(c, life, () => Stage.Explosion(At(c, heart), r * 0.5f, FrostColor, FxBurst.Frost));
        Drift(c, 30, life, life + 0.05f, After, Leaflet, [Ice, Pearl], () => Within(heart, r * 0.4f, 0f, 1f), () => Around(Rand(0f, Mathf.Tau)) * Rand(2f, 5f) + Vector3.Up * Rand(1f, 4f), 0.5f, gravity: 6f, slim: 0.6f);
        Shine(c, heart, r * 1.4f, Ice.Main, age => age < life ? 0.9f * Mathf.Min(age / Open, 1f) : 2.2f * Mathf.Max(0f, 1f - (age - life) / After));
    }

    private void SnowOwlDive(Cast c, float r)
    {
        const float Dive = 0.7f, Away1 = 0.5f, Life = Dive + Away1;
        var spot = new Vector3(0f, 0.7f, r * 0.2f);
        var from = spot + new Vector3(5f, 6f, -3f);
        var gone = spot + new Vector3(-5f, 4.5f, -1f);
        Func<float, Vector3> at = k =>
        {
            var t = k * Life;
            return t < Dive ? Curve(from, from + new Vector3(-2f, -1f, 1f), spot + new Vector3(2f, 0.4f, 0f), spot, t / Dive) : Curve(spot, spot + new Vector3(-2f, -0.2f, 0f), gone + new Vector3(2f, -1.5f, 0f), gone, (t - Dive) / Away1);
        };
        Mark(c, 0f, Life + 0.5f, Reticle, Ice, FxKit.Flat(spot), _ => r * 0.6f, k => Fade(k, 0.04f, 0.4f), count: 8f, spin: 3f, gain: 1.2f);
        Glyph(c, 0f, Life, Crow, Pearl, at, k => 2.8f, k => Fade(k, 0.1f, 0.1f), gain: 2f);
        Glyph(c, 0f, Life, Crow, Ice, k => at(Mathf.Max(k - 0.03f, 0f)), _ => 3.4f, k => Fade(k, 0.1f, 0.1f) * 0.5f, gain: 1.3f);
        Trail(c, 0f, Life, Ice, at, 0.8f, 0.22f, gain: 1.3f);
        // Talons rake where it passes.
        Glyph(c, Dive - 0.05f, 0.4f, Claw, Pearl, spot + Vector3.Up * 0.3f, 2.2f, 1.9f);
        Hit(c, Dive, Ice, spot, r * 0.6f);
        Cue(c, Dive, () => Stage.Patch(At(c, FxKit.Flat(spot)), r * 0.6f, 2f, FrostColor, FxGround.Frost));
        Cue(c, Dive, () => Stage.Explosion(At(c, spot), r * 0.4f, FrostColor, FxBurst.Frost));
        for (var i = 0; i < 18; i++)
        {
            var k0 = Rand(0.2f, 0.95f);
            var p0 = at(k0);
            Drift(c, 1, k0 * Life, k0 * Life, 1.1f, i % 3 == 0 ? Snow : Plume, [Pearl, Ice], () => p0, () => new Vector3(Rand(-0.4f, 0.4f), -1.2f, Rand(-0.4f, 0.4f)), 0.45f, sway: 0.4f, slim: i % 3 == 0 ? 1f : 0.42f, tumble: i % 3 != 0);
        }
        Shine(c, spot, r * 1.4f, Ice.Main, age => 0.5f + 1.8f * Bump(age, Dive, 0.5f));
    }

    private void FrostBars(Cast c, float r)
    {
        const int Bars = 10;
        const float Rise = 0.3f, Hold = 1.9f, Life = Rise + Hold;
        var spot = new Vector3(0f, 0f, r * 0.15f);
        var cage = r * 0.42f;
        const float Tall = 2.6f;
        Mark(c, 0f, Life + 0.4f, Links, Ice, spot, _ => cage * 1.5f, k => Fade(k, 0.04f, 0.3f), count: 6f, spin: 2f, gain: 1.4f);
        Cue(c, 0.1f, () => Stage.Patch(At(c, spot), cage * 1.5f, Life, FrostColor, FxGround.Frost));
        for (var i = 0; i < Bars; i++)
        {
            var foot = spot + Around(i * Mathf.Tau / Bars) * cage;
            var start = i * 0.02f;
            var life = Life - start;
            Tube(c, start, life, Beam, Ice, k => (foot, foot + Vector3.Up * Tall * Out(Mathf.Min(k * life / Rise, 1f))), _ => 0.07f, k => Fade(k, 0f, 0.05f), speed: 1.5f, gain: 1.7f);
            Glyph(c, start + Rise, life - Rise, Gem, i % 2 == 0 ? Ice : Pearl, _ => foot + Vector3.Up * (Tall + 0.15f), _ => 0.45f, k => Fade(k, 0.05f, 0.05f), gain: 1.8f);
            Disc(c, start, 0.3f, Shock, Ice, foot, k => 0.5f * Out(k), k => 1f - k, band: 0.1f);
            // At the end each bar breaks into shards.
            for (var j = 0; j < 3; j++)
            {
                var from = foot + Vector3.Up * Rand(0.3f, Tall);
                var v = (foot - spot).Normalized() * Rand(2f, 5f) + Vector3.Up * Rand(0f, 3f);
                var spin = Rand(0f, Mathf.Tau);
                Glyph(c, Life, 0.5f, Gem, Ice, k => from + v * k * 0.5f + Vector3.Down * 0.5f * Gravity * k * k * 0.25f, k => 0.5f * (1f - 0.5f * k), k => 1f - k, turn: k => Tumble(spin + k * 9f, spin), gain: 1.7f);
            }
        }
        foreach (var high in new[] { Tall, Tall * 0.5f })
            Disc(c, Rise, Hold, Band, Ice, k => Lay(spot + Vector3.Up * high, cage * 1.04f), k => Fade(k, 0.05f, 0.03f), band: 0.08f, gain: 1.6f);
        Disc(c, Rise, 0.4f, Shock, Ice, spot, k => r * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Twinkle(c, 26, Rise, Life, Pearl, () => spot + Around(Rand(0f, Mathf.Tau)) * cage + Vector3.Up * Rand(0.2f, Tall), 0.5f);
        Hit(c, Life, Ice, spot + Vector3.Up, r * 0.7f);
        Shine(c, spot + Vector3.Up, r * 1.3f, Ice.Main, age => age < Life ? 0.9f : 2f * Mathf.Max(0f, 1f - (age - Life) / 0.5f));
    }

    private void FrostCompass(Cast c, float r)
    {
        const float Draw = 0.45f, After = 1.5f;
        Mark(c, 0f, Draw + After, Compass, Ice, Vector3.Zero, _ => r, k => Fade(k, 0.03f, 0.3f), spin: 0f, gain: 1.6f);
        Disc(c, 0f, Draw, Glow, Ice, Vector3.Zero, _ => r * 0.8f, k => 0.2f + 0.4f * k);
        // Ice runs out along the long points first, then the short.
        for (var i = 0; i < 8; i++)
        {
            var way = Around(i * Mathf.Tau / 8f);
            var long1 = i % 2 == 0;
            var when = Draw + (long1 ? 0f : 0.15f);
            var steps = long1 ? 4 : 2;
            for (var j = 0; j < steps; j++)
            {
                var far = r * (0.25f + 0.2f * j);
                var delay = j * 0.05f;
                var tall = (long1 ? 1.7f : 1.1f) - 0.2f * j;
                var foot = way * far;
                Cue(c, when, () => Stage.Spike(At(c, foot), c.Root.Basis * way, tall, FxSpike.Ice, delay));
            }
            Line(c, when, 0.5f, Ice, k => way * r * 0.5f * Out(k) + Vector3.Up * 0.1f, k => way * r * (long1 ? 0.95f : 0.6f) * Out(k), 0.3f, k => (1f - k) * (1f - k));
            Glyph(c, when + 0.2f, 0.4f, Glint, Pearl, way * r * (long1 ? 0.9f : 0.55f) + Vector3.Up * 0.8f, 1.4f, 1.7f);
        }
        Cue(c, Draw, () => Stage.Explosion(At(c, Vector3.Up * 0.4f), r * 0.45f, FrostColor, FxBurst.Frost));
        Cue(c, Draw, () => Stage.Patch(At(c, Vector3.Zero), r * 0.95f, 2.4f, FrostColor, FxGround.Frost));
        Disc(c, Draw, 0.5f, Shock, Ice, Vector3.Zero, k => r * 1.4f * Out(k), k => 1f - k, band: 0.14f, gain: 1.5f);
        Flare(c, Draw, 0.35f, Star, Ice, Vector3.Up * 0.6f, 4f);
        Twinkle(c, 24, Draw, Draw + After - 0.4f, Pearl, () => Within(Vector3.Zero, r * 0.9f, 0.2f, 1.8f), 0.5f);
        Shine(c, Vector3.Up, r * 1.6f, Ice.Main, age => age < Draw ? 0.5f : 2.2f * Mathf.Max(0f, 1f - (age - Draw) / 0.9f));
    }

    private void CrystalRain(Cast c, float r)
    {
        const int Shards = 12;
        const float Fall = 0.28f, Stand1 = 0.9f, Every = 0.08f;
        var centre = new Vector3(0f, 0f, r * 0.15f);
        Mark(c, 0f, Shards * Every + Fall + 0.8f, Frostflake, Ice, centre, _ => r * 0.9f, k => Fade(k, 0.04f, 0.3f), gain: 1.1f);
        var spots = Spread(Shards, r * 0.85f);
        for (var i = 0; i < Shards; i++)
        {
            var spot = centre + spots[i] * (i % 3 == 0 ? 0.4f : 1f);
            var start = i * Every;
            var lean = (Vector3.Up + Around(Rand(0f, Mathf.Tau)) * 0.25f).Normalized();
            var tall = Rand(1.1f, 1.8f);
            var sky = spot + lean * 9f;
            Prop(c, start, Fall + Stand1, ShardMesh, IceBlock, k =>
            {
                var t = k * (Fall + Stand1);
                var fall = Mathf.Min(t / Fall, 1f);
                var sink = Mathf.Clamp((t - Fall - Stand1 + 0.3f) / 0.3f, 0f, 1f);
                return new Transform3D(Along(lean, 1f, 1f) * Basis.FromScale(new Vector3(1.3f, tall / 0.41f, 1.3f) * (1f - sink * 0.9f)), sky.Lerp(spot + lean * tall * 0.3f, fall * fall));
            });
            Line(c, start, Fall, Ice, k => sky.Lerp(spot, k * k) + lean * 1.4f, _ => lean * 2.4f, 0.09f, _ => 0.7f);
            Disc(c, start + Fall, 0.3f, Shock, Ice, spot, k => 1f * Out(k), k => 1f - k, band: 0.1f, gain: 1.3f);
            Glyph(c, start + Fall, 0.35f, Glint, Pearl, spot + lean * tall * 0.7f, 1.5f, 1.8f);
            Disc(c, start + Fall, Stand1, Cracks, Ice, spot, _ => 0.6f, k => Fade(k, 0.02f, 0.4f), k => k);
            Spray(c, start + Fall, Ice, spot + Vector3.Up * 0.1f, 5, 2.5f, 4f, 0.4f);
        }
        Cue(c, Fall + 0.3f, () => Stage.Patch(At(c, centre), r * 0.85f, 2.4f, FrostColor, FxGround.Frost));
        Shine(c, centre + Vector3.Up, r * 1.5f, Ice.Main, age => 1.2f * Fade(age / (Shards * Every + Fall + 0.6f), 0.1f, 0.3f));
    }

    private void Avalanche(Cast c, float r)
    {
        const float Life = 1.5f;
        var reach = r * 1.8f;
        var half = r * 0.8f;
        Func<float, float> front = k => 0.4f + (reach - 0.4f) * Mathf.SmoothStep(0f, 1f, k);
        // A tumbling front of powder with blocks of ice turning over in it.
        for (var i = 0; i < 44; i++)
        {
            var x = Rand(-half, half);
            var lag = Rand(0f, 1.2f);
            var high = Rand(0.3f, 1.8f);
            var size = Rand(1.4f, 2.4f);
            var start = Rand(0f, 0.25f);
            var roll = Rand(4f, 9f);
            Glyph(c, start, Life - start, Puff, i % 3 == 0 ? Ice : Pearl, k => new Vector3(x, high * (0.6f + 0.4f * Mathf.Abs(Mathf.Sin(k * roll + i))), front(k) - lag), k => size * (0.7f + 0.5f * k), k => Fade(k, 0.15f, 0.3f) * 0.75f, gain: 0.95f);
        }
        for (var i = 0; i < 14; i++)
        {
            var x = Rand(-half, half);
            var lag = Rand(0f, 0.8f);
            var size = Rand(0.8f, 1.6f);
            var axis = FxKit.RandomDirection();
            var hop = Rand(5f, 9f);
            var phase = Rand(0f, Mathf.Tau);
            Prop(c, 0.1f, Life - 0.1f, ShardMesh, IceBlock, k => new Transform3D(new Basis(axis, k * 14f) * Basis.FromScale(Vector3.One * size * Mathf.Min(1f, k * 6f) * Mathf.Min(1f, (1f - k) * 5f)),
                new Vector3(x, 0.3f + 0.9f * Mathf.Abs(Mathf.Sin(k * hop + phase)), front(k) - lag)));
        }
        Wall(c, 0f, Life, 1, Pearl, k => (new Vector3(-half, 0f, front(k) + 0.2f), new Vector3(half, 0f, front(k) + 0.2f)), k => 1.6f * Fade(k, 0.15f, 0.2f), k => Fade(k, 0.05f, 0.1f) * 0.7f, speed: 2f, gain: 1.4f);
        Disc(c, 0f, Life + 0.8f, Glow, Ice, new Vector3(0f, 0f, reach * 0.5f), _ => 1f, k => Fade(k, 0.1f, 0.5f) * 0.45f, stretch: new Vector3(half, 1f, reach * 0.5f));
        for (var i = 0; i < 4; i++)
        {
            var z = reach * (i + 1f) / 4.5f;
            var when = Life * (i + 1f) / 5f;
            Cue(c, when, () => Stage.Patch(At(c, new Vector3(0f, 0f, z)), half * 0.9f, 2f, FrostColor, FxGround.Frost));
        }
        var end = new Vector3(0f, 0f, reach);
        Disc(c, Life - 0.15f, 0.5f, Shock, Ice, end, k => r * 1.2f * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
        Spray(c, Life - 0.15f, Ice, end + Vector3.Up * 0.4f, 30, 5f, 6f, 0.6f, 0.08f);
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Ice.Main, age => 0.9f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void FrostWindow(Cast c, float r)
    {
        const float Form = 0.6f, Hold = 1.3f, Life = Form + Hold;
        const float Size = 2.2f;
        var at = new Vector3(0f, Size + 0.1f, r * 0.25f);
        var face = Stand(Vector3.Back);
        Mark(c, 0f, Life, Window, Ice, k => new Transform3D(face * Basis.FromScale(Vector3.One * Size), at), k => Fade(k, 0.02f, 0.02f), k => Mathf.Min(k * Life / Form, 1f), count: 8f, spin: 0f, gain: 1.8f);
        Mark(c, 0f, Life, Frostflake, Pearl, k => new Transform3D(face * Basis.FromScale(Vector3.One * Size * 0.45f), at + Vector3.Back * 0.03f), k => Fade(k, 0.1f, 0.02f) * 0.8f, k => Mathf.Min(k * Life / Form, 1f), spin: 2f, gain: 1.4f);
        Disc(c, 0f, Life, Glow, Ice, k => new Transform3D(face * Basis.FromScale(Vector3.One * Size * 0.95f), at + Vector3.Forward * 0.03f), k => Fade(k, 0.2f, 0.02f) * 0.35f);
        Mark(c, 0f, Life + 0.5f, Window, Ice, FxKit.Flat(at) + Vector3.Back * 1.6f, _ => 1.6f, k => Fade(k, 0.2f, 0.4f) * 0.5f, count: 8f, spin: 0f);
        Twinkle(c, 36, 0.1f, Life, Pearl, () => at + new Vector3(Rand(-Size, Size) * 0.7f, Rand(-Size, Size) * 0.7f, 0.1f), 0.55f);
        Line(c, 0f, Life, Ice, _ => FxKit.Flat(at) + Vector3.Up * 0.08f, _ => Vector3.Right * Size * 1.6f, 0.3f, k => Fade(k, 0.1f, 0.05f) * 0.8f);
        // It breaks outward: panes fly, turning.
        Hit(c, Life, Ice, at, r * 0.8f);
        Bolt(c, Life - 0.15f, 0.15f, Pearl, at + new Vector3(-1.5f, 1.3f, 0.05f), at + new Vector3(1.2f, -1.6f, 0.05f), 0.05f, 7, 0.3f);
        for (var i = 0; i < 40; i++)
        {
            var from = at + new Vector3(Rand(-Size, Size) * 0.7f, Rand(-Size, Size) * 0.7f, 0f);
            var v = new Vector3(Rand(-2f, 2f), Rand(-1f, 3f), Rand(3f, 8f));
            var size = Rand(0.4f, 0.9f);
            var spin = Rand(0f, Mathf.Tau);
            var turn = Rand(-10f, 10f);
            Glyph(c, Life, Rand(0.5f, 0.8f), Gem, i % 3 == 0 ? Pearl : Ice, k => from + v * k * 0.6f + Vector3.Down * 0.5f * Gravity * k * k * 0.3f, k => size * (1f - 0.4f * k), k => 1f - k * k, turn: k => Tumble(spin + k * turn, spin), gain: 1.8f, slim: 0.7f);
        }
        Cue(c, Life, () => Stage.Patch(At(c, FxKit.Flat(at) + Vector3.Back * 1.6f), r * 0.6f, 2f, FrostColor, FxGround.Frost));
        Shine(c, at, r * 1.5f, Ice.Main, age => age < Life ? 0.8f : 2.2f * Mathf.Max(0f, 1f - (age - Life) / 0.5f));
    }

    // ---- Storm.

    private void GearStorm(Cast c, float r)
    {
        const float Life = 3f;
        var big = Vector3.Zero;
        var small = Around(0.6f) * r * 0.98f;
        Mark(c, 0f, Life, Gear, Cyan, big, _ => r * 0.75f, k => Fade(k, 0.04f, 0.15f), count: 12f, spin: 2f, gain: 1.5f);
        Mark(c, 0.15f, Life - 0.15f, Gear, Storm, small, _ => r * 0.38f, k => Fade(k, 0.04f, 0.15f), count: 6f, spin: -4f, gain: 1.5f);
        Mark(c, 0.3f, Life - 0.3f, Gear, Gold, k => Lay(Vector3.Up * 1.6f, r * 0.42f), k => Fade(k, 0.04f, 0.15f), count: 8f, spin: -3f, gain: 1.6f);
        Column(c, 0.3f, Life - 0.3f, Beam, Cyan, Vector3.Zero, _ => new Vector2(0.07f, 1.6f), k => Fade(k, 0.05f, 0.15f), speed: 3f, gain: 1.5f);
        // Sparks where the teeth mesh, and arcs thrown off the rims.
        var mesh = Around(0.6f) * r * 0.68f + Vector3.Up * 0.15f;
        for (var t = 0.3f; t < Life - 0.3f; t += 0.12f)
        {
            for (var j = 0; j < 3; j++)
                Spark(c, t, 0.35f, j % 2 == 0 ? Cyan : Gold, mesh, FxKit.Jitter(2.5f) + Vector3.Up * 3f, Gravity, 0.05f, 0.03f);
            if ((int)(t / 0.12f) % 3 == 0)
            {
                var a = Around(Rand(0f, Mathf.Tau)) * r * 0.42f + Vector3.Up * 1.6f;
                var b = Around(Rand(0f, Mathf.Tau)) * r * Rand(0.6f, 0.95f);
                Cue(c, t, () => Stage.Lightning([(At(c, a) - Vector3.Up * Fx.LineLift, At(c, b) - Vector3.Up * Fx.LineLift)], 0.13f, 5, 0.5f));
                Glyph(c, t, 0.25f, Glint, Cyan, b + Vector3.Up * 0.2f, 1.2f, 1.7f);
            }
        }
        Flare(c, 0.3f, Life - 0.3f, Star, Cyan, _ => mesh, k => 0.9f + 0.4f * Mathf.Sin(k * 90f), k => Fade(k, 0.05f, 0.15f));
        Shine(c, Vector3.Up, r * 1.4f, Cyan.Main, age => (1f + 0.25f * Mathf.Sin(age * 29f)) * Fade(age / Life, 0.05f, 0.15f));
    }

    private void StormCompass(Cast c, float r)
    {
        const float Each = 0.75f;
        const int Strikes = 3;
        var life = Strikes * Each + 0.6f;
        Mark(c, 0f, life, Compass, Storm, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.04f, 0.2f), spin: 1f, gain: 1.4f);
        // The needle swings, settles on a bearing, and the bolt comes down there.
        var bearings = new float[Strikes];
        var prev = 0f;
        for (var i = 0; i < Strikes; i++)
            bearings[i] = prev += Rand(3.5f, 6.5f);
        Func<float, float> needle = t =>
        {
            var i = Mathf.Min((int)(t / Each), Strikes - 1);
            var from = i == 0 ? 0f : bearings[i - 1];
            return Mathf.Lerp(from, bearings[i], Out(Mathf.Clamp((t - i * Each) / (Each * 0.7f), 0f, 1f)));
        };
        Line(c, 0f, life, Pearl, k => Around(needle(k * life)) * r * 0.3f + Vector3.Up * 0.12f, k => Around(needle(k * life)) * r * 1.3f, 0.16f, k => Fade(k, 0.05f, 0.15f));
        Line(c, 0f, life, Storm, k => Around(needle(k * life)) * r * 0.3f + Vector3.Up * 0.1f, k => Around(needle(k * life)) * r * 1.4f, 0.4f, k => Fade(k, 0.05f, 0.15f) * 0.5f);
        Flare(c, 0f, life, Star, Storm, _ => Vector3.Up * 0.3f, _ => 1.2f, k => Fade(k, 0.05f, 0.15f));
        for (var i = 0; i < Strikes; i++)
        {
            var spot = Around(bearings[i]) * r * 0.85f;
            var when = i * Each + Each * 0.75f;
            var sky = spot + new Vector3(Rand(-1f, 1f), 13f, Rand(-1f, 1f));
            Mark(c, when - 0.3f, 0.3f, Reticle, Storm, spot, _ => 0.8f, _ => 0.9f, count: 6f, spin: 6f);
            Cue(c, when, () => Stage.Lightning([(At(c, sky), At(c, spot))], 0.22f, 9, 1.2f));
            Cue(c, when, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.4f), 1f, FxTheme.Of(FxElement.Lightning), FxBurst.Lightning));
            Hit(c, when, Storm, spot + Vector3.Up * 0.2f, 1.1f);
            Disc(c, when, 1f, Cracks, Storm, spot, _ => 0.9f, k => Fade(k, 0.02f, 0.5f), k => k);
        }
        Shine(c, Vector3.Up, r * 1.6f, Storm.Main, age => 0.8f + 2f * Bump(age, Each * 0.75f, 0.3f) + 2f * Bump(age, Each * 1.75f, 0.3f) + 2f * Bump(age, Each * 2.75f, 0.3f));
    }

    private void VoltChains(Cast c, float r)
    {
        const float Life = 2.8f;
        const int Posts = 5;
        Mark(c, 0f, Life, Links, Cyan, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.04f, 0.15f), count: 8f, spin: 3f, gain: 1.5f);
        var tops = Enumerable.Range(0, Posts).Select(i => Around(i * Mathf.Tau / Posts) * r * 0.8f + Vector3.Up * 1.6f).ToArray();
        for (var i = 0; i < Posts; i++)
        {
            var top = tops[i];
            var start = i * 0.05f;
            Column(c, start, Life - start, Beam, Cyan, FxKit.Flat(top), k => new Vector2(0.06f, 1.6f * Out(Mathf.Min(k * 8f, 1f))), k => Fade(k, 0.02f, 0.1f), speed: 3f, gain: 1.5f);
            Globe(c, "energy", Orb, start, Life - start, Cyan, _ => top, _ => 0.2f, k => Fade(k, 0.03f, 0.1f), gain: 1.7f);
            Flare(c, start, Life - start, Star, Cyan, _ => top, k => 1f + 0.3f * Mathf.Sin(k * 70f + i), k => Fade(k, 0.03f, 0.1f));
            Mark(c, start, Life - start, Links, Storm, FxKit.Flat(top), _ => 0.5f, k => Fade(k, 0.05f, 0.15f), count: 3f, spin: -5f);
        }
        // The current runs round the ring post to post, and every so often cuts across.
        for (var t = 0.3f; t < Life - 0.2f; t += 0.09f)
        {
            var n = (int)(t / 0.09f);
            var (a, b) = (tops[n % Posts], tops[(n + 1) % Posts]);
            Bolt(c, t, 0.14f, n % 2 == 0 ? Cyan : Pearl, a, b, 0.07f, 6, 0.25f);
            if (n % 5 == 0)
            {
                var (p, q) = (tops[n % Posts], tops[(n + 2) % Posts]);
                Cue(c, t, () => Stage.Lightning([(At(c, p) - Vector3.Up * Fx.LineLift, At(c, q) - Vector3.Up * Fx.LineLift)], 0.14f, 6, 0.6f));
            }
        }
        Disc(c, 0f, Life, Glow, Cyan, Vector3.Zero, _ => r * 0.9f, k => Fade(k, 0.1f, 0.15f) * 0.25f);
        Shine(c, Vector3.Up, r * 1.5f, Cyan.Main, age => (1.1f + 0.3f * Mathf.Sin(age * 35f)) * Fade(age / Life, 0.05f, 0.12f));
    }

    private void SkyArrows(Cast c, float r)
    {
        const int Arrows = 9;
        const float Fall = 0.22f, Every = 0.11f, Stuck = 0.6f;
        var centre = new Vector3(0f, 0f, r * 0.15f);
        Mark(c, 0f, Arrows * Every + Fall + 0.6f, Reticle, Storm, centre, _ => r * 0.95f, k => Fade(k, 0.04f, 0.3f), count: 12f, spin: 1.5f, gain: 1.2f);
        var spots = Spread(Arrows, r * 0.85f);
        for (var i = 0; i < Arrows; i++)
        {
            var spot = centre + spots[i];
            var start = i * Every;
            var sky = spot + new Vector3(0.6f, 11f, -0.6f);
            var way = (spot - sky).Normalized();
            Func<float, Vector3> at = k => sky.Lerp(spot + Vector3.Up * 0.9f, k);
            Mark(c, start, Fall + 0.1f, Reticle, Cyan, spot, _ => 0.6f, _ => 0.8f, count: 4f, spin: 8f);
            Glyph(c, start, Fall, Shaft, Pearl, at, _ => 2.4f, _ => 1f, _ => way, gain: 2f, slim: 0.5f);
            Line(c, start, Fall, Storm, k => at(k) - way * 2f, _ => way * 3.5f, 0.14f, _ => 0.9f);
            var sunk = Rand(0.25f, 0.45f);
            Glyph(c, start + Fall, Stuck, Shaft, Storm, _ => spot - way * 2.4f * (0.5f - sunk), _ => 2.4f, k => 1f - k * k, _ => way, gain: 1.6f, slim: 0.5f, sink: sunk);
            Hit(c, start + Fall, Storm, spot + Vector3.Up * 0.2f, 0.8f);
            // The charge it carried earths itself.
            var next = spot + Around(Rand(0f, Mathf.Tau)) * 1.3f;
            Bolt(c, start + Fall, 0.2f, Cyan, spot + Vector3.Up * 1.6f, next + Vector3.Up * 0.1f, 0.05f, 5, 0.2f);
            Disc(c, start + Fall, 0.8f, Cracks, Storm, spot, _ => 0.6f, k => Fade(k, 0.02f, 0.5f), k => k);
        }
        Shine(c, centre + Vector3.Up, r * 1.5f, Storm.Main, age => 1.3f * Fade(age / (Arrows * Every + Fall + 0.4f), 0.1f, 0.3f));
    }

    private void TeslaGate(Cast c, float r)
    {
        const float Life = 3f;
        const float Tall = 2.6f;
        var left = new Vector3(-r * 0.75f, 0f, r * 0.25f);
        var right = new Vector3(r * 0.75f, 0f, r * 0.25f);
        foreach (var foot in new[] { left, right })
        {
            Mark(c, 0f, Life, Gear, Cyan, foot, _ => 0.8f, k => Fade(k, 0.04f, 0.15f), count: 8f, spin: 3f, gain: 1.4f);
            Column(c, 0f, Life, Beam, Cyan, foot, k => new Vector2(0.1f, Tall * Out(Mathf.Min(k * Life / 0.25f, 1f))), k => Fade(k, 0.03f, 0.1f), speed: 3f, gain: 1.6f);
            Column(c, 0f, Life, Spiral, Storm, foot, k => new Vector2(0.22f, Tall * Out(Mathf.Min(k * Life / 0.25f, 1f))), k => Fade(k, 0.03f, 0.1f), twist: 4f, speed: 3f, gain: 1.4f);
            Globe(c, "energy", Orb, 0.2f, Life - 0.2f, Cyan, _ => foot + Vector3.Up * Tall, k => 0.3f * Overshoot(Mathf.Min(k * 6f, 1f)), k => Fade(k, 0.03f, 0.1f), gain: 1.7f);
            Flare(c, 0.2f, Life - 0.2f, Star, Cyan, _ => foot + Vector3.Up * Tall, k => 1.4f + 0.3f * Mathf.Sin(k * 80f), k => Fade(k, 0.03f, 0.1f));
        }
        Wall(c, 0.35f, Life - 0.35f, 0, Cyan, left, right, k => Tall * Out(Mathf.Min(k * 6f, 1f)), k => Fade(k, 0.03f, 0.12f) * 0.9f, k => Mathf.Max(0f, (k - 0.92f) / 0.08f), speed: 2f, gain: 1.5f);
        for (var t = 0.35f; t < Life - 0.2f; t += 0.1f)
        {
            var ha = Rand(0.2f, Tall);
            var hb = Rand(0.2f, Tall);
            Bolt(c, t, 0.14f, (int)(t / 0.1f) % 3 == 0 ? Pearl : Cyan, left + Vector3.Up * ha, right + Vector3.Up * hb, 0.06f, 9, 0.35f);
        }
        Line(c, 0.35f, Life - 0.35f, Cyan, _ => (left + right) * 0.5f + Vector3.Up * 0.08f, _ => right - left, 0.5f, k => Fade(k, 0.05f, 0.12f) * 0.8f);
        Twinkle(c, 26, 0.4f, Life - 0.3f, Pearl, () => left.Lerp(right, Rand(0f, 1f)) + Vector3.Up * Rand(0.2f, Tall), 0.5f, 0.25f);
        Shine(c, (left + right) * 0.5f + Vector3.Up, r * 1.6f, Cyan.Main, age => (1.2f + 0.3f * Mathf.Sin(age * 33f)) * Fade(age / Life, 0.05f, 0.12f));
    }

    private void CycloneWhorl(Cast c, float r)
    {
        const float Life = 3f;
        var reach = r * 0.6f;
        Mark(c, 0f, Life, Whorl, Wind, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.15f), count: 8f, spin: -16f, gain: 1.4f);
        Disc(c, 0f, Life, Swirl, Wind, Vector3.Zero, _ => r, k => Fade(k, 0.08f, 0.15f) * 0.6f, spin: 9f);
        Column(c, 0f, Life, Spiral, Wind, Vector3.Zero, k => new Vector2(reach * (0.45f + 0.08f * Mathf.Sin(k * 24f)), 6f * Out(Mathf.Min(k * Life / 0.4f, 1f))), k => Fade(k, 0.05f, 0.15f) * 0.75f, twist: 3f, speed: 4f, gain: 1.4f);
        Column(c, 0f, Life, Spiral, Pearl, Vector3.Zero, k => new Vector2(reach * 0.25f, 5f * Out(Mathf.Min(k * Life / 0.4f, 1f))), k => Fade(k, 0.05f, 0.15f) * 0.6f, twist: 4f, speed: 5f, gain: 1.2f);
        for (var i = 0; i < 4; i++)
        {
            var phase = i * Mathf.Tau / 4f;
            var start = i * 0.4f;
            Func<float, Vector3> path = s => Around(phase + s * 14f) * reach * (0.35f + 0.7f * s) + Vector3.Up * (0.2f + 4.4f * s);
            Ribbon(c, start, Life - start - 0.2f, 0, i % 2 == 0 ? Wind : Pearl, path, k => (Mathf.Max(0f, k - 0.3f), k), 0.5f, k => Fade(k, 0.03f, 0.15f), gain: 1.4f);
        }
        for (var i = 0; i < 44; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            var high = Rand(0.2f, 4f);
            var far = reach * (0.4f + 0.2f * high);
            Func<float, Vector3> at = k => Around(phase + k * 6f) * far + Vector3.Up * (high + 0.8f * k);
            Line(c, Rand(0f, Life - 0.45f), 0.45f, i % 4 == 0 ? Dust : Wind, at, k => Heading(at, k, 1f), 0.05f, k => Fade(k, 0.2f, 0.3f) * 0.8f);
        }
        Drift(c, 20, 0f, Life - 1f, 1f, Leaflet, [Leaf, Dust], () => Within(Vector3.Zero, reach, 0.2f, 1f), () => Around(Rand(0f, Mathf.Tau)) * 2f + Vector3.Up * Rand(1.5f, 3f), 0.3f, sway: 0.6f, slim: 0.5f);
        Shine(c, Vector3.Up, r * 1.2f, Wind.Main, age => 0.8f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void ShockTiles(Cast c, float r)
    {
        const float Life = 3f;
        Mark(c, 0f, Life, Checker, Cyan, Vector3.Zero, _ => r, k => Fade(k, 0.04f, 0.15f), count: 8f, spin: 1.5f, gain: 1.5f);
        Disc(c, 0f, Life, Glow, Cyan, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.15f) * 0.2f);
        // The board's squares are r / 4 across and lie on the diagonal.
        var step = r / 4f;
        var diag = new Basis(Vector3.Up, -Mathf.Pi * 0.25f);
        for (var t = 0.3f; t < Life - 0.4f; t += 0.07f)
        {
            var cell = new Vector3(Mathf.Floor(Rand(-3.5f, 3.5f)) + 0.5f, 0f, Mathf.Floor(Rand(-3.5f, 3.5f)) + 0.5f) * step;
            var spot = diag * cell;
            if (spot.Length() > r * 0.9f)
                continue;
            Column(c, t, 0.35f, Beam, Cyan, spot, k => new Vector2(step * 0.3f, 2.4f * Out(Mathf.Min(k * 5f, 1f))), k => 1f - k, speed: 5f, gain: 1.6f);
            Disc(c, t, 0.4f, Glow, Pearl, k => Lay(spot, step * 0.62f, new Basis(Vector3.Up, Mathf.Pi * 0.25f)), k => 1f - k, gain: 1.6f);
            Glyph(c, t, 0.3f, Glint, Pearl, spot + Vector3.Up * 0.3f, 1.1f, 1.7f);
            for (var j = 0; j < 2; j++)
                Spark(c, t, 0.3f, Cyan, spot + Vector3.Up * 0.1f, FxKit.Jitter(1.5f) + Vector3.Up * 4f, Gravity, 0.05f, 0.03f);
        }
        for (var t = 0.4f; t < Life - 0.3f; t += 0.5f)
        {
            var (a, b) = (Within(Vector3.Zero, r * 0.8f), Within(Vector3.Zero, r * 0.8f));
            Cue(c, t, () => Stage.Lightning([(At(c, a + Vector3.Up * 0.1f), At(c, b + Vector3.Up * 0.1f))], 0.15f, 5, 0.5f));
        }
        Shine(c, Vector3.Up, r * 1.5f, Cyan.Main, age => (1f + 0.3f * Mathf.Sin(age * 37f)) * Fade(age / Life, 0.05f, 0.12f));
    }

    // ---- Water.

    private void ScaleShield(Cast c, float r)
    {
        const float Life = 3f;
        var reach = r * 0.45f;
        Mark(c, 0f, Life, Scales, Sea, Vector3.Zero, _ => reach * 1.6f, k => Fade(k, 0.05f, 0.15f), count: 10f, spin: 1.5f, gain: 1.5f);
        Globe(c, "energy", Hexes, 0f, Life, Sea, _ => Vector3.Up * 1f, k => 1.3f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.03f, 0.1f) * 0.7f, gain: 1.2f);
        // Plates of scale turning round them.
        for (var i = 0; i < 4; i++)
        {
            var phase = i * Mathf.Tau / 4f;
            Mark(c, 0.1f, Life - 0.1f, Scales, i % 2 == 0 ? Foam : Sea, k =>
            {
                var angle = phase + k * Life * 1.4f;
                return new Transform3D(Stand(Around(angle)) * Basis.FromScale(Vector3.One * 0.8f * Out(Mathf.Min(k * Life / 0.3f, 1f))), Around(angle) * reach * 1.25f + Vector3.Up * 1.1f);
            }, k => Fade(k, 0.03f, 0.1f), _ => 1f, count: 5f, spin: 2f, gain: 1.7f);
        }
        Disc(c, 0f, 0.5f, Shock, Foam, Vector3.Zero, k => r * 0.9f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Drift(c, 22, 0f, Life - 1.1f, 1.1f, Bubble, [Foam], () => Within(Vector3.Zero, reach * 1.2f, 0.1f, 0.6f), () => Vector3.Up * Rand(0.8f, 1.6f), 0.22f, tumble: false, gain: 1.4f);
        Twinkle(c, 20, 0f, Life - 0.4f, Foam, () => Vector3.Up + FxKit.RandomDirection() * 1.35f, 0.5f);
        Shine(c, Vector3.Up, r, Sea.Main, age => 0.9f * Fade(age / Life, 0.05f, 0.1f));
    }

    private void TidalWhorl(Cast c, float r)
    {
        const float Draw = 1.6f, After = 0.9f;
        Mark(c, 0f, Draw + After, Whorl, Sea, Vector3.Zero, _ => r, k => Fade(k, 0.05f, 0.25f), count: 8f, spin: -12f, gain: 1.5f);
        Mark(c, 0f, Draw + After, Caustics, Sea, Vector3.Zero, _ => r, k => Fade(k, 0.08f, 0.25f) * 0.8f, count: 6f, spin: 2f);
        Disc(c, 0f, Draw, Glow, Sea, Vector3.Zero, _ => r * 0.35f, k => 0.3f + 0.5f * k);
        for (var i = 0; i < 44; i++)
        {
            var phase = Rand(0f, Mathf.Tau);
            Func<float, Vector3> at = k => Around(phase + k * 5.5f) * r * (1f - 0.9f * k) + Vector3.Up * 0.12f;
            Line(c, Rand(0f, Draw - 0.8f), 0.8f, i % 3 == 0 ? Sea : Foam, at, k => Heading(at, k, 0.6f), 0.06f, k => Fade(k, 0.2f, 0.2f));
        }
        for (var i = 0; i < 4; i++)
            Disc(c, i * 0.4f, 0.8f, Shock, Foam, Vector3.Zero, k => r * (1f - 0.85f * Out(k)), k => Fade(k, 0.2f, 0.3f) * 0.8f, band: 0.08f);
        // It gathers, and goes up in a spout.
        Column(c, Draw, After, Spiral, Sea, Vector3.Zero, k => new Vector2(r * 0.25f, 6f * Out(Mathf.Min(k * 5f, 1f)) * (1f - 0.6f * Mathf.Max(0f, (k - 0.6f) / 0.4f))), k => Fade(k, 0f, 0.3f), twist: 2f, speed: 4f, gain: 1.6f);
        Column(c, Draw, After, Light, Foam, Vector3.Zero, k => new Vector2(r * 0.15f, 6.6f * Out(Mathf.Min(k * 5f, 1f)) * (1f - 0.6f * Mathf.Max(0f, (k - 0.6f) / 0.4f))), k => Fade(k, 0f, 0.3f) * 0.7f, speed: 3f, gain: 1.6f);
        Hit(c, Draw, Foam, Vector3.Up * 0.3f, r * 0.8f);
        Cue(c, Draw, () => Stage.Explosion(At(c, Vector3.Up * 0.4f), r * 0.5f, WaterColor, FxBurst.Dome));
        Disc(c, Draw + 0.1f, 0.6f, Shock, Sea, Vector3.Zero, k => r * 1.5f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        for (var i = 0; i < 40; i++)
            Spark(c, Rand(Draw, Draw + 0.4f), Rand(0.6f, 0.9f), i % 2 == 0 ? Foam : Sea, Vector3.Up * Rand(2f, 5.5f), Around(Rand(0f, Mathf.Tau)) * Rand(1.5f, 4f) + Vector3.Up * Rand(0f, 3f), Gravity, 0.08f, 0.04f);
        Shine(c, Vector3.Up, r * 1.5f, Sea.Main, age => age < Draw ? 0.7f : 2f * Mathf.Max(0f, 1f - (age - Draw) / After));
    }

    private void SilverShoal(Cast c, float r)
    {
        const float Life = 3.2f;
        var ring = r * 0.5f;
        Mark(c, 0f, Life, Caustics, Sea, Vector3.Zero, _ => ring * 1.8f, k => Fade(k, 0.08f, 0.15f), count: 6f, spin: 1.5f, gain: 1.2f);
        Mark(c, 0f, Life, Scales, Foam, Vector3.Zero, _ => ring * 1.1f, k => Fade(k, 0.05f, 0.15f) * 0.6f, count: 9f, spin: 1f);
        // A shoal: each fish a sliver of silver, all turning together, a few breaking away and back.
        for (var i = 0; i < 26; i++)
        {
            var phase = i * 0.5f + Rand(-0.15f, 0.15f);
            var far = ring * Rand(0.85f, 1.25f);
            var high = 0.6f + (i % 4) * 0.35f + Rand(-0.1f, 0.1f);
            var wave = Rand(0f, Mathf.Tau);
            var size = Rand(0.5f, 0.8f);
            Func<float, Vector3> at = k => Around(phase + k * Life * 2.2f) * far * (1f + 0.12f * Mathf.Sin(k * 9f + wave)) + Vector3.Up * (high + 0.15f * Mathf.Sin(k * 7f + wave));
            Glyph(c, 0f, Life, Leaflet, i % 3 == 0 ? Sea : Pearl, at, _ => size, k => Fade(k, 0.05f, 0.08f) * (0.75f + 0.25f * Mathf.Sin(k * 40f + wave)), k => Heading(at, k, 1f), gain: 1.8f, slim: 0.42f);
        }
        for (var i = 0; i < 20; i++)
        {
            var k0 = Rand(0.05f, 0.9f);
            Mark(c, k0 * Life, 0.6f, Ripples, Foam, Within(Vector3.Zero, ring * 1.3f), k => 0.2f + 0.5f * k, k => (1f - k) * 0.8f, count: 2f, spin: 0f);
        }
        Drift(c, 20, 0f, Life - 1f, 1f, Bubble, [Foam], () => Within(Vector3.Zero, ring * 1.3f, 0.2f, 1.6f), () => Vector3.Up * Rand(0.6f, 1.2f), 0.18f, tumble: false, gain: 1.4f);
        Twinkle(c, 24, 0f, Life - 0.4f, Pearl, () => Around(Rand(0f, Mathf.Tau)) * ring * Rand(0.8f, 1.3f) + Vector3.Up * Rand(0.5f, 1.9f), 0.45f, 0.25f);
        Shine(c, Vector3.Up, r, Sea.Main, age => 0.8f * Fade(age / Life, 0.06f, 0.15f));
    }

    private void RainbowMist(Cast c, float r)
    {
        const float Draw = 0.9f, Life = 3f;
        var foot = new Vector3(0f, 0f, r * 0.1f);
        var span = r * 0.9f;
        Mark(c, 0f, Life, Ripples, Foam, foot, _ => r, k => Fade(k, 0.1f, 0.25f) * 0.5f, count: 3f, spin: 0.4f);
        // An arch of six colours drawn across from one side, standing in mist.
        for (var i = 0; i < Prism.Length; i++)
        {
            var far = span * (1f - 0.075f * i);
            Func<float, Vector3> arch = s => foot + new Vector3(-Mathf.Cos(s * Mathf.Pi) * far, Mathf.Sin(s * Mathf.Pi) * far * 1.05f, 0f);
            for (var seg = 0; seg < 6; seg++)
            {
                var (s0, s1) = (seg / 6f, (seg + 1) / 6f);
                var tone = Prism[i];
                // Short streaks laid end to end along the arch, so it stands square to the camera.
                Line(c, s0 * Draw, Life - s0 * Draw, tone, _ => (arch(s0) + arch(s1)) * 0.5f, k => (arch(s1) - arch(s0)) * 1.08f * Mathf.Min(k * (Life - s0 * Draw) / (Draw / 6f), 1f), 0.2f, k => Fade(k, 0f, 0.25f) * 0.9f);
            }
        }
        foreach (var side in new[] { -1f, 1f })
        {
            var end = foot + Vector3.Right * side * span * 0.84f;
            for (var i = 0; i < 12; i++)
            {
                var at = Within(end, 0.9f, 0.2f, 0.9f);
                var size = Rand(1.2f, 2f);
                Glyph(c, Rand(0f, Life - 1.6f), 1.6f, Puff, i % 3 == 0 ? Sea : Foam, k => at + Vector3.Up * 0.3f * k, k => size * (0.8f + 0.3f * k), k => Fade(k, 0.3f, 0.4f) * 0.6f, gain: 0.85f);
            }
            Mark(c, 0f, Life, Caustics, Sea, end, _ => 1.3f, k => Fade(k, 0.1f, 0.25f) * 0.8f, count: 5f, spin: 1f);
        }
        Twinkle(c, 40, 0.3f, Life - 0.5f, Pearl, () => foot + new Vector3(Rand(-span, span), Rand(0.2f, span), Rand(-0.3f, 0.3f)), 0.45f);
        Drift(c, 20, Draw, Life - 1.2f, 1.2f, Glint, Prism, () => foot + new Vector3(Rand(-span, span) * 0.8f, Rand(span * 0.5f, span), 0f), () => Vector3.Down * Rand(0.6f, 1.2f), 0.4f, tumble: false, gain: 1.6f);
        Shine(c, foot + Vector3.Up, r * 1.4f, Foam.Main, age => 0.7f * Fade(age / Life, 0.1f, 0.25f));
    }

    private void SongOfTheDeep(Cast c, float r)
    {
        const float Life = 3.4f;
        Mark(c, 0f, Life, Caustics, Sea, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.2f), count: 6f, spin: 0.8f, gain: 1.2f);
        Mark(c, 0f, Life, Ripples, Foam, Vector3.Zero, _ => r, k => Fade(k, 0.1f, 0.2f) * 0.8f, count: 4f, spin: 0.5f, gain: 1.2f);
        // A long call: slow rings swelling up out of the water.
        for (var i = 0; i < 5; i++)
        {
            var start = i * 0.6f;
            Glyph(c, start, 1.4f, Bubble, i % 2 == 0 ? Sea : Foam, k => Vector3.Up * (0.4f + 2.2f * k), k => 1f + r * 1.6f * k, k => Fade(k, 0.15f, 0.5f) * 0.8f, gain: 1.4f);
            Disc(c, start, 1.2f, Shock, Sea, Vector3.Zero, k => r * 1.1f * Out(k), k => (1f - k) * 0.6f, band: 0.06f);
        }
        for (var i = 0; i < 22; i++)
        {
            var spot = Within(Vector3.Zero, r * 0.85f);
            var start = Rand(0f, Life - 1.5f);
            var sway = Rand(0f, Mathf.Tau);
            var size = Rand(0.5f, 0.8f);
            Func<float, Vector3> at = k => spot + Vector3.Up * (0.2f + 2.6f * k) + new Vector3(Mathf.Sin(k * 6f + sway), 0f, Mathf.Cos(k * 5f + sway)) * 0.25f;
            Glyph(c, start, 1.5f, Note, i % 3 == 0 ? Pearl : i % 3 == 1 ? Sea : Foam, at, _ => size, k => Fade(k, 0.15f, 0.35f), k => new Vector3(0.3f * Mathf.Sin(k * 6f + sway), 1f, 0f), gain: 1.8f);
            Mark(c, start, 0.6f, Ripples, Foam, spot, k => 0.2f + 0.5f * k, k => 1f - k, count: 2f, spin: 0f);
        }
        Twinkle(c, 24, 0.2f, Life - 0.5f, Pearl, () => Within(Vector3.Zero, r * 0.9f, 0.3f, 2.8f), 0.4f);
        Shine(c, Vector3.Up, r * 1.3f, Sea.Main, age => (0.7f + 0.2f * Mathf.Sin(age * Mathf.Tau / 0.6f)) * Fade(age / Life, 0.1f, 0.2f));
    }

    private void TearDrop(Cast c, float r)
    {
        const float Hang = 0.5f, Fall = 0.35f, After = 1.3f;
        var spot = new Vector3(0f, 0f, r * 0.1f);
        var high = spot + Vector3.Up * 5f;
        var land = Hang + Fall;
        Mark(c, 0f, land + After, Caustics, Sea, spot, _ => r * 0.9f, k => Fade(k, 0.1f, 0.3f) * (0.4f + 0.6f * Mathf.Min(k * (land + After) / land, 1f)), count: 6f, spin: 1f);
        Func<float, Vector3> at = k =>
        {
            var t = k * land;
            return t < Hang ? high + Vector3.Up * 0.05f * Mathf.Sin(t * 12f) : high.Lerp(spot + Vector3.Up * 0.5f, Mathf.Pow((t - Hang) / Fall, 2f));
        };
        // One great drop swells, lets go, and falls.
        Glyph(c, 0f, land, Leaflet, Sea, at, k => 1.6f * Out(Mathf.Min(k * land / Hang, 1f)), _ => 1f, gain: 1.9f, slim: 0.7f);
        Glyph(c, 0f, land, Leaflet, Pearl, k => at(k) + new Vector3(-0.1f, -0.15f, 0.05f), k => 0.7f * Out(Mathf.Min(k * land / Hang, 1f)), _ => 0.9f, gain: 1.6f, slim: 0.7f);
        Line(c, Hang, Fall, Foam, k => at((Hang + k * Fall) / land) + Vector3.Up * 1.2f, _ => Vector3.Up * 2f, 0.1f, k => k * 0.8f);
        Hit(c, land, Foam, spot + Vector3.Up * 0.2f, r * 0.7f);
        Cue(c, land, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.3f), r * 0.4f, WaterColor, FxBurst.Dome));
        // The splash: a crown of drops, a bead thrown back up, and rings going out and out.
        for (var i = 0; i < 16; i++)
        {
            var way = Around(i * Mathf.Tau / 16f);
            Func<float, Vector3> drop = k => spot + way * r * 0.5f * k + Vector3.Up * (0.2f + 1.9f * Mathf.Sin(k * Mathf.Pi));
            Glyph(c, land, 0.6f, Leaflet, i % 2 == 0 ? Foam : Sea, drop, _ => 0.45f, k => 1f - k * k, k => Heading(drop, k, 1f), gain: 1.8f, slim: 0.6f);
        }
        Func<float, Vector3> bead = k => spot + Vector3.Up * (0.3f + 2.6f * Mathf.Sin(k * Mathf.Pi));
        Globe(c, "energy", Orb, land + 0.05f, 0.7f, Foam, bead, _ => 0.16f, k => 1f - k * k, gain: 1.7f);
        for (var i = 0; i < 5; i++)
            Mark(c, land + i * 0.2f, 1f, Ripples, i % 2 == 0 ? Foam : Sea, spot, k => r * (0.2f + 1f * k), k => (1f - k) * 0.9f, count: 2f, spin: 0f, gain: 1.4f);
        Twinkle(c, 16, land, land + 0.5f, Pearl, () => Within(spot, r * 0.5f, 0.3f, 2.2f), 0.5f);
        Shine(c, spot + Vector3.Up, r * 1.4f, Sea.Main, age => age < land ? 0.4f : 2f * Mathf.Max(0f, 1f - (age - land) / 0.8f));
    }

    private void WaterMirror(Cast c, float r)
    {
        const float Life = 3.4f;
        Mark(c, 0f, Life, Window, Sea, Vector3.Zero, _ => r * 0.95f, k => Fade(k, 0.05f, 0.2f), count: 12f, spin: 0.6f, gain: 1.4f);
        Disc(c, 0f, Life, Glow, Pearl, Vector3.Zero, _ => r * 0.85f, k => Fade(k, 0.15f, 0.2f) * 0.35f);
        Mark(c, 0f, Life, Caustics, Foam, Vector3.Zero, _ => r * 0.85f, k => Fade(k, 0.15f, 0.2f) * 0.35f, count: 5f, spin: 0.5f);
        // A moon and stars hang over it, and their doubles lie in it.
        var moon = new Vector3(-0.6f, 2.8f, 0f);
        Glyph(c, 0.2f, Life - 0.2f, Sickle, Moon, k => moon + Vector3.Up * 0.06f * Mathf.Sin(k * 8f), k => 1.3f * Overshoot(Mathf.Min(k * 5f, 1f)), k => Fade(k, 0.05f, 0.15f), gain: 1.9f);
        Mark(c, 0.2f, Life - 0.2f, Corona, Moon, k => Lay(new Vector3(moon.X, 0.05f, 0.9f + 0.05f * Mathf.Sin(k * 12f)), 0.7f), k => Fade(k, 0.1f, 0.15f) * (0.7f + 0.2f * Mathf.Sin(k * 30f)), spin: 2f, count: 8f, gain: 1.5f);
        for (var i = 0; i < 9; i++)
        {
            var star = new Vector3(Rand(-r, r) * 0.7f, Rand(1.6f, 3.2f), Rand(-0.4f, 0.4f));
            var start = 0.3f + i * 0.12f;
            var twinkle = Rand(0f, Mathf.Tau);
            Glyph(c, start, Life - start, Glint, Pearl, _ => star, k => 0.6f + 0.2f * Mathf.Sin(k * 20f + twinkle), k => Fade(k, 0.05f, 0.15f), gain: 1.7f);
            var below = new Vector3(star.X, 0.06f, star.Y * 0.35f - 0.2f);
            Disc(c, start, Life - start, Glow, Pearl, k => Lay(below, 0.22f + 0.05f * Mathf.Sin(k * 25f + twinkle)), k => Fade(k, 0.05f, 0.15f) * 0.9f, gain: 1.5f);
        }
        for (var i = 0; i < 7; i++)
            Mark(c, Rand(0.4f, Life - 1f), 1f, Ripples, Foam, Within(Vector3.Zero, r * 0.7f), k => 0.3f + 1.1f * k, k => (1f - k) * 0.6f, count: 2f, spin: 0f);
        Shine(c, Vector3.Up, r * 1.2f, Moon.Main, age => 0.6f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void Riptide(Cast c, float r)
    {
        const float Life = 1.6f;
        var reach = r * 1.8f;
        var half = r * 0.7f;
        Disc(c, 0f, Life + 0.4f, Glow, Sea, new Vector3(0f, 0f, reach * 0.5f), _ => 1f, k => Fade(k, 0.1f, 0.4f) * 0.6f, stretch: new Vector3(half, 1f, reach * 0.5f));
        // Currents running back toward the caster, dragging everything with them.
        for (var i = 0; i < 7; i++)
        {
            var x = (i - 3) * half / 3.2f;
            var wave = Rand(0f, Mathf.Tau);
            Func<float, Vector3> path = s => new Vector3(x * (0.4f + 0.6f * s) + 0.3f * Mathf.Sin(s * 9f + wave), 0.1f, 0.6f + (reach - 0.6f) * s);
            for (var t = 0f; t < Life - 0.55f; t += 0.3f)
                Ribbon(c, t + i * 0.04f, 0.6f, 0, i % 2 == 0 ? Sea : Foam, s => path(1f - s), k => (Mathf.Max(0f, k * 1.4f - 0.45f), Mathf.Min(k * 1.4f, 1f)), 0.55f, k => Fade(k, 0.1f, 0.3f), gain: 1.5f, head: 0.4f);
        }
        for (var i = 0; i < 40; i++)
        {
            var x = Rand(-half, half);
            var z0 = Rand(reach * 0.4f, reach);
            Line(c, Rand(0f, Life - 0.4f), 0.4f, i % 2 == 0 ? Foam : Sea, k => new Vector3(x * (1f - 0.3f * k), 0.15f, z0 - 2.4f * k), _ => Vector3.Forward * 0.9f, 0.06f, k => Fade(k, 0.2f, 0.3f));
        }
        for (var i = 0; i < 4; i++)
            Mark(c, i * 0.3f, 0.8f, Ripples, Foam, k => Lay(new Vector3(0f, 0f, reach * (0.95f - 0.75f * k)), half * 1.1f), k => Fade(k, 0.2f, 0.4f) * 0.8f, count: 2f, spin: 0f, gain: 1.2f);
        // What it drags in piles up at the caster's feet.
        var mouth = new Vector3(0f, 0f, 0.9f);
        Disc(c, 0.5f, Life - 0.5f, Swirl, Sea, mouth, _ => 1.4f, k => Fade(k, 0.2f, 0.3f), spin: 9f, gain: 1.3f);
        for (var i = 0; i < 30; i++)
            Spark(c, Rand(0.5f, Life - 0.2f), Rand(0.4f, 0.6f), Foam, mouth + Vector3.Up * 0.2f, Around(Rand(0f, Mathf.Tau)) * Rand(0.5f, 2f) + Vector3.Up * Rand(2f, 5f), Gravity, 0.07f, 0.04f);
        Disc(c, Life - 0.2f, 0.5f, Shock, Foam, mouth, k => r * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Shine(c, new Vector3(0f, 1f, reach * 0.5f), reach, Sea.Main, age => 1f * Fade(age / Life, 0.1f, 0.2f));
    }
}
