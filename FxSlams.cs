using Godot;

namespace Arc.Fx;

/// <summary>What spikes thrust out of the ground are made of.</summary>
public enum FxSpike { Stone, Ice }

/// <summary>What a smite brings down from the sky.</summary>
public enum FxSmite { Fist, Hammer }

/// <summary>
/// Things out of and onto the ground. Spikes (rock, or ice crystal) thrust up and sink back,
/// alone, in a ring across a slam, or racing along a rupture. A ring of spectral shields
/// (fx_spectral) pushes out from a point. A smite brings a huge spectral fist (punching down
/// from the sky) or hammer (swung down from upright) onto a spot, where it lands as a blast
/// throwing up rock.
/// </summary>
internal sealed class FxSlams(FxKit kit, FxWorld world)
{
    /// <summary>How long a spike takes to thrust up, stays up, and takes to sink back.</summary>
    private const float SpikeRise = 0.08f;
    private const float SpikeHold = 0.4f;
    private const float SpikeSink = 0.3f;
    public static readonly Color IceColor = new(0.6f, 0.85f, 1f);
    public static readonly Color DustColor = new(0.85f, 0.7f, 0.5f);

    /// <summary>How far apart a rupture's spikes come up along it.</summary>
    private const float RuptureStep = 0.5f;

    private const float ShieldHeight = 2f;
    /// <summary>How far a shield's face is tipped up toward the sky (rise over run).</summary>
    private const float ShieldTip = 1.1f;
    public static readonly Color ShieldColor = new(1f, 0.8f, 0.42f);

    /// <summary>Where a fist starts, from the spot: this far back and this high.</summary>
    private const float FistBack = 5f;
    private const float FistHigh = 11f;
    public static readonly Color FistColor = new(1f, 0.78f, 0.35f);
    public static readonly Color HammerColor = new(0.62f, 0.8f, 1f);

    private sealed class Spike : FxItem
    {
        public required Node3D Node;
        public FxSpike Look;
        public float Delay;
        public float Height;
        public Vector3 Base;
    }

    private sealed class Ruptured : FxItem
    {
        public Vector3 From;
        public Vector3 Direction;
        public float Length;
        public float HalfWidth;
        public float Travel;
        public float NextSpike;
        public int Side;
    }

    private sealed class ShieldSet : FxItem
    {
        public required Node3D[] Shields;
        public required GeometryInstance3D[] Parts;
        public Vector3 Centre;
        public float Radius;
        public float Turn;
    }

    private sealed class Smiting : FxItem
    {
        public required Node3D Node;
        public required GeometryInstance3D[] Parts;
        public FxSmite Look;
        public Vector3 Centre;
        public Vector3 Facing;
        public float Size;
        public float Radius;
        public bool Landed;
    }

    private readonly List<Spike> _spikes = [];
    private readonly List<Ruptured> _ruptures = [];
    private readonly List<ShieldSet> _rings = [];
    private readonly List<Smiting> _smites = [];
    private readonly ArrayMesh _iceSpikeMesh = FxCrystalMesh.Shard(6);
    private ShaderMaterial? _iceSpikeMaterial;

    /// <summary>A slam: a blast (a frost blast for ice, a burst of dust for stone) and a ring of spikes across it.</summary>
    public void Slam(Vector3 centre, float radius, FxSpike look)
    {
        centre.Y = 0f;
        if (look == FxSpike.Ice)
            world.Explosion(centre, radius, IceColor, FxBurst.Frost);
        else
            world.Explosion(centre, radius, DustColor);
        SpikeRing(centre, radius, look);
    }

    /// <summary>A few spikes close in and more round the edge, each ring a moment later, leaning outward.</summary>
    public void SpikeRing(Vector3 centre, float radius, FxSpike look)
    {
        var size = look == FxSpike.Ice ? 1.4f : 1.6f;
        foreach (var (reach, share) in new[] { (0.35f, 0.35f), (0.75f, 1f) })
        {
            var count = Mathf.Max(3, Mathf.RoundToInt(radius * 2.6f * share));
            var turn = GD.Randf() * Mathf.Tau;
            for (var i = 0; i < count; i++)
            {
                var way = Vector3.Forward.Rotated(Vector3.Up, turn + Mathf.Tau * (i + (float)GD.RandRange(-0.3, 0.3)) / count);
                var at = centre + way * radius * reach * (float)GD.RandRange(0.85, 1.15);
                Jut(at, way, size * (float)GD.RandRange(0.7, 1.1) * Mathf.Lerp(1.15f, 0.85f, reach), reach * 0.1f, look);
            }
        }
    }

