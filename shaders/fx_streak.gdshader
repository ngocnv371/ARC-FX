// A skill's spark, on a 1x1 quad. uStyle picks it:
//   0 a streak along the node's Y axis (its length is the axis's), turned round that axis to
//     face the camera: hot at its head, tapering to its tail (sparks, drops, speed lines);
//   1 a round mote, square on to the camera (its size is the node's X scale);
//   2 a four-pointed star flare, square on to the camera, its rays slowly turning.
// Faded with the instance uniform uAlpha. Additive.
shader_type spatial;
render_mode unshaded, fog_disabled, shadows_disabled, blend_add, cull_disabled, depth_draw_never;

#include "fx_common.gdshaderinc"

uniform int uStyle = 0;
uniform vec3 uColor = vec3(1.0);
uniform vec3 uHot = vec3(1.0);
uniform vec3 uDeep = vec3(0.2);
uniform float uGain = 1.0;

instance uniform float uProgress = 0.0;
instance uniform float uAlpha = 1.0;

void vertex()
{
	vec3 centre = MODEL_MATRIX[3].xyz;
	vec3 axis = MODEL_MATRIX[1].xyz;
	float len = length(axis);
	float wide = length(MODEL_MATRIX[0].xyz);
	vec3 world;
	if (uStyle == 0)
	{
		axis /= max(len, 1e-5);
		vec3 side = cross(axis, normalize(INV_VIEW_MATRIX[3].xyz - centre));
		side = length(side) < 1e-4 ? INV_VIEW_MATRIX[0].xyz : normalize(side);
		world = centre + side * VERTEX.x * wide + axis * VERTEX.y * len;
	}
	else
		world = centre + (INV_VIEW_MATRIX[0].xyz * VERTEX.x + INV_VIEW_MATRIX[1].xyz * VERTEX.y) * wide;
	VERTEX = world;
	MODELVIEW_MATRIX = VIEW_MATRIX;
}

void fragment()
{
	// x across, y along: +1 at the head (the quad's top).
	vec2 p = vec2(UV.x - 0.5, 0.5 - UV.y) * 2.0;
	float glow;
	float core;
	if (uStyle == 0)
	{
		float along = p.y * 0.5 + 0.5;
		float w = mix(0.2, 0.9, along);
		float body = exp(-pow(p.x / w, 2.0) * 3.0) * pow(along, 1.3) * smoothstep(1.0, 0.7, p.y);
		glow = body;
		core = exp(-pow(p.x / w, 2.0) * 16.0) * pow(along, 2.5) * smoothstep(1.0, 0.8, p.y);
	}
	else if (uStyle == 1)
	{
		float d = length(p);
		core = exp(-d * d * 14.0);
		glow = pow(1.0 - smoothstep(0.0, 1.0, d), 2.0) * 0.6;
	}
	else
	{
		float c = cos(TIME * 0.8), s = sin(TIME * 0.8);
		vec2 q = vec2(c * p.x - s * p.y, s * p.x + c * p.y);
		vec2 o = vec2(q.x + q.y, q.x - q.y) * 0.7071;
		float d = length(p);
		float rays = exp(-abs(q.x) * 26.0) * (1.0 - abs(q.y)) + exp(-abs(q.y) * 26.0) * (1.0 - abs(q.x));
		float minor = (exp(-abs(o.x) * 34.0) * (1.0 - abs(o.y) * 1.6) + exp(-abs(o.y) * 34.0) * (1.0 - abs(o.x) * 1.6)) * 0.45;
		core = exp(-d * d * 30.0) + max(rays, 0.0) * 0.6;
		glow = max(rays, 0.0) + max(minor, 0.0) + exp(-d * d * 6.0) * 0.5;
	}
	ALBEDO = (fx_out(uColor) * glow + fx_out(uHot) * core) * uAlpha * uGain;
}
