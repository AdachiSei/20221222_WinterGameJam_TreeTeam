using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���݂̉��̕����A���U���g�p
/// </summary>
public class Sanpo : MonoBehaviour
{
    [SerializeField]
    [Header("�݂̃^�O")]
    private string _mochiTag = "Mochi";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == _mochiTag)
        {
            collision.transform.SetParent(transform);
        }
    }
}
