// An aura's sigil (FxSigils): a disc of light on the ground at someone's feet that says what the
// aura does. An outer ring and a band of runes turn slowly one way, a dashed inner ring the
// other, a soft pulse runs out from the middle every couple of seconds, and the middle holds
// the aura's emblem, upright to the camera: a bolt, flames, a snowflake, a drop of venom, a
// cross, crossed swords, a shield, chevrons, a sunburst or a four-pointed star. Each emblem
// moves a little its own way (the bolt flickers, the flames lick, the cross beats). Drawn on a
// flat plane (2x2 before scaling), faded with the instance uniform uAlpha. Additive.
//
// Sigils that stack (FxSigilLook.Ring) are drawn in two parts instead: one RING per character, the
// same for every aura, its rings and runes turned by uTurn round an empty middle where they stand;
// and one BADGE per aura, a small medallion round its emblem that FxSigils sets outside the ring
// at that aura's own angle, so several auras sit side by side rather than on top of each other.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

// FxSigil's values.
const int LIGHTNING = 0;
const int FIRE = 1;
const int FROST = 2;
const int POISON = 3;
const int LIFE = 4;
const int MIGHT = 5;
const int GUARD = 6;
const int HASTE = 7;
const int HOLY = 8;
const int ARCANE = 9;

// Radians a second the rune band turns; the dashed ring turns back at half that.
const float SPIN = 0.35;
// The emblem fits in this share of the disc's radius, set this far toward the camera so the
// body standing on it hides less of it.
const float EMBLEM = 0.5;
const float EMBLEM_DROP = 0.1;

// What is drawn: a whole DISC, a stacking RING, or one aura's BADGE.
const int DISC = 0;
const int RING = 1;
const int BADGE = 2;
// A badge's emblem fits in this share of its radius.
const float BADGE_EMBLEM = 0.62;

uniform int uStyle = 0;
uniform int uMode = 0;
uniform vec3 uColor = vec3(1.0);

// 1 when fully there, 0 when gone; and a turn of its own, so two sigils don't spin in step.
instance uniform float uAlpha = 1.0;
instance uniform float uPhase = 0.0;
// A ring's turn in radians, set by FxSigils so its badges turn with it.
instance uniform float uTurn = 0.0;

varying vec2 vLocal;

void vertex()
{
	// Up the screen is away from the game camera, -Z.
	vLocal = vec2(VERTEX.x, -VERTEX.z);
}

vec2 rot(vec2 p, float a)
{
	float c = cos(a);
	float s = sin(a);
	return vec2(c * p.x - s * p.y, s * p.x + c * p.y);
}

float sd_segment(vec2 p, vec2 a, vec2 b)
{
	vec2 pa = p - a;
	vec2 ba = b - a;
	float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
	return length(pa - ba * h);
}

