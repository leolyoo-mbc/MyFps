using UnityEngine;

namespace MyFps
{
    public class ActorGroundState : ActorBaseState
    {

        public ActorGroundState(Actor actor) : base(actor)
        {
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void LogicUpdate()
        {
            actor.CheckGrounded();

            // 바닥에 없다면 떨어지는 중이므로 공중 상태로 전환
            if (!actor.isGrounded)
            {
                actor.ChangeState(actor.AirState);
                return;
            }

            // 지면에 있을 때 verticalVelocity 값이 무한히 마이너스로 누적되는 것 방지
            if (actor.verticalVelocity < 0.0f)
                actor.verticalVelocity = -2f;

            // 점프 입력 처리는 이제 OnJumpEvent() 콜백에서 알아서 실행되므로 여기서 매 프레임 검사하지 않습니다.

            // 공중 상태 전환 조건을 모두 통과했다면 지상 이동 처리
            actor.Move();
        }

        public override void PhysicsUpdate()
        {

        }

        // --- 이벤트 기반 콜백 ---
        public override void OnJumpEvent()
        {
            // 점프 높이만큼 속도를 지정
            actor.verticalVelocity = Mathf.Sqrt(actor.JumpHeiht * -2.0f * actor.gravity);

            // 공중 상태로 전환
            actor.ChangeState(actor.AirState);
        }
    }
}