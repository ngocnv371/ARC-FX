using Godot;

namespace Arc.Fx;

/// <summary>
/// The props the library builds itself, each one mesh (one draw, and one set of instance
/// uniforms, apiece): Bladefall's sword, and the shield and hammer the spectral effects fall
/// back to when the game lends none (<see cref="FxModels"/>). Made of plain facets, as
/// fx_spectral draws them: their cuts and edges are what glow.
/// </summary>
internal static class FxProps
{
    /// <summary>
    /// A sword a metre long, its point on the origin and its pommel at +Y: a blade with a fuller
    /// grooved down both faces (where fx_spectral etches its runes), a guard whose quillons sweep
    /// down toward the point and end in knobs round a block set with a stone, a grip bound in
    /// raised wraps, and a faceted pommel with a cap.
    /// </summary>
    public static ArrayMesh Sword()
    {
        var st = Begin();
        SwordBlade(st);

        // The guard: a block with a stone on each face, and two quillons swept toward the point.
        const float GuardY = 0.725f, Sweep = 0.35f, Arm = 0.17f;
        Append(st, new BoxMesh { Size = new Vector3(0.08f, 0.06f, 0.07f) }, Vector3.Up * GuardY);
        var stone = new SphereMesh { Radius = 0.02f, Height = 0.04f, RadialSegments = 4, Rings = 2 };
        Append(st, stone, new Vector3(0f, GuardY, 0.036f));
        Append(st, stone, new Vector3(0f, GuardY, -0.036f));
        var quillon = new CylinderMesh { TopRadius = 0.014f, BottomRadius = 0.024f, Height = Arm, RadialSegments = 6, Rings = 1 };
        var knob = new SphereMesh { Radius = 0.024f, Height = 0.048f, RadialSegments = 6, Rings = 3 };
        foreach (var side in new[] { 1f, -1f })
        {
            var way = new Vector3(side * Mathf.Cos(Sweep), -Mathf.Sin(Sweep), 0f);
            var root = new Vector3(side * 0.035f, GuardY, 0f);
            Append(st, quillon, new Transform3D(Toward(way), root + way * Arm * 0.5f));
            Append(st, knob, root + way * (Arm + 0.01f));
        }

        // The grip, bound in raised wraps, and the pommel.
        Append(st, new CylinderMesh { TopRadius = 0.024f, BottomRadius = 0.027f, Height = 0.2f, RadialSegments = 6, Rings = 1 }, Vector3.Up * 0.855f);
        var wrap = new CylinderMesh { TopRadius = 0.03f, BottomRadius = 0.03f, Height = 0.016f, RadialSegments = 6, Rings = 1 };
        for (var i = 0; i < 5; i++)
            Append(st, wrap, Vector3.Up * (0.785f + 0.035f * i));
        Append(st, new CylinderMesh { TopRadius = 0.034f, BottomRadius = 0.028f, Height = 0.025f, RadialSegments = 6, Rings = 1 }, Vector3.Up * 0.765f);
        Append(st, new SphereMesh { Radius = 0.048f, Height = 0.08f, RadialSegments = 8, Rings = 4 }, Vector3.Up * 0.985f);
        Append(st, new CylinderMesh { TopRadius = 0f, BottomRadius = 0.022f, Height = 0.04f, RadialSegments = 6, Rings = 1 }, Vector3.Up * 1.035f);
        return End(st);
    }

