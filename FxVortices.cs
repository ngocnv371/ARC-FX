using Godot;

namespace Arc.Fx;

/// <summary>What a blade vortex's blades are made of: plain steel, or one of the elements.</summary>
public enum FxVortexLook
{
    /// <summary>Bright steel trailing pale streaks.</summary>
    Steel,
    /// <summary>Crimson blades trailing deep red streaks (the physical one).</summary>
    Blood,
    /// <summary>Charred blades glowing at the edges, trailing flickering flame.</summary>
    Fire,
    /// <summary>Ice blades trailing finely streaked cold light.</summary>
    Frost,
    /// <summary>Blades of bright blue-white light trailing streaks whose cores crackle and jump.</summary>
    Lightning,
    /// <summary>Venom-dark blades trailing deep green streaks.</summary>
    Poison,
    /// <summary>Gold blades trailing golden light.</summary>
    Holy,
    /// <summary>Pale glowing blades trailing soft, drawn-out wisps.</summary>
    Spirit,
}

/// <summary>
/// Blade vortices: small blades (steel, or of an element: see <see cref="FxVortexLook"/>)
/// whirling round someone, each on its own slightly tilted orbit at its own radius and height,
/// so together they swirl rather than turn as one ring. Each blade is a faceted leaf blade lying
/// flat (fx_crystal, so its facets glint as it turns) with a fading streak bent along its orbit
/// behind it (fx_trail, in the look's colour and style). A vortex rides
/// on a node and is renewed by calling again, which can also change how many blades it has:
/// new ones swell in, dropped ones shrink away. Once nothing renews it, the blades fly out and go.
/// </summary>
internal sealed class FxVortices(FxKit kit)
{
    /// <summary>The most blades one vortex shows.</summary>
    public const int MaxBlades = 12;
    /// <summary>How long the vortex takes to wind up, how long its blades take to fly out at the end, and how long one blade takes to come or go.</summary>
    private const float In = 0.25f;
    private const float Out = 0.3f;
    private const float BladeFade = 0.2f;
    /// <summary>A blade's length in metres, and how far round its orbit its streak reaches behind it (radians).</summary>
    private const float BladeLength = 0.5f;
    private const float TrailArc = 1.4f;
    /// <summary>Radians a second the blades whirl round.</summary>
    private const float Spin = 8.5f;
    /// <summary>
    /// How far round each blade starts from the one before: the golden angle, so however many
    /// there are they are spread round evenly, and each new one lands in the widest gap.
    /// </summary>
    private const float GoldenAngle = 2.39996f;

    /// <summary>fx_trail.gdshader's styles.</summary>
    private const int PlainTrail = 0, BloodTrail = 1, FireTrail = 2, FrostTrail = 3, LightningTrail = 4, SpiritTrail = 5;

    /// <summary>
    /// Each look's blades (fx_crystal's base and accent colours, its rim and glow) and trail
    /// (colour and fx_trail style). The trail colour also lights the ground.
    /// </summary>
    private static (Color Base, Color Accent, float Rim, float Glow, Color Trail, int Style) Look(FxVortexLook look) => look switch
    {
        FxVortexLook.Blood => (new(0.36f, 0.02f, 0.03f), new(1f, 0.25f, 0.2f), 0.9f, 0.35f, new(0.85f, 0.08f, 0.06f), BloodTrail),
        FxVortexLook.Fire => (new(0.12f, 0.05f, 0.03f), new(1f, 0.5f, 0.12f), 1f, 0.6f, FxTheme.FireLight, FireTrail),
        FxVortexLook.Frost => (new(0.55f, 0.8f, 0.95f), new(0.85f, 0.97f, 1f), 0.9f, 0.35f, FxTheme.Of(FxElement.Frost), FrostTrail),
        FxVortexLook.Lightning => (new(0.5f, 0.62f, 0.95f), new(0.8f, 0.9f, 1f), 1f, 0.6f, new(0.55f, 0.7f, 1f), LightningTrail),
        FxVortexLook.Poison => (new(0.1f, 0.2f, 0.06f), new(0.5f, 1f, 0.3f), 0.9f, 0.45f, FxTheme.Of(FxElement.Poison), BloodTrail),
        FxVortexLook.Holy => (new(0.75f, 0.55f, 0.2f), new(1f, 0.9f, 0.6f), 0.9f, 0.5f, FxTheme.Of(FxElement.Holy), PlainTrail),
        FxVortexLook.Spirit => (new(0.35f, 0.5f, 0.8f), new(0.7f, 0.88f, 1f), 1f, 0.6f, FxTheme.Of(FxElement.Spirit), SpiritTrail),
        _ => (new(0.5f, 0.55f, 0.66f), new(0.88f, 0.95f, 1f), 0.9f, 0.3f, new(0.85f, 0.92f, 1f), PlainTrail),
    };

