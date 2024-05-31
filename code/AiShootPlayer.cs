using System;
using System.Threading;
using Sandbox;
using Sandbox.Navigation;
using static CustomFunctions;

public class AIShootPlayer : AIState
{
	int MaxAttackDistanceI;
	int DistanceResistanceI;
	int RandomMovementDisI;
	int RandomMoveChanceI;
	int BrainI;
	public void Enter( Aiagent agent )
	{
		BrainI = agent.Attributes.getAttributeSetIndex("Brain");
		MaxAttackDistanceI = agent.Attributes.getAttributeIndex("MaxAttackDistance","Brain");
		DistanceResistanceI = agent.Attributes.getAttributeIndex("DistanceResistance","Brain");
		RandomMovementDisI = agent.Attributes.getAttributeIndex("RandomMovementDis","Brain");
		RandomMoveChanceI = agent.Attributes.getAttributeIndex("RandomMoveChance","Brain");
	}

	public void Exit( Aiagent agent )
	{
		
	}

	public AIStateID GetID()
	{
		return AIStateID.ShootPlayer;
	}
	bool GoToDesiredDistance;
	Vector3 RandomPos;

	void GenerateRandomPos(Aiagent agent)
	{
		Vector3 direction = Vector3.Random;
		direction.z=0;
		RandomPos = agent.Transform.Position + direction.Normal*GetAttributeFloatUsingIndexs(agent.Attributes, BrainI, RandomMovementDisI)*((Game.Random.Next(0,50)+50)/100f);
	}
	float timer;
	float lastHealth;

	
	public void Update( Aiagent agent )
	{
		GameObject checkShoot = agent.weapon.CheckShoot();

		if(checkShoot!=null)
		{
			if(checkShoot == agent.Opp.GameObject) agent.weapon.Shooting = true;
		}


		agent.Controller.UpdateRotation = false;
		agent.Controller.Speed = agent.speed.floatValue*2;
		float DistanceToOpp = Vector3.DistanceBetween(agent.Transform.Position, agent.Opp.Transform.Position);
		float MaxAttackDistance = GetAttributeFloatUsingIndexs(agent.Attributes, BrainI, MaxAttackDistanceI) * (1+(1-agent.Health.floatValue/agent.MaxHealth.floatValue));
		float DistanceResistance = GetAttributeFloatUsingIndexs(agent.Attributes, BrainI, DistanceResistanceI);
		float RandomMoveChance = GetAttributeFloatUsingIndexs(agent.Attributes, BrainI, RandomMoveChanceI);

		if(MathF.Abs(DistanceToOpp - MaxAttackDistance) > DistanceResistance) GoToDesiredDistance = true;
		
		if(GoToDesiredDistance)
		{
			RandomPos = agent.Transform.Position;
			Vector3 Target;
			if(DistanceToOpp < MaxAttackDistance) Target = agent.Transform.Position+(agent.Transform.Position - agent.Opp.Transform.Position).Normal*(MaxAttackDistance-DistanceToOpp); // Move Away
			else Target = agent.Opp.Transform.Position; //Move Towards
			agent.Controller.currentTarget = Target;
			if(MathF.Abs(DistanceToOpp-MaxAttackDistance) < DistanceResistance/10) GoToDesiredDistance = false;
		}
		else
		{
			timer += Time.Delta;
			bool AHHH = agent.Health.floatValue < lastHealth && agent.Controller.DistanceToTarget() < agent.Controller.PositionAccuracy;
			if(timer > 1 || AHHH)
			{
				if(Vector3.DistanceBetween(agent.Transform.Position,RandomPos) < 10)
				{
					if(Game.Random.Next(0,100) <= RandomMoveChance * (AHHH ? 2f : 1f)) GenerateRandomPos(agent);
				}
				timer = 0;
			}
			agent.Controller.currentTarget = RandomPos;
		}
		agent.faceOpp();
		lastHealth = agent.Health.floatValue;
	}
}