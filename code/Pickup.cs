using Sandbox;

public sealed class Pickup : Component
{
	private Interactable interactable;
	private Attributes attributes;
	private Entity entity;
	private Entity player;
	protected override void OnStart()
	{
		player = CommandDealer.getCommandDealer().player.Components.Get<Entity>();
		entity = Components.Get<Entity>();
		interactable = Components.Get<Interactable>();
	}
	protected override void OnFixedUpdate()
	{
		if(interactable.Interacted)
		{
			interactable.Interacted = false;
			player.Container.Add(CustomFunctions.SaveEntity(entity));
			GameObject.Destroy();
		}
	}
}