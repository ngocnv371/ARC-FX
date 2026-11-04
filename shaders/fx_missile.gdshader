// A spell in flight, drawn on a 1x1 quad that faces the camera but is stretched along the way
// the node flies (seen end on it shrinks back to round). The node sits on the head; the trail
// streams out behind it.
//   uStyle 0, fire: a white-hot head, flame pouring back off it in torn tongues, and embers
//   flung loose along the trail.
//   uStyle 1, frost: no head (the ice spear is its own mesh); pale mist and glinting
//   specks of rime trailing behind.
//   uStyle 2, lightning: jagged arcs crawling over an arrow and fraying off behind it.
//   uStyle 3, phantasmal: soft wisps curling off a spectral arrow and drifting back.
//   uStyle 4, venom: sickly vapour streaming off an arrow in ribbons, shedding drops.
// For the arrows, uRadius is how far ahead of the node the effect starts (the arrowhead).
// Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform int uStyle = 0;
uniform vec3 uColor = vec3(1.0, 0.45, 0.1);
// Metres along the flight (the whole quad) and across it.
uniform float uLength = 1.4;
uniform float uWidth = 0.45;
// How far from the front of the quad the head sits, as a fraction of its length.
uniform float uHead = 0.18;
// Where the trail starts behind the head, in metres (frost starts behind the spear).
uniform float uGap = 0.0;
// The fire's head, in metres; the flame behind it starts about as wide.
uniform float uRadius = 0.1;

varying float vSeed;
varying float vStretch;

void vertex()
{
	vSeed = atan(MODEL_MATRIX[2].x, MODEL_MATRIX[2].z) * 0.8;
	vec3 centre = (VIEW_MATRIX * vec4(MODEL_MATRIX[3].xyz, 1.0)).xyz;
	// The node looks along -Z.
	vec3 forward = (VIEW_MATRIX * vec4(-normalize(MODEL_MATRIX[2].xyz), 0.0)).xyz;
	float across = length(forward.xy);
	vec2 d = across > 1e-3 ? forward.xy / across : vec2(1.0, 0.0);
	vec2 n = vec2(-d.y, d.x);
	vStretch = mix(uWidth, uLength, clamp(across, 0.0, 1.0));
	vec2 offset = d * (VERTEX.x - (0.5 - uHead)) * vStretch + n * VERTEX.y * uWidth;
	POSITION = PROJECTION_MATRIX * vec4(centre + vec3(offset, 0.0), 1.0);
}

