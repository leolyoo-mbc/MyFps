using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    public class CharacterInput : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        #region Variables
        private InputSystem_Actions inputActions;

        private Vector2 move;

        private Vector2 look;

        private bool isSprint;

        private bool isJump;

        private bool isInteracting;

        public bool isAttacking;

        #endregion

        #region Property
        public Vector2 Move => move;
        public Vector2 Look => look;

        public bool IsSprint => isSprint;

        public bool IsJump { get => isJump; set => isJump = value; }

        public bool IsInteracting { get => isInteracting; set => isInteracting = value; }

        #endregion

        #region Unity Event Method
        private void Awake()
        {
            inputActions = new InputSystem_Actions();
            inputActions.Player.AddCallbacks(this);

        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
            move = Vector2.zero;
            look = Vector2.zero;
            isSprint = false;
            isJump = false;
            isInteracting = false;
            isAttacking = false;
        }
        #endregion

        #region Custom Method
        public void OnMove(InputAction.CallbackContext context)
        {
            move = context.action.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            look = context.action.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started) isAttacking = true;
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started) IsInteracting = true;
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started) IsJump = true;
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            isSprint = !context.canceled;
        }
        #endregion
    }
}