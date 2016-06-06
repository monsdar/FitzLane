using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitzBot
{
    class SimpleErgReceiver : IErgReceiver
    {
        public event ErgEventHandler OnErgReceived;
        public bool IsConnected { get; private set; }

        public void Connect(string givenAddress)
        {
            IsConnected = true;
        }

        public bool TryReceive()
        {
            if(!IsConnected)
            {
                return false;
            }

            EasyErgsocket.Erg newErg = new EasyErgsocket.Erg();
            newErg.name = "ReceivedPlayer";
            OnErgReceived(this, new ErgEventArgs(newErg));

            return true;
        }
    }
}
