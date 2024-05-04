using Sandbox;

public sealed class SaveClasses : Component
{
	public class EntitySave
	{
		public List<string> Catagories;
		public bool displayContainer;
		public List<Attributes.SavedAttributeSet> AttributeSets;
		public List<EntitySave> Container;
	}
}
