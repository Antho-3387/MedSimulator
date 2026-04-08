using UnityEngine;

public class BandageLogic : MonoBehaviour
{
    public bool isBandageCorrect = false;

    public void CheckBandageQuality(float accuracy)
    {
        if (accuracy > 0.8f) 
        {
            isBandageCorrect = true;
            Debug.Log("Bandage réussi !");
        }
        else
        {
            isBandageCorrect = false;
            Debug.Log("Bandage trop lâche...");
        }
    }
}