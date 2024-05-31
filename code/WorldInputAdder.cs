using Sandbox;

public sealed class WorldInputAdder : Component
{
	protected override void OnStart()
	{
		WorldInput worldInput = Components.Create<WorldInput>();
		worldInput.LeftMouseAction = "Use";
	}
}
