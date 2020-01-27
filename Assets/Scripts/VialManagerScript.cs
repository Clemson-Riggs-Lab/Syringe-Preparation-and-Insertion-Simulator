using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VialManagerScript : MonoBehaviour {
    public Drug DrugInside { get; set; }

     GameObject FillingPlaceholder;
    public GameObject RemoveVialArea;

     GameObject SceneManager;

    public float FillingPlaceholderVicinityThreshold;
    public float RemoveVialAreaVicinityThreshold;

    // Use this for initialization
    void Start () {
        FillingPlaceholder = GameObject.Find("VialFillingPlaceholder");
        RemoveVialArea = GameObject.Find("RemoveVialArea");
        SceneManager = GameObject.Find("SceneManager");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Dropped()
    {
        gameObject.GetComponent<GeneralManagerScript>().Move = true;

        if (InVicinityOf(FillingPlaceholder, ref FillingPlaceholderVicinityThreshold))
        {
            if (FillingPlaceholder.GetComponent<PlaceholderManagerScript>().IsEmpty == true)
            {
                gameObject.GetComponent<GeneralManagerScript>().Destination = FillingPlaceholder;
                SceneManager.GetComponent<SceneManagementScript>().ConnectVial(this.gameObject);
            }

            else
                EditorUtility.DisplayDialog("Flow Violation", "A vial is already connected to this position ", "Ok");// Issue warning  
        }

        else if (InVicinityOf(RemoveVialArea, ref RemoveVialAreaVicinityThreshold))
        {
            SceneManager.GetComponent<CsvOutputWrite>().AddEvent("VialRemoved", this.gameObject);
            SceneManager.GetComponent<SceneManagementScript>().RemoveObject(this.gameObject);
            // TODO: remove syringe
        }




    }

    public bool InVicinityOf( GameObject placeholder, ref float VicinityThreshold)
    {
        if (Vector2.Distance(gameObject.transform.position, placeholder.transform.position) < VicinityThreshold)
            return true;
        else
            return false;
    }
}
