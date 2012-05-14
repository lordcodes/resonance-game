float4x4 World;
float4x4 View;
float4x4 Projection;
texture ColorTexture;
texture ShadowTexture;
texture DispMap;
float3 AmbientLightColor;
float3 DiffuseColor;
float3 xLightPos;
float xLightPower;
float xAmbient;
float3 LightDirection;
float3 DiffuseLightColor;
float3 LightDirection2;
float3 DiffuseLightColor2;
float4 SpecularColorPower;
float3 SpecularLightColor;
float3 CameraPosition;
float4x3 xBones[60];
float2 gvPos;
float4x4 xLightsWorldViewProjection;
float xFogStart;
float xFogEnd;
float3 xFogColor;
float xTransparency = 1;
float3 xSaturation = float3(1,1,1); 

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
	float3 Position3D: TEXCOORD3;
	float  Depth		: TEXCOORD4;
	float  XHeight		: TEXCOORD5;
	float height   : PSIZE;
};

float4 tex2DlodSmooth( sampler texSam, float4 uv )
{
	float textureSize = 32.0f;
	float texelSize =  1/32.0f;
	float4 height00 = tex2Dlod(texSam, uv);
	float4 height10 = tex2Dlod(texSam, uv + float4(texelSize, 0, 0, 0));
	float4 height01 = tex2Dlod(texSam, uv + float4(0, texelSize, 0, 0));
	float4 height11 = tex2Dlod(texSam, uv + float4(texelSize , texelSize, 0, 0));
	float2 f = frac( uv.xy * textureSize );
	float4 tA = lerp( height00, height10, f.x );
	float4 tB = lerp( height01, height11, f.x );
	return lerp( tA, tB, f.y );
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	float height = tex2DlodSmooth(DispMapSampler, input.TexCoord);
	output.TexCoord = input.TexCoord;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.Position3D = mul(input.Position, World);
	output.Normal = normalize(mul(input.Normal, (float3x3)World)); 
	output.View = CameraPosition - worldPosition;
	output.height = height;
	
	output.Depth =  output.Position.z;
	output.XHeight = worldPosition.y;
    return output;
}

float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);    
}

float ComputeFogFactor(float d, float fogStart, float fogEnd)
{
    return clamp((d - fogStart) / (fogEnd - fogStart), 0, 1) * 1;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 normal = normalize(input.Normal);
	float3 finalColor;
	float4 fullColor;
	fullColor = tex2D(ColorTextureSampler, input.TexCoord);

	float diffuseLightingFactor = DotProduct(xLightPos, input.Position3D, input.Normal);
	diffuseLightingFactor = saturate(diffuseLightingFactor);
	diffuseLightingFactor *= xLightPower;


	finalColor = float3(fullColor.x, fullColor.y, fullColor.z);
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
	
    float3 c = finalColor*(diffuseLightingFactor+xAmbient);
	c = lerp(c, xFogColor, ComputeFogFactor(input.Depth, xFogStart, xFogEnd));
	c = lerp(c, xFogColor, ComputeFogFactor(input.XHeight, 5, 15));

	float4 col = float4(c, fullColor.a * xTransparency);
	float grey = dot(col.rgb, float3(0.3, 0.59, 0.11));
	float3 saturation = lerp(grey, col, xSaturation);
	return float4(saturation* xTransparency, col.a);
}


///////////////////////////////////

 
 
struct VSInputNmTxWeights  
{  
    float4 Position : POSITION0;  
    float3 Normal   : NORMAL0;  
    float4 TexCoord : TEXCOORD0;  
    int4   Indices  : BLENDINDICES0;  
    float4 Weights  : BLENDWEIGHT0;  
};  
 
void Skin(inout VSInputNmTxWeights vin, uniform int boneCount)  
{  
    float4x3 skinning = 0;  
 
    [unroll]  
    for (int i = 0; i < boneCount; i++)  
    {  
        skinning += xBones[vin.Indices[i]] * vin.Weights[i];  
    }  
 
    vin.Position.xyz = mul(vin.Position, skinning);  
    vin.Normal = mul(vin.Normal, (float3x3)skinning);  
}  
 
