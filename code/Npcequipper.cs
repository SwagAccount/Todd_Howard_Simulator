using Microsoft.VisualBasic;
using Sandbox;

public sealed class Npcequipper : Component
{
	private Aiagent agent;
	public bool equipWeapon;
	protected override void OnStart()
	{
		agent = Components.Get<Aiagent>();
		UpdateEquips();
	}

	protected override void OnUpdate()
	{
		if(agent.weapon.Entity.Update())
		{
			UpdateEquips();
		}
	}
	public void UpdateEquips()
	{
		int weaponEquip = -1;
		float weaponDPS = 0;
		int apparelEquip = -1;
		//int apparelWHATEVER;
		for(int i = 0; i < agent.weapon.Entity.Container.Count; i++)
		{
			SaveClasses.EntitySave entitySave = agent.weapon.Entity.Container[i];
			if(entitySave.Categories.Contains("weapons"))
			{
				Weapon weapon = CustomFunctions.GetResource<Weapon>(entitySave.Categories, "weapon");
				float newWeaponDPS = (1/weapon.shootTime)*weapon.bulletStats[0].damage*weapon.bulletStats[0].shotsPer;
				if(newWeaponDPS > weaponDPS)
				{
					weaponDPS = newWeaponDPS;
					weaponEquip = i;
				}
			}
			if(entitySave.Categories.Contains("apparel"))
			{
				apparelEquip = i;
			}
		}

		agent.weapon.Entity.Equips = new List<Entity.Equiped>();
		if(weaponEquip!=-1 && equipWeapon)
		{
			
			Entity.Equiped equiped = new Entity.Equiped
			{
				ID = agent.weapon.Entity.Container[weaponEquip].id,
				equipType = "weapons"
			};
			
			agent.weapon.Entity.Equips.Add(equiped);

			agent.weapon.GunChange();
		} 
		if(apparelEquip!=-1)
		{
			Entity.Equiped equiped = new Entity.Equiped
			{
				ID = agent.weapon.Entity.Container[apparelEquip].id,
				equipType = "apparel"
			};
			agent.weapon.Entity.Equips.Add(equiped);
			agent.npcmodelAnimationManager.Clothes = agent.weapon.Entity.Container[apparelEquip];
		} 
		else
		{
			agent.npcmodelAnimationManager.Clothes = new SaveClasses.EntitySave
			{
				Categories = new List<string>()
			};
		}
	}
}
