using Sandbox;

public sealed class SaveClasses : Component
{
	public class EntitySave
	{
		public int id {get;set;}
		public List<string> Categories {get;set;}
		public bool displayContainer {get;set;}
		public List<Attributes.SavedAttributeSet> AttributeSets {get;set;}
		public List<EntitySave> Container {get;set;}
	}
}
