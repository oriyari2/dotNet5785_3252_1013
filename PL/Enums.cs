namespace PL;
using System.Collections;
using System.Collections.Generic;

// Collection for enumerating all CallType values
internal class CallTypeCollection : IEnumerable
{
    // Static field that holds all values of the CallType enum
    static readonly IEnumerable<BO.CallType> s_enums =
        (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

    // Returns an enumerator for iterating over the CallType values
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

// Collection for enumerating all RoleType values
internal class RoleTypeCollection : IEnumerable
{
    // Static field that holds all values of the RoleType enum
    static readonly IEnumerable<BO.RoleType> s_enums =
        (Enum.GetValues(typeof(BO.RoleType)) as IEnumerable<BO.RoleType>)!;

    // Returns an enumerator for iterating over the RoleType values
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

// Collection for enumerating all DistanceType values
internal class DistanceTypeCollection : IEnumerable
{
    // Static field that holds all values of the DistanceType enum
    static readonly IEnumerable<BO.DistanceType> s_enums =
        (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;

    // Returns an enumerator for iterating over the DistanceType values
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
