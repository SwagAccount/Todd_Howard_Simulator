using System;

using Sandbox;

public sealed class CommandDealer : Component
{
	[Property] public int sceneIdSelected {get;set;}
	[Property] public GameObject player {get;set;}
	[Property] public GameObject Saved {get;set;}
	[Property] public int nextCode {get;set;}
	bool inCmd;
	[Property] public CommandInterface commandInterface {get;set;}

	[Property] public PlayerController playerController {get;set;}
	[Property] public ContainerInteract containerInteract {get;set;}

	protected override void OnAwake()
	{
		commandInterface = Components.Create<CommandInterface>();
		FindPlayer();
		
	}
	public void FindPlayer()
	{
		player = Saved.Children[0];
	}
	public float lastTime = 0;
	public bool Colour = true;
	public static void DebugLog(string log)
	{
		CommandDealer commandDealer = getCommandDealer();

		if(commandDealer.lastTime != Time.Now)
		{
			commandDealer.Colour = !commandDealer.Colour;
		}
		commandDealer.lastTime = Time.Now;

		Log.Info(log);

		CommandInterface commandInterface = commandDealer.commandInterface;
		commandInterface.Logs.Add(log);
		commandInterface.Colour.Add(commandDealer.Colour);
		commandInterface.update++;
		

	}
	bool lastCmd;
	protected override void OnUpdate()
	{
		if(playerController == null)
		{
			playerController = player.Components.Get<PlayerController>();
			containerInteract = player.Components.Get<ContainerInteract>();
			if(playerController == null) return;
		}
		if(Input.Pressed("Open Console"))
		{
			inCmd = !inCmd;
			commandInterface.update++;
		}
		if(inCmd)
		{
			commandInterface.enable = true;
			playerController.weaponScript.canShoot = false;
			commandInterface.update++;
		}
		else if (lastCmd != inCmd)
		{
			commandInterface.enable = false;
			playerController.weaponScript.canShoot = true;
		}
		
		lastCmd = inCmd;
	}
	public void FindEntity(int entityID)
	{
		
	}
	public int CalculatePrice(Entity entity)
	{
		item item = CustomFunctions.GetResource<item>(entity.Ids.Categories,"item");
		int price = item.Value;
		foreach(SaveClasses.EntitySave entitySave in entity.Container)
		{
			item item1 = CustomFunctions.GetResource<item>(entitySave.Categories,"item");
			price += GetSavedPrice(entitySave);
		}
		return price;
	}
	public int GetSavedPrice(SaveClasses.EntitySave entity)
	{
		item item = CustomFunctions.GetResource<item>(entity.Categories,"item");
		int price = item.Value;
		foreach(SaveClasses.EntitySave entitySave in entity.Container)
		{
			item item1 = CustomFunctions.GetResource<item>(entitySave.Categories,"item");
			price += GetSavedPrice(entitySave);
		}
		return price;
	}
	public void TransferItem(Entity from, Entity to, int id)
	{
		
		if(from.Container == null) from.Container = new List<SaveClasses.EntitySave>();
		if(from.Equips == null) from.Equips = new List<Entity.Equiped>();
		
		if(to.Container == null) to.Container = new List<SaveClasses.EntitySave>();
		if(to.Equips == null) to.Equips = new List<Entity.Equiped>();
		
		for (int i = 0; i < from.Container.Count; i++)
		{
			
			if(from.Container[i].id == id) 
			{
				for(int I = 0; I < from.Equips.Count; I++)
				{
					if(from.Equips[I].ID == id)
					{
						from.Equips.RemoveAt(I);
						break;
					}
				}
				to.Container.Add(from.Container[i]);
				from.Container.RemoveAt(i);
				to.ContainerUpdated = Time.Now;
				from.ContainerUpdated = Time.Now;
				return;
			}
		}

		
		
	}

