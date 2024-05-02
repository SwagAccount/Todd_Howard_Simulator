using Sandbox;


public sealed class Interactor : Component
{
	
	private CrossHair crossHair;
	[Property] private float range {get; set;}
	[Property] private GameObject Grabber {get; set;}
	GameObject lastHit;
	Interactable interactable;
	Interactable lastInteractable;
	Entity entity;
	Rigidbody entityBody;
	Attributes attributes;
	Ids ids;

	[Property] SpringJoint joint;
	protected override void OnStart()
	{
		crossHair = Components.GetOrCreate<CrossHair>();
	}
	float useTime = 0;
	protected override void OnUpdate()
	{
		var trace = Scene.Trace.Ray(
			Transform.Position,
			Transform.Position+(Transform.World.Forward*range)
		).Run();
		bool checkgrab = joint == null;
		if(!checkgrab) checkgrab = !joint.Enabled;
		if(checkgrab)
		{
			if(trace.Hit && trace.GameObject != lastHit)
			{
				interactable = trace.GameObject.Components.Get<Interactable>();
				if(interactable != null)
				{
					entity = interactable.Components.Get<Entity>();
					entityBody = interactable.Components.Get<Rigidbody>();
					Grabber.Transform.Position = interactable.Transform.Position;
					if(entityBody != null)
					{
						joint = interactable.Components.GetOrCreate<SpringJoint>();
						joint.Frequency = 10;
						joint.Body = Grabber;
					}
					attributes = entity.Components.Get<Attributes>();
					ids = entity.Components.Get<Ids>();
				}
			}
			else
			{
				interactable = null;
				if(joint!=null)
				{
					joint.Destroy();
					joint = null;
				}
			}
		}
		
		if(interactable != null && interactable != lastInteractable)
		{
			crossHair.Name = CustomFunctions.CapitalizeWords(ids.Categories[ids.Categories.Count-1]);
			crossHair.stats = new List<CrossHair.stat>();
			if(ids.Categories.Contains("weapons"))
			{
				getWeaponStats();
			}
			if(ids.Categories.Contains("items"))
			{
				getItemStats();
			}
		}

		if(Input.Down("use")) useTime+=Time.Delta;
		else useTime = 0;

		if(interactable != null)
		{
			if(useTime > 0.1)
			{
				joint.Enabled = true;
			}	
			else
			{
				joint.Enabled = false;
				if(Input.Released("use")) interactable.Interacted = true;
			}
		}
		else
		{
			crossHair.Name = "";
			crossHair.stats = new List<CrossHair.stat>();
		}
	}
	void getWeaponStats()
	{
		Weapon weapon = CustomFunctions.GetResource<Weapon>(ids, "weapon");
		item item = CustomFunctions.GetResource<item>(ids, "item");

		CrossHair.stat newStat = new CrossHair.stat();
		newStat.StatName = "Damage";
		string spreadMultiplier = weapon.bulletStats[0].shotsPer > 0 ? $"{weapon.bulletStats[0].shotsPer}x" : "";
		newStat.Stat = $"{spreadMultiplier}{weapon.bulletStats[0].damage}";
		crossHair.stats.Add(newStat);
		
		newStat = new CrossHair.stat();
		newStat.StatName = "DPS";
		float DPS = (weapon.shootTime)*weapon.bulletStats[0].damage*weapon.bulletStats[0].shotsPer;
		newStat.Stat = $"{DPS}";
		crossHair.stats.Add(newStat);

		newStat = new CrossHair.stat();
		newStat.StatName = "Condition";
		newStat.Stat = $"{attributes.getAttribute("Condition").floatValue}";
		crossHair.stats.Add(newStat);
	}
	void getItemStats()
	{
		item item = CustomFunctions.GetResource<item>(ids, "item");
		
		CrossHair.stat newStat = new CrossHair.stat();
		newStat.StatName = "Weight";
		newStat.Stat = $"{item.Weight}";
		crossHair.stats.Add(newStat);

		newStat = new CrossHair.stat();
		newStat.StatName = "Value";
		newStat.Stat = $"{item.Value}";
		crossHair.stats.Add(newStat);

	}
}