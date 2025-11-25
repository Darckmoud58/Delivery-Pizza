using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Jugador")]
    public Transform player;

    [Header("Prefabs de Tramos")]
    public GameObject baseSegment;              // Prefab base al inicio
    public List<GameObject> randomSegments;     // Prefabs aleatorios

    [Header("Configuración de tramos")]
    public int initialSegments = 3;
    public int maxActiveSegments = 5;
    public float spawnTriggerDistance = 20f;    // Distancia antes del siguiente spawn
    public float segmentLength = 25f;           // Longitud fija de cada tramo

    [Header("Posicionamiento")]
    public float verticalOffsetY = 0f;
    public bool snapToGeneratorXY = true;
    public bool useFixedY = false;
    public float fixedY = 0f;

    [Header("Pooling")]
    public int poolSizePerPrefab = 5;           // Cuántos instanciar de cada tipo al inicio

    [Header("Coleccionables (Paso 3)")]
    public bool spawnCollectibles = false;
    public GameObject collectiblePrefab;
    public int collectiblesPerSegment = 3;
    public float collectibleOffsetY = 1f;
    public float[] carrilesX = new float[] { -2.3867f, -1.13f }; // mismos que tu CambioCarril

    // Estado interno
    private Vector3 nextSpawnPoint;
    private List<GameObject> activeSegments = new List<GameObject>();

    // Pools: prefab -> cola de instancias disponibles
    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    void Start()
    {
        nextSpawnPoint = transform.position;

        if (player == null)
        {
            var moto = FindObjectOfType<MovimientoMoto>();
            if (moto != null) player = moto.transform;
        }

        // Crear pools para todos los prefabs
        InitPoolForPrefab(baseSegment);
        if (randomSegments != null)
        {
            foreach (var p in randomSegments)
            {
                InitPoolForPrefab(p);
            }
        }

        // 1) Tramo base
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

        // Solo nos interesa el eje Z
        float distZ = nextSpawnPoint.z - player.position.z;

        if (distZ < spawnTriggerDistance)
        {
            SpawnRandomSegment();
        }
    }

    // =========================
    //   POOLING
    // =========================

    void InitPoolForPrefab(GameObject prefab)
    {
        if (prefab == null) return;
        if (pools.ContainsKey(prefab)) return;

        var queue = new Queue<GameObject>();
        pools[prefab] = queue;

        for (int i = 0; i < poolSizePerPrefab; i++)
        {
            GameObject obj = Instantiate(prefab, new Vector3(0, -1000, 0), Quaternion.identity);
            var info = obj.GetComponent<SegmentPoolInfo>();
            if (info == null) info = obj.AddComponent<SegmentPoolInfo>();
            info.prefabOrigin = prefab;

            obj.SetActive(false);
            queue.Enqueue(obj);
        }
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
        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, position, rotation);
        }

        // Aseguramos que tenga la referencia al prefab original
        var info = obj.GetComponent<SegmentPoolInfo>();
        if (info == null) info = obj.AddComponent<SegmentPoolInfo>();
        info.prefabOrigin = prefab;

        // Limpiamos coleccionables viejos y generamos nuevos (si está activado el paso 3)
        ClearCollectibles(obj);
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

        // Limpiar coleccionables que queden
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
        if (snapToGeneratorXY) spawnPos.x = transform.position.x;
        if (useFixedY) spawnPos.y = fixedY;

        GameObject newSegment = GetFromPool(prefab, spawnPos, transform.rotation);
        activeSegments.Add(newSegment);

        float advance = GetAdvanceLength(newSegment);
        nextSpawnPoint = spawnPos + transform.forward * advance;

        // Reciclado con pooling
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
        if (collectiblePrefab == null) return;
        if (carrilesX == null || carrilesX.Length == 0) return;
        if (collectiblesPerSegment <= 0) return;

        // Distribuimos los coleccionables a lo largo del tramo
        for (int i = 0; i < collectiblesPerSegment; i++)
        {
            float frac = (i + 1f) / (collectiblesPerSegment + 1f); // 0..1
            float zPos = segment.transform.position.z + segmentLength * frac;

            float laneX = carrilesX[Random.Range(0, carrilesX.Length)];

            Vector3 pos = new Vector3(
                laneX,
                segment.transform.position.y + collectibleOffsetY,
                zPos
            );

            GameObject item = Instantiate(collectiblePrefab, pos, Quaternion.identity);
            item.transform.SetParent(segment.transform); // para que viaje con el tramo
        }
    }

    void ClearCollectibles(GameObject segment)
    {
        // Destruye todos los hijos que tengan CollectiblePizza
        var collectibles = segment.GetComponentsInChildren<CollectiblePizza>(true);
        foreach (var c in collectibles)
        {
            if (c != null && c.gameObject != segment)
            {
                Destroy(c.gameObject);
            }
        }
    }
}

// Este componente marca un objeto como coleccionable hijo de un tramo
public class CollectibleMarker : MonoBehaviour
{
}

// Este componente guarda de qué prefab salió el tramo (para el pool)
public class SegmentPoolInfo : MonoBehaviour
{
    [HideInInspector] public GameObject prefabOrigin;
}
