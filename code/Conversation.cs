using Sandbox;
using Sandbox.UI;

public sealed class ConversationScript : Component
{
	[Property] private WeaponScript weaponScript{get;set;}
	[Property] private ConversationUI conversationUI {get;set;}
	[Property] public Conversation conversation {get;set;}
	[Property] public Entity talkedToEntity {get;set;}
	[Property] private bool TalkingOrChoosing {get;set;}
	[Property] private string currentBlock {get;set;}
	[Property] public string selectedChoice {get;set;}



	Conversation lastConversation;

	ContainerInteract containerInteract;

	string lastBlock;
	bool updateChoices;
	bool updateWords;
	Talker talker;
	protected override void OnStart()
	{
		talker = Components.Create<Talker>();
		containerInteract = GameObject.Parent.Components.Get<ContainerInteract>();
	}
	protected override void OnUpdate()
	{
		
		if(conversation != lastConversation)
		{
			conversationUI.Enable = conversation != null;
			
			if(conversationUI.Enable)
			{
				conversation.BlockIndex = -1;
				conversation.CurrentlyAccessedBlock = "";
				TalkingOrChoosing = conversation.StartTalkingOrChoosing;
				currentBlock = conversation.StartBlock;
				talker.npcmodelAnimationManager = talkedToEntity.GameObject.Children[0].Components.Get<NpcmodelAnimationManager>();
				weaponScript.canShoot = false;
			}
			else
			{
				weaponScript.canShoot = true;
			}
			
			updateWords = true;
			updateChoices=true;
		}
		
		lastConversation = conversation;
		if(conversation!=null)
		{
			
			if(TalkingOrChoosing)
			{
				
				TextBlock textBlock = conversation.TextBlocks[conversation.GetBlockIndex(currentBlock, true)];
				conversationUI.TalkText = textBlock.Block;
				if(updateWords)
				{
					updateWords = false;
					talker.AddWords(textBlock.Block);
				}
				
				if(Input.Pressed("attack1"))
				{
					if(textBlock.DirectTo == null || textBlock.DirectTo == "")
					{
						conversation = null;
						return;
					}
					TalkingOrChoosing = textBlock.DirectToTextBlock;
					updateWords = true;
					currentBlock = textBlock.DirectTo;
					updateChoices=true;
				}
			}
			else
			{
				if(updateChoices)
				{
					updateChoices = false;
					conversationUI.Choices = new List<string>();
					ChoiceBlock choiceBlock = conversation.ChoiceBlocks[conversation.GetBlockIndex(currentBlock, false)];
					foreach(Choice c in choiceBlock.choices)
					{
						conversationUI.Choices.Add(c.ChoiceText);
					}
				}
				if(selectedChoice != "")
				{
					ChoiceBlock choiceBlock = conversation.ChoiceBlocks[conversation.GetBlockIndex(currentBlock, false)];
					foreach(Choice c in choiceBlock.choices)
					{
						if(c.ChoiceText == selectedChoice || c.ChoiceText == "")
						{
							if(c.DirectToBarter)
							{
								conversation = null;
								containerInteract.LookedAtContainer = talkedToEntity;
								containerInteract.StartBarter(talkedToEntity);
								return;
							}
							TalkingOrChoosing = !c.DirectToConvoBlock;
							updateWords = true;
							currentBlock = c.DirectTo;
							break;
						}
					}
					selectedChoice = "";
				}
			}
		}
		
		conversationUI.TalkingOrChoosing = TalkingOrChoosing;
		
	}
}
