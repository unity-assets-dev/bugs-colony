using UnityEngine;

[CreateAssetMenu(menuName = "Pools/Create PredatorFactory", fileName = "PredatorFactory", order = 0)]
public class PredatorFactory : ActorModel<Predator>, IPredatorModel {
    
    [SerializeField] private  float _searchRadius = 10f;
    [SerializeField] private  float _lifeTime = 10f;
    [SerializeField] private  LayerMask _actorMask;
    
    public float SearchRadius => _searchRadius;
    public float LifeTime => _lifeTime;
    public LayerMask ActorMask => _actorMask;
    
    public override Predator CreateActor() {
        var instance = new Predator(this, _pool.Get<PredatorView>(), _map);
        instance.OnDisposed += OnInstanceDisposed;
        return instance;
    }

    private void OnInstanceDisposed(IActor instance, ActorView view) {
        instance.OnDisposed -= OnInstanceDisposed;
        _pool.Dispose(view as  PredatorView);
    }
}