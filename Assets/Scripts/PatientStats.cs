using UnityEngine;

public class PatientStats : MonoBehaviour
{
    [Header("Stats de Xeron")]
    public float bloodLevel = 100f; 
    public float stressLevel = 0f;  
    public bool isBleeding = true;

    [Header("Réglages")]
    public float hemorrhageSpeed = 1.5f; 

    void Update()
    {
        if (isBleeding && bloodLevel > 0)
        {
            bloodLevel -= hemorrhageSpeed * Time.deltaTime;
        }

        if (bloodLevel <= 0)
        {
            Debug.Log("Xeron est tombé dans le coma...");
        }
    }
}