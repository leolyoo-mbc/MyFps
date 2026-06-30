using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    public class Actor : MonoBehaviour
    {
        public Animator ActorAnimator { get; private set; }
        public CharacterController ActorController { get; private set; }

        private InputSystem_Actions inputActions;
        public Vector2 MoveInput { get; private set; }
        public bool IsSprint { get; private set; }

        [Header("스탯")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;
        [SerializeField] private float jumpHeight = 1.2f;

        [Header("상태 데이터")]
        public float verticalVelocity = 0f;
        public float gravity = -9.81f;
        public bool isGrounded = false;

        [Header("Ground Check")]
        [SerializeField] private float groundedOffset = -0.14f;
        [SerializeField] private float groundedRadius = 0.5f;
        [SerializeField] private LayerMask groundLayers;

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float JumpHeiht => jumpHeight;

        // 1. 상태 객체들을 직접 참조 (성능 최적화)
        public ActorGroundState GroundState { get; private set; }
        public ActorAirState AirState { get; private set; }
        //public CharacterAttackState AttackState { get; private set; }
        //public CharacterHitState HitState { get; private set; }
        //public CharacterDeathState DeathState { get; private set; }

        public ActorBaseState CurrentState { get; private set; }

        private void Awake()
        {
            ActorAnimator = GetComponent<Animator>();
            ActorController = GetComponent<CharacterController>();

            // 상태 인스턴스 생성 및 초기화
            GroundState = new ActorGroundState(this);
            AirState = new ActorAirState(this);
            //AttackState = new CharacterAttackState(this);
            //HitState = new CharacterHitState(this);
            //DeathState = new CharacterDeathState(this);

            ChangeState(GroundState);
        }

        private void Update() => CurrentState?.LogicUpdate();
        private void FixedUpdate() => CurrentState?.PhysicsUpdate();

        private void OnEnable() => inputActions.Enable();
        private void OnDisable() => inputActions.Disable();

        public void ChangeState(ActorBaseState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter();
        }

        // --- 공통 로직 메서드 ---

        public void CheckGrounded()
        {
            Vector3 checkPosition = new(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
            isGrounded = Physics.CheckSphere(checkPosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }

        public void Move()
        {
            float targetSpeed = IsSprint ? runSpeed : walkSpeed;
            if (MoveInput == Vector2.zero) targetSpeed = 0f;

            Vector3 inputDirection = Vector3.zero;
            if (MoveInput != Vector2.zero)
            {
                inputDirection = transform.right * MoveInput.x + transform.forward * MoveInput.y;
            }

            ActorController.Move(targetSpeed * Time.deltaTime * inputDirection + Time.deltaTime * verticalVelocity * Vector3.up);
        }
    }
}