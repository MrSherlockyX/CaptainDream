using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float destroyDelay = 2f; // Tempo antes da bala desaparecer após o impacto

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se a bala colidiu com a água
        if (collision.gameObject.CompareTag("Water"))
        {
            Debug.Log("Bala atingiu a água. Será destruída em breve.");
            Destroy(gameObject, destroyDelay); // Destroi a bala após o delay
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se a bala entrou em uma área de água usando Trigger
        if (other.CompareTag("Water"))
        {
            Debug.Log("Bala entrou na água (Trigger). Será destruída em breve.");
            Destroy(gameObject, destroyDelay); // Destroi a bala após o delay
        }
    }
}
