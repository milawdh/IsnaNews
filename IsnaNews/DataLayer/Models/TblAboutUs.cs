using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

public partial class TblAboutUs
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public string Value { get; set; } = null!;
}
