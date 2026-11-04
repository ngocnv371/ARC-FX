// Divine Ire's beam, on a quad stretched along it and turned round its axis to face the camera:
// a white-hot core in a golden glow, flame-like streaks licking along its ragged edges and
// jagged threads of crackle flickering through it. While the light gathers it is only a faint
// guide line; then the beam races out to its full length, flares, and thins as it fades.
// Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never, world_vertex_coords;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0, 0.66, 0.22);
// Along the beam on the ground; the node sits where it starts.
uniform vec3 uDir = vec3(0.0, 0.0, -1.0);
// Metres: its length and the width it hits (the glow and flames reach past that).
uniform float uLength = 16.0;
uniform float uWidth = 2.4;
// Seconds since the cast; the beam fires at uWindup and lasts uLife after.
uniform float uAge = 0.0;
uniform float uWindup = 0.6;
uniform float uLife = 0.5;

// How far the quad reaches past the hit width on each side, and behind the start.
const float SPREAD = 1.7;
const float BACK = 1.2;

// x metres along the beam, y across it in units of the hit half-width.
varying vec2 vBeam;

void vertex()
{
	vec3 origin = MODEL_MATRIX[3].xyz;
	float s = mix(-BACK, uLength + BACK, UV.x);
	vec3 at = origin + uDir * s;
	vec3 side = normalize(cross(uDir, normalize(INV_VIEW_MATRIX[3].xyz - at)));
	float v = (UV.y * 2.0 - 1.0) * SPREAD;
	VERTEX = at + side * v * uWidth * 0.5;
	vBeam = vec2(s, v);
}

void fragment()
{
	float s = vBeam.x;
	float v = vBeam.y;
	float fire = uAge - uWindup;
	float t = 0.0;
	float heat = 0.0;

	if (fire < 0.0)
	{
		// The guide line: a thin flickering thread that brightens as the light gathers.
		float charge = clamp(uAge / uWindup, 0.0, 1.0);
		float flicker = 0.7 + 0.3 * fx_noise(vec2(s * 0.8 - TIME * 20.0, 3.0));
		float line = exp(-pow(v / 0.05, 2.0)) * smoothstep(0.0, 0.5, s) * (1.0 - smoothstep(uLength - 2.0, uLength, s));
		t = line * charge * charge * 0.5 * flicker;
		heat = 0.75;
	}
	else
	{
		// It races out in a blink, holds, then thins and dims over its last half.
		float reach = uLength * smoothstep(0.0, 0.07, fire);
		float fade = 1.0 - smoothstep(uLife * 0.45, uLife, fire);
		float w = mix(0.3, 1.0, smoothstep(0.0, 0.05, fire)) * mix(0.35, 1.0, fade);
		float u = abs(v) / w;

		// Its ends: soft behind the hand, a ragged tip at its reach.
		float tipRag = fx_noise(vec2(v * 2.0, TIME * 9.0)) * 0.8;
		float body = smoothstep(-0.6, 0.2, s) * (1.0 - smoothstep(reach - 1.6 + tipRag, reach + 0.3, s));

		float core = exp(-pow(u / 0.2, 2.0));
		float glow = exp(-u * u * 1.8);
		float haze = exp(-u * 2.2) * 0.5;

		// Flames: streaks stretched along the beam, streaming away from the hand, over a ragged edge.
		float side = sign(v);
		float streak = fx_fbm(vec2(s * 0.3 - fire * 30.0 - TIME * 6.0, u * 3.5 + side * 7.0));
		float lick = fx_fbm(vec2(s * 0.9 - TIME * 14.0, u * 6.0 - side * 3.0));
		float edge = 0.95 + 0.55 * (lick - 0.5) * 2.0;
		float flame = smoothstep(0.42, 0.8, streak * 0.75 + lick * 0.45) * smoothstep(0.25, 0.7, u)
			* (1.0 - smoothstep(edge - 0.35, edge + 0.2, u));

		// Crackle: two jagged threads, reshaped twenty times a second.
		float crackle = 0.0;
		float tick = floor(TIME * 20.0);
		for (int k = 0; k < 2; k++)
		{
			float seed = tick * 1.7 + float(k) * 13.1;
			float y = 0.5 * fx_noise(vec2(s * 0.9, seed)) + 0.25 * fx_noise(vec2(s * 3.1, seed * 0.7));
			crackle += exp(-pow((u - abs(y) * 1.4) / 0.035, 2.0)) * step(0.35, fx_hash(vec2(seed, 1.0)));
		}

		// A flare at the hand and a burst where it ends, both brightest as it fires.
		float punch = 1.0 - smoothstep(0.0, 0.25, fire);
		float muzzle = exp(-(s * s * 1.2 + v * v * 0.6)) * (0.6 + punch);
		float impact = exp(-(pow(s - reach, 2.0) * 0.6 + v * v * 0.8)) * smoothstep(0.85, 1.0, reach / uLength) * punch;

		t = ((core * 2.2 + glow * 1.1 + haze + flame * 1.3 + crackle * 0.8) * body + muzzle + impact) * mix(0.15, 1.0, fade);
		heat = clamp(core + glow * 0.45 + crackle * 0.4 + muzzle * 0.5 + flame * 0.15, 0.0, 1.0);
	}

	ALBEDO = fx_out(fx_ramp(uColor, heat)) * t * 1.4;
}
