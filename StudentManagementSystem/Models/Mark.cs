namespace StudentManagementSystem.Models
{
    public class Mark
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;  // "Internal" or "External"
        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }

        // Which student this mark belongs to
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        // Computed percentage
        public double Percentage => MaxScore > 0
            ? Math.Round((double)(Score / MaxScore) * 100, 2)
            : 0;
    }
}
