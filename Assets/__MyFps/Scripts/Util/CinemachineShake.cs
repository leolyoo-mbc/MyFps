using Unity.Cinemachine;
using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 화면 흔들림 효과 구현 싱글톤 클래스
    /// </summary>
    public class CinemachineShake : Singleton<CinemachineShake>
    {
        #region Variables
        // 참조
        private CinemachineBasicMultiChannelPerlin multiChannelPerlin;

        // 흔들림 크기, 속도, 지속 시간 (기본값 설정용)
        [SerializeField] private float amplitudeGain = 1f;
        [SerializeField] private float frequencyGain = 1f;
        [SerializeField] private float duration = 1f;

        // 내부 타이머 및 시작 강도
        private float shakeTimer;
        private float shakeTimerTotal;
        private float startingAmplitude;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // 이 스크립트가 붙어있는 게임 오브젝트에서 노이즈 컴포넌트를 가져옵니다.
            multiChannelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
            
            if (multiChannelPerlin == null)
            {
                Debug.LogWarning("CinemachineBasicMultiChannelPerlin 컴포넌트를 찾을 수 없습니다! 카메라에 Noise를 추가해주세요.");
            }
            else
            {
                // 시작 시 카메라 흔들림이 없도록 0으로 초기화합니다.
                multiChannelPerlin.AmplitudeGain = 0f;
            }
        }

        private void Update()
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                
                // 시간이 지남에 따라 흔들림 강도를 서서히 줄입니다 (Decay)
                if (multiChannelPerlin != null)
                {
                    // 타이머의 진행도(0.0 ~ 1.0)에 따라 선형 보간하여 서서히 0이 되게 함
                    float lerpFactor = 1f - (shakeTimer / shakeTimerTotal);
                    multiChannelPerlin.AmplitudeGain = Mathf.Lerp(startingAmplitude, 0f, lerpFactor);
                }
            }
        }
        #endregion

        #region Custom Method
        /// <summary>
        /// 인스펙터에 설정된 기본값으로 카메라 흔들림을 발생시킵니다.
        /// </summary>
        public void ShakeCamera()
        {
            ShakeCamera(amplitudeGain, frequencyGain, duration);
        }

        /// <summary>
        /// 지정된 설정으로 카메라 흔들림을 발생시킵니다.
        /// </summary>
        /// <param name="amplitude">흔들림 강도 (진폭)</param>
        /// <param name="frequency">흔들림 속도 (주파수)</param>
        /// <param name="time">지속 시간 (초)</param>
        public void ShakeCamera(float amplitude, float frequency, float time)
        {
            if (multiChannelPerlin == null) return;

            // 노이즈 설정 적용
            multiChannelPerlin.AmplitudeGain = amplitude;
            multiChannelPerlin.FrequencyGain = frequency;
            
            // 서서히 흔들림을 줄이기 위해 초기값과 시간 저장
            startingAmplitude = amplitude;
            shakeTimerTotal = time;
            shakeTimer = time;
        }
        #endregion
    }
}