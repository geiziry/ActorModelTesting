using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorModel.Tests
{
    public class MockDatabaseActor : ReceiveActor
    {
        public MockDatabaseActor()
        {
            Receive<GetInitialStatisticsMessage>(
                message =>
                {
                    var stats = new Dictionary<string, int> { ["Codenan"] = 42 };
                    Sender.Tell(new InitialStatisticsMessage(new ReadOnlyDictionary<string, int>(stats)));
                });
        }
    }
}