using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

public partial class TblNewsComment
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(256)]
    public string Body { get; set; } = null!;

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DatePosted { get; set; }

    [StringLength(16)]
    public string Ip { get; set; } = null!;

    public long? ParentId { get; set; }

    public long PostId { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<TblNewsComment> InverseParent { get; set; } = new List<TblNewsComment>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual TblNewsComment? Parent { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("TblNewsComment")]
    public virtual TblNews Post { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("TblNewsComment")]
    public virtual TblUser User { get; set; } = null!;
}
