using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

[Index("Body", Name = "IX_TblKeyWord_Body", IsUnique = true)]
public partial class TblKeyWord
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [StringLength(16)]
    public string Body { get; set; } = null!;

    [InverseProperty("KeyWord")]
    public virtual ICollection<TblNewsKeyWordRel> TblNewsKeyWordRel { get; set; } = new List<TblNewsKeyWordRel>();
}
