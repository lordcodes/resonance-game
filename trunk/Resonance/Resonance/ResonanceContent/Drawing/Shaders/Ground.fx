float4x4 World;
float4x4 View;
float4x4 Projection;
texture ColorTexture;
texture ReflectionTexture;
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
float4x3 xBones[60];
float2 gvPos;
float2 camPos;
float groundSize;

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

sampler ReflectionTextureSampler : register(s1) = sampler_state
{
	Texture = (ReflectionTexture);
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
	

		input.Position.y = height * 0.005;
		float4 tc = float4(((1-input.TexCoord.x)), input.TexCoord.y,0,0);
		///if(gvPos.x+0.1 > tc.x) tc.x *= 10; 
		output.TexCoord = tc;


    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);



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
	fullColor = tex2D(ColorTextureSampler, input.TexCoord.xy);
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
    fullColor =  float4(finalColor,fullColor.a);

	float hx = input.TexCoord.x + 0.15;
	float lx = input.TexCoord.x - 0.15;
	float hy = input.TexCoord.y + 0.15;
	float ly = input.TexCoord.y - 0.15;

	if(gvPos.x < hx &&  gvPos.x > lx )
	{
		if(gvPos.y < hy &&  gvPos.y > ly)
		{
			float xv = (camPos.x-lx)/(hx-lx);
			float yv = (camPos.y-ly)/(hy-ly);
			float4 nc = tex2D(ReflectionTextureSampler, float2(xv,yv));
			//if(nc[0] != 0 && nc[1] != 0 && nc[2] != 0)
			//{
				//nc = float4(-1,-1,-1,1);
				nc.xyzw = nc.xyzw*0.2;
				fullColor += nc;
			//}
		}
	}
	if(gvPos.x < input.TexCoord.x + 0.02 &&  gvPos.x > input.TexCoord.x-0.02)
	{
		if(gvPos.y < input.TexCoord.y + 0.02 &&  gvPos.y > input.TexCoord.y-0.02)
		{
			//fullColor += float4(0.2,0,0,1);
			float ds = distance(input.TexCoord, gvPos);
			if(ds < 0.02 && ds > 0.018)
			{
				fullColor += float4(0.2,0,0,1);
			}
			/*if(gvPos.y < input.TexCoord.y + 0.0005 &&  gvPos.y > input.TexCoord.y-0.0005)
			{
				fullColor += float4(1,0,0,1);
			}
			if(gvPos.x < input.TexCoord.x + 0.0005 &&  gvPos.x > input.TexCoord.x-0.0005)
			{
				fullColor += float4(1,0,0,1);
			}*/
		}
	}

	return fullColor;
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
	output.Normal = mul(input.Normal, World);
	output.View = CameraPosition - worldPosition;
	output.height = 0;
    return output;  
}
 
technique Animation 
{  
    Pass  
    {  
        VertexShader = compile vs_3_0 AnimationVertexShader();  
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