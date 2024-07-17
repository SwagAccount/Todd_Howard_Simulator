using System;
using Sandbox;

public sealed class ContainerInteract : Component
{
	
	private ContainerUI from;
	private ContainerUI to;

	private BarterUI barter;
	private PlayerController playerController;
	private Interactor interactor;
	private CommandDealer commandDealer;

	private List<SaveClasses.EntitySave> barterChestStart;
	private Attributes.Attribute barterBux;
	private Attributes.Attribute playerBux;
	[Property] public Entity LookedAtContainer {get; set;}

	[Property] public States State {get; set;}

	public void changeLookedAt(Entity entity)
	{
		LookedAtContainer = entity;
		to.CalculateList();
		from.CalculateList();
	}

	public enum States
	{
		None,
		Look,
		Transfer, 
		Barter
	}
	protected override void OnStart()
	{
		playerBux = Components.Get<Attributes>().getAttribute("s&bux");
		commandDealer = CommandDealer.getCommandDealer();
		barter = Components.Create<BarterUI>();
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
		
		playerController = Components.Get<PlayerController>();
		
		interactor = playerController.Camera.Components.Get<Interactor>();
	}
	void Transfer()
	{
		barter.Price = 0;
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
	}
	public void StartBarter(Entity with)
	{
		to.Entity = commandDealer.GetEntity(with.Attributes.getAttribute("ContainerID","Vendor").intValue);
		barterChestStart = new List<SaveClasses.EntitySave>(to.Entity.Container);
		State = States.Barter;
	}
	void Barter()
	{
		if(from.MoveID != -1)
		{
			
			bool inCurrentTrade = false;
			for (int i = 0; i < barterChestStart.Count; i++)
			{
				
				if(barterChestStart[i].id == from.MoveID) 
				{
					inCurrentTrade = true;
					break;
				}
			}


			int price = 0;
			for (int i = 0; i < from.Entity.Container.Count; i++)
			{
				if(from.Entity.Container[i].id == from.MoveID) 
				{
					price = commandDealer.GetSavedPrice(from.Entity.Container[i]);
					break;
				}
			}
			commandDealer.TransferItem(from.Entity, to.Entity, from.MoveID);
			
			playerBux.intValue += price / (inCurrentTrade ? 1 : 2);
			barterBux.intValue -= price / (inCurrentTrade ? 1 : 2);
			barter.Price -= price / (inCurrentTrade ? 1 : 2);

			from.MoveID = -1;
			to.CalculateList();
			from.CalculateList();
		}
		if(to.MoveID != -1)
		{
			
			int price = 0;
			for (int i = 0; i < to.Entity.Container.Count; i++)
			{
				
				if(to.Entity.Container[i].id == to.MoveID) 
				{
					price = commandDealer.GetSavedPrice(to.Entity.Container[i]);
					break;
				}
			}
			commandDealer.TransferItem(to.Entity, from.Entity, to.MoveID);
			playerBux.intValue -= price;
			barterBux.intValue += price;
			barter.Price += price;

			to.MoveID = -1;
			to.CalculateList();
			from.CalculateList();
		}
	}

	protected override void OnUpdate()
	{

		switch(State)
		{
			case States.None:
				to.Entity = LookedAtContainer;
				playerController.Enabled = true;
				interactor.Enabled = true;
				from.Enabled = false;
				to.Enabled = false;
				barter.Enabled = false;
				playerBux = null;
				barterBux = null;
				Transfer();
				break;
			case States.Look:
				to.Entity = LookedAtContainer;
				playerBux = null;
				barterBux = null;
				to.EnableMouse = false;
				playerController.Enabled = true;
				interactor.Enabled = true;
				from.Enabled = false;
				to.Enabled = true;
				barter.Enabled = false;
				to.SortIndex = 0;
				to.SortDirection = -1;
				Transfer();
				break;
			case States.Transfer:
				to.Entity = LookedAtContainer;
				playerBux = null;
				barterBux = null;
				playerController.Enabled = false;
				interactor.Enabled = false;
				from.Enabled = true;
				to.Enabled = true;
				to.EnableMouse = true;
				barter.Enabled = false;
				Transfer();
				if(Input.Pressed("Score"))
				{
					State = States.None;
				}
				break;
			case States.Barter:
				
				playerController.Enabled = false;
				interactor.Enabled = false;
				from.Enabled = true;
				to.Enabled = true;
				to.EnableMouse = true;
				barter.Enabled = true;
				if(playerBux == null)
				{
					playerBux = from.Entity.Attributes.getAttribute("s&bux");
					barterBux = LookedAtContainer.Attributes.getAttribute("s&bux");
					
				}
				barter.You = playerBux.intValue;
				barter.Vendor = barterBux.intValue;
				Barter();
				if(Input.Pressed("Score") && barterBux.intValue >= 0 && playerBux.intValue >= 0)
				{
					State = States.None;
				}
				break;
		}
	}
}