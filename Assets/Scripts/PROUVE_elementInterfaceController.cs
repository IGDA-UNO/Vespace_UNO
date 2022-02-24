using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class PROUVE_elementInterfaceController : MonoBehaviour
{

    private RectTransform canvas ; 
    private RectTransform elementextRT ; 
    public Text elementText ; 
    public Text elementTitle ; 
    private bool firstDisplay;

    void Awake()
    {
        canvas = GetComponent<RectTransform>() ; 
        elementextRT = elementText.GetComponent<RectTransform>() ; 
    }

    public void setInfos(string title, string text) {
        elementTitle.text = title ; 
        elementText.text = text ; 
        firstDisplay = true ; 
    }

    void LateUpdate() {
        if(firstDisplay) {
            if(elementextRT.sizeDelta.y != 0) {
                float height = elementextRT.rect.height + 30.0f ; 
                canvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,height) ;
                firstDisplay = false ; 
            }
        }
    }
}
