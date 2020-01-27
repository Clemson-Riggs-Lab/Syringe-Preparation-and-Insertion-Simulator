using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SyringeManagerScript : MonoBehaviour {
    //housekeeping
    public enum Status { New, OnFillingPosition, Filled , OnIVLine, Administered };
    public Status SyringeStatus { get; set; } // registers the current status of the syringe
    public bool IsTagged { get; set; } // registeres if a tag was placed on it
    public bool IsTrashed { get; set; } // registres if it was thrown away

    //contents
  
    public Drug DrugInside { get; set; }
    public float VolumeInside { get; set; }
    public string TagDrug { get; set; }
    public float TagVolume { get; set; }
    //timing
    public string TimeStartedFilling { get; set; }
    public string TimeFinishedFilling { get; set; }
    public string TimeAdminstred {get;  set;}
    public string TimeTrashed { get; set; }

    //areaAndplaceholders
     GameObject NewSyringePosition;
     GameObject FillingPlaceholder;

     GameObject TrashCan;
     GameObject IVLinePlaceholder;
     GameObject SceneManager;

    GameObject DefaultTransformObject;


    public float FillingPlaceholderVicinityThreshold;
    public float TrashCanVicinityThreshold;
    public float InitialPositionVicinityThreshold;
    public float IVLineVicinityThreshold;

    // Use this for initialization
    void Start () {
        SyringeStatus = Status.New;
        NewSyringePosition = GameObject.Find("NewSyringePlaceholder");
        FillingPlaceholder = GameObject.Find("SyringeFillingPlaceholder");
        TrashCan = GameObject.Find("TrashCan");
        IVLinePlaceholder = GameObject.Find("IVLinePlaceholder");
        SceneManager = GameObject.Find("SceneManager");
        DefaultTransformObject = GameObject.Find("SyringeDefaultTransformObject");
    }
	


   
    public void Dropped()
    {

        gameObject.GetComponent<GeneralManagerScript>().Move = true;

        if (InVicinityOf(ref NewSyringePosition, ref InitialPositionVicinityThreshold))
        {
            // if Initialposition is empty, and syringe is new
            if (NewSyringePosition.GetComponent<PlaceholderManagerScript>().IsEmpty == true && SyringeStatus == Status.New)
            {
                gameObject.GetComponent<GeneralManagerScript>().Destination = NewSyringePosition; // set destination to initial position
            }
        }

        else if (InVicinityOf(ref FillingPlaceholder, ref FillingPlaceholderVicinityThreshold))
        {
            

            if (FillingPlaceholder.GetComponent<PlaceholderManagerScript>().IsEmpty == false)
            {
                EditorUtility.DisplayDialog("Flow Violation", "A syringe is already in filling position.", "Ok"); // Issue warning  Cant return a syringe that isnt empty.
                return;
            }

            else if (SceneManager.GetComponent<SceneManagementScript>().VialInFillingArea == null)
            {
                EditorUtility.DisplayDialog("Flow Violation", "Connect a vial first.", "Ok"); // Issue warning  Cant return a syringe that isnt empty.
                return;
            }

            else if( SyringeStatus != Status.Administered)
            {
                SceneManager.GetComponent<SceneManagementScript>().FillButton.interactable = false;

                if (DrugInside== null)
                {
                    SyringeStatus = Status.OnFillingPosition;// set as on filling position
                    TimeStartedFilling = DateTime.Now.ToShortTimeString();
                    SceneManager.GetComponent<CsvOutputWrite>().AddEvent("SyringeMoved", this.gameObject);
                    SceneManager.GetComponent<SceneManagementScript>().FillButton.interactable =true ;
                }
      
                gameObject.GetComponent<GeneralManagerScript>().Destination = FillingPlaceholder;    // set destination
                SceneManager.GetComponent<SceneManagementScript>().ConnectFillAreaSyringe(this.gameObject);// allocating the fill area to this syringe
                

            }

            else
                EditorUtility.DisplayDialog("Flow Violation", "You cant return a syringe that is administered to the filling position.", "Ok"); // Issue warning  Cant return a syringe that isnt empty.
            
        }

        else if (InVicinityOf(ref IVLinePlaceholder, ref IVLineVicinityThreshold))
        {
            // if syringe is filled and the iv line is not occupied

            if(DrugInside == null)
            {
                EditorUtility.DisplayDialog("Flow Violation", "Syringe is empty.", "Ok");// Issue warning, 
                return;
            }

            else if(IVLinePlaceholder.GetComponent<PlaceholderManagerScript>().IsEmpty != true)
            {
                EditorUtility.DisplayDialog("Flow Violation", "IV line not empty.", "Ok");// Issue warning, 
                return;
            }

            else if(SyringeStatus == Status.Administered)
            {
                EditorUtility.DisplayDialog("Flow Violation", "Syringe already Administered.", "Ok");// Issue warning, 
                return;
            }
            else
            {
                gameObject.GetComponent<GeneralManagerScript>().Destination = IVLinePlaceholder; //setting the destination
                SceneManager.GetComponent<SceneManagementScript>().DisconnectFillAreaSyringe(this.gameObject);//deallocating the fill area if it was allocated to this syringe
                SceneManager.GetComponent<SceneManagementScript>().ConnectIVAreaSyringe(this.gameObject); //allocating the IV area to this syringe
                SyringeStatus = Status.OnIVLine;  //it is now on the IV line
            }     
        }

        else if (InVicinityOf(ref TrashCan, ref TrashCanVicinityThreshold))
        {
          
            //if it was trashhed before being adminstered (discarded), we note that
            if (SyringeStatus != Status.Administered)
            {
                IsTrashed = true;
            }

            SceneManager.GetComponent<SceneManagementScript>().AddNewSyringe();//if is new: add new syringe to new syringes placeholder

            TimeTrashed = DateTime.Now.ToShortTimeString();
            //TODO: Trash
            SceneManager.GetComponent<CsvOutputWrite>().AddEvent("SyringeTrashed", this.gameObject);
            SceneManager.GetComponent<SceneManagementScript>().RemoveObject(this.gameObject);
        }
    }


    public void Retransform()
    {
        this.gameObject.transform.localScale = DefaultTransformObject.transform.localScale;
        this.gameObject.transform.rotation = DefaultTransformObject.transform.rotation;

    }

    //please notice that this assumes the vicinity along two axis
    //this might be inconvinient for some cases.
    //you can change this code to check for the distance along all axis
    public bool InVicinityOf(ref GameObject placeholder,ref float VicinityThreshold)
    {
        Vector2 obj1 = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        Vector2 obj2 = new Vector2(placeholder.transform.position.x, placeholder.transform.position.y);
        // if the object is within the vicinity threshold of the placeholder return true
        if (Vector2.Distance(obj1, obj2) < VicinityThreshold)
            return true;
        else
            return false;

    }
}
