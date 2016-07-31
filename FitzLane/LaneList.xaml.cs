using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FitzLaneConfig;
using FitzLanePlugin.Interfaces;

namespace FitzLane
{
    /// <summary>
    /// Interaction logic for LaneList.xaml
    /// </summary>
    public partial class LaneList : UserControl
    {
        IList<Lane> lanes = new List<Lane>();
        
        public LaneList()
        {
            InitializeComponent();

            //Create 5 empty lanes initially...
            //TODO: We should later allow to dynamically create new lanes as the user wishes...
            for(int numLane = 0; numLane < 5; ++numLane)
            {
                PlayerItemEmpty newEmpty = new PlayerItemEmpty();
                stack_Lanes.Children.Add(newEmpty);
            }
        }

        public void AddLaneConfig(IList<Lane> givenLanes)
        {
            foreach (Lane lane in givenLanes)
            {
                AddLaneConfig(lane);
            }
        }

        public void AddLaneConfig(Lane givenLane)
        {
            //check if the given lane has a higher index than we have slots
            if (stack_Lanes.Children.Count <= givenLane.laneIndex)
            {
                return;
            }

            lanes.Add(givenLane);
        }

        public void UpdatePlayer(IList<IPlayer> givenPlayers)
        {
            foreach (IPlayer player in givenPlayers)
            {
                UpdatePlayer(player);
            }
        }

        public void UpdatePlayer(IPlayer givenPlayer)
        {
            //check if the given player should be a main player
            Lane playerLane = null;
            foreach (Lane lane in lanes)
            {
                if(givenPlayer.GetErg().ergId == lane.ergId)
                {
                    playerLane = lane;
                    break;
                }
            }

            if(playerLane == null)
            {
                return;
            }

            PlayerItem item = stack_Lanes.Children[playerLane.laneIndex] as PlayerItem;
            if(item == null) //if this player isn't in the UI we need to add him
            {
                stack_Lanes.Children.RemoveAt(playerLane.laneIndex);
                item = new PlayerItem(givenPlayer, playerLane.isMainPlayer);
                stack_Lanes.Children.Add(item);
            }
            item.Update(givenPlayer, playerLane.isMainPlayer);
        }
    }
}
