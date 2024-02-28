using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public float Gravity;
    public float RotationSpeed;
    [Space]public float VelocityWalk = 2.5f;
    public float VelocityRunning = 5;
    public float Acceleration = 2;
    public float Deceleration = 2;
    public float AnimationVelocitySpeedIdle = 11;
    public float AnimationVelocitySpeed = 2;

    public CharacterController CharacterController;
    public CameraController CameraController;
    public Animator Animator;

    private Camera Camera;
    private Vector2 MoveInput;
    private Vector2 LookInput;
    private Vector3 CurrentVelocity;
    private bool IsRunning;
    private float CurrentAnimationVelocity;

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
        HandleAnimation();
        CameraController.SetLookInput(LookInput);
    }

    void HandleMovement()
    {
        Vector3 desiredMotion = ConvertInputToDirection();
        float velocity = IsRunning ? VelocityRunning : VelocityWalk;
        float transitionSpeed = MoveInput == Vector2.zero ? Deceleration : Acceleration;
        desiredMotion *= velocity;
        CurrentVelocity = Vector3.Lerp(CurrentVelocity, desiredMotion, Time.deltaTime * transitionSpeed);
        
        Vector3 finalMotion = CurrentVelocity;
        finalMotion.y = Gravity;
        CharacterController.Move(finalMotion * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (CurrentVelocity == Vector3.zero)
        {
            return;
        }

        float desiredAngle = GetRotationAngle();
        float currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, desiredAngle, Time.deltaTime * RotationSpeed);
        transform.eulerAngles = new Vector3(0, currentAngle, 0);
    }

    void HandleAnimation()
    {
        float desiredVelocity = 0;
        float desiredTransitionSpeed = AnimationVelocitySpeedIdle;

        if (MoveInput != Vector2.zero)
        {
            desiredVelocity = IsRunning ? 2 : 1;
            desiredTransitionSpeed = AnimationVelocitySpeed;
        }

        CurrentAnimationVelocity = Mathf.Lerp(CurrentAnimationVelocity, desiredVelocity, Time.deltaTime * desiredTransitionSpeed);
        
        Animator.SetFloat("Velocity", CurrentAnimationVelocity);
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

    void OnRunning(InputValue value)
    {
        IsRunning = !IsRunning;
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
