using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARUserTracker : MonoBehaviour
{
    void Start()
    {
        SessionManager.Instance.User.Camera = transform;
    }
}
