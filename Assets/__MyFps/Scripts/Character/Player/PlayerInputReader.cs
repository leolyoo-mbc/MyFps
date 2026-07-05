using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

namespace MyFps
{
    [RequireComponent(typeof(CharacterMove))]
    [RequireComponent(typeof(PlayerLook))]
    [RequireComponent(typeof(PlayerInteract))]
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
            playerMove = GetComponent<CharacterMove>();
            playerLook = GetComponent<PlayerLook>();
            playerInteract = GetComponent<PlayerInteract>();
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
            playerAttack.attackIntent = context.ReadValueAsButton();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            playerInteract.interactIntent = context.ReadValueAsButton();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            playerMove.jumpIntent = context.ReadValueAsButton();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            playerLook.lookIntent = context.ReadValue<Vector2>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            playerMove.horizontalMoveIntent = context.ReadValue<Vector2>();
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
            playerMove.sprintIntent = context.ReadValueAsButton();
        }
        #endregion
    }
}