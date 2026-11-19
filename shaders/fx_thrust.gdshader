// A thrust: a lance of light stabbing out from the hand. UV.x runs across it and UV.y from the
// hand (0) to its full reach (1). The point shoots out over the first third of its life; behind
// it a thin bright core runs back to the hand inside a faint cone that widens with the reach and
// closes to a point at the tip. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);
uniform float uAge = 0.0;
uniform float uLife = 0.22;

void fragment()
{
	float u = clamp(uAge / uLife, 0.0, 1.0);
	// How far the point has come, and where this spot is between the hand and the point.
	float head = clamp(u / 0.35, 0.02, 1.0);
	float s = UV.y / head;
	float x = abs(UV.x - 0.5) * 2.0;
	// The cone's half-width here: narrow at the hand, widest near the tip, then a point.
	float half_width = (0.12 + 0.88 * s) * (1.0 - smoothstep(0.75, 1.0, s));
	float core = exp(-pow(x / (half_width * 0.22 + 0.015), 2.0)) * (0.35 + 0.65 * smoothstep(0.0, 1.0, s));
	float cone = (1.0 - smoothstep(half_width * 0.5, half_width, x)) * 0.3 * s;
	float tear = 0.75 + 0.25 * fx_fbm(vec2(UV.x * 5.0, UV.y * 7.0 - uAge * 14.0));
	float fade = 1.0 - smoothstep(0.35, 1.0, u);
	float a = (core + cone * tear) * fade * step(s, 1.0) * smoothstep(0.0, 0.06, UV.y);
	ALBEDO = uColor * a * 0.8 + vec3(a * a * 0.3);
}
