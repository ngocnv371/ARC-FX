// A skill's small picture, on a 1x1 quad square on to the camera: the node's Y axis, as the
// camera sees it, is the picture's up (so a petal points the way it flies and a skull leans
// into its charge), its length the picture's height and the X scale its width. uStyle picks it:
//    0 a glint: a thin four-pointed sparkle, twinkling;
//    1 a petal (or a leaf): pointed at both ends, a pale rib down it;
//    2 a ring: a bubble's outline with a highlight;
//    3 a skull, its sockets burning and its jaw chattering;
//    4 a wraith: a hooded ghost with slanted eyes, its ragged tail swaying;
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
//   15 a coin, flipping.
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

void vertex()
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

void fragment()
{
	// x across, y up: +1 at the quad's top.
	vec2 p = vec2(UV.x - 0.5, 0.5 - UV.y) * 2.0;
	float d = length(p);
	float ty = p.y * 0.5 + 0.5;
	float glow = 0.0;
	float core = 0.0;

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
		float y = p.y;
		float sway = 0.14 * sin(y * 5.0 + TIME * 5.0 + uSeed * 30.0) * smoothstep(0.1, -1.0, y);
		float x = p.x - sway;
		float w = mix(0.26, 0.5, smoothstep(0.55, -0.15, y)) * smoothstep(-1.0, -0.2, y);
		if (y > 0.55)
			w = sqrt(max(0.073 - (y - 0.55) * (y - 0.55), 0.0));
		float n = fx_fbm(vec2(x * 3.0 + uSeed * 9.0, y * 2.5 + TIME * 1.8));
		float rag = smoothstep(0.0, 0.25, n + (y + 0.75) * 1.3);
		float ax = abs(x) / max(w, 0.001);
		float body = (1.0 - smoothstep(0.8, 1.0, ax)) * rag;
		float rimf = smoothstep(0.35, 0.95, ax);
		glow = body * (0.3 + 0.6 * rimf) * (0.6 + 0.4 * n) * (0.55 + 0.45 * smoothstep(-1.0, 0.4, y));
		vec2 e = vec2(abs(x) - 0.1, y - 0.5 - (abs(x) - 0.1) * 0.6);
		float eyes = exp(-(e.x * e.x * 500.0 + e.y * e.y * 1800.0));
		core = eyes * 1.6 + body * rimf * 0.2;
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
		glow = body * (0.22 + 0.6 * smoothstep(-0.14, 0.0, h)) + exp(-max(h, 0.0) * 14.0) * 0.3 * step(0.0, h);
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
		glow = m * 0.8 + exp(-max(d1, 0.0) * 8.0) * 0.2 * step(0.0, d1);
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
	else
	{
		float sq = max(abs(cos(TIME * 7.0 + uSeed * 30.0)), 0.12);
		vec2 q = vec2(p.x / sq, p.y);
		float dq = length(q);
		float body = 1.0 - smoothstep(0.82, 0.9, dq);
		float rim = exp(-pow((dq - 0.8) / 0.06, 2.0));
		glow = body * 0.5 + rim * 0.6;
		core = rim * 0.4 + exp(-pow(q.x - 0.2, 2.0) * 30.0) * body * 0.3;
	}
	// Nothing reaches the quad's edge at full strength.
	float inside = 1.0 - smoothstep(0.9, 1.0, max(abs(p.x), abs(p.y)));
	ALBEDO = (fx_out(uColor) * glow + fx_out(uHot) * core) * inside * uAlpha * uGain;
}
