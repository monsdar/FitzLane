using System;
using EasyErgsocket;

namespace FitzBot
{
    class BotConstant : IBot
    {
        EasyErgsocket.Erg thisErg;
        uint originalPace = 0;   //do not forget the original pace in case the bot gets reset
        double amplitude = 0.1;  //amplitude with which the rowing movement is calculated
        double offsetTime = 0.0; //needed if the boat changes its pace
        double offsetDist = 0.0; //needed if the boat changes its pace

        public BotConstant(string givenName = "BotConstant", uint givenPace = 120, uint spm = 18, double distance = 0.0)
        {
            thisErg = new EasyErgsocket.Erg();
            thisErg.cadence = spm;
            thisErg.calories = 1;
            thisErg.distance = distance;
            thisErg.ergtype = EasyErgsocket.ErgType.ROW;
            thisErg.exerciseTime = 0.0;
            thisErg.heartrate = 0;
            thisErg.name = givenName;
            thisErg.paceInSecs = givenPace;
            thisErg.playertype = EasyErgsocket.PlayerType.BOT;
            thisErg.power = 0;
            thisErg.ergId = givenName;

            originalPace = givenPace;
        }

        public string Description
        {
            get
            { return "This Bot rows with a constant pace while showing a typical rowing movement"; }
        }

        public string Name
        {
            get
            { return "BotConstant"; }
        }

        public Erg GetErg()
        {
            return thisErg;
        }

        public void Reset()
        {
            thisErg.paceInSecs = originalPace;
            offsetTime = 0.0;
            offsetDist = 0.0;
        }

        public void Update(double timePassed, Erg givenParent = null)
        {
            thisErg.exerciseTime = timePassed;
            double strokesPerSecond = thisErg.cadence / 60.0;
            double velocity = 500.0 / thisErg.paceInSecs;

            double timePassedOffset = (timePassed - offsetTime); //apply the offset onto the time
            double timeCalc = timePassedOffset + amplitude * -Math.Sin(timePassedOffset * strokesPerSecond * 2.0 * Math.PI);
            thisErg.distance = offsetDist + (velocity * timeCalc);
        }

        public void ChangePace(uint newPace)
        {
            thisErg.paceInSecs = newPace;
            offsetTime = thisErg.exerciseTime;
            offsetDist = thisErg.distance;
        }
    }
}
