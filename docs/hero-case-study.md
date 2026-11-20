# Hero case study: twenty kits, our versions

**Status: built.** All 62 skills below are in the library (`FxSkillsStudy.cs`), called with
`Fx.Skill(FxSkill.<Name>, ...)`, and in the gallery, where the **Case study** chip (or `#study`)
lists them. The four new building blocks are `fx_wall`, `fx_ribbon`, the cone and band styles
of `fx_disc`, and `FxProps.Totem`, `.Trap`, `.Rocket` and `.Lantern`.

A study of the skill effects of twenty heroes, ten from Dota 2 and ten from League of Legends,
picked for kits that are iconic and that look unlike each other. For each skill: what you see,
what makes it read in a fight, and the version we would build.

Our versions take the **mechanic** and **what makes it read** (its shape, timing and colour logic),
never the art, the names or the exact look. Every name below is ours. Where one of our 70
existing skills already covers a skill, the version is marked as a variant of it, or skipped.

Notes are from knowledge of the games, not from frame-by-frame footage, so details of timing are
approximate.

Legend for **Our version**: **new** needs nothing we lack; **new+** needs a new building block
(see the end); **variant** reworks one of our skills; **skip** is a passive or isn't an effect.

---

## Dota 2

### 1. Lina, the fire caster

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Dragon Slave | A wave of fire rolling out in a line, widening as it goes | One shape moving one way; widening shows the hit area growing | **Cinder Wave**, new+: a flame front skimming along a widening cone, scorch left in a wedge (needs a cone decal) |
| Light Strike Array | A ring appears, a beat later a short eruption of fire | Telegraph, delay, payoff: the ring is the warning, the burst lands exactly in it | **Pyre Strike**, variant of Flame Pillar: tighter ring, quicker beat, a squat explosive burst instead of a tall column |
| Fiery Soul | Flames wreathing the hands, stacking | A buff told by body flames, not by a ground marker | skip (passive buff look; Bloodrage covers it) |
| Laguna Blade | An instant, jagged red bolt from hand to target, a hard flash on impact | A familiar form (lightning) in an unfamiliar colour (red) is its whole identity | **Crimson Verdict**, new: thick red-and-black lightning beam, white core, impact flash, embers |

### 2. Crystal Maiden, the frost support

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Crystal Nova | A burst of ice at a spot, a frost circle left | Instant circle, crisp edge, lingering frost shows where it hit | variant of Glacial Burst: no spikes, a snowflake rune in the frost |
| Frostbite | The target encased in an ice block | The target itself changes: you can see they're stuck | **Rime Shackle**, new: an ice crystal cluster rising round the target, holding, then shattering |
| Arcane Aura | Nothing in fight | | skip |
| Freezing Field | A wide snowstorm round the caster, ice explosions popping at random inside it | The caster is the centre; random pops in a fixed radius say "don't stand here" | **Frozen Expanse**, new: snow dome edge, frost ground, small frost blasts at random spots for 3 s, following the caster |

### 3. Zeus, the lightning caster

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Arc Lightning | A bolt jumping between nearby enemies | The path draws the targets | skip (Chain Lightning) |
| Lightning Bolt | One bolt from a cloud to a spot | Sky-to-ground in one frame, flash on the ground | variant of Thunderclap: a small cloud swirl overhead first, a thinner bolt, no nova |
| Static Field | Sparks on enemies hit | Passive | skip |
| Thundergod's Wrath | Bolts on every enemy hero across the map at once, the caster charging up | Global: the caster's charge-up warns, every target lights at once | **Heaven's Wrath**, new: caster charge (rising sparks, crackling column), then bolts on several marked targets at once |

### 4. Invoker, the spell scholar

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Sun Strike | A narrow ring on the ground for a second or so, then a thin intense beam from the sky | The warning is faint so it can be dodged, the strike is blinding | **Solar Brand**, new: a thin gold ring that narrows over 1.5 s, then a needle beam and a gold flare |
| Chaos Meteor | A meteor lands and rolls forward, burning a trail | A thing that keeps moving after it lands; the trail shows where it went | **Rolling Cinderstone**, new: a burning boulder lands (our Meteor), then rolls along facing, leaving burning ground and sparks |
| Deafening Blast | A broad arc of force rolling out in front | A wide wavefront pushing things back | **Concussive Wave**, new: a wide pale-blue crescent wall rolling out, dust kicked up along its foot |
| Ice Wall | A row of ice blocks across in front | A barrier: perpendicular, solid, lasting | **Rime Rampart**, new+: a line of ice blocks across the way (needs a standing wall piece) |

