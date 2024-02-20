using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAirDuctOpenCloseDoor : MonoBehaviour
{
 




void OnTriggerEnter(Collider col){

        if ( col.GetComponent<CharacterController> () ){
            
            GetComponent<Animator>().ResetTrigger("CloseGate") ;
            GetComponent<Animator>().SetTrigger("OpenGateSide") ;
        }

    }
	void OnTriggerExit(Collider col) {
        if ( col.GetComponent<CharacterController> () ){
            GetComponent<Animator>().SetTrigger("CloseGate") ;
            GetComponent<Animator>().ResetTrigger("OpenGateSide") ;
        }


    }

}

 