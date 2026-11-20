using Godot;

namespace Arc.Fx;

/// <summary>
/// The fourth set of skills: shapes the first 132 left out. A falling moon cut, a rain of
/// swords, a meteor, a snaking flame, an ice comet, a tesla coil, a set of water skills (a wave,
/// a geyser, a whirlpool and a bubble), petals, roots, mirror images, a hexagram seal, tendrils
/// and a soul harvest.
/// </summary>
internal sealed partial class FxSkills
{
    private static readonly Tone Moon = new(new(0.72f, 0.76f, 1f), new(1f, 1f, 1f), new(0.16f, 0.16f, 0.45f));
    private static readonly Tone Sea = new(new(0.12f, 0.55f, 0.95f), new(0.8f, 0.95f, 1f), new(0.01f, 0.1f, 0.3f));
    private static readonly Tone Foam = new(new(0.7f, 0.92f, 1f), new(1f, 1f, 1f), new(0.15f, 0.4f, 0.6f));
    private static readonly Tone Petal = new(new(1f, 0.55f, 0.75f), new(1f, 0.95f, 0.97f), new(0.45f, 0.1f, 0.3f));
    private static readonly Tone Bark = new(new(0.35f, 0.7f, 0.15f), new(0.8f, 1f, 0.5f), new(0.12f, 0.08f, 0.02f));

    private static Color WaterColor => Sea.Main;

    private ShaderMaterial? _ghostBlade;

    private ShaderMaterial GhostBlade
    {
        get
        {
            if (_ghostBlade != null)
                return _ghostBlade;
            _ghostBlade = FxKit.Spectral(Moon.Main);
            _ghostBlade.SetShaderParameter("uGain", 1.4f);
            return _ghostBlade;
        }
    }

    private void Extra(FxSkill skill, Cast c, float r, float range)
    {
        switch (skill)
        {
            case FxSkill.MoonfallCut: MoonfallCut(c, r); break;
            case FxSkill.BladeRain: BladeRain(c, r); break;
            case FxSkill.HaloCut: HaloCut(c, r); break;
            case FxSkill.SandVortex: SandVortex(c, r); break;
            case FxSkill.Monolith: Monolith(c, r); break;
            case FxSkill.StoneshotFan: StoneshotFan(c, r); break;
            case FxSkill.SkyfireMeteor: SkyfireMeteor(c, r); break;
            case FxSkill.FlameSerpent: FlameSerpent(c, range); break;
            case FxSkill.EmberVolley: EmberVolley(c, range); break;
            case FxSkill.NapalmFlask: NapalmFlask(c, range); break;
            case FxSkill.IcicleBarrage: IcicleBarrage(c, range); break;
            case FxSkill.GlacialComet: GlacialComet(c, r); break;
            case FxSkill.PermafrostSpiral: PermafrostSpiral(c, r); break;
            case FxSkill.StormLance: StormLance(c, range); break;
            case FxSkill.TeslaCoil: TeslaCoil(c, r); break;
            case FxSkill.EyeOfTheStorm: EyeOfTheStorm(c, r); break;
            case FxSkill.TidalWave: TidalWave(c, r, range); break;
            case FxSkill.Geyser: Geyser(c, r); break;
            case FxSkill.Whirlpool: Whirlpool(c, r); break;
            case FxSkill.BubblePrison: BubblePrison(c); break;
            case FxSkill.PetalStorm: PetalStorm(c, r); break;
            case FxSkill.CreepingRoots: CreepingRoots(c, r); break;
            case FxSkill.ThornVolley: ThornVolley(c, r); break;
            case FxSkill.MirrorImages: MirrorImages(c); break;
            case FxSkill.AstralSigils: AstralSigils(c, r); break;
            case FxSkill.RiftTear: RiftTear(c, r); break;
            case FxSkill.Benediction: Benediction(c, r); break;
            case FxSkill.SealOfLight: SealOfLight(c, r); break;
            case FxSkill.ShadowTendrils: ShadowTendrils(c, r); break;
            default: SoulHarvest(c, r); break;
        }
    }

    /// <summary>A wall (fx_wall) whose ends move over its window: a wave rolling on.</summary>
    private void Wall(Cast c, float start, float life, int style, Tone tone, Func<float, (Vector3 A, Vector3 B)> ends, Func<float, float> height, Func<float, float> alpha,
        Func<float, float>? crumble = null, float speed = 1f, float gain = 1f)
    {
        var material = Material("wall", style, tone, gain, ("uSpeed", speed));
        Add(c, Kit.Quad, material, start, life, (n, k, _) =>
        {
            var (a, b) = ends(k);
            var along = FxKit.Flat(b - a);
            var length = Mathf.Max(along.Length(), 0.05f);
            var x = along / length;
            var h = Mathf.Max(height(k), 1e-3f);
            n.Transform = new Transform3D(new Basis(x * length, Vector3.Up * h, x.Cross(Vector3.Up)), FxKit.Flat((a + b) * 0.5f) + Vector3.Up * h * 0.5f);
            Look(n, alpha(k), crumble?.Invoke(k) ?? 0f);
            n.SetInstanceShaderParameter("uAspect", length / h);
        });
    }

    /// <summary>The way a point on <paramref name="at"/> is heading at <paramref name="k"/>, <paramref name="length"/> long.</summary>
    private static Vector3 Heading(Func<float, Vector3> at, float k, float length)
    {
        var a = at(Mathf.Max(k - 0.02f, 0f));
        var b = at(Mathf.Min(k + 0.02f, 1f));
        var way = b - a;
        return way.LengthSquared() < 1e-8f ? Vector3.Up * length : way.Normalized() * length;
    }

    // ---- Blades.

    private void MoonfallCut(Cast c, float r)
    {
        const float Swing = 0.22f, Land = 0.18f;
        var reach = r;
        // Upright along facing but leaning 40 degrees, so it reads from above too, sweeping from
        // overhead down into the ground in front.
        var plane = new Basis(Vector3.Back, Mathf.DegToRad(40f)) * new Basis(Vector3.Up, Vector3.Left, Vector3.Back);
        var hub = new Vector3(0f, 2f, 0.2f);
        Crescent(c, 0f, Swing + 0.12f, new Transform3D(plane, hub), reach, 150f, Moon, thick: 0.36f, sweep: 0.3f, gain: 1.6f);
        Crescent(c, 0.04f, Swing + 0.12f, new Transform3D(plane, hub + Vector3.Up * 0.2f), reach * 0.88f, 150f, Phantom, thick: 0.2f, sweep: 0.35f, gain: 1.1f);
        var strike = new Vector3(0f, 0f, reach * 0.9f);
        // The cut left in the ground along the swing.
        Line(c, Land, 1.3f, Moon, k => new Vector3(0f, 0.05f, reach * 0.65f), k => Vector3.Back * reach * 1.3f * Out(Mathf.Min(k * 8f, 1f)), 0.28f, k => Fade(k, 0f, 0.6f));
        Disc(c, Land, 1.4f, Cracks, Moon, strike, k => r * 0.55f, k => Fade(k, 0.02f, 0.5f), k => k, stretch: new Vector3(0.6f, 1f, 1.4f));
        Disc(c, Land, 0.4f, Shock, Moon, strike, k => r * 1.2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.4f);
        Flare(c, Land, 0.25f, Star, Moon, strike + Vector3.Up * 0.4f, 3.4f);
        Cue(c, Land, () => Stage.Explosion(At(c, strike + Vector3.Up * 0.2f), r * 0.4f, Moon.Main, FxBurst.Dome));
        for (var i = 0; i < 24; i++)
        {
            var way = Around(Rand(-1.3f, 1.3f));
            Spark(c, Land, Rand(0.4f, 0.7f), i % 2 == 0 ? Moon : Rubble, strike + Vector3.Up * 0.2f, way * Rand(2f, 5f) + Vector3.Up * Rand(4f, 8f), Gravity, 0.08f, 0.03f);
        }
        Shine(c, strike + Vector3.Up, r * 1.6f, Moon.Main, age => age < Land ? 0.6f : 2f * Mathf.Max(0f, 1f - (age - Land) / 0.5f));
    }

    private void BladeRain(Cast c, float r)
    {
        const int Blades = 9;
        const float Fall = 0.25f, Stand = 0.45f, Gone = 0.4f, Every = 0.07f, Size = 1.6f;
        var spots = Spread(Blades, r * 0.85f);
        Disc(c, 0f, Blades * Every + Fall + 0.4f, Runes, Moon, Vector3.Zero, k => r, k => Fade(k, 0.05f, 0.2f), k => Mathf.Min(k * 3f, 1f), gain: 1.2f);
        for (var i = 0; i < Blades; i++)
        {
            var spot = spots[i];
            var start = i * Every;
            var turn = new Basis(Vector3.Up, Rand(0f, Mathf.Tau)) * new Basis(Vector3.Right, Rand(-0.15f, 0.15f));
            var high = spot + Vector3.Up * 9f;
            var buried = spot + Vector3.Down * 0.3f;
            Add(c, SwordMesh, GhostBlade, start, Fall + Stand + Gone, (n, k, t) =>
            {
                var fall = Mathf.Min(t / Fall, 1f);
                n.Transform = new Transform3D(turn * Basis.FromScale(Vector3.One * Size), high.Lerp(buried, fall * fall));
                n.SetInstanceShaderParameter("uAlpha", Mathf.Min(1f, t / 0.06f));
                n.SetInstanceShaderParameter("uImpact", t < Fall ? 0f : Mathf.Max(0f, 1f - (t - Fall) / 0.2f));
                n.SetInstanceShaderParameter("uDissolve", Mathf.Clamp((t - Fall - Stand) / Gone, 0f, 1f));
            });
            Line(c, start, Fall, Moon, k => high.Lerp(buried, k * k) + Vector3.Up * 1.6f, k => Vector3.Up * 2.4f, 0.12f, k => 0.7f);
            Disc(c, start + Fall, 0.3f, Shock, Moon, spot, k => 1.2f * Out(k), k => 1f - k, band: 0.18f, gain: 1.3f);
            Disc(c, start + Fall, Stand + Gone, Cracks, Moon, spot, k => 0.6f, k => Fade(k, 0.02f, 0.4f), k => k);
            Flare(c, start + Fall, 0.2f, Star, Moon, spot + Vector3.Up * 0.3f, 1.8f);
            for (var j = 0; j < 6; j++)
            {
                var way = Around(Rand(0f, Mathf.Tau));
                Spark(c, start + Fall, Rand(0.3f, 0.5f), Moon, spot + Vector3.Up * 0.1f, way * Rand(2f, 4f) + Vector3.Up * Rand(3f, 6f), Gravity, 0.06f, 0.03f);
            }
        }
        Shine(c, Vector3.Up, r * 1.6f, Moon.Main, age => 1.2f * Fade(age / (Blades * Every + Fall + 0.5f), 0.1f, 0.3f));
    }

