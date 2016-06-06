using System.Collections.Generic;

namespace FitzBot
{
    interface IBotSender
    {
        void Connect(string address);
        void SendBots(IList<IBot> botList);
    }
}
