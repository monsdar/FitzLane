using System.Collections.Generic;

namespace FitzBot
{
    public interface IBotSender
    {
        void Connect(string address);
        void SendBots(IList<IBot> botList);
    }
}
