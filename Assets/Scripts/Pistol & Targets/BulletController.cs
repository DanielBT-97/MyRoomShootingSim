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
    [SerializeField] private BulletSpawnManager _spawnManager = null;
    public GameObject _bulletGameObject = null;
    public ParticleSystem _hitEffect = null;
    [SerializeField] private PistolController.BulletConfig _bulletConfig;
    [SerializeField] private Rigidbody _rigid = null;
	#endregion

    #region Standard Attributes
    private BulletSpawnManager.BulletTemplate _bulletManagerReference;
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
    public void SetBulletSettings(PistolController.BulletConfig config) {
        _bulletConfig = config;
    }

    public void SetBulletTemplate(BulletSpawnManager.BulletTemplate bulletTemp) {
        _bulletManagerReference = bulletTemp;
    }

    public void Launch() {
        _bulletGameObject.SetActive(true);
        _rigid.velocity = _bulletGameObject.transform.forward * _bulletConfig.bulletSpeed;
    }

    public void BulletCollided(Collision other)
    {
        Debug.Log("Collision: " + other);
        _rigid.velocity = Vector3.zero;
        _bulletGameObject.SetActive(false);
        
        _hitEffect.gameObject.transform.position = _bulletGameObject.transform.position;
        _hitEffect.gameObject.SetActive(true);
        _hitEffect.Play();
        StartCoroutine(HitEffectDelayDisable());
    }
	#endregion

    #region Unity Lifecycle
	#endregion

    #region Unity Callback
    
	#endregion

    #region Other methods
    private IEnumerator HitEffectDelayDisable() {
        yield return new WaitForSeconds(_hitEffect.main.duration);
        _hitEffect.gameObject.SetActive(false);
        _spawnManager.DespawnBullet(_bulletManagerReference);
    }
	#endregion

}
