using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

[Index("Name", Name = "IX_TblCategory_Name")]
public partial class TblCategory
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [StringLength(32)]
    public string Name { get; set; } = null!;

    public int? ParentId { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<TblCategory> InverseParent { get; set; } = new List<TblCategory>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual TblCategory? Parent { get; set; }

    [InverseProperty("Category")]
    [JsonIgnore]
    public virtual ICollection<TblNews> TblNews { get; set; } = new List<TblNews>();
}
