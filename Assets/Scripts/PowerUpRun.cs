using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PowerUpRun : MonoBehaviour
{
    public Transform player;
    public float fleeRadius = 6f; // player must get this close before it starts running
    public float fleeDistance = 8f; // how far ahead it aims when running away

    NavMeshAgent agent;
    Vector3 arenaCenter;
    float arenaRadius = 15f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        //the wall ring SpawnCircle builds is what defines the arena, so wander within it
        SpawnCircle arena = FindFirstObjectByType<SpawnCircle>();
        if (arena != null)
        {
            arenaCenter = arena.transform.position;
            arenaRadius = arena.wallRadius;
        }
        else
        {
            arenaCenter = transform.position;
        }

        PickNewWanderTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= fleeRadius)
        {
            Vector3 fleeDirection = (transform.position - player.position).normalized;
            MoveTo(transform.position + fleeDirection * fleeDistance);
            return;
        }

        //wandering - once we've arrived at the last spot, pick somewhere new
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            PickNewWanderTarget();
        }
    }

    void PickNewWanderTarget()
    {
        Vector2 randomPoint = Random.insideUnitCircle * arenaRadius;
        MoveTo(arenaCenter + new Vector3(randomPoint.x, 0f, randomPoint.y));
    }

    void MoveTo(Vector3 target)
    {
        //make sure the point is actually reachable on the NavMesh before committing to it
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, arenaRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
