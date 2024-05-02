using Sandbox;

public sealed class Ids : Component
{
	[Property] public List<string> Categories {get;set;} = new List<string>();
	[Property] public string sceneID {get;set;}
}