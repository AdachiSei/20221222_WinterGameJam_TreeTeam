using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class Generator : MonoBehaviour
{
    [SerializeField]
    [Header("�������ꂽ��&���e��Y��")]
    float _posY;

    [SerializeField]
    [Header("�N�[���^�C��")]
    Value<int> _coolTime;

    [SerializeField]
    [Header("x���͈̔�")]
    Value<float> _posXRange;

    [SerializeField]
    [Header("�J����")]
    Camera _camera;

    [SerializeField]
    [Header("�Ō�ɂ̂����")]
    BitterOrangeController _bitterOrange;

    [SerializeField]
    [Header("������(��&���e)")]
    ItemData[] _item = new ItemData[2];

    private bool _isGenerating = true;

    const float MAX_VALUE_F = 100f;

    private void Awake()
    {
        float _firstPosY = _posY;
        _camera
            .ObserveEveryValueChanged(camera => camera.transform.position.y)
            .Subscribe(y => _posY = _firstPosY + y);
        Generate();
    }

    /// <summary>
    /// �Ō�ɞ�𐶐�����
    /// �c�莞�Ԃ��Ō�̕��ɂȂ�����Ăяo���Ă�������
    /// </summary>
    public void ChangeIsGenerating()
    {
        _isGenerating = false;
    }

    async private void Generate()
    {
        var randomTime = Random.Range(_coolTime.MinValue, _coolTime.MaxValue);
        await UniTask.Delay(randomTime);

        float randomPosX = 0f;

        while (_isGenerating)
        {
            var item = Instantiate(_item[RandomIndex(_item)].Item);
            item.transform.SetParent(transform);

            randomPosX = Random.Range(_posXRange.MinValue, _posXRange.MaxValue);
            item.transform.ChangePosX(randomPosX);
            item.transform.ChangePosY(_posY);

            randomTime = Random.Range(_coolTime.MinValue, _coolTime.MaxValue);
            await UniTask.Delay(randomTime);
        }

        var bitterOrange = Instantiate(_bitterOrange);
        bitterOrange.transform.SetParent(transform);

        randomPosX = Random.Range(_posXRange.MinValue, _posXRange.MaxValue);
        bitterOrange.transform.ChangePosX(randomPosX);
        bitterOrange.transform.ChangePosY(_posY);
    }

    /// <summary>
    /// �K�`���̂悤�Ȋ֐�
    /// </summary>
    /// <param name="num">�m��</param>
    /// <returns>Index</returns>
    private int RandomIndex(ItemData[] num)
    {
        float[] probability = null;
        var sum = num.Select(x => x.Probability).Sum();
        var limitCount = 1;
        System.Array.Resize(ref probability, num.Length);
        for (int index = 0; index < num.Length; index++)
        {
            for (int count = 0; count < limitCount; count++)
            {
                probability[index] += num[count].Probability * MAX_VALUE_F / sum;
            }
            //Debug.Log(index + "�Ԗ� " + probability[index]);
            limitCount++;
        }
        var randomValue = Random.Range(0f, MAX_VALUE_F);
        //Debug.Log("���� " + randomValue);
        for (int i = 0; i < probability.Length; i++)
        {
            if (probability[i] > randomValue)
            {
                //Debug.Log("���ʂ�" + i);
                return i;
            }
        }
        return 0;
    }
}
