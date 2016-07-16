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
            newErg.distance += 0.1;
            newErg.exerciseTime += 1.0/60.0;
            OnErgReceived(this, new ErgEventArgs(newErg));

            // Do NOT keep the while loop running.
            return false;
        }
    }
}
