using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

/// <summary>
/// 0 Comment Blog
/// 1 Add News 
/// 2 Edit News-Blogs-Comments
/// 3 Users
/// </summary>
[Index("Name", Name = "IX_TblRole_Name", IsUnique = true)]
public partial class TblRole
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [StringLength(32)]
    public string Name { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<TblRoleRolePermissionsRel> TblRoleRolePermissionsRel { get; set; } = new List<TblRoleRolePermissionsRel>();

    [InverseProperty("Role")]
    public virtual ICollection<TblUser> TblUser { get; set; } = new List<TblUser>();
}
