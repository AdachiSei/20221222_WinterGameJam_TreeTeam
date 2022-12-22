using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���U���g�p�̋���
/// </summary>
public class ResultKagamiMochi : MonoBehaviour
{
    [SerializeField]
    [Header("���U���g�pUI�}�l�[�W���[")]
    ResultUIManager _resultUIManager;

    [SerializeField]
    [Header("��")]
    MochiController _mochi;

    [SerializeField]
    [Header("��")]
    BitterOrangeController _bitterOrange;

    [SerializeField]
    [Header("���݂𐶐�����N�[���^�C��(�~���b)")]
    int _coolTime = 500;

    [SerializeField]
    [Header("�����ʒu(Y)")]
    float _posY = 7f;

    async private void Awake()
    {
        await InstantiateMochi();
        _resultUIManager.ChangeActive();
    }

    async private UniTask InstantiateMochi()
    {
        for (int i = 0; i < GameManager.Instance.Score.Value; i++)
        {
            var mochi = Instantiate(_mochi);
            mochi.transform.SetParent(transform);
            mochi.transform.ChangePosX(-0.73f);
            mochi.transform.ChangePosY(_posY);
            await UniTask.Delay(_coolTime);
        }

        var bitterOrange = Instantiate(_bitterOrange);
        bitterOrange.transform.SetParent(transform);
        bitterOrange.transform.ChangePosX(0f);
        bitterOrange.transform.ChangePosY(_posY);
    }
}
