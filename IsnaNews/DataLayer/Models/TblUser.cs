using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

[Index("Tell", Name = "IX_TblUser_Tell", IsUnique = true)]
[Index("UserName", Name = "IX_TblUser_UserName", IsUnique = true)]
public partial class TblUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [StringLength(16)]
    public string Name { get; set; } = null!;

    [StringLength(16)]
    public string Tell { get; set; } = null!;

    [StringLength(32)]
    public string UserName { get; set; } = null!;

    [StringLength(128)]
    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateSigned { get; set; }

    public long ProfileImageId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastLoginDate { get; set; }

    [ForeignKey("ProfileImageId")]
    [InverseProperty("TblUser")]
    public virtual TblImage ProfileImage { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("TblUser")]
    public virtual TblRole Role { get; set; } = null!;

    [InverseProperty("Reporter")]
    public virtual ICollection<TblNews> TblNews { get; set; } = new List<TblNews>();

    [InverseProperty("User")]
    public virtual ICollection<TblNewsComment> TblNewsComment { get; set; } = new List<TblNewsComment>();
}
