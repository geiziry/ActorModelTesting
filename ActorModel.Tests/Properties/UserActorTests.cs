using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using System;
using Xunit;

namespace ActorModel.Tests.Properties
{
    public class UserActorTests : TestKit
    {
        [Fact]
        public void ShouldHaveInitialState()
        {
            TestActorRef<UserActor> actor = ActorOfAsTestActorRef<UserActor>(
                Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));

            Assert.Null(actor.UnderlyingActor.CurrentlyPlaying);
        }

        [Fact]
        public void ShouldUpdateCurrentlyPlayingState()
        {
            TestActorRef<UserActor> actor = ActorOfAsTestActorRef<UserActor>(
                Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));

            actor.Tell(new PlayMovieMessage("Codenan"));

            Assert.Equal("Codenan", actor.UnderlyingActor.CurrentlyPlaying);
        }

        [Fact]
        public void ShouldPlayMovie()
        {
            IActorRef actor = ActorOfAsTestActorRef<UserActor>(
                Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));

            actor.Tell(new PlayMovieMessage("Codenan"));
            var received = ExpectMsg<NowPlayingMessage>(TimeSpan.FromSeconds(5));
            Assert.Equal("Codenan", received.CurrentlyPlaying);
        }

        [Fact]
        public void ShouldLogPlayMovie()
        {
            var actor = ActorOf(Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));

            EventFilter.Info("Started playing Boolean Lies")
                .And
                .Info("Replying to sender")
                .Expect(2, () => actor.Tell(new PlayMovieMessage("Boolean Lies")));
        }

        [Fact]
        public void ShouldSendToDeadLetter()
        {
            var actor = ActorOf(Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));

            EventFilter.DeadLetter<PlayMovieMessage>(
                message => message.TitleName == "Boolean Lies"
                ).ExpectOne(() => actor.Tell(new PlayMovieMessage("Boolean Lies")));
        }

        [Fact]
        public void ShouldErrorOnUnknownMoview()
        {
            var actor = ActorOf(Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));

            EventFilter.Exception<NotSupportedException>()
                .ExpectOne(() => actor.Tell(new PlayMovieMessage("Null Terminator")));
        }

        [Fact]
        public void ShouldPublishPlayingMovie()
        {
            var actor = ActorOf(Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));

            var subscriber = CreateTestProbe();

            Sys.EventStream.Subscribe(subscriber, typeof(NowPlayingMessage));

            actor.Tell(new PlayMovieMessage("Codenan"));

            subscriber.ExpectMsg<NowPlayingMessage>(
                message => message.CurrentlyPlaying == "Codenan");
        }

        [Fact]
        public void ShouldTerminate()
        {
            var actor = ActorOf(Props.Create(() => new UserActor(ActorOf(BlackHoleActor.Props))));

            Watch(actor);

            actor.Tell(PoisonPill.Instance);

            ExpectTerminated(actor);
        }
    }
}