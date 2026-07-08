using UnityEngine;

namespace MyFps
{
    public class AutoDestroy : MonoBehaviour
    {
        [Tooltip("수동으로 지정할 파괴 지연 시간 (0이면 파티클 길이를 자동으로 계산합니다)")]
        [SerializeField] private float _customDelay = 0f;
        [SerializeField] private float _fallbackDelay = 2f;
        private void Start()
        {
            float finalDelay = _customDelay;

            // 1. 수동 지정 시간이 없고 파티클 시스템이 부착되어 있다면, 자동으로 최적의 수명을 계산합니다.
            if (finalDelay <= 0f && TryGetComponent(out ParticleSystem ps))
            {
                finalDelay = ps.main.duration + ps.main.startLifetime.constantMax;
            }

            // 2. 파티클도 없고 수동 시간도 지정하지 않았다면 예외 방지를 위한 기본 안전장치(2초) 적용
            if (finalDelay <= 0f)
            {
                finalDelay = _fallbackDelay;
            }

            // 3. 최종 결정된 시간에 파괴 요청
            Destroy(gameObject, finalDelay);
        }
    }
}
