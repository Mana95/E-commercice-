namespace DevFlow.Api.Services;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException() : base("Email is already registered.")
    {
    }
}

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Invalid email or password.")
    {
    }
}

public class InvalidRefreshTokenException : Exception
{
    public InvalidRefreshTokenException() : base("Refresh token is invalid or expired.")
    {
    }
}

public class WeakPasswordException : Exception
{
    public WeakPasswordException(string message) : base(message)
    {
    }
}
