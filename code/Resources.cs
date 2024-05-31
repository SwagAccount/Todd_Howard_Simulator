using System;
using Sandbox;
[GameResource("Weapon", "weapon","Info for the weapon information", Icon = "construction")]
public sealed class Weapon : GameResource
{
    [Category("Weapon Configuration")][Property] public GameObject flash { get; set; }
    [Category("Weapon Configuration")][Property] public bool CannotShoot { get; set; }
    [Category("Weapon Configuration")][Property] public bool CannotReload { get; set; }
    [Category("Weapon Configuration")][Property] public bool ExactArm { get; set; }
    [Category("Weapon Configuration")][Property] public bool shootFromCam { get; set; }
    [Category("Weapon Configuration")][Property] public float shootTime { get; set; }
    [Category("Weapon Configuration")][Property] public float deployTime { get; set; }
    [Category("Weapon Configuration")][Property] public float range { get; set; } = 52493f;
    [Category("Weapon Configuration")][Property] public int clipSize { get; set; }
    [Category("Weapon Configuration")][Property] public bool hasFakeProjectile { get; set; }
    [Category("Weapon Configuration")][Property] public List<Mode> modes { get; set; } = new();
    [Category("Weapon Configuration")][Property] public List<bulletStat> bulletStats { get; set; } = new();
    [Category("Weapon Configuration")][Property] public List<Vector3> minMaxRecoilPos { get; set; }
    [Category("Weapon Configuration")][Property] public List<Angles> minMaxRecoilRot { get; set; }
    [Category("Weapon Configuration")][Property] public Vector3 targetPosIdle { get; set; }
    [Category("Weapon Configuration")][Property] public Angles targetRotIdle { get; set; }
    [Category("Weapon Configuration")][Property] public Vector3 targetPosAim { get; set; }
    [Category("Weapon Configuration")][Property] public Vector3 targetPosNPC { get; set; }
    [Category("Weapon Configuration")][Property] public Angles targetRotAim { get; set; }
    [Category("Weapon Configuration")][Property] public Vector3 targetPosRun { get; set; }
    [Category("Weapon Configuration")][Property] public Angles targetRotRun { get; set; }

    [Category("Audio and Visual Effects")][Property] public SoundEvent sound { get; set; }
    [Category("Audio and Visual Effects")][Property] public string fireParam { get; set; }
    [Category("Audio and Visual Effects")][Property] public string reloadCParam { get; set; }
    [Category("Audio and Visual Effects")][Property] public string reloadNoCParam { get; set; }
    [Category("Audio and Visual Effects")][Property] public string unDeployParam { get; set; }
    [Category("Audio and Visual Effects")][Property] public string LoadedParam { get; set; }
    [Category("Audio and Visual Effects")][Property] public float recoilReset { get; set; }
    [Category("Audio and Visual Effects")][Property] public float posSpeed { get; set; }
    [Category("Audio and Visual Effects")][Property] public float rotSpeed { get; set; }
    [Category("Audio and Visual Effects")][Property] public float swayYaw { get; set; }
    [Category("Audio and Visual Effects")][Property] public float swayPitch { get; set; }
    [Category("Audio and Visual Effects")][Property] public List<int> leftHand { get; set; }
    [Category("Audio and Visual Effects")][Property] public List<int> rightHand { get; set; }

    [Category("Reload Mechanics")][Property] public bool interuptReload { get; set; }
    [Category("Reload Mechanics")][Property] public int reloadMount { get; set; }
    [Category("Reload Mechanics")][Property] public float reloadWarmTime { get; set; }
    [Category("Reload Mechanics")][Property] public bool notReloadable { get; set; }
    [Category("Reload Mechanics")][Property] public bool reloadTillFull { get; set; }
    [Category("Reload Mechanics")][Property] public bool cantOverReload { get; set; }
    [Category("Reload Mechanics")][Property] public bool letGoAtZeroAmmo { get; set; }
    [Category("Reload Mechanics")][Property] public string stopReload { get; set; }
    [Category("Reload Mechanics")][Property] public float reloadCTime { get; set; }
    [Category("Reload Mechanics")][Property] public float reloadNoCTime { get; set; }
    [Category("Reload Mechanics")][Property] public bool runReload { get; set; }

    [Category("Performance Modifiers")][Property] public float fovMult { get; set; } = 0.66f;
    [Category("Performance Modifiers")][Property] public float sensMult { get; set; } = 1f;
    
