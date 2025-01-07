namespace PL;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Collection for enumerating all <see cref="BO.CallType"/> values.
/// </summary>
internal class CallTypeCollection : IEnumerable
{
    /// <summary>
    /// Static field that holds all values of the <see cref="BO.CallType"/> enum.
    /// </summary>
    static readonly IEnumerable<BO.CallType> s_enums =
        (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

    /// <summary>
    /// Returns an enumerator for iterating over the <see cref="BO.CallType"/> values.
    /// </summary>
    /// <returns>An enumerator for the <see cref="BO.CallType"/> values.</returns>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class StatusCollection : IEnumerable
{
    /// <summary>
    /// Static field that holds all values of the <see cref="BO.Status"/> enum.
    /// </summary>
    static readonly IEnumerable<BO.Status> s_enums =
        (Enum.GetValues(typeof(BO.Status)) as IEnumerable<BO.Status>)!;

    /// <summary>
    /// Returns an enumerator for iterating over the <see cref="BO.Status"/> values.
    /// </summary>
    /// <returns>An enumerator for the <see cref="BO.Status"/> values.</returns>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// Collection for enumerating all <see cref="BO.RoleType"/> values.
/// </summary>
internal class RoleTypeCollection : IEnumerable
{
    /// <summary>
    /// Static field that holds all values of the <see cref="BO.RoleType"/> enum.
    /// </summary>
    static readonly IEnumerable<BO.RoleType> s_enums =
        (Enum.GetValues(typeof(BO.RoleType)) as IEnumerable<BO.RoleType>)!;

    /// <summary>
    /// Returns an enumerator for iterating over the <see cref="BO.RoleType"/> values.
    /// </summary>
    /// <returns>An enumerator for the <see cref="BO.RoleType"/> values.</returns>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// Collection for enumerating all <see cref="BO.DistanceType"/> values.
/// </summary>
internal class DistanceTypeCollection : IEnumerable
{
    /// <summary>
    /// Static field that holds all values of the <see cref="BO.DistanceType"/> enum.
    /// </summary>
    static readonly IEnumerable<BO.DistanceType> s_enums =
        (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;

    /// <summary>
    /// Returns an enumerator for iterating over the <see cref="BO.DistanceType"/> values.
    /// </summary>
    /// <returns>An enumerator for the <see cref="BO.DistanceType"/> values.</returns>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
