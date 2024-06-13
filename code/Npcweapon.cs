using System;
using Sandbox;

public sealed class Npcweapon : Component
{
	Aiagent aiagent;
	Attributes.Attribute GunBoneIndex;
	NpcmodelAnimationManager npcmodelAnimationManager;
	[Property] private GameObject pivot;
	[Property] private bool fuckoff {get;set;}
	private BulletHoleDB bHoleDB;
	public Entity Entity;
	Vector3 recoilOffsetPos;
    Angles recoilOffsetRot;
	[Property] int gunEquipSlot;
	int lastGunEquipSlot;

	public GameObject GetChild(GameObject parent, int[] indices)
    {
        GameObject current = parent;
		
        for (int i = 0; i < indices.Length; i++)
        {
			
			Log.Info(current);
			
            if (indices[i] < 0 || indices[i] >= parent.Children.Count)
            {
				
                return null;
            }
			try
			{
				current = current.Children[indices[i]];
			}
			catch
			{
				Log.Info("fuck");
				Log.Info(indices[i]);
				return null;
			}
            
			
			
        }
		
        return current;
    }
	GameObject handPosL;
	GameObject handPosR;
	protected override void OnStart()
	{
		bHoleDB = ResourceLibrary.Get<BulletHoleDB>("gameresources/bhole.bhdb");
		lastGunEquipSlot = -1;
		Entity = Components.Get<Entity>();
		aiagent = Components.Get<Aiagent>();
		
		npcmodelAnimationManager = GameObject.Children[0].Components.Get<NpcmodelAnimationManager>();
		
		GetPivot();
		
		foreach(GameObject g in GameObject.Children[0].Children)
		{
			if(g.Name == "hand_left") handPosL = g;
			if(g.Name == "hand_right") handPosR = g;
		}
		
		
	}
	[Property] public SkinnedModelRenderer SkinnedModel {get;set;}
	[Property] public Weapon weapon {get;set;}
	[Property] string clipContent;
	bool pauseGun;
	int weaponSlot;
	public int lastWeaponSlot;

	int currentMode;
	GameObject GunRotate ;
	
	void GetPivot()
	{
		Attributes attributes = Components.Get<Attributes>();
		GunBoneIndex = attributes.getAttribute("Gun Bone Index");
		
		string[] indexs = GunBoneIndex.stringValue.Split(",");
		List<int> GunBoneIndexs = new List<int>
		{
		};
		for(int i = 0; i < indexs.Length; i++)
		{
			GunBoneIndexs.Add(int.Parse(indexs[i]));
		}
		Log.Info(string.Join(",",GunBoneIndexs));
		
		pivot = GetChild(GameObject.Children[0].Children[0],GunBoneIndexs.ToArray());
		
		GunRotate = new GameObject();
		GunRotate.SetParent(pivot);
		GameObject Gun = new GameObject();
		Gun.SetParent(GunRotate);
		SkinnedModel = Gun.Components.Create<SkinnedModelRenderer>();
	}
	[Property] public bool Shooting {get;set;}
	bool isReloading;
	protected override async void OnUpdate()
	{
		

		if(weaponSlot != lastWeaponSlot) weaponSlot = Entity.Equips[gunEquipSlot].GetContainerIndex(Entity);

		if((gunEquipSlot != lastGunEquipSlot) || weaponSlot != lastWeaponSlot)
        {
			
            GunChange();
			
        }

        if(weapon == null) return;

        GunPos();

		if(fuckoff)
		{
			fuckoff = false;
			GetPivot();
		}

		if(gunEquipSlot != -1)
		{

			Transform RightHand = SkinnedModel.GetAttachment("Right_Hand").Value;
			Transform LeftHand = SkinnedModel.GetAttachment("Left_Hand").Value;

			npcmodelAnimationManager.rightHandEnabled = true;
			npcmodelAnimationManager.leftHandEnabled = true;

			handPosR.Transform.Position = RightHand.Position;
			handPosR.Transform.Rotation = RightHand.Rotation;

			handPosL.Transform.Position = LeftHand.Position;
			handPosL.Transform.Rotation = LeftHand.Rotation;
		}

		pivot.Transform.Rotation = Rotation.LookAt((aiagent.Opp.Transform.Position+Vector3.Up*40)-pivot.Transform.Position );
		
		lastWeaponSlot = weaponSlot;
		lastGunEquipSlot = gunEquipSlot;


		if (!weapon.CannotReload && clipContent.Length-1 <= 0 && !weapon.notReloadable && Shooting && !isReloading)
		{
			Reload();
		}

		if (!weapon.CannotShoot && Shooting && clipContent.Length-1 >= weapon.modes[currentMode].ammoNeeded && !cantShoot && !isReloading && clipContent.Length > 1)
        {
			
            if(SkinnedModel != null) SkinnedModel.Set(weapon.fireParam, true);
            cantShoot = true;
            await Task.DelayRealtimeSeconds(weapon.modes[currentMode].timeBeforeShooting);
            Shoot();
            isReloading = false;
        }

		
	}

