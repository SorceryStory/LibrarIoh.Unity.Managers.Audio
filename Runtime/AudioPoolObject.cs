using SorceressSpell.LibrarIoh.Timers;
using SorceressSpell.LibrarIoh.Unity.Pools;
using UnityEngine;

namespace SorceressSpell.LibrarIoh.Unity.Managers.Audio
{
    public enum OnMute
    {
        DoNothing,
        Deactivate,
    }

    public class AudioPoolObject : GameObjectPoolObject
    {
        #region Enums

        private enum TimerAction
        {
            None,
            Mute,
            Unmute
        }

        #endregion Enums

        #region Fields

        public bool AffectedByTimescale;
        private AudioSource _audioSource;
        private OnMute _onMute;
        private UpdateTimer _timer;
        private TimerAction _timerAction;
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
            Mute(time, OnMute.DoNothing);
        }

        public void Mute(float time, OnMute onMute)
        {
            if (!_audioSource.mute && _timer.IsPaused)
            {
                _timer.Reset(time, true);
                _timerAction = TimerAction.Mute;

                _onMute = onMute;
            }
        }

        public void Unmute(float time)
        {
            if (_audioSource.mute && _timer.IsPaused)
            {
                _audioSource.mute = false;

                _timer.Reset(time, true);
                _timerAction = TimerAction.Unmute;
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

            _timer = new UpdateTimer(0f, true, TimerEndBehaviour.Pause);
            _timerAction = TimerAction.None;
        }

        protected override void GameObjectPoolObject_OnUpdate(float deltaTime)
        {
            if (!_audioSource.isPlaying)
            {
                Deactivate();
            }

            switch (_timerAction)
            {
                case TimerAction.Mute:
                    if (_timer.Update(deltaTime))
                    {
                        _audioSource.mute = true;
                        _audioSource.volume = 0f;

                        _timerAction = TimerAction.None;

                        switch (_onMute)
                        {
                            case OnMute.Deactivate:
                                Deactivate();
                                break;

                            case OnMute.DoNothing:
                            default:
                                break;
                        }
                    }
                    else
                    {
                        _audioSource.volume = 1 - _timer.ElapsedTimePercentage;
                    }
                    break;

                case TimerAction.Unmute:
                    if (_timer.Update(deltaTime))
                    {
                        _audioSource.volume = 1f;

                        _timerAction = TimerAction.None;
                    }
                    else
                    {
                        AudioSource.volume = _timer.ElapsedTimePercentage;
                    }
                    break;

                case TimerAction.None:
                default:
                    break;
            }
        }

        #endregion Methods
    }
}
