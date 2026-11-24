using Godot;

namespace Arc.Fx;

/// <summary>A barrier that is still up round someone: light it where it is hit, keep it, or end it.</summary>
public readonly struct FxBarrier
{
    private readonly FxHandle _handle;

    internal FxBarrier(FxHandle handle) => _handle = handle;

    private FxCasts.Bubble? Live => _handle.Live as FxCasts.Bubble;

    public bool Alive => _handle.Alive;

    /// <summary>Shows for <paramref name="seconds"/> more from now.</summary>
    public void Keep(float seconds) => _handle.Keep(seconds);

    /// <summary>
    /// A blow lands on it from <paramref name="from"/>: a flash where it strikes and a ripple of
    /// lit cells running out across the skin. The four latest hits show at once.
    /// </summary>
    public void Hit(Vector3 from)
    {
        if (Live is { } bubble)
        {
            FxCasts.Hit(bubble, from);
            FxWorld.Of(bubble.Shell)?.Later(FxSound.Plate, bubble.Shell.GlobalPosition, 0.6f);
        }
    }

    /// <summary>Fades it away.</summary>
    public void Stop() => _handle.Stop();

    /// <summary>It gives way: it bursts in a flash of its colour and is gone.</summary>
    public void Break()
    {
        if (Live is { } bubble)
        {
            bubble.Broken = true;
            FxWorld.Of(bubble.Shell)?.Later(FxSound.Glass, bubble.Shell.GlobalPosition);
        }
        _handle.Stop();
    }
}

/// <summary>
/// What casters call on themselves and others: a meteor (a burning rock that falls slanting out of
/// the sky and lands as a fire blast over burning ground; any warning on the ground is the game's), a heal
/// (motes spiralling up round someone through a band of light washing up their body, over a
/// glow at their feet) and a barrier (a bubble of light, fx_barrier, round someone that ripples
/// where it is hit and bursts when it breaks).
/// </summary>
internal sealed class FxCasts(FxKit kit, FxWorld world)
{
    /// <summary>Where a meteor starts, back from and above where it lands.</summary>
    private static readonly Vector3 MeteorFrom = new(-4f, 12f, -2.5f);
    private static readonly Color MeteorRock = new(0.07f, 0.04f, 0.03f);
    private static readonly Color MeteorEmber = new(1f, 0.42f, 0.08f);
    /// <summary>How long burning ground smoulders where a meteor landed.</summary>
    private const float MeteorBurn = 2.5f;

    /// <summary>How long a heal plays, how many motes rise in it and how high its band of light climbs.</summary>
    private const float HealLife = 1.2f;
    private const int HealMotes = 16;
    private const float HealHeight = 2.1f;

    /// <summary>How long a barrier takes to swell in, to fade, and to burst when it breaks; how high its centre sits as a share of its radius.</summary>
    private const float BarrierIn = 0.2f;
    private const float BarrierOut = 0.25f;
    private const float BarrierLift = 0.55f;

    private sealed class Meteor : FxItem
    {
        public required MeshInstance3D Rock;
        public FxShot Flame;
        public Vector3 At;
        public Vector3 From;
        public float Radius;
        public Vector3 Tumble;
    }

    private sealed class Heal : FxItem
    {
        public required Node3D Node;
        public required MeshInstance3D Band;
        public required MeshInstance3D[] Motes;
        public required ShaderMaterial Material;
        public required ShaderMaterial BandMaterial;
        public Node3D? Follow;
        public Vector3 At;
        public Color Color;
        public float Size;
        public float[] Phase = new float[HealMotes];
        public float[] Delay = new float[HealMotes];
        public float[] Reach = new float[HealMotes];
    }

    internal sealed class Bubble : FxItem
    {
        public required MeshInstance3D Shell;
        public required ShaderMaterial Material;
        public Node3D? Follow;
        public Vector3 At;
        public Color Color;
        public float Radius;
        public float Fading = -1f;
        public bool Broken;
        public readonly Vector4[] Hits = [new(0f, 1f, 0f, 9f), new(0f, 1f, 0f, 9f), new(0f, 1f, 0f, 9f), new(0f, 1f, 0f, 9f)];
        public int NextHit;

