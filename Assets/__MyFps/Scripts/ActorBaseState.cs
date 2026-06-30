namespace MyFps
{
    public abstract class ActorBaseState
    {
        protected Actor actor;

        public ActorBaseState(Actor actor)
        {
            this.actor = actor;
        }

        public abstract void Enter();
        public abstract void LogicUpdate();
        public abstract void PhysicsUpdate();
        public abstract void Exit();

        // 입력 이벤트 콜백
        public virtual void OnJumpEvent() { }
    }
}