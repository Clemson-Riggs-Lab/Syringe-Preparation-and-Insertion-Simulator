//TouchPhysics Script
//Used for a Grabing/Dragging/Dropping Objects using a physic-ness type of Fixed Joint

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TouchPhysics : MonoBehaviour {
	
	//your main camera, this Variable is automatically set 
	private Camera MainCamera;

    //this is your virtual hand in the game, edit the Fixed Joint componet on this object to adjust the way you grab stuff
    public GameObject TouchObject;

	//Types of Inputs that are used
	public enum InputTypes{Touches = 0, Mouse = 1};
	public InputTypes InputType = InputTypes.Mouse;

	//the MouseButton that will be used to detect a an interaction with an object
	public int MouseButton = 0;

	//AutoTouchDistance will allow keep the object at it's "Z" position
	public bool AutoTouchDistance = true;

	//if AutoTouchDistance is false the object will move to this "Z" position when touched
	public float TouchDistance = 10f;

	//Exclusion Tags!
	public List<string> ExclusionTags = new List<string>();

	//these List keep track of stuff
	private List<GameObject> TouchObjects = new List<GameObject>();
	private List<GameObject> Objects = new List<GameObject>();
	private List<float> TouchDistances =  new List<float>();

    public GameObject SceneManager;

    //find main Camera
    void Start () 
	{
		MainCamera =  Camera.main;
	}
	
	// Update is called once per frame
	void Update () 
	{

		//if we are only processing Touches
		if (
			InputType == InputTypes.Touches
			)
		{
			//for each Touch
			Touch[] Touches = Input.touches;
			for(int i = 0; i < Input.touchCount; i++)
			{
				if (Touches[i].phase == TouchPhase.Began )
				{
					//if Touch began pickup/grab object
					Grab(Touches[i].position);
				}
				else if (Touches[i].phase == TouchPhase.Ended )
				{
					//if Touch end drop object
					Drop(i);
				}
				else
				{
					//else drag Object
					Drag(i, Touches[i].position);
				}
			}

		}

		//if we are only processing the Mouse
		if (
			InputType == InputTypes.Mouse
			)
		{
			if (Input.GetMouseButtonDown(MouseButton))
			{
				//if Click began pickup/grab object
				Grab(Input.mousePosition);
			}
			else if (Input.GetMouseButtonUp(MouseButton))
			{
				//if Click end drop object
				if (TouchObjects.Count >= 1)
				{
					Drop(0);
				}

			}
			else if (Input.GetMouseButton(MouseButton))
			{
				//if drag object
				if (TouchObjects.Count >= 1)
				{
					Drag(0, Input.mousePosition);
				}
			}
		}



	}

	//Grab requires the position of the touch or mouse
	private void Grab(Vector2 InterfacePosition)
	{
		//we will be using a raycast to detect objects are being touched
		Ray screenRay = MainCamera.ScreenPointToRay(InterfacePosition);
		RaycastHit RCH;
		if (Physics.Raycast(screenRay,out RCH))
		{
			//if the touched object is in the Exclusion Tags list exit this method and do nothing.
			if (ExclusionTags.Contains(RCH.collider.gameObject.tag))
			{
				return;
			}

			//check if the object has already been touched
			bool AlreadyTouched = false;
            if (RCH.collider.gameObject.tag == "Vial" || RCH.collider.gameObject.tag == "Syringe" || RCH.collider.gameObject.tag == "Tag")
            {
                if (RCH.collider.gameObject.tag == "Vial")
                {
                    SceneManager.GetComponent<DisplayTextScript>().SetVialInfo(RCH.collider.gameObject);
                }
                else if(RCH.collider.gameObject.tag == "Syringe")
                {
                    SceneManager.GetComponent<DisplayTextScript>().SetSyringeInfo(RCH.collider.gameObject);
                }
                
                foreach (GameObject Obj in Objects)
                {
                    print(RCH.collider.gameObject.name);

                    if (Obj == RCH.collider.gameObject)
                    {
                        AlreadyTouched = true;
                    }
                }


                //make sure the object has a rigidbody, and it has not already been touched
                if (
                    RCH.collider.gameObject.GetComponent<Rigidbody>() != null && !AlreadyTouched
                    )
                {

                    Vector3 WorldPosition = screenRay.GetPoint(RCH.distance);

                    if (AutoTouchDistance)
                    {
                        // record the Z position of the object
                        TouchDistances.Add(RCH.distance);
                    }

                    //create the TouchObject
                    GameObject TO = Instantiate(TouchObject, WorldPosition, Quaternion.Euler(new Vector3(0f, 0f, 0f))) as GameObject;
                    RCH.collider.gameObject.GetComponent<GeneralManagerScript>().Disconnect();
                    //bind the Object to the Fixed Joint of the touch object
                    TO.GetComponent<FixedJoint>().connectedBody = RCH.collider.gameObject.GetComponent<Rigidbody>();

                    //record the TouchObject and the Object being interacted with
                    TouchObjects.Add(TO);
                    Objects.Add(RCH.collider.gameObject);

                }
            }
		}
	}

	//Drab requires the ItemNum and position of the touch or mouse
	private void Drag(int ItemNum, Vector2 InterfacePosition)
	{
		//Screen Point to Ray to get the Ray (this will be used to get the worldPosition)
		Ray screenRay = MainCamera.ScreenPointToRay(InterfacePosition);

		Vector3 WorldPosition;

		//if AutoTouchDistance use that for the Z position
		if (AutoTouchDistance)
		{
			WorldPosition = screenRay.GetPoint(TouchDistances[ItemNum]);
		}
		else //else use the TouchDistance
		{
			WorldPosition = screenRay.GetPoint(TouchDistance);
		}

		//Move the TouchObject to the new position
		TouchObjects[ItemNum].transform.position = WorldPosition ; 
	}

	//Drop requires the ItemNum
	private void Drop(int ItemNum)
	{
        //Remove the rigidbody from the Fixed Joint
        TouchObjects[ItemNum].GetComponent<FixedJoint>().connectedBody = null;

		//Store the TouchObject in Temp
		GameObject Temp  = TouchObjects[ItemNum];
		//Remove the Touch Object from the List
		TouchObjects.Remove(TouchObjects[ItemNum]);
		//Destory the TouchObject
		Destroy(Temp);

        Objects[ItemNum].GetComponent<GeneralManagerScript>().Dropped();
        //remove Object from List
        Objects.RemoveAt(ItemNum);

		//remove the TouchDistance
		if (AutoTouchDistance)
		{
			TouchDistances.RemoveAt(ItemNum);
		}
	}



}
