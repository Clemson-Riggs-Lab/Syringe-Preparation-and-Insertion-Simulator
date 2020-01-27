using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using UnityEngine.UI;

public class CsvOutputWrite : MonoBehaviour
{

    private List<string[]> rowData = new List<string[]>();
    private string filepath;
    public DateTime SelectDrugStartTime { get; set; }
    public DateTime MoveSyringeStartTime { get; set; }
    public DateTime FillSyringeStartTime { get; set; }
    public DateTime ApplyTagStartTime { get; set; }
    public DateTime AdministerSyringeStartTime { get; set; }
    public DateTime TrashSyringeStartTime { get; set; }
    // Use this for initialization
    void Start()
    {
        SetUp();
        filepath = Application.dataPath + "/OutputFiles/" + "TasksOutput" + DateTime.Now.ToString("MM-dd-yy-H-mm") + ".csv";
    }

    void SetUp()
    {
        // Creating First row of titles manually..
        string[] rowDataTemp = new string[10];
        rowDataTemp[0] = "Start Time";
        rowDataTemp[1] = "EventType";
        rowDataTemp[2] = "Drug Inside";
        rowDataTemp[3] = "Volume In Syringe";
        rowDataTemp[4] = "Is Filled?";
        rowDataTemp[5] = "Is Tagged?";
        rowDataTemp[6] = "Is Administered?";
        rowDataTemp[7] = "Is Trashed?";
        rowDataTemp[8] = "End Time";
        rowDataTemp[9] = "Notes";
        rowData.Add(rowDataTemp);

    }
    public void AddEvent(string EventType, GameObject SyringeOrVial = null)
    {
        string[] rowDataTemp = new string[10];
        switch (EventType)
        {
            case "NewTask":

                rowDataTemp[0] = DateTime.Now.ToString("HH:mm:ss:ffff");
                rowDataTemp[1] = "New Task";
                rowDataTemp[2] = this.gameObject.GetComponent<TasksManagementScript>().CurrentTask.DrugName;
                rowDataTemp[3] = ""; //TODO what would that volume be. either kylie provides it and we add it to the input file, or I should know to calculate it from the data we have : patient weight, concentration, dosage ...
                rowDataTemp[4] = " ---";
                rowDataTemp[5] = "---";
                rowDataTemp[6] = "---";
                rowDataTemp[7] = "---";
                rowDataTemp[8] = "---";//SimulationStartTime.AddSeconds(this.gameObject.GetComponent<TasksManagementScript>().TasksList[TasksManagementScript.index + 1].TimeToPresent).ToString("HH:mm:ss:ffff");
                rowDataTemp[9] = this.gameObject.GetComponent<TasksManagementScript>().CurrentTask.FullText;
                //rowDataTemp[9] = "Prepare the appropriate dose of " + this.gameObject.GetComponent<TasksManagementScript>().CurrentTask.DrugName + " " +
                //                  this.gameObject.GetComponent<TasksManagementScript>().CurrentTask.Concentration + " " + this.gameObject.GetComponent<TasksManagementScript>().CurrentTask.ConcentrationUnit +
                //                  " (" + this.gameObject.GetComponent<TasksManagementScript>().CurrentTask.Unit + ") " + "for a  " + this.gameObject.GetComponent<TasksManagementScript>().CurrentTask.PatientWeight + " kg patient."
                //                 + " Dose for " + this.gameObject.GetComponent<TasksManagementScript>().CurrentTask.DrugName + ": " + this.gameObject.GetComponent<TasksManagementScript>().CurrentTask.DosingNumber +
                //                 " " + this.gameObject.GetComponent<TasksManagementScript>().CurrentTask.DosingUnit;
                rowData.Add(rowDataTemp);
                SelectDrugStartTime = DateTime.Now;
                break;

            case "DrugSelected":

                rowDataTemp[0] = SelectDrugStartTime.ToString("HH:mm:ss:ffff");
                rowDataTemp[1] = "Select Drug";
                rowDataTemp[2] = SyringeOrVial.GetComponent<VialManagerScript>().DrugInside.DrugName;
                rowDataTemp[3] = "-";
                rowDataTemp[4] = "-";
                rowDataTemp[5] = "-";
                rowDataTemp[6] = "-";
                rowDataTemp[7] = "-";
                rowDataTemp[8] = DateTime.Now.ToString("HH:mm:ss:ffff");
                rowData.Add(rowDataTemp);
                MoveSyringeStartTime = DateTime.Now;
                break;

            case "VialRemoved":
                if(MoveSyringeStartTime>FillSyringeStartTime)
                rowDataTemp[0] = MoveSyringeStartTime.ToString("HH:mm:ss:ffff");
                else
                rowDataTemp[0] = FillSyringeStartTime.ToString("HH:mm:ss:ffff");

                rowDataTemp[1] = "Remove Drug";
                rowDataTemp[2] = SyringeOrVial.GetComponent<VialManagerScript>().DrugInside.DrugName;
                rowDataTemp[3] = "-";
                rowDataTemp[4] = "-";
                rowDataTemp[5] = "-";
                rowDataTemp[6] = "-";
                rowDataTemp[7] = "-";
                rowDataTemp[8] = DateTime.Now.ToString("HH:mm:ss:ffff");
                

                SelectDrugStartTime = DateTime.Now;

                rowData.Add(rowDataTemp);
                break;

            case "SyringeMoved":

                rowDataTemp[0] = MoveSyringeStartTime.ToString("HH:mm:ss:ffff");
                rowDataTemp[1] = "Move Syringe";
                rowDataTemp[2] = "no";
                rowDataTemp[3] = "0.00";
                rowDataTemp[4] = "0";
                rowDataTemp[5] = "0";
                rowDataTemp[6] = "0";
                rowDataTemp[7] = "0";
                rowDataTemp[8] = DateTime.Now.ToString("HH:mm:ss:ffff");
                FillSyringeStartTime = DateTime.Now;
                rowData.Add(rowDataTemp);
                break;

            case "SyringeFilled":

                rowDataTemp[0] = FillSyringeStartTime.ToString("HH:mm:ss:ffff");
                rowDataTemp[1] = "Fill Syringe";
                rowDataTemp[2] = SyringeOrVial.GetComponent<SyringeManagerScript>().DrugInside.DrugName;
                rowDataTemp[3] = SyringeOrVial.GetComponent<SyringeManagerScript>().VolumeInside.ToString("F0");
                rowDataTemp[4] = "1";
                rowDataTemp[5] = "0";
                rowDataTemp[6] = "0";
                rowDataTemp[7] = "0";
                rowDataTemp[8] = DateTime.Now.ToString("HH:mm:ss:ffff");
                ApplyTagStartTime = DateTime.Now;
                rowData.Add(rowDataTemp);
                break;

            case "SyringeTrashed":

                rowDataTemp[0] = DateTime.Now.ToString("HH:mm:ss:ffff");
                rowDataTemp[1] = "Trash Syringe";
                

                if (SyringeOrVial.GetComponent<SyringeManagerScript>().DrugInside != null)
                {
                    rowDataTemp[2] = SyringeOrVial.GetComponent<SyringeManagerScript>().DrugInside.DrugName;
                    rowDataTemp[3] = SyringeOrVial.GetComponent<SyringeManagerScript>().VolumeInside.ToString("F0");

                    
                }

                else
                {
                    rowDataTemp[2] = "no";
                    rowDataTemp[3] = "0.00";

                    if (FillSyringeStartTime < MoveSyringeStartTime)// this means that the syringe was never moved to the filling area
                    {
                        rowDataTemp[0] = MoveSyringeStartTime.ToString("HH:mm:ss:ffff");
                    }
                    else
                        rowDataTemp[0] = FillSyringeStartTime.ToString("HH:mm:ss:ffff");

                }


                if (SyringeOrVial.GetComponent<SyringeManagerScript>().TimeFinishedFilling!=null)
                {
                    rowDataTemp[4] = "1";
                    rowDataTemp[0] = ApplyTagStartTime.ToString("HH:mm:ss:ffff");
                }
                else
                {
                    rowDataTemp[4] = "0";
                }

                if (SyringeOrVial.GetComponent<SyringeManagerScript>().IsTagged != false)
                {
                    rowDataTemp[5] = "1";
                    rowDataTemp[0] = AdministerSyringeStartTime.ToString("HH:mm:ss:ffff");
                }
                else
                {
                    rowDataTemp[5] = "0";
                }

                if (SyringeOrVial.GetComponent<SyringeManagerScript>().TimeAdminstred != null)
                {
                    rowDataTemp[6] = "1";
                    rowDataTemp[0] = TrashSyringeStartTime.ToString("HH:mm:ss:ffff");
                }
                else
                {
                    rowDataTemp[6] = "0";
                }

                rowDataTemp[7] = "1";
                rowDataTemp[8] = DateTime.Now.ToString("HH:mm:ss:ffff");
                SelectDrugStartTime = DateTime.Now;
                MoveSyringeStartTime = DateTime.Now;
                rowData.Add(rowDataTemp);
                break;

            case "SyringeTagged":

                rowDataTemp[0] = ApplyTagStartTime.ToString("HH:mm:ss:ffff");
                rowDataTemp[1] = "Apply Lable";
                rowDataTemp[2] = SyringeOrVial.GetComponent<SyringeManagerScript>().DrugInside.DrugName;
                rowDataTemp[3] = SyringeOrVial.GetComponent<SyringeManagerScript>().VolumeInside.ToString("F0");

                if (SyringeOrVial.GetComponent<SyringeManagerScript>().TimeFinishedFilling != null)
                {
                    rowDataTemp[4] = "1";
                }
                else
                {
                    rowDataTemp[4] = "0";
                }

                rowDataTemp[5] = "1";
                rowDataTemp[6] = "0";
                rowDataTemp[7] = "0";
                rowDataTemp[8] = DateTime.Now.ToString("HH:mm:ss:ffff");
                AdministerSyringeStartTime = DateTime.Now;
                rowData.Add(rowDataTemp);
                break;



            case "SyringeAdministered":

                rowDataTemp[0] = ApplyTagStartTime.ToString("HH:mm:ss:ffff");
                rowDataTemp[1] = "AdministerSyringe";
                rowDataTemp[2] = SyringeOrVial.GetComponent<SyringeManagerScript>().DrugInside.DrugName;
                rowDataTemp[3] = SyringeOrVial.GetComponent<SyringeManagerScript>().VolumeInside.ToString("F0");

                if (SyringeOrVial.GetComponent<SyringeManagerScript>().TimeFinishedFilling != null)
                {
                    rowDataTemp[4] = "1";
                }
                else
                {
                    rowDataTemp[4] = "0";
                }

                if (SyringeOrVial.GetComponent<SyringeManagerScript>().IsTagged != false)
                {
                    rowDataTemp[5] = "1";
                    rowDataTemp[0] = AdministerSyringeStartTime.ToString("HH:mm:ss:ffff");
                }
                else
                {
                    rowDataTemp[5] = "0";
                }
                rowDataTemp[6] = "1";
                rowDataTemp[7] = "0";
                rowDataTemp[8] = DateTime.Now.ToString("HH:mm:ss:ffff");
                TrashSyringeStartTime = DateTime.Now;
                rowData.Add(rowDataTemp);
                break;
        }

        Save();



    }

    public void Save()
    {

        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));


        StreamWriter outStream = System.IO.File.CreateText(filepath);
        outStream.WriteLine(sb);
        outStream.Close();
        //System.IO.File.WriteAllText(
    }


}