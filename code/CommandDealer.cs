using System;
using Sandbox;

public sealed class CommandDealer : Component
{
	[Property] public string sceneIdSelected {get;set;}
	[Property] public Attributes player {get;set;}
	[Property] public GameObject Saved {get;set;}
	[Property] public int nextCode {get;set;}

	protected override void OnAwake()
	{
		foreach(GameObject c in Saved.Children)
		{
			Ids ids = c.Components.Get<Ids>();
			nextCode++;
			ids.sceneID = nextCode.ToString();
		}
	}

	public void DropItem(Vector3 postion, Angles rotation, SaveClasses.EntitySave entitySave)
	{
		GameObject g = new GameObject();
		g.SetParent(Saved);
		g.Transform.Position = postion;
		g.Transform.Rotation = rotation;
		Ids ids = g.Components.Create<Ids>();
		nextCode++;
		ids.sceneID = nextCode.ToString();
		ids.Categories = entitySave.Categories;
		Entity entity = g.Components.Create<Entity>();
		entity.displayContainer = entitySave.displayContainer;
		entity.Container = entitySave.Container;
		Attributes attributes = g.Components.Create<Attributes>();
		attributes.attributeSets = new List<Attributes.AttributeSet>();
		foreach(Attributes.SavedAttributeSet sAS in entitySave.AttributeSets)
		{
			attributes.attributeSets.Add(Attributes.LoadAttributeSet(sAS));
		}
		attributes.player = player;
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

	public void SelectID(string id)
	{
		foreach(GameObject g in Saved.Children)
		{
			Ids ids = g.Components.Get<Ids>();
			if(ids != null)
			{
				if(ids.sceneID == id)
				{
					sceneIdSelected = id;
					return;
				}
			}
		}
		Log.Info("Did not find ID.");
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
	[ConCmd( "SeeAllIDs" )]
	public static void SelectIDsTo()
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.SeeAllIDs();
	}

	[ConCmd( "SelectID" )]
	public static void SelectIDTo(string id)
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