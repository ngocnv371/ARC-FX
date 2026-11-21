// Effects lying on the ground: burning ground, poison pools, the frost aura, Righteous Fire's ring, the shockwave a
// blast leaves, a fire blast's ring of flame and scorch, a frost blast's ring of ice and
// hoarfrost, Shock Nova's two rings and a monster's warning. Drawn on a flat plane (2x2 before scaling) with
// premultiplied alpha, so one pass can both darken the ground (soot, murk) and add light
// (embers, glow). The noise runs in world space, so overlapping patches join up seamlessly.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_premul_alpha, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

const int GLOW = 0;
const int FIRE = 1;
const int POISON = 2;
const int FROST = 3;
const int SHOCK = 4;
const int NOVA = 5;
const int FIRE_BLAST = 6;
const int FROST_BLAST = 7;
const int WARNING = 8;
const int INFERNO = 9;
// A fire or frost blast's plane reaches this far past its damage radius, room for flame
// tongues and crystals (Blasts.BurstPad).
const float BLAST_PAD = 1.3;
// When the blast's globe pops (Blasts.GlobeTime * GlobePop): the ring reaches the radius then.
const float BLAST_POP = 0.27;

// How far a fire or frost blast's front has got, in radii: it races out with the globe from
// the first frame, on the same ease (Blasts.PlaceGlobe's swell), and lands as the globe pops,
// the way a lightning blast's nova does.
float blast_front(float age)
{
	return 1.0 - pow(1.0 - min(age / BLAST_POP, 1.0), 3.0);
}

uniform int uStyle = 0;
uniform vec3 uColor = vec3(1.0);
uniform float uAge = 0.0;
uniform float uLife = 1.0;
// Half the plane's size in metres. Wider than tall makes a capsule;
// equal makes a disc of that radius.
uniform vec2 uExtent = vec2(1.0);
// The nova's inner ring, outer ring and the outer ring's band, in radii, and how long they take to spread.
uniform vec3 uRings = vec3(0.45, 0.8, 0.2);
uniform float uSpread = 0.25;
// How strongly the patch shows: scales its light and its darkening alike (FxPatch.Strength).
uniform float uStrength = 1.0;

varying vec2 vWorld;

void vertex()
{
	vWorld = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xz;
}

// Distance from the centre line in radii: 0 on it, 1 at the rim.
float rim_distance(vec2 uv)
{
	vec2 p = (uv - 0.5) * 2.0 * uExtent;
	float half_line = max(uExtent.x - uExtent.y, 0.0);
	p.x = max(abs(p.x) - half_line, 0.0);
	return length(p) / uExtent.y;
}

