using FishingGameTool.Fishing.Rod;
using System.Collections;
using UnityEngine;

public class CastingSystem
{
    private readonly Transform _fishingRod;
    private readonly GameObject _fishingFloatPrefab;
    private readonly float _maxCastForce;
    private readonly float _forceChargeRate;
    private readonly float _spawnFloatDelay;
    private readonly FishingRod _fishingRodComponent;

    private float _currentCastForce;
    private bool _castFloat;

    public CastingSystem(Transform fishingRod, GameObject fishingFloatPrefab, float maxCastForce, float forceChargeRate, float spawnFloatDelay, FishingRod fishingRodComponent)
    {
        _fishingRod = fishingRod;
        _fishingFloatPrefab = fishingFloatPrefab;
        _maxCastForce = maxCastForce;
        _forceChargeRate = forceChargeRate;
        _spawnFloatDelay = spawnFloatDelay;
        _fishingRodComponent = fishingRodComponent;
    }

    public void UpdateCastForce(bool castInput)
    {
        if (_fishingRodComponent._fishingFloat != null || _castFloat || _fishingRodComponent._lineStatus._isLineBroken)
            return;

        if (castInput)
        {
            _currentCastForce = CalculateCastForce(_currentCastForce, _maxCastForce, _forceChargeRate);
        }
        else if (!castInput && _currentCastForce != 0f)
        {
            Vector3 spawnPoint = _fishingRodComponent._line._lineAttachment.position;
            Vector3 castDirection = _fishingRod.forward + Vector3.up;

            _fishingRodComponent.StartCoroutine(CastingDelay(_spawnFloatDelay, castDirection, spawnPoint, _currentCastForce));

            _currentCastForce = 0f;
        }
    }

    private IEnumerator CastingDelay(float delay, Vector3 castDirection, Vector3 spawnPoint, float castForce)
    {
        _castFloat = true;

        yield return new WaitForSeconds(delay);
        _fishingRodComponent._fishingFloat = Cast(castDirection, spawnPoint, castForce);
        _castFloat = false;
    }

    private Transform Cast(Vector3 castDirection, Vector3 spawnPoint, float castForce)
    {
        GameObject spawnedFishingFloat = Object.Instantiate(_fishingFloatPrefab, spawnPoint, Quaternion.identity);
        spawnedFishingFloat.GetComponent<Rigidbody>().AddForce(castDirection * castForce, ForceMode.Impulse);

        return spawnedFishingFloat.transform;
    }

    private float CalculateCastForce(float currentCastForce, float maxCastForce, float forceChargeRate)
    {
        currentCastForce += forceChargeRate * Time.deltaTime;
        currentCastForce = Mathf.Min(currentCastForce, maxCastForce);
        return currentCastForce;
    }
}
