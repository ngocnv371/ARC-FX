using Godot;

namespace Arc.Fx;

/// <summary>
/// Meshes, materials and helpers the effects share. One per <see cref="FxWorld"/>, not static:
/// static Godot objects outlive the tree and show as leaks on exit.
/// </summary>
internal sealed class FxKit(Node3D parent)
{
    /// <summary>
    /// How far a bolt's blue sheath reaches past its white core on each side: about a pixel at
    /// 1080p from the default camera distance.
    /// </summary>
    public const float SheathRim = 0.012f;
    public const float GroundLift = 0.05f;

    public Node3D Parent { get; } = parent;
    public readonly BoxMesh Strand = new() { Size = Vector3.One };
    public readonly QuadMesh Quad = new() { Size = Vector2.One };
    public readonly PlaneMesh Ground = new() { Size = new Vector2(2f, 2f) };
    public readonly SphereMesh Orb = new() { Radius = 1f, Height = 2f, RadialSegments = 16, Rings = 8 };
    public readonly SphereMesh Globe = new() { Radius = 1f, Height = 2f, RadialSegments = 32, Rings = 16 };
    public readonly StandardMaterial3D Bolt = Glow(new Color(0.95f, 0.97f, 1f));
    public readonly StandardMaterial3D Sheath = Glow(new Color(0.1f, 0.4f, 1f));

    public static Vector3 Rgb(Color c) => new(c.R, c.G, c.B);

    public static Vector3 Flat(Vector3 v) => v with { Y = 0f };

    /// <summary>An unlit additive glow in one colour (alpha dims it).</summary>
    public static StandardMaterial3D Glow(Color color) => new()
    {
        ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
        Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
        BlendMode = BaseMaterial3D.BlendModeEnum.Add,
        AlbedoColor = color,
    };

    public static ShaderMaterial Shader(string name) => new() { Shader = FxShaders.Get(name) };

    /// <summary>A ground material (fx_ground) in a style. Drawn before every other effect: its soot darkens what is already there.</summary>
    public static ShaderMaterial GroundMaterial(int style, Color color)
    {
        var material = new ShaderMaterial { Shader = FxShaders.Get("ground"), RenderPriority = -1 };
        material.SetShaderParameter("uStyle", style);
        material.SetShaderParameter("uColor", Rgb(color));
        return material;
    }

    /// <summary>A hidden ground plane under the parent.</summary>
    public MeshInstance3D Plane(ShaderMaterial material)
    {
        var node = new MeshInstance3D { Mesh = Ground, MaterialOverride = material, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off, Visible = false };
        Parent.AddChild(node);
        return node;
    }

    public static MeshInstance3D Mesh(Mesh mesh, Material material) =>
        new() { Mesh = mesh, MaterialOverride = material, CastShadow = GeometryInstance3D.ShadowCastingSetting.Off };

    /// <summary>A bolt piece: a white core box wrapped in a blue sheath (its child). Lay it with <see cref="Lay"/>.</summary>
    public MeshInstance3D BoltPiece()
    {
        var core = Mesh(Strand, Bolt);
        core.AddChild(Mesh(Strand, Sheath));
        return core;
    }

    /// <summary>Stretches one bolt piece (a thin box and its sheath) from a to b in its parent's space.</summary>
    public static void Lay(MeshInstance3D strand, Vector3 a, Vector3 b, float width, float rim = SheathRim)
    {
        var along = b - a;
        var length = along.Length();
        var basis = length > 1e-3f
            ? Basis.LookingAt(along, Mathf.Abs(along.Y / length) > 0.99f ? Vector3.Right : Vector3.Up)
            : Basis.Identity;
        strand.Transform = new Transform3D(basis * Basis.FromScale(new Vector3(width, width, length)), (a + b) * 0.5f);
        var sheath = 1f + 2f * rim / width;
        strand.GetChild<MeshInstance3D>(0).Scale = new Vector3(sheath, sheath, 1f);
    }

