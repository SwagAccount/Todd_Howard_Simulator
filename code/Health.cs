using Sandbox;

public sealed class Health : Component
{
	private Attributes.Attribute health;
	private Attributes attributes;

	protected override void OnStart()
	{
		attributes = Components.Get<Attributes>();
		health = attributes.getAttribute("health", "default");
	}

	public void DealDamage(float amount)
	{
		health.floatValue -= amount;
	}

	public void SetHealth(float amount)
	{
		health.floatValue = amount;
	}

	public void resetHealth()
	{
		health.floatValue = attributes.getAttribute("maxhealth", "default").floatValue;
	}
}