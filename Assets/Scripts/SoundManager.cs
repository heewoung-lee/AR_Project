using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum SoundClip
{
    Casting,
    Drag,
    Tada,
    LureRand
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    public AudioClip[] audioClips;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(instance != this) 
            {
                 Destroy(this.gameObject);
            }
        }
    }

    public IEnumerator SFXPlay(string sfxName, AudioClip clip, float deley = 0f)
    {
        yield return new WaitForSeconds(deley);
        GameObject go = new GameObject(sfxName+"Sound");
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        Destroy(go,clip.length);
    }

    public void StopSFX(string sfxName)
    {
        GameObject soundObject = GameObject.Find(sfxName + "Sound");
        if (soundObject != null)
        {
            Destroy(soundObject);
        }
    }
}
