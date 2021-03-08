using System;
using System.Text.Json.Serialization;

namespace Web.Data
{
    public enum SupportRequestState
    {
        InQueue = 0,
        InProcessing,
        IsDone,
        IsDeclined
    }

    public class SupportRequest
    {
        public int Id { get; set; }
        public string Head { get; set; }
        public string Text { get; set; }
        public SupportRequestState State { get; set; }
        public DateTime Created { get; set; }
        public DateTime DoneTime { get; set; }
        [JsonIgnore] public AppUser? User { get; set; }
        public string UserId { get; set; }
        [JsonIgnore] public AppUser? Support { get; set; }
        public string SupportId { get; set; }

        public override bool Equals(object obj)
        {
            return obj as SupportRequest != null;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(SupportRequest x, SupportRequest y)
        {
            return x?.Id == y?.Id;
        }

        public static bool operator !=(SupportRequest x, SupportRequest y)
        {
            return x?.Id != y?.Id;
        }
    }
}