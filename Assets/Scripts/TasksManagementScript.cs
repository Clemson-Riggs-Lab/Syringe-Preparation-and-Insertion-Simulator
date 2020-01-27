using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TasksManagementScript : MonoBehaviour {
    public List<Task> TasksList = new List<Task>();
    public Task CurrentTask { get; set; }
    public static int index { get; set; }
    int numberofTasks;
    bool keepUpdating;
    public double SimulationStartTime;
    public double TaskEndTime;
    // Use this for initialization
    void Start () {
        TasksList = gameObject.GetComponent<TaskCSVManager>().GetTasksList();
        numberofTasks = gameObject.GetComponent<TaskCSVManager>().NumRows();
        CurrentTask = null;
        index = 0;
        keepUpdating = true;
        SimulationStartTime = Time.time;
        TaskEndTime = SimulationStartTime + TasksList[0].TimeToPresent;

    }

    public void NextTask()
    {
        if ((index+1) >= numberofTasks)
        {
            EditorUtility.DisplayDialog("Simulation Done", "All tasks have been finished.", "Ok");
            keepUpdating = false;
            return;
        }
            CurrentTask = TasksList[index];
        TaskEndTime = SimulationStartTime + TasksList[index+1].TimeToPresent;
        index++;
        gameObject.GetComponent<DisplayTextScript>().SetTaskRequirements(CurrentTask);
        this.gameObject.GetComponent<CsvOutputWrite>().AddEvent("NewTask");

    }

    // Update is called once per frame
    void Update () {
        if (keepUpdating)
        if (Time.time > TaskEndTime)
            NextTask();
		
	}
}
