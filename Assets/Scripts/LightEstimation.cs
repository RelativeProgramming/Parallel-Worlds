using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class LightEstimation : MonoBehaviour
{
    public ARCameraManager ARCameraManager;

    private Light Light;


    private void Awake()
    {
        Light = GetComponent<Light>();
    }

    private void OnEnable()
    {
        ARCameraManager.frameReceived += FrameUpdated;
    }

    private void OnDisable()
    {
        ARCameraManager.frameReceived -= FrameUpdated;
    }

    private void FrameUpdated(ARCameraFrameEventArgs args)
    {
        if(args.lightEstimation.averageBrightness.HasValue)
        {
            Light.intensity = args.lightEstimation.averageBrightness.Value;
        }

        if (args.lightEstimation.mainLightDirection.HasValue)
        {
            transform.rotation = Quaternion.Euler(args.lightEstimation.mainLightDirection.Value);
        }

        if (args.lightEstimation.averageColorTemperature.HasValue)
        {
            Light.colorTemperature = args.lightEstimation.averageColorTemperature.Value;
        }

        if (args.lightEstimation.colorCorrection.HasValue)
        {
            Light.color = args.lightEstimation.colorCorrection.Value;
        }
    }
}
