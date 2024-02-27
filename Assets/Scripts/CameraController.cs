using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    
    public float cameraDecreaseSmoothSpeed = 2f;
    public float cameraSensibility = 0.5f;
    public bool invertY = false;
    
    [SerializeField] private Transform cameraTransform;
    private Vector2 _lookInput;
    private float _horizontalRotation;
    private float _verticalRotation;
    
    private const float Threshold = 0.01f;

    #endregion

    // Update is called once per frame
    void Update()
    {
        HandleCameraRotation();
    }

    public void SetLookInput(Vector2 input)
    {
        _lookInput = input;
    }
    
    private void HandleCameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_lookInput.sqrMagnitude >= Threshold)
        {
            var newLookVector = _lookInput;
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = Time.deltaTime;

            _horizontalRotation += newLookVector.x * deltaTimeMultiplier * cameraSensibility;
            _verticalRotation += newLookVector.y * deltaTimeMultiplier * cameraSensibility * (invertY ? -1 : 1);
        }

        _horizontalRotation = Mathf.Repeat(_horizontalRotation, 360);

        _verticalRotation = ClampAngle(_verticalRotation, BottomClamp, TopClamp);

        cameraTransform.rotation = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
