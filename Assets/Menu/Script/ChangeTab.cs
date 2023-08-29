using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTab : MonoBehaviour
{
    [SerializeField] GameObject tab1, tab2;

    public void ChangeCurrentTab(bool enable) {
        tab1.SetActive(enable);
        tab2.SetActive(!enable);
    }
}