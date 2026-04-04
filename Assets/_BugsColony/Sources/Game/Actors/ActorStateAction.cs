public enum ActorState { None, Alive, Died, Consume, Evolution }

public struct ActorStateAction {
    
    public IActor Sender { get; set; }
    public IActor Target { get; set; }
    public ActorState SenderState { get; set; }

    private static ActorStateAction FromState(IActor actor, ActorState state, IActor target = null) =>
        new() {
            Sender = actor,
            Target = target,
            SenderState = state,
        };

    public static ActorStateAction None(IActor actor) => FromState(actor, ActorState.None);
    public static ActorStateAction Alive(IActor actor) => FromState(actor, ActorState.Alive);
    public static ActorStateAction Died(IActor actor) => FromState(actor, ActorState.Died);
    public static ActorStateAction Consume(IActor actor, IActor target) => FromState(actor, ActorState.Consume, target);
    public static ActorStateAction Evolution(IActor actor) => FromState(actor, ActorState.Evolution);

    public static ActorStateAction State(IActor actor, ActorState state) => FromState(actor, state);
}