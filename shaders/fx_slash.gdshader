// The streak an axe leaves through the air: a fan lying flat, UV.x running along the arc in the
// direction of the swing and UV.y from the hub (0) to the blade's reach (1). A bright edge sweeps
// round the arc over the first part of its life, trailing a fading smear toward the hub. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);
uniform float uAge = 0.0;
uniform float uLife = 0.3;

void fragment()
{
	float u = clamp(uAge / uLife, 0.0, 1.0);
	// The edge reaches the end of the arc at 60% of the life; behind it the smear fades.
	float head = u / 0.6;
	float behind = head - UV.x;
	float trail = step(0.0, behind) * exp(-behind * 6.0);
	// Brightest at the blade's reach, thinning toward the hub, torn a little by noise.
	float band = pow(smoothstep(0.55, 0.95, UV.y), 2.0) * (1.0 - smoothstep(0.96, 1.0, UV.y));
	float tear = 0.7 + 0.3 * fx_fbm(vec2(UV.x * 9.0 - uAge * 6.0, UV.y * 3.0));
	float fade = 1.0 - smoothstep(0.55, 1.0, u);
	float a = trail * band * tear * fade;
	ALBEDO = uColor * a * 0.75 + vec3(a * a * 0.25);
}
