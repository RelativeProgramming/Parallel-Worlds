using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts
{
    public class Spawner : MonoBehaviour
    {
        public GameObject SpawnerPrefab;
        public int SpawnAmount;

        private ObjectPool<GameObject> Pool;

        void Start()
        {
            Pool = new ObjectPool<GameObject>(() =>
            {
                return Instantiate(SpawnerPrefab);
            }, go =>
            {
                go.SetActive(true);
            }, go =>
            {
                go.SetActive(false);
            }, go =>
            {
                Destroy(go);
            }, false // saves CPU time, if object is returned multiple times to the pool
            , 10, 20); // initial and max size of the array
        }
    }
}