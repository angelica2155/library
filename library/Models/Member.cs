namespace library.Models
{
    public class Member
    {
        public long MemberId { get; set; }         
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public decimal? MemberType { get; set; } = null;
    }
}
