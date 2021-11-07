using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove : MonoBehaviour
{
    Vector3 moveV;
    Vector3 playerVelocity;
    Rigidbody r;
    Lab3InputActions inputActions;
    PlayerInput playerInput;
    CharacterController controller;
    [SerializeField]
    // Start is called before the first frame update
    void Start()
    {
        playerInput = this.gameObject.GetComponent<PlayerInput>();
        controller = this.gameObject.GetComponent<CharacterController>();
        r = this.gameObject.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Awake()
    {
        inputActions = new Lab3InputActions();
        inputActions.Enable();
    }
    private void FixedUpdate()
    {

        Vector2 cameraVector = inputActions.Player.Look.ReadValue<Vector2>();

        float rotationX = 30f * cameraVector.x * Time.deltaTime;
        float rotationY = 30f * cameraVector.y * Time.deltaTime;

        //Vector3 CameraRotation = playerInput.camera.transform.rotation.eulerAngles;
        Vector3 CameraRotation = playerInput.camera.transform.rotation.eulerAngles;

        //CameraRotation.x -= rotationY;
        CameraRotation.y += rotationX;
        this.gameObject.transform.rotation = Quaternion.Euler(CameraRotation);


        moveV = new Vector3(inputActions.Player.Move.ReadValue<Vector2>().x * Time.deltaTime,
            -r.velocity.y,
            inputActions.Player.Move.ReadValue<Vector2>().y * Time.deltaTime);
        moveV = transform.TransformDirection(moveV);

        controller.Move(moveV);
    }
}
