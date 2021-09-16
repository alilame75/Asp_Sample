using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asp_Sample.Areas.Account.Models
{
    public class VoteAnswerDto
    {
        public string VoteId { get; set; }
        public List<string> AnswerIds { get; set; }
    }
}
