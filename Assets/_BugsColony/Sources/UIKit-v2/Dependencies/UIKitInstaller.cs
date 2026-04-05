using UnityEngine;

public class UIKitInstaller : KitInstallerBase {
    [SerializeField] private PoolHandler[] _pools;
    
    protected override void BindServices() {
        Container
            .BindAsSingleFromInstanceMono<CameraDrag>();
        
        Container
            .BindAsSingleFromInstanceMono<ScenarioMap>();
        
        _pools.EachNonAlloc(pool => Container.BindAsSingleFromInstance(pool));
        Container.BindAsSingleFromInstance(_pools);
        
        Container
            .BindAsSingleFromInstanceMono<ViewsPool>();
        
        /*TypeOf<IActorFactory>() // Moved to scriptable objects to simplify extending
            .EachNonAlloc(type => Container.BindInterfacesAndSelfTo(type).AsSingle());*/
        
        Container
            .BindInterfacesAndSelfTo<ActorFactory>()
            .AsSingle();
        
        Container
            .BindInterfacesAndSelfTo<ActorContainer>()
            .AsSingle();
        
        Container
            .BindInterfacesAndSelfTo<SimulationController>()
            .AsSingle();
        
        Container
            .BindInterfacesAndSelfTo<SimulationScenario>()
            .AsSingle();
    }
    
    
    protected override void OnBindTargetInstances() {}
}
