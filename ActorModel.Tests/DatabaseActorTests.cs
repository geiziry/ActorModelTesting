using Akka.Actor;
using Akka.TestKit.Xunit2;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActorModel.Tests
{
    public class DatabaseActorTests : TestKit
    {
        [Fact]
        public void ShouldReadStatsFromDatabase()
        {
            var statsData = new Dictionary<string, int>
            {
                ["Boolean Lies"] = 42,
                ["Codenan"] = 200
            };

            var mockDb = new Mock<IDatabaseGateway>();
            mockDb.Setup(x => x.GetStoredStatistics()).Returns(statsData);

            var actor = ActorOf(Props.Create(() => new DatabaseActor(mockDb.Object)));

            actor.Tell(new GetInitialStatisticsMessage());

            var received = ExpectMsg<InitialStatisticsMessage>();

            Assert.Equal(received.PlayCounts["Boolean Lies"], 42);
            Assert.Equal(received.PlayCounts["Codenan"], 200);
        }
    }
}