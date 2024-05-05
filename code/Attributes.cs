using System;

using System.ComponentModel.Design.Serialization;
using Sandbox;

public sealed class Attributes : Component
{
	[Property] public List<AttributeSet> attributeSets {get;set;}
    [Property] public List<PerkEffector> perkEffectors {get;set;}

    private Ids ids;

    [Property] public Attributes player {get;set;}

    public void ApplyPerksToSets()
    {
        foreach(AttributeSet attributeSet in attributeSets)
        {
            if(attributeSet.applyPerks)
            {
                foreach(Attribute attribute in attributeSet.attributes)
                {
                    attribute.ApplyPerks();
                }
            }
        }
    }

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
                    perkeffector.perkName = attribute.AttributeName;
                    bool addEffector = true;
                    for (int i = 0; i < ids.Categories.Count && i < perkeffector.Catagory.Count; i++)
                    {
                        if(ids.Categories[i] != perkeffector.Catagory[i])
                            addEffector = false;
                    }
                    if(!addEffector) break;
                    perkEffectors.Add(perkeffector);
                }
                
            }
        }
        perkEffectors = perkEffectors.OrderBy(x => x.EffectType).ToList();

        ApplyPerksToSets();
	}
    protected override void OnUpdate()
    {

    }
    public static AttributeSet LoadAttributeSet(SavedAttributeSet savedAttributeSet)
    {
        AttributeSet AttributeSet = new AttributeSet();
        savedAttributeSet.attributes = new List<SavedAttribute>();
        foreach(SavedAttribute a in savedAttributeSet.attributes)
        {
            Attribute Attribute = new Attribute();
            Attribute.AttributeName = a.AttributeName;
            Attribute.attributeType = a.attributeType;
            switch(a.attributeType)
            {
                case Attribute.AttributeType.INT:
                    Attribute.intValue = a.intValue;
                    break;
                case Attribute.AttributeType.FLOAT:
                    Attribute.floatValue = a.floatValue;
                    break;
                case Attribute.AttributeType.STRING:
                    Attribute.stringValue = a.stringValue;
                    break;
                case Attribute.AttributeType.VECTOR3:
                    Attribute.vector3Value = new Vector3(a.vector3ValueX,a.vector3ValueY,a.vector3ValueZ);
                    break;
                case Attribute.AttributeType.BOOL:
                    Attribute.boolValue = a.boolValue;
                    break;
            }
            AttributeSet.attributes.Add(Attribute);
        }

        AttributeSet.applyPerks = savedAttributeSet.applyPerks;
        AttributeSet.setName = savedAttributeSet.setName;

        return AttributeSet;
    }
    public static SavedAttributeSet SaveAttributeSet(AttributeSet attributeSet)
    {
        SavedAttributeSet savedAttributeSet = new SavedAttributeSet();
        savedAttributeSet.attributes = new List<SavedAttribute>();
        foreach(Attribute a in attributeSet.attributes)
        {
            SavedAttribute savedAttribute = new SavedAttribute();
            savedAttribute.AttributeName = a.AttributeName;
            savedAttribute.attributeType = a.attributeType;
            switch(a.attributeType)
            {
                case Attribute.AttributeType.INT:
                    savedAttribute.intValue = a.intValue;
                    break;
                case Attribute.AttributeType.FLOAT:
                    savedAttribute.floatValue = a.floatValue;
                    break;
                case Attribute.AttributeType.STRING:
                    savedAttribute.stringValue = a.stringValue;
                    break;
                case Attribute.AttributeType.VECTOR3:
                    savedAttribute.vector3ValueX = a.vector3Value.x;
                    savedAttribute.vector3ValueY = a.vector3Value.y;
                    savedAttribute.vector3ValueZ = a.vector3Value.z;
                    break;
                case Attribute.AttributeType.BOOL:
                    savedAttribute.boolValue = a.boolValue;
                    break;
            }
            savedAttributeSet.attributes.Add(savedAttribute);
        }

        savedAttributeSet.applyPerks = attributeSet.applyPerks;
        savedAttributeSet.setName = attributeSet.setName;

        return savedAttributeSet;
    }
    public class SavedAttributeSet
    {
        public bool applyPerks {get;set;} = false;
        public string setName {get;set;} = "default";
        public List<SavedAttribute> attributes {get;set;}
    }
	public class AttributeSet
    {
        public bool applyPerks {get;set;} = false;
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
	public Attribute getAttribute(string name, string setName = "default", bool logInfo = true)
	{
		foreach(Attribute attribute in getAttributeSet(setName).attributes)
		{
			if(attribute.AttributeName == name)
			{
				return attribute;
			}
		}
        if(logInfo) Log.Info("Failed to Get Attribute From Name");
		return null;
	}

    public void AddAttribute( string value, Attribute.AttributeType attributeType,string name,string attributeSet = "default")
    {
        if(getAttribute(name, attributeSet, false) != null)
        {
            Log.Info("Attribute already exists.");
            return;
        }
        Attribute attribute = new Attribute();
        attribute.attributeType = attributeType;
        attribute.AttributeName = name;

        switch (attributeType)
        {
            case Attribute.AttributeType.INT:
                attribute.intValue = ConvertToInt(value);
                break;
            case Attribute.AttributeType.FLOAT:
                attribute.floatValue = ConvertToFloat(value);
                break;
            case Attribute.AttributeType.STRING:
                attribute.stringValue = value;
                break;
            case Attribute.AttributeType.VECTOR3:
                attribute.vector3Value = ConvertToVector3(value);
                break;
            case Attribute.AttributeType.BOOL:
                attribute.boolValue = ConvertToBool(value);
                break;
        }

        getAttributeSet(attributeSet).attributes.Add(attribute);
    }

    public int getAttributeIndex(string name, string setName = "default")
	{
        int i = 0;
		foreach(Attribute attribute in getAttributeSet(setName).attributes)
		{
			if(attribute.AttributeName == name)
			{
				return i;
			}
            i++;
		}
        Log.Info("Failed to Get Attribute From Name");
		return -1;
	}

    public object ConvertFromString(string input, Attribute.AttributeType desiredType)
    {
        try
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
                    Log.Info("Unsupported type conversion requested.");
                    return null;
            }
        }
        catch
        {
            return null;
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
    public class SavedAttribute
    {
        public string AttributeName { get; set; }
		public Attribute.AttributeType attributeType {get;set;}
        public float intValue { get; set; }
        public float floatValue { get; set; }
        public float floatValuePerks { get; set; }
        public string stringValue { get; set; }
        public float vector3ValueX { get; set; }
        public float vector3ValueY { get; set; }
        public float vector3ValueZ { get; set; }
        public bool boolValue { get; set; }
    }
	public class Attribute
	{
		public string AttributeName { get; set; }
		public AttributeType attributeType {get;set;}
        public float intValue { get; set; }
        public float floatValue { get; set; }
        public float floatValuePerks { get; set; }
        public string stringValue { get; set; }
        public Vector3 vector3Value { get; set; }
        public Attributes attributes { get; set; }
        public bool boolValue { get; set; }
        public void ApplyPerks()
        {
            floatValuePerks = floatValue;
            foreach(PerkEffector perkEffector in attributes.perkEffectors)
            {
                Attribute attribute = attributes.player.getAttribute(perkEffector.perkName, "perks");
                if(attribute != null)
                {
                    float multiplier = (float)attribute.GetValue();
                    switch(perkEffector.EffectType)
                    {
                        case EffectType.ADD:
                            floatValuePerks += perkEffector.Effector*multiplier;
                                break;
                        case EffectType.MULTIPLY:
                            floatValuePerks *= perkEffector.Effector*multiplier;
                            break;
                        case EffectType.SET:
                            floatValuePerks = perkEffector.Effector*multiplier;
                            break;
                    }
                }
            }
        }
        public object GetValue()
        {
            switch (attributeType)
            {
                case AttributeType.INT:
                    return (object)intValue;
                case AttributeType.FLOAT:
                    return (object)floatValue;
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