using Sandbox;

public sealed class FindEnemy : Component
{
	private Attributes.Attribute scanRadius {get;set;}
	private Attributes.Attribute faction {get;set;}
	private FactionRelationships factionRelationships {get;set;}
	private Aiagent aiagent {get;set;}
	protected override async void OnStart()
	{
		await Task.Frame();
		Attributes attributes = Components.Get<Attributes>();
		scanRadius = attributes.getAttribute("Scan Radius", "Brain");
		faction = attributes.getAttribute("Faction");
		factionRelationships = Game.ActiveScene.GetAllObjects(true).ElementAt<GameObject>(1).Components.Get<FactionRelationships>();

		GameObject scanner = new GameObject();
		scanner.SetParent(GameObject);
		scanner.Transform.LocalPosition = Vector3.Zero;
		scanner.Transform.LocalRotation = Angles.Zero;

		EnemyTrigger enemyTrigger = scanner.Components.Create<EnemyTrigger>();
		enemyTrigger.findEnemy = this;

		SphereCollider sphereCollider = scanner.Components.Create<SphereCollider>();
		sphereCollider.Radius = 0.2f;

		SphereCollider sphereTrigger = scanner.Components.Create<SphereCollider>();
		sphereTrigger.Radius = scanRadius.floatValue;
		sphereTrigger.IsTrigger = true;

		aiagent = Components.Get<Aiagent>();
	}

	public void CheckEnemy(GameObject gameObject)
	{
		PlayerController playerController = gameObject.Components.Get<PlayerController>();
		if(playerController != null)
		{
			if(factionRelationships.GetRelationshipValue(faction.stringValue, "Player") <= -0.5f)
				aiagent.Opp = gameObject.Components.Get<Entity>();
			return;
		}

		if(aiagent.Opp != null) return;
		Attributes attributes = gameObject.Parent.Components.Get<Attributes>();
		if(attributes == null) return;
		string faction2 = attributes.getAttribute("Faction").stringValue;
		if(factionRelationships.GetRelationshipValue(faction.stringValue, faction2) <= -0.5f)
			aiagent.Opp = gameObject.Parent.Components.Get<Entity>();
	}
}
