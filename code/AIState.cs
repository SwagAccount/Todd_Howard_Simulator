using System.Collections;
using System.Collections.Generic;
using Sandbox;

public enum AIStateID 
{
    Patrol,
    ShootPlayer,
    Idle
}
public interface AIState 
{
    AIStateID GetID();
    void Enter(Aiagent agent);
    void Update(Aiagent agent);
    void Exit(Aiagent agent);
}