    [System.Serializable]
    public class Mode
    {
        [Property] public string modeName {get; set;}
        [Property] public int ammoUse {get; set;}
        [Property] public float timeBetweenEachShot {get; set;}
        [Property] public float timeBeforeShooting {get; set;}
        [Property] public bool burst {get; set;}
        [Property] public bool buttonHold {get; set;}
        [Property] public float shotsPerShoot {get; set;}
        [Property] public int ammoNeeded {get; set;}
    }
	[System.Serializable]
    public class bulletStat
    {

        [Property] public string ammoType {get; set;}
        //[Header("Stats")]
        [Property] public float shotsForce {get; set;}
        [Property] public float shotsPer {get; set;}
        [Property] public float spreadX {get; set;}
        [Property] public float spreadY {get; set;}
        //[Header("Hitscan Only")]
        [Property] public float damage {get; set;}
        [Property] public float bleedDamage {get; set;}
        //[Header("Projectile Values")]
        [Property] public bool isProjectile {get; set;}
        [Property] public GameObject projectile {get; set;}
        [Property] public bool selfPropelled {get; set;}
        [Property]public float projectileSpeed {get; set;}

    }
}

[GameResource("Item", "item", "Info for game item.", Icon = "Description")]
public sealed class item : GameResource
{
    [Property] public float Weight {get;set;}
    [Property] public float Value {get;set;}
}

[GameResource("EquipTypes", "et", "Types to equip from inventory", Icon = "Description")]
public sealed class EquipTypes : GameResource
{
    [Property] public List<string> types {get;set;}
}

[GameResource("CatagoryComponents", "catagory", "Components for each catagory", Icon = "Apps")]
public sealed class CatagoryComponents : GameResource
{
    [Property] public List<Entity.Catagory> Catagories {get; set;}
}

[GameResource("Npc Logic", "logic", "Logic for an NPC", Icon = "Person")]
public sealed class NPCLogic : GameResource
{
    [Property] public Action action {get;set;}
}

[GameResource("Perk", "perk", "A perk", Icon = "Grade")]
public sealed class Perk : GameResource
{
    [Property] public List<PerkEffector> Effectors {get; set;}
}


public enum EffectType
{
    ADD,
    MULTIPLY,
    SET
}

public class PerkEffector
{
    public string perkName;
    public List<string> Catagory {get; set;}
    [Property] public string attributeName {get; set;}
    [Property] public string attributeSet {get; set;}
    public EffectType EffectType {get; set;}
    public float Effector {get;set;}
}

[GameResource("ItemTypeSort", "typesort","A way to sort an item type", Icon = "Sort")]
public sealed class ItemTypeSort : GameResource
{
    public string Name {get;set;}
    public List<ContainerUI.sortType> SortTypes {get; set;}
}



[GameResource("Player", "player","A player type", Icon = "Person")]
public sealed class PlayerReference : GameResource
{
    [Property] public GameObject gameObject {get;set;}
}

[GameResource("BulletHoleDataBase", "bhdb","Collection of all bullet holes", Icon = "Golf Course")]
public sealed class BulletHoleDB : GameResource
{
	[Property] public List<BulletHoleData> bulletHoles {get;set;} = new();
	[Property] public int defaultHole {get;set;}
	public class BulletHoleData
    {
        [Property] public List<string> bulletholeNames {get; set;}  = new();
        [Property] public List<GameObject> bulletHoles {get; set;} = new();
    }
    public int GetRandomNumberInRange(int minNumber, int maxNumber)
	{
		return (int)(Game.Random.NextInt64() * (maxNumber - minNumber) + minNumber);
	}
    public GameObject FindBulletHoleByMaterial(string mat)
    {
        int i = 0;
        if(mat != null)
        {
            foreach (BulletHoleData item in bulletHoles)
            {
                bool Yes = false;
                foreach(string s in item.bulletholeNames)
                {
                    if(s == mat)
                    {
                        Yes = true;
                        break;
                    }
                }
                if (Yes)
                {
                    if(item.bulletHoles.Count > 0) return item.bulletHoles[0]; 
                    else return null;
                }
                i++;
            }
        }
        else
        {
            return bulletHoles[defaultHole].bulletHoles[0];
        }
        return bulletHoles[defaultHole].bulletHoles[0];
    }
}