using UnityEngine;

[CreateAssetMenu(menuName = "Pools/Create CocoonFactory", fileName = "CocoonFactory", order = 0)]
public class CocoonFactory : ActorModel<Cocoon>, ICocoonModel {
    
    [SerializeField] private float _timeToGetNewBorn = 4f;
    
    public float TimeToGetNewBorn => _timeToGetNewBorn;

    public override Cocoon CreateActor() {
        var instance = new Cocoon(this, _pool.Get<CocoonView>());
        instance.OnDisposed += OnInstanceDisposed;
        return instance;
    }

    private void OnInstanceDisposed(IActor instance, ActorView view) {
        instance.OnDisposed -= OnInstanceDisposed;
        _pool.Dispose(view as CocoonView);
    }
}