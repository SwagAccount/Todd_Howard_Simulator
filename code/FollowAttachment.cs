using Sandbox;

public sealed class FollowAttachment : Component
{
	[Property] public SkinnedModelRenderer Gun {get;set;}
	[Property] public SkinnedModelRenderer Arm {get;set;}
	[Property] public GameObject handPos {get;set;}
	[Property] public GameObject handTarget {get;set;}
	[Property] public string attachment {get;set;}
	protected override void OnUpdate()
	{
		
		handPos.Transform.Position = (Gun.GetAttachment(attachment)?? default).Position;
		handPos.Transform.Rotation = (Gun.GetAttachment(attachment)?? default).Rotation;
		
		Arm.Set("HandPos",handTarget.Transform.Position);
		Arm.Set("HandRot",handTarget.Transform.Rotation);
	}
}