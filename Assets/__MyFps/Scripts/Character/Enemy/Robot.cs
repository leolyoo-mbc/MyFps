using UnityEngine;
using UnityEngine.Events;

namespace MyFps
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(CharacterMove))]
    public class Robot : MonoBehaviour, IDamageable
    {

        public enum State { Idle = 0, Walking = 1, Attacking = 2, Dead = 3 }

        #region Variables
        [Header("Settings")]
        [SerializeField] private Transform targetPlayer;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackDamage = 5f;
        [SerializeField] private float attackInterval = 2f;
        [SerializeField] private float maxHealth = 20f;


        private Animator animator;
        private static readonly int EnemyStateHash = Animator.StringToHash("EnemyState");
        private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");

        private CharacterMove characterMove;
        private CharacterController characterController;
        private float attackCooldown;

        private float currentHealth;
        private bool isDead = false;

        [SerializeField] private UnityEvent onDeath;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            animator = GetComponent<Animator>();
            characterMove = GetComponent<CharacterMove>();
            characterController = GetComponent<CharacterController>();
            attackCooldown = attackInterval;
            currentHealth = maxHealth;
        }

        private void Update()
        {
            // 죽은 상태면 로직 중지 (CharacterMove에서 중력은 계속 처리함)
            if (isDead) return;

            // 공격 쿨다운 감소
            attackCooldown = attackCooldown > Time.deltaTime ? attackCooldown - Time.deltaTime : 0f;

            // 플레이어가 없으면 리턴
            if (targetPlayer == null) return;

            Vector3 direction = targetPlayer.position - transform.position;

            // 플레이어 바라보기 (y축 고정)
            if (direction.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

            // 플레이어가 공격 범위 안에 있는지 체크
            if (direction.sqrMagnitude <= (attackRange * attackRange))
            {
                characterMove.horizontalMoveIntent = Vector2.zero;
                animator.SetBool(IsAttackingHash, true);
                if (attackCooldown <= 0)
                {
                    // 공격
                    animator.SetInteger(EnemyStateHash, (int)State.Attacking);
                }
                else
                {
                    // 쿨타임 중: 공격 상태가 아니라면 대기 상태로 전환
                    animator.SetInteger(EnemyStateHash, (int)State.Idle);
                }
            }
            else
            {
                animator.SetBool(IsAttackingHash, false);
                // 이동
                characterMove.horizontalMoveIntent = Vector2.up;
                animator.SetInteger(EnemyStateHash, (int)State.Walking);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;//공격 범위는 빨간색 선으로 표시
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
        #endregion

        #region Custom Method
        public void OnAttackHit()
        {
            if (targetPlayer == null) return;
            // 이벤트가 발생한 순간, 플레이어와의 거리를 다시 계산
            float sqrDistance = (targetPlayer.position - transform.position).sqrMagnitude;
            // 플레이어가 도망가지 못하고 공격 범위(혹은 약간 더 넓은 판정 범위) 안에 있다면
            if (sqrDistance <= (attackRange * attackRange))
            {
                // 플레이어에게 데미지 전달
                if (targetPlayer.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(attackDamage);
                }
            }

            // 공격 쿨다운 초기화
            attackCooldown = attackInterval;
        }

        public void TakeDamage(float damage, UnityEngine.Vector3 hitDirection = default)
        {
            if (isDead) return;

            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                isDead = true;

                // 에러 방지를 위해 CharacterController 자체는 끄지 않고, 
                // 다른 물리 객체(플레이어 등)와 겹쳐도 서로 밀어내지 않게 설정합니다.
                if (characterController != null) 
                {
                    characterController.detectCollisions = false;
                    
                    // 플레이어의 이동을 방해하지 않도록 크기를 최소화하되,
                    // 바닥 밑으로 꺼지지 않도록 중심점(Center)도 발바닥 위치로 같이 낮춰줍니다.
                    characterController.radius = 0.01f;
                    characterController.height = 0.01f;
                    characterController.center = new Vector3(0f, 0.01f, 0f);
                }

                characterMove.horizontalMoveIntent = Vector2.zero; // 이동 중지

                animator.SetInteger(EnemyStateHash, (int)State.Dead);
                onDeath.Invoke();
            }
        }
        #endregion
    }
}
