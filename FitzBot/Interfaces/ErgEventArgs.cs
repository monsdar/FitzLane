using System;

namespace FitzBot
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
