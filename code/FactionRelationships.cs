using Sandbox;

public sealed class FactionRelationships : Component
{
	[Property] public List<FactionRelationship> Relationships {get; set;}
	protected override void OnStart()
	{

	}

	public float GetRelationshipValue(string Faction1, string Faction2)
	{
		int index = RelationshipIndex(Faction1,Faction2);
		if(index != -1)
		{
			return Relationships[index].Value;
		}

		AddRelationship(Faction1,Faction2);
		return 0;
	}

	public int RelationshipIndex(string Faction1, string Faction2)
	{
		for(int i = 0; i < Relationships.Count; i++)
		{
			if(Relationships[i].Factions.Contains(Faction1) && Relationships[i].Factions.Contains(Faction2))
			{
				return i;
			}
		}
		return -1;
	}

	public void EffectRelationship(float Value, string Faction1, string Faction2)
	{
		int index = RelationshipIndex(Faction1,Faction2);
		if(index != -1)
		{
			Relationships[index].Value += Value;
			return;
		}
		AddRelationship(Faction1,Faction2);
	}

	public void AddRelationship(string Faction1, string Faction2)
	{
		FactionRelationship relationship = new FactionRelationship();
		relationship.Factions = new List<string>{Faction1,Faction2};
		relationship.Value = 0;
		Relationships.Add(relationship);
	}

	public class FactionRelationship
	{
		[KeyProperty] public List<string> Factions {get;set;}
		public float Value {get; set;}
	}
}
