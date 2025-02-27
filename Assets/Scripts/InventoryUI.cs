using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventorySystem _inventorySystem;
    [SerializeField] private Image[] _inventorySlots;
    [SerializeField] private Sprite _emptySlotSprite;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _selectedColor = Color.yellow;

    void Awake()
    {
        if (_inventorySystem == null)
        {
            Debug.LogError("InventorySystem не задан в InventoryUI!");
            return;
        }

        if (_inventorySlots.Length != _inventorySystem.GetInventorySize())
        {
            Debug.LogError("Количество слотов в UI не соответствует размеру инвентаря!");
            return;
        }

        _inventorySystem.OnInventoryChanged += UpdateInventoryUI;
        UpdateInventoryUI();
    }

    void OnDestroy()
    {
        if (_inventorySystem != null)
            _inventorySystem.OnInventoryChanged -= UpdateInventoryUI;
    }

    public void UpdateInventoryUI()
    {
        int currentSlot = _inventorySystem.GetCurrentSlotIndex();

        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            GameObject item = _inventorySystem.GetInventoryItem(i);
            if (item != null)
            {
                Pickupable pickupable = item.GetComponent<Pickupable>();
                if (pickupable != null && pickupable.ItemData != null && pickupable.ItemData.ItemSprite != null)
                {
                    _inventorySlots[i].sprite = pickupable.ItemData.ItemSprite;
                }
                else
                {
                    _inventorySlots[i].sprite = _emptySlotSprite;
                }
            }
            else
            {
                _inventorySlots[i].sprite = _emptySlotSprite;
            }

            _inventorySlots[i].transform.GetChild(0).GetComponent<Image>().color = (i == currentSlot && currentSlot >= 0) ? _selectedColor : _normalColor;
        }
    }
}