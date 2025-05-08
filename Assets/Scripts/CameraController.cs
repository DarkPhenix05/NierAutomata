using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    private float _fov, _dtFov, _startFov;
    public int _camSpeed;

    public Vector3 _startTrans;

    void Start()
    {        
        _camera = Camera.main;
        _startTrans = _camera.gameObject.transform.position;

        if(_camera != null)
        {
            _fov = _camera.fieldOfView;
            _startFov = _fov;
        }
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (_camera != null)
        { 
            if(Input.GetKey(KeyCode.W))
            {
                Vector3 pos = _camera.transform.localPosition;
                pos.y += _camSpeed * Time.deltaTime;
                _camera.transform.localPosition = pos;
            }
            else if(Input.GetKey(KeyCode.S))
            {
                Vector3 pos = _camera.transform.localPosition;
                pos.y -= _camSpeed * Time.deltaTime;
                _camera.transform.localPosition = pos;
            }


            if(Input.GetKey(KeyCode.A))
            {
                Vector3 pos = _camera.transform.localPosition;
                pos.x -= _camSpeed * Time.deltaTime;
                _camera.transform.localPosition = pos;
            }
            else if( Input.GetKey(KeyCode.D))
            {
                Vector3 pos = _camera.transform.localPosition;
                pos.x += _camSpeed * Time.deltaTime;
                _camera.transform.localPosition = pos;
            }

            _dtFov = -Input.mouseScrollDelta.y;
            if(_dtFov != 0)
            {
                if (_dtFov < 0 && _fov > 12)
                {
                    _fov += _dtFov;
                }
                else if (_dtFov > 0)
                {
                    _fov += _dtFov;
                }

            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                _camera.transform.position = _startTrans;
                _fov = _startFov;
            }
            
            _camera.fieldOfView = _fov;
        }
    }
}
