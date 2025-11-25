using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBoostCollectible : MonoBehaviour
{
    public float tiempoExtra = 5f;
    public int puntos = 0;
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

        if (GameManager.Inst != null && GameManager.Inst.enJuego)
        {
            GameManager.Inst.tiempo += tiempoExtra;
            if (puntos > 0)
                GameManager.Inst.SumarPuntos(puntos);
        }

        Destroy(gameObject);
    }
}
