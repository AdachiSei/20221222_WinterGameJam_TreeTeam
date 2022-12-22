using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    #region Public Properties

    public float BGMLength => _bgmLength;
    public int AudioCount => _audioCount;
    public bool IsStopCreate => _isStopCreate;

    #endregion

    #region Inspector Menber

    [SerializeField]
    [Header("�ŏ��ɗ���BGM")]
    private string _name;

    [SerializeField]
    [Header("�}�X�^�[����")]
    [Range(0, 1)]
    private float _masterVolume;

    [SerializeField]
    [Header("���y�̉���")]
    [Range(0, 1)]
    private float _bgmVolume;

    [SerializeField]
    [Header("���ʉ��̉���")]
    [Range(0, 1)]
    private float _sfxVolume;

    [SerializeField]
    [Header("����������܂ł̎���")]
    int _fadeTime = 2000;

    [SerializeField]
    [Header("���y���i�[����I�u�W�F�N�g")]
    private GameObject _bGMParent = null;

    [SerializeField]
    [Header("���ʉ����i�[����I�u�W�F�N�g")]
    private GameObject _sFXParent = null;

    [SerializeField]
    [Header("�I�[�f�B�I�\�[�X�����Ă���v���t�@�u")]
    private AudioSource _audioPrefab = null;

    [SerializeField]
    [Header("���y�̃N���b�v")]
    AudioClip[] _bgmClips;

    [SerializeField]
    [Header("���ʉ��̃N���b�v")]
    AudioClip[] _sfxClips;

    [SerializeField]
    [Header("BGM�p�̃I�[�f�B�I�\�[�X")]
    private List<AudioSource> _bgmAudioSources = new();

    [SerializeField]
    [Header("SFX�p�̃I�[�f�B�I�\�[�X")]
    private List<AudioSource> _sfxAudioSources = new();

    #endregion

    #region Private Menber

    private float _bgmLength = 10f;
    private int _audioCount;
    private int _newAudioSourceNum;
    private bool _isStopCreate;
    private bool _isPausing;

    #endregion

    #region Const Member

    private const int OFFSET = 1;

    #endregion

    #region Unity Method

    protected override void Awake()
    {
        base.Awake();

        //�I�[�f�B�I�̃v���t�@�u������������
        if (_audioPrefab == null) CreateAudio();

        //BGM���i�[����I�u�W�F�N�g������������
        if (_bGMParent == null) CreateBGMParent();

        //SFX���i�[����I�u�W�F�N�g������������
        if (_sFXParent == null) CreateSFXParent();

        if (_audioPrefab.playOnAwake) _audioPrefab.playOnAwake = false;

        PlayBGM(_name);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            PlaySFX(SFXNames.CLICK);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// ���y(BGM)���Đ�����֐�
    /// </summary>
    /// <param name="name">Data�ɐݒ肵�����y(BGM)�̖��O</param>
    /// <param name="volume">���̑傫��</param>
    public void PlayBGM(string name, float volume = 1)
    {
        var bgmVolume = volume * _masterVolume * _bgmVolume;
        //BGM���~�߂�
        foreach (var audio in _bgmAudioSources)
        {
            audio.Stop();
            audio.name = audio.clip.name;
        }
        if (name == "") return;
        //�Đ������������i�[���Ă���I�u�W�F�N�g����i�荞��
        foreach (var audio in _bgmAudioSources)
        {
            var audioName = audio.name == name;
            var clipName = audio.clip.name == name;
            if (audioName || clipName)
            {
                audio.name = $"�� {audio.name}";
                audio.volume = bgmVolume;
                audio.Play();
                return;
            }
        }
        //�Đ������������i�[���Ă���I�u�W�F�N�g������������
        //�Đ�����������Data����i�荞��
        foreach (var clip in _bgmClips/*_bGMData.BGMs*/)
        {
            //var bGMName = bgm.Name == name;
            var clipName = clip.name == name;
            if (clipName)
            {
                //�Đ�������������Audio�𐶐�
                var newAudio = Instantiate(_audioPrefab);
                newAudio.transform.SetParent(_bGMParent.transform);
                _bgmAudioSources.Add(newAudio);
                newAudio.volume = bgmVolume;
                newAudio.clip = clip;
                newAudio.name = $"New {clip.name}";
                newAudio.loop = true;
                newAudio.Play();
                return;
            }
        }
        Debug.Log("BGM��������Ȃ�����");
    }

    /// <summary>
    /// ���ʉ�(SFX)���Đ�����֐�
    /// </summary>
    /// <param name="name">Data�ɐݒ肵�����ʉ�(SFX)�̖��O</param>
    /// <param name="volume">���̑傫��</param>
    async public void PlaySFX(string name, float volume = 1)
    {
        var sfxVolume = volume * _masterVolume * _sfxVolume;
        //�Đ�����������Data������i�荞��
        foreach (var clip in _sfxClips/*_sFXData.SFXes*/)
        {
            //var sFXName = sfx.Name == name;
            var clipName = clip.name == name;
            if (clipName)
            {
                //Clip��null��Audio��T��
                foreach (var audio in _sfxAudioSources)
                {
                    if (audio.clip == null)
                    {
                        audio.clip = clip;
                        audio.volume = sfxVolume;
                        var privName = audio.name;
                        audio.name = clip.name;
                        audio.Play();
                        await UniTask.WaitUntil(() => !audio.isPlaying && !_isPausing);
                        audio.name = privName;
                        audio.clip = null;
                        return;
                    }
                }
                //����������V�������
                var audioSource = Instantiate(_audioPrefab);
                audioSource.transform.SetParent(_sFXParent.transform);
                audioSource.name = "NewSFX " + _newAudioSourceNum;
                _newAudioSourceNum++;
                _sfxAudioSources.Add(audioSource);
                var newAudio = _sfxAudioSources[_sfxAudioSources.Count - OFFSET];
                newAudio.clip = clip;
                newAudio.volume = sfxVolume;
                var newPrivName = newAudio.name;
                newAudio.name = clip.name;
                newAudio.Play();
                await UniTask.WaitUntil(() => !newAudio.isPlaying && !_isPausing);
                newAudio.name = newPrivName;
                newAudio.clip = null;
                return;
            }
        }
        Debug.Log("SFX��������Ȃ�����");
    }

    /// <summary>
    /// BGM���t�F�[�h����֐�
    /// </summary>
    async public UniTask FadeBGM()
    {
        //BGM�̉��ʂ�������������
        foreach (var audio in _bgmAudioSources)
        {
            //audio.Stop();
            if (audio.isPlaying) audio.DOFade(0, _fadeTime);
        }
        //await UniTask.NextFrame();
        await UniTask.Delay(_fadeTime);

        //BGM���~�߂�
        foreach (var audio in _bgmAudioSources)
        {
            if (audio.isPlaying)
            {
                audio.Stop();
                audio.name = audio.clip.name;
                audio.volume = 1;
            }
        }
    }

    /// <summary>
    /// �}�X�^�[���ʂ�ύX���Ĕ��f����֐�
    /// </summary>
    /// <param name="masterVolume">�}�X�^�[����</param>
    public void ChangeMasterVolume(float masterVolume)
    {
        _masterVolume = masterVolume;
        ReflectMasterVolume();
    }

    /// <summary>
    /// ���y�̉��ʂ�ύX���Ĕ��f����֐�
    /// </summary>
    /// <param name="bgmVolume">���y�̉���</param>
    public void ChangeBGMVolume(float bgmVolume)
    {
        _bgmVolume = bgmVolume;
        ReflectBGMVolume();
    }

    /// <summary>
    /// ���ʉ��̉��ʂ�ύX���Ĕ��f����֐�
    /// </summary>
    /// <param name="sfxVolume">���ʉ��̉���</param>
    public void ChangeSFXVolume(float sfxVolume)
    {
        _sfxVolume = sfxVolume;
        ReflectSFXVolume();
    }

    /// <summary>
    /// �Đ����Ă���S�Ẳ��̉��ʂ̕ύX�𔽉f����֐�
    /// </summary>
    public void ReflectMasterVolume()
    {
        ReflectBGMVolume();
        ReflectSFXVolume();
    }

    /// <summary>
    /// �Đ����Ă���S�Ẳ��y�̉��ʂ𔽉f����֐�
    /// </summary>
    public void ReflectBGMVolume()
    {
        foreach (var audioSource in _bgmAudioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.volume = _masterVolume * _bgmVolume;
            }
        }
    }

    /// <summary>
    /// �Đ����Ă���S�Ă̌��ʉ��̉��ʂ�ύX�𔽉f����֐�
    /// </summary>
    public void ReflectSFXVolume()
    {
        foreach (var audioSource in _sfxAudioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.volume = _masterVolume * _sfxVolume;
            }
        }
    }

    #endregion

    #region Inspector Methods

    /// <summary>
    /// ��������SFX�pAudio�̐���ύX����֐�
    /// </summary>
    /// <param name="count">��������Audio�̐�</param>
    public void ChangeAudioCount(int count) =>
        _audioCount = count;

    /// <summary>
    /// BGM�p��Prefab�𐶐�����֐�
    /// </summary>
    public void CreateBGM()
    {
        if (_audioPrefab == null) CreateAudio();
        if (_bGMParent == null) CreateBGMParent();
        _isStopCreate = false;
        _bgmAudioSources.Clear();
        InitBGM();
        for (var i = 0; i < _bgmClips.Length; i++)
        {
            var audio = Instantiate(_audioPrefab);
            audio.transform.SetParent(_bGMParent.transform);
            _bgmAudioSources.Add(audio);
            audio.name = _bgmClips[i].name;
            audio.clip = _bgmClips[i];
            audio.loop = true;
        }
        _bgmAudioSources = new(_bgmAudioSources.Distinct());
    }

    /// <summary>
    /// SFX�p��Prefab�𐶐�����֐�
    /// </summary>
    public void CreateSFX()
    {
        if (_audioPrefab == null) CreateAudio();
        if (_sFXParent == null) CreateSFXParent();
        _isStopCreate = false;
        _sfxAudioSources.Clear();
        InitSFX();
        for (var i = 0; i < _audioCount; i++)
        {
            var audio = Instantiate(_audioPrefab);
            audio.transform.SetParent(_sFXParent.transform);
            audio.loop = false;
            if (i < 10) audio.name = "SFX " + "00" + i;
            else if (i < 100) audio.name = "SFX " + "0" + i;
            else audio.name = "SFX " + i;
            _sfxAudioSources.Add(audio);
        }
    }

    /// <summary>
    /// BGM&SFX�p��Prefab��S�폜����֐�
    /// </summary>
    public void Init()
    {
        _isStopCreate = true;
        _bgmAudioSources.Clear();
        _sfxAudioSources.Clear();
        InitBGM();
        InitSFX();
    }

    #endregion

    #region Editor Methods

    public void ChangeAudioLength(float length)
    {
        _bgmLength = length;
    }

    public void ResizeBGMClips(int length)
    {
        Array.Resize(ref _bgmClips, length);
    }

    public void ResizeSFXClips(int length)
    {
        Array.Resize(ref _sfxClips, length);
    }

    public void AddBGMClip(int index, AudioClip clip)
    {
        _bgmClips[index] = clip;
    }

    public void AddSFXClip(int index, AudioClip clip)
    {
        _sfxClips[index] = clip;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// ���y���i�[����I�u�W�F�N�g�𐶐�����֐�
    /// </summary>
    private void CreateBGMParent()
    {
        _bGMParent = new();
        _bGMParent.name = "BGM";
        _bGMParent.transform.parent = transform;
    }

    /// <summary>
    /// ���ʉ����i�[����I�u�W�F�N�g�𐶐�����֐�
    /// </summary>
    private void CreateSFXParent()
    {
        _sFXParent = new();
        _sFXParent.name = "SFX";
        _sFXParent.transform.parent = transform;
    }

    /// <summary>
    /// �I�[�f�B�I�\�[�X���t���I�u�W�F�N�g�𐶐�����֐�
    /// </summary>
    private void CreateAudio()
    {
        GameObject gameObj = new();
        gameObj.transform.parent = transform;
        _audioPrefab = gameObj.AddComponent<AudioSource>();
        _audioPrefab.playOnAwake = false;
        _audioPrefab.name = "XD";
    }

    /// <summary>
    /// BGM�p��Prefab��S�폜����֐�
    /// </summary>
    private void InitBGM()
    {
        if (_bGMParent == null) return;
        while (true)
        {
            var children = _bGMParent.transform;
            var empty = children.childCount == 0;
            if (empty) break;
            var DestroyGO = children.GetChild(0).gameObject;
            DestroyImmediate(DestroyGO);
        }
    }

    /// <summary>
    /// SFX�p��Prefab��S�폜����֐�
    /// </summary>
    private void InitSFX()
    {
        if (_sFXParent == null) return;
        while (true)
        {
            var children = _sFXParent.transform;
            var empty = children.childCount == 0;
            if (empty) break;
            var DestroyGO = children.GetChild(0).gameObject;
            DestroyImmediate(DestroyGO);
        }
    }

    /// <summary>
    /// �|�[�Y�p�̊֐�
    /// </summary>
    private void Pause()
    {
        _isPausing = true;
        foreach (var bGMAudio in _bgmAudioSources)
        {
            if (bGMAudio.isPlaying) bGMAudio.Pause();
        }
        foreach (var sFXAudio in _sfxAudioSources)
        {
            if (sFXAudio.isPlaying) sFXAudio.Pause();
        }
    }

    /// <summary>
    /// �|�[�Y�����p�̊֐�
    /// </summary>
    private void Resume()
    {
        _isPausing = false;
        foreach (var bgm in _bgmAudioSources) bgm.UnPause();
        foreach (var sfx in _sfxAudioSources) sfx.UnPause();
    }

    #endregion
}
