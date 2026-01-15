using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string[] GetAssets = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Images/QTE"});
        print(GetAssets.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
