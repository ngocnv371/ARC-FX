// A fire blast's globe: a bubble of churning flame that swells from the impact, its surface
// boiling and streaked, then pops, tearing open in ragged holes with white-hot edges.
// Premultiplied alpha, so it both adds light and has some body over what is behind it.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_premul_alpha, cull_back, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);
// 0 at the impact, 1 when the bubble has popped.
uniform float uProgress = 0.0;

varying vec3 vLocal;
varying vec3 vSeed;

void vertex()
{
	// Each blast boils differently.
	vSeed = NODE_POSITION_WORLD * 0.37;
	vLocal = VERTEX;
	float boil = fx_noise3(VERTEX * 2.5 + vSeed + vec3(0.0, -TIME * 3.0, 0.0));
	VERTEX += NORMAL * (boil - 0.5) * 0.2;
}

void fragment()
{
	float u = clamp(uProgress, 0.0, 1.0);
	vec3 p = vLocal * 3.0 + vSeed + vec3(0.0, -TIME * 2.5, 0.0);
	float n = fx_noise3(p) * 0.55 + fx_noise3(p * 2.1 + 7.0) * 0.3 + fx_noise3(p * 4.3 + 3.0) * 0.15;
	float rim = fx_fresnel(NORMAL, VIEW, 2.0);
	// Streaks of brighter flame curling over the surface.
	float ridge = pow(1.0 - abs(fx_noise3(p * 1.4 + vec3(0.0, n * 2.5, 0.0)) * 2.0 - 1.0), 6.0);

	// The pop: holes open where the noise is lowest and spread until nothing is left.
	float pop = smoothstep(0.6, 1.0, u); // from Blasts.GlobePop
	float erode = pop * 1.1 - 0.05;
	float body = smoothstep(erode, erode + 0.1, n);
	float tear = smoothstep(0.06, 0.0, abs(n - erode - 0.05)) * step(0.01, pop);

	// Tatters: a second, slower noise thins the bubble out in drifting patches, hardest at the
	// silhouette, so it reads as a burst of flame rather than a solid ball (as fx_sparkball).
	vec3 q = vLocal * 2.4 + vSeed * 1.7 + vec3(TIME * 0.8, -TIME * 1.8, 0.0);
	float wisp = fx_noise3(q) * 0.65 + fx_noise3(q * 2.6 + 4.3) * 0.35;
	float cut = 0.18 + rim * 0.2 + u * 0.1;
	body *= smoothstep(cut, cut + 0.24, wisp);

	float flash = 1.0 - smoothstep(0.0, 0.3, u);
	float heat = clamp(-0.05 + n * 0.95 + rim * 0.35 + ridge * 0.3 + flash * 0.35 + tear * 0.6 - u * 0.1, 0.0, 1.0);
	vec3 col = fx_ramp(uColor, heat) * (0.6 + rim * 1.4 + ridge * 0.6 + tear * 1.2);
	// A bubble: thin in the middle, thick at the rim.
	float cover = body * (0.2 + 0.8 * rim);
	ALBEDO = fx_out(col) * body * (0.6 + 0.4 * rim);
	ALPHA = clamp(cover * 0.7, 0.0, 1.0);
}
