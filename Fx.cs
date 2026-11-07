using Godot;

namespace Arc.Fx;

/// <summary>
/// The front door: spawn an effect by name and forget it. Every call draws only; what an
/// effect does to the game (damage, statuses, knockback) is the caller's business.
/// Calls go to the <see cref="FxWorld"/> in the tree; with none, they do nothing.
/// <code>
/// Fx.Explosion(at, radius: 3f, FxElement.Fire);
/// </code>
/// </summary>
public static class Fx
{
    /// <summary>The scene's effects, set when an <see cref="FxWorld"/> enters the tree.</summary>
    public static FxWorld? World { get; internal set; }

    /// <summary>Where effects report the light they give off; null for none.</summary>
    public static IFxLighting? Lighting { get; set; }

    /// <summary>
    /// A blast at <paramref name="at"/> reaching <paramref name="radius"/>. Fire swells into a
    /// globe of flame that pops into a rolling ring and leaves a scorch; frost into a crystal globe
    /// that shatters into a ring of ice over hoarfrost; lightning into a crackling globe over a
    /// small nova. Anything else is a churning dome over a shockwave in the element's colour.
    /// </summary>
    public static void Explosion(Vector3 at, float radius, FxElement element) =>
        World?.Explosion(at, radius, FxTheme.Of(element), FxExplosions.BurstOf(element));

    /// <summary>An explosion in any colour, shaped as <paramref name="burst"/>.</summary>
    public static void Explosion(Vector3 at, float radius, Color color, FxBurst burst = FxBurst.Dome) =>
        World?.Explosion(at, radius, color, burst);

    /// <summary>
    /// A patch of ground at <paramref name="at"/> for <paramref name="life"/> seconds: burning
    /// ground for fire, hoarfrost for frost, a bubbling pool for poison, otherwise a glow in the
    /// element's colour. The handle moves, keeps or ends it.
    /// </summary>
    public static FxPatch Patch(Vector3 at, float radius, float life, FxElement element) =>
        World?.Patch(at, radius, life, FxTheme.Of(element), FxPatches.GroundOf(element)) ?? default;

    /// <summary>A patch of ground in any colour and look.</summary>
    public static FxPatch Patch(Vector3 at, float radius, float life, Color color, FxGround look = FxGround.Glow) =>
        World?.Patch(at, radius, life, color, look) ?? default;

    /// <summary>
    /// A patch that rides on <paramref name="follow"/> for <paramref name="keep"/> more seconds.
    /// Calling again for the same node and look refreshes that one, so an aura can be renewed
    /// every time it ticks.
    /// </summary>
    public static FxPatch Aura(Node3D follow, float radius, float keep, FxElement element) =>
        World?.Aura(follow, radius, keep, FxTheme.Of(element), FxPatches.GroundOf(element)) ?? default;

    /// <summary>
    /// An aura in any colour and look; <see cref="FxGround.Inferno"/> raises a bubble of fire over it,
    /// <see cref="FxGround.Miasma"/> a dome of poison vapour.
    /// </summary>
    public static FxPatch Aura(Node3D follow, float radius, float keep, Color color, FxGround look = FxGround.Glow) =>
        World?.Aura(follow, radius, keep, color, look) ?? default;

    /// <summary>
    /// An aura's sigil at <paramref name="follow"/>'s feet for <paramref name="keep"/> more seconds:
    /// a disc of light <paramref name="radius"/> round, its rings and runes slowly turning round
    /// the emblem of what the aura does, in <see cref="FxTheme.Of(FxSigil)"/>'s colour. Calling
    /// again for the same node and sigil renews that one; once nothing does, it fades.
    /// <para>
    /// For a character who may wear several auras at once, use <see cref="FxSigilLook.Ring"/>:
    /// all of them then share one ring of runes <paramref name="radius"/> round (the widest asked
    /// for) with nothing in the middle, and each aura adds a small badge with its emblem just
    /// outside it, at that sigil's own angle, so they sit side by side and never overlap.
    /// </para>
    /// </summary>
    public static FxHandle Sigil(Node3D follow, FxSigil sigil, float radius, float keep, FxSigilLook look = FxSigilLook.Disc) =>
        World?.Sigil(follow, sigil, radius, keep, FxTheme.Of(sigil), look) ?? default;

    /// <summary>An aura's sigil in any colour.</summary>
    public static FxHandle Sigil(Node3D follow, FxSigil sigil, float radius, float keep, Color color, FxSigilLook look = FxSigilLook.Disc) =>
        World?.Sigil(follow, sigil, radius, keep, color, look) ?? default;

