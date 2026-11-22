// A skill's ornament on the ground (or, stood upright, in the air): finer work than fx_disc's
// marks, on the same 2x2 plane, UV.y running toward the plane's +Z. uStyle picks it:
//    0 a lotus: three rings of uCount pointed petals, turning against each other;
//    1 a star: the uCount-pointed star drawn in one stroke (a pentagram, a hexagram), ringed;
//    2 rays: a sunburst of uCount wedges of uneven length, reaching out to uProgress;
//    3 ripples: uCount thin rings rolling outward at uSpin;
//    4 wings: a pair of feathered wings, +Z their top, unfolding from closed at their sides as
//      uProgress goes from 0 to 1;
//    5 a clock: a dial of ticks, a toothed ring and two hands turning at uSpin;
//    6 a snowflake;
//    7 a web: uCount spokes and sagging threads between them;
//    8 a galaxy: faint spiral arms full of twinkling stars;
//    9 a honeycomb, a pulse running out across it;
//   10 an eclipse: a thin ring with a flickering corona streaming off it;
//   11 caustics: the shifting net of light on shallow water;
//   12 a skull, crown toward -Z, ringed, its jaw chattering;
//   13 filigree: a band of scallops, beads and a woven pair of threads (a finer rune circle);
//   14 a reticle: rings, turning dashes and a cross;
//   15 a sun: a disc in a ring of long and short pointed rays;
//   16 yin and yang, turning;
//   17 a compass rose: four long points and four short, each half lit, in a ring of ticks;
//   18 a gear: a cogwheel of uCount teeth turning round a smaller one;
//   19 a maze: rings with gaps and walls between them;
//   20 a whorl: one clean spiral line of uCount / 2 turns;
//   21 scales: overlapping arcs with a sheen moving over them;
//   22 links: two rings of chain turning against each other;
//   23 a rose window: uCount panes of tracery, each glowing its own shade;
//   24 a triad: a triangle, a smaller one turned over inside it, and circles at the corners;
//   25 a chequerboard, squares lighting at random.
// All but the rays, ripples, galaxy, eclipse and caustics are drawn out from the middle behind
// a bright front as uProgress goes from 0 to 1. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"
#include "fx_shapes.gdshaderinc"

uniform int uStyle = 0;
uniform vec3 uColor = vec3(1.0);
uniform vec3 uHot = vec3(1.0);
uniform vec3 uDeep = vec3(0.2);
uniform float uGain = 1.0;
uniform float uCount = 8.0;
uniform float uSpin = 1.0;

instance uniform float uProgress = 0.0;
instance uniform float uAlpha = 1.0;

// A line along the zero of v, w wide, kept at least a pixel or so by the screen.
float line(float v, float w)
{
	float px = fwidth(v) * 1.2;
	return 1.0 - smoothstep(w, w + px, abs(v));
}

vec2 turn(vec2 p, float a)
{
	float c = cos(a), s = sin(a);
	return vec2(c * p.x - s * p.y, s * p.x + c * p.y);
}

// Signed distance to an equilateral triangle of half-side r, point toward +y (iq's).
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