    /// <summary>The look for an element's vortex: blood for physical.</summary>
    public static FxVortexLook LookOf(FxElement element) => element switch
    {
        FxElement.Fire => FxVortexLook.Fire,
        FxElement.Frost => FxVortexLook.Frost,
        FxElement.Lightning => FxVortexLook.Lightning,
        FxElement.Poison => FxVortexLook.Poison,
        FxElement.Holy => FxVortexLook.Holy,
        FxElement.Spirit => FxVortexLook.Spirit,
        _ => FxVortexLook.Blood,
    };

    private sealed class Blade
    {
        public required Node3D Pivot;
        public required MeshInstance3D Steel;
        public required MeshInstance3D Trail;
        /// <summary>Its orbit's share of the vortex's radius, its height, its tilt and how far round it is.</summary>
        public float Reach;
        public float Height;
        public Basis Tilt;
        public float Angle;
        public float Roll;
        /// <summary>How fully it is there, 0 to 1, easing toward whether it should be.</summary>
        public float Shown;
    }

    private sealed class Vortex : FxItem
    {
        public required Blade[] Blades;
        public Node3D? Follow;
        public int Count;
        public float Radius;
        public FxVortexLook Look;
        public Color Light;
        public bool Dressed;
    }

    private readonly List<Vortex> _vortices = [];
    private readonly Dictionary<FxVortexLook, (ShaderMaterial Blade, ShaderMaterial Trail)> _materials = [];
    private readonly ArrayMesh _bladeMesh = BladeMesh();
    private readonly ArrayMesh _trailMesh = TrailMesh(TrailArc, 24);

    public FxHandle Spawn(Node3D follow, int blades, float radius, float keep, FxVortexLook look)
    {
        var vortex = _vortices.Find(v => v.Active && v.Follow == follow);
        if (vortex == null)
        {
            vortex = _vortices.Find(v => !v.Active) ?? Make();
            vortex.Begin(keep);
            vortex.Follow = follow;
            var turn = GD.Randf() * Mathf.Tau;
            for (var i = 0; i < vortex.Blades.Length; i++)
            {
                Shuffle(vortex.Blades[i], turn + i * GoldenAngle);
                vortex.Blades[i].Shown = 0f;
            }
        }
        else if (vortex.Age > vortex.Life)
        {
            // Renewed while flying out: wind back up from where it had got to.
            vortex.Age = In * (1f - (vortex.Age - vortex.Life) / Out);
        }
        vortex.Life = vortex.Age + keep;
        vortex.Count = Mathf.Clamp(blades, 0, MaxBlades);
        vortex.Radius = radius;
        if (vortex.Look != look || !vortex.Dressed)
        {
            vortex.Look = look;
            vortex.Dressed = true;
            vortex.Light = Look(look).Trail;
            var (blade, trail) = Materials(look);
            foreach (var b in vortex.Blades)
            {
                b.Steel.MaterialOverride = blade;
                b.Trail.MaterialOverride = trail;
            }
        }
        Tick(vortex, 0f);
        return new FxHandle(vortex);
    }

    public void Tick(float dt)
    {
        foreach (var vortex in _vortices)
        {
            if (vortex.Active)
                Tick(vortex, dt);
        }
    }

