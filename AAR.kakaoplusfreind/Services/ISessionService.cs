using AAR.kakaoplusfreind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAR.kakaoplusfreind.Services
{
    public interface ISessionService
    {
        Task<ConversationInfo> GetInfoAsync(string userkey);

        Task SetInfoAsync(ConversationInfo info);

        Task DeleteInfoAsync(string userkey);
    }
}
