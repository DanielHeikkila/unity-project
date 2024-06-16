using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class CameraMovement : MonoBehaviour
{
    public float turnSpeed;
    public float speed;
    public float horizontalInput;
    public float verticalInput;
    public float sensitivity = 5;
    private bool isCursorLocked = true;
    public Camera mainCamera;
    public Camera uiCamera;
    public Canvas canvas;
    public LayerMask layerMask; // The layer(s) to include in the raycast.
    public LayerMask layerMaskEdit;
    public float maxDistance = 100f; // Maximum distance for the raycast.
    public GameObject hitMarkerPrefab; // Prefab for visualizing the hit point.
    public GameObject ball;
    public bool editing;
    public int belts;
    public bool addingItem;
    public bool tab;
    private float verticalRotation = 0f;
    public GameObject wall;
    public Vector3 wallLoc = new Vector3(-30.1f, 11.90701f, -26.5f);
    public float rotationSpeed = 50f;
    float mouseX;
    float mouseY;
    Vector3 translation;
    GameObject obj = null;

    // Start is called before the first frame update
    void Start()
    {
        turnSpeed = 100.0f;
        speed = 10.0f;
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        ball = Instantiate(hitMarkerPrefab, new Vector3(50, 0, 50), Quaternion.identity);
        editing = false;
        belts = 1;
        addingItem = false;
        tab = false;
        verticalRotation = transform.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || tab)
        {
            tab = false;
            isCursorLocked = !isCursorLocked;
            if (isCursorLocked)
            {
                LockCursor();
                wall.transform.position = wallLoc;
            }
            else
            {
                UnlockCursor();
                wall.transform.position = new Vector3(150, 0, 0);
            }
        }

        if (isCursorLocked)
        {

            mouseX = Input.GetAxis("Mouse X") * sensitivity;

            mouseY = -Input.GetAxis("Mouse Y") * sensitivity;

            transform.Rotate(Vector3.up, mouseX * turnSpeed * 0.02f);

            verticalRotation += mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
            transform.localEulerAngles = new Vector3(verticalRotation, transform.localEulerAngles.y, 0);


            transform.Translate(translation * speed * 0.01f, Space.World);

            // Translate camera based on input
            verticalInput = Input.GetAxis("Vertical") * Time.deltaTime * 100;
            horizontalInput = Input.GetAxis("Horizontal") * Time.deltaTime * 100;
            translation = transform.forward * verticalInput + transform.right * horizontalInput;

            // Ensure camera stays at fixed height
            transform.position = new Vector3(transform.position.x, 9, transform.position.z);
            if (addingItem == true)
            {
                ball.transform.rotation = new Quaternion(0, ball.transform.rotation.y, 0, ball.transform.rotation.w);
                // Create a ray from the camera through the mouse cursor position.
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
                {
                    ball.transform.position = hitInfo.point;
                }
                if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMaskEdit))
                {
                    ball.transform.position = hitInfo.point;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (!ball.CompareTag("Crate"))
                    {
                        fix(ball);
                    }
                    SetLayerRecursively(ball, LayerMask.NameToLayer("based"));
                    addingItem = false;
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    ball.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.E))
                {
                    ball.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                }
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    ball.transform.localScale += ball.transform.localScale * -0.01f;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    ball.transform.localScale += ball.transform.localScale * 0.01f;
                }
            }
            if (editing == true)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMaskEdit) && obj == null)
                {
                    obj = hitInfo.collider.gameObject;
                    ChangeColor(hitInfo.collider.gameObject, new Color(0, 204, 102));
                }
                else if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMaskEdit) && obj != hitInfo.collider.gameObject)
                {
                    ChangeColor(obj, new Color(0, -204, -102));
                    ChangeColor(hitInfo.collider.gameObject, new Color(0, 204, 102));
                    obj = hitInfo.collider.gameObject;
                }
                if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMaskEdit) && hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    ChangeColor(obj, new Color(0, -204, -102));
                    obj = null;
                }
                if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMaskEdit) && Input.GetMouseButtonDown(0) && hitInfo.collider.gameObject.layer != LayerMask.NameToLayer("Floor") && hitInfo.collider.gameObject.tag != "Top")
                {
                    ChangeColor(obj, new Color(0, -204, -102));
                    ball = hitInfo.collider.gameObject;
                    addingItem = true;
                    editing = false;
                    free(ball);
                    SetLayerRecursively(ball, LayerMask.NameToLayer("Ignore Raycast"));
                    obj = null;
                }
                else if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMaskEdit) && Input.GetMouseButtonDown(0) && hitInfo.collider.gameObject.tag == "Top")
                {
                    ChangeColor(obj, new Color(0, -204, -102));
                    ball = hitInfo.collider.gameObject.transform.parent.gameObject;
                    addingItem = true;
                    editing = false;
                    free(ball);
                    SetLayerRecursively(ball, LayerMask.NameToLayer("Ignore Raycast"));
                    obj = null;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isCursorLocked)
        {
        }

    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mainCamera.enabled = true;
        uiCamera.enabled = false;
        canvas.enabled = false;
    }

    public void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mainCamera.enabled = false;
        uiCamera.enabled = true;
        canvas.enabled = true;
    }

    public void toggleEditing()
    {
        editing = !editing;
    }

    public void addBelt()
    {
        belts++;
        ball = Instantiate(hitMarkerPrefab, new Vector3(50 + (belts * 10), 0, 50 + (belts * 10)), Quaternion.identity);
        addingItem = true;
    }

    public void fix(GameObject g)
    {
        Rigidbody rb = g.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void free(GameObject g)
    {
        Rigidbody rb = g.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    void ChangeColor(GameObject target, Color colorChange)
    {
        // Apply color change to the target object itself
        Renderer targetRenderer = target.GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            targetRenderer.material.color += colorChange;
        }

        // Apply color change to all child objects
        foreach (Renderer childRenderer in target.GetComponentsInChildren<Renderer>())
        {
            childRenderer.material.color += colorChange;
        }
    }

    Quaternion ClampRotation(Quaternion rotation, float minAngle, float maxAngle)
    {
        rotation.eulerAngles = new Vector3(
            ClampAngle(rotation.eulerAngles.x, minAngle, maxAngle),
            rotation.eulerAngles.y,
            rotation.eulerAngles.z
        );
        return rotation;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }


    public void Tab()
    {
        tab = true;
    }
}
