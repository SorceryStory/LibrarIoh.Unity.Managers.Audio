using SorceressSpell.IdleCamp;
using SorceressSpell.LibrarIoh.Timers;
using SorceressSpell.LibrarIoh.Unity.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SorceressSpell.LibrarIoh.Unity.Managers.Audio
{
    public class OldAudioManager
    {
        private const string _musicPrefKey = "Music";
        private const string _sfxPrefKey = "Sound";

        private const string _musicVolumeMixerKey = "VolumeMusic";
        private const string _ambianceVolumeMixerKey = "VolumeAmbiance";
        private const string _sfxVolumeMixerKey = "VolumeSFX";

        private const float _muteVolume = -100f;
        private const float _defaultCrossFadeTime = 0.75f;

        private GameObject _soundGameObject;

        private AudioMixer _audioMixer;

        private AudioMixerGroup _musicAudioMixerGroup;
        private AudioMixerGroup _ambianceAudioMixerGroup;
        private AudioMixerGroup _sfxAudioMixerGroup;

        private float _musicVolumeBase;
        private float _ambianceVolumeBase;
        private float _sfxVolumeBase;

        public bool Music { get; private set; }

        // Music
        private AudioSource _musicAudioSource;
        private AudioSource _musicTransitionAudioSource;

        private List<LongClipTransition> _musicTransitions;
        private bool _musicTransitioning;
        private UpdateTimer _musicTransitionTimer;

        // Ambiance
        private AudioSource _ambianceAudioSource;
        private AudioSource _ambianceTransitionAudioSource;

        private List<LongClipTransition> _ambianceTransitions;
        private bool _ambianceTransitioning;
        private UpdateTimer _ambianceTransitionTimer;

        //Story
        private AudioSource _storyAudioSource;


        public bool SFX { get; private set; }

        private List<AudioSource> _sfxAudioSources;

        public AudioBankScriptableObject AudioBank;

        private float _timeScale;





        private AudioSource _boatAudioSource;

        #region Constructors
        private void Awake()
        {
            //_timeScale = Time.timeScale;

            InitSoundVariablesAndObjects();
        }
        #endregion Constructors

        // Update is called once per frame
        public void Update(float deltaTime)
        {

            UpdateTransitions(deltaTime);

            UpdateSFXAudioSources(deltaTime);
        }

        private void ChangeTimeScale(float newTimeScale)
        {
            //if (_timeScale != Time.timeScale)
            //{
            //    _timeScale = Time.timeScale;

            //    foreach (AudioSource audioSource in _sfxAudioSources)
            //    {
            //        audioSource.pitch = _timeScale;
            //    }
            //}
        }

        private void InitSoundVariablesAndObjects()
        {
            _soundGameObject = new GameObject("Audio");

            _audioMixer = Resources.Load<AudioMixer>("Audio/MainAudioMixer");

            AudioMixerGroup[] musicAudioMixerGroups = _audioMixer.FindMatchingGroups("Music");
            AudioMixerGroup[] ambianceAudioMixerGroups = _audioMixer.FindMatchingGroups("Ambiance");
            AudioMixerGroup[] sfxAudioMixerGroups = _audioMixer.FindMatchingGroups("SFX");
            _musicAudioMixerGroup = musicAudioMixerGroups[0];
            _ambianceAudioMixerGroup = ambianceAudioMixerGroups[0];
            _sfxAudioMixerGroup = sfxAudioMixerGroups[0];

            // Variables
            _audioMixer.GetFloat(_musicVolumeMixerKey, out _musicVolumeBase);
            _audioMixer.GetFloat(_ambianceVolumeMixerKey, out _ambianceVolumeBase);
            _audioMixer.GetFloat(_sfxVolumeMixerKey, out _sfxVolumeBase);

            // Music
            _musicAudioSource = _soundGameObject.AddComponent<AudioSource>();

            _musicAudioSource.outputAudioMixerGroup = _musicAudioMixerGroup;

            _musicAudioSource.playOnAwake = false;
            _musicAudioSource.loop = true;

            _musicAudioSource.volume = 1f;

            // Music Transition
            _musicTransitionAudioSource = _soundGameObject.AddComponent<AudioSource>();

            _musicTransitionAudioSource.outputAudioMixerGroup = _musicAudioMixerGroup;

            _musicTransitionAudioSource.playOnAwake = false;
            _musicTransitionAudioSource.loop = true;

            _musicTransitionAudioSource.volume = 0f;

            // Music Transitions
            _musicTransitions = new List<LongClipTransition>();
            _musicTransitioning = false;
            _musicTransitionTimer = new UpdateTimer(0f);
            _musicTransitionTimer.Pause();

            // Ambiance
            _ambianceAudioSource = _soundGameObject.AddComponent<AudioSource>();

            _ambianceAudioSource.outputAudioMixerGroup = _ambianceAudioMixerGroup;

            _ambianceAudioSource.playOnAwake = false;
            _ambianceAudioSource.loop = true;

            _ambianceAudioSource.volume = 1f;

            // Ambiance Transition
            _ambianceTransitionAudioSource = _soundGameObject.AddComponent<AudioSource>();

            _ambianceTransitionAudioSource.outputAudioMixerGroup = _ambianceAudioMixerGroup;

            _ambianceTransitionAudioSource.playOnAwake = false;
            _ambianceTransitionAudioSource.loop = true;

            _ambianceTransitionAudioSource.volume = 0f;

            // Ambiance Transitions
            _ambianceTransitions = new List<LongClipTransition>();
            _ambianceTransitioning = false;
            _ambianceTransitionTimer = new UpdateTimer(0f);
            _ambianceTransitionTimer.Pause();

            //Story
            _storyAudioSource = _soundGameObject.AddComponent<AudioSource>();

            _storyAudioSource.outputAudioMixerGroup = _sfxAudioMixerGroup;

            _storyAudioSource.playOnAwake = false;
            _storyAudioSource.loop = false;

            _storyAudioSource.volume = 1f;

            //Boat
            _boatAudioSource = _soundGameObject.AddComponent<AudioSource>();

            _boatAudioSource.outputAudioMixerGroup = _ambianceAudioMixerGroup;

            _boatAudioSource.playOnAwake = false;
            _boatAudioSource.loop = true;

            _boatAudioSource.volume = 1f;

            // SFX & Other One-Shots
            _sfxAudioSources = new List<AudioSource>();
        }

        private void UpdateTransitions(float deltaTime)
        {
            if (!_musicTransitioning && _musicTransitions.Count > 0)
            {
                _musicTransitioning = true;

                //_beeBraveStateMachine.CoRoutineHandler.StartCoroutine(
                //PlayBackgroundClipCR(
                //    _audioBank.Music.GetAudioClip(_musicTransitions[_musicTransitions.Count - 1].HookName),
                //    _musicAudioSource,
                //    _musicTransitionAudioSource,
                //    _musicTransitionTimer,
                //    _musicTransitions[_musicTransitions.Count - 1].CrossFadeTime,
                //    SwitchMusicSources
                //    )
                //    );

                _musicTransitions.Clear();
            }

            if (!_ambianceTransitioning && _ambianceTransitions.Count > 0)
            {
                _ambianceTransitioning = true;

                //_beeBraveStateMachine.CoRoutineHandler.StartCoroutine(
                //    PlayBackgroundClipCR(
                //        _audioBank.Ambiance.GetAudioClip(_ambianceTransitions[_ambianceTransitions.Count - 1].HookName),
                //        _ambianceAudioSource,
                //        _ambianceTransitionAudioSource,
                //        _ambianceTransitionTimer,
                //        _ambianceTransitions[_ambianceTransitions.Count - 1].CrossFadeTime,
                //        SwitchAmbianceSources
                //        )
                //        );

                _ambianceTransitions.Clear();
            }
        }

        #region Pooling
        private AudioSource GetFirstUnusedSFXAudioSource()
        {
            foreach (AudioSource audioSource in _sfxAudioSources)
            {
                if (!audioSource.gameObject.activeSelf) { return audioSource; }
            }

            return CreateNewSFXAudioSource();
        }

        private AudioSource CreateNewSFXAudioSource()
        {
            GameObject newAudioSourceGameObject = new GameObject(_sfxAudioSources.Count.ToString());
            newAudioSourceGameObject.SetActive(false);
            newAudioSourceGameObject.transform.SetParent(_soundGameObject.transform);

            AudioSource newAudioSource = newAudioSourceGameObject.AddComponent<AudioSource>();

            newAudioSource.outputAudioMixerGroup = _sfxAudioMixerGroup;

            newAudioSource.playOnAwake = false;

            newAudioSource.pitch = _timeScale;

            _sfxAudioSources.Add(newAudioSource);

            return newAudioSource;
        }

        private void UpdateSFXAudioSources(float deltaTime)
        {
            foreach (AudioSource audioSource in _sfxAudioSources)
            {
                if (audioSource.gameObject.activeSelf && !audioSource.isPlaying)
                {
                    audioSource.clip = null;
                    audioSource.gameObject.SetActive(false);
                }
            }
        }
        #endregion Pooling

        #region Audio Playing
        public void ChangeMusic(string hookName, float crossFadeTime = _defaultCrossFadeTime)
        {
            _musicTransitions.Add(new LongClipTransition() { HookName = hookName, CrossFadeTime = crossFadeTime });
        }

        public void ChangeAmbiance(string hookName, float crossFadeTime = _defaultCrossFadeTime)
        {
            _ambianceTransitions.Add(new LongClipTransition() { HookName = hookName, CrossFadeTime = crossFadeTime });
        }

        //private IEnumerator PlayBackgroundClipCR(AudioClip audioClip, AudioSource audioSource, AudioSource transitionAudioSource, UpdateTimer timer, float crossFadeTime, Action onTransitionEnd)
        //{
            //if (audioSource.clip != audioClip)
            //{
            //    timer.Reset(crossFadeTime);
            //    timer.Resume();

            //    transitionAudioSource.clip = audioClip;
            //    if (audioClip != null) { transitionAudioSource.Play(); }

            //    while (!timer.Update(Time.deltaTime))
            //    {
            //        float elapsedTimePercentage = timer.ElapsedTimePercentage;

            //        transitionAudioSource.volume = elapsedTimePercentage;
            //        audioSource.volume = 1f - elapsedTimePercentage;

            //        yield return new WaitForEndOfFrame();
            //    }

            //    audioSource.volume = 0f;
            //    transitionAudioSource.volume = 1f;

            //    audioSource.Stop();
            //    audioSource.clip = null;

            //    timer.Pause();
            //}

            //onTransitionEnd();
        //}

        private void SwitchAudiosources(ref AudioSource audioSource, ref AudioSource transitionAudioSource)
        {
            AudioSource aux = audioSource;
            audioSource = transitionAudioSource;
            transitionAudioSource = aux;
        }

        private void SwitchMusicSources()
        {
            SwitchAudiosources(ref _musicAudioSource, ref _musicTransitionAudioSource);
            _musicTransitioning = false;
        }

        private void SwitchAmbianceSources()
        {
            SwitchAudiosources(ref _ambianceAudioSource, ref _ambianceTransitionAudioSource);
            _ambianceTransitioning = false;
        }

        public string PlaySFX(string hookName, bool loop = false)
        {
            return PlaySFX(hookName, Vector3.zero, loop);
        }

        public string PlaySFX(string hookName, Vector3 worldPosition, bool loop)
        {
            //AudioClip audioClip = _audioBank.SFX.GetAudioClip(hookName);

            //if (audioClip != null)
            //{
            //    AudioSource audioSource = SetupSFXAudioSource(audioClip, Vector3.zero, loop);
            //    return audioSource.gameObject.name;
            //}

            return "";
        }

        public void StopSFX(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                AudioSource audioSource = _sfxAudioSources.Find(aSrc => aSrc.gameObject.name == id);
                if (audioSource != null) { audioSource.Stop(); }
            }
        }

        private AudioSource SetupSFXAudioSource(AudioClip audioClip, Vector3 worldPosition, bool loop)
        {
            AudioSource audioSource = GetFirstUnusedSFXAudioSource();

            audioSource.gameObject.SetActive(true);

            audioSource.transform.position = worldPosition;

            audioSource.clip = audioClip;
            audioSource.loop = loop;

            audioSource.Play();
            return audioSource;
        }
        #endregion Audio Playing

        #region Volume Control

        // MUSIC
        public void MuteMusic()
        {
            Music = false;

            MuteMusicMixer();

            PlayerPrefs.SetInt(_musicPrefKey, Convert.ToInt32(Music));
        }

        private void MuteMusicMixer()
        {
            _audioMixer.SetFloat(_musicVolumeMixerKey, _muteVolume);
        }

        public void UnmuteMusic()
        {
            Music = true;

            UnMuteMusicMixer();

            PlayerPrefs.SetInt(_musicPrefKey, Convert.ToInt32(Music));
        }

        private void UnMuteMusicMixer()
        {
            _audioMixer.SetFloat(_musicVolumeMixerKey, _musicVolumeBase);
        }

        // SFX
        public void MuteSFX()
        {
            SFX = false;

            MuteSFXMixer();

            PlayerPrefs.SetInt(_sfxPrefKey, Convert.ToInt32(SFX));
        }

        private void MuteSFXMixer()
        {
            _audioMixer.SetFloat(_ambianceVolumeMixerKey, _muteVolume);
            _audioMixer.SetFloat(_sfxVolumeMixerKey, _muteVolume);
        }

        public void UnmuteSFX()
        {
            SFX = true;

            UnmuteSFXMixer();

            PlayerPrefs.SetInt(_sfxPrefKey, Convert.ToInt32(SFX));
        }

        private void UnmuteSFXMixer()
        {
            _audioMixer.SetFloat(_ambianceVolumeMixerKey, _ambianceVolumeBase);
            _audioMixer.SetFloat(_sfxVolumeMixerKey, _sfxVolumeBase);
        }
        #endregion Volume Control



        internal void PlayStorySFX(string hookName)
        {
            //_storyAudioSource.clip = _audioBank.SFX.GetAudioClip(hookName);

            //_storyAudioSource.Play();
        }

        internal void StopStorySFX()
        {
            _storyAudioSource.Stop();
        }

        internal void PlayBoatSFX(string hookName)
        {
            //AudioClip newAudioClip = _audioBank.SFX.GetAudioClip(hookName);

            //if (newAudioClip != _boatAudioSource.clip) { _boatAudioSource.clip = newAudioClip; }
            //if (!_boatAudioSource.isPlaying) { _boatAudioSource.Play(); }
        }

        internal void StopBoatSFX()
        {
            _boatAudioSource.Stop();
        }


        // Eh...
        private float _nonNarrativeambianceVolume;

        internal void TransitionToNarrative()
        {
            _audioMixer.GetFloat(_ambianceVolumeMixerKey, out _nonNarrativeambianceVolume);
            _audioMixer.SetFloat(_ambianceVolumeMixerKey, _muteVolume);
        }

        internal void TransitionToMain()
        {
            _audioMixer.SetFloat(_ambianceVolumeMixerKey, _nonNarrativeambianceVolume);
        }



    }
}

namespace SorceressSpell.IdleCamp
{
    struct LongClipTransition
    {
        public string HookName { get; internal set; }
        public float CrossFadeTime { get; internal set; }
    }
}