using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAssultRifle : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon; //무기장착 사운드

    private AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        PlaySound(audioClipTakeOutWeapon);
    }

    private void PlaySound(AudioClip clip){
        audioSource.Stop(); //기존에 재생되던 사운드 정지
        audioSource.clip = clip; //현재 clip설정
        audioSource.Play(); //재생
    }   

}
