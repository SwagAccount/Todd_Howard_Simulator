using Sandbox;

public sealed class FollowAttachment : Component
{
	[Property] public SkinnedModelRenderer Gun {get;set;}
	[Property] public SkinnedModelRenderer Arm {get;set;}
	[Property] public SkinnedModelRenderer ArmApparel {get;set;}
	[Property] public SkinnedModelRenderer Phone {get;set;}
	[Property] public WeaponScript WeaponScript {get;set;}
	
	[Property] public GameObject handPosR {get;set;}
	[Property] public GameObject handPosL {get;set;}
	[Property] public GameObject handTargetR {get;set;}
	[Property] public GameObject handTargetL {get;set;}
	[Property] public string attachmentR {get;set;}
	[Property] public string attachmentL {get;set;}
	[Property] public int LPose {get;set;}
	[Property] public int RPose {get;set;}

	protected override void OnPreRender()
	{
		
		if(WeaponScript.weapon.ExactArm)
		{
			Arm.BoneMergeTarget = Gun;
			Phone.BoneMergeTarget = Gun;
		}
		else
		{
			Phone.BoneMergeTarget = Arm;
			Arm.BoneMergeTarget = null;
			handPosR.Transform.Position = Gun.GetAttachment(attachmentR).Value.Position;
			handPosR.Transform.Rotation = Gun.GetAttachment(attachmentR).Value.Rotation;

			handPosL.Transform.Position = Gun.GetAttachment(attachmentL).Value.Position;
			handPosL.Transform.Rotation = Gun.GetAttachment(attachmentL).Value.Rotation;
			Arm.SetIk("hand_right", handTargetR.Transform.World);
			Arm.SetIk("hand_left", handTargetL.Transform.World);
			Arm.Set("HandPoseLeft", LPose);
			Arm.Set("HandPoseRight", RPose);
		}
		
		int apparelEquipSlot = WeaponScript.playerEntity.getEquip("apparel");
		if(apparelEquipSlot != -1)
		{
			int slot = WeaponScript.playerEntity.Equips[apparelEquipSlot].GetContainerIndex(WeaponScript.playerEntity);
			//ArmApparel.Model = Model.Load($"models/{string.Join("/", WeaponScript.playerEntity.Container[slot].Categories)}-arms.vmdl");
		}
		else
		{
			ArmApparel.Model = null;
		}

		Gun.OnGenericEvent = (a) =>
        {
          	if(a.Type == "LPose")
			{
				LPose = a.Int;
			}
			else if(a.Type == "RPose")
			{
				RPose = a.Int;
			}
        };
	}

}