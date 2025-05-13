using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Dtos
{
    public class GiftDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public GiftDataDto Data { get; set; }
    }
    public class GiftDataDto
    {
        public string Color { get; set; }
        public string Capacity { get; set; }
       
    }
}
