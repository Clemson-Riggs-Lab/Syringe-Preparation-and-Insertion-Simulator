using UnityEngine;
using System.Collections;
using System;

public class PopupWindow : MonoBehaviour
{
    //// 200x300 px window will apear in the center of the screen.
    //// private Rect windowRect = new Rect((Screen.width - 400) / 2, (Screen.height - 600) / 2, 400, 400);
    //// Only show it if needed.
    ////setting window on the buttom
    //private Rect windowRect = new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 350);
    //private bool show = false;
    //private string Message;
    //private bool FirstOptionShow;
    //private GUIStyle currentStyle=null;

    //void OnGUI()
    //{


    //    //GUI.backgroundColor = Color.green;
    //    GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 25;
    //    GUI.skin.label.alignment = TextAnchor.MiddleCenter;

    //    if (show)
    //    windowRect = GUI.Window(0, windowRect, DialogWindow,"");
    //}

    //// This is the actual window.
    //void DialogWindow(int windowID)
    //{
    //    float y = 20;
    //    GUI.Label(new Rect(0, y, windowRect.width - 20, 80), Message);
    //    if (FirstOptionShow)
    //    if (GUI.Button(new Rect(5, y+85, windowRect.width - 20, 40), "OK"))
    //    {
    //            show = false;
    //    }
    //}

    //// To open the dialogue from outside of the script.
    //public void Open(string message )
    //{
    //    Message = message;
    //}

}