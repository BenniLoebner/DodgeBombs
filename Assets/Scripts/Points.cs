using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Points : MonoBehaviour
{
    public Transform player;
    public float breakTime = 3f;
    public int value = 10; // The score value of the points
    NavMeshAgent agent;

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
        Invoke("Broke", breakTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ScoreManager.Instance.AddScore(value);
            Destroy(gameObject);
        }
    }
    
    private void Broke()
    {
        // Add logic for what happens when the points "break" after the fuse time
        Destroy(gameObject);
    }
}
