using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    Rigidbody r;
    Vector3 moveV;
    Lab3InputActions inputActions;
    PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {
        r = this.gameObject.GetComponent<Rigidbody>();
        playerInput = this.gameObject.GetComponent<PlayerInput>();
        
    }
    private void Awake()
    {
        inputActions = new Lab3InputActions();
        inputActions.Enable();
    }
    private void FixedUpdate()
    {
        moveV = new Vector3(inputActions.Player.Move.ReadValue<Vector2>().x, 0, inputActions.Player.Move.ReadValue<Vector2>().y);
        gameObject.transform.Translate(moveV * Time.deltaTime * 5);

        Vector2 cameraVector = inputActions.Player.Look.ReadValue<Vector2>();

        float rotationX = 30f * cameraVector.x * Time.deltaTime;
        float rotationY = 30f * cameraVector.y * Time.deltaTime;

        //Vector3 CameraRotation = playerInput.camera.transform.rotation.eulerAngles;
        Vector3 CameraRotation = this.gameObject.transform.rotation.eulerAngles;

        //CameraRotation.x -= rotationY;
        CameraRotation.y += rotationX;

        this.transform.rotation = Quaternion.Euler(CameraRotation);
    }
}
