using System;
namespace AutoTest.Messages
{
	public class BuildRunMessage : IMessage
    {
        private BuildRunResults _results;

        public BuildRunResults Results { get { return _results; } }

        public BuildRunMessage(BuildRunResults results)
        {
            _results = results;
        }
    }
}
