using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller used for the bullet logic.
/// By itself it does nothing, everything is called from the PistolController once the BulletSpawnManager passes the reference to this script upon spawning a bullet.
/// </summary>
public class BulletController : MonoBehaviour
{
    #region Type Definitions
	#endregion
	
    #region Events
	#endregion

    #region Constants
	#endregion

    #region Serialized Fields
    [SerializeField] private BulletSpawnManager _spawnManager = null;
    public GameObject _bulletGameObject = null;
    public ParticleSystem _hitEffect = null;
    [SerializeField] private PistolController.BulletConfig _bulletConfig;
    [SerializeField] private Rigidbody _rigid = null;
    [SerializeField] private AudioSource _audioSrc = null;
    [SerializeField] private LayerMask _targetLayer = 0;
	#endregion

    #region Standard Attributes
    private BulletSpawnManager.BulletTemplate _bulletManagerReference;
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
    /// <summary>
    /// Method used by the PistolController upon requesting a bullet spawn.
    /// Sets every bullet value (dmg, speed, size, etc...)
    /// </summary>
    /// <param name="config"></param>
    public void SetBulletSettings(PistolController.BulletConfig config) {
        _bulletConfig = config;
    }

    /// <summary>
    /// Method used by the BulletSpawnManager upon instantiating each bullet.
    /// Used so that each bullet is able to be referenced back to each list in the object pooling without the need to do a manual search.
    /// </summary>
    /// <param name="bulletTemp"></param>
    public void SetBulletTemplate(BulletSpawnManager.BulletTemplate bulletTemp) {
        _bulletManagerReference = bulletTemp;
    }

    /// <summary>
    /// Method used to activate the bullet and give it speed.
    /// Must be done after every setting is set.
    /// </summary>
    public void Launch() {
        _bulletGameObject.SetActive(true);
        _rigid.velocity = _bulletGameObject.transform.forward * _bulletConfig.bulletSpeed;
    }

    /// <summary>
    /// Method used by the BulletCollision script upon colliding with something.
    /// It stops the bullet's movement, disables the bullet itself and activates the hit effect as well as a delay coroutine to know when the effect has ended.
    /// </summary>
    /// <param name="other">Collision hit by the bullet</param>
    public void BulletCollided(Collision other) {
        //Call hit method on the hit target. Only check for component if the layer of the object hit is the Target's layer. (bit mask comparison)
        if( (((1<<other.gameObject.layer) & _targetLayer) != 0) && other.gameObject.TryGetComponent(out TargetController targetController) ) {
            targetController.Hit(_bulletConfig.bulletDamage);
        }
        
        //Debug.Log("Collision: " + other);
        _rigid.velocity = Vector3.zero;
        _bulletGameObject.SetActive(false);
        
        _hitEffect.gameObject.transform.position = _bulletGameObject.transform.position;
        _hitEffect.gameObject.transform.forward = other.GetContact(0).normal;
        _hitEffect.gameObject.SetActive(true);
        _hitEffect.Play();
        _audioSrc.Play();
        StartCoroutine(HitEffectDelayDisable());
    }
	#endregion

    #region Unity Lifecycle
	#endregion

    #region Unity Callback
    
	#endregion

    #region Other methods
    /// <summary>
    /// Coroutine used to delay the deactivation of the hit effect object as well as the reenabling of the bullet to be used again in the object pulling.
    /// Once the delay is finished, using the particle effect duration, the bullet itself calls the BulletSpawnManager to notify it that it is ready to be used again.
    /// </summary>
    private IEnumerator HitEffectDelayDisable() {
        yield return new WaitForSeconds(_hitEffect.main.duration);
        _hitEffect.gameObject.SetActive(false);
        _spawnManager.DespawnBullet(_bulletManagerReference);
    }
	#endregion

}
