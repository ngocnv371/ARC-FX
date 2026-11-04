// A tornado (Tornado): an open cone, narrow at the foot and flaring at the top, drawn as a funnel
// of dust and wind. Streaks wind up it in a helix, turning anticlockwise seen from above, the
// core faster than the dust, and the whole funnel sways, its top swinging further than its foot.
// Layer 0 is the outer funnel: pale dust, thin and torn so the core shows through it and dense at
// its silhouette. Layer 1 is the core: darker, denser dirt. Layer 2 is a flat quad on the ground
// under it: a swirl of dust kicked up round its foot. Premultiplied, not additive: dust covers
// what is behind it rather than brightening it, and what glows adds its light on top.
// Styles: 0 dust; 1 storm (slate cloud with lightning crawling in its core, lit up whole by
// uFlash when a bolt strikes it); 2 fire (flame wound round a white-hot core, smoke at the top,
// embers swirling round its foot); 3 ice (frost and snow round a deep blue core, glinting).
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_premul_alpha, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform int uLayer = 0;
uniform int uStyle = 0;
// What glows: lightning, flame or frost glints.
uniform vec3 uGlow = vec3(1.0);
// A storm tornado's flash as a bolt strikes it: 1 at the strike, dying away.
instance uniform float uFlash = 0.0;
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

const int DUST = 0;
const int STORM = 1;
const int FIRE = 2;
const int ICE = 3;

// The ground swirl at quad UV: colour and cover, and the light it adds.
vec4 ground(vec2 uv, float t, out vec3 light)
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
	light = vec3(0.0);
	if (uStyle == FIRE)
	{
		// Embers whirled round in the swirl, flickering.
		float ember = smoothstep(0.72, 0.85, fx_noise3(vec3(cos(b) * 9.0, sin(b) * 9.0, r * 6.0 - t * 3.0)));
		light = uGlow * ring * (n * 0.35 + ember * 1.2);
	}
	else if (uStyle == STORM)
		light = uGlow * ring * n * uFlash * 0.8;
	else if (uStyle == ICE)
	{
		float glint = step(0.9, fx_noise3(vec3(q * 14.0, floor(t * 10.0))));
		light = vec3(0.85, 0.95, 1.0) * glint * cover * 0.8;
	}
	return vec4(col, cover);
}

// The funnel (outer dust or core) at local position p, with its normal and the view: colour and
// cover, and the light it adds.
vec4 funnel(vec3 p, vec3 normal, vec3 view, float t, out vec3 light)
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
	vec3 wind = vec3(0.93, 0.91, 0.86);
	if (uStyle == ICE)
		wind = vec3(0.95, 0.98, 1.0);
	else if (uStyle == STORM)
		wind = vec3(0.62, 0.68, 0.8);
	col = mix(col, wind, smoothstep(0.6, 0.85, streak) * (1.0 - core) * (uStyle == FIRE ? 0.0 : 0.7));

	light = vec3(0.0);
	if (uStyle == STORM)
	{
		// Threads of lightning crawling through the core, re-forming many times a second, and
		// the whole cloud lit from inside when a bolt strikes it.
		float jump = floor(t * 14.0);
		float n = fx_noise3(vec3(cos(h) * 2.2, sin(h) * 2.2, y * 4.0 + jump * 1.7));
		float thread = exp(-pow((n - 0.5) / 0.025, 2.0)) * ends;
		light = uGlow * (thread * mix(0.25, 0.9, core) * (0.6 + uFlash)
			+ cover * uFlash * (0.25 + 0.5 * billow) * mix(0.7, 1.0, core));
	}
	else if (uStyle == FIRE)
	{
		// Flame wound round the funnel, hottest in the core and low down, going to smoke at
		// the top; the dust layer shows through only as soot.
		float smoke = smoothstep(0.6, 0.95, y + (wisp - 0.5) * 0.25);
		float heat = clamp(body * 0.7 + (streak - 0.5) * 0.8 + core * 0.35 + (1.0 - y) * 0.15, 0.0, 1.0);
		vec3 hot = mix(uGlow, vec3(1.0, 0.92, 0.6), smoothstep(0.55, 0.95, heat));
		light = hot * heat * cover * (1.0 - smoke) * mix(1.1, 1.5, core);
		cover *= mix(0.7, 1.0, smoke);
	}
	else if (uStyle == ICE)
	{
		// Glints of frost catching the light as they whirl past, and a faint cold glow in the core.
		float glint = step(0.92, fx_noise3(vec3(cos(h) * 10.0, sin(h) * 10.0, y * 18.0 + floor(t * 10.0) * 3.1)));
		light = vec3(0.85, 0.95, 1.0) * glint * cover * (1.0 - core) + uGlow * cover * core * 0.25 * billow;
	}
	return vec4(col, cover);
}

void fragment()
{
	vec3 light;
	vec4 c = uLayer == 2 ? ground(UV, TIME, light) : funnel(vLocal, NORMAL, VIEW, TIME, light);
	ALBEDO = fx_out(c.rgb) * c.a + fx_out(light);
	ALPHA = c.a;
}
