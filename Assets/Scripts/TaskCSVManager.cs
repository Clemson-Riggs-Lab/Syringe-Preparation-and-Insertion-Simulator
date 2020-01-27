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

        //System.Comparison<Task> comparison = (x, y) => x.TaskNumber.CompareTo(y.TaskNumber);
        //TasksList.Sort(comparison);

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
            task.DrugName = grid[i][1];
            task.FullText = grid[i][2];



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
    public Task Find_FullText(string find)
    {
        return TasksList.Find(x => x.FullText == find);
    }
    public List<Task> FindAll_FullText(string find)
    {
        return TasksList.FindAll(x => x.FullText == find);
    }
    public Task Find_DrugName(string find)
    {
        return TasksList.Find(x => x.DrugName == find);
    }
    public List<Task> FindAll_DrugName(string find)
    {
        return TasksList.FindAll(x => x.DrugName == find);
    }

}