using Sandbox;

public sealed class CompletedThings : Component
{
	[Property] private List<CompletionGroup> CompletionGroups {get;set;} 
	protected override void OnUpdate()
	{

	}

	public class CompletionGroup
	{
		[KeyProperty][Property] public string GroupName {get;set;}
		[Property] public List<string> Completed {get;set;}
	}
}
	