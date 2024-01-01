using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

[Table("TblRole-RolePermissionsRel")]
[Index("PermissionId", "RoleId", Name = "IX_TblRole-RolePermissionsRel_PermissionId-RoleId", IsUnique = true)]
public partial class TblRoleRolePermissionsRel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    [ForeignKey("PermissionId")]
    [InverseProperty("TblRoleRolePermissionsRel")]
    public virtual TblRolePermissions Permission { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("TblRoleRolePermissionsRel")]
    public virtual TblRole Role { get; set; } = null!;
}
