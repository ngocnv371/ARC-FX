// A column of light from the sky (the summoning circle calling the hero down): streaks racing
// down an open cylinder, brightest along its middle as seen and fading out toward the top.
// Additive; fades in, then out over its last half second.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);
uniform float uAge = 0.0;
uniform float uLife = 1.0;

varying vec3 vLocal;

void vertex()
{
	vLocal = VERTEX;
}

void fragment()
{
	float h = vLocal.y + 0.5; // 0 at the ground, 1 at the top
	float core = pow(abs(dot(NORMAL, VIEW)), 2.0);
	float streak = fx_noise3(vec3(vLocal.xz * 3.0, h * 8.0 + TIME * 6.0));
	float fade = smoothstep(0.0, 0.15, uAge) * (1.0 - smoothstep(uLife - 0.5, uLife, uAge));
	float t = clamp(core * (0.5 + streak * 0.7) * (1.0 - h * 0.9) * fade, 0.0, 1.0);
	ALBEDO = fx_out(fx_ramp(uColor, t) * t * 1.6);
	ALPHA = 1.0;
}
