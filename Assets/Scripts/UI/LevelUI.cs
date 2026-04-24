using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public static event Action OnRoundStarted;
    public static LevelUI Instance;

    [Header("Buttons")]
    [SerializeField] private Button _startRoundButton;

    [Header("Text")]
    [SerializeField] private TMP_Text _roundNumber;

    public void StartRound()
    {
        OnRoundStarted?.Invoke();
    }

    public Button GetStartRoundButton()
    {
        return _startRoundButton;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Spawner.OnRoundChanged += UpdateRoundNumberText;
    }

    private void OnDisable()
    {
        Spawner.OnRoundChanged -= UpdateRoundNumberText;
    }

    private void UpdateRoundNumberText(int roundNum)
    {
        _roundNumber.text = $"Round: {roundNum + 1}";
    }
}
