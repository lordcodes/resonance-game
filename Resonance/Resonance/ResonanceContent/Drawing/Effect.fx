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

float textureSize = 16.0f;
float texelSize =  1/16.0f;
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

float4 tex2DlodSmooth( sampler texSam, float4 uv )
{
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
	
	if(doDisp)
	{
		input.Position.y = height * 0.03;
	}

    float4 worldPosition = mul(input.Position, World);
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
	float3 finalColor;
	float4 fullColor;

	
	if(false)
	{
		finalColor = tex2DlodSmooth(DispMapSampler, input.TexCoord);
		fullColor = tex2DlodSmooth(DispMapSampler, input.TexCoord);
	}
	else
	{
		finalColor = tex2D(ColorTextureSampler, input.TexCoord);
		fullColor = tex2D(ColorTextureSampler, input.TexCoord);

	}

	
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

///////////////////////////////////

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

/////////////////////////////////////

float4x4 xBones[60];  
 
struct VSInputNmTxWeights  
{  
    float4 Position : POSITION0;  
    float3 Normal   : NORMAL0;  
    float2 TexCoord : TEXCOORD0;  
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
 
struct STextured_VSOut  
{  
    float4 Position         : POSITION0;       
    float3 Normal           : TEXCOORD0;  
    float2 TexCoords        : TEXCOORD1;  
};  
 
STextured_VSOut STexturedVertexShader(VSInputNmTxWeights input)  
{  
    STextured_VSOut Output = (STextured_VSOut)0;  
 
    Skin(input, 2);



    float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	Output.Position = mul(viewPosition, Projection);
	Output.Normal = normalize(mul( input.Normal, World ));
    Output.TexCoords = input.TexCoord;   
        
    return Output;  
}  
 
float4 STexturedPixelShader(STextured_VSOut PSIn) : Color  
{       
    //float4 diffuseColor = tex2D(ColorTextureSampler, PSIn.TexCoords);  
    float4 diffuseColor = float4(0,0,1,1);  
 
    return diffuseColor;  
}  
 
technique STextured  
{  
    Pass  
    {  
        VertexShader = compile vs_2_0 STexturedVertexShader();  
        PixelShader = compile ps_2_0 STexturedPixelShader();  
    }  
} 