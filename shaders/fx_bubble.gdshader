// What a dead anomaly evaporates into (DeathPuffs.cs), one multimesh instance each: a 1x1 quad
// turned square on to the camera. The instance colour is the mote's colour; the custom data
// is its life so far (0..1), whether it is vapour, a seed and how far its pop has gone.
// A bubble is a thin film: a bright rim, a faint shimmering fill and a glint; popping, the
// film tears into an expanding broken ring with droplets flung outward. Vapour is a soft
// ragged cloud that swells and thins away.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_mix, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

varying vec4 vData;
varying vec3 vColor;

void vertex()
{
	vData = INSTANCE_CUSTOM;
	vColor = COLOR.rgb;
	mat4 billboard = mat4(INV_VIEW_MATRIX[0], INV_VIEW_MATRIX[1], INV_VIEW_MATRIX[2], MODEL_MATRIX[3]);
	float size = length(MODEL_MATRIX[0].xyz);
	MODELVIEW_MATRIX = VIEW_MATRIX * billboard * mat4(
		vec4(size, 0.0, 0.0, 0.0), vec4(0.0, size, 0.0, 0.0), vec4(0.0, 0.0, size, 0.0), vec4(0.0, 0.0, 0.0, 1.0));
}

void bubble(vec2 p, float r, float seed, float pop, out vec3 color, out float alpha)
{
	float px = max(fwidth(r), 1e-4);
	float angle = atan(p.y, p.x);
	if (pop <= 0.0)
	{
		// The film: a rim with a soft glow inside it, its colour sliding with angle like oil.
		float edge = 0.78;
		float rim = 1.0 - smoothstep(0.0, px * 1.5 + 0.05, abs(r - edge));
		float inside = smoothstep(edge + px, edge - px, r);
		float sheen = 0.5 + 0.5 * sin(angle * 2.0 + seed * 20.0 + TIME * 3.0);
		vec3 film = mix(vColor, vec3(0.75, 0.9, 1.0), sheen * 0.35);
		float fill = inside * (0.12 + 0.25 * smoothstep(0.3, edge, r));
		// The glint, up and to the left, and a weaker one opposite.
		float glint = exp(-dot(p - vec2(-0.3, 0.32), p - vec2(-0.3, 0.32)) * 70.0)
			+ 0.4 * exp(-dot(p - vec2(0.32, -0.3), p - vec2(0.32, -0.3)) * 160.0);
		color = mix(film, vec3(1.0), clamp(glint + rim * 0.35, 0.0, 1.0));
		alpha = clamp(max(rim * 0.95, fill) + glint, 0.0, 1.0);
		return;
	}
	// Popping: the rim bursts outward and breaks into pieces that thin away, and a few
	// droplets fly past it.
	float edge = 0.78 + pop * 0.2;
	float band = px * 1.5 + 0.06 * (1.0 - pop);
	float ring = 1.0 - smoothstep(0.0, band, abs(r - edge));
	float pieces = step(pop * 0.85, fx_hash(vec2(floor((angle + 3.1416) / 6.2832 * 9.0), seed * 31.0)));
	float drops = 0.0;
	for (int i = 0; i < 6; i++)
	{
		float a = (float(i) + fx_hash(vec2(float(i), seed * 17.0)) * 0.6) * 6.2832 / 6.0;
		vec2 at = vec2(cos(a), sin(a)) * (0.7 + pop * 0.3);
		drops += exp(-dot(p - at, p - at) * 900.0);
	}
	float flash = exp(-pop * 10.0) * smoothstep(edge, 0.0, r) * 0.5;
	float fade = 1.0 - pop;
	color = mix(vColor, vec3(1.0), 0.5 + flash);
	alpha = clamp((ring * pieces + drops) * fade + flash, 0.0, 1.0);
}

void vapour(vec2 p, float r, float t, float seed, out vec3 color, out float alpha)
{
	// A ragged edge that churns, a denser heart, all thinning as it spreads.
	vec2 q = p * 1.6 + vec2(seed * 13.0, seed * 7.0 - t * 1.5);
	float n = fx_fbm(q + vec2(TIME * 0.4, 0.0));
	float body = smoothstep(0.95, 0.25, r + (n - 0.5) * 0.7);
	float heart = smoothstep(0.6, 0.0, r);
	float life = smoothstep(0.0, 0.12, t) * pow(1.0 - t, 1.6);
	color = mix(vColor, vec3(1.0), heart * 0.4 * (1.0 - t));
	alpha = body * (0.55 + 0.25 * n) * life;
}

void fragment()
{
	vec2 p = (UV - 0.5) * 2.0;
	float r = length(p);
	vec3 color;
	float alpha;
	if (vData.y > 0.5)
		vapour(p, r, vData.x, vData.z, color, alpha);
	else
		bubble(p, r, vData.z, vData.w, color, alpha);
	if (alpha < 0.004)
		discard;
	ALBEDO = fx_out(color);
	ALPHA = alpha;
}
