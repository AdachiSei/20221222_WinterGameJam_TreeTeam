using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �݂┚�e�̊��N���X
/// </summary>
public abstract class ItemBase : MonoBehaviour
{
    [SerializeField]
    [Header("�����X�s�[�h")]
    [Range(0f,0.1f)]
    protected float _speed = 0.05f;

    [SerializeField]
    [Header("�v���C��-�̃^�O")]
    protected string _playerTag = "Player";

    protected bool _isMoving = true;

    protected abstract void OnCollisionEnter(Collision collision);

    protected abstract void OnBecameInvisible();

    protected abstract void OnMove();
}
