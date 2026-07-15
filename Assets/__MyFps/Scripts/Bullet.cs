using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float speed = 50f;

        private float damage;
        private LayerMask hitLayer;
        private GameObject hitImpactPrefab;
        private Rigidbody rb;

        private Vector3 previousPosition; // 레이캐스트 궤적 추적용 (이전 위치)
        private bool isHit = false;       // 중복 충돌 방지 플래그
        #endregion

        #region Unity Event Method
        /*
         * [학습용 노트: Predictive Raycast (궤적 추적) 충돌 판정]
         * 
         * 유니티의 기본 물리 엔진(OnCollisionEnter, OnTriggerEnter)은 총알처럼 
         * 1프레임당 이동 거리가 매우 긴(빠른) 오브젝트의 충돌을 놓치는 '터널링(Tunneling)' 현상이 발생하기 쉽습니다.
         * 
         * 이를 해결하기 위해 AAA FPS 게임에서 주로 사용하는 'Predictive Raycast' 방식을 적용했습니다.
         * 
         * 핵심 원리:
         * 1. 매 프레임 총알이 '이동하기 직전 위치'에서 '현재 위치'까지 가상의 레이저(Raycast)를 쏩니다.
         * 2. 그 선분 사이에 맞은 물체가 있다면, 수학적으로 완벽한 충돌 지점(hit.point)을 찾아냅니다.
         * 
         * 최적화 이점:
         * 순전히 수학적 레이캐스트로 충돌을 계산하므로, 총알 프리팹 자체에 
         * 물리 컴포넌트인 'Collider(Sphere Collider 등)'를 부착할 필요가 아예 없어집니다!
         * 덕분에 물리 엔진이 씬 내에 수없이 쏟아지는 총알 콜라이더를 연산할 필요가 없어 성능 최적화에 큰 도움이 됩니다.
         */
        private void FixedUpdate()
        {
            if (isHit) return;

            Vector3 currentPosition = transform.position;
            Vector3 direction = currentPosition - previousPosition;
            float distance = direction.magnitude;

            // 이동한 거리가 있을 때만 레이캐스트로 궤적 검사
            if (distance > 0)
            {
                // 이전 프레임 위치에서 현재 프레임 위치로 선을 그어, 그 사이를 지나쳤는지 확인합니다.
                // QueryTriggerInteraction.Ignore를 사용해 isTrigger가 켜진 대상은 자동으로 무시합니다.
                if (Physics.Raycast(previousPosition, direction.normalized, out RaycastHit hitInfo, distance, hitLayer, QueryTriggerInteraction.Ignore))
                {
                    ProcessHit(hitInfo.collider, hitInfo.point, hitInfo.normal);
                }
            }

            // 다음 프레임 비교를 위해 현재 위치 갱신
            previousPosition = currentPosition;
        }
        #endregion

        #region Custom Method
        public void Init(Vector3 direction, float damage, LayerMask hitLayer, GameObject hitImpactPrefab)
        {
            this.damage = damage;
            this.hitLayer = hitLayer;
            this.hitImpactPrefab = hitImpactPrefab;

            rb = GetComponent<Rigidbody>();

            // 총알을 앞으로 강하게 날려보냅니다.
            rb.AddForce(direction * speed, ForceMode.VelocityChange);

            // 초기 궤적 검사용 위치 저장
            previousPosition = transform.position;

            Destroy(gameObject, 3f);
        }

        private void ProcessHit(Collider hitCollider, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (isHit) return;
            isHit = true; // 이펙트가 여러 개 터지는 것을 방지

            if (hitImpactPrefab != null)
            {
                // 표면 파묻힘 방지용 오프셋은 Z-fighting(텍스처 겹침)을 막기 위해 필수적입니다.
                Vector3 spawnPoint = hitPoint + (hitNormal * 0.05f);
                Instantiate(hitImpactPrefab, spawnPoint, Quaternion.LookRotation(hitNormal));
            }

            IDamageable target = hitCollider.GetComponentInParent<IDamageable>();
            target?.TakeDamage(damage, transform.forward);

            Destroy(gameObject);
        }
        #endregion
    }
}