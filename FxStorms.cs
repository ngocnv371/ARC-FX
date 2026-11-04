using Godot;

namespace Arc.Fx;

/// <summary>A storm orb that is still hanging in the air: where its heart is, for strikes to leave from.</summary>
public readonly struct FxOrb
{
    private readonly FxHandle _handle;

    internal FxOrb(FxHandle handle) => _handle = handle;

    public bool Alive => _handle.Alive;

    /// <summary>The orb's centre in the air (it bobs).</summary>
    public Vector3 Heart => (_handle.Live as FxStorms.Orb)?.Heart ?? Vector3.Zero;

    public void Stop() => _handle.Stop();
}

/// <summary>
/// The crackling things: a storm orb (a tangle of lightning threads round a white core over a
/// glow marking its reach, spitting little arcs), a frost bomb (a crystal cluster rising out of
/// hoarfrost, spinning faster and glowing brighter until it bursts) and a nova (two rings racing
/// out over a globe of lightning, jagged bolts arcing round the rim as they land).
/// </summary>
internal sealed class FxStorms(FxKit kit, FxWorld world)
{
    private const float OrbHeight = 2f;
    private const float OrbSize = 0.7f;
    private const int OrbThreads = 12;
    private const int OrbKinks = 5;
    public static readonly Color OrbColor = new(0.45f, 0.65f, 1f);

    /// <summary>How big a bomb's crystal cluster is drawn (the mesh is about a quarter metre tall).</summary>
    private const float BombSize = 6f;
    public static readonly Color BombColor = new(0.55f, 0.85f, 1f);

    /// <summary>The ground plane reaches this far past a nova's ring, so its crackle isn't clipped.</summary>
    private const float NovaPlane = 1.25f;
    private const int NovaStyle = 5;
    public static readonly Color NovaColor = new(0.45f, 0.7f, 1f);

    internal sealed class Orb : FxItem
    {
        public required Node3D Node;
        public required MeshInstance3D Core;
        public required MeshInstance3D Halo;
        public required OmniLight3D Light;
        public required MeshInstance3D[] Threads;
        public Vector3 Centre;
        public Vector3 Heart;
        public float NextCrackle;
        public float NextTangle;
    }

    private sealed class Bomb : FxItem
    {
        public required Node3D Node;
        public required MeshInstance3D Crystal;
        public required ShaderMaterial Material;
        public required MeshInstance3D Halo;
        public required OmniLight3D Light;
        public Vector3 Centre;
        public float Radius;
        public float Turn;
        public bool Burst;
        public override void Stop()
        {
            Burst = false;
            Life = Age;
        }
    }

    private sealed class Ring : FxItem
    {
        public required MeshInstance3D Ground;
        public required ShaderMaterial Material;
        public required MeshInstance3D Globe;
        public required ShaderMaterial GlobeMaterial;
        public Vector3 Centre;
        public float Radius;
        public bool Arced;
    }

    private readonly List<Orb> _orbs = [];
    private readonly List<Bomb> _bombs = [];
    private readonly List<Ring> _novas = [];
    private readonly StandardMaterial3D _haloMaterial = FxKit.Glow(new Color(0.15f, 0.35f, 1f, 0.22f));
    private readonly StandardMaterial3D _bombHaloMaterial = FxKit.Glow(new Color(0.3f, 0.65f, 1f, 0.2f));
    private readonly ArrayMesh _bombMesh = FxCrystalMesh.Cluster();

    public FxOrb StormOrb(Vector3 centre, float radius, float life)
    {
        var orb = _orbs.Find(o => !o.Active) ?? MakeOrb();
        orb.Begin(life);
        orb.Centre = centre with { Y = 0f };
        orb.NextCrackle = orb.NextTangle = 0f;
        orb.Node.Visible = true;
        world.Patch(orb.Centre, radius, life, OrbColor);
        Tick(orb, 0f);
        return new FxOrb(new FxHandle(orb));
    }

    public FxHandle FrostBomb(Vector3 centre, float radius, float fuse)
    {
        var bomb = _bombs.Find(b => !b.Active) ?? MakeBomb();
        bomb.Begin(fuse);
        bomb.Centre = centre with { Y = 0f };
        bomb.Radius = radius;
        bomb.Burst = true;
        bomb.Turn = GD.Randf() * Mathf.Tau;
        bomb.Node.Visible = true;
        world.Patch(bomb.Centre, radius, fuse, BombColor, FxGround.Frost);
        Tick(bomb, 0f);
        return new FxHandle(bomb);
    }

