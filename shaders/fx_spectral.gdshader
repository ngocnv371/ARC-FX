// A spectral thing called down by the warrior (Shield Crush's shields, Crushing Fist's fist,
// God Hammer's hammer): see-through light in its shape, its silhouette edges glowing brightest,
// a faint fill inside, bands of light drifting down through it and a sheen of noise. Additive,
// so it brightens what is behind it and never hides it.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_back, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0, 0.78, 0.35);
// How bright it is drawn: more for small things (a thrown axe) that must read against the ground.
uniform float uGain = 1.0;

// 1 when fully there, 0 when gone (fades it in and out).
instance uniform float uAlpha = 1.0;
// A flash on impact: 1 at the blow, settling to 0.
instance uniform float uImpact = 0.0;

varying vec3 vWorld;

void vertex()
{
	vWorld = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment()
{
	// Flat facets: the low-poly cut shows in the rim.
	vec3 n = normalize(cross(dFdx(VERTEX), dFdy(VERTEX)));
	float facing = abs(dot(n, normalize(VIEW)));
	float rim = pow(1.0 - facing, 2.0);
	// Bands of light drifting down through it, and a slow sheen.
	float band = smoothstep(0.75, 1.0, sin(vWorld.y * 3.0 + TIME * 6.0) * 0.5 + 0.5) * 0.35;
	float sheen = fx_noise(vWorld.xz * 0.9 + vec2(vWorld.y * 0.6, TIME * 0.8)) * 0.25;
	float t = 0.07 + rim * 1.6 + band * 0.7 + sheen * 0.6 + uImpact * 0.9;
	ALBEDO = fx_out(fx_ramp(uColor, min(t, 1.0))) * t * uAlpha * uGain;
}
