using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

[Index("Name", Name = "IX_TblRolePermissions_Name", IsUnique = true)]
public partial class TblRolePermissions
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [StringLength(128)]
    public string Name { get; set; } = null!;

    public int? ParentId { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<TblRolePermissions> InverseParent { get; set; } = new List<TblRolePermissions>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual TblRolePermissions? Parent { get; set; }

    [InverseProperty("Permission")]
    public virtual ICollection<TblRoleRolePermissionsRel> TblRoleRolePermissionsRel { get; set; } = new List<TblRoleRolePermissionsRel>();
}