	public void AddItem(Entity item, Entity to)
	{
		to.Container.Add(CustomFunctions.SaveEntity(item));
		item.GameObject.Destroy();
		to.ContainerUpdated = Time.Now;
	}
	public void RemoveItem(Entity entity, int id)
	{
		for (int i = 0; i < entity.Container.Count; i++)
		{
			
			if(entity.Container[i].id == id) 
			{
				entity.Container.RemoveAt(i);
				entity.ContainerUpdated = Time.Now;
				return;
			}
		}
	}
	public void SetAV(string name, string value, string attributeSet = "default")
	{
		foreach(GameObject g in Saved.Children)
		{
			Ids ids = g.Components.Get<Ids>();
			if(ids != null)
			{
				if(ids.sceneID == sceneIdSelected)
				{
					Attributes attributes = ids.Components.Get<Attributes>();
					if(attributes != null)
					{
						Attributes.Attribute attribute = attributes.getAttribute(name, attributeSet);
						if(attribute != null)
						{
							var set = attributes.ConvertFromString(value, attribute.attributeType);
							attribute.SetValue(set);
							DebugLog($"{name} => {set}");
						}
					}
					return;
				}
			}
		}
		DebugLog("Failed To Retrieve ID");
	}
	public void GetAV(string name,string attributeSet = "default")
	{
		foreach(GameObject g in Saved.Children)
		{
			Ids ids = g.Components.Get<Ids>();
			if(ids != null)
			{
				if(ids.sceneID == sceneIdSelected)
				{
					Attributes attributes = ids.Components.Get<Attributes>();
					if(attributes != null)
					{
						Attributes.Attribute attribute = attributes.getAttribute(name, attributeSet);
						if(attribute != null)
						{
							DebugLog($"{name} | {attribute.attributeType} => {attribute.GetValue()}");
						}
					}
					return;
				}
			}
		}
		DebugLog("Failed To Retrieve ID");
	}
	public void GetAllAV()
	{
		foreach(GameObject g in Saved.Children)
		{
			Ids ids = g.Components.Get<Ids>();
			if(ids != null)
			{
				if(ids.sceneID == sceneIdSelected)
				{
					Attributes attributes = ids.Components.Get<Attributes>();
					foreach(Attributes.AttributeSet attributeSet in attributes.attributeSets)
					{
						DebugLog($"┌──{attributeSet.setName}───");
						foreach(Attributes.Attribute attribute in attributeSet.attributes)
						{
							DebugLog($"├{attribute.AttributeName} | {attribute.attributeType} => {attribute.GetValue()}");
						}
						DebugLog($"└─────");
						
					}
					return;
				}
			}
		}
		DebugLog("Failed To Retrieve ID");
	}
	public void AddAV( string value, Attributes.Attribute.AttributeType attributeType,string name,string attributeSet = "default")
	{
		foreach(GameObject g in Saved.Children)
		{
			Ids ids = g.Components.Get<Ids>();
			if(ids != null)
			{
				if(ids.sceneID == sceneIdSelected)
				{
					Attributes attributes = ids.Components.Get<Attributes>();
					if(attributes != null)
					{
						attributes.AddAttribute( value, attributeType,name, attributeSet);
						DebugLog($"{name} (Added) => {value}");
					}
					return;
				}
			}
		}
		DebugLog("Failed To Retrieve ID");
	}
	public void SetHeatlh(float set)
	{
		foreach(GameObject g in Saved.Children)
		{
			Ids ids = g.Components.Get<Ids>();
			if(ids != null)
			{
				if(ids.sceneID == sceneIdSelected)
				{
					Attributes attributes = ids.Components.Get<Attributes>();
					if(attributes != null)
					{
						Attributes.Attribute attribute = attributes.getAttribute("health");
						if(attribute != null)
						{
							attribute.floatValue = set;
							DebugLog($"Health => {set}");
						}
					}
					return;
				}
			}
		}
		DebugLog("Failed To Retrieve ID");
	}
	public void RecalculatePerkEffects()
	{
		foreach(GameObject g in Saved.Children)
		{
			Attributes attributes = g.Components.Get<Attributes>();
			attributes.ApplyPerksToSets();
		}
	}
	public void ResetHeatlh()
	{
		foreach(GameObject g in Saved.Children)
		{
			Ids ids = g.Components.Get<Ids>();
			if(ids != null)
			{
				if(ids.sceneID == sceneIdSelected)
				{
					Attributes attributes = ids.Components.Get<Attributes>();
					if(attributes != null)
					{
						Attributes.Attribute health = attributes.getAttribute("health");
						Attributes.Attribute maxhealth = attributes.getAttribute("maxhealth");
						if( health != null)
						{
							health.floatValue = maxhealth.floatValue;
							DebugLog($"Health => {maxhealth.floatValue}");
						}
					}
					break;
				}
			}
		}
	}

	public void SelectID(int id)
	{
		int i = 0;
		foreach(GameObject g in Saved.Children)
		{
			Ids ids = g.Components.Get<Ids>();
			if(ids != null)
			{
				if(ids.sceneID == id)
				{
					sceneIdSelected = i;
					return;
				}
			}
			i++;
		}
		DebugLog("Did not find ID.");
	}
	public Entity GetEntity(int id)
	{
		int i = 0;
		foreach(GameObject g in Saved.Children)
		{
			Ids ids = g.Components.Get<Ids>();
			if(ids != null)
			{
				if(ids.sceneID == id)
				{
					return Saved.Children[i].Components.Get<Entity>();
				}
			}
			i++;
		}
		
		DebugLog("Did not find ID.");
		return null;
	}

	public void SeeAllIDs()
	{
		DebugLog($"┌─────");
		foreach(GameObject c in Saved.Children)
		{
			Ids ids = c.Components.Get<Ids>();
			DebugLog($"├ {ids.sceneID}: {CustomFunctions.CapitalizeWords(ids.Categories[ids.Categories.Count - 1])}");
		}
		DebugLog($"└─────");
	}

