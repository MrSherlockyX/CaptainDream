using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidade de movimento
    private Vector3 movementInput; // Input do jogador
    private Rigidbody rb; // Componente Rigidbody do personagem
    private Animator animator; // Componente Animator para controlar animações
    private SpriteRenderer spriteRenderer; // Para espelhar as animações

    private Vector2 lastDirection = Vector2.down; // Direção final do movimento (inicia como "baixo")

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // Certifique-se de que o Animator está sendo atribuído
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMove(InputValue inputValue)
    {
        // Captura o input do jogador (X e Z)
        Vector2 input = inputValue.Get<Vector2>();
        movementInput = new Vector3(input.x, 0f, input.y);

        // Atualiza a última direção apenas se o jogador estiver se movendo
        if (input.magnitude > 0.1f)
        {
            lastDirection = input; // Atualiza a direção
        }

        UpdateDirection(input); // Atualiza os parâmetros de direção no Animator
    }

    void FixedUpdate()
    {
        // Movimenta o personagem
        Vector3 movePosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(movePosition);

        // Atualiza animação de andar/parar
        UpdateAnimation();
    }

    void UpdateDirection(Vector2 input)
    {
        if (input != Vector2.zero) // Quando o personagem está se movendo
        {
            if (input.y > 0) // Para cima
            {
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", 1);
                animator.Play("Walk_Up");
            }
            else if (input.y < 0) // Para baixo
            {
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", -1);
                animator.Play("Walk_Down");
            }
            else if (input.x > 0) // Para direita
            {
                animator.SetFloat("MoveX", 1);
                animator.SetFloat("MoveY", 0);
                animator.Play("Walk_Right");
            }
            else if (input.x < 0) // Para esquerda
            {
                animator.SetFloat("MoveX", -1);
                animator.SetFloat("MoveY", 0);
                animator.Play("Walk_Left");
            }
        }
    }

    void UpdateAnimation()
    {
        // Determina se o personagem está se movendo
        bool isMoving = movementInput.magnitude > 0;

        animator.SetBool("isMoving", isMoving); // Atualiza o parâmetro para controle geral de movimento

        if (!isMoving) // Quando o personagem está parado
        {
            if (lastDirection.y > 0) // Idle para cima
            {
                animator.Play("Idle_Up");
            }
            else if (lastDirection.y < 0) // Idle para baixo
            {
                animator.Play("Idle_Down");
            }
            else if (lastDirection.x > 0) // Idle para direita
            {
                animator.Play("Idle_Right");
            }
            else if (lastDirection.x < 0) // Idle para esquerda
            {
                animator.Play("Idle_Left");
            }
        }
    }
}
