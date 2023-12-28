using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpenCloseDoor : MonoBehaviour
{
 
    public enum e_OrientationOpenDoor{ Down,Side }
    public enum e_TypeDoor{ one,two,three }
    public e_OrientationOpenDoor OrientationOpenDoor;
    public e_TypeDoor TypeDoor;

    public mp_door [] _mp_door;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _mp_door.Length; i++) if((int)TypeDoor!=i)
        {
            _mp_door[i].DoorL.SetActive(false);
            _mp_door[i].DoorR.SetActive(false);             
        }
 


 
      
    }

 



void OnTriggerEnter(Collider col){

        if ( col.GetComponent<CharacterController> () ){
            
            GetComponent<Animator>().ResetTrigger("CloseGate") ;
            if(OrientationOpenDoor==e_OrientationOpenDoor.Down)        GetComponent<Animator>().SetTrigger("OpenGateDown") ;
             else GetComponent<Animator>().SetTrigger("OpenGateSide") ;
        }

    }
	void OnTriggerExit(Collider col) {
        if ( col.GetComponent<CharacterController> () ){
            GetComponent<Animator>().SetTrigger("CloseGate") ;
            if(OrientationOpenDoor==e_OrientationOpenDoor.Down)   GetComponent<Animator>().ResetTrigger("OpenGateDown") ;
            else GetComponent<Animator>().ResetTrigger("OpenGateSide") ;
        }


    }

}
[System.Serializable]
public class mp_door{
      public GameObject DoorL,DoorR;

}
 