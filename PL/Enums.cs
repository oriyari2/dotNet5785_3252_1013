namespace PL;
using System.Collections;
using System.Collections.Generic;

internal class CallTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums =
    (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class RoleTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.RoleType> s_enums =
    (Enum.GetValues(typeof(BO.RoleType)) as IEnumerable<BO.RoleType>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class DistanceTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.DistanceType> s_enums =
    (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

