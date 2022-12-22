using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIManager : MonoBehaviour
{
    [SerializeField]
    [Header("�L�����o�X")]
    Canvas _canvas;

    [SerializeField]
    [Header("�X�R�A�̃e�L�X�g")]
    Text _scoreText;

    public void ChangeActive()
    {
        _scoreText.text = $"�X�R�A : {GameManager.Instance.Score.Value}";
        _canvas.gameObject.SetActive(true);
    }
}
