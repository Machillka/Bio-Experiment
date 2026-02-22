Shader "PCR/Liquid"
{
    Properties
    {
        _Color ("Liquid Color", Color) = (0,0,1,1)
        _SurfaceColor ("Top Surface Color", Color) = (0,0.5,1,1)
        _FillAmount ("Fill Amount (-0.5 to 0.5)", Range(-0.5, 0.5)) = 0.0
        _WobbleX ("Wobble X", Range(-1, 1)) = 0.0
        _WobbleZ ("Wobble Z", Range(-1, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        // 双面渲染，保证从上面能看到液面，从下面能看到杯底
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
            float3 objPos; // 我们需要自定义顶点数据传参
        };

        fixed4 _Color;
        fixed4 _SurfaceColor;
        float _FillAmount;
        float _WobbleX;
        float _WobbleZ;

        // 顶点修改函数：计算物体空间坐标
        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.objPos = v.vertex.xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 1. 核心逻辑：计算当前像素在物体空间的高度
            // 假设圆柱体模型的高度是 1米 (从 -0.5 到 0.5)
            // 我们加上晃动偏移
            float relativeHeight = IN.objPos.y;

            // 简单的平面方程模拟液面倾斜 (Wobble)
            float wobbleOffset = IN.objPos.x * _WobbleX + IN.objPos.z * _WobbleZ;

            // 2. 裁切逻辑 (Clip)
            // 如果当前像素高于液面，就不渲染 (Alpha = 0)
            // 实际上我们用 clip 函数，如果参数 < 0 则丢弃像素
            // 逻辑：液面高度(_FillAmount) - 当前高度(relativeHeight) - 晃动
            float level = _FillAmount - relativeHeight + wobbleOffset;

            // 为了让液面边缘平滑，不直接 clip，而是做 Alpha 过渡
            if(level < 0) discard;

            // 3. 表面判定
            // 如果非常接近液面边缘，视为“顶面”，渲染浅色
            float surfaceThickness = 0.02; // 顶面厚度
            if(level < surfaceThickness)
            {
                o.Albedo = _SurfaceColor.rgb;
                o.Alpha = 0.9;
                o.Smoothness = 0.2;
            }
            else
            {
                o.Albedo = _Color.rgb;
                o.Alpha = _Color.a;
                o.Smoothness = 0.8;
            }

            o.Metallic = 0.1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}