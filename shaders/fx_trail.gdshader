// The streak a blade leaves as it whirls (a blade vortex's trails): a flat ribbon bent along the
// blade's orbit, UV.x running from the tail (0) to the blade (1), UV.y across it. Brightest and
// widest at the blade, thinning and fading toward the tail, with a hot core along its middle and
// fine strands that shimmer, so it reads as swept air and steel catching the light. Each style
// gives it its own grain: blood and venom stay dark at the core rather than going white, fire
// flickers, frost is finely streaked, lightning's core crackles and jumps, and a spirit's is a
// soft drawn-out wisp. Faded with the instance uniform uAlpha. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

// FxVortices' trail styles.
const int PLAIN = 0;
const int BLOOD = 1;
const int FIRE = 2;
const int FROST = 3;
const int LIGHTNING = 4;
const int SPIRIT = 5;

uniform vec3 uColor = vec3(1.0);
uniform int uStyle = 0;

// 1 when fully there, 0 when gone.
instance uniform float uAlpha = 1.0;

void fragment()
{
	float along = UV.x;
	// Spirit wisps reach further back; the rest narrow toward the tail.
	float reach = uStyle == SPIRIT ? 1.4 : 2.2;
	float across = abs(UV.y - 0.5) * 2.0 / mix(0.25, 1.0, along);
	float body = (1.0 - smoothstep(0.55, 1.0, across)) * pow(along, reach);
	float core = (1.0 - smoothstep(0.0, 0.25, across)) * pow(along, 4.0);
	float strands = 0.7 + 0.3 * fx_noise(vec2(along * 18.0 - TIME * 10.0, UV.y * 6.0));
	float extra = 0.0;
	// How hot the core gets: blood and venom stay their colour rather than going white.
	float heat = 1.0;

	if (uStyle == BLOOD)
	{
		heat = 0.3;
		body *= 1.2;
	}
	else if (uStyle == FIRE)
	{
		strands = 0.55 + 0.6 * fx_noise(vec2(along * 26.0 - TIME * 16.0, UV.y * 9.0 + TIME * 3.0));
		body *= 1.5;
	}
	else if (uStyle == FROST)
	{
		strands = 0.6 + 0.4 * fx_noise(vec2(along * 30.0 - TIME * 6.0, UV.y * 14.0));
	}
	else if (uStyle == LIGHTNING)
	{
		// The core wanders across the ribbon in steps, as lightning jumps.
		float step_t = floor(TIME * 18.0);
		float wander = (fx_noise(vec2(along * 12.0, step_t)) - 0.5) * 0.7;
		float bolt = 1.0 - smoothstep(0.0, 0.07, abs(UV.y - 0.5 - wander * along));
		extra = bolt * smoothstep(0.1, 0.5, along) * 1.3;
		strands *= 0.6;
	}
	else if (uStyle == SPIRIT)
	{
		strands = 0.6 + 0.4 * fx_noise(vec2(along * 6.0 - TIME * 3.0, UV.y * 3.0));
		core *= 0.5;
	}

	float t = clamp(body * strands * 0.75 + core * 0.9 + extra, 0.0, 1.0);
	ALBEDO = fx_out(fx_ramp(uColor, 0.4 + 0.6 * t * heat)) * t * uAlpha;
}
