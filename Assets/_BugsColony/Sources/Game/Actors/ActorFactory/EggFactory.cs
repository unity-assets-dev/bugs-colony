using UnityEngine;

[CreateAssetMenu(menuName = "Pools/Create EggFactory", fileName = "EggFactory", order = 0)]
public class EggFactory : ActorModel<Egg> {
    
    public override Egg CreateActor() {
        var instance = new Egg(_pool.Get<EggView>());
        instance.OnDisposed += OnInstanceDisposed;
        return instance;
    }

    private void OnInstanceDisposed(IActor instance, ActorView view) {
        instance.OnDisposed -= OnInstanceDisposed;
        _pool.Dispose(view as EggView);
    }
}