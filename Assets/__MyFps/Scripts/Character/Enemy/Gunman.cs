using System;
using UnityEngine;
using UnityEngine.AI;

namespace MyFps
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class Gunman : MonoBehaviour, IDamageable
    {
        public enum State
        {
            Idle, Walk, Chase, Attack, Dead
        }

        #region Variables
        private Animator anim;
        private NavMeshAgent agent;

        // 상태 위임자 (현재 실행할 Update 로직)
        private Action stateUpdate;

        // Inspector에서 현재 상태 확인 및 테스트용
        [SerializeField] private State currentState;
        private State lastState;

        [Header("Target & Health")]
        [SerializeField] private Transform targetPlayer; // 에디터에서 플레이어 할당
        [SerializeField] private float maxHealth = 20f;
        private float currentHealth = 0f;

        // 애니메이션 파라미터 해시
        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static readonly int FireTriggerHash = Animator.StringToHash("FireTrigger");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

        [Header("Idle & Patrol")]
        [SerializeField] private float idleTimer = 2f;
        private float idleCountdown = 0f;

        [SerializeField] private bool isPatrol = false;
        public Transform[] wayPoints;
        private int wayPointIndex = 0;
        private Vector3 startPosition = Vector3.zero;

        [Header("Chase & Attack")]
        [SerializeField] private float detectRange = 15f; // 적 발견 거리
        [SerializeField] private float attackRange = 8f;  // 사격 가능 거리
        [SerializeField] private float attackInterval = 2f;
        private float attackCountdown = 0f;

        [SerializeField] private float attackDamage = 5f;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            anim = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            currentHealth = maxHealth;
            startPosition = transform.position;
            wayPointIndex = 1;

            // 타겟이 비어있다면 태그로 찾기 (안전 장치)
            if (targetPlayer == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) targetPlayer = playerObj.transform;
            }

            isPatrol = wayPoints != null && wayPoints.Length >= 2;

            ChangeState(State.Idle);
        }

        private void Update()
        {
            // switch문 없이 현재 상태에 연결된 함수만 호출
            stateUpdate?.Invoke();
        }
        #endregion

        #region Custom Method - State Machine
        public void ChangeState(State newState)
        {
            lastState = currentState;
            currentState = newState;

            // 상태 진입(Enter) 초기화 및 Update 함수 위임
            switch (currentState)
            {
                case State.Idle:
                    anim.SetLayerWeight(1, 0f); // 비전투 상태: 조준 풀기
                    idleCountdown = idleTimer; // 초기화 시 최대 시간으로 설정
                    agent.isStopped = true;
                    stateUpdate = UpdateIdle;
                    break;

                case State.Walk:
                    anim.SetLayerWeight(1, 0f); // 비전투 상태: 조준 풀기
                    agent.isStopped = false;
                    if (isPatrol) agent.SetDestination(wayPoints[wayPointIndex].position);
                    stateUpdate = UpdateWalk;
                    break;

                case State.Chase:
                    anim.SetLayerWeight(1, 1f); // 전투 상태: 조준 활성화
                    agent.isStopped = false;
                    stateUpdate = UpdateChase;
                    break;

                case State.Attack:
                    anim.SetLayerWeight(1, 1f); // 전투 상태: 조준 활성화
                    agent.isStopped = true;
                    attackCountdown = attackInterval; // 진입 시 쿨타임 초기화
                    stateUpdate = UpdateAttack;
                    break;

                case State.Dead:
                    anim.SetBool(IsDeadHash, true);
                    anim.SetLayerWeight(1, 0f); // 죽을 때 조준 덮어쓰기 해제
                    agent.isStopped = true;
                    agent.enabled = false; // 충돌체(길찾기) 제거
                    stateUpdate = null;    // Update 실행 중지
                    break;
            }
        }

        private void UpdateIdle()
        {
            anim.SetFloat(MoveSpeedHash, 0f);

            // 감지 범위 내에 플레이어가 들어오면 추적 시작
            if (targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= detectRange)
            {
                ChangeState(State.Chase);
                return;
            }

            // 순찰 대기 로직
            if (isPatrol)
            {
                // 삼항 연산자를 활용하여 0 아래로 내려가지 않도록 감소
                idleCountdown = idleCountdown > 0f ? idleCountdown - Time.deltaTime : 0f;

                if (idleCountdown <= 0f)
                {
                    ChangeState(State.Walk);
                }
            }
        }

        private void UpdateWalk()
        {
            anim.SetFloat(MoveSpeedHash, agent.velocity.magnitude);

            // 순찰 중에도 감지 범위 내에 플레이어가 들어오면 추적 시작
            if (targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.position) <= detectRange)
            {
                ChangeState(State.Chase);
                return;
            }

            // 도착 판정
            if (!agent.pathPending && agent.remainingDistance < 0.1f)
            {
                if (isPatrol)
                {
                    wayPointIndex++;
                    if (wayPointIndex >= wayPoints.Length) wayPointIndex = 0;
                }
                ChangeState(State.Idle);
            }
        }

        private void UpdateChase()
        {
            anim.SetFloat(MoveSpeedHash, agent.velocity.magnitude);

            if (targetPlayer == null) return;

            float distance = Vector3.Distance(transform.position, targetPlayer.position);

            // 거리가 너무 멀어지면 추적 포기 (여유를 두기 위해 감지 거리의 1.5배로 설정)
            if (distance > detectRange * 1.5f)
            {
                ChangeState(State.Idle);
                return;
            }

            // 공격 사거리 내에 들어오면 공격 상태로
            if (distance <= attackRange)
            {
                ChangeState(State.Attack);
                return;
            }

            // 플레이어 쫓아가기
            agent.SetDestination(targetPlayer.position);
        }

        private void UpdateAttack()
        {
            anim.SetFloat(MoveSpeedHash, 0f);

            if (targetPlayer == null) return;

            float distance = Vector3.Distance(transform.position, targetPlayer.position);

            // 사거리 밖으로 도망가면 다시 추적
            if (distance > attackRange)
            {
                ChangeState(State.Chase);
                return;
            }

            // 플레이어 방향으로 부드럽게 회전
            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }

            // 사격 쿨다운
            attackCountdown = attackCountdown > 0f ? attackCountdown - Time.deltaTime : 0f;

            if (attackCountdown <= 0f)
            {
                anim.SetTrigger(FireTriggerHash);
                attackCountdown = attackInterval;
            }
        }
        #endregion

        #region Interface Methods
        public void TakeDamage(float damage)
        {
            if (currentState == State.Dead) return;

            currentHealth -= damage;

            // 뒤에서 맞았거나 평화 상태일 때 맞으면 강제로 추적(Chase) 모드로 전환
            if (currentState == State.Idle || currentState == State.Walk)
            {
                ChangeState(State.Chase);
            }

            if (currentHealth <= 0)
            {
                ChangeState(State.Dead);
            }
        }

        // Animation Event 함수 (애니메이터의 Firing Rifle 클립 쏠 때 프레임에 추가)
        public void OnFireBullet()
        {
            if (targetPlayer == null || currentState == State.Dead) return;

            // 이곳에서 실제 총알 발사 혹은 플레이어 데미지 처리 로직을 넣으시면 됩니다.
            // if (Vector3.Distance(transform.position, targetPlayer.position) <= attackRange + 2f)
            // {
            //     targetPlayer.GetComponent<IDamageable>()?.TakeDamage(attackDamage);
            // }
        }
        #endregion
    }
}