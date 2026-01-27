using UnityEngine;

public class PatientStats : MonoBehaviour
{
    [Header("Stats de Xeron")]
    public float bloodLevel = 100f; // 100% au début
    public float stressLevel = 0f;  // 0% au début
    public bool isBleeding = true;

    [Header("Réglages")]
    public float hemorrhageSpeed = 1.5f; // Perte par seconde

    void Update()
    {
        if (isBleeding && bloodLevel > 0)
        {
            bloodLevel -= hemorrhageSpeed * Time.deltaTime;
        }

        if (bloodLevel <= 0)
        {
            // Appeler ici la fonction de Game Over / Coma
            Debug.Log("Xeron est tombé dans le coma...");
        }
    }
}