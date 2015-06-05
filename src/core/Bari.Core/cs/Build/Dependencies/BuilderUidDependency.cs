using Bari.Core.UI;

namespace Bari.Core.Build.Dependencies
{
    public class BuilderUidDependency : DependenciesBase
    {
         private readonly IBuilder builder;

        /// <summary>
        /// Creates the dependency on the given builder's UID
        /// </summary>
        /// <param name="builder">Builder to depend on</param>
        public BuilderUidDependency(IBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Creates fingerprint of the dependencies represented by this object, which can later be compared
        /// to other fingerprints.
        /// </summary>
        /// <returns>Returns the fingerprint of the dependent item's current state.</returns>
        protected override IDependencyFingerprint CreateFingerprint()
        {
            return new ObjectPropertiesFingerprint(builder, new[] { "Uid" });
        }

        public override void Dump(IUserOutput output)
        {
            output.Message("Builder UID {0}", builder.Uid);
        } 
    }
}