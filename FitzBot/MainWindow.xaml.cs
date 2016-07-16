using NetMQ;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FitzBot
{
    public partial class MainWindow : Window
    {
        IList<IBot> bots = new List<IBot>();
        IErgReceiver receiver = null;
        IBotSender botSender = null;
        const string filePath = "worldConfig.json";

        private void WriteDefaultConfig()
        {
            LanesContainer lanesCont = new LanesContainer();
            for (int i = 0; i < 5; ++i)
            {
                Lane lane = new Lane();
                if (i == 2)
                {
                    lane.index = i;
                    lane.isMainPlayer = true;
                    lane.ergId = "Ingo";
                    lane.playerType = typeof(HumanForwarder).Name;

                    HumanForwarderConfig humanFwdCfg = new HumanForwarderConfig();
                    humanFwdCfg.name = lane.ergId;
                    MemoryStream memStream = new MemoryStream();
                    DataContractJsonSerializer humFwdSerializer = new DataContractJsonSerializer(typeof(HumanForwarderConfig));
                    humFwdSerializer.WriteObject(memStream, humanFwdCfg);

                    lane.playerConfig = Encoding.UTF8.GetString(memStream.ToArray());
                }
                else
                {
                    lane.index = i;
                    lane.isMainPlayer = false;
                    lane.ergId = "";
                    lane.playerType = typeof(BotConstant).Name;
                    
                    BotConstantConfig botCfg = new BotConstantConfig();
                    if (i==0)
                    {
                        lane.ergId = "Bot1";
                        botCfg.name = lane.ergId;
                        botCfg.pace = 121;
                        botCfg.spm = 20;
                    }
                    else if (i == 1)
                    {
                        lane.ergId = "Bot2";
                        botCfg.name = lane.ergId;
                        botCfg.pace = 119;
                        botCfg.spm = 19;
                    }
                    else if (i == 3)
                    {
                        lane.ergId = "Bot3";
                        botCfg.name = lane.ergId;
                        botCfg.pace = 120;
                        botCfg.spm = 22;
                    }
                    else if (i == 4)
                    {
                        lane.ergId = "Bot4";
                        botCfg.name = lane.ergId;
                        botCfg.pace = 120;
                        botCfg.spm = 20;
                    }

                    MemoryStream memStream = new MemoryStream();
                    DataContractJsonSerializer botSerializer = new DataContractJsonSerializer(typeof(BotConstantConfig));
                    botSerializer.WriteObject(memStream, botCfg);

                    lane.playerConfig = Encoding.UTF8.GetString(memStream.ToArray());
                }
                lanesCont.laneList.Add(lane);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (FileStream filestream = File.Create(filePath))
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(LanesContainer));
                jsonSerializer.WriteObject(filestream, lanesCont);
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            WriteDefaultConfig();
            
            //init the networking right off the bat
            NetMQContext context = NetMQContext.Create();
            //receiver = new ZmqErgReceiver(context);
            receiver = new SimpleErgReceiver();
            receiver.OnErgReceived += ergReceived;
            receiver.Connect("tcp://127.0.0.1:21743");
            botSender = new ZmqBotSender(context);
            botSender.Connect("tcp://127.0.0.1:21744");
            
            //TODO: this is our synchronuous gameloop... better do some threading in the future
            CompositionTarget.Rendering += mainLoop;
        }
        
        void mainLoop(object sender, EventArgs e)
        {
            //update the UI with the info we have about the bots
            stack_Main.Children.Clear();
            foreach (IBot bot in bots)
            {
                string text = bot.Name + ": " + bot.GetErg().distance.ToString("#.00") + "m";
                stack_Main.Children.Add(new Label{ Content = text });
            }

            //Receive until there is nothing left to receive
            //TODO: What if there's more data than the RenderLoop could handle? This brings down our UI if there's massive traffic...
            while (receiver.TryReceive())
            {}
        }

        void ergReceived(object sender, ErgEventArgs e)
        {
            //update the bots with what we've got
            foreach (IBot bot in bots)
            {
                bot.Update(e.Erg.exerciseTime, e.Erg);
            }
            botSender.SendBots(bots);
        }

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (button_StartStop.Content.ToString() == "Stop")
            {
                bots.Clear();
                button_StartStop.Content = "Start";
            }
            else
            {
                if (File.Exists(filePath))
                {
                    using (FileStream filestream = File.OpenRead(filePath))
                    {
                        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(LanesContainer));
                        LanesContainer lanesCont = (LanesContainer)jsonSerializer.ReadObject(filestream);

                        foreach (Lane lane in lanesCont.laneList)
                        {
                            if (lane.playerType == typeof(HumanForwarder).Name)
                            {
                                MemoryStream memStream = new MemoryStream();
                                StreamWriter strWriter = new StreamWriter(memStream);
                                strWriter.Write(lane.playerConfig);
                                strWriter.Flush();
                                memStream.Position = 0;
                                DataContractJsonSerializer humFwdSerializer = new DataContractJsonSerializer(typeof(HumanForwarderConfig));
                                HumanForwarderConfig humanFwdCfg = (HumanForwarderConfig)humFwdSerializer.ReadObject(memStream);

                                IBot humanFwd = new HumanForwarder(lane.ergId);
                                bots.Add(humanFwd);
                            }
                            else if (lane.playerType == typeof(BotConstant).Name)
                            {
                                MemoryStream memStream = new MemoryStream();
                                StreamWriter strWriter = new StreamWriter(memStream);
                                strWriter.Write(lane.playerConfig);
                                strWriter.Flush();
                                memStream.Position = 0;
                                DataContractJsonSerializer botSerializer = new DataContractJsonSerializer(typeof(BotConstantConfig));
                                BotConstantConfig botCfg = (BotConstantConfig)botSerializer.ReadObject(memStream);

                                IBot newBot = new BotConstant(lane.ergId, botCfg.pace, botCfg.spm);
                                bots.Add(newBot);
                            }
                        }
                    }
                }

                button_StartStop.Content = "Stop";
            }
        }
    }
}