    private void HaloCut(Cast c, float r)
    {
        const float Life = 0.32f;
        for (var i = 0; i < 2; i++)
        {
            var start = i * 0.12f;
            var twist = i * 1.5f;
            var high = 0.9f + i * 0.35f;
            var tilt = new Basis(Vector3.Right, Mathf.DegToRad(i == 0 ? 4f : -4f));
            Crescent(c, start, Life, k => new Transform3D(tilt * new Basis(Vector3.Up, k * 2.5f + twist) * Basis.FromScale(Vector3.One * (0.35f + 0.65f * Out(k))), Vector3.Up * high),
                r, 360f, i == 0 ? Steel : Gold, thick: 0.2f, sweep: 0.55f, gain: 1.5f);
        }
        Disc(c, 0f, 0.45f, Shock, Steel, Vector3.Zero, k => r * 1.1f * Out(k), k => 1f - k, band: 0.12f, gain: 1.3f);
        Disc(c, 0f, 0.6f, Swirl, Dust, Vector3.Zero, k => r * 0.9f, k => (1f - k) * 0.6f, spin: 10f);
        for (var i = 0; i < 30; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var way = Around(angle);
            var spin = new Vector3(way.Z, 0f, -way.X);
            Spark(c, Rand(0f, 0.2f), 0.4f, Steel, way * r * 0.8f + Vector3.Up * Rand(0.8f, 1.3f), spin * 9f + way * 3f, Gravity * 0.3f, 0.06f, 0.04f);
        }
    }

    // ---- Earth.

    private void SandVortex(Cast c, float r)
    {
        const float Life = 2.8f;
        Disc(c, 0f, Life, Swirl, Dust, Vector3.Zero, k => r, k => Fade(k, 0.08f, 0.15f) * 0.9f, spin: 5f, gain: 1.2f);
        Disc(c, 0f, Life, Swirl, Rubble, Vector3.Zero, k => r * 0.5f, k => Fade(k, 0.08f, 0.15f), spin: 9f, gain: 1.1f);
        Disc(c, 0f, Life, Cracks, Rubble, Vector3.Zero, k => r * 0.6f, k => Fade(k, 0.05f, 0.2f) * 0.8f, k => Mathf.Min(k * 4f, 1f));
        Column(c, 0f, Life, Spiral, Dust, Vector3.Zero, k => new Vector2(0.55f, 1.4f * Out(Mathf.Min(k * Life / 0.3f, 1f))), k => Fade(k, 0.05f, 0.15f) * 0.7f, twist: 3f, speed: -3f, gain: 1.2f);
        for (var i = 0; i < 64; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var high = Rand(0.1f, 0.6f);
            var turn = Rand(5f, 7f);
            var tone = i % 3 == 0 ? Rubble : Dust;
            Func<float, Vector3> at = k => Around(angle + k * turn) * r * (1f - k * 0.88f) + Vector3.Up * (0.1f + high * (1f - k));
            Line(c, Rand(0f, Life - 0.8f), 0.8f, tone, at, k => Heading(at, k, 0.6f), 0.08f, k => Fade(k, 0.2f, 0.2f));
        }
        for (var i = 0; i < 10; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            Func<float, Vector3> at = k => Around(angle + k * 4f) * r * (1f - k * 0.8f) + Vector3.Up * 0.3f;
            Flare(c, Rand(0f, Life - 1f), 1f, Mote, Smoke, at, k => 0.8f + 0.6f * k, k => Fade(k, 0.2f, 0.3f) * 0.5f, gain: 0.8f);
        }
    }

