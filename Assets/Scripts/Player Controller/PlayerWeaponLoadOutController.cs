using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponLoadoutController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController inputController;
    [SerializeField] private PlayerWeaponEquipController equipController;

    [Header("Weapon Slots")]
    [SerializeField] private List<GameWeaponSO> weaponSlots;

    private int currentSlotIndex = -1;

    private void Reset()
    {
        inputController = GetComponent<PlayerInputController>();
        equipController = GetComponent<PlayerWeaponEquipController>();
    }

    private void OnEnable()
    {
        if (inputController != null)
        {
            inputController.OnWeaponSlotPressed += HandleWeaponSlotPressed;
        }
    }

    private void OnDisable()
    {
        if (inputController != null)
        {
            inputController.OnWeaponSlotPressed -= HandleWeaponSlotPressed;
        }
    }

    private void HandleWeaponSlotPressed(int slotIndex)
    {
        GameWeaponSO targetWeapon = weaponSlots[slotIndex];

        if (targetWeapon == null) return;

        bool isSameSlotPressed = currentSlotIndex == slotIndex;
        bool isSameWeaponEquipped = equipController.HasWeapon && equipController.CurrentWeapon == targetWeapon;

        if (isSameSlotPressed && isSameWeaponEquipped)
        {
            equipController.UnequipWeapon();
            currentSlotIndex = -1;
            return;
        }

        equipController.EquipWeapon(targetWeapon);
        currentSlotIndex = slotIndex;
    }
}