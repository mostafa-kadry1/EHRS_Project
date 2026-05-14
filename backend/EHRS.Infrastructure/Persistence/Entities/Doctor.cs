using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Persistence.Entities;

[Table("Doctor")]
[Index("Area", Name = "IX_Doctor_Area")]
public partial class Doctor
{
    [Key]
    public int DoctorId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(10)]
    public string? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    [StringLength(120)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? ContactNumber { get; set; }

    [StringLength(100)]
    public string? Specialization { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Salary { get; set; }

    [StringLength(300)]
    public string? ProfilePicture { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [Precision(0)]
    public DateTime? UpdatedAt { get; set; }

    [StringLength(500)]
    public string? Certificates { get; set; }

    [StringLength(200)]
    public string? AffiliatedHospital { get; set; }

    [StringLength(1000)]
    public string? About { get; set; }

    [StringLength(50)]
    public string MedicalLicense { get; set; } = null!;

    [StringLength(100)]
    public string? Area { get; set; }

    public byte ApprovalStatus { get; set; }

    [InverseProperty("Doctor")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Doctor")]
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    [InverseProperty("Doctor")]
    public virtual ICollection<SurgeryHistory> SurgeryHistories { get; set; } = new List<SurgeryHistory>();

    [InverseProperty("Doctor")]
    public virtual UserCredential? UserCredential { get; set; }
}
