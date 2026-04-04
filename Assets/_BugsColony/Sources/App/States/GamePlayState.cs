using BugsColony.App.Models;

namespace BugsColony.App.States {
    
    public class GamePlayState: ScreenState<GamePlayScreen>, IScreenState {
        
        private readonly SimulationModel _model;
        private readonly ISimulationScenario _scenario;
        private readonly IButtonCommand _pauseGameCommand;

        public GamePlayState(SimulationModel model, ISimulationScenario scenario, GamePlayScreen screen) : base(screen) {
            _model = model;
            _scenario = scenario;
        }

        protected override void OnStateEnter() {
            if (Screen.TryGetExtension<GameStatsExtension>(out var stats)) {
                _model.Predators.AddListener(stats.UpdateDiedPredators);
                _model.Workers.AddListener(stats.UpdateDiedWorks);
            }
            
            Screen.OnButtonClick<RestartGameButton>(() => _scenario.Restart());
            
            _scenario.Run();
        }

        protected override void OnStateExit() {
            if (Screen.TryGetExtension<GameStatsExtension>(out var stats)) {
                _model.Predators.RemoveListener(stats.UpdateDiedPredators);
                _model.Workers.RemoveListener(stats.UpdateDiedWorks);
            }
            
            _scenario.Stop();
        }
    }
}