using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeneralManagerScript : MonoBehaviour {
    public enum Type { Syringe, Vial, Tag }; // restricts the types to two ( syringe or vial)
    public Type type;
    public float speed; // the speed at which the object moves to its destination
    public float VicinityThreshold;  // this intger specifies the maximum distance between an object and its destination at which it is  assumed to be in its vicinity


    public bool Move { get; set; }// this boolean specifies weather the object should move to a destination or not
    public bool IsConnected { get; set; } // the object is connected with a fixed joint to a placeholder
    public GameObject Destination { get; set; } // this variable holds the transform of the object that this object is moving towards
    public GameObject Placeholder { get; set; } // this variable holds the Gameobject of the placeholder that this object is connected to
    FixedJoint fixedJoint;

    // Use this for initialization
    void Start () {

        fixedJoint = null;
	}
	
	// Update is called once per frame
	void Update () {

        if( Move==true) // object should be moving towards a destination
        {
            if( IsConnected==true) // if the object is connected to a placeholder
            {
                Disconnect();
            }

            if (Destination != null)
            {
                // if the object is close to its destination
                if (InVicinity(Destination)== true)
                {
                    // try to attach to the destination. If you could, stop the movement, and the placeholder is updated, and the destination is nullified
                    if (TryAttachTo(Destination)==true)
                    {
                        Placeholder = Destination;
                        Destination = null; 
                        Move = false;
                    }
                    //else, move back to initial placeholder
                    else
                        Destination = Placeholder;

                }

                // if the object is still far from its destination
                else
                {
                    // move to the destination along a straight line
                    float step = speed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, Destination.transform.position, step);
                }
            }
            else
            {
                Destination = Placeholder;
            }

        }



	}

    // this function gives applies the transform of the destination on the object.
    //and tries to attach the gameobject to its destination with a fixed joint.
    //in case of an error, it returns false;
    private bool TryAttachTo(GameObject destination)
    {
        try
        {
            this.gameObject.transform.position = destination.transform.position;
            this.gameObject.transform.localScale = destination.transform.localScale * 1.1f;
            this.gameObject.transform.rotation = destination.transform.rotation;
            if (fixedJoint != null)
            {
                Destroy(fixedJoint);
            }
            this.gameObject.AddComponent<FixedJoint>();
            this.gameObject.GetComponent<FixedJoint>().connectedBody = destination.GetComponent<Rigidbody>();
            fixedJoint = this.gameObject.GetComponent<FixedJoint>();
            destination.GetComponent<PlaceholderManagerScript>().IsEmpty = false;
            IsConnected = true;


            return true;
        }

        // in case of an error
        catch
        {
            //EditorUtility.DisplayDialog("An Error Occured", "the object couldnt attach to its destination. this shouldnt happen. contact the developer", "Ok");
            return false;
        }
    }

    //please notice that this assumes the vicinity along two axis.
    //this might be inconvinient for some cases.
    //you can change this code to check for the distance along all
    private bool InVicinity(GameObject destination)
    {
        Vector2 obj1 = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        Vector2 obj2 = new Vector2(destination.transform.position.x, destination.transform.position.y);
        // if the object is within the vicinity threshold of the placeholder return true
        if (Vector2.Distance(obj1, obj2) < VicinityThreshold)
            return true;
        else
            return false;
    }

    // this is called when touchphsics grabs an object, and when the object is supposed to move because of a command, but it is still connected to a placholder
    public void Disconnect()
    {

        if (type == Type.Syringe)
            this.gameObject.GetComponent<SyringeManagerScript>().Retransform();

        // if the object has a fixed joint, the fixed joint is deleted
        if (fixedJoint!=null)
        {
            Destroy(fixedJoint);
            Placeholder.GetComponent<PlaceholderManagerScript>().IsEmpty = true; // setting the placeholder to empty for it to be accessable to other objects.

        }
        IsConnected = false; // the object is detached from its placeholder if connected


    }

    public void Dropped()
    {

        if (this.type == Type.Syringe)
        {
            this.gameObject.GetComponent<SyringeManagerScript>().Dropped();

        }

        else if (this.type == Type.Vial)
        {
            this.gameObject.GetComponent<VialManagerScript>().Dropped();
        }

        else if (this.type == Type.Tag)
        {
            this.gameObject.GetComponent<TagManagerScript>().Dropped();
        }

        else
            throw new NotSupportedException(" the type of this object is neither syringe nor vial. this shouldnt happen. please check the code");

    }
}
