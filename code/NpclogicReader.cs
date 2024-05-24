using System;
using Sandbox;

public sealed class NpclogicReader : Component
{
	[Property] public Action action {get;set;}
	[Property] public NPCLogic NPCLogic {get;set;}
	protected override void OnUpdate()
	{
		action = NPCLogic.action;
		action.Invoke();
	}
}
