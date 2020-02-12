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
        public float bulletDamage;
        public float bulletSpeed;
        public float bulletSize;
        public Vector3 fireDirection;

        public BulletConfig(Vector3 bulletDir, float p_bulletDmg, float p_bulletSpeed, float p_bulletSize) {
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
    [Header("Weapon Settup")]
    [SerializeField] private Transform _gunTrans = null;            //Transform of the main object. (Allows for the script to be somwhere else in the GameObject)
    [SerializeField] private RectTransform _crosshairTrans = null;  //Transform of the crosshair. Used for the Camera Raycast to point the gun.
    [SerializeField] private Vector3 _pointGunOffset = new Vector3(0, -0.05f, 0);   //Offset used for the position the gun is pointing at to line up with the crosshair (There was some Y-axis offset to correct)
    [SerializeField] private Transform _spawnPoint = null;          //BulletSpawnPoint
    [SerializeField] private Transform _targetPositionTrans = null; //Target position of the gun in 3D Space.
    [SerializeField] private float _transitionPositionSpeed = 20f;  //Speed at which the gun lerps towards that target position.
    [SerializeField] private AudioSource _audioSrcShooting = null;  //Audio Source for the shooting sound effect.

    [Header("Weapon Settings")]
    [SerializeField] private bool _burstFire = false; //Whether you need to hold the fire button to fire the maximum bullets per fire.
    [SerializeField] private float _fireRate = 1f;  //Wait between each call to Shoot (Trigger pulled) -- (-1) => Once per trigger pull only.
    [SerializeField] private int _maxBulletsPerFire = 1;    //Number of bullets that can be fire before having to release the trigger.
    [SerializeField] private int _bulletsPerFire = 1;   //Number of bullets per fire (shotgun i.g)
    [SerializeField] private float _weaponDamage = 1f; //Damage per bullet.
    [SerializeField] private float _bulletSpeed = 5f;   //Speed of the bullets fired by this gun.

    [Header("Bullet Settup")]
    [SerializeField] private BulletSpawnManager _bulletSpawnManager = null; //Reference to the bullet spawn manager (Object Pooling)
	#endregion

    #region Standard Attributes
    private BulletConfig _bullet = default; //Configuration for damage, speed, size and
    private bool _canShoot = true;
    private bool _shooting = false;
    private int _bulletsFired = 0;

    private IEnumerator _fireCoroutine;
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
    /// <summary>
    /// This is a method that can be called to reset the state of the weapon to its default.
    /// Will stop shooting if it was still using the corroutine and will reset all the variables to starting values.
    /// </summary>
    public void FullWeaponReset() {
        _bulletsFired = 0;
        _shooting = false;
        _canShoot = true;
        if(_fireCoroutine != null) {
            StopCoroutine(_fireCoroutine);
            _fireCoroutine = null;
        }
    }
	#endregion

    #region Unity Lifecycle
    /// <summary>
    /// On Awake I check for the fire rate to be not be negative since that would break the wait in the coroutine.
    /// I set the values for the BulletConfig based on weapon settings. This implies that it cannot be modified in runtime so far (Not modified anywhere else, only used).
    /// </summary>
    private void Awake() {
        if(_fireRate <= 0) {
            Debug.LogException(new Exception("Fire rate is <= 0, it will be set to 0.1"), this.gameObject);
            _fireRate = 0.1f;
        }

        //Bullet Setup
        _bullet = new BulletConfig(_spawnPoint.forward, _weaponDamage, _bulletSpeed, CalculateBulletSize(_weaponDamage));
    }

    /// <summary>
    /// Rotate the gun towards where the crosshair is pointing.
    /// Check for LeftClick press in order to start the shooting action.
    ///     CanShoot is used to limit when the player can shoot. In case I want to use some reload/cooldown mechanic later.
    /// Check for LeftClick release in order to stop the shooting action.
    /// </summary>
    void Update() {
        //Debug.DrawRay(_spawnPoint.position, _spawnPoint.forward, Color.green, 1f);
        LerpGunToPosition();
        RotateGunTowardsCrosshair();

        if(Input.GetKeyDown(KeyCode.Mouse0) && _canShoot) {
            Shoot();
        }
        if(Input.GetKeyUp(KeyCode.Mouse0) && !_burstFire) {
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
    private void LerpGunToPosition() {
        Vector3 targetPos = _targetPositionTrans.position;
        _gunTrans.position = Vector3.Lerp(_gunTrans.position, targetPos, Time.deltaTime * _transitionPositionSpeed);
    }

    /// <summary>
    /// Creates a raycast from the camera towards the crosshair in order to know where the bullet should hit.
    /// Uses the hit point world position and the LookAt function to rotate the gun so that the bullet spawn's forward vector is pointing towards wwhere it needs to.
    /// </summary>
    private void RotateGunTowardsCrosshair() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(_crosshairTrans.position);
        Debug.DrawRay(ray.origin, ray.direction, Color.cyan);
        if(Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            Vector3 hitPos = hit.point;
            _gunTrans.LookAt(hitPos + _pointGunOffset);
        }
    }

    /// <summary>
    /// Method used when the player wants to shoot a bullet. Manages whether it needs to just spawn one bullet or to create a coroutine in order to shoot multiple in a row.
    /// TODO: So far Bullets per fire is not used. Implement Shotgun.
    /// TODO: So far its only Semi or Auto, there is no burst fire. In order to shoot burst of 3 you need to hold mouse0. Implement option to burst with one click.
    /// </summary>
    private void Shoot() {
        _bulletsFired = 0;
        if(_maxBulletsPerFire == 1) { //Once per trigger pull.
            SpawnBullet();
            _canShoot = false;
        } else {    //Start shooting until burst done or trigger released.
            _shooting = true;   //Shooting is happening. (Not used in single fire since it happens in a single frame)
            _canShoot = false;  //Block any other calls to shoot from Input.
            _fireCoroutine = FireCoroutine();
            StartCoroutine(_fireCoroutine);
        }
        
    }

    /// <summary>
    /// Method called in order to spawn a bullet.
    /// Spawns the bullet using the BulletManager pool and rotates and positions it in order to face the corect direction.
    /// Does all the settings needed for the bullet to work using the BulletController's interfaces.
    /// </summary>
    private void SpawnBullet() {
        //Debug.Log("Bullet spawned.");
        BulletSpawnManager.BulletTemplate bulletTemp = _bulletSpawnManager.SpawnBullet(out bool couldFire);
        if(couldFire) {
            bulletTemp.bulletController.gameObject.SetActive(true);
            GameObject bulletSpawned = bulletTemp.bulletController._bulletGameObject;
            bulletSpawned.transform.position = _spawnPoint.position;
            bulletSpawned.transform.rotation = Quaternion.LookRotation(_spawnPoint.forward, _spawnPoint.up);
            bulletTemp.bulletController.SetBulletSettings(_bullet);
            bulletTemp.bulletController.Launch();
            _audioSrcShooting.Play();
            ++_bulletsFired;
        } else {
            Debug.Log("COULD NOT FIRE");
        }
    }

    /// <summary>
    /// Function used to scale the bullet up depending on the damage it does.
    /// </summary>
    /// <param name="bulletDamage"></param>
    /// <returns></returns>
    private float CalculateBulletSize(float bulletDamage) {
        float tempSize = 0f;
        tempSize = bulletDamage * 0.1f;
        tempSize = Mathf.Clamp(tempSize, 0.05f, 1f);
        return tempSize;
    }

    /// <summary>
    /// This coroutine is used for Automatic Fire.
    /// Spawns a bullet every _fireRate seconds.
    /// </summary>
    private IEnumerator FireCoroutine() {
        while(_shooting && _bulletsFired < _maxBulletsPerFire) {
            SpawnBullet();
            if(_bulletsFired != _maxBulletsPerFire) yield return new WaitForSeconds(_fireRate); //Last bullet does not apply a trigger delay. TODO: Can be used to add a different delay between burst seperate from the fireRate.
        }
        
        //If the gun is set to burst fire it does not trigger the ButtonUp callback so these bools need to be reset at the end of the coroutine.
        if(_burstFire) {
            _shooting = false;
            _canShoot = true;
        }
        _fireCoroutine = null;
        yield return null;
    }
	#endregion

}
