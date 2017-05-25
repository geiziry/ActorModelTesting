using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorModel
{
    public class StatisticsActor : ReceiveActor
    {
        public Dictionary<string, int> PlayCounts { get; set; }
        private readonly IActorRef _databaseActor;

        public StatisticsActor(IActorRef databaseActor)
        {
            Receive<InitialStatisticsMessage>(message => HandleInitialStaticsMessage(message));
            Receive<string>(title => HandleTitleMessage(title));
            _databaseActor = databaseActor;
        }

        public void HandleTitleMessage(string title)
        {
            if (PlayCounts.ContainsKey(title))
            {
                PlayCounts[title]++;
            }
            else
            {
                PlayCounts.Add(title, 1);
            }
        }

        public void HandleInitialStaticsMessage(InitialStatisticsMessage message)
        {
            PlayCounts = new Dictionary<string, int>(message.PlayCounts);
        }

        protected override void PreStart()
        {
            _databaseActor.Tell(new GetInitialStatisticsMessage());
        }
    }
}