void fragment()
{
	// Drawn a little inside the plane, and faded out at its rim rather than discarded there:
	// a discard leaves the screen derivatives undefined in the pixels beside it, and the thin
	// lines, which are kept smooth by those derivatives, break up into a dashed hairline.
	vec2 p = (UV - 0.5) * 2.0 * 1.06;
	float r = length(p);
	float rim_fade = 1.0 - smoothstep(1.03, 1.055, r);
	// 0 toward +Z.
	float a = atan(p.x, p.y);
	float u = clamp(uProgress, 0.0, 1.0);
	float n_count = max(uCount, 1.0);
	float spin = TIME * uSpin;
	vec2 seed = NODE_POSITION_WORLD.xz * 0.31;
	float front = u * 1.12;
	float reveal = smoothstep(0.0, 0.04, front - r);
	float edge = exp(-pow((r - front) / 0.035, 2.0)) * (1.0 - smoothstep(0.9, 1.1, front)) * smoothstep(1.04, 0.97, r);
	float t = 0.0;
	float heat = 0.5;

	if (uStyle == 0)
	{
		float lines = 0.0;
		float fill = 0.0;
		for (int i = 0; i < 3; i++)
		{
			float fi = float(i);
			float reach = 0.94 - fi * 0.3;
			float way = i == 1 ? -1.0 : 1.0;
			float th = fract((a + spin * 0.12 * way) / TAU * n_count + fi * 0.5) - 0.5;
			float rim = reach * (1.0 - pow(abs(th) * 2.0, 1.15) * 0.55);
			float d = r - rim;
			float body = step(reach * 0.3, r);
			lines += line(d, 0.007) * body;
			lines += line(th / n_count * TAU * r, 0.004) * step(d, 0.0) * step(reach * 0.5, r) * 0.45;
			fill += step(d, 0.0) * smoothstep(-0.3, 0.0, d) * 0.16 * body;
		}
		float core = line(r - 0.1, 0.006) + exp(-r * r * 40.0) * 0.6;
		t = (lines + fill + core + line(r - 0.985, 0.003)) * reveal + edge * 0.9;
		heat = 0.45 + 0.5 * clamp(lines + edge, 0.0, 1.0);
	}
	else if (uStyle == 1)
	{
		float an = 3.14159265 / n_count;
		vec2 q0 = turn(p, spin * 0.1);
		float d = 10.0;
		for (int i = 0; i < 12; i++)
		{
			if (float(i) >= n_count)
				break;
			float a0 = float(i) * 2.0 * an;
			float a2 = a0 + 4.0 * an;
			d = min(d, fx_seg(q0, 0.9 * vec2(sin(a0), cos(a0)), 0.9 * vec2(sin(a2), cos(a2))));
		}
		float star = line(d, 0.007);
		float a1 = atan(q0.x, q0.y);
		float b = mod(a1 + an, 2.0 * an) - an;
		vec2 q = r * vec2(cos(b), sin(b));
		float tips = line(length(q - vec2(0.9, 0.0)) - 0.045, 0.006);
		vec2 q2 = turn(p, -spin * 0.16);
		float b2 = mod(atan(q2.x, q2.y) + an, 2.0 * an) - an;
		float inner = line(r * cos(b2) - 0.26, 0.005);
		float rings = line(r - 0.975, 0.004) + line(r - 0.94, 0.003);
		float fill = smoothstep(1.0, 0.8, r) * 0.05 + exp(-r * r * 8.0) * 0.08;
		t = (star + tips + inner + rings + fill) * reveal + edge * 0.9;
		heat = 0.45 + 0.5 * clamp(star + rings + edge, 0.0, 1.0);
	}
	else if (uStyle == 2)
	{
		float sum = 0.0;
		for (int i = 0; i < 2; i++)
		{
			float k = i == 0 ? 1.0 : 1.7;
			float way = i == 0 ? 1.0 : -1.0;
			float aa = (a + spin * 0.15 * way) / TAU * floor(n_count * k);
			float c = floor(aa);
			float v = abs(fract(aa) - 0.5);
			float h = fx_hash(vec2(mod(c, floor(n_count * k)), 7.0 + float(i)));
			float len = (0.4 + 0.6 * h) * u;
			float w = 0.5 * (0.25 + 0.5 * h);
			float ray = pow(max(0.0, 1.0 - v / w), 2.0) * (1.0 - smoothstep(len * 0.3, len, r));
			sum += ray * (i == 0 ? 1.0 : 0.5);
		}
		t = sum + exp(-r * r * 14.0) * 0.9;
		heat = 0.4 + 0.6 * exp(-r * 2.0);
	}
	else if (uStyle == 3)
	{
		float ring = pow(0.5 + 0.5 * sin((r * n_count - spin) * TAU), 12.0);
		float mask = smoothstep(1.0, 0.5, r) * smoothstep(0.0, 0.12, r);
		t = ring * mask * 0.9 + mask * 0.06;
		heat = 0.45 + 0.5 * ring;
	}
	else if (uStyle == 4)
	{
		vec2 q = vec2(abs(p.x) - 0.04, p.y + 0.3);
		float rho = length(q);
		float phi = atan(q.y, q.x);
		// Folded, every feather lies along the same line down the back; opening fans them out
		// from there, the lowest first to its place and the highest sweeping furthest.
		float spread = max(u, 0.04);
		float folded = -1.25;
		float f = ((phi - folded) / spread + folded + 0.55) / 1.9;
		float fn = f * n_count;
		float cell = floor(fn);
		float fr = fract(fn) - 0.5;
		float cn = (cell + 0.5) / n_count;
		// Longest where the wing reaches out and up, short again toward the middle, so the pair
		// parts in a notch over the back rather than closing into a fan.
		float len = mix(0.43, 0.94, smoothstep(0.0, 0.42, cn)) * mix(1.0, 0.28, smoothstep(0.5, 1.0, cn)) * (0.75 + 0.25 * min(spread, 1.0));
		float x = rho / len;
		float w = 0.5 * (1.0 - pow(min(x, 1.0), 2.5)) * smoothstep(0.0, 0.15, x);
		float inside = step(abs(fr), w) * step(0.0, f) * step(f, 1.0) * step(x, 1.0) * step(0.0, q.x + 0.04);
		float rimf = smoothstep(w - 0.14, w, abs(fr));
		float rib = 1.0 - smoothstep(0.0, 0.05, abs(fr));
		t = inside * (0.22 + 0.7 * rimf + 0.4 * rib * x) * (0.5 + 0.5 * x);
		heat = 0.4 + 0.55 * x * inside;
	}
	else if (uStyle == 5)
	{
		float rings = line(r - 0.97, 0.004) + line(r - 0.86, 0.004) + line(r - 0.3, 0.004);
		float a12 = fract(a / TAU * 12.0) - 0.5;
		float big = line(a12 / 12.0 * TAU * r, 0.012) * step(0.76, r) * step(r, 0.86);
		float a60 = fract(a / TAU * 60.0) - 0.5;
		float small = line(a60 / 60.0 * TAU * r, 0.004) * step(0.81, r) * step(r, 0.86);
		float gear = line(r - (0.36 + 0.03 * step(0.5, fract(a / TAU * 24.0 - spin * 0.05))), 0.004);
		float hands = line(fx_seg(p, vec2(0.0), vec2(sin(spin), cos(spin)) * 0.74), 0.012)
			+ line(fx_seg(p, vec2(0.0), vec2(sin(spin * 0.25), cos(spin * 0.25)) * 0.5), 0.02);
		float fill = smoothstep(1.0, 0.8, r) * 0.05 + exp(-r * r * 60.0) * 0.8;
		t = (rings + big + small + gear + hands + fill) * reveal + edge * 0.9;
		heat = 0.45 + 0.5 * clamp(hands + big + edge, 0.0, 1.0);
	}
	else if (uStyle == 6)
	{
		float flake = fx_flake(turn(p, spin * 0.05), 0.007);
		float fill = exp(-r * r * 4.0) * 0.08;
		t = (flake + line(r - 0.975, 0.003) + fill) * reveal + edge * 0.9;
		heat = 0.45 + 0.5 * clamp(flake + edge, 0.0, 1.0);
	}
	else if (uStyle == 7)
	{
		float th = fract(a / TAU * n_count) - 0.5;
		float so = th / n_count * TAU;
		float spokes = line(so * r, 0.004);
		float v = r / (1.0 - 0.12 * (1.0 - 4.0 * th * th));
		float threads = line((fract(v * 7.0) - 0.5) / 7.0, 0.004) * step(0.1, r);
		t = (spokes + threads) * smoothstep(1.0, 0.9, r) * reveal + edge * 0.6;
		heat = 0.45 + 0.4 * clamp(spokes + threads, 0.0, 1.0);
	}
	else if (uStyle == 8)
	{
		vec2 q = turn(p, -spin * 0.2);
		float s = a * 2.0 + log(max(r, 0.02)) * 3.5 + spin;
		float arm = pow(0.5 + 0.5 * sin(s), 3.0);
		float mask = smoothstep(1.0, 0.4, r);
		float dust = fx_fbm(q * 3.0 + seed) * arm * mask * 0.5;
		vec2 g = q * n_count;
		vec2 id = floor(g);
		vec2 f = fract(g) - 0.5;
		vec2 o = (fx_hash2(id) - 0.5) * 0.7;
		float h = fx_hash(id);
		float tw = 0.5 + 0.5 * sin(TIME * (2.0 + h * 5.0) + h * 40.0);
		vec2 dd = abs(f - o);
		float star = exp(-dot(dd, dd) * (140.0 - 90.0 * h)) * step(0.45, h);
		star += (exp(-dd.x * 60.0 - dd.y * 8.0) + exp(-dd.y * 60.0 - dd.x * 8.0)) * step(0.85, h) * 0.7;
		star *= tw * mask * (0.4 + 0.6 * arm);
		t = dust + star * 1.6 + exp(-r * r * 18.0) * 0.7;
		heat = clamp(0.3 + 0.4 * arm + star, 0.0, 1.0);
	}
	else if (uStyle == 9)
	{
		float e = fx_hex(p * n_count * 0.5);
		float lines = 1.0 - smoothstep(0.0, 0.06, e);
		float pulse = 0.5 + 0.5 * sin(TIME * 3.0 * uSpin - r * 8.0);
		float mask = smoothstep(1.0, 0.85, r);
		t = (lines * (0.3 + 0.65 * pulse) + 0.05) * mask * reveal + line(r - 0.975, 0.004) * reveal + edge * 0.9;
		heat = 0.45 + 0.5 * lines * pulse;
	}
	else if (uStyle == 10)
	{
		float d = r - 0.5;
		vec2 dir = p / max(r, 0.001);
		float n = fx_fbm(dir * 1.6 + vec2(TIME * 0.3 * uSpin, 0.0) + seed) * 0.9 + fx_noise(dir * n_count * 0.5 - vec2(0.0, TIME * 0.5 * uSpin)) * 0.2;
		float ring = exp(-pow(d / 0.02, 2.0));
		float corona = step(0.0, d) * exp(-d / (0.05 + 0.3 * max(n - 0.2, 0.0))) * 0.9;
		vec2 b = p - vec2(0.354, 0.354);
		float bead = exp(-dot(b, b) * 220.0) * 1.5;
		t = (ring * 1.2 + corona + bead) * smoothstep(1.0, 0.8, r);
		heat = clamp(0.35 + 0.6 * ring + 0.3 * corona + bead, 0.0, 1.0);
	}
	else if (uStyle == 11)
	{
		vec3 c1 = fx_cells(p * n_count * 0.5 + vec2(TIME * 0.15 * uSpin, TIME * 0.1) + seed);
		vec3 c2 = fx_cells(p * n_count * 0.8 - vec2(TIME * 0.12, TIME * 0.17 * uSpin) + seed * 2.0);
		float web = (1.0 - smoothstep(0.0, 0.14, c1.y)) + (1.0 - smoothstep(0.0, 0.1, c2.y)) * 0.6;
		float mask = smoothstep(1.0, 0.6, r);
		t = (web * 0.6 + 0.12) * mask;
		heat = 0.35 + 0.5 * web;
	}
	else if (uStyle == 12)
	{
		vec3 k = fx_skull(vec2(p.x, -p.y) * 1.5, 0.05 + 0.05 * sin(TIME * 6.0));
		float rings = line(r - 0.975, 0.004) + line(r - 0.93, 0.004);
		float a24 = fract((a + spin * 0.1) / TAU * 24.0) - 0.5;
		float ticks = line(a24 / 24.0 * TAU * r, 0.006) * step(0.86, r) * step(r, 0.93);
		t = (k.x * 0.8 + k.y * 1.5 + k.z * 0.22 * smoothstep(0.86, 0.6, r) + rings + ticks) * reveal + edge * 0.9;
		heat = clamp(0.4 + 0.25 * k.x + 0.6 * k.y + 0.3 * edge, 0.0, 1.0);
	}
	else if (uStyle == 13)
	{
		float aa = a + spin * 0.08;
		float scallop = line(r - (0.84 + 0.07 * abs(sin(aa * n_count * 0.5))), 0.006);
		float th = fract(aa / TAU * n_count) - 0.5;
		float dots = exp(-(pow(th / n_count * TAU * r, 2.0) + pow(r - 0.84, 2.0)) * 1800.0);
		float ab = a - spin * 0.12;
		float weave = line(r - (0.7 + 0.035 * sin(ab * n_count)), 0.005) + line(r - (0.7 - 0.035 * sin(ab * n_count)), 0.005);
		float th2 = fract(ab / TAU * n_count * 2.0) - 0.5;
		float beads = exp(-(pow(th2 / (n_count * 2.0) * TAU * r, 2.0) + pow(r - 0.6, 2.0)) * 2500.0);
		float rings = line(r - 0.975, 0.004) + line(r - 0.94, 0.003) + line(r - 0.77, 0.003) + line(r - 0.63, 0.003) + line(r - 0.57, 0.005);
		float fill = smoothstep(0.55, 0.8, r) * smoothstep(1.0, 0.9, r) * 0.06;
		t = (rings + scallop + dots * 1.2 + weave + beads + fill) * reveal + edge * 0.9;
		heat = 0.45 + 0.5 * clamp(rings + dots + beads + edge, 0.0, 1.0);
	}
	else if (uStyle == 14)
	{
		float rings = line(r - 0.95, 0.003) + line(r - 0.5, 0.004) + line(r - 0.08, 0.006);
		float dash = step(0.5, fract((a + spin * 0.5) / TAU * n_count));
		float arcs = line(r - 0.8, 0.012) * dash + line(r - 0.66, 0.02) * step(0.7, fract((a - spin * 0.3) / TAU * 3.0));
		float cross_lines = (line(p.x, 0.004) + line(p.y, 0.004)) * step(0.12, r) * step(r, 0.4);
		float ticks = line(p.x, 0.012) * step(0.86, abs(p.y)) + line(p.y, 0.012) * step(0.86, abs(p.x));
		t = (rings + arcs + cross_lines + ticks + 0.03) * reveal + edge * 0.9;
		heat = 0.45 + 0.5 * clamp(arcs + ticks + edge, 0.0, 1.0);
	}
	else if (uStyle == 15)
	{
		float aa = (a + spin * 0.1) / TAU * n_count;
		float th = abs(fract(aa) - 0.5) * 2.0;
		float len = mix(0.42, 0.24, mod(floor(aa), 2.0));
		float d = r - (0.5 + len * (1.0 - th));
		float rays = (line(d, 0.007) + step(d, 0.0) * 0.12) * step(0.5, r);
		float disc = line(r - 0.46, 0.008) + line(r - 0.4, 0.003) + step(r, 0.4) * 0.1 + exp(-r * r * 10.0) * 0.25;
		t = (rays + disc + line(r - 0.975, 0.0025)) * reveal + edge * 0.9;
		heat = 0.45 + 0.5 * clamp(rays + disc * 0.5 + edge, 0.0, 1.0);
	}
	else if (uStyle == 16)
	{
		vec2 q = turn(p, spin * 0.3) / 0.9;
		float rq = length(q);
		float up = length(q - vec2(0.0, 0.5));
		float dn = length(q + vec2(0.0, 0.5));
		float light = q.x > 0.0 ? 1.0 : 0.0;
		if (up < 0.5)
			light = 1.0;
		if (dn < 0.5)
			light = 0.0;
		if (up < 0.13)
			light = 0.0;
		if (dn < 0.13)
			light = 1.0;
		float within = 1.0 - smoothstep(0.98, 1.0, rq);
		t = (within * (0.07 + 0.5 * light) + line(rq - 1.0, 0.012) + line(r - 0.985, 0.0025)) * reveal + edge * 0.9;
		heat = 0.3 + 0.6 * light * within;
	}
	else if (uStyle == 17)
	{
		vec2 q0 = turn(p, spin * 0.06);
		float a1 = atan(q0.x, q0.y);
		float pts = 0.0;
		float lines = 0.0;
		for (int i = 0; i < 2; i++)
		{
			float off = i == 0 ? 0.0 : 0.7853982;
			float len = i == 0 ? 0.92 : 0.58;
			float wide = i == 0 ? 0.16 : 0.12;
			float b = mod(a1 + off + 0.7853982, 1.5707963) - 0.7853982;
			vec2 q = r * vec2(cos(b), sin(b));
			float rim = wide * (1.0 - q.x / len);
			float within = step(abs(q.y), rim) * step(0.0, q.x);
			pts = max(pts, within * (q.y > 0.0 ? 0.75 : 0.2));
			lines += line(abs(q.y) - rim, 0.006) * step(q.x, len) * step(0.0, q.x);
		}
		float rings = line(r - 0.975, 0.004) + line(r - 0.72, 0.004) + line(r - 0.66, 0.003);
		float a32 = fract(a / TAU * 32.0) - 0.5;
		float ticks = line(a32 / 32.0 * TAU * r, 0.005) * step(0.66, r) * step(r, 0.72);
		t = (pts + lines + rings + ticks + exp(-r * r * 50.0) * 0.6) * reveal + edge * 0.9;
		heat = 0.4 + 0.5 * clamp(pts + lines, 0.0, 1.0);
	}
	else if (uStyle == 18)
	{
		float aa = (a + spin * 0.4) / TAU * n_count;
		float tooth = step(0.5, fract(aa));
		float d = r - mix(0.78, 0.92, tooth);
		float sides = line((fract(aa * 2.0 + 0.5) - 0.5) / (n_count * 2.0) * TAU * r, 0.006) * step(0.78, r) * step(r, 0.92);
		float ab = (a - spin * 0.8) / TAU * floor(n_count * 0.5);
		float inner = line(r - mix(0.34, 0.42, step(0.5, fract(ab))), 0.006);
		float spokes = line((fract((a + spin * 0.4) / TAU * 6.0) - 0.5) / 6.0 * TAU * r, 0.02) * step(0.42, r) * step(r, 0.58);
		float rings = line(r - 0.58, 0.006) + line(r - 0.14, 0.006);
		float fill = step(d, 0.0) * step(0.58, r) * 0.12;
		t = (line(d, 0.007) + sides + inner + spokes + rings + fill) * reveal + edge * 0.9;
		heat = 0.45 + 0.4 * clamp(line(d, 0.007) + sides + inner, 0.0, 1.0);
	}
	else if (uStyle == 19)
	{
		float rings_n = 5.2;
		float k = floor(r * rings_n + 0.5);
		float dr = (r * rings_n - k) / rings_n;
		float way = mod(k, 2.0) < 0.5 ? 1.0 : -1.0;
		float ga = fract(a / TAU * (k + 1.0) + fx_hash(vec2(k, 1.0)) + spin * 0.03 * way);
		float walls = line(dr, 0.008) * step(0.14, abs(ga - 0.5)) * step(1.5, k) * step(k, 5.5);
		float c = floor(r * rings_n);
		float m = 4.0 + c * 2.0;
		float aa = a / TAU * m + fx_hash(vec2(c, 5.0));
		float pick = step(0.45, fx_hash(vec2(mod(floor(aa + 0.5), m), c)));
		float radial = line((fract(aa + 0.5) - 0.5) / m * TAU * r, 0.008) * pick * step(1.0, c) * step(c, 4.0);
		t = (walls + radial + exp(-r * r * 60.0) * 0.8) * reveal + edge * 0.9;
		heat = 0.45 + 0.4 * clamp(walls + radial, 0.0, 1.0);
	}
	else if (uStyle == 20)
	{
		float turns = n_count * 0.5;
		float v = fract(r * turns - a / TAU + spin * 0.15) - 0.5;
		float arm = line(v / turns, 0.01);
		float soft = exp(-v * v * 40.0) * 0.25;
		float mask = smoothstep(1.0, 0.85, r) * smoothstep(0.03, 0.1, r);
		t = (arm + soft) * mask * reveal + exp(-r * r * 40.0) * 0.5 + edge * 0.6;
		heat = 0.45 + 0.45 * arm;
	}
	else if (uStyle == 21)
	{
		vec2 v = p * n_count * 0.5;
		float row = floor(v.y);
		v.x += 0.5 * mod(row, 2.0);
		vec2 c = vec2(fract(v.x) - 0.5, fract(v.y));
		float arc = line(length(c - vec2(0.0, 1.0)) - 0.62, 0.04);
		float h = fx_hash(vec2(floor(v.x), row));
		float sheen = 0.5 + 0.5 * sin(TIME * 2.0 * uSpin + h * 6.283 + r * 5.0);
		float mask = smoothstep(1.0, 0.8, r);
		t = (arc * (0.5 + 0.5 * sheen) + sheen * 0.08) * mask * reveal + line(r - 0.975, 0.004) * reveal + edge * 0.9;
		heat = 0.4 + 0.5 * arc * sheen;
	}
	else if (uStyle == 22)
	{
		float sum = 0.0;
		for (int i = 0; i < 2; i++)
		{
			float rr = i == 0 ? 0.82 : 0.52;
			float way = i == 0 ? 1.0 : -1.0;
			float cnt = floor(n_count * (i == 0 ? 2.0 : 1.4));
			float aa = (a + spin * 0.25 * way) / TAU * cnt;
			float par = mod(floor(aa), 2.0);
			float x = (fract(aa) - 0.5) / cnt * TAU * rr;
			float y = r - rr;
			float half_len = 0.62 / cnt * TAU * rr;
			float oval = 1.0 - smoothstep(0.1, 0.2, abs(length(vec2(x / half_len, y / 0.055)) - 1.0));
			float bar = (1.0 - smoothstep(0.012, 0.02, abs(y))) * step(abs(x), half_len);
			sum += mix(oval, bar, par);
		}
		t = sum * reveal + edge * 0.9;
		heat = 0.45 + 0.45 * clamp(sum, 0.0, 1.0);
	}
	else if (uStyle == 23)
	{
		float aa = (a + spin * 0.05) / TAU * n_count;
		float cell = mod(floor(aa), n_count);
		float th = fract(aa) - 0.5;
		float spokes = line((fract(aa + 0.5) - 0.5) / n_count * TAU * r, 0.007) * step(0.2, r) * step(r, 0.9);
		float so = th / n_count * TAU;
		vec2 q = r * vec2(cos(so), sin(so));
		float lobe = sin(3.14159265 / n_count);
		float circ = line(length(q - vec2(0.7, 0.0)) - 0.7 * lobe * 0.9, 0.006);
		float petal = line(length(q - vec2(0.36, 0.0)) - 0.36 * lobe * 0.95, 0.005);
		float rings = line(r - 0.975, 0.005) + line(r - 0.9, 0.006) + line(r - 0.5, 0.005) + line(r - 0.2, 0.006);
		float pane = fx_hash(vec2(cell, floor(r * 3.0)));
		float glass = (0.06 + 0.16 * pane) * step(0.2, r) * step(r, 0.9) * (0.6 + 0.4 * sin(TIME * 1.5 + cell * 1.7));
		float tracery = clamp(spokes + circ + petal + rings, 0.0, 1.0);
		t = (spokes + circ + petal + rings + glass + exp(-r * r * 30.0) * 0.3) * reveal + edge * 0.9;
		heat = mix(0.2 + 0.75 * fx_hash(vec2(cell, floor(r * 3.0) + 2.0)), 0.9, tracery);
	}
	else if (uStyle == 24)
	{
		vec2 q = turn(p, spin * 0.1);
		float big = line(tri(q, 0.75), 0.008);
		float small = line(tri(-turn(p, -spin * 0.15), 0.36), 0.006);
		float b = mod(atan(q.x, q.y) + 1.0471976, 2.0943951) - 1.0471976;
		vec2 v = r * vec2(cos(b), sin(b)) - vec2(0.866, 0.0);
		float dots = line(length(v) - 0.09, 0.006) + exp(-dot(v, v) * 300.0);
		float rings = line(r - 0.975, 0.004) + line(r - 0.433, 0.006);
		t = (big + small + dots + rings + exp(-r * r * 12.0) * 0.08) * reveal + edge * 0.9;
		heat = 0.45 + 0.5 * clamp(big + dots, 0.0, 1.0);
	}
	else
	{
		vec2 g = turn(p, 0.7853982) * n_count * 0.5;
		vec2 id = floor(g);
		float chk = mod(id.x + id.y, 2.0);
		vec2 f = abs(fract(g) - 0.5);
		float wave = 0.5 + 0.5 * sin(TIME * 3.0 * uSpin - length(id) * 1.2);
		float lit = step(0.8, fx_hash(id + floor(TIME * 2.0 * uSpin)));
		float mask = smoothstep(1.0, 0.9, r);
		t = (chk * (0.1 + 0.25 * wave) + lit * 0.5 + smoothstep(0.44, 0.5, max(f.x, f.y)) * 0.6) * mask * reveal + line(r - 0.975, 0.004) * reveal + edge * 0.9;
		heat = 0.4 + 0.5 * lit + 0.2 * chk * wave;
	}

	vec3 col = heat < 0.5 ? mix(uDeep, uColor, heat * 2.0) : mix(uColor, uHot, (heat - 0.5) * 2.0);
	ALBEDO = fx_out(col) * t * rim_fade * uAlpha * uGain;
}
