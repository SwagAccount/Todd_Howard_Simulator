using System;
using Sandbox;

public sealed class CommandDealer : Component
{
	[Property] public string sceneIdSelected {get;set;}
	[Property] public GameObject Saved {get;set;}
	
	public string SetAV(string name, string value)
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
						Attributes.Attribute attribute = attributes.getAttribute(name);
						if(attribute != null)
						{
							var set =  Attributes.ConvertFromString(value, attribute.attributeType);
							attribute.SetValue<object>(set);
						}
					}
					break;
				}
			}
		}
		return "Failed To Retrieve ID";
	}

	[ConCmd( "SetAV" )]
	public static void SetAVTo( string name, string value )
	{
		
		if(Game.ActiveScene == null) throw new Exception("Did Not Grab Scene");
		CommandDealer commandDealer = Game.ActiveScene.GetAllObjects(true).ElementAt<GameObject>(0).Components.Get<CommandDealer>();
		if(commandDealer == null) throw new Exception("Failed To Grab Command Dealer");
		commandDealer.SetAV(name,value);
		
	}
}