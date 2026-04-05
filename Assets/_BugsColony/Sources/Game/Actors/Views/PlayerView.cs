using UnityEngine;

public class PlayerView : ActorView {
    [SerializeField] private Transform _modelView;
    
    public void RotateTo(Vector3 direction, float dt) {
        _modelView.localRotation = Quaternion.LookRotation(direction);
    }
}