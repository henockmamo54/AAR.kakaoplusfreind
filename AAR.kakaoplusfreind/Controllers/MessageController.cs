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
        string mytest = "heni";
        bool myval = false;

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
                Conversation = Client.Conversations.ReconnectToConversation(item.coversation.ConversationId.ToString()); myval = true;
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
                //var activitySet = await Client.Conversations.GetActivitiesAsync(Conversation.ConversationId, watermark);
                //watermark = activitySet?.Watermark;

                //var activities = from x in activitySet.Activities where x.From.Id == botId select x;


                //conversationinfo = new ConversationInfo
                //{
                //    coversation = Conversation,
                //    id = user_key,
                //    timestamp = DateTimeOffset.Now,
                //    watermark = watermark
                //};

                //if (results.Count() == 0)
                //    await myDbService.SetInfoAsync(conversationinfo);


                //Message message = new Message();
                //MessageResponse messageResponse = new MessageResponse();
                //messageResponse.message = message;

                //foreach (Activity activity in activities)
                //{
                //    message.text = ((Activity)activities.Last()).Text +  activities.Count()+activity.Text + "-" + this.Conversation.ConversationId + " # " + user_key + "#" + Session["cid"] + "###"+mytest;
                //}

                //return Json(messageResponse, JsonRequestBehavior.AllowGet);             // return View (); 

                //=============================================================================================================
                var activitySet = await Client.Conversations.GetActivitiesAsync(Conversation.ConversationId, watermark);
                watermark = activitySet?.Watermark;
                var response = from x in activitySet.Activities where x.From.Id == botId select x;

                // 발견된 복수의 Activity를 넘겨서 처리
                var msg = MessageConvertor.DirectLineToKakao(response.ToList(), this.Conversation.ConversationId, user_key, myval);
                return Json(msg);
            }
        }

        //============================================================================================================


        //private IDirectLineConversationService conversationService;

        //public MessageController(IDirectLineConversationService service)
        //{
        //    conversationService = service;
        //}

        ///// <summary>
        ///// 실제 카카오톡과 메시지를 주고 받는 부분. 
        ///// 
        ///// </summary>
        ///// <param name="user_key">해시된 사용자 키</param>
        ///// <param name="type">text / photo </param>
        ///// <param name="content">내용</param>
        ///// <returns></returns>
        //[AcceptVerbs(HttpVerbs.Post)]
        //public async Task<ActionResult> Index(string user_key, string type, string content)
        //{
        //    try
        //    {
        //        // covert from Kakao talk message to Bot Builder Activity
        //        Activity activity = new Activity
        //        {
        //            // Bot 에서 메시지가 kakao로 부터 요청되었음을 알수 있도록 name에 kakao를 써준다. 
        //            From = new ChannelAccount(id: user_key, name: "kakao"),
        //            Type = ActivityTypes.Message
        //        };
        //        if (type == "text")
        //        {
        //            activity.Text = content;
        //        }
        //        else if (type == "photo")
        //        {
        //            activity.Attachments = new List<Attachment>();
        //            activity.Attachments.Add(new Attachment
        //            {
        //                ContentUrl = content
        //            });
        //        }
        //        var response = await conversationService.SendAndReceiveMessageAsync(user_key, activity);
        //        // 발견된 복수의 Activity를 넘겨서 처리
        //        var msg = MessageConvertor.DirectLineToKakao(response);
        //        return Json(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidOperationException("Direct Line 연결오류", ex);
        //    }
        //}

        //=================================================================================
    }
}