
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
        newEntity.id = entity.Ids.sceneID;
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

    public static GameObject GetChildAtPath(GameObject parent, List<int> path)
    {

        GameObject current = parent;

        // Iterate over each index in the path
        foreach (int index in path)
        {
            if (index < 0 || index >= current.Children.Count)
            {
                Log.Info("Index out of range: " + index);
                return null;
            }

            current = current.Children[index];
        }

        return current;
    }

    public static float GetAttributeFloatUsingIndexs(Attributes attributes, int set, int attribute)
    {
        return attributes.attributeSets[set].attributes[attribute].floatValue;
    }

    
    public static float CalculateEffectiveDistance(float recoilAngle, float bulletSpread, float targetRadius, float desiredHitProbability, bool auto)
    {
        
        bulletSpread = MathF.Atan(bulletSpread) + (recoilAngle * (MathF.PI/180f) * (auto ? 6 : 1));
        Log.Info(bulletSpread * (180f/MathF.PI));
        for(int i = 0; i < 1000; i++)
        {
            float distance = 1000-i;
            float spreadRadius = distance * MathF.Tan(bulletSpread / 2);
            float hitProbability = targetRadius / (2 * spreadRadius);
            if (spreadRadius == 0) hitProbability = 1;
            else hitProbability = MathF.Max(0, MathF.Min(1, hitProbability));

            if(hitProbability > desiredHitProbability) return distance;
        }

        return 0;
        
    }
}