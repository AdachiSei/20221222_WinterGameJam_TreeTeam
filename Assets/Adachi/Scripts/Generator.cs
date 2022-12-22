using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField]
    [Header("Y��")]
    float _posY;

    [SerializeField]
    [Header("�N�[���^�C��")]
    Value<int> _coolTime;

    [SerializeField]
    [Header("x���͈̔�")]
    Value<float> _posXRange;

    [SerializeField]
    [Header("������(��&���e)")]
    ItemData[] _item = new ItemData[2];

    const float MAX_VALUE_F = 100f;

    private void Awake()
    {
        Generate();
    }

    async private void Generate()
    {
        var randomTime = Random.Range(_coolTime.MinValue, _coolTime.MaxValue);
        await UniTask.Delay(randomTime);

        while (true)
        {
            var item = Instantiate(_item[RandomIndex(_item)].Item);
            item.transform.SetParent(transform);

            var randomPosX = Random.Range(_posXRange.MinValue, _posXRange.MaxValue);
            item.transform.ChangePosX(randomPosX);
            item.transform.ChangePosY(_posY);

            randomTime = Random.Range(_coolTime.MinValue, _coolTime.MaxValue);
            await UniTask.Delay(randomTime);
        }
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
