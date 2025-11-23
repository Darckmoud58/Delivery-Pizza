using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambioCarril : MonoBehaviour
{
    public float distanciaCarril = 3f;
    public float velocidadCambio = 10f;
    public float xCentro = 0f;
    public bool usarCarrilesExplicitos = true;
    public float[] carrilesX = new float[] { -1.5f, 1.5f };
    public bool bloquearPosicionInicial = true;

    private int carrilActual = 0; // 0 = izquierda, 1 = derecha
    private Vector3 posicionObjetivo;
    private float xBase;


    // Start is called before the first frame update
    void Start()
    {
        // Si tienes 2 carriles, el índice 0 es Izquierda, el 1 es Derecha.
        // Vamos a forzar que empiece en la Derecha (índice 1) o Izquierda (0)
        carrilActual = 1; // 1 para empezar a la derecha, 0 para izquierda.
        
        // Opcional: Teletransportar visualmente a la moto al carril de inicio para que no se vea el salto
        if (carrilesX.Length > 0)
        {
             Vector3 inicio = transform.position;
             inicio.x = carrilesX[carrilActual];
             transform.position = inicio;
        }
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
