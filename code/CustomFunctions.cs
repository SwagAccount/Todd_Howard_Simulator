
using Sandbox;
using System;
using System.ComponentModel;
using System.Globalization;
public sealed class CustomFunctions
{
    public static string CapitalizeWords(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new Exception("Input string cannot be null or empty.");
        }

        string[] words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        for (int i = 0; i < words.Length; i++)
        {
            words[i] = textInfo.ToTitleCase(words[i]);
        }

        string result = string.Join(" ", words);
        
        return result;
    }

    public static T GetResource<T>(List<string> Categories, string resourceType) where T : GameResource
    {
        string path = string.Join("/", Categories);
        return ResourceLibrary.Get<T>($"gameresources/{path}.{resourceType}");
    }

    public static SaveClasses.EntitySave SaveEntity(Entity entity)
    {
        SaveClasses.EntitySave newEntity = new SaveClasses.EntitySave(); 
        newEntity.Categories = entity.Ids.Categories;
        newEntity.displayContainer = entity.displayContainer;

        newEntity.AttributeSets = new List<Attributes.SavedAttributeSet>();
        foreach(Attributes.AttributeSet attributeSet in entity.Attributes.attributeSets)
        {
            newEntity.AttributeSets.Add(Attributes.SaveAttributeSet(attributeSet));
        }

        newEntity.Container = entity.Container;

        return newEntity;
    }
}