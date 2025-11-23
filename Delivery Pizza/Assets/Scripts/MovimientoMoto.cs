using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovimientoMoto : MonoBehaviour
{
    public float velocidad = 10f;
    public float aceleracion = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocidad += aceleracion * Time.deltaTime;
        transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
        if (GameManager.Inst != null && GameManager.Inst.enJuego)
        {
            GameManager.Inst.SumarPuntosPorDistancia(velocidad * Time.deltaTime);
        }
    }
}
