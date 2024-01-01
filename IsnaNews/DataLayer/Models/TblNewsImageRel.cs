using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

public partial class TblNewsImageRel
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long NewsId { get; set; }

    public long ImageId { get; set; }

    [ForeignKey("ImageId")]
    [InverseProperty("TblNewsImageRel")]
    public virtual TblImage Image { get; set; } = null!;

    [ForeignKey("NewsId")]
    [InverseProperty("TblNewsImageRel")]
    public virtual TblNews News { get; set; } = null!;
}
