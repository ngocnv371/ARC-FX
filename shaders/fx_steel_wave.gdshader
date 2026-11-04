// Lacerate in flight, on a quad lying flat on the ground just under it: Cold Wave's crescent
// (fx_wave.gdshader) cut in polished metal and wider. A blade of brushed steel, pale at its
// honed edge and darkening toward its spine, a dark bevel line between, a white glint sliding
// from horn to horn, its horns tapering to points, and thin streaks of light trailing it.
// Blended over the ground rather than added to it, so the steel keeps its darks. It holds its
// strength until the last stretch of its range, then fades.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_mix, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

// The honed edge and the spine of the blade.
uniform vec3 uEdge = vec3(0.86, 0.9, 0.95);
uniform vec3 uSpine = vec3(0.3, 0.34, 0.42);
// The quad across and along the flight, and how far its front is ahead of the node, in metres
// (Projectiles' SteelQuad and WaveLead).
uniform vec2 uSize = vec2(5.6, 4.6);
uniform float uLead = 0.5;
// The crescent's half-width at launch and at the end of its range (Projectiles' SteelStart and SteelEnd).
uniform float uStart = 1.3;
uniform float uEnd = 2.4;

// 1 at launch, 0 at the end of its range.
instance uniform float uStrength = 1.0;

varying float vSeed;

void vertex()
{
	vSeed = fract(MODEL_MATRIX[3].x * 0.37 + MODEL_MATRIX[3].z * 0.61) * 20.0;
}

void fragment()
{
	float age = 1.0 - clamp(uStrength, 0.0, 1.0);
	// Metres: x across, b behind the node (negative ahead of it). UV.y 0 is the front.
	float x = (UV.x - 0.5) * uSize.x;
	float b = UV.y * uSize.y - uLead;
	float w = mix(uStart, uEnd, age);
	float u = abs(x) / w;
	// The crescent's centre line: its middle leads, its horns sweep back.
	float d = b - w * 0.75 * u * u;

	// The blade: deepest in the middle, tapering to points at the horns.
	float across = 1.0 - smoothstep(0.82, 1.0, u);
	float depth = 0.5 * mix(1.0, 0.15, u * u);
	float blade = smoothstep(-0.015, 0.015, d) * (1.0 - smoothstep(depth * 0.8, depth, d)) * across;
	float v = clamp(d / depth, 0.0, 1.0);

	// Brushed steel from the pale edge to the dark spine, streaked along the cut, with a dark
	// bevel line where the edge is ground.
	float brush = fx_noise(vec2(x * 1.2 + vSeed, d * 40.0)) * 0.18 - 0.09;
	vec3 steel = mix(uEdge, uSpine, smoothstep(0.0, 1.0, v)) + brush;
	steel *= 1.0 - 0.45 * exp(-pow((v - 0.3) / 0.05, 2.0));
	// The honed edge and a glint sliding from horn to horn.
	float edge = exp(-pow(d / 0.03, 2.0)) * across;
	float glint = exp(-pow((x / w - sin(TIME * 4.0 + vSeed) * 0.75) / 0.12, 2.0)) * exp(-d / 0.18) * blade;
	steel = mix(steel, vec3(1.0), clamp(edge + glint, 0.0, 1.0));

	// Thin streaks of light trailing the blade.
	float trail = mix(0.6, 2.2, smoothstep(0.0, 0.3, age));
	float along = clamp((d - depth) / trail, 0.0, 1.0);
	float lane = fx_noise(vec2(x * 9.0 + vSeed, 0.0));
	float flick = fx_noise(vec2(x * 9.0 + vSeed, b * 0.7 - TIME * 9.0));
	float lines = smoothstep(0.6, 0.8, lane) * smoothstep(0.3, 0.7, flick) * step(depth, d) * (1.0 - along)
		* (1.0 - smoothstep(0.55, 0.9, u)) * 0.5;

	float fade = smoothstep(0.0, 0.3, uStrength);
	float a = max(max(blade * 0.92, edge), lines);
	ALBEDO = fx_out(mix(vec3(0.9, 0.94, 1.0), steel, step(lines, max(blade, edge))));
	ALPHA = clamp(a, 0.0, 1.0) * fade;
}
