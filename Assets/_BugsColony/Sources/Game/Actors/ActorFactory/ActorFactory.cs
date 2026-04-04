using System;
using System.Collections.Generic;
using System.Linq;

public class ActorFactory {

    private readonly HashSet<IActorFactory> _factories = new();

    public ActorFactory(IActorFactory[] factories) => _factories.UnionWith(factories);

    private bool TryGetFactory<TActor>(out IActorFactory<TActor> factory) where TActor : IActor {
        factory = _factories.OfType<IActorFactory<TActor>>().FirstOrDefault();
        
        return factory != null;
    }
    
    public TActor CreateActor<TActor>() where TActor : IActor {
        if (TryGetFactory(out IActorFactory<TActor> factory))
            return factory.CreateActor();

        throw new NullReferenceException($"Factory for {typeof(TActor).Name} not found.");
    }
}