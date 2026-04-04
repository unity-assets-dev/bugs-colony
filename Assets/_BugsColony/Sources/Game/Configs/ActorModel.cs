using UnityEngine;

public abstract class PoolHandler:  ScriptableObject {
    public abstract void RegisterPool(ViewsPool pool, IHeatMap heatMap);
}

public abstract class ActorModel<TActor> : PoolHandler, IActorFactory<TActor> where TActor : IActor {
    
    [SerializeField] private ActorView _prefab;

    protected ViewsPool _pool;
    protected IHeatMap _map;

    public override void RegisterPool(ViewsPool pool, IHeatMap heatMap) {
        _map = heatMap;
        _pool = pool;
        _pool.AddView(_prefab);
    }

    public abstract TActor CreateActor();
}
