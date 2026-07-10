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
        public bool attackIntent;

        // 적중 판정 Ray를 쏠 기준
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private float maxRange = 100f;
        [SerializeField] private LayerMask hitLayer = ~0; // 기본적으로 모든 레이어와 충돌

        private static readonly int FireTriggerHash = Animator.StringToHash("FireTrigger");
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (attackIntent)
            {
                var inventory = GetComponentInParent<IInventory>();
                if (inventory != null && inventory.HasItem(ItemType.Ammo))
                {
                    inventory.ModifyItem(ItemType.Ammo, -1);
                    animator.SetTrigger(FireTriggerHash);
                }
                else Debug.Log("You need to reload");
            }

            attackIntent = false;
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
                    if (bulletObj.TryGetComponent<Bullet>(out var bullet))
                    {
                        bullet.Init(cameraRoot.forward, damage, hitLayer, hitImpactPrefab);
                    }
                }
            }
        #endregion
        }
    }
}