using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class CelularAutomat1D : MonoBehaviour
{
    public static CelularAutomat1D Instance;

    [Header("PARAMETERS")]
    public GameObject _preCell;

    public int _numberOfCells;
    private float _size;
    public float _spacing;

    [Header("LISTS")]
    private List<GameObject> _availableCells = new List<GameObject>();

    private List<GameObject> _currentCells = new List<GameObject>();
    private List<bool> _parentCells = new List<bool>();

    private List<bool> _childCells = new List<bool>();

    private bool _stateL;
    private bool _stateS;
    private bool _stateR;

    [Header("BUTTONS")]
    //CERO vivos
    public Button _B000;
    private bool _S000;

    //UN vivo
    public Button _B001;
    private bool _S001;

    public Button _B010;
    private bool _S010;

    public Button _B100;
    private bool _S100;

    //DOS vivos
    public Button _B011;
    private bool _S011;

    public Button _B101;
    private bool _S101;

    public Button _B110;
    private bool _S110;

    //TRES vivos
    public Button _B111;
    private bool _S111;

    public Button _startStop;
    private bool _running = false;

    [Header("INPUT-FIELD")]
    public TMP_InputField _inputField;
    private float _wait = 0.1f;
    private float _timer;

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
        Set000(Random.value < 0.5f);

        Set001(Random.value < 0.5f);
        Set010(Random.value < 0.5f);
        Set100(Random.value < 0.5f);
        
        Set011(Random.value < 0.5f);
        Set101(Random.value < 0.5f);
        Set110(Random.value < 0.5f);

        Set111(Random.value < 0.5f);

        SetRun(false);

        _inputField.text = "Wait: " + _wait.ToString("F1");

        _size = _preCell.transform.localScale.x;

        GenerateFirstGeneration();
        SetGenerationText();
    }

    public void Update()
    {
        if (_running)
        {
            _timer += Time.deltaTime;
            if (_timer >= _wait)
            {
                OperateGeneration();
                _timer = 0.0f;
            }
        }
    }

    public void OperateGeneration()
    {
        MoveActiveCellsUp();

        UpdateParentCells();
        UpdateChildCells();

        for (int i = 0; i < _numberOfCells; i++)
        {
            GameObject cell = _availableCells[GetInactive()];

            _currentCells[i] = cell;

            cell.transform.position =
                new Vector3(((_size * i + _spacing * i) - (_size * _numberOfCells + _spacing * _numberOfCells) / 2f),
                    0f, 0);
            cell.SetActive(true);
            cell.GetComponent<Cell>().SetState(_childCells[i]);
        }

        AddGeneration();

    }

    public void UpdateParentCells()
    {
        _parentCells.Clear();
        for(int i = 0; i < _numberOfCells; i++)
        {
            _parentCells.Add(_currentCells[i].GetComponent<Cell>().GetState());
        }
    }

    public void UpdateChildCells()
    {
        _childCells.Clear();
        for (int i = 0; i < _numberOfCells; i++)
        {
            GetNeighbors(i);
            _childCells.Add(GenerateChild());
        }
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
        GameObject curCell = Instantiate(_preCell);
        curCell.SetActive(false);
        curCell.transform.SetParent(this.gameObject.transform);

        _availableCells.Add(curCell);
    }

    public void MoveActiveCellsUp()
    {
        for (int i = 0; i < _availableCells.Count; i++)
        {
            if (_availableCells[i].activeInHierarchy)
            {
                _availableCells[i].transform.localPosition += new Vector3(0f, _size + _spacing, 0f);
            }
        }
    }

    public void MoveUp(float step)
    {
        transform.localPosition += new Vector3(0f, step, 0f);
    }
    
    public void GetNeighbors(int val)
    {
        // Left neighbor
        if (val != 0)
        {
            _stateL = _parentCells[val - 1];
        }
        else
        {
            _stateL = _parentCells[_parentCells.Count - 1];
        }

        // Self
        _stateS = _parentCells[val];

        // Right neighbor
        if (val != _parentCells.Count - 1)
        {
            _stateR = _parentCells[val + 1];
        }
        else
        {
            _stateR = _parentCells[0];
        }

        /*Debug.Log(val);
        Debug.Log("  L-" + val + ": " + _stateL);
        Debug.Log("  S-" + val + ": " + _stateS);
        Debug.Log("  R-" + val + ": " + _stateR);*/
    }

    public bool GenerateChild()
    {
        //ZERO vivos
        if(!_stateL && !_stateS && !_stateR)
        {
            return _S000;
        }

        //UN VIVO
        else if(!_stateL && !_stateS && _stateR)
        {
            return _S001;
        }
        else if (!_stateL && _stateS && !_stateR)
        {
            return _S010;
        }
        else if (_stateL && !_stateS && !_stateR)
        {
            return _S100;
        }

        //DOS vivos
        else if (!_stateL && _stateS && _stateR)
        {
            return _S011;
        }
        else if (_stateL && !_stateS && _stateR)
        {
            return _S101;
        }
        else if (_stateL && _stateS && !_stateR)
        {
            return _S110;
        }

        //TRES vivos
        else if (_stateL && _stateS && _stateR)
        {
            return _S111;
        }


        //DEBUG
        else
        {
            Debug.LogError("ERROR-GeneratingChild");
            return false;
        }
    }
    public void GenerateFirstGeneration()
    {
        SetGenerationText();

        for (int i = 0; i < _numberOfCells; i++)
        {
            GameObject curCell = Instantiate(_preCell);
            curCell.transform.SetParent(this.gameObject.transform);
            curCell.transform.position =
                new Vector3(((_size * i + _spacing * i) - (_size * _numberOfCells + _spacing * _numberOfCells) / 2f),
                    0f, 0);

            curCell.GetComponent<Cell>().SetState(Random.value < 0.5f);

            _availableCells.Add(curCell);
            _currentCells.Add(curCell);

            curCell.SetActive(true);
        }
    }


