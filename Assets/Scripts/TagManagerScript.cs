using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TagManagerScript : MonoBehaviour {

    public TextMesh textMeshReference { get; set; }

    GameObject FillingPlaceholder;
    GameObject SceneManager;
    //GameObject NewTagPlaceholder;
    public float VicintyThreshold;
    public string DrugName;
    public float Volume;
    // Use this for initialization
    void Start () {
        textMeshReference = gameObject.GetComponentInChildren<TextMesh>();
        FillingPlaceholder = GameObject.Find("SyringeFillingPlaceholder");
        SceneManager = GameObject.Find("SceneManager");
        //NewTagPlaceholder = GameObject.Find("NewTagPlaceholder");

    }

    // Update is called once per frame

    public void Dropped()
    {
        gameObject.GetComponent<GeneralManagerScript>().Move = true;

       if (InVicinityOf(ref FillingPlaceholder, ref VicintyThreshold))
        {
            // filling placeholder is empty + syringe is new
            if (FillingPlaceholder.GetComponent<PlaceholderManagerScript>().IsEmpty == true)
            {
                EditorUtility.DisplayDialog("Flow Violation", "Drag a syringe to tag it .", "Ok"); // Issue warning  Cant return a syringe that isnt empty.
                return;
            }
            GameObject SyringeFilled = SceneManager.GetComponent<SceneManagementScript>().SyringeInFillingArea;


            this.gameObject.transform.SetParent(SyringeFilled.transform);
            gameObject.GetComponent<GeneralManagerScript>().Destination = SyringeFilled.transform.Find("TagPlaceholder").gameObject;    // set destination
            SceneManager.GetComponent<SceneManagementScript>().ApplyTag(this.gameObject);

           

        }


    }


    public bool InVicinityOf(ref GameObject placeholder, ref float VicinityThreshold)
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
