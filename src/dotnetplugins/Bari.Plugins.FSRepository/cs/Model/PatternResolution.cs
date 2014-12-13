using System;

namespace Bari.Plugins.FSRepository.Model
{
    public sealed class PatternResolution
    {
        private readonly string result;
        private readonly string failLog;

        public bool IsSuccesful
        {
            get { return result != null; }
        }

        public string Result
        {
            get { return result; }
        }

        public string FailLog
        {
            get { return failLog; }
        }

        private PatternResolution(string result, string failLog)
        {
            this.result = result;
            this.failLog = failLog;
        }

        public static PatternResolution Success(string path)
        {
            return new PatternResolution(path, String.Empty);
        }

        public static PatternResolution Failure(string log)
        {
            return new PatternResolution(null, log);
        }
    }
}