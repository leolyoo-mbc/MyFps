using UnityEngine;

namespace MyFps
{
    public class CharacterInput : MonoBehaviour
    {
        #region Variables
        private InputSystem_Actions inputActions;
        //wasd 입력값
        private Vector2 move;
        private Vector2 look;
        #endregion

        #region Property
        public Vector2 Move => move;
        public Vector2 Look => look;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        private void Update()
        {
            //wasd 입력값 처리: 인스턴스이름.액션맵이름.액션이름.ReadValue
            move = inputActions.Player.Move.ReadValue<Vector2>();
            look = inputActions.Player.Look.ReadValue<Vector2>();
        }
        #endregion

        #region Custom Method

        #endregion
    }
}