    /// <summary>
    /// A heater shield a metre wide and 1.3 tall, its face toward -Z (the way a spectral shield
    /// is turned to face, as <c>LookAt</c> does): curved back across its width, with a raised rim
    /// and a groove inside it studded with rivets, a face swelling toward a domed boss, and a
    /// cross standing proud across it.
    /// </summary>
    public static ArrayMesh Shield()
    {
        const float Front = -1f;
        var st = Begin();
        var centre = new Vector2(0f, 0.02f);
        // The outline, anticlockwise seen from the front: flat across the top, straight down the
        // sides, then curving in to the point.
        var outline = new List<Vector2>();
        const int Steps = 9;
        float SideX(float y) => y > 0.1f ? 0.5f : 0.5f * Mathf.Sqrt(Mathf.Max(0f, 1f - Mathf.Pow((0.1f - y) / 0.78f, 2f)));
        for (var k = 0; k <= Steps; k++)
        {
            var y = Mathf.Lerp(0.62f, -0.68f, (float)k / Steps);
            outline.Add(new Vector2(SideX(y), y));
        }
        for (var k = Steps - 1; k >= 0; k--)
            outline.Add(new Vector2(-outline[k].X, outline[k].Y));
        // Its face bends back toward the edges.
        Vector3 At(Vector2 p, float scale, float depth)
        {
            var q = centre + (p - centre) * scale;
            return new Vector3(q.X, q.Y, Front * (depth - 0.1f * q.X * q.X));
        }
        // Rings across the face (how far out, how proud) from the edge in: the rim's crest, the
        // groove inside it, then the face swelling to the boss.
        var rings = new (float Scale, float Depth)[] { (1f, 0.035f), (0.94f, 0.055f), (0.89f, 0.035f), (0.86f, 0.02f), (0.55f, 0.045f) };
        var n = outline.Count;
        for (var r = 0; r + 1 < rings.Length; r++)
        {
            for (var i = 0; i < n; i++)
            {
                var j = (i + 1) % n;
                Vector3 a = At(outline[i], rings[r].Scale, rings[r].Depth), b = At(outline[j], rings[r].Scale, rings[r].Depth);
                Vector3 c = At(outline[i], rings[r + 1].Scale, rings[r + 1].Depth), d = At(outline[j], rings[r + 1].Scale, rings[r + 1].Depth);
                Tri(st, a, b, d, Vector3.Back * Front);
                Tri(st, a, d, c, Vector3.Back * Front);
            }
        }
        var face = new Vector3(centre.X, centre.Y, Front * 0.06f);
        var back = new Vector3(centre.X, centre.Y, -Front * 0.035f);
        for (var i = 0; i < n; i++)
        {
            var j = (i + 1) % n;
            var (last, lastDepth) = rings[^1];
            Tri(st, face, At(outline[i], last, lastDepth), At(outline[j], last, lastDepth), Vector3.Back * Front);
            // The flat back, and the edge between it and the face.
            Vector3 bi = At(outline[i], 1f, -0.035f), bj = At(outline[j], 1f, -0.035f);
            Tri(st, back, bi, bj, Vector3.Forward * Front);
            Vector3 fi = At(outline[i], 1f, 0.035f), fj = At(outline[j], 1f, 0.035f);
            var side = (outline[i] + outline[j]) * 0.5f - centre;
            var outward = new Vector3(side.X, side.Y, 0f);
            Tri(st, fi, fj, bj, outward);
            Tri(st, fi, bj, bi, outward);
        }
        // Rivets round the groove inside the rim.
        var rivet = new SphereMesh { Radius = 0.022f, Height = 0.03f, RadialSegments = 6, Rings = 2 };
        for (var i = 0; i < n; i += 2)
            Append(st, rivet, At(outline[i], 0.875f, 0.04f));
        // The cross, standing proud of the face, and the boss where its arms meet.
        Append(st, new BoxMesh { Size = new Vector3(0.08f, 0.98f, 0.035f) }, new Vector3(0f, -0.04f, Front * 0.055f));
        Append(st, new BoxMesh { Size = new Vector3(0.7f, 0.08f, 0.035f) }, new Vector3(0f, 0.2f, Front * 0.05f));
        Append(st, new SphereMesh { Radius = 0.13f, Height = 0.26f, RadialSegments = 10, Rings = 5 },
            new Transform3D(Basis.FromScale(new Vector3(1f, 1f, 0.45f)), new Vector3(0f, 0.2f, Front * 0.07f)));
        Append(st, new SphereMesh { Radius = 0.04f, Height = 0.08f, RadialSegments = 6, Rings = 3 }, new Vector3(0f, 0.2f, Front * 0.13f));
        return End(st);
    }

