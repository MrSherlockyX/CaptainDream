using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ShipCombat : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    public GameObject leftIndicator; // Objeto de mira para o lado esquerdo
    public GameObject rightIndicator; // Objeto de mira para o lado direito
    public List<Transform> leftFirePoints; // Lista de pontos de disparo do canhão esquerdo
    public List<Transform> rightFirePoints; // Lista de pontos de disparo do canhão direito
    public Transform leftStartPoint; // Ponto inicial do lado esquerdo
    public Transform leftEndPoint; // Ponto final do lado esquerdo
    public Transform rightStartPoint; // Ponto inicial do lado direito
    public Transform rightEndPoint; // Ponto final do lado direito
    public GameObject cannonBallPrefab; // Prefab da bala de canhão
    public List<Transform> leftIndicatorTargets; // Destinos para os canhões do lado esquerdo
    public List<Transform> rightIndicatorTargets; // Destinos para os canhões do lado direito


    public float aimSpeed = 5f; // Velocidade de movimento da mira
    public float fireCooldown = 2f; // Tempo entre disparos
    public float indicatorCooldown = 3f; // Tempo antes de os indicadores se desligarem
    public float cannonBallSpeed = 10f; // Velocidade da bala de canhão
    public float arcHeight = 2f; // Altura do arco da bala

    private float leftCooldownTimer = 0f;
    private float rightCooldownTimer = 0f;
    private bool isAimingLeft = false;
    private bool isAimingRight = false;
    private bool isInSafeZone = false; // Define se está em uma safe zone
    private bool movingForward = true;

    private float leftIndicatorTimer = 0f;
    private float rightIndicatorTimer = 0f;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        // Liga a mira para o lado esquerdo
        inputActions.Player.AimL.started += ctx =>
        {
            if (!isInSafeZone && leftCooldownTimer <= 0)
                StartAiming(leftIndicator, ref isAimingLeft, ref isAimingRight, ref leftIndicatorTimer);
        };
        inputActions.Player.AimL.canceled += ctx =>
        {
            if (!isInSafeZone)
                StartIndicatorCooldown(ref leftIndicatorTimer);
        };

        // Liga a mira para o lado direito
        inputActions.Player.AimR.started += ctx =>
        {
            if (!isInSafeZone && rightCooldownTimer <= 0)
                StartAiming(rightIndicator, ref isAimingRight, ref isAimingLeft, ref rightIndicatorTimer);
        };
        inputActions.Player.AimR.canceled += ctx =>
        {
            if (!isInSafeZone)
                StartIndicatorCooldown(ref rightIndicatorTimer);
        };

        ResetIndicators();
    }

    void Update()
    {
        // Reduz os cooldowns ao longo do tempo
        if (leftCooldownTimer > 0) leftCooldownTimer -= Time.deltaTime;
        if (rightCooldownTimer > 0) rightCooldownTimer -= Time.deltaTime;

        // Desativa os indicadores após o cooldown
        if (leftIndicatorTimer > 0)
        {
            leftIndicatorTimer -= Time.deltaTime;
            if (leftIndicatorTimer <= 0)
            {
                leftIndicator.SetActive(false);
                Debug.Log("Indicador esquerdo desativado por cooldown.");
            }
        }

        if (rightIndicatorTimer > 0)
        {
            rightIndicatorTimer -= Time.deltaTime;
            if (rightIndicatorTimer <= 0)
            {
                rightIndicator.SetActive(false);
                Debug.Log("Indicador direito desativado por cooldown.");
            }
        }

        // Controle da mira dinâmica
        if (isAimingLeft)
        {
            Aim(leftIndicator, leftStartPoint.position, leftEndPoint.position);
        }

        if (isAimingRight)
        {
            Aim(rightIndicator, rightStartPoint.position, rightEndPoint.position);
        }

        // Disparo
        if (inputActions.Player.Fire.triggered && !isInSafeZone)
        {
            if (leftIndicator.activeSelf && leftCooldownTimer <= 0)
            {
                FireCannons(leftFirePoints, leftIndicatorTargets); // Passa a lista de destinos
                leftCooldownTimer = fireCooldown;
                ResetIndicators();
            }

            if (rightIndicator.activeSelf && rightCooldownTimer <= 0)
            {
                FireCannons(rightFirePoints, rightIndicatorTargets); // Passa a lista de destinos
                rightCooldownTimer = fireCooldown;
                ResetIndicators();
            }
        }

    }

    void StartAiming(GameObject indicator, ref bool currentSide, ref bool otherSide, ref float indicatorTimer)
    {
        DeactivateIndicators(); // Desativa o outro lado automaticamente
        currentSide = true;
        otherSide = false;
        indicator.SetActive(true);
        indicatorTimer = 0f; // Reseta o cooldown
        Debug.Log($"{indicator.name} ativado.");
    }

    void StartIndicatorCooldown(ref float indicatorTimer)
    {
        indicatorTimer = indicatorCooldown;
        Debug.Log($"Cooldown iniciado. Indicador será desativado em {indicatorCooldown} segundos.");
    }

    void Aim(GameObject indicator, Vector3 startPoint, Vector3 endPoint)
    {
        float step = aimSpeed * Time.deltaTime;
        if (movingForward)
        {
            indicator.transform.position = Vector3.MoveTowards(indicator.transform.position, endPoint, step);
            if (Vector3.Distance(indicator.transform.position, endPoint) < 0.1f)
            {
                movingForward = false; // Inverte a direção ao atingir o ponto final
            }
        }
        else
        {
            indicator.transform.position = Vector3.MoveTowards(indicator.transform.position, startPoint, step);
            if (Vector3.Distance(indicator.transform.position, startPoint) < 0.1f)
            {
                movingForward = true; // Inverte a direção ao atingir o ponto inicial
            }
        }
    }

    void FireCannons(List<Transform> firePoints, List<Transform> targets)
    {
        for (int i = 0; i < firePoints.Count; i++)
        {
            // Garante que cada canhão tenha um destino correspondente
            if (i >= targets.Count) break;

            Transform firePoint = firePoints[i];
            Transform target = targets[i];

            // Cria a bala de canhão no ponto de disparo correspondente
            GameObject cannonBall = Instantiate(cannonBallPrefab, firePoint.position, Quaternion.identity);

            // Movimenta a bala manualmente para o destino
            StartCoroutine(MoveCannonBall(cannonBall, firePoint.position, target.position));
        }
    }

    System.Collections.IEnumerator MoveCannonBall(GameObject cannonBall, Vector3 startPosition, Vector3 targetPosition)
    {
        float travelTime = Vector3.Distance(startPosition, targetPosition) / cannonBallSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            elapsedTime += Time.deltaTime;

            // Calcula o progresso da bala no tempo (0 a 1)
            float t = elapsedTime / travelTime;

            // Interpolação linear para o movimento horizontal
            Vector3 horizontalPosition = Vector3.Lerp(startPosition, targetPosition, t);

            // Adiciona o arco no eixo vertical
            float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
            horizontalPosition.y += height;

            // Atualiza a posição da bala
            cannonBall.transform.position = horizontalPosition;

            yield return null;
        }

        // Remova a linha que destrói a bala automaticamente
        // Destroy(cannonBall);
    }





    void DeactivateIndicators()
    {
        isAimingLeft = false;
        isAimingRight = false;
        leftIndicator.SetActive(false);
        rightIndicator.SetActive(false);
        ResetIndicators();
        Debug.Log("Indicadores desativados e reposicionados.");
    }

    void ResetIndicators()
    {
        leftIndicator.transform.position = leftStartPoint.position;
        rightIndicator.transform.position = rightStartPoint.position;
        leftIndicator.SetActive(false);
        rightIndicator.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isInSafeZone = true;
            DeactivateIndicators();
            Debug.Log("Entrou em uma zona segura.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isInSafeZone = false;
            Debug.Log("Saiu de uma zona segura.");
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
