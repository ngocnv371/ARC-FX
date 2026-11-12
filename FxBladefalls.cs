using Godot;

namespace Arc.Fx;

/// <summary>
/// Bladefall: a volley of spectral swords (fx_spectral, so drawn only as light) raining point
/// first onto a circle of ground. Each falls out of the sky along a shared slant, spread over
/// <see cref="FxTiming.BladefallSpread"/>, buries its tip with a flash, a shiver and a small glow
/// on the ground, then disintegrates where it stands: it crumbles away in grains from the hilt
/// down along glowing edges (fx_spectral's uDissolve), shedding specks of light that drift off.
/// </summary>
internal sealed class FxBladefalls(FxKit kit, FxWorld world)
{
    /// <summary>The most blades one fall shows.</summary>
    public const int MaxBlades = 48;
    /// <summary>How far along its slant a blade falls, and the way the whole volley slants (from where it comes).</summary>
    private const float DropHeight = 8f;
    private static readonly Vector3 Slant = new Vector3(0.22f, 1f, 0.12f).Normalized();
    /// <summary>A blade's length in metres, and how much of it buries into the ground.</summary>
    private const float BladeLength = 1.35f;
    private const float Bury = 0.18f;
    /// <summary>How long a blade stands whole once it lands, then how long it takes to crumble away.</summary>
    private const float Stand = 0.12f;
    private const float Crumble = 0.5f;
    /// <summary>How many specks of light each blade sheds as it crumbles, and how long each drifts.</summary>
    private const int Specks = 6;
    private const float SpeckLife = 0.4f;
    /// <summary>How long after landing a blade's last speck is gone.</summary>
    private const float Linger = Stand + Crumble * 0.7f + SpeckLife;
    public static readonly Color BladeColor = new(0.62f, 0.85f, 1f);

    private sealed class Blade
    {
        public required Node3D Node;
        public required GeometryInstance3D[] Parts;
        public required MeshInstance3D[] Specks;
        public readonly Vector3[] Drift = new Vector3[FxBladefalls.Specks];
        public Vector3 Land;
        public Vector3 From;
        public Basis Basis;
        public float Delay;
        public float Size;
        public float Shiver;
        public bool Landed;
    }

    private sealed class Fall : FxItem
    {
        public required Blade[] Blades;
        public required ShaderMaterial Material;
        public int Count;
        public Color Color;
    }

    private readonly List<Fall> _falls = [];
    private readonly ArrayMesh _swordMesh = FxProps.Sword();
    private readonly BoxMesh _speckMesh = new() { Size = Vector3.One * 0.06f };

    public FxHandle Spawn(Vector3 centre, float radius, Color color)
    {
        var fall = _falls.Find(f => !f.Active) ?? Make();
        fall.Begin(FxTiming.BladefallSpread + FxTiming.BladefallDrop + Linger);
        if (fall.Color != color)
        {
            fall.Color = color;
            fall.Material.SetShaderParameter("uColor", FxKit.Rgb(color));
        }
        centre = FxKit.Flat(centre);
        fall.Count = Mathf.Clamp(Mathf.RoundToInt(radius * radius * 2.2f), 6, MaxBlades);
        // Spread over the circle as a sunflower (even however many), jittered so it reads as a
        // volley rather than a pattern; the order they land in is shuffled across it.
        var turn = GD.Randf() * Mathf.Tau;
        var size = Mathf.Clamp(radius * 0.32f, 0.8f, 1.3f);
        for (var i = 0; i < fall.Count; i++)
        {
            var blade = fall.Blades[i];
            var r = radius * Mathf.Sqrt((i + 0.5f) / fall.Count) * (float)GD.RandRange(0.85, 1.05);
            var way = Vector3.Forward.Rotated(Vector3.Up, turn + i * 2.39996f + (float)GD.RandRange(-0.25, 0.25));
            blade.Land = centre + way * r;
            var slant = (Slant + FxKit.Jitter(0.08f)).Normalized();
            blade.From = blade.Land + slant * DropHeight;
            blade.Size = size * BladeLength * (float)GD.RandRange(0.85, 1.1);
            blade.Delay = GD.Randf() * FxTiming.BladefallSpread;
            blade.Shiver = GD.Randf() * Mathf.Tau;
            blade.Landed = false;
            // Hilt up the slant, tip leading; each turned its own way about its length.
            var x = Vector3.Right.Rotated(Vector3.Up, GD.Randf() * Mathf.Tau);
            x = (x - slant * x.Dot(slant)).Normalized();
            blade.Basis = new Basis(x, slant, x.Cross(slant));
            for (var j = 0; j < Specks; j++)
                blade.Drift[j] = Vector3.Up * (float)GD.RandRange(0.3, 0.7) + FxKit.Jitter(0.25f);
            Hide(blade);
        }
        for (var i = fall.Count; i < MaxBlades; i++)
            Hide(fall.Blades[i]);
        Tick(fall, 0f);
        return new FxHandle(fall);
    }

    public void Tick(float dt)
    {
        foreach (var fall in _falls)
        {
            if (fall.Active)
                Tick(fall, dt);
        }
    }

