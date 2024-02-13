using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenAI;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using ChatMessage = OpenAI_API.Chat.ChatMessage;

public class MemDealerOpenAIManager : MonoBehaviour
{
    private OpenAIAPI _api;
    private List<ChatMessage> _storyChatMessages;
    [SerializeField] private string _storyPrePrompt = "";
    [SerializeField] public int numMemories = 3;

    public string generatedStoryString;
    public List<PhotoData> _memoriesForStory;
    private bool _calledForFullStory = false;
    
    // Start is called before the first frame update
    private void Awake()
    {
        // TODO: Place in Env
        var _key = "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";
        _api = new OpenAIAPI(_key);
        // _api = new OpenAIAPI(Environment.GetEnvironmentVariable(_key, EnvironmentVariableTarget.User));
    }

    void Start()
    {
        _memoriesForStory = new List<PhotoData>();
        StartStoryConversation();
    }

    private void Update()
    {
        if (_calledForFullStory) return;
        if (_memoriesForStory.Count < numMemories)
        {
            Debug.Log("Waiting for new memories");
        }
        else
        {
            Debug.Log("Memories ready... ready to call dealer");
            // if (!_calledForFullStory)
            //{
                // _calledForFullStory = true;
                // GetFullStoryUsingCaptions();
            // }
        }
    }

    public async Task<PhotoData> AddToCaptionsAndGenerateImage(string caption)
    {
        Debug.Log("ADDING NEW MEMORY: " + caption);
        var imageUrl = await GetImageForCaption(caption);
        PhotoData data = new PhotoData();
        data.caption = caption;
        data.imageUrl = imageUrl;
        
        _memoriesForStory.Add((data));
        Debug.Log("MEMORY COUNT: " + _memoriesForStory.Count);
        return data;
    }

    private async Task<string> GetImageForCaption(string caption)
    {
        Debug.Log("FETCHING NEW IMAGE FOR CAPTION: " + caption);
        var prompt = new OpenAI_API.Images.ImageGenerationRequest
        {
            Prompt = caption + " (The photo should be in first person perspective, and style it like a hazy dream)",
            Size = OpenAI_API.Images.ImageSize._256
        };
        var response = await _api.ImageGenerations.CreateImageAsync(prompt);

            if (response.Data != null && response.Data.Count > 0)
            {
                return response.Data[0].Url;
            }
            else
            {
                Debug.LogWarning("No image was created from this prompt.");
                return "";
            }
    }

    public bool CheckIfStoryIsReady()
    {
        return !String.IsNullOrEmpty(generatedStoryString);
    }
    

    public async Task<string> GetFullStoryUsingCaptions()
    {
        if (_memoriesForStory.Count < numMemories) return "";
        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        string promptContent = "Generate a short story (max 7 sentences) in first-person POV using the following memories below. Keep the sentences short and concise, and make the tone feel like the author is dreaming. (The memories below are in chronological order): ";
        for (var i = 0; i < _memoriesForStory.Count; i++)
        {
            promptContent += $"\n- {_memoriesForStory[i].caption}";
        }
        userMessage.Content = promptContent;
        
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content ));
        
        _storyChatMessages.Add((userMessage));
        
        var chatResult = await _api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,
            MaxTokens = 300,
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
