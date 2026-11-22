// A skill's small picture, on a 1x1 quad. With the instance uniform uFlat set it lies in the
// node's own plane, turned however the node is (a card, a shard of glass, a petal: flat things
// that are somewhere and face some way). Otherwise it is square on to the camera: the node's Y
// axis, as the camera sees it, is the picture's up (so a petal points the way it flies and a skull leans
// into its charge), its length the picture's height and the X scale its width. uStyle picks it:
//    0 a glint: a thin four-pointed sparkle, twinkling;
//    1 a petal (or a leaf): pointed at both ends, a pale rib down it;
//    2 a ring: a bubble's outline with a highlight;
//    3 a skull, its sockets burning and its jaw chattering;
//    4 a wraith: a figure of luminous smoke (head, shoulders, a trailing robe), its edges
//      brightest and torn by drifting noise, motes of light round it;
//    5 a feather: a shaft and combed vanes;
//    6 a heart;
//    7 a puff of smoke or mist;
//    8 a butterfly, its wings beating;
//    9 a crescent moon;
//   10 a shard: a cut gem;
//   11 a snowflake, slowly turning;
//   12 an eye with a slit pupil;
//   13 a bird in flight, wings beating;
//   14 a wisp: a small flame licking upward;
//   15 a coin, flipping;
//   16 a note of music;
//   17 a five-pointed star;
//   18 a cross;
//   19 a claw mark: three raking cuts;
//   20 a playing card;
//   21 an hourglass, its sand running;
//   22 a horned mask with burning eyes and a grin;
//   23 a sword, point up;
//   24 an arrow, point up;
//   25 a shield;
//   26 a crown with jewels on its points;
//   27 a daisy, slowly turning.
// uSeed sets each one's own rhythm. Faded with the instance uniform uAlpha. Additive.
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

instance uniform float uProgress = 0.0;
instance uniform float uAlpha = 1.0;
instance uniform float uSeed = 0.0;
// The share of the picture, from its top down, that is hidden: sunk into what it has struck
// (an arrow in the ground shows only its tail end).
instance uniform float uSink = 0.0;
// Above a half, the picture keeps the node's own orientation instead of turning to the camera.
instance uniform float uFlat = 0.0;

// A soft union of two distances, rounded over k.
float fx_smin(float a, float b, float k)
{
	float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
	return mix(b, a, h) - k * h * (1.0 - h);
}

void vertex()
{
	if (uFlat > 0.5)
	{
		VERTEX = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
		MODELVIEW_MATRIX = VIEW_MATRIX;
	}
	else
	{
	vec3 centre = MODEL_MATRIX[3].xyz;
	vec3 axis = MODEL_MATRIX[1].xyz;
	float tall = length(axis);
	float wide = length(MODEL_MATRIX[0].xyz);
	vec3 view = normalize(INV_VIEW_MATRIX[3].xyz - centre);
	vec3 up = axis - view * dot(axis, view);
	up = length(up) < 1e-4 * max(tall, 1e-4) ? INV_VIEW_MATRIX[1].xyz : normalize(up);
	vec3 side = normalize(cross(up, view));
	VERTEX = centre + side * VERTEX.x * wide + up * VERTEX.y * tall;
	MODELVIEW_MATRIX = VIEW_MATRIX;
	}
}

