using OhIlSeokBot.KakaoPlusFriend.Helpers;
using OhIlSeokBot.KakaoPlusFriend.Models;
using OhIlSeokBot.KakaoPlusFriend.Services;
using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents;
using AAR.kakaoplusfreind.Services;

namespace AAR.kakaoplusfreind.Controllers
{
    public class MessageController : Controller
    {

        private string directLineSecret = ConfigurationManager.AppSettings["DirectLineSecret"];
        private string botId = ConfigurationManager.AppSettings["BotId"];
        private string fromUser = "DirectLineSampleClientUser";
        private Conversation Conversation = null;

        DirectLineClient Client = null;

        private ConversationInfo conversationinfo;
        private My_DBService myDbService;

        // GET: Message 
        public async Task<ActionResult> Index(string user_key, string type, string content)
        {

            Client = new DirectLineClient(directLineSecret);
            myDbService = new My_DBService();

            conversationinfo = new ConversationInfo
            {
                coversation = Conversation,
                id = user_key,
                timestamp = DateTimeOffset.Now,
                watermark = ""
            };

            var results = await myDbService.readDataFromDocument(conversationinfo);
            var item = results.FirstOrDefault();

            if (results.Count() != 0)
            {
                Conversation = Client.Conversations.ReconnectToConversation(item.coversation.ConversationId.ToString());
            }
            else
            {
                Conversation = Client.Conversations.StartConversation();

                conversationinfo = new ConversationInfo
                {
                    coversation = Conversation,
                    id = user_key,
                    timestamp = DateTimeOffset.Now,
                    watermark = ""
                };

                await myDbService.SetInfoAsync(conversationinfo);
            }


            Activity userMessage = new Activity
            {
                From = new ChannelAccount(fromUser),
                Type = ActivityTypes.Message,
                Text = content
            };

            await Client.Conversations.PostActivityAsync(this.Conversation.ConversationId, userMessage);
            
            // the part that receives the message 
            string watermark = null;

            while (true)
            {
                
                var activitySet = await Client.Conversations.GetActivitiesAsync(Conversation.ConversationId, watermark);
                watermark = activitySet?.Watermark;
                var response = from x in activitySet.Activities where x.From.Id == botId select x;

                // Handle multiple discovered activities - 발견된 복수의 Activity를 넘겨서 처리
                var msg = MessageConvertor.DirectLineToKakao(response.ToList());
                return Json(msg);
            }
        }

        
    }
}