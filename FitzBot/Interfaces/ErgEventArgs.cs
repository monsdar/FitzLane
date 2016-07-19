using System;

namespace FitzLaneManager.Interfaces
{
    public class ErgEventArgs : EventArgs
    {
        public EasyErgsocket.Erg Erg { get; set; }

        public ErgEventArgs(EasyErgsocket.Erg givenErg)
        {
            this.Erg = givenErg;
        }
    }
}
