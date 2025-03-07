namespace Timesheet.Core;

public class Result<T>
{
    public T? Value { get; }
    public bool IsSuccess => Code == ResultValues.Success;
    public ResultValues Code { get; }

    public Result(T? value)
    {
        Value = value;
        Code = ResultValues.Success;
    }

    private Result(ResultValues code)
    {
        Code = code;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(ResultValues code) => new(code);
}

public enum ResultValues
{
    Success = 0,
    TaskNotFound = 1,
    ProjectNotFound = 2,
    TaskCodeNotFound = 3,
    InvalidInput = 4,
    UnauthorizedAccess = 5,
    DatabaseError = 6,
    ValidationError = 7,
}
