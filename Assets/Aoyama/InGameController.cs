using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DG.Tweening;
using UniRx;

public class InGameController : MonoBehaviour
{
    [SerializeField]
    private Scene_system _sceneSystem;

    [SerializeField]
    private CinemachineVirtualCamera _cinemachine;

    [Header("�e�L�X�g�֌W")]

    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _timeText;

    [SerializeField]
    private Text _inGameText;

    [SerializeField]
    private string _startText = "�X�^�[�g";

    [SerializeField]
    private string _finishText = "�����";

    [Header("�Q�[���̎���")]

    [SerializeField]
    private float _gameTime = 0f;

    [Header("���o�֌W")]

    [SerializeField]
    private Color _timerChangeColor = Color.red;

    [SerializeField]
    private float _timerColorChangeInterval = 1.0f;

    private float _time = 0f;
    /// <summary>
    /// DoTween���d�����Ȃ��悤�̃t���O
    /// </summary>
    bool _isTimeTextchange = false;

    void Start()
    {
        GameManager.Instance.Cinemachine = _cinemachine;
        _time = _gameTime;

        GameManager.Instance.Score
            .Subscribe(SetScore)
            .AddTo(gameObject);
    }

    void Update()
    {
        SetTime();
        TimeControl();
    }

    private void SetTime()
    {
        if (_timeText == null) return;

        _timeText.text = _time.ToString("00");
    }

    private void SetScore(int score)
    {
        if (_scoreText == null) return;

        _scoreText.text = score.ToString("00");

        var sequence = DOTween.Sequence();
        sequence.Insert(0f, _scoreText.transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack));
        sequence.Insert(0.5f, _scoreText.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack));

        sequence.Play();

    }

    private void TimeControl()
    {
        _time -= Time.deltaTime;

        if (_time < _gameTime / 3 && !_isTimeTextchange)
        {
            //�c�莞�Ԃ�1/3�ɂȂ�����^�C�}�[�̕�����_�ł�����
            _isTimeTextchange = true;
            _timeText.DOColor(
                _timerChangeColor,
                _timerColorChangeInterval)
                .SetLoops(-1, LoopType.Yoyo);
        }

        if (_time < 0)
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        _sceneSystem.Result_Scene();
    }
}
