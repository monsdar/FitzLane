using System.Collections.Generic;

namespace FitzLane.Interfaces
{
    public interface IErgSender
    {
        void Connect(string address);
        void SendErgs(IList<IPlayer> playerList);
    }
}