    /// <summary>
    /// A war maul about a metre long, the end of its haft on the origin and its head at +Y, the
    /// head lying along Z with a face at either end (a smite swings it down about X, so the one
    /// at -Z lands): a capped butt, a grip in raised wraps up to a collar, a haft
    /// strapped with langets under the head, and a studded block with a flared, banded face
    /// either side and a spike on top.
    /// </summary>
    public static ArrayMesh Hammer()
    {
        var st = Begin();
        // The butt, the haft and its grip.
        Append(st, new SphereMesh { Radius = 0.042f, Height = 0.07f, RadialSegments = 8, Rings = 4 }, Vector3.Up * 0.025f);
        Append(st, new CylinderMesh { TopRadius = 0.034f, BottomRadius = 0.04f, Height = 0.03f, RadialSegments = 8, Rings = 1 }, Vector3.Up * 0.06f);
        Append(st, new CylinderMesh { TopRadius = 0.026f, BottomRadius = 0.03f, Height = 0.84f, RadialSegments = 8, Rings = 1 }, Vector3.Up * 0.48f);
        var wrap = new CylinderMesh { TopRadius = 0.035f, BottomRadius = 0.035f, Height = 0.016f, RadialSegments = 8, Rings = 1 };
        for (var i = 0; i < 6; i++)
            Append(st, wrap, Vector3.Up * (0.1f + 0.04f * i));
        Append(st, new CylinderMesh { TopRadius = 0.036f, BottomRadius = 0.042f, Height = 0.03f, RadialSegments = 8, Rings = 1 }, Vector3.Up * 0.33f);

        // Langets strapping the haft under the head, and a ring where it enters.
        var langet = new BoxMesh { Size = new Vector3(0.012f, 0.16f, 0.05f) };
        Append(st, langet, new Vector3(0.03f, 0.72f, 0f));
        Append(st, langet, new Vector3(-0.03f, 0.72f, 0f));
        Append(st, new CylinderMesh { TopRadius = 0.05f, BottomRadius = 0.04f, Height = 0.03f, RadialSegments = 8, Rings = 1 }, Vector3.Up * 0.775f);

        // The head: a block studded on its sides, a spike on top, and a face at either end.
        const float HeadY = 0.88f, Block = 0.08f;
        Append(st, new BoxMesh { Size = new Vector3(0.17f, 0.2f, Block * 2f) }, Vector3.Up * HeadY);
        var stud = new SphereMesh { Radius = 0.02f, Height = 0.03f, RadialSegments = 6, Rings = 2 };
        foreach (var x in new[] { 0.085f, -0.085f })
        {
            foreach (var z in new[] { -0.045f, 0.045f })
            {
                foreach (var y in new[] { -0.05f, 0.05f })
                    Append(st, stud, new Vector3(x, HeadY + y, z));
            }
        }
        Append(st, new CylinderMesh { TopRadius = 0f, BottomRadius = 0.045f, Height = 0.11f, RadialSegments = 6, Rings = 1 }, Vector3.Up * (HeadY + 0.155f));
        var flare = new CylinderMesh { TopRadius = 0.105f, BottomRadius = 0.08f, Height = 0.12f, RadialSegments = 8, Rings = 1 };
        var band = new CylinderMesh { TopRadius = 0.1f, BottomRadius = 0.1f, Height = 0.022f, RadialSegments = 8, Rings = 1 };
        var plate = new CylinderMesh { TopRadius = 0.112f, BottomRadius = 0.118f, Height = 0.026f, RadialSegments = 8, Rings = 1 };
        foreach (var side in new[] { 1f, -1f })
        {
            var turn = Toward(Vector3.Back * side);
            Vector3 Out(float z) => new(0f, HeadY, side * z);
            Append(st, flare, new Transform3D(turn, Out(Block + 0.06f)));
            Append(st, band, new Transform3D(turn, Out(Block + 0.035f)));
            Append(st, plate, new Transform3D(turn, Out(Block + 0.133f)));
        }
        return End(st);
    }

