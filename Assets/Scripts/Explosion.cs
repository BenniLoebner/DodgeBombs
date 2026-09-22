using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float lifeTime = 1f; // Time in seconds before the explosion is destroyed
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add logic to damage the player here
            Debug.Log("Player hit by explosion!");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
