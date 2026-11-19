// A skill's crescent (Crimson Slash, Whirlwind, the blows at the end of a dash): a fan lying in
// its own plane, UV.x running along the arc in the swing's direction and UV.y from the hub (0)
// out to the rim (1). The leading edge whips round the arc in the first uSweep of its life,
// leaving a thick crescent behind it, pointed at both ends and fullest in the middle: a
// white-hot rim, the colour through its body and a deep shade along its inner edge, combed with
// streaks along the swing. Once swept it thins toward the rim and burns away from its tail in
// ragged grains with glowing edges. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(0.9, 0.06, 0.08);
uniform vec3 uHot = vec3(1.0, 0.75, 0.55);
uniform vec3 uDeep = vec3(0.35, 0.0, 0.03);
uniform float uGain = 1.0;
// How thick the crescent is at its fullest, as a share of the way from the hub to the rim.
uniform float uThick = 0.4;
// The share of its life the leading edge takes to cross the arc.
uniform float uSweep = 0.3;

// 0 at the start of the swing, 1 when it is gone.
instance uniform float uProgress = 0.0;
// 1 when fully there, 0 when gone.
instance uniform float uAlpha = 1.0;

void fragment()
{
	float u = clamp(uProgress, 0.0, 1.0);
	float x = UV.x;
	float head = u / uSweep;
	float behind = head - x;
	if (behind < 0.0)
		discard;
	// How far past the sweep it is: 0 while the edge is still travelling, 1 at the end.
	float after = clamp((u - uSweep) / (1.0 - uSweep), 0.0, 1.0);
	vec2 seed = NODE_POSITION_WORLD.xz * 0.37;

	// Pointed at both ends, fullest just behind the middle; thinning toward the rim as it goes.
	float shape = pow(sin(3.14159 * clamp(x, 0.0, 1.0)), 0.55) * (0.85 + 0.15 * x);
	float w = uThick * shape * (1.0 - after * 0.7);
	float d = 1.0 - UV.y;
	float ragged = (fx_noise(vec2(x * 7.0, u * 3.0) + seed) * 0.5 + 0.5) * 0.25 * w;
	float inside = 1.0 - smoothstep(w * 0.8 - ragged, w + 0.01 - ragged, d);
	if (inside <= 0.0)
		discard;
	// 1 at the rim, 0 at the crescent's inner edge.
	float t = clamp(1.0 - d / max(w, 1e-3), 0.0, 1.0);

	// Streaks combed along the swing, sliding with it.
	float comb = fx_fbm(vec2(x * 2.5 - u * 3.0, UV.y * 38.0) + seed);
	float streak = 0.6 + 0.8 * smoothstep(0.35, 0.75, comb);

	// The leading edge flares; the rim burns white-hot.
	float lead = exp(-behind * 9.0) * (1.0 - after);
	float rim = pow(t, 7.0);
	vec3 body = mix(uDeep, uColor, smoothstep(0.0, 0.65, t));
	vec3 col = body * streak + uHot * (rim * 1.4 + lead * 0.8 * t);

	// Burns away from the tail, the rim last, its edges glowing as they go.
	float grain = fx_fbm(vec2(x * 9.0, UV.y * 7.0) + seed * 3.0);
	float keep = grain * 0.6 + x * 0.35 + t * 0.25 - after * 1.15;
	if (keep < 0.0)
		discard;
	float ember = (1.0 - smoothstep(0.0, 0.06, keep)) * step(0.001, after);
	col += uHot * ember * 0.9;

	float fadeIn = smoothstep(0.0, 0.08, behind + 0.02);
	float a = inside * fadeIn * (1.0 - after * after * 0.6);
	ALBEDO = fx_out(col) * a * uAlpha * uGain;
}
