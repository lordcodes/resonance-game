float4x4 World;
float4x4 View;
float4x4 Projection;
texture ColorTexture;
texture DispMap;
float3 AmbientLightColor;
float3 DiffuseColor;
float3 LightDirection;
float3 DiffuseLightColor;
float3 LightDirection2;
float3 DiffuseLightColor2;
float4 SpecularColorPower;
float3 SpecularLightColor;
float3 CameraPosition;
bool doDisp;

sampler DispMapSampler = sampler_state
{
	Texture   = (DispMap);
	MipFilter = None;
	MinFilter = Point;
	MagFilter = Point;
	AddressU  = Clamp;
	AddressV  = Clamp;
};

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
	float3 Normal   : TEXCOORD1;
	float3 View     : TEXCOORD2;
	float height   : COLOR;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	float height = tex2Dlod(DispMapSampler, input.TexCoord);
    float4 worldPosition = mul(input.Position, World);
	
	if(doDisp)
	{
		worldPosition.y = height;
	}

    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
	output.Normal = mul(input.Normal, World);
	output.View = CameraPosition - worldPosition;
	output.height = height;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 normal = normalize(input.Normal);
	float3 finalColor = tex2D(ColorTextureSampler, input.TexCoord);
	float4 fullColor = tex2D(ColorTextureSampler, input.TexCoord);
	//float3 finalColor = float3(0,0,0);
	float3 diffuse = AmbientLightColor;
	float NdotL = saturate(dot(normal,LightDirection));
	diffuse += NdotL * DiffuseLightColor;
	NdotL = saturate(dot(normal, LightDirection2));
	diffuse += NdotL * DiffuseLightColor2;
	finalColor = (finalColor*1) + DiffuseColor * diffuse * fullColor.a;
	float3 view = normalize(input.View);
	float3 halfF = normalize(view + LightDirection);
	float NdotH = saturate(dot(normal,halfF));
	float specular = 0;
	if (NdotL != 0) specular += pow(NdotH, SpecularColorPower.w) * SpecularLightColor;
	finalColor += SpecularColorPower.xyz * specular * fullColor.a;
	clip( fullColor.a < 0.1f ? -1:1 );
	if(doDisp)
	{
		if(input.height < 0)
		{
			finalColor += input.height*1.2;
		}
	}
    return float4(finalColor,fullColor.a);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

struct VS_OUTPUT
{
    float4 position : POSITION;
    float4 color : COLOR0;
};

VS_OUTPUT Transform(
    float4 Pos  : POSITION, 
    float4 Color : COLOR0 )
{
    VS_OUTPUT Out = (VS_OUTPUT)0;

    Out.position = mul(Pos, Projection);
    Out.color = Color;

    return Out;
}

float4 ApplyAPixelShader( VS_OUTPUT vsout ) : COLOR
{
	return vsout.color;
}

technique TransformTechnique
{
    pass P0
    {
        vertexShader = compile vs_2_0 Transform();
        pixelShader = compile ps_2_0 ApplyAPixelShader();
    }
}

