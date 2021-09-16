using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asp_Sample.Areas.Admin.Models
{
    public class VoteTimeDto
    {
        public Guid VoteId { get; set; }
        public DateTime? VoteTime { get; set; }
    }
}
