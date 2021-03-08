using System.Text.Json.Serialization;

namespace Web.Data.SupportIdentity
{
    public enum SupportIdentityState
    {
        Ready,
        Working
    }

    public class SupportIdentity : ISupportIdentity<AppUser>
    {
        public int Id { get; set; }
        [JsonIgnore] public AppUser Owner { get; set; }
        public string OwnerId { get; set; }
        public SupportIdentityState State { get; set; }
        [JsonIgnore] public SupportRequest? CurrentRequest { get; set; }
        public int? CurrentRequestId { get; set; }
    }
}