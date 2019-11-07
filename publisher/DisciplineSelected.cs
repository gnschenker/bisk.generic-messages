using System;

namespace publisher
{
    public class DisciplineSelected
    {
        public Guid ApplicationId { get; set; }
        public Guid DisciplineId { get; set; }

        public override string ToString()
        {
            return $"ApplicationId: {ApplicationId}, DisciplineId: {DisciplineId}";
        } 
    }
}
