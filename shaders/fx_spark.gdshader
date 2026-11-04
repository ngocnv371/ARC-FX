// A spark in flight: a 1x1 quad turned square on to the camera, with a crackling clump of
// lightning drawn on it: a hot centre, jagged tendrils thrown outward and a tangle of arcs
// around the middle, each a thin white line in a blue glow. The pattern jumps in steps rather
// than flowing, the way lightning does, and each spark takes its own from the way it flies.
// Line widths come from the screen, so the strokes stay a pixel or two at any zoom. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uCore = vec3(0.8, 0.88, 1.0);
uniform vec3 uGlow = vec3(0.2, 0.3, 1.0);

varying float vSeed;

void vertex()
{
	// The node faces where it flies, and sparks wobble, so this drifts as each one veers.
	vSeed = atan(MODEL_MATRIX[2].x, MODEL_MATRIX[2].z) * 0.8;
	mat4 billboard = mat4(INV_VIEW_MATRIX[0], INV_VIEW_MATRIX[1], INV_VIEW_MATRIX[2], MODEL_MATRIX[3]);
	mat4 scale = mat4(
		vec4(length(MODEL_MATRIX[0].xyz), 0.0, 0.0, 0.0),
		vec4(0.0, length(MODEL_MATRIX[1].xyz), 0.0, 0.0),
		vec4(0.0, 0.0, length(MODEL_MATRIX[2].xyz), 0.0),
		vec4(0.0, 0.0, 0.0, 1.0));
	MODELVIEW_MATRIX = VIEW_MATRIX * billboard * scale;
}

// A line along the zero contour of n: a sharp core and a wider glow, both measured in pixels.
vec2 strand(float n)
{
	float px = max(fwidth(n), 1e-4);
	float d = abs(n) / px;
	return vec2(1.0 - smoothstep(0.4, 1.4, d), exp(-d * 0.35));
}

void fragment()
{
	vec2 p = (UV - 0.5) * 2.0;
	float r = max(length(p), 1e-3);
	vec2 dir = p / r;
	float t = floor(TIME * 16.0) * 1.37 + vSeed;

	vec2 light = vec2(0.0);
	// Tendrils: noise that depends mostly on direction, so its zero lines run outward, kinked
	// by a fine fractal layer that changes along the radius (the jaggedness). A second noise
	// on the direction lights only some of them, and each layer reaches its own distance.
	for (int i = 0; i < 2; i++)
	{
		float fi = float(i);
		float n = fx_noise(dir * 1.1 + vec2(fi * 3.1 + t, fi * 1.7 - t * 0.6))
			+ 0.5 * (fx_fbm(p * 5.0 + vec2(t * 1.3, fi * 4.0)) - 0.5);
		float lit = smoothstep(-0.3, 0.2, fx_noise(dir * 1.3 + vec2(fi * 7.3 - t * 0.8, t)));
		float reach = 0.6 + 0.4 * fx_hash(vec2(fi, floor(TIME * 16.0) + vSeed));
		light += strand(n) * lit * smoothstep(reach, reach * 0.25, r);
	}
	// A tangle of short arcs close round the middle.
	float knot = fx_noise(p * 2.6 + vec2(t * 0.9, -t * 1.1)) + 0.5 * (fx_fbm(p * 6.0 - t) - 0.5);
	light += strand(knot) * smoothstep(0.4, 0.05, r) * vec2(0.7, 0.6);

	// The hot centre everything leaves from.
	float heart = exp(-r * r * 90.0);
	float core = clamp(light.x * 0.85 + heart, 0.0, 1.0);
	float glow = clamp(light.y * 0.9 + exp(-r * r * 9.0) * 0.45, 0.0, 1.0);
	ALBEDO = fx_out(uCore) * core + fx_out(uGlow) * glow * (1.0 - core * 0.5);
	ALPHA = 1.0;
}
