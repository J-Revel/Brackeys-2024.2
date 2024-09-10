float3 GerstnerWave(
	float4 wave, float3 p, float t, inout float3 tangent, inout float3 binormal
) {
	float steepness = wave.z;
	float wavelength = wave.w;
	float k = 2 * 3.14f / wavelength;
	float c = sqrt(9.8 / k);
	float2 d = normalize(wave.xy);
	float f = k * (dot(d, p.xz) - c * t);
	float a = steepness / k;
	
	//p.x += d.x * (a * cos(f));
	//p.y = a * sin(f);
	//p.z += d.y * (a * cos(f));

	tangent += float3(
		-d.x * d.x * (steepness * sin(f)),
		d.x * (steepness * cos(f)),
		-d.x * d.y * (steepness * sin(f))
	);
	binormal += float3(
		-d.x * d.y * (steepness * sin(f)),
		d.y * (steepness * cos(f)),
		-d.y * d.y * (steepness * sin(f))
	);
	return float3(
		d.x * (a * cos(f)),
		a * sin(f),
		d.y * (a * cos(f))
	);
}
void gerstner_float(float4 wave1, float4 wave2, float4 wave3, float3 p, float t, out float3 out_pos, out float3 out_normal)
{
	float3 tangent = float3(1, 0, 0);
	float3 binormal = float3(0, 0, 1);
	out_pos = p;
	out_pos += GerstnerWave(wave1, p, t, tangent, binormal);
	out_pos += GerstnerWave(wave2, p, t, tangent, binormal);
	out_pos += GerstnerWave(wave3, p, t, tangent, binormal);
	out_normal = normalize(cross(binormal, tangent));
}
