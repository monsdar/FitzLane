using NetMQ;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public MainWindow()
        {
            InitializeComponent();

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
                IBot newBot1 = new BotConstant("Bot1", 121, 20);
                IBot newBot2 = new BotConstant("Bot2", 119, 19);
                IBot newBot3 = new BotConstant("Bot3", 120, 22);

                bots.Add(newBot1);
                bots.Add(newBot2);
                bots.Add(newBot3);

                button_StartStop.Content = "Stop";
            }
        }
    }
}
