using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public bool _state;
    public Button _button;

    public void FlipButton()
    {
        _state = !_state;
        _button.GetComponentInChildren<TextMeshProUGUI>().text = _state.ToString();
    }

    public void SetButton(bool state)
    {
        _state = state;
        _button.GetComponentInChildren<TextMeshProUGUI>().text = _state.ToString();
    }

    public bool GetState()
    { 
        return _state; 
    }
}
