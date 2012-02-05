float4x4 World;
float4x4 View;
float4x4 Projection;
texture ColorTexture;
float3 AmbientLightColor;
float3 DiffuseColor;
float3 DiffuseLightColor;
float3 CameraPosition;

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
	float3 finalColor;
	float4 fullColor;
	fullColor = tex2D(ColorTextureSampler, input.TexCoord);
	finalColor = float3(fullColor.x, fullColor.y, fullColor.z);
	float3 diffuse = AmbientLightColor;
	finalColor = (finalColor*1) + DiffuseColor * diffuse * fullColor.a;
	float3 view = normalize(input.View);
	clip( fullColor.a < 0.1f ? -1:1 );
    return float4(finalColor,fullColor.a);
}

technique StaticObject
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}