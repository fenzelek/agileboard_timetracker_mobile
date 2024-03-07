namespace TimeTrackerXamarin._Domains.Projects.Tickets.Dto
{
    public class SuccessResponseDto<T>
    {
        public T data { get; set; }
        public Meta meta {get;set;}
    }
}
