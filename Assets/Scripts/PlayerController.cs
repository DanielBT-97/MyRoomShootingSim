using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    #region Type Definitions
	#endregion
	
    #region Events
	#endregion

    #region Constants
	#endregion

    #region Serialized Fields
    [SerializeField] private Transform _cameraTrans = null;

    [Header("Camera Settings")]
    [SerializeField] private float _playerHeight = 1.67f;
    [SerializeField] private float _horizontalTurnSpeed = 2.0f;
    [FormerlySerializedAs("_VerticalTurnSpeed")] [SerializeField] private float _verticalTurnSpeed = 2.0f;
	#endregion

    #region Standard Attributes
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
	#endregion

    #region Unity Lifecycle
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if(_cameraTrans == null) {
            _cameraTrans = Camera.main.gameObject.transform;
        }

        _cameraTrans.SetYPos(_playerHeight);
    }

    void Update () {
        MouseLock();

        CameraRotation();
    }
	#endregion

    #region Unity Callback
	#endregion

    #region Other methods
    private void MouseLock() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
        }

        if(Input.GetKeyDown(KeyCode.Mouse0) && Cursor.lockState == CursorLockMode.None) {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void CameraRotation() {
        _yaw += _horizontalTurnSpeed * Input.GetAxis("Mouse X");
        _pitch -= _verticalTurnSpeed * Input.GetAxis("Mouse Y");

        _cameraTrans.eulerAngles = new Vector3(_pitch, _yaw, 0.0f);
    }
	#endregion

}
