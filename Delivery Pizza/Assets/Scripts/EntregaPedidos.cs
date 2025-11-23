using UnityEngine;

public class EntregaPedidos : MonoBehaviour
{
    public int puntosPorEntrega = 50;
    public bool tienePedido;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pedido"))
        {
            tienePedido = true;
        }
        else if (other.CompareTag("Entrega"))
        {
            if (tienePedido)
            {
                tienePedido = false;
                if (GameManager.Inst != null) GameManager.Inst.SumarPuntos(puntosPorEntrega);
            }
        }
    }
}