    /// <summary>A spike <paramref name="height"/> metres tall thrusting up at <paramref name="at"/> after <paramref name="delay"/> seconds, leaning toward <paramref name="lean"/>.</summary>
    public FxHandle Jut(Vector3 at, Vector3 lean, float height, float delay, FxSpike look)
    {
        var spike = _spikes.Find(s => !s.Active && s.Look == look) ?? MakeSpike(look);
        spike.Begin(delay + SpikeRise + SpikeHold + SpikeSink);
        spike.Delay = delay;
        spike.Height = height;
        spike.Base = FxKit.Flat(at);
        var basis = new Basis(Vector3.Up, GD.Randf() * Mathf.Tau);
        lean = FxKit.Flat(lean);
        if (lean.LengthSquared() > 1e-4f)
            basis = new Basis(Vector3.Up.Cross(lean.Normalized()).Normalized(), (float)GD.RandRange(0.15, 0.45)) * basis;
        spike.Node.Basis = basis.Scaled(Vector3.One * height);
        spike.Node.Visible = false;
        Tick(spike, 0f);
        return new FxHandle(spike);
    }

    /// <summary>A line of rock spikes racing out from <paramref name="from"/> at <see cref="FxTiming.RuptureSpeed"/>, either side of the line within half of <paramref name="width"/>.</summary>
    public FxHandle Rupture(Vector3 from, Vector3 direction, float length, float width)
    {
        var rupture = _ruptures.Find(r => !r.Active);
        if (rupture == null)
            _ruptures.Add(rupture = new Ruptured());
        rupture.Begin(length / FxTiming.RuptureSpeed + 1f);
        rupture.From = FxKit.Flat(from);
        rupture.Direction = FxKit.Flat(direction).Normalized();
        rupture.Length = length;
        rupture.HalfWidth = width * 0.5f;
        rupture.Travel = 0f;
        rupture.NextSpike = 0.6f;
        rupture.Side = 1;
        return new FxHandle(rupture);
    }

    public FxHandle ShieldRing(Vector3 centre, int count, float radius)
    {
        var ring = _rings.Find(r => !r.Active && r.Shields.Length == count) ?? MakeShieldRing(count);
        ring.Begin(FxTiming.ShieldLife);
        ring.Centre = FxKit.Flat(centre);
        ring.Radius = radius;
        ring.Turn = GD.Randf() * Mathf.Tau;
        foreach (var shield in ring.Shields)
            shield.Visible = true;
        Tick(ring, 0f);
        return new FxHandle(ring);
    }

    public FxHandle Smite(FxSmite look, Vector3 centre, Vector3 facing, float radius)
    {
        var smite = _smites.Find(s => !s.Active && s.Look == look) ?? MakeSmite(look);
        smite.Begin(FxTiming.SmiteFall + FxTiming.SmiteLinger);
        smite.Centre = FxKit.Flat(centre);
        smite.Facing = FxKit.Flat(facing).LengthSquared() > 1e-4f ? FxKit.Flat(facing).Normalized() : Vector3.Forward;
        smite.Radius = radius;
        smite.Size = look == FxSmite.Fist ? radius * 1.1f : radius * 1.7f;
        smite.Landed = false;
        smite.Node.Visible = true;
        Tick(smite, 0f);
        return new FxHandle(smite);
    }

    public void Tick(float dt)
    {
        foreach (var spike in _spikes)
        {
            if (spike.Active)
                Tick(spike, dt);
        }
        foreach (var rupture in _ruptures)
        {
            if (rupture.Active)
                Tick(rupture, dt);
        }
        foreach (var ring in _rings)
        {
            if (ring.Active)
                Tick(ring, dt);
        }
        foreach (var smite in _smites)
        {
            if (smite.Active)
                Tick(smite, dt);
        }
    }

    private static void Tick(Spike spike, float dt)
    {
        spike.Age += dt;
        var t = spike.Age - spike.Delay;
        if (spike.Age >= spike.Life)
        {
            spike.Active = false;
            spike.Node.Visible = false;
            return;
        }
        if (t < 0f)
            return;
        // Out fast with a little overshoot, then down slower.
        var up = t < SpikeRise ? Mathf.Sin(t / SpikeRise * Mathf.Pi * 0.6f) / Mathf.Sin(Mathf.Pi * 0.6f)
            : t < SpikeRise + SpikeHold ? 1f
            : 1f - Mathf.SmoothStep(0f, 1f, (t - SpikeRise - SpikeHold) / SpikeSink);
        spike.Node.Visible = true;
        spike.Node.Position = spike.Base + Vector3.Up * (up - 1f) * spike.Height;
    }

