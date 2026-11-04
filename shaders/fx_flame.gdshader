// One tongue of a fire wall: a 1x1 quad turned square on to the camera, with a cluster of
// pointed flames torn by noise rising through them, each tip pulsing and tearing off into
// wisps, and embers drifting up above. White-yellow where it is hottest, low in the middle,
// through orange to deep red at its edges and tips. Each tongue takes its
// own phase from where it stands, so one material serves the whole wall. Premultiplied, not
// additive: overlapping tongues build up colour rather than washing out to white, and the
// red edges cover the ground instead of brightening it.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_premul_alpha, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

// The fire's colours from cold to hot, as display colours: maroon soot, deep red, orange,
// yellow, and a pale yellow only the hottest streaks reach.
vec3 fire(float t)
{
	t = clamp(t, 0.0, 1.0) * 4.0;
	vec3 c0 = vec3(0.22, 0.03, 0.02);
	vec3 c1 = vec3(0.62, 0.09, 0.03);
	vec3 c2 = vec3(0.95, 0.38, 0.05);
	vec3 c3 = vec3(1.0, 0.7, 0.18);
	vec3 c4 = vec3(1.0, 0.93, 0.62);
	return t < 1.0 ? mix(c0, c1, t) : t < 2.0 ? mix(c1, c2, t - 1.0) : t < 3.0 ? mix(c2, c3, t - 2.0) : mix(c3, c4, t - 3.0);
}

varying float vSeed;

void vertex()
{
	vSeed = fx_hash(MODEL_MATRIX[3].xz * 3.1);
	// Square on to the camera: from the tilted follow camera an upright quad looks squat.
	mat4 billboard = mat4(INV_VIEW_MATRIX[0], INV_VIEW_MATRIX[1], INV_VIEW_MATRIX[2], MODEL_MATRIX[3]);
	mat4 scale = mat4(
		vec4(length(MODEL_MATRIX[0].xyz), 0.0, 0.0, 0.0),
		vec4(0.0, length(MODEL_MATRIX[1].xyz), 0.0, 0.0),
		vec4(0.0, 0.0, length(MODEL_MATRIX[2].xyz), 0.0),
		vec4(0.0, 0.0, 0.0, 1.0));
	MODELVIEW_MATRIX = VIEW_MATRIX * billboard * scale;
}

void fragment()
{
	float x = UV.x - 0.5;
	float y = 1.0 - UV.y;
	// Two layers of noise rushing upward at different speeds, the fine one faster.
	float slow = fx_fbm(vec2(x * 3.0 + vSeed * 17.0, y * 1.8 - TIME * 2.6 - vSeed * 5.0));
	float fast = fx_fbm(vec2(x * 7.0 - vSeed * 9.0, y * 4.0 - TIME * 5.0));
	// fBm stays near its middle; stretch both to about -1..1 so they can really move things.
	float s = (slow - 0.5) * 2.5;
	float f = (fast - 0.5) * 2.5;
	// Three flames side by side in each tongue, each with its own lean, width and reach, so the
	// wall's top is a jagged row of points rather than a row of cones. Each sways more the
	// higher it reaches, its sides eaten ragged by the fine noise.
	float body = 0.0;
	float edge = 8.0;
	for (int i = 0; i < 3; i++)
	{
		float fi = float(i);
		float h = fx_hash(vec2(vSeed * 13.0, fi * 7.1));
		float offset = (fi - 1.0) * 0.17 + (h - 0.5) * 0.12;
		float lean = (fx_hash(vec2(fi * 3.7, vSeed * 5.0)) - 0.5) * 0.35;
		float bent = x - offset - lean * y + (s * 0.2 + f * 0.09) * y;
		float width = (0.2 + 0.09 * h) * pow(1.0 - y, 0.85) + 0.015;
		float e = abs(bent) / width;
		float ragged = e + f * (0.12 + 0.3 * y);
		// Where this flame's tip reaches: the middle one highest; each pulses on its own
		// beat, and the noise licks it higher or tears it lower in wisps.
		float pulse = 0.08 * sin(TIME * (5.0 + 3.0 * h) + h * 40.0);
		float reach = (i == 1 ? 0.92 : 0.5 + 0.35 * h) + pulse + s * 0.14 + f * 0.08;
		body = max(body, (1.0 - smoothstep(0.78, 0.95, ragged)) * smoothstep(reach, reach - 0.3, y));
		edge = min(edge, e);
	}
	// Above about half height the flames tear apart: gaps open where a third noise is low and
	// rush upward, leaving wisps that break off the tips.
	float gap = fx_fbm(vec2(x * 5.0 + vSeed * 3.0, y * 2.5 - TIME * 3.5));
	body *= 1.0 - smoothstep(0.35, 0.8, y) * (1.0 - smoothstep(0.32, 0.5, gap));
	// A yellow-hot bank along the foot that the flames rise out of; neighbouring tongues'
	// banks overlap into one unbroken line of fire.
	float bank = (1.0 - smoothstep(0.08, 0.28, y + f * 0.06)) * (1.0 - smoothstep(0.3, 0.5, abs(x)));
	body = max(body, bank) * smoothstep(0.0, 0.12, y);
	// Hottest low in the middle of the flames, cooling to red at their edges and tips, with
	// brighter streaks rushing up through them.
	float core = (1.0 - smoothstep(0.1, 0.85, edge)) * (1.0 - smoothstep(0.2, 0.75, y)) * smoothstep(0.04, 0.15, y);
	float streak = smoothstep(0.35, 0.85, fast * 0.6 + slow * 0.5);
	// The noise pushes the heat both ways, so dark red patches and yellow streaks run through
	// the orange rather than one smooth glow.
	float heat = body * 0.42 + core * 0.58 + bank * 0.1 + (streak - 0.45) * 0.5 * body - y * 0.15;
	// Only the very edge cools to red: the heat drops off in a thin band where the body fades.
	heat = clamp(heat, 0.0, 1.0) * mix(0.3, 1.0, smoothstep(0.05, 0.3, body));
	// Nearly opaque where it burns, so the ground behind doesn't wash the colours out, but
	// thin where it has cooled to red, so the edges don't leave a dark haze round the flames.
	float cover = body * mix(0.3, 0.95, smoothstep(0.15, 0.5, heat));
	// Embers: a few sparks rising and winking above the flames.
	vec3 sparks = fx_cells(vec2(x * 6.0 + vSeed * 31.0, y * 6.0 - TIME * 2.5));
	float ember = smoothstep(0.12, 0.04, sparks.x) * step(0.65, sparks.z) * smoothstep(0.2, 0.45, y)
		* (1.0 - smoothstep(0.75, 1.0, y)) * (1.0 - smoothstep(0.3, 0.5, abs(x)))
		* (0.5 + 0.5 * sin(TIME * 20.0 + sparks.z * 60.0));
	ALBEDO = fx_out(fire(heat)) * cover * (0.9 + heat * 0.4) + fx_out(fire(0.8)) * ember;
	ALPHA = max(cover, ember * 0.5);
}
