using System;
using Sandbox;

public sealed class Aiagent : Component
{
	[Property] public Entity Opp {get;set;}
	[Property] public NpcmodelAnimationManager npcmodelAnimationManager {get;set;}
	[Property] public AIStateMachine stateMachine {get;set;}
	[Property] public AIStateID initialState {get;set;}
	[Property] public NavMeshCharacter Controller {get;set;}
	[Property] public Npcweapon weapon {get;set;}
	public Attributes Attributes {get; set;}
	public Attributes.Attribute Health {get;set;}
	public Attributes.Attribute MaxHealth {get;set;}
	public Attributes.Attribute speed {get;set;}
	public Npcequipper npcequipper {get;set;}
	public GameObject player {get;set;}
	protected override void OnStart()
	{

		npcequipper = Components.Get<Npcequipper>();
		weapon = Components.Get<Npcweapon>();
		Controller = Components.GetOrCreate<NavMeshCharacter>();
		Controller.currentTarget = Transform.Position;
		Attributes = Components.Get<Attributes>();
		player = GameObject.Parent.Children[0];
		Controller.Height = Attributes.getAttribute("Height","NavMeshController").floatValue;
		Controller.Radius = Attributes.getAttribute("Radius","NavMeshController").floatValue;
		speed = Attributes.getAttribute("Max Speed","NavMeshController");
		Controller.Acceleration = Attributes.getAttribute("Acceleration","NavMeshController").floatValue;
		stateMachine = new AIStateMachine(this);
		//Log.Info(stateMachine.states.Count);
		stateMachine.RegisterState(new AIIdle());
		stateMachine.RegisterState(new AIShootPlayer());
		stateMachine.RegisterState(new AIDie());
		initialState = AIStateID.Idle;
		InitializeState();
		Health = Attributes.getAttribute("Health");
		MaxHealth = Attributes.getAttribute("MaxHealth");
		npcmodelAnimationManager = GameObject.Children[0].Components.Get<NpcmodelAnimationManager>();
		//Opp = GameObject.Parent.Children[0].Components.Get<Entity>();
	}
	async void InitializeState()
	{
		await Task.Frame();
		stateMachine.ChangeState(initialState,false);
	}
	protected override void OnUpdate()
	{
		stateMachine.Update();
		if(Health.floatValue <= 0)
		{
			stateMachine.ChangeState(AIStateID.Dead);
		}
	}
	public void faceThing(GameObject thing)
	{
		
		Angles angles = Rotation.LookAt(thing.Transform.Position - Transform.Position);
		
		angles.pitch = 0;
		angles.roll = 0;
		Transform.Rotation = angles;
	}
}
