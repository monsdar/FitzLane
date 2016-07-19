using System.Collections.Generic;

namespace FitzLaneManager.Interfaces
{
    public interface IErgSender
    {
        void Connect(string address);
        void SendErgs(IList<IPlayer> playerList);
    }
}