//BUTTONS
    //ZERO vivos
    public void Set000()
    {
        _S000 = !_S000;
        _B000.GetComponentInChildren<TextMeshProUGUI>().text = _S000.ToString();
    }

    //UN vivo
    public void Set001()
    {
        _S001 = !_S001;
        _B001.GetComponentInChildren<TextMeshProUGUI>().text = _S001.ToString();
    }
    public void Set010()
    {
        _S010 = !_S010;
        _B010.GetComponentInChildren<TextMeshProUGUI>().text = _S010.ToString();
    }
    public void Set100()
    {
        _S100 = !_S100;
        _B100.GetComponentInChildren<TextMeshProUGUI>().text = _S100.ToString();
    }

    //DOS vivos
    public void Set011()
    {
        _S011 = !_S011;
        _B011.GetComponentInChildren<TextMeshProUGUI>().text = _S011.ToString();
    }
    public void Set101()
    {
        _S101 = !_S101;
        _B101.GetComponentInChildren<TextMeshProUGUI>().text = _S101.ToString();
    }
    public void Set110()
    {
        _S110 = !_S110;
        _B110.GetComponentInChildren<TextMeshProUGUI>().text = _S110.ToString();
    }

    //TRES vivos
    public void Set111()
    {
        _S111 = !_S111;
        _B111.GetComponentInChildren<TextMeshProUGUI>().text = _S111.ToString();
    }

//Manual
    //CERO vivos
    public void Set000(bool val)
    {
        _S000 = val;
        _B000.GetComponentInChildren<TextMeshProUGUI>().text = _S000.ToString();
    }

    //UN vivo
    public void Set001(bool val)
    {
        _S001 = val;
        _B001.GetComponentInChildren<TextMeshProUGUI>().text = _S001.ToString();
    }
    public void Set010(bool val)
    {
        _S010 = val;
        _B010.GetComponentInChildren<TextMeshProUGUI>().text = _S010.ToString();
    }
    public void Set100(bool val)
    {
        _S100 = val;
        _B100.GetComponentInChildren<TextMeshProUGUI>().text = _S100.ToString();
    }

    //DOS vivos
    public void Set011(bool val)
    {
        _S011 = val;
        _B011.GetComponentInChildren<TextMeshProUGUI>().text = _S011.ToString();
    }
    public void Set101(bool val)
    {
        _S101 = val;
        _B101.GetComponentInChildren<TextMeshProUGUI>().text = _S101.ToString();
    }
    public void Set110(bool val)
    {
        _S110 = val;
        _B110.GetComponentInChildren<TextMeshProUGUI>().text = _S110.ToString();
    }

    //TRES vivos
    public void Set111(bool val)
    {
        _S111 = val;
        _B111.GetComponentInChildren<TextMeshProUGUI>().text = _S111.ToString();
    }

    //EXTRAS
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

    public bool GetRunning()
    {
        return _running;
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

    public void AddGeneration()
    {
        if(!_genTMP) return;

        _generationN++;
        _genTMP.text = "Number of Generations: " + _generationN.ToString();
        PlaySFX();
    }
    public void SetGenerationText(int val = 0)
    {
        if(!_genTMP) return;

        _generationN = val;
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
