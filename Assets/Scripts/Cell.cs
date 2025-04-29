using UnityEngine;

public class Cell : MonoBehaviour
{
    private bool Life;



    public void SetState(bool state)
    {
        Life = state;
    }

    public bool GetState()
    {
        return (Life);
    }
}
