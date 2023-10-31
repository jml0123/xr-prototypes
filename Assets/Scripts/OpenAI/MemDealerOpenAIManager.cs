using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;

public class MemDealerOpenAIManager : MonoBehaviour
{
    private OpenAIAPI _api;
    private List<ChatMessage> _storyChatMessages;
    [SerializeField] private string _storyPrePrompt = "";
    [SerializeField] private int _numMemories = 3;

    public string generatedImageUrl;
    public string generatedStoryString;
    public List<PhotoData> _memoriesForStory;
    
    // Start is called before the first frame update
    void Start()
    {
        // TODO: Place in Env
        var _key = "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";
        _api = new OpenAIAPI(_key);
        // _api = new OpenAIAPI(Environment.GetEnvironmentVariable(_key, EnvironmentVariableTarget.User));
        StartStoryConversation();
    }

    public void AddToCaptionsAndGenerateImage(string caption)
    {
        // var imageUrl = GetImageForCaption(caption);
        PhotoData data = new PhotoData();
        data.caption = caption;
        // data.imageUrl = imageUrl;
        
        _memoriesForStory.Add((data));
    }

    public async void GetImageForCaption(string caption)
    {
    // TODO: Implement
    }
    
    public async Task<string> GetFullStoryUsingCaptions()
    {
        if (_memoriesForStory.Count < _numMemories) return "";
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        string promptContent = "Generate a short story in first-person POV using the following sentences: ";
        for (var i = 0; i < _memoriesForStory.Count; i++)
        {
            promptContent += $"- {_memoriesForStory[i].caption}";
        }
        userMessage.Content = promptContent;
        
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content ));
        
        _storyChatMessages.Add((userMessage));
        
        var chatResult = await _api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,
            MaxTokens = 50,
            Messages = _storyChatMessages
        });

        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

        _storyChatMessages.Add(responseMessage);

        generatedStoryString = responseMessage.Content;
        return generatedStoryString;
    }
    
    private void StartStoryConversation()
    {
        _storyChatMessages = new List<ChatMessage> { 
            new ChatMessage(ChatMessageRole.System, _storyPrePrompt) 
        };
    }
}