    /// <summary>
    /// A clenched fist in an armoured gauntlet, about a metre across, knuckles down on the origin
    /// and the forearm rising from it along +Y, the fingers curled toward -Z (the way it faces),
    /// the thumb on the -X side. All of it is bevelled plates, packed close so it reads as one
    /// mass: each finger a plate along the bottom and one up the front, hinged between; a guard
    /// across the knuckles studded with short spikes; the back of the hand in overlapping lames;
    /// the thumb folded across the fingers; and a flared, riveted cuff. Above the cuff the forearm
    /// is plain, for fx_spectral to fade out (its uFadeFrom and uFadeTo, <see cref="FistFade"/>).
    /// </summary>
    public static ArrayMesh Fist()
    {
        var st = Begin();
        // Index to little finger: how much lower each reaches, and how big.
        var drop = new[] { 0.04f, 0f, 0f, 0.035f };
        var size = new[] { 1f, 1f, 0.96f, 0.88f };
        var spike = new CylinderMesh { TopRadius = 0f, BottomRadius = 0.04f, Height = 0.085f, RadialSegments = 4, Rings = 1 };
        for (var i = 0; i < 4; i++)
        {
            var x = -0.345f + 0.23f * i;
            var (d, k) = (drop[i], size[i]);
            // Along the bottom from the knuckle to the bend, then up the front to the curled tip,
            // with a hinge across the bend.
            Bar(st, new Vector3(x, 0.12f + d, 0.26f), new Vector3(x, 0.12f + d, -0.2f), 0.215f * k, 0.22f * k, Vector3.Right);
            Bar(st, new Vector3(x, 0.08f + d, -0.22f), new Vector3(x, 0.42f + d, -0.25f), 0.205f * k, 0.17f * k, Vector3.Right);
            Bar(st, new Vector3(x - 0.1f * k, 0.1f + d, -0.21f), new Vector3(x + 0.1f * k, 0.1f + d, -0.21f), 0.15f * k, 0.15f * k, Vector3.Up);
            // A short spike on each knuckle, leaning out of the fist's leading edge.
            Append(st, spike, new Transform3D(Toward(new Vector3(0f, -0.7f, 1f)), new Vector3(x, 0.04f + d, 0.31f)));
        }
        // The guard across the knuckles.
        Bar(st, new Vector3(-0.48f, 0.16f, 0.29f), new Vector3(0.48f, 0.16f, 0.29f), 0.17f, 0.1f, Vector3.Up);

        // The hand, the lames over its back, and the thumb folded across the fingers.
        Bar(st, new Vector3(0f, 0.3f, 0.04f), new Vector3(0f, 0.9f, 0.04f), 0.9f, 0.46f, Vector3.Right);
        Bar(st, new Vector3(0f, 0.3f, 0.26f), new Vector3(0f, 0.56f, 0.28f), 0.86f, 0.07f, Vector3.Right);
        Bar(st, new Vector3(0f, 0.52f, 0.27f), new Vector3(0f, 0.8f, 0.26f), 0.8f, 0.07f, Vector3.Right);
        var joint = new Vector3(-0.5f, 0.5f, -0.27f);
        Bar(st, new Vector3(-0.52f, 0.84f, 0.02f), joint, 0.22f, 0.2f, Vector3.Back);
        Bar(st, joint, new Vector3(-0.03f, 0.45f, -0.38f), 0.19f, 0.17f, Vector3.Up);
        Bar(st, joint + new Vector3(0f, 0f, -0.1f), joint + new Vector3(0f, 0f, 0.1f), 0.2f, 0.2f, Vector3.Up);

        // The cuff, flaring from the wrist, riveted round its rim; then the bare forearm.
        Append(st, new CylinderMesh { TopRadius = 0.5f, BottomRadius = 0.33f, Height = 0.45f, RadialSegments = 10, Rings = 1, CapBottom = false, CapTop = false }, Vector3.Up * 1.06f);
        Append(st, new CylinderMesh { TopRadius = 0.52f, BottomRadius = 0.52f, Height = 0.06f, RadialSegments = 10, Rings = 1 }, Vector3.Up * 1.27f);
        var rivet = new SphereMesh { Radius = 0.032f, Height = 0.05f, RadialSegments = 6, Rings = 2 };
        for (var r = 0; r < 10; r++)
            Append(st, rivet, new Vector3(0f, 1.22f, 0.49f).Rotated(Vector3.Up, Mathf.Tau * (r + 0.5f) / 10f));
        Append(st, new CylinderMesh { TopRadius = 0.42f, BottomRadius = 0.37f, Height = 1.5f, RadialSegments = 10, Rings = 1, CapBottom = false }, Vector3.Up * 1.95f);
        return End(st);
    }

