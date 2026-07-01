using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class Pistol : MonoBehaviour
    {

        #region Variables
        [SerializeField] private float damage = 5;
        private AudioSource audioSource;
        [SerializeField] private AudioClip fireSound;
        [SerializeField] private ParticleSystem muzzleFlash;
        private Animator animator;
        private CharacterInput input;

        // 적중 판정 Ray를 쏠 기준
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private float maxRange = 100f;
        [SerializeField] private LayerMask hitLayer = ~0; // 기본적으로 모든 레이어와 충돌

        private PlayerStats stats;

        private static readonly int FireTriggerHash = Animator.StringToHash("FireTrigger");
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            input = GetComponentInParent<CharacterInput>();
            stats = GetComponentInParent<PlayerStats>();
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
                if (stats.AmmoCount > 0)
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
            if (fireSound != null) audioSource.PlayOneShot(fireSound);
            if (muzzleFlash != null) muzzleFlash.Play();

            if (Physics.Raycast(cameraRoot.position, cameraRoot.forward, out RaycastHit hitInfo, maxRange, hitLayer, QueryTriggerInteraction.Ignore))
            {
                Collider hitCollider = hitInfo.collider;
                if (hitCollider != null)
                {
                    IDamageable target = hitCollider.GetComponentInParent<IDamageable>();
                    target?.TakeDamage(damage);
                }
            }
        #endregion



        }
    }
}