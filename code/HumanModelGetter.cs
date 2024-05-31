using Sandbox;

public sealed class HumanModelGetter : Component
{
	protected override void OnAwake()
	{
		PlayerReference player = ResourceLibrary.Get<PlayerReference>("gameresources/humannpc.player");
		GameObject gameObject = player.gameObject.Clone();
		gameObject.Transform.Position = Transform.Position;
		gameObject.Transform.Rotation = Transform.Rotation;
		gameObject.SetParent(GameObject);
	}
}
