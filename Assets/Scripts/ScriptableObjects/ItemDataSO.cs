using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/ItemData", order = 1)]
public class ItemDataSO : ScriptableObject
{
    [SerializeField] private string _itemName = "Unnamed Item"; // Имя предмета
    [SerializeField] private Sprite _itemSprite; // Спрайт для UI
    [SerializeField] private bool _isScannable = false; // Может ли быть отсканирован
    [SerializeField] private float _price = 0f; // Цена (для сканируемых объектов)

    public string ItemName => _itemName;
    public Sprite ItemSprite => _itemSprite;
    public bool IsScannable => _isScannable;
    public float Price => _price;
}