    /// <summary>
    /// Lightning along each segment for <paramref name="life"/> seconds: jagged strands that
    /// flicker as they reshape, with <paramref name="tendrils"/> tiny twigs off each. Drawn
    /// <see cref="LineLift"/> above both ends, so a segment along the ground is a fence at chest
    /// height and one from the sky to a spot on the ground is a strike.
    /// </summary>
    public static FxHandle Lightning((Vector3 A, Vector3 B)[] segments, float life, int tendrils = 0) =>
        World?.Lightning(segments, life, tendrils) ?? default;

    /// <summary>A wall of flame tongues along each segment, over a strip of burning ground, for <paramref name="life"/> seconds.</summary>
    public static FxHandle FireWall((Vector3 A, Vector3 B)[] segments, float life) =>
        World?.FireWall(segments, life) ?? default;

    /// <summary>How far above their ends lightning is drawn.</summary>
    public const float LineLift = FxLines.Lift;

    /// <summary>
    /// A streak through the air at chest height: a fan <paramref name="radius"/> across and
    /// <paramref name="arc"/> degrees wide round <paramref name="forward"/>, swept right to left.
    /// It rides on <paramref name="follow"/> if given.
    /// </summary>
    public static FxHandle Slash(Vector3 centre, Vector3 forward, float radius, float arc, Color color, Node3D? follow = null) =>
        World?.Slash(centre, forward, radius, arc, color, follow) ?? default;

    /// <summary>
    /// A blade vortex round <paramref name="follow"/> for <paramref name="keep"/> more seconds:
    /// <paramref name="blades"/> small blades (up to <see cref="BladeVortexMax"/>) whirling round
    /// them within <paramref name="radius"/>, each on its own tilted orbit trailing a streak, made
    /// as <paramref name="look"/> says: steel, blood, fire, frost and so on. Calling again for the
    /// same node renews it and sets its blade count, so stacks can grow and shrink (new blades
    /// swell in, dropped ones shrink away); once nothing renews it, the blades fly out and go.
    /// </summary>
    public static FxHandle BladeVortex(Node3D follow, int blades, float radius, float keep, FxVortexLook look = FxVortexLook.Steel) =>
        World?.BladeVortex(follow, blades, radius, keep, look) ?? default;

    /// <summary>A blade vortex of an element: blood for physical, otherwise the element's own look.</summary>
    public static FxHandle BladeVortex(Node3D follow, int blades, float radius, float keep, FxElement element) =>
        World?.BladeVortex(follow, blades, radius, keep, FxVortices.LookOf(element)) ?? default;

    /// <summary>The most blades a blade vortex shows.</summary>
    public const int BladeVortexMax = FxVortices.MaxBlades;

    /// <summary>A column of light from the sky down to <paramref name="at"/>, lighting the ground round its foot.</summary>
    public static FxHandle Beam(Vector3 at, float radius, float life, Color color) =>
        World?.Beam(at, radius, life, color) ?? default;

    /// <summary>
    /// A crackling orb hanging over <paramref name="centre"/> for <paramref name="life"/> seconds,
    /// over a glow marking <paramref name="radius"/>. Strikes from it are the game's: draw them
    /// with <see cref="Lightning"/> from <see cref="FxOrb.Heart"/> (less <see cref="LineLift"/>).
    /// </summary>
    public static FxOrb StormOrb(Vector3 centre, float radius, float life) =>
        World?.StormOrb(centre, radius, life) ?? default;

    /// <summary>
    /// An ice crystal rising out of hoarfrost over <paramref name="radius"/>, charging for
    /// <paramref name="fuse"/> seconds, then bursting as a frost explosion. Stopping it defuses it.
    /// </summary>
    public static FxHandle FrostBomb(Vector3 centre, float radius, float fuse) =>
        World?.FrostBomb(centre, radius, fuse) ?? default;

    /// <summary>
    /// A nova: two rings racing out to <paramref name="radius"/> (see <see cref="FxTiming.NovaFront"/>)
    /// over a globe of lightning, sparks inside and lightning at the edge, bolts arcing round the rim as they land.
    /// </summary>
    public static FxHandle Nova(Vector3 centre, float radius) =>
        World?.Nova(centre, radius) ?? default;

    /// <summary>
    /// Divine Ire: light gathers in front of <paramref name="caster"/> for <see cref="FxTiming.IreWindup"/>
    /// seconds (keep it there and aimed with <see cref="FxIreCast.Aim"/>), then a beam
    /// <paramref name="length"/> by <paramref name="width"/> fires along the aim and lingers.
    /// </summary>
    public static FxIreCast DivineIre(Vector3 caster, Vector3 direction, float length, float width) =>
        World?.DivineIre(caster, direction, length, width) ?? default;

    /// <summary>A slam: a blast (frost for ice, dust for stone) and a ring of spikes thrusting up across it.</summary>
    public static void Slam(Vector3 centre, float radius, FxSpike look) => World?.Slam(centre, radius, look);

    /// <summary>A ring of spikes thrusting up across <paramref name="radius"/>, a few close in and more round the edge.</summary>
    public static void SpikeRing(Vector3 centre, float radius, FxSpike look) => World?.SpikeRing(centre, radius, look);

