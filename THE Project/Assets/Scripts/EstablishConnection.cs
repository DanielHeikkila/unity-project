using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class EstablishConnection : MonoBehaviour
{
    //Make sure to attach these Buttons in the Inspector
    public Button Button;
    public Button ButtonClear;
    public Button ButtonEdit;
    public Button ButtonAdd;
    public Button ButtonBack;
    public GameObject Crate;
    public CameraMovement cameraMovement;
    public Canvas canvas;
    public Canvas canvasMain;
    public GameObject wall;
    public Vector3 wallLoc = new Vector3(-30.1f, 11.90701f, -26.5f);

    void Start()
    {
        //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
        Button.onClick.AddListener(() => TaskOnClick(Button.name));
        //m_YourSecondButton.onClick.AddListener(delegate { TaskWithParameters("Hello"); });
        //m_YourThirdButton.onClick.AddListener(() => ButtonClicked(42));
        ButtonClear.onClick.AddListener(() => TaskOnClick(ButtonClear.name));
        ButtonEdit.onClick.AddListener(() => TaskOnClick(ButtonEdit.name));
        ButtonAdd.onClick.AddListener(() => TaskOnClick(ButtonAdd.name));
        ButtonBack.onClick.AddListener(() => TaskOnClick(ButtonBack.name));
        cameraMovement = FindObjectOfType<CameraMovement>();
    }

    void TaskOnClick(string ButtonName)
    {
        if (ButtonName == Button.name)
        {
            Debug.Log("You have clicked the button!");
            Vector3 spawnPos = new Vector3(-45.5f, 1, 15.4f);
            Instantiate(Crate, spawnPos, Crate.transform.rotation);
        }
        else if (ButtonName == ButtonClear.name)
        {
            GameObject[] crates = GameObject.FindGameObjectsWithTag("Crate");
            foreach (GameObject crate in crates)
            {
                Destroy(crate);
            }
        }
        else if (ButtonName == ButtonEdit.name)
        {
            cameraMovement.toggleEditing();
            cameraMovement.Tab();
            wall.transform.position = wallLoc;
        }
        else if (ButtonName == ButtonAdd.name)
        {
            cameraMovement.addBelt();
            cameraMovement.Tab();
            cameraMovement.SetLayerRecursively(cameraMovement.ball, LayerMask.NameToLayer("Ignore Raycast"));
            wall.transform.position = wallLoc;
        }
        else if (ButtonName == ButtonBack.name)
        {
            canvas.transform.position = new Vector3(150, 10, - 28);
            canvasMain.transform.position = new Vector3(-23, 10, -28);
            wall.transform.position = wallLoc;
        }
    }
}
