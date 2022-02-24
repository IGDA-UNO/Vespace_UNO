using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VESPACE_Specific : MonoBehaviour
{
    public GameObject[] personnages ; 
    public bool VisibleAvatars = false ; 

    // Start is called before the first frame update
    void Start()
    {
        if(VisibleAvatars) {
            showAvatars() ; 
        } else {
            hideAvatars() ; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) {
            switchVisibility() ;    
        }
    }

    private void switchVisibility() {
        if(VisibleAvatars) {
            hideAvatars() ; 
        } else {
            showAvatars() ; 
        }
        VisibleAvatars = !VisibleAvatars ; 
    }

    private void hideAvatars() {
        foreach(GameObject avatar in personnages) {
            avatar.SetActive(false) ; 
        }
    }

    private void showAvatars() {
        foreach(GameObject avatar in personnages) {
            avatar.SetActive(true) ; 
        }
    }
}
