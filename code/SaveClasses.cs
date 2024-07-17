using Sandbox;

public static class SaveClasses
{
	public class EntitySave
	{
		[KeyProperty] public string Name {get;set;}
		public Vector3 Position {get;set;}
		public Angles Rotation {get;set;}
		public int id {get;set;}
		public List<string> Categories {get;set;}
		public bool displayContainer {get;set;}
		public List<Attributes.SavedAttributeSet> AttributeSets {get;set;}
		public List<Entity.Equiped> Equips {get;set;}
		public List<PerkEffector> PerkEffectors {get;set;}
		public List<EntitySave> Container {get;set;}
	}
}
