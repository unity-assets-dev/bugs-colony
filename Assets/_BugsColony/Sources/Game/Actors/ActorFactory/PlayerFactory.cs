using UnityEngine;

[CreateAssetMenu(menuName = "Pools/Create PlayerFactory", fileName = "PlayerFactory", order = 0)]
public class PlayerFactory : ActorModel<Player>, IPlayerModel {
    
    [SerializeField] private  float _searchRadius = 10f;
    [SerializeField] private  float _lifeTime = 10f;
    [SerializeField] private  LayerMask _actorMask;
    
    public float SearchRadius => _searchRadius;
    public float LifeTime => _lifeTime;
    public LayerMask ActorMask => _actorMask;

    public override Player CreateActor() {
        var instance = new Player(this, _pool.Get<PlayerView>(), _map);
        instance.OnDisposed += OnInstanceDisposed;
        return instance;
    }

    private void OnInstanceDisposed(IActor instance, ActorView view) {
        instance.OnDisposed -= OnInstanceDisposed;
        _pool.Dispose(view as  PredatorView);
    }
}