using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActorModel.Tests.Properties
{
    public class IntegrationTests : TestKit
    {
        [Fact]
        public void UserShouldUpdatePlayCounts()
        {
            var stats = ActorOfAsTestActorRef(() => new StatisticsActor(ActorOf(BlackHoleActor.Props)));
            var initialMovieStats = new Dictionary<string, int>();
            initialMovieStats.Add("Codenan", 42);
            stats.Tell(
                new InitialStatisticsMessage(new ReadOnlyDictionary<string, int>(initialMovieStats)));

            var user = ActorOfAsTestActorRef<UserActor>(Props.Create(() => new UserActor(stats)));

            user.Tell(new PlayMovieMessage("Codenan"));

            Assert.Equal(43, stats.UnderlyingActor.PlayCounts["Codenan"]);
        }
    }
}