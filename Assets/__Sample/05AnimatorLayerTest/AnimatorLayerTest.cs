using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorLayerTest : MonoBehaviour
    {
        #region Variables
        // 참조
        private Animator animator;
        private readonly int IsMoveHash = Animator.StringToHash("IsMove");

        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference aimAction;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            moveAction.action.Enable();
            aimAction.action.Enable();
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            aimAction.action.Disable();
        }

        private void Update()
        {
            animator.SetBool(IsMoveHash, moveAction.action.ReadValue<Vector2>() != Vector2.zero);

            animator.SetLayerWeight(1, aimAction.action.IsPressed() ? 1f : 0f);
        }
        #endregion

        #region Custom Method
        #endregion
    }
}
