// A spectral thing called down by the warrior (Shield Crush's shields, Crushing Fist's fist,
// God Hammer's hammer, Bladefall's swords): see-through light in its shape, its silhouette edges glowing brightest,
// a faint fill inside, bands of light drifting down through it and a sheen of noise. Additive,
// so it brightens what is behind it and never hides it.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_back, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0, 0.78, 0.35);
// How bright it is drawn: more for small things (a thrown axe) that must read against the ground.
uniform float uGain = 1.0;
// Runes etched down a sword's fuller (Bladefall's swords: the blade's middle, |x| under 0.017
// and y from 0.34 to 0.61 in the mesh), 0 for none.
uniform float uRunes = 0.0;

// 1 when fully there, 0 when gone (fades it in and out).
instance uniform float uAlpha = 1.0;
// A flash on impact: 1 at the blow, settling to 0.
instance uniform float uImpact = 0.0;
// Disintegration, 0 whole to 1 gone: it crumbles away in grains, from the top down, along
// glowing edges. Its pattern is fixed in the world, so the thing must hold still while it does.
instance uniform float uDissolve = 0.0;

varying vec3 vWorld;
varying vec3 vLocal;

void vertex()
{
	vWorld = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	vLocal = VERTEX;
}

// A column of runes down a fuller: each a stem with strokes branching off it, picked per rune.
float runes(vec3 p)
{
	const float Width = 0.017, Low = 0.34, High = 0.61, Pitch = 0.034;
	if (abs(p.x) > Width || p.y < Low || p.y > High)
		return 0.0;
	float cell = floor((p.y - Low) / Pitch);
	vec2 c = vec2(p.x / Width, fract((p.y - Low) / Pitch));
	if (c.y < 0.12 || c.y > 0.88)
		return 0.0;
	float pick = fx_hash(vec2(cell, 7.0 + sign(p.z)));
	float w = 0.2;
	float mark = step(abs(c.x), w * 0.8);
	if (fract(pick * 2.0) > 0.4)
		mark = max(mark, step(abs(c.x - (c.y - 0.5) * 1.5), w) * step(0.5, c.y));
	if (fract(pick * 4.0) > 0.5)
		mark = max(mark, step(abs(c.x + (c.y - 0.35) * 1.5), w) * step(c.y, 0.6));
	if (fract(pick * 8.0) > 0.6)
		mark = max(mark, step(abs(c.y - 0.5), 0.08) * step(abs(c.x), 0.7));
	return mark;
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
	if (uRunes > 0.0)
		t += runes(vLocal) * uRunes * (0.9 + 0.3 * sin(TIME * 9.0 + vWorld.y * 4.0));
	if (uDissolve > 0.0)
	{
		// Grains first, then a lean toward the top going first; what is left glows at its edge.
		float grain = fx_fbm(vec2(vWorld.x * 7.0 + vWorld.z * 5.0, vWorld.y * 9.0)) * 0.75
			+ fx_hash(floor(vWorld.xy * 40.0) + floor(vWorld.z * 40.0)) * 0.1;
		float keep = grain + clamp(1.0 - vWorld.y * 0.5, 0.0, 1.0) * 0.3;
		float left = keep - uDissolve * 1.2;
		if (left < 0.0)
			discard;
		t += (1.0 - smoothstep(0.0, 0.08, left)) * 2.0;
	}
	ALBEDO = fx_out(fx_ramp(uColor, min(t, 1.0))) * t * uAlpha * uGain;
}
