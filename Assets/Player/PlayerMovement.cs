using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidade de movimento
    private Vector3 movementInput; // Input do jogador
    private Rigidbody rb; // Componente Rigidbody do personagem
    private Animator animator; // Componente Animator para controlar anima��es
    private SpriteRenderer spriteRenderer; // Para espelhar as anima��es

    private Vector2 lastDirection = Vector2.down; // Dire��o final do movimento (inicia como "baixo")

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); // Certifique-se de que o Animator est� sendo atribu�do
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMove(InputValue inputValue)
    {
        // Captura o input do jogador (X e Z)
        Vector2 input = inputValue.Get<Vector2>();
        movementInput = new Vector3(input.x, 0f, input.y);

        // Atualiza a �ltima dire��o apenas se o jogador estiver se movendo
        if (input.magnitude > 0.1f)
        {
            lastDirection = input; // Atualiza a dire��o
        }

        UpdateDirection(input); // Atualiza os par�metros de dire��o no Animator
    }

    void FixedUpdate()
    {
        // Movimenta o personagem
        Vector3 movePosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(movePosition);

        // Atualiza anima��o de andar/parar
        UpdateAnimation();
    }

    void UpdateDirection(Vector2 input)
    {
        if (input != Vector2.zero) // Quando o personagem est� se movendo
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
        // Determina se o personagem est� se movendo
        bool isMoving = movementInput.magnitude > 0;

        animator.SetBool("isMoving", isMoving); // Atualiza o par�metro para controle geral de movimento

        if (!isMoving) // Quando o personagem est� parado
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
