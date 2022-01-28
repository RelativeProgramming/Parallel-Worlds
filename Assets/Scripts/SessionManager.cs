using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Normal.Realtime.Realtime;

public class SessionManager : MonoBehaviour
{
    public Camera MainCamera;

    private const bool IS_SERVER = true;

    private Realtime _realtime;

    void Awake()
    {
        _realtime = GetComponent<Realtime>();
        _realtime.didConnectToRoom += DidConnectToRoom;
    }

    void DidConnectToRoom(Realtime realtime)
    {
        if (IS_SERVER)
        {
            // Instantiate the Simulation once successfully connected to the room
            var options = Realtime.InstantiateOptions.defaults;
            options.useInstance = _realtime;
            GameObject simulationGameObject = Realtime.Instantiate(prefabName: "Simulation",  options: options);

            Simulation simulation = simulationGameObject.GetComponent<Simulation>();
            simulation.Realtime = _realtime;
        } else
        {
            // Instantiate player avatar
            var options = Realtime.InstantiateOptions.defaults;
            options.useInstance = _realtime;
            GameObject userGameObject = Realtime.Instantiate(prefabName: "User", options: options);

            RealtimeTransform userTransform = userGameObject.GetComponent<RealtimeTransform>();
            userTransform.RequestOwnership();

            User user = userGameObject.GetComponent<User>();
            user.Camera = MainCamera;

        }
    }
}
