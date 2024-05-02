using Sandbox;

public sealed class Interactable : Component
{
	[Property] public bool Interacted {get;set;}
	[Property] public bool LookedAt {get;set;}
}