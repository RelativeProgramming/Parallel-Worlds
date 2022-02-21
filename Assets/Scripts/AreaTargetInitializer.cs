using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class AreaTargetInitializer : MonoBehaviour
{
    public Transform StartPosition;
    public float StartPositionRadius;

    private AreaTargetBehaviour AreaTarget;

    private void Awake()
    {
        AreaTarget = GetComponent<AreaTargetBehaviour>();
    }

    // Start is called before the first frame update
    void Start()
    {
        AreaTarget.SetExternal2DPosition(new Vector2(StartPosition.position.x, StartPosition.position.z), StartPositionRadius);
    }
}
