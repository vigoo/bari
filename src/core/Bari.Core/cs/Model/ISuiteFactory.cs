using System.Collections.Generic;

namespace Bari.Core.Model
{
    public interface ISuiteFactory
    {
        Suite CreateSuite(ISet<Goal> goals);
    }
}