namespace BugsColony.App.Models {
    
    public class SimulationModel: IUIKitModel {
        
        public IObservableField<int> Targets { get; } = new ObservableField<int>();
        public IObservableField<int> Kills { get; } = new ObservableField<int>();
        public IObservableField<int> Workers { get; } = new ObservableField<int>();

        public void ResetAll() {
            Kills.Set(0);
            Targets.Set(0);
            Workers.Set(0);
        }
    }
}