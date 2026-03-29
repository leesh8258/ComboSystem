using UnityEngine;

public class WeaponEquipUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerWeaponEquipController weaponEquipController;

    [Header("Weapon List")]
    [SerializeField] private GameWeaponSO[] weaponList;

    public void EquipWeaponIndex(int index)
    {
        GameWeaponSO weapon = weaponList[index];
        if (weapon == null) return;

        weaponEquipController.EquipWeapon(weapon);
    }

    public void UnequipWeapon()
    {
        if (weaponEquipController == null) return;

        weaponEquipController.UnequipWeapon();
    }
}
