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
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static readonly string CollectionId = ConfigurationManager.AppSettings["collection"];
        string mytest = "heni";

        // GET: Message 
        public async Task<ActionResult> Index(string user_key, string type, string content)
        {

            Client = new DirectLineClient(directLineSecret);

            conversationinfo = new ConversationInfo
            {
                coversation = Conversation,
                id = user_key,
                timestamp = DateTimeOffset.Now,
                watermark = ""
            };

            var results = await readDataFromDocument(conversationinfo);
            var item = results.FirstOrDefault();
            string mytest = item.coversation.ConversationId + "";


            //if (Session["cid"] as string != null)
            if (item != null)
            {
                //this.Conversation = Client.Conversations.ReconnectToConversation((string)Session["CONVERSTAION_ID"]);
                Conversation = Client.Conversations.ReconnectToConversation(item.coversation.ConversationId + "");
            }
            else
            {
                this.Conversation = Client.Conversations.StartConversation();

                Session["cid"] = Conversation.ConversationId;
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

                var activities = from x in activitySet.Activities where x.From.Id == botId select x;


                conversationinfo = new ConversationInfo
                {
                    coversation = Conversation,
                    id = user_key,
                    timestamp = DateTimeOffset.Now,
                    watermark = watermark
                };

                //await SetInfoAsync(conversationinfo);

                //var results = await readDataFromDocument(conversationinfo);
                //var item = results.FirstOrDefault();
                //string mytest = item.coversation.ConversationId + "";


                Message message = new Message();
                MessageResponse messageResponse = new MessageResponse();
                messageResponse.message = message;

                foreach (Activity activity in activities)
                {
                    message.text = activity.Text + "-" + this.Conversation.ConversationId + " # " + user_key + "#" + Session["cid"] + " ##" + mytest;
                }

                return Json(messageResponse, JsonRequestBehavior.AllowGet);             // return View (); 
            }
        }

        public async Task<IEnumerable<ConversationInfo>> readDataFromDocument(ConversationInfo info)
        {

            var client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);
            //await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), info);
            IDocumentQuery<ConversationInfo> query = client.CreateDocumentQuery<ConversationInfo>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(d => d.id == info.id)
                .AsDocumentQuery();

            List<ConversationInfo> results = new List<ConversationInfo>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<ConversationInfo>());
            }

            return results;
        }
        public async Task SetInfoAsync(ConversationInfo info)
        {
            var client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);
            await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), info);
            //IDocumentQuery<ConversationInfo> query = client.CreateDocumentQuery<ConversationInfo>(
            //    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
            //    new FeedOptions { MaxItemCount = -1 })
            //    .Where(d => d.id == info.id)
            //    .AsDocumentQuery();

            //List<ConversationInfo> results = new List<ConversationInfo>();
            //while (query.HasMoreResults)
            //{
            //    results.AddRange(await query.ExecuteNextAsync<ConversationInfo>());
            //}

            //var item = results.FirstOrDefault();
            //string mytest = item.coversation.ConversationId +"";

            //============================
            //var items = await DocumentDBRepository<ConversationInfo>.GetItemsAsync(d => d.id == info.id);
            //var item = items.FirstOrDefault();
            //if (item == null)
            //{
            //    await DocumentDBRepository<ConversationInfo>.CreateItemAsync(info);
            //}
            //else
            //{
            //    await DocumentDBRepository<ConversationInfo>.UpdateItemAsync(info.id, info);
            //}
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