float sd_box(vec2 p, vec2 b)
{
	vec2 d = abs(p) - b;
	return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

// A teardrop standing on the origin: round foot r1, tapering up h to r2.
float sd_drop(vec2 p, float r1, float r2, float h)
{
	p.x = abs(p.x);
	float b = (r1 - r2) / h;
	float a = sqrt(1.0 - b * b);
	float k = dot(p, vec2(-b, a));
	if (k < 0.0)
		return length(p) - r1;
	if (k > a * h)
		return length(p - vec2(0.0, h)) - r2;
	return dot(p, vec2(a, b)) - r1;
}

// A star of n points reaching r; m from 2 (spiky) to n (a polygon).
float sd_star(vec2 p, float r, float n, float m)
{
	float an = PI / n;
	float en = PI / m;
	vec2 acs = vec2(cos(an), sin(an));
	vec2 ecs = vec2(cos(en), sin(en));
	float bn = mod(atan(p.x, p.y), 2.0 * an) - an;
	p = length(p) * vec2(cos(bn), abs(sin(bn)));
	p -= r * acs;
	p += ecs * clamp(-dot(p, ecs), 0.0, r * acs.y / ecs.y);
	return length(p) * sign(p.x);
}

float smin(float a, float b, float k)
{
	float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
	return mix(b, a, h) - k * h * (1.0 - h);
}

float sd_sword(vec2 p)
{
	float blade = sd_drop(vec2(p.x, p.y + 0.3), 0.1, 0.0, 1.15);
	float guard = sd_box(p - vec2(0.0, -0.36), vec2(0.3, 0.045));
	float grip = sd_box(p - vec2(0.0, -0.56), vec2(0.04, 0.16));
	float pommel = length(p - vec2(0.0, -0.76)) - 0.075;
	return min(min(blade, guard), min(grip, pommel));
}

float sd_chevron(vec2 p, float y)
{
	p.x = abs(p.x);
	return sd_segment(p, vec2(0.0, y), vec2(0.6, y - 0.42)) - 0.09;
}

// The emblem's shape in its own space (|q| < 1, y up): x the distance, y extra light on it.
vec2 emblem(vec2 q, float t)
{
	if (uStyle == LIGHTNING)
	{
		float d = min(sd_segment(q, vec2(0.32, 0.9), vec2(-0.22, 0.02)), sd_segment(q, vec2(-0.22, 0.02), vec2(0.2, 0.02)));
		d = min(d, sd_segment(q, vec2(0.2, 0.02), vec2(-0.3, -0.9)));
		// It flickers in steps, as lightning does.
		float flick = step(0.7, fx_hash(vec2(floor(t * 14.0), 3.0)));
		return vec2(d - 0.11, flick * 0.8);
	}
	if (uStyle == FIRE && uMode == BADGE)
	{
		// On a badge nothing stands on it: three tongues, licking.
		vec2 w = q;
		w.x += 0.07 * sin(q.y * 7.0 - t * 6.0) * (q.y + 0.6);
		float mid = sd_drop(w - vec2(0.0, -0.75), 0.34, 0.02, 1.6);
		float left = sd_drop(rot(w - vec2(-0.38, -0.72), 0.45), 0.22, 0.02, 0.85);
		float right = sd_drop(rot(w - vec2(0.38, -0.72), -0.45), 0.22, 0.02, 0.85);
		float flare = 0.3 * fx_noise(vec2(q.y * 3.0 - t * 4.0, q.x * 3.0));
		return vec2(smin(mid, min(left, right), 0.12), flare);
	}
	if (uStyle == FIRE)
	{
		// A ring of upright flames licking round a bed of embers, so it still reads round the
		// feet of whoever stands on it.
		float d = 1e3;
		for (int i = 0; i < 8; i++)
		{
			float a = float(i) / 8.0 * 2.0 * PI + PI / 8.0;
			vec2 base = 0.62 * vec2(sin(a), -cos(a)) - vec2(0.0, 0.12);
			float h = 0.42 + 0.12 * sin(t * 5.0 + float(i) * 2.3);
			vec2 w = q - base;
			w.x += 0.05 * sin(w.y * 9.0 - t * 7.0 + float(i)) * clamp(w.y * 2.5, 0.0, 1.0);
			d = smin(d, sd_drop(w, 0.13, 0.01, h), 0.06);
		}
		d = smin(d, abs(length(q) - 0.55) - 0.04, 0.06);
		float flare = 0.3 * fx_noise(vec2(q.y * 3.0 - t * 4.0, q.x * 3.0));
		return vec2(d, flare);
	}
	if (uStyle == FROST)
	{
		// Six arms with two pairs of branches, folded from one.
		q = rot(q, t * 0.15);
		float a = abs(mod(atan(q.y, q.x) + PI / 6.0, PI / 3.0) - PI / 6.0);
		vec2 s = length(q) * vec2(cos(a), sin(a));
		float d = sd_segment(s, vec2(0.0), vec2(0.92, 0.0));
		d = min(d, sd_segment(s, vec2(0.42, 0.0), vec2(0.62, 0.2)));
		d = min(d, sd_segment(s, vec2(0.66, 0.0), vec2(0.8, 0.13)));
		d = min(d - 0.055, abs(sd_star(q, 0.24, 6.0, 6.0)) - 0.03);
		return vec2(d, 0.0);
	}
	if (uStyle == POISON)
	{
		// A drop of venom, bubbles rising beside it.
		float d = sd_drop(vec2(q.x, -q.y) - vec2(0.0, -0.55), 0.45, 0.02, 1.05);
		d = max(d, -(length(q - vec2(-0.15, -0.3)) - 0.1));
		for (int i = 0; i < 3; i++)
		{
			float fi = float(i);
			float y = fract(t * 0.35 + fi * 0.33);
			vec2 c = vec2(mix(0.55, -0.6, step(0.5, fract(fi * 0.5))) + 0.05 * sin(t * 2.0 + fi), mix(-0.8, 0.8, y));
			float r = 0.14 - 0.04 * fi;
			d = min(d, abs(length(q - c) - r) - 0.035 + 0.06 * smoothstep(0.75, 1.0, y));
		}
		return vec2(d, 0.0);
	}
	if (uStyle == LIFE)
	{
		// A cross that beats: two quick swells, then rest.
		float beat = fract(t * 0.9);
		float swell = exp(-pow((beat - 0.08) * 22.0, 2.0)) + 0.6 * exp(-pow((beat - 0.25) * 22.0, 2.0));
		q /= 1.0 + 0.1 * swell;
		float d = min(sd_box(q, vec2(0.24, 0.78)), sd_box(q, vec2(0.78, 0.24))) - 0.06;
		return vec2(d, swell * 0.7);
	}
	if (uStyle == MIGHT)
	{
		float d = min(sd_sword(rot(q, 0.7)), sd_sword(rot(q, -0.7)));
		// A gleam runs along the blades.
		float gleam = exp(-pow((q.y - mix(-1.2, 1.2, fract(t * 0.5))) * 6.0, 2.0));
		return vec2(d, gleam * 0.8);
	}
	if (uStyle == GUARD)
	{
		// A kite shield (two circles' overlap, its top cut flat) with a border inside.
		float d = max(max(length(q - vec2(0.44, 0.2)), length(q + vec2(0.44, -0.2))) - 1.14, q.y - 0.7);
		d = max(d, -(abs(d + 0.17) - 0.035));
		// A sheen sweeps across it.
		float sweep = exp(-pow((q.x + q.y * 0.5 - mix(-1.5, 1.5, fract(t * 0.4))) * 4.0, 2.0));
		return vec2(d, sweep * 0.7);
	}
	if (uStyle == HASTE)
	{
		// Three chevrons lit one after another, pointing ahead.
		float d = 1e3;
		float lit = 0.0;
		for (int i = 0; i < 3; i++)
		{
			float y = 0.72 - float(i) * 0.6;
			float c = sd_chevron(q, y);
			float on = exp(-pow(fract(t * 1.6 - float(i) * 0.33 + 0.5) - 0.5, 2.0) * 30.0);
			lit = max(lit, on * (1.0 - smoothstep(0.0, 0.1, c)));
			d = min(d, c);
		}
		return vec2(d, lit);
	}
	if (uStyle == HOLY)
	{
		// A sun: a disc in a ring of rays that turns slowly.
		float rays = sd_star(rot(q, t * 0.2), 0.98, 12.0, 4.5);
		float sun = length(q) - 0.42;
		float d = min(max(rays, -(length(q) - 0.55)), sun);
		return vec2(d, 0.25 + 0.25 * sin(t * 2.0));
	}
	// Arcane: a four-pointed star round a smaller one turning the other way, a mote circling.
	float big = sd_star(q, 0.98, 4.0, 2.6);
	float small = abs(sd_star(rot(q, PI / 4.0 - t * 0.6), 0.5, 4.0, 2.3)) - 0.03;
	float d = min(abs(big) - 0.06, small);
	vec2 mote = 0.72 * vec2(cos(t * 1.5), sin(t * 1.5));
	d = min(d, length(q - mote) - 0.07);
	return vec2(d, 0.3 * (0.5 + 0.5 * sin(t * 3.0)));
}

// A thin line along d's zero: 1 on it, fading over w either side.
float line(float d, float w)
{
	float aa = fwidth(d);
	return 1.0 - smoothstep(w - aa, w + aa, abs(d));
}

// A band of runes round the disc between radii r0 and r1, turned by spin.
float runes(vec2 p, float r0, float r1, float spin)
{
	float r = length(p);
	if (r < r0 - 0.02 || r > r1 + 0.02)
		return 0.0;
	const float CELLS = 26.0;
	float a = (atan(p.x, p.y) + spin) / (2.0 * PI) * CELLS;
	float id = floor(a);
	// Within the cell: across in disc units, and up the band from -1 to 1.
	float width = 2.0 * PI * (r0 + r1) * 0.5 / CELLS;
	vec2 g = vec2((fract(a) - 0.5) * width, (r - (r0 + r1) * 0.5) / ((r1 - r0) * 0.5));
	g.y *= (r1 - r0) * 0.5;
	float h = (r1 - r0) * 0.5;
	vec2 k = vec2(fx_hash(vec2(id, 1.0)), fx_hash(vec2(id, 2.0)));
	vec2 k2 = vec2(fx_hash(vec2(id, 3.0)), fx_hash(vec2(id, 4.0)));
	float x = (k.x - 0.5) * width * 0.3;
	float d = sd_segment(g, vec2(x, -h * 0.8), vec2(x, h * 0.8));
	float side = k2.x < 0.5 ? -1.0 : 1.0;
	float ty = mix(-0.6, 0.6, k.y) * h;
	d = min(d, sd_segment(g, vec2(x, ty), vec2(x + side * width * 0.3, ty + h * 0.35)));
	if (k2.y > 0.45)
		d = min(d, sd_segment(g, vec2(x - side * width * 0.28, -h * 0.7), vec2(x - side * width * 0.28, h * 0.1)));
	if (k2.y > 0.8)
		d = min(d, length(g - vec2(x + side * width * 0.25, -h * 0.55)) - 0.004);
	return line(d, 0.009);
}

// A stacking ring: two rings bounding a band of runes, a soft edge of light falling inward
// from it and nothing in the middle.
float ring(vec2 p, float r)
{
	float frame = line(r - 0.965, 0.011) + line(r - 0.8, 0.008) * 0.8;
	frame += runes(p, 0.83, 0.94, uTurn) * 0.85;
	float rim = smoothstep(0.45, 0.8, r) * (1.0 - step(0.8, r)) * 0.18;
	return frame + rim;
}

// One aura's badge: its emblem in a double ring, over a soft glow.
float badge(vec2 p, float r, float t)
{
	float frame = line(r - 0.93, 0.02) + line(r - 0.8, 0.012) * 0.7;
	vec2 e = emblem(p / BADGE_EMBLEM, t);
	float d = e.x * BADGE_EMBLEM;
	float aa = fwidth(d);
	float fill = 1.0 - smoothstep(-aa, aa, d);
	float mark = fill * (0.6 + 0.6 * e.y) + line(d, 0.02) * 1.1;
	return frame + mark + 0.22 * (1.0 - r * r);
}

void fragment()
{
	vec2 p = vLocal;
	float r = length(p);
	if (r > 1.0)
		discard;
	float t = TIME + uPhase * 10.0;
	if (uMode != DISC)
	{
		float v = uMode == RING ? ring(p, r) : badge(p, r, t);
		ALBEDO = fx_out(fx_ramp(uColor, clamp(0.38 + 0.4 * v, 0.0, 1.0))) * v * uAlpha;
	}
	else
	{
		float turn = t * SPIN + uPhase * 2.0 * PI;

		// The frame: two rings bounding the runes, a dashed ring turning back inside them.
		float frame = line(r - 0.965, 0.012) + line(r - 0.73, 0.008) * 0.8;
		frame += runes(p, 0.76, 0.93, turn) * 0.9;
		float dash = step(0.45, fract((atan(p.x, p.y) - turn * 0.5) / (2.0 * PI) * 30.0));
		frame += line(r - 0.665, 0.01) * dash * 0.7;

		// The emblem: a soft fill, a bright outline and its own light.
		vec2 e = emblem((p + vec2(0.0, EMBLEM_DROP)) / EMBLEM, t);
		float d = e.x * EMBLEM;
		float aa = fwidth(d);
		float fill = 1.0 - smoothstep(-aa, aa, d);
		float edge = line(d, 0.01);
		float mark = fill * (0.55 + 0.6 * e.y) + edge * 1.1;

		// A soft floor of light, and a pulse running out every couple of seconds.
		float pulse_r = fract(t * 0.45);
		float pulse = exp(-pow((r - pulse_r) * 14.0, 2.0)) * (1.0 - pulse_r) * 0.5;
		float floor_glow = 0.16 * (1.0 - r * r) + 0.1 * smoothstep(1.0, 0.7, r) * smoothstep(0.55, 0.75, r);

		float v = frame + mark + pulse + floor_glow;
		ALBEDO = fx_out(fx_ramp(uColor, clamp(0.38 + 0.4 * v, 0.0, 1.0))) * v * uAlpha;
	}
}
