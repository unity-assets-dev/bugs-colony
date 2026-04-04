using System;
using UnityEngine;

public interface IActor {
    
    event Action<ActorStateAction> OnStateChanged;
    event Action<IActor, ActorView> OnDisposed;
    
    Vector3 Position { get; set; }

    void OnUpdateFrame(float dt);
    void Dispose();
    
}

public interface IBugActor : IActor {
    void SetState(ActorState state);
    void OnMutate(ISimulationController simulation);
    
}

public interface ICocoonActor: IActor {
    void CompleteEvolution();
}

public interface IConsumableTarget : IActor {
    void OnConsume();
}

public interface IInteractionHandler {
    void ConsumeTarget(IActor actor);
}

public interface IActorView {
    Transform Transform { get; }
    IActor Actor { get; set; }
    void SetDestination(Vector3 destination);
}