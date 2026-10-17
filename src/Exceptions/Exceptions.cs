
public class BrapiApiException : Exception
{
    public BrapiApiException(string message, Exception? innerException = null) 
        : base(message, innerException) { }
}

public class EmailSendException : Exception
{
    public EmailSendException(string message, Exception? innerException = null) 
        : base(message, innerException) { }
}