    public FxHandle Nova(Vector3 centre, float radius)
    {
        var nova = _novas.Find(n => !n.Active) ?? MakeNova();
        nova.Begin(FxTiming.NovaLife);
        nova.Centre = centre with { Y = 0f };
        nova.Radius = radius;
        nova.Arced = false;
        var extent = radius * NovaPlane;
        nova.Ground.Position = nova.Centre + Vector3.Up * FxKit.GroundLift;
        nova.Ground.Scale = new Vector3(extent, 1f, extent);
        nova.Material.SetShaderParameter("uExtent", new Vector2(extent, extent));
        nova.Material.SetShaderParameter("uRings", new Vector3(FxTiming.NovaInner / NovaPlane, 1f / NovaPlane, FxTiming.NovaBand / extent));
        nova.Ground.Visible = true;
        nova.Globe.Visible = true;
        Tick(nova, 0f);
        return new FxHandle(nova);
    }

    public void Tick(float dt)
    {
        foreach (var nova in _novas)
        {
            if (nova.Active)
                Tick(nova, dt);
        }
        foreach (var orb in _orbs)
        {
            if (orb.Active)
                Tick(orb, dt);
        }
        foreach (var bomb in _bombs)
        {
            if (bomb.Active)
                Tick(bomb, dt);
        }
    }

    private void Tick(Orb orb, float dt)
    {
        orb.Age += dt;
        if (orb.Age >= orb.Life)
        {
            orb.Active = false;
            orb.Node.Visible = false;
            return;
        }
        // Swells in, bobs and flickers while it lasts, and shrinks away at the end.
        var size = OrbSize * Mathf.Clamp(Mathf.Min(orb.Age / 0.25f, (orb.Life - orb.Age) / 0.3f), 0f, 1f);
        var height = OrbHeight + 0.12f * Mathf.Sin(orb.Age * 3f);
        orb.Heart = orb.Centre + Vector3.Up * height;
        orb.Node.Position = orb.Heart;
        orb.Core.Scale = Vector3.One * size * (float)GD.RandRange(0.22, 0.32);
        orb.Halo.Scale = Vector3.One * size * (float)GD.RandRange(1.2, 1.45);
        orb.Light.LightEnergy = (float)GD.RandRange(1.6, 2.4) * size / OrbSize;
        FxKit.Pool(orb.Light, 0.35f);
        if (orb.Age >= orb.NextTangle)
        {
            orb.NextTangle += 0.05f;
            Tangle(orb, size);
        }
        if (orb.Age >= orb.NextCrackle)
        {
            orb.NextCrackle += (float)GD.RandRange(0.06, 0.16);
            // Lines are drawn Lift above their ends, so this puts the arc's start in the orb.
            var heart = orb.Heart - Vector3.Up * FxLines.Lift;
            var arc = Vector3.Forward.Rotated(Vector3.Right, (float)GD.RandRange(-1.2, 1.2)).Rotated(Vector3.Up, GD.Randf() * Mathf.Tau);
            world.Lightning([(heart, heart + arc * (float)GD.RandRange(0.5, 1.1))], 0.07f);
        }
    }

    /// <summary>
    /// Knots the orb's threads afresh: each runs over the ball from one random point on its
    /// surface to another, bulging in and out as it goes; a third start from the core instead,
    /// so the bundle looks fed from its heart.
    /// </summary>
    private static void Tangle(Orb orb, float size)
    {
        for (var t = 0; t < OrbThreads; t++)
        {
            var (from, to) = (FxKit.RandomDirection(), FxKit.RandomDirection());
            var previous = t % 3 == 0 ? from * size * 0.1f : from * size;
            for (var k = 1; k <= OrbKinks; k++)
            {
                var reach = k == OrbKinks ? 1f : (float)GD.RandRange(0.55, 1.15);
                var point = from.Slerp(to, (float)k / OrbKinks) * size * reach;
                FxKit.Lay(orb.Threads[t * OrbKinks + k - 1], previous, point, (float)GD.RandRange(0.015, 0.03));
                previous = point;
            }
        }
    }

    private void Tick(Bomb bomb, float dt)
    {
        bomb.Age += dt;
        if (bomb.Age >= bomb.Life)
        {
            bomb.Active = false;
            bomb.Node.Visible = false;
            if (bomb.Burst)
                world.Explosion(bomb.Centre, bomb.Radius, BombColor, FxBurst.Frost);
            return;
        }
        // Rises out of the ground, then spins faster and glows brighter as the fuse burns down,
        // and shivers for the last moments before it bursts.
        var charge = bomb.Age / bomb.Life;
        var rise = Mathf.Clamp(bomb.Age / 0.2f, 0f, 1f);
        bomb.Turn += dt * Mathf.Lerp(0.6f, 7f, charge * charge);
        var shiver = charge > 0.75f ? (charge - 0.75f) * 0.6f : 0f;
        var jitter = new Vector3((float)GD.RandRange(-shiver, shiver), 0f, (float)GD.RandRange(-shiver, shiver));
        var size = BombSize * rise * (1f + 0.12f * charge);
        bomb.Node.Position = bomb.Centre + jitter + Vector3.Up * (size * 0.17f);
        bomb.Crystal.Scale = Vector3.One * size;
        bomb.Crystal.Rotation = new Vector3(0f, bomb.Turn, 0f);
        // Throbs quicker as it charges.
        var throb = 0.5f + 0.5f * Mathf.Sin(bomb.Age * Mathf.Lerp(6f, 30f, charge));
        bomb.Material.SetShaderParameter("uGlow", Mathf.Lerp(0.15f, 0.8f, charge) + 0.2f * throb * charge);
        bomb.Halo.Scale = Vector3.One * size * 0.22f * (1f + 0.6f * charge + 0.15f * throb);
        bomb.Light.LightEnergy = rise * Mathf.Lerp(0.8f, 4f, charge * charge) * (0.85f + 0.3f * throb);
        FxKit.Pool(bomb.Light, 0.3f);
    }

