using Godot;

namespace Arc.Fx;

/// <summary>
/// A Divine Ire being cast. Until it fires (<see cref="FxTiming.IreWindup"/> after it starts),
/// the game keeps it on the caster and turned toward its aim with <see cref="Aim"/>; after
/// that it ignores aiming and fires along the last line it was given.
/// </summary>
public readonly struct FxIreCast
{
    private readonly FxHandle _handle;

    internal FxIreCast(FxHandle handle) => _handle = handle;

    public bool Alive => _handle.Alive;

    /// <summary>Holds the gathering light in front of <paramref name="caster"/>, aimed along <paramref name="direction"/>.</summary>
    public void Aim(Vector3 caster, Vector3 direction)
    {
        if (_handle.Live is FxIres.Ire { Fired: false } ire)
        {
            ire.Caster = caster;
            ire.Direction = FxKit.Flat(direction).Normalized();
        }
    }
}

/// <summary>
/// Divine Ire: golden light swells in the caster's hands, then a huge beam (fx_ire) races out
/// along the aim and lingers a moment, thinning as it fades.
/// </summary>
internal sealed class FxIres(FxKit kit)
{
    /// <summary>How high the beam runs.</summary>
    private const float Height = 1.1f;
    public static readonly Color IreColor = new(1f, 0.66f, 0.22f);

    internal sealed class Ire : FxItem
    {
        public required MeshInstance3D Ray;
        public required ShaderMaterial Material;
        public required Node3D Gather;
        public required MeshInstance3D Core;
        public required MeshInstance3D Halo;
        public required OmniLight3D Light;
        public Vector3 Caster;
        public Vector3 Direction;
        public float Length;
        public bool Fired;
    }

    private readonly List<Ire> _pool = [];
    private readonly QuadMesh _mesh = new() { Size = Vector2.One };
    private readonly StandardMaterial3D _coreMaterial = FxKit.Glow(new Color(1f, 0.96f, 0.8f));
    private readonly StandardMaterial3D _haloMaterial = FxKit.Glow(new Color(1f, 0.6f, 0.15f, 0.16f));

    public FxIreCast Spawn(Vector3 caster, Vector3 direction, float length, float width)
    {
        var ire = _pool.Find(i => !i.Active) ?? Make();
        ire.Begin(FxTiming.IreWindup + FxTiming.IreLinger);
        ire.Fired = false;
        ire.Caster = caster;
        ire.Direction = FxKit.Flat(direction).Normalized();
        ire.Length = length;
        ire.Material.SetShaderParameter("uLength", length);
        ire.Material.SetShaderParameter("uWidth", width);
        ire.Ray.CustomAabb = new Aabb(new Vector3(-length, -2f, -length), new Vector3(length * 2f, 4f, length * 2f));
        ire.Ray.Visible = ire.Gather.Visible = true;
        Tick(ire, 0f);
        return new FxIreCast(new FxHandle(ire));
    }

    public void Tick(float dt)
    {
        foreach (var ire in _pool)
        {
            if (ire.Active)
                Tick(ire, dt);
        }
    }

    private static void Tick(Ire ire, float dt)
    {
        ire.Age += dt;
        if (ire.Age >= ire.Life)
        {
            ire.Active = false;
            ire.Ray.Visible = ire.Gather.Visible = false;
            return;
        }
        ire.Material.SetShaderParameter("uAge", ire.Age);
        if (ire.Fired)
        {
            // The light rides down the beam and dies with it.
            var left = 1f - (ire.Age - FxTiming.IreWindup) / FxTiming.IreLinger;
            ire.Light.LightEnergy = 6f * left * left;
            FxKit.Pool(ire.Light, 0.25f);
            var flare = Mathf.Max(0f, 1f - (ire.Age - FxTiming.IreWindup) / 0.15f);
            ire.Core.Scale = Vector3.One * 0.35f * flare;
            ire.Halo.Scale = Vector3.One * 1.2f * flare;
            return;
        }

        var from = FxKit.Flat(ire.Caster) + ire.Direction * FxTiming.IreLead + Vector3.Up * Height;
        ire.Ray.Position = from;
        ire.Gather.Position = from;
        ire.Material.SetShaderParameter("uDir", ire.Direction);

        var charge = ire.Age / FxTiming.IreWindup;
        var size = Mathf.Lerp(0.1f, 0.35f, charge * charge);
        ire.Core.Scale = Vector3.One * size * (float)GD.RandRange(0.85, 1.1);
        ire.Halo.Scale = Vector3.One * size * (float)GD.RandRange(1.9, 2.5);
        ire.Light.Position = Vector3.Zero;
        ire.Light.OmniRange = 4f;
        ire.Light.LightEnergy = Mathf.Lerp(0.5f, 3f, charge) * (float)GD.RandRange(0.8, 1.2);
        FxKit.Pool(ire.Light, 0.25f);
        if (ire.Age < FxTiming.IreWindup)
            return;
        ire.Fired = true;
        ire.Light.Position = ire.Direction * ire.Length * 0.4f;
        ire.Light.OmniRange = ire.Length * 0.6f;
    }

    private Ire Make()
    {
        var material = FxKit.Shader("ire");
        material.SetShaderParameter("uColor", FxKit.Rgb(IreColor));
        material.SetShaderParameter("uWindup", FxTiming.IreWindup);
        material.SetShaderParameter("uLife", FxTiming.IreLinger);
        var ray = FxKit.Mesh(_mesh, material);
        var gather = new Node3D();
        var core = FxKit.Mesh(kit.Orb, _coreMaterial);
        var halo = FxKit.Mesh(kit.Orb, _haloMaterial);
        var light = new OmniLight3D { LightColor = IreColor, ShadowEnabled = false };
        gather.AddChild(halo);
        gather.AddChild(core);
        gather.AddChild(light);
        kit.Parent.AddChild(ray);
        kit.Parent.AddChild(gather);
        var ire = new Ire { Ray = ray, Material = material, Gather = gather, Core = core, Halo = halo, Light = light };
        _pool.Add(ire);
        return ire;
    }
}
