using Sandbox;

public sealed class Entity : Component
{
	[Property] public Attributes Attributes {get;set;}
	[Property] public Ids Ids {get;set;}
	[Property] public List<Catagory> Catagories {get;set;}
	[Property] public bool displayContainer { get; set;}
	[Property] public List<SaveClasses.EntitySave> Container {get;set;}
	[Property] public List<Equiped> Equips {get;set;}
	public int getEquip(string equipType)
	{
		for(int i = 0; i < Equips.Count; i++)
		{
			if(Equips[i].equipType == equipType) return i;
		}
		return -1;
	}
	public class Equiped
	{
		public string equipType {get;set;}
		public int ID {get;set;}
		public int GetContainerIndex(Entity entity)
		{
			for(int i = 0; i < entity.Container.Count; i++)
			{
				if(entity.Container[i].id == ID)
				{
					return i;
				}
			}
			return -1;
		}
	}
	public class Catagory
	{
		public string CatagoryName {get;set;}
		public List<string> Components {get;set;}
		public List<Catagory> Catagories {get;set;}
	}
	protected override void OnStart()
	{
		Attributes = Components.Get<Attributes>();
		Ids = Components.Get<Ids>();
		Catagories = ResourceLibrary.Get<CatagoryComponents>("gameresources/catagorycomponents.catagory").Catagories;
		addCatagories(Catagories);
	}
	void addCatagories(List<Catagory> catagories)
	{
		foreach(Catagory c in catagories)
		{
			if(Ids.Categories.Contains(c.CatagoryName))
			{
				foreach(string component in c.Components)
				{
					TypeDescription comp = TypeLibrary.GetType(component);
					if(comp!=null) Components.Create(comp,true);
				}
				addCatagories(c.Catagories);
			}
		}
	}
}