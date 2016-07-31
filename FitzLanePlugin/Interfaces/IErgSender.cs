using System.Collections.Generic;

namespace FitzLanePlugin.Interfaces
{
    public interface IErgSender
    {
        void Connect(string address);
        void SendErgs(IList<IPlayer> playerList);
    }
}
