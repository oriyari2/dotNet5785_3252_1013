namespace Dal;

internal static class Config
{
    internal const int StartCallId = 1;
    private static int s_nextCallId = StartCallId;
    internal static int NextCallId { get => s_nextCallId++; }

    internal const int StartAssignmentId = 1;
    private static int s_nextAssignmentId = StartAssignmentId;
    internal static int NextAssignmentId { get => s_nextAssignmentId++; }

    internal static DateTime Clock { get; set; } = DateTime.Now;

    internal static void Reset()
    {
        s_nextAssignmentId = StartAssignmentId;
        s_nextCallId = StartCallId;
        Clock = DateTime.Now;
        
    }
}


