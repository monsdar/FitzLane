using System;
using FitzLane.Interfaces;

namespace FitzLane.Bots
{
    class SimpleBot : IPlayer
    {
        EasyErgsocket.Erg thisErg;

        public SimpleBot() : this("SimpleBot")
        {}

        public SimpleBot(string givenName)
        {
            thisErg = new EasyErgsocket.Erg();
            thisErg.cadence = 1;
            thisErg.calories = 1;
            thisErg.distance = 0.0;
            thisErg.ergtype = EasyErgsocket.ErgType.ROW;
            thisErg.exerciseTime = 0.0;
            thisErg.heartrate = 0;
            thisErg.name = givenName;
            thisErg.paceInSecs = 120;
            thisErg.playertype = EasyErgsocket.PlayerType.BOT;
            thisErg.power = 0;

            thisErg.ergId = NameToId(givenName);
        }

        public string Description
        {
            get
            {
                return "This is a SimpleBot, it just returns given parent distance";
            }
        }

        public string Name
        {
            get
            {
                return "SimpleBot";
            }
        }

        public EasyErgsocket.Erg GetErg()
        {
            return thisErg;
        }

        public void Reset()
        {
            thisErg.distance = 0.0;
        }

        public void Update(EasyErgsocket.Erg givenParent = null)
        {
            if(givenParent != null)
            {
                thisErg.distance = givenParent.distance;
                thisErg.exerciseTime = givenParent.exerciseTime;
            }
        }

        private string NameToId(string givenName)
        {
            int nameSeed = 0;
            foreach (char character in givenName)
            {
                nameSeed += character;
            }
            Random rnd = new Random(nameSeed);
            return rnd.Next().ToString();
        }
    }
}
