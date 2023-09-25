namespace NMX.SpaceShooter.Managers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
        [System.Serializable]
        private class Sound
        {
            public string name;
            public AudioClip clip;

            [Range(0f, 1f)]
            public float volume;
            public bool loop;

            [HideInInspector]
            public AudioSource source;
        }
        [SerializeField]
        private Sound[] sounds;
        private Dictionary<string, AudioSource> soundsList;
        public static AudioManager instance;

        void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);

            soundsList = new(sounds.Length);
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.loop = s.loop;
                soundsList.Add(s.name, s.source);
            }
        }

        public bool IsPlaying(string name)
        {
            if (!soundsList.ContainsKey(name)) { Debug.LogWarning("Sound: " + name + " not found!"); return false; }
            return soundsList[name].isPlaying;
        }

        public void Play(string name)
        {
            if (!soundsList.ContainsKey(name)) { Debug.LogWarning("Sound: " + name + " not found!"); return; }
            soundsList[name].Play();
        }

        public void PlayOneShot(string name)
        {
            if (!soundsList.ContainsKey(name)) { Debug.LogWarning("Sound: " + name + " not found!"); return; }
            soundsList[name].PlayOneShot(soundsList[name].clip);
        }

        public void Pause(string name)
        {
            if (!soundsList.ContainsKey(name)) { Debug.LogWarning("Sound: " + name + " not found!"); return; }
            soundsList[name].Pause();
        }

        public void UnPause(string name)
        {
            if (!soundsList.ContainsKey(name)) { Debug.LogWarning("Sound: " + name + " not found!"); return; }
            soundsList[name].UnPause();
        }

        public void Stop(string name)
        {
            if (!soundsList.ContainsKey(name)) { Debug.LogWarning("Sound: " + name + " not found!"); return; }
            soundsList[name].Stop();
        }

    }
}