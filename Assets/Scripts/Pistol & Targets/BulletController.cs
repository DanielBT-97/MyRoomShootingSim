using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    #region Type Definitions
	#endregion
	
    #region Events
	#endregion

    #region Constants
	#endregion

    #region Serialized Fields
    [SerializeField] private ParticleSystem _hitEffect = null;
    [SerializeField] private PistolController.BulletConfig _bulletConfig;
    [SerializeField] private Rigidbody _rigid = null;
	#endregion

    #region Standard Attributes
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
    public void SetBulletSettings(PistolController.BulletConfig config) {
        Debug.Log("BULLET SETTINGS");
        _bulletConfig = config;
    }

    public void Launch() {
        Debug.Log("BULLET LAUNCH");
        _rigid.velocity = this.transform.forward * _bulletConfig.bulletSpeed;
    }
	#endregion

    #region Unity Lifecycle
    private void Start() {
        Debug.Log("BULLET START");
        Launch();
    }
	#endregion

    #region Unity Callback
    void OnCollisionEnter(Collision other)
    {
        Debug.Log(other);
        Destroy(this.gameObject);
    }
	#endregion

    #region Other methods
	#endregion

}
