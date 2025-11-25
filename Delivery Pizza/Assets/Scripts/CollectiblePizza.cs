using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblePizza : MonoBehaviour
{
    // Start is called before the first frame update
    public int puntos = 10;
    public float tiempoExtra = 0f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (GetComponent<CollectibleMarker>() == null)
            gameObject.AddComponent<CollectibleMarker>();
    }

    private void OnTriggerEnter(Collider other)
    {
         if (!other.CompareTag("Player")) return;

        if (GameManager.Inst != null && GameManager.Inst.enJuego)
        {
            GameManager.Inst.SumarPuntos(puntos);
            GameManager.Inst.tiempo += tiempoExtra;
        }

        Destroy(gameObject);
    }
}
