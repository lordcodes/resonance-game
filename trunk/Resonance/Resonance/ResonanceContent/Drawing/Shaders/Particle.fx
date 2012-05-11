float4x4 World;
float4x4 View;
float4x4 Projection;
texture ColorTexture;
float3 AmbientLightColor;
float3 DiffuseColor;
float3 DiffuseLightColor;
float3 CameraPosition;
float4 Colour;
float xTransparency = 1;
float3 xSaturation = float3(1,1,1); 

sampler ColorTextureSampler : register(s0) = sampler_state
{
	Texture = (ColorTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 TexCoord : TEXCOORD0;
	float3 Normal   : NORMAL;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 TexCoord : TEXCOORD0;
	float3 View     : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	output.TexCoord = input.TexCoord;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.View = CameraPosition - worldPosition;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 texColor = tex2D(ColorTextureSampler, input.TexCoord);
	clip(texColor.a - 0.1);
    float4 c = float4(Colour.xyz, texColor.a*xTransparency);
	float grey = dot(c.rgb, float3(0.3, 0.59, 0.11));
	float3 saturation = lerp(grey, c, xSaturation);
	return float4(saturation, c.a);
}

technique StaticObject
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}