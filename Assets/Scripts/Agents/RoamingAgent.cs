using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Agents
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class RoamingAgent : MonoBehaviour
    {
        private NavMeshAgent Agent;
        private RealtimeView RealtimeView;
        private Simulation Simulation;

        private RaycastHit[] Hits = new RaycastHit[1];

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            RealtimeView = GetComponent<RealtimeView>();
        }

        private void Start()
        {
            Simulation = transform.parent.GetComponent<Simulation>();
            if (RealtimeView.isOwnedLocallySelf)
            {
                StartCoroutine(ActionLoop());
            }
        }

        private void Update()
        {
            if (RealtimeView.isOwnedLocallySelf)
            {
                // Command Agent to move to certain position
                //if (Input.GetKeyUp(KeyCode.Mouse0))
                //{
                //    Ray ray = ServerCamera.Instance.ScreenPointToRay(Input.mousePosition);

                //    if (Physics.RaycastNonAlloc(ray, Hits) > 0)
                //    {
                //        Agent.SetDestination(Hits[0].point);
                //    }
                //}

                var direction = (Destination - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    var lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, (90 + Random.Range(45f, 135f)) * Time.deltaTime);
                    if (Quaternion.Angle(transform.rotation, lookRotation) < 2)
                    {
                        Agent.SetDestination(Destination);
                    }
                }

                // Rotate Agent to moving direction
                if (!Agent.isStopped && Agent.destination == Destination)
                {
                    var targetPosition = Agent.pathEndPosition;
                    var targetPoint = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
                    var _direction = (targetPoint - transform.position).normalized;
                    if (_direction != Vector3.zero)
                    {
                        var _lookRotation = Quaternion.LookRotation(_direction);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, 360);
                    }
                }
            }
        }


        public float UpdateRate = 0.1f;
        public Vector3 Destination = Vector3.zero;
        private IEnumerator ActionLoop()
        {
            while (true)
            {
                if (!Agent.enabled || !Agent.isOnNavMesh)
                {
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    GameObject food = Simulation.getClosestFoodWithinReach(transform.position, 5f);
                    if (food != null)
                    {
                        // Food is in proximity
                        var foodPos = food.transform.position;
                        if (Vector3.Distance(transform.position, foodPos) < 0.3)
                        {
                            // Consume Food when close
                            Simulation.DestroyFoodItem(food);
                            yield return new WaitForSeconds(Random.Range(1f, 3f));
                        }
                        else
                        {
                            // Walk towards food
                            NavMeshHit hit;
                            if (NavMesh.SamplePosition(new Vector3(foodPos.x, 0, foodPos.z), out hit, 2f, Agent.areaMask))
                            {
                                Destination = hit.position;
                                Destination.y = transform.position.y;
                            }
                        }
                    } 
                    else if (Agent.remainingDistance <= Agent.stoppingDistance)
                    {
                        // Wander
                        Vector2 point = Random.insideUnitCircle * 3;
                        NavMeshHit hit;

                        if (NavMesh.SamplePosition(Agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 2f, Agent.areaMask))
                        {
                            Destination = hit.position;
                            Destination.y = transform.position.y;
                        }
                    }

                    yield return new WaitForSeconds(UpdateRate * Random.Range(0.5f, 1.5f));
                }
            }
        }
    }
}
