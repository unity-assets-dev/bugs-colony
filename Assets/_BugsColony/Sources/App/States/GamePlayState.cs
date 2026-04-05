using BugsColony.App.Models;

namespace BugsColony.App.States {
    
    public class GamePlayState: ScreenState<GamePlayScreen>, IScreenState {
        
        private readonly SimulationModel _model;
        private readonly ISimulationScenario _scenario;
        private readonly IButtonCommand _pauseGameCommand;
        private GameStatsExtension _stats;
        private int _count;

        public GamePlayState(SimulationModel model, ISimulationScenario scenario, GamePlayScreen screen) : base(screen) {
            _model = model;
            _scenario = scenario;
        }

        protected override void OnStateEnter() {
            _count = 0;
            _model.Targets.Set(20);
            
            if (Screen.TryGetExtension<GameStatsExtension>(out var stats)) {
                _stats = stats;
                _model.Kills.AddListener(stats.UpdateKills);
                _model.Targets.AddListener(stats.UpdateTargets);
                _model.Workers.AddListener(OnWorkersCountChanged);
            }
            
            Screen.OnButtonClick<RestartGameButton>(() => _scenario.Restart());
            
            _scenario.Run();
        }

        private void OnWorkersCountChanged(int count) {
            if (_model.Targets.Value <= count) {
                _scenario.Restart();
                return;
            }
            
            if (_count != count && count <= 0) {
                _count = count;
                _scenario.Restart();
                return;
            }
            _stats.UpdateWorkers(count);
        }

        protected override void OnStateExit() {
            if (_stats != null) {
                _model.Kills.RemoveListener(_stats.UpdateKills);
                _model.Targets.RemoveListener(_stats.UpdateTargets);
                _model.Workers.RemoveListener(OnWorkersCountChanged);
            }
            
            _scenario.Stop();
        }
    }
}