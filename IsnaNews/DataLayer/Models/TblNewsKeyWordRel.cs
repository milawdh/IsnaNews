using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

[Index("KeyWordId", "NewsId", Name = "IX_TblNewsKeyWordRel_KeyWordId-NewsId", IsUnique = true)]
public partial class TblNewsKeyWordRel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public long NewsId { get; set; }

    public int KeyWordId { get; set; }

    [ForeignKey("KeyWordId")]
    [InverseProperty("TblNewsKeyWordRel")]
    public virtual TblKeyWord KeyWord { get; set; } = null!;

    [ForeignKey("NewsId")]
    [InverseProperty("TblNewsKeyWordRel")]
    public virtual TblNews News { get; set; } = null!;
}
