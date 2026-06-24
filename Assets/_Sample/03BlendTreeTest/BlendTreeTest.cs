using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{
    /// <summary>
    /// 애니메이터의 블랜드 트리 테스트 예제 클래스
    /// </summary>
    public class BlendTreeTest : MonoBehaviour
    {
        #region Variables
        // 참조
        private Animator animator;

        //인풋 액션
        [SerializeField] private InputActionReference moveAction;

        private Vector2 inputMove;

        // 이동
        private float moveSpeed = 10f;

        //애니메이션
        private static readonly string moveStateString = "MoveState";
        private static readonly string moveXString = "MoveX";
        private static readonly string moveYString = "MoveY";

        #endregion

        #region Propoerty
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
        }

        private void OnDisable()
        {
            // 인풋 액션 비활성화
            moveAction.action.Disable();
        }

        private void Update()
        {
            // 인풋 처리
            inputMove = moveAction.action.ReadValue<Vector2>();

            // 애니메이터 처리
            //AnimationStateTest(inputMove);
            animator.SetFloat(moveXString, inputMove.x);
            animator.SetFloat (moveYString, inputMove.y);

            // 캐릭터 이동: 방향 * Time.deltaTime * moveSpeed
            Vector3 dir = new(inputMove.x, 0f, inputMove.y);
            transform.Translate(moveSpeed * Time.deltaTime * dir, Space.World);

        }
        #endregion

        #region Custom Method
        private void AnimationStateTest(Vector2 moveDir)
        {
            if (moveDir == Vector2.zero)
            {
                animator.SetInteger(moveStateString, 0);
            }
            else
            {
                if (moveDir.y > 0f) animator.SetInteger(moveStateString, 1); // 전진
                if (moveDir.y < 0f) animator.SetInteger(moveStateString, 2); // 후진
                if (moveDir.x < 0f) animator.SetInteger(moveStateString, 3); // 좌로
                if (moveDir.x > 0f) animator.SetInteger(moveStateString, 4); // 우로
            }
        }
        #endregion
    }
}