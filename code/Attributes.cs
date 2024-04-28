using System;
using Sandbox;
using Sandbox.Utility.Svg;

public sealed class Attributes : Component
{
	[Property] public List<Attribute> attributes {get;set;}

	public Attribute getAttribute(string name)
	{
		foreach(Attribute attribute in attributes)
		{
			if(attribute.AttributeName == name)
			{
				return attribute;
			}
		}
		throw new InvalidOperationException("Failed to Get Attribute From Name");
	}
    public static dynamic ConvertFromString(string input, Attribute.AttributeType desiredType)
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
		public AttributeType attributeType;
        public float intValue { get; set; }
        public float floatValue { get; set; }
        public string stringValue { get; set; }
        public Vector3 vector3Value { get; set; }
        public bool boolValue { get; set; }
        public T GetValue<T>()
        {
            switch (attributeType)
            {
                case AttributeType.INT when typeof(T) == typeof(int):
                    return (T)(object)intValue;
                case AttributeType.FLOAT when typeof(T) == typeof(float):
                    return (T)(object)floatValue;
                case AttributeType.STRING when typeof(T) == typeof(string):
                    return (T)(object)stringValue;
                case AttributeType.VECTOR3 when typeof(T) == typeof(Vector3):
                    return (T)(object)vector3Value;
                case AttributeType.BOOL when typeof(T) == typeof(bool):
                    return (T)(object)boolValue;
                default:
                    throw new InvalidOperationException("Incorrect Type for AV");
            }
        }
		public void SetValue<T>(T value)
        {
            switch (attributeType)
            {
                case AttributeType.INT when value is int:
                    intValue = (int)(object)value;
                    break;
                case AttributeType.FLOAT when value is float:
                    floatValue = (float)(object)value;
                    break;
                case AttributeType.STRING when value is string:
                    stringValue = (string)(object)value;
                    break;
                case AttributeType.VECTOR3 when value is Vector3:
                    vector3Value = (Vector3)(object)value;
                    break;
                case AttributeType.BOOL when value is bool:
                    boolValue = (bool)(object)value;
                    break;
                default:
                    throw new ArgumentException("Incorrect Type for AV");
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