void fragment()
{
	// Metres from the head: x forward (negative along the trail), y across.
	float x = (UV.x - (1.0 - uHead)) * vStretch;
	float y = (0.5 - UV.y) * uWidth;
	float behind = max(-x - uGap, 0.0);
	float trail = vStretch * (1.0 - uHead) - uGap;
	float along = clamp(behind / max(trail, 1e-3), 0.0, 1.0);
	float t = 0.0;

	if (uStyle == 0)
	{
		float r = uRadius;
		// Noise rushing backward, the fine layer faster; scaled to the head so a fireball
		// churns in bigger tongues than a bolt.
		vec2 q = vec2(x, y) / r;
		float slow = fx_fbm(vec2(q.x * 0.45 + TIME * 7.0 + vSeed * 5.0, q.y * 0.8));
		float fast = fx_fbm(vec2(q.x * 0.9 + TIME * 13.0 - vSeed * 3.0, q.y * 1.6 + 3.0));
		float s = (slow - 0.5) * 2.5;
		float f = (fast - 0.5) * 2.5;
		// Everything behind the head fades in across it, so no seam shows where it starts.
		float behindHead = smoothstep(r * 0.8, -r * 0.3, x);
		// Flame: a tapering column that the noise bends, tears and tips.
		float bent = y + (s * 0.5 + f * 0.25) * along * r;
		float width = r * (1.1 - along * 0.7) + 0.002;
		float column = exp(-pow(bent / width, 2.0));
		float tip = smoothstep(0.0, 0.3, 1.0 - along - 0.15 + s * 0.3 + f * 0.15);
		float flame = column * tip * (0.35 + 0.45 * smoothstep(0.3, 0.8, fast * 0.6 + slow * 0.6)) * behindHead;
		// The head: hot, round in front and flowing into the flame behind, churning a little.
		float head = exp(-dot(q * vec2(1.25, 1.0), q * vec2(1.25, 1.0)) * (1.1 + f * 0.25));
		t = max(flame * (1.0 - along * 0.6), head * 1.05);
		// Embers: bright specks on cells drifting back and outward, along most of the trail.
		vec3 cell = fx_cells(vec2(q.x * 1.6 + TIME * 14.0, q.y * 1.6 + sign(y) * TIME * 2.0) + vSeed);
		float ember = (1.0 - smoothstep(0.03, 0.09, cell.x)) * step(0.45, cell.z) * smoothstep(0.0, 0.15, along)
			* (1.0 - along) * exp(-pow(y / (r * 2.2), 2.0)) * behindHead;
		t = max(t, ember);
		// A red glow just outside the flame, so its edge burns rather than fading to nothing.
		float glow = exp(-pow(bent / (width * 2.2), 2.0)) * tip * (1.0 - along) * behindHead
			+ exp(-dot(q, q) * 0.7) * 0.6;
		ALBEDO = fx_out(fx_ramp(uColor, min(t, 1.0))) * t * 1.15 + fx_out(uColor * vec3(0.9, 0.3, 0.1)) * glow * 0.3;
	}
	else if (uStyle == 2)
	{
		// Arcs crawling over the arrow: jagged lines along the flight (zero lines of noise that
		// changes fast along x), jumping to new shapes in steps, white in a blue-violet glow.
		// They hug the shaft at the head and fray outward behind it.
		float step_t = floor(TIME * 18.0) * 1.37 + vSeed;
		float span = smoothstep(uRadius, 0.0, x) * smoothstep(-trail, -trail * 0.4, x);
		float spread = 0.45 + 0.9 * along;
		vec2 light = vec2(0.0);
		for (int i = 0; i < 3; i++)
		{
			float fi = float(i);
			float n = y / (uWidth * 0.22 * spread) + 1.3 * (fx_fbm(vec2(x * 4.0 + step_t + fi * 9.1, fi * 3.3 + step_t)) - 0.5) * 2.0
				+ 0.6 * (fx_noise(vec2(x * 14.0 - step_t, fi * 5.0)));
			float px = max(fwidth(n), 1e-4);
			float d = abs(n) / px;
			float lit = smoothstep(0.2, 0.6, fx_noise(vec2(x * 1.5 + fi * 4.0, step_t)) * 0.5 + 0.5);
			light += vec2(1.0 - smoothstep(0.4, 1.4, d), exp(-d * 0.3)) * lit;
		}
		light *= span;
		// A soft violet sheath round the shaft, brightest at the head.
		float sheath = exp(-pow(y / (uWidth * 0.12), 2.0)) * span * (1.0 - along * 0.7);
		float core = clamp(light.x, 0.0, 1.0);
		ALBEDO = fx_out(vec3(0.85, 0.9, 1.0)) * core + fx_out(uColor) * clamp(light.y * 0.7 + sheath * 0.45, 0.0, 1.0) * (1.0 - core * 0.5);
	}
	else if (uStyle == 3)
	{
		// Phantasmal wisps: a few soft strands curling off the shaft and drifting back, each
		// swinging on its own wave, fading out down the trail; a pale glow along the shaft.
		float fade = (1.0 - along) * smoothstep(uRadius, -0.05, x);
		float wisp = 0.0;
		for (int i = 0; i < 4; i++)
		{
			float fi = float(i);
			// Each wisp drifts off to its own side on noise rather than a regular wave, and
			// comes and goes in patches along its length.
			float drift = (fx_fbm(vec2(x * 1.4 + TIME * 4.0 + fi * 7.3, fi * 3.1 + vSeed)) - 0.5) * 2.4;
			float swing = drift * uWidth * (0.06 + 0.42 * along) + sin(x * 3.0 + TIME * 7.0 + fi * 2.1) * uWidth * 0.03 * along;
			float d = abs(y - swing) / (uWidth * (0.02 + 0.05 * along));
			float patchy = smoothstep(0.35, 0.7, fx_fbm(vec2(x * 2.2 + TIME * 8.0, fi * 5.7)));
			wisp += exp(-d * d) * patchy;
		}
		float shaft = exp(-pow(y / (uWidth * 0.15), 2.0)) * smoothstep(uRadius + 0.1, -0.1, x) * (1.0 - along * 0.7);
		t = clamp(wisp * fade * 0.9 + shaft * 0.75, 0.0, 1.0);
		ALBEDO = fx_out(mix(uColor, vec3(0.85, 1.0, 0.9), t * 0.5)) * t;
	}
	else if (uStyle == 4)
	{
		// Venom: streaks of sickly vapour streaming back off the arrow, torn into long ribbons
		// by noise stretched along the flight, and drops of it shed behind that fall away.
		float fade = (1.0 - along) * smoothstep(uRadius, 0.0, x);
		float width = uWidth * (0.08 + 0.3 * along);
		float ribbons = fx_fbm(vec2(x * 1.6 + TIME * 9.0 + vSeed, y / uWidth * 9.0));
		float body = exp(-pow(y / width, 2.0)) * smoothstep(0.35, 0.75, ribbons + 0.25 * (1.0 - along));
		float shaft = exp(-pow(y / (uWidth * 0.06), 2.0)) * smoothstep(uRadius, -0.1, x) * (1.0 - along);
		vec3 cell = fx_cells(vec2(x * 10.0 + TIME * 10.0, y / uWidth * 10.0 - along * 3.0) + vSeed);
		float drop = (1.0 - smoothstep(0.05, 0.14, cell.x)) * step(0.55, cell.z) * smoothstep(0.05, 0.3, along)
			* exp(-pow(y / (uWidth * 0.3), 2.0));
		t = clamp((body * 1.0 + shaft * 0.5) * fade + drop * (1.0 - along) * 0.9, 0.0, 1.0);
		ALBEDO = fx_out(mix(uColor * vec3(0.6, 0.8, 0.4), mix(uColor, vec3(0.95, 1.0, 0.6), 0.4), t)) * t;
	}
	else
	{
		float mist = fx_fbm(vec2(x * 2.5 + TIME * 2.0 + vSeed, y * 6.0));
		float column = exp(-pow(y / (uWidth * (0.12 + 0.2 * along)), 2.0));
		t = column * smoothstep(0.0, 0.08, behind) * (1.0 - along) * (0.3 + 0.7 * mist) * 0.6;
		// Rime glints: tiny specks that twinkle as they pass.
		vec3 cell = fx_cells(vec2(x * 14.0 + TIME * 4.0, y * 14.0) + vSeed);
		float glint = (1.0 - smoothstep(0.02, 0.07, cell.x)) * step(0.6, cell.z)
			* (0.5 + 0.5 * sin(TIME * 20.0 + cell.z * 60.0)) * exp(-pow(y / (uWidth * 0.3), 2.0)) * (1.0 - along) * step(0.0, behind - 0.02);
		ALBEDO = fx_out(mix(uColor, vec3(1.0), 0.4)) * t + fx_out(vec3(0.9, 0.97, 1.0)) * glint;
	}
	// Nothing reaches the quad's edges, so its rectangle never shows.
	ALBEDO *= smoothstep(0.0, 0.1, UV.x) * smoothstep(1.0, 0.9, UV.x) * smoothstep(0.0, 0.15, UV.y) * smoothstep(1.0, 0.85, UV.y);
	ALPHA = 1.0;
}
