//using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Transform player;

    public GameObject baseSegment;
    public List<GameObject> randomSegments;

    public int initialSegments = 3;
    public int maxActiveSegments = 5;
    public float spawnTriggerDistance = 20f;
    public float segmentLength = 25f;

    public float verticalOffsetY = 0f;
    public bool snapToGeneratorXY = true;
    public bool useFixedY = false;
    public float fixedY = 0f;

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

        // 1) Base inicial
        SpawnBaseSegment();

        // 2) Tramos iniciales
        for (int i = 1; i < initialSegments; i++)
        {
            SpawnRandomSegment();
        }
    }

    void Update()
    {
        if (player == null) return;

        // --- CAMBIO IMPORTANTE: solo usamos la diferencia en Z ---
        float distZ = nextSpawnPoint.z - player.position.z;

        if (distZ < spawnTriggerDistance)
        {
            SpawnRandomSegment();
        }
    }

    void SpawnBaseSegment()
    {
        if (baseSegment == null) return;

        Vector3 spawnPos = nextSpawnPoint + Vector3.up * verticalOffsetY;

        if (snapToGeneratorXY)
        {
            spawnPos.x = transform.position.x;
        }

        if (useFixedY)
        {
            spawnPos.y = fixedY;
        }

        GameObject newSegment = Instantiate(baseSegment, spawnPos, transform.rotation);
        activeSegments.Add(newSegment);

        float advance = GetAdvanceLength(newSegment);
        nextSpawnPoint = spawnPos + transform.forward * advance;
    }

    void SpawnRandomSegment()
    {
        if (randomSegments == null || randomSegments.Count == 0)
        {
            Debug.LogWarning("LevelGenerator: randomSegments está vacío.");
            return;
        }

        GameObject prefab = randomSegments[Random.Range(0, randomSegments.Count)];

        Vector3 spawnPos = nextSpawnPoint + Vector3.up * verticalOffsetY;

        if (snapToGeneratorXY)
        {
            spawnPos.x = transform.position.x;
        }

        if (useFixedY)
        {
            spawnPos.y = fixedY;
        }

        GameObject newSegment = Instantiate(prefab, spawnPos, transform.rotation);
        activeSegments.Add(newSegment);

        float advance = GetAdvanceLength(newSegment);
        nextSpawnPoint = spawnPos + transform.forward * advance;

        // Reciclado
        if (activeSegments.Count > maxActiveSegments)
        {
            Destroy(activeSegments[0]);
            activeSegments.RemoveAt(0);
        }
    }

    float GetAdvanceLength(GameObject segment)
    {
        // Transform endPoint = segment.transform.Find("EndPoint");
        // if (endPoint != null)
        // {
        //     Vector3 delta = endPoint.position - segment.transform.position;
        //     float proj = Vector3.Dot(delta, transform.forward);
        //     if (proj > 0.01f) return proj;
        // }
        // float len = segmentLength;
        // var rends = segment.GetComponentsInChildren<Renderer>();
        // if (rends != null && rends.Length > 0)
        // {
        //     Bounds b = rends[0].bounds;
        //     for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
        //     len = b.size.z; // asumimos orientación en Z
        // }
        // return len;
        return segmentLength;
    }
}
