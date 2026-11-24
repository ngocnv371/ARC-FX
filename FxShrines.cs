using Godot;

namespace Arc.Fx;

/// <summary>A shrine standing somewhere: take it, ask whether it has been, or take it away.</summary>
public readonly struct FxShrine
{
    private readonly FxHandle _handle;

    internal FxShrine(FxHandle handle) => _handle = handle;

    private FxShrines.Shrine? Live => _handle.Live as FxShrines.Shrine;

    /// <summary>Still standing (charged or spent).</summary>
    public bool Alive => _handle.Alive;

    /// <summary>Its power has been taken: it stands dark.</summary>
    public bool Taken => Live is { Taker: not null };

    /// <summary>
    /// <paramref name="who"/> takes it: its gem flares and breaks, its light rushes out across the
    /// ground in a burst and streams into them (reaching them <see cref="FxTiming.ShrineReach"/>
    /// later, the moment to give the boon, where a heal washes up them), and the stone goes dark.
    /// With <paramref name="boon"/> above 0, the shrine's sigil then lies at their feet for that many
    /// seconds, the boon's time; renew or end it with <see cref="Fx.Sigil(Node3D, FxSigil, float, float, FxSigilLook)"/>
    /// as for any aura. Taking a spent shrine does nothing.
    /// </summary>
    public void Take(Node3D who, float boon = 0f)
    {
        if (Live is { Taker: null } shrine)
        {
            FxShrines.Take(shrine, who, boon);
            // The gem breaks, and its light reaches the taker.
            var world = FxWorld.Of(shrine.Root);
            world?.Later(FxSound.Glass, shrine.Root.Position, 0.7f);
            world?.Later(FxSound.LevelUp, who.Position, 0.8f, FxTiming.ShrineReach);
        }
    }

    /// <summary>Takes it away, charged or spent.</summary>
    public void Stop() => _handle.Stop();
}

/// <summary>
/// Pooled shrines: a stone pillar (<see cref="FxProps.Shrine"/>, fx_crystal) cradling a gem of
/// what the shrine gives, hanging and turning in a column of light (fx_column) with motes (fx_mote)
/// drifting up round it, over the sigil of what it does (<see cref="FxSigils"/>). Taken, the gem
/// breaks in a burst, its light streams into the taker as a heal, and the stone stands dark.
/// </summary>
internal sealed class FxShrines(FxKit kit, FxWorld world)
{
    /// <summary>How many motes drift up round a charged shrine, and how long each takes to rise.</summary>
    private const int MoteCount = 12;
    private const float MoteRise = 2.4f;
    /// <summary>How many sparks of light stream into the taker.</summary>
    private const int SparkCount = 14;
    /// <summary>How big the gem is drawn (the shard mesh is about 0.4 metres tall).</summary>
    private const float GemScale = 1.7f;
    /// <summary>How tall the column of light stands above the gem, and how wide it is.</summary>
    private const float ColumnHeight = 4.5f;
    private const float ColumnRadius = 0.3f;
    /// <summary>How wide the sigil under a shrine is, and how often it is renewed.</summary>
    private const float SigilRadius = 1.7f;
    private const float SigilEvery = 0.2f;
    /// <summary>How long a taken shrine takes to go dark.</summary>
    private const float Darken = 0.9f;
    /// <summary>The stone, and the dull glint it is left with once spent.</summary>
    private static readonly Color Stone = new(0.24f, 0.23f, 0.26f);
    private static readonly Color Spent = new(0.3f, 0.3f, 0.34f);

    internal sealed class Shrine : FxItem
    {
        public required Node3D Root;
        public required MeshInstance3D Pillar;
        public required MeshInstance3D Gem;
        public required MeshInstance3D Halo;
        public required MeshInstance3D Column;
        public required MeshInstance3D[] Motes;
        public required MeshInstance3D[] Sparks;
        public required ShaderMaterial PillarMaterial;
        public required ShaderMaterial GemMaterial;
        public required ShaderMaterial GlowMaterial;
        public required ShaderMaterial ColumnMaterial;
        public FxSigil Sigil;
        public Color Color;
        public float Size;
        public float NextSigil;
        public Node3D? Taker;
        public Vector3 TakerAt;
        public float Boon;
        public float TakenAt;
        public bool Burst;
        public bool Arrived;
        public readonly float[] MotePhase = new float[MoteCount];
        public readonly Vector3[] SparkBend = new Vector3[SparkCount];
        public readonly float[] SparkDelay = new float[SparkCount];
    }

