using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPowerUp : MonoBehaviour
{
    public float duracion = 5f; // segundos de magneto
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (GetComponent<CollectibleMarker>() == null)
            gameObject.AddComponent<CollectibleMarker>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Inst == null || !GameManager.Inst.enJuego) return;

        MagnetEffect magnet = other.GetComponent<MagnetEffect>();
        if (magnet != null)
        {
            magnet.Activar(duracion);
        }

        Destroy(gameObject);
    }
}
