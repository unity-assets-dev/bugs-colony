using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatValue : MonoBehaviour {
    [SerializeField] private Sprite _iconSource;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _valueField;

    [SerializeField] private bool _animateChanges = false;

    private void OnValidate() {
        if(_iconSource && _icon.sprite != _iconSource)
            _icon.sprite = _iconSource;
    }

    public void ChangeValue(string value) {
        if(_valueField.text != value) {
            _valueField.text = value;

            if (_animateChanges) {
                // TODO: Icon shake?
                _icon.DOShakeScale( .25f, Vector2.one * .5f);
            }
        }
        
    }
}
