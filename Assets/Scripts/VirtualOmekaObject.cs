using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualOmekaObject : MonoBehaviour
{
    public int OmekaVirtualObjectID ; 
    
    private GameObject target_object ; 
    private bool displayLine = false ; 
    private LineRenderer linerenderer ; 
    private Vector3 thisCenter ; 
    private PROUVE_SceneHandler sceneHandler ; 

    public void drawLineFromObject(GameObject targetObject, float duration) {       
        target_object = targetObject ; 
        linerenderer = gameObject.AddComponent<LineRenderer>() ; 
        linerenderer.material = new Material(Shader.Find("Sprites/Default")) ;
        Color couleur = new Color(0.0f, 0.7f, 1.0f, 1.0f) ; 
        linerenderer.startColor = couleur ; 
        linerenderer.endColor = couleur ;
        linerenderer.widthMultiplier = 0.01f ; 
        thisCenter = GetComponent<Renderer>().bounds.center ; 
        var points = new Vector3[2] ; 
        points[0] = target_object.transform.position ; 
        points[1] = thisCenter ; 
        linerenderer.SetPositions(points) ;  
        displayLine=true ; 
        Invoke("destroyLine",duration) ;  
    } 

    private void destroyLine() {
        displayLine = false ; 
        Destroy(linerenderer) ; 
    }

    void Awake()
    {
        sceneHandler = GameObject.Find("PROUVE/SceneHandler").GetComponent<PROUVE_SceneHandler>() ; 
        //gameObject.onClick.AddListener(clickAction) ; 
    }

    void OnMouseDown() {
        Debug.Log("Mouse down"); 
        sceneHandler.clickOnObject(OmekaVirtualObjectID, gameObject.transform.position) ; 
    }

    void Update() {
        if(displayLine) {
            var points = new Vector3[2] ; 
            points[0] = target_object.transform.position ; 
            points[1] = thisCenter ; 
            linerenderer.SetPositions(points) ;    
        }
    }

}
