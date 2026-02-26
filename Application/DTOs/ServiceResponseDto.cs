namespace fenixjobs_api.Application.DTOs
{
    public class ServiceResponseDto<T>
    {
        public bool Status { get; set; } = true;

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }
    }
}