    /// <summary>Over what height (in the fist's own metre) its forearm fades out above the cuff.</summary>
    public static readonly Vector2 FistFade = new(1.3f, 2.5f);

    /// <summary>How many looks <see cref="RockSpike"/> has.</summary>
    public const int RockSpikeLooks = 4;

    /// <summary>
    /// A jagged spike of rock a metre tall, its foot on the origin and its point up, in one of
    /// <see cref="RockSpikeLooks"/> looks (<paramref name="look"/>): a crooked, faceted column whose
    /// sides bulge and pinch unevenly and break into a ledge or two on the way up, smaller shards
    /// bursting out of its foot, and rubble round it.
    /// </summary>
    public static ArrayMesh RockSpike(int look)
    {
        var rng = new Random(look * 7919 + 17);
        float R() => (float)rng.NextDouble();
        var st = Begin();
        Crag(st, Transform3D.Identity, 6 + look % 2, R);
        // Shards breaking out of the foot, leaning away from the main spike.
        var shards = 2 + look % 2;
        var turn = R() * Mathf.Tau;
        for (var i = 0; i < shards; i++)
        {
            var angle = turn + Mathf.Tau * (i + 0.3f * (R() - 0.5f)) / shards;
            var outward = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            var lean = new Basis(Vector3.Up.Cross(outward).Normalized(), 0.45f + 0.35f * R());
            var basis = lean * new Basis(Vector3.Up, R() * Mathf.Tau) * Basis.FromScale(Vector3.One * (0.32f + 0.18f * R()));
            Crag(st, new Transform3D(basis, outward * 0.17f + Vector3.Down * 0.06f), 5, R);
        }
        // Rubble round the foot.
        var chunk = new SphereMesh { Radius = 1f, Height = 2f, RadialSegments = 5, Rings = 3 };
        for (var i = 0; i < 4; i++)
        {
            var angle = turn + Mathf.Tau * (i + 0.5f + 0.4f * (R() - 0.5f)) / 4f;
            var at = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * (0.28f + 0.12f * R());
            var size = new Vector3(0.05f + 0.04f * R(), 0.03f + 0.03f * R(), 0.05f + 0.04f * R());
            Append(st, chunk, new Transform3D(new Basis(RandomAxis(R), R() * Mathf.Tau) * Basis.FromScale(size), at));
        }
        return End(st);
    }

