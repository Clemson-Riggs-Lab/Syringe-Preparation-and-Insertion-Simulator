using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;

public class SceneManagementScript : MonoBehaviour {
    //prefabs
    public GameObject VialPrefab;
    public GameObject SyringePrefab;
    public GameObject TagPrefab;

    //UI
    public Dropdown DrugsDropdown;
    public Slider AmmountSlider;
    public Button FillButton;

    //objects that matter
    public GameObject SyringeInFillingArea { get; set; }
    public GameObject SyringeOnIVLine { get; set; }
    public GameObject VialInFillingArea { get; set; }
   

    //initial positions
    public GameObject SyringeInitialPosition;
    public GameObject VialInitialPosition;
    public GameObject TagInitialPlaceholder;
    public GameObject NewSyringePlaceholder;
    public GameObject VialFillingAreaPlaceholder;
    public GameObject SyringeFillingAreaPlaceholder;
    public GameObject RemoveVialArea;

    // Use this for initialization
    void Start () {      
        AddNewSyringe();
    }
	
	// Update is called once per frame

    public void AddNewSyringe()
    {
        if (NewSyringePlaceholder.GetComponent<PlaceholderManagerScript>().IsEmpty)
        {

            GameObject Syringe = Instantiate(SyringePrefab) as GameObject;
            Syringe.gameObject.transform.position = SyringeInitialPosition.transform.position;
            Syringe.GetComponent<SyringeManagerScript>().SyringeStatus = SyringeManagerScript.Status.New;
            Syringe.GetComponent<GeneralManagerScript>().type = GeneralManagerScript.Type.Syringe;
            Syringe.GetComponent<GeneralManagerScript>().Move = true;
            Syringe.GetComponent<GeneralManagerScript>().Placeholder = NewSyringePlaceholder;
            Syringe.GetComponent<GeneralManagerScript>().Destination = NewSyringePlaceholder;
            Syringe.transform.position = SyringeInitialPosition.transform.position;


        }
        else
           // this.gameObject.GetComponent<PopupWindow>().Open("cant add a syringe because there is already one attached to the initial position. this isnt supposed to happen.contact developer");
        EditorUtility.DisplayDialog("Flow Violation", "cant add a syringe because there is already one attached to the initial position. this isnt supposed to happen. contact developer", "Ok"); // Issue Warning

    }

    public void RemoveObject( GameObject obj)
    {
        DisconnectFillAreaSyringe(obj);
        DisconnectIVAreaSyringe(obj);
        DisconnectVial(obj);
        ResetInfoDisplay(obj);

        Destroy(obj);

    }

    private void ResetInfoDisplay(GameObject obj)
    {
        if (GameObject.ReferenceEquals(gameObject.GetComponent<DisplayTextScript>().SelectedSyringe,obj))
        {
            gameObject.GetComponent<DisplayTextScript>().SetSyringeInfo();
        }
        if (GameObject.ReferenceEquals(gameObject.GetComponent<DisplayTextScript>().SelectedVial, obj))
        {
            gameObject.GetComponent<DisplayTextScript>().SetVialInfo();
        }
    }

    public void AddNewVial()
    {
        if(DrugsDropdown.value==0)
        {
           // this.gameObject.GetComponent<PopupWindow>().Open("Please select a vial to add");
            EditorUtility.DisplayDialog("Flow Violation", "Please select a vial to add", "Ok"); // Issue Warning
            return;

        }
        else if (VialInFillingArea!= null)
        {
           // this.gameObject.GetComponent<PopupWindow>().Open("Drug Vial already in filling area");
            EditorUtility.DisplayDialog("Flow Violation", "Drug Vial already in filling area", "Ok"); // Issue Warning
            return;
        }
        else if (NewSyringePlaceholder.GetComponent<PlaceholderManagerScript>().IsEmpty==true)
        {
          //  this.gameObject.GetComponent<PopupWindow>().Open( "Finish the previous task first");
            EditorUtility.DisplayDialog("Flow Violation", "Finish the previous task first", "Ok"); // Issue Warning
            return;
        }
        else
        {
            Drug drugInside = this.gameObject.GetComponent<DrugCSVManager>().Find_ID(DrugsDropdown.value);
            GameObject Vial = Instantiate(VialPrefab) as GameObject; //creating object
            Vial.GetComponent<VialManagerScript>().DrugInside = drugInside;
            Vial.transform.position = VialInitialPosition.transform.position;// initial positioning
            Vial.GetComponent<GeneralManagerScript>().type = GeneralManagerScript.Type.Vial;
            Vial.GetComponent<GeneralManagerScript>().Move = true;
            Vial.GetComponent<GeneralManagerScript>().Placeholder = VialFillingAreaPlaceholder;
            Vial.GetComponent<GeneralManagerScript>().Destination = VialFillingAreaPlaceholder;
            Vial.transform.Find("Tag/FirstColor").gameObject.GetComponent<Renderer>().material.color = drugInside.PrimaryColor;
            Vial.transform.Find("Tag/SecondColor").gameObject.GetComponent<Renderer>().material.color = drugInside.SecondaryColor;
            Vial.transform.Find("Tag/TagText").gameObject.GetComponent<TextMesh>().text = drugInside.DrugName + " \n volume: " + AmmountSlider.value.ToString("F0") + " ml";
            gameObject.GetComponent<DisplayTextScript>().SetVialInfo(Vial);
            ConnectVial(Vial);
            this.gameObject.GetComponent<CsvOutputWrite>().AddEvent("DrugSelected", Vial);

        }

    }


