using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    #region Type Definitions
	#endregion
	
    #region Events
	#endregion

    #region Constants
	#endregion

    #region Serialized Fields
    [SerializeField] private ParticleSystem _destroyEffect = null;
    [SerializeField] private float _targetHealth = 1f;
	#endregion

    #region Standard Attributes
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
    public void Hit(float bulletDamage) {
        _targetHealth -= bulletDamage;

        UpdateState();
    }
	#endregion

    #region Unity Lifecycle
	#endregion

    #region Unity Callback
	#endregion

    #region Other methods
    private void UpdateState() {
        if(_targetHealth <= 0) {
            //Kill target.
        }
    }
	#endregion

}
