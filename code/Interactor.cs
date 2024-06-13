using Sandbox;


public sealed class Interactor : Component
{
	
	private CrossHair crossHair;
	[Property] private float range {get; set;}
	[Property] private GameObject Grabber {get; set;}
	ConversationScript conversationScript;
	GameObject lastHit;
	Hitbox lastHitbox;
	Interactable interactable;
	Interactable lastInteractable;
	Entity entity;
	Entity player;
	ContainerInteract containerInteract;
	Rigidbody entityBody;
	Attributes attributes;
	Ids ids;
	float oldAngularDamping;
	float oldLinearDamping;
	[Property] SpringJoint joint;
	protected override void OnStart()
	{
		crossHair = Components.GetOrCreate<CrossHair>();
		player = GameObject.Parent.Components.Get<Entity>();
		containerInteract = GameObject.Parent.Components.Get<ContainerInteract>();
		conversationScript = Components.Get<ConversationScript>();
	}
	float useTime = 0;
	protected override void OnUpdate()
	{
		var trace = Scene.Trace.Ray(
			Transform.Position,
			Transform.Position+(Transform.World.Forward*range)
		).UseHitboxes().Run();
		bool checkgrab = joint == null;
		if(!checkgrab) checkgrab = !joint.Enabled;
		if(checkgrab)
		{
			GameObject traceGameObject = trace.GameObject;
			if(trace.Hitbox != null)
			{
				traceGameObject = trace.Hitbox.GameObject;
			}
			if(trace.Hit)// && (trace.GameObject != lastHit || trace.Hitbox != lastHitbox))
			{
				//lastHit = trace.GameObject;
				//lastHitbox = trace.Hitbox;
				entity = trace.GameObject.Components.Get<Entity>();
				if(entity != null)
				{
					if(entity.displayContainer)
					{
						containerInteract.LookedAtContainer = entity;
						containerInteract.State = ContainerInteract.States.Look;
						if(Input.Pressed("use"))
						{
							containerInteract.State = ContainerInteract.States.Transfer;
						}
					}
					Attributes.Attribute convoAttribute = entity.Attributes.getAttribute("Conversation", "default", false);
					if(convoAttribute != null)
					{
						if(Input.Pressed("use"))
						{
							conversationScript.conversation = ResourceLibrary.Get<Conversation>($"gameresources/conversations/{convoAttribute.stringValue}.conv");
							conversationScript.talkedToEntity = entity;
						}
					}
				}
				interactable = trace.GameObject.Components.Get<Interactable>();
				if(interactable != null)
				{
					entityBody = interactable.Components.Get<Rigidbody>();
					Grabber.Transform.Position = interactable.Transform.Position;
					if(entityBody != null)
					{
						joint = interactable.Components.GetOrCreate<SpringJoint>();
						joint.Frequency = 100;
						oldAngularDamping = entityBody.AngularDamping;
						
						oldLinearDamping = entityBody.LinearDamping;
						
						joint.Body = Grabber;
					}
					attributes = entity.Components.Get<Attributes>();
					ids = entity.Components.Get<Ids>();
				}
			}
			else
			{
				containerInteract.State = ContainerInteract.States.None;
				interactable = null;
				if(joint!=null)
				{
					
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

		

		if(interactable != null)
		{
			if(useTime > 0.1)
			{
				joint.Enabled = true;
				entityBody.AngularDamping = 24f;
				entityBody.LinearDamping = 16f;
			}	
			else
			{
				joint.Enabled = false;
				entityBody.AngularDamping=oldAngularDamping;
					entityBody.LinearDamping=entityBody.AngularDamping;
				if(Input.Released("use")) interactable.Interacted = true;
			}
		}
		else
		{
			crossHair.Name = "";
			crossHair.stats = new List<CrossHair.stat>();
		}

		if(Input.Down("use")) useTime+=Time.Delta;
		else useTime = 0;
	}
	void getWeaponStats()
	{
		Weapon weapon = CustomFunctions.GetResource<Weapon>(ids.Categories, "weapon");
		item item = CustomFunctions.GetResource<item>(ids.Categories, "item");

		CrossHair.stat newStat = new CrossHair.stat();
		newStat.StatName = "Damage";
		string spreadMultiplier = weapon.bulletStats[0].shotsPer > 0 ? $"{weapon.bulletStats[0].shotsPer}x" : "";
		newStat.Stat = $"{spreadMultiplier}{weapon.bulletStats[0].damage}";
		crossHair.stats.Add(newStat);
		
		newStat = new CrossHair.stat();
		newStat.StatName = "DPS";
		float DPS = (1/weapon.shootTime)*weapon.bulletStats[0].damage*weapon.bulletStats[0].shotsPer;
		newStat.Stat = $"{MathX.FloorToInt(DPS*10)/10}";
		crossHair.stats.Add(newStat);

		newStat = new CrossHair.stat();
		newStat.StatName = "Condition";
		newStat.Stat = $"{attributes.getAttribute("Condition").floatValue}";
		crossHair.stats.Add(newStat);
	}
	void getItemStats()
	{
		item item = CustomFunctions.GetResource<item>(ids.Categories, "item");
		
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