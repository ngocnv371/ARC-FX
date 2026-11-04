// A phantasmal object: the shape only as light. Faint through its middle, glowing at its
// edges where it turns away from the camera, with a pulse running along it. Additive, so
// whatever is behind shows through.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_back, depth_draw_never;

#include "fx_common.gdshaderinc"

uniform vec3 uColor = vec3(0.3, 1.0, 0.55);

varying vec3 vLocal;

void vertex()
{
	vLocal = VERTEX;
}

void fragment()
{
	float rim = fx_fresnel(NORMAL, VIEW, 1.6);
	float pulse = 0.75 + 0.25 * sin(dot(vLocal, vec3(1.0)) * 6.0 - TIME * 14.0);
	float t = (0.6 + rim * 1.4) * pulse;
	ALBEDO = fx_out(mix(uColor, vec3(0.9, 1.0, 0.92), rim * 0.6)) * t;
}
