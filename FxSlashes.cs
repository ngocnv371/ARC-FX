using Godot;

namespace Arc.Fx;

/// <summary>
/// Streaks in the air (fx_slash): a fan from a small hub out to a radius, swept right to left
/// the way a right-handed swing travels, at chest height. One can ride on a node.
/// </summary>
internal sealed class FxSlashes(FxKit kit)
{
    public const float Life = 0.3f;
    private const float Height = 0.9f;

    private sealed class Slash : FxItem
    {
        public required MeshInstance3D Node;
        public required ShaderMaterial Material;
        public Node3D? Follow;
    }

    private readonly List<Slash> _pool = [];
    private readonly Dictionary<int, ArrayMesh> _fans = [];

    public FxHandle Spawn(Vector3 centre, Vector3 forward, float radius, float arc, Color color, Node3D? follow)
    {
        forward = FxKit.Flat(forward).Normalized();
        var slash = _pool.Find(x => !x.Active);
        if (slash == null)
        {
            var material = FxKit.Shader("slash");
            material.SetShaderParameter("uLife", Life);
            slash = new Slash
            {
                Node = new MeshInstance3D { MaterialOverride = material, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off },
                Material = material,
            };
            kit.Parent.AddChild(slash.Node);
            _pool.Add(slash);
        }
        slash.Begin(Life);
        slash.Follow = follow;
        slash.Node.Mesh = Fan(Mathf.RoundToInt(Mathf.Min(arc, 360f)));
        slash.Node.Transform = new Transform3D(new Basis(Vector3.Up, Mathf.Atan2(forward.X, forward.Z)).Scaled(Vector3.One * radius),
            centre with { Y = Height });
        slash.Material.SetShaderParameter("uColor", FxKit.Rgb(color));
        slash.Material.SetShaderParameter("uAge", 0f);
        slash.Node.Visible = true;
        return new FxHandle(slash);
    }

    public void Tick(float dt)
    {
        foreach (var slash in _pool)
        {
            if (!slash.Active)
                continue;
            slash.Age += dt;
            if (slash.Age >= slash.Life)
            {
                slash.Active = false;
                slash.Node.Visible = false;
                continue;
            }
            if (slash.Follow != null && GodotObject.IsInstanceValid(slash.Follow))
                slash.Node.Position = slash.Follow.Position with { Y = Height };
            slash.Material.SetShaderParameter("uAge", slash.Age);
        }
    }

    /// <summary>
    /// A flat fan of <paramref name="degrees"/> centred on +Z with radius 1, from a small hub out:
    /// UV.x runs along the arc in the swing's direction, UV.y from the hub to the rim.
    /// </summary>
    private ArrayMesh Fan(int degrees)
    {
        if (_fans.TryGetValue(degrees, out var cached))
            return cached;
        const float Hub = 0.25f;
        var steps = Mathf.Max(6, degrees / 8);
        var vertices = new Vector3[(steps + 1) * 2];
        var uvs = new Vector2[vertices.Length];
        var indices = new int[steps * 6];
        var half = Mathf.DegToRad(degrees) * 0.5f;
        for (var k = 0; k <= steps; k++)
        {
            var t = (float)k / steps;
            // Right to left, the way a right-handed swing travels.
            var angle = half - t * half * 2f;
            var dir = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle));
            vertices[k * 2] = dir * Hub;
            vertices[k * 2 + 1] = dir;
            uvs[k * 2] = new Vector2(t, 0f);
            uvs[k * 2 + 1] = new Vector2(t, 1f);
            if (k == steps)
                continue;
            var (o, v) = (k * 6, k * 2);
            (indices[o], indices[o + 1], indices[o + 2]) = (v, v + 1, v + 2);
            (indices[o + 3], indices[o + 4], indices[o + 5]) = (v + 1, v + 3, v + 2);
        }
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = vertices;
        arrays[(int)Mesh.ArrayType.TexUV] = uvs;
        arrays[(int)Mesh.ArrayType.Index] = indices;
        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        return _fans[degrees] = mesh;
    }
}

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
