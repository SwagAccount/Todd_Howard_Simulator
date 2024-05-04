using Sandbox;

public sealed class SaveClasses : Component
{
	public class EntitySave
	{
		public int id;
		public List<string> Categories;
		public bool displayContainer;
		public List<Attributes.SavedAttributeSet> AttributeSets;
		public List<EntitySave> Container;
	}
}
