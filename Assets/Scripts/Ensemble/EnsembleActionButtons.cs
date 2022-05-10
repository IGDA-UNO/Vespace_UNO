using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnsembleActionButtons : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       Button btn = gameObject.GetComponent<Button>();
       btn.onClick.RemoveAllListeners();
       btn.onClick.AddListener(delegate { Debug.Log("test"); });
    }
}