    private void Tick(Vortex vortex, float dt)
    {
        vortex.Age += dt;
        var gone = vortex.Follow == null || !GodotObject.IsInstanceValid(vortex.Follow);
        var after = vortex.Age - vortex.Life;
        if (after >= Out || gone)
        {
            vortex.Active = false;
            vortex.Follow = null;
            foreach (var blade in vortex.Blades)
                blade.Pivot.Visible = false;
            return;
        }
        // Winds up from tight to its full reach, and at the end flings its blades outward.
        var wind = Mathf.Clamp(vortex.Age / In, 0f, 1f);
        var fling = after > 0f ? after / Out : 0f;
        var reach = vortex.Radius * (0.35f + 0.65f * (1f - (1f - wind) * (1f - wind))) * (1f + 0.6f * fling);
        var fade = 1f - fling;
        var centre = vortex.Follow!.Position with { Y = 0f };

        var lit = 0f;
        for (var i = 0; i < vortex.Blades.Length; i++)
        {
            var blade = vortex.Blades[i];
            // Every blade turns, shown or not, so one that comes in later arrives in its gap.
            blade.Angle += dt * Spin * (0.6f + 0.4f * wind);
            var want = i < vortex.Count ? 1f : 0f;
            blade.Shown = Mathf.MoveToward(blade.Shown, want, dt / BladeFade);
            if (blade.Shown <= 0f)
            {
                blade.Pivot.Visible = false;
                continue;
            }
            blade.Pivot.Visible = true;
            var shown = blade.Shown * fade;
            var r = reach * blade.Reach;
            blade.Pivot.Position = centre + Vector3.Up * blade.Height;
            blade.Pivot.Basis = blade.Tilt * new Basis(Vector3.Up, blade.Angle);
            // The blade lies flat on its orbit, tip forward along its flight, rolling a little.
            blade.Steel.Transform = new Transform3D(
                new Basis(Vector3.Forward, Mathf.Sin(blade.Angle * 0.5f + blade.Roll) * 0.35f).Scaled(Vector3.One * BladeLength * shown),
                new Vector3(r, 0f, 0f));
            blade.Trail.Scale = new Vector3(r, 1f, r);
            blade.Trail.SetInstanceShaderParameter("uAlpha", shown);
            lit += shown;
        }
        Fx.Light(centre + Vector3.Up * 0.9f, vortex.Radius * 1.5f, vortex.Light, 0.06f * lit);
    }

    /// <summary>
    /// A fresh orbit for a blade starting at <paramref name="angle"/>, so each vortex swirls its own
    /// way. They all turn at one speed, so they keep their spacing; the swirl comes from their
    /// differing reach, height and tilt.
    /// </summary>
    private static void Shuffle(Blade blade, float angle)
    {
        blade.Reach = (float)GD.RandRange(0.6, 1.0);
        blade.Height = (float)GD.RandRange(0.55, 1.35);
        var axis = Vector3.Right.Rotated(Vector3.Up, GD.Randf() * Mathf.Tau);
        blade.Tilt = new Basis(axis, (float)GD.RandRange(-0.25, 0.25));
        blade.Angle = angle;
        blade.Roll = GD.Randf() * Mathf.Tau;
    }

    private (ShaderMaterial Blade, ShaderMaterial Trail) Materials(FxVortexLook look)
    {
        if (_materials.TryGetValue(look, out var cached))
            return cached;
        var (colour, accent, rim, glow, trailColour, style) = Look(look);
        var trail = FxKit.Shader("trail");
        trail.SetShaderParameter("uColor", FxKit.Rgb(trailColour));
        trail.SetShaderParameter("uStyle", style);
        return _materials[look] = (FxKit.Crystal(colour, accent, rim, glow), trail);
    }

