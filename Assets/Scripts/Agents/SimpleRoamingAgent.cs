using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class SimpleRoamingAgent : MonoBehaviour
{
    private NavMeshAgent Agent;
    private RealtimeView RealtimeView;

    private RaycastHit[] Hits = new RaycastHit[1];

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        RealtimeView = GetComponent<RealtimeView>();
    }

    private void Start()
    {
        if (RealtimeView.isOwnedLocallySelf)
        {
            StartCoroutine(Wander());
        }
    }

    private void Update()
    {
        if (RealtimeView.isOwnedLocallySelf)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Ray ray = ServerCamera.Instance.ScreenPointToRay(Input.mousePosition);

                if (Physics.RaycastNonAlloc(ray, Hits) > 0)
                {
                    Agent.SetDestination(Hits[0].point);
                }
            }
        }
    }


    public float UpdateRate = 0.1f;

    private IEnumerator Wander()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

        while (true)
        {
            if (!Agent.enabled || !Agent.isOnNavMesh)
            {
                yield return Wait;
            }
            else if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                Vector2 point = Random.insideUnitCircle * 3;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(Agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f, Agent.areaMask))
                {
                    Agent.SetDestination(hit.position);
                }
            }

            yield return Wait;
        }
    }
}
