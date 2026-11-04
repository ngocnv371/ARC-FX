using Godot;

namespace Arc.Fx;

/// <summary>
/// What a dead monster leaves behind as it shrinks away: a wisp of vapour in its body colour
/// that swells and thins as it drifts up, and a cluster of bubbles in its accent colour that
/// float off, wobble and pop one after another. Every puff and bubble in the field is one
/// instance of a single multimesh (fx_bubble.gdshader draws both), so a horde dying at once
/// costs one draw call; when the pool is full the oldest give way. Add one to the scene and
/// call <see cref="Tick"/> from the game's clock.
/// </summary>
public partial class FxPuffs : Node3D
{
    private const int Capacity = 768;
    /// <summary>The share of a bubble's life spent popping, at its end.</summary>
    private const float PopShare = 0.22f;

    private struct Mote
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public float Size;
        public float Age;
        public float Life;
        public float Seed;
        public bool Vapour;
        public Color Color;
    }

    private readonly Mote[] _motes = new Mote[Capacity];
    private int _count;
    private MultiMesh _multimesh = null!;
    private readonly RandomNumberGenerator _random = new(); // its own: the game seeds GD's for reproducible runs

    public override void _Ready()
    {
        _multimesh = new MultiMesh
        {
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
            UseColors = true,
            UseCustomData = true,
            Mesh = new QuadMesh { Size = Vector2.One },
            InstanceCount = Capacity,
            VisibleInstanceCount = 0,
        };
        AddChild(new MultiMeshInstance3D
        {
            Multimesh = _multimesh,
            MaterialOverride = new ShaderMaterial { Shader = FxShaders.Get("bubble") },
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            // The motes are anywhere on the field, so the batch is never culled by its bounds.
            CustomAabb = new Aabb(new Vector3(-1e4f, -10f, -1e4f), new Vector3(2e4f, 40f, 2e4f)),
        });
    }

    /// <summary>
    /// A body of <paramref name="size"/> (about its height in metres) evaporating where it stands
    /// at <paramref name="feet"/>: vapour in its <paramref name="body"/> colour, bubbles in it and
    /// its <paramref name="accent"/>. A <paramref name="big"/> one (an elite) gives off more.
    /// </summary>
    public void Burst(Vector3 feet, float size, Color body, Color accent, bool big = false)
    {
        size = Mathf.Max(0.5f, size);
        var centre = feet + Vector3.Up * size * 0.45f;
        body = body.Lerp(Colors.White, 0.35f);
        accent = accent.Lerp(Colors.White, 0.25f);

        var puffs = big ? 3 : 2;
        for (var i = 0; i < puffs; i++)
        {
            Add(new Mote
            {
                Position = centre + Jitter(size * 0.25f),
                Velocity = Vector3.Up * Rand(0.5f, 0.9f) + Jitter(0.3f),
                Size = size * Rand(1f, 1.4f),
                Life = Rand(0.6f, 0.85f),
                Seed = _random.Randf(),
                Vapour = true,
                Color = body,
            });
        }

        var bubbles = Mathf.RoundToInt(5 + 4 * size) + (big ? 4 : 0);
        for (var i = 0; i < bubbles; i++)
        {
            var spread = Jitter(1f) with { Y = 0f };
            Add(new Mote
            {
                Position = centre + spread * size * 0.35f + Vector3.Up * Rand(-0.2f, 0.25f) * size,
                Velocity = spread * Rand(0.4f, 1.1f) + Vector3.Up * Rand(0.9f, 2f),
                Size = size * Rand(0.16f, 0.34f),
                // Staggered, so they pop in a patter rather than all at once.
                Life = Rand(0.45f, 1f),
                Seed = _random.Randf(),
                Color = i % 3 == 0 ? body : accent,
            });
        }
    }

    /// <summary>One small bubble of <paramref name="color"/> floating off from <paramref name="at"/>, as a hit's mark.</summary>
    public void Bubble(Vector3 at, Color color)
    {
        var spread = Jitter(1f) with { Y = 0f };
        Add(new Mote
        {
            Position = at + spread * 0.15f,
            Velocity = spread * Rand(0.4f, 0.9f) + Vector3.Up * Rand(1.2f, 1.8f),
            Size = Rand(0.22f, 0.3f),
            Life = Rand(0.55f, 0.75f),
            Seed = _random.Randf(),
            Color = color,
        });
    }

    public void Tick(float dt)
    {
        for (var i = _count - 1; i >= 0; i--)
        {
            ref var m = ref _motes[i];
            m.Age += dt;
            if (m.Age >= m.Life)
            {
                _motes[i] = _motes[--_count];
                continue;
            }
            // Bubbles wobble as they rise and slow down; vapour just drifts and spreads.
            var wobble = m.Vapour ? Vector3.Zero : new Vector3(Mathf.Sin(m.Age * 9f + m.Seed * 40f), 0f, Mathf.Cos(m.Age * 7f + m.Seed * 25f)) * 0.6f;
            m.Velocity *= Mathf.Exp(-(m.Vapour ? 1.5f : 2.2f) * dt);
            m.Position += (m.Velocity + wobble) * dt;
        }

        for (var i = 0; i < _count; i++)
        {
            ref var m = ref _motes[i];
            var t = m.Age / m.Life;
            float scale;
            float pop;
            if (m.Vapour)
            {
                scale = m.Size * (0.6f + 0.8f * Mathf.Sqrt(t));
                pop = 0f;
            }
            else
            {
                // Blown up quickly, then held until it bursts.
                scale = m.Size * Mathf.Min(1f, 0.35f + t * 4f);
                pop = Mathf.Max(0f, (t - (1f - PopShare)) / PopShare);
            }
            _multimesh.SetInstanceTransform(i, new Transform3D(Basis.FromScale(Vector3.One * scale), m.Position));
            _multimesh.SetInstanceColor(i, m.Color);
            _multimesh.SetInstanceCustomData(i, new Color(t, m.Vapour ? 1f : 0f, m.Seed, pop));
        }
        _multimesh.VisibleInstanceCount = _count;
    }

    private void Add(Mote mote)
    {
        // Full: the oldest makes way (the slot with the most of its life used up).
        if (_count == Capacity)
        {
            var oldest = 0;
            for (var i = 1; i < _count; i++)
            {
                if (_motes[i].Age / _motes[i].Life > _motes[oldest].Age / _motes[oldest].Life)
                    oldest = i;
            }
            _motes[oldest] = mote;
            return;
        }
        _motes[_count++] = mote;
    }

    private float Rand(float from, float to) => _random.RandfRange(from, to);

    private Vector3 Jitter(float amount) =>
        new Vector3(Rand(-1f, 1f), Rand(-1f, 1f), Rand(-1f, 1f)) * amount;
}
