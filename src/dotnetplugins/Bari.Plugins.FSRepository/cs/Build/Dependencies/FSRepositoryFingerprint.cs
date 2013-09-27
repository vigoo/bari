using System;
using System.Diagnostics.Contracts;
using System.IO;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies.Protocol;
using Bari.Plugins.FSRepository.Build.Dependencies.Protocol;
using Bari.Plugins.FSRepository.Model;

namespace Bari.Plugins.FSRepository.Build.Dependencies
{
    public class FSRepositoryFingerprint: IDependencyFingerprint, IEquatable<FSRepositoryFingerprint>
    {
        private readonly string path;
        private readonly DateTime lastModifiedDate;
        private readonly long lastSize;

        public FSRepositoryFingerprint(IFileSystemRepositoryAccess repository, string path)
        {
            Contract.Requires(repository != null);
            Contract.Requires(path != null);

            this.path = path;

            var dir = repository.GetDirectory(path);
            var fileName = Path.GetFileName(path);
            lastModifiedDate = dir.GetLastModifiedDate(fileName);
            lastSize = dir.GetFileSize(fileName);
        }

        public FSRepositoryFingerprint(IProtocolSerializer serializer, Stream sourceStream)
            : this(serializer.Deserialize<FSRepositoryFingerprintProtocol>(sourceStream))
        {
            Contract.Requires(serializer != null);
            Contract.Requires(sourceStream != null);
        }
        
        public FSRepositoryFingerprint(FSRepositoryFingerprintProtocol proto)
        {
            path = proto.Path;
            lastModifiedDate = proto.LastModified;
            lastSize = proto.LastSize;
        }

        public void Save(IProtocolSerializer serializer, Stream targetStream)
        {
            serializer.Serialize(targetStream, Protocol);
        }

        public IDependencyFingerprintProtocol Protocol
        {
            get
            {
                var proto = new FSRepositoryFingerprintProtocol
                    {
                        Path = path,
                        LastModified = lastModifiedDate,
                        LastSize = lastSize
                    };

                return proto;
            }
        }

        public bool Equals(FSRepositoryFingerprint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(path, other.path) && lastModifiedDate.Equals(other.lastModifiedDate) && lastSize == other.lastSize;
        }

        public bool Equals(IDependencyFingerprint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != this.GetType()) return false;
            return Equals((FSRepositoryFingerprint)other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FSRepositoryFingerprint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = path.GetHashCode();
                hashCode = (hashCode*397) ^ lastModifiedDate.GetHashCode();
                hashCode = (hashCode*397) ^ lastSize.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(FSRepositoryFingerprint left, FSRepositoryFingerprint right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FSRepositoryFingerprint left, FSRepositoryFingerprint right)
        {
            return !Equals(left, right);
        }
    }
}