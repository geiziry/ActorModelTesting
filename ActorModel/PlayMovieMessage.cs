using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorModel
{
    public class PlayMovieMessage
    {
        public PlayMovieMessage(string titleName)
        {
            TitleName = titleName;
        }

        public string TitleName { get; private set; }
    }
}