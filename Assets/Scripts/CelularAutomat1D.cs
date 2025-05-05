using UnityEngine;
using System.Collections.Generic;
using System;
using System.Drawing;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;
using System.Collections;

public class CelularAutomat1D : MonoBehaviour
{
    public static CelularAutomat1D Instance;

    [Header("PARAMETERS")]
    public GameObject preCell;

    private float StarterPos;

    public int NumberOfCells;
    private float size;
    public float Spacing;

    [Header("LISTS")]
    private List<GameObject> AveliableCells = new List<GameObject>();

    private List<GameObject> CurrentCells = new List<GameObject>();
    private List<bool> ParentCells = new List<bool>();

    private List<bool> ChildCells = new List<bool>();

    private bool stateL;
    private bool stateS;
    private bool stateR;

    [Header("BUTTONS")]
    //CERO vivos
    public Button B000;
    private bool S000;

    //UN vivo
    public Button B001;
    private bool S001;

    public Button B010;
    private bool S010;

    public Button B100;
    private bool S100;

    //DOS vivos
    public Button B011;
    private bool S011;

    public Button B101;
    private bool S101;

    public Button B110;
    private bool S110;

    //TRES vivos
    public Button B111;
    private bool S111;

    public Button StartStop;
    private bool Running = false;

    [Header("INPUT-FIELD")]
    public TMP_InputField InpputField;
    private float wait = 0.1f;
    private float timer;

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

        InpputField.text = "Wait: " + wait.ToString("F1");

        size = preCell.transform.localScale.x;

