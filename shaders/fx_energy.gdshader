// A ball or dome of energy, on a unit sphere. uStyle picks it:
//   0 an orb: a churning, swirling glow, rim brightest, white at its heart;
//   1 a dome: a shell of glowing hexagons over a bright rim, a band of light climbing it;
//   2 a shell: rim light only, a sheen of noise on it (a burst, a ghost of someone).
// uProgress above 0 burns it away in holes. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_back, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform int uStyle = 0;
uniform vec3 uColor = vec3(1.0);
uniform vec3 uHot = vec3(1.0);
uniform vec3 uDeep = vec3(0.2);
uniform float uGain = 1.0;

instance uniform float uProgress = 0.0;
instance uniform float uAlpha = 1.0;

varying vec3 vLocal;

void vertex()
{
	vLocal = VERTEX;
}

// Distance to the nearest hexagon border, for a grid of unit-wide hexagons.
float hex_edge(vec2 p)
{
	vec2 s = vec2(1.0, 1.7320508);
	vec4 c = floor(vec4(p, p - vec2(0.5, 1.0)) / s.xyxy) + 0.5;
	vec4 h = vec4(p - c.xy * s, p - (c.zw + 0.5) * s);
	vec2 q = dot(h.xy, h.xy) < dot(h.zw, h.zw) ? h.xy : h.zw;
	q = abs(q);
	return 0.5 - max(dot(q, s * 0.5), q.x);
}

void fragment()
{
	vec3 n = normalize(vLocal);
	float rim = fx_fresnel(NORMAL, VIEW, 2.0);
	float t;
	float heat;

	if (uStyle == 0)
	{
		float spin = TIME * 1.6;
		vec3 q = vec3(n.x * cos(spin) - n.z * sin(spin), n.y, n.x * sin(spin) + n.z * cos(spin));
		float swirl = fx_noise3(q * 3.0 + vec3(0.0, TIME * 1.5, 0.0)) * 0.6 + fx_noise3(q * 6.5 - TIME) * 0.4;
		float heart = pow(1.0 - rim, 3.0);
		t = 0.25 + rim * 1.2 + swirl * 0.6 + heart * 0.5;
		heat = 0.45 + 0.35 * swirl + 0.4 * heart;
	}
	else if (uStyle == 1)
	{
		float lon = atan(n.x, n.z);
		float lat = asin(clamp(n.y, -1.0, 1.0));
		vec2 p = vec2(lon / 6.2831853 * 26.0, lat / 6.2831853 * 26.0 * 1.15);
		float e = hex_edge(p);
		float lines = 1.0 - smoothstep(0.0, 0.07, e);
		float climb = exp(-pow(fract(lat / 1.5708 * 0.8 - TIME * 0.45) - 0.5, 2.0) * 40.0);
		float flicker = fx_noise3(vec3(floor(p * 1.0), TIME * 0.5)) * 0.5 + 0.5;
		t = 0.05 + rim * 1.1 + lines * (0.18 + 0.5 * climb + 0.2 * flicker) * (0.4 + rim);
		heat = 0.45 + 0.4 * lines * climb + 0.3 * rim;
	}
	else
	{
		float sheen = fx_noise3(n * 4.0 + vec3(0.0, -TIME * 2.0, 0.0));
		t = rim * (1.1 + sheen * 0.6) + 0.04;
		heat = 0.45 + 0.45 * rim;
	}

	if (uProgress > 0.0)
	{
		float holes = fx_noise3(n * 3.5 + NODE_POSITION_WORLD * 0.2);
		float left = holes - uProgress;
		if (left < 0.0)
			discard;
		t += (1.0 - smoothstep(0.0, 0.08, left)) * 0.8;
	}

	heat = clamp(heat, 0.0, 1.0);
	vec3 col = heat < 0.5 ? mix(uDeep, uColor, heat * 2.0) : mix(uColor, uHot, (heat - 0.5) * 2.0);
	ALBEDO = fx_out(col) * t * uAlpha * uGain;
}