    private readonly List<Shrine> _shrines = [];
    private readonly ArrayMesh _stoneMesh = FxProps.Shrine();
    private readonly ArrayMesh _gemMesh = FxCrystalMesh.Shard(6);
    private readonly CylinderMesh _tube = new() { TopRadius = 1f, BottomRadius = 1f, Height = 1f, RadialSegments = 24, Rings = 2, CapTop = false, CapBottom = false };

    public FxShrine Spawn(Vector3 at, FxSigil sigil, Color color, float size)
    {
        var shrine = _shrines.Find(s => !s.Active) ?? Make();
        shrine.Begin(float.MaxValue);
        shrine.Root.Position = at with { Y = 0f };
        shrine.Root.Scale = Vector3.One * size;
        // Unturned: the prongs then stand either side and behind the gem as the game camera sees it, never across it.
        shrine.Sigil = sigil;
        shrine.Size = size;
        shrine.NextSigil = 0f;
        shrine.Taker = null;
        shrine.Burst = false;
        shrine.Arrived = false;
        shrine.Color = color;
        var rgb = FxKit.Rgb(color);
        shrine.PillarMaterial.SetShaderParameter("uAccent", rgb);
        shrine.PillarMaterial.SetShaderParameter("uGlow", 0.08f);
        shrine.GemMaterial.SetShaderParameter("uBase", rgb);
        shrine.GemMaterial.SetShaderParameter("uAccent", FxKit.Rgb(color.Lerp(Colors.White, 0.45f)));
        shrine.GlowMaterial.SetShaderParameter("uColor", rgb);
        shrine.ColumnMaterial.SetShaderParameter("uColor", rgb);
        shrine.ColumnMaterial.SetShaderParameter("uHot", FxKit.Rgb(color.Lerp(Colors.White, 0.7f)));
        shrine.ColumnMaterial.SetShaderParameter("uDeep", FxKit.Rgb(color.Darkened(0.6f)));
        for (var i = 0; i < MoteCount; i++)
            shrine.MotePhase[i] = (i + (float)GD.RandRange(-0.3, 0.3)) / MoteCount;
        shrine.Root.Visible = true;
        shrine.Gem.Visible = true;
        shrine.Halo.Visible = true;
        shrine.Column.Visible = true;
        foreach (var spark in shrine.Sparks)
            spark.Visible = false;
        Tick(shrine, 0f);
        return new FxShrine(new FxHandle(shrine));
    }

    internal static void Take(Shrine shrine, Node3D who, float boon)
    {
        shrine.Taker = who;
        shrine.TakerAt = who.Position;
        shrine.Boon = boon;
        shrine.TakenAt = shrine.Age;
        for (var i = 0; i < SparkCount; i++)
        {
            // Each bows out its own way, up and to one side, so the stream braids.
            var side = (i % 2 == 0 ? 1f : -1f) * (float)GD.RandRange(0.4, 1.6);
            shrine.SparkBend[i] = new Vector3(side, (float)GD.RandRange(0.8, 2.2), (float)GD.RandRange(-0.6, 0.6));
            shrine.SparkDelay[i] = FxTiming.ShrineReach * 0.3f * i / (SparkCount - 1);
        }
    }

    public void Tick(float dt)
    {
        foreach (var shrine in _shrines)
        {
            if (shrine.Active)
                Tick(shrine, dt);
        }
    }

