using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void DespawnBullet(BulletTemplate temp) {
        _onUseBullets.Remove(temp);
        _availaleBullets.Add(temp);
    }
	#endregion

    #region Unity Lifecycle
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