        // A barrier fades rather than vanishing (or bursts, if broken).
        public override void Stop()
        {
            if (Fading < 0f)
                Fading = 0f;
        }
    }

    private readonly List<Meteor> _meteors = [];
    private readonly List<Heal> _heals = [];
    private readonly List<Bubble> _bubbles = [];
    // Few faces, flat shaded by fx_crystal, so it reads as a lump of rock.
    private readonly SphereMesh _rockMesh = new() { Radius = 1f, Height = 2f, RadialSegments = 7, Rings = 4 };
    private readonly CylinderMesh _bandMesh = new() { TopRadius = 1f, BottomRadius = 1f, Height = 1f, RadialSegments = 24, Rings = 1, CapTop = false, CapBottom = false };
    private readonly SphereMesh _bubbleMesh = new() { Radius = 1f, Height = 2f, RadialSegments = 48, Rings = 24 };

    public FxHandle SpawnMeteor(Vector3 at, float radius)
    {
        var meteor = _meteors.Find(m => !m.Active) ?? MakeMeteor();
        meteor.Begin(FxTiming.MeteorFall);
        meteor.At = at with { Y = 0f };
        meteor.Radius = radius;
        // Each falls from a slightly different way, tumbling its own way.
        meteor.From = meteor.At + MeteorFrom.Rotated(Vector3.Up, (float)GD.RandRange(-0.35, 0.35));
        meteor.Tumble = FxKit.RandomDirection() * 7f;
        meteor.Rock.Visible = true;
        meteor.Flame = world.Shot(FxShotLook.Fireball);
        Tick(meteor, 0f);
        return new FxHandle(meteor);
    }

    public FxHandle SpawnHeal(Node3D? follow, Vector3 at, Color color, float size)
    {
        var heal = _heals.Find(h => !h.Active) ?? MakeHeal();
        heal.Begin(HealLife);
        heal.Follow = follow;
        heal.At = (follow?.Position ?? at) with { Y = 0f };
        heal.Size = size;
        if (heal.Color != color)
        {
            heal.Color = color;
            heal.Material.SetShaderParameter("uColor", FxKit.Rgb(color));
            heal.BandMaterial.SetShaderParameter("uColor", FxKit.Rgb(color));
        }
        for (var i = 0; i < HealMotes; i++)
        {
            heal.Phase[i] = Mathf.Tau * i / HealMotes + (float)GD.RandRange(-0.2, 0.2);
            heal.Delay[i] = (float)GD.RandRange(0.0, 0.35);
            heal.Reach[i] = (float)GD.RandRange(0.45, 0.75);
        }
        heal.Node.Visible = true;
        world.Patch(heal.At, 1.1f * size, HealLife * 0.8f, color);
        Tick(heal, 0f);
        return new FxHandle(heal);
    }

    public FxBarrier SpawnBarrier(Node3D? follow, Vector3 at, float radius, float life, Color color)
    {
        var bubble = _bubbles.Find(b => !b.Active) ?? MakeBubble();
        bubble.Begin(life);
        bubble.Follow = follow;
        bubble.At = (follow?.Position ?? at) with { Y = 0f };
        bubble.Radius = radius;
        bubble.Fading = -1f;
        bubble.Broken = false;
        bubble.NextHit = 0;
        for (var i = 0; i < bubble.Hits.Length; i++)
            bubble.Hits[i] = new Vector4(0f, 1f, 0f, 9f);
        if (bubble.Color != color)
        {
            bubble.Color = color;
            bubble.Material.SetShaderParameter("uColor", FxKit.Rgb(color));
        }
        bubble.Shell.Visible = true;
        Tick(bubble, 0f);
        return new FxBarrier(new FxHandle(bubble));
    }

    internal static void Hit(Bubble bubble, Vector3 from)
    {
        var centre = bubble.Shell.Position;
        var way = from - centre;
        // From straight above or inside: strike the crown.
        way = way.LengthSquared() > 1e-4f ? way.Normalized() : Vector3.Up;
        bubble.Hits[bubble.NextHit] = new Vector4(way.X, way.Y, way.Z, 0f);
        bubble.NextHit = (bubble.NextHit + 1) % bubble.Hits.Length;
    }

