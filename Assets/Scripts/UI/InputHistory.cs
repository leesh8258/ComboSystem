using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputHistory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController inputController;
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private TextMeshProUGUI itemPrefab;

    [Header("Settings")]
    [SerializeField] private int maxItemCount = 8;
    [SerializeField] private bool newestOnTop = true;

    private readonly Queue<TextMeshProUGUI> activeItems = new Queue<TextMeshProUGUI>();

    private void OnEnable()
    {
        if (inputController == null) return;

        inputController.OnAttackInput += HandleAttackInput;
        inputController.OnWeaponSlotPressed += HandleWeaponSlotPressed;
    }

    private void OnDisable()
    {
        if (inputController == null) return;

        inputController.OnAttackInput -= HandleAttackInput;
        inputController.OnWeaponSlotPressed -= HandleWeaponSlotPressed;
    }

    private void HandleAttackInput(GameAttackInputType attackType)
    {
        switch (attackType)
        {
            case GameAttackInputType.LeftClick:
                AddInputText("L");
                break;

            case GameAttackInputType.RightClick:
                AddInputText("R");
                break;

            default:
                AddInputText(attackType.ToString());
                break;
        }
    }

    private void HandleWeaponSlotPressed(int slotIndex)
    {
        AddInputText((slotIndex + 1).ToString());
    }

    private void AddInputText(string text)
    {
        if (contentRoot == null || itemPrefab == null) return;

        TextMeshProUGUI newItem = Instantiate(itemPrefab, contentRoot);
        newItem.text = text;

        if (newestOnTop)
        {
            newItem.transform.SetAsFirstSibling();
        }

        else
        {
            newItem.transform.SetAsLastSibling();
        }

        activeItems.Enqueue(newItem);

        while (activeItems.Count > maxItemCount)
        {
            TextMeshProUGUI oldestItem = activeItems.Dequeue();

            if (oldestItem != null)
            {
                Destroy(oldestItem.gameObject);
            }
        }
    }

    public void ClearHistory()
    {
        while (activeItems.Count > 0)
        {
            TextMeshProUGUI item = activeItems.Dequeue();

            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
    }
}
