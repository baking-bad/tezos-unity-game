using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCharacter : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 4.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    private bool shiftPress;
     public GameObject cam; 
        
 
     private float rotY = 0.0f; // rotation around the up/y axis
     private float rotX = 0.0f; // rotation around the right/x axis
    private void Start()
    {
         
        controller = gameObject.GetComponent<CharacterController>();
          Vector3 rot = transform.localRotation.eulerAngles;
         rotY = rot.y;
         rotX = rot.x;
    } 
    void Update()
    {
        groundedPlayer = controller.isGrounded;

 
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        
        }
        if   (Input.GetButtonDown("Jump")  )
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -5.0f * gravityValue);
        }  

        Vector3 movementDirection = (cam.transform.forward * Input.GetAxis("Vertical")  ) + (cam.transform.right * Input.GetAxis("Horizontal")  );  
        var speed=playerSpeed;
        if (Input.GetKeyDown(KeyCode.LeftShift)) shiftPress=true;
        if (Input.GetKeyUp(KeyCode.LeftShift)) shiftPress=false;
        if(shiftPress) speed=playerSpeed*2;
        controller.Move(movementDirection * Time.deltaTime * speed);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

         float mouseX = Input.GetAxis("Mouse X");
         float mouseY = -Input.GetAxis("Mouse Y");
 
         rotY += mouseX *1.5f  ;
         rotX += mouseY *1.5f;
 
 
 
     
        transform.rotation = Quaternion.Euler(0, rotY, 0.0f);
        cam.transform.rotation = Quaternion.Euler(rotX, rotY,  0.0f);

   
    }
}