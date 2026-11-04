// A blast's dome: a white-hot flash that churns, cools through the effect's colour and burns
// away in ragged holes from the top down. Additive; the rim glows brightest.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);
// 0 at the burst, 1 when it is gone.
uniform float uProgress = 0.0;

varying vec3 vLocal;

void vertex()
{
	vLocal = VERTEX;
}

void fragment()
{
	float u = clamp(uProgress, 0.0, 1.0);
	vec3 p = vLocal * 3.0 + vec3(0.0, -TIME * 2.0, 0.0);
	float n = fx_noise3(p) * 0.6 + fx_noise3(p * 2.3 + 7.0) * 0.4;
	float rim = fx_fresnel(NORMAL, VIEW, 1.5);
	// The dome eats away as it ages, the top first.
	float erode = u * 1.25 + (vLocal.y * 2.0) * 0.25;
	float body = smoothstep(erode - 0.05, erode + 0.15, n);
	float t = clamp((1.0 - u) * (0.45 + rim * 0.8) + n * 0.35 * (1.0 - u), 0.0, 1.0);
	ALBEDO = fx_out(fx_ramp(uColor, t) * (0.35 + rim * 0.9) * body * (1.0 - u * u));
	ALPHA = 1.0;
}
