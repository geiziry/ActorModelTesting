using System.Collections.Generic;

namespace ActorModel
{
    public interface IDatabaseGateway
    {
        IDictionary<string, int> GetStoredStatistics();
    }
}