    /// <summary>
    /// One crooked spike of rock a metre tall on <paramref name="xf"/>: rings of
    /// <paramref name="sides"/> uneven corners narrowing up to an off-centre point, each ring's
    /// centre drifting from the last, and a ledge stepping in partway up.
    /// </summary>
    private static void Crag(SurfaceTool st, Transform3D xf, int sides, Func<float> R)
    {
        // Heights, how wide (a share of the foot) and whether the side steps in there.
        var profile = new (float Y, float Width, bool Ledge)[]
        {
            (-0.05f, 1f, false), (0.14f, 0.86f, false), (0.3f, 0.74f, true), (0.48f, 0.52f, false), (0.64f, 0.4f, R() > 0.4f), (0.82f, 0.22f, false),
        };
        const float Foot = 0.3f;
        var spin = R() * Mathf.Tau;
        var angles = new float[sides];
        for (var i = 0; i < sides; i++)
            angles[i] = spin + Mathf.Tau * (i + 0.45f * (R() - 0.5f)) / sides;
        var rings = new List<(Vector3 Centre, Vector3[] Corners)>();
        var drift = Vector3.Zero;
        foreach (var (y, width, ledge) in profile)
        {
            drift += new Vector3(R() - 0.5f, 0f, R() - 0.5f) * 0.07f;
            var centre = drift + Vector3.Up * y;
            var corners = new Vector3[sides];
            for (var i = 0; i < sides; i++)
                corners[i] = centre + new Vector3(Mathf.Cos(angles[i]), (R() - 0.5f) * 0.06f, Mathf.Sin(angles[i])) * Foot * width * (0.72f + 0.5f * R());
            rings.Add((centre, corners));
            if (ledge)
            {
                // The ledge: the side steps in at the same height, leaving a shelf.
                var inner = new Vector3[sides];
                for (var i = 0; i < sides; i++)
                    inner[i] = centre + (corners[i] - centre) * (0.72f + 0.1f * R()) + Vector3.Up * 0.015f;
                rings.Add((centre + Vector3.Up * 0.015f, inner));
            }
        }
        var tip = drift + new Vector3(R() - 0.5f, 0f, R() - 0.5f) * 0.06f + Vector3.Up;

        // Faces wound outward from the column's axis (and upward on a ledge's shelf).
        void Face(Vector3 a, Vector3 b, Vector3 c, Vector3 axis)
        {
            var mid = (a + b + c) / 3f;
            var radial = mid - axis;
            radial.Y = 0f;
            Tri(st, xf * a, xf * b, xf * c, xf.Basis * (radial + Vector3.Up * 0.3f));
        }
        for (var r = 0; r + 1 < rings.Count; r++)
        {
            var (lo, hi) = (rings[r], rings[r + 1]);
            var axis = (lo.Centre + hi.Centre) * 0.5f;
            for (var i = 0; i < sides; i++)
            {
                var j = (i + 1) % sides;
                Face(lo.Corners[i], hi.Corners[i], hi.Corners[j], axis);
                Face(lo.Corners[i], hi.Corners[j], lo.Corners[j], axis);
            }
        }
        var (topCentre, top) = rings[^1];
        var (footCentre, foot) = rings[0];
        for (var i = 0; i < sides; i++)
        {
            var j = (i + 1) % sides;
            Face(top[i], top[j], tip, topCentre);
            Tri(st, xf * foot[i], xf * foot[j], xf * footCentre, xf.Basis * Vector3.Down);
        }
    }

    /// <summary>Where a shrine's stone (<see cref="Shrine"/>) cradles its gem: the gem's centre, in metres above the foot.</summary>
    public const float ShrineCradle = 1.72f;

