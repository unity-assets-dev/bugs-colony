using System;
using PrimeTween;
using UnityEngine;

public class CameraDrag : MonoBehaviour {
    private Vector3 _startDrag;
    private Vector3 _position;

    private void Awake() {
        _position = transform.position;
    }

    private Vector3 RotateToCameraRotation(Vector3 input, float cameraRotation = 45f) {
        var rotation = Quaternion.Euler(0, cameraRotation, 0);
        return rotation * input;
    }

    public void Follow(Vector3 position) {
        _position = new Vector3(position.x, transform.position.y, position.z);
    }
    
    private void Update() {
        transform.position = Vector3.MoveTowards(transform.position, _position, Time.deltaTime * 15f);
        /*if (Input.GetMouseButtonDown(0)) {
            _startDrag = Input.mousePosition;
        }
        
        if (Input.GetMouseButtonUp(0)) {
            _startDrag = Vector3.zero;

            OnDragRelease();
        }
        
        if (Input.GetMouseButton(0)) {
            var offset = (Input.mousePosition - _startDrag) * .05f;
            transform.position = RotateToCameraRotation(Vector3.up * 20 - new  Vector3(offset.x, 0, offset.y));
        }*/
    }

    private void OnDragRelease() {
        transform.DOMove(Vector3.up * 20, .25f);
    }
}
