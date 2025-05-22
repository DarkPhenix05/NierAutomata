using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _playerUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseMenu()
    {
        if(!_pauseMenu || !_playerUI)
        {
            return;
        }
        
        if(!_pauseMenu.activeInHierarchy)
        {
            _pauseMenu.SetActive(true);
            _playerUI.SetActive(false);
        }
        else
        {
            _pauseMenu.SetActive(false);
            _playerUI.SetActive(true);
        }
    }

    public void ExitButton()
    {
#if UNITY_EDITOR

        Debug.Log("EXIT BUTTON ACTION");
        
#endif
        Application.Quit();
    }
}
