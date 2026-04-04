using UnityEngine;
using Zenject;

public class AppEntry : MonoBehaviour {
    
    private AppStates _states;
    private SimulationScenario _scenario;

    [Inject]
    private void Construct(AppStates states, SimulationScenario scenario) {
        _scenario = scenario;
        _states = states;
    }

    private void Start() {
        _states.ChangeState<BootstrapState>();
    }

    private void Update() {
        _scenario.UpdateFrame(Time.deltaTime);
    }
}
