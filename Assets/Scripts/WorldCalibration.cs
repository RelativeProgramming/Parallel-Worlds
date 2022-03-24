using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class WorldCalibration : MonoBehaviour
{
    public ARSessionOrigin arSessionOrigin;
    public List<GameObject> trackingPoints;
    public GameObject trackingMarker;
    public XRReferenceImageLibrary referenceImageLibrary;

    public Button btnCalibrate;
    public Image btnCalibrateImage;
    public Sprite btnCalibrateSprite;
    public Sprite btnRecalibrateSprite;
    public Text debugInfoText;

    private ARTrackedImageManager trackedImageManager;

    private Dictionary<string, Vector3> lastTrackedPositions = new Dictionary<string, Vector3>();
    private Dictionary<string, Quaternion> lastTrackedRotations = new Dictionary<string, Quaternion>();
    private string lastImageUpdated = null;

    private bool calibrated = false;
    private int btnCount = 0;

    private void Awake()
    {
        trackedImageManager = arSessionOrigin.GetComponent(typeof(ARTrackedImageManager)) as ARTrackedImageManager;
        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string message, string stackTrace, LogType type)
    {
        debugInfoText.text += message + "\n" + stackTrace + "\n";
        if(debugInfoText.text.Split("\n").Length > 20)
        {
            debugInfoText.text = debugInfoText.text.Substring(debugInfoText.text.IndexOf("\n") + 1);
        }
    }

    void Start()
    {
        btnCalibrate.onClick.AddListener(() => Calibrate());
    }

    void OnEnable() => trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;


    void OnDisable() => trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;

    void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var removedImage in eventArgs.removed)
        {
            lastTrackedPositions.Remove(removedImage.referenceImage.name);
            lastTrackedRotations.Remove(removedImage.referenceImage.name);
        }

        foreach (var newImage in eventArgs.added)
        {
            lastTrackedPositions[newImage.referenceImage.name] = newImage.transform.position;
            lastTrackedRotations[newImage.referenceImage.name] = newImage.transform.rotation;
            lastImageUpdated = newImage.referenceImage.name;
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            lastTrackedPositions[updatedImage.referenceImage.name] = updatedImage.transform.position;
            lastTrackedRotations[updatedImage.referenceImage.name] = updatedImage.transform.rotation;
            lastImageUpdated = updatedImage.referenceImage.name;
        }
    }

    private void Update()
    {
        //debugInfoText.text = "Origin Position: " + arSessionOrigin.transform.position.ToString() + "\nOrigin Rotation: " + arSessionOrigin.transform.rotation.ToString();

        if (lastImageUpdated != null && lastTrackedPositions.ContainsKey(lastImageUpdated))
        {
            //debugInfoText.text += "\nLast Updated Image: " + lastImageUpdated;
            //debugInfoText.text += "\nLast Position: " + lastTrackedPositions[lastImageUpdated].ToString() + "\nLast Rotation: " + lastTrackedRotations[lastImageUpdated].ToString();
        }

        if (calibrated)
        {
            btnCalibrateImage.sprite = btnRecalibrateSprite;
        }
        else
        {
            btnCalibrateImage.sprite = btnCalibrateSprite;
        }

    }

    private void Calibrate()
    {
        btnCount++;
        if (calibrated)
        {
            trackedImageManager.enabled = true;
            arSessionOrigin.transform.rotation = Quaternion.identity;
            arSessionOrigin.transform.position = Vector3.zero;
            calibrated = false;
        }
        else
        {
            // check if tracked image transform is available
            if (lastImageUpdated == null || !lastTrackedPositions.ContainsKey(lastImageUpdated))
                return;

            // get tracking point transform in game world
            Transform trackingPointTransform = null;
            foreach (var go in trackingPoints)
            {
                if (go.name == lastImageUpdated)
                {
                    trackingPointTransform = go.transform;
                    break;
                }
            }
            if (trackingPointTransform == null)
                return;


            // calibrate
            arSessionOrigin.transform.rotation = Quaternion.identity;
            arSessionOrigin.transform.position = Vector3.zero;

            Vector3 newPosition = lastTrackedPositions[lastImageUpdated];
            Quaternion newRotation = Quaternion.Euler(
                new Vector3(
                    0f,
                    lastTrackedRotations[lastImageUpdated].eulerAngles.y - trackingPointTransform.rotation.eulerAngles.y,
                    0f
                    )
                );

            var m = Matrix4x4.TRS(newPosition, newRotation, Vector3.one);
            arSessionOrigin.transform.FromMatrix(m);

            var mm = arSessionOrigin.transform.worldToLocalMatrix;

            arSessionOrigin.transform.FromMatrix(mm);

            arSessionOrigin.transform.position += trackingPointTransform.position;

            trackedImageManager.enabled = false;

            

            calibrated = true;
        }

    }

}
