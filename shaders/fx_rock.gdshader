// Rock and earth (a boulder, a standing stone, a block of wall, the chunks a blast throws up):
// never one flat colour. Warm and cool stone are blotched together by noise that lives in the
// thing's own space, so the pattern stays put as it tumbles; dark flecks, pale grit and faint
// strata run through it; every flat facet is a shade of its own; tops are dusted pale and a
// fixed key light shapes it. The instance uniform uSeed gives each rock its own blotches and
// its own cast of colour, so no two are alike. uFade below 1 thins it away through a fine
// screen of holes, as fx_crystal does.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, cull_back;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uWarm = vec3(0.58, 0.41, 0.25);
uniform vec3 uCool = vec3(0.4, 0.39, 0.4);
uniform vec3 uDark = vec3(0.16, 0.12, 0.09);
uniform vec3 uPale = vec3(0.78, 0.7, 0.58);
// How big the blotches are: more is finer.
uniform float uGrain = 2.2;

instance uniform float uSeed = 0.0;
instance uniform float uFade = 1.0;

varying vec3 vLocal;
varying vec3 vFacet;

void vertex()
{
	vLocal = VERTEX;
	vFacet = NORMAL;
}

void fragment()
{
	if (uFade < 1.0 && fract(52.9829189 * fract(dot(FRAGCOORD.xy, vec2(0.06711056, 0.00583715)))) >= uFade)
		discard;
	vec3 N = fx_world_normal(NORMAL, INV_VIEW_MATRIX);
	vec3 q = vLocal * uGrain + vec3(uSeed * 37.0, uSeed * 11.0, uSeed * 23.0);
	float blotch = fx_noise3(q) * 0.65 + fx_noise3(q * 2.3 + 5.0) * 0.35;
	float fleck = fx_noise3(q * 4.6 + 9.0);
	float grit = fx_noise3(q * 11.0 + 3.0);
	// Warm earth to cool stone, each rock leaning one way or the other.
	vec3 col = mix(uCool, uWarm, smoothstep(0.32, 0.68, blotch + (uSeed - 0.5) * 0.45));
	// Strata: faint bands across it, bent by the noise.
	float strata = 0.5 + 0.5 * sin((vLocal.y + 0.3 * blotch) * 13.0 + uSeed * 20.0);
	col *= 0.86 + 0.2 * strata;
	col = mix(col, uDark, smoothstep(0.6, 0.85, fleck) * 0.6);
	col = mix(col, uPale, smoothstep(0.72, 0.9, grit) * 0.45);
	// Each flat facet a shade of its own (the mesh's normals are per face, so this is too).
	vec3 cell = floor(normalize(vFacet) * 5.0 + 0.5);
	float facet = fract(sin(dot(cell, vec3(12.9898, 78.233, 37.719)) + uSeed * 9.0) * 43758.5453);
	col *= 0.84 + 0.3 * facet;
	// Its own cast of colour and brightness.
	col *= mix(vec3(1.07, 0.98, 0.9), vec3(0.92, 0.98, 1.07), fract(uSeed * 3.7)) * (0.88 + 0.24 * fract(uSeed * 7.3));
	// A fixed key light, pale dust on what faces up, a little light on the edges.
	vec3 L = normalize(vec3(-0.45, 0.8, 0.4));
	float key = clamp(dot(N, L) * 0.5 + 0.5, 0.0, 1.0);
	col *= mix(0.42, 1.15, key * key);
	col = mix(col, uPale * (0.55 + 0.45 * key), smoothstep(0.55, 0.95, N.y) * 0.3);
	col += uPale * fx_fresnel(NORMAL, VIEW, 2.5) * 0.14;
	ALBEDO = fx_out(col);
}
