using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAreaManager : MonoBehaviour
{
    private List<Vector3[]> spawnAreaRects = new List<Vector3[]>();
    private void Awake()
    {
        if (transform.childCount % 2 != 0)
            return;

        for(int i = 0; i < transform.childCount; i+=2)
        {
            Transform point1 = transform.Find("Marker" + (i / 2) + "-0");
            Transform point2 = transform.Find("Marker" + (i / 2) + "-1");
            spawnAreaRects.Add(new Vector3[] { 
                new Vector3(Mathf.Min(point1.position.x, point2.position.x), point1.position.y, Mathf.Min(point1.position.z, point2.position.z)),
                new Vector3(Mathf.Max(point1.position.x, point2.position.x), point1.position.y, Mathf.Max(point1.position.z, point2.position.z)),
            });
        }
    }

    public Vector3 GenerateRandomPoint(int spawnAreaIndex)
    {
        var markers = spawnAreaRects[spawnAreaIndex];
        return new Vector3(
            Random.Range(markers[0].x, markers[1].x),
            markers[0].y,
            Random.Range(markers[0].z, markers[1].z)
        );
    }
}
