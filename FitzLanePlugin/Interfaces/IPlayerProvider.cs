using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitzLanePlugin.Interfaces
{
    public interface IPlayerProvider
    {
        string GetPlayerName();
        bool IsValidPlayertype(string playerType);
        IPlayer GetPlayer(string config);
        string GetDefaultPlayerConfig();
    }
}
