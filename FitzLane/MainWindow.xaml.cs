using NetMQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Media;
using FitzLane.Interfaces;
using FitzLane.Config;
using FitzLane.Receiver;
using FitzLane.Bots;

namespace FitzLane
{
    public partial class MainWindow : Window
    {
        IDictionary<string, IPlayer> players = new Dictionary<string, IPlayer>();
        IErgReceiver receiver = null;
        IErgSender ergSender = null;
        const string configFilePath = "worldConfig.json";
        List<Lane> lanes = new List<Lane>();
        ConfigReader reader = null;
        
        public MainWindow()
        {
            InitializeComponent();

            //check if there's a config... if not write the default config
            if (!File.Exists(configFilePath))
            {
                ConfigWriter.WriteDefaultConfig(configFilePath);
            }

            //prefill the UI - this is an easy way to have every lane filled, even if it is replaced by a configured lane in the next step
            for (int i=0; i < 5; ++i)
            {
                Lane lane = new Lane();
                lane.laneIndex = i;
                lanes.Add(lane);
            }

            // Load default config (this loads an existing config or the previously created default config
            reader = new ConfigReader(configFilePath);
            foreach (Lane lane in reader.getLanes())
            {
                playerConfiguredReceived(lane.laneIndex, lane);
            }
            
            updateButtons();

            //init the networking
            NetMQContext context = NetMQContext.Create();
            ergSender = new ZmqErgSender(context);
            ergSender.Connect("tcp://127.0.0.1:21744");
            
            //TODO: this is our synchronuous gameloop... better do some threading in the future
            CompositionTarget.Rendering += mainLoop;
        }

        void mainLoop(object sender, EventArgs e)
        {
            //Update all the players
            //This loop tries to update at least 1 player in each iteration, if it fails the loop is either over or there is a failed parent/child link
            List<string> alreadyUpdatedPlayers = new List<string>();
            while (alreadyUpdatedPlayers.Count < players.Count)
            {
                //see if there's a player that we could update
                bool hasPlayerBeenUpdated = false;
                foreach (Lane lane in lanes)
                {
                    if (players.ContainsKey(lane.ergId) &&
                        !alreadyUpdatedPlayers.Contains(lane.ergId))
                    {
                        if (lane.parentId == "")
                        {
                            players[lane.ergId].Update(null);
                            alreadyUpdatedPlayers.Add(lane.ergId);
                            hasPlayerBeenUpdated = true;
                        }
                        else if (alreadyUpdatedPlayers.Contains(lane.parentId))
                        {
                            EasyErgsocket.Erg parent = players[lane.parentId].GetErg();
                            players[lane.ergId].Update(parent);
                            alreadyUpdatedPlayers.Add(lane.ergId);
                            hasPlayerBeenUpdated = true;
                        }
                        else
                        {
                            //This lane has a parent that does not exist...
                            continue;
                        }
                    }
                }

                if (!hasPlayerBeenUpdated)
                {
                    //no player updated... there seems to be a problem?
                    //TODO: Have a plan on what to do...
                    break;
                }
            }

            //update the GUI
            foreach (KeyValuePair<string, IPlayer> playerItem in players)
            {
                foreach (UIElement element in stack_Main.Children)
                {
                    if (element.GetType() == typeof(PlayerItem))
                    {
                        ((PlayerItem)element).Update(playerItem.Value);
                    }
                }
            }

            //send the bots to network
            IList<IPlayer> flattenedList = players.Values.ToList<IPlayer>();
            ergSender.SendErgs(flattenedList);
        }

        void updateButtons()
        {
            stack_Main.Children.Clear();

            foreach (Lane lane in lanes)
            {
                if (lane != null
                    && lane.playerType != "")
                {
                    if (lane.playerType == typeof(BotConstant).Name)
                    {
                        IPlayer player = new BotConstant(lane.ergId);
                        stack_Main.Children.Add(new PlayerItem(player, lane.isMainPlayer));
                    }
                }
                else
                {
                    PlayerItemEmpty newEmpty = new PlayerItemEmpty(lane.laneIndex);
                    newEmpty.OnAdd += addPlayerReceived;
                    stack_Main.Children.Add(newEmpty);
                }
            }
        }

        private void addPlayerReceived(int laneIndex)
        {
            ConfigureLaneWindow w = new ConfigureLaneWindow(laneIndex);
            w.OnOk += playerConfiguredReceived;
            w.Show();
        }

        void playerConfiguredReceived(int laneIndex, Lane laneCfg)
        {
            for (int i=0; i < lanes.Count; ++i)
            {
                if (lanes[i].laneIndex == laneIndex)
                {
                    lanes[i] = laneCfg;
                    updateButtons();
                    return;
                }
            }
            lanes.Add(laneCfg);
            updateButtons();
        }

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (button_StartStop.Content.ToString() == "Stop")
            {
                players.Clear();
                button_StartStop.Content = "Start";
            }
            else
            {
                foreach (Lane lane in lanes)
                {
                    if (lane.playerType == typeof(BotConstant).Name)
                    {
                        MemoryStream memStream = new MemoryStream();
                        StreamWriter strWriter = new StreamWriter(memStream);
                        strWriter.Write(lane.playerConfig);
                        strWriter.Flush();
                        memStream.Position = 0;
                        DataContractJsonSerializer playerSerializer = new DataContractJsonSerializer(typeof(BotConstantConfig));
                        BotConstantConfig botCfg = (BotConstantConfig)playerSerializer.ReadObject(memStream);

                        IPlayer newPlayer = new BotConstant(lane.ergId, botCfg.pace, botCfg.spm);
                        players[lane.ergId] = newPlayer;
                    }
                }
            }

            button_StartStop.Content = "Stop";
        }
    }
}
