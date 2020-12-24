Shader "WWFramework/FootPrintGenerator"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass //Trace Move
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            half _WorldSize;
            half _FootPrintAtten;
            float3 _DeltaWorldPosition;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 deltaPos = _DeltaWorldPosition;
                float2 deltaUV = deltaPos.xz / _WorldSize;
                float2 lastUV = i.uv - deltaUV;
                // 上一步的信息
                half4 lastPrintTrace = tex2D(_MainTex, lastUV);
                // 脚印变浅
                lastPrintTrace.a -= _FootPrintAtten;

                // 边缘计算
                half2 lastFootPrints = step(0.001, i.uv) * step(i.uv, 0.999);
                lastPrintTrace *= lastFootPrints.x * lastFootPrints.y;

                return lastPrintTrace;
            }
            ENDCG
        }

        Pass //Trace Generation
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _FootPrintBump;
            half _IsLeftFoot;

            half _FootPrintSize;
            half _WorldSize;
            float3 _DeltaFootPosition;
            // half _YDegress;
            float4x4 _RotationFootPrint;
            // float3 _WorldPosition;
            // float3 _DeltaWorldPosition;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2 RotateAroundYInDegrees(float2 uv, float degrees)
            {
                float alpha = degrees * UNITY_PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);

                return mul(m, uv);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 lastUV = i.uv;
                // 上一步的信息
                half4 lastPrintTrace = tex2D(_MainTex, lastUV);

                // 算出该点在脚印上的uv
                float2 uv = (i.uv - 0.5 - _DeltaFootPosition.xz / _WorldSize) * _WorldSize / _FootPrintSize + 0.5;

                // 根据脚朝向旋转
                uv = uv - 0.5;
                // uv = RotateAroundYInDegrees(uv, _YDegress);
                uv = mul((float2x2)_RotationFootPrint, uv);
                uv = uv + 0.5;

                // 左右脚互换
                uv.x = lerp(1 - uv.x, uv.x, _IsLeftFoot);

                // 暂不考虑朝向
                half4 footPrint = tex2D(_FootPrintBump, uv);
                // 反向脚印的法线，即将xy反向，可以推导出来
                // footPrint.rg = 1 - footPrint.rg;
                // // 左右脚互换
                // footPrint.r = footPrint.r * _IsLeftFoot + (1 - footPrint.r) * (1 - _IsLeftFoot);
                half2 footUVCheck = step(0.001, uv) * step(uv, 0.999);
                footPrint *= footUVCheck.x * footUVCheck.y;

                half4 col = footPrint;
                // 深的位置决定最后的效果
                half stepDeep = step(lastPrintTrace.a, col.a);
                col.a = max(lastPrintTrace.a, col.a);
                col.rgb = lerp(lastPrintTrace.rgb, col.rgb, stepDeep);

                // // 边缘检测
                // half2 uvCheck = step(0.01, i.uv) * step(i.uv, 0.99);
                // col.a *= uvCheck.x * uvCheck.y;

                return col;
            }
            ENDCG
        }

        Pass //Init
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return 0;
            }
            ENDCG
        }
    }
}