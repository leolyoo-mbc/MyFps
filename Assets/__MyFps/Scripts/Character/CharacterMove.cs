using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMove : MonoBehaviour
    {
        #region Variables
        private CharacterController controller;

        [Header("Input Intent")]
        public Vector2 horizontalMoveIntent;
        public bool sprintIntent = false;
        public bool jumpIntent = false;

        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        [SerializeField] private float jumpHeight = 1.5f;

        [Header("Gravity Settings")]
        [SerializeField] private float baseGravity = -9.81f;
        public float gravityMultiplier = 1.0f; // 외부 상태(무중력, 물속 등)에서 조절할 배율

        private float verticalMomentum = 0f; // 수직 이동(중력/점프) 상태 유지용
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (horizontalMoveIntent == null) horizontalMoveIntent = Vector2.zero;
            // 0. 공통 중력값 계산 (점프와 중력 누적에 모두 사용)
            float currentGravity = baseGravity * gravityMultiplier;

            // 1. 바닥 체크 및 수직 속도(중력 누적치) 초기화
            // isGrounded일 때 속도를 살짝 음수로 유지해 계단/경사로에서 안 튀어오르고 밀착되게 합니다.
            if (controller.isGrounded)
            {
                if (jumpIntent)
                {
                    // 점프 의지가 켜져있다면 물리 공식에 따라 위로 솟구침
                    verticalMomentum = Mathf.Sqrt(jumpHeight * -2f * currentGravity);
                }
                else if (verticalMomentum < 0)
                {
                    // 점프 의지가 없고 바닥에 있다면 밀착 유지
                    verticalMomentum = -2f;
                }
            }
            else if (verticalMomentum < 0)
            {
                // 정점을 찍고 떨어지기 시작하면 점프 상태(의지) 초기화 (Fall 애니메이션 재생용)
                jumpIntent = false;
            }

            // 2. 수평 이동 계산 (상대 좌표 기준)
            // 마우스 회전에 따라 내가 바라보는 앞쪽(forward)과 오른쪽(right)을 기준으로 삼습니다.
            float currentSpeed = sprintIntent ? sprintSpeed : walkSpeed;
            Vector3 horizontalMove = ((transform.right * horizontalMoveIntent.x) + (transform.forward * horizontalMoveIntent.y)) * currentSpeed;

            // 3. 중력 누적 계산 (수직 이동)
            verticalMomentum += currentGravity * Time.deltaTime;

            // 4. 수평 + 수직 이동 벡터를 합쳐서 딱 한 번만 Move() 호출
            controller.Move(new Vector3(horizontalMove.x, verticalMomentum, horizontalMove.z) * Time.deltaTime);
        }
        #endregion
    }
}