using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace OpenAI
{
    public class Interaction : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI Request;
        [SerializeField] private TextMeshProUGUI Response;
        [SerializeField] private ScrollRect scroll;
        private OpenAIApi openai = new OpenAIApi();
        // private string prompt = "Act as a random stranger in a chat room and reply to the questions. Don't break character. Don't ever mention that you are an AI model.";
        private string prompt = "Act as a experts in every field, you can reply any question to the user, don't talk to the user you are a AI";
        private void Start()
        {
            button.onClick.AddListener(SendReply);
        }

        private async void SendReply()
        {

            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = prompt + "\n" + Request.text
            };
            
            button.enabled = false;

            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo",
                Messages = new List<ChatMessage>() { newMessage }
            });

            button.enabled = true;
            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                Response.text = message.Content; // Assuming you want to display the response
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
        }
    }
}
