namespace Application.Response
{
    public record ResponseBase<T, ResponseInfo>
    {
        public ResponseInfo? InfoError { get; set; }
        public T? Value { get; set; }
    }
}
