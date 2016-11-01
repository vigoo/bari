using System;
using Bari.Core.Model;

namespace Bari.Core.Build
{
    public class BuilderName
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BuilderName));
        
        private readonly Project relatedProject;
        private readonly string relativeName;
        
        public BuilderName(string globalName)
        {
            relatedProject = null;
            relativeName = globalName;
        }
        
        public BuilderName(Project relatedProject, string relativeName)
        {
            this.relatedProject = relatedProject;
            this.relativeName = relativeName;
        } 
        
        public bool MatchesProjectRelativeName(Project project, string relativeName)
        {
            log.DebugFormat("Match check {0}: {1}:{2}", AsFull, project, relativeName);
            return relatedProject == project && this.relativeName == relativeName;
        } 
        
        public string AsFull
        {
            get
            {
                if (relatedProject == null)                    
                    return relativeName;
                else
                    return String.Format("[{0}.{1}]:{2}", relatedProject.Module.Name, relatedProject.Name, relativeName);
            }
        }
    }
}