using UnityEngine;

public class Scanner : MonoBehaviour
{
    [SerializeField] private ItemDataSO[] _scannableItems; // ������ ���������� ItemDataSO ��� ������������

    public bool CanScan(Pickupable pickupable)
    {
        if (pickupable != null && pickupable.ItemData != null && _scannableItems != null)
        {
            foreach (var scannableItem in _scannableItems)
            {
                if (pickupable.ItemData == scannableItem)
                {
                    return true;
                }
            }
        }
        return false;
    }
}