    private void Monolith(Cast c, float r)
    {
        const float Rise = 0.4f, Stand = 2.4f;
        Disc(c, 0f, Rise + 0.02f, Telegraph, Dust, Vector3.Zero, k => r * 0.45f, k => 0.8f, k => k);
        var size = new Vector3(1.5f, 3.4f, 1.5f) * r / 3f;
        var turn = new Basis(Vector3.Up, Rand(0f, Mathf.Tau));
        Prop(c, Rise, Stand, Crags[0], _stone, k =>
        {
            var t = k * Stand;
            var rise = Out(Mathf.Min(t / 0.12f, 1f));
            var sink = Mathf.Max(0f, (t - (Stand - 0.5f)) / 0.5f);
            return new Transform3D(turn * Basis.FromScale(size), Vector3.Up * size.Y * (rise - 1f - sink * 0.95f));
        });
        Disc(c, Rise, 0.5f, Shock, Dust, Vector3.Zero, k => r * 1.6f * Out(k), k => 1f - k, band: 0.2f, gain: 1.3f);
        Disc(c, Rise, Stand, Cracks, Rubble, Vector3.Zero, k => r * 0.8f, k => Fade(k, 0.02f, 0.2f), k => Mathf.Min(k * 5f, 1f));
        for (var i = 0; i < 6; i++)
        {
            var way = Around(i * Mathf.Tau / 6f + Rand(-0.2f, 0.2f));
            Cue(c, Rise, () => Stage.Spike(At(c, way * r * 0.5f), c.Root.Basis * way, Rand(0.8f, 1.2f), FxSpike.Stone, 0.05f));
        }
        for (var i = 0; i < 30; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Rise, Rand(0.5f, 0.9f), Rubble, way * 0.6f + Vector3.Up * 0.3f, way * Rand(2f, 6f) + Vector3.Up * Rand(5f, 10f), Gravity * 1.2f, 0.13f, 0.02f);
        }
        for (var i = 0; i < 14; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            var far = Rand(0.8f, r);
            Flare(c, Rise, 1.1f, Mote, Smoke, k => way * far * Out(k) + Vector3.Up * (0.3f + 0.5f * k), k => 0.8f + 1.2f * k, k => (1f - k) * 0.55f, gain: 0.8f);
        }
    }

    private void StoneshotFan(Cast c, float r)
    {
        const float Flight = 0.4f, Spread1 = 0.5f;
        var reach = r * 1.3f;
        Disc(c, 0f, 1.2f, Cone, Dust, k => Lay(Vector3.Zero, reach), k => Fade(k, 0.02f, 0.5f) * 0.8f, k => Out(Mathf.Min(k * 1.2f / Flight, 1f)), cone: Spread1, gain: 1.1f);
        var mouth = new Vector3(0f, 1f, 0.5f);
        Flare(c, 0f, 0.2f, Star, Dust, mouth, 2.2f);
        for (var i = 0; i < 16; i++)
        {
            var land = Around(Rand(-Spread1, Spread1) * 0.9f) * reach * Rand(0.5f, 1f);
            var start = Rand(0f, 0.06f);
            var v = (land - mouth) / Flight + Vector3.Up * 0.5f * Gravity * Flight;
            var size = Rand(0.14f, 0.28f);
            var axis = FxKit.RandomDirection();
            Func<float, Vector3> at = k => mouth + v * k * Flight + Vector3.Down * 0.5f * Gravity * k * k * Flight * Flight;
            Prop(c, start, Flight, _rock, _stone, k => new Transform3D(new Basis(axis, k * 5f) * Basis.FromScale(Vector3.One * size), at(k)));
            Line(c, start, Flight, Dust, k => at(Mathf.Max(k - 0.08f, 0f)), k => Heading(at, k, 0.7f), 0.1f, k => 0.6f);
            Disc(c, start + Flight, 0.3f, Shock, Dust, land, k => 0.7f * Out(k), k => 1f - k, band: 0.2f);
            Flare(c, start + Flight, 0.6f, Mote, Smoke, k => land + Vector3.Up * (0.2f + 0.3f * k), k => 0.4f + 0.5f * k, k => (1f - k) * 0.5f, gain: 0.8f);
            for (var j = 0; j < 3; j++)
                Spark(c, start + Flight, 0.4f, Rubble, land + Vector3.Up * 0.1f, FxKit.Jitter(2f) + Vector3.Up * 3f, Gravity, 0.08f, 0.02f);
        }
    }

    // ---- Fire.

    private void SkyfireMeteor(Cast c, float r)
    {
        const float Fall = 1f;
        var land = Vector3.Up * 0.4f;
        var sky = new Vector3(-5f, 16f, -7f);
        var back = (sky - land).Normalized();
        Func<float, Vector3> at = k => sky.Lerp(land, k * k);
        Disc(c, 0f, Fall + 0.02f, Telegraph, Flame, Vector3.Zero, k => r * 0.7f, k => 0.8f, k => k);
        Disc(c, 0f, Fall, Glow, Flame, Vector3.Zero, k => r * 0.7f, k => k * 0.5f);
        var axis = new Vector3(1f, 0.3f, 0.2f).Normalized();
        Prop(c, 0f, Fall, _rock, _stone, k => new Transform3D(new Basis(axis, k * 8f) * Basis.FromScale(Vector3.One * 0.9f), at(k)));
        Globe(c, "energy", Shell, 0f, Fall, Magma, at, k => 1.25f, k => 1f, gain: 1.5f);
        Line(c, 0f, Fall, Flame, k => at(k) + back * 2.4f, k => back * 4.8f, 1f, k => 1f);
        Line(c, 0f, Fall, Flame with { Main = Flame.Hot }, k => at(k) + back * 1.6f, k => back * 3f, 0.35f, k => 1f);
        for (var i = 0; i < 20; i++)
        {
            var s = (i + 0.5f) / 20f;
            var puff = at(s * 0.95f);
            Flare(c, s * Fall, 0.9f, Mote, Smoke, k => puff + Vector3.Up * k * 0.5f, k => 0.8f + 1.2f * k, k => (1f - k) * 0.45f, gain: 0.8f);
        }
        for (var i = 0; i < 24; i++)
        {
            var t = Rand(0.2f, Fall);
            Spark(c, t, Rand(0.3f, 0.5f), Flame, at(t), FxKit.Jitter(2f) + back * 3f, Gravity * 0.4f, 0.07f, 0.04f);
        }
        Cue(c, Fall, () => Stage.Explosion(At(c, land), r, FireColor, FxBurst.Fire));
        Cue(c, Fall, () => Stage.Patch(At(c, Vector3.Zero), r * 0.7f, 3f, FireColor, FxGround.Inferno));
        Disc(c, Fall, 0.6f, Shock, Flame, Vector3.Zero, k => r * 1.8f * Out(k), k => 1f - k, band: 0.16f, gain: 1.5f);
        Disc(c, Fall, 2f, Cracks, Magma, Vector3.Zero, k => r * 0.8f, k => Fade(k, 0.02f, 0.4f), k => k);
        Flare(c, Fall, 0.35f, Star, Flame, Vector3.Up * 0.8f, 5f);
        for (var i = 0; i < 8; i++)
        {
            var foot = Around(i * Mathf.Tau / 8f + Rand(-0.2f, 0.2f)) * r * 0.6f;
            var tall = Rand(1.2f, 2f);
            Column(c, Fall + Rand(0f, 0.1f), 0.8f, Fire, Flame, foot, k => new Vector2(0.35f, tall * Out(Mathf.Min(k * 5f, 1f))), k => 1f - k, gain: 1.4f, speed: 2f);
        }
        for (var i = 0; i < 30; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Fall, Rand(0.5f, 0.9f), i % 2 == 0 ? Rubble : Flame, way * 0.5f + Vector3.Up * 0.3f, way * Rand(3f, 7f) + Vector3.Up * Rand(5f, 10f), Gravity * 1.2f, 0.12f, 0.02f);
        }
        Shine(c, Vector3.Up, r * 2f, Flame.Main, age => age < Fall ? 0.3f + 0.5f * age / Fall : 2.6f * Mathf.Max(0f, 1f - (age - Fall) / 0.8f));
    }

    private void FlameSerpent(Cast c, float range)
    {
        const float Slither = 0.9f, Trail = 0.4f;
        var length = Mathf.Max(range, 2f);
        Func<float, Vector3> path = s => new Vector3(0.9f * Mathf.Sin(s * Mathf.Pi * 3f) * (1f - 0.4f * s), 0.25f + 0.15f * Mathf.Sin(s * Mathf.Pi * 6f), 0.6f + (length - 0.6f) * s);
        Func<float, (float, float)> span = k =>
        {
            var head = Mathf.Min(k * (Slither + Trail) / Slither, 1f);
            return (Mathf.Max(0f, head - Trail), head);
        };
        Ribbon(c, 0f, Slither + Trail, 1, Flame, path, span, 1f, k => 1f, gain: 1.6f);
        Ribbon(c, 0f, Slither + Trail, 0, Ember, path, span, 0.4f, k => 1f, gain: 1.4f);
        Globe(c, "energy", Orb, 0f, Slither, Flame, path, k => 0.38f, k => 1f, gain: 1.5f);
        Flare(c, 0f, Slither, Star, Flame, path, k => 1.3f, k => 1f);
        for (var i = 0; i < 5; i++)
        {
            var s = (i + 0.5f) / 5f;
            var spot = FxKit.Flat(path(s));
            Cue(c, s * Slither, () => Stage.Patch(At(c, spot), 0.7f, 1.6f, FireColor, FxGround.Fire));
        }
        for (var i = 0; i < 24; i++)
        {
            var s = Rand(0f, 1f);
            Spark(c, s * Slither, Rand(0.4f, 0.6f), Flame, path(s), FxKit.Jitter(1.5f) + Vector3.Up * 2.5f, Gravity * 0.5f, 0.06f, 0.04f);
        }
        // At the end it rears up in a burst of flame.
        var end = FxKit.Flat(path(1f));
        Column(c, Slither, 0.7f, Fire, Flame, end, k => new Vector2(0.6f, 3.2f * Out(Mathf.Min(k * 5f, 1f))), k => 1f - k * k, gain: 1.6f, speed: 2.5f);
        Cue(c, Slither, () => Stage.Explosion(At(c, end + Vector3.Up * 0.4f), 1.3f, FireColor, FxBurst.Fire));
        Disc(c, Slither, 0.4f, Shock, Flame, end, k => 2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.4f);
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Flame.Main, age => 1.3f * Fade(age / (Slither + 0.6f), 0.05f, 0.3f));
    }

    private void EmberVolley(Cast c, float range)
    {
        const float Flight = 0.45f, Every = 0.08f;
        var length = Mathf.Max(range, 2f);
        var from = new Vector3(0f, Fx.ShotHeight, 0.5f);
        float[] fans = [-0.26f, 0f, 0.26f];
        Flare(c, 0f, 0.25f, Star, Flame, from, 2.4f);
        Disc(c, 0f, 0.35f, Shock, Flame, Vector3.Zero, k => 1.4f * Out(k), k => 1f - k, band: 0.16f);
        for (var i = 0; i < fans.Length; i++)
        {
            var to = new Vector3(0f, Fx.ShotHeight, 0f) + Around(fans[i]) * length;
            var start = i * Every;
            Fly(c, start, Flight, FxShotLook.Fireball, k => from.Lerp(to, k), 1f);
            var ground = FxKit.Flat(to);
            Cue(c, start + Flight, () => Stage.Explosion(At(c, ground + Vector3.Up * 0.3f), 1.1f, FireColor, FxBurst.Fire));
            Cue(c, start + Flight, () => Stage.Patch(At(c, ground), 0.8f, 1.6f, FireColor, FxGround.Fire));
            Disc(c, start + Flight, 0.4f, Shock, Flame, ground, k => 1.6f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        }
    }

    private void NapalmFlask(Cast c, float range)
    {
        const float Flight = 0.5f, Burn = 2.6f;
        var length = Mathf.Max(range * 0.7f, 2f);
        var from = new Vector3(0f, 1.4f, 0.4f);
        var spot = new Vector3(0f, 0.15f, length);
        var ground = FxKit.Flat(spot);
        Func<float, Vector3> at = k => from.Lerp(spot, k) + Vector3.Up * 2.2f * Mathf.Sin(k * Mathf.Pi);
        Globe(c, "energy", Orb, 0f, Flight, Ember, at, k => 0.22f, k => 1f, gain: 1.6f);
        Flare(c, 0f, Flight, Star, Flame, at, k => 0.9f, k => 1f);
        Line(c, 0f, Flight, Flame, k => at(Mathf.Max(k - 0.06f, 0f)), k => Heading(at, k, 0.9f), 0.14f, k => 1f);
        Cue(c, Flight, () => Stage.Explosion(At(c, spot + Vector3.Up * 0.2f), 1.2f, FireColor, FxBurst.Fire));
        Cue(c, Flight, () => Stage.Patch(At(c, ground), 1.8f, Burn, FireColor, FxGround.Inferno));
        Disc(c, Flight, Burn, Glow, Magma, ground, k => 1.8f * Out(Mathf.Min(k * Burn / 0.4f, 1f)), k => Fade(k, 0.02f, 0.3f), gain: 1.3f);
        Disc(c, Flight, 0.4f, Shock, Flame, ground, k => 2.4f * Out(k), k => 1f - k, band: 0.16f, gain: 1.4f);
        for (var i = 0; i < 14; i++)
        {
            var foot = ground + Around(Rand(0f, Mathf.Tau)) * 1.6f * Mathf.Sqrt(Rand(0f, 1f));
            var tall = Rand(0.8f, 1.5f);
            Column(c, Flight + Rand(0f, Burn - 0.8f), 0.7f, Fire, Flame, foot, k => new Vector2(0.25f, tall * Out(Mathf.Min(k * 5f, 1f))), k => Fade(k, 0.1f, 0.5f), gain: 1.4f, speed: 2.2f);
        }
        for (var i = 0; i < 20; i++)
        {
            var way = Around(Rand(0f, Mathf.Tau));
            Spark(c, Flight, Rand(0.4f, 0.7f), Flame, ground + Vector3.Up * 0.2f, way * Rand(2f, 5f) + Vector3.Up * Rand(3f, 6f), Gravity, 0.07f, 0.04f);
        }
        Shine(c, ground + Vector3.Up, 3.5f, Flame.Main, age => age < Flight ? 0.3f : 1.6f * Fade((age - Flight) / Burn, 0.05f, 0.3f));
    }

    // ---- Frost.

    private void IcicleBarrage(Cast c, float range)
    {
        const int Shards = 12;
        const float Flight = 0.3f, Every = 0.05f;
        var length = Mathf.Max(range, 2f);
        var from = new Vector3(0f, 1.3f, 0.5f);
        Flare(c, 0f, Shards * Every, Star, Ice, k => from, k => 1.4f, k => Fade(k, 0.05f, 0.3f));
        for (var i = 0; i < Shards; i++)
        {
            var start = i * Every;
            var to = new Vector3(Rand(-0.9f, 0.9f), Rand(0.6f, 1.4f), length + Rand(-0.4f, 0.3f));
            var o = from + new Vector3(Rand(-0.4f, 0.4f), Rand(-0.3f, 0.3f), 0f);
            var way = (to - o).Normalized();
            Prop(c, start, Flight, ShardMesh, IceBlock, k => new Transform3D(Along(way, 1f, 1f) * Basis.FromScale(new Vector3(0.16f, 0.75f, 0.16f)), o.Lerp(to, k)));
            Line(c, start, Flight, Ice, k => o.Lerp(to, Mathf.Max(k - 0.1f, 0f)), k => way * 1.1f, 0.08f, k => 0.8f);
            Flare(c, start + Flight, 0.2f, Star, Ice, to, 1.2f);
            for (var j = 0; j < 4; j++)
                Spark(c, start + Flight, 0.35f, Ice, to, FxKit.Jitter(3f) - way * 2f, Gravity, 0.06f, 0.03f);
        }
        var end = new Vector3(0f, 0f, length);
        Cue(c, Flight, () => Stage.Patch(At(c, end), 1.2f, 2f, FrostColor, FxGround.Frost));
        Cue(c, (Shards - 1) * Every + Flight, () => Stage.Explosion(At(c, end + Vector3.Up), 0.9f, FrostColor, FxBurst.Frost));
    }

    private void GlacialComet(Cast c, float r)
    {
        const float Fall = 0.8f, Long = 2.6f;
        var land = Vector3.Up * 0.2f;
        var sky = new Vector3(4f, 14f, -6f);
        var way = (land - sky).Normalized();
        Func<float, Vector3> at = k => sky.Lerp(land, k * k);
        Disc(c, 0f, Fall + 0.02f, Telegraph, Ice, Vector3.Zero, k => r * 0.6f, k => 0.8f, k => k);
        Prop(c, 0f, Fall, ShardMesh, IceBlock, k => new Transform3D(Along(way, 1f, 1f) * Basis.FromScale(new Vector3(0.9f, Long, 0.9f)), at(k) - way * Long));
        Globe(c, "energy", Shell, 0f, Fall, Ice, k => at(k) - way * Long * 0.5f, k => 1.3f, k => 0.6f, gain: 1.3f);
        Line(c, 0f, Fall, Ice, k => at(k) - way * 4f, k => way * 5f, 0.9f, k => 1f);
        for (var i = 0; i < 24; i++)
        {
            var s = (i + 0.5f) / 24f;
            var p = at(s * 0.95f) - way * Long * 0.5f;
            Flare(c, s * Fall, 0.6f, Mote, Ice, k => p + Vector3.Down * k * 0.6f, k => 0.25f, k => 1f - k, gain: 1.2f);
        }
        Cue(c, Fall, () => Stage.Explosion(At(c, land + Vector3.Up * 0.3f), r, FrostColor, FxBurst.Frost));
        Cue(c, Fall, () => Stage.Patch(At(c, Vector3.Zero), r, 3f, FrostColor, FxGround.Frost));
        for (var i = 0; i < 8; i++)
        {
            var out1 = Around(i * Mathf.Tau / 8f + Rand(-0.2f, 0.2f));
            Cue(c, Fall, () => Stage.Spike(At(c, out1 * r * 0.45f), c.Root.Basis * out1, Rand(1.1f, 1.6f), FxSpike.Ice, Rand(0f, 0.08f)));
        }
        Disc(c, Fall, 0.5f, Shock, Ice, Vector3.Zero, k => r * 1.6f * Out(k), k => 1f - k, band: 0.16f, gain: 1.5f);
        Flare(c, Fall, 0.35f, Star, Ice, Vector3.Up * 0.8f, 5f);
        for (var i = 0; i < 14; i++)
        {
            var v = Around(Rand(0f, Mathf.Tau)) * Rand(3f, 6f) + Vector3.Up * Rand(5f, 9f);
            var spin = FxKit.RandomDirection();
            Prop(c, Fall, 0.8f, ShardMesh, IceBlock, k =>
            {
                var t = k * 0.8f;
                return new Transform3D(new Basis(spin, t * 8f) * Basis.FromScale(new Vector3(0.12f, 0.35f, 0.12f) * (1f - k * 0.5f)), land + v * t + Vector3.Down * 0.5f * Gravity * t * t);
            });
        }
        Shine(c, Vector3.Up, r * 2f, Ice.Main, age => age < Fall ? 0.3f : 2.2f * Mathf.Max(0f, 1f - (age - Fall) / 0.6f));
    }

    private void PermafrostSpiral(Cast c, float r)
    {
        const int Arms = 3, Per = 7;
        Disc(c, 0f, 1.8f, Swirl, Ice, Vector3.Zero, k => r, k => Fade(k, 0.05f, 0.3f) * 0.8f, spin: 4f);
        Flare(c, 0f, 0.3f, Star, Ice, Vector3.Up * 0.4f, 2.6f);
        Cue(c, 0f, () => Stage.Patch(At(c, Vector3.Zero), r, 2.5f, FrostColor, FxGround.Frost));
        for (var a = 0; a < Arms; a++)
        {
            for (var j = 0; j < Per; j++)
            {
                var s = (j + 1f) / Per;
                var angle = a * Mathf.Tau / Arms + s * 2.2f;
                var spot = Around(angle) * r * s * 0.95f;
                var lean = Around(angle + 0.6f);
                var delay = s * 0.45f;
                Cue(c, 0f, () => Stage.Spike(At(c, spot), c.Root.Basis * lean, 0.5f + s * 1.1f, FxSpike.Ice, delay));
                Disc(c, delay, 0.3f, Shock, Ice, spot, k => 0.8f * Out(k), k => 1f - k, band: 0.2f);
                for (var p = 0; p < 3; p++)
                    Spark(c, delay, 0.4f, Ice, spot + Vector3.Up * 0.2f, FxKit.Jitter(1.5f) + Vector3.Up * 3f, Gravity, 0.06f, 0.03f);
            }
        }
    }

    // ---- Storm.

    private void StormLance(Cast c, float range)
    {
        const float Charge = 0.15f, Flight = 0.3f;
        var length = Mathf.Max(range, 2f);
        var hand = new Vector3(0f, Fx.ShotHeight, 0.5f);
        var foe = new Vector3(0f, Fx.ShotHeight, length);
        var hit = Charge + Flight;
        Flare(c, 0f, Charge + 0.05f, Star, Storm, k => hand, k => 0.5f + 1.8f * k, k => 1f);
        for (var i = 0; i < 4; i++)
            Bolt(c, Rand(0f, Charge * 0.5f), Charge * 0.6f, Storm, hand + FxKit.RandomDirection() * 0.7f, hand, 0.06f, 3, 0.15f);
        Fly(c, Charge, Flight, FxShotLook.LightningArrow, k => hand.Lerp(foe, k), 1.3f);
        Line(c, Charge, Flight, Storm, k => hand.Lerp(foe, Mathf.Max(k - 0.15f, 0f)), k => Vector3.Back * 2f, 0.12f, k => 1f);
        Cue(c, hit, () => Stage.Explosion(At(c, foe), 1.2f, FxTheme.Of(FxElement.Lightning), FxBurst.Lightning));
        Disc(c, hit, 0.35f, Shock, Storm, FxKit.Flat(foe), k => 1.8f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        // The bolt jumps on to three more.
        var prev = foe;
        for (var i = 0; i < 3; i++)
        {
            var next = FxKit.Flat(prev) + Around(Rand(0f, Mathf.Tau)) * Rand(1.8f, 2.6f) + Vector3.Up;
            var (a, b) = (prev, next);
            var when = hit + 0.08f * (i + 1);
            Cue(c, when, () => Stage.Lightning([(At(c, a) - Vector3.Up * Fx.LineLift, At(c, b) - Vector3.Up * Fx.LineLift)], 0.18f, 8, 0.6f));
            Flare(c, when, 0.2f, Star, Storm, b, 1.8f);
            Disc(c, when, 0.3f, Shock, Storm, FxKit.Flat(b), k => 1.1f * Out(k), k => 1f - k, band: 0.18f);
            prev = next;
        }
        Shine(c, foe, 4f, Storm.Main, age => 1.8f * Bump(age, hit, 0.4f));
    }

    private void TeslaCoil(Cast c, float r)
    {
        const float Life = 3f, First = 0.4f, Every = 0.32f;
        var orb = Vector3.Up * 2.4f;
        Column(c, 0f, Life, Beam, Cyan, Vector3.Zero, k => new Vector2(0.1f, 2.4f * Out(Mathf.Min(k * Life / 0.25f, 1f))), k => Fade(k, 0.03f, 0.1f), speed: 3f, gain: 1.4f);
        Globe(c, "energy", Orb, 0f, Life, Cyan, k => orb, k => 0.45f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f)) * (1f + 0.08f * Mathf.Sin(k * Life * 20f)), k => Fade(k, 0.03f, 0.08f), gain: 1.6f);
        Globe(c, "energy", Hexes, 0f, Life, Cyan, k => orb, k => 0.75f * Out(Mathf.Min(k * Life / 0.3f, 1f)), k => Fade(k, 0.03f, 0.08f) * 0.6f);
        Flare(c, 0f, Life, Star, Cyan, k => orb, k => 1.6f + 0.3f * Mathf.Sin(k * Life * 31f), k => Fade(k, 0.03f, 0.08f));
        Disc(c, 0f, Life, Runes, Cyan, Vector3.Zero, k => r * 0.9f, k => Fade(k, 0.03f, 0.1f), k => Mathf.Min(k * Life / 0.4f, 1f), gain: 1.2f);
        Disc(c, 0f, Life, Glow, Cyan, Vector3.Zero, k => r * 0.9f, k => Fade(k, 0.05f, 0.1f) * 0.3f);
        var zaps = (int)((Life - First - 0.2f) / Every);
        for (var i = 0; i < zaps; i++)
        {
            var t = First + i * Every;
            var spot = Around(Rand(0f, Mathf.Tau)) * Rand(r * 0.4f, r * 0.9f);
            Cue(c, t, () => Stage.Lightning([(At(c, orb) - Vector3.Up * Fx.LineLift, At(c, spot) - Vector3.Up * Fx.LineLift)], 0.14f, 8, 0.6f));
            Flare(c, t, 0.2f, Star, Cyan, spot + Vector3.Up * 0.2f, 1.6f);
            Disc(c, t, 0.3f, Shock, Cyan, spot, k => 1f * Out(k), k => 1f - k, band: 0.2f, gain: 1.3f);
            Disc(c, t, 0.8f, Cracks, Cyan, spot, k => 0.6f, k => Fade(k, 0.02f, 0.5f), k => k);
            for (var j = 0; j < 4; j++)
                Spark(c, t, 0.35f, Cyan, spot + Vector3.Up * 0.1f, FxKit.Jitter(2f) + Vector3.Up * 3f, Gravity, 0.05f, 0.03f);
        }
        Shine(c, orb, r * 1.5f, Cyan.Main, age => 1.2f * Fade(age / Life, 0.05f, 0.1f));
    }

    private void EyeOfTheStorm(Cast c, float r)
    {
        const float Life = 3.2f, Turn = 1.6f;
        const int Funnels = 4;
        var ring = r * 0.8f;
        Disc(c, 0f, Life, Swirl, Storm, Vector3.Zero, k => r, k => Fade(k, 0.08f, 0.15f) * 0.8f, spin: 5f);
        Disc(c, 0f, Life, Glow, Storm, Vector3.Zero, k => r * 0.4f, k => Fade(k, 0.08f, 0.15f) * 0.5f);
        for (var i = 0; i < Funnels; i++)
        {
            var phase = i * Mathf.Tau / Funnels;
            Fly(c, 0f, Life, FxShotLook.StormTornado, k => Around(phase + k * Life * Turn) * ring + Vector3.Up * Fx.ShotHeight, 0.7f,
                k => Mathf.SmoothStep(0f, 1f, Mathf.Clamp(Mathf.Min(k * Life / 0.3f, (1f - k) * Life / 0.4f), 0f, 1f)));
        }
        // Lightning leaps between neighbouring funnels.
        for (var t = 0.4f; t < Life - 0.4f; t += 0.35f)
        {
            var i = (int)(t / 0.35f) % Funnels;
            var when = t;
            Func<int, Vector3> place = j => Around(j * Mathf.Tau / Funnels + when * Turn) * ring + Vector3.Up * 1.2f;
            var (a, b) = (place(i), place((i + 1) % Funnels));
            Cue(c, when, () => Stage.Lightning([(At(c, a) - Vector3.Up * Fx.LineLift, At(c, b) - Vector3.Up * Fx.LineLift)], 0.14f, 8, 0.5f));
        }
        for (var i = 0; i < 30; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var far = Rand(0.5f, r);
            var high = Rand(0.2f, 2f);
            Func<float, Vector3> at = k => Around(angle + k * 5f) * far + Vector3.Up * (high + k * 0.6f);
            Line(c, Rand(0f, Life - 0.6f), 0.6f, i % 2 == 0 ? Storm : Dust, at, k => Heading(at, k, 0.6f), 0.06f, k => Fade(k, 0.2f, 0.3f));
        }
    }

    // ---- Water.

    private void TidalWave(Cast c, float r, float range)
    {
        const float Rise = 0.25f, Roll = 0.9f, Crash = 0.3f;
        // A wave rolls no further than 2.5 radii, so a bare direction doesn't send it off the screen.
        var length = Mathf.Clamp(range, 3f, r * 2.5f);
        var half = r * 0.75f;
        const float Life = Rise + Roll;
        Func<float, float> front = t => 0.6f + (length - 0.6f) * Mathf.SmoothStep(0f, 1f, Mathf.Clamp((t - Rise * 0.5f) / Roll, 0f, 1f));
        Func<float, float> tall = t => 2.4f * Out(Mathf.Min(t / Rise, 1f)) * (1f - 0.6f * Mathf.Max(0f, (t - (Life - Crash)) / Crash));
        Func<float, float> crash = t => Mathf.Max(0f, (t - (Life - Crash)) / Crash);
        Wall(c, 0f, Life, 1, Sea, k => (new Vector3(-half, 0f, front(k * Life)), new Vector3(half, 0f, front(k * Life))), k => tall(k * Life), k => Fade(k, 0.03f, 0.05f),
            k => crash(k * Life), speed: 2.5f, gain: 1.6f);
        Wall(c, 0f, Life, 0, Foam, k => (new Vector3(-half, 0f, front(k * Life) + 0.05f), new Vector3(half, 0f, front(k * Life) + 0.05f)), k => tall(k * Life) * 0.95f, k => Fade(k, 0.03f, 0.05f) * 0.45f,
            k => crash(k * Life), gain: 1.2f);
        Wall(c, 0f, Life, 1, Sea, k => (new Vector3(-half * 0.9f, 0f, front(k * Life) - 0.5f), new Vector3(half * 0.9f, 0f, front(k * Life) - 0.5f)), k => tall(k * Life) * 0.6f, k => Fade(k, 0.03f, 0.05f) * 0.7f,
            k => crash(k * Life), speed: 2f, gain: 1.2f);
        for (var i = 0; i < 44; i++)
        {
            var x = Rand(-half, half);
            var t0 = Rand(0.1f, Life - 0.2f);
            Spark(c, t0, Rand(0.4f, 0.6f), Foam, new Vector3(x, tall(t0), front(t0)), new Vector3(Rand(-1f, 1f), Rand(1f, 3f), Rand(2f, 4f)), Gravity * 0.8f, 0.08f, 0.04f);
        }
        Disc(c, Rise, Roll + 1f, Glow, Sea, new Vector3(0f, 0f, length * 0.5f), k => 1f, k => Fade(k, 0.1f, 0.6f) * 0.5f, stretch: new Vector3(half, 1f, length * 0.5f));
        var end = new Vector3(0f, 0f, length);
        Cue(c, Life - Crash, () => Stage.Explosion(At(c, end + Vector3.Up * 0.5f), r * 0.6f, WaterColor, FxBurst.Dome));
        Disc(c, Life - Crash, 0.5f, Shock, Foam, end, k => r * 1.2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        for (var i = 0; i < 30; i++)
        {
            var at = end + Vector3.Right * Rand(-half, half);
            Spark(c, Life - Crash, Rand(0.5f, 0.8f), Foam, at + Vector3.Up * 0.5f, new Vector3(Rand(-2f, 2f), Rand(4f, 8f), Rand(0f, 3f)), Gravity, 0.08f, 0.04f);
        }
        Shine(c, new Vector3(0f, 1f, length * 0.5f), length, Sea.Main, age => 1f * Fade(age / Life, 0.1f, 0.2f));
    }

    private void Geyser(Cast c, float r)
    {
        const float Bubble = 0.6f, Spout = 1.4f;
        var reach = r * 0.45f;
        Disc(c, 0f, Bubble + Spout, Glow, Sea, Vector3.Zero, k => reach * 1.3f, k => Fade(k, 0.1f, 0.3f) * 0.8f);
        Disc(c, 0f, Bubble, Telegraph, Sea, Vector3.Zero, k => reach, k => 0.8f, k => k);
        for (var i = 0; i < 18; i++)
        {
            var at = Around(Rand(0f, Mathf.Tau)) * reach * Mathf.Sqrt(Rand(0f, 1f));
            var size = Rand(0.2f, 0.35f);
            Flare(c, Rand(0f, Bubble - 0.2f), 0.3f, Mote, Foam, k => at + Vector3.Up * (0.1f + 0.4f * k), k => size * (1f + k), k => 1f - k, gain: 1.2f);
        }
        Column(c, Bubble, Spout, Spiral, Sea, Vector3.Zero, k => new Vector2(reach * (0.8f + 0.2f * k), 5.5f * Out(Mathf.Min(k * Spout / 0.15f, 1f)) * (1f - 0.7f * Mathf.Max(0f, (k - 0.7f) / 0.3f))),
            k => Fade(k, 0f, 0.3f), twist: 1.5f, speed: 4f, gain: 1.5f);
        Column(c, Bubble, Spout, Light, Foam, Vector3.Zero, k => new Vector2(reach * 0.55f, 6.2f * Out(Mathf.Min(k * Spout / 0.15f, 1f)) * (1f - 0.7f * Mathf.Max(0f, (k - 0.7f) / 0.3f))),
            k => Fade(k, 0f, 0.3f) * 0.7f, speed: 3f, gain: 1.6f);
        Cue(c, Bubble, () => Stage.Explosion(At(c, Vector3.Up * 0.4f), reach * 1.2f, WaterColor, FxBurst.Dome));
        Disc(c, Bubble, 0.45f, Shock, Foam, Vector3.Zero, k => r * 1.2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.4f);
        for (var i = 0; i < 3; i++)
            Disc(c, Bubble + 0.3f + i * 0.35f, 0.5f, Shock, Sea, Vector3.Zero, k => reach * 2.2f * Out(k), k => (1f - k) * 0.8f, band: 0.12f);
        for (var i = 0; i < 56; i++)
        {
            var t = Rand(Bubble, Bubble + Spout - 0.4f);
            var from = Vector3.Up * Rand(2.5f, 5.5f);
            var v = Around(Rand(0f, Mathf.Tau)) * Rand(1.5f, 4f) + Vector3.Up * Rand(1f, 4f);
            Spark(c, t, Rand(0.6f, 0.9f), i % 2 == 0 ? Foam : Sea, from, v, Gravity, 0.08f, 0.04f);
        }
        Shine(c, Vector3.Up * 2f, r * 1.4f, Sea.Main, age => age < Bubble ? 0.4f : 1.4f * Fade((age - Bubble) / Spout, 0f, 0.3f));
    }

    private void Whirlpool(Cast c, float r)
    {
        const float Life = 3f;
        Disc(c, 0f, Life, Swirl, Sea, Vector3.Zero, k => r, k => Fade(k, 0.08f, 0.15f) * 0.95f, spin: 5f, gain: 1.3f);
        Disc(c, 0f, Life, Swirl, Foam, Vector3.Zero, k => r * 0.45f, k => Fade(k, 0.08f, 0.15f) * 0.8f, spin: 9f, gain: 1.3f);
        Disc(c, 0f, Life, Glow, Sea, Vector3.Zero, k => r * 0.3f, k => Fade(k, 0.08f, 0.15f) * 0.7f);
        for (var i = 0; i < 6; i++)
            Disc(c, i * 0.42f, 0.9f, Shock, Foam, Vector3.Zero, k => r * (1f - Out(k) * 0.85f), k => Fade(k, 0.2f, 0.3f) * 0.8f, band: 0.1f);
        for (var i = 0; i < 56; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            var turn = Rand(5f, 7f);
            Func<float, Vector3> at = k => Around(angle + k * turn) * r * (1f - k * 0.9f) + Vector3.Up * (0.15f - 0.1f * k);
            Line(c, Rand(0f, Life - 0.9f), 0.9f, i % 3 == 0 ? Sea : Foam, at, k => Heading(at, k, 0.5f), 0.06f, k => Fade(k, 0.2f, 0.2f));
        }
        for (var i = 0; i < 12; i++)
        {
            var angle = Rand(0f, Mathf.Tau);
            Func<float, Vector3> at = k => Around(angle + k * 5f) * r * (0.9f - 0.8f * k) + Vector3.Up * (0.2f - 0.15f * k);
            Flare(c, Rand(0f, Life - 1f), 1f, Mote, Foam, at, k => 0.25f, k => Fade(k, 0.2f, 0.2f), gain: 1.2f);
        }
    }

    private void BubblePrison(Cast c)
    {
        const float Form = 0.15f, Pop = 2.2f;
        var centre = Vector3.Up * 1.1f;
        Func<float, Vector3> wobble = k => centre + Vector3.Up * 0.08f * Mathf.Sin(k * Pop * 5f);
        Globe(c, "energy", Shell, 0f, Pop, Foam, wobble, k => 1.3f * Overshoot(Mathf.Min(k * Pop / Form, 1f)) * (1f + 0.04f * Mathf.Sin(k * Pop * 9f)), k => 1f, gain: 1.4f);
        Globe(c, "energy", Orb, 0f, Pop, Sea, wobble, k => 1.22f * Out(Mathf.Min(k * Pop / Form, 1f)), k => 0.35f, gain: 0.8f);
        Flare(c, 0f, Pop, Mote, Foam, k => wobble(k) + new Vector3(-0.45f, 0.5f, 0.6f), k => 0.45f, k => Fade(k, 0.1f, 0.02f) * 0.8f, gain: 1.5f);
        Disc(c, 0f, Pop, Glow, Sea, Vector3.Zero, k => 1.5f, k => Fade(k, 0.05f, 0.05f) * 0.6f);
        Disc(c, 0f, 0.35f, Shock, Foam, Vector3.Zero, k => 1.8f * Out(k), k => 1f - k, band: 0.16f);
        for (var i = 0; i < 26; i++)
        {
            var at = centre + new Vector3(Rand(-0.7f, 0.7f), Rand(-0.9f, 0.2f), Rand(-0.7f, 0.7f));
            Flare(c, Rand(0f, Pop - 0.6f), 0.6f, Mote, Foam, k => at + Vector3.Up * k * 0.8f, k => 0.14f, k => Fade(k, 0.2f, 0.3f), gain: 1.3f);
        }
        Flare(c, Pop, 0.3f, Star, Foam, centre, 3.6f);
        Disc(c, Pop, 0.4f, Shock, Foam, Vector3.Zero, k => 2.4f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        Cue(c, Pop, () => Stage.Explosion(At(c, centre), 1.2f, WaterColor, FxBurst.Dome));
        for (var i = 0; i < 40; i++)
        {
            var way = FxKit.RandomDirection();
            Spark(c, Pop, Rand(0.4f, 0.7f), i % 2 == 0 ? Foam : Sea, centre + way * 1.3f, way * Rand(3f, 6f) + Vector3.Up * 2f, Gravity, 0.07f, 0.04f);
        }
    }

    // ---- Nature.

    private void PetalStorm(Cast c, float r)
    {
        const float Swirl1 = 1.6f, Burst = 0.5f;
        const float Life = Swirl1 + Burst;
        Disc(c, 0f, Life, Runes, Petal, Vector3.Zero, k => r * 0.8f, k => Fade(k, 0.05f, 0.2f), k => Mathf.Min(k * 3f, 1f), gain: 1.2f);
        Disc(c, 0f, Life, Glow, Petal, Vector3.Zero, k => r * 0.8f, k => Fade(k, 0.1f, 0.2f) * 0.4f);
        for (var i = 0; i < 80; i++)
        {
            var a0 = Rand(0f, Mathf.Tau);
            var rad = Rand(0.4f, r * 0.8f);
            var h0 = Rand(0.1f, 0.6f);
            var rise = Rand(1.2f, 2.8f);
            var spin = Rand(2.5f, 4f);
            var tone = i % 4 == 0 ? Rose : i % 4 == 1 ? Fey : Petal;
            Func<float, Vector3> swirl = t => Around(a0 + t * spin) * rad * (1f - 0.3f * t / Swirl1) + Vector3.Up * (h0 + rise * t / Swirl1);
            var last = swirl(Swirl1);
            var outward = (FxKit.Flat(last).Normalized() * Rand(2.5f, 4f) + Vector3.Up * Rand(0f, 1.2f));
            Func<float, Vector3> at = k =>
            {
                var t = k * Life;
                return t < Swirl1 ? swirl(t) : last + outward * Out((t - Swirl1) / Burst);
            };
            Flare(c, 0f, Life, Mote, tone, at, k => 0.2f, k => Fade(k, 0.1f, 0.2f), gain: 1.4f);
            if (i % 3 == 0)
                Line(c, 0f, Life, tone, at, k => Heading(at, k, 0.35f), 0.07f, k => Fade(k, 0.1f, 0.2f));
        }
        Flare(c, Swirl1, 0.35f, Star, Rose, Vector3.Up * 1.5f, 3.6f);
        Disc(c, Swirl1, 0.45f, Shock, Petal, Vector3.Zero, k => r * 1.3f * Out(k), k => 1f - k, band: 0.16f, gain: 1.4f);
        Cue(c, Swirl1, () => Stage.Explosion(At(c, Vector3.Up * 1f), r * 0.5f, Petal.Main, FxBurst.Dome));
        Shine(c, Vector3.Up, r * 1.4f, Petal.Main, age => age < Swirl1 ? 0.3f + 0.5f * age / Swirl1 : 1.8f * Mathf.Max(0f, 1f - (age - Swirl1) / 0.5f));
    }

    private void CreepingRoots(Cast c, float r)
    {
        const float Creep = 0.6f, Hold = 1.6f;
        Disc(c, 0f, 0.4f, Shock, Bark, Vector3.Zero, k => r * 0.4f * Out(k), k => 1f - k, band: 0.2f);
        foreach (var tip in Spread(5, r))
        {
            var side = tip.Cross(Vector3.Up).Normalized();
            var wiggle = Rand(0.3f, 0.6f) * (Rand(0f, 1f) < 0.5f ? -1f : 1f);
            Func<float, Vector3> path = s => tip * s + side * wiggle * Mathf.Sin(s * Mathf.Tau) + Vector3.Up * 0.06f;
            Ribbon(c, 0f, Creep + Hold, 0, Bark, path, k => (0f, Mathf.Min(k * (Creep + Hold) / Creep, 1f)), 0.45f, k => Fade(k, 0f, 0.2f), gain: 1.3f);
            Flare(c, 0f, Creep, Mote, Leaf, path, k => 0.5f, k => 1f, gain: 1.4f);
            // Where it reaches, the root coils up round the spot.
            Column(c, Creep, Hold, Spiral, Bark, tip, k => new Vector2(0.45f, 1.4f * Out(Mathf.Min(k * Hold / 0.2f, 1f))), k => Fade(k, 0f, 0.2f), twist: 3f, speed: 1.5f, gain: 1.4f);
            Disc(c, Creep, Hold, Cracks, Bark, tip, k => 0.8f, k => Fade(k, 0.02f, 0.3f), k => Mathf.Min(k * 5f, 1f));
            Flare(c, Creep, 0.25f, Star, Leaf, tip + Vector3.Up * 0.4f, 1.8f);
            for (var j = 0; j < 6; j++)
                Spark(c, Creep, Rand(0.5f, 0.8f), Leaf, tip + Vector3.Up * 0.3f, FxKit.Jitter(2f) + Vector3.Up * 3f, Gravity * 0.5f, 0.08f, 0.04f);
        }
    }

    private void ThornVolley(Cast c, float r)
    {
        const int Thorns = 26;
        const float Flight = 0.7f, Stuck = 0.8f;
        var from = new Vector3(0f, 1.6f, 0.3f);
        var centre = new Vector3(0f, 0f, r * 0.9f);
        Disc(c, 0f, Flight + 0.1f, Telegraph, Leaf, centre, k => r * 0.6f, k => 0.6f, k => k);
        Flare(c, 0f, 0.25f, Star, Leaf, from, 2f);
        for (var i = 0; i < Thorns; i++)
        {
            var start = Rand(0f, 0.25f);
            var spot = centre + Around(Rand(0f, Mathf.Tau)) * r * 0.6f * Mathf.Sqrt(Rand(0f, 1f));
            var v = (spot - from) / Flight + Vector3.Up * 0.5f * Gravity * Flight;
            Func<float, Vector3> at = k => from + v * k * Flight + Vector3.Down * 0.5f * Gravity * k * k * Flight * Flight;
            var tone = i % 3 == 0 ? Hive : Bark;
            Line(c, start, Flight, tone, at, k => Heading(at, k, 1.1f), 0.16f, k => 1f);
            var dive = Heading(at, 1f, 0.7f);
            Line(c, start + Flight, Stuck, tone, k => spot - dive * 0.3f, k => dive, 0.1f, k => 1f - k * k);
            Disc(c, start + Flight, 0.3f, Shock, Leaf, spot, k => 0.6f * Out(k), k => 1f - k, band: 0.2f);
            for (var j = 0; j < 3; j++)
                Spark(c, start + Flight, 0.35f, Leaf, spot + Vector3.Up * 0.1f, FxKit.Jitter(2f) + Vector3.Up * 2.5f, Gravity, 0.06f, 0.03f);
        }
        Cue(c, Flight, () => Stage.Patch(At(c, centre), r * 0.6f, 1.5f, Leaf.Main, FxGround.Glow));
    }

    // ---- Arcane.

    private void MirrorImages(Cast c)
    {
        const float Split = 0.25f, Stand = 1.6f;
        const float Life = Split + Stand;
        var body = Vector3.Up * 0.85f;
        Flare(c, 0f, 0.3f, Star, Fey, body, 3f);
        Disc(c, 0f, 0.4f, Shock, Fey, Vector3.Zero, k => 2.2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        foreach (var angle in new[] { -2.3f, -0.8f, 0.8f, 2.3f })
        {
            var spot = Around(angle) * 1.7f + body;
            Func<float, Vector3> at = k => body.Lerp(spot, Out(Mathf.Min(k * Life / Split, 1f)));
            Globe(c, "energy", Shell, 0f, Life, Fey, at, k => 1f, k =>
            {
                var t = k * Life;
                return (t < Split ? 0.9f : 0.75f + 0.15f * Mathf.Sin(t * 14f + angle)) * Fade(k, 0f, 0.08f);
            }, mesh: _ghost, gain: 1.2f);
            for (var j = 0; j < 4; j++)
            {
                var off = new Vector3(Rand(-0.25f, 0.25f), Rand(-0.6f, 0.6f), Rand(-0.25f, 0.25f));
                Line(c, 0f, Split + 0.1f, Fey, k => at(k * (Split + 0.1f) / Life) + off, k => (body - spot).Normalized() * 1.2f * (1f - k), 0.04f, k => 1f - k);
            }
            Disc(c, Split, Stand, Glow, Fey, FxKit.Flat(spot), k => 0.7f, k => Fade(k, 0.1f, 0.1f) * 0.6f);
            Flare(c, Life, 0.25f, Star, Fey, spot, 1.8f);
            for (var j = 0; j < 10; j++)
                Spark(c, Life, Rand(0.3f, 0.5f), Fey, spot + new Vector3(Rand(-0.3f, 0.3f), Rand(-0.7f, 0.7f), Rand(-0.3f, 0.3f)), FxKit.Jitter(3f) + Vector3.Up, Gravity * 0.5f, 0.05f, 0.03f);
        }
    }

    private void AstralSigils(Cast c, float r)
    {
        const float Fire1 = 0.55f, Beam1 = 0.35f;
        var target = new Vector3(0f, 0.3f, r * 1.2f);
        var upright = new Basis(Vector3.Right, Mathf.Pi / 2f);
        Vector3[] sigils = [new(-1.7f, 2f, -0.4f), new(0f, 2.9f, -0.7f), new(1.7f, 2f, -0.4f)];
        for (var i = 0; i < sigils.Length; i++)
        {
            var s = sigils[i];
            var delay = i * 0.06f;
            var life = Fire1 + Beam1 + 0.2f - delay;
            Disc(c, delay, life, Runes, Arcane, s, k => 0.75f * Overshoot(Mathf.Min(k * life / 0.25f, 1f)), k => Fade(k, 0.05f, 0.2f), k => Mathf.Min(k * 3f, 1f), basis: upright, gain: 1.4f);
            Disc(c, delay, life, Glow, Arcane, s, k => 0.75f, k => Fade(k, 0.1f, 0.2f) * 0.5f, basis: upright);
            Flare(c, Fire1 - 0.15f, 0.15f + Beam1, Star, Arcane, k => s, k => 1.2f + 0.6f * k, k => Fade(k, 0.2f, 0.3f));
            for (var j = 0; j < 6; j++)
            {
                var from = s + FxKit.RandomDirection() * 1.2f;
                Flare(c, delay + Rand(0f, 0.2f), 0.3f, Mote, Arcane, k => from.Lerp(s, k * k), k => 0.2f, k => 1f);
            }
            Tube(c, Fire1, Beam1, Beam, Arcane, k => (s, target), k => 0.16f * (1f - k * 0.5f), k => Fade(k, 0f, 0.4f), speed: 4f, gain: 1.6f);
        }
        Cue(c, Fire1, () => Stage.Explosion(At(c, target), r * 0.5f, Arcane.Main, FxBurst.Dome));
        Disc(c, Fire1, 0.45f, Shock, Arcane, FxKit.Flat(target), k => r * 1.1f * Out(k), k => 1f - k, band: 0.16f, gain: 1.4f);
        Disc(c, Fire1, 1f, Runes, Arcane, FxKit.Flat(target), k => r * 0.5f, k => Fade(k, 0.02f, 0.5f), k => 1f, gain: 1.2f);
        Flare(c, Fire1, 0.3f, Star, Arcane, target + Vector3.Up * 0.3f, 3.6f);
        Shine(c, target + Vector3.Up, r * 1.4f, Arcane.Main, age => age < Fire1 ? 0.5f : 2f * Mathf.Max(0f, 1f - (age - Fire1) / 0.5f));
    }

    private void RiftTear(Cast c, float r)
    {
        const float Open = 0.25f, Snap = 1.6f;
        var at = new Vector3(0f, 1.6f, r * 0.6f);
        var upright = new Basis(Vector3.Right, Mathf.Pi / 2f);
        Disc(c, 0f, Snap, Swirl, Void, k => new Transform3D(upright * Basis.FromScale(new Vector3(1f * Out(Mathf.Min(k * Snap / Open, 1f)) * (1f - Mathf.Max(0f, (k * Snap - Snap + 0.1f) / 0.1f)), 1f, 2.4f)), at),
            k => Fade(k, 0.05f, 0.02f), spin: 5f, gain: 1.4f);
        Disc(c, 0f, Snap, Glow, Arcane, k => new Transform3D(upright * Basis.FromScale(new Vector3(1.5f * Out(Mathf.Min(k * Snap / Open, 1f)), 1f, 2.9f)), at),
            k => Fade(k, 0.05f, 0.02f) * 0.7f);
        Bolt(c, 0f, Snap, Arcane, at + Vector3.Down * 1.5f, at + Vector3.Up * 1.5f, 0.12f, 8, 0.25f);
        Disc(c, 0f, Snap, Glow, Void, FxKit.Flat(at), k => r * 0.6f, k => Fade(k, 0.1f, 0.05f) * 0.7f);
        for (var i = 0; i < 44; i++)
        {
            var from = at + FxKit.RandomDirection() * Rand(2f, 3.2f);
            Flare(c, Rand(0f, Snap - 0.5f), 0.5f, Mote, i % 2 == 0 ? Void : Arcane, k => from.Lerp(at, k * k), k => 0.25f * (1f - k * 0.5f), k => Fade(k, 0.2f, 0.1f), gain: 1.3f);
        }
        Cue(c, Snap, () => Stage.Explosion(At(c, at), r * 0.5f, Arcane.Main, FxBurst.Dome));
        Flare(c, Snap, 0.3f, Star, Arcane, at, 4f);
        Disc(c, Snap, 0.45f, Shock, Arcane, FxKit.Flat(at), k => r * 1.4f * Out(k), k => 1f - k, band: 0.16f, gain: 1.4f);
        for (var i = 0; i < 24; i++)
            Spark(c, Snap, Rand(0.3f, 0.6f), Arcane, at, FxKit.RandomDirection() * Rand(4f, 8f), Gravity * 0.5f, 0.06f, 0.04f);
        Shine(c, at, r * 1.4f, Arcane.Main, age => age < Snap ? 0.6f : 2f * Mathf.Max(0f, 1f - (age - Snap) / 0.5f));
    }

    // ---- Holy.

    private void Benediction(Cast c, float r)
    {
        const float Life = 2.4f;
        Column(c, 0f, Life, Light, Gold, Vector3.Zero, k => new Vector2(0.95f, 12f), k => Fade(k, 0.08f, 0.25f) * 0.9f, gain: 1.5f, speed: -1.5f);
        Disc(c, 0.1f, Life - 0.1f, Band, Gold, k => Lay(Vector3.Up * 2.2f, 0.42f * Overshoot(Mathf.Min(k * Life / 0.3f, 1f))), k => Fade(k, 0.05f, 0.15f), band: 0.18f, gain: 1.8f);
        Disc(c, 0f, Life, Runes, Gold, Vector3.Zero, k => r * 0.6f, k => Fade(k, 0.05f, 0.2f), k => Mathf.Min(k * 3f, 1f), gain: 1.2f);
        foreach (var at in new[] { 0.2f, 0.8f, 1.4f })
            Disc(c, at, 0.6f, Shock, Gold, Vector3.Zero, k => r * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
        for (var i = 0; i < 30; i++)
        {
            var spot = Around(Rand(0f, Mathf.Tau)) * Rand(0.3f, 1.6f) + Vector3.Up * Rand(3f, 5f);
            var side = Around(Rand(0f, Mathf.Tau));
            var phase = Rand(0f, Mathf.Tau);
            // A feather rocking from side to side as it drifts down.
            Line(c, Rand(0f, Life - 1.2f), 1.2f, Gold, k => spot + Vector3.Down * k * 2.4f + side * Mathf.Sin(k * 9f + phase) * 0.35f,
                k => side * 0.3f * Mathf.Cos(k * 9f + phase) + Vector3.Up * 0.12f, 0.09f, k => Fade(k, 0.2f, 0.3f));
        }
        for (var i = 0; i < 20; i++)
        {
            var spot = Around(Rand(0f, Mathf.Tau)) * Rand(0.2f, 0.9f);
            Flare(c, Rand(0f, Life - 0.8f), 0.8f, Mote, Gold, k => spot + Vector3.Up * (0.2f + 2.2f * k), k => 0.2f, k => Fade(k, 0.2f, 0.4f));
        }
        Shine(c, Vector3.Up * 2f, r * 1.4f, Gold.Main, age => 1.4f * Fade(age / Life, 0.08f, 0.25f));
    }

    private void SealOfLight(Cast c, float r)
    {
        const float Rise = 0.5f, Draw = 0.6f, Flash = 1.2f, After = 0.6f;
        const float Life = Flash + After;
        var corners = Enumerable.Range(0, 6).Select(i => Around(i * Mathf.Tau / 6f) * r * 0.8f).ToArray();
        Disc(c, 0f, Life, Runes, Gold, Vector3.Zero, k => r * 0.85f, k => Fade(k, 0.05f, 0.3f), k => Mathf.Min(k * Life / Flash, 1f), gain: 1.2f);
        for (var i = 0; i < 6; i++)
        {
            var p = corners[i];
            var delay = i * 0.06f;
            var life = Life - delay;
            Column(c, delay, life, Light, Gold, p, k => new Vector2(0.28f, 3.2f * Out(Mathf.Min(k * life / 0.25f, 1f))), k => Fade(k, 0f, 0.3f), gain: 1.5f, speed: -2f);
            Flare(c, delay, 0.25f, Star, Gold, p + Vector3.Up * 0.2f, 1.6f);
        }
        // A hexagram: two triangles, drawn edge by edge.
        int[][] triangles = [[0, 2, 4], [1, 3, 5]];
        const float Edge = Draw / 6f;
        var n = 0;
        foreach (var tri in triangles)
        {
            for (var e = 0; e < 3; e++)
            {
                var a = corners[tri[e]] + Vector3.Up * 0.08f;
                var b = corners[tri[(e + 1) % 3]] + Vector3.Up * 0.08f;
                var start = Rise + n++ * Edge;
                var life = Life - start;
                Tube(c, start, life, Beam, Gold, k => (a, a.Lerp(b, Mathf.Min(k * life / Edge, 1f))), k => 0.07f, k => Fade(k, 0f, 0.25f), speed: 3f, gain: 1.8f);
            }
        }
        Column(c, Flash, After, Light, Gold, Vector3.Zero, k => new Vector2(r * 0.8f, 10f), k => (1f - k) * (1f - k), gain: 1.8f, speed: -2f);
        Disc(c, Flash, 0.5f, Shock, Gold, Vector3.Zero, k => r * 1.5f * Out(k), k => 1f - k, band: 0.14f, gain: 1.5f);
        Cue(c, Flash, () => Stage.Explosion(At(c, Vector3.Up * 0.4f), r * 0.6f, FxTheme.Of(FxElement.Holy), FxBurst.Dome));
        Flare(c, Flash, 0.3f, Star, Gold, Vector3.Up, 4.4f);
        Shine(c, Vector3.Up, r * 1.8f, Gold.Main, age => age < Flash ? 0.3f + 0.6f * age / Flash : 2.2f * Mathf.Max(0f, 1f - (age - Flash) / After));
    }

    // ---- Shadow.

    private void ShadowTendrils(Cast c, float r)
    {
        const float Life = 2f;
        const int Tendrils = 7, Segments = 10;
        var pool = new Vector3(0f, 0f, r * 0.5f);
        Disc(c, 0f, Life, Swirl, Void, pool, k => r * 0.55f, k => Fade(k, 0.08f, 0.15f), spin: 3f, gain: 1.2f);
        Disc(c, 0f, Life, Glow, Shadow, pool, k => r * 0.65f, k => Fade(k, 0.08f, 0.15f) * 0.6f);
        for (var v = 0; v < Tendrils; v++)
        {
            var angle = v * Mathf.Tau / Tendrils + Rand(-0.2f, 0.2f);
            var foot = pool + Around(angle) * Rand(0.2f, r * 0.45f);
            var lean = Around(angle + Rand(-0.5f, 0.5f));
            var tall = Rand(1.8f, 2.8f);
            var phase = Rand(0f, Mathf.Tau);
            var delay = Rand(0f, 0.3f);
            var life = Life - delay;
            Vector3 Along1(float s, float t) => foot + lean * s * 0.9f + Vector3.Up * s * tall
                + new Vector3(Mathf.Sin(t * 5f + s * 3f + phase), 0f, Mathf.Cos(t * 4f + s * 2.5f + phase)) * 0.4f * s;
            float Grow(float t) => Out(Mathf.Min(t / 0.35f, 1f)) * (1f - Mathf.Max(0f, (t - (life - 0.35f)) / 0.35f));
            for (var j = 0; j < Segments; j++)
            {
                var s0 = (float)j / Segments;
                var s1 = (j + 1f) / Segments;
                var width = 0.24f * (1f - s0 * 0.75f);
                Line(c, delay, life, j % 3 == 0 ? Void : Shadow, k =>
                {
                    var t = k * life;
                    return (Along1(s0 * Grow(t), t) + Along1(s1 * Grow(t), t)) * 0.5f;
                }, k =>
                {
                    var t = k * life;
                    return (Along1(s1 * Grow(t), t) - Along1(s0 * Grow(t), t)) * 1.25f;
                }, width, k => Fade(k, 0f, 0.15f));
            }
            Flare(c, delay, life, Mote, Shadow, k => Along1(Grow(k * life), k * life), k => 0.35f, k => Fade(k, 0.1f, 0.15f));
        }
        for (var i = 0; i < 18; i++)
        {
            var spot = pool + Around(Rand(0f, Mathf.Tau)) * Rand(0f, r * 0.5f);
            Flare(c, Rand(0f, Life - 0.8f), 0.8f, Mote, Void, k => spot + Vector3.Up * (0.1f + 1.5f * k), k => 0.25f, k => Fade(k, 0.2f, 0.4f));
        }
        Cue(c, 0.3f, () => Stage.Patch(At(c, pool), r * 0.55f, Life - 0.3f, Shadow.Main, FxGround.Miasma));
    }

    private void SoulHarvest(Cast c, float r)
    {
        const float Sweep = 0.3f, Pull = 0.9f;
        var chest = Vector3.Up * 1.2f;
        Crescent(c, 0f, Sweep + 0.1f, new Transform3D(new Basis(Vector3.Right, Mathf.DegToRad(-8f)), Vector3.Up * 1f), r, 300f, Doom, thick: 0.3f, sweep: 0.4f, gain: 1.5f);
        Disc(c, 0.05f, 0.45f, Shock, Doom, Vector3.Zero, k => r * 1.1f * Out(k), k => 1f - k, band: 0.14f, gain: 1.3f);
        var latest = 0f;
        foreach (var soul in Spread(8, r * 0.9f))
        {
            var from = soul + Vector3.Up;
            var delay = 0.15f + Rand(0f, 0.2f);
            latest = Mathf.Max(latest, delay + Pull);
            var curl = Rand(-1.5f, 1.5f);
            var side = soul.Cross(Vector3.Up).Normalized();
            Func<float, Vector3> at = k => from.Lerp(chest, k * k) + side * curl * Mathf.Sin(k * Mathf.Pi) * 0.6f + Vector3.Up * Mathf.Sin(k * Mathf.Pi) * 0.5f;
            Flare(c, delay - 0.1f, 0.25f, Star, Soul, from, 1.3f);
            Flare(c, delay, Pull, Mote, Soul, at, k => 0.9f, k => 1f, gain: 1.6f);
            Globe(c, "energy", Orb, delay, Pull, Soul, at, k => 0.22f, k => 1f, gain: 1.5f);
            Line(c, delay, Pull, Soul, k => at(Mathf.Max(k - 0.08f, 0f)), k => Heading(at, k, 1.4f), 0.22f, k => 1f);
        }
        Flare(c, latest, 0.35f, Star, Soul, chest, 3f);
        Column(c, latest - 0.1f, 0.7f, Spiral, Soul, Vector3.Zero, k => new Vector2(0.9f, 2.4f * Out(Mathf.Min(k * 4f, 1f))), k => 1f - k, twist: 2f, speed: 3f, gain: 1.4f);
        Disc(c, latest, 0.45f, Shock, Soul, Vector3.Zero, k => 2.2f * Out(k), k => 1f - k, band: 0.16f, gain: 1.3f);
        Shine(c, chest, r * 1.4f, Soul.Main, age => age < latest ? 0.6f : 1.8f * Mathf.Max(0f, 1f - (age - latest) / 0.5f));
    }
}
