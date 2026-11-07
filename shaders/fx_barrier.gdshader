// A barrier round someone (FxCasts.Barrier): a see-through bubble of light, its outline glowing
// and its skin a faint web of cells whose seams shimmer as a slow band of light climbs through
// them. Hits light it up: each sends a ripple of brightened cells out across the skin from the
// point it struck. Mostly clear face on, so whoever stands inside stays readable. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_back, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);
// 0 when gone, 1 at full strength (it swells in and fades away).
uniform float uAlpha = 1.0;
// Up to four hits: xyz the way to where it struck (from the centre, unit length), w its age in
// seconds (large when there is none).
uniform vec4 uHits[4];

// How fast a hit's ripple crosses the skin (radians of arc a second), and how long it lasts.
const float RIPPLE_SPEED = 5.0;
const float RIPPLE_LIFE = 0.6;

varying vec3 vLocal;

void vertex()
{
	vLocal = VERTEX;
}

// Cells on a sphere: three planar sets blended by which way the skin faces.
vec3 cells(vec3 n)
{
	vec3 w = pow(abs(n), vec3(4.0));
	w /= w.x + w.y + w.z;
	vec3 a = fx_cells(n.yz * 3.2 + 1.7);
	vec3 b = fx_cells(n.zx * 3.2 + 5.3);
	vec3 c = fx_cells(n.xy * 3.2 + 9.1);
	return a * w.x + b * w.y + c * w.z;
}

void fragment()
{
	vec3 n = normalize(vLocal);
	vec3 cell = cells(n);
	float seam = 1.0 - smoothstep(0.0, 0.08, cell.y);
	float rim = fx_fresnel(NORMAL, VIEW, 2.5);

	// A slow band of light climbing the bubble, and a faint shimmer cell by cell.
	float band = smoothstep(0.82, 1.0, sin(n.y * 4.0 - TIME * 2.2) * 0.5 + 0.5);
	float shimmer = 0.5 + 0.5 * sin(TIME * 3.0 + cell.z * 40.0);

	// Hits: a ring of lit cells racing out from where each struck, and a flash at the spot.
	float hit = 0.0;
	for (int i = 0; i < 4; i++)
	{
		float age = uHits[i].w;
		if (age < RIPPLE_LIFE)
		{
			float arc = acos(clamp(dot(n, uHits[i].xyz), -1.0, 1.0));
			float front = age * RIPPLE_SPEED;
			float fade = 1.0 - age / RIPPLE_LIFE;
			float ring = exp(-pow((arc - front) * 3.0, 2.0));
			float flash = exp(-arc * arc * 6.0) * (1.0 - smoothstep(0.0, 0.2, age));
			hit += (ring * (0.35 + 1.2 * seam) + flash * 1.5) * fade;
		}
	}

	float t = 0.03 + rim * 0.9 + seam * (0.12 + 0.18 * shimmer + 0.5 * band) * (0.3 + 0.7 * rim) + hit;
	ALBEDO = fx_out(fx_ramp(uColor, clamp(0.35 + t * 0.6, 0.0, 1.0))) * t * uAlpha;
}
