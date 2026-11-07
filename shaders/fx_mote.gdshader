// A mote of light (a heal's sparkles): a 1x1 quad turned square on to the camera with a soft
// round glow on it, a white-hot point in the middle fading out through the colour. Each one is
// faded with the instance uniform uAlpha. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);

// 1 when fully there, 0 when gone.
instance uniform float uAlpha = 1.0;

void vertex()
{
	mat4 billboard = mat4(INV_VIEW_MATRIX[0], INV_VIEW_MATRIX[1], INV_VIEW_MATRIX[2], MODEL_MATRIX[3]);
	mat4 scale = mat4(
		vec4(length(MODEL_MATRIX[0].xyz), 0.0, 0.0, 0.0),
		vec4(0.0, length(MODEL_MATRIX[1].xyz), 0.0, 0.0),
		vec4(0.0, 0.0, length(MODEL_MATRIX[2].xyz), 0.0),
		vec4(0.0, 0.0, 0.0, 1.0));
	MODELVIEW_MATRIX = VIEW_MATRIX * billboard * scale;
}

void fragment()
{
	float d = length(UV - 0.5) * 2.0;
	float core = exp(-d * d * 14.0);
	float halo = pow(1.0 - smoothstep(0.0, 1.0, d), 2.0) * 0.45;
	float t = core + halo;
	ALBEDO = fx_out(fx_ramp(uColor, 0.45 + 0.55 * core)) * t * uAlpha * 1.5;
}