### 5. Juggernaut, the swordmaster

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Blade Fury | The hero spinning inside a whirling blade storm | A column of blades round the body, not crescents in the air | **Steel Cyclone**, new: a whirling column of streaks and steel round the caster, sparks flung off, following them |
| Healing Ward | A small totem pulsing green | An object you can see and protect | **Mending Totem**, new+: a small spectral totem with green pulses rolling out (needs a totem prop) |
| Blade Dance | Crit hits | | skip (Critical Strike) |
| Omnislash | The hero blinking from target to target, a slash mark at each | Afterimages and slash marks scattered, the hero untouchable | **Thousand Steps**, new: a sequence of blinks between points, a slash and a ghost at each, a final flourish |

### 6. Faceless Void, the time warrior

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Time Walk | A fast dash with a ghostly trail | Movement told by its trail | variant of Phantom Dash: a blue-grey trail with clock-hand streaks, no slash |
| Time Dilation | A ripple, cooldowns frozen | | skip |
| Time Lock | A strike that freezes the target for a beat | A flash, then stillness: a ring of clock marks round the target | **Stilled Moment**, new: a hit flash, then a clock ring standing round the target for a moment |
| Chronosphere | A large dome where everything inside stops | A sharp edge and frozen bits inside | variant of Stasis Dome: bigger, a clock-face disc with turning hands, motes stopped mid-fall |

### 7. Earthshaker, the earth breaker

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Fissure | A ridge of rock tearing up in a line, blocking the way | A line that is also a wall: you can't walk through it | **Riven Ridge**, new+: a continuous rock ridge rising along facing, dust, lingering, then crumbling (needs the wall piece in rock) |
| Enchant Totem | The hero's totem glowing, a big next hit | | skip |
| Aftershock | A dust ring at the feet on each cast | A small tell that repeats | **Tremor Ring**, new: a quick dust ring and cracks at the feet |
| Echo Slam | A slam, then echo waves going out to each enemy and bouncing | The number of echoes shows how strong it is | **Resounding Slam**, new: the slam, then rings travelling to several points, each sending out its own smaller ring |

### 8. Puck, the fey trickster

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Illusory Orb | A slow glowing orb you can teleport to | The orb is a place you might be | **Drifting Mote**, new: a slow pastel orb with a long glittering trail |
| Waning Rift | A blink with a burst of sparkles | Pastel colours make it read as playful, not dangerous | **Fey Rift**, new: a blink with a pastel sparkle burst and a short silence ring |
| Phase Shift | The hero vanishing briefly | | skip (Shadow Blink) |
| Dream Coil | A coil at a spot, tethers to each enemy caught, snapping if they leave | The tethers say who is caught; the snap punishes | **Dreamtether**, new: a central coil, tethers to several points, a snap flash at the end |

