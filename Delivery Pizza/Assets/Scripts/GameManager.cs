using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;

    [Header("Estado de juego")]
    public int puntuacion;
    public float tiempo = 60f;
    public bool enJuego = true;

    [Header("UI HUD")]
    public TMP_Text textoPuntuacion;
    public TMP_Text textoTiempo;

    [Header("UI Game Over")]
    public GameObject panelGameOver;
    public TMP_Text textoPuntuacionFinal;
    public TMP_Text textoRecord;

    public string nombreEscenaJuego = "SampleScene";

    void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Destroy(gameObject);
            return;
        }
        Inst = this;
        // Opcional: si quieres que sobreviva a cambios de escena
        // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ActualizarUI();

        if (panelGameOver != null)
            panelGameOver.SetActive(false);
    }

    void Update()
    {
        if (!enJuego) return;

        tiempo -= Time.deltaTime;

        if (tiempo <= 0f)
        {
            tiempo = 0f;
            enJuego = false;
            GameOver();
        }

        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (textoTiempo != null)
            textoTiempo.text = Mathf.CeilToInt(tiempo).ToString();

        if (textoPuntuacion != null)
            textoPuntuacion.text = puntuacion.ToString();
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

    void GameOver()
    {
        // Guardar y mostrar récord
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (puntuacion > highScore)
        {
            highScore = puntuacion;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        if (panelGameOver != null)
            panelGameOver.SetActive(true);

        if (textoPuntuacionFinal != null)
            textoPuntuacionFinal.text = puntuacion.ToString();

        if (textoRecord != null)
            textoRecord.text = highScore.ToString();
    }

    // Llamado por el botón "Reintentar"
    public void BotonReintentar()
    {
        SceneManager.LoadScene(nombreEscenaJuego);
    }
}
