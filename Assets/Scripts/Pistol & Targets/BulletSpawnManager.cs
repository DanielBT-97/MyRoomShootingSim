using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object Pooling manager for the bullets.
/// Manages the instantiation at the start as well as being the middle man between Spawned bullet and PistolController.
/// This script gives a reference to the BulletController of the newly spawned bullet to the PistolController for it to set everything up for the bullet to launch.
/// </summary>
public class BulletSpawnManager : MonoBehaviour
{
    #region Type Definitions
    [Serializable]
    public struct BulletTemplate {
        public BulletController bulletController;
        public int index;
    }
	#endregion
	
    #region Events
	#endregion

    #region Constants
	#endregion

    #region Serialized Fields
    [SerializeField] private BulletTemplate _templateBullet = default;
    [SerializeField] private Transform _bulletParent = null;
    [SerializeField] private int _maxNumberOfBullets = 100;
	#endregion

    #region Standard Attributes
    private List<BulletTemplate> _availaleBullets = null;
    private List<BulletTemplate> _onUseBullets = null;
    private int _counter = 0;
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
    /// <summary>
    /// Method used to spawn a bullet from the object pool.
    /// When called it will move the new bullet from one list to another, return the bullet's references in order to be used by the Pistol Controller.
    /// After being called the bullet is only referenced, no operations are done the actual bullet, those are done in the pistol/bullet controller.
    /// </summary>
    /// <param name="isBulletAvailable">Out variable that says if there was an available bullet to be used.</param>
    /// <returns></returns>
    public BulletTemplate SpawnBullet(out bool isBulletAvailable) {
        BulletTemplate temp = default;
        if(_availaleBullets.Count > 0) {
            temp = _availaleBullets[0];
            _onUseBullets.Add(temp);
            _availaleBullets.Remove(temp);
            isBulletAvailable = true;
        } else {
            isBulletAvailable = false;
        }
        
        return temp;
    }

    /// <summary>
    /// Reenable bullet to be used.
    /// </summary>
    /// <param name="temp">Bullet's template reference. Used to delete the correspondent item from the available/onUse lists.</param>
    public void DespawnBullet(BulletTemplate temp) {
        temp.bulletController.gameObject.SetActive(false);
        _onUseBullets.Remove(temp);
        _availaleBullets.Add(temp);
    }
	#endregion

    #region Unity Lifecycle
    /// <summary>
    /// On Awake the bullet spawn manager instantiates the maximum number of bullets needed to be used.
    /// Instantiate the bullet prefab (Parent + 2 Childs [Bullet, HitFX]).
    /// Create a temporal BulletTemplate that contains the info of the newly created bullet (Reference to the BulletController that has the references to the bullet's components and methods).
    /// Pass the BulletTemplate to the BulletController in order for each bullet to know which one they are so that I can reference them back when disabling.
    /// Add this new bullet object to the available list.
    /// So far the counter is only to diferenciate between each instance of bullets.
    /// </summary>
    private void Awake() {
        _availaleBullets = new List<BulletTemplate>();
        _onUseBullets = new List<BulletTemplate>();

        for (int i = 0; i < _maxNumberOfBullets; i++) {
            BulletTemplate temp = default;
            temp.bulletController = GameObject.Instantiate(_templateBullet.bulletController.gameObject, _bulletParent).GetComponent<BulletController>();
            temp.index = _counter;
            temp.bulletController.SetBulletTemplate(temp);
            _availaleBullets.Add(temp);
            ++_counter;
        }
        _onUseBullets.Clear();
    }
	#endregion

    #region Unity Callback
	#endregion

    #region Other methods
	#endregion

}
