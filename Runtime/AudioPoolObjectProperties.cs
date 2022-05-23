using SorceressSpell.LibrarIoh.Collections;
using SorceressSpell.LibrarIoh.Core;
using SorceressSpell.LibrarIoh.Unity.Pools;

namespace SorceressSpell.LibrarIoh.Unity.Managers.Audio
{
    public class AudioPoolObjectProperties : IPoolObjectProperties<AudioPoolObject>, ICopyFrom<AudioPoolObjectProperties>, ICopyFrom<AudioSourceProperties>
    {
        #region Fields

        public readonly AudioSourceProperties AudioSource;
        public readonly TransformProperties Transform;
        public bool AffectedByTimescale;

        #endregion Fields

        #region Constructors

        public AudioPoolObjectProperties()
        {
            Transform = new TransformProperties();
            AudioSource = new AudioSourceProperties();

            AffectedByTimescale = false;
        }

        #endregion Constructors

        #region Methods

        public void ApplyTo(AudioPoolObject poolObject)
        {
            Transform.ApplyTo(poolObject.Transform);
            AudioSource.ApplyTo(poolObject.AudioSource);
        }

        public void CopyFrom(AudioSourceProperties original)
        {
            Transform.ResetChanges();
            AudioSource.CopyFrom(original);

            AffectedByTimescale = false;
        }

        public void CopyFrom(AudioPoolObjectProperties original)
        {
            Transform.CopyFrom(original.Transform);
            AudioSource.CopyFrom(original.AudioSource);

            AffectedByTimescale = original.AffectedByTimescale;
        }

        public void ResetChanges()
        {
            Transform.ResetChanges();
            AudioSource.ResetChanges();
        }

        #endregion Methods
    }
}
