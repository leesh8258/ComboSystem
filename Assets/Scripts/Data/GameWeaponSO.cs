using UnityEngine;

[CreateAssetMenu(fileName = "GameWeaponSO", menuName = "Game/Combat/Weapon")]
public class GameWeaponSO : ScriptableObject
{
    [Header("Name")]
    [SerializeField] private string weaponName;

    [Header("Weapon Visual")]
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private Vector3 equipOffsetPosition;
    [SerializeField] private Vector3 equipOffsetRotation;

    [Header("Combo")]
    [SerializeField] private GameComboData comboData;

    public string WeaponName => weaponName;
    public GameObject WeaponPrefab => weaponPrefab;
    public Vector3 EquipOffsetPosition => equipOffsetPosition;
    public Vector3 EquipOffsetRotation => equipOffsetRotation;
    public GameComboData ComboData => comboData;
}