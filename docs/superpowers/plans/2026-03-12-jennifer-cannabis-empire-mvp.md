# Jennifer's Cannabis Empire — MVP Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implementiere den vollständigen MVP eines 3D Low-Poly Cannabis-Simulators in Unity mit Jennifer als spielbarem Charakter, Anbau-/Trocken-/Verpackungs-/Verkaufs-Loop, Wut-Meter und Polizei-Patrouille.

**Architecture:** Unity 3D (URP), Single-Scene-Loading für MVP, New Input System, Cinemachine FreeLook Kamera. Alle Gameplay-Logiken als entkoppelte C# MonoBehaviours mit ScriptableObjects für Datenkonfiguration. EditMode-Tests für alle pure-C# Logiken (Wirtschaft, Wut, Zeit).

**Tech Stack:** Unity 6 LTS, URP, Cinemachine 3.x, Unity Input System 1.x, Unity Test Framework (EditMode), Newtonsoft.Json (Speichern), TextMeshPro (UI)

---

## Chunk 1: Unity Setup & Projektstruktur

### Task 1: Unity Hub & Unity installieren

**Voraussetzung:** Diese Schritte werden manuell im Terminal ausgeführt.

- [ ] **Step 1: Unity Hub installieren (Pop!_OS)**

```bash
# AppImage herunterladen
wget -O ~/Downloads/UnityHub.AppImage \
  "https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.AppImage"
chmod +x ~/Downloads/UnityHub.AppImage

# FUSE installieren (benötigt für AppImage)
sudo apt install libfuse2 -y

# Unity Hub starten
~/Downloads/UnityHub.AppImage
```

- [ ] **Step 2: Unity 6 LTS installieren**
  - Unity Hub öffnen → Installs → Install Editor
  - Version wählen: **Unity 6 (6000.x LTS)**
  - Module hinzufügen: **Linux Build Support**, **Windows Build Support**
  - Warten bis Installation abgeschlossen

- [ ] **Step 3: Neues Projekt erstellen**
  - Unity Hub → New Project
  - Template: **3D (URP)**
  - Name: `JenniferGame`
  - Location: `/home/tony/`
  - Create Project klicken

- [ ] **Step 4: Projekt-Ordnerstruktur anlegen**

Öffne das Unity-Projekt. Im Project-Panel rechtsklick auf `Assets` → Create → Folder:

```
Assets/
├── Characters/
├── City/
├── Materials/
├── Scenes/
│   └── Interior/
├── ScriptableObjects/
│   ├── Data/
├── Scripts/
│   ├── Core/
│   ├── Jennifer/
│   ├── NPC/
│   ├── Systems/
│   └── UI/
├── Tests/
│   └── EditMode/
└── Audio/
    ├── Music/
    └── SFX/
```

- [ ] **Step 5: Packages installieren**

Window → Package Manager → Add package by name:

```
com.unity.cinemachine          (Cinemachine)
com.unity.inputsystem          (New Input System)
com.unity.textmeshpro          (TextMeshPro)
com.unity.nuget.newtonsoft-json (JSON)
```

Nach Input System Installation: Popup "Enable new input system?" → **Yes** (Editor startet neu)

- [ ] **Step 6: Assembly Definitions erstellen (WICHTIG für Tests)**

**6a: Game-Scripts Assembly Definition**

In `Assets/Scripts/` rechtsklick → Create → Assembly Definition
Name: `JenniferGame.Runtime`
Inspector-Settings: alle Platforms ✓, keine Referenzen nötig

**6b: Test Assembly Definition**

In `Assets/Tests/EditMode/` rechtsklick → Create → Testing → **Assembly Definition**
Name: `JenniferGame.Tests.EditMode`

In der .asmdef Inspector:
- `Include Platforms`: Editor only ✓
- `Assembly Definition References`: `JenniferGame.Runtime` hinzufügen ✓

Ohne diese beiden `.asmdef` Dateien und die Referenz sind alle EditMode-Tests **unsichtbar** für den Test Runner und kompilieren nicht.

- [ ] **Step 7: Git-Repository verknüpfen**

Unity erzeugt bereits das Projekt unter `~/JenniferGame/`. Wir verknüpfen mit GitHub:

```bash
cd ~/JenniferGame
# .gitignore für Unity hinzufügen
curl -o .gitignore https://raw.githubusercontent.com/github/gitignore/main/Unity.gitignore

export PATH="$HOME/.local/bin:$PATH"
git add .gitignore
git commit -m "chore: add Unity .gitignore"

# GitHub Repo erstellen und pushen
gh repo create JenniferGame --public --source=. --remote=origin --push
```

- [ ] **Step 8: Verify**
  - Unity öffnet ohne Fehler
  - Package Manager zeigt alle 4 Packages als installed
  - `gh repo view JenniferGame` zeigt das Repository

---

## Chunk 2: Core Infrastructure

**Files:**
- Create: `Assets/Scripts/Core/GameManager.cs`
- Create: `Assets/Scripts/Core/TimeManager.cs`
- Create: `Assets/Scripts/Core/SaveData.cs`
- Create: `Assets/Scripts/Core/SaveSystem.cs`
- Create: `Assets/Tests/EditMode/TimeManagerTests.cs`
- Create: `Assets/Tests/EditMode/SaveSystemTests.cs`

### Task 2: TimeManager (Spielzeit-System)

1 Spieltag = 20 Minuten Echtzeit. 1 Spieltag hat 16 Spielstunden (06:00–22:00 aktiv).

- [ ] **Step 1: Failing Test schreiben**

`Assets/Tests/EditMode/TimeManagerTests.cs`:
```csharp
using NUnit.Framework;

public class TimeManagerTests
{
    // Testet TimeLogic (pure C# Klasse, kein MonoBehaviour)
    [Test]
    public void GetHourFromProgress_At0_Returns6()
    {
        float hour = TimeLogic.GetHourFromProgress(0f);
        Assert.AreEqual(6f, hour, 0.01f);
    }

    [Test]
    public void GetHourFromProgress_At05_Returns14()
    {
        float hour = TimeLogic.GetHourFromProgress(0.5f);
        Assert.AreEqual(14f, hour, 0.01f);
    }

    [Test]
    public void GetHourFromProgress_At1_Returns22()
    {
        float hour = TimeLogic.GetHourFromProgress(1f);
        Assert.AreEqual(22f, hour, 0.01f);
    }

    [Test]
    public void IsNight_At22_ReturnsTrue()
    {
        Assert.IsTrue(TimeLogic.IsNight(22f));
    }

    [Test]
    public void IsNight_At14_ReturnsFalse()
    {
        Assert.IsFalse(TimeLogic.IsNight(14f));
    }

    [Test]
    public void GetTimeString_At6h30_Returns0630()
    {
        string result = TimeLogic.GetTimeString(6.5f);
        Assert.AreEqual("06:30", result);
    }
}
```

- [ ] **Step 2: Test fehlschlagen lassen**

Unity → Window → General → Test Runner → EditMode → Run All
Erwartung: **FAIL** — `TimeLogic` existiert noch nicht

