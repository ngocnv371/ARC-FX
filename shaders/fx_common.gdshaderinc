// Shared pieces of every ARC FX shader. The colour maths is the ARC look's: fragments are
// computed in display (sRGB) values, and Godot treats an unshaded ALBEDO as linear, so each
// shader decodes its final colour with fx_out(). Kept apart from a game's own model shaders
// so the effects carry no instance uniforms and depend on nothing outside this folder.
#ifndef FX_COMMON
#define FX_COMMON

vec3 fx_out(vec3 c)
{
	c = clamp(c, 0.0, 1.0);
	vec3 lo = c / 12.92;
	vec3 hi = pow((c + 0.055) / 1.055, vec3(2.4));
	return mix(lo, hi, step(vec3(0.04045), c));
}

vec3 fx_world_normal(vec3 view_normal, mat4 inv_view)
{
	return normalize((inv_view * vec4(view_normal, 0.0)).xyz);
}

float fx_fresnel(vec3 view_normal, vec3 view_dir, float power)
{
	return pow(1.0 - clamp(dot(normalize(view_normal), view_dir), 0.0, 1.0), power);
}

#endif
