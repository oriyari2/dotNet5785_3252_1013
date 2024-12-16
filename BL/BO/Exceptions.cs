namespace BO;

#region BlException
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlInvalidValueExeption : Exception
{
    public BlInvalidValueExeption(string? message) : base(message) { }
    public BlInvalidValueExeption(string message, Exception innerException)
                : base(message, innerException) { }
}

#endregion 

#region DalEException
[Serializable]
public class BlUserCantUpdateItemExeption : Exception
{
    public BlUserCantUpdateItemExeption(string? message) : base(message) { }
}


[Serializable]
public class BlcantDeleteItem : Exception
{
    public BlcantDeleteItem(string? message) : base(message) { }
}



[Serializable]
public class BlIncorrectValueException : Exception
{
    public BlIncorrectValueException(string? message) : base(message) { }
}

[Serializable]
public class BlOurSystemExeption : Exception
{
    public BlOurSystemExeption(string? message) : base(message) { }
}

//[Serializable]
//public class BlNullPropertyException : Exception
//{
//    public BlNullPropertyException(string? message) : base(message) { }
//}

#endregion



