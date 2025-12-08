namespace SoruCevapPortal.Models;

public class QuestionVote : BaseEntity
{
    public int QuestionId { get; set; }
    public int UserId { get; set; }
    public bool IsUpVote { get; set; } = true; // true = upvote, false = downvote
    
    // Navigation properties
    public virtual Question Question { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}

