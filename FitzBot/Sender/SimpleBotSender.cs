using System;
using System.Collections.Generic;

namespace FitzBot
{
    class SimpleBotSender : IBotSender
    {
        public void Connect(string address)
        {
            Console.WriteLine("Connecting to " + address);
        }

        public void SendBots(IList<IBot> botList)
        {
            foreach (IBot bot in botList)
            {
                Console.WriteLine("Sending " + bot.GetErg().name + " (" + bot.GetErg().ergId + ")");
            }
        }
    }
}
