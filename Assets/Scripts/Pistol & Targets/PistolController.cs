using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Change shooting direction to match crosshair pointing.

public class PistolController : MonoBehaviour
{
    #region Type Definitions
    [Serializable]
    public struct BulletConfig {
        public GameObject bulletObject;
        public float bulletDamage;
        public float bulletSpeed;
        public float bulletSize;
        public Vector3 fireDirection;

        public BulletConfig(GameObject p_bulletObj, Vector3 bulletDir, float p_bulletDmg, float p_bulletSpeed, float p_bulletSize) {
            bulletObject = p_bulletObj;
            fireDirection = bulletDir;
            bulletDamage = p_bulletDmg;
            bulletSpeed = p_bulletSpeed;
            bulletSize = p_bulletSize;
        }

    }
	#endregion
	
    #region Events
	#endregion

    #region Constants
	#endregion

    #region Serialized Fields
    [SerializeField] private Transform _spawnPoint = null;
    [SerializeField] private float _fireRate = -1f;  //Wait between each call to Shoot (Trigger pulled) -- (-1) => Once per trigger pull only.
    [SerializeField] private int _maxBulletsPerFire = 1;    //Number of bullets that can be fire before having to release the trigger.
    [SerializeField] private int _bulletsPerFire = 1;   //Number of bullets per fire (shotgun i.g)
    [SerializeField] private float _weaponDamage = 1f; //Damage per bullet.
    [SerializeField] private GameObject _bulletPrefab = null;
    [SerializeField] private float _bulletSpeed = 5f;   //Speed of the bullets fired by this gun.

    [SerializeField] private AnimationCurve _sizeCurve;
	#endregion

    #region Standard Attributes
    private BulletConfig _bullet = default;
    private bool _canShoot = true;
    private bool _shooting = false;
    private int _bulletsFired = 0;

    private IEnumerator _fireCoroutine;
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
	#endregion

    #region Unity Lifecycle
    private void Awake() {
        if(_fireRate <= 0) {
            Debug.LogException(new Exception("Fire rate is <= 0, it will be set to 0.1"), this.gameObject);
            _fireRate = 0.1f;
        }

        if(_bulletPrefab == null) {
            Debug.LogException(new Exception("Bullet prefab is not set. Will result in errors when firing."), this.gameObject);
        }

        //Bullet Setup
        _bullet = new BulletConfig(_bulletPrefab, _spawnPoint.forward, _weaponDamage, _bulletSpeed, CalculateBulletSize(_weaponDamage));
    }

    void Update()
    {
        Debug.DrawRay(_spawnPoint.position, _spawnPoint.forward, Color.green, 1f);

        if(Input.GetKeyDown(KeyCode.Mouse0) && _canShoot) {
            Shoot();
        }
        if(Input.GetKeyUp(KeyCode.Mouse0)) {
            if(_fireCoroutine != null) {
                StopCoroutine(_fireCoroutine);
                _fireCoroutine = null;
            }
            _canShoot = true;
            _shooting = false;
        }
    }
	#endregion

    #region Unity Callback
	#endregion

    #region Other methods
    private void Shoot() {
        _bulletsFired = 0;
        if(_maxBulletsPerFire == 1) { //Once per trigger pull.
            SpawnBullet();
            _canShoot = false;
        } else {    //Start shooting until burst done or trigger released.
            _fireCoroutine = FireCorroutine();
            _shooting = true;
            StartCoroutine(_fireCoroutine);
        }
        
    }

    private void SpawnBullet() {
        Debug.Log("Bullet spawned.");
        GameObject bulletSpawned = GameObject.Instantiate(_bullet.bulletObject, _spawnPoint.position, Quaternion.LookRotation(_spawnPoint.forward, _spawnPoint.up));
        BulletController tempBullet = bulletSpawned.GetComponent<BulletController>();
        tempBullet.SetBulletSettings(_bullet);
        //tempBullet.Launch();
        ++_bulletsFired;
    }

    private IEnumerator FireCorroutine() {
        while(_shooting && _bulletsFired < _maxBulletsPerFire) {
            SpawnBullet();
            yield return new WaitForSeconds(_fireRate);
        }

        _fireCoroutine = null;
        yield return null;
    }

    private float CalculateBulletSize(float bulletDamage) {
        float tempSize = 0f;
        tempSize = bulletDamage * 0.1f;
        tempSize = Mathf.Clamp(tempSize, 0.05f, 1f);
        return tempSize;
    }
	#endregion

}
