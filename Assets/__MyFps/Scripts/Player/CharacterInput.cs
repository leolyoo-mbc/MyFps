using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
        }

        private void Update()
        {
            //wasd 입력값 처리: 인스턴스이름.액션맵이름.액션이름.ReadValue
            move = inputActions.Player.Move.ReadValue<Vector2>();
            look = inputActions.Player.Look.ReadValue<Vector2>();

            //버튼 입력값 처리
            isSprint = inputActions.Player.Sprint.IsPressed();

            if (inputActions.Player.Jump.WasPressedThisFrame()) IsJump = true;

            if (inputActions.Player.Interact.WasPressedThisFrame()) IsInteracting = true;


            //          if (inputActions.Player.Sprint.WasPressedThisFrame()) isSprint = true;
            //          else if (inputActions.Player.Sprint.WasReleasedThisFrame()) isSprint = false;

            //          네, 결론부터 말씀드리면 두 코드는 99 % 똑같이 동작합니다.둘 다 "버튼을 누르고 있는 동안에는  true , 떼면  false
            //"라는 결과를 만들어냅니다.

            //하지만 실무에서는 첫 번째 방식인 IsPressed() 를 훨씬 더 권장합니다. 그 이유는 다음과 같습니다.

            //### 1. 코드가 훨씬 간결합니다

            //  isSprint = inputActions.Player.Sprint.IsPressed();

            //          이 한 줄이면 끝날 것을, 두 번째 방식은 불필요하게  if ~ else if  문을 사용하여 코드가 길어지고 가독성이 떨어집니다.

            //### 2. 예외 상황(버그)에 더 안전합니다

            //두 번째 방식(WasPressed 와  WasReleased 를 조합하는 방식)은 상태가 변화하는 순간(Event) 에만 의존합니다.
            //만약 게임 중에 렉(Lag)이 심하게 걸려서 프레임이 건너뛰어지거나, 유저가 쉬프트 키를 누른 상태로 알트탭(Alt+Tab) 을
            //눌러 윈도우 밖으로 나갔다가 키를 떼고 돌아오면 어떻게 될까요?

            //• 두 번째 방식: 게임은 유저가 키를 뗐다는(WasReleasedThisFrame ) 신호를 놓치게 됩니다.그래서 키보드에서 손을
            //뗐는데도 캐릭터가 계속 무한정 달리는 버그가 발생할 수 있습니다.
            //• 첫 번째 방식(IsPressed()): 매 프레임마다 "지금 당장 버튼이 물리적으로 눌려 있는 상태인가?" 를 직접 확인합버 다.
            //알트탭을 하고 돌아오든 렉이 걸리든, 현재 키가 안 눌려 있으면 바로 깔끔하게  false 로 처리되므로 버그가 생길 확률이
            //훨씬 낮습니다.

            //요약하자면:
            //두 로직의 목적과 결과는 같지만,  IsPressed() 가 더 안전하고 깔끔한 정답이라고 생각하시면 됩니다!


        }


        #endregion

        #region Custom Method
        public void OnMove(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }
        #endregion
    }
}