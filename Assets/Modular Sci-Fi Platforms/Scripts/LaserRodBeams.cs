using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRodBeams : MonoBehaviour
{
    LaserRodBeams LRB;
    bool canSee;
    public string connectRodName;
 
    void FixedUpdate()
    {
        if(connectRodName!="" && LRB){
                RaycastHit hit;	
                 
                var rayDirection = (LRB.transform.position  + new Vector3(0,0.4f,0)) - (transform.position+ new Vector3(0,0.4f,0));
                //Debug.DrawRay(transform.position+ new Vector3(0,0.4f,0), rayDirection, Color.green);
                if (Physics.Raycast (transform.position+ new Vector3(0,0.4f,0), rayDirection, out hit,Vector3.Distance(LRB.transform.position,transform.position),-5, QueryTriggerInteraction.Ignore)) {
                  
                if (hit.transform == LRB.transform) {
                    if(!canSee) {
                        for (int i = 0; i < 4; i++){
                            transform.GetChild(i).GetComponent<LineRenderer>().SetPosition(1, new Vector3(LRB.transform.position.x-transform.GetChild(i).GetComponent<LineRenderer>().transform.position.x,LRB.transform.position.y+transform.GetChild(i).GetComponent<LineRenderer>().transform.localPosition.y-transform.GetChild(i).GetComponent<LineRenderer>().transform.position.y,LRB.transform.position.z-transform.GetChild(i).GetComponent<LineRenderer>().transform.position.z));
                          }                        
                    }
                    canSee=true;
                } else {
                    if(canSee) {
                        for (int i = 0; i < 4; i++){
                        transform.GetChild(i).GetComponent<LineRenderer>().SetPosition(1, new Vector3(0,0,0));
                        }                        
                    }
                    canSee=false;
                }}       
        } 
    }

void OnTriggerEnter(Collider col){

        if (connectRodName=="" && col.GetComponent<LaserRodBeams>() ){
           LRB= col.GetComponent<LaserRodBeams>();
           if (LRB.connectRodName==""){
                connectRodName= LRB.name;

                

                for (int i = 0; i < 4; i++){
                    transform.GetChild(i).GetComponent<LineRenderer>().SetPosition(1, new Vector3(LRB.transform.position.x-transform.GetChild(i).GetComponent<LineRenderer>().transform.position.x,LRB.transform.position.y+transform.GetChild(i).GetComponent<LineRenderer>().transform.localPosition.y-transform.GetChild(i).GetComponent<LineRenderer>().transform.position.y,LRB.transform.position.z-transform.GetChild(i).GetComponent<LineRenderer>().transform.position.z));
                }
           }
           
            
        }

    }
	void OnTriggerExit(Collider col) {
        if (connectRodName!="" && col.GetComponent<LaserRodBeams>() ){
           LRB= col.GetComponent<LaserRodBeams>();
           if (connectRodName==LRB.name){
                connectRodName= "";
                for (int i = 0; i < 4; i++){
                 transform.GetChild(i).GetComponent<LineRenderer>().SetPosition(1, new Vector3(0,0,0));
                }
           }
           
            
        }


    }
 
 
}