- [ ] **Step 3: TimeLogic (pure C#) + TimeManager implementieren**

`Assets/Scripts/Core/TimeLogic.cs`:
```csharp
public static class TimeLogic
{
    public const float StartHour = 6f;
    public const float EndHour = 22f;
    public const float DayDuration = EndHour - StartHour; // 16 hours

    public static float GetHourFromProgress(float dayProgress) =>
        StartHour + dayProgress * DayDuration;

    public static bool IsNight(float hour) =>
        hour >= EndHour || hour < StartHour;

    public static string GetTimeString(float hour)
    {
        int h = (int)hour;
        int m = (int)((hour - h) * 60f);
        return $"{h:D2}:{m:D2}";
    }
}
```

`Assets/Scripts/Core/TimeManager.cs`:
```csharp
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    [SerializeField] private float realSecondsPerGameDay = 1200f; // 20 min
    [SerializeField] private float startHour = 6f;
    [SerializeField] private float dayEndHour = 22f;

    public float CurrentHour { get; private set; }
    public int CurrentDay { get; private set; } = 1;
    public float DayProgress { get; private set; } // 0.0 to 1.0

    public UnityEvent<int> OnNewDay;
    public UnityEvent<float> OnHourChanged;

    private float _timer;
    private float _lastHour;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentHour = startHour;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        DayProgress = _timer / realSecondsPerGameDay;

        if (DayProgress >= 1f)
        {
            _timer = 0f;
            DayProgress = 0f;
            CurrentDay++;
            OnNewDay?.Invoke(CurrentDay);
        }

        CurrentHour = TimeLogic.GetHourFromProgress(DayProgress);
        if (Mathf.Floor(CurrentHour) != Mathf.Floor(_lastHour))
            OnHourChanged?.Invoke(CurrentHour);
        _lastHour = CurrentHour;
    }

    public bool IsNight() => TimeLogic.IsNight(CurrentHour);

    public string GetTimeString() => TimeLogic.GetTimeString(CurrentHour);

    // For save/load
    public void SetTime(float hour, int day, float timer)
    {
        CurrentHour = hour;
        CurrentDay = day;
        _timer = timer;
    }
}
```

- [ ] **Step 4: Tests laufen lassen → PASS**

- [ ] **Step 5: Commit**
```bash
git add Assets/Scripts/Core/TimeManager.cs Assets/Tests/EditMode/TimeManagerTests.cs
git commit -m "feat: add TimeManager with 20min game day cycle"
```

---

### Task 3: SaveData & SaveSystem

- [ ] **Step 1: SaveData Datenstruktur**

`Assets/Scripts/Core/SaveData.cs`:
```csharp
using System;
using System.Collections.Generic;

[Serializable]
public class PlantSaveData
{
    public string plantType; // "cheap", "normal", "premium"
    public int plantedDay;
    public int lastWateredDay;
    public int missedWaterDays;
    public bool isAlive;
}

[Serializable]
public class DryingItemSaveData
{
    public string quality; // "low", "medium", "high", "premium"
    public int startDay;
    public int durationDays;
    public int units;
}

[Serializable]
public class InventoryItemSaveData
{
    public string itemType; // "dried", "package_small", "package_medium", "package_large"
    public string quality;
    public int quantity;
}

[Serializable]
public class CustomerSaveData
{
    public string customerId;
    public int purchaseCount;
    public bool isLoyalCustomer;
}

[Serializable]
public class SaveData
{
    public float money = 500f;        // Aktuelles Guthaben
    public float totalEarned = 0f;    // Gesamt verdient (für Progression!)
    public int currentDay = 1;
    public float currentHour = 6f;
    public float dayTimer = 0f;
    public int progressionLevel = 1;
    public float wutLevel = 0f;
    public float policeAttention = 0f;
    public bool blackMarketUnlocked = false;
    public bool employeeHired = false;
    public float playerX, playerY, playerZ;
    public List<PlantSaveData> plants = new();
    public List<DryingItemSaveData> dryingItems = new();
    public List<InventoryItemSaveData> inventory = new();
    public List<CustomerSaveData> customers = new();
}
```

- [ ] **Step 2: Failing SaveSystem Test**

`Assets/Tests/EditMode/SaveSystemTests.cs`:
```csharp
using NUnit.Framework;
using System.IO;

public class SaveSystemTests
{
    private string _testPath;

    [SetUp]
    public void Setup()
    {
        _testPath = Path.Combine(Path.GetTempPath(), "jennifer_test_save.json");
    }

    [TearDown]
    public void Teardown()
    {
        if (File.Exists(_testPath)) File.Delete(_testPath);
    }

    [Test]
    public void Save_And_Load_PreservesMoney()
    {
        var data = new SaveData { money = 1234.56f };
        SaveSystem.SaveToPath(data, _testPath);
        var loaded = SaveSystem.LoadFromPath(_testPath);
        Assert.AreEqual(1234.56f, loaded.money, 0.01f);
    }

    [Test]
    public void Load_WhenFileNotExists_ReturnsNewSaveData()
    {
        var loaded = SaveSystem.LoadFromPath("/nonexistent/path.json");
        Assert.IsNotNull(loaded);
        Assert.AreEqual(500f, loaded.money, 0.01f);
    }

    [Test]
    public void Save_And_Load_PreservesProgressionLevel()
    {
        var data = new SaveData { progressionLevel = 2 };
        SaveSystem.SaveToPath(data, _testPath);
        var loaded = SaveSystem.LoadFromPath(_testPath);
        Assert.AreEqual(2, loaded.progressionLevel);
    }
}
```

- [ ] **Step 3: Test fehlschlagen lassen** → Test Runner → FAIL (SaveSystem existiert nicht)

- [ ] **Step 4: SaveSystem implementieren**

`Assets/Scripts/Core/SaveSystem.cs`:
```csharp
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveSystem
{
    private static string DefaultPath =>
        Path.Combine(Application.persistentDataPath, "jennifer_save.json");

    public static void Save(SaveData data) => SaveToPath(data, DefaultPath);

    public static SaveData Load() => LoadFromPath(DefaultPath);

    public static void SaveToPath(SaveData data, string path)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public static SaveData LoadFromPath(string path)
    {
        if (!File.Exists(path)) return new SaveData();
        try
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<SaveData>(json) ?? new SaveData();
        }
        catch
        {
            return new SaveData();
        }
    }
}
```

- [ ] **Step 5: Tests laufen lassen → PASS**

- [ ] **Step 6: GameManager implementieren**

`Assets/Scripts/Core/GameManager.cs`:
```csharp
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float Money { get; private set; }
    public float TotalEarned { get; private set; } // Für Progression
    public int ProgressionLevel { get; private set; } = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() => LoadGame();

    public void AddMoney(float amount)
    {
        Money += amount;
        if (amount > 0) TotalEarned += amount; // Nur Einnahmen zählen für Progression
        CheckProgression();
    }

    public bool SpendMoney(float amount)
    {
        if (Money < amount) return false;
        Money -= amount;
        return true;
    }

    private void CheckProgression()
    {
        // Prüft gesamt verdientes Geld, nicht aktuelles Guthaben (spec: "$1.500 verdient gesamt")
        if (ProgressionLevel == 1 && TotalEarned >= 1500f) UnlockLevel(2);
        else if (ProgressionLevel == 2 && TotalEarned >= 8000f) UnlockLevel(3);
    }

    private void UnlockLevel(int level)
    {
        ProgressionLevel = level;
        Debug.Log($"[GameManager] Level {level} freigeschaltet!");
    }

    public void SaveGame()
    {
        var data = new SaveData
        {
            money = Money,
            totalEarned = TotalEarned,
            progressionLevel = ProgressionLevel,
            currentDay = TimeManager.Instance?.CurrentDay ?? 1,
            currentHour = TimeManager.Instance?.CurrentHour ?? 6f,
            dayTimer = TimeManager.Instance != null
                ? TimeManager.Instance.CurrentDay * 1200f // approximiert
                : 0f,
            wutLevel = WutMeter.Instance?.WutLevel ?? 0f,
            policeAttention = PoliceAttentionSystem.Instance?.Attention ?? 0f,
        };

        // Spieler-Position
        var jennifer = GameObject.FindWithTag("Player");
        if (jennifer != null)
        {
            data.playerX = jennifer.transform.position.x;
            data.playerY = jennifer.transform.position.y;
            data.playerZ = jennifer.transform.position.z;
        }

        // Inventar
        if (InventoryManager.Instance != null)
            foreach (var item in InventoryManager.Instance.GetItems())
                data.inventory.Add(new InventoryItemSaveData
                    { itemType = item.itemType, quality = item.quality, quantity = item.quantity });

        SaveSystem.Save(data);
        Debug.Log("[GameManager] Spiel gespeichert.");
    }

    public void LoadGame()
    {
        var data = SaveSystem.Load();
        Money = data.money;
        TotalEarned = data.totalEarned;
        ProgressionLevel = data.progressionLevel;

        // Zeit wiederherstellen
        TimeManager.Instance?.SetTime(data.currentHour, data.currentDay, data.dayTimer);

        // Wut + Polizei wiederherstellen
        WutMeter.Instance?.AddWut(data.wutLevel);
        PoliceAttentionSystem.Instance?.AddAttention(data.policeAttention);

        // Spieler-Position wiederherstellen
        var jennifer = GameObject.FindWithTag("Player");
        if (jennifer != null)
            jennifer.transform.position = new Vector3(data.playerX, data.playerY, data.playerZ);

        // Inventar wiederherstellen
        if (InventoryManager.Instance != null)
            foreach (var item in data.inventory)
                InventoryManager.Instance.AddItem(item.itemType, item.quality, item.quantity);
    }
}
```

- [ ] **Step 7: Commit**
```bash
git add Assets/Scripts/Core/ Assets/Tests/EditMode/SaveSystemTests.cs
git commit -m "feat: add SaveData, SaveSystem, GameManager with JSON persistence"
```

---

## Chunk 3: Jennifer Bewegung & Kamera

**Files:**
- Create: `Assets/Scripts/Jennifer/JenniferMovement.cs`
- Create: `Assets/Scripts/Jennifer/JenniferInteraction.cs`
- Create: `Assets/Input/JenniferInputActions.inputactions`
- Modify: Unity Scene `Assets/Scenes/City.unity` (manuell im Editor)

### Task 4: Input Actions konfigurieren

- [ ] **Step 1: Input Action Asset erstellen**

In Unity: `Assets/Input/` → rechtsklick → Create → Input Actions
Name: `JenniferInputActions`

- [ ] **Step 2: Actions konfigurieren**

Doppelklick auf `JenniferInputActions` → Editor öffnet:

**Action Map: `Player`**

| Action | Type | Binding |
|--------|------|---------|
| Move | Value / Vector2 | WASD + Arrow Keys |
| Look | Value / Vector2 | Mouse Delta |
| Interact | Button | E |
| Intimidate | Button | F |
| Pause | Button | Escape |

Checkbox: **Generate C# Class** ✓ → Apply → Save

- [ ] **Step 3: Commit**
```bash
git add Assets/Input/
git commit -m "feat: add Jennifer input actions (WASD, look, interact, intimidate)"
```

---

### Task 5: Jennifer Bewegung (Third-Person)

- [ ] **Step 1: City Scene vorbereiten (manuell im Unity Editor)**

  - Öffne `Assets/Scenes/City.unity` (oder erstelle neue Scene)
  - Erstelle Plane als Boden: GameObject → 3D Object → Plane, Scale (10,1,10)
  - Erstelle Capsule als Jennifer-Placeholder: GameObject → 3D Object → Capsule
    - Tag: `Player`
    - Position: (0, 1, 0)
  - Erstelle leeres GameObject namens `CameraTarget` als Child von Jennifer (Position 0,1.5,0)

- [ ] **Step 2: Cinemachine FreeLook Camera einrichten**

  - GameObject → Cinemachine → Cinemachine FreeLook Camera
  - Follow: Jennifer's `CameraTarget`
  - Look At: Jennifer's `CameraTarget`
  - Orbits: Top(4,2), Middle(2.5,1.5), Bottom(0.5,0.5)

- [ ] **Step 3: JenniferMovement implementieren**

`Assets/Scripts/Jennifer/JenniferMovement.cs`:
```csharp
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class JenniferMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController _controller;
    private JenniferInputActions _input;
    private Vector2 _moveInput;
    private Vector3 _velocity;
    private bool _isPaused;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = new JenniferInputActions();
    }

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Move.performed += OnMove;
        _input.Player.Move.canceled += OnMove;
        _input.Player.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        _input.Player.Move.performed -= OnMove;
        _input.Player.Move.canceled -= OnMove;
        _input.Player.Pause.performed -= OnPause;
        _input.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx) =>
        _moveInput = ctx.ReadValue<Vector2>();

    private void OnPause(InputAction.CallbackContext ctx)
    {
        // PauseMenuUI wird in Chunk 9 implementiert — null-safe call
        var pauseMenu = FindObjectOfType<PauseMenuUI>();
        pauseMenu?.TogglePause();
    }

    private void Update()
    {
        if (_isPaused) return;

        ApplyGravity();
        MoveCharacter();
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void MoveCharacter()
    {
        if (_moveInput.sqrMagnitude < 0.01f) return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;

        Vector3 moveDir = (camForward.normalized * _moveInput.y +
                           camRight.normalized * _moveInput.x).normalized;

        _controller.Move(moveDir * walkSpeed * Time.deltaTime);

        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot,
                                               rotationSpeed * Time.deltaTime);
    }

    public void SetPaused(bool paused) => _isPaused = paused;
}
```

- [ ] **Step 4: JenniferInteraction implementieren**

`Assets/Scripts/Jennifer/JenniferInteraction.cs`:
```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class JenniferInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactableLayer;

    private JenniferInputActions _input;

    private void Awake() => _input = new JenniferInputActions();

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Interact.performed += OnInteract;
        _input.Player.Intimidate.performed += OnIntimidate;
    }

    private void OnDisable()
    {
        _input.Player.Interact.performed -= OnInteract;
        _input.Player.Intimidate.performed -= OnIntimidate;
        _input.Player.Disable();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        // Raycast nach vorne für interaktierbare Objekte
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward,
            out RaycastHit hit, interactRange, interactableLayer))
        {
            hit.collider.GetComponent<IInteractable>()?.Interact(this);
            AudioManager.Instance?.PlayInteract();
        }
    }

    private void OnIntimidate(InputAction.CallbackContext ctx)
    {
        // Nur im Shop aktiv — ShopUI verarbeitet dies
        ShopUI.Instance?.OnIntimidatePressed();
    }
}

public interface IInteractable
{
    void Interact(JenniferInteraction jennifer);
}
```

- [ ] **Step 5: In Unity Editor einrichten**
  - JenniferMovement + JenniferInteraction auf Jennifer Capsule ziehen
  - `Camera Transform`: **Main Camera** Transform zuweisen (NICHT das virtuelle Cinemachine Camera Object — der Main Camera hat den CinemachineBrain Component)
  - Interactable Layer für Türen, Pflanzen, Waschmaschine etc. anlegen

- [ ] **Step 6: Play-Test im Editor**
  - Play drücken → WASD bewegt Jennifer ✓
  - Kamera folgt Jennifer ✓

- [ ] **Step 6: Commit**
```bash
git add Assets/Scripts/Jennifer/JenniferMovement.cs
git commit -m "feat: add Jennifer third-person movement with Cinemachine camera"
```

---

### Task 6: Scene Fade-to-Black Transition

- [ ] **Step 1: SceneTransitionManager implementieren**

`Assets/Scripts/Core/SceneTransitionManager.cs`:
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // Canvas + fadeImage müssen ebenfalls DontDestroyOnLoad sein!
        // Stelle sicher, dass das Canvas-GameObject ein Child dieses GameObjects ist
        // oder DontDestroyOnLoad separat aufruft.
        if (fadeImage != null)
            DontDestroyOnLoad(fadeImage.transform.root.gameObject);
    }

    public void LoadScene(string sceneName) =>
        StartCoroutine(FadeAndLoad(sceneName));

    private IEnumerator FadeAndLoad(string sceneName)
    {
        yield return Fade(0f, 1f);
        // Async load damit die Coroutine und das fadeImage überleben
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (op != null && !op.isDone) yield return null;
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = to;
        fadeImage.color = c;
    }
}
```

- [ ] **Step 2: DoorTrigger implementieren**

`Assets/Scripts/Core/DoorTrigger.cs`:
```csharp
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private string requiredTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(requiredTag)) return;
        SceneTransitionManager.Instance?.LoadScene(targetScene);
    }
}
```

- [ ] **Step 3: In Unity einrichten**
  - Canvas (Screen Space - Overlay) mit schwarzem Image als Child zu `SceneTransitionManager` GameObject
  - Image Alpha = 0 zu Beginn
  - DoorTrigger auf Türen platzieren (Box Collider, Is Trigger ✓)

- [ ] **Step 4: Commit**
```bash
git add Assets/Scripts/Core/SceneTransitionManager.cs Assets/Scripts/Core/DoorTrigger.cs
git commit -m "feat: add fade-to-black scene transitions with door triggers"
```

---

## Chunk 4: Wut-Meter System

**Files:**
- Create: `Assets/Scripts/Jennifer/WutMeter.cs`
- Create: `Assets/Scripts/Jennifer/WutState.cs`
- Create: `Assets/Tests/EditMode/WutMeterTests.cs`
- Create: `Assets/Scripts/UI/HUDController.cs`

### Task 7: WutMeter Logik

- [ ] **Step 1: WutState Enum**

`Assets/Scripts/Jennifer/WutState.cs`:
```csharp
public enum WutState
{
    Entspannt,  // 0-30%
    Gereizt,    // 31-60%
    Wuetend,    // 61-85%
    Ausraster   // 86-100%
}
```

- [ ] **Step 2: Failing Tests schreiben**

`Assets/Tests/EditMode/WutMeterTests.cs`:
```csharp
using NUnit.Framework;