    private void Tick(Ruptured rupture, float dt)
    {
        rupture.Age += dt;
        rupture.Travel = Mathf.Min(rupture.Length, rupture.Travel + FxTiming.RuptureSpeed * dt);
        var side = Vector3.Up.Cross(rupture.Direction);
        while (rupture.NextSpike <= rupture.Travel)
        {
            var at = rupture.From + rupture.Direction * rupture.NextSpike
                + side * rupture.Side * rupture.HalfWidth * (float)GD.RandRange(0.05, 0.45);
            var grow = Mathf.Lerp(0.8f, 1.25f, rupture.NextSpike / rupture.Length);
            Jut(at, rupture.Direction + side * rupture.Side * 0.6f, (float)GD.RandRange(0.9, 1.4) * grow, 0f, FxSpike.Stone);
            rupture.NextSpike += RuptureStep * (float)GD.RandRange(0.8, 1.2);
            rupture.Side = -rupture.Side;
        }
        if (rupture.Travel >= rupture.Length || rupture.Age >= rupture.Life)
            rupture.Active = false;
    }

    private static void Tick(ShieldSet ring, float dt)
    {
        ring.Age += dt;
        if (ring.Age >= ring.Life)
        {
            ring.Active = false;
            foreach (var shield in ring.Shields)
                shield.Visible = false;
            return;
        }
        var spread = Mathf.Min(1f, ring.Age / FxTiming.ShieldSpread);
        var radius = FxTiming.ShieldRingRadius(ring.Age, ring.Radius);
        var alpha = Mathf.Min(1f, ring.Age / 0.06f)
            * (1f - Mathf.SmoothStep(0f, 1f, (ring.Age - FxTiming.ShieldSpread) / (FxTiming.ShieldLife - FxTiming.ShieldSpread)));
        var count = ring.Shields.Length;
        for (var i = 0; i < count; i++)
        {
            var way = Vector3.Forward.Rotated(Vector3.Up, ring.Turn + Mathf.Tau * i / count);
            var shield = ring.Shields[i];
            shield.Position = ring.Centre + way * radius + Vector3.Up * ShieldHeight * 0.5f;
            // Faced outward and tipped back, so the high camera sees their faces rather than their rims.
            shield.LookAt(shield.Position + way + Vector3.Up * ShieldTip, Vector3.Up);
            shield.Scale = Vector3.One * Mathf.Lerp(0.8f, 1.15f, spread);
        }
        foreach (var part in ring.Parts)
            part.SetInstanceShaderParameter("uAlpha", alpha);
    }

    private void Tick(Smiting smite, float dt)
    {
        smite.Age += dt;
        if (smite.Age >= smite.Life)
        {
            smite.Active = false;
            smite.Node.Visible = false;
            return;
        }
        var fall = Mathf.Min(1f, smite.Age / FxTiming.SmiteFall);
        // It gathers speed all the way down.
        var k = fall * fall * fall;
        var after = Mathf.Max(0f, smite.Age - FxTiming.SmiteFall) / FxTiming.SmiteLinger;
        var yaw = new Basis(Vector3.Up, Mathf.Atan2(-smite.Facing.X, -smite.Facing.Z));
        if (smite.Look == FxSmite.Fist)
        {
            // Punching down along a slant from high behind the spot, the arm trailing back up it.
            var start = smite.Centre - smite.Facing * FistBack + Vector3.Up * FistHigh;
            var travel = (smite.Centre - start).Normalized();
            var at = start.Lerp(smite.Centre, k) + Vector3.Down * after * 0.6f;
            var tilt = new Basis(yaw.X, Mathf.Acos(Mathf.Clamp(-travel.Y, -1f, 1f)));
            smite.Node.Basis = (tilt * yaw).Scaled(Vector3.One * smite.Size);
            smite.Node.Position = at;
        }
        else
        {
            // Swung down from leaning back past upright, about the end of its haft behind the spot.
            var pivot = smite.Centre - smite.Facing * smite.Size + Vector3.Up * smite.Size * 0.12f;
            var swing = Mathf.Lerp(-0.4f, Mathf.Pi * 0.5f, k);
            smite.Node.Basis = (new Basis(yaw.X, -swing) * yaw).Scaled(Vector3.One * smite.Size);
            smite.Node.Position = pivot;
        }
        var alpha = Mathf.Min(1f, smite.Age / 0.1f) * (1f - Mathf.SmoothStep(0f, 1f, after));
        var flash = smite.Landed ? Mathf.Max(0f, 1f - after * 3f) : 0f;
        foreach (var part in smite.Parts)
        {
            part.SetInstanceShaderParameter("uAlpha", alpha);
            part.SetInstanceShaderParameter("uImpact", flash);
        }
        if (smite.Landed || fall < 1f)
            return;
        smite.Landed = true;
        var hammer = smite.Look == FxSmite.Hammer;
        world.Explosion(smite.Centre, smite.Radius, hammer ? HammerColor : FistColor, hammer ? FxBurst.Lightning : FxBurst.Dome);
        SpikeRing(smite.Centre, smite.Radius * 0.85f, FxSpike.Stone);
    }

