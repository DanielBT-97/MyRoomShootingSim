using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    #region Type Definitions
	#endregion
	
    #region Events
	#endregion

    #region Constants
	#endregion

    #region Serialized Fields
    [SerializeField] private Renderer _targetRenderer = null;
    [SerializeField] private ParticleSystem _destroyEffect = null;
    [SerializeField] private float _currentHealth = 1f;
    [SerializeField, ColorUsageAttribute(true, true)] private Color[] _targetColorProgression = new Color[3];
	#endregion

    #region Standard Attributes
    private MaterialPropertyBlock _matPropBlock;
    private Color _currentColor = Color.black;
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
    public void Hit(float bulletDamage) {
        Debug.Log("HIT TARGET");
        _currentHealth -= bulletDamage;

        UpdateState();
    }
	#endregion

    #region Unity Lifecycle
    private void Awake() {
        _matPropBlock = new MaterialPropertyBlock();
        Debug.Log("CEIL 0: " + Mathf.CeilToInt(0));
    }

    private void Update() {
        //Update target colors based on current health.
        if(_currentHealth <= 0) {   //If current health is lower than 0 (Should get destroyed) use same color as 1 hit.
            UpdateTargetColor(_targetColorProgression[0]);
        } else {    //Else use the color corresponding to the rounded UP int value of current health (In case I decide to use a float dmg value at some point).
            UpdateTargetColor(_targetColorProgression[Mathf.CeilToInt(_currentHealth - 1)]);    //FloorToInt could be used, would change the behaviour so testing needed.
        }
    }
	#endregion

    #region Unity Callback
	#endregion

    #region Other methods
    private void UpdateState() {
        if(_currentHealth <= 0) {
            //Kill target.
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
