using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
// This script aligns all child elements by rotation and position.
//the rotation aligns by an integer, and the position by one tenth of a number.
//To use this script, just tick the "start" checkbox, do not forget to save the scene beforehand.

public class doEditor : MonoBehaviour
{
    public bool _start;
    public bool _AlignRotation=true;
    public bool _AlignPosition=true;
 
    bool old_start;
 
 
 
 
    void Update()
    {
        if(_start!=old_start ){
           old_start=_start;
 
           if(_start ) {
          
               foreach (Transform  child in transform){
         
                  // break;
                   if (_AlignRotation) child.transform.localRotation =  Quaternion.Euler(Mathf.RoundToInt(child.transform.localEulerAngles.x), Mathf.RoundToInt(child.transform.localEulerAngles.y), Mathf.RoundToInt(child.transform.localEulerAngles.z));
                   float x=Mathf.RoundToInt(child.transform.position.x*10);
                   float y=Mathf.RoundToInt(child.transform.position.y*10);
                   float z=Mathf.RoundToInt(child.transform.position.z*10);
                   
                   if (_AlignPosition) child.transform.position =  new Vector3(x/10,y/10,z/10);
 
               }
 
 

               
           }
            ;
        }
    }
}