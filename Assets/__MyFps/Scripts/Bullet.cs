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
        private Transform shooterRoot; // 발사자 정보 저장
        #endregion

        #region Unity Event Method

        private void OnTriggerEnter(Collider other)
        {
            ProcessHit(other, transform.position, -transform.forward);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Vector3 hitPoint = transform.position;
            Vector3 hitNormal = -transform.forward;
            // 핵심 수정: 접촉점 데이터가 비어있지 않을 때만 안전하게 가져옴 (에러 원천 차단)
            if (collision.contactCount > 0)
            {
                hitPoint = collision.contacts[0].point;
                hitNormal = collision.contacts[0].normal;
            }
            ProcessHit(collision.collider, hitPoint, hitNormal);
        }
        #endregion

        #region Custom Method
        // Pistol.cs에서 총알을 생성할 때 호출하여 초기값을 전달받습니다.
        public void Init(Vector3 direction, float damage, LayerMask hitLayer, GameObject hitImpactPrefab, Transform shooterRoot)
        {
            this.damage = damage;
            this.hitLayer = hitLayer;
            this.hitImpactPrefab = hitImpactPrefab;
            this.shooterRoot = shooterRoot;

            rb = GetComponent<Rigidbody>();

            // 총알을 앞으로 강하게 날려보냅니다.
            rb.AddForce(direction * speed, ForceMode.VelocityChange);

            // 메모리 누수 방지: 3초 뒤에 맞지 않더라도 자동 파괴
            Destroy(gameObject, 3f);
        }

        private void ProcessHit(Collider hitCollider, Vector3 hitPoint, Vector3 hitNormal)
        {
            // 내 몸(플레이어)이랑 부딪혔다면 자해 방지 (무시)
            if (shooterRoot != null && hitCollider.transform.root == shooterRoot)
            {
                return;
            }
            // 부딪힌 대상이 우리가 맞추길 원하는 Layer인지 확인
            if (((1 << hitCollider.gameObject.layer) & hitLayer) != 0)
            {
                // 피격 이펙트 생성
                if (hitImpactPrefab != null)
                {
                    Instantiate(hitImpactPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
                }
                // 데미지 전달
                IDamageable target = hitCollider.GetComponentInParent<IDamageable>();
                target?.TakeDamage(damage);
                // 총알 파괴
                Destroy(gameObject);
            }
        }
        #endregion




    }
}