using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public Transform player;
    public float fuseTime = 3f;
    public float reachedFuseTime = 1f;
    public GameObject explosionPrefab;
    public float detectionRadius = 8f; // the distance enemies try to keep from the player while fleeing
    public int value = 10; // The score value when collected while fleeing

    NavMeshAgent agent;
    bool hasReachedPlayer;

    //shared by every enemy - a counter (not a bool) so overlapping power-ups don't
    //cancel each other's effect early when one of them ends first
    static int fleeEffectCount;
    static bool FleeingFromPlayer => fleeEffectCount > 0;

    public static void BeginFleeEffect()
    {
        fleeEffectCount++;
    }

    public static void EndFleeEffect()
    {
        fleeEffectCount = Mathf.Max(0, fleeEffectCount - 1);
    }

    //statics survive scene loads, and survive between Editor play sessions when domain
    //reloading is disabled - reset explicitly so a stuck count can't outlive a run
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetFleeEffect()
    {
        fleeEffectCount = 0;
    }

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
        Invoke("Explode", fuseTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (FleeingFromPlayer)
        {
            //collectible like Points while running from the power-up, instead of dangerous
            ScoreManager.Instance.AddScore(value);
            Destroy(gameObject);
            return;
        }

        if (!hasReachedPlayer)
        {
            hasReachedPlayer = true;
            //swap the spawn-time fuse for a shorter one now that we've actually reached the player
            CancelInvoke("Explode");
            Invoke("Explode", reachedFuseTime);
        }
    }

    private void Explode()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        // Add explosion logic here (e.g., damage player, play explosion effect)
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }

        if (FleeingFromPlayer)
        {
            //aim for the point exactly detectionRadius away from the player on our own side of
            //them - too close and that sits further out, too far and it sits closer in, so we
            //converge on the ring from either direction instead of stopping dead
            Vector3 directionFromPlayer = (transform.position - player.position).normalized;
            Vector3 ringTarget = player.position + directionFromPlayer * detectionRadius;

            //make sure the point is actually reachable on the NavMesh before committing to it
            if (NavMesh.SamplePosition(ringTarget, out NavMeshHit hit, detectionRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }
}
