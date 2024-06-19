using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    //Make sure to attach these Buttons in the Inspector
    public Button ButtonRestart;
    public Button ButtonEdit;
    public Button ButtonStart;
    public CameraMovement cameraMovement;
    public Canvas canvas;
    public Canvas canvasEdit;
    public GameObject wall;
    public Vector3 wallLoc = new Vector3(-30.1f, 11.90701f, -26.5f);
    public Button ButtonWake;
    public OnContactWithBelt OnContactWithBelt;

    void Start()
    {
        //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
        ButtonRestart.onClick.AddListener(() => TaskOnClick(ButtonRestart.name));
        ButtonEdit.onClick.AddListener(() => TaskOnClick(ButtonEdit.name));
        ButtonStart.onClick.AddListener(() => TaskOnClick(ButtonStart.name));
        ButtonWake.onClick.AddListener(() => TaskOnClick(ButtonWake.name));
        cameraMovement = FindObjectOfType<CameraMovement>();
        OnContactWithBelt = FindObjectOfType<OnContactWithBelt>();
    }

    void TaskOnClick(string ButtonName)
    {
        if (ButtonName == ButtonRestart.name)
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        else if (ButtonName == ButtonEdit.name)
        {
            canvas.transform.position = new Vector3(150, 10, -28); 
            canvasEdit.transform.position = new Vector3(-23, 10, -28);
            wall.transform.position = new Vector3(150, 0, 0);
        }
        else if (ButtonName == ButtonStart.name)
        {
            cameraMovement.Tab();
            Cursor.visible = false;
        }
        else if (ButtonName == ButtonWake.name)
        {
            OnContactWithBelt.SimStart();
        }
    }
}
