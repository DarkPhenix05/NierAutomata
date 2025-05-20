using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool _life = false;
    
    public int _curState = 0;
    public int _maxState = 0;
    
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
    
    public void SetState(int state)
    {
        _curState = state;
        
        if(!_SRenderer)
        {
            _SRenderer = GetComponent<SpriteRenderer>();
        }

        float lerp = _maxState > 0 ? (float) _curState / _maxState : 0;
        _SRenderer.color = Color.Lerp(Color.white, Color.black, lerp);
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
        if (CelularAutomat1D.Instance != null)
        {
            if (!CelularAutomat1D.Instance.GetRunning() && Input.GetMouseButtonDown(0))
            {
                Flip();
                CelularAutomat1D.Instance.SetGenerationText();
            }
        }
        
        else if(CelularAutomat2D.Instance != null)
        {
            
        }

        else if (GameOfLife.Instance != null)
        {
            if (!GameOfLife.Instance.GetRunning() && Input.GetMouseButtonDown(0))
            {
                Flip();
                GameOfLife.Instance.SetGenerationText();
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
