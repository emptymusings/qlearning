namespace QLearning.Core.Environment
{
    public interface IRLEnvironment
    {
        int NumberOfStates { get; set; }
        int NumberOfActions { get; set; }
        bool IsValidState(int state);

    }
}
