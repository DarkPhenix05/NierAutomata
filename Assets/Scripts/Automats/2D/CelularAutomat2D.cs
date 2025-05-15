using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class CelularAutomat2D : MonoBehaviour
{
    public static CelularAutomat2D Instance;

    [Header("PARAMETERS")]
    public GameObject _preCell;

    public int _numberOfCells;
    private float _size;
    public float _spacing;

    [Header("LISTS")]
    public List<List<Cell>> _cells = new List<List<Cell>>();
    public List<GameObject> _availableCells = new List<GameObject>();

    public List<List<int>> _parentMatrix = new List<List<int>>();
    public List<List<int>> _childMatrix = new List<List<int>>();

    private bool corners;
    private bool wrap;

    [Header("BUTTONS")]

    public Button _startStop;
    private bool _running = false;

    [Header("INPUT-FIELD")]
    public TMP_InputField _inputWait;
    private float _wait = 0.1f;
    private float _timer;

    public TMP_InputField _inputState;
    private int _state;

    public TMP_InputField _inputLive;
    private List<int> _liveVals;

    public TMP_InputField _inputDead;
    private List<int> _deadVals;

    public TMP_InputField _inputBirth;
    private List<int> _birthVals;


    [Header("GENERATION-FIELD")]
    public TextMeshProUGUI _genTMP;
    public int _generationN = 0;

    [Header("SFX")]
    private AudioSource _audioSource;
    public List<AudioClip> _SFX;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Start()
    {
        _inputWait.text = "Wait: " + _wait.ToString("F1");

        _size = _preCell.transform.localScale.x;
    }

    public void Update()
    {
        if (_running)
        {
            _timer += Time.deltaTime;
            if (_timer >= _wait)
            {

                _timer = 0.0f;
            }
        }
    }

    public int Neighbors(int posX, int posY)
    {
        int neighborsCount = 0;

        for (int x = -1; x <= 2; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int tempX = x;
                int tempY = y;

                if (tempX == 0 && tempY == 0)
                {
                    continue; // Skip self
                }

                //SET X
                if (posX == 0 && x == -1)
                {
                    if(wrap)
                    {
                        tempX = _parentMatrix.Count - 1;
                    }
                    else
                    {
                        continue; // Skip self
                    }
                }
                else if(posX == _parentMatrix.Count && x == +1)
                {
                    if (wrap)
                    {
                        tempX = 0;
                    }
                    else
                    {
                        continue; // Skip self
                    }
                }
                else
                {
                    tempX += posX;
                }

                //SET Y
                if(posY == 0 && y == -1)
                {
                    if (wrap)
                    {
                        tempY = _parentMatrix[0].Count - 1;
                    }
                    else
                    {
                        continue; // Skip self
                    }
                }
                else if (posY == _parentMatrix[0].Count && y == +1)
                {
                    if (wrap)
                    {
                        tempX = 0;
                    }
                    else
                    {
                        continue; // Skip self
                    }
                }
                else
                {
                    tempY += posY;
                }
                
                if (_parentMatrix[posX + x][posY + y] > 0)
                {
                    neighborsCount++;
                }
            }
        }

        return neighborsCount;
    }

    public void SetChildren()
    {
        for (int x = 0; x < _parentMatrix.Count;  x++)
        {
            for(int y = 0; y< _parentMatrix[x].Count; y++)
            {
                int liveNeighbors = Neighbors(x, y);
                int self = _parentMatrix[x][y];
                
                _childMatrix[x][y] =  OperateChild(self, liveNeighbors);
            }
        }
    }

    public int OperateChild(int self, int neighbors)
    {
        if(self == 0)
        {
            for(int i = 0; i < _birthVals.Count; i++)
            {
                if (_birthVals[i] == neighbors)
                {
                    return _state;
                }
            }
        }
        else if(self == _state)
        {
            for (int i = 0; i < _liveVals.Count; i++)
            {
                if (_liveVals[i] == neighbors)
                {
                    return _state;
                }
            }
            for (int i = 0; i < _deadVals.Count; i++)
            {
                if (_deadVals[i] == neighbors)
                {
                    return _state - 1;
                }
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

    }

    private void SetUpMatrix()
    {
        for (int x = 0; x < _size; x++)
        {
            for (int y = 0; y < _size; y++)
            {
                bool state;

                state = (Random.value > 0.5f);
            }
        }
    }

    public void SetState(ButtonScript button)
    {

    }
}