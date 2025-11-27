using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Jugador")]
    public Transform player;

    [Header("Prefabs de Tramos")]
    public GameObject baseSegment;
    public List<GameObject> randomSegments;

    [Header("Configuración de tramos")]
    public int initialSegments = 3;
    public int maxActiveSegments = 5;
    public float spawnTriggerDistance = 20f;
    public float segmentLength = 25f;

    [Header("Posicionamiento")]
    public float verticalOffsetY = 0f;
    public bool snapToGeneratorXY = true;
    public bool useFixedY = false;
    public float fixedY = 0f;

    // Coleccionables / powerups: deja aquí lo que ya tenías
    [Header("Coleccionables / PowerUps")]
    public bool spawnCollectibles = true;
    public GameObject pizzaPrefab;
    public GameObject timeBoostPrefab;
    public GameObject magnetPrefab;
    public int maxCollectiblesPerSegment = 3;
    [Range(0f, 1f)] public float collectibleSpawnChance = 0.8f;
    [Range(0f, 1f)] public float timeBoostChance = 0.1f;
    [Range(0f, 1f)] public float magnetChance = 0.05f;
    public float collectibleOffsetY = 0.5f;
    public float[] carrilesX = new float[] { -2.3867f, -1.13f };

    // Estado interno
    private Vector3 nextSpawnPoint;
    private List<GameObject> activeSegments = new List<GameObject>();

    // Pools
    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();
    private Transform segmentsParent;


    [Header("Obstáculos")]
    public bool spawnObstacles = true;
    public List<GameObject> obstaclePrefabs;

    // cuántos tramos sin obstáculos al inicio (zona segura)
    public int safeSegmentsWithoutObstacles = 2;

    public int maxObstaclesPerSegment = 1;
    [Range(0f, 1f)] public float obstacleSpawnChance = 0.3f;
    public float obstacleOffsetY = 0f;

    // distancia mínima entre obstáculos consecutivos en el eje Z
    public float minObstacleDistanceZ = 10f;
    private int segmentsSpawned = 0;
    private float lastObstacleZ = Mathf.NegativeInfinity;

    void Start()
    {
        nextSpawnPoint = transform.position;

        if (player == null)
        {
            var moto = FindObjectOfType<MovimientoMoto>();
            if (moto != null) player = moto.transform;
        }

        // contenedor para tener ordenada la jerarquía
        segmentsParent = new GameObject("SegmentsPool").transform;
        segmentsParent.SetParent(transform);

        // Solo creamos las colas vacías
        InitPoolForPrefab(baseSegment);
        if (randomSegments != null)
        {
            foreach (var p in randomSegments)
                InitPoolForPrefab(p);
        }

        // Base + iniciales
        SpawnBaseSegment();
        for (int i = 1; i < initialSegments; i++)
            SpawnRandomSegment();
    }

    void Update()
    {
        if (player == null) return;

        float distZ = nextSpawnPoint.z - player.position.z;
        if (distZ < spawnTriggerDistance)
            SpawnRandomSegment();
    }

    // =========================
    //   POOLING
    // =========================

    void InitPoolForPrefab(GameObject prefab)
    {
        if (prefab == null) return;
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();
    }

    GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        if (!pools.TryGetValue(prefab, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            pools[prefab] = queue;
        }

        GameObject obj;

        // Si hay uno disponible, lo reusamos
        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        else
        {
            // Si no hay, instanciamos uno nuevo
            obj = Instantiate(prefab, position, rotation, segmentsParent);
        }

        // Aseguramos info de origen
        var info = obj.GetComponent<SegmentPoolInfo>();
        if (info == null) info = obj.AddComponent<SegmentPoolInfo>();
        info.prefabOrigin = prefab;

        // Limpiamos y generamos coleccionables
        ClearCollectibles(obj);
        ClearObstacles(obj);
        SpawnObstaclesOnSegment(obj);
        SpawnCollectiblesOnSegment(obj);

        return obj;
    }

    void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

        var info = obj.GetComponent<SegmentPoolInfo>();
        if (info == null || info.prefabOrigin == null)
        {
            obj.SetActive(false);
            return;
        }


        // limpiar antes de guardarlo
        ClearObstacles(obj);
        ClearCollectibles(obj);

        obj.SetActive(false);

        if (!pools.TryGetValue(info.prefabOrigin, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            pools[info.prefabOrigin] = queue;
        }

        queue.Enqueue(obj);
    }

    // =========================
    //   SPAWN DE TRAMOS
    // =========================

    void SpawnBaseSegment()
    {
        if (baseSegment == null) return;

        Vector3 spawnPos = nextSpawnPoint + Vector3.up * verticalOffsetY;
        if (snapToGeneratorXY) spawnPos.x = transform.position.x;
        if (useFixedY) spawnPos.y = fixedY;

        GameObject newSegment = GetFromPool(baseSegment, spawnPos, transform.rotation);
        activeSegments.Add(newSegment);

        // 🔹 este es el primer segmento
        segmentsSpawned++;

        // obsts / coleccionables
        SpawnObstaclesOnSegment(newSegment);
        SpawnCollectiblesOnSegment(newSegment);

        float advance = GetAdvanceLength(newSegment);
        nextSpawnPoint = spawnPos + transform.forward * advance;
    }

    void SpawnRandomSegment()
    {
        if (randomSegments == null || randomSegments.Count == 0)
        {
            Debug.LogWarning("LevelGenerator: randomSegments vacío.");
            return;
        }

        GameObject prefab = randomSegments[Random.Range(0, randomSegments.Count)];

        Vector3 spawnPos = nextSpawnPoint + Vector3.up * verticalOffsetY;
        if (snapToGeneratorXY) spawnPos.x = transform.position.x;
        if (useFixedY) spawnPos.y = fixedY;

        GameObject newSegment = GetFromPool(prefab, spawnPos, transform.rotation);
        activeSegments.Add(newSegment);

        // 🔹 nuevo segmento generado
        segmentsSpawned++;

        // obsts / coleccionables
        SpawnObstaclesOnSegment(newSegment);
        SpawnCollectiblesOnSegment(newSegment);

        float advance = GetAdvanceLength(newSegment);
        nextSpawnPoint = spawnPos + transform.forward * advance;

        if (activeSegments.Count > maxActiveSegments)
        {
            GameObject oldest = activeSegments[0];
            activeSegments.RemoveAt(0);
            ReturnToPool(oldest);
        }
    }

    float GetAdvanceLength(GameObject segment)
    {
        // Por ahora usamos longitud fija definida en el inspector
        return segmentLength;
    }

    // =========================
    //   COLECCIONABLES (Paso 3)
    // =========================

    void SpawnCollectiblesOnSegment(GameObject segment)
    {
        if (!spawnCollectibles) return;

        bool hayPizza = pizzaPrefab != null;
        bool hayTiempo = timeBoostPrefab != null;
        bool hayIman = magnetPrefab != null;

        if (!hayPizza && !hayTiempo && !hayIman) return;
        if (carrilesX == null || carrilesX.Length == 0) return;
        if (maxCollectiblesPerSegment <= 0) return;

        // Número aleatorio de slots en este tramo (1..max)
        int slots = Random.Range(1, maxCollectiblesPerSegment + 1);

        for (int i = 0; i < slots; i++)
        {
            // Probabilidad de NO generar nada en este slot
            if (Random.value > collectibleSpawnChance)
                continue;

            // Calcular posición Z del slot en el tramo
            float frac = (i + 1f) / (slots + 1f);
            float zPos = segment.transform.position.z + segmentLength * frac;

            // Elegir carril aleatorio
            float laneX = carrilesX[Random.Range(0, carrilesX.Length)];

            Vector3 pos = new Vector3(
                laneX,
                segment.transform.position.y + collectibleOffsetY,
                zPos
            );

            // --- Determinar tipo de ítem ---
            GameObject prefabElegido = null;
            float roll = Random.value;

            if (hayTiempo && roll < timeBoostChance)
            {
                prefabElegido = timeBoostPrefab;
            }
            else if (hayIman && roll < timeBoostChance + magnetChance)
            {
                prefabElegido = magnetPrefab;
            }
            else if (hayPizza)
            {
                prefabElegido = pizzaPrefab;
            }

            if (prefabElegido == null) continue;

            // Instanciar ítem como hijo del tramo
            GameObject item = Instantiate(prefabElegido, pos, Quaternion.identity);
            item.transform.SetParent(segment.transform);

            // Añadir marker para limpieza automática en pooling
            if (item.GetComponent<CollectibleMarker>() == null)
                item.AddComponent<CollectibleMarker>();
        }
    }

    void ClearCollectibles(GameObject segment)
    {
        var markers = segment.GetComponentsInChildren<CollectibleMarker>(true);
        foreach (var m in markers)
        {
            if (m != null && m.gameObject != segment)
                Destroy(m.gameObject);
        }
    }

    void SpawnObstaclesOnSegment(GameObject segment)
    {
        if (!spawnObstacles) return;
        if (obstaclePrefabs == null || obstaclePrefabs.Count == 0) return;
        if (carrilesX == null || carrilesX.Length == 0) return;
        if (maxObstaclesPerSegment <= 0) return;

        // 🔹 Zona segura al inicio: los primeros N segmentos no tienen obstáculos
        if (segmentsSpawned <= safeSegmentsWithoutObstacles)
            return;

        int slots = Random.Range(1, maxObstaclesPerSegment + 1);

        for (int i = 0; i < slots; i++)
        {
            if (Random.value > obstacleSpawnChance)
                continue;

            float frac = (i + 1f) / (slots + 1f);
            float zPos = segment.transform.position.z + segmentLength * frac;

            // 🔹 Distancia mínima global entre obstáculos
            if (zPos - lastObstacleZ < minObstacleDistanceZ)
                continue;

            float laneX = carrilesX[Random.Range(0, carrilesX.Length)];

            Vector3 pos = new Vector3(
                laneX,
                segment.transform.position.y + obstacleOffsetY,
                zPos
            );

            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
            if (prefab == null) continue;

            GameObject obst = Instantiate(prefab, pos, Quaternion.identity);
            obst.transform.SetParent(segment.transform);

            if (obst.GetComponent<ObstacleMarker>() == null)
                obst.AddComponent<ObstacleMarker>();

            // 🔹 Actualizamos la última Z donde hay obstáculo
            lastObstacleZ = zPos;
        }
    }

    void ClearObstacles(GameObject segment)
    {
        var markers = segment.GetComponentsInChildren<ObstacleMarker>(true);
        foreach (var m in markers)
        {
            if (m != null && m.gameObject != segment)
                Destroy(m.gameObject);
        }
    }
}

// Este componente marca un objeto como coleccionable hijo de un tramo
public class CollectibleMarker : MonoBehaviour
{
}

public class ObstacleMarker : MonoBehaviour
{

}

// Este componente guarda de qué prefab salió el tramo (para el pool)
public class SegmentPoolInfo : MonoBehaviour
{
    [HideInInspector] public GameObject prefabOrigin;
}