    private Vortex Make()
    {
        var blades = new Blade[MaxBlades];
        for (var i = 0; i < MaxBlades; i++)
        {
            var pivot = new Node3D { Visible = false };
            var (bladeMaterial, trailMaterial) = Materials(FxVortexLook.Steel);
            var steel = FxKit.Mesh(_bladeMesh, bladeMaterial);
            var trail = FxKit.Mesh(_trailMesh, trailMaterial);
            pivot.AddChild(steel);
            pivot.AddChild(trail);
            kit.Parent.AddChild(pivot);
            blades[i] = new Blade { Pivot = pivot, Steel = steel, Trail = trail, Tilt = Basis.Identity };
        }
        var vortex = new Vortex { Blades = blades };
        _vortices.Add(vortex);
        return vortex;
    }

    /// <summary>
    /// A leaf blade a metre long lying flat, its tip toward -Z (the way it flies): a kite outline
    /// with a ridge down its middle on both faces, flat shaded so each half catches the light
    /// on its own, and a short tang behind.
    /// </summary>
    private static ArrayMesh BladeMesh()
    {
        var st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);
        st.SetSmoothGroup(uint.MaxValue);
        var tip = new Vector3(0f, 0f, -0.5f);
        var left = new Vector3(-0.13f, 0f, 0.12f);
        var right = new Vector3(0.13f, 0f, 0.12f);
        var heel = new Vector3(0f, 0f, 0.3f);
        var up = new Vector3(0f, 0.035f, 0.02f);
        var down = new Vector3(0f, -0.035f, 0.02f);
        // Wound to face away from the centre of the piece it is on (each piece is convex);
        // Godot's front faces wind clockwise as seen from outside.
        var centre = Vector3.Zero;
        void Tri(Vector3 a, Vector3 b, Vector3 c)
        {
            if ((b - a).Cross(c - a).Dot((a + b + c) / 3f - centre) < 0f)
                (b, c) = (c, b);
            st.AddVertex(a);
            st.AddVertex(c);
            st.AddVertex(b);
        }
        foreach (var ridge in new[] { up, down })
        {
            Tri(tip, left, ridge);
            Tri(tip, ridge, right);
            Tri(left, heel, ridge);
            Tri(ridge, heel, right);
        }
        // The tang: a thin wedge behind the heel.
        var tangEnd = new Vector3(0f, 0f, 0.5f);
        var tangL = new Vector3(-0.03f, 0f, 0.3f);
        var tangR = new Vector3(0.03f, 0f, 0.3f);
        var tangUp = new Vector3(0f, 0.02f, 0.38f);
        var tangDown = new Vector3(0f, -0.02f, 0.38f);
        centre = new Vector3(0f, 0f, 0.38f);
        foreach (var ridge in new[] { tangUp, tangDown })
        {
            Tri(tangL, tangEnd, ridge);
            Tri(ridge, tangEnd, tangR);
            Tri(tangL, ridge, tangR);
        }
        st.GenerateNormals();
        return st.Commit();
    }

    /// <summary>
    /// A flat ribbon along a unit circle in the XZ plane, from <paramref name="arc"/> radians behind
    /// +X up to +X (orbits turn from +X toward -Z): UV.x 0 at its tail to 1 at its head, UV.y across.
    /// Scaled by the orbit's radius, its width scales with it.
    /// </summary>
    private static ArrayMesh TrailMesh(float arc, int steps)
    {
        const float HalfWidth = 0.09f;
        var st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);
        for (var i = 0; i < steps; i++)
        {
            float u0 = (float)i / steps, u1 = (float)(i + 1) / steps;
            // Behind the blade is the way it came: from +X back toward +Z.
            Vector3 At(float u, float side)
            {
                var a = arc * (1f - u);
                return new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * (1f + side * HalfWidth);
            }
            var a0 = At(u0, -1f);
            var b0 = At(u0, 1f);
            var a1 = At(u1, -1f);
            var b1 = At(u1, 1f);
            void V(Vector3 p, float u, float v)
            {
                st.SetUV(new Vector2(u, v));
                st.AddVertex(p);
            }
            V(a0, u0, 0f);
            V(b0, u0, 1f);
            V(a1, u1, 0f);
            V(b0, u0, 1f);
            V(b1, u1, 1f);
            V(a1, u1, 0f);
        }
        return st.Commit();
    }
}
