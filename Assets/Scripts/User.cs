using Normal.Realtime;
using UnityEngine;

public class User : RealtimeComponent<UserModel>
{
    public Transform Camera;
    public Vector3 cameraOffset;

    private SessionManager SessionManager;

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
            // sim.AddUser(SystemInfo.deviceUniqueIdentifier);
            GameObject acorn = SessionManager.InstantiateRealtimePrefab("Acorn");
            acorn.transform.position = transform.position;
            acorn.transform.Rotate(new Vector3(Random.Range(1f, 89f), 0, 0));
            acorn.GetComponent<Rigidbody>().velocity = transform.rotation * Vector3.forward * 5;
            acorn.GetComponent<FoodItem>().SetCreator(SystemInfo.deviceUniqueIdentifier);
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

    //protected override void OnRealtimeModelReplaced(UserModel prevModel, UserModel newModel)
    //{
    //    Debug.Log("User-Model: " + newModel.username);
    //}

    public void SetUsername()
    {
        model.username = SystemInfo.deviceUniqueIdentifier;
    }

    public string GetUsername()
    {
        return model.username;
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
