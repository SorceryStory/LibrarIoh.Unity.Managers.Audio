using System.Collections.Generic;
using UnityEngine;

namespace SorceressSpell.LibrarIoh.Unity.Managers.Audio
{
    [CreateAssetMenu(fileName = "AudioBankScriptableObject", menuName = "SorceressSpell.LibrarIoh.Unity.Audio/AudioBankScriptableObject")]
    public class AudioBankScriptableObject : ScriptableObject
    {
        #region Fields

        public List<AudioClipEntry> AudioClips;

        #endregion Fields

        public AudioClip GetClip(string hookName)
        {
            foreach (AudioClipEntry entry in AudioClips)
            {
                if (entry.HookName == hookName)
                {
                    return entry.AudioClip;
                }
            }

            return null;
        }

        public bool TryGetClip(string hookName, out AudioClip audioClip)
        {
            foreach (AudioClipEntry entry in AudioClips)
            {
                if (entry.HookName == hookName)
                {
                    audioClip = entry.AudioClip;
                    return true;
                }
            }

            audioClip = null;
            return false;
        }
    }
}