    public void Tick(float dt)
    {
        foreach (var meteor in _meteors)
        {
            if (meteor.Active)
                Tick(meteor, dt);
        }
        foreach (var heal in _heals)
        {
            if (heal.Active)
                Tick(heal, dt);
        }
        foreach (var bubble in _bubbles)
        {
            if (bubble.Active)
                Tick(bubble, dt);
        }
    }

    private void Tick(Meteor meteor, float dt)
    {
        meteor.Age += dt;
        if (meteor.Age >= meteor.Life)
        {
            meteor.Active = false;
            meteor.Rock.Visible = false;
            meteor.Flame.Release();
            // Stopped early (called off): it simply goes.
            if (meteor.Age < FxTiming.MeteorFall - 1e-3f)
                return;
            world.Explosion(meteor.At, meteor.Radius, FxTheme.FireLight, FxBurst.Fire);
            world.Patch(meteor.At, meteor.Radius * 0.75f, MeteorBurn, FxTheme.FireLight, FxGround.Fire);
            return;
        }
        // Falls faster as it comes: eased in, so it hangs a moment high up and then slams down.
        var u = meteor.Age / FxTiming.MeteorFall;
        var fall = Mathf.Pow(u, 1.7f);
        var position = meteor.From.Lerp(meteor.At, fall);
        var velocity = (meteor.At - meteor.From) * (1.7f * Mathf.Pow(Mathf.Max(u, 0.05f), 0.7f) / FxTiming.MeteorFall);
        var size = Mathf.Clamp(meteor.Radius / 3f, 0.6f, 2f);
        meteor.Flame.Place(position, velocity, 2.2f * size, Mathf.Clamp(meteor.Age / 0.15f, 0f, 1f));
        meteor.Rock.Position = position;
        meteor.Rock.Rotation = meteor.Tumble * meteor.Age;
        meteor.Rock.Scale = Vector3.One * 0.42f * size;
    }

    private void Tick(Heal heal, float dt)
    {
        heal.Age += dt;
        if (heal.Age >= heal.Life)
        {
            heal.Active = false;
            heal.Node.Visible = false;
            return;
        }
        if (heal.Follow != null && GodotObject.IsInstanceValid(heal.Follow))
            heal.At = heal.Follow.Position with { Y = 0f };
        heal.Node.Position = heal.At;
        var u = heal.Age / heal.Life;
        var size = heal.Size;

        // The band washes up the body, easing out, and thins as it goes.
        var climb = 1f - (1f - u) * (1f - u);
        heal.Band.Position = Vector3.Up * (0.1f + climb * HealHeight * 0.7f) * size + Vector3.Up * 0.25f * size;
        heal.Band.Scale = new Vector3(0.7f * size * (1f - 0.25f * climb), 0.5f * size, 0.7f * size * (1f - 0.25f * climb));
        heal.Band.SetInstanceShaderParameter("uAlpha", 1.5f * Mathf.Clamp(heal.Age / 0.1f, 0f, 1f) * (1f - u) * (1f - u));

        // Motes spiral up, drawing in toward the body as they rise, then wink out at the top.
        for (var i = 0; i < HealMotes; i++)
        {
            var mote = heal.Motes[i];
            var t = (heal.Age - heal.Delay[i]) / (heal.Life - heal.Delay[i]);
            if (t <= 0f)
            {
                mote.Visible = false;
                continue;
            }
            mote.Visible = true;
            var angle = heal.Phase[i] + t * 3.2f;
            var reach = heal.Reach[i] * (1f - 0.45f * t) * size;
            var height = (0.1f + t * HealHeight) * size;
            mote.Position = new Vector3(Mathf.Sin(angle) * reach, height, Mathf.Cos(angle) * reach);
            var twinkle = 0.75f + 0.25f * Mathf.Sin(heal.Age * 22f + i * 1.7f);
            mote.Scale = Vector3.One * 0.45f * size * (1f - 0.5f * t);
            mote.SetInstanceShaderParameter("uAlpha", Mathf.Clamp(t / 0.12f, 0f, 1f) * (1f - t * t) * twinkle);
        }
        Fx.Light(heal.At + Vector3.Up * 0.8f, 2.5f * size, heal.Color, 0.6f * (1f - u));
    }