    public void FillSyringe()
    {
        if(SyringeInFillingArea==null )
        {
            //this.gameObject.GetComponent<PopupWindow>().Open("You cant Fill without connecting a syringe.");
            EditorUtility.DisplayDialog("Flow Violation", "You cant Fill without connecting a syringe.", "Ok"); // Issue warning  Cant fill without syringe
            return;
        }
        if (SyringeInFillingArea.GetComponent<SyringeManagerScript>().SyringeStatus == SyringeManagerScript.Status.OnFillingPosition)
        {
            if (VialInFillingArea != null)
            {
     
                //filling syringe
                SyringeInFillingArea.GetComponent<SyringeManagerScript>().DrugInside =VialInFillingArea.GetComponent<VialManagerScript>().DrugInside;
                SyringeInFillingArea.GetComponent<SyringeManagerScript>().VolumeInside = AmmountSlider.value; 
                SyringeInFillingArea.GetComponent<SyringeManagerScript>().SyringeStatus = SyringeManagerScript.Status.Filled;
                SyringeInFillingArea.GetComponent<SyringeManagerScript>().TimeFinishedFilling = DateTime.Now.ToShortTimeString();

                //adding a tag to the label area
                AddNewTag(SyringeInFillingArea.GetComponent<SyringeManagerScript>().DrugInside);
                FillButton.interactable = false;

                //removing vial
                VialInFillingArea.GetComponent<GeneralManagerScript>().Disconnect();
                gameObject.GetComponent<DisplayTextScript>().SetVialInfo();
                Destroy(VialInFillingArea);

                this.gameObject.GetComponent<CsvOutputWrite>().AddEvent("SyringeFilled", SyringeInFillingArea);

            }
            else
                EditorUtility.DisplayDialog("Flow Violation", "You cant Fill a syringe without connecting a vial.", "Ok"); // Issue warning  Cant fill without vial
            return;
        }
        else
            throw new Exception("trying to fill a filled syringe"); // the flow shouldnt take us here
    }


    public bool ApplyTag(GameObject tag)
    {
        if (SyringeInFillingArea == null)
        {
            EditorUtility.DisplayDialog("Flow Violation", "You need a syringe to apply a tag on.", "Ok"); // Issue warning  Cant adminster without syringe
            tag.GetComponent<GeneralManagerScript>().Destination = TagInitialPlaceholder;
            return false;
        }
        else if (SyringeInFillingArea.GetComponent<SyringeManagerScript>().IsTagged)
        {
            EditorUtility.DisplayDialog("Flow Violation", "The syringe is already tagged.", "Ok"); // Issue warning  Cant adminster without syringe
            tag.GetComponent<GeneralManagerScript>().Destination = TagInitialPlaceholder;
            return false;
        }
        else if (SyringeInFillingArea.GetComponent<SyringeManagerScript>().DrugInside == null)
        {
            EditorUtility.DisplayDialog("Flow Violation", "Please fill the syringe first.", "Ok"); // Issue warning  Cant adminster without syringe
            tag.GetComponent<GeneralManagerScript>().Destination = TagInitialPlaceholder;
            return false;
        }
        else
        {
            // if it passed all the above ifs, then it is good to go
            SyringeInFillingArea.GetComponent<SyringeManagerScript>().IsTagged = true;
            SyringeInFillingArea.GetComponent<SyringeManagerScript>().TagDrug = tag.GetComponent<TagManagerScript>().DrugName;
            SyringeInFillingArea.GetComponent<SyringeManagerScript>().TagVolume = tag.GetComponent<TagManagerScript>().Volume;
            tag.transform.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            this.gameObject.GetComponent<CsvOutputWrite>().AddEvent("SyringeTagged", SyringeInFillingArea);
            return true;
        }
    }

