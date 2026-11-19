// A hole in the world (Singularity's heart, a shadow blink): a sphere drawn dark rather than
// as light, uDeep through its middle with a slow swirl in it, rimmed in uColor. Blended over
// what is behind it, not added, so it reads as a void against any ground.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_mix, cull_back, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(0.6, 0.3, 1.0);
uniform vec3 uHot = vec3(0.9, 0.75, 1.0);
uniform vec3 uDeep = vec3(0.03, 0.0, 0.08);

instance uniform float uProgress = 0.0;
instance uniform float uAlpha = 1.0;

varying vec3 vLocal;

void vertex()
{
	vLocal = VERTEX;
}

void fragment()
{
	vec3 n = normalize(vLocal);
	float rim = fx_fresnel(NORMAL, VIEW, 2.6);
	float spin = TIME * 2.2;
	vec3 q = vec3(n.x * cos(spin) - n.z * sin(spin), n.y, n.x * sin(spin) + n.z * cos(spin));
	float swirl = fx_noise3(q * 4.0 + vec3(0.0, TIME, 0.0));
	vec3 col = mix(uDeep, uColor * 0.35, smoothstep(0.55, 0.95, swirl) * (1.0 - rim));
	col = mix(col, uColor, smoothstep(0.35, 0.8, rim));
	col = mix(col, uHot, smoothstep(0.85, 1.0, rim));
	ALBEDO = fx_out(col);
	ALPHA = clamp(uAlpha, 0.0, 1.0) * mix(0.94, 1.0, rim);
}
