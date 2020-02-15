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
    [SerializeField] private ParticleSystem _destroyTargetEffect = null;    //Particle System for the destruction effect.
    [SerializeField] private TargetSpawnManager _targetSpawnManager = null; //Target Spawn Manager.
    [SerializeField] private Animator _targetAnimator = null;               //Animator reference for the target. (Hit animation)
    [SerializeField] private GameObject[] _pointsObj;                       //Point gameobject --> 0: 100 | 1: 200 | 2: 500

    [Header("Target Settings")]
    [SerializeField] private Renderer _targetRenderer = null;       //Target renderer. Used for material properties blocks. (also as the reference to the target gameobject)
    [SerializeField, ColorUsageAttribute(true, true)] private Color[] _targetColorProgression = new Color[3];   //Color array for the target's different colors for each current health.
    [SerializeField] private float _movementSpeed = 5f;             //Target's movement speed.
	#endregion

    #region Standard Attributes
    //Color
    private MaterialPropertyBlock _matPropBlock = null;     //Property block variable.
    public Color _currentColor = Color.black;              //Current color of the target.
    
    //Other
    private TargetSpawnManager.TargetReferences _targetReference = default;     //Target reference variable. Used to identify each target when communicating with the Object Pooling system.
    private bool _targetIsAlive = false;                    //Flag marking the target as alive. Used for position and color update.
    
    //Health
    public float _currentHealth = 1f;                      //Current health of the target.
    private float _initialHealth = 0f;                      //Initial health of the target.

    //Movement
    private Vector3 _movementDirection = Vector3.zero;      //Movement direction of the target. Set when spawning.
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

    /// <summary>
    /// Method used to instakill the target without any animations or transitions.
    /// Used for the DeathArea despawn so that the target is immediately usable again.
    /// </summary>
    public void InstaKillTarget() {
        _currentHealth = 0;
        _targetIsAlive = false;
        _movementDirection = Vector3.zero;
        _targetRenderer.gameObject.SetActive(false);
        _targetSpawnManager.TargetDestroyed(_targetReference);
    }

    /// <summary>
    /// Method used to reset the target back to its initial state with a set target health.
    /// Used when the target has just spawned.
    /// </summary>
    /// <param name="health"></param>
    public void ResetTarget(int health) {
        _currentHealth = health;
        _initialHealth = health;
        _movementDirection = Vector3.zero;
        _targetRenderer.transform.localPosition = Vector3.zero;

        for (int i = 0; i < _pointsObj.Length; i++) {
            _pointsObj[i].SetActive(false);
        }
    }

    /// <summary>
    /// Method used to set the target's reference when first created.
    /// This reference is used later for moving the target between the object pooling lists.
    /// </summary>
    /// <param name="targetRef"></param>
    public void SetTargetReference(TargetSpawnManager.TargetReferences targetRef) {
        _targetReference = targetRef;
    }

    /// <summary>
    /// Method to be called once the target has been spawned in order to tell it to enable itself.
    /// </summary>
    public void TargetSpawned() {
        _targetRenderer.gameObject.SetActive(true);
        _targetIsAlive = true;
    }

    /// <summary>
    /// Method used when a target has been spawned.
    /// Orientates the target and sets the movement direction.
    /// </summary>
    /// <param name="direction">Global Movement direction</param>
    /// <param name="forwardOrientation">Looking direction</param>
    public void OrientateAndLaunchTarget(Vector3 direction, Vector3 forwardOrientation) {
        this.gameObject.transform.right = forwardOrientation;   //Since the target is rotated instead of forward I use right as the front.
        _movementDirection = direction;
    }
	#endregion

    #region Unity Lifecycle
    /// <summary>
    /// Creates materialPropBlock once.
    /// </summary>
    private void Awake() {
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
                UpdateTargetColor(_targetColorProgression[RoundUpHealthToInt(_currentHealth) - 1]);    //FloorToInt could be used, would change the behaviour so testing needed.
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
        _pointsObj[RoundUpHealthToInt(_initialHealth) - 1].SetActive(true);
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

    /// <summary>
    /// Used to be able to use health as an index for the PointsObject and TargetColor selection arrays.
    /// Function that returns the rounded up int of the float health of the target.
    /// </summary>
    /// <param name="health">Health to be used as index.</param>
    /// <returns>CeilToInt rounding.</returns>
    private int RoundUpHealthToInt(float health) {
        return Mathf.CeilToInt(health);
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