### 9. Lion, the demonic sorcerer

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Earth Spike | A line of spikes erupting one after another | The order of the spikes shows the direction | variant of Rupture: spikes that curve and thicken toward the end, a dust plume at each |
| Hex | The target turned into a harmless animal in a puff | A puff of smoke hides the change | **Polymorph Puff**, new: a puff of pastel smoke and sparkles (the change itself is the game's) |
| Mana Drain | A tether pulling energy back | Flow direction along the tether | skip (Soul Drain) |
| Finger of Death | A jagged green-black beam from the finger, a heavy blast on the target | Overkill: it's meant to look like too much | **Sundering Finger**, new: a forked dark-green beam, spectral claws closing on the target, a heavy blast |

### 10. Phoenix, the fire bird

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Icarus Dive | A swooping dive out and back, leaving a ribbon of fire | A path drawn in fire, a loop | **Firewing Dive**, new+: an arcing dash out and back with a fire ribbon along it (needs a ribbon trail) |
| Fire Spirits | Spirits orbiting the hero, launched one by one | Charges you can count | **Ember Wisps**, new: four wisps orbiting, launched as arcing fireballs at a spot |
| Sun Ray | A long beam held and turned slowly | A long-lasting, turnable beam | **Solar Lance**, new: a long sustained beam, wobbling and pulsing, hot at the source |
| Supernova | The hero becomes a sun, then explodes | A countdown you can see: the sun swells | **Reborn Star**, new: a blazing sun with orbiting rings, swelling, then a huge fire blast |

---

## League of Legends

### 11. Yasuo, the wind swordsman

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Steel Tempest | A quick thrust; the third throws a tornado | Stacks shown by the look changing | **Gale Thrust**, new: our Lunge in wind colours; a third cast sends a tornado (our Tornado shot) |
| Wind Wall | A curved standing wall of wind that blocks shots | A curved barrier that stands for a few seconds | **Gale Barrier**, new+: a curved standing wall of streaming wind (needs the wall piece) |
| Sweeping Blade | A dash through a target with a swoosh | Short, sharp, through something | variant of Phantom Dash: blue-white wind streaks, no afterimages |
| Last Breath | A target lifted, a flurry of slashes in the air | Air is the stage: everything happens above the ground | **Skyward Cut**, new: a lifting gust, a flurry of slashes overhead, a downward finish |

### 12. Ahri, the fox mage

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Orb of Deception | An orb thrown out and returning | The return path is part of the skill | **Returning Orb**, new: an orb out and back, blue going out and white coming back |
| Fox-Fire | Wisps orbiting, then darting to targets | Orbit, then pounce | **Spirit Wisps**, new: three wisps orbiting the caster, then darting off to points |
| Charm | A kiss-shaped projectile | A shape that tells the effect | **Beguiling Mote**, new: a rose-pink orb with a ring glyph, a pink burst on hit (our shape, not a heart) |
| Spirit Rush | Three short dashes, bolts fired at each | Repetition: three beats | **Spirit Steps**, new: three short dashes, three bolts from each stop |

### 13. Lux, the light mage

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Light Binding | A light orb; on hit, light rings bind the target | Bound = rings round the body | **Bind of Dawn**, new: a light orb, then three light rings closing round the target |
| Prismatic Barrier | A wand thrown out and back, shielding allies it passes | A projectile that helps | **Prism Ward**, new: a prismatic spinning shard out and back, a shield bubble flashing at its turn |
| Lucent Singularity | A glowing zone that slows, then detonates | A lingering zone with a second press to pop | **Lingering Star**, new: a thrown star lands, a glowing zone pulses, then bursts |
| Final Spark | A thin line, then a huge long laser | Warning line, then massive beam: telegraph and payoff at the largest scale | **Radiant Lance**, new: a thin aiming line for 1 s, then a huge white-gold beam a long way, sparkles all along it |

### 14. Jinx, the gunner

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Zap | A charged laser shot | A charge glow, then a line | **Shock Line**, new: a charge glow, then a crackling cyan line shot |
| Flame Chompers | Traps tossed down, biting and bursting | Objects on the ground waiting | **Snap Traps**, new+: three small traps tossed, snapping shut with a fire burst (needs a trap prop) |
| Switcheroo | A weapon swap | | skip |
| Super Mega Death Rocket | A huge rocket crossing the map, a big blast | A projectile big enough to see coming | **Grand Rocket**, new+: a rocket with a smoke and fire trail flying a long way, a big blast (needs a rocket prop) |

### 15. Thresh, the jailer

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Death Sentence | A hooked chain thrown and pulled back | The chain is the hit area and the pull | variant of Grapple: ghostly green chain, a hook flash, a pull back |
| Dark Passage | A lantern thrown, allies pulled in | A thing you click to be saved | **Lantern Toss**, new+: a green lantern arcing out and landing glowing (needs a lantern prop) |
| Flay | A sweep of the chain, knocking back | A swipe along a line | **Chain Sweep**, new: a green lash arcing across in front |
| The Box | A pentagon of walls trapping enemies | Walls on every side: you're trapped | **Spectral Cage**, new+: five green standing walls in a ring, cracking when broken (needs the wall piece) |

### 16. Ezreal, the explorer

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Mystic Shot | A quick bolt | A small fast projectile | **Arcane Bolt**, new: a quick gold bolt with a short trail and a pop |
| Essence Flux | A slow orb marking enemies | A mark on the target | skip (Ember Mark covers marks) |
| Arcane Shift | A short blink, a bolt fired at the nearest enemy | Blink and shoot in one | **Shift Shot**, new: a short blink with a homing bolt from where you land |
| Trueshot Barrage | A wide band of energy crossing the map | Wide and long: hard to dodge | **Longshot Barrage**, new: a wide golden band rolling a long way with a charge-up first |

### 17. Malphite, the living rock

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Seismic Shard | A rock shard thrown | A heavy thrown thing | skip (Boulder Toss) |
| Granite Shield | A rock shield | | skip |
| Ground Slam | A shockwave | | skip (Shockwave) |
| Unstoppable Force | A charge that becomes rock and throws everyone up | Nothing stops it: a straight line and an eruption | **Landslide Charge**, new: a rock-form charge along facing (a boulder rolling), an eruption of rubble and spikes where it ends |

### 18. Annie, the fire child

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Disintegrate | A fireball | | skip (our fire shots) |
| Incinerate | A cone of fire | A cone: one shape, short and wide | **Flare Fan**, new+: a burst of fire in a cone, scorched wedge on the ground (needs the cone decal) |
| Molten Shield | Flames swirling round the hero | A defensive swirl | **Ember Shroud**, new: a swirl of fire ribbons round the caster, following them |
| Summon | A summoned guardian lands in a fiery blast | The landing is the hit | **Guardian's Fall**, new: something heavy lands in a fire blast with a rune circle (the guardian itself is the game's) |

