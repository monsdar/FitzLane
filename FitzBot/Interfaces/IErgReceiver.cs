using System;

namespace FitzBot
{
    public delegate void ErgEventHandler(object sender, ErgEventArgs args);
    interface IErgReceiver
    {
        bool IsConnected { get; }
        event ErgEventHandler OnErgReceived;

        void Connect(string givenAddress);
        bool TryReceive();
    }
}
