using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetEffect : MonoBehaviour
{
    [Header("Magneto")]
    public float radio = 5f;            // qué tan lejos alcanza
    public float velocidadArrastre = 10f; // qué tan rápido vienen las pizzas

    private float tiempoRestante = 0f;

    public bool Activo => tiempoRestante > 0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (tiempoRestante <= 0f) return;

        tiempoRestante -= Time.deltaTime;

        // Atraer pizzas cercanas
        Collider[] hits = Physics.OverlapSphere(transform.position, radio);
        foreach (var h in hits)
        {
            CollectiblePizza pizza = h.GetComponent<CollectiblePizza>();
            if (pizza != null)
            {
                h.transform.position = Vector3.MoveTowards(
                    h.transform.position,
                    transform.position,
                    velocidadArrastre * Time.deltaTime
                );
            }
        }

    }

    public void Activar(float duracion)
    {
        // Si ya está activo, extiende la duración
        tiempoRestante = Mathf.Max(tiempoRestante, duracion);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}
