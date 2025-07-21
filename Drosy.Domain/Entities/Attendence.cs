using Drosy.Domain.Interfaces.Common;

namespace Drosy.Domain.Entities
{
    public class Attendence : ICreateAt
    {
        public int SessionId { get; set; }
        public int StudentId { get; set; }
        public string Status { get; set; } = null!;
        public string Note { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
      
        #region Nav Properties
        public Student Student { get; set; } = null!;
        public Session Session { get; set; } = null!;

        #endregion
    }

    public class Session
    {

    }
}