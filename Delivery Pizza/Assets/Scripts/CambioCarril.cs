using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambioCarril : MonoBehaviour
{
    public float distanciaCarril = 3f; 
    public float velocidadCambio = 10f;

    private int carrilActual = 0; // 0 = izquierda, 1 = derecha
    private Vector3 posicionObjetivo;


    // Start is called before the first frame update
    void Start()
    {
        posicionObjetivo = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Mover a la izquierda
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (carrilActual == 1)
            {
                carrilActual = 0;
                ActualizarPosicionObjetivo();
            }
        }

        // Mover a la derecha
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (carrilActual == 0)
            {
                carrilActual = 1;
                ActualizarPosicionObjetivo();
            }
        }

        // Movimiento suave entre carriles
        transform.position = Vector3.Lerp(
            transform.position,
            posicionObjetivo,
            velocidadCambio * Time.deltaTime
        );
    }

    void ActualizarPosicionObjetivo()
    {
        // Carril 0 = izquierda (-distanciaCarril)
        // Carril 1 = derecha   (+distanciaCarril)
        float x = (carrilActual == 0) ? -distanciaCarril : distanciaCarril;
        posicionObjetivo = new Vector3(x, transform.position.y, transform.position.z);
    }
}