    private void Tick(Ring nova, float dt)
    {
        nova.Age += dt;
        if (nova.Age >= nova.Life)
        {
            nova.Active = false;
            nova.Ground.Visible = false;
            nova.Globe.Visible = false;
            return;
        }
        nova.Material.SetShaderParameter("uAge", nova.Age);
        FxExplosions.PlaceGlobe(nova.Globe, nova.GlobeMaterial, nova.Centre, nova.Radius, nova.Age, spark: true);
        var spent = 1f - nova.Age / FxTiming.NovaLife;
        Fx.Light(nova.Centre + Vector3.Up * 0.8f, nova.Radius * 1.6f + 1f, NovaColor, 1.4f * spent * spent);

        // Once the ring lands, jagged bolts arc round its rim for a moment.
        if (nova.Age < FxTiming.NovaSpread || nova.Arced)
            return;
        nova.Arced = true;
        var count = Mathf.Max(10, Mathf.RoundToInt(Mathf.Tau * nova.Radius / 1.1f));
        var turn = GD.Randf() * Mathf.Tau;
        // Lines are drawn Lift above their ends: this keeps them just off the ground.
        var low = Vector3.Up * (0.3f - FxLines.Lift);
        var segments = new (Vector3 A, Vector3 B)[count];
        for (var i = 0; i < count; i++)
        {
            var a = Vector3.Forward.Rotated(Vector3.Up, turn + Mathf.Tau * i / count) * (nova.Radius - (float)GD.RandRange(0.0, 0.4));
            var b = Vector3.Forward.Rotated(Vector3.Up, turn + Mathf.Tau * (i + 1) / count) * (nova.Radius - (float)GD.RandRange(0.0, 0.4));
            segments[i] = (nova.Centre + a + low, nova.Centre + b + low);
        }
        world.Lightning(segments, 0.3f);
    }

    private Orb MakeOrb()
    {
        var node = new Node3D();
        var core = FxKit.Mesh(kit.Orb, kit.Bolt);
        var halo = FxKit.Mesh(kit.Orb, _haloMaterial);
        var light = new OmniLight3D { LightColor = OrbColor, OmniRange = 6f, ShadowEnabled = false };
        node.AddChild(halo);
        node.AddChild(core);
        node.AddChild(light);
        var threads = new MeshInstance3D[OrbThreads * OrbKinks];
        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = kit.BoltPiece();
            node.AddChild(threads[i]);
        }
        kit.Parent.AddChild(node);
        var orb = new Orb { Node = node, Core = core, Halo = halo, Light = light, Threads = threads };
        _orbs.Add(orb);
        return orb;
    }

    private Bomb MakeBomb()
    {
        var node = new Node3D();
        var material = FxKit.Crystal(BombColor, new Color(0.9f, 0.98f, 1f), rim: 0.8f);
        var crystal = new MeshInstance3D { Mesh = _bombMesh, MaterialOverride = material };
        var halo = FxKit.Mesh(kit.Orb, _bombHaloMaterial);
        var light = new OmniLight3D { LightColor = BombColor, OmniRange = 5f, ShadowEnabled = false };
        node.AddChild(crystal);
        node.AddChild(halo);
        node.AddChild(light);
        kit.Parent.AddChild(node);
        var bomb = new Bomb { Node = node, Crystal = crystal, Material = material, Halo = halo, Light = light };
        _bombs.Add(bomb);
        return bomb;
    }

    private Ring MakeNova()
    {
        var material = FxKit.GroundMaterial(NovaStyle, NovaColor);
        material.SetShaderParameter("uLife", FxTiming.NovaLife);
        material.SetShaderParameter("uSpread", FxTiming.NovaSpread);
        var globeMaterial = FxKit.Shader("sparkball");
        globeMaterial.SetShaderParameter("uColor", FxKit.Rgb(NovaColor));
        var globe = FxKit.Mesh(kit.Globe, globeMaterial);
        kit.Parent.AddChild(globe);
        var nova = new Ring { Ground = kit.Plane(material), Material = material, Globe = globe, GlobeMaterial = globeMaterial };
        _novas.Add(nova);
        return nova;
    }
}
