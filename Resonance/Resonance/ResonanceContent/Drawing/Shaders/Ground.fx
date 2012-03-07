float4x4 World;
float4x4 View;
float4x4 Projection;
texture ColorTexture;
texture ShadowTexture;
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
float3 xLightPos;
float xLightPower;
float xAmbient;
float4x4 xLightsWorldViewProjection;
float4x4 xWorldViewProjection;


sampler ShadowMapSampler = sampler_state
{
	texture = <ShadowTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter=LINEAR;
	AddressU = clamp;
	AddressV = clamp;
};

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
	float3 Position3D: TEXCOORD3;
	float height   : PSIZE;
    float4 Pos2DAsSeenByLight    : TEXCOORD4;
};

float4 tex2DlodSmooth( sampler texSam, float4 uv )
{
	float textureSize = 128.0f;
	float texelSize =  1/128.0f;
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


	
	output.Position3D = mul(input.Position, World);
	output.Normal = normalize(mul(input.Normal, (float3x3)World)); 
	output.View = CameraPosition - worldPosition;
	output.height = height;   
    output.Pos2DAsSeenByLight= mul(input.Position, xLightsWorldViewProjection);   
    return output;
}

float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);    
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

	float hx = input.TexCoord.x + 0.19;
	float lx = input.TexCoord.x - 0.19;
	float hy = input.TexCoord.y + 0.19;
	float ly = input.TexCoord.y - 0.19;
	
	if(camPos.x < hx &&  camPos.x > lx && camPos.y < hy &&  camPos.y > ly)
	{
		float xv = (camPos.x-lx)/(hx-lx);
		float yv = (camPos.y-ly)/(hy-ly);
		float4 nc = tex2D(ReflectionTextureSampler, float2(xv,yv));
		nc.xyzw = nc.xyzw*0.2;
		fullColor += nc;
	}

	if(gvPos.x < input.TexCoord.x + 0.02 &&  gvPos.x > input.TexCoord.x-0.02)
	{
		if(gvPos.y < input.TexCoord.y + 0.02 &&  gvPos.y > input.TexCoord.y-0.02)
		{
			//fullColor += float4(0.2,0,0,1);
			float ds = distance(input.TexCoord, gvPos);
			if(ds < 0.02 && ds > 0.018)
			{
				fullColor += float4(0.1,0.1,0.1,0.1);
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

	float diffuseLightingFactor = DotProduct(xLightPos, input.Position3D, input.Normal);
	diffuseLightingFactor = saturate(diffuseLightingFactor);
	diffuseLightingFactor *= xLightPower;
	return (fullColor*(diffuseLightingFactor+xAmbient));

    float2 ProjectedTexCoords;
    ProjectedTexCoords[0] = input.Pos2DAsSeenByLight.x/input.Pos2DAsSeenByLight.w/2.0f +0.5f;
    ProjectedTexCoords[1] = -input.Pos2DAsSeenByLight.y/input.Pos2DAsSeenByLight.w/2.0f +0.5f;
    
    diffuseLightingFactor = 0;
    if ((saturate(ProjectedTexCoords).x == ProjectedTexCoords.x) && (saturate(ProjectedTexCoords).y == ProjectedTexCoords.y))
    {
        float depthStoredInShadowMap = tex2D(ShadowMapSampler, ProjectedTexCoords).r;
        float realDistance = input.Pos2DAsSeenByLight.z/input.Pos2DAsSeenByLight.w;
        if ((realDistance - 1.0f/100.0f) <= depthStoredInShadowMap)
        {
            diffuseLightingFactor = DotProduct(xLightPos, input.Position3D, input.Normal);
            diffuseLightingFactor = saturate(diffuseLightingFactor);
            diffuseLightingFactor *= xLightPower;            
        }
    }
        
    float4 baseColor = tex2D(ColorTextureSampler, input.TexCoord);                
    fullColor = baseColor*(diffuseLightingFactor + xAmbient);

    //return fullColor;
}

technique StaticObject
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
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


SSceneVertexToPixel ShadowedSceneVertexShader( float4 inPos : POSITION)
{
    SSceneVertexToPixel Output = (SSceneVertexToPixel)0;

    Output.Position = mul(inPos, xWorldViewProjection);    
    Output.Pos2DAsSeenByLight= mul(inPos, xLightsWorldViewProjection);    
    return Output;
}


SScenePixelToFrame ShadowedScenePixelShader(SSceneVertexToPixel PSIn)
{
    SScenePixelToFrame Output = (SScenePixelToFrame)0;    

    float2 ProjectedTexCoords;
    ProjectedTexCoords[0] = PSIn.Pos2DAsSeenByLight.x/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;
    ProjectedTexCoords[1] = -PSIn.Pos2DAsSeenByLight.y/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;

    Output.Color = tex2D(ColorTextureSampler, ProjectedTexCoords);

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