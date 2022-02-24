using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Valve.VR.Extras;
using Valve.VR;

public class ENSEMBLE_UIHandler : MonoBehaviour
{
    public GameObject ensembleUI;
    public bool activeUI;
    public GameObject leftHand;


    // Start is called before the first frame update

    void Start()
    {
        //Display();
    }
    void Update()
    {
        Orientation();
        
    }

    public void Display()
    {
        
        if (activeUI)
        {
            ensembleUI.SetActive(false);
            activeUI = false;
        } else if (!activeUI)
        {
            ensembleUI.SetActive(true);
            activeUI = true;
        }
    }

    private void Orientation()
    {
        ensembleUI.transform.SetParent(leftHand.transform);
        ensembleUI.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
        ensembleUI.transform.localPosition = new Vector3(0.0f, 0.0f, 0.5f);
        ensembleUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);
    }
}
