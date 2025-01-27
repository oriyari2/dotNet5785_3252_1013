namespace BO;
#region BlException

// Exception for cases where an entity does not exist in the business logic.
[Serializable]
public class BlDoesNotExistException : Exception
{
    // Constructor that takes a message to describe the exception.
    public BlDoesNotExistException(string? message) : base(message) { }

    // Constructor that takes a message and an inner exception.
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

// Exception for cases where an entity already exists in the business logic.
[Serializable]
public class BlAlreadyExistsException : Exception
{
    // Constructor that takes a message to describe the exception.
    public BlAlreadyExistsException(string? message) : base(message) { }

    // Constructor that takes a message and an inner exception.
    public BlAlreadyExistsException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BLTemporaryNotAvailableException : Exception
{
    // Constructor that takes a message to describe the exception.
    public BLTemporaryNotAvailableException(string? message) : base(message) { }

    // Constructor that takes a message and an inner exception.
    public BLTemporaryNotAvailableException(string message, Exception innerException)
                : base(message, innerException) { }
}

// Exception for invalid values passed in business logic operations.
[Serializable]
public class BlInvalidValueExeption : Exception
{
    // Constructor that takes a message to describe the exception.
    public BlInvalidValueExeption(string? message) : base(message) { }

    // Constructor that takes a message and an inner exception.
    public BlInvalidValueExeption(string message, Exception innerException)
                : base(message, innerException) { }
}

#endregion 

#region DalEException

// Exception for when a user cannot update an item due to business logic rules.
[Serializable]
public class BlUserCantUpdateItemExeption : Exception
{
    // Constructor that takes a message to describe the exception.
    public BlUserCantUpdateItemExeption(string? message) : base(message) { }
}

// Exception for cases when an item cannot be deleted.
[Serializable]
public class BlcantDeleteItem : Exception
{
    // Constructor that takes a message to describe the exception.
    public BlcantDeleteItem(string? message) : base(message) { }
}

// Exception for incorrect values encountered in the data access layer.
[Serializable]
public class BlIncorrectValueException : Exception
{
    // Constructor that takes a message to describe the exception.
    public BlIncorrectValueException(string? message) : base(message) { }
}

// General exception for issues related to the internal system.
[Serializable]
public class BlOurSystemExeption : Exception
{
    // Constructor that takes a message to describe the exception.
    public BlOurSystemExeption(string? message) : base(message) { }
}

// Uncommented exception for null properties if needed in the future
//[Serializable]
//public class BlNullPropertyException : Exception
//{
//    // Constructor that takes a message to describe the exception.
//    public BlNullPropertyException(string? message) : base(message) { }
//}

#endregion