    /// <summary>
    /// A shrine's stone, its foot on the origin, about 1.9 metres tall: a broad eight-sided dais
    /// of two steps, a tapering pillar banded twice, a flared capital, and three prongs curling up
    /// out of its rim and in again to cradle a gem at <see cref="ShrineCradle"/>; a little rubble
    /// lies round the foot.
    /// </summary>
    public static ArrayMesh Shrine()
    {
        var st = Begin();
        // The dais: two steps.
        Append(st, new CylinderMesh { TopRadius = 0.74f, BottomRadius = 0.78f, Height = 0.12f, RadialSegments = 8, Rings = 1 }, Vector3.Up * 0.06f);
        Append(st, new CylinderMesh { TopRadius = 0.54f, BottomRadius = 0.58f, Height = 0.12f, RadialSegments = 8, Rings = 1 }, Vector3.Up * 0.18f);
        // The pillar, a plinth at its foot and two bands up it.
        Append(st, new CylinderMesh { TopRadius = 0.36f, BottomRadius = 0.4f, Height = 0.1f, RadialSegments = 8, Rings = 1 }, Vector3.Up * 0.29f);
        Append(st, new CylinderMesh { TopRadius = 0.21f, BottomRadius = 0.29f, Height = 0.86f, RadialSegments = 8, Rings = 1 }, Vector3.Up * 0.77f);
        var band = new CylinderMesh { TopRadius = 1f, BottomRadius = 1f, Height = 0.05f, RadialSegments = 8, Rings = 1 };
        Append(st, band, new Transform3D(Basis.FromScale(new Vector3(0.3f, 1f, 0.3f)), Vector3.Up * 0.5f));
        Append(st, band, new Transform3D(Basis.FromScale(new Vector3(0.25f, 1f, 0.25f)), Vector3.Up * 1.0f));
        // The capital, flaring out to a rim.
        Append(st, new CylinderMesh { TopRadius = 0.38f, BottomRadius = 0.22f, Height = 0.16f, RadialSegments = 8, Rings = 1 }, Vector3.Up * 1.28f);
        Append(st, new CylinderMesh { TopRadius = 0.34f, BottomRadius = 0.38f, Height = 0.05f, RadialSegments = 8, Rings = 1 }, Vector3.Up * 1.385f);
        // Three prongs curling up and in round the gem, each tipped with a point.
        for (var i = 0; i < 3; i++)
        {
            var angle = i * Mathf.Tau / 3f + Mathf.Pi / 6f;
            var outward = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            var across = Vector3.Up.Cross(outward);
            var root = outward * 0.3f + Vector3.Up * 1.4f;
            var elbow = outward * 0.44f + Vector3.Up * 1.68f;
            var hook = outward * 0.34f + Vector3.Up * 1.98f;
            var tip = outward * 0.2f + Vector3.Up * 2.1f;
            Bar(st, root, elbow, 0.11f, 0.08f, across);
            Bar(st, elbow, hook, 0.09f, 0.07f, across);
            Append(st, new CylinderMesh { TopRadius = 0f, BottomRadius = 0.045f, Height = (tip - hook).Length(), RadialSegments = 5, Rings = 1 },
                new Transform3D(Toward(tip - hook), (hook + tip) * 0.5f));
        }
        // Rubble round the foot.
        var chunk = new SphereMesh { Radius = 1f, Height = 2f, RadialSegments = 5, Rings = 3 };
        var rubble = new (float Angle, float Reach, Vector3 Size)[]
        {
            (0.6f, 0.92f, new(0.09f, 0.06f, 0.07f)), (2.5f, 0.88f, new(0.06f, 0.04f, 0.08f)), (4.3f, 0.95f, new(0.08f, 0.05f, 0.06f)),
        };
        foreach (var (angle, reach, size) in rubble)
            Append(st, chunk, new Transform3D(new Basis(Vector3.Up, angle * 2f) * Basis.FromScale(size), new Vector3(Mathf.Cos(angle), 0.02f, Mathf.Sin(angle)) * reach));
        return End(st);
    }

    private static Vector3 RandomAxis(Func<float> R) =>
        new Vector3(R() - 0.5f, R() - 0.5f, R() - 0.5f).Normalized() is var v && v.LengthSquared() > 0.5f ? v : Vector3.Up;

    /// <summary>
    /// A bevelled plate (an eight-sided bar) from <paramref name="a"/> to <paramref name="b"/>,
    /// <paramref name="width"/> across along <paramref name="across"/> and <paramref name="thick"/>
    /// through the third way.
    /// </summary>
    private static void Bar(SurfaceTool st, Vector3 a, Vector3 b, float width, float thick, Vector3 across)
    {
        var along = b - a;
        var y = along.Normalized();
        var x = (across - y * across.Dot(y)).Normalized();
        var z = x.Cross(y);
        var basis = new Basis(x * width, y * along.Length(), z * thick) * new Basis(Vector3.Up, Mathf.Pi / 8f);
        Append(st, new CylinderMesh { TopRadius = 0.5f / Mathf.Cos(Mathf.Pi / 8f), BottomRadius = 0.5f / Mathf.Cos(Mathf.Pi / 8f), Height = 1f, RadialSegments = 8, Rings = 1 },
            new Transform3D(basis, (a + b) * 0.5f));
    }

