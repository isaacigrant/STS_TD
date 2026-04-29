using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages all in-game HUD elements for a level: round counter, player health display,
/// and the Start Round button. Implements a simple singleton so other systems
/// (Spawner, GameManager) can show/hide the start button without a direct Inspector reference.
///
/// NOTE: The singleton pattern here is basic — Instance is never cleared in OnDestroy,
/// and there is no guard against multiple instances existing. Fine for a single-scene
/// project; revisit if you add scene transitions or multiple levels.
/// </summary>
public class LevelUI : MonoBehaviour
{
    /// <summary>
    /// Fired when the player presses the Start Round button.
    /// <see cref="Spawner"/> listens to this to begin the next round.
    /// </summary>
    public static event Action OnRoundStarted;
    public static LevelUI Instance { get; private set; }

    [Header("Buttons")]
    [SerializeField] private Button _startRoundButton;

    [Header("Text")]
    [SerializeField] private TMP_Text _fpsText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _roundText;

    private float _fpsPollingTime = 1f;
    private float _fpsTimer;
    private int _frameCounter;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[LevelUI] Duplicate instance detected — destroying this one.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        Spawner.OnRoundChanged += HandleRoundChanged;
        GameManager.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        Spawner.OnRoundChanged -= HandleRoundChanged;
        GameManager.OnHealthChanged -= HandleHealthChanged;
    }

    private void Start()
    {
        Application.targetFrameRate = 120;
    }

    private void Update()
    {
        FPSUpdate();
    }

    private void FPSUpdate()
    {
        _fpsTimer += Time.deltaTime;
        _frameCounter++;

        if (_fpsTimer >= _fpsPollingTime)
        {
            int frameRate = Mathf.RoundToInt(_frameCounter / _fpsTimer);
            _fpsText.text = $"{frameRate} FPS";

            _fpsTimer -= _fpsPollingTime;
            _frameCounter = 0;
        }
    }

    /// <summary>
    /// Updates the round counter text when <see cref="Spawner"/> advances to a new round.
    /// Adds 1 to convert from zero-based index to a player-facing round number.
    /// </summary>
    private void HandleRoundChanged(int roundNum)
    {
        _roundText.text = $"Round: {roundNum + 1}";
    }

    /// <summary>
    /// Updates the health display when <see cref="GameManager"/> reports a health change.
    /// </summary>
    private void HandleHealthChanged(int healthNum)
    {
        _healthText.text = $"Health: {healthNum}";
    }

    /// <summary>
    /// Returns the Start Round button so external systems (e.g. <see cref="Spawner"/>) can
    /// show or hide it at the appropriate time.
    ///
    /// NOTE: Exposing the Button directly gives callers full control of the component.
    /// A cleaner future refactor would be to replace this with dedicated
    /// ShowStartButton() / HideStartButton() methods, keeping the Button private.
    /// </summary>
    public Button GetStartRoundButton()
    {
        return _startRoundButton;
    }

    /// <summary>
    /// Bound to the Start Round button's OnClick event in the Inspector.
    /// Fires OnRoundStarted so <see cref="Spawner"/> can begin the next round.
    /// </summary>
    public void StartRound()
    {
        OnRoundStarted?.Invoke();
    }
}
