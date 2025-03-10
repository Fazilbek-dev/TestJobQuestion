# First Person Inventory and Scanner System

Эта система реализована для Unity и предоставляет управление игроком от первого лица, инвентарь с подбираемыми предметами и механизм сканирования товаров с использованием `ScriptableObject`. Основные функции включают движение, подбор предметов, переключение слотов инвентаря, сканирование и отображение информации через UI.

## Основные компоненты

### 1. PlayerMovement.cs
Управляет движением игрока и вращением камеры от первого лица.

- **Функции**:
  - Движение: WASD.
  - Прыжок: Space.
  - Приседание: X.
  - Вращение камеры: Мышь (чувствительность настраивается).
  - Покачивание камеры и звук шагов при движении.

- **Настройка**:
  - Прикрепите к объекту игрока с `CharacterController` и `AudioSource`.
  - Камера должна быть дочерним объектом с тегом `MainCamera`.
  - Настройте `_mouseSensitivity` (например, 300) и `_moveSpeed` (например, 5) в инспекторе.

### 2. InventorySystem.cs
Управляет инвентарем, подбором предметов и сканированием.

- **Функции**:
  - Подбор: `E` — поднимает объект в текущий слот и берет в руки.
  - Выброс: `Q` — выбрасывает предмет из рук.
  - Переключение слотов: `1`, `2`, `3`.
  - Взятие в руки: `F` (хотя предмет берется автоматически при подборе).
  - Вращение предмета: `R` (45°) и `Shift + R` (90°).
  - Зум: `V` — приближает/отдаляет предмет.
  - Сканирование: `C` — сканирует объект, отображает текст на 5 секунд.

- **Настройка**:
  - Прикрепите к объекту игрока.
  - Задайте `_handTransform` (например, пустой объект "Hand" в иерархии).
  - Задайте `_scanText` (TextMeshProUGUI в Canvas).
  - Настройте `_inventorySize` (по умолчанию 3).

### 3. InventoryUI.cs
Отображает инвентарь в UI с подсветкой активного слота.

- **Функции**:
  - Показывает спрайты предметов в слотах.
  - Подсвечивает текущий слот желтым цветом.

- **Настройка**:
  - Прикрепите к объекту UI (например, "InventoryUI").
  - Задайте `_inventorySystem` (ссылка на объект с `InventorySystem`).
  - Задайте `_inventorySlots` (массив `Image`, соответствующий `_inventorySize`).
  - Задайте `_emptySlotSprite` (спрайт для пустого слота).

### 4. ItemDataSO.cs
Универсальный `ScriptableObject` для данных о подбираемых предметах.

- **Поля**:
  - `_itemName`: Имя предмета.
  - `_itemSprite`: Спрайт для UI.
  - `_isScannable`: Флаг сканируемости.
  - `_price`: Цена (для сканируемых объектов).

- **Создание**:
  - ПКМ в Project → Create → Inventory → ItemData.
  - Примеры: "ScannerSO" (`_isScannable = false`), "AppleSO" (`_isScannable = true`, `_price = 1.5`).

### 5. Pickupable.cs
Маркер для подбираемых объектов.

- **Настройка**:
  - Добавьте на объект с `Rigidbody`.
  - Задайте `_itemData` (ссылка на `ItemDataSO`).

### 6. Scanner.cs
Компонент для сканера, определяет, какие объекты можно сканировать.

- **Настройка**:
  - Добавьте на объект сканера (вместе с `Pickupable`).
  - Задайте `_scannableItems` (массив `ItemDataSO`, например, "AppleSO").

### 7. ScanManager.cs
Управляет состоянием сканирования.

- **Функции**:
  - Отмечает объекты как отсканированные.
  - Хранит данные в `Dictionary`.

- **Настройка**:
  - Прикрепите к объекту (например, "ScanManager") с `DontDestroyOnLoad`.

---

## Установка

1. **Создание игрока**:
   - Создайте объект с `CharacterController`, `AudioSource`, `PlayerMovement`, `InventorySystem`.
   - Добавьте дочерний объект "Camera" (тег `MainCamera`).
   - Добавьте дочерний объект "Hand" (пустой, для `_handTransform`).

2. **Создание UI**:
   - Создайте Canvas.
   - Добавьте 3 объекта с `Image` для слотов инвентаря (например, "Slot0", "Slot1", "Slot2").
   - Добавьте `TextMeshProUGUI` для текста сканирования (например, "ScanText").
   - Создайте объект "InventoryUI" с `InventoryUI`, настройте ссылки.

3. **Создание объектов**:
   - **Сканер**: Объект с `Pickupable` (`_itemData = ScannerSO`), `Scanner` (`_scannableItems = [AppleSO]`), `Rigidbody`.
   - **Товар**: Объект с `Pickupable` (`_itemData = AppleSO`), `Rigidbody`.

4. **Настройка ItemDataSO**:
   - Создайте экземпляры в Project (например, "ScannerSO", "AppleSO").

---

## Использование

1. **Движение**:
   - Используйте WASD, Space, X и мышь для управления игроком.

2. **Инвентарь**:
   - Нажмите `E`, чтобы подобрать объект.
   - Переключайтесь между слотами (`1`, `2`, `3`).
   - Нажмите `Q`, чтобы выбросить предмет.

3. **Сканирование**:
   - Возьмите сканер в руки, направьте на товар, нажмите `C`.
   - Текст сканирования появится в UI на 5 секунд.

---

## Пример работы

- Подбираете сканер (`E`) → слот показывает спрайт "Scanner".
- Подбираете яблоко (`E`) → слот показывает спрайт "Apple".
- Сканируете яблоко (`C`) → текст "Товар Apple отсканирован, цена: 1.5" появляется на 5 секунд.

---

## Возможные улучшения

- **UI анимация**: Добавить анимацию появления/исчезновения текста.
- **Категории**: Расширить `ItemDataSO` для поддержки типов предметов.
- **Звук**: Добавить звуки для подбора и сканирования.
