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
    public float segmentLength = 25f;
    public float verticalOffsetY = 0f;
    public bool snapToGeneratorXY = true;
    public bool useFixedY = false;
    public float fixedY = 0f;
    public float groundY = 0f;

    private Vector3 nextSpawnPoint;
    private List<GameObject> activeSegments = new List<GameObject>();

    void Start()
    {
        nextSpawnPoint = transform.position;

        if (player == null)
        {
            var moto = FindObjectOfType<MovimientoMoto>();
            if (moto != null) player = moto.transform;
        }

        // 1) SIEMPRE generar el Base primero
        SpawnBaseSegment();

        // 2) Generar los dem�s de forma aleatoria
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
        if (baseSegment == null) return;
        Vector3 spawnPos = nextSpawnPoint + Vector3.up * verticalOffsetY;
        if (snapToGeneratorXY) spawnPos.x = transform.position.x;
        if (useFixedY) spawnPos.y = fixedY;
        GameObject newSegment = Instantiate(baseSegment, spawnPos, transform.rotation);
        activeSegments.Add(newSegment);

        float advance = GetAdvanceLength(newSegment);
        nextSpawnPoint = spawnPos + transform.forward * advance;
    }

    void SpawnRandomSegment()
    {
        GameObject prefab = randomSegments[Random.Range(0, randomSegments.Count)];

        Vector3 spawnPos = nextSpawnPoint + Vector3.up * verticalOffsetY;
        if (snapToGeneratorXY) spawnPos.x = transform.position.x;
        if (useFixedY) spawnPos.y = fixedY;
        GameObject newSegment = Instantiate(prefab, spawnPos, transform.rotation);
        activeSegments.Add(newSegment);

        float advance = GetAdvanceLength(newSegment);
        Debug.Log("Se generó tramo: " + newSegment.name + " | Longitud calculada: " + advance);
        nextSpawnPoint = spawnPos + transform.forward * advance;

        if (activeSegments.Count > maxActiveSegments)
        {
            Destroy(activeSegments[0]);
            activeSegments.RemoveAt(0);
        }
    }

    float GetAdvanceLength(GameObject segment)
    {
        Transform endPoint = segment.transform.Find("EndPoint");
        if (endPoint != null)
        {
            Vector3 delta = endPoint.position - segment.transform.position;
            float proj = Vector3.Dot(delta, transform.forward);
            if (proj > 0.01f) return proj;
        }
        float len = segmentLength;
        var rends = segment.GetComponentsInChildren<Renderer>();
        if (rends != null && rends.Length > 0)
        {
            Bounds b = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
            len = b.size.z; // asumimos orientación en Z
        }
        return len;
    }
}