	public double GetRandomNumberInRange(double minNumber, double maxNumber)
	{
		return Game.Random.NextDouble() * (maxNumber - minNumber) + minNumber;
	}


	void Recoil()
    {
        float randomx = (float)GetRandomNumberInRange(weapon.minMaxRecoilPos[0].x, weapon.minMaxRecoilPos[1].x);
        float randomy = (float)GetRandomNumberInRange(weapon.minMaxRecoilPos[0].y, weapon.minMaxRecoilPos[1].y);
        float randomz = (float)GetRandomNumberInRange(weapon.minMaxRecoilPos[0].z, weapon.minMaxRecoilPos[1].z);
        float randompitch = (float)GetRandomNumberInRange(weapon.minMaxRecoilRot[0].pitch, weapon.minMaxRecoilRot[1].pitch);
        float  randomyaw = (float)GetRandomNumberInRange(weapon.minMaxRecoilRot[0].yaw, weapon.minMaxRecoilRot[1].yaw);
        float randomroll = (float)GetRandomNumberInRange(weapon.minMaxRecoilRot[0].roll, weapon.minMaxRecoilRot[1].roll);
        recoilOffsetPos += new Vector3(randomx,randomy,randomz)*2;
        recoilOffsetRot += new Angles(randompitch,randomyaw,randomroll)*2;
    }
	int shotsFired;
	bool cantShoot;

	async void Reload()
	{
		isReloading = true;
		await Task.DelaySeconds((weapon.reloadCTime * (weapon.clipSize/weapon.reloadMount))+weapon.reloadWarmTime);
		for (int i = 0; i < weapon.clipSize; i++)
        {
            if(clipContent.Length-1 < weapon.clipSize)
            {
                clipContent = $"{clipContent}{0}";
                
                //Entity.Attributes.attributeSets[0].attributes[0].intValue--;
            }
        }
		isReloading = false;
		
	}

	public GameObject CheckShoot()
	{
        if(weapon == null) return null;
        
		Vector3 rayDirection = (SkinnedModel.GetAttachment("Tip")?? default).Forward;

		Vector3 rayPosition = (SkinnedModel.GetAttachment("Tip")?? default).Position;
        if(weapon.shootFromCam) rayPosition = GunRotate.Transform.Position;

		var sTR = Scene.Trace.Ray(rayPosition,rayPosition+(rayDirection * weapon.range)).Size(1f).IgnoreGameObject(GameObject).UseHitboxes().Run();

		return sTR.GameObject;
	}

	private Vector3 GetShotDirection()
    {
        Vector3 shotDirection = (SkinnedModel.GetAttachment("Tip")?? default).Forward;
        if (weapon.shootFromCam)
        {
            shotDirection = GunRotate.Transform.World.Forward;
        }
        if (weapon.bulletStats[int.Parse($"{clipContent[1]}")].spreadX + weapon.bulletStats[int.Parse($"{clipContent[1]}")].spreadY > 0f)
        {
            float randomPosX = (float)GetRandomNumberInRange(-weapon.bulletStats[int.Parse($"{clipContent[1]}")].spreadX, weapon.bulletStats[int.Parse($"{clipContent[1]}")].spreadX); 
            float randomPosY = (float)GetRandomNumberInRange(-weapon.bulletStats[int.Parse($"{clipContent[1]}")].spreadY, weapon.bulletStats[int.Parse($"{clipContent[1]}")].spreadY);

            shotDirection += (SkinnedModel.GetAttachment("Tip")?? default).Up * randomPosY;
            shotDirection += (SkinnedModel.GetAttachment("Tip")?? default).Right * randomPosX;
        }
        return shotDirection;
    }

