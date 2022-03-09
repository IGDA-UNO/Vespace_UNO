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
    //public bool display;

    public SteamVR_LaserPointer laserPointer;
    public Color hit_color;
    public Color miss_color;

    private SteamVR_Action_Boolean actionBoolean;


    // Start is called before the first frame update

    void Start()
    {
        //Display();
        activeUI = false;
        actionBoolean = SteamVR_Actions._default.GrabGrip;
        laserPointer.color = miss_color;
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }
    void Update()
    {
        
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
        ensembleUI.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
        ensembleUI.transform.localPosition = new Vector3(0.2f, 0.3f, 0.2f);
        ensembleUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);
    }

    //LaserPointer functions overload:
    //Surcharge des fonctions de LaserPointer : 

    public void PointerClick(object sender, PointerEventArgs e)
    {
        EnsembleObject ensemble = e.target.GetComponent<EnsembleObject>();
        if (ensemble != null)
        {
            //The object is an Ensemble object
            
            Orientation();
            Display();
            //omekaPad.displayItem(voo.OmekaVirtualObjectID);
        }
        if (e.target.gameObject.layer == 5)
        {
            //layer of the UI: 5
            //layer de l'UI : 5 
            Button bouton = e.target.GetComponent<Button>();
            if (bouton != null)
            {
                bouton.onClick.Invoke();
            }
        }
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        laserPointer.color = hit_color;
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        laserPointer.color = miss_color;
    }
}
