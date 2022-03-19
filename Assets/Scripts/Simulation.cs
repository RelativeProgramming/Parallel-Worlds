using Assets.Scripts.Agents;
using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Simulation : MonoBehaviour
{
    public Realtime Realtime;

    private RealtimeView RealtimeView;

    private SpawnAreaManager SpawnAreaManager;

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
            InstantiateRealtimePrefab("FoyerAPBCollider");
            SpawnAreaManager = InstantiateRealtimePrefab("FoyerAPBSpawnAreas").GetComponent<SpawnAreaManager>();
            

            for (int i = 0; i < 3; i++)
            {
                var go = InstantiateRealtimePrefab("Roaming Agent");
                RoamingAgent agent = go.GetComponent<RoamingAgent>();
                agent.transform.position = new Vector3(-0.5f + i * 0.5f, 0.6f, -4);
                agent.UpdateRate = Random.Range(1f, 4f);
                NavMeshAgent navAgent = go.GetComponent<NavMeshAgent>();
                navAgent.speed = 0.5f + Random.Range(0f, 2f);
            }

            StartCoroutine(SpawnFood());
        }
    }

    private void Update()
    {
        if (RealtimeView.isOwnedLocallySelf)
        {
            // Simulation Update Code
            if(Input.GetMouseButtonDown(0))
            {
                GameObject acorn = InstantiateRealtimePrefab("Acorn");
                acorn.transform.position = new Vector3(0, 1, 0);
            }
        }
    }
    public GameObject InstantiateRealtimePrefab(string prefabName, Transform parent = null)
    {
        var options = Realtime.InstantiateOptions.defaults;
        options.useInstance = Realtime;

        var go = Realtime.Instantiate(prefabName: prefabName, options: options);

        RealtimeTransform goTransform = go.GetComponent<RealtimeTransform>();
        goTransform.RequestOwnership();

        go.transform.SetParent(parent == null ? transform : parent);
        return go;
    }

    public void DestroyRealtimeObject(GameObject go)
    {
        RealtimeTransform goTransform = go.GetComponent<RealtimeTransform>();
        goTransform.RequestOwnership();
        Realtime.Destroy(go);
    }


    private List<GameObject> FoodObjects = new List<GameObject>();
    private IEnumerator SpawnFood()
    {
        while(true)
        {
            //WaitForSeconds Wait = new WaitForSeconds(Random.Range(10f, 20f));
            WaitForSeconds Wait = new WaitForSeconds(Random.Range(1f, 2f));

            // Delete oldest acorn if object limit is reached
            if (FoodObjects.Count >= 150)
            {
                DestroyRealtimeObject(FoodObjects[0]);
                FoodObjects.RemoveAt(0);
            }

            // Add new Acorn

            Vector3 pos = SpawnAreaManager.GenerateRandomPoint(0);
            // Bit shift the index of the layer 6 to get a bit mask
            int layerMask = 1 << 6;
            RaycastHit hit;
            // Does the ray intersect any objects on the floor layer
            if (Physics.Raycast(pos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            {
                // It hit -> spawn Food
                GameObject acorn = InstantiateRealtimePrefab("Acorn");
                acorn.transform.position = pos;
                acorn.transform.Rotate(new Vector3(Random.Range(1f, 89f), 0, 0));
                FoodObjects.Add(acorn);
            }

            yield return Wait;
        }
    }

    public void DestroyFoodItem(GameObject food)
    {
        DestroyRealtimeObject(food);
        FoodObjects.Remove(food);
    }

    public GameObject getClosestFoodWithinReach(Vector3 pos, float range)
    {
        GameObject result = null;
        float closestDistance = float.PositiveInfinity;
        foreach(GameObject go in FoodObjects)
        {
            float dist = Vector3.Distance(pos, go.transform.position);
            if (dist < range && dist < closestDistance)
            {
                result = go;
                closestDistance = dist;
            }
        }
        return result;
    }
}
