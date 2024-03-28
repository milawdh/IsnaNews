using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

public partial class TblNewsVideoRel
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long NewsId { get; set; }

    public long VideoId { get; set; }

    [ForeignKey("NewsId")]
    [InverseProperty("TblNewsVideoRel")]
    public virtual TblNews News { get; set; } = null!;

    [ForeignKey("VideoId")]
    [InverseProperty("TblNewsVideoRel")]
    public virtual TblVideo Video { get; set; } = null!;
}
