// A tornado (Tornado): an open cone, narrow at the foot and flaring at the top, drawn as a funnel
// of dust and wind. Streaks wind up it in a helix, turning anticlockwise seen from above, the
// core faster than the dust, and the whole funnel sways, its top swinging further than its foot.
// Layer 0 is the outer funnel: pale dust, thin and torn so the core shows through it and dense at
// its silhouette. Layer 1 is the core: darker, denser dirt. Layer 2 is a flat quad on the ground
// under it: a swirl of dust kicked up round its foot. Premultiplied, not additive: dust covers
// what is behind it rather than brightening it.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_premul_alpha, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform int uLayer = 0;
uniform float uHeight = 3.6;
uniform vec3 uDust = vec3(0.72, 0.64, 0.52);
uniform vec3 uDirt = vec3(0.34, 0.27, 0.19);

varying vec3 vLocal;
varying float vSeed;

// How far the funnel leans at height fraction y.
vec2 sway(float y, float t, float seed)
{
	return vec2(sin(t * 1.7 + y * 2.4 + seed), cos(t * 1.3 + y * 2.0 + seed * 1.7)) * 0.4 * y * y;
}

void vertex()
{
	vSeed = fx_hash(vec2(float(uLayer), 3.7));
	vLocal = VERTEX;
	if (uLayer < 2)
	{
		float y = clamp(VERTEX.y / uHeight + 0.5, 0.0, 1.0);
		vec2 s = sway(y, TIME, 0.0);
		// It breathes: the walls bulge and pinch a little as they rise.
		float bulge = 1.0 + 0.08 * sin(TIME * 5.0 - y * 9.0);
		VERTEX.xz = VERTEX.xz * bulge + s;
	}
}

// The ground swirl at quad UV: colour and cover.
vec4 ground(vec2 uv, float t)
{
	// In quad radii: a ring of dust spiralling in toward the foot.
	vec2 q = (uv - 0.5) * 2.0;
	float r = length(q);
	float a = atan(q.y, q.x);
	float b = a + t * 4.0 - r * 3.5;
	float n = fx_noise3(vec3(cos(b) * 2.5, sin(b) * 2.5, r * 3.0 - t * 1.2));
	float streak = fx_noise3(vec3(cos(b) * 7.0, sin(b) * 7.0, r * 1.5));
	float ring = smoothstep(0.05, 0.25, r) * (1.0 - smoothstep(0.5, 1.0, r));
	float cover = ring * clamp(n * 1.4 - 0.15 + streak * 0.4, 0.0, 1.0) * 0.85;
	vec3 col = mix(uDirt, uDust, 0.45 + 0.4 * streak);
	return vec4(col, cover);
}

// The funnel (outer dust or core) at local position p, with its normal and the view: colour and cover.
vec4 funnel(vec3 p, vec3 normal, vec3 view, float t)
{
	float y = clamp(p.y / uHeight + 0.5, 0.0, 1.0);
	float a = atan(p.z, p.x);
	float core = float(uLayer == 1);
	// The helix: anticlockwise from above (the angle falls), winding up the funnel. One spin rate
	// for all of it (a faster foot would wind the helix tighter forever). Noise taken round a
	// circle in it is seamless, quick across the bands and slow along them, so it draws streaks
	// of wind wrapping round; the core turns faster than the dust.
	float h = a - t * mix(6.0, 9.0, core) - y * 2.5 + core * 1.7;
	float billow = fx_noise3(vec3(cos(h) * 1.6, sin(h) * 1.6, y * 2.5 - t * 1.1 + core * 11.0));
	float streak = fx_noise3(vec3(cos(h) * 3.5, sin(h) * 3.5, y * 0.7 + core * 5.0));
	float wisp = fx_noise3(vec3(cos(h * 2.0) * 3.0, sin(h * 2.0) * 3.0, y * 1.5 - t * 0.6));

	// Dense at its silhouette, thinner where we look straight through it.
	float facing = abs(dot(normalize(normal), normalize(view)));
	float edge = mix(1.0, 0.7, facing);

	float body = clamp(0.58 + (billow - 0.5) * 1.4 + (streak - 0.5) * 1.0 + (wisp - 0.5) * 0.5, 0.0, 1.0);
	// Narrowing to a thin foot where it touches down, frayed and fading out at the top.
	float ends = smoothstep(0.0, 0.1, y) * (1.0 - smoothstep(0.7, 1.0, y + (wisp - 0.5) * 0.3));
	float cover = clamp(body * edge * ends * mix(0.85, 1.0, core), 0.0, 1.0);

	// The core is dark dirt; the outer funnel pale dust, darker low down where it drags up the
	// ground, with bright streaks of wind through it.
	vec3 col = core > 0.5
		? mix(uDirt * 0.75, uDirt * 1.15, billow)
		: mix(uDirt, uDust, smoothstep(0.0, 0.45, y) * 0.65 + 0.35 * billow);
	col = mix(col, vec3(0.93, 0.91, 0.86), smoothstep(0.6, 0.85, streak) * (1.0 - core) * 0.7);
	return vec4(col, cover);
}

void fragment()
{
	vec4 c = uLayer == 2 ? ground(UV, TIME) : funnel(vLocal, NORMAL, VIEW, TIME);
	ALBEDO = fx_out(c.rgb) * c.a;
	ALPHA = c.a;
}
