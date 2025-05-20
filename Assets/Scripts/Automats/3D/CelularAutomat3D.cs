using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class CelularAutomat3D : MonoBehaviour
{
    public static CelularAutomat3D Instance;

    [Header("PARAMETERS")]
    public GameObject _preCell;
    
    public int _matixSize;
    public float _size;
    
    public float _spacing;

    [Header("LISTS")] 
    private List<List<List<Cell>>> _cells = new List<List<List<Cell>>>();

    private List<List<List<int>>> _parentMatrix = new List<List<List<int>>>();
    private List<List<List<int>>> _childMatrix = new List<List<List<int>>>();

    public bool _corners;
    public bool _wrap;

    [Header("BUTTONS")]

    public Button _startStop;
    public bool _running = false;

    [Header("INPUT-FIELD")]
    public TMP_InputField _inputWait;
    public float _wait = 0.1f;
    private float _timer;

    public TMP_InputField _inputState;
    public int _state;

    public TMP_InputField _inputLive;
    public List<int> _liveVals;

    public TMP_InputField _inputDead;
    public List<int> _deadVals;

    public TMP_InputField _inputBirth;
    public List<int> _birthVals;


    [Header("GENERATION-FIELD")]
    public TextMeshProUGUI _genTMP;
    public int _generationN = 0;

    [Header("SFX")]
    private AudioSource _audioSource;
    public List<AudioClip> _SFX;

    public void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Start()
    {
        _inputWait.text = "Wait: " + _wait.ToString("F1");

        _size = _preCell.transform.localScale.x;
        
        SetRun(false);
        
        ResetGeneration();
        SetStateText();
        
        MakeFirstGeneration();
    }

    public void Update()
    {
        if (_running)
        {
            _timer += Time.deltaTime;
            if (_timer >= _wait)
            {
                UperateGeneration();
                _timer = 0.0f;
            }
        }
    }

    private void UperateGeneration()
    {
        SetChildren();
        SetUpCells();
        _parentMatrix = _childMatrix;
        
        AddGeneration();
    }

    private void SetChildren()
    {
        for (var x = 0; x < _parentMatrix.Count;  x++)
        {
            for(var y = 0; y < _parentMatrix[x].Count; y++)
            {
                for (var z = 0; z < _parentMatrix[y].Count; z++)
                {
                    var liveNeighbors = Neighbors(x, y);
                    var self = _parentMatrix[x][y][z];

                    _childMatrix[x][y][z] = OperateChild(self, liveNeighbors);
                }
            }
        }
    }

    private int Neighbors(int posX, int posY, int posZ = 0)
    {
        var neighborsCount = 0;

        for (int dtX = -1; dtX < 2; dtX++)
        {
            for (int dtY = -1; dtY < 2; dtY++)
            {
                for (int dtZ = -1; dtZ < 2; dtZ++)
                {
                    var X = 0;
                    var Y = 0;
                    var Z = 0;

                    if (dtX == 0 && dtY == 0 && dtZ == 0)
                    {
                        continue; // Skip self
                    }

                    if (!_corners)
                    {
                        if (dtX == -1 && dtY == -1 && dtZ == -1||
                            dtX == 1 && dtY == 1 && dtZ == 1||
                            dtX == 1 && dtY == 1 && dtZ == -1||
                            dtX == 1 && dtY == -1 && dtZ == 1||
                            dtX == -1 && dtY == 1 && dtZ == 1||
                            dtX == 1 && dtY == -1 && dtZ == -1||
                            dtX == -1 && dtY == 1 && dtZ == -1||
                            dtX == -1 && dtY == -1 && dtZ == 1||
                            dtX == -1 && dtY == -1 && dtZ == -1)
                        {
                            continue; // Skip self
                        }
                    }

                    //SET X
                    if (posX == 0 && dtX == -1)
                    {
                        if (_wrap)
                        {
                            X = _parentMatrix.Count - 1;
                        }
                        else
                        {
                            continue; // Skip self
                        }
                    }
                    else if (posX == _parentMatrix.Count - 1 && dtX == +1)
                    {
                        if (_wrap)
                        {
                            X = 0;
                        }
                        else
                        {
                            continue; // Skip self
                        }
                    }
                    else
                    {
                        X = posX + dtX;
                    }

                    //SET Y
                    if (posY == 0 && dtY == -1)
                    {
                        if (_wrap)
                        {
                            Y = _parentMatrix[0].Count - 1;
                        }
                        else
                        {
                            continue; // Skip self
                        }
                    }
                    else if (posY == _parentMatrix[0].Count - 1 && dtY == +1)
                    {
                        if (_wrap)
                        {
                            Y = 0;
                        }
                        else
                        {
                            continue; // Skip self
                        }
                    }
                    else
                    {
                        Y = posY + dtY;
                    }
                    
                    //SET Z
                    if (posZ == 0 && dtZ == -1)
                    {
                        if (_wrap)
                        {
                            Z = _parentMatrix[0].Count - 1;
                        }
                        else
                        {
                            continue; // Skip self
                        }
                    }
                    else if (posZ == _parentMatrix[0].Count - 1 && dtZ == +1)
                    {
                        if (_wrap)
                        {
                            Z = 0;
                        }
                        else
                        {
                            continue; // Skip self
                        }
                    }
                    else
                    {
                        Z = posZ + dtZ;
                    } 

                    if (_parentMatrix[X][Y][Z] > 0)
                    {
                        neighborsCount++;
                    }
                }
            }
        }

        return neighborsCount;
    }

    private int OperateChild(int self, int neighbors)
    {
        if(self == 0)
        {
            if (_birthVals.Any(t => t == neighbors))
            {
                return _state;
            }
        }
        else if(self == _state)
        {
            if (_liveVals.Any(t => t == neighbors))
            {
                return _state;
            }

            if (_deadVals.Any(t => t == neighbors))
            {
                return self - 1;
            }
        }
        else if(self > 0)
        {
            return self - 1;
        }

        return 0;
    }

    private void MakeFirstGeneration()
    {
        SetUpMatrixs();
        GenerateCells();
        SetUpCells();
    }

    private void SetUpMatrixs()
    {
       for (var i = 0; i < _matixSize; i++)
       {
            var row2 = new List<List<int>>();
            
            for (var j = 0; j < _matixSize; j++)
            {
                var row = new List<int>();
                for (var k = 0; k < _matixSize; k++)
                {
                    var state = Random.value > 0.5f ? _state : 0;
                    row.Add(state);
                }
                row2.Add(row);
            }
            _parentMatrix.Add(row2);
       } 
       _childMatrix = _parentMatrix;
    }

    private void GenerateCells()
    {
        for (var x = 0; x < _matixSize; x++)
        {
            var row2 = new List<List<Cell>>();
            for (var y = 0; y < _matixSize; y++)
            {
                var row = new List<Cell>();
                
                for (var z = 0; z < _matixSize; z++)
                {
                    GameObject curCell = Instantiate(_preCell, this.gameObject.transform, true);
                    curCell.transform.position =
                        new Vector3(((_size * x + _spacing * x) - (_size * _matixSize + _spacing * _matixSize) / 2f),
                            ((_size * y + _spacing * y) - (_size * _matixSize + _spacing * _matixSize) / 2f),
                            (_size * z + _spacing * z) - (_size * _matixSize + _spacing * _matixSize) / 2f);
                    
                    curCell.SetActive(true);

                    Cell curCellScript = curCell.GetComponent<Cell>();
                    curCellScript.SetState(_parentMatrix[x][y][z]);
                    curCellScript._maxState = _state;

                    row.Add(curCellScript);
                }
                
                row2.Add(row);
            }
            _cells.Add(row2);
        } 
    }

    private void SetUpCells()
    {
        for (var x = 0; x < _matixSize; x++)
        {
            for (var y = 0; y < _matixSize; y++)
            {
                for (var z = 0; z < _matixSize; z++)
                {
                    _cells[x][y][z].SetState(_childMatrix[x][y][z]);
                }
            }
        } 
    }

    private void AddGeneration()
    {
        if (!_genTMP) return;
        
        _generationN++;
        _genTMP.text = "Generation: " + _generationN;
    }

    private void ResetGeneration()
    {
        if (!_genTMP) return;
        
        _generationN = 0;
        _genTMP.text = "Generation: " + _generationN;
    }

    public void SwitchRun()
    {
        if (!_running)
        {
           //Set Up 
        }

        _running = !_running;
        
        _startStop.GetComponentInChildren<TextMeshProUGUI>().text = "Running: " + _running.ToString();
    }

    private void SetRun(bool state)
    {
        _running = state;
        _startStop.GetComponentInChildren<TextMeshProUGUI>().text = "Running: " + _running.ToString();
    }
    
    //Wait
    public void SetWait()
    {
        if (!_inputWait) return;

        _wait = float.Parse(_inputWait.text);
        _inputWait.text = "Wait: " + _wait.ToString("F1");
    }

    public void SelectWait()
    {
        if (!_inputWait) return;

        _inputWait.text = _wait.ToString("F1");
    }
    
    public void DeselectWait()
    {
        if (!_inputWait) return;

        _inputWait.text = "Wait: " + _wait.ToString("F1");
    } 
    
    //State
    private void SetStateText()
    {
        if (!_inputState || _running) return;
        
        _inputState.text = "State: " + _state.ToString("N0");
    }
    
    public void SetState()
    {
        if (!_inputState || _running) return;

        _state = int.Parse(_inputState.text);
        _inputState.text = "State: " + _state.ToString("N0");

        for (var x = 0; x < _matixSize; x++)
        {
            for (var y = 0; y < _matixSize; y++)
            {
                for (var z = 0; z < _matixSize; z++)
                {
                    _cells[x][y][z]._maxState = _state;
                }
            }
        }
    }

    public void SelectState()
    {
        if (!_inputState || _running) return;

        _inputState.text = _state.ToString("N0");
    }


    public void DeselectState()
    {
        if (!_inputState) return;
        
        _inputState.text = "State: " + _state.ToString("N0");
    }
}
