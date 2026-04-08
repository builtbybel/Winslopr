using System.Threading.Tasks;

namespace Winslop
{
    public abstract class FeatureBase
    {
        public abstract string ID();

        public abstract string GetFeatureDetails();

        public abstract Task<bool> CheckFeature();  // async

        public abstract Task<bool> DoFeature(); 

        public abstract bool UndoFeature();

        public virtual string HelpAnchorId()
        {
            return ID();
        }

        // Returns true if this feature is supported on the current OS/environment.
        public virtual bool IsApplicable()
        {
            return true;
        }

        // Optional UI/log hint when the feature is not applicable.
        public virtual string InapplicableReason()
        {
            return null;
        }
    }
}