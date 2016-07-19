using System;
using FitzLane.Interfaces;

namespace FitzLane.Receiver
{
    class SimpleErgReceiver : IErgReceiver
    {
        public event ErgEventHandler OnErgReceived;
        public bool IsConnected { get; private set; }
        private EasyErgsocket.Erg newErg = new EasyErgsocket.Erg();
        private DateTime starttime = DateTime.Now;

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

            TimeSpan timeSinceStart = DateTime.Now - starttime;
            newErg.exerciseTime = timeSinceStart.TotalSeconds;
            OnErgReceived(this, new ErgEventArgs(newErg));

            // Do NOT keep the while loop running.
            return false;
        }
    }
}
