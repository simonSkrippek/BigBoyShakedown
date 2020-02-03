using UnityEngine.Audio;
using System;
using UnityEngine;

namespace BigBoyShakedown.Manager
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        public AudioMixerGroup mixerGroup;

        public Sound[] sounds;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.loop = s.loop;

                s.source.outputAudioMixerGroup = mixerGroup;
            }
        }

        public void Play(string sound)
        {
            Debug.Log("sound played:" + sound);
            Sound s = Array.Find(sounds, item => item.name == sound);

            if (s == null)
            {
                Debug.LogWarning("Sound: " + sound + " not found!");
                return;
            }
            else
            {
                s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
                var calcPitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
                if (calcPitch == 0f)
                {
                    calcPitch = 1f;
                }
                s.source.pitch = calcPitch;

                s.source.Play();
            }
        }
        public void StopPlaying(string sound)
        {
            Debug.Log("sound stopped:" + sound);
            Sound s = Array.Find(sounds, item => item.name == sound);

            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }
            else
            {
                s.source.Stop();
            }
        }
        public void StopPlaying()
        {
            foreach (var sound in sounds)
            {
                sound.source.Stop();
            }
        }
    }
}
