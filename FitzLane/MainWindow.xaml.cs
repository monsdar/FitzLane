﻿using NetMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using FitzLane.Interfaces;
using FitzLane.Config;
using FitzLane.Plugin;
using FitzLanePlugin.Interfaces;

namespace FitzLane
{
    public partial class MainWindow : Window
    {
        //First: ErgId, Second: Player instance... putting the players into a map makes it easier to find them
        IDictionary<string, IPlayer> players = new Dictionary<string, IPlayer>();
        IErgSender ergSender = null;
        const string configFilePath = "worldConfig.json";
        ConfigReader reader = null;
        PlayerProviderLoader playerProviderLoader = null;

        public MainWindow()
        {
            InitializeComponent();

            //get the PlayerLoaders from the plugins
            playerProviderLoader =  new PlayerProviderLoader("plugins/player/");

            // Load default config (this loads an existing config or the previously created default config
            reader = new ConfigReader(configFilePath);
            foreach (Lane lane in reader.getLanes())
            {
                laneList.AddLaneConfig(lane);
                CreatePlayerFromLane(lane);
            }
            
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
                foreach (IPlayer player in players.Values)
                {
                    //do not update the same player twice
                    if(alreadyUpdatedPlayers.Contains(player.GetErg().ergId))
                    {
                        continue;
                    }

                    //update all the player without parents or whose parents have already been updated
                    if(player.ParentId == "")
                    {
                        player.Update(null);
                        alreadyUpdatedPlayers.Add(player.GetErg().ergId);
                        hasPlayerBeenUpdated = true;
                    }
                    else if (alreadyUpdatedPlayers.Contains(player.ParentId))
                    {
                        EasyErgsocket.Erg parent = players[player.ParentId].GetErg();
                        player.Update(parent);
                        alreadyUpdatedPlayers.Add(player.GetErg().ergId);
                        hasPlayerBeenUpdated = true;
                    }
                    else
                    {
                        //This lane has a parent that does not exist yet...
                        continue;
                    }
                }

                if (!hasPlayerBeenUpdated)
                {
                    //no player updated... there seems to be a problem?
                    //TODO: What should we do? This typically means there is a erg without parent or a circular dependency...
                    break;
                }
            }

            //update the GUI
            laneList.UpdatePlayer(players.Values.ToList());

            //send the bots to network
            ergSender.SendErgs(players.Values.ToList());
        }

        void CreatePlayerFromLane(Lane givenLane)
        {
            foreach (IPlayerProvider provider in playerProviderLoader.GetPlayerProvider())
            {
                if (provider.IsValidPlayertype(givenLane.playerType))
                {
                    IPlayer player = provider.GetPlayer(givenLane.playerConfig);
                    player.ParentId = givenLane.parentId;
                    players[givenLane.ergId] = player;
                    laneList.UpdatePlayer(player);
                }
            }
        }

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            if (button_StartStop.Content.ToString() == "Stop")
            {
                button_StartStop.Content = "Start";
            }
            else
            {
                button_StartStop.Content = "Stop";
            }

        }
    }
}
