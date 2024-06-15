using Sandbox;
using System;
using System.Text.RegularExpressions;

public sealed class Talker : Component
{
    [Property] public List<word> words {get;set;}
    public string speaker = "basic guy";
    public NpcmodelAnimationManager npcmodelAnimationManager;
    SoundPointComponent soundPointComponent;
    public class word
    {
        public int syllables;
        public bool containsFullStop;
        public bool containsComma;
    }
	protected override void OnStart()
	{
        soundPointComponent = Components.Create<SoundPointComponent>();
        soundPointComponent.SoundEvent = ResourceLibrary.Get<SoundEvent>($"gameresources/voices/{speaker}/voices.sound");
        words = new List<word>();
	}
    float currentSpeakTime;
    protected override void OnUpdate()
    {
        currentSpeakTime-=Time.Delta;
        if(words.Count > 0 && currentSpeakTime <= 0)
        {
            
            if(words[0].syllables > 0)
            {
                currentSpeakTime = 0.226f;
                soundPointComponent.StopSound();
                soundPointComponent.StartSound();
                npcmodelAnimationManager.SetTalk();
                words[0].syllables--;
            }
            else
            {
                currentSpeakTime = 
                (words[0].containsComma ? 0.266f : 0) + 
                (words[0].containsFullStop ? 0.532f : 0);
                words.RemoveAt(0);
            }
            
        }
    }
    public void AddWords(string text)
    {
        string[] wordSplit = text.Split(' ');
        foreach(string word in wordSplit)
        {
			word newWord = new word
			{
				syllables = Math.Clamp(word.Length / 3, 1, 100),
				containsFullStop = word.Contains( "." ),
				containsComma = word.Contains( "," )
			};
			words.Add(newWord);
        }
        currentSpeakTime = 0.226f;
    }
    
}
