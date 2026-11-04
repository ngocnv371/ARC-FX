// Righteous Fire's bubble: the fire blast's globe of churning flame (fx_fireball.gdshader), but it
// never pops. Its flames lick upward and the whole of it turns slowly round the hero. Fire
// gathers at the silhouette and the base and thins to a faint shimmer facing the camera, so
// the hero inside stays readable. Premultiplied alpha, like the globe.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_premul_alpha, cull_back, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);
// 0 when it is out, 1 at full burn: it swells in and dies away.
uniform float uBurn = 1.0;

// Radians a second it turns round the hero (fx_ground.gdshader's INFERNO ring turns with it).
const float SPIN = 0.6;

varying vec3 vLocal;

void vertex()
{
	vLocal = VERTEX;
	// Turned in the noise, not the mesh, so the flames ride round while the shape stays put.
	float a = TIME * SPIN;
	vec3 q = vec3(VERTEX.x * cos(a) - VERTEX.z * sin(a), VERTEX.y, VERTEX.x * sin(a) + VERTEX.z * cos(a));
	float boil = fx_noise3(q * 2.5 + vec3(0.0, -TIME * 1.5, 0.0));
	VERTEX += NORMAL * (boil - 0.5) * 0.08;
}

void fragment()
{
	float a = TIME * SPIN;
	vec3 q = vec3(vLocal.x * cos(a) - vLocal.z * sin(a), vLocal.y, vLocal.x * sin(a) + vLocal.z * cos(a));
	vec3 p = q * 3.0 + vec3(0.0, -TIME * 1.8, 0.0);
	float n = fx_noise3(p) * 0.55 + fx_noise3(p * 2.1 + 7.0) * 0.3 + fx_noise3(p * 4.3 + 3.0) * 0.15;
	float rim = fx_fresnel(NORMAL, VIEW, 2.0);
	// Streaks of brighter flame curling up and round the surface.
	float ridge = pow(1.0 - abs(fx_noise3(p * 1.4 + vec3(0.0, n * 2.5, 0.0)) * 2.0 - 1.0), 6.0);
	// The base is a wall of fire; toward the crown only tongues reach.
	float height = clamp(vLocal.y, 0.0, 1.0);
	float base = 1.0 - smoothstep(0.0, 0.55, height);
	float tongue = smoothstep(0.35 + height * 0.5, 0.75 + height * 0.2, n);
	// Mostly the edge: what faces the camera is nearly clear, whatever the flames do there.
	float edge = smoothstep(0.05, 0.85, rim);
	float flame = clamp(tongue * (0.35 + 0.65 * edge) + base * edge * 0.6, 0.0, 1.0);

	// Tinted as the ground's ring (fx_ground.gdshader's INFERNO): orange flame, gold streaks.
	float streak = ridge * edge * flame;
	// A little cooler than the ring: the bubble stacks over it and the ground, and would wash out to white.
	float heat = clamp(0.25 + flame * 0.3 + streak * 0.35, 0.0, 0.8);
	vec3 col = fx_ramp(uColor, heat) * flame * 1.3 + vec3(1.0, 0.82, 0.45) * streak * 0.6;
	float cover = flame * (0.04 + 0.96 * edge) * uBurn;
	ALBEDO = fx_out(col) * (0.15 + 0.85 * edge) * uBurn;
	ALPHA = clamp(cover * 0.6, 0.0, 1.0);
}
