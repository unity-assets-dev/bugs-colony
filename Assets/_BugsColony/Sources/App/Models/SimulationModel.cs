namespace BugsColony.App.Models {
    
    public class SimulationModel: IUIKitModel {
        
        public IObservableField<int> Predators { get; } = new ObservableField<int>();
        public IObservableField<int> Workers { get; } = new ObservableField<int>();

        public void ResetAll() {
            Predators.Set(0);
            Workers.Set(0);
        }
    }
}