    private void Tick(Shrine shrine, float dt)
    {
        shrine.Age += dt;
        if (shrine.Age >= shrine.Life)
        {
            shrine.Active = false;
            shrine.Root.Visible = false;
            foreach (var spark in shrine.Sparks)
                spark.Visible = false;
            shrine.Taker = null;
            return;
        }
        var size = shrine.Size;
        var foot = shrine.Root.Position;
        var cradle = foot + Vector3.Up * FxProps.ShrineCradle * size;
        var since = shrine.Taker != null ? shrine.Age - shrine.TakenAt : -1f;
        // 1 while charged, falling to 0 as it goes dark once taken.
        var charge = since < 0f ? 1f : Mathf.Clamp(1f - since / Darken, 0f, 1f);
        var swell = Mathf.Clamp(shrine.Age / 0.4f, 0f, 1f);

        if (charge > 0f && (shrine.NextSigil -= dt) <= 0f)
        {
            shrine.NextSigil = SigilEvery;
            if (since < 0f)
                world.Sigil(shrine.Root, shrine.Sigil, SigilRadius * size, SigilEvery * 2f, shrine.Color);
        }

        // The stone's rim of light dims to a cold glint as it goes dark.
        var accent = since < 0f ? shrine.Color : Spent.Lerp(shrine.Color, charge * charge);
        if (since >= 0f && since - dt < Darken + 0.1f)
        {
            shrine.PillarMaterial.SetShaderParameter("uAccent", FxKit.Rgb(accent));
            shrine.PillarMaterial.SetShaderParameter("uGlow", 0.08f * charge);
        }

        // The gem hangs in its cradle, bobbing and turning; taken, it flares and breaks.
        var bob = Mathf.Sin(shrine.Age * 1.7f) * 0.06f;
        var flare = since < 0f ? 1f : since < 0.12f ? 1f + 2.5f * since : Mathf.Max(0f, 1.3f * (1f - (since - 0.12f) / 0.12f));
        shrine.Gem.Visible = flare > 0f;
        shrine.Gem.Position = Vector3.Up * (FxProps.ShrineCradle + bob);
        shrine.Gem.Rotation = new Vector3(0f, shrine.Age * 0.9f, 0f);
        shrine.Gem.Scale = Vector3.One * GemScale * swell * Mathf.Max(flare, 0.001f);

        var pulse = 0.85f + 0.15f * Mathf.Sin(shrine.Age * 3.1f);
        var flash = since >= 0f && since < 0.35f ? 1f - since / 0.35f : 0f;
        shrine.Halo.Visible = charge > 0f;
        shrine.Halo.Position = shrine.Gem.Position;
        shrine.Halo.Scale = Vector3.One * (1.5f * pulse + 2.5f * flash);
        shrine.Halo.SetInstanceShaderParameter("uAlpha", swell * Mathf.Max(charge * 0.8f, flash));

        // The column of light rising out of the gem, flaring up as it is taken and then gone.
        shrine.Column.Visible = charge > 0f;
        var height = ColumnHeight * (0.4f + 0.6f * swell) * (1f + 0.5f * flash);
        shrine.Column.Position = Vector3.Up * (FxProps.ShrineCradle - 0.3f + height * 0.5f);
        shrine.Column.Scale = new Vector3(ColumnRadius * (1f - 0.5f * (1f - charge)), height, ColumnRadius * (1f - 0.5f * (1f - charge)));
        shrine.Column.SetInstanceShaderParameter("uAlpha", swell * charge * (0.75f + 0.25f * pulse + 1.2f * flash));

        // Motes drifting up round the stone, winding in toward the column as they climb.
        for (var i = 0; i < MoteCount; i++)
        {
            var mote = shrine.Motes[i];
            var t = Mathf.PosMod(shrine.Age / MoteRise + shrine.MotePhase[i], 1f);
            var angle = shrine.MotePhase[i] * Mathf.Tau * 3f + t * 2.6f;
            var reach = 0.85f * (1f - 0.55f * t);
            mote.Visible = charge > 0f;
            mote.Position = new Vector3(Mathf.Sin(angle) * reach, 0.25f + t * 3f, Mathf.Cos(angle) * reach);
            mote.Scale = Vector3.One * 0.32f * (1f - 0.4f * t);
            var twinkle = 0.75f + 0.25f * Mathf.Sin(shrine.Age * 19f + i * 2.3f);
            mote.SetInstanceShaderParameter("uAlpha", swell * charge * Mathf.Sin(t * Mathf.Pi) * twinkle);
        }

        if (since >= 0f)
            Stream(shrine, since, cradle);

        var glow = since < 0f ? 0.7f * pulse : 0.7f * charge + 2f * flash;
        Fx.Light(cradle, 3.5f * size, shrine.Color, swell * glow);
    }