    private void Tick(Fall fall, float dt)
    {
        fall.Age += dt;
        if (fall.Age >= fall.Life)
        {
            fall.Active = false;
            foreach (var blade in fall.Blades)
                Hide(blade);
            return;
        }
        var lit = 0f;
        var litAt = Vector3.Zero;
        var landed = 0;
        for (var i = 0; i < fall.Count; i++)
        {
            var blade = fall.Blades[i];
            var t = fall.Age - blade.Delay;
            if (t < 0f || t >= FxTiming.BladefallDrop + Linger)
            {
                Hide(blade);
                continue;
            }
            float alpha, impact = 0f, crumble = 0f;
            var basis = blade.Basis;
            Vector3 at;
            Vector3 scale;
            if (t < FxTiming.BladefallDrop)
            {
                // Gathering speed all the way down, drawn out a little by its speed.
                var k = t / FxTiming.BladefallDrop;
                at = blade.From.Lerp(blade.Land, k * k) + Vector3.Down * Bury * blade.Size * k * k;
                alpha = Mathf.Min(1f, t / 0.06f);
                scale = new Vector3(0.9f, 1f + 0.35f * k, 0.9f);
            }
            else
            {
                var after = t - FxTiming.BladefallDrop;
                if (!blade.Landed)
                {
                    blade.Landed = true;
                    world.Patch(blade.Land, 0.28f * blade.Size, Stand + Crumble, fall.Color);
                }
                // A flash and a shiver as it bites, settled before it crumbles: the crumbling's
                // pattern is fixed in the world, so the blade holds still from then on.
                impact = Mathf.Max(0f, 1f - after / 0.15f);
                var shiver = Mathf.Sin(after * 70f + blade.Shiver) * 0.1f * Mathf.Exp(-after * 14f) * Mathf.Max(0f, 1f - after / Stand);
                basis = new Basis(basis.X, shiver) * basis;
                crumble = Mathf.Clamp((after - Stand) / Crumble, 0f, 1f);
                at = blade.Land + Vector3.Down * Bury * blade.Size;
                alpha = 1f;
                scale = Vector3.One;
                Shed(blade, after - Stand);
                lit += impact + (1f - crumble) * 0.3f;
                litAt += blade.Land;
                landed++;
            }
            blade.Node.Visible = crumble < 1f;
            blade.Node.Transform = new Transform3D(basis * Basis.FromScale(scale * blade.Size), at);
            foreach (var part in blade.Parts)
            {
                part.SetInstanceShaderParameter("uAlpha", alpha);
                part.SetInstanceShaderParameter("uImpact", impact);
                part.SetInstanceShaderParameter("uDissolve", crumble);
            }
        }
        if (landed > 0)
            Fx.Light(litAt / landed + Vector3.Up * 0.5f, 3f, fall.Color, 0.05f * lit);
    }

    /// <summary>
    /// The specks a blade sheds <paramref name="since"/> seconds into crumbling: each leaves a
    /// point along it as the crumbling reaches there (the hilt's first), drifting up and off
    /// as it shrinks and fades.
    /// </summary>
    private static void Shed(Blade blade, float since)
    {
        for (var j = 0; j < Specks; j++)
        {
            var speck = blade.Specks[j];
            var start = Crumble * 0.7f * j / Specks;
            var k = (since - start) / SpeckLife;
            if (k < 0f || k >= 1f)
            {
                speck.Visible = false;
                continue;
            }
            var along = 1f - 0.85f * (j + 0.5f) / Specks;
            var from = blade.Land + blade.Basis.Y * blade.Size * along + Vector3.Down * Bury * blade.Size;
            speck.Visible = true;
            speck.Position = from + blade.Drift[j] * (1f - (1f - k) * (1f - k));
            speck.Scale = Vector3.One * (1f - k * 0.7f);
            speck.SetInstanceShaderParameter("uAlpha", 1f - k);
        }
    }

    private static void Hide(Blade blade)
    {
        blade.Node.Visible = false;
        foreach (var speck in blade.Specks)
            speck.Visible = false;
    }

    private Fall Make()
    {
        var material = FxKit.Spectral(BladeColor);
        material.SetShaderParameter("uGain", 1.3f);
        material.SetShaderParameter("uRunes", 1.8f);
        var blades = new Blade[MaxBlades];
        for (var i = 0; i < MaxBlades; i++)
        {
            var node = new Node3D { Visible = false };
            node.AddChild(new MeshInstance3D { Mesh = _swordMesh });
            var parts = FxKit.Paint(node, material);
            kit.Parent.AddChild(node);
            var specks = new MeshInstance3D[Specks];
            for (var j = 0; j < Specks; j++)
            {
                specks[j] = FxKit.Mesh(_speckMesh, material);
                specks[j].Visible = false;
                specks[j].SetInstanceShaderParameter("uImpact", 1f);
                kit.Parent.AddChild(specks[j]);
            }
            blades[i] = new Blade { Node = node, Parts = parts, Specks = specks, Basis = Basis.Identity };
        }
        var fall = new Fall { Blades = blades, Material = material, Color = BladeColor };
        _falls.Add(fall);
        return fall;
    }
}
