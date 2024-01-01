using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

public partial class TblAdvertisement
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public long MainBanerId { get; set; }

    public string Link { get; set; } = null!;

    [ForeignKey("MainBanerId")]
    [InverseProperty("TblAdvertisement")]
    public virtual TblImage MainBaner { get; set; } = null!;
}
