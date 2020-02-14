using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is the controller that manages all the target's logic.
/// </summary>
public class TargetController : MonoBehaviour
{
    #region Type Definitions
	#endregion
	
    #region Events
	#endregion

    #region Constants
	#endregion

    #region Serialized Fields
    [Header("Target References")]
    [SerializeField] private ParticleSystem _destroyTargetEffect = null;
    [SerializeField] private TargetSpawnManager _targetSpawnManager = null;
    [SerializeField] private Animator _targetAnimator = null;

    [Header("Target Settings")]
    [SerializeField] private Renderer _targetRenderer = null;
    [SerializeField] private float _currentHealth = 1f;
    [SerializeField, ColorUsageAttribute(true, true)] private Color[] _targetColorProgression = new Color[3];
    [SerializeField] private float _movementSpeed = 5f;
	#endregion

    #region Standard Attributes
    private MaterialPropertyBlock _matPropBlock = null;
    private Color _currentColor = Color.black;
    private TargetSpawnManager.TargetReferences _targetReference = default;
    private bool _targetIsAlive = false;

    //Movement
    private Vector3 _movementDirection = Vector3.zero;
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
    /// <summary>
    /// Target has been hit and needs its health reduced.
    /// Trigger Hit animation once added.
    /// </summary>
    /// <param name="bulletDamage"></param>
    public void Hit(float bulletDamage) {
        _currentHealth -= bulletDamage;

        UpdateState();
    }

    
    public void InstaKillTarget() {
        _currentHealth = 0;
        _targetIsAlive = false;
        _movementDirection = Vector3.zero;
        _targetRenderer.gameObject.SetActive(false);
        _targetSpawnManager.TargetDestroyed(_targetReference);
    }

    public void ResetTarget(int health) {
        _currentHealth = health;
        _movementDirection = Vector3.zero;
        _targetRenderer.transform.localPosition = Vector3.zero;
    }

    public void SetTargetReference(TargetSpawnManager.TargetReferences targetRef) {
        _targetReference = targetRef;
    }

    public void TargetSpawned() {
        _targetRenderer.gameObject.SetActive(true);
        _targetIsAlive = true;
    }

    public void OrientateAndLaunchTarget(Vector3 direction, Vector3 forwardOrientation) {
        this.gameObject.transform.right = forwardOrientation;
        _movementDirection = direction;
    }
	#endregion

    #region Unity Lifecycle
    /// <summary>
    /// Creates materialPropBlock once.
    /// </summary>
    private void Awake() {
        Debug.Log("AWAKE");
        if(_matPropBlock == null) _matPropBlock = new MaterialPropertyBlock();
        //_targetIsAlive = true;
        //_targetRenderer.gameObject.SetActive(true);
    }

    /// <summary>
    /// Manages the target's color.
    /// </summary>
    private void Update() {
        //Update target colors based on current health.
        if(_targetIsAlive) {
            if(_currentHealth <= 0) {   //If current health is lower than 0 (Should get destroyed) use same color as 1 hit.
                UpdateTargetColor(_targetColorProgression[0]);
            } else {    //Else use the color corresponding to the rounded UP int value of current health (In case I decide to use a float dmg value at some point).
                UpdateTargetColor(_targetColorProgression[Mathf.CeilToInt(_currentHealth - 1)]);    //FloorToInt could be used, would change the behaviour so testing needed.
            }

            _targetRenderer.transform.position += _movementDirection * _movementSpeed * Time.deltaTime;
        }
    }
	#endregion

    #region Unity Callback
	#endregion

    #region Other methods
    /// <summary>
    /// Function used to know when the target is ready to be destroyed. (Health <= 0)
    /// </summary>
    private void UpdateState() {
        if(_currentHealth <= 0) {
            //Kill target.
            ExplodeTarget();
        } else {
            _targetAnimator.SetTrigger("TargetHit");
        }
    }

    /// <summary>
    /// Set the emission and diffuse color for the target using PropertyBlocks. Allows for a single mat instance to use different colors at the same time without unity creating an instance per variation.
    /// Uses the following tutorial: http://thomasmountainborn.com/2016/05/25/materialpropertyblocks/ 
    /// </summary>
    /// <param name="color"></param>
    private void UpdateTargetColor(Color color) {
        _targetRenderer.GetPropertyBlock(_matPropBlock);    //Takes current values of the properties. (If a PerRendererData prop is not set in each SetPropertyBlock call it will reset. E.g. if I forget to set emission prop the emission will set itself to black)
        _matPropBlock.SetColor("_Color", color);            //Set diffuse color.
        _matPropBlock.SetColor("_Emission", color);         //Set emission color to the same as diffuse.
        _targetRenderer.SetPropertyBlock(_matPropBlock);    //Set property block.
        _currentColor = color;
    }

    /// <summary>
    /// Function that manages the target's destruction.
    /// Triggers the explosion FX.
    /// </summary>
    private void ExplodeTarget() {
        _targetIsAlive = false;
        _destroyTargetEffect.gameObject.SetActive(true);
        _destroyTargetEffect.gameObject.transform.position = _targetRenderer.gameObject.transform.position;
        _movementDirection = Vector3.zero;
        _targetRenderer.gameObject.SetActive(false);
        StartCoroutine(HitEffectDelayDisable());
    }

    /// <summary>
    /// Coroutine used to delay the deactivation of the explosion effect object as well as the reenabling of the target to be used again in the object pulling.
    /// Once the delay is finished, using the particle effect duration, the target itself calls the TargetSpawnManager to notify it that it is ready to be used again.
    /// </summary>
    private IEnumerator HitEffectDelayDisable() {
        yield return new WaitForSeconds(_destroyTargetEffect.main.duration);
        _destroyTargetEffect.gameObject.SetActive(false);
        _targetSpawnManager.TargetDestroyed(_targetReference);
    }

    #region HealthTesting_ContextMenu
    /*
    [ContextMenu("LowerHealthBy1")]
    private void LowerHealthBy1() {
        Hit(1);
    }

    [ContextMenu("LowerHealthBy2")]
    private void LowerHealthBy2() {
        Hit(2);
    }

    [ContextMenu("LowerHealthBy3")]
    private void LowerHealthBy3() {
        Hit(3);
    }

    [ContextMenu("ResetHealthTo1")]
    private void ResetHealthTo1() {
        _currentHealth = 1;
    }

    [ContextMenu("ResetHealthTo2")]
    private void ResetHealthTo2() {
        _currentHealth = 2;
    }

    [ContextMenu("ResetHealthTo3")]
    private void ResetHealthTo3() {
        _currentHealth = 3;
    }*/
    #endregion
	#endregion

}
