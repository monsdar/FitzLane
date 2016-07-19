using NetMQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Media;
using FitzLaneManager.Interfaces;
using FitzLaneManager.Config;
using FitzLaneManager.Receiver;
using FitzLaneManager.Bots;

namespace FitzLaneManager
{
    public partial class MainWindow : Window
    {
        IList<IPlayer> players = new List<IPlayer>();
        IErgReceiver receiver = null;
        IErgSender ergSender = null;
        const string configFilePath = "worldConfig.json";
        LanesContainer lanesContainer = new LanesContainer();
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
                lanesContainer.laneList.Add(lane);
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
            //receiver = new ZmqErgReceiver(context);
            receiver = new SimpleErgReceiver();
            receiver.OnErgReceived += ergReceived;
            receiver.Connect("tcp://127.0.0.1:21743");
            ergSender = new ZmqErgSender(context);
            ergSender.Connect("tcp://127.0.0.1:21744");
            
            //TODO: this is our synchronuous gameloop... better do some threading in the future
            CompositionTarget.Rendering += mainLoop;
        }

        void mainLoop(object sender, EventArgs e)
        {
            //Receive until there is nothing left to receive
            //TODO: What if there's more data than the RenderLoop could handle? This brings down our UI if there's massive traffic...
            while (receiver.TryReceive())
            {}
        }

        void updateButtons()
        {
            stack_Main.Children.Clear();

            foreach (Lane lane in lanesContainer.laneList)
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

        void ergReceived(object sender, ErgEventArgs e)
        {
            //update the bots with what we've got
            foreach (IPlayer player in players)
            {
                player.Update(e.Erg.exerciseTime, e.Erg);

                foreach (UIElement element in stack_Main.Children)
                {
                    if (element.GetType() == typeof(PlayerItem))
                    {
                        ((PlayerItem)element).Update(player);
                    }
                }
            }
            
            ergSender.SendErgs(players);
        }

        private void addPlayerReceived(int laneIndex)
        {
            ConfigureLaneWindow w = new ConfigureLaneWindow(laneIndex);
            w.OnOk += playerConfiguredReceived;
            w.Show();
        }

        void playerConfiguredReceived(int laneIndex, Lane laneCfg)
        {
            for (int i=0; i < lanesContainer.laneList.Count; ++i)
            {
                if (lanesContainer.laneList[i].laneIndex == laneIndex)
                {
                    lanesContainer.laneList[i] = laneCfg;
                    updateButtons();
                    return;
                }
            }
            lanesContainer.laneList.Add(laneCfg);
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
                foreach (Lane lane in lanesContainer.laneList)
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
                        players.Add(newPlayer);
                    }
                }
            }

            button_StartStop.Content = "Stop";
        }
    }
}
