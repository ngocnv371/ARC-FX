// A skill's column (a pillar of light or flame, a spiral round someone levelling up or
// recalling): an open cylinder a unit round and tall, standing on its foot. uStyle picks it:
//   0 light: streaks racing up it, brightest low and thinning out toward the top;
//   1 flame: tongues of fire licking up it, ragged at their tips, hot at the root;
//   2 spiral: uTwist turns of bright bands winding up round it;
//   3 beam: even all along, streaks racing from its foot to its end, its core (as seen) hottest.
// uProgress above 0 burns it away in holes. Its sides as seen glow brightest, so it reads as
// a column rather than a disc. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform int uStyle = 0;
uniform vec3 uColor = vec3(1.0);
uniform vec3 uHot = vec3(1.0);
uniform vec3 uDeep = vec3(0.2);
uniform float uGain = 1.0;
uniform float uTwist = 1.0;
uniform float uSpeed = 1.0;

instance uniform float uProgress = 0.0;
instance uniform float uAlpha = 1.0;

varying vec3 vLocal;

void vertex()
{
	vLocal = VERTEX;
}

void fragment()
{
	float h = vLocal.y + 0.5;
	float a = atan(vLocal.x, vLocal.z);
	float edge = 1.0 - abs(dot(NORMAL, VIEW));
	vec3 q = vec3(vLocal.x, vLocal.z, 0.0) * 1.6 + NODE_POSITION_WORLD * 0.3;
	float t;
	float heat;

	if (uStyle == 0)
	{
		float streak = fx_noise3(vec3(q.xy * 2.0, h * 4.0 - TIME * uSpeed * 5.0));
		streak = pow(streak, 1.5);
		float low = pow(1.0 - h, 1.3) * smoothstep(0.0, 0.04, h);
		t = (0.3 + 0.9 * streak) * low * (0.45 + 0.75 * edge);
		heat = 0.45 + 0.55 * t;
	}
	else if (uStyle == 1)
	{
		vec3 f = vec3(q.xy * 1.5, h * 2.6 - TIME * uSpeed * 2.6);
		float n = fx_noise3(f) * 0.65 + fx_noise3(f * 2.3 + 5.0) * 0.35;
		// Tongues rather than a solid tube: the body only holds where the noise is strong, and the
		// side facing the camera is fainter than the edges, so it reads as fire round something.
		float body = n * 1.45 - h * 1.1 - 0.05;
		float flame = smoothstep(0.0, 0.2, body);
		heat = clamp(body * 1.8 + 0.15 * (1.0 - h), 0.0, 1.0);
		t = flame * (0.35 + 0.75 * heat) * (0.3 + 0.9 * edge) * smoothstep(0.0, 0.05, h);
	}
	else if (uStyle == 3)
	{
		float streak = fx_noise3(vec3(q.xy * 3.0, h * 6.0 - TIME * uSpeed * 8.0));
		float ends = smoothstep(0.0, 0.04, h) * smoothstep(1.0, 0.96, h);
		float face = 1.0 - edge;
		t = (0.35 + 0.8 * streak) * (0.3 + 0.9 * face * face) * ends;
		heat = 0.45 + 0.55 * face * (0.6 + 0.4 * streak);
	}
	else
	{
		float band = 0.5 + 0.5 * sin(a * 3.0 + h * uTwist * 6.2831853 - TIME * uSpeed * 5.0);
		band = pow(band, 10.0);
		float glow = (1.0 - h) * 0.12;
		float shape = smoothstep(0.0, 0.08, h) * (1.0 - smoothstep(0.55, 1.0, h));
		t = (band * 1.3 + glow) * shape * (0.5 + 0.7 * edge);
		heat = 0.5 + 0.5 * band;
	}

	if (uProgress > 0.0)
	{
		float holes = fx_noise3(vec3(q.xy * 3.0, h * 5.0));
		float left = holes - uProgress;
		if (left < 0.0)
			discard;
		t *= smoothstep(0.0, 0.15, left);
	}

	vec3 col = heat < 0.5 ? mix(uDeep, uColor, heat * 2.0) : mix(uColor, uHot, (heat - 0.5) * 2.0);
	ALBEDO = fx_out(col) * t * uAlpha * uGain;
}
