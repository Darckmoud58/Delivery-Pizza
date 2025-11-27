using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Comportamiento")]
    public bool muerteInstantanea = false;
    public float penalizacionTiempo = 5f;
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

        if (GetComponent<ObstacleMarker>() == null)
            gameObject.AddComponent<ObstacleMarker>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Inst == null || !GameManager.Inst.enJuego) return;

        if (muerteInstantanea)
        {
            // Game Over directo
            GameManager.Inst.tiempo = 0f;
        }
        else
        {
            // Solo resta tiempo
            GameManager.Inst.tiempo = Mathf.Max(0f, GameManager.Inst.tiempo - penalizacionTiempo);
        }
         // Aquí luego podemos meter animación, sonido, cámara shake, etc.


    }
    
}
