using System;

public class AIStateMachine
{
    public AIState[] states;
    public Aiagent agent;
    [Property] public AIStateID currentState {get;set;}

    public AIStateMachine(Aiagent agent) 
    {
        
        this.agent = agent;
        
        int numStates = Enum.GetNames<AIStateID>().Length;

        states = new AIState[numStates];

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

    public void ChangeState(AIStateID newState, bool Override = false)
    {
        if(newState == currentState && !Override) return;
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }
}