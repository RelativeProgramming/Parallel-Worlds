using Assets.Scripts.Agents;
using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Simulation : MonoBehaviour
{
    public Realtime Realtime;

    private RealtimeView RealtimeView;

    private List<GameObject> SimulationObjects = new List<GameObject>();

    private void Awake()
    {
        RealtimeView = GetComponent<RealtimeView>();
    }

    private void Start()
    {
        if(RealtimeView.isOwnedLocallySelf)
        {
            //InstantiateRealtimePrefab("Origin Rock");
            InstantiateRealtimePrefab("FoyerAPBNavMesh");

            for(int i = 0; i < 3; i++)
            {
                var go = InstantiateRealtimePrefab("Roaming Agent");
                RoamingAgent agent = go.GetComponent<RoamingAgent>();
                agent.transform.position = new Vector3(-0.5f + i * 0.5f, 0.6f, -4);
                agent.UpdateRate = Random.Range(1f, 4f);
                NavMeshAgent navAgent = go.GetComponent<NavMeshAgent>();
                navAgent.speed = 0.5f + Random.Range(0f, 2f);
            }
        }
    }

    private void Update()
    {
        if (RealtimeView.isOwnedLocallySelf)
        {
            // Simulation Update Code
        }
    }
    private GameObject InstantiateRealtimePrefab(string prefabName, Transform parent = null)
    {
        var options = Realtime.InstantiateOptions.defaults;
        options.useInstance = Realtime;

        var go = Realtime.Instantiate(prefabName: prefabName, options: options);

        RealtimeTransform goTransform = go.GetComponent<RealtimeTransform>();
        goTransform.RequestOwnership();

        go.transform.SetParent(parent == null ? transform : parent);
        SimulationObjects.Add(go);
        return go;
    }
}
