using System;
using System.Security.Cryptography;
using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    public class Robot : MonoBehaviour, IDamageable
    {
        #region Variables
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 0.5f;
        [SerializeField] private Transform targetPlayer;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackDamage = 5f;
        [SerializeField] private float attackInterval = 2f;
        [SerializeField] private float maxHealth = 20f;


        private Animator animator;
        private static readonly int EnemyStateHash = Animator.StringToHash("EnemyState");
        private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");

        private CharacterController characterController;
        private float attackCooldown;

        private float currentHealth;
        private bool isDead = false;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            attackCooldown = attackInterval;
            currentHealth = maxHealth;
        }

        private void Update()
        {
            Vector3 moveDirection = Vector3.zero;

            // 죽은 상태면 충돌체를 끄고 리턴
            if (animator.GetInteger(EnemyStateHash) == 3)
            {
                if (characterController != null) characterController.enabled = false;
                return;
            }

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
                animator.SetBool(IsAttackingHash, true);
                if (attackCooldown <= 0)
                {
                    // 공격
                    animator.SetInteger(EnemyStateHash, 2);
                }
                else
                {
                    // 쿨타임 중: 공격 상태가 아니라면 대기 상태로 전환
                    animator.SetInteger(EnemyStateHash, 0);
                }
            }
            else
            {
                animator.SetBool(IsAttackingHash, false);
                // 이동
                moveDirection = transform.forward * moveSpeed;
                animator.SetInteger(EnemyStateHash, 1);
            }

            // 중력 적용 및 이동
            if (characterController != null && characterController.enabled)
            {
                if (characterController.isGrounded)
                {
                    // 바닥에 닿아있을 때 경사로 등을 안정적으로 타기 위해 약간의 음수 유지
                    moveDirection.y = -2f;
                }
                else
                {
                    // 공중에 있을 때 중력 적용
                    moveDirection.y = -9.81f;
                }

                // CharacterController를 통한 이동
                characterController.Move(moveDirection * Time.deltaTime);
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

        public void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                animator.SetInteger(EnemyStateHash, 3);
                isDead = true;
            }
        #endregion
        }
    }
}