    private Spike MakeSpike(FxSpike look)
    {
        Node3D node;
        if (look == FxSpike.Ice)
        {
            _iceSpikeMaterial ??= FxKit.Crystal(IceColor, new Color(0.9f, 0.98f, 1f), rim: 0.8f);
            node = FxKit.Upright(FxKit.Wrap(FxKit.Mesh(_iceSpikeMesh, _iceSpikeMaterial)), 0.45f);
        }
        else
            node = FxKit.Upright(FxModels.RockSpikeOrFallback(), 0.45f);
        kit.Parent.AddChild(node);
        var spike = new Spike { Node = node, Look = look };
        _spikes.Add(spike);
        return spike;
    }

    private ShieldSet MakeShieldRing(int count)
    {
        var shields = new Node3D[count];
        var parts = new List<GeometryInstance3D>();
        var material = FxKit.Spectral(ShieldColor);
        for (var i = 0; i < count; i++)
        {
            var shield = new Node3D();
            shield.AddChild(FxKit.Facing(FxModels.ShieldOrFallback(), ShieldHeight));
            parts.AddRange(FxKit.Paint(shield, material));
            shield.Visible = false;
            kit.Parent.AddChild(shield);
            shields[i] = shield;
        }
        var ring = new ShieldSet { Shields = shields, Parts = [.. parts] };
        _rings.Add(ring);
        return ring;
    }

    private Smiting MakeSmite(FxSmite look)
    {
        var node = new Node3D { Visible = false };
        node.AddChild(look == FxSmite.Fist ? BuildFist() : BuildHammer());
        var parts = FxKit.Paint(node, FxKit.Spectral(look == FxSmite.Fist ? FistColor : HammerColor));
        kit.Parent.AddChild(node);
        var smite = new Smiting { Node = node, Parts = parts, Look = look };
        _smites.Add(smite);
        return smite;
    }

    /// <summary>
    /// A clenched fist about a metre across, knuckles down on the origin and the forearm rising
    /// from it along +Y, the fingers curled toward -Z (the way it faces).
    /// </summary>
    private static Node3D BuildFist()
    {
        var fist = new Node3D();
        void Block(Vector3 size, Vector3 at, float roll = 0f) =>
            fist.AddChild(new MeshInstance3D { Mesh = new BoxMesh { Size = size }, Position = at, Rotation = new Vector3(0f, 0f, roll) });
        // Four curled fingers, the middle two reaching lowest, then the back of the hand and the thumb across them.
        var drop = new[] { 0.05f, 0f, 0f, 0.04f };
        for (var i = 0; i < 4; i++)
            Block(new Vector3(0.23f, 0.34f, 0.5f), new Vector3(-0.36f + 0.24f * i, 0.17f + drop[i], -0.05f));
        Block(new Vector3(1.02f, 0.62f, 0.62f), new Vector3(0f, 0.62f, 0.04f));
        Block(new Vector3(0.24f, 0.5f, 0.26f), new Vector3(-0.6f, 0.42f, -0.12f), 0.35f);
        fist.AddChild(new MeshInstance3D
        {
            Mesh = new CylinderMesh { TopRadius = 0.42f, BottomRadius = 0.34f, Height = 1.8f, RadialSegments = 8, Rings = 1 },
            Position = new Vector3(0f, 1.8f, 0.04f),
        });
        return fist;
    }

    /// <summary>
    /// A war hammer a metre long, the end of its haft on the origin and the head at +Y, lying
    /// along Z: the smite swings it down about X, so a face (the one at -Z) is what lands.
    /// </summary>
    private static Node3D BuildHammer()
    {
        if (FxModels.Hammer?.Invoke() is not { } model)
            return FxKit.Wrap(new MeshInstance3D { Mesh = FxProps.Hammer() });
        Node3D hammer = FxKit.Upright(model, 0f);
        // The model's head may be at either end of its haft: put it at the top.
        if (FxKit.HeadLow(hammer))
        {
            var flip = new Node3D { Basis = new Basis(Vector3.Right, Mathf.Pi), Position = Vector3.Up };
            flip.AddChild(hammer);
            hammer = new Node3D();
            hammer.AddChild(flip);
        }
        // And its head may lie across the swing: turn it to lie along it.
        if (!FxKit.HeadAcross(hammer))
            return hammer;
        var turn = new Node3D { Basis = new Basis(Vector3.Up, Mathf.Pi / 2f) };
        turn.AddChild(hammer);
        var holder = new Node3D();
        holder.AddChild(turn);
        return holder;
    }
}
