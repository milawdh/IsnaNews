using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

public partial class TblVideo
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public string VideoUrl { get; set; } = null!;

    [InverseProperty("Video")]
    public virtual ICollection<TblNewsVideoRel> TblNewsVideoRel { get; set; } = new List<TblNewsVideoRel>();
}