    public static Vector3 Jitter(float size) =>
        new Vector3((float)GD.RandRange(-1.0, 1.0), (float)GD.RandRange(-1.0, 1.0), (float)GD.RandRange(-1.0, 1.0)) * size;

    public static Vector3 RandomDirection() =>
        new Vector3((float)GD.RandRange(-1.0, 1.0), (float)GD.RandRange(-1.0, 1.0), (float)GD.RandRange(-1.0, 1.0)).Normalized();

    /// <summary>A steady pseudo-random 0..1 for a number, so a flame keeps its looks between reshapes.</summary>
    public static float Hash(float n) => Mathf.PosMod(Mathf.Sin(n) * 43758.547f, 1f);

    /// <summary>The fx_crystal material: cut facets in <paramref name="color"/> with an <paramref name="accent"/> rim and glints.</summary>
    public static ShaderMaterial Crystal(Color color, Color accent, float rim = 0.6f, float glow = 0.1f)
    {
        var material = Shader("crystal");
        material.SetShaderParameter("uBase", Rgb(color));
        material.SetShaderParameter("uAccent", Rgb(accent));
        material.SetShaderParameter("uRim", rim);
        material.SetShaderParameter("uGlow", glow);
        return material;
    }

    /// <summary>The fx_spectral material: a thing drawn only as light, faded with the instance uniform uAlpha.</summary>
    public static ShaderMaterial Spectral(Color color)
    {
        var material = Shader("spectral");
        material.SetShaderParameter("uColor", Rgb(color));
        return material;
    }

    /// <summary>Draws every mesh under <paramref name="root"/> with <paramref name="material"/>; returns them.</summary>
    public static GeometryInstance3D[] Paint(Node3D root, ShaderMaterial material)
    {
        var parts = root.FindChildren("*", nameof(MeshInstance3D), true, false).Cast<GeometryInstance3D>().ToArray();
        foreach (var part in parts)
        {
            part.MaterialOverride = material;
            part.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        }
        return parts;
    }

    /// <summary>The box round every mesh under <paramref name="root"/>, in its space.</summary>
    public static Aabb Bounds(Node3D root)
    {
        var box = new Aabb();
        var first = true;
        foreach (var mesh in root.FindChildren("*", nameof(MeshInstance3D), true, false).Cast<MeshInstance3D>())
        {
            if (mesh.Mesh == null)
                continue;
            var b = ToRoot(mesh, root) * mesh.Mesh.GetAabb();
            box = first ? b : box.Merge(b);
            first = false;
        }
        return box;
    }

    private static Transform3D ToRoot(Node node, Node3D root)
    {
        var t = Transform3D.Identity;
        for (Node? n = node; n != null && n != root; n = n.GetParent())
        {
            if (n is Node3D n3)
                t = n3.Transform * t;
        }
        return t;
    }

    /// <summary>A mesh under a plain node, as the model helpers expect (they measure what is under the root).</summary>
    public static Node3D Wrap(MeshInstance3D mesh)
    {
        var node = new Node3D();
        node.AddChild(mesh);
        return node;
    }

    /// <summary>
    /// <paramref name="model"/> stood on its base at the origin and scaled to a metre tall (its
    /// tallest side up); <paramref name="width"/> above 0 also squeezes it to that width.
    /// </summary>
    public static Node3D Upright(Node3D model, float width)
    {
        var holder = new Node3D();
        var bounds = Bounds(model);
        var size = bounds.Size;
        var tall = new[] { 0, 1, 2 }.OrderByDescending(i => size[i]).First();
        var basis = tall switch
        {
            0 => new Basis(Vector3.Back, Mathf.Pi / 2f),
            2 => new Basis(Vector3.Right, -Mathf.Pi / 2f),
            _ => Basis.Identity,
        };
        var scale = 1f / Mathf.Max(size[tall], 1e-4f);
        var squeeze = width > 0f ? width / Mathf.Max(Mathf.Max(size[(tall + 1) % 3], size[(tall + 2) % 3]) * scale, 1e-4f) : 1f;
        var orient = new Node3D { Basis = Basis.FromScale(new Vector3(squeeze, 1f, squeeze)) * basis.Scaled(Vector3.One * scale) };
        model.Position = -bounds.GetCenter();
        orient.Position = Vector3.Up * 0.5f;
        orient.AddChild(model);
        holder.AddChild(orient);
        return holder;
    }

