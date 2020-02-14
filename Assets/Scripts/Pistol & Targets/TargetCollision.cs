using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script that is used on the Target object that has the collider so that the bullet can call Hit and communicate with the TargetController.
/// </summary>
public class TargetCollision : MonoBehaviour
{
    #region Type Definitions
	#endregion
	
    #region Events
	#endregion

    #region Constants
	#endregion

    #region Serialized Fields
    public TargetController _targetController = null;
	#endregion

    #region Standard Attributes
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
    /// <summary>
    /// Call TargetController Hit method. Called by the bullet hiting the target.
    /// </summary>
    /// <param name="bulletDamage"></param>
    public void Hit(float bulletDamage) {
        _targetController.Hit(bulletDamage);
    }
	#endregion

    #region Unity Lifecycle
	#endregion

    #region Unity Callback
    //Call Instakill target when hitting a despawn area.
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "DeathArea") {
            _targetController.InstaKillTarget();
        }
    }
	#endregion

    #region Other methods
	#endregion

}
