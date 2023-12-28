using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorAction : MonoBehaviour
{
 
 
public GameObject startPos,EndPos;
GameObject soll;
float changePos,moveSpeed;
private void Start() {
    moveSpeed=0.5f;
}
private void Update() {
    if(moveSpeed!=0 ){
        changePos=changePos+moveSpeed* Time.deltaTime;
        if (changePos<0   ) {
                changePos=0;
                moveSpeed=0;
            }
            else if (  changePos>1  ){
                changePos=1;
                moveSpeed=0;

            }
    transform.position= Vector3.Lerp(startPos.transform.position, EndPos.transform.position, changePos);
 
    }
}
void OnTriggerEnter(Collider col){

        if ( col.GetComponent<CharacterController> () && moveSpeed==0){
           

       
            if(changePos==0) moveSpeed=0.5f;
            else moveSpeed=-0.5f;
    
            

        }

    }
 
}

 