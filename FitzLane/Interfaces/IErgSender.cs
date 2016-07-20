using System.Collections.Generic;
using FitzLanePlugin.Interfaces;

namespace FitzLane.Interfaces
{
    public interface IErgSender
    {
        void Connect(string address);
        void SendErgs(IList<IPlayer> playerList);
    }
}
