using Sandbox;

public sealed class EnemyTrigger : Component, Component.ITriggerListener
{
	public FindEnemy findEnemy;
	void ITriggerListener.OnTriggerEnter(Collider other)
    {
		findEnemy.CheckEnemy(other.GameObject);
    }

}
