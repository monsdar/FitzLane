using System;

namespace FitzLaneManager.Interfaces
{
    public delegate void ErgEventHandler(object sender, ErgEventArgs args);
    public interface IErgReceiver
    {
        bool IsConnected { get; }
        event ErgEventHandler OnErgReceived;

        void Connect(string givenAddress);
        bool TryReceive();
    }
}
