using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;

    public int puntuacion;
    public float tiempo = 60f;
    public bool enJuego = true;

    public Text textoPuntuacion;
    public Text textoTiempo;

    void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
    }

    void Update()
    {
        if (!enJuego) return;
        tiempo -= Time.deltaTime;
        if (tiempo <= 0f)
        {
            tiempo = 0f;
            enJuego = false;
        }
        if (textoTiempo != null) textoTiempo.text = Mathf.CeilToInt(tiempo).ToString();
        if (textoPuntuacion != null) textoPuntuacion.text = puntuacion.ToString();
    }

    public void SumarPuntos(int cantidad)
    {
        puntuacion += cantidad;
    }

    public void SumarPuntosPorDistancia(float metros)
    {
        puntuacion += Mathf.RoundToInt(metros);
    }

    public void Reiniciar(float nuevoTiempo)
    {
        puntuacion = 0;
        tiempo = nuevoTiempo;
        enJuego = true;
    }
}
