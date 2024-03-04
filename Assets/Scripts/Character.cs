using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public float Gravity = -9;
    public float GroundRadius = 0.25f;
    public float GroundDetectionThresHold = 1.2f;
    public float GroundDetectionThresHoldInAir = 0.75f;
    public LayerMask GroundLayer;
    public float RotationSpeed = 5;
    [Space]public float VelocityWalk = 3;
    public float VelocityRunning = 6;
    public float Acceleration = 2;
    public float Deceleration = 3;
    public float AnimationVelocitySpeedIdle = 4;
    public float AnimationVelocitySpeed = 2;
    [Space] public float JumpForce = 5;

    public CharacterController CharacterController;
    public CameraController CameraController;
    public Animator Animator;

    private Camera Camera;
    private Vector2 MoveInput;
    private Vector2 LookInput;
    private Vector3 CurrentVelocity;
    private bool IsRunning;
    private bool IsGrounded;
    private float CurrentAnimationVelocity;
    private float CurrentGravity;

    // Start is called before the first frame update
    void Start()
    {
        Camera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        HandleGravity();
        HandleMovement();
        HandleRotation();
        HandleAnimation();
        CameraController.SetLookInput(LookInput);
    }

    void HandleGravity()
    {
        HandleGroundDetection();
        
        if (GetIsGrounded() && CurrentGravity < 0)
        {
            CurrentGravity = -1;
        }

        CurrentGravity += Gravity * Time.deltaTime;
    }

    void HandleMovement()
    {
        Vector3 desiredMotion = ConvertInputToDirection();
        float velocity = IsRunning ? VelocityRunning : VelocityWalk;
        float transitionSpeed = MoveInput == Vector2.zero ? Deceleration : Acceleration;
        desiredMotion *= velocity;
        CurrentVelocity = Vector3.Lerp(CurrentVelocity, desiredMotion, Time.deltaTime * transitionSpeed);
        
        Vector3 finalMotion = CurrentVelocity;
        finalMotion.y = CurrentGravity;
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
        Animator.SetBool("IsGrounded", GetIsGrounded());
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

    void OnJump(InputValue value)
    {
        StartJump();
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

    private void StartJump()
    {
        if(GetIsGrounded() == false)
        {
            return;
        }

        CurrentGravity += JumpForce;

        Animator.Play("Jump Idle Start");
    }

    private void HandleGroundDetection()
    {
        float Distance = IsGrounded ? GroundDetectionThresHold : GroundDetectionThresHoldInAir;

        var groundCast = Physics.SphereCast(transform.position + Vector3.up, GroundRadius, Vector3.down,out var hitInfo, Distance, GroundLayer);

        IsGrounded = hitInfo.collider && CurrentGravity < 0;
    }

    private void OnDrawGizmos()
    {
        float Distance = IsGrounded ? GroundDetectionThresHold : GroundDetectionThresHoldInAir;

        Gizmos.DrawSphere(transform.position + Vector3.up, GroundRadius);
        Gizmos.DrawSphere(transform.position + Vector3.up * Distance, GroundRadius);
    }

    private bool GetIsGrounded()
    {
        return IsGrounded;
    }
}
