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
            Idle, Walk, Chase, Attack, Search, Dead
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
        // 이제 에디터에서 플레이어를 할당하지 않아도 됨 (FOV 센서로 자동 감지)
        private Transform targetPlayer;
        [SerializeField] private float maxHealth = 20f;
        private float currentHealth = 0f;

        // 애니메이션 파라미터 해시
        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        private static readonly int FireTriggerHash = Animator.StringToHash("FireTrigger");
        private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

        [Header("Field of View (시야 센서)")]
        [SerializeField] private Transform eyeTransform;  // 눈(머리) 위치 뼈대
        [SerializeField] private float detectRange = 15f; // 시야 거리
        [SerializeField] private float viewAngle = 120f;  // 시야각
        [SerializeField] private LayerMask targetMask;    // 플레이어 레이어 (인스펙터에서 7번 Player 할당 필요!)
        [SerializeField] private LayerMask obstacleMask;  // 벽/장애물 레이어 (보통 Default나 Obstacle)

        [Header("Idle & Patrol")]
        [SerializeField] private float idleTimer = 2f;
        private float idleCountdown = 0f;

        [SerializeField] private bool isPatrol = false;
        public Transform[] wayPoints;
        private int wayPointIndex = 0;
        private Vector3 startPosition = Vector3.zero;

        [Header("Chase & Attack")]
        [SerializeField] private float attackRange = 8f;  // 사격 가능 거리
        [SerializeField] private float attackInterval = 2f;
        private float attackCountdown = 0f;

        [Header("Search (수색)")]
        [SerializeField] private float searchDuration = 5f; // 수색 대기 시간
        private float searchCountdown = 0f;
        private Vector3 lastKnownPosition; // 마지막으로 본 위치
        private Vector3 searchDirection;   // 등 뒤에서 맞았을 때 돌아볼 방향

        [Header("Damage")]
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

            isPatrol = wayPoints != null && wayPoints.Length >= 2;

            ChangeState(State.Idle);
        }

        private void Update()
        {
            // switch문 없이 현재 상태에 연결된 함수만 호출
            stateUpdate?.Invoke();
        }

        // 씬 뷰에서 시야를 확인하기 위한 기즈모 그리기
        private void OnDrawGizmosSelected()
        {
            Vector3 eyePos = eyeTransform != null ? eyeTransform.position : transform.position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(eyePos, detectRange);

            Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(eyePos, eyePos + leftBoundary * detectRange);
            Gizmos.DrawLine(eyePos, eyePos + rightBoundary * detectRange);
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

                case State.Search:
                    anim.SetLayerWeight(1, 1f); // 전투 상태: 총을 든 채로 수색
                    
                    if (searchDirection != Vector3.zero)
                    {
                        agent.isStopped = true; // 방향만 돌릴 때는 이동 정지
                    }
                    else
                    {
                        agent.isStopped = false;
                        agent.SetDestination(lastKnownPosition); // 마지막 위치로 이동
                    }
                    
                    searchCountdown = searchDuration;
                    stateUpdate = UpdateSearch;
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

        // 시야 판정 (FOV + Raycast)
        private bool CanSeePlayer()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, targetMask);
            if (hits.Length > 0)
            {
                Transform target = hits[0].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                // 내 정면 기준 시야각 안에 있는지 확인
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2f)
                {
                    // 에디터에서 할당한 눈 위치가 있으면 사용, 없으면 기본 transform.position 사용
                    Vector3 eyePosition = eyeTransform != null ? eyeTransform.position : transform.position;

                    // 플레이어와 벽 레이어를 합친 마스크로 '최대 시야 거리(detectRange)'만큼 레이를 쏩니다.
                    int combinedMask = targetMask | obstacleMask;

                    if (Physics.Raycast(eyePosition, dirToTarget, out RaycastHit hit, detectRange, combinedMask))
                    {
                        // 레이에 가장 먼저 맞은 물체의 레이어가 '플레이어 레이어'라면 벽에 가려지지 않은 것입니다!
                        if (((1 << hit.collider.gameObject.layer) & targetMask) != 0)
                        {
                            targetPlayer = target;
                            lastKnownPosition = target.position; // 보일 때마다 마지막 위치 갱신
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        private void UpdateIdle()
        {
            anim.SetFloat(MoveSpeedHash, 0f);

            // 시야에 플레이어가 들어오면 추적 시작
            if (CanSeePlayer())
            {
                ChangeState(State.Chase);
                return;
            }

            // 순찰 대기 로직
            if (isPatrol)
            {
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

            // 시야에 플레이어가 들어오면 추적 시작
            if (CanSeePlayer())
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

            // 계속 눈에 보이는지 체크 (안 보이면 수색 상태로 전환)
            if (!CanSeePlayer())
            {
                ChangeState(State.Search);
                return;
            }

            float sqrDistance = (transform.position - targetPlayer.position).sqrMagnitude;

            // 공격 사거리 내에 들어오면 공격 상태로
            if (sqrDistance <= attackRange * attackRange)
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

            // 시야에서 벗어나거나 벽에 숨으면 수색 상태로
            if (!CanSeePlayer())
            {
                ChangeState(State.Search);
                return;
            }

            float sqrDistance = (transform.position - targetPlayer.position).sqrMagnitude;

            // 사거리 밖으로 도망가면 다시 추적
            if (sqrDistance > attackRange * attackRange)
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

        private void UpdateSearch()
        {
            anim.SetFloat(MoveSpeedHash, agent.velocity.magnitude);

            // 수색 중에도 언제든 다시 시야에 들어오면 즉시 추적 시작
            if (CanSeePlayer())
            {
                searchDirection = Vector3.zero; // 시야 확보 시 초기화
                ChangeState(State.Chase);
                return;
            }

            // 방향 탐색 모드 (피격 당해서 방향만 알 때)
            if (searchDirection != Vector3.zero)
            {
                anim.SetFloat(MoveSpeedHash, 0f); // 제자리에서
                
                // 맞은 반대 방향으로 몸을 돌립니다.
                if (searchDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(searchDirection), Time.deltaTime * 5f);
                }
                
                searchCountdown = searchCountdown > 0f ? searchCountdown - Time.deltaTime : 0f;
                if (searchCountdown <= 0f)
                {
                    searchDirection = Vector3.zero;
                    targetPlayer = null;
                    ChangeState(State.Walk);
                }
            }
            // 위치 탐색 모드 (플레이어가 도망가서 마지막 위치로 갈 때)
            else
            {
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    anim.SetFloat(MoveSpeedHash, 0f); // 애니메이션 멈춤
                    
                    searchCountdown = searchCountdown > 0f ? searchCountdown - Time.deltaTime : 0f;
                    if (searchCountdown <= 0f)
                    {
                        // 시간이 다 되도록 못 찾으면 타겟을 잊고 순찰로 돌아감
                        targetPlayer = null;
                        ChangeState(State.Walk); // 바로 걷게 하려면 Walk, 대기하게 하려면 Idle
                    }
                }
            }
        }
        #endregion

        #region Interface Methods
        public void TakeDamage(float damage, UnityEngine.Vector3 hitDirection = default)
        {
            if (currentState == State.Dead) return;

            currentHealth -= damage;

            // 평화 상태(Idle/Walk)에서 공격받았다면 (방향 감지)
            if (currentState == State.Idle || currentState == State.Walk)
            {
                if (hitDirection != default)
                {
                    // 총알이 날아온 반대 방향을 저장
                    searchDirection = -hitDirection.normalized;
                    searchDirection.y = 0; // 수평 고정
                    
                    if (searchDirection != Vector3.zero)
                    {
                        ChangeState(State.Search);
                    }
                }
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

            // 총알이 날아가는 시작점
            Vector3 firePosition = eyeTransform != null ? eyeTransform.position : transform.position;
            
            // 타겟의 콜라이더(충돌체)의 정확한 중심점(bounds.center)을 가져와서 조준합니다.
            // 억지로 +1.0f를 더할 필요 없이 모델의 크기나 앉은 자세 등에 맞춰 자동으로 중앙을 노립니다.
            Collider targetCollider = targetPlayer.GetComponent<Collider>();
            Vector3 targetCenter = targetCollider != null ? targetCollider.bounds.center : targetPlayer.position;
            Vector3 dirToTarget = (targetCenter - firePosition).normalized;

            // 플레이어와 벽을 모두 감지하는 합친 마스크
            int combinedMask = targetMask | obstacleMask;

            // 사거리(attackRange) 내에서 레이캐스트 판정
            if (Physics.Raycast(firePosition, dirToTarget, out RaycastHit hit, attackRange, combinedMask))
            {
                // 가장 먼저 맞은 물체가 플레이어라면 (벽에 안 가려졌다면) 데미지 적용
                if (((1 << hit.collider.gameObject.layer) & targetMask) != 0)
                {
                    IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                    damageable?.TakeDamage(attackDamage, dirToTarget);
                }
            }
        }
        #endregion
    }
}