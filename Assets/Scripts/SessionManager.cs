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
        //DontDestroyOnLoad(gameObject);
    }

    void DidConnectToRoom(Realtime realtime)
    {
        if (IsServer)
        {
            // Instantiate the Simulation once successfully connected to the room
            GameObject simulationGameObject = InstantiateRealtimePrefab("Simulation");
            SceneManager.LoadScene("ServerScene", LoadSceneMode.Additive);
        } else
        {

            var acorn = InstantiateRealtimePrefab("Acorn");
            acorn.GetComponent<FoodItem>().SetCreator("test");

            // Instantiate player avatar
            GameObject userGameObject = InstantiateRealtimePrefab("User");
            userGameObject.GetComponent<MeshRenderer>().enabled = false;
            User = userGameObject.GetComponent<User>();
            User.SetUsername();

            SceneManager.LoadScene("ARClientScene", LoadSceneMode.Additive);
        }
    }

    public GameObject InstantiateRealtimePrefab(string prefabName)
    {
        var options = Realtime.InstantiateOptions.defaults;
        options.useInstance = Realtime;

        var go = Realtime.Instantiate(prefabName: prefabName, options: options);

        RealtimeTransform goTransform = go.GetComponent<RealtimeTransform>();
        if(goTransform != null)
            goTransform.RequestOwnership();

        return go;
    }
}
