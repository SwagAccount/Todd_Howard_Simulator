using System;
using System.Threading;
using Sandbox;
using Sandbox.Navigation;
using static CustomFunctions;

public class AIShootPlayer : AIState
{
	public void Enter( Aiagent agent )
	{
		agent.npcequipper.equipWeapon = true;
		agent.npcequipper.UpdateEquips();
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
		if(agent.Opp == null)
		{
			agent.stateMachine.ChangeState(AIStateID.Idle);
			return;
		}
		GameObject checkShoot = agent.weapon.CheckShoot();

		if(checkShoot!=null)
		{
			if(checkShoot == agent.Opp.GameObject) agent.weapon.Shooting = true;
		}


		agent.Controller.UpdateRotation = false;
		agent.Controller.Speed = agent.speed.floatValue*2;
		float DistanceToOpp = Vector3.DistanceBetween(agent.Transform.Position, agent.Opp.Transform.Position);
		float MaxAttackDistance = agent.weapon.weapon == null ? 500 : CalculateEffectiveDistance((
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
		agent.faceThing(agent.Opp.GameObject);
		lastHealth = agent.Health.floatValue;
	}
}

public class AIDie : AIState
{
	public void Enter( Aiagent agent )
	{
		agent.npcequipper.equipWeapon = false;
		agent.npcequipper.UpdateEquips();
		int gunEquipSlot = agent.weapon.Entity.getEquip("weapons");
		if(gunEquipSlot!=-1)
		{
			agent.weapon.DropWeapon();  
			Log.Info("FUCK");
		}
		agent.npcmodelAnimationManager.Dead = true;
		agent.weapon.Entity.displayContainer = true;
		NavMeshCharacter navMeshCharacter = agent.Components.Get<NavMeshCharacter>();
		navMeshCharacter.boxCollider.Destroy();
		navMeshCharacter.characterController.Destroy();
		navMeshCharacter.Destroy();
	}

	public void Exit( Aiagent agent )
	{
		
	}

	public AIStateID GetID()
	{
		return AIStateID.Dead;
	}

	public void Update( Aiagent agent )
	{
		
	}
}

public class AIIdle : AIState
{
	List<int> PatrolIDs;
	Attributes.AttributeSet defaultSet;
	Attributes.Attribute Stare;
	Attributes.AttributeSet patrolSet;
	CommandDealer commandDealer;
	GameObject player;
	int currentPatrolPoint = -1;
	public void Enter( Aiagent agent )
	{
		agent.npcequipper.equipWeapon = false;
		agent.npcequipper.UpdateEquips();
		Log.Info("Start");
		agent.Controller.UpdateRotation = false;
		commandDealer = CommandDealer.getCommandDealer();
		patrolSet = agent.Attributes.attributeSets[agent.Attributes.getAttributeSetIndex("Patrol Points")];
		defaultSet = agent.Attributes.attributeSets[agent.Attributes.getAttributeSetIndex("default")];
		for(int i = 0; i < defaultSet.attributes.Count; i++)
		{
			if(defaultSet.attributes[i].AttributeName == "Stare")
			{
				Stare = defaultSet.attributes[i];
				break;
			}
		}
		if(patrolSet.attributes.Count > 0)
		{
			currentPatrolPoint = 0;
			agent.Controller.currentTarget = patrolSet.attributes[currentPatrolPoint].vector3Value;
		}
	}

	public void Exit( Aiagent agent )
	{
		
	}

	public AIStateID GetID()
	{
		return AIStateID.Idle;
	}
	
	public void Update( Aiagent agent )
	{
		if(agent.Opp != null) agent.stateMachine.ChangeState(AIStateID.ShootPlayer);
		if(true)
		{
			float distanceToPlayer = Vector3.DistanceBetween(agent.Transform.Position, agent.player.Transform.Position);
	
			if(distanceToPlayer < 150f)
			{
				
				agent.faceThing(agent.player);
				
			}
		}

		if(currentPatrolPoint != -1)
		{
			Vector3 currentPoint = patrolSet.attributes[currentPatrolPoint].vector3Value;
			float distanceToPoint = Vector3.DistanceBetween(agent.Transform.Position, currentPoint);
			if(distanceToPoint<10) 
			{
				currentPatrolPoint++;
				agent.Controller.currentTarget = patrolSet.attributes[currentPatrolPoint].vector3Value;
			}
		}
	}
}