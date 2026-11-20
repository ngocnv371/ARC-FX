using Godot;

namespace Arc.Fx;

/// <summary>Columns of light from the sky down to a spot (fx_beam), each lighting the ground round its foot.</summary>
internal sealed class FxBeams(FxKit kit)
{
    private const float Height = 30f;

    private sealed class Beam : FxItem
    {
        public required MeshInstance3D Node;
        public required ShaderMaterial Material;
        public Color Color;
    }

    private readonly List<Beam> _beams = [];
    private readonly CylinderMesh _mesh = new() { TopRadius = 1f, BottomRadius = 1f, Height = 1f, RadialSegments = 24, Rings = 1, CapTop = false, CapBottom = false };

    public FxHandle Spawn(Vector3 at, float radius, float life, Color color)
    {
        var material = FxKit.Shader("beam");
        material.SetShaderParameter("uColor", FxKit.Rgb(color));
        material.SetShaderParameter("uLife", life);
        var node = new MeshInstance3D
        {
            Mesh = _mesh,
            MaterialOverride = material,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            Position = at with { Y = Height / 2f },
            Scale = new Vector3(radius, Height, radius),
        };
        kit.Parent.AddChild(node);
        var beam = new Beam { Node = node, Material = material, Color = color };
        beam.Begin(life);
        _beams.Add(beam);
        return new FxHandle(beam);
    }

    public void Tick(float dt)
    {
        for (var i = _beams.Count - 1; i >= 0; i--)
        {
            var beam = _beams[i];
            beam.Age += dt;
            if (beam.Age >= beam.Life)
            {
                beam.Active = false;
                beam.Node.QueueFree();
                _beams.RemoveAt(i);
                continue;
            }
            beam.Material.SetShaderParameter("uAge", beam.Age);
            var shine = Mathf.Min(beam.Age / 0.1f, 1f) * (1f - beam.Age / beam.Life);
            Fx.Light(beam.Node.Position with { Y = 1.2f }, beam.Node.Scale.X * 3f + 2f, beam.Color, 1.3f * shine);
        }
    }
}
