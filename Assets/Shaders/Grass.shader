Shader "Unlit/Grass"
{
    Properties
    {
        _GrassMap ("Grass Visibility Map", 2D) = "white" { }
        _GrassThreshold ("Grass Threshold", Range(-0.1, 1)) = 0.5
        _GrassFalloff ("Grass Visibility Fade-In Falloff", Range(0, 0.5)) = 0.05


        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _TipColor ("Tip Color", Color) = (1, 1, 1, 1)
        [NoScaleOffset]_BladeTexture ("Blade Texture", 2D) = "white" { }

        _BendDelta ("Bend Delta", Range(0, 1)) = 0.2

        _BladeWidthMin ("Blade Width (Min)", Range(0, 0.1)) = 0.02
        _BladeWidthMax ("Blade Width (Max)", Range(0, 0.1)) = 0.05
        _BladeHeightMin ("Blade Height (Min)", Range(0, 2)) = 0.1
        _BladeHeightMax ("Blade Height (Max)", Range(0, 2)) = 0.2

        _BladeBendDistance ("Blade Forward Amount", Float) = 0.38
        _BladeBendCurve ("Blade Curvature Amount", Range(1, 4)) = 2
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalRenderPipeline" }
        LOD 100
        Cull Off
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        
        #define UNITY_PI 3.14159265359f
        #define UNITY_TWO_PI 6.28318530718f

        #define BLADE_SEGMENTS 4

        CBUFFER_START(UnityPerMaterial)
            sampler2D _GrassMap;
            float4 _GrassMap_ST;
            float _GrassThreshold;
            float _GrassFalloff;

            float4 _BaseColor;
            float4 _TipColor;
            sampler2D _BladeTexture;

            float _BendDelta;

            float _BladeWidthMin;
            float _BladeWidthMax;
            float _BladeHeightMin;
            float _BladeHeightMax;

            float _BladeBendDistance;
            float _BladeBendCurve;

        CBUFFER_END


        struct VertexInput
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 tangent : TANGENT;
            float2 uv : TEXCOORD0;
        };

        struct Interpolator
        {
            float4 vertex : SV_POSITION;
            float3 normal : NORMAL;
            float4 tangent : TANGENT;
            float2 uv : TEXCOORD0;
        };

        struct GeomData
        {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 worldPos : TEXCOORD1;
        };

        // Following functions from Roystan's code:
        // (https://github.com/IronWarrior/UnityGrassGeometryShader)

        // Simple noise function, sourced from http://answers.unity.com/answers/624136/view.html
        // Extended discussion on this function can be found at the following link:
        // https://forum.unity.com/threads/am-i-over-complicating-this-random-function.454887/#post-2949326
        // Returns a number in the 0...1 range.

        float rand(float3 co)
        {
            return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
        }

        // Construct a rotation matrix that rotates around the provided axis, sourced from:
        // https://gist.github.com/keijiro/ee439d5e7388f3aafc5296005c8c3f33
        float3x3 angleAxis3x3(float angle, float3 axis)
        {
            float c, s;
            sincos(angle, s, c);

            float t = 1 - c;
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;

            return float3x3
            (
                t * x * x + c, t * x * y - s * z, t * x * z + s * y,
                t * x * y + s * z, t * y * y + c, t * y * z - s * x,
                t * x * z - s * y, t * y * z + s * x, t * z * z + c
            );
        }


        Interpolator vert(VertexInput v)
        {
            Interpolator o;
            o.vertex = TransformObjectToHClip(v.vertex.xyz);
            o.normal = v.normal;
            o.tangent = v.tangent;
            o.uv = TRANSFORM_TEX(v.uv, _GrassMap);
            return o;
        }


        Interpolator geomVert(VertexInput v)
        {
            Interpolator o;
            o.vertex = float4(TransformObjectToWorld(v.vertex.xyz), 1.0f);
            o.normal = TransformObjectToWorldNormal(v.normal);
            o.tangent = v.tangent;
            o.uv = TRANSFORM_TEX(v.uv, _GrassMap);
            return o;
        }

        

        ENDHLSL
        

        Pass
        {
            Name "GrassPass"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex geomVert
            #pragma fragment frag
            #pragma require geometry
            #pragma geometry geom

            GeomData TransformGeomToClip(float3 pos, float3 offset, float3x3 transformationMatrix, float2 uv)
            {
                GeomData o;

                o.pos = TransformObjectToHClip(pos + mul(transformationMatrix, offset));
                o.uv = uv;
                o.worldPos = TransformObjectToWorld(pos + mul(transformationMatrix, offset));

                return o;
            }

            [maxvertexcount(BLADE_SEGMENTS * 2 + 1)]
            void geom(point Interpolator input[1], inout TriangleStream<GeomData> triStream)
            {
                float grassVisibility = tex2Dlod(_GrassMap, float4(input[0].uv, 0, 0)).r;

                if (grassVisibility >= _GrassThreshold)
                {

                    float3 pos = input[0].vertex.xyz;
                    float3 normal = input[0].normal;
                    float4 tangent = input[0].tangent;

                    float3 bitangent = cross(normal, tangent.xyz) * tangent.w;

                    float3x3 tangentToLocal = float3x3(
                        tangent.x, bitangent.x, normal.x,
                        tangent.y, bitangent.y, normal.y,
                        tangent.z, bitangent.z, normal.z
                    );

                    float3x3 randRotMatrix = angleAxis3x3(rand(pos) * UNITY_TWO_PI, float3(0, 0., 1.0f));
                    float3x3 randBendMatrix = angleAxis3x3((rand(pos.zzx) - 0.5) * _BendDelta * UNITY_PI * 0.5, float3(-1.0f, 0, 0));

                    float3x3 baseTransformationMatrix = mul(tangentToLocal, randRotMatrix);
                    float3x3 tipTransformationMatrix = mul(mul(tangentToLocal, randBendMatrix), randRotMatrix);
                    
                    float falloff = smoothstep(_GrassThreshold, _GrassThreshold + _GrassFalloff, grassVisibility);

                    float width = lerp(_BladeWidthMin, _BladeWidthMax, rand(pos.xzy) * falloff);
                    float height = lerp(_BladeHeightMin, _BladeHeightMax, rand(pos.zxy) * falloff);
                    float forward = rand(pos.yyz) * _BladeBendDistance;

                    for (int i = 0; i < BLADE_SEGMENTS; i++)
                    {
                        float t = i / (float) BLADE_SEGMENTS;

                        float3 offset = float3(width * (1 - t), pow(t, _BladeBendCurve) * forward, height * t);

                        float3x3 transformationMatrix = (i == 0) ? baseTransformationMatrix : tipTransformationMatrix;

                        triStream.Append(TransformGeomToClip(pos, float3(offset.x, offset.y, offset.z), transformationMatrix, float2(0, t)));
                        triStream.Append(TransformGeomToClip(pos, float3(-offset.x, offset.y, offset.z), transformationMatrix, float2(1, t)));
                    }

                    triStream.Append(TransformGeomToClip(pos, float3(0, forward, height), tipTransformationMatrix, float2(0.5, 1)));

                    triStream.RestartStrip();
                }
            }

            float4 frag(GeomData i) : SV_TARGET
            {
                float4 color = tex2D(_BladeTexture, i.uv);
                return color * lerp(_BaseColor, _TipColor, i.uv.y);
            }


            ENDHLSL
        }
    }
}