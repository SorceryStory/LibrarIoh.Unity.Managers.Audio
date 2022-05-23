using SorceressSpell.LibrarIoh.Unity.Pools;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

#if SORCERESSSPELL_LIBRARIOH_UNITY_MANAGERS_TIME

using SorceressSpell.LibrarIoh.Unity.Managers.Time;

#endif

namespace SorceressSpell.LibrarIoh.Unity.Managers.Audio
{
    public class AudioManager : MonoBehaviour
    {
        #region Fields

        public AudioBankScriptableObject AudioBank;
        public AudioMixer AudioMixer;

        private Dictionary<string, AudioMixerGroupController> _audioMixerGroupControllers;
        private Dictionary<string, AudioPoolObjectProperties> _audioMixerGroupProperties;
        private GameObject _gameObjectAudio;
        private GameObjectPool<AudioPoolObject> _poolAudio;
        private AudioPoolObjectProperties _propertiesAudio;

        #endregion Fields

        #region Methods

        public void AudioMixerGroupSetMute(string audioMixerGroupName, bool mute)
        {
            if (_audioMixerGroupControllers.TryGetValue(audioMixerGroupName, out AudioMixerGroupController audioMixerGroupController))
            {
                audioMixerGroupController.Muted = mute;
            }
        }

        public void AudioMixerGroupSetVolume(string audioMixerGroupName, float volume)
        {
            if (_audioMixerGroupControllers.TryGetValue(audioMixerGroupName, out AudioMixerGroupController audioMixerGroupController))
            {
                audioMixerGroupController.Volume = volume;
            }
        }

        public AudioPoolObjectProperties GetAudioProperties(string audioMixerGroupName)
        {
            return _audioMixerGroupProperties.ContainsKey(audioMixerGroupName) ? _audioMixerGroupProperties[audioMixerGroupName] : null;
        }

        public AudioPoolObject PlayAudio(string hookName, string audioMixerGroupName)
        {
            return PlayAudio(hookName, audioMixerGroupName, GetAudioProperties(audioMixerGroupName));
        }

        public AudioPoolObject PlayAudio(string hookName, string audioMixerGroupName, AudioPoolObjectProperties properties)
        {
            if (AudioBank.TryGetClip(hookName, out AudioClip audioClip) && _audioMixerGroupControllers.TryGetValue(audioMixerGroupName, out AudioMixerGroupController audioMixerGroupController) && properties != null)
            {
                return ActivateAudioObject(properties, audioClip, audioMixerGroupController); ;
            }

            return null;
        }

        public AudioPoolObject PlayAudio(string hookName, string audioMixerGroupName, bool affectedByTimescale, AudioSourceProperties audioProperties)
        {
            if (AudioBank.TryGetClip(hookName, out AudioClip audioClip) && _audioMixerGroupControllers.TryGetValue(audioMixerGroupName, out AudioMixerGroupController audioMixerGroupController) && audioProperties != null)
            {
                return ActivateAudioObject(audioProperties, audioClip, audioMixerGroupController, affectedByTimescale); ;
            }

            return null;
        }

        public bool TryGetAudioProperties(string audioMixerGroupName, out AudioPoolObjectProperties audioProperties)
        {
            return _audioMixerGroupProperties.TryGetValue(audioMixerGroupName, out audioProperties);
        }

        public bool TryPlayAudio(string hookName, string audioMixerGroupName, out AudioPoolObject audioPoolObject)
        {
            if (TryGetAudioProperties(audioMixerGroupName, out AudioPoolObjectProperties audioProperties))
            {
                return TryPlayAudio(hookName, audioMixerGroupName, audioProperties, out audioPoolObject);
            }

            audioPoolObject = null;
            return false;
        }

        public bool TryPlayAudio(string hookName, string audioMixerGroupName, AudioPoolObjectProperties properties, out AudioPoolObject audioPoolObject)
        {
            if (AudioBank.TryGetClip(hookName, out AudioClip audioClip) && _audioMixerGroupControllers.TryGetValue(audioMixerGroupName, out AudioMixerGroupController audioMixerGroupController) && properties != null)
            {
                audioPoolObject = ActivateAudioObject(properties, audioClip, audioMixerGroupController);
                return true;
            }

            audioPoolObject = null;
            return false;
        }

