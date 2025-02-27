using UnityEngine;
using System.Collections.Generic;

public class ScanManager : MonoBehaviour
{
    private Dictionary<GameObject, bool> _scannedItems = new Dictionary<GameObject, bool>();
    public static ScanManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsScanned(GameObject item)
    {
        return _scannedItems.ContainsKey(item) && _scannedItems[item];
    }

    public void ScanItem(GameObject item)
    {
        if (!_scannedItems.ContainsKey(item))
        {
            _scannedItems[item] = true;
        }
        else
        {
            _scannedItems[item] = true; // Ќа случай, если нужно пересканировать
        }
    }

    public void ResetScannedItems()
    {
        _scannedItems.Clear();
    }
}