void fragment()
{
	// x across, y up: +1 at the quad's top.
	vec2 p = vec2(UV.x - 0.5, 0.5 - UV.y) * 2.0;
	float d = length(p);
	float ty = p.y * 0.5 + 0.5;
	float glow = 0.0;
	float core = 0.0;
	// For a halo round a shape: gone well before the quad's edge, so the quad never shows.
	float soft = 1.0 - smoothstep(0.55, 0.96, max(abs(p.x), abs(p.y)));

	if (uStyle == 0)
	{
		float tw = 0.6 + 0.4 * sin(TIME * 7.0 + uSeed * 40.0);
		vec2 q = p / tw;
		float tall = exp(-abs(q.x) * 38.0) * pow(max(1.0 - abs(q.y), 0.0), 2.0);
		float wide = exp(-abs(q.y) * 38.0) * pow(max(1.0 - abs(q.x) * 1.4, 0.0), 2.0) * 0.8;
		float arms = tall + wide;
		core = exp(-d * d * 60.0) + arms * 0.7;
		glow = arms + exp(-d * d * 10.0) * 0.35;
	}
	else if (uStyle == 1)
	{
		float w = 0.6 * pow(max(sin(3.14159265 * pow(ty, 0.75)), 0.0), 0.9);
		float x = abs(p.x) / max(w, 0.001);
		float body = 1.0 - smoothstep(0.75, 1.0, x);
		float rib = exp(-p.x * p.x * 400.0) * smoothstep(0.0, 0.2, ty) * smoothstep(1.0, 0.6, ty);
		glow = body * (0.45 + 0.4 * x) * (0.5 + 0.5 * ty);
		core = body * rib * 0.6 + body * pow(ty, 3.0) * 0.25;
	}
	else if (uStyle == 2)
	{
		float ring = exp(-pow((d - 0.82) / 0.05, 2.0));
		vec2 h = p - vec2(-0.35, 0.4);
		glow = ring + (1.0 - smoothstep(0.0, 0.85, d)) * 0.08;
		core = exp(-dot(h, h) * 40.0) * 0.8 + ring * 0.35 * smoothstep(-0.2, 0.8, p.y - p.x);
	}
	else if (uStyle == 3)
	{
		vec3 k = fx_skull(p * 1.08, 0.05 + 0.05 * sin(TIME * 14.0 + uSeed * 20.0));
		float flick = 0.8 + 0.2 * sin(TIME * 23.0 + uSeed * 31.0);
		// The glow dies away well inside the quad, so its square never shows.
		glow = k.x * 0.75 + k.z * 0.35 * (1.0 - smoothstep(0.5, 0.95, d));
		core = k.y * 1.6 * flick + k.x * 0.3;
	}
	else if (uStyle == 4)
	{
		// A figure of luminous smoke: head, shoulders, arms and a robe trailing away to nothing,
		// its outline torn by drifting noise, its inside streaked with smoke climbing through it,
		// its edges brightest, motes of light hanging round it.
		float t = TIME + uSeed * 30.0;
		vec2 flow = vec2(fx_fbm(p * 2.6 + vec2(uSeed * 9.0, -t * 0.5)), fx_fbm(p * 2.6 + vec2(4.7 - t * 0.35, uSeed * 5.0))) - 0.5;
		vec2 q = p + flow * (0.1 + 0.22 * smoothstep(0.3, -1.0, p.y));
		float sway = 0.1 * sin(q.y * 4.5 + t * 2.6) * smoothstep(0.0, -1.0, q.y);
		float head = length(q - vec2(0.0, 0.6)) - 0.19;
		float chest = fx_box(q - vec2(0.0, 0.2), vec2(0.33, 0.19), 0.16);
		float arm = fx_seg(vec2(abs(q.x), q.y), vec2(0.33, 0.28), vec2(0.43, -0.16)) - 0.07;
		float width = mix(0.27, 0.015, smoothstep(0.05, -0.95, q.y));
		float robe = max(abs(q.x - sway) - width, max(q.y - 0.1, -0.97 - q.y));
		float body = fx_smin(fx_smin(head, chest, 0.09), fx_smin(arm, robe, 0.08), 0.08);
		// Smoke climbing through it: thin bright ridges.
		float n = fx_fbm(q * vec2(3.2, 2.2) + vec2(uSeed * 3.0, -t * 0.9));
		float ridges = pow(1.0 - abs(2.0 * n - 1.0), 2.5);
		float torn = body + (n - 0.5) * 0.16 * (1.0 + 2.0 * smoothstep(0.0, -1.0, p.y));
		float figure = 1.0 - smoothstep(-0.05, 0.05, torn);
		float thin = smoothstep(-1.0, -0.25, p.y);
		float rim = exp(-pow(torn / 0.045, 2.0));
		float halo = exp(-max(torn, 0.0) * 7.0) * step(0.0, torn) * (0.35 + 0.65 * n) * soft;
		glow = figure * (0.2 + 0.55 * ridges) * (0.45 + 0.55 * thin) + rim * 0.55 * (0.4 + 0.6 * thin) + halo * 0.3;
		vec2 e = vec2(abs(q.x) - 0.075, q.y - 0.62 - (abs(q.x) - 0.075) * 0.5);
		float eyes = exp(-(e.x * e.x * 1500.0 + e.y * e.y * 4500.0));
		// Motes: a few to a cell, each blinking in its own time, thick near the figure.
		vec2 g = p * 7.0 + vec2(0.0, -t * 0.25);
		vec2 cell = floor(g);
		vec2 spot = fx_hash2(cell + uSeed * 17.0);
		float blink = pow(0.5 + 0.5 * sin(t * (2.0 + 4.0 * spot.x) + spot.y * 40.0), 4.0);
		vec2 off = fract(g) - 0.2 - 0.6 * spot;
		float motes = exp(-dot(off, off) * 260.0) * blink * exp(-max(torn, 0.0) * 5.0) * soft;
		core = eyes * 1.5 + rim * ridges * 0.55 * thin + figure * pow(ridges, 3.0) * 0.3 + motes * 1.2;
	}
	else if (uStyle == 5)
	{
		float w = 0.5 * pow(max(sin(3.14159265 * pow(ty, 0.6)), 0.0), 0.7) * smoothstep(0.0, 0.25, ty);
		float x = abs(p.x) / max(w, 0.001);
		float body = (1.0 - smoothstep(0.8, 1.0, x)) * step(0.12, ty);
		float barbs = 0.6 + 0.4 * sin(p.y * 22.0 - abs(p.x) * 30.0);
		float shaft = exp(-p.x * p.x * 900.0);
		glow = body * barbs * (0.4 + 0.4 * (1.0 - x));
		core = shaft * 0.8 * smoothstep(1.0, 0.5, ty) + body * 0.15;
	}
	else if (uStyle == 6)
	{
		float beat = 1.0 + 0.06 * sin(TIME * 8.0 + uSeed * 20.0);
		float h = fx_heart(vec2(p.x * 0.62, p.y * 0.62 + 0.52) * beat);
		float body = 1.0 - smoothstep(-0.02, 0.0, h);
		glow = body * (0.22 + 0.6 * smoothstep(-0.14, 0.0, h)) + exp(-max(h, 0.0) * 14.0) * 0.3 * step(0.0, h) * soft;
		vec2 s = p - vec2(-0.3, 0.35);
		core = exp(-h * h * 2500.0) * 0.8 + exp(-dot(s, s) * 30.0) * 0.5 * body;
	}
	else if (uStyle == 7)
	{
		float n = fx_fbm(p * 1.8 + uSeed * 13.0 + vec2(0.0, -TIME * 0.3));
		float body = smoothstep(1.0, 0.2, d + (0.5 - n) * 0.7) * (1.0 - smoothstep(0.7, 1.0, d));
		glow = body * 0.55;
	}
	else if (uStyle == 8)
	{
		float flap = 0.3 + 0.7 * abs(sin(TIME * 9.0 + uSeed * 50.0));
		vec2 q = vec2(abs(p.x) / flap, p.y);
		float rr = length(q);
		float phi = atan(q.y, q.x);
		float r1 = 0.95 * pow(max(cos((phi - 0.7) * 1.5), 0.0), 0.6);
		float r2 = 0.62 * pow(max(cos((phi + 0.8) * 1.8), 0.0), 0.6);
		float x = rr / max(max(r1, r2), 0.001);
		float wing = (1.0 - smoothstep(0.85, 1.0, x)) * step(0.02, q.x);
		float veins = 0.7 + 0.3 * sin(phi * 14.0);
		glow = wing * (0.3 + 0.6 * smoothstep(0.4, 1.0, x)) * veins;
		core = wing * exp(-pow((x - 0.55) / 0.12, 2.0)) * 0.35 + exp(-p.x * p.x * 600.0) * step(abs(p.y), 0.35) * 0.8;
	}
	else if (uStyle == 9)
	{
		float d1 = d - 0.8;
		float d2 = length(p - vec2(0.38, 0.12)) - 0.7;
		float m = (1.0 - smoothstep(-0.02, 0.0, d1)) * smoothstep(0.0, 0.02, d2);
		glow = m * 0.8 + exp(-max(d1, 0.0) * 8.0) * 0.2 * step(0.0, d1) * (1.0 - smoothstep(0.84, 0.99, d));
		core = m * exp(-abs(d1) * 14.0) * 0.8;
	}
	else if (uStyle == 10)
	{
		float g = abs(p.x) / 0.42 + abs(p.y);
		float body = 1.0 - smoothstep(0.92, 1.0, g);
		float facet = (p.x > 0.0 ? 0.55 : 1.0) * (p.y > 0.25 ? 1.0 : 0.75);
		glow = body * facet * 0.7;
		core = body * (exp(-abs(g - 0.93) * 30.0) * 0.8 + exp(-abs(p.x) * 60.0) * 0.4);
	}
	else if (uStyle == 11)
	{
		float a = TIME * 0.5 + uSeed * 6.0;
		float c = cos(a), s = sin(a);
		float f = fx_flake(vec2(c * p.x - s * p.y, s * p.x + c * p.y), 0.02);
		glow = f * 0.8 + exp(-d * d * 5.0) * 0.2;
		core = f * 0.4;
	}
	else if (uStyle == 12)
	{
		float lid = 0.52 * (1.0 - p.x * p.x * 1.05);
		float dl = abs(p.y) - lid;
		float inside = step(dl, 0.0) * step(abs(p.x), 0.97);
		float outline = exp(-dl * dl * 900.0) * step(abs(p.x), 0.98);
		float iris = exp(-pow((d - 0.36) / 0.04, 2.0)) * inside;
		float pupil = exp(-p.x * p.x * 260.0) * step(abs(p.y), 0.3) * inside;
		glow = outline + iris + inside * 0.12;
		core = pupil + outline * 0.3;
	}
	else if (uStyle == 13)
	{
		float flap = sin(TIME * 11.0 + uSeed * 50.0);
		float ax = abs(p.x);
		float thick = 0.2 * (1.0 - ax * 0.8);
		float dy = abs(p.y - (flap * 0.7 * ax * (1.0 - 0.4 * ax) + 0.1 * ax));
		float wing = (1.0 - smoothstep(thick * 0.6, thick, dy)) * step(ax, 0.98);
		float body = exp(-(p.x * p.x * 40.0 + p.y * p.y * 14.0));
		glow = wing * 0.8 + body;
		core = body * 0.5 + wing * 0.2;
	}
	else if (uStyle == 14)
	{
		float n = fx_fbm(vec2(p.x * 2.5 + uSeed * 7.0, p.y * 2.0 - TIME * 3.0));
		float w = 0.5 * pow(max(1.0 - ty, 0.0), 0.8) * smoothstep(0.0, 0.18, ty);
		float x = abs(p.x + (n - 0.5) * 0.35 * ty) / max(w, 0.001);
		float body = (1.0 - smoothstep(0.5, 1.0, x)) * smoothstep(0.25, 0.55, n + (1.0 - ty) * 0.6);
		glow = body * 0.8;
		core = body * (1.0 - smoothstep(0.0, 0.5, x)) * (1.0 - ty) * 0.9;
	}
	else if (uStyle == 15)
	{
		float sq = max(abs(cos(TIME * 7.0 + uSeed * 30.0)), 0.12);
		vec2 q = vec2(p.x / sq, p.y);
		float dq = length(q);
		float body = 1.0 - smoothstep(0.82, 0.9, dq);
		float rim = exp(-pow((dq - 0.8) / 0.06, 2.0));
		glow = body * 0.5 + rim * 0.6;
		core = rim * 0.4 + exp(-pow(q.x - 0.2, 2.0) * 30.0) * body * 0.3;
	}
	else if (uStyle == 16)
	{
		vec2 h = p - vec2(-0.22, -0.6);
		float c = cos(0.4), s = sin(0.4);
		h = vec2(c * h.x + s * h.y, -s * h.x + c * h.y);
		float dn = (length(h / vec2(0.3, 0.21)) - 1.0) * 0.21;
		dn = min(dn, fx_seg(p, vec2(0.05, -0.55), vec2(0.05, 0.8)) - 0.04);
		dn = min(dn, fx_seg(p, vec2(0.05, 0.8), vec2(0.45, 0.35)) - 0.06);
		float body = 1.0 - smoothstep(0.0, 0.04, dn);
		glow = body * 0.8 + exp(-max(dn, 0.0) * 12.0) * 0.25 * step(0.0, dn) * soft;
		core = body * 0.35;
	}
	else if (uStyle == 17)
	{
		float an = 0.6283185;
		float b = mod(atan(p.x, p.y) + an, 2.0 * an) - an;
		vec2 q = d * vec2(cos(b), abs(sin(b)));
		vec2 tip = vec2(0.95, 0.0);
		vec2 notch = vec2(0.38 * cos(an), 0.38 * sin(an));
		vec2 e = notch - tip;
		vec2 w = q - tip;
		float ds = fx_seg(q, tip, notch);
		float body = step(0.0, e.x * w.y - e.y * w.x);
		glow = body * (0.3 + 0.55 * (1.0 - smoothstep(0.0, 0.14, ds))) + (1.0 - body) * exp(-ds * 14.0) * 0.3 * soft;
		core = body * exp(-d * d * 6.0) * 0.5 + exp(-ds * ds * 2000.0) * 0.5;
	}
	else if (uStyle == 18)
	{
		float dc = min(fx_box(p, vec2(0.13, 0.9), 0.03), fx_box(p - vec2(0.0, 0.3), vec2(0.55, 0.13), 0.03));
		float body = 1.0 - smoothstep(0.0, 0.03, dc);
		glow = body * 0.75 + exp(-max(dc, 0.0) * 9.0) * 0.35 * step(0.0, dc) * soft;
		core = body * (0.3 + 0.5 * exp(-dc * dc * 400.0));
	}
	else if (uStyle == 19)
	{
		float cl = 0.0;
		for (int i = -1; i <= 1; i++)
		{
			float x0 = float(i) * 0.42 + 0.2 * p.y * p.y - 0.1 + 0.12 * p.y;
			float wdt = 0.13 * (1.0 - p.y * p.y) * (i == 0 ? 1.0 : 0.8);
			cl = max(cl, 1.0 - smoothstep(wdt * 0.5, wdt, abs(p.x - x0)));
		}
		cl *= step(abs(p.y), 0.98);
		glow = cl * 0.8;
		core = cl * cl * 0.5 * (0.5 + 0.5 * ty);
	}
	else if (uStyle == 20)
	{
		float dk = fx_box(p, vec2(0.58, 0.88), 0.1);
		float body = 1.0 - smoothstep(0.0, 0.03, dk);
		float rimk = exp(-dk * dk * 900.0);
		float pip = 1.0 - smoothstep(0.26, 0.3, abs(p.x) / 0.7 + abs(p.y));
		vec2 cp = abs(p) - vec2(0.38, 0.66);
		float cpip = 1.0 - smoothstep(0.07, 0.1, abs(cp.x) + abs(cp.y));
		glow = body * 0.22 + rimk * 0.9 + pip * 0.8 + cpip * 0.6;
		core = rimk * 0.4 + pip * 0.4;
	}
	else if (uStyle == 21)
	{
		float ay = abs(p.y);
		float wall = abs(p.x) - (0.06 + 0.6 * ay / 0.85);
		float within = step(wall, 0.0) * step(ay, 0.85);
		float outline = exp(-wall * wall * 700.0) * step(ay, 0.85);
		float caps = 1.0 - smoothstep(0.0, 0.03, fx_box(vec2(p.x, ay - 0.9), vec2(0.72, 0.05), 0.02));
		float lvl = fract(TIME * 0.25 + uSeed);
		float sand = within * (step(p.y, -0.85 + 0.55 * lvl) + step(0.0, p.y) * step(p.y, 0.6 * (1.0 - lvl)));
		float stream = exp(-p.x * p.x * 900.0) * step(p.y, 0.0) * step(-0.85, p.y);
		glow = outline * 0.9 + caps * 0.8 + within * 0.1 + sand * 0.6 + stream * 0.6;
		core = sand * 0.3 + caps * 0.3;
	}
	else if (uStyle == 22)
	{
		float face = length(p / vec2(0.66, 0.82)) - 1.0;
		float body = 1.0 - smoothstep(0.0, 0.05, face);
		vec2 e = vec2(abs(p.x) - 0.3, p.y - 0.15 - (abs(p.x) - 0.3) * 0.7);
		float eyes = 1.0 - smoothstep(0.8, 1.0, length(e / vec2(0.2, 0.075)));
		float grin = (1.0 - smoothstep(0.035, 0.06, abs(p.y + 0.42 - 0.5 * p.x * p.x))) * step(abs(p.x), 0.4);
		float dh = fx_seg(vec2(abs(p.x), p.y), vec2(0.36, 0.62), vec2(0.62, 0.98));
		float horn = 1.0 - smoothstep(0.0, 0.1 * (1.0 - clamp((p.y - 0.62) / 0.36, 0.0, 1.0)) + 0.01, dh);
		float rimm = exp(-face * face * 60.0) * step(face, 0.0);
		glow = body * (0.18 + 0.5 * rimm) * (1.0 - eyes) * (1.0 - grin) + horn * 0.7;
		core = eyes * 1.5 + grin * 0.9;
	}
	else if (uStyle == 23)
	{
		float bl = step(abs(p.x), 0.11 * clamp((0.97 - p.y) / 0.22, 0.0, 1.0)) * step(-0.36, p.y) * step(p.y, 0.97);
		float guard = 1.0 - smoothstep(0.0, 0.02, fx_box(p - vec2(0.0, -0.4), vec2(0.34, 0.045), 0.03));
		float grip = step(abs(p.x), 0.045) * step(-0.82, p.y) * step(p.y, -0.4);
		float pommel = 1.0 - smoothstep(0.07, 0.1, length(p - vec2(0.0, -0.88)));
		float fuller = exp(-p.x * p.x * 1500.0) * bl;
		glow = bl * (0.55 + 0.3 * abs(p.x) / 0.11) + guard * 0.8 + grip * 0.5 + pommel * 0.8;
		core = fuller * 0.8 + bl * 0.2 + guard * 0.3;
	}
	else if (uStyle == 24)
	{
		float shaft = step(abs(p.x), 0.03) * step(-0.9, p.y) * step(p.y, 0.6);
		float head = step(abs(p.x), 0.2 * (1.0 - p.y) / 0.45) * step(0.55, p.y) * step(p.y, 1.0);
		// Fletching: vanes swept back from the shaft, notched at the tail.
		float fl = step(abs(p.x), 0.15 * smoothstep(-0.5, -0.66, p.y)) * step(0.15 * (-0.8 - p.y) / 0.14, abs(p.x)) * step(-0.94, p.y) * step(p.y, -0.5);
		glow = shaft * 0.8 + head + fl * 0.6;
		core = head * 0.6 + exp(-p.x * p.x * 2500.0) * shaft * 0.6;
	}
	else if (uStyle == 25)
	{
		float half_w = p.y > 0.0 ? 0.64 : 0.64 * sqrt(max(1.0 - (p.y / 0.92) * (p.y / 0.92), 0.0));
		// The top edge dips a little toward its corners, so it reads as a shield and not a box.
		float ds = max(abs(p.x) - half_w, p.y - 0.86 + 0.28 * p.x * p.x);
		float body = 1.0 - smoothstep(0.0, 0.03, ds);
		float rims = exp(-ds * ds * 500.0) * step(ds, 0.01);
		float boss = exp(-pow((length(p - vec2(0.0, 0.1)) - 0.2) / 0.03, 2.0));
		float bars = (exp(-p.x * p.x * 700.0) + exp(-(p.y - 0.1) * (p.y - 0.1) * 700.0)) * body * 0.5;
		glow = body * 0.2 + rims + boss * 0.8 + bars;
		core = rims * 0.4 + boss * 0.4;
	}
	else if (uStyle == 26)
	{
		float u2 = (p.x + 0.62) / 1.24 * 2.0;
		float peak = 1.0 - abs(fract(u2 + 0.5) - 0.5) * 2.0;
		float top = -0.2 + 0.85 * peak;
		float span = step(abs(p.x), 0.62);
		float body = span * step(-0.6, p.y) * step(p.y, top);
		float band = body * step(p.y, -0.32);
		float crest = exp(-pow(p.y - top, 2.0) * 300.0) * span * step(-0.2, p.y);
		vec2 j = vec2(abs(p.x) > 0.31 ? abs(p.x) - 0.62 : p.x, p.y - 0.68);
		float jewel = exp(-dot(j, j) * 160.0);
		float gems = exp(-(pow(fract(p.x * 2.4 + 0.5) - 0.5, 2.0) * 60.0 + pow(p.y + 0.46, 2.0) * 300.0)) * step(abs(p.x), 0.6);
		glow = body * 0.45 + band * 0.35 + crest * 0.6 + jewel;
		core = jewel * 1.2 + gems * 0.9 + crest * 0.3;
	}
	else
	{
		float aa = atan(p.x, p.y) + TIME * 0.6 + uSeed * 6.0;
		float th = fract(aa / 6.2831853 * 8.0) - 0.5;
		float reach = 0.92 * pow(max(cos(th * 3.14159265), 0.0), 0.45);
		float petal = (1.0 - smoothstep(reach - 0.08, reach, d)) * step(0.2, d);
		float centre = 1.0 - smoothstep(0.17, 0.22, d);
		glow = petal * (0.4 + 0.4 * d) * (0.75 + 0.25 * cos(th * 6.2831853));
		core = centre * 1.1 + petal * exp(-th * th * 60.0) * 0.3;
	}
	// Nothing reaches the quad's edge at full strength.
	float inside = 1.0 - smoothstep(0.9, 1.0, max(abs(p.x), abs(p.y)));
	inside *= 1.0 - smoothstep(1.0 - uSink - 0.015, 1.0 - uSink, ty) * step(0.001, uSink);
	ALBEDO = (fx_out(uColor) * glow + fx_out(uHot) * core) * inside * uAlpha * uGain;
}
