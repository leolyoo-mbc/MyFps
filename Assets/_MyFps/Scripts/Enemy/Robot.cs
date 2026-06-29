using UnityEngine;

namespace MyFps
{
[RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    public class Robot : MonoBehaviour
    {
        #region Variables
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 0.5f;
        [SerializeField] private Transform targetPlayer;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip appearSound;

        private Animator animator;
        private CharacterController controller;
        private static readonly int EnemyStateHash = Animator.StringToHash("EnemyState");
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            
            if (audioSource == null) audioSource = GetComponent<AudioSource>();

            // 타겟이 할당 안 되어있으면 태그로 찾기
            if (targetPlayer == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) targetPlayer = player.transform;
            }
        }

        private void OnEnable()
        {
            // 활성화될 때 등장 사운드 재생
            if (audioSource != null && appearSound != null)
            {
                audioSource.PlayOneShot(appearSound);
            }
        }

        private void Update()
        {
            if (targetPlayer == null) return;

            // 플레이어 바라보기 (y축 고정)
            Vector3 direction = targetPlayer.position - transform.position;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // 이동
            Vector3 moveDirection = transform.forward * moveSpeed;
            
            if (controller != null)
            {
                // 중력 적용: 바닥으로 떨어지도록 강제
                moveDirection.y = -9.81f;
                controller.Move(moveDirection * Time.deltaTime);
            }
            else
            {
                transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
            }

            // 걷기 애니메이션 상태 (1: Walk)
            if (animator != null)
            {
                animator.SetInteger(EnemyStateHash, 1);
            }
        }
        #endregion
    }
}