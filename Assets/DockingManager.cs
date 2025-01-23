using System.Collections.Generic;
using UnityEngine;

public class DockingManager : MonoBehaviour
{
    public GameObject player;
    public GameObject ship;
    public GameObject playerCamera;
    public GameObject shipCamera;

    public List<DockingPoint> dockingPoints;

    private DockingPoint currentDockingPoint;
    private DockingPoint playerDockingPoint;
    private bool isControllingShip = false;

    private float checkInterval = 0.5f; // Intervalo para verificar pontos
    private float nextCheckTime = 0f;

    void Update()
    {
        // Verifica Docking Points apenas em intervalos regulares
        if (Time.time >= nextCheckTime)
        {
            playerDockingPoint = CheckForDockingPoint(player.transform.position);
            currentDockingPoint = CheckForDockingPoint(ship.transform.position);
            nextCheckTime = Time.time + checkInterval;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isControllingShip)
            {
                if (currentDockingPoint != null)
                {
                    DockAndExit();
                }
                else
                {
                    Debug.Log("Não é possível atracar: nenhum ponto de atracagem detectado.");
                }
            }
            else
            {
                EnterShip();
            }
        }
    }

    void EnterShip()
    {
        if (isControllingShip) return;

        if (playerDockingPoint == null)
        {
            Debug.Log("Você precisa estar em um ponto de atracagem para entrar no navio.");
            return;
        }

        if (playerDockingPoint != currentDockingPoint)
        {
            Debug.Log($"O navio não está aqui. PlayerDockingPoint: {playerDockingPoint.name}, CurrentDockingPoint: {currentDockingPoint?.name ?? "Nenhum"}");
            return;
        }

        player.SetActive(false);
        ToggleControl(true);
    }

    void DockAndExit()
    {
        if (!isControllingShip) return;

        if (currentDockingPoint == null)
        {
            Debug.LogError("Nenhum ponto de atracagem ativo. Não é possível atracar.");
            return;
        }

        ship.transform.position = currentDockingPoint.shipDockPosition.position;
        ship.transform.rotation = currentDockingPoint.shipDockPosition.rotation;

        player.transform.position = currentDockingPoint.playerSpawnPosition.position;
        player.transform.rotation = currentDockingPoint.playerSpawnPosition.rotation;
        player.SetActive(true);

        ToggleControl(false);
    }

    void ToggleControl(bool controllingShip)
    {
        isControllingShip = controllingShip;
        ship.GetComponent<ShipMovement>().SetControlled(controllingShip);
        playerCamera.SetActive(!controllingShip);
        shipCamera.SetActive(controllingShip);
    }

    DockingPoint CheckForDockingPoint(Vector3 position)
    {
        float detectionRadius = 3f;
        Collider[] hitColliders = Physics.OverlapSphere(position, detectionRadius);

        foreach (var collider in hitColliders)
        {
            if (collider.TryGetComponent<DockingPoint>(out DockingPoint dockingPoint))
            {
                return dockingPoint;
            }
        }

        return null;
    }

    public bool IsPlayerControllingShip()
    {
        return isControllingShip;
    }
}
