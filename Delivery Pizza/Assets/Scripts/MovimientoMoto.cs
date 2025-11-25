using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoMoto : MonoBehaviour
{
    [Header("Velocidad")]
    public float velocidadInicial = 10f;
    public float aceleracion = 0.5f;
    public float velocidadMaxima = 30f;

    private float velocidadActual;

    void Start()
    {
        // Empezamos con la velocidad inicial
        velocidadActual = velocidadInicial;
    }

    void Update()
    {

        // Si el juego está parado, no avanzamos
        if (GameManager.Inst != null && !GameManager.Inst.enJuego)
            return;

        // Aumentar velocidad con el tiempo
        velocidadActual += aceleracion * Time.deltaTime;
        velocidadActual = Mathf.Clamp(velocidadActual, 0f, velocidadMaxima);

        // Mover moto
        transform.Translate(Vector3.forward * velocidadActual * Time.deltaTime);

        // Sumar puntos por distancia
        if (GameManager.Inst != null && GameManager.Inst.enJuego)
        {
            GameManager.Inst.SumarPuntosPorDistancia(velocidadActual * Time.deltaTime);
        }
    }

    // Opcional: por si quieres resetear la moto sin recargar la escena
    public void ReiniciarVelocidad()
    {
        velocidadActual = velocidadInicial;
    }
}
