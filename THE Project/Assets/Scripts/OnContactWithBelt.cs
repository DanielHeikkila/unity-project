using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class OnContactWithBelt : MonoBehaviour
{
    public float speed;
    public bool isOnBelt = false;
    public List<GameObject> crates = new List<GameObject>();
    public Vector3 direction;
    public float halfspeed = 1.0f; 
    public Animator armAnimator;
    public GameObject platform;
    public bool isInHold;
    public GameObject crateHeld;
    public GameObject armHold;

    void Start()
    {
        speed = 2.0f;
        direction = new Vector3(0, 0, 0);
        isInHold = false;
        crateHeld = null;
        armHold = null;
        armAnimator = GameObject.Find("INDUSTRIAL ROBOTIC ARM").GetComponent<Animator>();
        platform = GameObject.Find("Cube");
}

    void Update()
    {
        if (isOnBelt)
        {
            foreach (GameObject crate in crates)
            {
                Rigidbody rb = crate.GetComponent<Rigidbody>();
                Vector3 horizontalVelocity = direction * speed;
                rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
                rb.angularDrag = 10;
            }
        }
        if (isInHold == true && armAnimator.GetBool("boxDropped") == false)
        {
            BoxCollider armRealPos = armHold.GetComponent<BoxCollider>();
            crateHeld.gameObject.transform.position = armRealPos.transform.position + new Vector3(0, 1, 0);
            crateHeld.gameObject.transform.rotation = armRealPos.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.CompareTag("Crate") && other.CompareTag("belts") || this.CompareTag("Crate") && other.CompareTag("Top"))
        {
            direction = direction + other.gameObject.transform.right;
            GameObject crate = this.gameObject;
            crates.Add(crate);
            isOnBelt = true;
        }
        if (this.CompareTag("boxSpawn"))
        {
            armAnimator.SetBool("boxOnPlatform", true);
            armAnimator.SetBool("boxDropped", false);
        }
        if (this.CompareTag("Crate") && other.CompareTag("arm"))
        {
            crateHeld = this.gameObject;
            armHold = other.gameObject;
            isInHold = true;
        }
        if (this.CompareTag("Crate") && other.CompareTag("fall") || this.CompareTag("fall") && other.CompareTag("Crate"))
        {
            Stop(crateHeld.GetComponent<Rigidbody>());
            crateHeld = null;
            armHold = null;
            isInHold = false;
            armAnimator.SetBool("boxOnPlatform", false);
            armAnimator.SetBool("boxDropped", true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("belts") || other.CompareTag("Top"))
        {
            if (direction == new Vector3(0, 0, 0))
            {
                direction = direction + other.gameObject.transform.right;
            }
            if (this.CompareTag("Crate"))
            {
                GameObject crate = this.gameObject;
                if (!crates.Contains(crate))
                {
                    crates.Add(crate);
                }
                isOnBelt = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.CompareTag("Crate") && !other.CompareTag("arm") && !other.CompareTag("fall") && !other.CompareTag("boxSpawn"))
        {
            direction = direction - other.gameObject.transform.right;
            GameObject crate = this.gameObject;
            Rigidbody rb = crate.GetComponent<Rigidbody>();
            rb.angularDrag = 1;
            crates.Remove(crate);
            if (crates.Count == 0)
            {
                isOnBelt = false;
            }
        }
    }

    public void Stop(Rigidbody x)
    {
        x.velocity = new Vector3(0, 0, 0);
    }

    public void SimStart()
    {
        armAnimator.SetBool("simStarted", true);
    }
}
