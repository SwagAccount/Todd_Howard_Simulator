
using Sandbox;
using System;
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

    public static T GetResource<T>(Ids ids, string resourceType) where T : GameResource
    {
        string path = string.Join("/", ids.Categories);
        Log.Info($"gameresources/{path}.{resourceType}");
        return ResourceLibrary.Get<T>($"gameresources/{path}.{resourceType}");
    }
}