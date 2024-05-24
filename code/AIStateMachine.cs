public class AIStateMachine
{
    public List<AIState> states;
    public Aiagent agent;
    public AIStateID currentState;

    public AIStateMachine(Aiagent agent) 
    {
        this.agent = agent;
        int numStates = System.Enum.GetNames<AIStateID>().Length;
        states = new List<AIState>(numStates);
    }

    public void RegisterState(AIState state)
    {
        int index = (int)state.GetID();
        states[index] = state;
    }

    public AIState GetState(AIStateID stateID)
    {
        int index = (int)stateID;
        return states[index];
    }

    public void Update()
    {
        GetState(currentState)?.Update(agent);
    }

    public void ChangeState(AIStateID newState)
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }
}