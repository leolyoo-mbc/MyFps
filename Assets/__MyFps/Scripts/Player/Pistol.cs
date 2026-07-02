using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(Animator))]
    public class Pistol : MonoBehaviour
    {

        #region Variables
        [Tooltip("0은 레이, 1은 총알 발사")]
        public int shootMode = 0;
        [SerializeField] private GameObject bulletPrefab;

        [SerializeField] private float damage = 5;
        [SerializeField] private AudioSource fireSound;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private GameObject hitImpactPrefab;
        private Animator animator;
        private CharacterInput input;

        // 적중 판정 Ray를 쏠 기준
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private float maxRange = 100f;
        [SerializeField] private LayerMask hitLayer = ~0; // 기본적으로 모든 레이어와 충돌

        [SerializeField] private PlayerStatsData stats;

        private static readonly int FireTriggerHash = Animator.StringToHash("FireTrigger");
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            animator = GetComponent<Animator>();
            input = GetComponentInParent<CharacterInput>();
        }

        private void OnEnable()
        {
            // 활성화 이전의 입력 제거
            input.isAttacking = false;
        }

        private void Update()
        {
            if (input.isAttacking)
            {
                if (stats != null && stats.AmmoCount > 0)
                {
                    stats.AmmoCount--;
                    animator.SetTrigger(FireTriggerHash);
                }
                else Debug.Log("You need to reload");
            }

            input.isAttacking = false;
        }
        #endregion

        #region Custom Method
        public void OnFire()
        {
            if (fireSound != null) fireSound.Play();
            if (muzzleFlash != null) muzzleFlash.Play();

            if (shootMode == 0)
            {
                if (Physics.Raycast(cameraRoot.position, cameraRoot.forward, out RaycastHit hitInfo, maxRange, hitLayer, QueryTriggerInteraction.Ignore))
                {
                    // 총알이 맞은 위치(hitInfo.point)에 피격 이펙트 생성
                    // hitInfo.normal(맞은 표면의 수직 방향)을 바라보게 회전시킵니다.
                    if (hitImpactPrefab != null)
                    {
                        Instantiate(hitImpactPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    }

                    Collider hitCollider = hitInfo.collider;
                    if (hitCollider != null)
                    {
                        IDamageable target = hitCollider.GetComponentInParent<IDamageable>();
                        target?.TakeDamage(damage);
                    }
                }
            }
            else if (shootMode == 1)
            {
                // 총알 발사 (물리 탄환)
                if (bulletPrefab != null)
                {
                    Vector3 spawnPos = cameraRoot.position + cameraRoot.forward * 0.5f;
                    GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, cameraRoot.rotation);
                    Bullet bullet = bulletObj.GetComponent<Bullet>();
                    
                    if (bullet != null)
                    {
                        // 총알 초기화 시 내 몸(플레이어의 최상위 객체) 정보도 같이 넘겨줍니다.
                        // transform.root 대신 input.transform을 사용하여 씬 구조에 의한 버그를 방지합니다.
                        bullet.Init(cameraRoot.forward, damage, hitLayer, hitImpactPrefab, input.transform);

                        // 총알에 달린 '모든' 콜라이더와 플레이어의 '모든' 콜라이더 간의 충돌을 무시합니다.
                        Collider[] bulletColliders = bulletObj.GetComponentsInChildren<Collider>();
                        Collider[] playerColliders = input.transform.GetComponentsInChildren<Collider>();
                        
                        foreach (Collider bCol in bulletColliders)
                        {
                            foreach (Collider pCol in playerColliders)
                            {
                                Physics.IgnoreCollision(bCol, pCol);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Bullet 프리팹에 Bullet 스크립트가 없습니다!");
                    }
                }
            }
        #endregion
        }
    }
}