﻿using System;
using System.Collections.Generic;
using FitzLaneManager.Interfaces;

namespace FitzLaneManager
{
    class SimpleErgSender : IErgSender
    {
        public void Connect(string address)
        {
            Console.WriteLine("Connecting to " + address);
        }

        public void SendErgs(IList<IPlayer> playerList)
        {
            foreach (IPlayer player in playerList)
            {
                Console.WriteLine("Sending " + player.GetErg().name + " (" + player.GetErg().ergId + ")");
            }
        }
    }
}