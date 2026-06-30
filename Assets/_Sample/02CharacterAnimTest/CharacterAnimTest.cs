using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{
    /// <summary>
    /// 캐릭터 애니메이션을 제어하는 예제 클래스
    /// New Input System
    /// 기본이 대기 상태
    /// W키가 들어오면 걷기 상태
    /// + Shift키를 누르면 뛰기 상태
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimTest : MonoBehaviour
    {
        #region Variables
        // 참조
        private Animator animator;
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference runAction;

        // 이동
        [SerializeField] private bool isMove;
        [SerializeField] private bool isRun;

        [SerializeField] private float moveDeadZone = 0.01f;
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 7f;

        // 키 캐싱
        private static readonly int IsMoveHash = Animator.StringToHash("IsMove");
        private static readonly int IsRunHash = Animator.StringToHash("IsRun");
        private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
        #endregion

        #region Property
        public bool IsMove
        {
            get => isMove;
            private set
            {
                isMove = value;
                animator.SetBool(IsMoveHash, value);
            }
        }
        public bool IsRun
        {
            get => isRun;
            private set
            {
                isRun = value;
                animator.SetBool(IsRunHash, value);
            }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            animator = GetComponent<Animator>();

        }

        private void OnEnable()
        {
            // 인풋 액션 활성화
            moveAction.action.Enable();
            runAction.action.Enable();
        }

        private void OnDisable()
        {
            // 인풋 액션 비활성화
            moveAction.action.Disable();
            runAction.action.Disable();
        }

        private void Update()
        {
            // 인풋 처리
            Vector2 inputMove = moveAction.action.ReadValue<Vector2>();
            IsMove = inputMove.sqrMagnitude > moveDeadZone;
            IsRun = runAction.action.IsPressed();

            float moveSpeed = IsMove ? IsRun ? runSpeed : walkSpeed : 0;

            animator.SetFloat(MoveSpeedHash, moveSpeed);
        }
        #endregion

        #region Custom Method

        #endregion
    }
}