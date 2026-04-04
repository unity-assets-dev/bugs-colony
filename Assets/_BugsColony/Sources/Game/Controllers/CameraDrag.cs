using PrimeTween;
using UnityEngine;

public class CameraDrag : MonoBehaviour {
    private Vector3 _startDrag;

    private Vector3 RotateToCameraRotation(Vector3 input, float cameraRotation = 45f) {
        var rotation = Quaternion.Euler(0, cameraRotation, 0);
        return rotation * input;
    }
    
    private void Update() {
        
        if (Input.GetMouseButtonDown(0)) {
            _startDrag = Input.mousePosition;
        }
        
        if (Input.GetMouseButtonUp(0)) {
            _startDrag = Vector3.zero;

            OnDragRelease();
        }
        
        if (Input.GetMouseButton(0)) {
            var offset = (Input.mousePosition - _startDrag) * .05f;
            transform.position = RotateToCameraRotation(Vector3.up * 20 + new  Vector3(offset.x, 0, offset.y));
        }
    }

    private void OnDragRelease() {
        transform.DOMove(Vector3.up * 20, .25f);
    }
}
