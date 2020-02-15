using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a manager that spawns a target, once the target's health is down to 0 it despawns it and spawns the explosion effect.
/// Once the explosion effect is done it despawns the explosion effect.
/// </summary>
public class TargetSpawnManager : MonoBehaviour
{
    #region Type Definitions
    public struct TargetReferences {
        public GameObject mainObj;
        public TargetController targetController;
    }

    [Serializable]
    private struct TargetSpawnArea {
        public Collider areaCollider;
        public Vector3 forwardOrientation;
        public Vector3 launchDirection;
    }
	#endregion
	
    #region Events
	#endregion

    #region Constants
	#endregion

    #region Serialized Fields
    [SerializeField] private Transform _targetsParent = null;   //Reference to the transform of the parent where all the targets will be instantiated as childs of.
    [SerializeField] private int _maxNumberOfTargets = 20;      //Maximum number of targets that can be used at once.
    [SerializeField] private GameObject _targetPrefab = null;   //Main Parent of the TargetPrefab (Has the TargetController component on it).

    [SerializeField] private Transform _targetSpawn = null;     //Temporal spawn method.
    [SerializeField] private TargetSpawnArea[] _targetSpawnAreas;
	#endregion

    #region Standard Attributes
    private List<TargetReferences> _availableTargets;   //List of available targets to be used by the spawner.
    private List<TargetReferences> _onUseTargets;       //List of the targets already spawned.

    int _pointSearchCounter = 0;
	#endregion

    #region Consultors and Modifiers
	#endregion

    #region API Methods
    /// <summary>
    /// Method called by the Targets themselves once they are hit enough times to be destroyed.
    /// Disables the GO of the target and adds it to the available targets list.
    /// </summary>
    /// <param name="target"></param>
    public void TargetDestroyed(TargetReferences target) {
        target.mainObj.SetActive(false);
        _onUseTargets.Remove(target);
        _availableTargets.Add(target);
    }
	#endregion

    #region Unity Lifecycle
    /// <summary>
    /// Instatiate all the needed targets.
    /// </summary>
    private void Awake() {
        _availableTargets = new List<TargetReferences>();
        _onUseTargets = new List<TargetReferences>();

        InstantiateObjectPool();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1)) {
            SpawnTarget1();
        }

        if(Input.GetKeyDown(KeyCode.F2)) {
            SpawnTarget2();
        }

        if(Input.GetKeyDown(KeyCode.F3)) {
            SpawnTarget3();
        }
    }
	#endregion

    #region Unity Callback
	#endregion

    #region Other methods
    /// <summary>
    /// Fills up the list of available Targets to be used when needed to be spawned.
    /// Sends a copy of the reference correspondent to each target to itself so that it can be used when destruction is needed.
    /// </summary>
    private void InstantiateObjectPool() {
        for(int i = 0; i < _maxNumberOfTargets; ++i) {
            GameObject temp = GameObject.Instantiate(_targetPrefab, _targetsParent);
            TargetReferences tempTargetRef = default;
            tempTargetRef.mainObj = temp;
            tempTargetRef.targetController = temp.GetComponent<TargetController>();
            tempTargetRef.targetController.SetTargetReference(tempTargetRef);
            _availableTargets.Add(tempTargetRef);
        }
    }

    private void SpawnTarget1() {
        SpawnTarget(1);
    }

    private void SpawnTarget2() {
        SpawnTarget(2);
    }

    private void SpawnTarget3() {
        SpawnTarget(3);
    }

    /// <summary>
    /// Spawns a target.
    /// Resets the target health value.
    /// Calls the TargetSpawned method that tells the target that it has been spawned so that it can do whatever is needed.
    /// </summary>
    private void SpawnTarget(int targetHealth = 1) {
        _pointSearchCounter = 0;
        int randomSpawnArea = UnityEngine.Random.Range(0, _targetSpawnAreas.Length);
        Vector3 tempSpawnPos = GetRandomPointInCollider(_targetSpawnAreas[randomSpawnArea].areaCollider);
        if(_availableTargets.Count > 0) {
            TargetReferences tempTargetRef = _availableTargets[_availableTargets.Count - 1];
            tempTargetRef.mainObj.transform.position = tempSpawnPos;
            tempTargetRef.targetController.ResetTarget(targetHealth);
            tempTargetRef.targetController.TargetSpawned();
            tempTargetRef.mainObj.SetActive(true);
            tempTargetRef.targetController.OrientateAndLaunchTarget(_targetSpawnAreas[randomSpawnArea].launchDirection, _targetSpawnAreas[randomSpawnArea].forwardOrientation);
            _onUseTargets.Add(tempTargetRef);
            _availableTargets.RemoveAt(_availableTargets.Count - 1);
        }
    }

    /// <summary>
    /// Function that returns a random point inside a collider bounds.
    /// If the point is not inside the collider (Bounds are a box when the collider might actually be a sphere): Look for another point for up to ten times.
    /// </summary>
    /// <param name="collider">Spawn area collider.</param>
    /// <returns>Random point inside those bounds.</returns>
    private Vector3 GetRandomPointInCollider(Collider collider) {
        ++_pointSearchCounter;

        Vector3 point = new Vector3(
            UnityEngine.Random.Range(collider.bounds.min.x, collider.bounds.max.x),
            UnityEngine.Random.Range(collider.bounds.min.y, collider.bounds.max.y),
            UnityEngine.Random.Range(collider.bounds.min.z, collider.bounds.max.z)
        );
 
        if (_pointSearchCounter < 10 && point != collider.ClosestPoint(point))
        {
            Debug.Log("Out of the collider! Looking for other point...");
            point = GetRandomPointInCollider(collider);
        }
 
        return point;
    }
    
    [ContextMenu("Spawn Chain of Targets")]
    private void TargetSpawnChain() {
        StartCoroutine(TimedRandomTargetSpawn());
    }

    private IEnumerator TimedRandomTargetSpawn() {
        int targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
        targetHealth = UnityEngine.Random.Range(1, 4);
        SpawnTarget(targetHealth);
        yield return new WaitForSeconds(0.5f);
    }
	#endregion

}