    /// <summary>One spike <paramref name="height"/> metres tall thrusting up at <paramref name="at"/>, leaning toward <paramref name="lean"/>.</summary>
    public static FxHandle Spike(Vector3 at, Vector3 lean, float height, FxSpike look, float delay = 0f) =>
        World?.Spike(at, lean, height, look, delay) ?? default;

    /// <summary>
    /// A line of rock spikes racing out from <paramref name="from"/> along <paramref name="direction"/>
    /// for <paramref name="length"/> metres at <see cref="FxTiming.RuptureSpeed"/>, either side of it within half of <paramref name="width"/>.
    /// </summary>
    public static FxHandle Rupture(Vector3 from, Vector3 direction, float length, float width) =>
        World?.Rupture(from, direction, length, width) ?? default;

    /// <summary>
    /// <paramref name="count"/> spectral shields pushed out from <paramref name="centre"/> to
    /// <paramref name="radius"/> (see <see cref="FxTiming.ShieldRingRadius"/>). The shield is <see cref="FxModels.Shield"/>.
    /// </summary>
    public static FxHandle ShieldRing(Vector3 centre, int count, float radius) =>
        World?.ShieldRing(centre, count, radius) ?? default;

    /// <summary>
    /// A huge spectral fist (punching down from the sky) or hammer (swung down from upright) onto
    /// <paramref name="centre"/>, seen from <paramref name="facing"/>; after <see cref="FxTiming.SmiteFall"/>
    /// it lands as a blast throwing up rock.
    /// </summary>
    public static FxHandle Smite(FxSmite look, Vector3 centre, Vector3 facing, float radius) =>
        World?.Smite(look, centre, facing, radius) ?? default;

    /// <summary>
    /// A ram along <paramref name="direction"/> for up to <paramref name="life"/> seconds, following
    /// <paramref name="at"/> (called every frame; it ends early once that gives null): a spectral
    /// shield held out in front, or a rolling stone's dust and tail of wind. <paramref name="size"/>
    /// scales it to the one ramming.
    /// </summary>
    public static FxHandle Ram(FxRam look, Func<Vector3?> at, Vector3 direction, float life, float size = 1f) =>
        World?.Ram(look, at, direction, life, size) ?? default;

    /// <summary>
    /// A meteor onto <paramref name="at"/>: a burning rock falls slanting out of the sky and, after
    /// <see cref="FxTiming.MeteorFall"/>, lands as a fire blast <paramref name="radius"/> across and
    /// leaves the ground burning. Stopping it calls it off. It marks nothing on the ground first:
    /// a game that warns does so itself (a <see cref="FxGround.Warning"/> patch for MeteorFall).
    /// </summary>
    public static FxHandle Meteor(Vector3 at, float radius) => World?.Meteor(at, radius) ?? default;

    /// <summary>
    /// A heal on <paramref name="target"/>: motes of light spiral up round them through a band
    /// of light washing up their body, over a glow at their feet. <paramref name="size"/> scales
    /// it to whoever is healed.
    /// </summary>
    public static FxHandle Heal(Node3D target, Color color, float size = 1f) =>
        World?.Heal(target, color, size) ?? default;

    /// <summary>
    /// A barrier round <paramref name="target"/> for <paramref name="life"/> seconds: a bubble of
    /// light <paramref name="radius"/> round that swells in, ripples where
    /// <see cref="FxBarrier.Hit"/> says a blow landed, and fades at the end or bursts on
    /// <see cref="FxBarrier.Break"/>.
    /// </summary>
    public static FxBarrier Barrier(Node3D target, float radius, float life, Color color) =>
        World?.Barrier(target, radius, life, color) ?? default;

    /// <summary>
    /// A projectile's look, shown until released: the game flies it and calls
    /// <see cref="FxShot.Place"/> every frame. Shots fly <see cref="ShotHeight"/> above the ground.
    /// </summary>
    public static FxShot Shot(FxShotLook look) => World?.Shot(look) ?? default;

    /// <summary>How high above the ground shots are meant to fly: waves and tornados reach down this far to the ground.</summary>
    public const float ShotHeight = FxShots.Height;

    /// <summary>How wide a cold or steel wave is drawn (and so should hit) with <paramref name="left"/> of its flight to go, 1 to 0.</summary>
    public static float WaveHalfWidth(FxShotLook look, float left) => FxShots.WaveHalfWidth(look, left);

    /// <summary>Whether this look is a tornado (it stands upright on the ground and whirls, whatever way it flies).</summary>
    public static bool IsTornado(FxShotLook look) => FxShots.IsTornado(look);

    internal static void Light(Vector3 at, float reach, Color color, float strength) =>
        Lighting?.Light(at, reach, color, strength);
}