    private void Tick(Bubble bubble, float dt)
    {
        bubble.Age += dt;
        // Running out of life or told to stop: fade (or burst) rather than vanish.
        if (bubble.Age >= bubble.Life && bubble.Fading < 0f)
            bubble.Fading = 0f;
        if (bubble.Fading >= 0f)
            bubble.Fading += dt;
        if (bubble.Follow != null && GodotObject.IsInstanceValid(bubble.Follow))
            bubble.At = bubble.Follow.Position with { Y = 0f };

        if (bubble.Broken && bubble.Fading >= 0f)
        {
            // It gives way at once: a burst of its colour in its place.
            world.Explosion(bubble.At, bubble.Radius * 1.1f, bubble.Color, FxBurst.Dome);
            End(bubble);
            return;
        }
        if (bubble.Fading >= BarrierOut)
        {
            End(bubble);
            return;
        }

        // Swells in with a little overshoot; shrinks a touch as it fades.
        var swell = Mathf.Clamp(bubble.Age / BarrierIn, 0f, 1f);
        var pop = 1f + 0.12f * Mathf.Sin(swell * Mathf.Pi);
        var fade = bubble.Fading >= 0f ? 1f - bubble.Fading / BarrierOut : 1f;
        var radius = bubble.Radius * swell * pop * (0.9f + 0.1f * fade);
        bubble.Shell.Position = bubble.At + Vector3.Up * bubble.Radius * BarrierLift;
        bubble.Shell.Scale = Vector3.One * Mathf.Max(radius, 0.01f);
        bubble.Material.SetShaderParameter("uAlpha", Mathf.Min(swell * 1.5f, 1f) * fade);

        // Hits age; the latest is brightest.
        var lit = 0f;
        for (var i = 0; i < bubble.Hits.Length; i++)
        {
            var h = bubble.Hits[i];
            h.W += dt;
            bubble.Hits[i] = h;
            if (h.W < 0.3f)
                lit = Mathf.Max(lit, 1f - h.W / 0.3f);
        }
        bubble.Material.SetShaderParameter("uHits", bubble.Hits);
        Fx.Light(bubble.Shell.Position, bubble.Radius * 1.6f, bubble.Color, (0.25f + 0.9f * lit) * fade * swell);
    }

    private static void End(Bubble bubble)
    {
        bubble.Active = false;
        bubble.Shell.Visible = false;
    }

    private Meteor MakeMeteor()
    {
        var rock = FxKit.Mesh(_rockMesh, FxKit.Crystal(MeteorRock, MeteorEmber, rim: 0.9f, glow: 0.35f));
        rock.Visible = false;
        kit.Parent.AddChild(rock);
        var meteor = new Meteor { Rock = rock };
        _meteors.Add(meteor);
        return meteor;
    }

    private Heal MakeHeal()
    {
        var node = new Node3D { Visible = false };
        var material = FxKit.Shader("mote");
        var bandMaterial = FxKit.Shader("rise");
        var band = FxKit.Mesh(_bandMesh, bandMaterial);
        node.AddChild(band);
        var motes = new MeshInstance3D[HealMotes];
        for (var i = 0; i < HealMotes; i++)
        {
            motes[i] = FxKit.Mesh(kit.Quad, material);
            node.AddChild(motes[i]);
        }
        kit.Parent.AddChild(node);
        var heal = new Heal { Node = node, Band = band, Motes = motes, Material = material, BandMaterial = bandMaterial, Color = Colors.Transparent };
        _heals.Add(heal);
        return heal;
    }

    private Bubble MakeBubble()
    {
        var material = FxKit.Shader("barrier");
        var shell = FxKit.Mesh(_bubbleMesh, material);
        shell.Visible = false;
        kit.Parent.AddChild(shell);
        var bubble = new Bubble { Shell = shell, Material = material, Color = Colors.Transparent };
        _bubbles.Add(bubble);
        return bubble;
    }
}
