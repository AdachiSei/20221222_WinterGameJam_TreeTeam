using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]

public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("�ړ����x")]
    private float _speed = 1f;
    /// <summary>Rigidbody</summary>
    private Rigidbody _rb = null;
    [SerializeField]
    private string _tagName = "";

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation | 
            RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        _rb.velocity = new Vector3(h * _speed, 0f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == _tagName)
        {
            collision.transform.SetParent(transform);
            GameManager.Instance.PushMochi(collision.gameObject);
            Debug.Log(collision.gameObject.name);
        }
    }
}
