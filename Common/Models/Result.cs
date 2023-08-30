namespace Common.Models;

public struct Result<T>
{
    public readonly ResultStatusCode ResultStatus;
    public readonly T Value;

    public Result(ResultStatusCode resultStatus, T value)
    {
        ResultStatus = resultStatus;
        Value = value;
    }
    
    public Result(ResultStatusCode resultStatus)
    {
        ResultStatus = resultStatus;
        Value = default;
    }

    public bool IsSuccess() => ResultStatus == ResultStatusCode.Success;
    public bool IsFailure() => ResultStatus == ResultStatusCode.Failure;
}

public enum ResultStatusCode
{
    Success,
    Failure
}