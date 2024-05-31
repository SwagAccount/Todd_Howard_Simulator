using System;
using Sandbox;

public sealed class HealthGizmo : Component
{
	Health health;
	Attributes.Attribute healthAttribute;
	[Property] public float Height {get;set;}
	[Property] public float HealthValue {get;set;}
	[Property] public bool DisplayHealth {get;set;} = true;
	protected override void OnStart()
	{
		health = Components.Get<Health>();
		healthAttribute = Components.Get<Attributes>().getAttribute("Health");
	}
	protected override void OnUpdate()
	{
		HealthValue = healthAttribute.floatValue;
		if(!DisplayHealth) return;
		Transform transform = GameObject.Transform.World;
		transform.Position+= Vector3.Up*Height;
		Gizmo.Draw.Text($"{MathF.Round(healthAttribute.floatValue)}", transform);
	}
}
