using Sandbox;
using static SaveClasses;
public sealed class EntitySaver : Component
{
	[Property] private bool save {get;set;}
	[Property] private bool load {get;set;}
	protected override void OnUpdate()
	{
		if(save)
		{
			Save();
			save = false;
		}
		if(load)
		{
			Load();
			load = false;
		}
	}
	public void Save(string saveName = "Quicksave")
	{
		List<EntitySave> entities = new List<EntitySave>();
		
		for(int i = 0; i < GameObject.Children.Count; i++)
		{
			Entity entity = GameObject.Children[i].Components.Get<Entity>();
			if(entity != null)
			{
				EntitySave entitySave = new EntitySave();
				
				entities.Add(CustomFunctions.SaveEntity(entity,true));
			} 
		}
		if(!FileSystem.Data.DirectoryExists("Saves"))
		{
			FileSystem.Data.CreateDirectory("Saves");
		}
		string dirName = $"Saves/{saveName}/";
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			FileSystem.Data.CreateDirectory(dirName);
		}

		FileSystem.Data.WriteAllText($"{dirName}{Scene.Name}.json", Json.Serialize(entities));
		Log.Info("SHIT");
	}

	public void Load(string saveName = "Quicksave")
	{
		string dirName = $"Saves/{saveName}/";
		if(!FileSystem.Data.DirectoryExists(dirName))
		{
			return;
		}
		if(!FileSystem.Data.FileExists($"{dirName}{Scene.Name}.json"))
		{
			Log.Info("No Saves");
			return;
		}
		for(int i = 0; i < GameObject.Children.Count; i++ )
		{
			GameObject.Children[i].Destroy();
		}
		string shit = FileSystem.Data.ReadAllText( $"{dirName}enemies.json");
		List<EntitySave> entities = Json.Deserialize<List<EntitySave>>(shit);
		foreach(EntitySave entitySave in entities)
		{
			CustomFunctions.SpawnEntity(entitySave,true);
		}
	}
}
