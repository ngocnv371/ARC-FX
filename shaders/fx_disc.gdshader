// A skill's mark on the ground (or, turned upright, in the air): a 2x2 plane, its UV running
// 0..1 across it, round its middle. uStyle picks what it draws:
//   0 a shockwave: a ring at uProgress of the radius, sharp ahead and trailing a wake behind,
//     broken up by noise and hottest at its front;
//   1 a rune circle: rings, turning ticks, a band of runes and a hexagram, spreading out from
//     the middle behind a bright front as uProgress goes from 0 to 1;
//   2 cracks: a web of glowing cracks round the middle, cooling as uProgress goes to 1;
//   3 a soft glow;
//   4 a swirl: spiral arms winding into the middle, turning at uSpin;
//   5 a telegraph: a faint disc with a bright rim, filling from the middle to uProgress;
//   6 a cone: a wedge uCone either side of the plane's +Z, filling out to uProgress, its sides
//     and front bright (a breath's or a blast's hit area);
//   7 a band: one clean glowing ring near the rim, rippling (a binding round someone).
// Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform int uStyle = 0;
uniform vec3 uColor = vec3(1.0);
uniform vec3 uHot = vec3(1.0);
uniform vec3 uDeep = vec3(0.2);
uniform float uGain = 1.0;
// A shockwave's depth, as a share of the radius.
uniform float uBand = 0.12;
// How fast a swirl turns.
uniform float uSpin = 3.0;
// A cone's half-angle, in radians.
uniform float uCone = 0.5;

instance uniform float uProgress = 0.0;
instance uniform float uAlpha = 1.0;

// A line along the zero of v, w wide, kept at least a pixel or so by the screen.
float line(float v, float w)
{
	float px = fwidth(v) * 1.2;
	return 1.0 - smoothstep(w, w + px, abs(v));
}

// Signed distance to an equilateral triangle of half-side r, point up (iq's).
float tri(vec2 p, float r)
{
	const float k = 1.7320508;
	p.x = abs(p.x) - r;
	p.y = p.y + r / k;
	if (p.x + k * p.y > 0.0)
		p = vec2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
	p.x -= clamp(p.x, -2.0 * r, 0.0);
	return -length(p) * sign(p.y);
}

vec2 turn(vec2 p, float a)
{
	float c = cos(a), s = sin(a);
	return vec2(c * p.x - s * p.y, s * p.x + c * p.y);
}

// One rune in a cell: x across it (-0.5..0.5), y up it (-1..1), strokes picked by pick.
float rune(vec2 c, float pick)
{
	float w = 0.07;
	float m = (1.0 - smoothstep(w, w + 0.05, abs(c.x))) * step(abs(c.y), 0.75);
	if (fract(pick * 3.0) > 0.45)
		m = max(m, (1.0 - smoothstep(w, w + 0.05, abs(c.x - c.y * 0.35))) * step(0.0, c.y) * step(c.y, 0.75));
	if (fract(pick * 7.0) > 0.5)
		m = max(m, (1.0 - smoothstep(w, w + 0.05, abs(c.y + 0.2))) * step(abs(c.x), 0.3));
	if (fract(pick * 13.0) > 0.55)
		m = max(m, (1.0 - smoothstep(w, w + 0.05, abs(c.x + c.y * 0.35))) * step(c.y, 0.0) * step(-0.75, c.y));
	return m;
}

