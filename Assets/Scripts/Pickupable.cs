using UnityEngine;

public class Pickupable : MonoBehaviour
{
    [SerializeField] private ItemDataSO _itemData; // ”ниверсальный SO дл€ всех данных

    public ItemDataSO ItemData => _itemData;
}