using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float maxSpeed = 20f; // Velocidade m�xima do navio
    public float acceleration = 5f; // Taxa de acelera��o
    public float deceleration = 3f; // Taxa de desacelera��o
    public float baseRotationSpeed = 100f; // Velocidade de rota��o base
    public float minRotationSpeedFactor = 0.3f; // Fator m�nimo para a rota��o em alta velocidade
    public float stoppedRotationSpeedFactor = 0.5f; // Fator de rota��o quando o navio est� parado
    private bool isControlled = false; // Define se o navio est� sendo controlado pelo jogador

    private float currentSpeed = 0f; // Velocidade atual do navio
    private float targetSpeed = 0f; // Velocidade desejada (controlada pelo jogador)
    private float rotationInput; // Input de rota��o

    void Update()
    {
        if (!isControlled) return; // Ignora o input se o navio n�o estiver sendo controlado

        // Captura o input do jogador para velocidade e rota��o
        if (Input.GetKey(KeyCode.W))
        {
            targetSpeed = maxSpeed; // Aumenta a velocidade para o m�ximo
        }
        else if (Input.GetKey(KeyCode.S))
        {
            targetSpeed = 0f; // Reduz a velocidade para parar
        }

        rotationInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        if (!isControlled) return; // Ignora a f�sica se o navio n�o estiver sendo controlado

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

        // Ajusta a velocidade de rota��o com base na velocidade atual
        float rotationSpeed;
        if (currentSpeed <= 0.1f) // Se o navio est� parado ou quase parado
        {
            rotationSpeed = baseRotationSpeed * stoppedRotationSpeedFactor;
        }
        else // Se o navio est� em movimento
        {
            rotationSpeed = Mathf.Lerp(baseRotationSpeed * minRotationSpeedFactor, baseRotationSpeed, 1f - (currentSpeed / maxSpeed));
        }

        // Aplica a movimenta��o
        Vector3 moveDirection = transform.forward * currentSpeed * Time.fixedDeltaTime;
        transform.position += moveDirection;

        // Aplica a rota��o
        transform.Rotate(0f, rotationInput * rotationSpeed * Time.fixedDeltaTime, 0f);
    }

    // M�todo p�blico para ativar o controle do navio
    public void SetControlled(bool controlled)
    {
        isControlled = controlled;
    }
}