        public bool TryPlayAudio(string hookName, string audioMixerGroupName, bool affectedByTimescale, AudioSourceProperties audioProperties, out AudioPoolObject audioPoolObject)
        {
            if (AudioBank.TryGetClip(hookName, out AudioClip audioClip) && _audioMixerGroupControllers.TryGetValue(audioMixerGroupName, out AudioMixerGroupController audioMixerGroupController) && audioProperties != null)
            {
                audioPoolObject = ActivateAudioObject(audioProperties, audioClip, audioMixerGroupController, affectedByTimescale);
                return true;
            }

            audioPoolObject = null;
            return false;
        }

        private AudioPoolObject ActivateAudioObject(AudioSourceProperties audioProperties, AudioClip audioClip, AudioMixerGroupController audioMixerGroupController, bool affectedByTimescale)
        {
            _propertiesAudio.ResetChanges();
            _propertiesAudio.CopyFrom(audioProperties);
            _propertiesAudio.AffectedByTimescale = affectedByTimescale;

            return ActivateAudioObjectCommon(audioClip, audioMixerGroupController);
        }

        private AudioPoolObject ActivateAudioObject(AudioPoolObjectProperties properties, AudioClip audioClip, AudioMixerGroupController audioMixerGroupController)
        {
            _propertiesAudio.ResetChanges();
            _propertiesAudio.CopyFrom(properties);

            return ActivateAudioObjectCommon(audioClip, audioMixerGroupController);
        }

        private AudioPoolObject ActivateAudioObjectCommon(AudioClip audioClip, AudioMixerGroupController audioMixerGroupController)
        {
            _propertiesAudio.AudioSource.AudioClip = audioClip;
            _propertiesAudio.AudioSource.OutputAudioMixerGroup = audioMixerGroupController.AudioMixerGroup;

            if (_propertiesAudio.AffectedByTimescale)
            {
                _propertiesAudio.AudioSource.Pitch = UnityEngine.Time.timeScale;
            }

            AudioPoolObject poolObject = _poolAudio.Activate(_propertiesAudio);
            poolObject.AffectedByTimescale = _propertiesAudio.AffectedByTimescale;

            return poolObject;
        }

        private void Awake()
        {
#if SORCERESSSPELL_LIBRARIOH_UNITY_MANAGERS_TIME
            if (TryGetComponent(out TimeManager timeManager))
            {
                timeManager.OnTimeScaleChanged += ChangePitch;
            }
#endif
            _audioMixerGroupControllers = new Dictionary<string, AudioMixerGroupController>();
            _audioMixerGroupProperties = new Dictionary<string, AudioPoolObjectProperties>();

            if (AudioMixer != null)
            {
                foreach (var audioMixerGroup in AudioMixer.FindMatchingGroups(string.Empty))
                {
                    _audioMixerGroupControllers.Add(audioMixerGroup.name, new AudioMixerGroupController(audioMixerGroup, string.Format("Volume{0}", audioMixerGroup.name)));
                    _audioMixerGroupProperties.Add(audioMixerGroup.name, new AudioPoolObjectProperties());
                }
            }

            _gameObjectAudio = new GameObject("Audio");


            _propertiesAudio = new AudioPoolObjectProperties();

            _poolAudio = new GameObjectPool<AudioPoolObject>(_gameObjectAudio, null, "Audio", 10);
        }

        private void ChangePitch(float pitch)
        {
            _propertiesAudio.ResetChanges();

            _propertiesAudio.AudioSource.Pitch = pitch;

            _poolAudio.ApplyProperties(IsAffectedByTimeScale, _propertiesAudio);
        }

        private bool IsAffectedByTimeScale(AudioPoolObject audioPoolObject)
        {
            return audioPoolObject.AffectedByTimescale;
        }

        private void OnDestroy()
        {
#if SORCERESSSPELL_LIBRARIOH_UNITY_MANAGERS_TIME
            if (TryGetComponent(out TimeManager timeManager))
            {
                timeManager.OnTimeScaleChanged -= ChangePitch;
            }
#endif
        }

        private void Update()
        {
            _poolAudio.Update(UnityEngine.Time.deltaTime);
        }

        #endregion Methods
    }
}
