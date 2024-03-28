using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

[Index("Title", Name = "IX_TblNews_Title", IsUnique = true)]
public partial class TblNews
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(128)]
    public string Title { get; set; } = null!;

    public int ReporterId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DatePosted { get; set; }

    public string Body { get; set; } = null!;

    public int CategoryId { get; set; }

    public long MainImageId { get; set; }

    /// <summary>
    /// If it is 1 Will show on Index Page Top carousel
    /// 
    /// </summary>
    public bool IsImportantNews { get; set; }

    public long ViewCount { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("TblNews")]
    public virtual TblCategory Category { get; set; } = null!;

    [ForeignKey("MainImageId")]
    [InverseProperty("TblNews")]
    public virtual TblImage MainImage { get; set; } = null!;

    [ForeignKey("ReporterId")]
    [InverseProperty("TblNews")]
    public virtual TblUser Reporter { get; set; } = null!;

    [InverseProperty("Post")]
    public virtual ICollection<TblNewsComment> TblNewsComment { get; set; } = new List<TblNewsComment>();

    [InverseProperty("News")]
    public virtual ICollection<TblNewsImageRel> TblNewsImageRel { get; set; } = new List<TblNewsImageRel>();

    [InverseProperty("News")]
    public virtual ICollection<TblNewsKeyWordRel> TblNewsKeyWordRel { get; set; } = new List<TblNewsKeyWordRel>();

    [InverseProperty("News")]
    public virtual ICollection<TblNewsVideoRel> TblNewsVideoRel { get; set; } = new List<TblNewsVideoRel>();
}
