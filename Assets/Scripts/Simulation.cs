using Assets.Scripts.Agent;
using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public Realtime Realtime;

    private RealtimeView RealtimeView;

    private List<GameObject> SimulationObjects = new List<GameObject>();

    private int selectedCube = 0;
    private bool cubeInMovement = false;
    private Vector3 move = Vector3.zero;


    private void Awake()
    {
        RealtimeView = GetComponent<RealtimeView>();
    }

    private void Start()
    {
        if(RealtimeView.isOwnedLocallySelf)
        {
            //for (int i = -1; i <= 1; i++)
            //{
            //    var cube = InstantiateRealtimePrefab("Cube");
            //    cube.transform.position = new Vector3(i * 1.5f, 0, 0);
            //    cube.transform.localScale = Vector3.one * 0.3f;
            //}

            //InstantiateRealtimePrefab("Origin Rock");
            InstantiateRealtimePrefab("Environment");

            for(int i = 0; i < 3; i++)
            {
                var go = InstantiateRealtimePrefab("Roaming Agent");
                RoamingAgent agent = go.GetComponent<RoamingAgent>();
                agent.transform.position = new Vector3(-0.5f + i * 0.5f, 0.6f, -4);
            }
        }
    }

    private void Update()
    {
        //if(RealtimeView.isOwnedLocallySelf)
        //{
        //    if (!cubeInMovement)
        //    {
        //        // select new Random Cube
        //        selectedCube = System.Math.Min(Mathf.FloorToInt(Random.value * SimulationObjects.Count), SimulationObjects.Count - 1);
        //        cubeInMovement = true;
        //        if (SimulationObjects[selectedCube].transform.position.y > 0)
        //            move = new Vector3(0, -1, 0);
        //        else
        //            move = new Vector3(0, 1, 0);
        //    }

        //    var cube = SimulationObjects[selectedCube];
        //    cube.transform.position += move * Time.deltaTime;

        //    if (cube.transform.position.y < 0)
        //    {
        //        cube.transform.position.Set(cube.transform.position.x, 0, cube.transform.position.z);
        //        cubeInMovement = false;
        //    }
        //    if (cube.transform.position.y > 1)
        //    {
        //        cube.transform.position.Set(cube.transform.position.x, 1, cube.transform.position.z);
        //        cubeInMovement = false;
        //    }
        //}
    }
    private GameObject InstantiateRealtimePrefab(string prefabName, Transform parent = null)
    {
        var options = Realtime.InstantiateOptions.defaults;
        options.useInstance = Realtime;

        var cube = Realtime.Instantiate(prefabName: prefabName, options: options);

        RealtimeTransform cubeTransform = cube.GetComponent<RealtimeTransform>();
        cubeTransform.RequestOwnership();

        cube.transform.SetParent(parent == null ? transform : parent);
        SimulationObjects.Add(cube);
        return cube;
    }
}
