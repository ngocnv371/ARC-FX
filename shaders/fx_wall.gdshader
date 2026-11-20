// A skill's standing wall (a barrier of wind, a cage of light): a 1x1 quad stood upright along a
// segment, UV.x running along it and UV.y down it. uStyle picks it:
//   0 energy: a bright foot and top edge, a web of cells and streaks rising through a faint sheet;
//   1 wind: streams of air racing along it, thickest in the middle, frayed at top and foot.
// uProgress above 0 breaks it away in ragged holes with glowing edges. The instance uniform
// uAspect (its length over its height) keeps the pattern the same size however long it is.
// Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform int uStyle = 0;
uniform vec3 uColor = vec3(1.0);
uniform vec3 uHot = vec3(1.0);
uniform vec3 uDeep = vec3(0.2);
uniform float uGain = 1.0;
uniform float uSpeed = 1.0;

instance uniform float uProgress = 0.0;
instance uniform float uAlpha = 1.0;
instance uniform float uAspect = 1.0;

void fragment()
{
	float x = UV.x;
	float y = 1.0 - UV.y;
	vec2 m = vec2(x * uAspect, y) + NODE_POSITION_WORLD.xz * 0.21;
	float ends = smoothstep(0.0, 0.03, x) * smoothstep(1.0, 0.97, x);
	float t;
	float heat;
	if (uStyle == 0)
	{
		float foot = exp(-y * 7.0);
		float top = exp(-pow((1.0 - y) / 0.035, 2.0));
		float streak = pow(fx_noise(vec2(m.x * 5.0, m.y * 1.4 - TIME * uSpeed * 1.6)) * 0.5 + 0.5, 3.0);
		float web = 1.0 - smoothstep(0.0, 0.05, fx_cells(m * 3.5 + vec2(0.0, -TIME * 0.25)).y);
		t = 0.1 + foot * 0.9 + top * 0.8 + streak * 0.6 * (1.0 - y) + web * 0.28 * (0.4 + 0.6 * (1.0 - y));
		heat = clamp(0.4 + 0.55 * (foot + top) + 0.2 * web, 0.0, 1.0);
	}
	else
	{
		float flow = fx_fbm(vec2(m.x * 1.1 - TIME * uSpeed * 2.2, m.y * 7.0));
		float gust = fx_fbm(vec2(m.x * 2.3 - TIME * uSpeed * 3.1, m.y * 13.0 + 4.0));
		float lines = smoothstep(0.52, 0.82, flow) + smoothstep(0.6, 0.85, gust) * 0.6;
		float body = smoothstep(0.0, 0.12, y) * smoothstep(1.0, 0.55, y);
		t = (0.08 + lines * 0.9) * body;
		heat = 0.4 + 0.5 * clamp(lines, 0.0, 1.0);
	}
	if (uProgress > 0.0)
	{
		float left = fx_fbm(m * 3.0) * 0.8 + y * 0.25 - uProgress * 1.1;
		if (left < 0.0)
			discard;
		t += (1.0 - smoothstep(0.0, 0.06, left)) * 1.2;
	}
	vec3 col = heat < 0.5 ? mix(uDeep, uColor, heat * 2.0) : mix(uColor, uHot, (heat - 0.5) * 2.0);
	ALBEDO = fx_out(col) * t * ends * uAlpha * uGain;
}
