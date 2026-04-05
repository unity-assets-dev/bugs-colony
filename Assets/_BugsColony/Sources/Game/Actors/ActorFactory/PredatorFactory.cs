using UnityEngine;

[CreateAssetMenu(menuName = "Pools/Create PredatorFactory", fileName = "PredatorFactory", order = 0)]
public class PredatorFactory : ActorModel<Predator>, IPredatorModel {
    
    [SerializeField] private  float _searchRadius = 10f;
    [SerializeField] private  float _lifeTime = 10f;
    [SerializeField] private  LayerMask _actorMask;
    
    [SerializeField] private int _foodToSplit = 3;
    [SerializeField] private int _childrenCount = 2;
    [SerializeField] private bool _renewOnSupply = true;
    
    public float SearchRadius => _searchRadius;
    public float LifeTime => _lifeTime;
    public LayerMask ActorMask => _actorMask;
    public bool RenewOnSupply => _renewOnSupply;
    
    public bool CanMutateInto(int collectedFood, Vector3 position, ISimulationController simulation) {
        if (collectedFood <= _foodToSplit) {
            for (var i = 0; i < _childrenCount; i++) {
                var random = Random.insideUnitCircle * 2;
                simulation.AddCocoon<Predator>(position + new Vector3(random.x, 0, random.y));
            }
            
            return true;
        }

        return false;
    }


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