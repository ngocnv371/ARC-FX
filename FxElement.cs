using Godot;

namespace Arc.Fx;

/// <summary>What an effect is made of. Picks its colour from <see cref="FxTheme"/> and, for some effects, its shape.</summary>
public enum FxElement { Physical, Fire, Frost, Lightning, Poison, Holy, Spirit }

/// <summary>
/// The ARC palette: one colour per element, so every game's fire is the same fire. Colours are
/// display (sRGB) values, as the shaders take them. A game may retint an element by assigning
/// to <see cref="Colors"/> at startup, but the point is to leave it alone.
/// </summary>
public static class FxTheme
{
    public static readonly Dictionary<FxElement, Color> Colors = new()
    {
        [FxElement.Physical] = new Color(0.85f, 0.92f, 1f),
        [FxElement.Fire] = new Color(1f, 0.45f, 0.1f),
        [FxElement.Frost] = new Color(0.6f, 0.85f, 1f),
        [FxElement.Lightning] = new Color(0.75f, 0.85f, 1f),
        [FxElement.Poison] = new Color(0.35f, 0.9f, 0.2f),
        [FxElement.Holy] = new Color(1f, 0.78f, 0.35f),
        [FxElement.Spirit] = new Color(0.6f, 0.82f, 1f),
    };

    public static Color Of(FxElement element) => Colors[element];

    /// <summary>The colour a fire effect lights its surroundings with, whatever it is tinted.</summary>
    public static Color FireLight => Colors[FxElement.Fire];
}
