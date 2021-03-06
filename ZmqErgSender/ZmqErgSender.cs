﻿using System.Collections.Generic;
using NetMQ;
using NetMQ.Sockets;
using ProtoBuf;
using FitzLanePlugin.Interfaces;

namespace ZmqErgSender
{
    class ZmqErgSender : IErgSender
    {
        public bool IsConnected { get; private set; }

        NetMQContext context = null;
        PublisherSocket pubSocket = null;

        public ZmqErgSender()
        {
            context = NetMQContext.Create();
            IsConnected = false;
        }

        ~ZmqErgSender()
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
            // Use connect when working with FitzCore. For direct connection use Bind.
            //pubSocket.Connect(givenAddress);
            pubSocket.Bind(givenAddress);

            IsConnected = true;
        }

        public void SendErgs(IList<IPlayer> playerList)
        {
            if (!IsConnected)
            {
                return;
            }

            var message = new NetMQMessage();
            message.Append("EasyErgsocket");

            foreach (IPlayer player in playerList)
            {
                using (var buffer = new System.IO.MemoryStream())
                {
                    Serializer.Serialize(buffer, player.GetErg());
                    message.Append(buffer.ToArray());
                }
            }

            pubSocket.TrySendMultipartMessage(message);
        }
    }
}
