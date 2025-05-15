using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOfLife : MonoBehaviour
{
    public static GameOfLife Instance;

    private Vector2 _matixSize;

    public GameObject _cellPre;
    private Transform _cellHolder;
    private float _size;
    public float _spacing;

    public List<List<Cell>> _cells = new List<List<Cell>>();
    public List<GameObject> _availableCells = new List<GameObject>();

    public List<List<bool>> _parentMatrix = new List<List<bool>>();
    public List<List<bool>> _childMatrix = new List<List<bool>>();

    [Header("INPUT-FIELDS")]
    public TMP_InputField _inputWait;
    private float _wait = 0.1f;
    private float _timer;

    public TMP_InputField _inputSizeX;
    public TMP_InputField _inputSizeY;
    private Vector2 _tempSize = new Vector2(10,10);


    [Header("GENERATION-FIELD")]
    public TextMeshProUGUI _genTMP;
    public int _generationN = 0;

    public Button _startStop;
    private bool _running = false;

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

    void Start()
    {
        _cellHolder = this.gameObject.transform;
        _size = _cellPre.transform.localScale.x;

        SetUpMatrixs();
        SetGenerationText();
        SetWaitText();
        SetSiceText();
    }
    public void Update()
    {
        if (_running && _parentMatrix.Count > 0)
        {
            _timer += Time.deltaTime;
            if (_timer >= _wait)
            {
                OperateGeneration();
                AddGeneration();
                _timer = 0.0f;
            }
        }
    }

    /*
        Any live cell with fewer than two live neighbours dies, as if by underpopulation.
        Any live cell with two or three live neighbours lives on to the next generation.
        Any live cell with more than three live neighbours dies, as if by overpopulation.
        Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
     */

    public void OperateGeneration()
    {
        // Ensure child matrix is fresh
        _childMatrix = new List<List<bool>>();

        for (int x = 0; x < _parentMatrix.Count; x++)
        {
            var newRow = new List<bool>();
            for (int y = 0; y < _parentMatrix[x].Count; y++)
            {
                int liveNeighbors = CountLiveNeighbors(x, y);
                bool isAlive = _parentMatrix[x][y];

                bool newState = false;

                if (isAlive && (liveNeighbors == 2 || liveNeighbors == 3))
                    newState = true;
                else if (!isAlive && liveNeighbors == 3)
                    newState = true;

                newRow.Add(newState);

                // Update cell visuals
                _cells[x][y].SetState(newState);
            }
            _childMatrix.Add(newRow);
        }

        // Swap matrices
        _parentMatrix = _childMatrix;
    }

    private int CountLiveNeighbors(int x, int y)
    {
        int count = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue; // Skip self

                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && ny >= 0 && nx < _parentMatrix.Count && ny < _parentMatrix[0].Count)
                {
                    if (_parentMatrix[nx][ny])
                        count++;
                }
            }
        }

        return count;
    }

    public void SetUpMatrixs()
    {
        SetGenerationText();

        for (int i = 0; i < _availableCells.Count; i++)
        {
            _availableCells[i].SetActive(false);
        }

        _cells.Clear();

        _parentMatrix.Clear();
        _childMatrix.Clear();

        _matixSize = _tempSize;

        for (int i = 0; i < _matixSize.x; i++)
        {
            var row = new List<bool>();
            var rowC = new List<Cell>();

            for (int j = 0; j < _matixSize.y; j++)
            {
                bool state;
                
                state = (Random.value > 0.5f);
                row.Add(state);
                rowC.Add(SetCell(i, j, state));
                
                
            }

            _parentMatrix.Add(row);
            _cells.Add(rowC);
        }

        _childMatrix = _parentMatrix;
    }

    public void UpdateLists(bool rand)
    {
        _parentMatrix.Clear();
        _childMatrix.Clear();

        for (int i = 0; i < _matixSize.x; i++)
        {
            var row = new List<bool>();

            for (int j = 0; j < _matixSize.y; j++)
            {
                row.Add(_cells[i][j].GetState());
            }

            _parentMatrix.Add(row);
        }

        _childMatrix = _parentMatrix;
    }

    private Cell SetCell(int x, int y, bool state)
    {
        GameObject newGO = _availableCells[GetInactive()];
        newGO.transform.parent = _cellHolder;
        newGO.transform.position = 
            new Vector3(((_size * x + _spacing * x) - (_size * _matixSize.x + _spacing * _matixSize.x) / 2f),
                        ((_size * y + _spacing * y) - (_size * _matixSize.y + _spacing * _matixSize.y) / 2f), 
                        0);
        newGO.SetActive(true);

        Cell newCell = newGO.GetComponent<Cell>();
        newCell.SetState(state);

        return newCell;
    }

    public int GetInactive()
    {
        for (int i = 0; i < _availableCells.Count; i++)
        {
            if (!_availableCells[i].activeInHierarchy)
            {
                return i;
            }
        }

        SpawnNewCell();

        return GetInactive();
    }

    public void SpawnNewCell()
    {
        GameObject curCell = Instantiate(_cellPre);
        curCell.SetActive(false);
        curCell.transform.SetParent(this.gameObject.transform);

        _availableCells.Add(curCell);
    }

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

    public void SetX()
    {
        if (!_inputSizeX) return;

        _tempSize.x = float.Parse(_inputSizeX.text);
        _inputSizeX.text = "X: " + _tempSize.x.ToString("n0");
    }
    public void SelectX()
    {
        if (!_inputSizeX) return;

        _inputSizeX.text = _tempSize.x.ToString("n0");
    }
    public void DeselecX()
    {
        if (!_inputSizeX) return;

        _inputSizeX.text = "X: " + _tempSize.x.ToString("n0");
    }

    public void SetY()
    {
        if (!_inputSizeY) return;

        _tempSize.y = float.Parse(_inputSizeY.text);
        _inputSizeY.text = "Y: " + _tempSize.y.ToString("n0");
    }
    public void SelectY()
    {
        if (!_inputSizeY) return;

        _inputSizeY.text = _tempSize.y.ToString("n0");
    }
    public void DeselecY()
    {
        if (!_inputSizeY) return;

        _inputSizeY.text = "Y: " + _tempSize.y.ToString("n0");
    }

    public bool GetRunning()
    {
        return _running;
    }

    public void SwitchRun()
    {
        if (!_running)
        {
            UpdateLists(false);
            SetAudioSource();
        }

        _running = !_running;
        
        _startStop.GetComponentInChildren<TextMeshProUGUI>().text = "Running: " + _running.ToString();
    }
    public void SetRun(bool state)
    {
        _running = state;
        _startStop.GetComponentInChildren<TextMeshProUGUI>().text = "Running: " + _running.ToString();
    }

    public void SetWaitText()
    {
        _inputWait.text = "Wait: " + _wait.ToString("F2");
    }

    public void SetSiceText()
    {
        if (!_inputSizeX || !_inputSizeY) return;
        
        _inputSizeX.text = "X: " + _tempSize.x.ToString("n0");
        _inputSizeY.text = "Y: " + _tempSize.y.ToString("n0");
    }

    public void AddGeneration()
    {
        if (!_genTMP) return;

        _generationN++;
        _genTMP.text = "Number of Generations: " + _generationN.ToString();
        PlaySFX();
    }
    public void SetGenerationText(int val = 0)
    {
        if (!_genTMP) return;

        _generationN = val;
        _genTMP.text = "Number of Generations: " + _generationN.ToString();
    }

    public void ResetGenerationCount()
    {
        if (!_genTMP) return;

        _generationN = 0;
        _genTMP.text = "Number of Generations: " + _generationN.ToString();
    }

    private void SetAudioSource()
    {
        if (_SFX.Count == 0) return;

        _audioSource = this.gameObject.GetComponent<AudioSource>();
        _audioSource.volume = 1.0f;
        _audioSource.bypassListenerEffects = true;
        _audioSource.bypassEffects = true;
        _audioSource.bypassReverbZones = true;
        _audioSource.clip = _SFX[Random.Range(0, _SFX.Count)];
    }
    private void PlaySFX()
    {
        if (_SFX.Count == 0) return;

        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.Play();
    }
}
