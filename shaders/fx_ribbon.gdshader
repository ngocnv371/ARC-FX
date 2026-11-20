// A skill's ribbon: a strip drawn along a moving thing's path (a dive's wake, an orb's trail, fire
// swirling round someone). UV.x runs from its tail (0) to its head (1), UV.y across it. uStyle:
//   0 energy: a bright core down its middle, brightening toward the head, rippling as it flows;
//   1 fire: flames licking off it, ragged at the edges, hottest near the head.
// It fades in from its tail, so it seems to stream off what leads it. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform int uStyle = 0;
uniform vec3 uColor = vec3(1.0);
uniform vec3 uHot = vec3(1.0);
uniform vec3 uDeep = vec3(0.2);
uniform float uGain = 1.0;

instance uniform float uProgress = 0.0;
instance uniform float uAlpha = 1.0;

void fragment()
{
	float s = UV.x;
	float across = abs(UV.y * 2.0 - 1.0);
	float core = exp(-across * across * 9.0);
	float t;
	float heat;
	if (uStyle == 0)
	{
		float n = fx_noise(vec2(s * 10.0 - TIME * 5.0, UV.y * 3.0)) * 0.5 + 0.5;
		float body = 1.0 - smoothstep(0.55, 1.0, across);
		t = (body * 0.45 + core * 0.9) * (0.6 + 0.6 * n) * pow(s, 1.2);
		heat = 0.4 + 0.6 * core * s;
	}
	else
	{
		float n = fx_fbm(vec2(s * 7.0 - TIME * 3.5, UV.y * 3.5 + TIME * 1.3));
		float lick = smoothstep(0.3, 0.7, n * 1.25 - across * 0.65 + s * 0.25);
		t = lick * (0.35 + 0.85 * s) + core * s * 0.5;
		heat = clamp(n * 1.1 * (0.4 + 0.6 * s) + core * 0.35, 0.0, 1.0);
	}
	t *= smoothstep(0.0, 0.1, s);
	vec3 col = heat < 0.5 ? mix(uDeep, uColor, heat * 2.0) : mix(uColor, uHot, (heat - 0.5) * 2.0);
	ALBEDO = fx_out(col) * t * uAlpha * uGain;
}
