using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    [Tooltip("Transform del jugador a seguir")]
    public Transform target;

    [Header("Ajustes de Seguimiento")]
    [Tooltip("Suavidad de la cámara (menor valor = más rápida)")]
    public float smoothTime = 0.15f;

    [Tooltip("Desplazamiento respecto al jugador")]
    public Vector3 offset = new Vector3(0f, 0.35f, -10f);

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        BuscarJugador();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            BuscarJugador();
            if (target == null) return;
        }

        Vector3 targetPosition = target.position + offset;
        targetPosition.z = offset.z; // Mantener la profundidad de la cámara constante

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private void BuscarJugador()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        if (player != null)
        {
            target = player.transform;
        }
    }
}
