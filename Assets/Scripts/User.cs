using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    // Camera
    public Camera Camera;
    public Vector3 cameraOffset;

    private RealtimeView RealtimeView;

    private void Awake()
    {
        RealtimeView = GetComponent<RealtimeView>();
    }


    private void Update()
    {
        if (RealtimeView.isOwnedLocallySelf)
        {
            CalculateTargetMovement();
        }
    }

    private void CalculateTargetMovement()
    {
        Vector3 inputMovement = new Vector3();
        inputMovement.x = Input.GetAxisRaw("Horizontal") * 6.0f;
        inputMovement.z = Input.GetAxisRaw("Vertical") * 6.0f;
        transform.position += inputMovement * Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (RealtimeView.isOwnedLocallySelf)
        {
            Camera.transform.position = transform.position + cameraOffset;
            Camera.transform.LookAt(transform.position);
        }
    }
}
