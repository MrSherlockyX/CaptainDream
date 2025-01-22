using UnityEngine;

public class EnterShip : MonoBehaviour
{
    public GameObject player; // Referência ao jogador
    public GameObject ship;   // Referência ao navio
    public GameObject playerCamera; // Cinemachine do jogador
    public GameObject shipCamera;   // Cinemachine do navio

    private bool nearShip = false; // Indica se o jogador está próximo ao navio
    private bool nearDockingPoint = false; // Indica se o navio está em um ponto de atracagem

    void Update()
    {
        if (nearShip && Input.GetKeyDown(KeyCode.E)) // Entrar no navio
        {
            Debug.Log("Entrando no navio...");
            ToggleControl(true);
        }
        else if (nearDockingPoint && Input.GetKeyDown(KeyCode.E)) // Atracar e sair do navio
        {
            Debug.Log("Tentativa de atracar detectada.");
            DockAndExit();
        }
    }


    void ToggleControl(bool controllingShip)
    {
        Debug.Log("Alternando controle: " + (controllingShip ? "Navio" : "Jogador"));

        player.SetActive(!controllingShip);
        playerCamera.SetActive(!controllingShip);

        ship.SetActive(controllingShip);
        shipCamera.SetActive(controllingShip);

        ship.GetComponent<ShipMovement>().SetControlled(controllingShip);
    }


    void DockAndExit()
    {
        Debug.Log("Tentando atracar...");

        if (!nearDockingPoint)
        {
            Debug.Log("Erro: Navio não está em um ponto de atracagem.");
            return; // Certifique-se de que só executa se o navio estiver em um DockingPoint
        }

        Debug.Log("Atracando o navio e transferindo o controle para o jogador.");

        // Alterna o controle de volta para o jogador
        ToggleControl(false);

        // Posiciona o jogador no ponto de atracagem mais próximo
        Transform dockingPoint = GetNearestDockingPoint();
        if (dockingPoint != null)
        {
            player.transform.position = dockingPoint.position;
            player.transform.rotation = dockingPoint.rotation;
            Debug.Log("Jogador posicionado no ponto de atracagem.");
        }
        else
        {
            Debug.LogError("Erro: Nenhum ponto de atracagem encontrado.");
        }
    }


    Transform GetNearestDockingPoint()
    {
        GameObject dockingPoint = GameObject.FindGameObjectWithTag("DockingPoint");
        if (dockingPoint != null)
        {
            return dockingPoint.transform;
        }

        Debug.LogError("Nenhum ponto de atracagem encontrado com a tag 'DockingPoint'.");
        return null;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            nearShip = true;
            Debug.Log("Player está próximo do navio.");
        }
        else if (other.CompareTag("DockingPoint"))
        {
            nearDockingPoint = true;
            Debug.Log("Navio entrou em um ponto de atracagem.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            nearShip = false;
            Debug.Log("Player saiu da área do navio.");
        }
        else if (other.CompareTag("DockingPoint"))
        {
            nearDockingPoint = false;
            Debug.Log("Navio saiu do ponto de atracagem.");
        }
    }

}