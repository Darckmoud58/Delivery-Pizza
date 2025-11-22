//using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Transform player;

    // Prefab base al inicio
    public GameObject baseSegment;

    // Prefabs aleatorios
    public List<GameObject> randomSegments;

    public int initialSegments = 3;
    public int maxActiveSegments = 5;
    public float spawnTriggerDistance = 20f;

    private Vector3 nextSpawnPoint;
    private List<GameObject> activeSegments = new List<GameObject>();

    void Start()
    {
        nextSpawnPoint = Vector3.zero;

        // 1) SIEMPRE generar el Base primero
        SpawnBaseSegment();

        // 2) Generar los demás de forma aleatoria
        for (int i = 1; i < initialSegments; i++)
        {
            SpawnRandomSegment();
        }
    }

    void Update()
    {
        if (Vector3.Distance(player.position, nextSpawnPoint) < spawnTriggerDistance)
        {
            SpawnRandomSegment();
        }
    }

    void SpawnBaseSegment()
    {
        GameObject newSegment = Instantiate(baseSegment, nextSpawnPoint, Quaternion.identity);
        activeSegments.Add(newSegment);

        Transform endPoint = newSegment.transform.Find("EndPoint");
        nextSpawnPoint = endPoint.position;
    }

    void SpawnRandomSegment()
    {
        GameObject prefab = randomSegments[Random.Range(0, randomSegments.Count)];

        GameObject newSegment = Instantiate(prefab, nextSpawnPoint, Quaternion.identity);
        activeSegments.Add(newSegment);

        Transform endPoint = newSegment.transform.Find("EndPoint");
        nextSpawnPoint = endPoint.position;

        if (activeSegments.Count > maxActiveSegments)
        {
            Destroy(activeSegments[0]);
            activeSegments.RemoveAt(0);
        }
    }
}
