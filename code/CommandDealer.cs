using System;

using Sandbox;

public sealed class CommandDealer : Component
{
	[Property] public int sceneIdSelected {get;set;}
	[Property] public GameObject player {get;set;}
	[Property] public GameObject Saved {get;set;}
	[Property] public int nextCode {get;set;}

	protected override void OnAwake()
	{
		player = Saved.Children[0];
		foreach(GameObject c in Saved.Children)
		{
			Ids ids = c.Components.Get<Ids>();
			nextCode++;
			ids.sceneID = nextCode;
		}
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
							Log.Info($"{name} => {set}");
						}
					}
					return;
				}
			}
		}
		Log.Info("Failed To Retrieve ID");
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
							Log.Info($"{name} | {attribute.attributeType} => {attribute.GetValue()}");
						}
					}
					return;
				}
			}
		}
		Log.Info("Failed To Retrieve ID");
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
						Log.Info($"┌──{attributeSet.setName}───");
						foreach(Attributes.Attribute attribute in attributeSet.attributes)
						{
							Log.Info($"├{attribute.AttributeName} | {attribute.attributeType} => {attribute.GetValue()}");
						}
						Log.Info($"└─────");
						
					}
					return;
				}
			}
		}
		Log.Info("Failed To Retrieve ID");
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
						Log.Info($"{name} (Added) => {value}");
					}
					return;
				}
			}
		}
		Log.Info("Failed To Retrieve ID");
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
							Log.Info($"Health => {set}");
						}
					}
					return;
				}
			}
		}
		Log.Info("Failed To Retrieve ID");
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
							Log.Info($"Health => {maxhealth.floatValue}");
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
		Log.Info("Did not find ID.");
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
		
		Log.Info("Did not find ID.");
		return null;
	}

	public void SeeAllIDs()
	{
		Log.Info($"┌─────");
		foreach(GameObject c in Saved.Children)
		{
			Ids ids = c.Components.Get<Ids>();
			Log.Info($"├ {ids.sceneID}: {CustomFunctions.CapitalizeWords(ids.Categories[ids.Categories.Count - 1])}");
		}
		Log.Info($"└─────");
	}

	public static void ValueExamples()
	{
		Log.Info("---Value Examples---");
		Log.Info("INT - 1");
		Log.Info("FLOAT - 1.5");
		Log.Info("STRING - {Anything}");
		Log.Info("VECTOR3 - X,Y,Z");
		Log.Info("BOOL - True/False (anything not 'True' = False)");
		Log.Info("------");
	}

	[ConCmd( "CalcDis" )]
	public static void CalcDis(float recoilAngle, float bulletSpread, float targetRadius, float desiredHitProbability, bool auto)
	{
		Log.Info(CustomFunctions.CalculateEffectiveDistance(recoilAngle,bulletSpread,targetRadius,desiredHitProbability, auto));
	}

	[ConCmd( "CalcDamage" )]
	public static void CalcDamage(float velocity, float weight, float diameter)
	{
		Log.Info(MathF.Pow(weight,2f)*velocity/(700000*MathF.Pow(diameter,2f))*0.06);
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
		Log.Info("Selects an ID to peform commands on.");
		Log.Info("Command Structure: Id");
		Log.Info("");
		Log.Info("To see all IDs use SeeAllIDs");
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
		Log.Info("Change value of an entitys attribute.");
		Log.Info("Command Structure: setName(most likely default), attribute name, value");
		ValueExamples();
		
		Log.Info("To get the value type and info use GetAV");
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
		Log.Info("Returns the value and type of an attribute");
		Log.Info("Command Structure: setName(most likely default), attribute name");
		Log.Info("To see all attributes of an entity use GetAllAV");
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
		Log.Info("Adds an attribute to an attribute set.");
		Log.Info("Command Structure: setName(most likely default), attribute name, attribute type, value");
		ValueExamples();
	}
	[ConCmd( "SetHealth" )]
	public static void SetHeatlhTo( float set )
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.SetHeatlh(set);
	}
	[ConCmd( "ResetHealth" )]
	public static void ResetHeatlhTo()
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.ResetHeatlh();
	}
	[ConCmd( "RecalculatePerkEffects" )]
	public static void RecalculatePerkEffectsTo()
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.RecalculatePerkEffects();
	}
	
	public static CommandDealer getCommandDealer()
	{
		if(Game.ActiveScene == null) throw new Exception("Did Not Grab Scene");
		CommandDealer commandDealer = Game.ActiveScene.GetAllObjects(true).ElementAt<GameObject>(1).Components.Get<CommandDealer>();
		if(commandDealer == null) throw new Exception("Failed To Grab Command Dealer");
		return commandDealer;
	}
}