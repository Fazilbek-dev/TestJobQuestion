using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/ItemData", order = 1)]
public class ItemDataSO : ScriptableObject
{
    [SerializeField] private string _itemName = "Unnamed Item"; // ��� ��������
    [SerializeField] private Sprite _itemSprite; // ������ ��� UI
    [SerializeField] private bool _isScannable = false; // ����� �� ���� ������������
    [SerializeField] private float _price = 0f; // ���� (��� ����������� ��������)

    public string ItemName => _itemName;
    public Sprite ItemSprite => _itemSprite;
    public bool IsScannable => _isScannable;
    public float Price => _price;
}