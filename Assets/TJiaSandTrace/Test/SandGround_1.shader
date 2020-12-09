Shader "TJia/SandGround_1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpTex ("Bump", 2D) = "bump" {}
        _BumpScale ("BumpScale", Range(0,5)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "LightMode" = "ForwardBase"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityPBSLighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 tangent : TEXCOORD1;
                float3 bitangent : TEXCOORD2;
                float3 normal : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
            };

            sampler2D _MainTex;
            sampler2D _BumpTex;
            float4 _MainTex_ST;
            float _BumpScale;

            half _FootPrintBumpScale;
            sampler2D _FootPrintTrace;
            half _WorldSize;
            float3 _WorldPosition;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.bitangent = normalize(cross(o.normal, o.tangent) * v.tangent.w * unity_WorldTransformParams.w);
                o.worldPos = mul(UNITY_MATRIX_M, v.vertex);
                return o;
            }

            fixed3 UnpackNormalmapNew(fixed4 packednormal)
            {
                fixed3 normal;
                normal.xy = packednormal.xy * 2 - 1;
                normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));
                return normal;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float3x3 tangentToWorld = float3x3(normalize(i.tangent), normalize(i.bitangent), normalize(i.normal));

                float3 normalLocal = UnpackNormal(tex2D(_BumpTex, i.uv));
                float4 stepNormalCol = tex2D(_FootPrintTrace, (i.worldPos.xz - _WorldPosition.xz) / _WorldSize + 0.5);
                // return stepNormalCol;
                float3 stepNormal = stepNormalCol.rgb * 2 - 1;
                // float3 stepNormal = UnpackNormalmapNew(stepNormalCol);
                stepNormal.xy *= _FootPrintBumpScale;

                normalLocal = lerp(normalLocal, stepNormal, stepNormalCol.a);

                normalLocal.xy *= _BumpScale;

                float3 normal = mul(normalize(normalLocal), tangentToWorld);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                float nl = dot(normal, lightDir);

                float3 diff = pow(saturate(nl), 2.2);

                diff = lerp(lerp(float3(0.25, 0.4, 1), float3(0.7, 0.3, 0.05), diff.r), 1, diff.r);

                col.rgb = diff * _LightColor0.rgb * col.rgb;

                float dis = max(distance(i.worldPos.xyz, _WorldSpaceCameraPos.xyz) - 20, 0) / 2000;
                float fogDensity = saturate(1 - exp(-pow(20 * dis, 2)));

                col.rgb = lerp(col.rgb, float3(1, 0.6, 0.1), fogDensity);

                //col.rgb = stepNormalCol.a;

                return col;
            }
            ENDCG
        }
    }
    Fallback "VertexLit"
}