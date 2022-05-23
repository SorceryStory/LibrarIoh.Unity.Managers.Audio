using UnityEngine.Audio;

namespace SorceressSpell.LibrarIoh.Unity.Managers.Audio
{
    public class AudioMixerGroupController
    {
        #region Fields

        public readonly AudioMixerGroup AudioMixerGroup;
        public readonly string VolumeParameterName;
        private bool _muted;
        private float _volume;

        #endregion Fields

        #region Properties

        public bool Muted
        {
            set
            {
                _muted = value;
                AudioMixerGroup.audioMixer.SetVolume(VolumeParameterName, _muted ? 0f : _volume);
            }
            get
            {
                return _muted;
            }
        }

        public float Volume
        {
            set
            {
                _volume = value;
                if (!_muted)
                {
                    AudioMixerGroup.audioMixer.SetVolume(VolumeParameterName, _volume);
                }
            }
            get
            {
                return _volume;
            }
        }

        #endregion Properties

        #region Constructors

        public AudioMixerGroupController(AudioMixerGroup audioMixerGroup, string volumeParameterName)
        {
            AudioMixerGroup = audioMixerGroup;
            VolumeParameterName = volumeParameterName;

            _volume = AudioMixerGroup.audioMixer.GetVolume(VolumeParameterName);
            _muted = false;
        }

        #endregion Constructors
    }
}
