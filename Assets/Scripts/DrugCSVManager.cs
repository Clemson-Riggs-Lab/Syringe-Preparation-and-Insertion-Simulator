using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

public class DrugCSVManager : MonoBehaviour
{
    public TextAsset file;
    List<Drug> rowList = new List<Drug>();
    bool Loaded;

    void Start()
    {
        Loaded = false;
        Load(file);
    }
    

    public bool IsLoaded()
    {
        return Loaded;
    }

    public List<Drug> GetRowList()
    {
        return rowList;
    }

    public void Load(TextAsset csv)
    {
        rowList.Clear();
        string[][] grid = CsvParser2.Parse(csv.text);
        for (int i = 1; i < grid.Length; i++)
        {

            Drug row = new Drug();
            row.ID = int.Parse(grid[i][0]);
            row.DrugName = grid[i][1];

            if (grid[i][2] != "")
            {
                row.Concentration = float.Parse(grid[i][2]);
            }
            else
            {
                row.Concentration = null;
            }

            row.ConcentrationUnit = grid[i][3];
            row.AdditionalText = grid[i][4];
            row.Unit = grid[i][5];
            row.IsSyringe = (grid[i][6]== "1");

            row.FullLabelText = row.DrugName + " " + row.Concentration + " " + row.ConcentrationUnit + " " + row.AdditionalText + " " + row.Unit;
            
            if (grid[i][7]!="")
                row.PrimaryColor = new Color32(byte.Parse(grid[i][7]), byte.Parse(grid[i][8]), byte.Parse(grid[i][9]), 255);
            else
                row.PrimaryColor = new Color32(255, 255, 255,255);

            if (grid[i][10] != "")
                row.SecondaryColor = new Color32(byte.Parse(grid[i][10]), byte.Parse(grid[i][11]), byte.Parse(grid[i][12]), 255);
            else
                row.SecondaryColor = row.PrimaryColor;

            rowList.Add(row);
        }
        Loaded = true;
        
    }

    public int NumRows()
    {
        return rowList.Count;
    }

    public Drug GetAt(int i)
    {
        if (rowList.Count <= i)
            return null;
        return rowList[i];
    }

    public Drug Find_ID(int find)
    {
        return rowList.Find(x => x.ID == find);
    }
    public List<Drug> FindAll_ID(int find)
    {
        return rowList.FindAll(x => x.ID == find);
    }
    public Drug Find_DrugName(string find)
    {
        return rowList.Find(x => x.DrugName == find);
    }
    public List<Drug> FindAll_DrugName(string find)
    {
        return rowList.FindAll(x => x.DrugName == find);
    }
    public Drug Find_Concentration(int find)
    {
        return rowList.Find(x => x.Concentration == find);
    }
    public List<Drug> FindAll_Concentration(int find)
    {
        return rowList.FindAll(x => x.Concentration == find);
    }
    public Drug Find_ConcentrationUnit(string find)
    {
        return rowList.Find(x => x.ConcentrationUnit == find);
    }
    public List<Drug> FindAll_ConcentrationUnit(string find)
    {
        return rowList.FindAll(x => x.ConcentrationUnit == find);
    }
    public Drug Find_AdditionalText(string find)
    {
        return rowList.Find(x => x.AdditionalText == find);
    }
    public List<Drug> FindAll_AdditionalText(string find)
    {
        return rowList.FindAll(x => x.AdditionalText == find);
    }
    public Drug Find_Unit(string find)
    {
        return rowList.Find(x => x.Unit == find);
    }
    public List<Drug> FindAll_Unit(string find)
    {
        return rowList.FindAll(x => x.Unit == find);
    }

}