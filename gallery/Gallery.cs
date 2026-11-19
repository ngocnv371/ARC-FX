using Godot;

namespace Arc.Fx.Gallery;

/// <summary>
/// ARC FX Gallery: every effect in the library on a plain floor, under the camera the ARC games
/// use, replaying every few seconds, with the call that makes it written underneath. Up and
/// Down (or the list) pick one, Space plays it again, V switches between the game camera and a
/// close look, F the floor, - and + slow it down or speed it back up, P pauses, Esc quits.
/// The box over the list (/ or Ctrl+F to type in it) narrows the list to entries whose group and
/// name hold every word typed; Enter or Esc leaves it.
/// Options: <c>--effect=name</c> starts on one; <c>--filter=words</c> starts with the list narrowed; <c>--view=close</c>; <c>--speed=0.25</c>;
/// <c>--shot=dir</c> plays each in turn and saves dir/name.png as it peaks, then quits
/// (<c>--only=a,b</c> limits that to a few).
/// </summary>
public partial class Gallery : Node3D
{
    private sealed record Entry(string Group, string Name, string Code, float Every, float Peak, Action<Gallery> Play, string Tags = "");

    private static readonly (string Name, Vector3 Eye, Vector3 Target)[] Views =
    [
        // As in a run: 55 degrees down, a little closer than the games' follow camera.
        ("Game camera", new Vector3(0f, 16f * 0.819f, 16f * 0.574f), new Vector3(0f, 0.5f, 0f)),
        ("Close look", new Vector3(0f, 4.5f, 8f), new Vector3(0f, 1f, 0f)),
    ];
    private static readonly (string Name, Color Color)[] Floors =
        [("Meadow", new Color(0.32f, 0.42f, 0.3f)), ("Scorched", new Color(0.12f, 0.08f, 0.06f)), ("Snow", new Color(0.78f, 0.82f, 0.86f))];

    // Slow motion for fast things (arrows cross the stage in a fraction of a second). It sets
    // Engine.TimeScale, which scales process delta and shader TIME alike, so the effects, the
    // replay clock and their flicker all slow together.
    private static readonly float[] Speeds = [1f, 0.5f, 0.25f, 0.1f, 0.05f];

    private readonly Camera3D _camera = new() { Fov = 34f, Near = 0.1f, Far = 300f };
    private readonly MeshInstance3D _floor = new()
    {
        Mesh = new PlaneMesh { Size = new Vector2(300f, 300f) },
        CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
    };
    private readonly StandardMaterial3D _floorMaterial = new() { ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded };
    private readonly Node3D _caster = new() { Name = "Caster" };
    private readonly List<Func<float, bool>> _runners = [];
    private readonly ItemList _list = new();
    private readonly LineEdit _filter = new() { PlaceholderText = "Filter: skill, fire, slash...", ClearButtonEnabled = true };
    // The entries the list shows, in order: indices into _entries.
    private readonly List<int> _shown = [];
    private readonly Label _code = new();
    private readonly Label _hint = new();
    private FxWorld _world = null!;
    private FxPuffs _puffs = null!;
    private List<Entry> _entries = [];
    private int _selected;
    private int _view;
    private int _floorIndex;
    private int _speed;
    private bool _paused;
    private float _clock;
    private Dictionary<string, string> _options = [];

    public override void _Ready()
    {
        _options = ParseOptions();
        _entries = BuildEntries();
        AddChild(_camera);
        _floor.MaterialOverride = _floorMaterial;
        AddChild(_floor);
        // Whoever casts: a plain grey capsule, so auras, beams and rams have something to come from.
        _caster.AddChild(new MeshInstance3D
        {
            Mesh = new CapsuleMesh { Radius = 0.3f, Height = 1.6f },
            Position = Vector3.Up * 0.8f,
            MaterialOverride = new StandardMaterial3D { AlbedoColor = new Color(0.55f, 0.55f, 0.6f), ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded },
        });
        AddChild(_caster);
        NewWorld();
        BuildUi();

        _view = _options.GetValueOrDefault("view") == "close" ? 1 : 0;
        ApplyView();
        ApplyFloor();
        if (_options.TryGetValue("speed", out var speed) && float.TryParse(speed, System.Globalization.CultureInfo.InvariantCulture, out var wanted))
        {
            // The nearest step at or below what was asked for.
            _speed = Array.FindIndex(Speeds, s => s <= wanted + 0.001f);
            if (_speed < 0)
                _speed = Speeds.Length - 1;
        }
        // A bake times its pictures by the effects' own clock, so it always runs at full speed.
        if (!_options.ContainsKey("shot"))
            ApplySpeed();
        if (_options.TryGetValue("effect", out var start))
            _selected = Math.Max(0, _entries.FindIndex(e => NameOf(e).Equals(start, StringComparison.OrdinalIgnoreCase)));
        if (_options.TryGetValue("filter", out var filter))
        {
            _filter.Text = filter.Replace(',', ' ');
            ApplyFilter();
            if (_shown.Count > 0 && !_shown.Contains(_selected))
                _selected = _shown[0];
        }
        if (_options.TryGetValue("shot", out var dir))
            _ = Bake(dir);
        else
            Select(_selected);
    }

