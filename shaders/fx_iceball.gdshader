// A frost blast's globe: a bubble of cut crystal that swells from the impact, frosted inside
// with facets catching the light, then cracks all over and shatters, its pieces winking out
// one by one with bright edges. Premultiplied alpha, so it both adds light and has some body
// over what is behind it. Drawn on a sphere with few faces; the facets come from flat shading.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_premul_alpha, cull_back, depth_draw_never;

#include "fx_common.gdshaderinc"
#include "fx_noise.gdshaderinc"

uniform vec3 uColor = vec3(1.0);
// 0 at the impact, 1 when the globe has shattered.
uniform float uProgress = 0.0;

varying vec3 vLocal;
varying vec3 vSeed;

void vertex()
{
	// Each blast is cut differently: every corner pushed in or out a little.
	vSeed = NODE_POSITION_WORLD * 0.37;
	vLocal = VERTEX;
	VERTEX *= 0.85 + 0.3 * fx_noise3(VERTEX * 2.0 + vSeed * 3.0);
}

void fragment()
{
	float u = clamp(uProgress, 0.0, 1.0);
	// Flat facets: each triangle's own normal, not the smooth sphere's.
	vec3 facet = normalize(cross(dFdx(VERTEX), dFdy(VERTEX)));
	float rim = pow(1.0 - abs(dot(facet, VIEW)), 2.0);
	// Some facets catch the light, picked by their direction on the globe so they hold still.
	vec3 side = normalize(cross(dFdx(vLocal), dFdy(vLocal)));
	float glint = smoothstep(0.75, 0.95, fx_hash(floor(side.xz * 4.0 + side.y * 2.0) + vSeed.xz));

	// Frost clouding the inside.
	float n = fx_noise3(vLocal * 4.0 + vSeed) * 0.6 + fx_noise3(vLocal * 9.0 + vSeed) * 0.4;

	// Cracks spread over it before it breaks, then the pieces between them wink out.
	vec3 cells = fx_cells(vLocal.xz / (1.3 + vLocal.y) * 3.0 + vSeed.xz);
	float crack = (1.0 - smoothstep(0.0, 0.06, cells.y)) * smoothstep(0.3, 0.6, u); // to Blasts.GlobePop
	float pop = smoothstep(0.6, 1.0, u);
	float body = step(pop * 1.05, cells.z * 0.95 + 0.02);
	// Missing shards: some pieces are gone from the start, the most at the silhouette (cut by
	// facet, so the outline breaks along the crystal's edges), so it reads as a burst of ice,
	// not a solid lump. They are the pieces that would wink out first, so the shatter carries on.
	body *= step(0.1 + rim * 0.3 + u * 0.08, cells.z);
	float edge = (1.0 - smoothstep(0.0, 0.12, cells.y)) * pop;

	float flash = 1.0 - smoothstep(0.0, 0.3, u);
	float bright = clamp(0.35 + rim * 0.4 + n * 0.25 + glint * 0.5 + crack * 0.6 + edge * 0.5 + flash * 0.3, 0.0, 1.0);
	vec3 col = fx_ramp(uColor, bright) * (0.7 + rim * 1.2 + glint * 1.0 + crack * 1.2 + edge);
	float cover = body * (0.25 + 0.6 * rim + n * 0.15);
	ALBEDO = fx_out(col) * body * (0.55 + 0.45 * rim);
	ALPHA = clamp(cover * 0.7, 0.0, 1.0);
}
