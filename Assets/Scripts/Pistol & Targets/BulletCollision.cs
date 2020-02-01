using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    #region Type Definitions
	#endregion
	
    #region Events
	#endregion

    #region Constants
	#endregion

    #region Serialized Fields
    public BulletController _bulletController = null;
	#endregion

    #region Standard Attributes
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
	#endregion

    #region Unity Lifecycle
	#endregion

    #region Unity Callback
    void OnCollisionEnter(Collision other)
    {
        _bulletController.BulletCollided(other);
    }
	#endregion

    #region Other methods
	#endregion

}