	async void Shoot()
    {
        if (!weapon.CannotReload && clipContent.Length-1 <= 0 && !weapon.notReloadable)
        {
            Reload();
            return;
        }
        if (!cantShoot)
        {
            shotsFired = 0;
        }
        else
        {
            if(SkinnedModel != null) SkinnedModel.Set(weapon.fireParam, true);
        }
        cantShoot = true;

		

        Recoil();
        if(weapon.flash!=null)
        {
            GameObject mF = weapon.flash.Clone();
            mF.Transform.Position = (SkinnedModel.GetAttachment("Tip")?? default).Position;
            mF.Transform.Rotation = (SkinnedModel.GetAttachment("Tip")?? default).Rotation;
        }
        if(weapon.sound!=null)
        {
			GameObject sound = new GameObject();
			sound.Name = "Sound";
            SoundPointComponent sP = sound.Components.GetOrCreate<SoundPointComponent>();
            sP.Transform.Position = Transform.World.Position;
            sP.SoundEvent = weapon.sound;
            sP.StartSound();
        }
        
        
        for (int i = 0; i < weapon.bulletStats[int.Parse($"{clipContent[1]}")].shotsPer; i++)
        {
            Vector3 rayDirection = GetShotDirection();
            
            if (!weapon.bulletStats[int.Parse($"{clipContent[1]}")].isProjectile)
            {
                
                Vector3 rayPosition = (SkinnedModel.GetAttachment("Tip")?? default).Position;
                if(weapon.shootFromCam) rayPosition = GunRotate.Transform.Position;

				var sTR = Scene.Trace.Ray(rayPosition,rayPosition+(rayDirection * weapon.range)).Size(1f).IgnoreGameObject(GameObject).UseHitboxes().Run(); //.WithoutTags(bulletIgnore.ToArray())
                if(sTR.GameObject != null || sTR.Hitbox != null)
                {
                    float dm = 1;
                    GameObject bh = bHoleDB.FindBulletHoleByMaterial(sTR.Surface.ResourceName);
                    if(bh!=null)
                    {
                        
                        GameObject bulletHole = bh.Clone();
                        bulletHole.Transform.Position = sTR.EndPosition;
                        bulletHole.Transform.Rotation = Rotation.LookAt(-sTR.Normal);
                        GameObject parent = sTR.GameObject;
                        if ( sTR.Hitbox != null)
                        {
                            IEnumerable<string> tags = sTR.Hitbox.Tags.TryGetAll();
                            string tag = "1";
                            foreach(string s in tags)
                            {
                                tag = s;
                                break; 
                            }
                            dm = tag.ToFloat();
                            try
                            {
                                SkinnedModelRenderer renderer = sTR.Hitbox.GameObject.Components.Get<SkinnedModelRenderer>();
                                parent = renderer.GetBoneObject( sTR.Hitbox.Bone );
                            }catch{}
                            
                        }

                        bulletHole.SetParent(parent);
                    }
                    
                    Health healthScript = sTR.GameObject.Components.Get<Health>();
                    if (healthScript != null && !sTR.GameObject.Tags.Has("player"))
                    {
                        healthScript.DealDamage(weapon.bulletStats[int.Parse($"{clipContent[1]}")].damage*dm);
                        //healthScript.bleedAmount += weapon.bulletStats[int.Parse($"{clipContent[1]}")].bleedDamage*dm;
                    }

                    

                    Rigidbody rb = sTR.GameObject.Components.Get<Rigidbody>();
                    if (rb != null)
                    {
                        rb.ApplyForceAt(sTR.EndPosition,rayDirection * weapon.bulletStats[int.Parse($"{clipContent[1]}")].shotsForce);             
                    }
					if(sTR.Body != null)
                    {
                        sTR.Body.ApplyImpulseAt( sTR.HitPosition, sTR.Direction * weapon.bulletStats[int.Parse($"{clipContent[1]}")].shotsForce * 0.0001f * sTR.Body.Mass.Clamp( 0, 200 ) );
                    }
                }
            }
            else
            {
                
                //GameObject projectile = Instantiate(weapon.bulletStats[int.Parse($"{clipContent[1]}")].projectile, Tip.position, Quaternion.identity);
				GameObject projectile = weapon.bulletStats[int.Parse($"{clipContent[1]}")].projectile.Clone();
                
				projectile.Transform.Position = (SkinnedModel.GetAttachment("Tip")?? default).Position;
                projectile.Transform.Rotation = Rotation.LookAt(rayDirection);
                if (!weapon.bulletStats[int.Parse($"{clipContent[1]}")].selfPropelled) projectile.Components.Get<Rigidbody>().ApplyForce(rayDirection * weapon.bulletStats[int.Parse($"{clipContent[1]}")].projectileSpeed);
            }
        }
        
        shotsFired++;
        
        for(int b = 0; b < weapon.modes[currentMode].ammoUse; b++) clipContent = clipContent.Length > 1 ? $".{clipContent.Substring(2)}" : ".";

        if (shotsFired < weapon.modes[currentMode].shotsPerShoot)
        {
			await Task.DelaySeconds(weapon.shootTime);
            Shoot();
        }
        else
        {
			await Task.DelaySeconds(weapon.modes[currentMode].buttonHold ? weapon.shootTime : 0.5f);
            cantShoot = false;
        }

    }

