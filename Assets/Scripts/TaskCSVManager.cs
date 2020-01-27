using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


public class TaskCSVManager : MonoBehaviour
{
    public TextAsset file;
    List<Task> TasksList = new List<Task>();
    bool isLoaded = false;

    void Start()
    {
        isLoaded = false;
        Load(file);

        System.Comparison<Task> comparison = (x, y) => x.TimeToPresent.CompareTo(y.TimeToPresent);
        TasksList.Sort(comparison);

    }




    public bool IsLoaded()
    {
        return isLoaded;
    }

    public List<Task> GetTasksList()
    {
        return TasksList;
    }

    public void Load(TextAsset csv)
    {
        TasksList.Clear();
        string[][] grid = CsvParser2.Parse(csv.text);
        for (int i = 1; i < grid.Length; i++)
        {
            Task task = new Task();
            task.TaskNumber = grid[i][0];
            task.TimeToPresent = TimeSpan.Parse(grid[i][1]).TotalSeconds;
            task.DrugName = grid[i][2];
            task.Concentration = grid[i][3];
            task.ConcentrationUnit = grid[i][4];
            task.Unit = grid[i][5];
            task.PatientWeight = grid[i][6];
            task.DosingNumber = grid[i][7];
            task.DosingUnit = grid[i][8];


            TasksList.Add(task);
        }
        isLoaded = true;
    }

    public int NumRows()
    {
        return TasksList.Count;
    }

    public Task GetAt(int i)
    {
        if (TasksList.Count <= i)
            return null;
        return TasksList[i];
    }

    public Task Find_TaskNumber(string find)
    {
        return TasksList.Find(x => x.TaskNumber == find);
    }
    public List<Task> FindAll_TaskNumber(string find)
    {
        return TasksList.FindAll(x => x.TaskNumber == find);
    }
    public Task Find_TimeToPresent(double find)
    {
        return TasksList.Find(x => x.TimeToPresent == find);
    }
    public List<Task> FindAll_TimeToPresent(double find)
    {
        return TasksList.FindAll(x => x.TimeToPresent == find);
    }
    public Task Find_DrugName(string find)
    {
        return TasksList.Find(x => x.DrugName == find);
    }
    public List<Task> FindAll_DrugName(string find)
    {
        return TasksList.FindAll(x => x.DrugName == find);
    }
    public Task Find_Concentration(string find)
    {
        return TasksList.Find(x => x.Concentration == find);
    }
    public List<Task> FindAll_Concentration(string find)
    {
        return TasksList.FindAll(x => x.Concentration == find);
    }
    public Task Find_ConcentrationUnit(string find)
    {
        return TasksList.Find(x => x.ConcentrationUnit == find);
    }
    public List<Task> FindAll_ConcentrationUnit(string find)
    {
        return TasksList.FindAll(x => x.ConcentrationUnit == find);
    }
    public Task Find_Unit(string find)
    {
        return TasksList.Find(x => x.Unit == find);
    }
    public List<Task> FindAll_Unit(string find)
    {
        return TasksList.FindAll(x => x.Unit == find);
    }
    public Task Find_PatientWeight(string find)
    {
        return TasksList.Find(x => x.PatientWeight == find);
    }
    public List<Task> FindAll_PatientWeight(string find)
    {
        return TasksList.FindAll(x => x.PatientWeight == find);
    }
    public Task Find_DosingNumber(string find)
    {
        return TasksList.Find(x => x.DosingNumber == find);
    }
    public List<Task> FindAll_DosingNumber(string find)
    {
        return TasksList.FindAll(x => x.DosingNumber == find);
    }
    public Task Find_DosingUnit(string find)
    {
        return TasksList.Find(x => x.DosingUnit == find);
    }
    public List<Task> FindAll_DosingUnit(string find)
    {
        return TasksList.FindAll(x => x.DosingUnit == find);
    }

}