struct Animation_VSOut  
{  
    float4 Position         : POSITION0;       
    float3 Normal           : TEXCOORD0;  
    float2 TexCoords        : TEXCOORD1;  
};  
 
VertexShaderOutput AnimationVertexShader(VSInputNmTxWeights input)  
{  
    VertexShaderOutput output;
    Skin(input, 2);
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
	output.Position3D = mul(input.Position, World);
	output.Normal = normalize(mul(input.Normal, (float3x3)World)); 
	output.Normal = mul(input.Normal, World);
	output.View = CameraPosition - worldPosition;
	output.height = 0;
	output.Depth =  output.Position.z;
	output.XHeight = worldPosition.y;
    return output;  
}

struct SMapVertexToPixel
{
    float4 Position     : POSITION;
    float4 Position2D    : TEXCOORD0;
};



struct SMapPixelToFrame
{
    float4 Color : COLOR0;
};
 
SMapVertexToPixel AnimationShadowVertexShader(VSInputNmTxWeights input)  
{  
    Skin(input, 2);
    SMapVertexToPixel Output = (SMapVertexToPixel)0;

    Output.Position = mul(input.Position, xLightsWorldViewProjection);
    Output.Position2D = Output.Position;

    return Output;
} 

SMapVertexToPixel ShadowMapVertexShader( float4 inPos : POSITION)
{
    SMapVertexToPixel Output = (SMapVertexToPixel)0;

    Output.Position = mul(inPos, xLightsWorldViewProjection);
    Output.Position2D = Output.Position;

    return Output;
}
 
float4 AnimationPixelShader(Animation_VSOut PSIn) : Color  
{       
    float4 diffuseColor = tex2D(ColorTextureSampler, PSIn.TexCoords);
    return diffuseColor;  
}  
 
technique Animation 
{  
    Pass  
    {  
        VertexShader = compile vs_3_0 AnimationVertexShader();  
        PixelShader = compile ps_3_0 PixelShaderFunction();  
    }  
} 
technique AnimationShadow
{  
    Pass  
    {  
        VertexShader = compile vs_3_0 AnimationShadowVertexShader();  
        PixelShader = compile ps_3_0 PixelShaderFunction();  
    }  
}

technique StaticObject
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}



SMapPixelToFrame ShadowMapPixelShader(SMapVertexToPixel PSIn)
{
    SMapPixelToFrame Output = (SMapPixelToFrame)0;            

    Output.Color = PSIn.Position2D.z/PSIn.Position2D.w;

    return Output;
}

technique ShadowMap
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 ShadowMapVertexShader();
        PixelShader = compile ps_2_0 ShadowMapPixelShader();
    }
}


struct SSceneVertexToPixel
{
    float4 Position             : POSITION;
    float4 Pos2DAsSeenByLight    : TEXCOORD0;
};

struct SScenePixelToFrame
{
    float4 Color : COLOR0;
};

sampler ShadowMapSampler = sampler_state
{
	texture = <ShadowTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = clamp;
	AddressV = clamp;
};

SSceneVertexToPixel ShadowedSceneVertexShader( float4 inPos : POSITION)
{
    SSceneVertexToPixel Output = (SSceneVertexToPixel)0;

    Output.Position = mul(inPos, Projection);    
    Output.Pos2DAsSeenByLight= mul(inPos, xLightsWorldViewProjection);    
    return Output;
}


SScenePixelToFrame ShadowedScenePixelShader(SSceneVertexToPixel PSIn)
{
    SScenePixelToFrame Output = (SScenePixelToFrame)0;    

    float2 ProjectedTexCoords;
    ProjectedTexCoords[0] = PSIn.Pos2DAsSeenByLight.x/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;
    ProjectedTexCoords[1] = -PSIn.Pos2DAsSeenByLight.y/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;

    Output.Color = tex2D(ShadowMapSampler, ProjectedTexCoords);
	Output.Color.z = 1.0f;
    return Output;
}

technique ShadowedScene
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 ShadowedSceneVertexShader();
        PixelShader = compile ps_2_0 ShadowedScenePixelShader();
    }
}