void fragment()
{
	vec2 p = (UV - 0.5) * 2.0;
	float r = length(p);
	if (r > 1.0)
		discard;
	float a = atan(p.y, p.x);
	float u = uProgress;
	vec2 seed = NODE_POSITION_WORLD.xz * 0.31;
	float t = 0.0;
	float heat = 0.5;

	if (uStyle == 0)
	{
		float front = u;
		float d = r - front;
		float n = fx_fbm(vec2(a * 2.5, r * 3.0 - TIME * 1.5) + seed);
		float ahead = exp(-pow(max(d, 0.0) / (uBand * 0.3), 2.0));
		float wake = exp(min(d, 0.0) / uBand);
		float ring = (d > 0.0 ? ahead : wake) * (0.55 + 0.75 * n);
		float trail = step(d, 0.0) * exp(d / (uBand * 4.0)) * 0.18 * smoothstep(0.0, 0.3, front);
		t = ring + trail;
		heat = 0.35 + 0.65 * ring;
	}
	else if (uStyle == 1)
	{
		// Drawn out from the middle: everything inside the front shows, and the front itself
		// is a bright ring that fades once it reaches the rim.
		float front = u * 1.1;
		float reveal = smoothstep(0.0, 0.03, front - r);
		float edge = exp(-pow((r - front) / 0.035, 2.0)) * (1.0 - smoothstep(0.9, 1.1, front)) * step(r, 1.0);
		float rings = line(r - 0.97, 0.009) + line(r - 0.9, 0.004) + line(r - 0.6, 0.006) + line(r - 0.55, 0.003);
		float a1 = a + TIME * 0.35;
		float tick = line((fract(a1 / 6.2831853 * 48.0) - 0.5) / 48.0 * 6.2831853 * r, 0.006)
			* step(0.905, r) * step(r, 0.965) * step(0.5, fract(a1 / 6.2831853 * 12.0) + 0.6);
		float a2 = a - TIME * 0.22;
		float cells = 18.0;
		float cell = floor(a2 / 6.2831853 * cells);
		vec2 c = vec2(fract(a2 / 6.2831853 * cells) - 0.5, (r - 0.75) / 0.09);
		float runes = rune(c * vec2(1.6, 1.0), fx_hash(vec2(cell, 3.0))) * step(abs(c.y), 1.0);
		vec2 q = turn(p, TIME * 0.18);
		float star = line(tri(q, 0.47), 0.006) + line(tri(-q, 0.47), 0.006);
		float fill = smoothstep(1.0, 0.85, r) * 0.07 + exp(-r * r * 3.0) * 0.06;
		t = (rings + tick + runes * 0.9 + star * 0.8 + fill) * reveal + edge * 0.9;
		heat = 0.5 + 0.5 * clamp(rings + star + edge, 0.0, 1.0);
	}
	else if (uStyle == 2)
	{
		vec3 c = fx_cells(p * 3.2 + seed * 7.0);
		vec3 c2 = fx_cells(p * 7.0 + seed * 3.0);
		float crack = (1.0 - smoothstep(0.0, 0.07, c.y)) + (1.0 - smoothstep(0.0, 0.05, c2.y)) * 0.5 * step(r, 0.6);
		float mask = smoothstep(1.0, 0.25, r + fx_noise(vec2(a * 3.0, 1.0) + seed) * 0.15);
		float cool = 1.0 - clamp(u, 0.0, 1.0);
		t = crack * mask * (0.25 + cool) + exp(-r * r * 9.0) * cool * 0.8;
		heat = 0.3 + 0.7 * cool * cool;
	}
	else if (uStyle == 3)
	{
		float g = pow(1.0 - smoothstep(0.0, 1.0, r), 2.0);
		t = g * (0.75 + 0.35 * fx_noise(p * 2.5 + vec2(TIME * 0.6, 0.0) + seed));
		heat = 0.4 + 0.5 * g;
	}
	else if (uStyle == 4)
	{
		float s = a * 3.0 + log(max(r, 0.02)) * 4.5 + TIME * uSpin;
		float arm = pow(0.5 + 0.5 * sin(s), 5.0);
		float n = fx_fbm(vec2(s * 0.6, r * 4.0 - TIME) + seed);
		float mask = smoothstep(1.0, 0.55, r) * smoothstep(0.02, 0.2, r);
		t = arm * mask * (0.5 + 0.8 * n) + exp(-pow((r - 0.14) / 0.05, 2.0)) * 0.6 + mask * 0.1;
		heat = 0.35 + 0.6 * arm * (1.0 - r);
	}
	else if (uStyle == 6)
	{
		// UV.y runs toward the plane's +Z, the way the cone faces.
		float off = abs(atan(p.x, p.y));
		float inside = 1.0 - smoothstep(uCone - 0.03, uCone, off);
		float reach = u;
		float filled = inside * step(r, reach);
		float front = exp(-abs(r - reach) / 0.03) * inside * step(r, reach + 0.03);
		float sides = (1.0 - smoothstep(0.0, 0.025, abs(off - uCone) * r)) * step(r, reach);
		float n = fx_fbm(p * 3.0 + seed + vec2(0.0, -TIME * 0.8));
		t = filled * (0.2 + 0.4 * n) * (0.4 + 0.6 * r) + front * 0.9 + sides * 0.55;
		heat = 0.4 + 0.5 * front + 0.25 * n;
	}
	else if (uStyle == 7)
	{
		float ring = exp(-pow((r - 0.85) / 0.06, 2.0));
		float n = fx_noise(vec2(a * 4.0 + TIME * 3.0, 1.0) + seed) * 0.5 + 0.5;
		t = ring * (0.55 + 0.6 * n) + exp(-pow((r - 0.85) / 0.015, 2.0)) * 0.6;
		heat = 0.45 + 0.5 * ring;
	}
	else
	{
		float edge = line(r - 0.975, 0.012);
		float filled = step(r, u);
		float front = exp(-abs(r - u) / 0.02) * step(r, u + 0.02);
		t = 0.07 + edge * 0.9 + filled * 0.16 + front * 0.6;
		heat = 0.45 + 0.4 * edge;
	}

	vec3 col = heat < 0.5 ? mix(uDeep, uColor, heat * 2.0) : mix(uColor, uHot, (heat - 0.5) * 2.0);
	ALBEDO = fx_out(col) * t * uAlpha * uGain;
}
