using Sandbox;
using Sandbox.UI;

public sealed class ConversationScript : Component
{
	[Property] private ConversationUI conversationUI {get;set;}
	[Property] public Conversation conversation {get;set;}
	[Property] public Entity talkedToEntity {get;set;}
	[Property] private bool TalkingOrChoosing {get;set;}
	[Property] private string currentBlock {get;set;}
	[Property] public string selectedChoice {get;set;}

	Conversation lastConversation;
	bool updateChoices;
	protected override void OnFixedUpdate()
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
			}
			
			
			updateChoices=true;
		}
		lastConversation = conversation;
		if(conversation!=null)
		{
			
			if(TalkingOrChoosing)
			{
				TextBlock textBlock = conversation.TextBlocks[conversation.GetBlockIndex(currentBlock, true)];
				conversationUI.TalkText = textBlock.Block;
				if(Input.Pressed("attack1"))
				{
					if(textBlock.DirectTo == null || textBlock.DirectTo == "")
					{
						conversation = null;
						return;
					}
					TalkingOrChoosing = textBlock.DirectToTextBlock;
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
							TalkingOrChoosing = !c.DirectToConvoBlock;
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