	public static void ValueExamples()
	{
		DebugLog("---Value Examples---");
		DebugLog("INT - 1");
		DebugLog("FLOAT - 1.5");
		DebugLog("STRING - {Anything}");
		DebugLog("VECTOR3 - X,Y,Z");
		DebugLog("BOOL - True/False (anything not 'True' = False)");
		DebugLog("------");
	}
	[ConCmd( "_Help" )]
	public static void Help()
	{
		List<string> Commands = new List<string>
		{
			"CalcDis",
			"CalcDamage",
			"SeeAllIDs",
			"SelectID",
			"SetAv",
			"GetAllAV",
			"GetAV",
			"AddAV",
			"SetHealth",
			"ResetHealth",
			"RecalculatePerkEffects"
		};
		Commands.Sort();

		DebugLog("To get more details on commands, run the command name with added _Help");
		foreach(string s in Commands)
		{
			DebugLog(s);
		}
	}

	[ConCmd( "CalcDis" )]
	public static void CalcDis(float recoilAngle, float bulletSpread, float targetRadius, float desiredHitProbability, bool auto)
	{
		DebugLog($"{CustomFunctions.CalculateEffectiveDistance(recoilAngle,bulletSpread,targetRadius,desiredHitProbability, auto)}");
	}

	

	[ConCmd( "CalcDamage" )]
	public static void CalcDamage(float velocity, float weight, float diameter)
	{
		DebugLog($"{MathF.Pow(weight,2f)*velocity/(700000*MathF.Pow(diameter,2f))*0.06}");
	}
	[ConCmd( "CalcDamage_Help" )]
	public static void CalcDamageHelp()
	{
		DebugLog("Dev command to calculate the damage of a gun");
		DebugLog("Command Structure: Veclocity(Ft/S) Weight(Grains) Diameter(Inches)");
		DebugLog("");
		DebugLog("(Most guns damage is calcualted using this, the reason it is a command is because sometimes I might want to set a guns damage differently)");
	}

	[ConCmd( "SeeAllIDs" )]
	public static void SelectIDsTo()
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.SeeAllIDs();
	}

	[ConCmd( "SelectID" )]
	public static void SelectIDTo(int id)
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.SelectID(id);
	}
	[ConCmd( "SelectID_Help" )]
	public static void SelectIDHelp()
	{
		DebugLog("Selects an ID to peform commands on.");
		DebugLog("Command Structure: Id");
		DebugLog("");
		DebugLog("To see all IDs use SeeAllIDs");
	}

	[ConCmd( "SetAV" )]
	public static void SetAVTo(string setName, string name , string value)
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.SetAV(name,value,setName);
	}
	[ConCmd( "SetAV_Help" )]
	public static void SetAVHelp()
	{
		DebugLog("Change value of an entitys attribute.");
		DebugLog("Command Structure: setName(most likely default), attribute name, value");
		ValueExamples();
		
		DebugLog("To get the value type and info use GetAV");
	}
	[ConCmd( "GetAllAV" )]
	public static void GetAllAVTo()
	{
		
		CommandDealer commandDealer = getCommandDealer();;
		commandDealer.GetAllAV();
	}
	[ConCmd( "GetAV" )]
	public static void GetAVTo(string setName, string name)
	{
		
		CommandDealer commandDealer = getCommandDealer();;
		commandDealer.GetAV(name, setName);
	}
	[ConCmd( "GetAV_Help" )]
	public static void GetAVHelp()
	{
		DebugLog("Returns the value and type of an attribute");
		DebugLog("Command Structure: setName(most likely default), attribute name");
		DebugLog("To see all attributes of an entity use GetAllAV");
	}
	[ConCmd( "AddAV" )]
	public static void AddAVTo(string attributeSet, string name, Attributes.Attribute.AttributeType attributeType,string value)
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.AddAV(value,attributeType,name,attributeSet);
	}
	[ConCmd( "AddAV_Help" )]
	public static void AddAVHelp()
	{
		CommandDealer commandDealer = getCommandDealer();
		DebugLog("Adds an attribute to an attribute set.");
		DebugLog("Command Structure: setName(most likely default), attribute name, attribute type, value");
		ValueExamples();
	}
	[ConCmd( "SetHealth" )]
	public static void SetHeatlhTo( float set )
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.SetHeatlh(set);
	}
	[ConCmd( "SetHealth_Help" )]
	public static void SetHeatlhHelp()
	{
		DebugLog("Sets the health of the selected ID");
		DebugLog("Command Structure: Value");
	}
	[ConCmd( "ResetHealth" )]
	public static void ResetHeatlhTo()
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.ResetHeatlh();
	}
	[ConCmd( "SetHealth_Help" )]
	public static void ResetHeatlhHelp()
	{
		DebugLog("Resets health of selected ID to the max health");
	}
	[ConCmd( "RecalculatePerkEffects" )]
	public static void RecalculatePerkEffectsTo()
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.RecalculatePerkEffects();
	}
	[ConCmd( "RecalculatePerkEffects_Help" )]
	public static void RecalculatePerkEffectsHelp()
	{
		DebugLog("Recalculates the perk effects for everything");
	}

	public static CommandDealer getCommandDealer()
	{
		if(Game.ActiveScene == null) throw new Exception("Did Not Grab Scene");
		CommandDealer commandDealer = Game.ActiveScene.GetAllObjects(true).ElementAt<GameObject>(1).Components.Get<CommandDealer>();
		if(commandDealer == null) throw new Exception("Failed To Grab Command Dealer");
		return commandDealer;
	}
}