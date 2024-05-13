using Sandbox;

public sealed class Inventory : Component
{
	private PlayerController playerController;	
	private Entity playerEntity;	
	private WeaponScript weaponScript;	
	private ContainerUI containerUI;	
	protected override void OnStart()
	{
		containerUI = Components.Create<ContainerUI>();
		containerUI.Position = "center";
		containerUI.Width = 31;
		containerUI.Height = 75;
		
		containerUI.EnableMouse = true;
	}
	bool inInv;
	bool lastInInv;
	protected override async void OnUpdate()
	{
		if(weaponScript == null || playerController == null)
		{
			playerController = Components.Get<PlayerController>();
			playerEntity = Components.Get<Entity>();
			weaponScript = GameObject.Children[0].Children[0].Components.Get<WeaponScript>();
			return;
		}
		containerUI.Entity = playerEntity;
		weaponScript.inInv = inInv;
		if(Input.Pressed("Menu")) 
		{
			inInv = !inInv;
			await Task.DelaySeconds(0.1f);
		}

		if(inInv)
		{
			playerController.Enabled = false;
			if(!lastInInv)
			{
				containerUI.CalculateList();
			}
			if(!weaponScript.pauseGun)
			{
				containerUI.Enabled = true;
			}
			else
			{
				containerUI.Enabled = false;
			}

			if(containerUI.MoveID != -1)
			{
				int containerItemIndex = playerEntity.getContainerItem(containerUI.MoveID);
				if(playerEntity.ItemEquipped(playerEntity.Container[containerItemIndex]))
				{
					playerEntity.UnEquipItem(playerEntity.Container[containerItemIndex]);
				}
				else
				{
					playerEntity.EquipItem(playerEntity.Container[containerItemIndex]);
				}
				containerUI.MoveID = -1;
			}
		}
		else
		{
			if(lastInInv)
			{
				
			}
			playerController.Enabled = true;
			containerUI.Enabled = false;
		}
		lastInInv = inInv;
	}
}