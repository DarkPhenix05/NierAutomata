using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool _life = false;
    private SpriteRenderer _SRenderer;
    private Camera _cam;

    public int CellAutomatCounter = 0;

    void Start()
    {
        _SRenderer = GetComponent<SpriteRenderer>();
        _cam = Camera.main;
    }

    public void SetState(bool state)
    {
        _life = state;

        if (!_SRenderer)
        {
            _SRenderer = GetComponent<SpriteRenderer>();
        }
        _SRenderer.color = _life ? Color.white : Color.black;
    }

    private void Flip()
    {
        _life = !_life;

        if (!_SRenderer)
        {
            _SRenderer = GetComponent<SpriteRenderer>();
        }

        _SRenderer.color = _life ? Color.white : Color.black;
    }

    public bool GetState()
    {
        return (_life);
    }

    private void OnMouseOver()
    {
        //Debug.Log("Over: " + this.gameObject.ToString());
        if (CelularAutomat1D.Instance != null)
        {
            //Debug.Log("D1");
            if (!CelularAutomat1D.Instance.GetRunning() && Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Click D1");
                Flip();
            }
        }

        else if (CelularAutomat2D.Instance != null)
        {
            //Debug.Log("D1");
            if (!CelularAutomat2D.Instance.GetRunning() && Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Click D2");
                Flip();
            }
        }
    }

    void FixedUpdate()
    {
        if (CelularAutomat1D.Instance != null)
        {
            Vector3 viewPos = _cam.WorldToScreenPoint(transform.position);

            if (viewPos.y < 0f || viewPos.y > Screen.height ||
                viewPos.z < 0f) // z < 0 means behind the camera
            {
                gameObject.SetActive(false);
            }
        }
    }
}
