using System;
using System.Collections;
using System.Collections.Generic;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenAIController : MonoBehaviour
{
    public TMP_Text textField;

    public TMP_Text inputField;

    public Button okButton;

    private OpenAIAPI _api;

    private List<ChatMessage> _messages;

    [SerializeField] private string _prePrompt =
        "You are a bouncer at a popular nightclub, and you will only allow someone who has a ticket to enter (sometimes, there are exceptions). I am currently trying to convince you to let me into the club without a ticket. You should respond to every prompt with some dialogue, and more importantly, with a probability (in %) that I will be let into the club. Keep the dialogue concise.";

    [SerializeField] private string _startString = "Do you have a ticket?";
    // Start is called before the first frame update
    void Start()
    {
        // TODO: Place in Env
        var _key = "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";
        _api = new OpenAIAPI(_key);
        // _api = new OpenAIAPI(Environment.GetEnvironmentVariable(_key, EnvironmentVariableTarget.User));
        StartConversation();
        okButton.onClick.AddListener(() => GetResponse());
    }
    private void StartConversation()
    {
        _messages = new List<ChatMessage> { 
            new ChatMessage(ChatMessageRole.System, _prePrompt) 
        };

        // inputField.text = "";
        textField.text = _startString;
        Debug.Log(_startString);
    }
    private async void GetResponse()
    {
        Debug.Log("Clicked OK Button");
        if (inputField.text.Length < 1)
        {
            return;
        }

        okButton.enabled = false;

        ChatMessage userMessage = new ChatMessage();
        userMessage.Role = ChatMessageRole.User;
        userMessage.Content = inputField.text;
        if (userMessage.Content.Length > 100)
        {
            userMessage.Content = userMessage.Content.Substring(0, 100);
        }
        Debug.Log(string.Format("{0}: {1}", userMessage.rawRole, userMessage.Content ));
        
        _messages.Add((userMessage));

        textField.text = string.Format("You: {0}", userMessage.Content);

        inputField.text = "";

        var chatResult = await _api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,
            MaxTokens = 50,
            Messages = _messages
        });

        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;
        Debug.Log(string.Format("{0}: {1}", responseMessage.rawRole, responseMessage.Content));

        _messages.Add(responseMessage);

        textField.text = string.Format("You: {0} \n\nGuard: {1}", userMessage.Content, responseMessage.Content);

        okButton.enabled = true;
    }
}
