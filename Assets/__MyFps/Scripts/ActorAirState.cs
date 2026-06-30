using UnityEngine;

namespace MyFps
{
    public class ActorAirState : ActorBaseState
    {
        public ActorAirState(Actor actor) : base(actor)
        {
        }

        public override void Enter()
        {
            // 필요하다면 공중 상태 진입 시 애니메이션 설정 등을 할 수 있습니다.
        }

        public override void Exit()
        {
        }

        public override void LogicUpdate()
        {
            actor.CheckGrounded();

            // 바닥에 닿았다면 지상 상태로 전환
            if (actor.isGrounded)
            {
                actor.ChangeState(actor.GroundState);
                return; // 상태 전환 시 아래 코드를 실행하지 않고 리턴
            }

            // 중력 적용 (공중에 있을 때 매 프레임 적용)
            actor.verticalVelocity += actor.gravity * Time.deltaTime;

            // 공중에서도 이동이 가능하게 Move() 호출
            actor.Move();
        }

        public override void PhysicsUpdate()
        {
        }
    }
}
