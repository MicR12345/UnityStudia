using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//Przerobiłem ten skrypt by działał na nowym inpucie
public class LookAround : MonoBehaviour
{
    // ruch wokół osi Y będzie wykonywany na obiekcie gracza, więc
    // potrzebna nam referencja do niego (konkretnie jego komponentu Transform)
    public Transform player;
    Lab3InputActions inputActions;
    PlayerInput playerInput;
    public float sensitivity = 200f;

    private void Awake()
    {
        inputActions = new Lab3InputActions();
        inputActions.Enable();
    }
    void Start()
    {
        // zablokowanie kursora na środku ekranu, oraz ukrycie kursora
        playerInput = this.gameObject.GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 cameraVector = inputActions.Player.Look.ReadValue<Vector2>();

        // pobieramy wartości dla obu osi ruchu myszy
        float mouseXMove = cameraVector.x * sensitivity * Time.deltaTime;
        float mouseYMove = cameraVector.y * sensitivity * Time.deltaTime;

        // wykonujemy rotację wokół osi Y
        player.Rotate(Vector3.up * mouseXMove);

        // a dla osi X obracamy kamerę dla lokalnych koordynatów
        // -mouseYMove aby uniknąć ofektu mouse inverse
        Vector3 euler = transform.rotation.eulerAngles;
        
        //Jeśli ruch kamerą obróciłby kamerę o 180 osi z(tylko zdarza się gdy jesteśmy na -90 lub 90 od horyzontu) to nie pozwala na dalszy ruch
        //Zadanie 4
        if (Quaternion.Euler(new Vector3( euler.x - mouseYMove,euler.y,euler.z)).eulerAngles.z == 180f && transform.rotation.eulerAngles.x >= 270f)
        {
            transform.rotation = Quaternion.Euler(new Vector3(270f, transform.rotation.y, 0f));
        }
        else if (Quaternion.Euler(new Vector3(euler.x - mouseYMove, euler.y, euler.z)).eulerAngles.z == 180f && transform.rotation.eulerAngles.x <= 90f)
        {
            transform.rotation = Quaternion.Euler(new Vector3(90f, transform.rotation.y, 0f));
        }
        else
        {
            transform.Rotate(new Vector3(-mouseYMove, 0f, 0f), Space.Self);
        }


    }
}