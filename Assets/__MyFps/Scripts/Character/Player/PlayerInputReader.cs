using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

namespace MyFps
{
    public class PlayerInputReader : MonoBehaviour, IPlayerActions
    {
        #region Variables
        private InputSystem_Actions inputActions;
        [SerializeField] private CharacterMove playerMove;
        [SerializeField] private PlayerLook playerLook;
        [SerializeField] private PlayerInteract playerInteract;
        [SerializeField] private Pistol playerAttack;
        #endregion

        #region Unity Event Method
        void Awake()
        {
            inputActions = new InputSystem_Actions();

            if (TryGetComponent<CharacterMove>(out var playerMove)) this.playerMove = playerMove;
            if (TryGetComponent<PlayerLook>(out var playerLook)) this.playerLook = playerLook;
            if (TryGetComponent<PlayerInteract>(out var playerInteract)) this.playerInteract = playerInteract;
        }

        void OnEnable()
        {
            inputActions.Enable();
            inputActions.Player.AddCallbacks(this);
            playerLook.enabled = true;
        }

        void OnDisable()
        {
            inputActions.Disable();
            inputActions.Player.RemoveCallbacks(this);
            playerLook.enabled = false;
        }
        #endregion

        #region Input Callbacks
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (playerAttack != null) playerAttack.attackIntent = context.ReadValueAsButton();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (playerInteract != null) playerInteract.interactIntent = context.ReadValueAsButton();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (playerMove != null) playerMove.jumpIntent = context.ReadValueAsButton();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if (playerLook != null) playerLook.lookIntent = context.ReadValue<Vector2>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (playerMove != null) playerMove.horizontalMoveIntent = context.ReadValue<Vector2>();
        }

        public void OnNext(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (playerMove != null) playerMove.sprintIntent = context.ReadValueAsButton();
        }
        #endregion
    }
}