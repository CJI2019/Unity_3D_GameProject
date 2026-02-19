Shader "Custom/GPUSkinnedInstancing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AnimTex ("Animation Texture", 2D) = "white" {}
        [NoScaleOffset] _EmissionMap("Emission Map", 2D) = "white" {}
        [HDR] _EmissionColor("Emission Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing // 인스턴싱 변수 활성화

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint vid : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID // 인스턴스 ID 입력
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // 인스턴스 ID 출력
            };

            sampler2D _MainTex;
            TEXTURE2D(_AnimTex);
            SAMPLER(sampler_AnimTex);
            float4 _AnimTex_TexelSize;

            Texture2D _EmissionMap;
            SamplerState sampler_EmissionMap;
            float4 _EmissionColor; // HDR 색상 (Intensity 포함)

            // 인스턴스별 개별 데이터 정의
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _CurrentFrame)
            UNITY_INSTANCING_BUFFER_END(Props)

            CBUFFER_START(UnityPerMaterial)
                float _SharedScale; 
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                // float frame = fmod(_Time.y * 30.0f, _AnimTex_TexelSize.w);
                float frame = UNITY_ACCESS_INSTANCED_PROP(Props, _CurrentFrame);
                
                // 텍스처의 실제 해상도를 직접 가져옵니다
                float width, height;
                _AnimTex.GetDimensions(width, height);

                // 정점 ID가 텍스처 너비를 넘지 않는지 확인이 필요합니다.
                // 0.5를 더하는 이유는 픽셀의 정중앙을 샘플링하기 위함입니다.
                float u = (v.vid + 0.5) / width;
                float v_coord = (frame + 0.5) / height;

                // SampleLevel을 사용하여 LOD 0에서 직접 샘플링합니다.
                float4 bakedPos = _AnimTex.SampleLevel(sampler_AnimTex, float2(u, v_coord), 0);

                v.vertex.xyz = bakedPos.xyz * _SharedScale;

                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                // 기본 알베도 샘플링
                float4 col = tex2D(_MainTex, i.uv);
                
                // 이미시브 텍스처 샘플링
                float3 emission = _EmissionMap.Sample(sampler_EmissionMap, i.uv).rgb;
                
                // 색상 결합
                // _EmissionColor의 RGB 값에 1보다 큰 값을 넣으면 HDR 효과가 납니다.
                col.rgb += emission * _EmissionColor.rgb;
                
                return col;
            }
            ENDHLSL
        }
    }
}