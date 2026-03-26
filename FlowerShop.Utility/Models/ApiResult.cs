namespace FlowerShop.Utility;

public class ApiResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public ApiResult() { }
    public ApiResult(T data, string message = "Success")
    {
        Success = true;
        Message = message;
        Data = data;
    }

    public ApiResult(string message, List<string>? errors = null)
    {
        Success = false;
        Message = message;
        Errors = errors;
    }
}