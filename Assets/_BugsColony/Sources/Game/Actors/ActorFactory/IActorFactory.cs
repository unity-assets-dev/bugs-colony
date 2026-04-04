public interface IActorFactory {}

public interface IActorFactory<out T>: IActorFactory where T : IActor {
    T CreateActor();
}