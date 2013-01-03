namespace Bari.Plugins.CodeContracts.Model
{
    /// <summary>
    /// Contract checking levels
    /// </summary>
    public enum ContractCheckingLevel
    {
        Full,
        PreAndPost,
        Preconditions,
        ReleaseRequires,
        None
    }
}