    public void AdminsterSyringe()
    {
        if (SyringeOnIVLine == null)
        {
            EditorUtility.DisplayDialog("Flow Violation", "You cant adminster without connecting a syringe.", "Ok"); // Issue warning  Cant adminster without syringe
            return;
        }
        if (SyringeOnIVLine.GetComponent<SyringeManagerScript>().SyringeStatus != SyringeManagerScript.Status.OnIVLine)
        {
            throw new Exception("trying to adminster a syringe that isnt on the IV line"); // the flow shouldnt take us here

        }
        if (SyringeOnIVLine.GetComponent<SyringeManagerScript>().DrugInside == null)
        {
            EditorUtility.DisplayDialog("Flow Violation", "You cant adminster an empty syringe.", "Ok"); // Issue warning  Cant adminster without syringe
            return;
        }
        SyringeOnIVLine.GetComponent<SyringeManagerScript>().SyringeStatus = SyringeManagerScript.Status.Administered;
        SyringeOnIVLine.GetComponent<SyringeManagerScript>().TimeAdminstred = DateTime.Now.ToShortTimeString();
        this.gameObject.GetComponent<CsvOutputWrite>().AddEvent("SyringeAdministered", SyringeOnIVLine);

        EditorUtility.DisplayDialog("Success", "Syringe Administered Successfully.", "Ok"); // ! Done!!!


    }


    private void AddNewTag(Drug drugInside)
    {
        GameObject Tag;
        if (GameObject.FindGameObjectWithTag("Tag") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("Tag"));
            TagInitialPlaceholder.GetComponent<PlaceholderManagerScript>().IsEmpty = true;
        }

        Tag = Instantiate(TagPrefab) as GameObject; //creating object

        Tag.GetComponent<GeneralManagerScript>().Move = true;
        Tag.GetComponent<GeneralManagerScript>().Destination = TagInitialPlaceholder;
        Tag.GetComponent<TagManagerScript>().DrugName = drugInside.DrugName;
        Tag.GetComponent<TagManagerScript>().Volume = AmmountSlider.value;
        Tag.transform.position = TagInitialPlaceholder.transform.position;// giving it a position in order to find a distance in the search function
        Tag.transform.Find("FirstColor").gameObject.GetComponent<Renderer>().material.color = drugInside.PrimaryColor;
        Tag.transform.Find("SecondColor").gameObject.GetComponent<Renderer>().material.color = drugInside.SecondaryColor;
        Tag.transform.Find("TagText").gameObject.GetComponent<TextMesh>().text= drugInside.DrugName + " \n volume: " + AmmountSlider.value.ToString("F0") + " ml";

        //  throw new NotImplementedException();
    }

    // the few functions below are to used to keep track of the vial and syringe connected to the filling area
    public void DisconnectVial(GameObject vial)
    {
        if (GameObject.ReferenceEquals(VialInFillingArea, vial))
        {
            VialInFillingArea = null;
        }

    }

    public void ConnectVial(GameObject vial)
    {
        if (VialInFillingArea != null)//double check
        {
            if (!GameObject.ReferenceEquals(VialInFillingArea, vial))
            {
                EditorUtility.DisplayDialog("An Error Occured", "Please Contact the Developer", "Ok");// Issue a warning
                return;
            }
         }
        else
        VialInFillingArea = vial;
    }

    public void DisconnectFillAreaSyringe(GameObject Syringe)
    {
        if (GameObject.ReferenceEquals(SyringeInFillingArea, Syringe))
        {
            SyringeInFillingArea = null;
        }
    }

    public void DisconnectIVAreaSyringe(GameObject Syringe)
    {
        if (GameObject.ReferenceEquals(SyringeOnIVLine, Syringe))
        {
            SyringeOnIVLine = null;
        }
    }

    public void ConnectIVAreaSyringe(GameObject Syringe)
    {
        if (SyringeOnIVLine !=null && !GameObject.ReferenceEquals(SyringeOnIVLine, Syringe) )//double check
        {
            EditorUtility.DisplayDialog("An Error Occured", "Please Contact the Developer", "Ok");// Issue a warning
        }
        else
        SyringeOnIVLine = Syringe;
    }

    public void ConnectFillAreaSyringe(GameObject Syringe)
    {
        if (SyringeInFillingArea != null && !GameObject.ReferenceEquals(SyringeInFillingArea, Syringe))//double check
        {
            EditorUtility.DisplayDialog("An Error Occured", "Please Contact the Developer", "Ok");// Issue a warning
        }
        else
        SyringeInFillingArea = Syringe;
    }

}
