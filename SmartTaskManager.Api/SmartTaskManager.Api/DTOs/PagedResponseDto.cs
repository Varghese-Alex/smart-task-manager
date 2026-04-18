namespace SmartTaskManager.Api.DTOs
{
    public class PagedResponseDto<T>
    {
        public List<T> Data { get; set; } = new();

        public int TotalCount { get; set; }
    }
}
