using System;
using System.Threading;
using Sandbox;
using Sandbox.Navigation;
using static CustomFunctions;

public class AIShootPlayer : AIState
{
	public void Enter( Aiagent agent )
	{
		
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
		//add random move chance thingy here
		RandomPos = agent.Transform.Position + direction.Normal*0.5f*((Game.Random.Next(0,50)+50)/100f);
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
		float MaxAttackDistance = CalculateEffectiveDistance((
			agent.weapon.weapon.minMaxRecoilPos[0].Length + agent.weapon.weapon.minMaxRecoilPos[1].Length)/2,
			agent.weapon.weapon.bulletStats[0].spreadX+agent.weapon.weapon.bulletStats[0].spreadY,
			16,
			0.5f,
			agent.weapon.weapon.modes[0].buttonHold);
		float DistanceResistance = MaxAttackDistance/10;
		float RandomMoveChance = 0.5f;

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