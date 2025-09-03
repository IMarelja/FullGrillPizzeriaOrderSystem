namespace GrillPizzeriaOrderWebApp.Models
{
    public record ApiOperationResult
    {
        public bool Succeeded { get; init; }
        public string? Message { get; init; }
        public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

        public static ApiOperationResult Ok(string? message = null)
            => new() { Succeeded = true, Message = message };

        public static ApiOperationResult Fail(params string[] errors)
            => new() { Succeeded = false, Errors = errors };
    }

    public record ApiOperationResult<T> : ApiOperationResult
    {
        public T? Data { get; init; }

        public static ApiOperationResult<T> Ok(T data, string? message = null)
            => new() { Succeeded = true, Data = data, Message = message };

        public new static ApiOperationResult<T> Fail(params string[] errors)
            => new() { Succeeded = false, Errors = errors };
    }

}
