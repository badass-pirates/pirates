using UnityEngine;

// 충돌 시 소리를 재생하는 스크립트
public class PlayCollisionSound : MonoBehaviour
{
    // 충돌 시 재생할 AudioSource
    public AudioSource collideSound;

    // 충돌이 감지될 때 호출되는 콜백
    private void OnCollisionEnter(Collision other)
    {
        // AudioSource를 이용하여 충돌 소리를 재생
        collideSound.Play();
    }
}
