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

    [SerializeField]
    private Generator _generator;

    [Header("�e�L�X�g�֌W")]

    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _timeText;

    [SerializeField]
    private Text _inGameText;

    [SerializeField]
    private Text _fText;

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

    float _countDown = 3.9f;
    private float _time = 0f;
    /// <summary>
    /// DoTween���d�����Ȃ��悤�̃t���O
    /// </summary>
    bool _isTimeTextchange = false;

    private static bool _isGame = false;
    public static bool IsGame => _isGame;

    private bool _isFinish = false;

    void Start()
    {
        GameManager.Instance.Reset();
        GameManager.Instance.Cinemachine = _cinemachine;
        GameManager.Instance.Player = GameObject.FindGameObjectWithTag("Player");
        _time = _gameTime;

        GameManager.Instance.Score
            .Skip(1)
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
        if (!_isGame)
        {
            //�J�E���g�_�E��
            _countDown -= Time.deltaTime;

            _inGameText.text = Mathf.Floor(_countDown).ToString();
        }
        else if (!_isFinish)
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
        }

        if (_countDown < 1 && _countDown >= 0)
        {
            //�J�E���g�_�E�����I�������Q�[�����J�n
            _inGameText.text = _startText;           
        } 
        else if (_countDown < 0)
        {
            _inGameText.text = "";
            _isGame = true;
        }

        if (_time < 0 && !_isFinish)
        {
            StartCoroutine(FinishGame());
        }
    }

    private IEnumerator FinishGame()
    {
        _isFinish = true;
        _isGame = false;
        _generator.ChangeIsGenerating();
        _fText.text = _finishText;
        yield return new WaitForSeconds(3.0f);
        
        _sceneSystem.Result_Scene();
    }
}
