using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(CharacterInput))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour
    {
        #region Variables
        private CharacterInput input;
        private CharacterController controller;

        [Header("Move")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        private float moveSpeed;

        [Header("Ground Check")]
        [SerializeField] private bool isGrounded = false;
        private float groundedOffset = -0.14f; // 체크 지점 조정값
        private float goundedRadius = 0.5f; // 체크 범위 영역
        public LayerMask groundLayers; // 그라운드 레이어 체크

        [Header("Jump")]
        [SerializeField] private float gravity = -9.81f; // 중력값
        [SerializeField] private float verticalVelocity = 0f; // y축의 속도 값

        [SerializeField] private float jumpHeight = 1.2f; // 점프 높이
        [SerializeField] private float jumpTimeout = 0.1f; // 점프 키입력 타이머
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = GetComponent<CharacterInput>();
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            CheckGrounded();

            GravityAndJump();

            Move();
        }
        #endregion

        #region Custom Method
        private void GravityAndJump()
        {
            if (isGrounded)
            {
                // 지면에 있을 때 verticalVelocity 값이 무한히 마이너스로 누적되는 것 방지
                if (verticalVelocity < 0.0f) verticalVelocity = -2f;

                // 점프
                if (input.IsJump && jumpTimeout <= 0.0f)
                {
                    // 점프높이만큼 속도를 지정
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
                }

                // 점프 쿨타임 감소
                if (jumpTimeout >= 0.0f) jumpTimeout -= Time.deltaTime;
            }
            else
            {
                // 공중에 있을 때 점프 쿨타임 초기화
                jumpTimeout = 0.1f;
            }

            // 입력 소비 (땅에 있든 공중에 있든 소비해서 무한 점프 방지)
            input.IsJump = false;

            // 중력은 '공중에 있을 때도' 매 프레임 적용되어야 합니다!
            verticalVelocity += gravity * Time.deltaTime;
        }

        private void CheckGrounded()
        {
            // 방법1:
            // 캐릭터 컨트롤러에서 IsGrounded 값 가져오기
            //isGrounded = controller.isGrounded;

            // 방법2:
            // 체크 위치 설정
            Vector3 checkPosition = new(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
            // 체크 지점에서 그라운드 레이어가 있는지 체크
            isGrounded = Physics.CheckSphere(checkPosition, goundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }

        private void Move()
        {
            moveSpeed = input.IsSprint ? sprintSpeed : walkSpeed;

            if (input.Move == Vector2.zero) moveSpeed = 0f;

            Vector3 inputDirection = Vector3.zero;

            if (input.Move != Vector2.zero)
            {
                inputDirection = transform.right * input.Move.x + transform.forward * input.Move.y;
            }

            // 이동: 방향(전후좌우) * Time.deltaTime * speed + 중력(상하) * Time.deltaTime * verticalVelocity
            controller.Move(moveSpeed * Time.deltaTime * inputDirection + Time.deltaTime * verticalVelocity * Vector3.up);
        }
        #endregion
    }
}