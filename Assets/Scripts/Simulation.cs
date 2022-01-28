using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    public Realtime Realtime;
    public GameObject CubePrefab;

    private RealtimeView RealtimeView;

    private List<GameObject> cubes = new List<GameObject>();

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
            for (int i = -1; i <= 1; i++)
            {
                var options = Realtime.InstantiateOptions.defaults;
                options.useInstance = Realtime;
                var cube = Realtime.Instantiate(prefabName: "Cube", options: options);
                RealtimeTransform cubeTransform = cube.GetComponent<RealtimeTransform>();
                cubeTransform.RequestOwnership();

                cube.transform.SetParent(transform);
                cube.transform.position = new Vector3(i * 1.5f, 0, 0);
                cubes.Add(cube);
            }
        }
    }

    private void Update()
    {
        if(RealtimeView.isOwnedLocallySelf)
        {
            if (!cubeInMovement)
            {
                // select new Random Cube
                selectedCube = System.Math.Min(Mathf.FloorToInt(Random.value * cubes.Count), cubes.Count - 1);
                cubeInMovement = true;
                if (cubes[selectedCube].transform.position.y > 0)
                    move = new Vector3(0, -1, 0);
                else
                    move = new Vector3(0, 1, 0);
            }

            var cube = cubes[selectedCube];
            cube.transform.position += move * Time.deltaTime;

            if (cube.transform.position.y < 0)
            {
                cube.transform.position.Set(cube.transform.position.x, 0, cube.transform.position.z);
                cubeInMovement = false;
            }
            if (cube.transform.position.y > 1)
            {
                cube.transform.position.Set(cube.transform.position.x, 1, cube.transform.position.z);
                cubeInMovement = false;
            }
        }
    }
}
