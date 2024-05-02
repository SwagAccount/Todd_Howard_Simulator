using System;
using Sandbox;

public sealed class CommandDealer : Component
{
	[Property] public string sceneIdSelected {get;set;}
	[Property] public Attributes player {get;set;}
	[Property] public GameObject Saved {get;set;}
	
	public string SetAV(string name, string value, string attributeSet = "default")
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
					break;
				}
			}
		}
		return "Failed To Retrieve ID";
	}
	public string GetAV(string name,string attributeSet = "default")
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
							Log.Info($"{name} => {attribute.GetValue()}");
						}
					}
					break;
				}
			}
		}
		return "Failed To Retrieve ID";
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
					break;
				}
			}
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

	[ConCmd( "SetAV" )]
	public static void SetAVTo(string setName, string name , string value)
	{
		CommandDealer commandDealer = getCommandDealer();
		commandDealer.SetAV(name,value,setName);
	}
	[ConCmd( "GetAV" )]
	public static void GetAVTo(string setName, string name)
	{
		
		CommandDealer commandDealer = getCommandDealer();;
		commandDealer.GetAV(name, setName);
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
	public static CommandDealer getCommandDealer()
	{
		if(Game.ActiveScene == null) throw new Exception("Did Not Grab Scene");
		Log.Info(Game.ActiveScene.GetAllObjects(true).ElementAt<GameObject>(0));
		CommandDealer commandDealer = Game.ActiveScene.GetAllObjects(true).ElementAt<GameObject>(1).Components.Get<CommandDealer>();
		if(commandDealer == null) throw new Exception("Failed To Grab Command Dealer");
		return commandDealer;
	}
}