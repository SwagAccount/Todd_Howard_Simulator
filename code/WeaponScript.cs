using System;
using Sandbox;

public sealed class WeaponScript : Component
{
	[Property] public Weapon weapon {get;set;}
	[Property] public SkinnedModelRenderer SkinnedModel {get;set;}
	[Property] public PlayerController playerController {get;set;}
	[Property] public bool inInv {get;set;}
	[Property] public int WeaponIndex {get;set;}
	[Property] public bool canShoot {get;set;}
	[Property] private List<string> bulletIgnore{ get; set; }
	[Property] private GameObject empty { get; set; }
	[Property] private FollowTransform leftHand { get; set; }
	[Property] private FollowTransform rightHand { get; set; }
	[Property] public GameObject noGunHandLPos { get; set; }
	[Property] public GameObject noGunHandRPos { get; set; }
	[Property] private GameObject gunSound { get; set; }
	[Property] private BulletHoleDB bHoleDB { get; set; }
	[Property] public Entity playerEntity { get; set; }
	[Property] private CameraComponent cam { get; set; }
	[Property] private float swaySmooth { get; set; }  = 0.5f;
	[Property] private float cameraBlockDis { get; set; }  = 50f;
	private float mainFov { get; set; }  = 100f;
	[Property] private float runFovAdd { get; set; }  = 10f;
	[Property] private float bulletHoleDuration { get; set; }  = 30f;
	[Property] private List<string> ignoredTags { get; set; }  = new();
	[Property] public int selectedGunID;
	string lastSG;
    bool startreload;
    Vector3 gpos;
	Angles gang;
    [Property] public bool pauseGun {get;set;}= false;
    int gunEquipSlot;
    int lastGunEquipSlot = -1;
	protected override void OnStart()
	{
		playerController = GameObject.Parent.Parent.Components.Get<PlayerController>();
        playerController.weaponScript = this;
		playerEntity = GameObject.Parent.Parent.Components.Get<Entity>();
        bHoleDB = ResourceLibrary.Get<BulletHoleDB>("gameresources/bhole.bhdb");
	}
	[Property] int currentMode;
	[Property] int bulletType;
	[Property] string clipContent;

    [Property] int weaponSlot;
    int lastWeaponSlot = -1;
    bool lastInInv;
	protected override void OnUpdate()
	{
        gunEquipSlot = playerEntity.getEquip("weapons");
        if(gunEquipSlot > -1 && !inInv) weaponSlot = playerEntity.Equips[gunEquipSlot].GetContainerIndex(playerEntity);


        if((gunEquipSlot != lastGunEquipSlot && !inInv) || weaponSlot != lastWeaponSlot || inInv != lastInInv)
        {
            changeGun();
        }
        
        mainFov = 90; //settings.fovValue
        GunPosition();
        if(!pauseGun)
        {
            if(weapon!=null)
            {

                gunInput();
                
                if(weapon.LoadedParam != null) SkinnedModel.Set(weapon.LoadedParam, clipContent.Length-1 > 0);
                if(gunEquipSlot != -1 && !inInv)
                {
                    playerEntity.Container[WeaponIndex].AttributeSets[0].attributes[0].stringValue = clipContent.Length > 1 ? clipContent.Substring(1) : "";
                    playerEntity.Container[WeaponIndex].AttributeSets[0].attributes[1].intValue = currentMode;
                    playerEntity.Container[WeaponIndex].AttributeSets[0].attributes[2].intValue = bulletType;
                }
                
            }
        }

        

        
        lastInInv = inInv;
        lastWeaponSlot = weaponSlot;
        lastGunEquipSlot = gunEquipSlot;
		
	}
	int ammoIndex;
	[Property] bool isReloading {get;set;}
    bool cantShoot;
    bool delayingShot;
	float targetFov;
	int shotsFired = 0;
    Vector3 recoilOffsetPos;
    Angles recoilOffsetRot;
    
