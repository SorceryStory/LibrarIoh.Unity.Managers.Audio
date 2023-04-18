using SorceressSpell.LibrarIoh.Timers;
using SorceressSpell.LibrarIoh.Unity.Pools;
using UnityEngine;
using UnityEngine.Audio;

namespace SorceressSpell.LibrarIoh.Unity.Managers.Audio
{
    public class BackgroundAudio
    {
        #region Fields

        private AudioSource _auxSource;
        private AudioSource _mainSource;
        private readonly UpdateTimer _transitionTimer;

        #endregion Fields

        #region Constructors

        public BackgroundAudio(GameObject gameObject, AudioClip audioClip, AudioMixerGroup audioMixerGroup)
        {
            _mainSource = gameObject.AddComponent<AudioSource>();

            _mainSource.clip = audioClip;
            _mainSource.outputAudioMixerGroup = audioMixerGroup;
            _mainSource.loop = true;
            _mainSource.volume = 1f;

            _mainSource.Play();


            _auxSource = gameObject.AddComponent<AudioSource>();

            _auxSource.clip = null;
            _auxSource.outputAudioMixerGroup = audioMixerGroup;
            _auxSource.loop = true;
            _mainSource.volume = 0f;


            _transitionTimer = new UpdateTimer(0f, true, TimerEndBehaviour.Pause);
        }

        #endregion Constructors

        #region Methods

        public void ApplyProperties(AudioSourceProperties audioSourceProperties)
        {
            audioSourceProperties.ApplyTo(_mainSource);
            audioSourceProperties.ApplyTo(_auxSource);
        }

        public void ChangeAudioClip(AudioClip audioClip, float crossFadeTime)
        {
            if (_transitionTimer.IsPaused)
            {
                _auxSource.clip = audioClip;
                _auxSource.volume = 0f;

                _auxSource.Play();

                _transitionTimer.Reset(crossFadeTime, true);
            }
        }

        public void UnMute(float unmuteTime)
        {
            if (!_transitionTimer.IsPaused && _mainSource.volume == 0f)
            {
            }
        }

        public void Update(float deltaTime)
        {
            if (!_transitionTimer.IsPaused)
            {
                if (_transitionTimer.Update(deltaTime))
                {
                    _mainSource.volume = 0f;
                    _auxSource.volume = 1f;

                    AudioSource aux = _mainSource;
                    _mainSource = _auxSource;
                    _auxSource = aux;
                }
                else
                {
                    _mainSource.volume = 1f - _transitionTimer.ElapsedTimePercentage;
                    _auxSource.volume = _transitionTimer.ElapsedTimePercentage;
                }
            }
        }

        #endregion Methods
    }
}
