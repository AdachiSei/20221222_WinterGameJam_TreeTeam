using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public ItemBase Item => _item;
    public float Probability => _probability;

    [SerializeField]
    [Header("�A�C�e��")]
    ItemBase _item;

    [SerializeField]
    [Header("�m��")]
    float _probability;
}
