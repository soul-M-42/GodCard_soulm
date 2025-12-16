public abstract class GameState
{
    protected GameStateMachine stateMachine;

    protected GameState(GameStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
