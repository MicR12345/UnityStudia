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


    public float speed = 1f;

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

        Vector3 CameraRotation = playerInput.camera.transform.rotation.eulerAngles;

        
        CameraRotation.y += rotationX;

        if(CameraRotation.x - rotationY >= 90 && CameraRotation.x < 90)
        {
            CameraRotation.x = 89;
        }
        else if (CameraRotation.x - rotationY <= 270 && CameraRotation.x > 270)
        {
            CameraRotation.x = 271;
        }
        else
        {
            CameraRotation.x -= rotationY;
        }
        this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, CameraRotation.y));
        playerInput.camera.transform.localRotation = Quaternion.Euler(new Vector3(CameraRotation.x,0));

        if (!controller.isGrounded)
        {
            r.velocity = new Vector3(r.velocity.x, r.velocity.y -9.81f , r.velocity.z);
        }

        moveV = new Vector3(
            inputActions.Player.Move.ReadValue<Vector2>().x * Time.deltaTime * speed,
            0,
            inputActions.Player.Move.ReadValue<Vector2>().y * Time.deltaTime * speed
            );
        moveV = transform.TransformDirection(moveV);

        controller.Move(moveV);
        controller.Move(new Vector3(0, r.velocity.y, 0));
    }
}
