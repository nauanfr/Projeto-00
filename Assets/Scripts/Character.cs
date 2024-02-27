using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public float Velocity;
    public float Gravity;
    public float RotationSpeed;

    public CharacterController CharacterController;
    public CameraController CameraController;

    private Camera Camera;
    private Vector2 MoveInput;
    private Vector2 LookInput;
    private Vector3 CurrentVelocity;


    // Start is called before the first frame update
    void Start()
    {
        Camera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
        CameraController.SetLookInput(LookInput);
    }

    void HandleMovement()
    {
        Vector3 Motion = ConvertInputToDirection();
        Motion *= Velocity;
        CurrentVelocity = Motion;
        Motion.y = Gravity;
        CharacterController.Move(Motion * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (CurrentVelocity == Vector3.zero)
        {
            return;
        }

        float desiredAngle = GetRotationAngle();
        float currentAngle = Mathf.LerpAngle(transform.eulerAngles.y,desiredAngle,Time.deltaTime * RotationSpeed);
        transform.eulerAngles = new Vector3(0, currentAngle, 0);
    }

    void OnMove(InputValue value)
    {
        Vector2 newMoveInput = value.Get<Vector2>();
        MoveInput = newMoveInput;
    }

    void OnLook(InputValue value)
    {
        Vector2 newLookInput = value.Get<Vector2>();
        LookInput = newLookInput;
    }

    Vector3 ConvertInputToDirection()
    {
        if (MoveInput == Vector2.zero)
        {
            return Vector3.zero;
        }

        float targetAngle = Mathf.Atan2(MoveInput.x, MoveInput.y) * Mathf.Rad2Deg;
        float finalAngle = (targetAngle + Camera.transform.eulerAngles.y) * Mathf.Deg2Rad;

        return new Vector3(Mathf.Sin(finalAngle), 0f, Mathf.Cos(finalAngle));
    }

    float GetRotationAngle()
    {
        return Mathf.Atan2(CurrentVelocity.x, CurrentVelocity.z) * Mathf.Rad2Deg;
    }
}
