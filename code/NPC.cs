using System;
using Sandbox;
using Sandbox.ActionGraphs;

public sealed class NPC : Component
{
	public GameObject player {get;set;}
	public NavMeshAgent agent {get;set;}
	protected override void OnStart()
	{

	}
	protected override void OnUpdate()
	{

	}

	public void GoTo(Vector3 position)
	{
		agent.MoveTo(position);
	}
}
