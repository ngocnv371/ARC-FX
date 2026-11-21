// Shapes shared by fx_mark and fx_glyph: a segment, a rounded box, a hexagon grid, a
// snowflake, a heart and a skull, each as a distance or a brightness.
#ifndef FX_SHAPES
#define FX_SHAPES

float fx_seg(vec2 p, vec2 a, vec2 b)
{
	vec2 pa = p - a;
	vec2 ba = b - a;
	float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
	return length(pa - ba * h);
}

float fx_box(vec2 p, vec2 half_size, float round)
{
	vec2 q = abs(p) - half_size + round;
	return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - round;
}

// Distance to the nearest hexagon border, for a grid of unit-wide hexagons.
float fx_hex(vec2 p)
{
	vec2 s = vec2(1.0, 1.7320508);
	vec4 c = floor(vec4(p, p - vec2(0.5, 1.0)) / s.xyxy) + 0.5;
	vec4 h = vec4(p - c.xy * s, p - (c.zw + 0.5) * s);
	vec2 q = dot(h.xy, h.xy) < dot(h.zw, h.zw) ? h.xy : h.zw;
	q = abs(q);
	return 0.5 - max(dot(q, s * 0.5), q.x);
}

// A six-armed snowflake within the unit circle, its lines w thick: arms, three pairs of
// branches and a hexagon.
float fx_flake(vec2 p, float w)
{
	float r = length(p);
	float a = atan(p.y, p.x);
	float s = mod(a + 0.5235988, 1.0471976) - 0.5235988;
	vec2 q = r * vec2(cos(s), abs(sin(s)));
	vec2 up = vec2(0.5, 0.8660254);
	float d = fx_seg(q, vec2(0.06, 0.0), vec2(0.92, 0.0));
	d = min(d, fx_seg(q, vec2(0.34, 0.0), vec2(0.34, 0.0) + up * 0.24));
	d = min(d, fx_seg(q, vec2(0.56, 0.0), vec2(0.56, 0.0) + up * 0.2));
	d = min(d, fx_seg(q, vec2(0.76, 0.0), vec2(0.76, 0.0) + up * 0.13));
	d = min(d, abs(q.x - 0.2));
	return (1.0 - smoothstep(w, w * 2.0 + 0.004, d)) + exp(-d * d / (w * w * 8.0)) * 0.4;
}

// Distance to a heart filling about x -0.5..0.5, y 0..1 (iq's).
float fx_heart(vec2 p)
{
	p.x = abs(p.x);
	if (p.y + p.x > 1.0)
	{
		vec2 q = p - vec2(0.25, 0.75);
		return length(q) - 0.35355339;
	}
	vec2 a = p - vec2(0.0, 1.0);
	vec2 b = p - 0.5 * max(p.x + p.y, 0.0);
	return sqrt(min(dot(a, a), dot(b, b))) * sign(p.x - p.y);
}

// A skull facing out of the plane, crown toward +y, within about -1..1; open drops its jaw.
// x is the bone, y the embers in its sockets, z a glow round it.
vec3 fx_skull(vec2 p, float open)
{
	float cranium = length(p - vec2(0.0, 0.22)) - 0.6;
	float cheek = fx_box(p - vec2(0.0, -0.3), vec2(0.36, 0.22), 0.12);
	float jaw = fx_box(p - vec2(0.0, -0.68 - open), vec2(0.27, 0.1), 0.08);
	float d = min(min(cranium, cheek), jaw);
	vec2 e = vec2(abs(p.x) - 0.25, p.y - 0.1);
	float eye = length(e / vec2(0.2, 0.18)) - 1.0;
	// A slanted brow cuts the top of each socket, so it glares.
	float brow = p.y - (0.2 + 0.7 * (abs(p.x) - 0.22));
	float socket = step(eye, 0.0) * step(brow, 0.0);
	float nose = step(abs(p.x), (-0.1 - p.y) * 0.45) * step(-0.3, p.y) * step(p.y, -0.1);
	float teeth = step(p.y, -0.4) * step(-0.52, p.y) * step(abs(p.x), 0.3) * step(abs(fract(p.x * 7.0) - 0.5), 0.12);
	float bone = (1.0 - smoothstep(0.0, 0.03, d)) * (1.0 - socket) * (1.0 - nose) * (1.0 - teeth * 0.7);
	bone *= 0.5 + 0.5 * smoothstep(-0.45, 0.0, d);
	vec2 g = vec2(abs(p.x) - 0.25, p.y - 0.06);
	float ember = exp(-dot(g, g) * 70.0);
	float aura = d > 0.0 ? exp(-d * 5.0) : 0.0;
	return vec3(bone, ember, aura);
}

#endif
