using Sandbox;

public sealed class ItemRenderer : Component
{
	private SkinnedModelRenderer modelRenderer;
	private ModelCollider modelCollider;
	private Ids ids; 
	protected override void OnStart()
	{
		ids = Components.Get<Ids>();
		modelRenderer = Components.GetOrCreate<SkinnedModelRenderer>();
		modelCollider = Components.GetOrCreate<ModelCollider>();
		Components.Create<Rigidbody>();
		Model model = Model.Load($"models/{string.Join("/", ids.Categories)}.vmdl");
		modelRenderer.Model = model;
		modelCollider.Model = model;
	}
}