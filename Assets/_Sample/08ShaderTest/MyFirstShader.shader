// Shader (셰이더): 3D 물체의 색깔이나 질감을 어떻게 그릴지 그래픽 카드에게 알려주는 '계산서/설명서'입니다.
// "Custom/MyFirstShader": 유니티 에디터에서 물감(머티리얼)을 만들 때 고르는 경로와 이름입니다.
Shader "Custom/MyFirstShader"
{
    // Properties (프로퍼티즈): 유니티 화면 우측의 '인스펙터' 창에 띄워줄 설정값들을 적는 곳입니다.
    // 여기서 변수를 만들면 코드를 수정하지 않고도 유니티 화면에서 마우스로 색상이나 이미지를 바꿀 수 있습니다.
    Properties
    {
        // _BaseColor: 이 셰이더 코드 안에서 쓸 변수(이름)입니다.
        // "Base Color": 유니티 에디터 화면(인스펙터)에 보여질 글자입니다.
        // Color: 이 변수가 '색상'을 담는 상자라는 뜻입니다. (빨강, 초록, 파랑, 투명도를 0~1로 가짐)
        // (1, 1, 1, 1): 유니티에서 아무 설정도 안 했을 때의 기본값입니다. (흰색, 불투명)
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        
        // _BaseMap: 3D 물체 표면에 씌울 포장지(이미지)를 담을 변수입니다.
        // 2D: 일반적인 평면 이미지 파일이라는 뜻입니다.
        // "white" {}: 아무 이미지도 안 넣으면 기본적으로 하얀색 이미지를 씌운다는 뜻입니다.
        _BaseMap("Base Map", 2D) = "white" {}
    }

    // SubShader (서브 셰이더): 실제 그림을 그리는 핵심 코드들이 들어가는 곳입니다.
    // 기기 성능(PC, 모바일 등)에 따라 여러 개를 만들 수 있는데, 제일 위에 있는 것부터 실행해 봅니다.
    SubShader
    {
        // Tags (태그): 유니티 엔진에게 이 셰이더가 어떤 특징을 가졌는지 힌트를 주는 곳입니다.
        // "RenderType" = "Opaque": Opaque는 '불투명하다'는 뜻입니다. 반투명(유리창 등)이 아니라고 엔진에 알려줍니다.
        // "RenderPipeline" = "UniversalPipeline": 유니티의 최신 그리기 방식인 URP에서만 작동한다는 뜻입니다.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        // Pass (패스): 화면에 물체를 그리는 한 번의 '작업 과정'입니다. 한 번의 붓칠이라고 생각하면 됩니다.
        Pass
        {
            // 여기서부터는 HLSL이라는 진짜 셰이더 프로그래밍 언어를 쓰겠다고 선언하는 겁니다.
            HLSLPROGRAM

            // #pragma(프래그마): 컴퓨터에게 내리는 특별한 명령입니다.
            // vertex vert: 3D 모델의 뼈대(점들)를 다루는 함수 이름이 'vert'라고 컴퓨터에 알려줍니다.
            // fragment frag: 뼈대에 살(색상 픽셀)을 입히는 함수 이름이 'frag'라고 알려줍니다.
            #pragma vertex vert
            #pragma fragment frag

            // #include: 유니티가 미리 만들어둔 유용한 수학 공식이나 규칙들이 담긴 파일을 가져와서 씁니다.
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // --- 1단계: 유니티에서 데이터 받아오기 ---

            // Attributes (어트리뷰트): 유니티 엔진이 셰이더에게 처음 넘겨주는 3D 모델 원본의 데이터 꾸러미입니다.
            struct Attributes
            {
                // float4: 소수점 숫자 4개가 한 세트인 데이터입니다. (x, y, z, w 좌표)
                // positionOS: Object Space(오브젝트 공간)의 약자로, 게임 세상 전체 중심이 아닌 이 3D 물체 자체의 중심을 기준으로 한 점의 위치입니다.
                // : POSITION - 콜론(:) 뒤에 붙은 단어를 '시맨틱(이름표)'이라고 합니다. 그래픽 카드에게 "이 데이터는 위치야!"라고 알려주는 필수 이름표입니다.
                float4 positionOS : POSITION;

                // float2: 소수점 숫자 2개가 한 세트입니다.
                // uv: 3D 물체에 포장지(이미지)를 씌울 때 사용하는 가로/세로 좌표입니다. (0~1 사이의 값)
                // : TEXCOORD0 - "이건 첫 번째 텍스처(이미지) 좌표야!"라는 이름표입니다.
                float2 uv : TEXCOORD0;
            };

            // --- 2단계: 다음 단계로 데이터 넘겨주기 ---

            // Varyings (베링스): 점들의 위치(버텍스) 계산을 끝내고, 색상을 칠하는(프래그먼트) 단계로 넘겨줄 데이터 꾸러미입니다.
            // 그래픽 카드는 이 데이터를 넘기는 과정에서 점과 점 사이의 빈 공간을 부드럽게 채워주는데, 이 과정을 '보간(Interpolation)'이라고 합니다.
            struct Varyings
            {
                // positionHCS: 3D 입체 공간에 있던 점을 모니터(2D 평면 화면)에 납작하게 그리기 위해 계산된 '화면 전용 좌표(Clip Space)'입니다.
                // : SV_POSITION - 화면 어디에 점을 찍을지 그래픽 카드가 무조건 알아야 하므로 반드시 붙여야 하는 '시스템 필수 이름표'입니다.
                float4 positionHCS : SV_POSITION;

                // 다음 단계로 넘겨줄 이미지 좌표(UV)입니다.
                float2 uv : TEXCOORD0;
            };

            // --- 3단계: 사용할 이미지와 색상 준비하기 ---

            // TEXTURE2D: 인스펙터에서 넣은 이미지 파일(Base Map)을 가져옵니다.
            // SAMPLER (샘플러): 가져온 이미지를 화면에 그릴 때, 어떻게 픽셀을 부드럽게 섞어서 읽어올지 결정하는 돋보기 같은 도구입니다.
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            // CBUFFER_START / END: 같은 머티리얼(물감)을 쓰는 물체들을 모아서 한 번에 아주 빠르게 그리기 위한 마법의 상자(SRP Batcher)입니다.
            // Properties에서 만들었던 변수들을 이 상자 안에도 똑같이 적어줘야 유니티 화면과 코드가 서로 연결됩니다.
            CBUFFER_START(UnityPerMaterial)
                // half4: float4와 비슷하지만, 계산의 정밀도를 절반(16비트)으로 낮춰서 컴퓨터가 더 빨리 계산하게 만든 타입입니다. 주로 색깔을 저장할 때 씁니다.
                half4 _BaseColor;

                // _BaseMap_ST: _ST는 Scale(크기)과 Translate(이동)의 약자입니다.
                // 유니티 인스펙터에서 이미지의 크기나 위치를 조절할 수 있는데, 유니티가 그 조절 값을 이 변수에 자동으로 넣어줍니다.
                float4 _BaseMap_ST;
            CBUFFER_END

            // --- 4단계: 버텍스 셰이더 (뼈대 위치 계산하기) ---

            // vert(버텍스 함수): 3D 모델을 이루는 '점(Vertex)'의 개수만큼 반복해서 실행되는 함수입니다.
            Varyings vert(Attributes IN)
            {
                // 다음 단계(프래그먼트)로 보낼 빈 꾸러미(OUT)를 만듭니다.
                Varyings OUT;
                
                // TransformObjectToHClip(): 3D 물체 자기 자신의 좌표(positionOS)를 가져와서, 우리가 보는 모니터 화면의 납작한 2D 좌표(positionHCS)로 변환해 주는 아주 유용한 내장 공식입니다.
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                // TRANSFORM_TEX(): 이미지가 물체에 씌워질 좌표(UV)에, 인스펙터에서 설정한 크기/이동 값(_BaseMap_ST)을 수학적으로 곱하고 더해서 최종 이미지 좌표를 만들어줍니다.
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                
                // 계산이 끝난 꾸러미를 다음 단계로 넘깁니다.
                return OUT;
            }

            // --- 5단계: 프래그먼트 셰이더 (색칠하기) ---

            // frag(프래그먼트 함수): 화면에 그려질 아주 작은 네모난 점(픽셀/프래그먼트) 하나하나마다 실행되는 함수입니다.
            // : SV_Target - "여기서 계산한 결과값이 최종적으로 우리가 보는 화면(렌더 타겟)에 칠해질 색상이야!"라는 의미의 이름표입니다.
            half4 frag(Varyings IN) : SV_Target
            {
                // SAMPLE_TEXTURE2D(): 돋보기(샘플러)를 이용해 텍스처(이미지)의 특정 위치(UV 좌표)에 있는 픽셀 색깔을 하나 콕 찍어서 가져옵니다. 이 과정을 '샘플링(추출)'이라고 합니다.
                // 가져온 텍스처 색깔에 유니티에서 지정한 색상(_BaseColor)을 곱해서 최종적으로 화면에 나갈 섞인 색깔을 만듭니다.
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                
                // 최종 색깔을 반환하여 모니터 화면에 출력합니다.
                return color;
            }
            
            // HLSL 셰이더 프로그래밍 영역이 끝났다는 표시입니다.
            ENDHLSL
        }
    }
}
