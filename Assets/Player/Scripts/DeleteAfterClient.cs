using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterClient : MonoBehaviour
{
    [SerializeField] float time = 2f;
    void Start()
    {
        Destroy(gameObject, time);
    }
}
