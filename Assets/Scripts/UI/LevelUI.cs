using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public static event Action OnRoundStarted;
    public static LevelUI Instance;

    [SerializeField] private Button _startRoundButton;

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
}
