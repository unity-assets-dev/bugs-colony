using UnityEngine;

[CreateAssetMenu(menuName = "Pools/Create WorkerFactory", fileName = "WorkerFactory", order = 0)]
public class WorkerFactory : ActorModel<Worker>, IWorkerModel {
    [SerializeField] private float _searchRadius = 10;
    [SerializeField] private LayerMask _eggMask;

    public float SearchRadius => _searchRadius;
    public LayerMask EggMask => _eggMask;

    public override Worker CreateActor() {
        var instance = new Worker(this, _pool.Get<WorkerView>(), _map);
        instance.OnDisposed += OnInstanceDisposed;
        return instance;
    }

    private void OnInstanceDisposed(IActor instance, ActorView view) {
        instance.OnDisposed -= OnInstanceDisposed;
        _pool.Dispose(view as WorkerView);
    }
}