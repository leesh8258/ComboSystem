using System;
using UnityEngine;

public class PlayerWeaponEquipController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerDamageController damageController;

    [Header("Equip Socket")]
    [SerializeField] private Transform equipSocket;

    private GameWeaponSO currentWeapon;
    private GameObject currentWeaponInstance;

    public event Action<GameWeaponSO> OnWeaponEquipped;
    public event Action OnWeaponCleared;

    public GameWeaponSO CurrentWeapon => currentWeapon;
    public GameObject CurrentWeaponInstance => currentWeaponInstance;
    public bool HasWeapon => currentWeapon != null;

    private void Reset()
    {
        damageController = GetComponent<PlayerDamageController>();
    }

    public void EquipWeapon(GameWeaponSO weapon)
    {
        if (!CanEquipWeapon(weapon)) return;

        ClearCurrentWeapon();

        GameObject spawnedInstance = SpawnWeaponInstance(weapon);
        if (spawnedInstance == null) return;

        currentWeapon = weapon;
        currentWeaponInstance = spawnedInstance;

        ApplyWeaponTransform(weapon, spawnedInstance);
        BindWeaponHitbox(spawnedInstance);

        OnWeaponEquipped?.Invoke(currentWeapon);
    }

    public void UnequipWeapon()
    {
        ClearCurrentWeapon();
    }

    private bool CanEquipWeapon(GameWeaponSO weapon)
    {
        if (weapon == null)
        {
            return false;
        }

        if (equipSocket == null)
        {
            return false;
        }

        if (weapon.WeaponPrefab == null)
        {
            return false;
        }

        return true;
    }

    private GameObject SpawnWeaponInstance(GameWeaponSO weapon)
    {
        return Instantiate(weapon.WeaponPrefab, equipSocket);
    }

    private void ApplyWeaponTransform(GameWeaponSO weapon, GameObject instance)
    {
        Transform instanceTransform = instance.transform;
        instanceTransform.localPosition = weapon.EquipOffsetPosition;
        instanceTransform.localRotation = Quaternion.Euler(weapon.EquipOffsetRotation);
    }

    private void ClearCurrentWeapon()
    {
        bool hadWeapon = currentWeapon != null || currentWeaponInstance != null;

        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
        }

        currentWeapon = null;

        if (hadWeapon)
        {
            OnWeaponCleared?.Invoke();
        }
    }

    private void BindWeaponHitbox(GameObject instance)
    {
        if (instance == null || damageController == null) return;

        WeaponHitbox hitbox = instance.GetComponent<WeaponHitbox>();
        if (hitbox == null) return;

        hitbox.Setup(damageController);
    }
}