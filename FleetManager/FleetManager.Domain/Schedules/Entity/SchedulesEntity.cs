namespace FleetManager.Domain.Schedules.Entity
{
    public class SchedulesEntity
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int WeekNum { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string DayOfWeek { get; set; } 
    }
}
