using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField]
    [Header("�����X�s�[�h")]
    protected float _speed;

    protected abstract void OnCollisionEnter(Collision collision);

    protected abstract void OnBecameInvisible();
}
