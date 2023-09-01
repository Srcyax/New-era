using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] GameObject info;

    public void ShowPanel() {
        info.SetActive(!info.activeSelf);
    }
}
