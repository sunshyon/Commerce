using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Domain.DbModels
{
    public partial class TbSpecGroup
    {
        public long Id { get; set; }
        public long Cid { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public List<TbSpecParam> Params = new List<TbSpecParam>();
    }
}
