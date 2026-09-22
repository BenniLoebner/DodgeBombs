using UnityEngine;

public class SpawnCircle : MonoBehaviour
{
    public GameObject spawnerPrefab;
    public int spawnerCount = 4;

    public float radius = 10f;
    public float rotationSpeed = 20f; // degrees per second

    public GameObject wallPrefab;
    public int wallSegmentCount = 24;
    public float wallRadius = 15f;

    Transform[] spawners;
    float[] angleOffsets;
    float currentAngle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawners = new Transform[spawnerCount];
        angleOffsets = new float[spawnerCount];

        for (int i = 0; i < spawnerCount; i++)
        {
            angleOffsets[i] = 360f / spawnerCount * i;
            spawners[i] = Instantiate(spawnerPrefab, transform).transform;
        }

        //place spawners on the circle immediately so they don't pop in at the wrong spot on the first frame
        UpdateSpawnerPositions();

        GenerateWall();
    }

    // Update is called once per frame
    void Update()
    {
        currentAngle += rotationSpeed * Time.deltaTime;
        UpdateSpawnerPositions();
    }

    void UpdateSpawnerPositions()
    {
        for (int i = 0; i < spawners.Length; i++)
        {
            float angle = (currentAngle + angleOffsets[i]) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * radius;
            spawners[i].position = transform.position + offset;
        }
    }

    void GenerateWall()
    {
        //wall segments are placed once and don't move, forming a static ring around the spawner circle
        for (int i = 0; i < wallSegmentCount; i++)
        {
            float angle = (360f / wallSegmentCount * i) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * wallRadius;
            Vector3 position = transform.position + offset;

            //face the segment outward along the radius so its width lines up tangentially with its neighbors
            Quaternion rotation = Quaternion.LookRotation(offset.normalized, Vector3.up);

            Instantiate(wallPrefab, position, rotation, transform);
        }
    }
}
