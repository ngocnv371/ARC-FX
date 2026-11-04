using Godot;

namespace Arc.Fx;

/// <summary>
/// Experience crystals built in code: cut shards with a pointed crown, a band of zig-zag
/// facets round the girdle and a longer pavilion, flat-shaded so every facet catches the
/// light on its own as the crystal spins. Local space, in metres, centred on the origin.
/// </summary>
public static class FxCrystalMesh
{
    private const float Crown = 0.21f; // tip height above the centre
    private const float Pavilion = 0.2f; // tip depth below it
    private const float Radius = 0.11f;

    /// <summary>A single shard with <paramref name="sides"/> facets round the crown.</summary>
    public static ArrayMesh Shard(int sides) => Build(st => AddShard(st, Transform3D.Identity, sides));

    /// <summary>A tall shard with three small ones leaning out from its waist.</summary>
    public static ArrayMesh Cluster() => Build(st =>
    {
        AddShard(st, new Transform3D(Basis.FromScale(new Vector3(1f, 1.15f, 1f)), Vector3.Zero), 6);
        for (var i = 0; i < 3; i++)
        {
            var angle = i * Mathf.Tau / 3f + 0.4f;
            var outward = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            // Rotating up about up × outward leans the shard's tip toward outward.
            var basis = new Basis(Vector3.Up.Cross(outward), 0.65f) * new Basis(Vector3.Up, angle) * Basis.FromScale(Vector3.One * 0.55f);
            AddShard(st, new Transform3D(basis, outward * 0.1f + Vector3.Down * 0.07f), 5);
        }
    });

    private static ArrayMesh Build(Action<SurfaceTool> add)
    {
        var st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);
        st.SetSmoothGroup(uint.MaxValue); // flat facets
        add(st);
        st.GenerateNormals();
        return st.Commit();
    }

    private static void AddShard(SurfaceTool st, Transform3D xf, int sides)
    {
        var top = xf * new Vector3(0f, Crown, 0f);
        var bottom = xf * new Vector3(0f, -Pavilion, 0f);
        // The lower girdle ring sits half a facet round from the upper one, so the band
        // between them is a ring of triangles rather than flat prism sides.
        var upper = new Vector3[sides];
        var lower = new Vector3[sides];
        for (var i = 0; i < sides; i++)
        {
            var a = i * Mathf.Tau / sides;
            var b = (i + 0.5f) * Mathf.Tau / sides;
            upper[i] = xf * new Vector3(Mathf.Cos(a) * Radius, 0.06f, Mathf.Sin(a) * Radius);
            lower[i] = xf * new Vector3(Mathf.Cos(b) * Radius * 0.92f, -0.03f, Mathf.Sin(b) * Radius * 0.92f);
        }

        for (var i = 0; i < sides; i++)
        {
            var next = (i + 1) % sides;
            var prev = (i + sides - 1) % sides;
            Tri(st, xf.Origin, top, upper[i], upper[next]);
            Tri(st, xf.Origin, upper[i], upper[next], lower[i]);
            Tri(st, xf.Origin, lower[prev], upper[i], lower[i]);
            Tri(st, xf.Origin, bottom, lower[i], lower[next]);
        }
    }

    /// <summary>One triangle, wound to face away from <paramref name="centre"/> (shards are convex).</summary>
    private static void Tri(SurfaceTool st, Vector3 centre, Vector3 a, Vector3 b, Vector3 c)
    {
        if ((b - a).Cross(c - a).Dot((a + b + c) / 3f - centre) < 0f)
            (b, c) = (c, b);
        // Godot's front faces wind clockwise as seen from outside.
        st.AddVertex(a);
        st.AddVertex(c);
        st.AddVertex(b);
    }
}
