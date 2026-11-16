// Sky Split in flight: a tall crescent of pale light standing upright along its flight, and the
// thin line it scores along the ground under it. One mesh of two quads, told apart by COLOR.r:
// 0 the blade (UV.x from its front to its back, UV.y from its foot to its top), 1 the ground line
// (UV.x across, UV.y from its front back). The blade's middle leads and its tips sweep back to
// points; streaks of light trail it. Translucent: added to what is behind it. It holds its
// strength over most of its flight, then fades.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(0.72, 0.86, 1.0);

// 1 at launch, 0 at the end of its range.
instance uniform float uStrength = 1.0;

varying float vPart;

void vertex()
{
	vPart = COLOR.r;
}

void fragment()
{
	float a;
	if (vPart < 0.5)
	{
		float h = UV.y * 2.0 - 1.0;
		// How far behind the leading edge this spot is, and how deep the blade is at this height.
		float d = UV.x - (0.08 + 0.42 * h * h);
		float depth = 0.24 * (1.0 - h * h) + 0.01;
		float tips = 1.0 - smoothstep(0.88, 1.0, abs(h));
		float edge = exp(-pow(d / 0.022, 2.0)) * tips;
		float body = smoothstep(0.0, 0.01, d) * (1.0 - smoothstep(0.0, depth, d));
		float streak = fx_noise(vec2(h * 14.0, UV.x * 2.0 - TIME * 8.0));
		float tail = step(0.0, d) * exp(-d * 5.0) * smoothstep(0.45, 0.8, streak) * (1.0 - abs(h)) * 0.5;
		a = edge + body * 0.5 + tail;
	}
	else
	{
		float x = abs(UV.x - 0.5) * 2.0;
		a = exp(-pow(x / 0.25, 2.0)) * exp(-UV.y * 4.0) * smoothstep(0.0, 0.04, UV.y) * 0.8;
	}
	a *= smoothstep(0.0, 0.45, uStrength);
	ALBEDO = uColor * a + vec3(a * a * 0.35);
}
