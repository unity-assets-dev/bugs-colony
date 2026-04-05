using UnityEngine;

[CreateAssetMenu(menuName = "Pools/Create WorkerFactory", fileName = "WorkerFactory", order = 0)]
public class WorkerFactory : ActorModel<Worker>, IWorkerModel {
    
    [SerializeField] private float _searchRadius = 10;
    [SerializeField] private LayerMask _eggMask;
    [SerializeField] private int _foodToSplit;
    [SerializeField] private int _childrenCount;
    
    [SerializeField, Range(10, 100)] private int _mutationChance;

    public float SearchRadius => _searchRadius;
    public LayerMask EggMask => _eggMask;
    
    public bool CanMutateInto(int collectedFood, Vector3 position, ISimulationController simulation) {
        
        if (collectedFood >= _foodToSplit) {
            for (var i = 0; i < _childrenCount; i++) {
                AddChild();
            }

            return true;
        }
        
        return false;

        void AddChild() {
            var random = Random.insideUnitCircle * 2;
            
            if(Random.Range(0, 100) <= _mutationChance && simulation.CountOf<Worker>() >= 10) {
                simulation.AddCocoon<Predator>(position + new Vector3(random.x, 0, random.y));
                return;
            }
            simulation.AddCocoon<Worker>(position + new Vector3(random.x, 0, random.y));
        }
    }

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