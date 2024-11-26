namespace Dal;

/// <summary>
///Static class serving as a data source.
/// </summary>
internal static class DataSource
{
    internal static List<DO.Assignment> Assignments { get; } = new(); // Static list to store Assignment objects

    internal static List<DO.Call> Calls { get; } = new(); // Static list to store Call objects

    internal static List<DO.Volunteer> Volunteers { get; } = new(); // Static list to store Volunteer objects
}

