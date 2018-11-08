namespace TobiiTracker.Helpers
{
    internal interface IStoppable
    {
        bool Stopped { get; }
        void Start();
        void Stop();
    }
}
