// Cold Wave in flight, on a quad lying flat on the ground just under it: a crescent of frost,
// bowed forward, with a bright leading edge glinting with rime and pale mist streaming back off
// it in wisps that narrow toward where it came from. It widens as it flies and dims as its
// strength runs out, until only a faint wisp is left. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(0.55, 0.82, 1.0);
// The quad across and along the flight, and how far its front is ahead of the node, in metres
// (Projectiles' WaveQuad and WaveLead).
uniform vec2 uSize = vec2(4.2, 4.6);
uniform float uLead = 0.5;
// The crescent's half-width at launch and as it fades out (Projectiles' WaveStart and WaveEnd).
uniform float uStart = 0.8;
uniform float uEnd = 1.8;

// 1 at launch, 0 when it has dissipated.
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

	// Ragged horns: the edge thins out unevenly past the wave's half-width.
	float rag = fx_noise(vec2(x * 2.5 + vSeed, TIME * 3.0)) * 0.12;
	float across = 1.0 - smoothstep(0.65 + rag, 1.05 + rag, u);

	// The leading edge: sharp in front, a soft body of cold behind it.
	float ripple = fx_noise(vec2(x * 4.0 - vSeed, TIME * 6.0)) * 0.04;
	float dd = d + ripple;
	float edge = dd < 0.0 ? exp(-pow(dd / 0.06, 2.0)) : exp(-dd / 0.16);
	float body = smoothstep(-0.05, 0.1, dd) * exp(-dd / 0.5) * 0.35;

	// The trail grows from nothing as it leaves the hand, and narrows toward its tail.
	float trail = mix(0.5, 2.8, smoothstep(0.0, 0.35, age));
	float along = clamp(d / trail, 0.0, 1.0);
	float narrow = 1.0 - smoothstep(0.35 + 0.55 * (1.0 - along), 0.6 + 0.6 * (1.0 - along), u);
	// Wisps: streaks of mist stretched along the flight, streaming back.
	float streak = fx_fbm(vec2(x * 2.6 + vSeed, b * 0.8 - TIME * 4.5));
	float curl = fx_fbm(vec2(x * 5.0 - vSeed + b * 0.6, b * 1.6 - TIME * 7.0));
	float mist = smoothstep(0.35, 0.8, streak * 0.7 + curl * 0.5) * smoothstep(0.0, 0.25, d) * (1.0 - along) * narrow;

	// Rime: glinting specks along the edge, twinkling.
	vec3 cell = fx_cells(vec2(x * 7.0, b * 7.0 + TIME * 2.0) + vSeed);
	float glint = (1.0 - smoothstep(0.03, 0.08, cell.x)) * step(0.6, cell.z) * exp(-abs(dd) / 0.25) * across;

	float t = (edge * 1.6 + body) * across + mist * 1.4 + glint;
	// It dims as it goes: the edge loses its glare first, then everything fades to nothing.
	float fade = smoothstep(0.0, 0.25, uStrength) * mix(0.35, 1.0, uStrength);
	float heat = min(t * mix(0.6, 1.0, uStrength), 1.0);
	ALBEDO = fx_out(fx_ramp(uColor, heat)) * t * fade * 1.6;
}
