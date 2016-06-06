using System.Collections.Generic;
using NetMQ;
using NetMQ.Sockets;
using ProtoBuf;

namespace FitzBot
{
    class ZmqBotSender : IBotSender
    {
        public bool IsConnected { get; private set; }

        NetMQContext context = null;
        PublisherSocket pubSocket = null;

        public ZmqBotSender(NetMQContext context)
        {
            this.context = context;
            IsConnected = false;
        }

        ~ZmqBotSender()
        {
            pubSocket.Close();
            context.Terminate();
        }

        public void Connect(string givenAddress)
        {
            if (IsConnected)
            {
                return;
            }
            
            pubSocket = context.CreatePublisherSocket();
            pubSocket.Connect(givenAddress);

            IsConnected = true;
        }

        public void SendBots(IList<IBot> botList)
        {
            if (!IsConnected)
            {
                return;
            }

            var message = new NetMQMessage();
            message.Append("EasyErgsocket");

            foreach (IBot bot in botList)
            {
                using (var buffer = new System.IO.MemoryStream())
                {
                    Serializer.Serialize(buffer, bot.GetErg());
                    message.Append(buffer.ToArray());
                }
            }

            pubSocket.TrySendMultipartMessage(message);
        }
    }
}