	public async void GunChange()
	{
		gunEquipSlot = Entity.getEquip("weapons");
		
        if(weapon!=null)
        {
            pauseGun = true;
            if(SkinnedModel != null) SkinnedModel.Set(weapon.unDeployParam, true);
            await Task.DelayRealtimeSeconds(weapon.deployTime);
        }

        if (gunEquipSlot == -1)
        {
            weapon = null;
            SkinnedModel.Enabled = false;
        }
        else
        {
			SkinnedModel.Enabled = true;
            weapon = CustomFunctions.GetResource<Weapon>(Entity.Container[weaponSlot].Categories, "weapon");
            SkinnedModel.Model = Model.Load($"models/{string.Join("/", Entity.Container[weaponSlot].Categories)}.vmdl");
            SkinnedModel.AnimationGraph = AnimationGraph.Load($"models/{string.Join("/", Entity.Container[weaponSlot].Categories)}.vanmgrph");
            clipContent = ".";
            clipContent += Entity.Container[weaponSlot].AttributeSets[0].attributes[0].stringValue;
        }
        GunRotate.Transform.LocalPosition = weapon.targetPosNPC + recoilOffsetPos;
        SkinnedModel.Transform.LocalRotation = weapon.targetRotIdle;
        await Task.DelayRealtimeSeconds(weapon.deployTime);
        pauseGun = false;
	}
	void GunPos()
	{
		Rotation lookatRot = Rotation.LookAt(Scene.Camera.Transform.Position-SkinnedModel.Transform.Position );
		recoilOffsetPos = Vector3.Lerp(recoilOffsetPos,Vector3.Zero,weapon.recoilReset*Time.Delta);
		recoilOffsetRot = Angles.Lerp(recoilOffsetRot,Angles.Zero,weapon.recoilReset*Time.Delta);
		GunRotate.Transform.LocalPosition = weapon.targetPosNPC + recoilOffsetPos;
		GunRotate.Transform.Rotation = lookatRot;
		SkinnedModel.Transform.LocalRotation = weapon.targetRotIdle + recoilOffsetRot;
	}

    public void DropWeapon(bool removeOld = true)
    {
        
		GameObject Gun = CustomFunctions.SpawnEntity(Entity.Container[weaponSlot]);
		Gun.Transform.Position = SkinnedModel.Transform.Position;
		Gun.Transform.Rotation = SkinnedModel.Transform.Rotation;
        SkinnedModel.Destroy();
        if(removeOld) Entity.Container.RemoveAt(weaponSlot);
        Destroy();
    }
}
