using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//Przerobiłem ten skrypt by działał na nowym inpucie
public class MoveWithCharacterController : MonoBehaviour
{
    private CharacterController controller;
    //Publiczne by mieć dostęp z obsługi płyty naciskowej
    public Vector3 playerVelocity;
    public bool groundedPlayer;
    
    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;

    private float playerSpeed = 10.0f;
    Lab3InputActions inputActions;
    PlayerInput playerInput;

    private void Start()
    {
        // zakładamy, że komponent CharacterController jest już podpięty pod obiekt
        playerInput = this.gameObject.GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
    }
    private void Awake()
    {
        inputActions = new Lab3InputActions();
        inputActions.Enable();

        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnJump(InputAction.CallbackContext obj)
    {
        //Irytowało mnie upośledzone działanie isGrounded więc jest raycast
        groundedPlayer = Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), 1.1f);
        if (groundedPlayer)
        {
            Debug.Log("Jumped");
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
    }

    void Update()
    {
        // wyciągamy wartości, aby możliwe było ich efektywniejsze wykorzystanie w funkcji
        Vector2 moveVector = inputActions.Player.Move.ReadValue<Vector2>();

        float moveX = moveVector.x;
        float moveZ = moveVector.y;

        // dzięki parametrowi playerGrounded możemy dodać zachowania, które będą
        // mogły być uruchomione dla każdego z dwóch stanów
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // zmieniamy sposób poruszania się postaci
        // Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // transform.right odpowiada za ruch wzdłuż osi x (pamiętajmy, że wartości będą zarówno dodatnie
        // jak i ujemne, a punkt (0,0) jest na środku ekranu) a transform.forward za ruch wzdłóż osi z.
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * Time.deltaTime * playerSpeed);

        // to już nam potrzebne nie będzie
        //if (move != Vector3.zero)
        //{
        //    gameObject.transform.forward = move;
        //}

        

        // prędkość swobodnego opadania zgodnie ze wzorem y = (1/2 * g) * t-kwadrat 
        // okazuje się, że jest to zbyt wolne opadanie, więc zastosowano g * t-kwadrat
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}