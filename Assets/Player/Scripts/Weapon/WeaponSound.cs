using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSound : MonoBehaviour
{
    public AudioSource audioSource;

    public void ShootSound() {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
