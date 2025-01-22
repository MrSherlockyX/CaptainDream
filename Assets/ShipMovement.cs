using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float maxSpeed = 20f; // Velocidade máxima do navio
    public float acceleration = 5f; // Taxa de aceleração
    public float deceleration = 3f; // Taxa de desaceleração
    public float baseRotationSpeed = 100f; // Velocidade de rotação base
    public float minRotationSpeedFactor = 0.3f; // Fator mínimo para a rotação em alta velocidade
    public float stoppedRotationSpeedFactor = 0.5f; // Fator de rotação quando o navio está parado
    private bool isControlled = false; // Define se o navio está sendo controlado pelo jogador

    private float currentSpeed = 0f; // Velocidade atual do navio
    private float targetSpeed = 0f; // Velocidade desejada (controlada pelo jogador)
    private float rotationInput; // Input de rotação

    void Update()
    {
        if (!isControlled) return; // Ignora o input se o navio não estiver sendo controlado

        // Captura o input do jogador para velocidade e rotação
        if (Input.GetKey(KeyCode.W))
        {
            targetSpeed = maxSpeed; // Aumenta a velocidade para o máximo
        }
        else if (Input.GetKey(KeyCode.S))
        {
            targetSpeed = 0f; // Reduz a velocidade para parar
        }

        rotationInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        if (!isControlled) return; // Ignora a física se o navio não estiver sendo controlado

        // Gradualmente ajusta a velocidade atual para a velocidade desejada
        if (currentSpeed < targetSpeed)
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
            if (currentSpeed > targetSpeed) currentSpeed = targetSpeed;
        }
        else if (currentSpeed > targetSpeed)
        {
            currentSpeed -= deceleration * Time.fixedDeltaTime;
            if (currentSpeed < targetSpeed) currentSpeed = targetSpeed;
        }

        // Ajusta a velocidade de rotação com base na velocidade atual
        float rotationSpeed;
        if (currentSpeed <= 0.1f) // Se o navio está parado ou quase parado
        {
            rotationSpeed = baseRotationSpeed * stoppedRotationSpeedFactor;
        }
        else // Se o navio está em movimento
        {
            rotationSpeed = Mathf.Lerp(baseRotationSpeed * minRotationSpeedFactor, baseRotationSpeed, 1f - (currentSpeed / maxSpeed));
        }

        // Aplica a movimentação
        Vector3 moveDirection = transform.forward * currentSpeed * Time.fixedDeltaTime;
        transform.position += moveDirection;

        // Aplica a rotação
        transform.Rotate(0f, rotationInput * rotationSpeed * Time.fixedDeltaTime, 0f);
    }

    // Método público para ativar o controle do navio
    public void SetControlled(bool controlled)
    {
        isControlled = controlled;
    }
}
