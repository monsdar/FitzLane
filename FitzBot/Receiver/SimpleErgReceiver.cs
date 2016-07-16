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
        private double distance = 0.0;
        private EasyErgsocket.Erg newErg = new EasyErgsocket.Erg();

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

            newErg.name = "ReceivedPlayer";
            distance += 0.1;
            newErg.distance = distance;
            newErg.exerciseTime = 10.0;
            OnErgReceived(this, new ErgEventArgs(newErg));

            return true;
        }
    }
}
