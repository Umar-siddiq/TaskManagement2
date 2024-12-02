namespace TaskManagement.Models
{
    public class TaskItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
    }
}
