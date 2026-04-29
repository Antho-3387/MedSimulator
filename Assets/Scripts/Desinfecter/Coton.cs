using UnityEngine;

public class Coton : MonoBehaviour
{
    public CanvasGroup cg;

    void Update()
    {
        transform.position = Input.mousePosition;

        if (Input.GetMouseButton(0))
            cg.alpha = 1f;
        else
            cg.alpha = 0f;
    }
}