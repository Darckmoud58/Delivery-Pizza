using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
     public Transform objetivo;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float suavizado = 5f;
    // Start is called before the first frame update
    void Start()
    {
        if (objetivo == null)
        {
            var moto = FindObjectOfType<MovimientoMoto>();
            if (moto != null) objetivo = moto.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        Vector3 posicionDeseada = objetivo.position + offset;
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
    }
}
