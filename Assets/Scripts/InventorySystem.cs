using UnityEngine;
using System.Collections.Generic;
using TMPro; // ��������� ������������ ���� ��� TextMeshPro
using System.Collections; // ��� �������

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private int _inventorySize = 3;
    [SerializeField] private float _reachDistance = 2f;
    [SerializeField] private float _rotationSpeed = 360f;
    [SerializeField] private Transform _handTransform;
    [SerializeField] private TextMeshProUGUI _scanText; // UI-������� ��� ����������� ������ ������������

    private List<GameObject> _inventory;
    private int _currentItemIndex = 0;
    private GameObject _heldItem;
    private Camera _playerCamera;

    public System.Action OnInventoryChanged;

    void Awake()
    {
        _inventory = new List<GameObject>(_inventorySize);
        for (int i = 0; i < _inventorySize; i++)
        {
            _inventory.Add(null);
        }
        _playerCamera = Camera.main;

        if (_handTransform == null)
        {
            Debug.LogError("HandTransform �� ����� � InventorySystem!");
        }

        if (_scanText == null)
        {
            Debug.LogError("ScanText �� ����� � InventorySystem!");
        }
        else
        {
            _scanText.gameObject.SetActive(false); // �������� ����� ��� ������
        }

        OnInventoryChanged?.Invoke();
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
            TryPickupItem();

        if (Input.GetKeyDown(KeyCode.Q))
            DropHeldItem();

        for (int i = 0; i < _inventorySize; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                SelectSlot(i);
        }

        if (Input.GetKeyDown(KeyCode.F))
            EquipHeldItem();

        if (_heldItem != null && Input.GetKey(KeyCode.R))
            RotateItem();

        if (Input.GetKeyDown(KeyCode.V))
            ZoomItem(true);
        if (Input.GetKeyUp(KeyCode.V))
            ZoomItem(false);

        if (Input.GetKeyDown(KeyCode.C))
            TryScanItem();
    }

    public void TryPickupItem()
    {
        Ray ray = _playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, _reachDistance))
        {
            Pickupable pickupable = hit.collider.GetComponent<Pickupable>();
            if (pickupable != null)
            {
                GameObject item = hit.collider.gameObject;

                if (_currentItemIndex >= 0 && _currentItemIndex < _inventory.Count && _inventory[_currentItemIndex] == null)
                {
                    _inventory[_currentItemIndex] = item;
                    item.SetActive(false);
                    EquipHeldItem();
                    Debug.Log($"������� {pickupable.ItemData?.ItemName ?? item.name} �������� � ���� {_currentItemIndex} � ���� � ����");
                    OnInventoryChanged?.Invoke();
                    return;
                }

                for (int i = 0; i < _inventory.Count; i++)
                {
                    if (_inventory[i] == null)
                    {
                        _inventory[i] = item;
                        _currentItemIndex = i;
                        item.SetActive(false);
                        EquipHeldItem();
                        Debug.Log($"������� {pickupable.ItemData?.ItemName ?? item.name} �������� � ���� {i} � ���� � ����");
                        OnInventoryChanged?.Invoke();
                        return;
                    }
                }
                Debug.Log("��������� �����!");
            }
            else
            {
                Debug.Log("������ �� �������� �����������!");
            }
        }
        else
        {
            Debug.Log("������ �� ������ � ��� �������!");
        }
    }

    private void SelectSlot(int index)
    {
        if (index >= 0 && index < _inventory.Count)
        {
            if (_heldItem != null)
                UnequipHeldItem();

            _currentItemIndex = index;
            EquipHeldItem();
            Debug.Log($"������ ���� {index}" + (_inventory[index] != null ? $" � ��������� {_inventory[index].GetComponent<Pickupable>()?.ItemData?.ItemName ?? _inventory[index].name}" : " (������)"));
            OnInventoryChanged?.Invoke();
        }
    }

    private void EquipHeldItem()
    {
        if (_currentItemIndex >= 0 && _currentItemIndex < _inventory.Count && _inventory[_currentItemIndex] != null && _handTransform != null)
        {
            if (_heldItem != null)
                UnequipHeldItem();

            _heldItem = _inventory[_currentItemIndex];
            _heldItem.SetActive(true);
            _heldItem.transform.SetParent(_handTransform);
            _heldItem.transform.localPosition = Vector3.zero;
            _heldItem.transform.localRotation = Quaternion.identity;

            Rigidbody rb = _heldItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            Debug.Log($"� �����: {_heldItem.GetComponent<Pickupable>()?.ItemData?.ItemName ?? _heldItem.name}");
            OnInventoryChanged?.Invoke();
        }
    }

    private void UnequipHeldItem()
    {
        if (_heldItem != null)
        {
            Rigidbody rb = _heldItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }

            _heldItem.transform.SetParent(null);
            _heldItem.SetActive(false);
            _heldItem = null;
            Debug.Log("������� ����� �� ���");
            OnInventoryChanged?.Invoke();
        }
    }

    private void DropHeldItem()
    {
        if (_heldItem != null)
        {
            int index = _currentItemIndex;
            Vector3 dropPosition = _heldItem.transform.position;

            Rigidbody rb = _heldItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
            }

            _heldItem.transform.SetParent(null);
            _heldItem.transform.position = dropPosition;
            _heldItem.SetActive(true);
            _inventory[index] = null;
            _heldItem = null;
            _currentItemIndex = -1;
            Debug.Log($"������� �������� �� ����� {index}");
            OnInventoryChanged?.Invoke();
        }
        else
        {
            Debug.Log("� ����� ������ ��� ��� �������!");
        }
    }

    private void TryScanItem()
    {
        if (_heldItem != null && _heldItem.GetComponent<Scanner>() != null)
        {
            Scanner scanner = _heldItem.GetComponent<Scanner>();
            Ray ray = _playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, _reachDistance))
            {
                Pickupable pickupable = hit.collider.GetComponent<Pickupable>();
                if (pickupable != null && scanner.CanScan(pickupable))
                {
                    ItemDataSO data = pickupable.ItemData;
                    if (data != null && data.IsScannable && !ScanManager.Instance.IsScanned(hit.collider.gameObject))
                    {
                        string scanMessage = $"����� {data.ItemName} ������������, ����: {data.Price}";
                        Debug.Log(scanMessage);
                        if (_scanText != null)
                        {
                            _scanText.text = scanMessage;
                            _scanText.gameObject.SetActive(true);
                            StartCoroutine(HideScanTextAfterDelay(5f)); // �������� ����� 5 ������
                        }
                    }
                    else if(data != null && data.IsScannable && ScanManager.Instance.IsScanned(hit.collider.gameObject))
                    {
                        string scanMessage = $"����� {data.ItemName} ��� ��� ������������, ����: {data.Price}";
                        Debug.Log(scanMessage);
                        if (_scanText != null)
                        {
                            _scanText.text = scanMessage;
                            _scanText.gameObject.SetActive(true);
                            StartCoroutine(HideScanTextAfterDelay(5f)); // �������� ����� 5 ������
                        }
                    }
                    ScanManager.Instance.ScanItem(hit.collider.gameObject);
                    OnInventoryChanged?.Invoke();
                }
                else
                {
                    Debug.Log("������ �� �������� �����������");
                }
            }
            else
            {
                Debug.Log("������ �� ������ � ��� ��� ������������!");
            }
        }
        else
        {
            Debug.Log("� ����� ������ ���� ������ ��� ������������!");
        }
    }

    private IEnumerator HideScanTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_scanText != null)
        {
            _scanText.gameObject.SetActive(false);
        }
    }

    private void RotateItem()
    {
        float angle = Input.GetKey(KeyCode.LeftShift) ? 90f : 45f;
        _heldItem.transform.Rotate(0, angle * Time.deltaTime * _rotationSpeed / 45f, 0, Space.Self);
    }

    private void ZoomItem(bool zoomIn)
    {
        if (_heldItem != null)
        {
            Vector3 targetPos = zoomIn ? new Vector3(0, 0, -0.5f) : Vector3.zero;
            _heldItem.transform.localPosition = targetPos;
        }
    }

    public GameObject GetHeldItem() => _heldItem;
    public int GetInventorySize() => _inventorySize;
    public int GetCurrentSlotIndex() => _currentItemIndex;
    public GameObject GetInventoryItem(int index)
    {
        if (_inventory == null)
        {
            Debug.LogError("������ _inventory �� ���������������!");
            return null;
        }

        if (index >= 0 && index < _inventory.Count)
        {
            return _inventory[index];
        }

        Debug.LogWarning($"������������ ������ {index} ��� ��������� �������� {_inventory.Count}");
        return null;
    }
}