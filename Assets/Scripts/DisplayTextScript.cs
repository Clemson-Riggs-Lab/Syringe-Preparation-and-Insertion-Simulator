using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTextScript : MonoBehaviour {

    public Text DisplaySyringeInformationTextArea;
    public Text DisplayVialInformationTextArea;
    public Text AmmountText;
    public Text TaskRequirementsText;

    public GameObject SelectedSyringe { get; set; }
    public GameObject SelectedVial { get; set; }
    // Use this for initialization
    void Start () {

        SetSyringeInfo();
        SetVialInfo();
    }
	
	// Update is called once per frame
	void Update () {
        if (SelectedSyringe != null)
            SetSyringeInfo(SelectedSyringe);
        if (SelectedVial != null)
            SetVialInfo(SelectedVial);
		
	}

    public void SetSyringeInfo(GameObject selected)
    {
        string info;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "The set display Info has been passed a null object. this shouldnt happen. please contact developer", "Ok"); // Issue warning  Cant return a syringe that isnt empty.
            return;
        }
        //check if the object is a syringe
        if (selected.GetComponent<GeneralManagerScript>().type == GeneralManagerScript.Type.Syringe)
        {
            string tag; // new tag that varies weither the syringe is tagged or not
            string drug; // this varies weather there is a drug in the syringe or not
            string status;

            if (selected.GetComponent<SyringeManagerScript>().IsTagged == false) // if its not tagged
            {
                tag = " No";
            }
            else // if tagged
            {
                tag = "Yes";
            }
            if (selected.GetComponent<SyringeManagerScript>().DrugInside == null)
            {
                drug = "Not filled";
                status = "New";
            }
            else // if there is a drug in the syringe
            {
                drug = selected.GetComponent<SyringeManagerScript>().DrugInside.FullLabelText;
                status = "Filled";

                if (selected.GetComponent<SyringeManagerScript>().SyringeStatus == SyringeManagerScript.Status.Administered)
                    status = "Administered";
            }
            // creating the info string with some rich text features ( bold, new line, ...)
            info =  "<b>Status: </b> " + status+ "\n"
            + "<b>Drug: </b>" + drug + "\n"
            + "<b>Volume: </b>" + selected.GetComponent<SyringeManagerScript>().VolumeInside.ToString("F0") + " ml" + "\n"
            + "<b>Syringe Lable: </b>" + tag + "\n";
            
        }

        // if its a vial
        // setinfo shouldnt be called for any object other than a syringe. so something fishy has happened
        else
        {
            EditorUtility.DisplayDialog("Error", "The set display Info has been passed an object that shouldnt be displayed. this shouldnt happen. please contact developer", "Ok"); // Issue warning  Cant return a syringe that isnt empty.
            return;
        }
        SelectedSyringe = selected;
        DisplaySyringeInformationTextArea.text =info;

    }

    public void SetVialInfo(GameObject selected)
    {
        string info;

        if (selected == null)
        {
            EditorUtility.DisplayDialog("Error", "The set display Info has been passed a null object. this shouldnt happen. please contact developer", "Ok"); // Issue warning  Cant return a syringe that isnt empty.
            return;
        }

        else if (selected.GetComponent<GeneralManagerScript>().type == GeneralManagerScript.Type.Vial)
        {

            // creating the info string with some rich text features ( bold, new line, ...)
            info ="<b>Drug Name: </b>" + selected.GetComponent<VialManagerScript>().DrugInside.DrugName + "\n"
                + "<b>Drug Concentration: </b>" + selected.GetComponent<VialManagerScript>().DrugInside.Concentration + " " + selected.GetComponent<VialManagerScript>().DrugInside.ConcentrationUnit + "\n"
                + "<b>Additional Text: </b>" + selected.GetComponent<VialManagerScript>().DrugInside.AdditionalText + "\n";
        }

        else
        {
            EditorUtility.DisplayDialog("Error", "The set display Info has been passed an object that shouldnt be displayed. this shouldnt happen. please contact developer", "Ok"); // Issue warning  Cant return a syringe that isnt empty.
            return;
        }
        SelectedVial = selected;
        DisplayVialInformationTextArea.text = info;

    }

    // this is called to clear the text area
    public void SetSyringeInfo()
    {
        string info =  " Select a syringe to display its information.";
        SelectedSyringe = null;
        DisplaySyringeInformationTextArea.text = info;

    }

    public void SetVialInfo()
    {
        string info = "Select a vial to display its information.";
        SelectedVial = null;
        DisplayVialInformationTextArea.text = info;

    }

    public void SetAmmount(float ammount)
    {
        AmmountText.text = " Volume: " + ammount.ToString("F0") + " ml";
    }

    public void SetTaskRequirements(Task CurrentTask)
    {
        TaskRequirementsText.text = CurrentTask.FullText;
    }
}
