using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace ActorModel.Tests.Properties
{
    public class StatisticsActorTest : TestKit
    {
        [Fact]
        public void ShouldHaveInitialPlayCountsValue()
        {
            StatisticsActor actor = new StatisticsActor(null);

            Assert.Null(actor.PlayCounts);
        }

        [Fact]
        public void ShouldSetInitialPlayCounts()
        {
            StatisticsActor actor = new StatisticsActor(null);

            var initialMovieStats = new Dictionary<string, int>();
            initialMovieStats.Add("Codenan", 42);

            actor.HandleInitialStaticsMessage(
                new InitialStatisticsMessage(new ReadOnlyDictionary<string, int>(initialMovieStats)));
            Assert.Equal(42, actor.PlayCounts["Codenan"]);
        }

        [Fact]
        public void ShouldReceiveInitialStatisticsMessage()
        {
            TestActorRef<StatisticsActor> actor = ActorOfAsTestActorRef(() => new StatisticsActor(ActorOf(BlackHoleActor.Props)));

            var initialMovieStats = new Dictionary<string, int>();
            initialMovieStats.Add("Codenan", 42);

            actor.Tell(new InitialStatisticsMessage(new ReadOnlyDictionary<string, int>(initialMovieStats)));

            Assert.Equal(42, actor.UnderlyingActor.PlayCounts["Codenan"]);
        }

        [Fact]
        public void ShouldUpdatePlayCounts()
        {
            TestActorRef<StatisticsActor> actor = ActorOfAsTestActorRef(() => new StatisticsActor(ActorOf(BlackHoleActor.Props)));

            var initialMoviewStats = new Dictionary<string, int>();
            initialMoviewStats.Add("Codenan", 42);
            actor.Tell(
                new InitialStatisticsMessage(new ReadOnlyDictionary<string, int>(initialMoviewStats)));
            actor.Tell("Codenan");
            Assert.Equal(43, actor.UnderlyingActor.PlayCounts["Codenan"]);
        }

        [Fact]
        public void ShouldGetInitialStatsFromDatabase()
        {
            var mockDb = CreateTestProbe();

            var messageHandler = new DelegateAutoPilot((sender, message) =>
              {
                  if (message is InitialStatisticsMessage)
                  {
                      var stats = new Dictionary<string, int> { ["Codenan"] = 42 };
                      sender.Tell(new InitialStatisticsMessage(new ReadOnlyDictionary<string, int>(stats)));
                  }
                  return AutoPilot.KeepRunning;
              });

            mockDb.SetAutoPilot(messageHandler);

            TestActorRef<StatisticsActor> actor = ActorOfAsTestActorRef(
                () => new StatisticsActor(mockDb));

            Assert.Equal(42, actor.UnderlyingActor.PlayCounts["Codenan"]);
        }

        [Fact]
        public void ShouldAskDatabaseForInitialStats()
        {
            var mockDb = CreateTestProbe();
            var actor = ActorOf(() => new StatisticsActor(mockDb));

            mockDb.ExpectMsg<GetInitialStatisticsMessage>();
        }
    }
}