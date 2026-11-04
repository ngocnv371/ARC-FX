// A poison cloud's fog (FxGround.Miasma): a low dome of churning green vapour over a poison pool.
// Thick at its foot and thinning to wisps toward the crown, it drifts and rolls slowly so it reads
// as gas, not a solid shell. What faces the camera stays thin, so whatever stands inside it is
// still readable. Premultiplied alpha, like the inferno's bubble (fx_inferno.gdshader).
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_premul_alpha, cull_back, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);
// 0 when it is gone, 1 at full strength: it rolls in and thins away.
uniform float uBurn = 1.0;

// Radians a second the vapour drifts round (slower and lazier than fire).
const float SPIN = 0.25;

varying vec3 vLocal;

void vertex()
{
	vLocal = VERTEX;
	// Billows: the surface heaves slowly into lumps, more at the crown than at the foot.
	float a = TIME * SPIN;
	vec3 q = vec3(VERTEX.x * cos(a) - VERTEX.z * sin(a), VERTEX.y, VERTEX.x * sin(a) + VERTEX.z * cos(a));
	float heave = fx_noise3(q * 1.8 + vec3(0.0, TIME * 0.35, 0.0));
	VERTEX += NORMAL * (heave - 0.5) * (0.2 + 0.25 * clamp(VERTEX.y, 0.0, 1.0));
}

void fragment()
{
	float a = TIME * SPIN;
	vec3 q = vec3(vLocal.x * cos(a) - vLocal.z * sin(a), vLocal.y, vLocal.x * sin(a) + vLocal.z * cos(a));
	// Vapour rises slowly through the dome.
	vec3 p = q * 2.0 + vec3(0.0, -TIME * 0.45, 0.0);
	float n = fx_noise3(p) * 0.55 + fx_noise3(p * 2.3 + 5.0) * 0.3 + fx_noise3(p * 4.7 + 11.0) * 0.15;
	float rim = fx_fresnel(NORMAL, VIEW, 1.4);
	float height = clamp(vLocal.y, 0.0, 1.0);
	// Patchy vapour, a denser bank at the foot.
	float foot = 1.0 - smoothstep(0.0, 0.55, height);
	float puff = smoothstep(0.3, 0.75, n);
	// No shell: the outline frays into the air, at a ragged, drifting edge.
	float fray = 1.0 - smoothstep(0.3 + 0.35 * n, 0.95, rim);
	float fog = clamp(puff * (0.55 + 0.45 * foot) * fray, 0.0, 1.0);

	// A sickly body, paler where the vapour thickens; nothing shiny.
	vec3 col = mix(uColor * 0.35, uColor * 0.85, puff) * fog;
	ALBEDO = fx_out(col) * uBurn;
	ALPHA = clamp(fog * 0.6 * uBurn, 0.0, 1.0);
}