    /// <summary>The taken shrine's light streaming into its taker, and what it does when it reaches them.</summary>
    private void Stream(Shrine shrine, float since, Vector3 cradle)
    {
        var taker = shrine.Taker!;
        if (GodotObject.IsInstanceValid(taker))
            shrine.TakerAt = taker.Position;
        var size = shrine.Size;
        var into = shrine.TakerAt + Vector3.Up * 1f * size;
        if (!shrine.Burst)
        {
            shrine.Burst = true;
            world.Explosion(shrine.Root.Position, 1.8f * size, shrine.Color, FxBurst.Dome);
        }

        // Each spark flies a bowed path from the gem into the taker, quickening as it goes.
        var way = into - cradle;
        var flat = new Vector3(way.X, 0f, way.Z);
        var right = flat.LengthSquared() > 1e-4f ? Vector3.Up.Cross(flat).Normalized() : Vector3.Right;
        var flight = FxTiming.ShrineReach * 0.7f;
        for (var i = 0; i < SparkCount; i++)
        {
            var spark = shrine.Sparks[i];
            var t = (since - shrine.SparkDelay[i]) / flight;
            if (t <= 0f || t >= 1f)
            {
                spark.Visible = false;
                continue;
            }
            var bend = shrine.SparkBend[i];
            var k = t * t * (3f - 2f * t);
            var bow = right * bend.X + Vector3.Up * bend.Y + flat.Normalized() * bend.Z;
            var mid = (cradle + into) * 0.5f + bow * size;
            var at = cradle.Lerp(mid, k).Lerp(mid.Lerp(into, k), k);
            spark.Visible = true;
            spark.Position = at;
            spark.Scale = Vector3.One * 0.45f * size * (1f - 0.35f * t);
            spark.SetInstanceShaderParameter("uAlpha", Mathf.Clamp(t / 0.1f, 0f, 1f) * (1f - t * t * 0.5f));
        }

        if (!shrine.Arrived && since >= FxTiming.ShrineReach)
        {
            shrine.Arrived = true;
            if (GodotObject.IsInstanceValid(taker))
            {
                world.Heal(taker, shrine.Color, size);
                if (shrine.Boon > 0f)
                    world.Sigil(taker, shrine.Sigil, 1.3f * size, shrine.Boon, shrine.Color);
            }
        }
    }

    private Shrine Make()
    {
        var root = new Node3D { Visible = false };
        var pillarMaterial = FxKit.Crystal(Stone, Colors.White, rim: 0.55f, glow: 0.08f);
        var pillar = FxKit.Mesh(_stoneMesh, pillarMaterial);
        root.AddChild(pillar);
        var gemMaterial = FxKit.Crystal(Colors.White, new Color(1f, 1f, 0.95f), rim: 0.9f, glow: 0.55f);
        var gem = FxKit.Mesh(_gemMesh, gemMaterial);
        root.AddChild(gem);
        var glowMaterial = FxKit.Shader("mote");
        var halo = FxKit.Mesh(kit.Quad, glowMaterial);
        root.AddChild(halo);
        var columnMaterial = FxKit.Shader("column");
        columnMaterial.SetShaderParameter("uStyle", 0);
        columnMaterial.SetShaderParameter("uGain", 1.1f);
        columnMaterial.SetShaderParameter("uSpeed", 0.5f);
        var column = FxKit.Mesh(_tube, columnMaterial);
        root.AddChild(column);
        var motes = new MeshInstance3D[MoteCount];
        for (var i = 0; i < MoteCount; i++)
        {
            motes[i] = FxKit.Mesh(kit.Quad, glowMaterial);
            root.AddChild(motes[i]);
        }
        // Sparks fly in the world's space, not the shrine's, so they are not under the root.
        var sparksNode = new Node3D();
        var sparks = new MeshInstance3D[SparkCount];
        for (var i = 0; i < SparkCount; i++)
        {
            sparks[i] = FxKit.Mesh(kit.Quad, glowMaterial);
            sparks[i].Visible = false;
            sparksNode.AddChild(sparks[i]);
        }
        kit.Parent.AddChild(root);
        kit.Parent.AddChild(sparksNode);
        var shrine = new Shrine
        {
            Root = root,
            Pillar = pillar,
            Gem = gem,
            Halo = halo,
            Column = column,
            Motes = motes,
            Sparks = sparks,
            PillarMaterial = pillarMaterial,
            GemMaterial = gemMaterial,
            GlowMaterial = glowMaterial,
            ColumnMaterial = columnMaterial,
        };
        _shrines.Add(shrine);
        return shrine;
    }
}
