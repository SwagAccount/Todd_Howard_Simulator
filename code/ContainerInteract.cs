using Sandbox;

public sealed class ContainerInteract : Component
{
	private ContainerUI from;
	private ContainerUI to;
	private PlayerController playerController;
	private Interactor interactor;
	private CommandDealer commandDealer;
	[Property] public Entity LookedAtContainer {get; set;}
	[Property] public States State {get; set;}

	public enum States
	{
		None,
		Look,
		Transfer
	}
	protected override void OnStart()
	{
		commandDealer = CommandDealer.getCommandDealer();
		from = Components.Create<ContainerUI>();
		from.EnableMouse = true;
		from.Entity = Components.Get<Entity>();
		to = Components.Create<ContainerUI>();
		from.Enabled = false;
		to.Enabled = false;
		from.Position = "flex-start";
		to.Position = "flex-end";
		from.Width = 50;
		from.TableSize = 60;
		to.Width = 50;
		to.TableSize = 60;
		foreach(GameObject child in GameObject.Children)
		{
			if(child.Name == "Camera") interactor = child.Components.Get<Interactor>();
		}
		playerController = Components.Get<PlayerController>();
	}

	protected override void OnUpdate()
	{
		if(from.MoveID != -1)
		{
			commandDealer.TransferItem(from.Entity, to.Entity, from.MoveID);
			from.MoveID = -1;
			to.CalculateList();
			from.CalculateList();
		}
		if(to.MoveID != -1)
		{
			commandDealer.TransferItem(to.Entity, from.Entity, to.MoveID);
			to.MoveID = -1;
			to.CalculateList();
			from.CalculateList();
		}

		to.Entity = LookedAtContainer;
		switch(State)
		{
			case States.None:
				playerController.Enabled = true;
				interactor.Enabled = true;
				from.Enabled = false;
				to.Enabled = false;
				
				break;
			case States.Look:
				
				to.EnableMouse = false;
				playerController.Enabled = true;
				interactor.Enabled = true;
				from.Enabled = false;
				to.Enabled = true;
				to.SortIndex = 0;
				to.SortDirection = -1;
				
				break;
			case States.Transfer:
				
				playerController.Enabled = false;
				interactor.Enabled = false;
				from.Enabled = true;
				to.Enabled = true;
				to.EnableMouse = true;
				if(Input.Pressed("Score"))
				{
					State = States.None;
				}
				break;
		}
	}
}