public class WutMeterTests
{
    [Test]
    public void WutState_At0_IsEntspannt() =>
        Assert.AreEqual(WutState.Entspannt, WutMeterLogic.GetState(0f));

    [Test]
    public void WutState_At30_IsEntspannt() =>
        Assert.AreEqual(WutState.Entspannt, WutMeterLogic.GetState(30f));

    [Test]
    public void WutState_At31_IsGereizt() =>
        Assert.AreEqual(WutState.Gereizt, WutMeterLogic.GetState(31f));

    [Test]
    public void WutState_At61_IsWuetend() =>
        Assert.AreEqual(WutState.Wuetend, WutMeterLogic.GetState(61f));

    [Test]
    public void WutState_At86_IsAusraster() =>
        Assert.AreEqual(WutState.Ausraster, WutMeterLogic.GetState(86f));

    [Test]
    public void Wut_NeverExceeds100()
    {
        float wut = 95f;
        wut = WutMeterLogic.Add(wut, 20f);
        Assert.AreEqual(100f, wut);
    }

    [Test]
    public void Wut_NeverBelow0()
    {
        float wut = 5f;
        wut = WutMeterLogic.Add(wut, -20f);
        Assert.AreEqual(0f, wut);
    }

    [Test]
    public void AfterAusraster_WutDropsTo70()
    {
        float wut = 100f;
        wut = WutMeterLogic.ApplyAusrasterReset(wut);
        Assert.AreEqual(70f, wut);
    }

