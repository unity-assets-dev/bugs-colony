using UnityEngine;

public class GameStatsExtension : MonoBehaviour, ILayoutElement {
    
    [SerializeField] private StatValue _kills;
    [SerializeField] private StatValue _targets;
    [SerializeField] private StatValue _workers;
    
    
    public void OnLayoutShow() {}
    
    public void UpdateKills(int value) => _kills.ChangeValue(value.ToString());

    public void UpdateTargets(int value) => _targets.ChangeValue(value.ToString());
    public void UpdateWorkers(int value) => _workers.ChangeValue(value.ToString());

    public void OnLayoutHide() {}

    
}