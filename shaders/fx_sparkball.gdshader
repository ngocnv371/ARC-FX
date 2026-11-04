// A lightning blast's globe: a bubble of crackling plasma that swells from the impact, a faint
// blue haze with a bright rim and threads of lightning crawling over it, re-forming many times
// a second; then it pops in a white flash, its arcs flaring as the bubble fades. Premultiplied
// alpha with little body: it is mostly light.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_premul_alpha, cull_back, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);
// 0 at the impact, 1 when the globe has popped.
uniform float uProgress = 0.0;

varying vec3 vLocal;
varying vec3 vSeed;

void vertex()
{
	// It flickers in and out a little, a new shape every frame or two.
	vSeed = NODE_POSITION_WORLD * 0.37;
	vLocal = VERTEX;
	VERTEX *= 0.97 + 0.06 * fx_noise3(VERTEX * 3.0 + vec3(floor(TIME * 30.0) * 1.3));
}

void fragment()
{
	float u = clamp(uProgress, 0.0, 1.0);
	float rim = fx_fresnel(NORMAL, VIEW, 2.5);
	float pop = smoothstep(0.75, 1.0, u); // from Blasts.SparkPop

	// Arcs: where a noise field crosses its middle, a wiggly line over the sphere, made
	// jagged by a finer noise and re-drawn many times a second.
	float arcs = 0.0;
	for (int k = 0; k < 3; k++)
	{
		float fk = float(k);
		vec3 p = vLocal * 2.2 + vSeed + vec3(fk * 5.1, floor(TIME * 16.0 + fk * 0.37) * 1.7, fk * 2.3);
		float n = fx_noise3(p) + (fx_noise3(p * 6.0) - 0.5) * 0.12;
		arcs += exp(-pow((n - 0.5) / 0.018, 2.0));
	}
	arcs = min(arcs, 1.5);

	float haze = 0.12 + 0.12 * fx_noise3(vLocal * 5.0 + vec3(0.0, TIME * 6.0, 0.0));
	float flash = 1.0 - smoothstep(0.0, 0.12, abs(u - 0.78));
	// The bubble fades away after the pop; its arcs flare and linger a moment longer.
	float fade = 1.0 - pop;
	float bolt = arcs * (0.8 + pop * 1.2) * (1.0 - smoothstep(0.85, 1.0, u));
	// The flash lights up the rim and the arcs, not the middle, so it stays a bubble.
	float bright = clamp(haze + rim * (0.6 + flash * 0.6), 0.0, 1.0) * fade;
	vec3 col = fx_ramp(uColor, 0.4 + bright * 0.4) * bright * 1.4
		+ fx_ramp(uColor, 0.8 + min(bolt, 1.0) * 0.2) * bolt * (1.0 + flash * 0.5)
		+ fx_ramp(uColor, 0.7) * flash * 0.15;
	ALBEDO = fx_out(col);
	ALPHA = clamp((0.08 + rim * 0.35) * fade, 0.0, 1.0);
}
