using SorceressSpell.LibrarIoh.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SorceressSpell.LibrarIoh.Unity.Managers.Audio
{
    public static class AudioMixerExtensions
    {
        public static void SetVolume(this AudioMixer mixer, string exposedName, float value)
        {
            mixer.SetFloat(exposedName, MathOperations.Lerp(-80.0f, 0.0f, MathOperations.Clamp(value, 0f, 1f)));
        }

        public static float GetVolume(this AudioMixer mixer, string exposedName)
        {
            if (mixer.GetFloat(exposedName, out float volume))
            {
                return MathOperations.InverseLerp(-80.0f, 0.0f, volume);
            }

            return 0f;
        }
    }
}