using Common.Extensions;

namespace Common.Exceptions.General;

public class InputHaveErrorsException : Exception
{
    public ModelStateErrorsCollection Errors;

    public InputHaveErrorsException(ModelStateErrorsCollection errors)
    {
        Errors = errors;
    }
}