using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RecoilSystem : MonoBehaviour
{
    public CinemachineVirtualCamera playerCamera;

    [SerializeField] float verticalRecoil;
    [SerializeField] float duration;

    float time;
    void Start()
    {
        time = duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0) {
            playerCamera.GetComponent<CinemachineRecomposer>().m_Tilt -= ((verticalRecoil/1000) * Time.deltaTime) / duration;
            time -= Time.deltaTime;
        }
    }
}
