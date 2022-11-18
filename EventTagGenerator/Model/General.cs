namespace EventTagGenerator.Model
{
    public class General
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Summary { get; set; }
        public string Footer { get; set; }
        public RoleType TRoleType { get; set; }
    }

    public enum RoleType
    {
        Attendee,
        Organizer,
        Speaker,
        Volunteer
    }
}