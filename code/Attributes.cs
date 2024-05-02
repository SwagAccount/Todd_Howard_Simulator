using System;

using System.ComponentModel.Design.Serialization;
using Sandbox;

public sealed class Attributes : Component
{
	[Property] public List<AttributeSet> attributeSets {get;set;}
    [Property] public List<PerkEffector> perkEffectors {get;set;}

    private Ids ids;

    [Property] public Attributes player {get;set;}

	protected override void OnStart()
	{
        ids = Components.Get<Ids>();
		player = CommandDealer.getCommandDealer().player;
        perkEffectors = new List<PerkEffector>();
        foreach(AttributeSet aS in attributeSets)
        {
            foreach(Attribute a in aS.attributes)
            {
                a.attributes = this;
            }
        }
        foreach (var attribute in player.getAttributeSet("perks").attributes)
        {
            Perk perk = ResourceLibrary.Get<Perk>($"gameresources/perks/{attribute.AttributeName}.perk");
            if(perk!=null)
            {
                foreach(PerkEffector perkeffector in perk.Effectors)
                {
                    PerkEffector perkEffector = new PerkEffector();
                    perkEffector.Catagory = perkeffector.Catagory;
                    perkEffector.attributeName = perkeffector.attributeName;
                    perkEffector.attributeSet = perkeffector.attributeSet;
                    perkEffector.EffectType = perkeffector.EffectType;
                    perkEffector.Effector = perkeffector.Effector;
                    bool addEffector = true;
                    for (int i = 0; i < ids.Categories.Count && i < perkEffector.Catagory.Count; i++)
                    {
                        if(ids.Categories[i] != perkEffector.Catagory[i])
                            addEffector = false;
                    }
                    if(!addEffector) break;
                    perkEffector.perkName = attribute.AttributeName;
                    perkEffector.multiply = (int)attribute.intValue;
                    perkEffectors.Add(perkEffector);
                }
                
            }
        }
        perkEffectors = perkEffectors.OrderBy(x => x.EffectType).ToList();
	}
    protected override void OnUpdate()
    {

    }
	public class AttributeSet
    {
        int changed = 0;
        public string setName {get;set;} = "default";
        public List<Attribute> attributes {get;set;}
    }
    public AttributeSet getAttributeSet(string name)
    {
        foreach(AttributeSet aS in attributeSets)
        {
            if(aS.setName == name) return aS;
        }
        return attributeSets[0];
    }
	public Attribute getAttribute(string name, string setName = "default")
	{
		foreach(Attribute attribute in getAttributeSet(setName).attributes)
		{
			if(attribute.AttributeName == name)
			{
				return attribute;
			}
		}
        Log.Info("Failed to Get Attribute From Name");
		return null;
	}
    public object ConvertFromString(string input, Attribute.AttributeType desiredType)
    {
        switch (desiredType)
        {
            case Attribute.AttributeType.INT:
                return ConvertToInt(input);
            case Attribute.AttributeType.FLOAT:
                return ConvertToFloat(input);
            case Attribute.AttributeType.STRING:
                return input;
            case Attribute.AttributeType.VECTOR3:
                return ConvertToVector3(input);
            case Attribute.AttributeType.BOOL:
                return ConvertToBool(input);
            default:
                throw new InvalidOperationException("Unsupported type conversion requested.");
        }
    }
     private static int ConvertToInt(string input)
    {
        int value;
        if(Int32.TryParse(input, out value))
        {
            return value;
        }
        else
        {
            throw new InvalidOperationException("Invalid Int.");
        }
    }

    private static float ConvertToFloat(string input)
    {
        float value;
        if(float.TryParse(input, out value))
        {
            return value;
        }
        else
        {
            throw new InvalidOperationException("Invalid Float.");
        }
    }

    private static Vector3 ConvertToVector3(string input)
    {
        string[] xyz = input.Split(',');
        if(xyz.Length == 3)
        {
            List<float> v3 = new List<float>();
            foreach(string s in xyz)
            {
                float value;
                if(float.TryParse(s, out value))
                {
                    v3.Add(value);
                }
                else
                {
                    throw new InvalidOperationException("Vector 3 contains invalid number.");
                }
            }
            return new Vector3(v3[0], v3[1],v3[2]);
        }
        else
        {
            throw new InvalidOperationException("Vector 3 not formated correctly.");
        }
    }

    private static bool ConvertToBool(string input)
    {
        return input == "True";
    }

	public class Attribute
	{
		public string AttributeName { get; set; }
		public AttributeType attributeType {get;set;}
        public float intValue { get; set; }
        public float floatValue { get; set; }
        public string stringValue { get; set; }
        public Vector3 vector3Value { get; set; }
        public Attributes attributes { get; set; }
        public bool boolValue { get; set; }
        public bool changed { get; set;} = true;
        public object GetValue()
        {
            if(!changed) return null;
            changed = false;
            switch (attributeType)
            {
                case AttributeType.INT:
                    return (object)intValue;
                case AttributeType.FLOAT:
                    {
                        float returnFloat = floatValue;
                        foreach(PerkEffector perkEffector in attributes.perkEffectors)
                        {
                            if(attributes.player.getAttribute(perkEffector.perkName, "perks") != null)
                            {
                                switch(perkEffector.EffectType)
                                {
                                    case EffectType.ADD:
                                        returnFloat += perkEffector.Effector*perkEffector.multiply;
                                        break;
                                    case EffectType.MULTIPLY:
                                        returnFloat *= perkEffector.Effector*perkEffector.multiply;
                                        break;
                                    case EffectType.SET:
                                        returnFloat = perkEffector.Effector*perkEffector.multiply;
                                        break;
                                }
                            }
                            else returnFloat = 0;
                        }
                        return (object)returnFloat;
                    }  
                case AttributeType.STRING:
                    return (object)stringValue;
                case AttributeType.VECTOR3:
                    return (object)vector3Value;
                case AttributeType.BOOL:
                    return (object)boolValue;
                default:
                    throw new InvalidOperationException("Incorrect Type for AV");
            }
        }
		public void SetValue(object value)
        {
            changed = true;
            switch (attributeType)
            {
                case AttributeType.INT:
                    if (value is int intValue) this.intValue = intValue;
                    else throw new ArgumentException("Invalid type for INT");
                    break;
                case AttributeType.FLOAT:
                    if (value is float floatValue) this.floatValue = floatValue;
                    else throw new ArgumentException("Invalid type for FLOAT");
                    break;
                case AttributeType.STRING:
                    if (value is string stringValue) this.stringValue = stringValue;
                    else throw new ArgumentException("Invalid type for STRING");
                    break;
                case AttributeType.VECTOR3:
                    if (value is Vector3 vector3Value) this.vector3Value = vector3Value;
                    else throw new ArgumentException("Invalid type for VECTOR3");
                    break;
                case AttributeType.BOOL:
                    if (value is bool boolValue) this.boolValue = boolValue;
                    else throw new ArgumentException("Invalid type for BOOL");
                    break;
                default:
                    throw new InvalidOperationException("Unsupported type conversion requested.");
            }
        }

		

		public enum AttributeType
		{
			INT,
			FLOAT,
			STRING,
			VECTOR3,
			BOOL
		}
    }
}