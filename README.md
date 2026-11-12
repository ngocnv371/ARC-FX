# ARC FX

The shared effects library for the ARC games: explosions, burning ground and auras, aura sigils,
lightning, chain lightning arcs and fire walls, slashes, blade vortices, beams, storm orbs, frost bombs, novas, Divine
Ire, slams and spikes, spectral shields, fists and hammers, rams, meteors, bladefalls, heals and barriers,
every projectile look and the puffs dead monsters leave. One call each, in the ARC look, so every
game's fire is the same fire.

Effects only draw. What one does to a game (who it hits, how hard, which status) is the game's:
it calls the effect for the look and does its own hits, timed by `FxTiming` where an effect unfolds.

```csharp
Fx.Explosion(at, radius: 3f, FxElement.Fire);
Fx.Patch(at, 3f, life: 4f, FxElement.Poison);
Fx.Aura(hero, 3.5f, keep: 1f, FxElement.Frost);     // renew it each time the aura ticks
Fx.Sigil(hero, FxSigil.Haste, 1.6f, keep: 1f);      // the disc at their feet that says what it does
Fx.Sigil(hero, FxSigil.Fire, 1.4f, keep: 1f, FxSigilLook.Ring);  // several auras: one ring, a badge each
Fx.Lightning([(at + Vector3.Up * 12f, at)], 0.2f, tendrils: 12, thickness: 0.6f);  // a strike from the sky
Fx.Arc(hero.Position, foe.Position, burst: 1.6f);   // one jump of chain lightning; the next FxTiming.ArcLeap later
Fx.Slash(hero.Position, facing, 3f, arc: 120f, steel);
Fx.BladeVortex(hero, stacks, 1.6f, keep: 1f, FxElement.Fire);  // renew each cast; blood for physical
Fx.Nova(hero.Position, 6f);                         // hit with FxTiming.NovaFront(age, 6f)
Fx.Meteor(at, 3f);                                  // lands after FxTiming.MeteorFall
Fx.Bladefall(at, 4f);                               // spectral swords land over FxTiming.BladefallSpread
var barrier = Fx.Barrier(hero, 1.4f, life: 5f, FxTheme.Of(FxElement.Spirit));
barrier.Hit(attacker.Position);                     // ripples where the blow lands

var shot = Fx.Shot(FxShotLook.Fireball);            // the game flies it:
shot.Place(position, velocity);                     //   every frame
shot.Release();                                     //   when it lands
```

## Setting up

Mount this repo at `addons/arc_fx/` (the shaders find each other by relative path, and C# finds
them through `FxShaders.Root`). Add one `FxWorld` node to the scene. By default it ticks itself;
a game with its own clock (hit-stop, slow motion) sets `AutoTick = false` and calls `Tick(dt)`.

Optional hooks, set once at startup:

| Hook | What for | Left unset |
|---|---|---|
| `Fx.Lighting` (`IFxLighting`) | Pools of light the game's unlit shaders add (fire lights the grass) | Effects still look right, they just don't light the ground |
| `FxModels.Arrow`, `.Axe`, `.Shield`, `.Hammer`, `.RockSpike` | The props a few looks are built round (the library ships no art) | Built-in props for the shield, hammer and rock spike (a heater shield, a war maul, a jagged crag), plain shapes for the rest; all read the same from the game camera |
| `FxTheme.Colors` | Retinting an element | The ARC palette (leave it, for consistency) |

`FxPuffs` (death puffs) and `FxShaft` (a standing column of light) are nodes the game owns and
places itself.

## What's here

| Path | Contents |
|---|---|
| `Fx.cs` | The front door: every effect as one static call |
| `FxWorld.cs` | The node that holds and ticks effects |
| `FxTiming.cs` | When unfolding effects reach things (nova front, shield ring radius, Ire windup, an arc's leap, smite fall, meteor fall, bladefall, rupture speed) |
| `FxElement.cs`, `FxLighting.cs`, `FxModels.cs` | Elements and their palette, the lighting and model hooks |
| `Fx*.cs` | The effects, one family per file; `FxKit.cs` holds what they share and `FxHandle`, the handle most calls return |
| `shaders/` | The `fx_*` shaders; `fx_common.gdshaderinc` and `fx_noise.gdshaderinc` are their shared pieces |
| `gallery/` | ARC FX Gallery, a Godot app that plays every effect with the call that makes it |

## The gallery

`gallery/run.cmd` opens ARC FX Gallery: every effect on a plain floor under the game camera,
replaying every few seconds, with its call written underneath. Up and Down pick one, Space replays
it, V switches to a close look, F changes the floor, - and + step the speed down to 0.05× and
back (P pauses) to follow fast things like arrows. `run.cmd --shot=dir` saves a picture of each
effect as it peaks and quits (`--only=explosion_fire,storm_nova` for a few), in a window that never
takes focus. It needs a Godot 4.7 .NET editor (`GODOT`, or the newest under `D:\installed`).

The gallery is its own Godot project. It sees the library at `res://addons/arc_fx` through a
junction to this folder that `run.cmd` makes (git can't hold one); the game ignores the gallery
folder because it holds a `project.godot` of its own, and a game's `.csproj` should leave it out:

```xml
<Compile Remove="addons/arc_fx/gallery/**" />
```

## Rules

- Nothing here refers to a game: no game namespaces, no game assets. A game lends what it has
  through the hooks above.
- Colours are display (sRGB) values; the shaders decode them (`fx_out`).
- Pooled effects return handles that go inert once their slot is reused, so holding one is safe.
