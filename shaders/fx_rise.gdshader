// A band of light rising up round someone (a heal washing up the body): an open cylinder, bright
// along its lower lip and thinning to nothing at its top, streaked with soft strands drifting up.
// Brightest along its sides as seen, so it reads as a ring, not a disc. Faded with the instance
// uniform uAlpha. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);

// 1 when fully there, 0 when gone.
instance uniform float uAlpha = 1.0;

varying vec3 vLocal;

void vertex()
{
	vLocal = VERTEX;
}

void fragment()
{
	float h = vLocal.y + 0.5; // 0 at its lip, 1 at its top
	float around = atan(vLocal.x, vLocal.z);
	float edge = 1.0 - abs(dot(NORMAL, VIEW));
	float strand = fx_noise(vec2(around * 4.0, h * 2.0 - TIME * 3.0));
	float lip = exp(-h * 9.0);
	float body = pow(1.0 - h, 2.5) * (0.35 + 0.65 * strand);
	float t = clamp((lip * 0.9 + body * 0.6) * (0.4 + 0.8 * edge), 0.0, 1.0);
	ALBEDO = fx_out(fx_ramp(uColor, 0.4 + 0.6 * t)) * t * uAlpha * 1.4;
}