### 19. Garen, the soldier

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Decisive Strike | A silencing hit | | skip |
| Courage | A brief guard | | skip |
| Judgment | Spinning with the sword | A spin that lasts | skip (Whirlwind) |
| Demacian Justice | A giant sword falling from the sky onto the target | Sky-to-target: the size of the sword is the size of the hit | **Sword of Verdict**, new: one giant spectral sword (our Bladefall sword) falling into the target, a gold pillar and blast |

### 20. Morgana, the fallen angel

| Skill | What you see | Why it reads | Our version |
|---|---|---|---|
| Dark Binding | A dark orb; the target bound in chains | Bound = chains round the body | **Umbral Bind**, new: a dark orb, then dark chains wrapping the target |
| Tormented Shadow | A dark pool that hurts | A lasting pool | **Blight Pool**, new: a dark violet pool with drifting spirits |
| Black Shield | A dark shield on an ally | A spell shield | **Umbral Ward**, new: a dark violet shield bubble with runes |
| Soul Shackles | Tethers to nearby enemies, tightening, then a stun | Tethers fade in strength; the snap is the payoff | **Soul Fetters**, new: dark tethers to several points, glowing brighter, then snapping with bursts |

---

## What the twenty kits have in common

1. **Telegraph, delay, payoff.** Light Strike Array, Sun Strike, Final Spark and Lucent Singularity
   all warn before they hit. The warning is faint and exact; the payoff is loud and lands exactly
   inside it. Ours should keep the warning faint so the payoff contrasts.
2. **One dominant shape.** Each skill has one: a wave (Dragon Slave), a beam (Laguna Blade), a ring
   (Crystal Nova), a dome (Chronosphere), a line (Fissure), a cone (Incinerate). Detail sits on
   that shape, never competes with it.
3. **Colour logic.** White-hot core, saturated element colour, dark or transparent edge. Identity
   often comes from a familiar shape in an unexpected colour (Laguna Blade's red lightning).
4. **Anticipation and residue.** A wind-up before (Thundergod's Wrath, Supernova, Sun Ray), and
   something left after (scorch, frost, cracks) that says where it landed.
5. **The edge is the hitbox.** Areas have crisp edges (Chronosphere, Freezing Field, The Box);
   our effects should draw their radius sharply, matching what the game hits.
6. **Things that stay.** Walls, totems, traps, lanterns and pools are objects on the field, not
   flashes. We have almost none of these.
7. **Paths drawn.** Returning orbs, dives and dashes draw their path (Ahri's orb, Icarus Dive).
   Our streaks only hint at a path; a ribbon would draw it.
8. **Charges you can count.** Fire Spirits, Fox-Fire, Spirit Rush, Steel Tempest's third cast:
   the number is part of the look.

## Building blocks we lack

| Block | Used by | What it is |
|---|---|---|
| **Wall** | Rime Rampart, Riven Ridge, Gale Barrier, Spectral Cage | An upright sheet along a segment (energy, wind, ice blocks or rock), rising, standing, breaking |
| **Ribbon trail** | Firewing Dive, Ember Shroud, Returning Orb, dashes | A strip following a moving point, fading along its length |
| **Cone decal** | Cinder Wave, Flare Fan | A wedge on the ground, the cone's hit area, scorching or glowing |
| **Small props** | Mending Totem, Snap Traps, Grand Rocket, Lantern Toss | A totem, a trap, a rocket, a lantern: simple meshes built in code, like our sword and shield |

## Proposed build

Everything marked new or new+ above: **54 skills**, plus **8 variants** of ours, 62 in all.
Built in this order, so each block is ready before the skills that need it:

1. The four building blocks.
2. The 43 **new** skills, built from what we have.
3. The 11 skills that need the new blocks (**new+**), then the 8 **variants**.
