using SorceressSpell.LibrarIoh.Math;
using SorceressSpell.LibrarIoh.Timers;
using SorceressSpell.LibrarIoh.Unity.Pools;
using UnityEngine;

namespace SorceressSpell.LibrarIoh.Unity.Managers.Audio
{
    public class AudioPoolObject : GameObjectPoolObject
    {
        #region Fields

        public bool AffectedByTimescale;
        private AudioSource _audioSource;
        private bool _timerMuteEnd;
        private UpdateTimer _timerVolume;
        private float _timerVolumeEnd;
        private VolumeTimerEndBehaviour _timerVolumeEndBehaviour;
        private float _timerVolumeStart;
        private Transform _transform;

        #endregion Fields

        #region Properties

        public AudioSource AudioSource
        {
            get { return _audioSource; }
        }

        public Transform Transform
        {
            get { return _transform; }
        }

        #endregion Properties

        #region Methods

        public void Mute(float time)
        {
            Mute(time, VolumeTimerEndBehaviour.DoNothing);
        }

        public void Mute(float time, VolumeTimerEndBehaviour endBehaviour)
        {
            if (!_audioSource.mute)
            {
                SetVolumeOverTime(_audioSource.volume, 0f, time, true, endBehaviour);
            }
        }

        public void Pause()
        {
            _audioSource.Pause();
        }

        public void Play()
        {
            _audioSource.Play();
        }

        public void Play(float startingTime)
        {
            _audioSource.time = startingTime;
            _audioSource.Play();
        }

        public void SetVolumeOverTime(float volumeStart, float volumeEnd, float time)
        {
            SetVolumeOverTime(volumeStart, volumeEnd, time, false, VolumeTimerEndBehaviour.DoNothing);
        }

        public void SetVolumeOverTime(float volumeStart, float volumeEnd, float time, bool muteEnd, VolumeTimerEndBehaviour endBehaviour)
        {
            _timerMuteEnd = muteEnd;

            _timerVolumeStart = volumeStart;
            _timerVolumeEnd = volumeEnd;

            _timerVolume.Reset(time, true);

            _timerVolumeEndBehaviour = endBehaviour;
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void Unmute(float time)
        {
            Unmute(1f, time);
        }

        public void Unmute(float volumeEnd, float time)
        {
            if (_audioSource.mute)
            {
                _audioSource.mute = false;

                SetVolumeOverTime(0f, volumeEnd, time, false, VolumeTimerEndBehaviour.DoNothing);
            }
        }

        protected override void GameObjectPoolObject_OnActivate()
        {
            _audioSource.Play();
        }

        protected override void GameObjectPoolObject_OnDeactivate()
        {
            _audioSource.Stop();
        }

        protected override void GameObjectPoolObject_OnInstantiation()
        {
            _transform = GameObject.transform;

            if (!GameObject.TryGetComponent<AudioSource>(out _audioSource))
            {
                _audioSource = GameObject.AddComponent<AudioSource>();
            }

            _timerVolume = new UpdateTimer(0f, true, TimerEndBehaviour.Pause);
        }

        protected override void GameObjectPoolObject_OnUpdate(float deltaTime)
        {
            if (!_audioSource.isPlaying)
            {
                Deactivate();
            }

            if (_timerVolume.Update(deltaTime))
            {
                _audioSource.mute = _timerMuteEnd;
                _audioSource.volume = _timerVolumeEnd;

                switch (_timerVolumeEndBehaviour)
                {
                    case VolumeTimerEndBehaviour.Pause:
                        Pause();
                        break;

                    case VolumeTimerEndBehaviour.Deactivate:
                        Deactivate();
                        break;

                    case VolumeTimerEndBehaviour.DoNothing:
                    default:
                        break;
                }
            }
            else
            {
                _audioSource.volume = MathOperations.LerpClamp(_timerVolumeStart, _timerVolumeEnd, _timerVolume.ElapsedTimePercentage);
            }
        }

        #endregion Methods
    }
}
