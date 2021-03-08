namespace Web.Data.SupportIdentity
{
    public interface ISupportIdentity<TUser> where TUser : AppUser
    {
        public int Id { get; set; }
        public TUser Owner { get; set; }
        public string OwnerId { get; set; }
    }
}