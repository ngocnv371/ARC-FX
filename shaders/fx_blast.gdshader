// A blast's dome: a white-hot flash that churns, cools through the effect's colour and burns
// away in ragged holes from the top down. It billows in lumps rather than holding a clean
// sphere. Additive; the rim glows brightest.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);
// 0 at the burst, 1 when it is gone.
uniform float uProgress = 0.0;

varying vec3 vLocal;

// How far the dome bulges out (or sinks in) at a point on the unit sphere: big billows that
// roll as it churns, with smaller lumps on them, swelling as it grows.
float billow(vec3 p, vec3 seed)
{
	vec3 q = p * 1.8 + seed + vec3(0.0, -TIME * 1.5, 0.0);
	return (fx_noise3(q) - 0.5) * 0.7 + (fx_noise3(q * 2.7 + 3.1) - 0.5) * 0.3;
}

void vertex()
{
	vLocal = VERTEX;
	vec3 seed = NODE_POSITION_WORLD * 0.37;
	float amp = 0.22 + 0.18 * clamp(uProgress, 0.0, 1.0);
	float h = billow(VERTEX, seed);
	// Tilt the normal by the slope of the bulge, so the rim light follows the lumps.
	vec3 side = normalize(abs(NORMAL.y) < 0.9 ? cross(NORMAL, vec3(0.0, 1.0, 0.0)) : cross(NORMAL, vec3(1.0, 0.0, 0.0)));
	vec3 up = cross(NORMAL, side);
	float e = 0.08;
	float ds = (billow(VERTEX + side * e, seed) - h) / e;
	float du = (billow(VERTEX + up * e, seed) - h) / e;
	VERTEX += NORMAL * h * amp;
	NORMAL = normalize(NORMAL - (side * ds + up * du) * amp);
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
