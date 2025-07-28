using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    [System.Serializable]
    public class AudioClipLibraryEntry
    {
        public string Name;
        public AudioClip Clip;
    }

    [CreateAssetMenu(fileName = "AudioClipLibrary", menuName = "MIN/AudioClipLibrary")]
    public class AudioClipLibrary : ScriptableObject
    {
        [SerializeField] private List<AudioClipLibraryEntry> _audioClips;

        public Dictionary<string, AudioClip> AudioClips { get; private set; }

        public void Init()
        {
            AudioClips = new();
            foreach (var entry in _audioClips)
            {
                if (!AudioClips.TryAdd(entry.Name, entry.Clip))
                {
                    Debug.LogWarning($"중복된 키 '{entry.Name}'가 감지되어 추가되지 않았습니다.");
                }
            }
        }

        public bool TryGetClip(string key, out AudioClip clip)
        {
            if (AudioClips == null || AudioClips.Count == 0)
            {
                Debug.LogWarning("AudioClipLibrary가 초기화되지 않았습니다.");
                clip = null;
                return false;
            }

            return AudioClips.TryGetValue(key, out clip);
        }
    }
}
