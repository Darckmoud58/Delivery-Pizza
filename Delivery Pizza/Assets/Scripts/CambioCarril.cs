using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambioCarril : MonoBehaviour
{
    public float distanciaCarril = 3f;
    public float velocidadCambio = 10f;
    public float xCentro = 0f;
    public bool usarCarrilesExplicitos = true;
    public float[] carrilesX = new float[] { -2.3867f, -1.13f };
    public bool bloquearPosicionInicial = true;

    private int carrilActual = 0; // 0 = izquierda, 1 = derecha
    private Vector3 posicionObjetivo;
    private float xBase;


    // Start is called before the first frame update
    void Start()
    {
        // 1. OBLIGAR al código a usar estas coordenadas exactas en este orden:
        // Índice 0 (Izq): -2.3867f
        // Índice 1 (Der): -1.13f
        carrilesX = new float[] { -2.3867f, -1.13f };

        // 2. Decirle que estamos en la Izquierda (Índice 0)
        carrilActual = 0;

        // 3. Teletransportar la moto a esa posición
        float xDestino = carrilesX[carrilActual];
        transform.position = new Vector3(xDestino, transform.position.y, transform.position.z);

        // 4. Asegurarnos de que el "objetivo" interno también esté sincronizado
        posicionObjetivo = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // 1. Detectar Input (Izquierda o Derecha)
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (carrilActual > 0)
            {
                carrilActual--; // Bajar índice (ir a la izquierda)
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (carrilActual < carrilesX.Length - 1)
            {
                carrilActual++; // Subir índice (ir a la derecha)
            }
        }

        // 2. Calcular la posición X deseada
        // Esto busca en tu lista de carriles cuál es la X a la que debemos ir
        float xDestino = carrilesX[carrilActual];

        // 3. Mover suavemente SOLO en el eje X
        // Usamos Mathf.Lerp solo para el número X, no para todo el Vector3
        float nuevaX = Mathf.Lerp(transform.position.x, xDestino, velocidadCambio * Time.deltaTime);

        // 4. Aplicar el movimiento
        // Mantenemos la Y y la Z actuales (así la moto sigue avanzando por el otro script)
        transform.position = new Vector3(nuevaX, transform.position.y, transform.position.z);
    }

    void ActualizarPosicionObjetivo()
    {
        // Carril 0 = izquierda (-distanciaCarril)
        // Carril 1 = derecha   (+distanciaCarril)
        if (usarCarrilesExplicitos && carrilesX != null && carrilesX.Length > 0)
        {
            float xTarget = carrilesX[carrilActual];
            posicionObjetivo = new Vector3(xTarget, transform.position.y, transform.position.z);
        }
        else
        {
            float xOffset = (carrilActual == 0) ? -distanciaCarril : distanciaCarril;
            posicionObjetivo = new Vector3(xBase + xOffset, transform.position.y, transform.position.z);
        }
    }
}
