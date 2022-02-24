using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR ; 

namespace Valve.VR.Extras
{
    public class PROUVE_interfaceController : MonoBehaviour {
        public SteamVR_Action_Vector2 touchPosition ; 

        public SteamVR_Input_Sources inputSource ; 

        public event SwipeEventHandler SwipeRight;
        public event SwipeEventHandler SwipeLeft;
        public event SwipeEventHandler SwipeUp;
        public event SwipeEventHandler SwipeDown;

        public event SwipeEventHandler SwipeHorizontal; 
        public event SwipeEventHandler SwipeVertical;

        // Start is called before the first frame update
        void Start()
        {
            touchPosition.AddOnAxisListener(touchUpdated,inputSource) ; 
        }

        public virtual void OnSwipeHorizontal(SwipeEventArgs e)
        {
            if (SwipeHorizontal != null) 
                SwipeHorizontal(this, e);
        }

        public virtual void OnSwipeVertical(SwipeEventArgs e)
        {
            if (SwipeVertical != null) 
                SwipeVertical(this, e);
        }


        public virtual void OnSwipeRight(SwipeEventArgs e)
        {
            if (SwipeRight != null) {
                SwipeRight(this, e);
            }
        }

        public virtual void OnSwipeLeft(SwipeEventArgs e)
        {
            if (SwipeLeft != null)
                SwipeLeft(this, e);
        }

        public virtual void OnSwipeUp(SwipeEventArgs e)
        {
            if (SwipeUp != null)
                SwipeUp(this, e);
        }

        public virtual void OnSwipeDown(SwipeEventArgs e)
        {
            if (SwipeDown != null)
                SwipeDown(this, e);
        }

       private void touchUpdated(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta) { 
            if(axis != Vector2.zero) {
                float length = axis.magnitude;
                float angle = Mathf.Acos(axis.x/length) ; 
                SwipeEventArgs eventArgs = new SwipeEventArgs() ; 
                SwipeEventArgs eventArgsNeg = new SwipeEventArgs() ; 
                eventArgsNeg.velocity = -length ; 
                eventArgs.velocity = length ; 
                float piSurSix = Mathf.PI/6 ;
                if(angle <= piSurSix) {
                    OnSwipeRight(eventArgs) ; 
                    OnSwipeHorizontal(eventArgs) ; 
                } else  if(angle >= 5*piSurSix) {
                    OnSwipeLeft(eventArgs) ; 
                    OnSwipeHorizontal(eventArgsNeg) ; 

                } else {
                    if(angle <= 4*piSurSix && angle >= 2*piSurSix) {
                        if(axis.y >= 0) {
                            OnSwipeUp(eventArgs) ;
                            OnSwipeVertical(eventArgs) ; 
                        } else {
                            OnSwipeDown(eventArgs) ; 
                            OnSwipeVertical(eventArgsNeg) ; 
                        }
                    } else {
                        //do nothing
                    }
                }
            }
        }
    }

    public struct SwipeEventArgs
    {
        //public SteamVR_Input_Sources fromInputSource;
        //public uint flags;
        //public float distance;
        public float velocity ; 
        //public Transform target;
    }
    
    public delegate void SwipeEventHandler(object sender, SwipeEventArgs e);

}