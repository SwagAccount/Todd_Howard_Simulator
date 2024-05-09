using Sandbox;

public sealed class FollowAttachment : Component
{
	[Property] public SkinnedModelRenderer Gun {get;set;}
	[Property] public SkinnedModelRenderer Arm {get;set;}
	[Property] public string attachment {get;set;}
	protected override void OnPreRender()
	{
		GameObject.Transform.Position = (Gun.GetAttachment(attachment)?? default).Position;
		GameObject.Transform.Rotation = (Gun.GetAttachment(attachment)?? default).Rotation;
		
	}
}