    /// <summary><paramref name="model"/> centred, its tallest side up and its thinnest along Z, <paramref name="height"/> metres tall.</summary>
    public static Node3D Facing(Node3D model, float height)
    {
        var bounds = Bounds(model);
        var size = bounds.Size;
        var order = new[] { 0, 1, 2 }.OrderByDescending(i => size[i]).ToArray();
        var columns = new Vector3[3];
        columns[order[0]] = Vector3.Up;
        columns[order[1]] = Vector3.Right;
        columns[order[2]] = Vector3.Back;
        var basis = new Basis(columns[0], columns[1], columns[2]);
        if (basis.Determinant() < 0f)
        {
            columns[order[2]] = Vector3.Forward;
            basis = new Basis(columns[0], columns[1], columns[2]);
        }
        var orient = new Node3D { Basis = basis.Scaled(Vector3.One * (height / Mathf.Max(size[order[0]], 1e-4f))) };
        model.Position = -bounds.GetCenter();
        orient.AddChild(model);
        return orient;
    }

    /// <summary>Whether a stood-up model's widest part is in its lower half (a hammer upside down).</summary>
    public static bool HeadLow(Node3D root)
    {
        float low = 0f, high = 0f;
        foreach (var mesh in root.FindChildren("*", nameof(MeshInstance3D), true, false).Cast<MeshInstance3D>())
        {
            var t = ToRoot(mesh, root);
            foreach (var v in mesh.Mesh.GetFaces())
            {
                var p = t * v;
                var spread = new Vector2(p.X, p.Z).Length();
                if (p.Y < 0.5f)
                    low = Mathf.Max(low, spread);
                else
                    high = Mathf.Max(high, spread);
            }
        }
        return low > high;
    }

    /// <summary>A light pool from an OmniLight3D's settings: the unlit shaders cannot be lit by the light itself.</summary>
    public static void Pool(OmniLight3D light, float scale) =>
        Fx.Light(light.GlobalPosition, light.OmniRange * 1.3f, light.LightColor, light.LightEnergy * scale);
}

/// <summary>Something pooled that a handle can point at: alive while active and of the same generation.</summary>
internal abstract class FxItem
{
    public bool Active;
    public int Generation;
    public float Age;
    public float Life;

    /// <summary>Marks it in use with a fresh generation, so handles to its last use go inert.</summary>
    public void Begin(float life)
    {
        Active = true;
        Generation++;
        Age = 0f;
        Life = life;
    }

    /// <summary>Called by <see cref="FxHandle.Stop"/>; by default it ends now.</summary>
    public virtual void Stop() => Life = Age;
}

/// <summary>
/// A handle on an effect that is still showing: keep it longer, or stop it. Does nothing once
/// the effect has gone, even if its pooled slot was reused.
/// </summary>
public readonly struct FxHandle
{
    private readonly FxItem? _item;
    private readonly int _generation;

    internal FxHandle(FxItem item)
    {
        _item = item;
        _generation = item.Generation;
    }

    internal FxItem? Live => _item is { Active: true } item && item.Generation == _generation ? item : null;

    /// <summary>Still on screen.</summary>
    public bool Alive => Live != null;

    /// <summary>How long it has been showing.</summary>
    public float Age => Live?.Age ?? 0f;

    /// <summary>Shows for <paramref name="seconds"/> more from now.</summary>
    public void Keep(float seconds)
    {
        if (Live is { } item)
            item.Life = item.Age + seconds;
    }

    /// <summary>Ends it (most effects vanish; a few fade out).</summary>
    public void Stop() => Live?.Stop();
}
