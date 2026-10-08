using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class MovePlayer : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    [Tooltip("Velocidad horizontal del jugador")]
    public float velocidad = 2.2f;

    [Tooltip("Fuerza del salto")]
    public float salto = 6.2f;

    [Header("Detección de Suelo")]
    [Tooltip("Capa que representa el suelo (Tilemap / plataformas). Si es Nothing detecta cualquier superficie.")]
    public LayerMask capaSuelo;

    private ControlesPlayer controlPlayer;
    private Vector2 movimiento;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;
    private float tiempoUltimoSalto = 0f;

    void Awake()
    {
        controlPlayer = new ControlesPlayer();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        // Evitar que el personaje gire en el eje Z
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (salto <= 0f)
            salto = 6.2f;

        if (velocidad <= 0f)
            velocidad = 2.2f;
    }

    void Update()
    {
        if (controlPlayer != null)
        {
            movimiento = controlPlayer.Player.Move.ReadValue<Vector2>();
            this.cambioEstado();
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Movimiento horizontal manteniendo la velocidad vertical (física de salto y caída)
            rb.linearVelocity = new Vector2(movimiento.x * velocidad, rb.linearVelocity.y);
        }
    }

    private void cambioEstado()
    {
        if (animator == null) return;

        bool caminando = Mathf.Abs(movimiento.x) > 0.05f;

        animator.SetFloat("x", movimiento.x);
        animator.SetFloat("y", 0f);

        // Si camina -> descansando = false (reproduce player_este o player_oeste)
        // Si está quieto -> descansando = true (reproduce player_descansando)
        animator.SetBool("descansando", !caminando);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    private void OnJump(InputValue value)
    {
        Jump();
    }

    public void Jump()
    {
        if (rb == null) return;

        // Evitar dobles pulsaciones en el mismo instante
        if (Time.time - tiempoUltimoSalto < 0.2f) return;

        // Verificar que esté en el piso antes de permitir el salto
        if (EstaEnPiso())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, salto);
            tiempoUltimoSalto = Time.time;
            ChecaColision.isPiso = false;
        }
    }

    public bool EstaEnPiso()
    {
        // 1. Detección por el script ChecaColision
        if (ChecaColision.isPiso)
        {
            return true;
        }

        // 2. Detección física directa en los pies del jugador
        if (col != null)
        {
            Bounds b = col.bounds;
            Vector2 centroPies = new Vector2(b.center.x, b.min.y);
            Vector2 tamañoCaja = new Vector2(b.size.x * 0.85f, 0.05f);

            int capa = capaSuelo.value != 0 ? capaSuelo.value : ~LayerMask.GetMask("Ignore Raycast");
            Collider2D hit = Physics2D.OverlapBox(centroPies, tamañoCaja, 0f, capa);

            if (hit != null && hit.gameObject != gameObject && !hit.isTrigger)
            {
                return true;
            }
        }

        return false;
    }

    void OnEnable()
    {
        if (controlPlayer == null)
            controlPlayer = new ControlesPlayer();

        controlPlayer.Enable();
        controlPlayer.Player.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        if (controlPlayer != null)
        {
            controlPlayer.Player.Jump.performed -= OnJump;
            controlPlayer.Disable();
        }
    }

    void OnDestroy()
    {
        controlPlayer?.Dispose();
    }
}
