using Normal.Realtime;
using Normal.Realtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class RoamingAgent : MonoBehaviour // RealtimeComponent<RoamingAgentModel>
{
    private NavMeshAgent Agent;
    private RealtimeView RealtimeView;
    private Simulation Simulation;

    private RaycastHit[] Hits = new RaycastHit[1];
    private string UserTarget = "";
    private Dictionary<string, int> TamingStatus = new Dictionary<string, int>();

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        RealtimeView = GetComponent<RealtimeView>();
    }

    private void Start()
    {
        if (RealtimeView.isOwnedLocallySelf)
        {
            Simulation = GameObject.FindObjectOfType<Simulation>();
            StartCoroutine(ActionLoop());
        }
    }

    //private void TamingUpdate(RealtimeDictionary<RoamingAgentFoodModel> dictionary, uint key, RoamingAgentFoodModel oldModel, RoamingAgentFoodModel newModel, bool remote)
    //{
    //    if(UserTarget.Length == 0)
    //    {
    //        if (newModel.count > 3)
    //        {
    //            UserTarget = Simulation.GetModel().users[key].name;
    //        }
    //    }
    //}

    //protected override void OnRealtimeModelReplaced(RoamingAgentModel prevModel, RoamingAgentModel newModel)
    //{
    //    Debug.Log("Agent-Model: " + newModel);
    //    if (RealtimeView.isOwnedLocallySelf)
    //    {
    //        if (prevModel != null)
    //        {
    //            prevModel.tamingStatus.modelReplaced -= TamingUpdate;
    //        }
    //        if (newModel != null)
    //        {
    //            prevModel.tamingStatus.modelReplaced += TamingUpdate;
    //        }
    //    }
    //}

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
                GameObject foodItem = Simulation.getClosestFoodWithinReach(transform.position, 5f, UserTarget.Length > 0 ? UserTarget : null);
                if (foodItem != null)
                {
                    // Food is in proximity
                    var foodPos = foodItem.transform.position;
                    if (Vector3.Distance(transform.position, foodPos) < 0.3)
                    {
                        // Consume Food when close
                        ConsumeFoodItem(foodItem);
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
                    if (UserTarget.Length > 0)
                    {
                        var users = GameObject.FindObjectsOfType<User>();
                        foreach (var user in users)
                        {
                            if(user.GetUsername() == UserTarget)
                            {
                                Destination = user.transform.position;
                                Destination.y = transform.position.y;
                                break;
                            }
                        }
                    }
                    else
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
                }

                yield return new WaitForSeconds(UpdateRate * Random.Range(0.5f, 1.5f));
            }
        }
    }

    private void ConsumeFoodItem(GameObject foodItem)
    {
        string creator = foodItem.GetComponent<FoodItem>().GetCreator();
        if (creator.Length > 0 && UserTarget.Length == 0)
        {
            if (!TamingStatus.ContainsKey(creator))
                TamingStatus[creator] = 0;
            TamingStatus[creator]++;
            if (TamingStatus[creator] >= 3)
                UserTarget = creator;
            //uint id = (uint) creator.GetHashCode();
            //int count = 1;
            //if (model.tamingStatus.ContainsKey(id))
            //    count = model.tamingStatus[id].count + 1;
            //model.tamingStatus[id].count = count;
        }
        Simulation.DestroyFoodItem(foodItem);
    }

    
}

