using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DamService.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreateTime { get; set; }
    }
}