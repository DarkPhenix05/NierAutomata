using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool Life = false;
    private SpriteRenderer SRenderer;
    private Camera cam;

    void Start()
    {
        SRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
    }

    public void SetState(bool state)
    {
        Life = state;

        if (!SRenderer)
        {
            SRenderer = GetComponent<SpriteRenderer>();
        }
        SRenderer.color = Life ? Color.white : Color.black;
    }

    private void Flip()
    {
        Life = !Life;

        if (!SRenderer)
        {
            SRenderer = GetComponent<SpriteRenderer>();
        }

        SRenderer.color = Life ? Color.white : Color.black;
    }

    public bool GetState()
    {
        return (Life);
    }

    private void OnMouseOver()
    {
        //Debug.Log("Over: " + this.gameObject.ToString());
        if (CelularAutomat1D.Instance != null)
        {
            if (!CelularAutomat1D.Instance.GetRunning() && Input.GetMouseButtonDown(0))
            {
                Flip();
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 viewPos = cam.WorldToScreenPoint(transform.position);

        if (viewPos.y < 0f || viewPos.y > Screen.height ||
            viewPos.x < 0f || viewPos.x > Screen.width ||
            viewPos.z < 0f) // z < 0 means behind the camera
        {
            gameObject.SetActive(false);
        }
    }
}
