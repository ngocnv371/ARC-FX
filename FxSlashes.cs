using Godot;

namespace Arc.Fx;

/// <summary>
/// Streaks in the air (fx_slash): a fan from a small hub out to a radius, swept right to left
/// the way a right-handed swing travels, at chest height. One can ride on a node, be rolled
/// about its forward (a diagonal cut, or an upright one) and be swept the other way.
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

    public FxHandle Spawn(Vector3 centre, Vector3 forward, float radius, float arc, Color color, Node3D? follow, float tilt = 0f, bool reverse = false)
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
        // Turned to the aim, rolled about it, mirrored for a swing the other way, then sized.
        var basis = new Basis(Vector3.Up, Mathf.Atan2(forward.X, forward.Z)) * new Basis(Vector3.Back, Mathf.DegToRad(tilt))
            * Basis.FromScale(new Vector3(reverse ? -radius : radius, radius, radius));
        slash.Node.Transform = new Transform3D(basis, centre with { Y = Height });
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

/// <summary>
/// Thrusts (fx_thrust): a lance of light stabbing out from the hand at chest height, two crossed
/// quads so it reads from any side. One can ride on a node.
/// </summary>
internal sealed class FxThrusts(FxKit kit)
{
    public const float Life = 0.22f;
    private const float Height = 0.9f;

    private sealed class Thrust : FxItem
    {
        public required MeshInstance3D Node;
        public required ShaderMaterial Material;
        public Node3D? Follow;
    }

    private readonly List<Thrust> _pool = [];
    private ArrayMesh? _lance;

    public FxHandle Spawn(Vector3 at, Vector3 forward, float length, float width, Color color, Node3D? follow)
    {
        forward = FxKit.Flat(forward).Normalized();
        var thrust = _pool.Find(x => !x.Active);
        if (thrust == null)
        {
            var material = FxKit.Shader("thrust");
            material.SetShaderParameter("uLife", Life);
            thrust = new Thrust
            {
                Node = new MeshInstance3D { Mesh = _lance ??= Lance(), MaterialOverride = material, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off },
                Material = material,
            };
            kit.Parent.AddChild(thrust.Node);
            _pool.Add(thrust);
        }
        thrust.Begin(Life);
        thrust.Follow = follow;
        thrust.Node.Transform = new Transform3D(
            new Basis(Vector3.Up, Mathf.Atan2(forward.X, forward.Z)) * Basis.FromScale(new Vector3(width, width, length)),
            at with { Y = Height });
        thrust.Material.SetShaderParameter("uColor", FxKit.Rgb(color));
        thrust.Material.SetShaderParameter("uAge", 0f);
        thrust.Node.Visible = true;
        return new FxHandle(thrust);
    }

    public void Tick(float dt)
    {
        foreach (var thrust in _pool)
        {
            if (!thrust.Active)
                continue;
            thrust.Age += dt;
            if (thrust.Age >= thrust.Life)
            {
                thrust.Active = false;
                thrust.Node.Visible = false;
                continue;
            }
            if (thrust.Follow != null && GodotObject.IsInstanceValid(thrust.Follow))
                thrust.Node.Position = thrust.Follow.Position with { Y = Height };
            thrust.Material.SetShaderParameter("uAge", thrust.Age);
        }
    }

    /// <summary>
    /// Two quads crossed along +Z, one lying flat and one upright, each 1 across and 1 long:
    /// UV.x runs across and UV.y from the hand (0) to the reach (1).
    /// </summary>
    private static ArrayMesh Lance()
    {
        var vertices = new List<Vector3>();
        var uvs = new List<Vector2>();
        var indices = new List<int>();
        foreach (var across in new[] { Vector3.Right, Vector3.Up })
        {
            var v = vertices.Count;
            vertices.AddRange([across * -0.5f, across * 0.5f, across * -0.5f + Vector3.Back, across * 0.5f + Vector3.Back]);
            uvs.AddRange([new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f)]);
            indices.AddRange([v, v + 1, v + 2, v + 1, v + 3, v + 2]);
        }
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
        arrays[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
        arrays[(int)Mesh.ArrayType.Index] = indices.ToArray();
        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        return mesh;
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