    public override void _Process(double delta)
    {
        var dt = (float)delta;
        _puffs.Tick(dt);
        for (var i = _runners.Count - 1; i >= 0; i--)
        {
            if (!_runners[i](dt))
                _runners.RemoveAt(i);
        }
        if (_options.ContainsKey("shot"))
            return;
        _clock += dt;
        if (_clock >= _entries[_selected].Every)
            Play();
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e is not InputEventKey { Pressed: true, Echo: false } key)
            return;
        switch (key.Keycode)
        {
            case Key.Escape:
                GetTree().Quit();
                break;
            case Key.Slash:
            case Key.F when key.CtrlPressed:
                _filter.GrabFocus();
                _filter.SelectAll();
                GetViewport().SetInputAsHandled();
                break;
            case Key.Down:
                Step(1);
                break;
            case Key.Up:
                Step(-1);
                break;
            case Key.Space:
                Play();
                break;
            case Key.V:
                _view = (_view + 1) % Views.Length;
                ApplyView();
                break;
            case Key.F:
                _floorIndex = (_floorIndex + 1) % Floors.Length;
                ApplyFloor();
                break;
            case Key.Minus or Key.KpSubtract or Key.Bracketleft:
                _speed = Math.Min(_speed + 1, Speeds.Length - 1);
                ApplySpeed();
                break;
            case Key.Equal or Key.Plus or Key.KpAdd or Key.Bracketright:
                _speed = Math.Max(_speed - 1, 0);
                ApplySpeed();
                break;
            case Key.P:
                _paused = !_paused;
                ApplySpeed();
                break;
        }
    }

    /// <summary>Moves to the next (or previous) entry the list shows, wrapping round.</summary>
    private void Step(int by)
    {
        if (_shown.Count == 0)
            return;
        var at = _shown.IndexOf(_selected);
        at = at < 0 ? (by > 0 ? 0 : _shown.Count - 1) : (at + by + _shown.Count) % _shown.Count;
        Select(_shown[at]);
    }

    /// <summary>Highlights entry <paramref name="index"/> in the list, if the filter lets it show.</summary>
    private void Mark(int index)
    {
        var at = _shown.IndexOf(index);
        if (at < 0)
        {
            _list.DeselectAll();
            return;
        }
        _list.Select(at);
        _list.EnsureCurrentIsVisible();
    }

    /// <summary>Fills the list with the entries whose group and name hold every word in the filter.</summary>
    private void ApplyFilter()
    {
        var words = _filter.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        _shown.Clear();
        _list.Clear();
        for (var i = 0; i < _entries.Count; i++)
        {
            var text = $"{_entries[i].Group} {_entries[i].Name} {NameOf(_entries[i])} {_entries[i].Tags}";
            if (!words.All(w => text.Contains(w, StringComparison.OrdinalIgnoreCase)))
                continue;
            _shown.Add(i);
            _list.AddItem(_entries[i].Tags == "" ? $"{_entries[i].Group} · {_entries[i].Name}" : $"{_entries[i].Group} · {_entries[i].Name}  ({_entries[i].Tags})");
        }
        Mark(_selected);
    }

    private void Select(int index)
    {
        _selected = index;
        Mark(index);
        var entry = _entries[index];
        _code.Text = entry.Code;
        Play();
    }

    /// <summary>Clears the stage and plays the selected effect from the start.</summary>
    private void Play()
    {
        _clock = 0f;
        NewWorld();
        _entries[_selected].Play(this);
    }

    /// <summary>A fresh FxWorld (and puffs), so nothing from the last effect lingers.</summary>
    private void NewWorld()
    {
        _runners.Clear();
        if (_world != null)
        {
            RemoveChild(_world);
            _world.QueueFree();
            RemoveChild(_puffs);
            _puffs.QueueFree();
        }
        _world = new FxWorld { Name = "Fx" };
        AddChild(_world);
        _puffs = new FxPuffs { Name = "Puffs" };
        AddChild(_puffs);
        _caster.Position = Vector3.Zero;
    }

    private void ApplyView()
    {
        var (_, eye, target) = Views[_view];
        _camera.Position = eye;
        _camera.LookAt(target, Vector3.Up);
        UpdateHint();
    }

    private void ApplyFloor()
    {
        _floorMaterial.AlbedoColor = Floors[_floorIndex].Color;
        UpdateHint();
    }

    private void ApplySpeed()
    {
        Engine.TimeScale = _paused ? 0.0 : Speeds[_speed];
        UpdateHint();
    }

    private void UpdateHint() =>
        _hint.Text = $"↑↓ effect   / filter   Space replay   V view: {Views[_view].Name}   F floor: {Floors[_floorIndex].Name}   " +
            $"−/+ speed: {Speeds[_speed]:0.##}×{(_paused ? " (paused)" : "")}   P pause   Esc quit";

    private void BuildUi()
    {
        var ui = new CanvasLayer();
        AddChild(ui);
        var panel = new PanelContainer { Position = new Vector2(16f, 16f), CustomMinimumSize = new Vector2(280f, 0f) };
        panel.SetAnchorsPreset(Control.LayoutPreset.LeftWide);
        panel.OffsetLeft = 16f;
        panel.OffsetTop = 16f;
        panel.OffsetBottom = -120f;
        panel.OffsetRight = 296f;
        ui.AddChild(panel);
        var column = new VBoxContainer();
        panel.AddChild(column);
        column.AddChild(_filter);
        _list.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        _list.ItemSelected += at => Select(_shown[(int)at]);
        column.AddChild(_list);
        _filter.TextChanged += _ =>
        {
            ApplyFilter();
            // Show the first match straight away, unless what is playing still matches.
            if (_shown.Count > 0 && !_shown.Contains(_selected))
                Select(_shown[0]);
        };
        // Enter or Esc hands the keys back to the gallery (Up and Down, Space, V...).
        _filter.TextSubmitted += _ => _filter.ReleaseFocus();
        _filter.GuiInput += e =>
        {
            if (e is InputEventKey { Pressed: true, Keycode: Key.Escape })
            {
                _filter.ReleaseFocus();
                _filter.AcceptEvent();
            }
            else if (e is InputEventKey { Pressed: true, Keycode: Key.Down or Key.Up } arrow)
            {
                Step(arrow.Keycode == Key.Down ? 1 : -1);
                _filter.AcceptEvent();
            }
        };
        ApplyFilter();

        var bottom = new VBoxContainer();
        bottom.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
        bottom.OffsetLeft = 16f;
        bottom.OffsetTop = -100f;
        bottom.OffsetRight = -16f;
        bottom.OffsetBottom = -16f;
        ui.AddChild(bottom);
        _code.AddThemeFontSizeOverride("font_size", 20);
        _code.AddThemeColorOverride("font_color", new Color(1f, 0.92f, 0.7f));
        _code.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        bottom.AddChild(_code);
        _hint.AddThemeColorOverride("font_color", new Color(0.75f, 0.75f, 0.8f));
        bottom.AddChild(_hint);
        UpdateHint();
    }

    /// <summary>Plays every effect in turn (or the --only ones), saving dir/name.png as each peaks, then quits.</summary>
    private async Task Bake(string dir)
    {
        var only = _options.TryGetValue("only", out var list) ? list.Split(',') : null;
        var cwd = System.Environment.GetEnvironmentVariable("GALLERY_CWD") ?? ".";
        var root = System.IO.Path.IsPathRooted(dir) ? dir : System.IO.Path.Combine(cwd, dir);
        System.IO.Directory.CreateDirectory(root);
        // A few frames first, so the window has drawn once and the first picture isn't black.
        for (var i = 0; i < 5; i++)
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        for (var i = 0; i < _entries.Count; i++)
        {
            var entry = _entries[i];
            if (only != null && !only.Contains(NameOf(entry), StringComparer.OrdinalIgnoreCase))
                continue;
            _selected = i;
            _code.Text = entry.Code;
            Mark(i);
            // Once untimed first: the first frame that draws a new shader stalls while it compiles,
            // and a long frame would carry the effect past its peak before the picture is taken.
            Play();
            for (var f = 0; f < 3; f++)
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            Play();
            var waited = 0f;
            while (waited < entry.Peak)
            {
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                waited += (float)GetProcessDeltaTime();
            }
            // Read in process, as the games' --shot does: a minimised window never signals a drawn frame.
            GetViewport().GetTexture().GetImage().SavePng(System.IO.Path.Combine(root, NameOf(entry) + ".png"));
        }
        GetTree().Quit();
    }

    private static string NameOf(Entry entry) => $"{entry.Group}_{entry.Name}".ToLowerInvariant().Replace(' ', '_');

    private static Dictionary<string, string> ParseOptions()
    {
        var options = new Dictionary<string, string>();
        foreach (var arg in OS.GetCmdlineUserArgs())
        {
            var text = arg.TrimStart('-');
            var eq = text.IndexOf('=');
            options[eq < 0 ? text : text[..eq]] = eq < 0 ? "" : text[(eq + 1)..];
        }
        return options;
    }

    /// <summary>Runs <paramref name="step"/> every frame until it returns false.</summary>
    private void Run(Func<float, bool> step) => _runners.Add(step);

    /// <summary>A shot flying across the stage, placed every frame as a game would.</summary>
    private void Fly(FxShotLook look, float speed = 14f, float range = 16f)
    {
        var shot = Fx.Shot(look);
        // Right to left across the stage.
        var from = new Vector3(range * 0.5f, Fx.ShotHeight, 1.5f);
        var velocity = Vector3.Left * speed;
        var age = 0f;
        var life = range / speed;
        Run(dt =>
        {
            age += dt;
            if (age >= life)
            {
                shot.Release();
                return false;
            }
            var grow = Fx.IsTornado(look) ? Mathf.SmoothStep(0f, 1f, Mathf.Clamp(Mathf.Min(age / 0.3f, (life - age) / 0.4f), 0f, 1f)) : 1f;
            shot.Place(from + velocity * age, velocity, 1f, grow);
            shot.Strength(1f - age / life);
            return true;
        });
    }

    private List<Entry> BuildEntries()
    {
        return
        [
            new("Explosion", "Fire", "Fx.Explosion(at, 3f, FxElement.Fire);", 2.5f, 0.35f, _ => Fx.Explosion(Vector3.Zero, 3f, FxElement.Fire)),
            new("Explosion", "Frost", "Fx.Explosion(at, 3f, FxElement.Frost);", 2.5f, 0.35f, _ => Fx.Explosion(Vector3.Zero, 3f, FxElement.Frost)),
            new("Explosion", "Lightning", "Fx.Explosion(at, 3f, FxElement.Lightning);", 2f, 0.25f, _ => Fx.Explosion(Vector3.Zero, 3f, FxElement.Lightning)),
            new("Explosion", "Holy dome", "Fx.Explosion(at, 3f, FxElement.Holy);", 2f, 0.15f, _ => Fx.Explosion(Vector3.Zero, 3f, FxElement.Holy)),
            new("Explosion", "Poison dome", "Fx.Explosion(at, 3f, FxElement.Poison);", 2f, 0.15f, _ => Fx.Explosion(Vector3.Zero, 3f, FxElement.Poison)),

            new("Ground", "Burning", "Fx.Patch(at, 3f, life: 3f, FxElement.Fire);", 3.5f, 1f, _ => Fx.Patch(Vector3.Zero, 3f, 3f, FxElement.Fire)),
            new("Ground", "Poison pool", "Fx.Patch(at, 3f, life: 3f, FxElement.Poison);", 3.5f, 1f, _ => Fx.Patch(Vector3.Zero, 3f, 3f, FxElement.Poison)),
            new("Ground", "Poison cloud", "Fx.Patch(at, 3f, life: 4f, poison, FxGround.Miasma);", 4.5f, 1.5f,
                _ => Fx.Patch(Vector3.Zero, 3f, 4f, FxTheme.Of(FxElement.Poison), FxGround.Miasma)),
            new("Ground", "Hoarfrost", "Fx.Patch(at, 3f, life: 3f, FxElement.Frost);", 3.5f, 1f, _ => Fx.Patch(Vector3.Zero, 3f, 3f, FxElement.Frost)),
            new("Ground", "Glow", "Fx.Patch(at, 3f, life: 3f, FxElement.Holy);", 3.5f, 1f, _ => Fx.Patch(Vector3.Zero, 3f, 3f, FxElement.Holy)),
            new("Ground", "Warning", "Fx.Patch(at, 2.5f, life: 1.2f, red, FxGround.Warning);", 2f, 0.9f,
                _ => Fx.Patch(Vector3.Zero, 2.5f, 1.2f, new Color(1f, 0.25f, 0.15f), FxGround.Warning)),
            new("Ground", "Inferno aura", "Fx.Aura(caster, 3.5f, keep: 1f, gold, FxGround.Inferno);  // renewed as it ticks", 4f, 1.5f,
                g => g.Run(new Aura(g._caster, new Color(1f, 0.55f, 0.15f), FxGround.Inferno, 3.5f).Step)),
            new("Ground", "Frost aura", "Fx.Aura(caster, 3.5f, keep: 1f, FxElement.Frost);  // renewed as it ticks", 4f, 1.5f,
                g => g.Run(new Aura(g._caster, FxTheme.Of(FxElement.Frost), FxGround.Frost, 3.5f).Step)),

            .. Enum.GetValues<FxSigil>().Select(sigil => new Entry("Sigil", sigil.ToString(),
                $"Fx.Sigil(caster, FxSigil.{sigil}, 1.6f, keep: 1f);  // renewed as the aura ticks", 4f, 1.5f,
                g => g.Run(new Sigil(g._caster, sigil).Step))),
            new("Sigil ring", "One", "Fx.Sigil(caster, FxSigil.Might, 1.4f, keep: 1f, FxSigilLook.Ring);", 4.5f, 1.5f,
                g => g.Run(new Stack(g._caster, [FxSigil.Might]).Step)),
            new("Sigil ring", "Two", "Fx.Sigil(caster, FxSigil.Fire, 1.4f, keep: 1f, FxSigilLook.Ring);  Fx.Sigil(caster, FxSigil.Life, ...);", 4.5f, 1.5f,
                g => g.Run(new Stack(g._caster, [FxSigil.Fire, FxSigil.Life]).Step)),
            new("Sigil ring", "Three", "// lightning, guard and haste, arriving one after another: they share one ring", 4.5f, 1.8f,
                g => g.Run(new Stack(g._caster, [FxSigil.Lightning, FxSigil.Guard, FxSigil.Haste]).Step)),
            new("Sigil ring", "All ten", "// every sigil at once, each at its own angle round the one ring", 5f, 2.4f,
                g => g.Run(new Stack(g._caster, Enum.GetValues<FxSigil>()).Step)),

            new("Line", "Lightning strike", "Fx.Lightning([(at + Vector3.Up * 12f, at)], 0.2f, tendrils: 12, thickness: 0.6f);  // fine, like an arc", 1f, 0.05f,
                _ => Fx.Lightning([(Vector3.Up * 12f, Vector3.Zero)], 0.2f, 12, 0.6f)),
            new("Line", "Lightning fence", "Fx.Lightning([(a, b), (b, c), (c, a)], 2f, tendrils: 8, thickness: 0.6f);  // fine, like an arc", 2.5f, 0.5f,
                _ => Fx.Lightning([(new Vector3(-3f, 0f, 2f), new Vector3(3f, 0f, 2f)), (new Vector3(3f, 0f, 2f), new Vector3(0f, 0f, -3f)), (new Vector3(0f, 0f, -3f), new Vector3(-3f, 0f, 2f))], 2f, 8, 0.6f)),
            new("Line", "Arc", "Fx.Arc(from, to);  // one jump; a chain calls it once a jump, FxTiming.ArcLeap apart", 1.5f, 0.17f,
                g => g.Run(new Chain([new(0f, 0f, 4f), new(-2.5f, 0f, -1f), new(1f, 0f, -3.5f), new(3.5f, 0f, -0.5f)], 0f).Step)),
            new("Line", "Arc with blasts", "Fx.Arc(from, to, burst: 1.6f);  // a lightning blast where each jump lands", 1.5f, 0.2f,
                g => g.Run(new Chain([new(0f, 0f, 4f), new(-2.5f, 0f, -1f), new(1f, 0f, -3.5f), new(3.5f, 0f, -0.5f)], 1.6f).Step)),

            new("Melee", "Slash", "Fx.Slash(at, forward, 3f, arc: 120f, steel);", 1f, 0.1f,
                _ => Fx.Slash(Vector3.Zero, Vector3.Back, 3f, 120f, new Color(0.85f, 0.92f, 1f))),
            new("Melee", "Whirl", "Fx.Slash(at, forward, 3f, arc: 360f, steel);", 1f, 0.1f,
                _ => Fx.Slash(Vector3.Zero, Vector3.Back, 3f, 360f, new Color(0.85f, 0.92f, 1f))),
            new("Melee", "Crossing slashes", "Fx.Slash(at, forward, 2.6f, 110f, steel, tilt: 38f);  // and one tilted -38f, reverse: true", 1f, 0.1f,
                _ =>
                {
                    Fx.Slash(Vector3.Zero, Vector3.Back, 2.6f, 110f, new Color(0.85f, 0.92f, 1f), tilt: 38f);
                    Fx.Slash(Vector3.Zero, Vector3.Back, 2.6f, 110f, new Color(0.85f, 0.92f, 1f), tilt: -38f, reverse: true);
                }),
            new("Melee", "Upward slash", "Fx.Slash(at, forward, 3f, 150f, steel, tilt: -72f);  // not quite upright, so it is never edge on", 1f, 0.1f,
                _ => Fx.Slash(Vector3.Zero, Vector3.Back, 3f, 150f, new Color(0.85f, 0.92f, 1f), tilt: -72f)),
            new("Melee", "Thrust", "Fx.Thrust(at, forward, 4.5f, 1.2f, steel);", 1f, 0.1f,
                _ => Fx.Thrust(Vector3.Zero, Vector3.Back, 4.5f, 1.2f, new Color(0.85f, 0.92f, 1f))),
            new("Melee", "Blade vortex", "Fx.BladeVortex(caster, blades: 6, 1.6f, keep: 1f);  // renewed as it ticks", 4f, 1.5f,
                g => g.Run(new Vortex(g._caster, 6, 6).Step)),
            new("Melee", "Blade vortex stacking", "Fx.BladeVortex(caster, stacks, 1.6f, keep: 1f);  // a blade more each cast, up to Fx.BladeVortexMax", 5f, 3f,
                g => g.Run(new Vortex(g._caster, 1, Fx.BladeVortexMax).Step)),
            .. Enum.GetValues<FxVortexLook>().Where(look => look != FxVortexLook.Steel).Select(look => new Entry("Melee", $"{look} vortex",
                $"Fx.BladeVortex(caster, blades: 8, 1.6f, keep: 1f, FxVortexLook.{look});", 4f, 1.5f,
                g => g.Run(new Vortex(g._caster, 8, 8, look).Step))),

            new("Sky", "Beam", "Fx.Beam(at, 1.2f, life: 2f, gold);", 2.5f, 0.5f, _ => Fx.Beam(Vector3.Zero, 1.2f, 2f, new Color(1f, 0.62f, 0.25f))),
            new("Storm", "Storm orb", "var orb = Fx.StormOrb(at, 5f, life: 4f);  // strikes: Fx.Lightning from orb.Heart", 4.5f, 1.2f,
                g => g.Run(new Orb(Fx.StormOrb(Vector3.Zero, 5f, 4f)).Step)),
            new("Storm", "Frost bomb", "Fx.FrostBomb(at, 3f, fuse: 1.5f);", 3f, 1.3f, _ => Fx.FrostBomb(Vector3.Zero, 3f, 1.5f)),
            new("Storm", "Nova", "Fx.Nova(at, 6f);", 1.5f, 0.3f, _ => Fx.Nova(Vector3.Zero, 6f)),
            new("Storm", "Divine Ire", "var ire = Fx.DivineIre(caster, aim, 14f, 2.5f);  // ire.Aim(...) while it gathers", 2f, 0.7f,
                g =>
                {
                    var ire = Fx.DivineIre(g._caster.Position, Vector3.Right, 14f, 2.5f);
                    g.Run(_ =>
                    {
                        ire.Aim(g._caster.Position, Vector3.Right);
                        return ire.Alive;
                    });
                }),

            new("Cast", "Meteor", "Fx.Meteor(at, 3f);  // lands after FxTiming.MeteorFall", 2.8f, 0.7f, _ => Fx.Meteor(Vector3.Zero, 3f)),
            new("Cast", "Meteor with warning", "Fx.Meteor(at, 3f);  Fx.Patch(at, 3f, life: FxTiming.MeteorFall, red, FxGround.Warning);  // the game's own warning", 2.8f, 0.7f,
                _ =>
                {
                    Fx.Meteor(Vector3.Zero, 3f);
                    Fx.Patch(Vector3.Zero, 3f, FxTiming.MeteorFall, new Color(1f, 0.25f, 0.15f), FxGround.Warning);
                }),
            new("Cast", "Bladefall", "Fx.Bladefall(at, 4f);  // blades land from FxTiming.BladefallDrop over FxTiming.BladefallSpread", 2.5f, 0.6f,
                _ => Fx.Bladefall(Vector3.Zero, 4f)),
            new("Cast", "Heal", "Fx.Heal(target, green);", 1.6f, 0.45f, g => Fx.Heal(g._caster, new Color(0.55f, 1f, 0.6f))),
            new("Cast", "Barrier", "var barrier = Fx.Barrier(target, 1.4f, life: 3f, spirit);  // barrier.Hit(from), barrier.Break()", 3.2f, 1f,
                g => g.Run(new Barrier(Fx.Barrier(g._caster, 1.4f, 3f, FxTheme.Of(FxElement.Spirit))).Step)),

            .. Enum.GetValues<FxSkill>().OrderBy(skill => Array.IndexOf(Themes, SkillLook(skill).Theme)).Select(SkillEntry),

            new("Earth", "Stone slam", "Fx.Slam(at, 3f, FxSpike.Stone);", 2f, 0.2f, _ => Fx.Slam(Vector3.Zero, 3f, FxSpike.Stone)),
            new("Earth", "Ice slam", "Fx.Slam(at, 3f, FxSpike.Ice);", 2f, 0.2f, _ => Fx.Slam(Vector3.Zero, 3f, FxSpike.Ice)),
            new("Earth", "Rupture", "Fx.Rupture(from, forward, 12f, width: 2f);", 2f, 0.45f, _ => Fx.Rupture(new Vector3(-6f, 0f, 0f), Vector3.Right, 12f, 2f)),
            new("Earth", "Shield ring", "Fx.ShieldRing(at, 6, 4f);", 1.5f, 0.25f, _ => Fx.ShieldRing(Vector3.Zero, 6, 4f)),
            new("Earth", "Fist", "Fx.Smite(FxSmite.Fist, at, facing, 3f);", 2f, 0.3f, _ => Fx.Smite(FxSmite.Fist, Vector3.Zero, Vector3.Forward, 3f)),
            new("Earth", "Hammer", "Fx.Smite(FxSmite.Hammer, at, facing, 3f);", 2f, 0.28f, _ => Fx.Smite(FxSmite.Hammer, Vector3.Zero, Vector3.Forward, 3f)),
            new("Earth", "Shield charge", "Fx.Ram(FxRam.Shield, () => runner.Position, way, 1f);", 2f, 0.5f,
                g => g.Charge(FxRam.Shield)),
            new("Earth", "Rolling stone", "Fx.Ram(FxRam.Stone, () => runner.Position, way, 1f);", 2f, 0.6f,
                g => g.Charge(FxRam.Stone)),

            new("Shot", "Arrow", "var shot = Fx.Shot(FxShotLook.Arrow);  // shot.Place(position, velocity) each frame", 1.5f, 0.3f, g => g.Fly(FxShotLook.Arrow, 26f)),
            new("Shot", "Fire arrow", "Fx.Shot(FxShotLook.FireArrow)", 1.5f, 0.3f, g => g.Fly(FxShotLook.FireArrow, 26f)),
            new("Shot", "Ice arrow", "Fx.Shot(FxShotLook.IceArrow)", 1.5f, 0.3f, g => g.Fly(FxShotLook.IceArrow, 26f)),
            new("Shot", "Lightning arrow", "Fx.Shot(FxShotLook.LightningArrow)", 1.5f, 0.3f, g => g.Fly(FxShotLook.LightningArrow, 26f)),
            new("Shot", "Poison arrow", "Fx.Shot(FxShotLook.PoisonArrow)", 1.5f, 0.3f, g => g.Fly(FxShotLook.PoisonArrow, 26f)),
            new("Shot", "Phantom arrow", "Fx.Shot(FxShotLook.PhantomArrow)", 1.5f, 0.3f, g => g.Fly(FxShotLook.PhantomArrow, 26f)),
            new("Shot", "Fire bolt", "Fx.Shot(FxShotLook.FireBolt)", 1.5f, 0.55f, g => g.Fly(FxShotLook.FireBolt, 18f)),
            new("Shot", "Fireball", "Fx.Shot(FxShotLook.Fireball)", 1.8f, 0.55f, g => g.Fly(FxShotLook.Fireball, 15f)),
            new("Shot", "Ice spear", "Fx.Shot(FxShotLook.IceSpear)", 1.5f, 0.55f, g => g.Fly(FxShotLook.IceSpear, 20f)),
            new("Shot", "Frostbolt", "Fx.Shot(FxShotLook.Frostbolt)  // shot.Strength(left) as it fades", 1.8f, 0.55f, g => g.Fly(FxShotLook.Frostbolt, 12f)),
            new("Shot", "Spark", "Fx.Shot(FxShotLook.Spark)", 1.8f, 0.55f, g => g.Fly(FxShotLook.Spark, 12f)),
            new("Shot", "Cold wave", "Fx.Shot(FxShotLook.ColdWave)  // shot.Strength(left) widens it", 1.8f, 0.6f, g => g.Fly(FxShotLook.ColdWave, 12f)),
            new("Shot", "Steel wave", "Fx.Shot(FxShotLook.SteelWave)", 1.8f, 0.6f, g => g.Fly(FxShotLook.SteelWave, 12f)),
            new("Shot", "Sky split", "Fx.Shot(FxShotLook.SkySplit)", 1.2f, 0.4f, g => g.Fly(FxShotLook.SkySplit, 18f)),
            new("Shot", "Bone spear", "Fx.Shot(FxShotLook.BoneSpear)", 1.2f, 0.4f, g => g.Fly(FxShotLook.BoneSpear, 24f)),
            new("Shot", "Thrown axe", "Fx.Shot(FxShotLook.Axe)", 1.8f, 0.55f, g => g.Fly(FxShotLook.Axe, 16f)),
            new("Shot", "Tornado", "Fx.Shot(FxShotLook.Tornado)", 3.5f, 1.5f, g => g.Fly(FxShotLook.Tornado, 5f)),
            new("Shot", "Storm tornado", "Fx.Shot(FxShotLook.StormTornado)  // calls down its own bolts", 3.5f, 1.5f, g => g.Fly(FxShotLook.StormTornado, 5f)),
            new("Shot", "Fire tornado", "Fx.Shot(FxShotLook.FireTornado)", 3.5f, 1.5f, g => g.Fly(FxShotLook.FireTornado, 5f)),
            new("Shot", "Ice tornado", "Fx.Shot(FxShotLook.IceTornado)", 3.5f, 1.5f, g => g.Fly(FxShotLook.IceTornado, 5f)),

            new("Death", "Puff", "puffs.Burst(feet, size: 1.2f, body, accent);  // an FxPuffs node", 1.5f, 0.35f,
                g => g._puffs.Burst(Vector3.Zero, 1.2f, new Color(0.55f, 0.35f, 0.75f), new Color(0.4f, 0.9f, 0.8f))),
            new("Death", "Elite puff", "puffs.Burst(feet, size: 2f, body, accent, big: true);", 1.8f, 0.35f,
                g => g._puffs.Burst(Vector3.Zero, 2f, new Color(0.8f, 0.3f, 0.25f), new Color(1f, 0.8f, 0.3f), big: true)),
        ];
    }

    /// <summary>The skill themes, in the order the list shows them.</summary>
    private static readonly string[] Themes = ["blade", "earth", "fire", "frost", "storm", "nature", "arcane", "holy", "shadow", "buff"];

    /// <summary>How the gallery casts a skill: in front, side on, on the caster, along a line, or travelling.</summary>
    private enum Cast { Front, Side, Follow, Line, Dash, Blink }

    /// <summary>A skill's theme (shown in the list and matched by the filter), how often it replays, when it peaks, and how it is cast.</summary>
    private static (string Theme, float Every, float Peak, Cast How) SkillLook(FxSkill skill) => skill switch
    {
            FxSkill.CrimsonSlash => ("blade", 1.2f, 0.09f, Cast.Front),
            FxSkill.CrossSlash => ("blade", 1.2f, 0.2f, Cast.Front),
            FxSkill.RisingCleave => ("blade", 1.2f, 0.1f, Cast.Side),
            FxSkill.Whirlwind => ("blade", 2.4f, 0.5f, Cast.Follow),
            FxSkill.PhantomDash => ("shadow", 1.6f, 0.21f, Cast.Dash),
            FxSkill.Shockwave => ("earth", 2f, 0.18f, Cast.Front),
            FxSkill.ArcaneBurst => ("arcane", 1.8f, 0.5f, Cast.Front),
            FxSkill.Singularity => ("arcane", 3.2f, 1.5f, Cast.Front),
            FxSkill.StasisDome => ("arcane", 3.6f, 1.2f, Cast.Front),
            FxSkill.GlacialBurst => ("frost", 2.5f, 0.4f, Cast.Front),
            FxSkill.FlamePillar => ("fire", 2.2f, 0.65f, Cast.Front),
            FxSkill.Judgment => ("holy", 2.2f, 0.68f, Cast.Front),
            FxSkill.ThunderCall => ("storm", 2.2f, 0.62f, Cast.Front),
            FxSkill.ToxicNova => ("nature", 2f, 0.3f, Cast.Front),
            FxSkill.ShadowBlink => ("shadow", 1.5f, 0.15f, Cast.Blink),
            FxSkill.Sanctuary => ("holy", 3.8f, 1.5f, Cast.Front),
            FxSkill.LevelUp => ("buff", 2f, 0.3f, Cast.Follow),
            FxSkill.Recall => ("buff", 3.2f, 2.44f, Cast.Follow),
            FxSkill.Bloodrage => ("buff", 3f, 1f, Cast.Follow),
            FxSkill.CriticalStrike => ("blade", 1f, 0.06f, Cast.Front),
            FxSkill.ThousandCuts => ("blade", 1.4f, 0.45f, Cast.Front),
            FxSkill.Execution => ("blade", 1.4f, 0.14f, Cast.Side),
            FxSkill.BladeDance => ("blade", 1.4f, 0.4f, Cast.Front),
            FxSkill.Lunge => ("blade", 1.2f, 0.1f, Cast.Line),
            FxSkill.GroundWave => ("blade", 1.4f, 0.3f, Cast.Line),
            FxSkill.Riposte => ("blade", 1.2f, 0.08f, Cast.Front),
            FxSkill.LeapSlam => ("earth", 1.6f, 0.4f, Cast.Dash),
            FxSkill.Grapple => ("blade", 1.2f, 0.22f, Cast.Line),
            FxSkill.EarthenWall => ("earth", 2.2f, 0.4f, Cast.Front),
            FxSkill.MagmaFissure => ("fire", 2.6f, 0.6f, Cast.Line),
            FxSkill.Quake => ("earth", 2f, 0.85f, Cast.Front),
            FxSkill.BoulderToss => ("earth", 1.8f, 0.53f, Cast.Line),
            FxSkill.StoneArmor => ("earth", 3f, 1f, Cast.Follow),
            FxSkill.InfernoRing => ("fire", 2.6f, 0.8f, Cast.Front),
            FxSkill.DragonBreath => ("fire", 1.8f, 0.6f, Cast.Line),
            FxSkill.FireRain => ("fire", 2.6f, 1f, Cast.Front),
            FxSkill.BlazingDash => ("fire", 1.6f, 0.25f, Cast.Dash),
            FxSkill.Combustion => ("fire", 2f, 0.6f, Cast.Front),
            FxSkill.EmberMark => ("fire", 2.2f, 1.55f, Cast.Front),
            FxSkill.Blizzard => ("frost", 3.4f, 1.5f, Cast.Front),
            FxSkill.IcePrison => ("frost", 2.6f, 0.5f, Cast.Front),
            FxSkill.FrostLance => ("frost", 1.6f, 0.36f, Cast.Line),
            FxSkill.GlacierPath => ("frost", 2.6f, 0.6f, Cast.Line),
            FxSkill.FrozenOrb => ("frost", 2.2f, 0.9f, Cast.Line),
            FxSkill.ChainLightning => ("storm", 1.2f, 0.3f, Cast.Line),
            FxSkill.StaticField => ("storm", 2.8f, 1f, Cast.Front),
            FxSkill.BallLightning => ("storm", 1.8f, 0.7f, Cast.Line),
            FxSkill.Thunderclap => ("storm", 1.6f, 0.34f, Cast.Front),
            FxSkill.Tempest => ("storm", 3f, 1.2f, Cast.Front),
            FxSkill.Thornbind => ("nature", 2.4f, 0.6f, Cast.Front),
            FxSkill.VenomSpit => ("nature", 2f, 0.56f, Cast.Line),
            FxSkill.PlagueCloud => ("nature", 3.6f, 1.5f, Cast.Front),
            FxSkill.Regrowth => ("nature", 2f, 0.5f, Cast.Follow),
            FxSkill.Swarm => ("nature", 2f, 0.8f, Cast.Line),
            FxSkill.ArcaneMissiles => ("arcane", 1.6f, 0.5f, Cast.Line),
            FxSkill.ManaShield => ("arcane", 3.4f, 1f, Cast.Follow),
            FxSkill.RiftBeam => ("arcane", 1.6f, 0.5f, Cast.Line),
            FxSkill.GravityWell => ("arcane", 2.6f, 1f, Cast.Front),
            FxSkill.Portal => ("arcane", 2.6f, 1f, Cast.Line),
            FxSkill.PrismaticSpray => ("arcane", 1.2f, 0.2f, Cast.Line),
            FxSkill.DivineShield => ("holy", 3.4f, 1f, Cast.Follow),
            FxSkill.Consecration => ("holy", 3.6f, 1.5f, Cast.Front),
            FxSkill.HolyNova => ("holy", 1.6f, 0.2f, Cast.Front),
            FxSkill.SeraphWings => ("holy", 2.4f, 0.8f, Cast.Follow),
            FxSkill.SacredLances => ("holy", 1.8f, 0.5f, Cast.Front),
            FxSkill.ShadowClaws => ("shadow", 1.1f, 0.1f, Cast.Front),
            FxSkill.SoulDrain => ("shadow", 2f, 0.8f, Cast.Line),
            FxSkill.DeathCoil => ("shadow", 1.6f, 0.42f, Cast.Line),
            FxSkill.ShadeClones => ("shadow", 1.4f, 0.35f, Cast.Front),
            FxSkill.Curse => ("shadow", 2.6f, 1f, Cast.Front),
        _ => ("shadow", 2f, 0.5f, Cast.Front),
    };

    /// <summary>A skill's entry in the list.</summary>
    private static Entry SkillEntry(FxSkill skill)
    {
        var (theme, every, peak, how) = SkillLook(skill);
        var name = string.Concat(skill.ToString().Select((ch, i) => i > 0 && char.IsUpper(ch) ? " " + char.ToLowerInvariant(ch) : ch.ToString()));
        var call = $"Fx.Skill(FxSkill.{skill}";
        var lands = FxTiming.SkillHit(skill) > 0f ? "  // lands after FxTiming.SkillHit" : "";
        return how switch
        {
            Cast.Dash => new("Skill", name, $"{call}, from, to - from, 1.5f);{lands}", every, peak,
                g => g.Travel(skill, skill == FxSkill.LeapSlam ? 3f : 1.5f, FxTiming.SkillHit(skill), smooth: true), theme),
            Cast.Blink => new("Skill", name, $"{call}, from, to - from);{lands}", every, peak,
                g => g.Travel(skill, 3f, FxTiming.SkillHit(skill), smooth: false), theme),
            Cast.Follow => new("Skill", name, $"{call}, caster.Position, facing, 3f, follow: caster);", every, peak,
                g => Fx.Skill(skill, g._caster.Position, Vector3.Back, 3f, g._caster), theme),
            Cast.Line => new("Skill", name, $"{call}, caster.Position, target - caster.Position, 3f);{lands}", every, peak,
                g =>
                {
                    g._caster.Position = new Vector3(-3.5f, 0f, 1f);
                    Fx.Skill(skill, g._caster.Position, Vector3.Right * 7f, 3f);
                }, theme),
            // Upright along its facing, so it is cast across the camera's view.
            Cast.Side => new("Skill", name, $"{call}, at, facing, 3f);{lands}", every, peak, _ => Fx.Skill(skill, Vector3.Zero, Vector3.Right, 3f), theme),
            _ => new("Skill", name, $"{call}, at, facing, 3f);{lands}", every, peak, _ => Fx.Skill(skill, Vector3.Zero, Vector3.Back, 3f), theme),
        };
    }

    /// <summary>A dash or blink across the stage: the caster runs (or jumps, at <paramref name="arrive"/>) from left to right.</summary>
    private void Travel(FxSkill skill, float radius, float arrive, bool smooth)
    {
        var from = new Vector3(-4f, 0f, 1f);
        var to = new Vector3(4f, 0f, 1f);
        var age = 0f;
        _caster.Position = from;
        Fx.Skill(skill, from, to - from, radius);
        Run(dt =>
        {
            age += dt;
            var k = Mathf.Min(age / arrive, 1f);
            _caster.Position = smooth ? from.Lerp(to, k) : k < 1f ? from : to;
            return age <= arrive + 0.5f;
        });
    }

    /// <summary>A shield charge or rolling stone, run across the stage by the caster.</summary>
    private void Charge(FxRam look)
    {
        var from = new Vector3(-6f, 0f, 1f);
        var age = 0f;
        const float Life = 1f;
        _caster.Position = from;
        Fx.Ram(look, () => age <= Life ? _caster.Position : null, Vector3.Right, Life);
        Run(dt =>
        {
            age += dt;
            _caster.Position = from + Vector3.Right * 12f * Mathf.Min(age / Life, 1f);
            return age <= Life + 0.5f;
        });
    }

    /// <summary>An aura renewed every half second, as a game's aura skill would.</summary>
    /// <summary>An aura's sigil renewed every half second for three seconds, then left to fade.</summary>
    private sealed class Sigil(Node3D caster, FxSigil sigil)
    {
        private float _next;
        private float _age;

        public bool Step(float dt)
        {
            _age += dt;
            if (_age < 3f && (_next -= dt) <= 0f)
            {
                _next = 0.5f;
                Fx.Sigil(caster, sigil, 1.6f, 1f);
            }
            return _age < 3f;
        }
    }

    /// <summary>
    /// Several ring sigils on one caster, each arriving a fifth of a second after the last and
    /// renewed every half second; the first leaves early, so one can be seen going while others stay.
    /// </summary>
    private sealed class Stack(Node3D caster, FxSigil[] sigils)
    {
        private float _next;
        private float _age;

        public bool Step(float dt)
        {
            _age += dt;
            if ((_next -= dt) <= 0f)
            {
                _next = 0.1f;
                for (var i = 0; i < sigils.Length; i++)
                {
                    var until = i == 0 && sigils.Length > 1 ? 3f : 3.8f;
                    if (_age >= i * 0.2f && _age < until)
                        Fx.Sigil(caster, sigils[i], 1.4f, 0.5f, FxSigilLook.Ring);
                }
            }
            return _age < 3.8f;
        }
    }

    /// <summary>A blade vortex renewed every quarter second, its blades rising from <paramref name="from"/> to <paramref name="to"/>, then left to fly out.</summary>
    private sealed class Vortex(Node3D caster, int from, int to, FxVortexLook look = FxVortexLook.Steel)
    {
        private float _next;
        private float _age;

        public bool Step(float dt)
        {
            _age += dt;
            if (_age < 3.5f && (_next -= dt) <= 0f)
            {
                _next = 0.25f;
                var blades = Mathf.Min(to, from + (int)(_age / 0.28f));
                Fx.BladeVortex(caster, blades, 1.6f, 0.5f, look);
            }
            return _age < 3.5f;
        }
    }

    private sealed class Aura(Node3D caster, Color color, FxGround look, float radius)
    {
        private float _next;
        private float _age;

        public bool Step(float dt)
        {
            _age += dt;
            if (_age < 3f && (_next -= dt) <= 0f)
            {
                _next = 0.5f;
                Fx.Aura(caster, radius, 1f, color, look);
            }
            return _age < 3f;
        }
    }

    /// <summary>A barrier struck from somewhere round it every so often, giving way at the end.</summary>
    private sealed class Barrier(FxBarrier barrier)
    {
        private float _next = 0.5f;
        private float _age;

        public bool Step(float dt)
        {
            if (!barrier.Alive)
                return false;
            _age += dt;
            if (_age >= 2.6f)
            {
                barrier.Break();
                return false;
            }
            if ((_next -= dt) > 0f)
                return true;
            _next = 0.45f;
            // Mostly from the camera's side, so the ripples show.
            var from = Vector3.Back.Rotated(Vector3.Up, (float)GD.RandRange(-1.3, 1.3)) * 4f + Vector3.Up * (float)GD.RandRange(0.4, 1.6);
            barrier.Hit(from);
            return true;
        }
    }

    /// <summary>An arc chaining along <paramref name="points"/>: one jump every <see cref="FxTiming.ArcLeap"/>.</summary>
    private sealed class Chain(Vector3[] points, float burst)
    {
        private int _jump;
        private float _next;

        public bool Step(float dt)
        {
            // Every jump that is due, so a long frame doesn't hold the chain back.
            for (_next -= dt; _next <= 0f; _next += FxTiming.ArcLeap)
            {
                Fx.Arc(points[_jump], points[_jump + 1], burst);
                if (++_jump >= points.Length - 1)
                    return false;
            }
            return true;
        }
    }

    /// <summary>A storm orb's strikes: every half second a bolt from its heart to a spot under it.</summary>
    private sealed class Orb(FxOrb orb)
    {
        private float _next = 0.25f;

        public bool Step(float dt)
        {
            if (!orb.Alive)
                return false;
            if ((_next -= dt) > 0f)
                return true;
            _next = 0.5f;
            var spot = Vector3.Forward.Rotated(Vector3.Up, GD.Randf() * Mathf.Tau) * 4f * Mathf.Sqrt(GD.Randf());
            Fx.Lightning([(orb.Heart - Vector3.Up * Fx.LineLift, spot)], 0.16f);
            return true;
        }
    }
}
