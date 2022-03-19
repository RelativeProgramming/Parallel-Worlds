using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Normal.Realtime.Realtime;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    public bool IsServer = true;

    [HideInInspector]
    public User User = null;

    private Realtime _realtime;

    void Awake()
    {
        _realtime = GetComponent<Realtime>();
        _realtime.didConnectToRoom += DidConnectToRoom;
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void DidConnectToRoom(Realtime realtime)
    {
        if (IsServer)
        {
            // Instantiate the Simulation once successfully connected to the room
            var options = Realtime.InstantiateOptions.defaults;
            options.useInstance = _realtime;
            GameObject simulationGameObject = Realtime.Instantiate(prefabName: "Simulation",  options: options);

            Simulation simulation = simulationGameObject.GetComponent<Simulation>();
            simulation.Realtime = _realtime;
            SceneManager.LoadScene("ServerScene", LoadSceneMode.Additive);
        } else
        {
            // Instantiate player avatar
            var options = Realtime.InstantiateOptions.defaults;
            options.useInstance = _realtime;
            GameObject userGameObject = Realtime.Instantiate(prefabName: "User", options: options);
            userGameObject.GetComponent<MeshRenderer>().enabled = false;

            RealtimeTransform userTransform = userGameObject.GetComponent<RealtimeTransform>();
            userTransform.RequestOwnership();
            

            User = userGameObject.GetComponent<User>();

            SceneManager.LoadScene("ARClientScene", LoadSceneMode.Additive);
        }
    }
}