void fragment()
{
	float r = rim_distance(UV);
	float u = clamp(uAge / uLife, 0.0, 1.0);
	float fade = min(1.0, min(uAge / 0.2, (uLife - uAge) / 0.5));
	vec3 glow = vec3(0.0);
	float cover = 0.0;

	if (uStyle == FIRE)
	{
		// A ragged scorch with molten cracks and licks of flame drifting over it; the heat
		// dies down toward the end, leaving the soot to fade last.
		float edge = fx_fbm(vWorld * 1.3 + vec2(0.0, TIME * 0.3));
		float mask = smoothstep(1.0, 0.6, r + (edge - 0.5) * 0.6);
		float heat = (1.0 - u * 0.8) * smoothstep(0.0, 0.15, uAge);
		// Warped cells of uneven width, so the cracks wander instead of tiling.
		vec2 bend = vec2(fx_fbm(vWorld * 0.9), fx_fbm(vWorld * 0.9 + 3.7)) - 0.5;
		vec3 cells = fx_cells((vWorld + bend * 1.2) * 1.6);
		float crack = 1.0 - smoothstep(0.0, 0.04 + 0.1 * fx_fbm(vWorld * 2.5), cells.y);
		// Only some stretches are still live at any moment, and they drift.
		crack *= 0.2 + 0.8 * smoothstep(0.35, 0.7, fx_fbm(vWorld * 0.8 + TIME * 0.12));
		float lick = fx_fbm(vWorld * 2.2 + vec2(TIME * 0.7, TIME * 1.6));
		float flare = smoothstep(0.55, 0.85, lick) * (0.7 + 0.3 * sin(TIME * 9.0 + cells.z * 30.0));
		float smoulder = lick * 0.2;
		float t = clamp((crack * (0.5 + lick * 0.5) + flare * 0.6 + smoulder) * heat * mask * (1.1 - r * 0.4), 0.0, 1.0);
		glow = fx_ramp(uColor, t) * t * 1.5;
		cover = mask * 0.3;
	}
	else if (uStyle == POISON)
	{
		// A murky pool swirling on itself, with bubbles swelling and popping across it.
		vec2 q = vWorld * 0.8;
		vec2 warp = vec2(fx_fbm(q + vec2(TIME * 0.15, 0.0)), fx_fbm(q + vec2(5.2, 1.3) - TIME * 0.12));
		float murk = fx_fbm(q * 1.5 + warp * 2.0);
		float mask = smoothstep(1.0, 0.75, r + (warp.x - 0.5) * 0.4);
		vec3 cells = fx_cells(vWorld * 2.6);
		float phase = fract(TIME * 0.7 + cells.z * 7.0);
		float size = phase * 0.4;
		float bubble = smoothstep(0.06, 0.0, abs(cells.x - size)) * (1.0 - phase) * step(0.35, cells.z);
		float lip = smoothstep(0.7, 0.95, r) * smoothstep(1.05, 0.9, r);
		float t = clamp(murk * 0.7 + bubble * 0.9 + lip * 0.5, 0.0, 1.0);
		glow = fx_ramp(uColor, t * 0.85) * t * mask * 0.9;
		cover = mask * 0.45;
	}
	else if (uStyle == FROST)
	{
		// Hoarfrost creeping out from the hero: a bright rim, crystal veins and glints.
		vec2 local = (UV - 0.5) * 2.0 * uExtent;
		// The veins creep slowly outward: the pattern grows out from the hero, two copies an
		// octave apart taking turns so it never runs out, and a drifting noise warp bends each
		// vein as it goes, so they wander like frost spreading over glass.
		float vein = 0.0;
		for (int i = 0; i < 2; i++)
		{
			float k = fract(TIME * 0.1 + float(i) * 0.5);
			float weight = sin(k * 3.14159265);
			vec2 q = local * exp2(-k) * 1.4 + float(i) * 13.7;
			vec2 bend = vec2(fx_fbm(q * 0.6 + TIME * 0.07), fx_fbm(q * 0.6 + 5.1 - TIME * 0.07)) - 0.5;
			vec3 cells = fx_cells(q + bend * 1.6);
			vein += (1.0 - smoothstep(0.0, 0.03 + 0.05 * bend.x, cells.y)) * weight;
		}
		float rim = smoothstep(0.84, 0.95, r) * smoothstep(1.0, 0.95, r) * (0.2 + 0.35 * fx_fbm(vWorld * 3.0 + TIME));
		float wave = smoothstep(0.08, 0.0, abs(r - fract(TIME * 0.6))) * 0.25;
		vec3 dots = fx_cells(vWorld * 5.0);
		float glint = smoothstep(0.08, 0.0, dots.x) * pow(0.5 + 0.5 * sin(TIME * 6.0 + dots.z * 40.0), 8.0);
		float mask = smoothstep(1.02, 0.9, r);
		float t = clamp(vein * (0.3 + r * 0.5) + rim + wave + glint + 0.08, 0.0, 1.0);
		glow = fx_ramp(uColor, 0.4 + t * 0.6) * t * mask;
		cover = mask * 0.15;
	}
	else if (uStyle == SHOCK)
	{
		// A blast's ring racing outward, then a churning afterglow that cools and fades. The
		// ring is no clean circle: its front bulges and lags round the way, it is thick and
		// bright in places and thin or gone in others, and a ragged wake trails behind it.
		float front = 1.0 - pow(1.0 - min(uAge / 0.3, 1.0), 2.0);
		float n = fx_fbm(vWorld * 1.8 + TIME * 0.5);
		vec2 dir = normalize((UV - 0.5) * 2.0 + vec2(1e-4));
		vec2 seed = NODE_POSITION_WORLD.xz * 0.37;
		float bulge = fx_fbm(dir * 1.6 + seed) - 0.5;
		float jag = fx_fbm(dir * 7.0 + seed + vec2(uAge * 2.0, 0.0)) - 0.5;
		float at = front * (0.88 + bulge * 0.28) + jag * 0.05 * front;
		float width = 0.1 + 0.12 * fx_fbm(dir * 3.5 + seed + 4.7);
		float d = r - at;
		// A hard leading edge, a softer, longer wake.
		float ring = (d > 0.0 ? smoothstep(width * 0.45, 0.0, d) : smoothstep(width * 1.6, 0.0, -d)) * (1.0 - u);
		// Uneven along the way; faded in as it grows, so it doesn't pinch to spokes at the centre.
		float patchy = 0.35 + 0.9 * smoothstep(0.3, 0.7, fx_fbm(dir * 5.0 + seed - vec2(0.0, r * 3.0)));
		ring *= mix(1.0, patchy, smoothstep(0.15, 0.45, front));
		float after = (1.0 - smoothstep(0.0, 1.0, u)) * smoothstep(1.0, 0.3, r + (n - 0.5) * 0.5 + bulge * 0.3) * n;
		float t = clamp(ring * 1.2 + after * 0.9, 0.0, 1.0);
		glow = fx_ramp(uColor, t) * t * 1.4;
		cover = after * 0.3;
		fade = 1.0;
	}
	else if (uStyle == WARNING)
	{
		// A monster's blow on its way: a thin hard pulsing rim, and a hollow ring growing out from
		// the centre that meets it as the blow lands. How loud it is overall is the patch's strength
		// (uStrength): a game turns it down so it reads without shouting over the fight.
		// Both lines keep at least a pixel or so wide, so a small disc's don't break up.
		float line = max(0.025, fwidth(r) * 1.5);
		float rim = smoothstep(line, 0.0, abs(r - 0.86));
		// The inner ring is a wide band (twenty lines across), its edges crisp, kept inside the rim.
		float band = 0.25;
		float aa = max(fwidth(r), 0.005);
		// It grows 20% faster than the wind-up, reaching the rim at about five sixths of it and holding there.
		float grow = min(u * 1.2, 1.0);
		float fill = smoothstep(band + aa, band, abs(r - grow * 0.86)) * step(0.02, u) * step(r, 0.86);
		float pulse = 0.75 + 0.25 * sin(TIME * 14.0);
		float t = clamp(rim * pulse + fill * 0.4 + 0.1, 0.0, 1.0);
		glow = uColor * t * 1.3;
		cover = (rim * 0.5 + fill * 0.3 + 0.12) * step(r, 0.9);
		fade = min(1.0, uAge / 0.08);
	}
	else if (uStyle == FIRE_BLAST)
	{
		// A fire blast seen from above: a thick ring of swirling flame rolls out to the damage
		// radius with bright streaks curling round its edge, a white-hot core and embers flare
		// where it started, and it leaves a scorch that smoulders and fades.
		float d = r * BLAST_PAD;
		vec2 local = (UV - 0.5) * 2.0 * uExtent / (uExtent.y / BLAST_PAD);
		vec2 dir = local / max(d, 1e-4);
		float age = uAge;

		// The front rolls out with the globe and stops at the radius as it pops; its edge is
		// ragged with tongues of flame.
		float wave = max(age - BLAST_POP, 0.0);
		float front = blast_front(age);
		float twist = d * 4.0 - age * 3.0;
		vec2 swirl = mat2(vec2(cos(twist), sin(twist)), vec2(-sin(twist), cos(twist))) * local;
		float churn = fx_fbm(swirl * 2.2 + vec2(age * 1.5, 0.0));
		float edge = front + (churn - 0.5) * 0.14;
		float width = 0.12 + 0.3 * front;
		float inside = (edge - d) / width;
		float band = smoothstep(-0.05, 0.1, inside) * smoothstep(1.4, 0.25, inside);
		// The ring burns out in ragged holes once it has arrived.
		float burnout = smoothstep(0.05, 0.7, wave);
		band *= smoothstep(burnout - 0.1, burnout + 0.15, churn * (1.0 - burnout * 0.3));

		// Streaks: thin ridges stretched round the ring and curled by the churn, brightest near the edge.
		float ridge = fx_noise3(vec3(dir * 2.5, d * 11.0 + churn * 2.5 - age * 4.0));
		float streak = pow(1.0 - abs(ridge * 2.0 - 1.0), 7.0) * smoothstep(1.4, 0.0, inside) * band;

		// Tatters, as on its globe (fx_fireball): a drifting noise thins the ring out in gaps so
		// it reads as rolling flame, not a drawn circle. The streaks survive more of it.
		float wisp = fx_fbm(local * 2.5 + vec2(TIME * 0.3, -age * 0.6));
		float cut = 0.37 + min(age / uLife, 1.0) * 0.12;
		band *= smoothstep(cut, cut + 0.1, wisp);
		streak *= smoothstep(cut - 0.1, cut, wisp);

		// The core: a flash of flame at the centre that sinks into embers.
		float hot = 1.0 - smoothstep(0.05, 0.6, age);
		float core = exp(-pow(d / (0.16 + 0.16 * front), 2.0)) * hot * (0.6 + 0.8 * churn);
		vec3 dots = fx_cells(local * 7.0);
		float flicker = 0.5 + 0.5 * sin(age * 30.0 + dots.z * 40.0);
		float ember = smoothstep(0.1, 0.0, dots.x) * step(0.55, dots.z) * flicker
			* step(d, edge) * (1.0 - smoothstep(0.3, 1.1, age)) * smoothstep(1.0, 0.2, d);

		// The scorch: soot over what the front has passed, with a few cracks glowing on a while.
		float scorched = smoothstep(edge + 0.02, edge - 0.12, d) * smoothstep(1.08, 0.9, d + (churn - 0.5) * 0.3);
		float soot = scorched * (0.55 + 0.3 * smoothstep(0.9, 0.0, d)) * (0.75 + 0.5 * fx_fbm(local * 3.0));
		vec3 cells = fx_cells(local * 3.0 + churn);
		float crack = (1.0 - smoothstep(0.0, 0.05, cells.y)) * scorched * (1.0 - smoothstep(0.2, uLife * 0.7, age)) * 0.35;

		// The band's body: tongues of orange flame, hotter toward the leading edge.
		float tongues = smoothstep(0.25, 0.75, fx_fbm(swirl * 4.5 - vec2(0.0, age * 2.0)));
		float body = band * (0.35 + 0.65 * tongues) * (0.6 + 0.4 * smoothstep(1.0, 0.0, inside));
		float heat = clamp(0.3 + body * 0.45 + streak * 0.5, 0.0, 1.0);
		glow = fx_ramp(uColor, heat) * body * 1.6
			+ vec3(1.0, 0.82, 0.45) * streak * 1.1
			+ fx_ramp(uColor, 0.6 + core * 0.4) * core * 1.6
			+ fx_ramp(uColor, 0.75) * (ember + crack);
		cover = soot;
		// Fade in fast; the soot fades out last.
		fade = min(1.0, min(age / 0.03, (uLife - age) / 0.6));
	}
	else if (uStyle == FROST_BLAST)
	{
		// A frost blast seen from above: a band of ice rolls out with the crystal globe to the
		// damage radius, jagged with crystals at its front, a cold flash bursts at
		// the centre as the globe shatters, and it leaves hoarfrost with glinting veins that melts away.
		float d = r * BLAST_PAD;
		vec2 local = (UV - 0.5) * 2.0 * uExtent / (uExtent.y / BLAST_PAD);
		vec2 dir = local / max(d, 1e-4);
		float age = uAge;
		float wave = max(age - BLAST_POP, 0.0);
		float front = blast_front(age);

		// Crystals jutting past the front: sharp triangles of uneven length, about 30 round.
		float spin = (atan(dir.y, dir.x) / 6.2831853 + 0.5) * 30.0;
		float tall = fx_hash(vec2(floor(spin), 7.3));
		float spike = (1.0 - abs(fract(spin) - 0.5) * 2.0) * (0.03 + 0.12 * tall);
		float edge = front + spike - 0.04;
		float inside = (edge - d) / (0.08 + 0.16 * front);
		float band = smoothstep(-0.02, 0.03, inside) * smoothstep(1.2, 0.1, inside);
		// The band is chunks of ice, each facet its own shade with bright seams; once it has
		// arrived the chunks break away one by one.
		vec3 chunks = fx_cells(local * 4.0 + 1.7);
		float seam = 1.0 - smoothstep(0.0, 0.05, chunks.y);
		band *= step(smoothstep(0.0, 0.55, wave) * 1.05, chunks.z * 0.95 + 0.02);
		// Missing chunks, as on its globe (fx_iceball): some pieces are gone from the start, in
		// clusters, so the ring reads as shattering ice, not a drawn circle.
		float gaps = fx_fbm(local * 1.8 + 3.7);
		band *= step(0.4, chunks.z * 0.55 + gaps * 0.45);
		float ice = band * (0.45 + 0.35 * chunks.z + seam * 0.6);

		// A cold flash where the globe burst.
		float flash = exp(-pow(d / 0.5, 2.0)) * smoothstep(BLAST_POP - 0.1, BLAST_POP, age)
			* (1.0 - smoothstep(BLAST_POP, BLAST_POP + 0.3, age));

		// Hoarfrost over what the front has passed: crystal veins and glints, melting away.
		vec2 bend = vec2(fx_fbm(local * 1.5), fx_fbm(local * 1.5 + 5.1)) - 0.5;
		// Its outer edge ragged, not a clean circle.
		float frosted = smoothstep(edge + 0.02, edge - 0.1, d) * smoothstep(1.1, 0.85, d + bend.y * 0.35);
		vec3 veins = fx_cells((local + bend * 0.5) * 3.0);
		float vein = 1.0 - smoothstep(0.0, 0.04 + 0.04 * bend.x, veins.y);
		vec3 dots = fx_cells(local * 9.0);
		float glint = smoothstep(0.1, 0.0, dots.x) * step(0.6, dots.z)
			* pow(0.5 + 0.5 * sin(age * 12.0 + dots.z * 40.0), 4.0);
		float rime = frosted * (0.25 + 0.15 * fx_fbm(local * 4.0) + vein * 0.45 + glint);

		float t = clamp(ice + flash + rime, 0.0, 1.0);
		glow = fx_ramp(uColor, 0.4 + t * 0.5) * (ice * 1.3 + flash * 1.6 + rime * 0.8);
		cover = band * 0.25;
		// Fade in fast; the hoarfrost melts away last.
		fade = min(1.0, min(age / 0.03, (uLife - age) / 0.8));
	}
	else if (uStyle == NOVA)
	{
		// Two rings racing out together and settling: white-gold sparks inside, a calm gap,
		// and a band of crackling lightning at the edge in the effect's colour.
		float spread = 1.0 - pow(1.0 - min(uAge / uSpread, 1.0), 2.0);
		float settle = 1.0 - smoothstep(uSpread, uLife, uAge);
		vec2 local = (UV - 0.5) * 2.0 * uExtent;
		vec2 dir = normalize(local + vec2(1e-4));
		float metres = uExtent.y;

		// A spark every 45 cm round the inner ring, each a small blob that flickers on its own.
		float inner = uRings.x * spread * metres;
		float count = max(8.0, floor(6.2831853 * uRings.x * metres / 0.45));
		float cell = (atan(dir.y, dir.x) / 6.2831853 + 0.5) * count;
		float h = fx_hash(vec2(floor(cell), 3.1));
		float across = (fract(cell) - 0.5) * 6.2831853 * inner / count;
		float out_by = r * metres - inner - (h - 0.5) * 0.12;
		float flick = 0.55 + 0.45 * sin(TIME * 34.0 + h * 50.0);
		float spark = exp(-pow(out_by / 0.26, 2.0) - pow(across / 0.13, 2.0)) * flick;
		float trace = exp(-pow(out_by / 0.05, 2.0)) * 0.35;
		float gold = clamp(spark * 1.3 + trace, 0.0, 1.0) * mix(1.0, settle, 0.85);

		// The outer band: a bright rim where it ends and threads of lightning wandering across
		// it, re-forming many times a second.
		float outer = uRings.y * spread * metres;
		float band = uRings.z * metres;
		float d = r * metres - outer;
		float arcs = 0.0;
		for (int k = 0; k < 3; k++)
		{
			float fk = float(k);
			float wander = fx_fbm(dir * (2.5 + fk) * outer * 0.35 + vec2(floor(TIME * 18.0 + fk * 0.37) * 1.7, fk * 5.3));
			float line = -band * clamp(0.5 + (wander - 0.5) * 2.5, 0.05, 0.95);
			float live = smoothstep(0.35, 0.6, fx_fbm(dir * 3.0 * outer * 0.35 + vec2(fk * 9.1, floor(TIME * 14.0) * 2.3)));
			arcs += exp(-pow((d - line) / 0.03, 2.0)) * live;
		}
		float rim = exp(-pow(d / 0.05, 2.0)) * (0.6 + 0.4 * fx_fbm(dir * 6.0 + TIME * 7.0));
		float haze = exp(-pow((d + band * 0.5) / (band * 0.6), 2.0)) * 0.22;
		float blue = clamp(arcs + rim + haze, 0.0, 1.2) * settle;

		// Tatters, as on its globe (fx_sparkball): a drifting noise eats gaps in both rings so
		// they read as bursts of energy, not drawn circles, and more of them as they settle.
		// The arcs and sparks survive more of it than the rim and haze.
		vec2 q = local / metres * 3.2 + vec2(TIME * 0.9, -TIME * 0.6);
		float tear = fx_fbm(q);
		float cut = 0.37 + u * 0.12;
		float body = smoothstep(cut, cut + 0.1, tear);
		float arcBody = smoothstep(cut - 0.1, cut, tear);
		blue = clamp((arcs * arcBody + (rim + haze) * body), 0.0, 1.2) * settle;
		gold *= arcBody;

		glow = fx_ramp(vec3(1.0, 0.72, 0.3), 0.55 + gold * 0.45) * gold * 2.4
			+ fx_ramp(uColor, 0.45 + min(blue, 1.0) * 0.55) * blue * 1.5;
		cover = 0.0;
		fade = 1.0;
	}
	else if (uStyle == INFERNO)
	{
		// Righteous Fire's ground: the fire blast's ring of flame held at the radius for good,
		// turning slowly with the bubble above it, over a light scorch with a few live cracks.
		// The middle stays nearly clear, so the hero stands out.
		// Its plane is padded like a fire blast's (Blasts.BurstPad), so the ring's outer edge sits
		// on the damage radius, under the bubble's base, with room for its tongues past it.
		float d = r * BLAST_PAD;
		vec2 local = (UV - 0.5) * 2.0 * uExtent / (uExtent.y / BLAST_PAD);
		float turn = TIME * 0.6; // the bubble's spin (fx_inferno.gdshader)
		float twist = d * 4.0 - turn;
		vec2 swirl = mat2(vec2(cos(twist), sin(twist)), vec2(-sin(twist), cos(twist))) * local;
		float churn = fx_fbm(swirl * 2.2 + vec2(TIME * 0.4, 0.0));
		float edge = 1.0 + (churn - 0.5) * 0.08;
		float inside = (edge - d) / 0.32;
		float band = smoothstep(-0.05, 0.1, inside) * smoothstep(1.4, 0.25, inside);
		vec2 dir = local / max(d, 1e-4);
		float ridge = fx_noise3(vec3(dir * 2.5, d * 11.0 + churn * 2.5 - TIME * 1.5));
		float streak = pow(1.0 - abs(ridge * 2.0 - 1.0), 7.0) * smoothstep(1.4, 0.0, inside) * band;
		float tongues = smoothstep(0.25, 0.75, fx_fbm(swirl * 4.5 - vec2(0.0, TIME * 1.2)));
		float body = band * (0.35 + 0.65 * tongues) * (0.6 + 0.4 * smoothstep(1.0, 0.0, inside));
		float heat = clamp(0.3 + body * 0.45 + streak * 0.5, 0.0, 1.0);
		float scorched = smoothstep(edge, edge - 0.15, d);
		vec3 cells = fx_cells(vWorld * 1.4 + churn);
		float crack = (1.0 - smoothstep(0.0, 0.05, cells.y)) * scorched * smoothstep(0.2, 0.8, d)
			* (0.2 + 0.8 * smoothstep(0.4, 0.7, fx_fbm(vWorld * 0.8 + TIME * 0.12)));
		glow = fx_ramp(uColor, heat) * body * 1.5
			+ vec3(1.0, 0.82, 0.45) * streak
			+ fx_ramp(uColor, 0.7) * crack * 0.4;
		cover = scorched * 0.18 * (0.75 + 0.5 * fx_fbm(local * 3.0));
	}
	else
	{
		// Any other colour: a soft disc that breathes, with a slow swirl and a brighter rim.
		float n = fx_fbm(vWorld * 1.2 + vec2(TIME * 0.2, -TIME * 0.15));
		float mask = smoothstep(1.0, 0.85, r);
		float t = clamp(n * 0.35 + smoothstep(0.75, 1.0, r) * 0.35, 0.0, 1.0) * mask;
		glow = fx_ramp(uColor, t + 0.3) * t;
		cover = mask * 0.1;
	}

	// Whatever the noise does, nothing reaches the plane's edge.
	fade *= smoothstep(1.0, 0.85, r) * uStrength;
	ALBEDO = fx_out(glow) * fade;
	ALPHA = clamp(cover * fade, 0.0, 1.0);
}