        GenerateFirstGeneration();
    }

    public void Update()
    {
        if (Running)
        {
            timer += Time.deltaTime;
            if (timer >= wait)
            {
                OperateGeneration();
                timer = 0.0f;
            }
        }
    }

    public void OperateGeneration()
    {
        StarterPos = this.gameObject.transform.position.y;

        MoveActiveCellsUp();

        UpdateParentCells();
        UpdateChildCells();

        for (int i = 0; i < NumberOfCells; i++)
        {
            GameObject cell = AveliableCells[GetInactive()];

            CurrentCells[i] = cell;

            cell.transform.position =
                new Vector3(((size * i + Spacing * i) - (size * NumberOfCells + Spacing * NumberOfCells) / 2f),
                    -3.0f, 0);
            cell.SetActive(true);
            cell.GetComponent<Cell>().SetState(ChildCells[i]);
        }

    }

    public void UpdateParentCells()
    {
        ParentCells.Clear();
        for(int i = 0; i < NumberOfCells; i++)
        {
            ParentCells.Add(CurrentCells[i].GetComponent<Cell>().GetState());
        }
    }

    public void UpdateChildCells()
    {
        ChildCells.Clear();
        for (int i = 0; i < NumberOfCells; i++)
        {
            GetNeighbors(i);
            ChildCells.Add(GenerateChild());
        }
    }

    public int GetInactive()
    {
        for (int i = 0; i < AveliableCells.Count; i++)
        {
            if (!AveliableCells[i].activeInHierarchy)
            {
                return i;
            }
        }

        SpawnNewCell();

        return GetInactive();
    }

    public void SpawnNewCell()
    {
        GameObject curCell = Instantiate(preCell);
        curCell.SetActive(false);
        curCell.transform.SetParent(this.gameObject.transform);

        AveliableCells.Add(curCell);
    }

    public void MoveActiveCellsUp()
    {
        for (int i = 0; i < AveliableCells.Count; i++)
        {
            if (AveliableCells[i].activeInHierarchy)
            {
                AveliableCells[i].transform.localPosition += new Vector3(0f, size + Spacing, 0f);
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
            stateL = ParentCells[val - 1];
        }
        else
        {
            stateL = ParentCells[ParentCells.Count - 1];
        }

        // Self
        stateS = ParentCells[val];

        // Right neighbor
        if (val != ParentCells.Count - 1)
        {
            stateR = ParentCells[val + 1];
        }
        else
        {
            stateR = ParentCells[0];
        }

        Debug.Log(val);
        Debug.Log("  L-" + val + ": " + stateL);
        Debug.Log("  S-" + val + ": " + stateS);
        Debug.Log("  R-" + val + ": " + stateR);
    }

    public bool GenerateChild()
    {
        //ZERO vivos
        if(!stateL && !stateS && !stateR)
        {
            return S000;
        }

        //UN VIVO
        else if(!stateL && !stateS && stateR)
        {
            return S001;
        }
        else if (!stateL && stateS && !stateR)
        {
            return S010;
        }
        else if (stateL && !stateS && !stateR)
        {
            return S100;
        }

        //DOS vivos
        else if (!stateL && stateS && stateR)
        {
            return S011;
        }
        else if (stateL && !stateS && stateR)
        {
            return S101;
        }
        else if (stateL && stateS && !stateR)
        {
            return S110;
        }

        //TRES vivos
        else if (stateL && stateS && stateR)
        {
            return S111;
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
        for (int i = 0; i < NumberOfCells; i++)
        {
            GameObject curCell = Instantiate(preCell);
            curCell.transform.SetParent(this.gameObject.transform);
            curCell.transform.position =
                new Vector3(((size * i + Spacing * i) - (size * NumberOfCells + Spacing * NumberOfCells) / 2f),
                    -3.0f, 0);

            curCell.GetComponent<Cell>().SetState(Random.value < 0.5f);

            AveliableCells.Add(curCell);
            CurrentCells.Add(curCell);

            curCell.SetActive(true);
        }
    }


//BUTTONS
    //ZERO vivos
    public void Set000()
    {
        S000 = !S000;
        B000.GetComponentInChildren<TextMeshProUGUI>().text = S000.ToString();
    }

    //UN vivo
    public void Set001()
    {
        S001 = !S001;
        B001.GetComponentInChildren<TextMeshProUGUI>().text = S001.ToString();
    }
    public void Set010()
    {
        S010 = !S010;
        B010.GetComponentInChildren<TextMeshProUGUI>().text = S010.ToString();
    }
    public void Set100()
    {
        S100 = !S100;
        B100.GetComponentInChildren<TextMeshProUGUI>().text = S100.ToString();
    }

    //DOS vivos
    public void Set011()
    {
        S011 = !S011;
        B011.GetComponentInChildren<TextMeshProUGUI>().text = S011.ToString();
    }
    public void Set101()
    {
        S101 = !S101;
        B101.GetComponentInChildren<TextMeshProUGUI>().text = S101.ToString();
    }
    public void Set110()
    {
        S110 = !S110;
        B110.GetComponentInChildren<TextMeshProUGUI>().text = S110.ToString();
    }

    //TRES vivos
    public void Set111()
    {
        S111 = !S111;
        B111.GetComponentInChildren<TextMeshProUGUI>().text = S111.ToString();
    }

//Manual
    //CERO vivos
    public void Set000(bool val)
    {
        S000 = val;
        B000.GetComponentInChildren<TextMeshProUGUI>().text = S000.ToString();
    }

    //UN vivo
    public void Set001(bool val)
    {
        S001 = val;
        B001.GetComponentInChildren<TextMeshProUGUI>().text = S001.ToString();
    }
    public void Set010(bool val)
    {
        S010 = val;
        B010.GetComponentInChildren<TextMeshProUGUI>().text = S010.ToString();
    }
    public void Set100(bool val)
    {
        S100 = val;
        B100.GetComponentInChildren<TextMeshProUGUI>().text = S100.ToString();
    }

    //DOS vivos
    public void Set011(bool val)
    {
        S011 = val;
        B011.GetComponentInChildren<TextMeshProUGUI>().text = S011.ToString();
    }
    public void Set101(bool val)
    {
        S101 = val;
        B101.GetComponentInChildren<TextMeshProUGUI>().text = S101.ToString();
    }
    public void Set110(bool val)
    {
        S110 = val;
        B110.GetComponentInChildren<TextMeshProUGUI>().text = S110.ToString();
    }

    //TRES vivos
    public void Set111(bool val)
    {
        S111 = val;
        B111.GetComponentInChildren<TextMeshProUGUI>().text = S111.ToString();
    }

    //EXTRAS
    public void SwitchRun()
    {
        Running = !Running;
        StartStop.GetComponentInChildren<TextMeshProUGUI>().text = "Running: " + Running.ToString();
    }

    public void SetRun(bool state)
    {
        Running = state;
        StartStop.GetComponentInChildren<TextMeshProUGUI>().text = "Running: " + Running.ToString();
    }

    public bool GetRunning()
    {
        return Running;
    }

    public void SetWait()
    {
        wait = float.Parse(InpputField.text);
        InpputField.text = "Wait: " + wait.ToString("F1");
    }

    public void SelectIF()
    {
        InpputField.text = wait.ToString("F1");
    }

    public void DeselectIF()
    {
        InpputField.text = "Wait: " + wait.ToString("F1");
    }
}
