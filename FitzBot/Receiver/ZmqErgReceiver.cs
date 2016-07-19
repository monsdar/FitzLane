using System.Linq;
using ProtoBuf;
using NetMQ;
using NetMQ.Sockets;
using FitzLaneManager.Interfaces;

namespace FitzLaneManager
{
    class ZmqErgReceiver : IErgReceiver
    {
        public event ErgEventHandler OnErgReceived;
        public bool IsConnected { get; private set; }

        NetMQContext context = null;
        SubscriberSocket subSocket = null;

        public ZmqErgReceiver(NetMQContext context)
        {
            this.context = context;
            IsConnected = false;
        }

        ~ZmqErgReceiver()
        {
            subSocket.Close();
            context.Terminate();
        }

        public void Connect(string givenAddress)
        {
            if (IsConnected)
            {
                return;
            }
            
            subSocket = context.CreateSubscriberSocket();
            subSocket.Connect(givenAddress);
            subSocket.Subscribe("EasyErgsocket");

            IsConnected = true;
        }

        public bool TryReceive()
        {
            if (!IsConnected)
            {
                return false;
            }

            //try to receive something from the network... if that succeeds get the distance from the given Erg
            var message = new NetMQMessage();
            if (subSocket.TryReceiveMultipartMessage(System.TimeSpan.Zero, ref message))
            {
                foreach (var frame in message.Skip(1)) //the first frame is always just the envelope/topic... let's ignore it by using Linq
                {
                    HandleFrame(frame);
                }
                return true;
            }
            return false;
        }

        private void HandleFrame(NetMQFrame frame)
        {
            byte[] rawMessage = frame.Buffer;
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(rawMessage))
            {
                var givenErg = Serializer.Deserialize<EasyErgsocket.Erg>(memoryStream);

                //TODO: Let user decide which erg he wants to use as "parent"
                if (givenErg.playertype == EasyErgsocket.PlayerType.HUMAN)
                {
                    if (OnErgReceived != null) //do not call if the event hasn't been subscribed to
                    {
                        OnErgReceived(this, new ErgEventArgs(givenErg));
                    }
                }
            }
        }
    }
}