	async void gunInput()
	{
        
        if(weapon.CannotShoot && weapon.CannotReload)
        {
            return;
        }
		if(!weapon.CannotReload) ammoIndex = playerEntity.Attributes.getAttributeIndex(weapon.bulletStats[bulletType].ammoType, "default");

		if (Input.Pressed("changeAmmoType") && !isReloading)
        {
            bulletType++;
            if (bulletType >= weapon.bulletStats.Count)
            {
                bulletType = 0;
            }
        }
        if (Input.Pressed("changeMode"))
        {
            currentMode++;
            if (currentMode >= weapon.modes.Count)
            {
                currentMode = 0;
            }
        }

        if (weapon.hasFakeProjectile)
        {
            //playerEntity.Attributes.attributeSets[0].attributes[ammoIndex].stringValue.Lengthweapon.fakeProjectile.Enabled = clipContent.Length-1 > 0 || isReloading;
        }
		if (!weapon.notReloadable)
        {
            if(!weapon.CannotReload && !cantShoot && Input.Pressed("reload") && playerEntity.Attributes.attributeSets[0].attributes[ammoIndex].intValue > 0 && !isReloading)
            {
                Log.Info("Try Reload");
                toReload();
            }
        }
        if (!weapon.CannotShoot && canShoot && Input.Pressed("attack1") && clipContent.Length-1 >= weapon.modes[currentMode].ammoNeeded && !delayingShot && !cantShoot && (!isReloading || weapon.interuptReload))
        {
            if(SkinnedModel != null) SkinnedModel.Set(weapon.fireParam, true);
            delayingShot = true;
            await Task.DelayRealtimeSeconds(weapon.modes[currentMode].timeBeforeShooting);
            Shoot();
            isReloading = false;
        }
        else if (!weapon.CannotShoot && weapon.modes[currentMode].buttonHold && canShoot && Input.Down("attack1") && clipContent.Length-1 >= weapon.modes[currentMode].ammoNeeded && !delayingShot && !cantShoot && ((!isReloading && clipContent.Length > 1) || weapon.interuptReload))
        {
            if(SkinnedModel != null) SkinnedModel.Set(weapon.fireParam, true);
            delayingShot = true;
            cantShoot = true;
            await Task.DelayRealtimeSeconds(weapon.modes[currentMode].timeBeforeShooting);
            Shoot();
            isReloading = false;
        }
        
	}
	async void toReload()
    {
        if (!weapon.cantOverReload && clipContent.Length-1 >= weapon.clipSize)
        {
            return;
        }
        isReloading = true;
        
		if(clipContent.Length-1 > 0)
        {
            if(SkinnedModel != null) SkinnedModel.Set(weapon.reloadCParam, true);
        }
        else
        {
            if(SkinnedModel != null) SkinnedModel.Set(weapon.reloadNoCParam, true);
        }
        await Task.DelayRealtimeSeconds(weapon.reloadWarmTime);
        Reload();
        
    }
	public double GetRandomNumberInRange(double minNumber, double maxNumber)
	{
		return Game.Random.NextDouble() * (maxNumber - minNumber) + minNumber;
	}
	async void Reload()
    {
        
        cantShoot = false;
        shotsFired = 0;
        float length = weapon.reloadNoCTime;
        if(clipContent.Length-1 > 0)
        {
            length = weapon.reloadCTime;
        }
        if (isReloading)
		{
			await Task.DelayRealtimeSeconds(length);
			ResetReload();
		}
    }
    void Recoil()
    {
        float randomx = (float)GetRandomNumberInRange(weapon.minMaxRecoilPos[0].x, weapon.minMaxRecoilPos[1].x);
        float randomy = (float)GetRandomNumberInRange(weapon.minMaxRecoilPos[0].y, weapon.minMaxRecoilPos[1].y);
        float randomz = (float)GetRandomNumberInRange(weapon.minMaxRecoilPos[0].z, weapon.minMaxRecoilPos[1].z);
        float randompitch = (float)GetRandomNumberInRange(weapon.minMaxRecoilRot[0].pitch, weapon.minMaxRecoilRot[1].pitch);
        float  randomyaw = (float)GetRandomNumberInRange(weapon.minMaxRecoilRot[0].yaw, weapon.minMaxRecoilRot[1].yaw);
        float randomroll = (float)GetRandomNumberInRange(weapon.minMaxRecoilRot[0].roll, weapon.minMaxRecoilRot[1].roll);
        recoilOffsetPos += new Vector3(randomx,randomy,randomz);
        recoilOffsetRot += new Angles(randompitch,randomyaw,randomroll);
    }
	private Vector3 GetShotDirection()
    {
        cam = Scene.Camera;
        Vector3 shotDirection = (SkinnedModel.GetAttachment("Tip")?? default).Forward;
        if (weapon.shootFromCam)
        {
            shotDirection = cam.Transform.World.Forward;
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
        delayingShot = false;
        if (!weapon.CannotReload && clipContent.Length-1 <= 0 && !weapon.notReloadable)
        {
            toReload();
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
        if(isReloading) await Task.Frame();
        Recoil();
        if(weapon.flash!=null)
        {
            GameObject mF = weapon.flash.Clone();
            mF.Transform.Position = (SkinnedModel.GetAttachment("Tip")?? default).Position;
            mF.Transform.Rotation = (SkinnedModel.GetAttachment("Tip")?? default).Rotation;
        }
        if(weapon.sound!=null)
        {
            SoundPointComponent sP = gunSound.Clone().Components.Get<SoundPointComponent>();
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
                if(weapon.shootFromCam) rayPosition = cam.Transform.Position;

				var sTR = Scene.Trace.Ray(rayPosition,rayPosition+(rayDirection * weapon.range)).Size(1f).IgnoreGameObject(playerController.GameObject).WithoutTags(bulletIgnore.ToArray()).UseHitboxes().Run();
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
                    //sTR.Surface.ResourceName
                    //bulletHole.transform.parent = bulletHoleParent.transform;
                    

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
			await Task.DelaySeconds(weapon.shootTime);
            ResetShoot();
        }

    }
	private void ResetShoot()
    {
        cantShoot = false;
        /*
        if (clipContent.Length-1 <= 0 && weapon.letGoAtZeroAmmo)
        {
            weapon = null;
            inv.weapons[inv.currentWeapon] = "";
            inv.weaponsData[inv.currentWeapon] = new GunSaveData();
            inv.threeDinv.deleteItem(inv.threeDinv.currentEquip);
        }
        */
    }
	private void ResetReload()
    {
        if (isReloading)
        {
            for (int i = 0; i < weapon.reloadMount; i++)
            {
                if(clipContent.Length-1 < weapon.clipSize && playerEntity.Attributes.attributeSets[0].attributes[ammoIndex].intValue > 0)
                {
                    clipContent = $"{clipContent}{bulletType}";
                    
                    playerEntity.Attributes.attributeSets[0].attributes[ammoIndex].intValue--;
                }
            }
            if (clipContent.Length-1 >= weapon.clipSize-(weapon.reloadTillFull ? 0 : 1) || playerEntity.Attributes.attributeSets[0].attributes[ammoIndex].intValue <= 0)
            {
                isReloading = false;
                if(weapon.stopReload != "" && weapon.stopReload != null) SkinnedModel.Set(weapon.stopReload, true);
            }
            else
            {
                Reload();
            }
        }
        
    }

    Angles sway;
    float bob;
    float bobPosZ;
    float bobPosY;
	void GunPosition()
	{
		Vector3 targetPos = Vector3.Zero;
		Angles targetAngles = Angles.Zero;
 		if(Input.Down("attack2"))
		{
			targetFov = mainFov * weapon.fovMult;
			targetPos = weapon.targetPosAim;
			targetAngles = weapon.targetRotAim;
		}
		else
		{
			targetFov = mainFov;
			targetPos = weapon.targetPosIdle;
			targetAngles = weapon.targetRotIdle;
		}
        float rayDis = Vector3.DistanceBetween(SkinnedModel.Transform.World.Position,(SkinnedModel.GetAttachment("Tip")?? default).Position)+1;
        Vector3 forward = cam.Transform.World.Forward;//((SkinnedModel.GetAttachment("Tip")?? default).Position - SkinnedModel.Transform.Position).Normal;
        var trace = Scene.Trace.Ray(
            SkinnedModel.Transform.Position-(forward*5),
            SkinnedModel.Transform.Position+(forward*rayDis))
            .Size(1f)
            .IgnoreGameObject(playerController.GameObject)
            .UseHitboxes()
            .Run();
        float A = MathF.Sqrt(rayDis+(rayDis+1));
        float angle = gunEquipSlot == -1 ? 0 : (trace.Hit && !float.IsNaN(-MathF.Acos(trace.Distance/(rayDis+1))) && !float.IsNaN(-MathF.Acos(trace.Distance/(rayDis+1)))) ? ((-MathF.Acos(trace.Distance/(rayDis+1)) * 180 / MathF.PI) - cam.GameObject.Parent.Transform.LocalRotation.Angles().pitch) : 0f;

        recoilOffsetPos = Vector3.Lerp(recoilOffsetPos,Vector3.Zero,weapon.recoilReset*Time.Delta);
        recoilOffsetRot = Angles.Lerp(recoilOffsetRot,Angles.Zero,weapon.recoilReset*Time.Delta);

        float flatVel = new Vector3(playerController.Velocity.x,playerController.Velocity.y,0).Length;
        bob += Time.Delta * flatVel * weapon.BobSpeed * 0.75f;
        
        if(flatVel > 0 && playerController.IsOnGround)
        {
            bobPosZ = MathF.Sin(bob*0.05f)*weapon.BobDis*0.2f * (Input.Down("attack2") ? 0.25f:1) ;
            bobPosY = MathF.Cos(bob*0.025f)*weapon.BobDis*0.1f * (Input.Down("attack2") ? 0.25f:1);
        }
        else
        {
            bob = 0;
            bobPosZ = bobPosZ < 0 ? bobPosZ+Time.Delta*0.2f : bobPosZ-Time.Delta*0.2f;
            bobPosY = bobPosY < 0 ? bobPosY+Time.Delta*0.2f : bobPosY-Time.Delta*0.2f;
        }
        

        //cam.FieldOfView = MathX.Lerp(cam.FieldOfView, targetFov,weapon.posSpeed*Time.Delta);
		gpos = Vector3.Lerp(gpos, targetPos, weapon.posSpeed*Time.Delta);
		gang = Angles.Lerp(gang, targetAngles, weapon.rotSpeed*Time.Delta);
        sway = playerController.Enabled ? Angles.Lerp(sway,new Angles(Input.AnalogLook.pitch*weapon.swayPitch*Preferences.Sensitivity,Input.AnalogLook.yaw*Preferences.Sensitivity*weapon.swayYaw,0),swaySmooth*Time.Delta) : Angles.Zero;
        
        SkinnedModel.Transform.LocalPosition = gpos+recoilOffsetPos+new Vector3(0,bobPosY,bobPosZ);
        SkinnedModel.Transform.LocalRotation = gang+recoilOffsetRot+(weapon.AvoidAngle*angle)+sway * (Input.Down("attack2") ? 0.25f:1);
	}
    async void changeGun()
    {
        gunEquipSlot = playerEntity.getEquip("weapons");
        if(weapon!=null)
        {
            pauseGun = true;
            if(SkinnedModel != null) SkinnedModel.Set(weapon.unDeployParam, true);
            await Task.DelayRealtimeSeconds(weapon.deployTime);
        }

        if(inInv)
        {
            weapon = ResourceLibrary.Get<Weapon>("gameresources/phone.weapon");
            SkinnedModel.Model = Model.Load($"models/phoneweapon.vmdl");
            SkinnedModel.AnimationGraph = AnimationGraph.Load($"models/phoneweapon.vanmgrph");
            clipContent = ".";
        }
        else if (gunEquipSlot == -1)
        {
            weapon = ResourceLibrary.Get<Weapon>("gameresources/punch.weapon");
            SkinnedModel.Model = Model.Load($"models/punch.vmdl");
            SkinnedModel.AnimationGraph = AnimationGraph.Load($"models/punch.vanmgrph");
            clipContent = ".0";
        }
        else
        {
            weapon = CustomFunctions.GetResource<Weapon>(playerEntity.Container[weaponSlot].Categories, "weapon");
            SkinnedModel.Model = Model.Load($"models/{string.Join("/", playerEntity.Container[weaponSlot].Categories)}.vmdl");
            SkinnedModel.AnimationGraph = AnimationGraph.Load($"models/{string.Join("/", playerEntity.Container[weaponSlot].Categories)}.vanmgrph");
            clipContent = ".";
            clipContent += playerEntity.Container[weaponSlot].AttributeSets[0].attributes[0].stringValue;
            currentMode = playerEntity.Container[weaponSlot].AttributeSets[0].attributes[1].intValue;
            bulletType = playerEntity.Container[weaponSlot].AttributeSets[0].attributes[2].intValue;
        }
        SkinnedModel.Transform.LocalPosition = weapon.targetPosIdle;
        SkinnedModel.Transform.LocalRotation = weapon.targetRotIdle;
        await Task.DelayRealtimeSeconds(weapon.deployTime);
        pauseGun = false;
		
    }
}