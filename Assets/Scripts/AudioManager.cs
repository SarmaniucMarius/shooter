using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource gameMusic;

    float masterVolume = 1;
    float sfxVolume = 1;
    float musicVolume = 1;

    private static AudioManager instance;

    public static AudioManager Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;

        gameMusic.volume = 0;
        gameMusic.Play();
        StartCoroutine("MusicFade", 5f);
    }

    IEnumerator MusicFade(float duration)
    {
        float precent = 0;
        while(precent <= 1f)
        {
            precent += Time.deltaTime * (1 / duration);
            gameMusic.volume = Mathf.Lerp(0, masterVolume * musicVolume, precent);
            yield return null;
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        if(clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, masterVolume * sfxVolume);
        }
    }
}
