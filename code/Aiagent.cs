using Sandbox;

public sealed class Aiagent : Component
{
	public AIStateMachine stateMachine;
	[Property] public AIStateID initialState {get;set;}
	protected override void OnStart()
	{
		stateMachine = new AIStateMachine(this);
		stateMachine.ChangeState(initialState);
	}
	protected override void OnUpdate()
	{
		stateMachine.Update();
	}
}
