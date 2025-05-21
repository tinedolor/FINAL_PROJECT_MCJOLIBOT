namespace Helpdesk.Api.Models
{
    public static class TicketStatus
    {
        public const string New = "New";
        public const string Open = "Open";
        public const string InProgress = "In Progress";
        public const string Resolved = "Resolved";
        public const string Closed = "Closed";

        public static readonly string[] ValidStatuses = new[]
        {
            New,
            Open,
            InProgress,
            Resolved,
            Closed
        };

        public static bool IsValid(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;

            return ValidStatuses.Contains(status);
        }
    }
}