using UnityEngine;

public class Pickupable : MonoBehaviour
{
    [SerializeField] private ItemDataSO _itemData; // ������������� SO ��� ���� ������

    public ItemDataSO ItemData => _itemData;
}