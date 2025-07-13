using System.Diagnostics.Metrics;

namespace Drosy.Domain.Entities
{
    public class Student : BaseEntity<int>
    {
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string? ThirdName { get; set; } = string.Empty!;
        public string LastName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string EmergencyNumber { get; set; } = null!;

        public int GradeId { get; set; }
        public int? UserId { get; set; } = default;
        public int CityId { get; set; }

        #region Nav Properties
        public AppUser? AppUser;
        public City City { get; set; } = null!;
       public Grade Grade { get; set; } = null!;
        public List<PlanStudent> Plans { get; set; } = new();

        #endregion
    }
}