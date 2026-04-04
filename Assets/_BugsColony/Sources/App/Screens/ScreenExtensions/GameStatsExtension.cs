using UnityEngine;

public class GameStatsExtension : MonoBehaviour, ILayoutElement {
    
    [SerializeField] private StatValue _predators;
    [SerializeField] private StatValue _workers;
    
    
    public void OnLayoutShow() {}

    public void UpdateDiedPredators(int value) => _predators.ChangeValue(value.ToString());
    public void UpdateDiedWorks(int value) => _workers.ChangeValue(value.ToString());

    public void OnLayoutHide() {}
}