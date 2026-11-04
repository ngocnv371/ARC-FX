// A spell's solid shard: the ice spear's crystal, or the charred bolt inside a fire bolt.
// Flat facets (normals from the screen, so the low-poly cut shows) from deep to pale, edges
// catching uRim and a glint of uGlint sliding down it as it flies. Mostly opaque, so it reads
// as a solid thing in the middle of the light around it.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_mix, cull_back, depth_draw_always;

#include "fx_common.gdshaderinc"

uniform vec3 uDeep = vec3(0.12, 0.38, 0.75);
uniform vec3 uPale = vec3(0.7, 0.92, 1.0);
uniform vec3 uRim = vec3(1.0);
uniform vec3 uGlint = vec3(0.6, 0.85, 1.0);

varying vec3 vLocal;

void vertex()
{
	vLocal = VERTEX;
}

void fragment()
{
	vec3 facet = normalize(cross(dFdx(VERTEX), dFdy(VERTEX)));
	// Light from above and slightly in front of the camera, so the facets break up.
	float lit = clamp(dot(facet, normalize(vec3(0.3, 0.8, 0.5))) * 0.5 + 0.5, 0.0, 1.0);
	float rim = fx_fresnel(facet, VIEW, 2.0);
	vec3 color = mix(uDeep, uPale, lit * lit);
	color = mix(color, uRim, rim * 0.7);
	// A band of glint running down the shard (along local Y, which is its length).
	float glint = exp(-pow(fract(vLocal.y * 1.2 - TIME * 2.5) - 0.5, 2.0) * 120.0);
	color += uGlint * glint * 0.5;
	ALBEDO = fx_out(color);
	ALPHA = 0.9;
}
