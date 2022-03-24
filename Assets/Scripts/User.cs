using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public Transform Camera;
    public Vector3 cameraOffset;

    private RealtimeView RealtimeView;
    private SessionManager SessionManager;

    private void Awake()
    {
        RealtimeView = GetComponent<RealtimeView>();
    }

    private void Start()
    {
        SessionManager = SessionManager.Instance;
    }


    private void Update()
    {
        if(Camera != null)
        {
            transform.position = Camera.transform.position;
            transform.rotation = Camera.transform.rotation;
        }
        //if (RealtimeView.isOwnedLocallySelf)
        //{
        //    CalculateTargetMovement();
        //}
    }

    public void ThrowFood()
    {
        var sim = GameObject.FindObjectOfType<Simulation>();
        if(sim != null)
        {
            GameObject acorn = SessionManager.InstantiateRealtimePrefab("Acorn");
            acorn.transform.position = transform.position;
            acorn.transform.Rotate(new Vector3(Random.Range(1f, 89f), 0, 0));
            acorn.GetComponent<Rigidbody>().velocity = transform.rotation * Vector3.forward * 5;
            acorn.GetComponent<FoodItem>().Creator = SessionManager.Realtime.clientID.ToString();
        } else
        {
            Debug.Log("No simulation object found!");
        }
        
    }

    // only for testing purposes on PC
    private void CalculateTargetMovement()
    {
        Vector3 inputMovement = new Vector3();
        inputMovement.x = Input.GetAxisRaw("Horizontal") * 6.0f;
        inputMovement.z = Input.GetAxisRaw("Vertical") * 6.0f;
        transform.position += inputMovement * Time.deltaTime;
    }

    //private void LateUpdate()
    //{
    //    if (Camera != null && RealtimeView.isOwnedLocallySelf)
    //    {
    //        Camera.transform.position = transform.position + cameraOffset;
    //        Camera.transform.LookAt(transform.position);
    //    }
    //}
}
