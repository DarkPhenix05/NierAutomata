using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CelularAutomat2D : MonoBehaviour
{
    public static CelularAutomat2D Instance;

    public Vector2 _matixSize;
    public GameObject _cellPre;
    private Transform _cellHolder;
    private float _size;
    public float _spacing;

    public List<List<Cell>> _cells = new List<List<Cell>>();

    public List<List<bool>> _parentMatrix = new List<List<bool>>();
    public List<List<bool>> _childMatrix = new List<List<bool>>();

    [Header("INPUT-FIELD")]
    public TMP_InputField _inputField;
    private float _wait = 0.1f;
    private float _timer;

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

        SetUpMatrixs(_matixSize);
        SetGenerationText();

        _inputField.text = "Wait: " + _wait.ToString("F1");
    }
    public void Update()
    {
        if (_running)
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

    public void SetUpMatrixs(Vector2 size)
    {
        for (int i = 0; i < size.x; i++)
        {
            var row = new List<bool>();
            var rowC = new List<Cell>();

            for (int j = 0; j < size.y; j++)
            {
                bool state = (Random.value > 0.5f);
                row.Add(state);
                rowC.Add(SpawnCell(i, j, state));
            }

            _parentMatrix.Add(row);
            _cells.Add(rowC);
        }

        _childMatrix = _parentMatrix;
    }

    private Cell SpawnCell(int x, int y, bool state)
    {
        GameObject newGO = Instantiate(_cellPre);
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

    public void SetWait()
    {
        if (!_inputField) return;

        _wait = float.Parse(_inputField.text);
        _inputField.text = "Wait: " + _wait.ToString("F1");
    }

    public void SelectIF()
    {
        if (!_inputField) return;

        _inputField.text = _wait.ToString("F1");
    }

    public void DeselectIF()
    {
        if (!_inputField) return;

        _inputField.text = "Wait: " + _wait.ToString("F1");
    }

    public bool GetRunning()
    {
        return _running;
    }

    public void SwitchRun()
    {
        _running = !_running;
        if (_running)
        {
            SetAudioSource();
        }
        
        _startStop.GetComponentInChildren<TextMeshProUGUI>().text = "Running: " + _running.ToString();
    }
    public void SetRun(bool state)
    {
        _running = state;
        _startStop.GetComponentInChildren<TextMeshProUGUI>().text = "Running: " + _running.ToString();
    }

    public void AddGeneration()
    {
        if (!_genTMP) return;

        _generationN++;
        _genTMP.text = "Number of Generations: " + _generationN.ToString();
        PlaySFX();
    }
    public void SetGenerationText()
    {
        if (!_genTMP) return;

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
        _audioSource.clip = _SFX[Random.Range(0, _SFX.Count -1)];
    }
    private void PlaySFX()
    {
        if (_SFX.Count == 0) return;

        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.Play();
    }

}
