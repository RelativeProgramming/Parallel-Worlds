using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Normal.Realtime.Realtime;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    public bool IsServer = true;

    [HideInInspector]
    public User User = null;

    [HideInInspector]
    public Realtime Realtime;

    void Awake()
    {
        Realtime = GetComponent<Realtime>();
        Realtime.didConnectToRoom += DidConnectToRoom;
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void DidConnectToRoom(Realtime realtime)
    {
        if (IsServer)
        {
            // Instantiate the Simulation once successfully connected to the room
            var options = Realtime.InstantiateOptions.defaults;
            options.useInstance = Realtime;
            GameObject simulationGameObject = Realtime.Instantiate(prefabName: "Simulation",  options: options);

            var simulation = simulationGameObject.GetComponent<Simulation>();
            SceneManager.LoadScene("ServerScene", LoadSceneMode.Additive);
        } else
        {
            // Instantiate player avatar
            var options = Realtime.InstantiateOptions.defaults;
            options.useInstance = Realtime;
            GameObject userGameObject = Realtime.Instantiate(prefabName: "User", options: options);
            userGameObject.GetComponent<MeshRenderer>().enabled = false;

            RealtimeTransform userTransform = userGameObject.GetComponent<RealtimeTransform>();
            userTransform.RequestOwnership();
            
            User = userGameObject.GetComponent<User>();

            SceneManager.LoadScene("ARClientScene", LoadSceneMode.Additive);
        }
    }

    public GameObject InstantiateRealtimePrefab(string prefabName)
    {
        var options = Realtime.InstantiateOptions.defaults;
        options.useInstance = Realtime;

        var go = Realtime.Instantiate(prefabName: prefabName, options: options);

        RealtimeTransform goTransform = go.GetComponent<RealtimeTransform>();
        goTransform.RequestOwnership();

        return go;
    }
}