    [Test]
    public void IntimidationBonus_Is20Percent()
    {
        float basePrice = 100f;
        float result = WutMeterLogic.ApplyIntimidationBonus(basePrice);
        Assert.AreEqual(120f, result, 0.01f);
    }
}
```

- [ ] **Step 3: Test fehlschlagen lassen** → FAIL (WutMeterLogic existiert nicht)

- [ ] **Step 4: WutMeterLogic implementieren (pure C#, testbar)**

`Assets/Scripts/Jennifer/WutMeterLogic.cs`:
```csharp
public static class WutMeterLogic
{
    public static WutState GetState(float wut)
    {
        if (wut <= 30f) return WutState.Entspannt;
        if (wut <= 60f) return WutState.Gereizt;
        if (wut <= 85f) return WutState.Wuetend;
        return WutState.Ausraster;
    }

    public static float Add(float current, float delta) =>
        UnityEngine.Mathf.Clamp(current + delta, 0f, 100f);

    public static float ApplyAusrasterReset(float current) => 70f;

    public static float ApplyIntimidationBonus(float basePrice) =>
        basePrice * 1.2f;
}
```

- [ ] **Step 5: Tests laufen lassen → PASS**

- [ ] **Step 6: WutMeter MonoBehaviour implementieren**

`Assets/Scripts/Jennifer/WutMeter.cs`:
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WutMeter : MonoBehaviour
{
    public static WutMeter Instance { get; private set; }

    [SerializeField] private float homeDecayRate = 2f;       // % per 10s in Wohnung
    [SerializeField] private float policeRageGainRate = 5f;  // % per 5s in Sichtweite
    [SerializeField] private float ausrasterCooldown = 30f;

    public float WutLevel { get; private set; }
    public WutState CurrentState => WutMeterLogic.GetState(WutLevel);
    public bool CanIntimidate => CurrentState == WutState.Wuetend; // Nur Wuetend (61-85%), nicht Ausraster!

    public UnityEvent<float> OnWutChanged;
    public UnityEvent OnAusraster;

    private bool _isInHome;
    private bool _policeInSight;
    private bool _ausrasterOnCooldown;
    private float _policeTimer;
    private float _homeTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        HandlePoliceRage();
        HandleHomeDecay();
        CheckAusraster();
    }

    private void HandlePoliceRage()
    {
        if (!_policeInSight) return;
        _policeTimer += Time.deltaTime;
        if (_policeTimer >= 5f)
        {
            _policeTimer = 0f;
            AddWut(policeRageGainRate);
        }
    }

    private void HandleHomeDecay()
    {
        if (!_isInHome) return;
        _homeTimer += Time.deltaTime;
        if (_homeTimer >= 10f)
        {
            _homeTimer = 0f;
            AddWut(-homeDecayRate);
        }
    }

    private void CheckAusraster()
    {
        if (WutLevel >= 100f && !_ausrasterOnCooldown)
            StartCoroutine(TriggerAusraster());
    }

    private IEnumerator TriggerAusraster()
    {
        _ausrasterOnCooldown = true;
        OnAusraster?.Invoke(); // InventoryManager abonniert dies in seiner Awake()
        WutLevel = WutMeterLogic.ApplyAusrasterReset(WutLevel);
        OnWutChanged?.Invoke(WutLevel);

        // Direkt 1 zufälliges Item zerstören (zusätzlich zum Event, als Fallback)
        InventoryManager.Instance?.DestroyRandomItemOnAusraster();
        PoliceAttentionSystem.Instance?.AddAttention(15f);

        yield return new WaitForSeconds(ausrasterCooldown);
        _ausrasterOnCooldown = false;
    }

    public void AddWut(float delta)
    {
        WutLevel = WutMeterLogic.Add(WutLevel, delta);
        OnWutChanged?.Invoke(WutLevel);
    }

    public void SetInHome(bool inHome) => _isInHome = inHome;
    public void SetPoliceInSight(bool inSight) { _policeInSight = inSight; _policeTimer = 0f; }
}
```

- [ ] **Step 7: Commit**
```bash
git add Assets/Scripts/Jennifer/ Assets/Tests/EditMode/WutMeterTests.cs
git commit -m "feat: add WutMeter system with state machine, ausraster cooldown, police/home triggers"
```

---

## Chunk 5: Polizei-System

**Files:**
- Create: `Assets/Scripts/NPC/PoliceOfficer.cs`
- Create: `Assets/Scripts/NPC/PoliceAttentionSystem.cs`
- Create: `Assets/Tests/EditMode/PoliceSystemTests.cs`

### Task 8: PoliceAttentionSystem

- [ ] **Step 1: Failing Tests**

`Assets/Tests/EditMode/PoliceSystemTests.cs`:
```csharp
using NUnit.Framework;

public class PoliceSystemTests
{
    [Test]
    public void Attention_ClampedAt100()
    {
        float att = PoliceAttentionLogic.Add(90f, 20f);
        Assert.AreEqual(100f, att);
    }

    [Test]
    public void Attention_NeverBelow0()
    {
        float att = PoliceAttentionLogic.Add(5f, -20f);
        Assert.AreEqual(0f, att);
    }

    [Test]
    public void At100_TriggersMVPWarning()
    {
        bool triggered = PoliceAttentionLogic.ShouldTriggerMVPWarning(100f);
        Assert.IsTrue(triggered);
    }

    [Test]
    public void At99_NoWarning()
    {
        bool triggered = PoliceAttentionLogic.ShouldTriggerMVPWarning(99f);
        Assert.IsFalse(triggered);
    }

    [Test]
    public void DailyDecay_ReducesByFive()
    {
        float att = PoliceAttentionLogic.ApplyDailyDecay(50f);
        Assert.AreEqual(45f, att);
    }
}
```

