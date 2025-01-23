using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float destroyDelay = 2f; // Tempo antes da bala desaparecer ap�s o impacto

    private void OnCollisionEnter(Collision collision)
    {
        // Verifica se a bala colidiu com a �gua
        if (collision.gameObject.CompareTag("Water"))
        {
            Debug.Log("Bala atingiu a �gua. Ser� destru�da em breve.");
            Destroy(gameObject, destroyDelay); // Destroi a bala ap�s o delay
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se a bala entrou em uma �rea de �gua usando Trigger
        if (other.CompareTag("Water"))
        {
            Debug.Log("Bala entrou na �gua (Trigger). Ser� destru�da em breve.");
            Destroy(gameObject, destroyDelay); // Destroi a bala ap�s o delay
        }
    }
}
