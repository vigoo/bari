namespace Bari.Core.Model
{
    /// <summary>
    /// Project types
    /// </summary>
    public enum ProjectType
    {
        /// <summary>
        /// Project is built to a Windows GUI executable file
        /// </summary>
        WindowsExecutable,

        /// <summary>
        /// Project is built to an exectuable file
        /// </summary>
        Executable,

        /// <summary>
        /// Project is built to a DLL
        /// </summary>
        Library,

        /// <summary>
        /// Project is a static library
        /// </summary>
        StaticLibrary
    }
}