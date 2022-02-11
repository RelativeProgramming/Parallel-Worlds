using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ServerCamera : MonoBehaviour
{
    public static Camera Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = GetComponent<Camera>();
        }
    }
}
