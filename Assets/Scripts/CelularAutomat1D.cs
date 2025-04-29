using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;

public class CelularAutomat1D : MonoBehaviour
{
    public int NumberOfCells;
    public float ActiveTime;
    public float Sice;

    public GameObject preCell;

    public List<Cell> ParrentCells = new List<Cell>();
    public List<Cell> ChildCells = new List<Cell>();

    private bool stateL;
    private bool stateS;
    private bool stateR;

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

    public void Awake()
    {
        Set000(true);

        Set001(true);
        Set010(true);
        Set100(true);
        
        Set011(true);
        Set101(true);
        Set110(true);

        Set111(true);


        
    }

    public void GenerateFirstGeneration()
    {
        for (int i = 0; i < NumberOfCells; i++)
        {
            GameObject curCell = Instantiate(preCell);
            curCell.transform.position = new Vector3((Sice*i), 0, 0);
        }
    }

    public void OperateGeneration()
    {
        for(int i = 0; i < ParrentCells.Count; i++)
        {
            GetNeighbors(i);
            ChildCells[i].SetState(GenerateChild());
        }

        for (int i = 0; i < ChildCells.Count; i++)
        {
            Debug.Log(ChildCells[i].GetState());
        }

        ParrentCells = ChildCells;
    }

    public void GetNeighbors(int val)
    {
        //Previous Neighbor
        if(val != 0) 
        {            
            stateL = ParrentCells[val - 1].GetState();            
        }
        else
        {
            stateL = ParrentCells[ParrentCells.Count - 2].GetState();
        }

        //Self
        stateS = ParrentCells[val].GetState();

        //Next Neighbor
        if (val != ParrentCells.Count - 1)
        {
            stateL = ParrentCells[val + 1].GetState();
        }
        else
        {
            stateR = ParrentCells[0].GetState();
        }
       
    }

    public bool GenerateChild()
    {
        //CERO vivos
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
        return true;
    }


//Tugle
    //CERO vivos
    public void Set000()
    {
        S000 = !S000;
        B000.GetComponentInChildren<TextMeshPro>().text = S000.ToString();
    }

    //UN vivo
    public void Set001()
    {
        S001 = !S001;
        B001.GetComponentInChildren<TextMeshPro>().text = S001.ToString();
    }
    public void Set010()
    {
        S010 = !S010;
        B010.GetComponentInChildren<TextMeshPro>().text = S010.ToString();
    }
    public void Set100()
    {
        S100 = !S100;
        B100.GetComponentInChildren<TextMeshPro>().text = S100.ToString();
    }

    //DOS vivos
    public void Set011()
    {
        S011 = !S011;
        B011.GetComponentInChildren<TextMeshPro>().text = S011.ToString();
    }
    public void Set101()
    {
        S101 = !S101;
        B101.GetComponentInChildren<TextMeshPro>().text = S101.ToString();
    }
    public void Set110()
    {
        S110 = !S110;
        B110.GetComponentInChildren<TextMeshPro>().text = S110.ToString();
    }

    //TRES vivos
    public void Set111()
    {
        S111 = !S111;
        B111.GetComponentInChildren<TextMeshPro>().text = S111.ToString();
    }

//Manual
    //CERO vivos
    public void Set000(bool val)
    {
        S000 = val;
        B000.GetComponentInChildren<TextMeshPro>().text = S000.ToString();
    }

    //UN vivo
    public void Set001(bool val)
    {
        S001 = val;
        B001.GetComponentInChildren<TextMeshPro>().text = S001.ToString();
    }
    public void Set010(bool val)
    {
        S010 = val;
        B010.GetComponentInChildren<TextMeshPro>().text = S010.ToString();
    }
    public void Set100(bool val)
    {
        S100 = val;
        B100.GetComponentInChildren<TextMeshPro>().text = S100.ToString();
    }

    //DOS vivos
    public void Set011(bool val)
    {
        S011 = val;
        B011.GetComponentInChildren<TextMeshPro>().text = S011.ToString();
    }
    public void Set101(bool val)
    {
        S101 = val;
        B101.GetComponentInChildren<TextMeshPro>().text = S101.ToString();
    }
    public void Set110(bool val)
    {
        S110 = val;
        B110.GetComponentInChildren<TextMeshPro>().text = S110.ToString();
    }

    //TRES vivos
    public void Set111(bool val)
    {
        S111 = val;
        B111.GetComponentInChildren<TextMeshPro>().text = S111.ToString();
    }
}
