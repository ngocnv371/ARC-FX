// Procedural noise for the skill effects: gradient noise, fractal sums of it, and cells
// (Voronoi) for cracks and bubbles. Hash-based, so no textures and no tiling seams.

vec2 fx_hash2(vec2 p)
{
	p = vec2(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)));
	return fract(sin(p) * 43758.5453);
}

float fx_hash(vec2 p)
{
	return fract(sin(dot(p, vec2(41.3, 289.1))) * 45758.5453);
}

// Gradient noise in about -1..1.
float fx_noise(vec2 p)
{
	vec2 i = floor(p);
	vec2 f = fract(p);
	vec2 u = f * f * (3.0 - 2.0 * f);
	float a = dot(fx_hash2(i) * 2.0 - 1.0, f);
	float b = dot(fx_hash2(i + vec2(1.0, 0.0)) * 2.0 - 1.0, f - vec2(1.0, 0.0));
	float c = dot(fx_hash2(i + vec2(0.0, 1.0)) * 2.0 - 1.0, f - vec2(0.0, 1.0));
	float d = dot(fx_hash2(i + vec2(1.0, 1.0)) * 2.0 - 1.0, f - vec2(1.0, 1.0));
	return mix(mix(a, b, u.x), mix(c, d, u.x), u.y) * 1.4;
}

// Four octaves of gradient noise, in about 0..1.
float fx_fbm(vec2 p)
{
	float sum = 0.0;
	float amp = 0.5;
	for (int i = 0; i < 4; i++)
	{
		sum += amp * fx_noise(p);
		p = mat2(vec2(1.6, 1.2), vec2(-1.2, 1.6)) * p;
		amp *= 0.5;
	}
	return sum * 0.5 + 0.5;
}

// Cells: x is the distance to the nearest point, y the gap to the second nearest (0 on a
// border, so small y draws cracks), z a random value per cell.
vec3 fx_cells(vec2 p)
{
	vec2 i = floor(p);
	vec2 f = fract(p);
	float f1 = 8.0;
	float f2 = 8.0;
	float id = 0.0;
	for (int y = -1; y <= 1; y++)
	{
		for (int x = -1; x <= 1; x++)
		{
			vec2 g = vec2(float(x), float(y));
			vec2 o = fx_hash2(i + g);
			float d = length(g + o - f);
			if (d < f1)
			{
				f2 = f1;
				f1 = d;
				id = fx_hash(i + g);
			}
			else if (d < f2)
				f2 = d;
		}
	}
	return vec3(f1, f2 - f1, id);
}

// 3D value noise in 0..1, for shapes without a flat UV (the blast dome).
float fx_noise3(vec3 p)
{
	vec3 i = floor(p);
	vec3 f = fract(p);
	f = f * f * (3.0 - 2.0 * f);
	float n = dot(i, vec3(1.0, 57.0, 113.0));
	vec4 h = fract(sin(vec4(n, n + 1.0, n + 57.0, n + 58.0)) * 43758.5453);
	vec4 k = fract(sin(vec4(n + 113.0, n + 114.0, n + 170.0, n + 171.0)) * 43758.5453);
	vec4 z = mix(h, k, f.z);
	vec2 y = mix(z.xy, z.zw, f.y);
	return mix(y.x, y.y, f.x);
}

// A heat ramp through the effect's colour: dark below it, toward white above it.
// t runs 0..1 (about 0.5 is the colour itself).
vec3 fx_ramp(vec3 color, float t)
{
	vec3 low = color * color * 0.4;
	vec3 high = mix(color, vec3(1.0, 0.97, 0.85), 0.75);
	return t < 0.5 ? mix(low, color, t * 2.0) : mix(color, high, (t - 0.5) * 2.0);
}
