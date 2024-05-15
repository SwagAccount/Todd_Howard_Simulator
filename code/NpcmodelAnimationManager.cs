using System;
using Sandbox;

public sealed class NpcmodelAnimationManager : Component
{
	[Category("Body")][Property] private float SkinColour {get;set;}
	[Category("Body")][Property] private SaveClasses.EntitySave Clothes {get; set;}
	[Category("Body")][Property] private SkinnedModelRenderer Body {get; set;}
	[Category("Body")][Property] private SkinnedModelRenderer Apparel {get; set;}
	[Category("Body")][Property] private Gradient SkinColourGradient {get; set;}
	[Category("Face")][Property] private Vector2 FaceStrech {get;set;} = new Vector2(3,3);
	[Category("Face")][Property] private string FaceType {get;set;} = "basic guy";
	[Category("Face")][Property] private string Expression {get;set;} = "";
	[Category("Face")][Property] private bool Talking {get;set;}
	[Category("Face")][Property] private float TalkRate {get;set;} = 1;
	[Category("Face")][Property] private float BlinkRate {get;set;} = 1;
	[Category("Face")][Property] private DecalRenderer LeftEye {get;set;}
	[Category("Face")][Property] private DecalRenderer RightEye {get;set;}
	[Category("Face")][Property] private DecalRenderer Mouth {get;set;}
	float Talk;
	float Blink;

	Vector3 lWigg;
	Vector3 rWigg;
	Vector3 mWigg;
	protected override void OnUpdate()
	{
		Body.Tint = SkinColourGradient.Evaluate(SkinColour);
		if(Clothes.Categories.Count > 0)
		{
			Body.Model = Model.Load($"models/{string.Join("/", Clothes.Categories)}-body.vmdl");
			Apparel.Model = Model.Load($"models/{string.Join("/", Clothes.Categories)}-apparel.vmdl");
			Apparel.GameObject.Enabled = true;
		}
		else
		{
			Body.Model = Model.Load($"models/items/apparel/nude.vmdl");
			Apparel.GameObject.Enabled = false;
		}

		Talk += Time.Delta * TalkRate * 20f;
		Blink += Time.Delta * BlinkRate * 1.25f;

		lWigg += Vector3.Random/20;
		mWigg += Vector3.Random/20;
		rWigg += Vector3.Random/20;

		lWigg = lWigg.Clamp(Vector3.Zero, Vector3.One/20);
		rWigg = rWigg.Clamp(Vector3.Zero, Vector3.One/20);
		mWigg = mWigg.Clamp(Vector3.Zero, Vector3.One/20);

		LeftEye.Size = new Vector3(FaceStrech.x,FaceStrech.y,3) + lWigg*FaceStrech.Length;
		RightEye.Size = new Vector3(FaceStrech.x, FaceStrech.y,-3) + rWigg*FaceStrech.Length;
		Mouth.Size = new Vector3(FaceStrech.x,FaceStrech.y,3) + mWigg*FaceStrech.Length;
		Mouth.Material = Talking && MathF.Sin(Talk) >= 0 ? getFaceMaterial("mouth", true, true, "open","closed") : getFaceMaterial("mouth", true, false, "open", "closed");

		Material eyeMat = MathF.Sin(Blink) >= -0.99f ? getFaceMaterial("eye", false, false, "closed","open") : getFaceMaterial("eye", false, true, "closed","open");
		if(eyeMat != null)
		{
			LeftEye.Enabled = true;
			RightEye.Enabled = true;
			LeftEye.Material = eyeMat;
			RightEye.Material = eyeMat;
		}
		else
		{
			LeftEye.Enabled = false;
			RightEye.Enabled = false;
		}

	}

	Material getFaceMaterial(string type, bool hasAlt, bool getAlt, string altName, string defaultName)
	{
		string directory = $"faces/{FaceType}/{type}/{Expression}.vmat";
		if(!getAlt)
		{

			if(FileSystem.Mounted.FileExists(directory))
				return Material.Load(directory);
			else
				return Material.Load($"faces/{FaceType}/{type}/{defaultName}.vmat");
		}
		else
		{
			
			if(hasAlt)
				return Material.Load($"faces/{FaceType}/{type}/{altName}.vmat");
			else
				return null;
		}
	}
	
}
