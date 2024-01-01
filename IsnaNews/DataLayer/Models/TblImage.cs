using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

public partial class TblImage
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    [InverseProperty("MainBaner")]
    public virtual ICollection<TblAdvertisement> TblAdvertisement { get; set; } = new List<TblAdvertisement>();

    [InverseProperty("MainImage")]
    public virtual ICollection<TblNews> TblNews { get; set; } = new List<TblNews>();

    [InverseProperty("Image")]
    public virtual ICollection<TblNewsImageRel> TblNewsImageRel { get; set; } = new List<TblNewsImageRel>();

    [InverseProperty("ProfileImage")]
    public virtual ICollection<TblUser> TblUser { get; set; } = new List<TblUser>();
}