    /// <summary>
    /// The blade, its point on the origin and its base at <c>y = 0.7</c>: an eight-sided section,
    /// a flat diamond near the point and the base and a groove (the fuller) sunk into both faces
    /// between, keeping its width to the shoulders, then tapering to the point.
    /// </summary>
    private static void SwordBlade(SurfaceTool st)
    {
        const float Half = 0.055f, Ridge = 0.016f;
        // Heights up the blade, how deep the fuller is there (a share of the ridge) and how wide.
        var levels = new (float Y, float Groove, float Width)[]
        {
            (0.2f, 0f, 1f), (0.28f, 0f, 1f), (0.33f, 0.7f, 1f), (0.62f, 0.7f, 1f), (0.67f, 0f, 1f), (0.7f, 0f, 1.08f),
        };
        // Each ring runs anticlockwise seen from above (+Y), as (x, z) pairs.
        static Vector3[] Ring(float y, float groove, float width)
        {
            var h = Half * width;
            var side = Ridge * 0.65f;
            var mid = Ridge * (1f - groove);
            return
            [
                new(h, y, 0f), new(0.35f * h, y, -side), new(0f, y, -mid), new(-0.35f * h, y, -side),
                new(-h, y, 0f), new(-0.35f * h, y, side), new(0f, y, mid), new(0.35f * h, y, side),
            ];
        }
        // Outward from a side face: away from the blade's axis across the section, which holds
        // in the fuller too (its faces lean in, but still face out of the blade's face).
        static Vector3 Out(Vector3 a, Vector3 b)
        {
            var edge = b - a;
            var normal = new Vector3(edge.Z, 0f, -edge.X);
            var mid = (a + b) * 0.5f;
            return normal.Dot(mid with { Y = 0f }) >= 0f ? normal : -normal;
        }
        var rings = levels.Select(l => Ring(l.Y, l.Groove, l.Width)).ToArray();
        var tip = Vector3.Zero;
        for (var i = 0; i < 8; i++)
        {
            var j = (i + 1) % 8;
            var first = rings[0];
            Tri(st, tip, first[i], first[j], Out(first[i], first[j]) + Vector3.Down * 0.1f);
            for (var r = 0; r + 1 < rings.Length; r++)
            {
                var (lo, hi) = (rings[r], rings[r + 1]);
                var outward = Out(lo[i], lo[j]) + Out(hi[i], hi[j]);
                Tri(st, lo[i], hi[i], hi[j], outward);
                Tri(st, lo[i], hi[j], lo[j], outward);
            }
        }
        var top = rings[^1];
        var centre = new Vector3(0f, top[0].Y, 0f);
        for (var i = 0; i < 8; i++)
            Tri(st, centre, top[i], top[(i + 1) % 8], Vector3.Up);
    }

    private static SurfaceTool Begin()
    {
        var st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);
        st.SetSmoothGroup(uint.MaxValue);
        return st;
    }

    private static ArrayMesh End(SurfaceTool st)
    {
        st.GenerateNormals();
        return st.Commit();
    }

    /// <summary>A basis turning +Y (a cylinder's axis, its top end) to point along <paramref name="way"/>.</summary>
    private static Basis Toward(Vector3 way)
    {
        way = way.Normalized();
        var axis = Vector3.Up.Cross(way);
        return axis.LengthSquared() < 1e-6f
            ? way.Y > 0f ? Basis.Identity : new Basis(Vector3.Right, Mathf.Pi)
            : new Basis(axis.Normalized(), Mathf.Acos(Mathf.Clamp(Vector3.Up.Dot(way), -1f, 1f)));
    }

    /// <summary>
    /// A triangle wound so its front faces <paramref name="outward"/>; Godot's front faces wind
    /// clockwise as seen from outside.
    /// </summary>
    private static void Tri(SurfaceTool st, Vector3 a, Vector3 b, Vector3 c, Vector3 outward)
    {
        if ((b - a).Cross(c - a).Dot(outward) < 0f)
            (b, c) = (c, b);
        st.AddVertex(a);
        st.AddVertex(c);
        st.AddVertex(b);
    }

    /// <summary>Adds a primitive mesh's triangles at <paramref name="at"/>.</summary>
    private static void Append(SurfaceTool st, PrimitiveMesh mesh, Vector3 at) => Append(st, mesh, new Transform3D(Basis.Identity, at));

    private static void Append(SurfaceTool st, PrimitiveMesh mesh, Transform3D transform)
    {
        var arrays = mesh.SurfaceGetArrays(0);
        var vertices = arrays[(int)Mesh.ArrayType.Vertex].AsVector3Array();
        var indices = arrays[(int)Mesh.ArrayType.Index].AsInt32Array();
        foreach (var index in indices)
            st.AddVertex(transform * vertices[index]);
    }
}
