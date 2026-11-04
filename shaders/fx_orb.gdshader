// A frostbolt in flight: a 1x1 quad turned square on to the camera with a glassy orb of cold
// drawn on it: a bright rim, frost streaks swirling round inside it, a soft glow at its heart,
// glinting specks of rime and a pale halo just outside. The orb fills uOrb of the quad's
// half-width; the halo the rest. As its strength runs out it shrinks a little and dims to
// nothing. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(0.55, 0.82, 1.0);
uniform float uOrb = 0.62;

// 1 at launch, 0 when it has dissipated.
instance uniform float uStrength = 1.0;

varying float vSeed;

void vertex()
{
	vSeed = atan(MODEL_MATRIX[2].x, MODEL_MATRIX[2].z) * 0.8;
	mat4 billboard = mat4(INV_VIEW_MATRIX[0], INV_VIEW_MATRIX[1], INV_VIEW_MATRIX[2], MODEL_MATRIX[3]);
	mat4 scale = mat4(
		vec4(length(MODEL_MATRIX[0].xyz), 0.0, 0.0, 0.0),
		vec4(0.0, length(MODEL_MATRIX[1].xyz), 0.0, 0.0),
		vec4(0.0, 0.0, length(MODEL_MATRIX[2].xyz), 0.0),
		vec4(0.0, 0.0, 0.0, 1.0));
	MODELVIEW_MATRIX = VIEW_MATRIX * billboard * scale;
}

mat2 turn(float a)
{
	return mat2(vec2(cos(a), sin(a)), vec2(-sin(a), cos(a)));
}

void fragment()
{
	// In orb radii from its centre.
	vec2 q = (UV - 0.5) * 2.0 / (uOrb * mix(0.7, 1.0, uStrength));
	float r = length(q);
	float inside = 1.0 - smoothstep(0.97, 1.03, r);
	float z = sqrt(max(1.0 - r * r, 0.0));

	// The glass: a rim that brightens toward the edge and a faint body.
	float rim = pow(1.0 - z, 3.0) * inside;
	float body = 0.12 * inside;

	// Frost streaks swirling round inside, the middle turning faster than the edge.
	float swirl = 0.0;
	for (int i = 0; i < 2; i++)
	{
		float fi = float(i);
		vec2 s = turn(TIME * (2.2 + fi) + (1.0 - r) * 2.5 + vSeed + fi * 2.0) * q;
		float n = fx_noise(s * (1.8 + fi * 0.9) + vec2(fi * 7.3, vSeed));
		float px = max(fwidth(n), 1e-4);
		swirl += exp(-abs(n) / px * 0.25) * (0.55 - fi * 0.15);
	}
	swirl *= inside * (0.35 + 0.65 * z);

	// A soft glow at its heart, specks of rime twinkling, and a halo just outside.
	float heart = exp(-r * r * 3.0) * 0.45;
	vec3 cell = fx_cells(turn(TIME * 1.5) * q * 3.0 + vSeed);
	float glint = (1.0 - smoothstep(0.03, 0.09, cell.x)) * step(0.65, cell.z) * inside;
	float halo = exp(-max(r - 1.0, 0.0) * 7.0) * (1.0 - inside) * 0.55;

	float t = rim * 1.3 + body + swirl + heart + glint + halo;
	float fade = smoothstep(0.0, 0.25, uStrength) * mix(0.35, 1.0, uStrength);
	ALBEDO = fx_out(fx_ramp(uColor, min(t * mix(0.6, 1.0, uStrength), 1.0))) * t * fade * 1.3;
}