- [ ] **Step 2: PoliceAttentionLogic (pure C#)**

`Assets/Scripts/NPC/PoliceAttentionLogic.cs`:
```csharp
public static class PoliceAttentionLogic
{
    public static float Add(float current, float delta) =>
        UnityEngine.Mathf.Clamp(current + delta, 0f, 100f);

    public static bool ShouldTriggerMVPWarning(float attention) => attention >= 100f;

    public static float ApplyDailyDecay(float current) => Add(current, -5f);

    public static float AfterMVPWarning(float current) => 50f; // reset bei MVP
}
```

- [ ] **Step 3: Tests → PASS**

- [ ] **Step 4: PoliceAttentionSystem MonoBehaviour**

`Assets/Scripts/NPC/PoliceAttentionSystem.cs`:
```csharp
using UnityEngine;
using UnityEngine.Events;

public class PoliceAttentionSystem : MonoBehaviour
{
    public static PoliceAttentionSystem Instance { get; private set; }

    public float Attention { get; private set; }
    public UnityEvent<float> OnAttentionChanged;
    public UnityEvent OnMVPWarning; // Blink + Wut +20% bei 100% im MVP

    private bool _isInHome;
    private bool _warningOnCooldown; // Verhindert Re-Trigger im selben Frame

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnNewDay.AddListener(OnNewDay);
    }

    public void AddAttention(float delta)
    {
        if (_isInHome) return;
        Attention = PoliceAttentionLogic.Add(Attention, delta);
        OnAttentionChanged?.Invoke(Attention);

        if (PoliceAttentionLogic.ShouldTriggerMVPWarning(Attention) && !_warningOnCooldown)
            TriggerMVPWarning();
    }

    // Separate Methode für kontinuierlichen Patrol-Input (kleiner delta/frame)
    public void AddAttentionFromPatrol(float delta) => AddAttention(delta);

    private void TriggerMVPWarning()
    {
        _warningOnCooldown = true;
        WutMeter.Instance?.AddWut(20f);
        Attention = PoliceAttentionLogic.AfterMVPWarning(Attention);
        OnAttentionChanged?.Invoke(Attention);
        OnMVPWarning?.Invoke();
        Debug.Log("[Police] Zu heiß! Polizei-Aufmerksamkeit zurückgesetzt.");
        // Cooldown nach 1 Spieltag aufheben
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnNewDay.AddListener(_ => _warningOnCooldown = false);
    }

    private void OnNewDay(int day)
    {
        Attention = PoliceAttentionLogic.ApplyDailyDecay(Attention);
        OnAttentionChanged?.Invoke(Attention);
    }

    public void SetInHome(bool inHome) => _isInHome = inHome;
}
```

- [ ] **Step 5: PoliceOfficerTracker (löst Multi-Offizier-Konflikt)**

`Assets/Scripts/NPC/PoliceOfficerTracker.cs`:
```csharp
using System.Collections.Generic;
using UnityEngine;

/// Zentrale Stelle für alle Polizisten — verhindert Boolean-Race-Condition bei 2+ Offizieren
public class PoliceOfficerTracker : MonoBehaviour
{
    public static PoliceOfficerTracker Instance { get; private set; }

    private readonly HashSet<int> _officersInSight = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ReportSight(int officerId, bool inSight)
    {
        bool wasSeen = _officersInSight.Count > 0;
        if (inSight) _officersInSight.Add(officerId);
        else _officersInSight.Remove(officerId);
        bool isSeen = _officersInSight.Count > 0;
        if (wasSeen != isSeen) WutMeter.Instance?.SetPoliceInSight(isSeen);
    }
}
```

- [ ] **Step 6: PoliceOfficer Waypoint-Patrol**

`Assets/Scripts/NPC/PoliceOfficer.cs`:
```csharp
using UnityEngine;

public class PoliceOfficer : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waypointTolerance = 0.5f;
    [SerializeField] private float sightRange = 8f;
    [SerializeField] private LayerMask playerLayer;

    private int _currentWaypoint;
    private Transform _player;

    private void Start()
    {
        var playerGo = GameObject.FindWithTag("Player");
        if (playerGo != null) _player = playerGo.transform;
    }

    private void Update()
    {
        Patrol();
        CheckPlayerSight();
        UpdatePatrolAttention();
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[_currentWaypoint];
        transform.position = Vector3.MoveTowards(
            transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < waypointTolerance)
            _currentWaypoint = (_currentWaypoint + 1) % waypoints.Length;
    }

    private void CheckPlayerSight()
    {
        if (_player == null) return;
        float dist = Vector3.Distance(transform.position, _player.position);
        // MVP: Distanz-basiert (kein Raycast durch Wände — vereinfachung für MVP)
        // Post-MVP: Physics.Raycast für echte Line-of-Sight ergänzen
        bool inSight = dist <= sightRange;

        // Verwende officerId statt direktem boolean um Multi-Offizier-Konflikt zu vermeiden
        PoliceOfficerTracker.Instance?.ReportSight(_officerId, inSight);
    }

    private int _officerId;
    private static int _nextId = 0;
    private void Awake() => _officerId = _nextId++;

    // Zusätzlich: Polizei-Aufmerksamkeit beim Patrouillieren erhöhen
    private void UpdatePatrolAttention()
    {
        if (_player == null) return;
        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist <= sightRange)
            PoliceAttentionSystem.Instance?.AddAttentionFromPatrol(0.1f * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
```

- [ ] **Step 6: In Unity einrichten**
  - 2 leere GameObjects als Waypoints in der Stadt platzieren
  - PoliceOfficer auf Capsule (blau/dunkel) hinzufügen
  - Waypoints zuweisen

- [ ] **Step 7: Commit**
```bash
git add Assets/Scripts/NPC/ Assets/Tests/EditMode/PoliceSystemTests.cs
git commit -m "feat: add police waypoint patrol, attention system, MVP warning at 100%"
```

---

## Chunk 6: Anbau-System (Gewächshaus)

**Files:**
- Create: `Assets/ScriptableObjects/Data/PlantData.cs`
- Create: `Assets/Scripts/Systems/Plant.cs`
- Create: `Assets/Scripts/Systems/GreenhouseManager.cs`
- Create: `Assets/Tests/EditMode/PlantSystemTests.cs`

### Task 9: Plant ScriptableObject & Logik

- [ ] **Step 1: PlantData ScriptableObject**

`Assets/ScriptableObjects/Data/PlantData.cs`:
```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "PlantData", menuName = "Jennifer/Plant Data")]
public class PlantData : ScriptableObject
{
    public string plantName;
    public float cost;
    public int growthDays;
    public int yieldUnits;
    public string maxQuality; // "low", "medium", "high"
    public int dryingDays;
}
```

In Unity: Rechtsklick → Create → Jennifer → Plant Data
Drei Assets erstellen:
- `PlantCheap`: cost=50, growthDays=3, yieldUnits=2, maxQuality="low", dryingDays=1
- `PlantNormal`: cost=150, growthDays=5, yieldUnits=4, maxQuality="medium", dryingDays=2
- `PlantPremium`: cost=400, growthDays=7, yieldUnits=6, maxQuality="high", dryingDays=3

- [ ] **Step 2: Failing Tests**

`Assets/Tests/EditMode/PlantSystemTests.cs`:
```csharp
using NUnit.Framework;

public class PlantSystemTests
{
    [Test]
    public void Plant_ReadyToHarvest_After3DaysGrowth()
    {
        var plant = new PlantLogic("cheap", plantedDay: 1, growthDays: 3);
        Assert.IsTrue(plant.IsReadyToHarvest(currentDay: 4));
    }

    [Test]
    public void Plant_NotReady_Before3Days()
    {
        var plant = new PlantLogic("cheap", plantedDay: 1, growthDays: 3);
        Assert.IsFalse(plant.IsReadyToHarvest(currentDay: 3));
    }

    [Test]
    public void Plant_Dies_After2MissedWaterDays()
    {
        var plant = new PlantLogic("cheap", plantedDay: 1, growthDays: 3);
        plant.MissWatering();
        plant.MissWatering();
        Assert.IsFalse(plant.IsAlive);
    }

    [Test]
    public void Plant_LosesGrowthDay_After1MissedWater()
    {
        var plant = new PlantLogic("cheap", plantedDay: 1, growthDays: 3);
        plant.MissWatering();
        // Growth effectively needs 4 days instead of 3
        Assert.AreEqual(1, plant.MissedWaterDays);
        Assert.IsTrue(plant.IsAlive);
    }

    [Test]
    public void Watering_ResetsMissedDays()
    {
        var plant = new PlantLogic("cheap", plantedDay: 1, growthDays: 3);
        plant.MissWatering();
        plant.Water();
        Assert.AreEqual(0, plant.MissedWaterDays);
    }
}
```

- [ ] **Step 3: PlantLogic (pure C#)**

`Assets/Scripts/Systems/PlantLogic.cs`:
```csharp
public class PlantLogic
{
    public string PlantType { get; }
    public int PlantedDay { get; }
    public int GrowthDays { get; }
    public int MissedWaterDays { get; private set; }
    public bool IsAlive { get; private set; } = true;
    public bool WateredToday { get; private set; }

    public PlantLogic(string plantType, int plantedDay, int growthDays)
    {
        PlantType = plantType;
        PlantedDay = plantedDay;
        GrowthDays = growthDays;
    }

    public bool IsReadyToHarvest(int currentDay)
    {
        if (!IsAlive) return false;
        int effectiveDays = GrowthDays + MissedWaterDays;
        return currentDay >= PlantedDay + effectiveDays;
    }

    public void Water()
    {
        WateredToday = true;
        MissedWaterDays = 0;
    }

    public void MissWatering()
    {
        MissedWaterDays++;
        if (MissedWaterDays >= 2) IsAlive = false;
    }

    public void OnNewDay() => WateredToday = false;
}
```

- [ ] **Step 4: Tests → PASS**

- [ ] **Step 5: GreenhouseManager MonoBehaviour**

`Assets/Scripts/Systems/GreenhouseManager.cs`:
```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GreenhouseManager : MonoBehaviour
{
    public static GreenhouseManager Instance { get; private set; }

    [SerializeField] private PlantData cheapPlantData;
    [SerializeField] private int maxPlants = 1; // Stufe 1

    public UnityEvent OnPlantDied;
    public UnityEvent OnPlantHarvested;

    private readonly List<PlantLogic> _plants = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnNewDay.AddListener(OnNewDay);
    }

    public bool PlantSeed(PlantData data)
    {
        if (_plants.Count >= maxPlants) return false;
        if (!GameManager.Instance.SpendMoney(data.cost)) return false;

        _plants.Add(new PlantLogic(data.plantName,
                                   TimeManager.Instance.CurrentDay,
                                   data.growthDays));
        return true;
    }

    public bool WaterPlant(int index)
    {
        if (index < 0 || index >= _plants.Count) return false;
        if (!_plants[index].IsAlive) return false;
        _plants[index].Water();
        return true;
    }

    public PlantLogic HarvestPlant(int index)
    {
        if (index < 0 || index >= _plants.Count) return null;
        var plant = _plants[index];
        if (!plant.IsReadyToHarvest(TimeManager.Instance.CurrentDay)) return null;
        _plants.RemoveAt(index);
        OnPlantHarvested?.Invoke();
        return plant;
    }

    private void OnNewDay(int day)
    {
        for (int i = _plants.Count - 1; i >= 0; i--)
        {
            if (!_plants[i].WateredToday)
                _plants[i].MissWatering();

            if (!_plants[i].IsAlive)
            {
                _plants.RemoveAt(i);
                WutMeter.Instance?.AddWut(20f);
                OnPlantDied?.Invoke();
            }
            else
            {
                _plants[i].OnNewDay();
            }
        }
    }

    public IReadOnlyList<PlantLogic> GetPlants() => _plants.AsReadOnly();
}
```

- [ ] **Step 6: Commit**
```bash
git add Assets/Scripts/Systems/PlantLogic.cs Assets/Scripts/Systems/GreenhouseManager.cs
git add Assets/ScriptableObjects/ Assets/Tests/EditMode/PlantSystemTests.cs
git commit -m "feat: add plant system with growth, watering, harvest and death mechanics"
```

---

## Chunk 7: Trocknen, Verpacken & Inventar

**Files:**
- Create: `Assets/Scripts/Systems/DryingRack.cs`
- Create: `Assets/Scripts/Systems/PackagingStation.cs`
- Create: `Assets/Scripts/Systems/InventoryManager.cs`
- Create: `Assets/Scripts/Systems/EconomyLogic.cs`
- Create: `Assets/Tests/EditMode/EconomyTests.cs`

### Task 10: Economy Logik & Verpackungspreise

- [ ] **Step 1: Failing Tests**

`Assets/Tests/EditMode/EconomyTests.cs`:
```csharp
using NUnit.Framework;

public class EconomyTests
{
    [Test]
    public void SmallPackage_LowQuality_PriceRange_80to120_With07Multiplier()
    {
        float baseMin = 80f, baseMax = 120f;
        float mult = EconomyLogic.GetQualityMultiplier("low");
        Assert.AreEqual(0.7f, mult, 0.001f);
        Assert.AreEqual(56f, baseMin * mult, 0.1f);
    }

    [Test]
    public void LargePackage_PremiumQuality_ExceedsCustomerBudget()
    {
        float price = EconomyLogic.CalculatePackagePrice("large", "premium");
        Assert.Greater(price, 700f);
    }

    [Test]
    public void MediumPackage_MediumQuality_WithinBudget()
    {
        float price = EconomyLogic.CalculatePackagePrice("medium", "medium");
        Assert.LessOrEqual(price, 400f);
    }

    [Test]
    public void UnitsRequired_SmallPackage_Is1()
    {
        Assert.AreEqual(1, EconomyLogic.GetRequiredUnits("small"));
    }

    [Test]
    public void UnitsRequired_LargePackage_Is6()
    {
        Assert.AreEqual(6, EconomyLogic.GetRequiredUnits("large"));
    }
}
```

- [ ] **Step 2: EconomyLogic (pure C#)**

`Assets/Scripts/Systems/EconomyLogic.cs`:
```csharp
using UnityEngine;

public static class EconomyLogic
{
    public static float GetQualityMultiplier(string quality) => quality switch
    {
        "low"     => 0.7f,
        "medium"  => 1.0f,
        "high"    => 1.3f,
        "premium" => 1.6f,
        _ => 1.0f
    };

    public static int GetRequiredUnits(string size) => size switch
    {
        "small"  => 1,
        "medium" => 3,
        "large"  => 6,
        _ => 1
    };

    public static float CalculatePackagePrice(string size, string quality)
    {
        float basePrice = size switch
        {
            "small"  => Random.Range(80f,  120f),
            "medium" => Random.Range(220f, 350f),
            "large"  => Random.Range(400f, 650f),
            _ => 80f
        };
        return basePrice * GetQualityMultiplier(quality);
    }

    public static float GetCustomerBudget(string packageSize) => packageSize switch
    {
        "small"  => 150f,
        "medium" => 400f,
        "large"  => 700f,
        _ => 150f
    };
}
```

- [ ] **Step 3: Tests → PASS**

- [ ] **Step 4: InventoryManager**

`Assets/Scripts/Systems/InventoryManager.cs`:
```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventoryItem
{
    public string itemType; // "dried", "package_small", "package_medium", "package_large"
    public string quality;
    public int quantity;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public UnityEvent OnInventoryChanged;

    private readonly List<InventoryItem> _items = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Ausraster zerstört 1 zufälliges Item
        WutMeter.Instance?.OnAusraster.AddListener(DestroyRandomItem);
    }

    public void AddItem(string type, string quality, int qty = 1)
    {
        var existing = _items.Find(i => i.itemType == type && i.quality == quality);
        if (existing != null) existing.quantity += qty;
        else _items.Add(new InventoryItem { itemType = type, quality = quality, quantity = qty });
        OnInventoryChanged?.Invoke();
    }

    public bool RemoveItem(string type, string quality, int qty = 1)
    {
        var item = _items.Find(i => i.itemType == type && i.quality == quality && i.quantity >= qty);
        if (item == null) return false;
        item.quantity -= qty;
        if (item.quantity <= 0) _items.Remove(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int CountItem(string type, string quality) =>
        _items.Find(i => i.itemType == type && i.quality == quality)?.quantity ?? 0;

    public IReadOnlyList<InventoryItem> GetItems() => _items.AsReadOnly();

    // Wird direkt von WutMeter.TriggerAusraster() aufgerufen
    public void DestroyRandomItemOnAusraster()
    {
        if (_items.Count == 0) return;
        int idx = Random.Range(0, _items.Count);
        _items[idx].quantity--;
        if (_items[idx].quantity <= 0) _items.RemoveAt(idx);
        OnInventoryChanged?.Invoke();
        Debug.Log("[Inventory] Ausraster! Ein Item zerstört.");
    }
}
```

- [ ] **Step 5: DryingRack**

`Assets/Scripts/Systems/DryingRack.cs`:
```csharp
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DryingItem
{
    public string quality;
    public int startDay;
    public int durationDays;
    public int units;
    public bool perfectDrying; // für Premium-Qualität
}

public class DryingRack : MonoBehaviour
{
    public static DryingRack Instance { get; private set; }

    private readonly List<DryingItem> _drying = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnNewDay.AddListener(CheckDryingComplete);
    }

    public bool StartDrying(string quality, int units, int durationDays)
    {
        _drying.Add(new DryingItem
        {
            quality = quality,
            startDay = TimeManager.Instance.CurrentDay,
            durationDays = durationDays,
            units = units,
            perfectDrying = true
        });
        return true;
    }

    private void CheckDryingComplete(int day)
    {
        for (int i = _drying.Count - 1; i >= 0; i--)
        {
            var item = _drying[i];
            int daysElapsed = day - item.startDay;
            if (daysElapsed >= item.durationDays)
            {
                string finalQuality = item.perfectDrying && item.quality == "high"
                    ? "premium" : item.quality;
                InventoryManager.Instance?.AddItem("dried", finalQuality, item.units);
                _drying.RemoveAt(i);
                Debug.Log($"[Drying] Fertig getrocknet: {item.units}x {finalQuality}");
            }
        }
    }

    public IReadOnlyList<DryingItem> GetDryingItems() => _drying.AsReadOnly();
}
```

- [ ] **Step 6: PackagingStation**

`Assets/Scripts/Systems/PackagingStation.cs`:
```csharp
using UnityEngine;

public class PackagingStation : MonoBehaviour
{
    public static PackagingStation Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool Pack(string size, string quality)
    {
        int required = EconomyLogic.GetRequiredUnits(size);
        if (!InventoryManager.Instance.RemoveItem("dried", quality, required))
        {
            Debug.Log($"[Packaging] Nicht genug getrocknete Ware für {size}!");
            WutMeter.Instance?.AddWut(5f);
            return false;
        }
        string packageType = $"package_{size}";
        InventoryManager.Instance.AddItem(packageType, quality);
        return true;
    }
}
```

- [ ] **Step 7: Commit**
```bash
git add Assets/Scripts/Systems/ Assets/Tests/EditMode/EconomyTests.cs
git commit -m "feat: add drying rack, packaging station, inventory manager, economy logic"
```

---

## Chunk 8: Shop-System & Kunden-KI

**Files:**
- Create: `Assets/Scripts/NPC/CustomerNPC.cs`
- Create: `Assets/Scripts/NPC/CustomerScheduler.cs`
- Create: `Assets/Scripts/Systems/ShopManager.cs`
- Create: `Assets/Scripts/UI/ShopUI.cs`

### Task 11: Kunden-KI State Machine

- [ ] **Step 1: CustomerNPC**

`Assets/Scripts/NPC/CustomerNPC.cs`:
```csharp
using System.Collections;
using UnityEngine;

public enum CustomerState { Waiting, InShop, Buying, Leaving }

public class CustomerNPC : MonoBehaviour
{
    [SerializeField] private float waitTimeout = 60f;
    [SerializeField] private string preferredSize = "small"; // small/medium/large
    [SerializeField] private bool isLoyalCustomer;

    public string CustomerId { get; set; }
    public int PurchaseCount { get; private set; }
    public CustomerState State { get; private set; } = CustomerState.Waiting;

    private float _waitTimer;

    private void Update()
    {
        if (State != CustomerState.Waiting) return;
        _waitTimer += Time.deltaTime;
        if (_waitTimer >= waitTimeout) Leave();
    }

    public void OnJenniferEntersShop()
    {
        if (State != CustomerState.Waiting) return;
        State = CustomerState.InShop;
        ShopManager.Instance?.StartCustomerInteraction(this);
    }

    public bool TryBuy(string packageType, float offeredPrice)
    {
        float budget = EconomyLogic.GetCustomerBudget(preferredSize);
        if (!isLoyalCustomer && offeredPrice > budget)
        {
            // Haggling: 1 Versuch
            WutMeter.Instance?.AddWut(10f);
            return false;
        }

        GameManager.Instance?.AddMoney(offeredPrice);
        InventoryManager.Instance?.RemoveItem(packageType,
            InventoryManager.Instance.GetItems()[0]?.quality ?? "medium");
        WutMeter.Instance?.AddWut(-10f);
        PurchaseCount++;
        if (PurchaseCount >= 5) isLoyalCustomer = true;
        State = CustomerState.Leaving;
        StartCoroutine(DespawnAfterDelay(1f));
        return true;
    }

    public bool TryIntimidate(float basePrice)
    {
        if (!WutMeter.Instance.CanIntimidate) return false;
        float chance = Random.value;
        if (chance <= 0.7f)
        {
            float bonus = WutMeterLogic.ApplyIntimidationBonus(basePrice);
            TryBuy($"package_{preferredSize}", bonus);
            return true;
        }
        // 30% fliehen
        WutMeter.Instance?.AddWut(5f);
        Leave();
        return false;
    }

    private void Leave()
    {
        State = CustomerState.Leaving;
        StartCoroutine(DespawnAfterDelay(0.5f));
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
```

- [ ] **Step 2: CustomerScheduler**

`Assets/Scripts/NPC/CustomerScheduler.cs`:
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerScheduler : MonoBehaviour
{
    public static CustomerScheduler Instance { get; private set; }

    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform shopDoorSpawnPoint;
    [SerializeField] private int minDayCustomers = 3;
    [SerializeField] private int maxDayCustomers = 5;

    private readonly List<CustomerNPC> _activeCustomers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnNewDay.AddListener(ScheduleDayCustomers);
            TimeManager.Instance.OnHourChanged.AddListener(OnHourChanged);
        }
    }

    private void ScheduleDayCustomers(int day) =>
        StartCoroutine(SpawnCustomersForDay());

    private IEnumerator SpawnCustomersForDay()
    {
        int count = Random.Range(minDayCustomers, maxDayCustomers + 1);
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(Random.Range(60f, 300f));
            SpawnCustomer(isEvening: false);
        }
    }

    private void OnHourChanged(float hour)
    {
        if (hour >= 20f && hour < 21f)
            StartCoroutine(SpawnEveningCustomers());
    }

    private IEnumerator SpawnEveningCustomers()
    {
        int count = Random.Range(1, 3);
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(Random.Range(30f, 120f));
            SpawnCustomer(isEvening: true);
        }
    }

    private void SpawnCustomer(bool isEvening)
    {
        if (customerPrefab == null || shopDoorSpawnPoint == null) return;
        var go = Instantiate(customerPrefab, shopDoorSpawnPoint.position, Quaternion.identity);
        var npc = go.GetComponent<CustomerNPC>();
        npc.CustomerId = $"customer_{System.Guid.NewGuid():N}";
        _activeCustomers.Add(npc);
    }

    public void NotifyJenniferInShop()
    {
        foreach (var c in _activeCustomers)
            if (c != null && c.State == CustomerState.Waiting)
                c.OnJenniferEntersShop();
    }
}
```

- [ ] **Step 3: ShopManager**

`Assets/Scripts/Systems/ShopManager.cs`:
```csharp
using UnityEngine;
using UnityEngine.Events;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    public UnityEvent<CustomerNPC> OnCustomerInteractionStart;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartCustomerInteraction(CustomerNPC customer)
    {
        OnCustomerInteractionStart?.Invoke(customer);
        ShopUI.Instance?.ShowShopUI(customer);
    }
}
```

- [ ] **Step 4: Commit**
```bash
git add Assets/Scripts/NPC/ Assets/Scripts/Systems/ShopManager.cs
git commit -m "feat: add customer AI state machine, scheduler, shop manager"
```

---

## Chunk 9: UI System

**Files:**
- Create: `Assets/Scripts/UI/HUDController.cs`
- Create: `Assets/Scripts/UI/MainMenuUI.cs`
- Create: `Assets/Scripts/UI/PauseMenuUI.cs`
- Create: `Assets/Scripts/UI/ShopUI.cs`

### Task 12: HUD

- [ ] **Step 1: HUDController**

`Assets/Scripts/UI/HUDController.cs`:
```csharp
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI packageCountText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Slider wutSlider;
    [SerializeField] private Image wutFill;
    [SerializeField] private Slider policeSlider;

    [Header("Wut Colors")]
    [SerializeField] private Color entspanntColor = Color.green;
    [SerializeField] private Color gereizterColor = Color.yellow;
    [SerializeField] private Color wuetendColor = new Color(1f, 0.5f, 0f); // orange
    [SerializeField] private Color ausrasterColor = Color.red;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        WutMeter.Instance?.OnWutChanged.AddListener(UpdateWutUI);
        PoliceAttentionSystem.Instance?.OnAttentionChanged.AddListener(UpdatePoliceUI);
        PoliceAttentionSystem.Instance?.OnMVPWarning.AddListener(FlashPoliceWarning);
    }

    private void Update()
    {
        if (GameManager.Instance != null)
            moneyText.text = $"$ {GameManager.Instance.Money:F0}";
        if (TimeManager.Instance != null)
            timeText.text = TimeManager.Instance.GetTimeString();

        int packages = 0;
        if (InventoryManager.Instance != null)
            foreach (var item in InventoryManager.Instance.GetItems())
                if (item.itemType.StartsWith("package")) packages += item.quantity;
        if (packageCountText != null) packageCountText.text = $"📦 {packages}";
    }

    private void UpdateWutUI(float wut)
    {
        if (wutSlider != null) wutSlider.value = wut / 100f;
        if (wutFill == null) return;
        wutFill.color = WutMeterLogic.GetState(wut) switch
        {
            WutState.Entspannt  => entspanntColor,
            WutState.Gereizt    => gereizterColor,
            WutState.Wuetend    => wuetendColor,
            WutState.Ausraster  => ausrasterColor,
            _ => entspanntColor
        };
    }

    private void UpdatePoliceUI(float attention)
    {
        if (policeSlider != null) policeSlider.value = attention / 100f;
    }

    private void FlashPoliceWarning()
    {
        // Kurzes rotes Flash - kann mit DOTween oder Coroutine implementiert werden
        if (policeSlider != null)
            StartCoroutine(FlashColor(policeSlider.GetComponentInChildren<Image>(), Color.red, 0.5f));
    }

    private System.Collections.IEnumerator FlashColor(Image img, Color flashColor, float duration)
    {
        if (img == null) yield break;
        Color original = img.color;
        img.color = flashColor;
        yield return new WaitForSeconds(duration);
        img.color = original;
    }
}
```

- [ ] **Step 2: HUD in Unity aufbauen**

Canvas (Screen Space - Overlay) erstellen mit:
- Oben links: TextMeshPro für Geld + Pakete
- Oben rechts: TextMeshPro für Zeit
- Unten links: Slider für Wut-Meter (grün→rot Farbverlauf)
- Unten rechts: Slider für Polizei-Aufmerksamkeit
- HUDController-Script auf Canvas GameObject

- [ ] **Step 3: MainMenuUI & PauseMenuUI**

`Assets/Scripts/UI/MainMenuUI.cs`:
```csharp
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        newGameButton.onClick.AddListener(NewGame);
        continueButton.onClick.AddListener(Continue);
        quitButton.onClick.AddListener(Quit);

        bool hasSave = System.IO.File.Exists(
            System.IO.Path.Combine(Application.persistentDataPath, "jennifer_save.json"));
        continueButton.interactable = hasSave;
    }

    private void NewGame()
    {
        SaveSystem.Save(new SaveData()); // Reset
        SceneTransitionManager.Instance?.LoadScene("City");
    }

    private void Continue() =>
        SceneTransitionManager.Instance?.LoadScene("City");

    private void Quit() => Application.Quit();
}
```

`Assets/Scripts/UI/PauseMenuUI.cs`:
```csharp
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance { get; private set; }

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveQuitButton;

    private bool _isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        pausePanel.SetActive(false);
    }

    private void Start()
    {
        resumeButton.onClick.AddListener(TogglePause);
        saveQuitButton.onClick.AddListener(SaveAndQuit);
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        pausePanel.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0f : 1f;
    }

    private void SaveAndQuit()
    {
        Time.timeScale = 1f;
        GameManager.Instance?.SaveGame();
        SceneTransitionManager.Instance?.LoadScene("MainMenu");
    }
}
```

- [ ] **Step 4: ShopUI (Button-basiert für MVP)**

`Assets/Scripts/UI/ShopUI.cs`:
```csharp
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance { get; private set; }

    [SerializeField] private GameObject shopPanel;
    [SerializeField] private TextMeshProUGUI customerInfoText;
    [SerializeField] private Button sellSmallButton;
    [SerializeField] private Button sellMediumButton;
    [SerializeField] private Button intimidateButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI blackMarketHintText;

    private CustomerNPC _currentCustomer;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        shopPanel.SetActive(false);
    }

    private void Start()
    {
        sellSmallButton.onClick.AddListener(() => SellPackage("small"));
        sellMediumButton.onClick.AddListener(() => SellPackage("medium"));
        intimidateButton.onClick.AddListener(Intimidate);
        closeButton.onClick.AddListener(CloseShop);
    }

    public void ShowShopUI(CustomerNPC customer)
    {
        _currentCustomer = customer;
        shopPanel.SetActive(true);

        int smallCount = InventoryManager.Instance?.CountItem("package_small", "medium") ?? 0;
        int mediumCount = InventoryManager.Instance?.CountItem("package_medium", "medium") ?? 0;

        customerInfoText.text = $"Kunde wartet...\nKlein: {smallCount}x | Mittel: {mediumCount}x";
        sellSmallButton.interactable = smallCount > 0;
        sellMediumButton.interactable = mediumCount > 0;
        intimidateButton.interactable = WutMeter.Instance?.CanIntimidate ?? false;

        bool hasPremium = (InventoryManager.Instance?.CountItem("package_large", "premium") ?? 0) > 0;
        if (blackMarketHintText != null)
            blackMarketHintText.gameObject.SetActive(hasPremium && GameManager.Instance?.ProgressionLevel < 3);
    }

    private void SellPackage(string size)
    {
        if (_currentCustomer == null) return;
        string quality = "medium"; // MVP: medium quality fixed
        float price = EconomyLogic.CalculatePackagePrice(size, quality);
        _currentCustomer.TryBuy($"package_{size}", price);
        CloseShop();
    }

    private void Intimidate() => OnIntimidatePressed();

    // Wird auch von JenniferInteraction (F-Taste) aufgerufen
    public void OnIntimidatePressed()
    {
        if (_currentCustomer == null || !shopPanel.activeSelf) return;
        float basePrice = EconomyLogic.CalculatePackagePrice("small", "medium");
        _currentCustomer.TryIntimidate(basePrice);
        CloseShop();
    }

    private void CloseShop() => shopPanel.SetActive(false);
}
```

- [ ] **Step 5: Commit**
```bash
git add Assets/Scripts/UI/
git commit -m "feat: add HUD, main menu, pause menu, shop UI (button-based MVP)"
```

---

## Chunk 10: Audio & GitHub Push

### Task 13: Audio Setup

- [ ] **Step 1: Audio Manager erstellen**

`Assets/Scripts/Core/AudioManager.cs`:
```csharp
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip mainMenuMusic;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip interactSFX;
    [SerializeField] private AudioClip purchaseSFX;
    [SerializeField] private AudioClip ausrasterSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        WutMeter.Instance?.OnAusraster.AddListener(PlayAusrasterSFX);
        PlayMusic(mainMenuMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayInteract() => sfxSource.PlayOneShot(interactSFX);
    public void PlayPurchase() => sfxSource.PlayOneShot(purchaseSFX);
    private void PlayAusrasterSFX() => sfxSource.PlayOneShot(ausrasterSFX);
}
```

- [ ] **Step 2: Lizenzfreie Audio-Assets herunterladen**

```bash
# freesound.org Assets (manuell herunterladen, kostenloser Account nötig)
# Empfehlung für lo-fi Musik: suche "lofi hip hop loop" auf freesound.org
# SFX: "cash register" (Kauf), "angry yell" (Ausraster), "click" (Interaktion)
# In Assets/Audio/ ablegen
```

- [ ] **Step 3: Commit**
```bash
git add Assets/Scripts/Core/AudioManager.cs
git commit -m "feat: add audio manager with music and SFX hooks"
```

---

### Task 14: Final Build & GitHub Push

- [ ] **Step 1: Build Settings konfigurieren**

  - File → Build Settings
  - Platform: PC, Mac & Linux Standalone
  - Target Platform: Linux (Pop!_OS nativ)
  - Alle Scenes hinzufügen: MainMenu (Index 0), City (Index 1), Greenhouse (Index 2), Apartment (Index 3)

- [ ] **Step 2: Test-Build erstellen**

  - Build → Wähle `~/JenniferGame/Builds/`
  - Prüfen ob Build startet ohne Fehler

- [ ] **Step 3: Alle Änderungen committen & pushen**

```bash
export PATH="$HOME/.local/bin:$PATH"
cd ~/JenniferGame
git add -A
git commit -m "feat: complete MVP - Jennifer's Cannabis Empire playable build"
git push origin main
```

- [ ] **Step 4: Verify**

```bash
gh repo view JenniferGame --web
```

GitHub zeigt alle Commits. Spielbarer MVP ist fertig.

---

## MVP Checkliste (Abnahme)

- [ ] Jennifer bewegt sich mit WASD durch die Stadt
- [ ] Cinemachine Kamera folgt Jennifer
- [ ] Tür betreten → Fade-to-Black → neue Szene
- [ ] 1 Pflanze pflanzen, bewässern, ernten
- [ ] Ernte trocknen → Paket schnüren
- [ ] Shop betreten → Kunde erscheint → Verkauf über UI
- [ ] Wut-Meter steigt/sinkt korrekt
- [ ] Einschüchtern funktioniert (F-Taste im Shop bei Wütend-Zustand)
- [ ] 2 Polizisten patrouillieren → Wut steigt bei Sichtkontakt
- [ ] Polizei-Aufmerksamkeit bei 100% → Warning-Flash + Wut +20%
- [ ] Spiel speichert (JSON) und lädt beim nächsten Start
- [ ] Hauptmenü + Pause-Menü + Quit funktionieren
- [ ] lo-fi Musik + 3 SFX hörbar
