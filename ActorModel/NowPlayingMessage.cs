using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorModel
{
    public class NowPlayingMessage
    {
        public NowPlayingMessage(string currentlyPlaying)
        {
            CurrentlyPlaying = currentlyPlaying;
        }

        public string CurrentlyPlaying { get; private set; }
    }
}