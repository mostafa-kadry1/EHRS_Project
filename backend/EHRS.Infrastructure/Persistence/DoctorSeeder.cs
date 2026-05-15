using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Persistence;

/// <summary>
/// Seeds 459 doctors (27 governorates × 17 specialties, 1 doctor each)
/// with matching UserCredentials.
/// Safe to run multiple times — skips rows that already exist.
/// Default login password for all seeded doctors: EhrsDoctor@2024
/// </summary>
public static class DoctorSeeder
{
    private const string DefaultPassword = "EhrsDoctor@2024";

    public static async Task SeedAsync(EHRSContext db)
    {
        // Fast-exit: if we already have 459+ doctors seeded, do nothing.
        var existingCount = await db.Doctors
            .CountAsync(d => d.MedicalLicense.StartsWith("EG-") && d.MedicalLicense.EndsWith("-MD"));
        if (existingCount >= 459)
            return;

        var hasher = new PasswordHasher<string>();
        var passwordHash = hasher.HashPassword("Doctor", DefaultPassword);
        var now = DateTime.UtcNow;

        var seedData = BuildSeedData();

        foreach (var (doctorData, licenseNo) in seedData)
        {
            // Skip if this specific license already exists
            if (await db.Doctors.AnyAsync(d => d.MedicalLicense == licenseNo))
                continue;

            // Skip if email already taken
            if (!string.IsNullOrEmpty(doctorData.Email) &&
                await db.Doctors.AnyAsync(d => d.Email == doctorData.Email))
                continue;

            doctorData.CreatedAt = now;
            doctorData.ApprovalStatus = 1; // Approved — seeded doctors are immediately available

            db.Doctors.Add(doctorData);
            await db.SaveChangesAsync();

            var cred = new UserCredential
            {
                Role = "Doctor",
                DoctorId = doctorData.DoctorId,
                PasswordHash = passwordHash,
                CreatedAt = now,
            };
            db.UserCredentials.Add(cred);
            await db.SaveChangesAsync();
        }
    }

    private static List<(Doctor Doctor, string License)> BuildSeedData()
    {
        var list = new List<(Doctor, string)>();

        list.Add((new Doctor
        {
            FullName         = "Dr. Youssef Ibrahim",
            Gender           = "Male",
            Email            = "dr.youssef.ibrahim.cairo.gp@ehrs-clinic.eg",
            ContactNumber    = "01110000001",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00001-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Royal Medical Center",
            About            = "Experienced General Practitioner (GP) based in Cairo governorate with over 12 years of clinical practice.",
        }, "EG-00001-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mostafa Farouk",
            Gender           = "Male",
            Email            = "dr.mostafa.farouk.cairo.peds@ehrs-clinic.eg",
            ContactNumber    = "01210000002",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00002-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Nile Medical Center",
            About            = "Experienced Pediatrics Specialist based in Cairo governorate with over 22 years of clinical practice.",
        }, "EG-00002-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rana Selim",
            Gender           = "Female",
            Email            = "dr.rana.selim.cairo.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01510000003",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00003-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Ain Shams University Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Cairo governorate with over 6 years of clinical practice.",
        }, "EG-00003-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mohamed Ahmed",
            Gender           = "Male",
            Email            = "dr.mohamed.ahmed.cairo.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01010000004",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00004-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Al-Zahraa Hospital",
            About            = "Experienced General Surgery Specialist based in Cairo governorate with over 12 years of clinical practice.",
        }, "EG-00004-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Doaa Abdel-Rahman",
            Gender           = "Female",
            Email            = "dr.doaa.abdelrahman.cairo.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01110000005",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00005-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo University Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Cairo governorate with over 22 years of clinical practice.",
        }, "EG-00005-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Heba Ghoneim",
            Gender           = "Female",
            Email            = "dr.heba.ghoneim.cairo.cardio@ehrs-clinic.eg",
            ContactNumber    = "01210000006",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00006-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Ain Shams University Hospital",
            About            = "Experienced Cardiology Specialist based in Cairo governorate with over 12 years of clinical practice.",
        }, "EG-00006-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ashraf Selim",
            Gender           = "Male",
            Email            = "dr.ashraf.selim.cairo.neuro@ehrs-clinic.eg",
            ContactNumber    = "01510000007",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00007-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Royal Medical Center",
            About            = "Experienced Neurology Specialist based in Cairo governorate with over 5 years of clinical practice.",
        }, "EG-00007-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sherif Hamdy",
            Gender           = "Male",
            Email            = "dr.sherif.hamdy.cairo.vasc@ehrs-clinic.eg",
            ContactNumber    = "01010000008",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00008-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Dar Al-Shifa Hospital",
            About            = "Experienced Vascular Specialist based in Cairo governorate with over 13 years of clinical practice.",
        }, "EG-00008-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sara Ramadan",
            Gender           = "Female",
            Email            = "dr.sara.ramadan.cairo.pulm@ehrs-clinic.eg",
            ContactNumber    = "01110000009",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00009-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Dar Al-Shifa Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Cairo governorate with over 8 years of clinical practice.",
        }, "EG-00009-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Fawzy",
            Gender           = "Male",
            Email            = "dr.hassan.fawzy.cairo.ortho@ehrs-clinic.eg",
            ContactNumber    = "01210000010",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00010-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Nile Medical Center",
            About            = "Experienced Orthopedic Specialist based in Cairo governorate with over 16 years of clinical practice.",
        }, "EG-00010-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ehab Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.ehab.abdelrahman.cairo.derm@ehrs-clinic.eg",
            ContactNumber    = "01510000011",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00011-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Royal Medical Center",
            About            = "Experienced Dermatology Specialist based in Cairo governorate with over 6 years of clinical practice.",
        }, "EG-00011-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Halim",
            Gender           = "Female",
            Email            = "dr.reham.halim.cairo.intmed@ehrs-clinic.eg",
            ContactNumber    = "01010000012",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00012-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Nile Medical Center",
            About            = "Experienced Internal Medicine Specialist based in Cairo governorate with over 17 years of clinical practice.",
        }, "EG-00012-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Barakat",
            Gender           = "Male",
            Email            = "dr.hassan.barakat.cairo.dent@ehrs-clinic.eg",
            ContactNumber    = "01110000013",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00013-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Al-Maadi Hospital",
            About            = "Experienced Dentist based in Cairo governorate with over 25 years of clinical practice.",
        }, "EG-00013-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Metwally",
            Gender           = "Male",
            Email            = "dr.karim.metwally.cairo.ent@ehrs-clinic.eg",
            ContactNumber    = "01210000014",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00014-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Al-Zahraa Hospital",
            About            = "Experienced ENT Specialist based in Cairo governorate with over 7 years of clinical practice.",
        }, "EG-00014-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mariam Shalaby",
            Gender           = "Female",
            Email            = "dr.mariam.shalaby.cairo.uro@ehrs-clinic.eg",
            ContactNumber    = "01510000015",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00015-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Al-Maadi Hospital",
            About            = "Experienced Urology Specialist based in Cairo governorate with over 7 years of clinical practice.",
        }, "EG-00015-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mostafa Omar",
            Gender           = "Male",
            Email            = "dr.mostafa.omar.cairo.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01010000016",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00016-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Cleopatra Hospital",
            About            = "Experienced Ophthalmology Specialist based in Cairo governorate with over 13 years of clinical practice.",
        }, "EG-00016-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bassem Fouad",
            Gender           = "Male",
            Email            = "dr.bassem.fouad.cairo.rheum@ehrs-clinic.eg",
            ContactNumber    = "01110000017",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00017-MD",
            Area             = "Cairo",
            AffiliatedHospital = "Cairo Al-Ahrar Hospital",
            About            = "Experienced Rheumatology Specialist based in Cairo governorate with over 16 years of clinical practice.",
        }, "EG-00017-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hana Ramadan",
            Gender           = "Female",
            Email            = "dr.hana.ramadan.giza.gp@ehrs-clinic.eg",
            ContactNumber    = "01210000018",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00018-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Royal Medical Center",
            About            = "Experienced General Practitioner (GP) based in Giza governorate with over 25 years of clinical practice.",
        }, "EG-00018-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Khaled Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.khaled.abdelrahman.giza.peds@ehrs-clinic.eg",
            ContactNumber    = "01510000019",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00019-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Al-Ahrar Hospital",
            About            = "Experienced Pediatrics Specialist based in Giza governorate with over 22 years of clinical practice.",
        }, "EG-00019-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Mansour",
            Gender           = "Male",
            Email            = "dr.ayman.mansour.giza.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01010000020",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00020-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Cairo University Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Giza governorate with over 17 years of clinical practice.",
        }, "EG-00020-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Yasmin Barakat",
            Gender           = "Female",
            Email            = "dr.yasmin.barakat.giza.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01110000021",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00021-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Al-Helal Medical Complex",
            About            = "Experienced General Surgery Specialist based in Giza governorate with over 15 years of clinical practice.",
        }, "EG-00021-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ola Ali",
            Gender           = "Female",
            Email            = "dr.ola.ali.giza.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01210000022",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00022-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Al-Helal Medical Complex",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Giza governorate with over 6 years of clinical practice.",
        }, "EG-00022-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Wael Tantawi",
            Gender           = "Male",
            Email            = "dr.wael.tantawi.giza.cardio@ehrs-clinic.eg",
            ContactNumber    = "01510000023",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00023-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Royal Medical Center",
            About            = "Experienced Cardiology Specialist based in Giza governorate with over 7 years of clinical practice.",
        }, "EG-00023-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Heba Metwally",
            Gender           = "Female",
            Email            = "dr.heba.metwally.giza.neuro@ehrs-clinic.eg",
            ContactNumber    = "01010000024",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00024-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Dar Al-Shifa Hospital",
            About            = "Experienced Neurology Specialist based in Giza governorate with over 11 years of clinical practice.",
        }, "EG-00024-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Fady Tantawi",
            Gender           = "Male",
            Email            = "dr.fady.tantawi.giza.vasc@ehrs-clinic.eg",
            ContactNumber    = "01110000025",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00025-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Cairo University Hospital",
            About            = "Experienced Vascular Specialist based in Giza governorate with over 9 years of clinical practice.",
        }, "EG-00025-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Maged Farouk",
            Gender           = "Male",
            Email            = "dr.maged.farouk.giza.pulm@ehrs-clinic.eg",
            ContactNumber    = "01210000026",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00026-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Al-Helal Medical Complex",
            About            = "Experienced Chest Specialist (Pulmonology) based in Giza governorate with over 22 years of clinical practice.",
        }, "EG-00026-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Eman Gohar",
            Gender           = "Female",
            Email            = "dr.eman.gohar.giza.ortho@ehrs-clinic.eg",
            ContactNumber    = "01510000027",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00027-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Ain Shams University Hospital",
            About            = "Experienced Orthopedic Specialist based in Giza governorate with over 23 years of clinical practice.",
        }, "EG-00027-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samir Fouad",
            Gender           = "Male",
            Email            = "dr.samir.fouad.giza.derm@ehrs-clinic.eg",
            ContactNumber    = "01010000028",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00028-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Al-Helal Medical Complex",
            About            = "Experienced Dermatology Specialist based in Giza governorate with over 9 years of clinical practice.",
        }, "EG-00028-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Gerges Naguib",
            Gender           = "Male",
            Email            = "dr.gerges.naguib.giza.intmed@ehrs-clinic.eg",
            ContactNumber    = "01110000029",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00029-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Central Medical Center",
            About            = "Experienced Internal Medicine Specialist based in Giza governorate with over 6 years of clinical practice.",
        }, "EG-00029-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Dalia Saad",
            Gender           = "Female",
            Email            = "dr.dalia.saad.giza.dent@ehrs-clinic.eg",
            ContactNumber    = "01210000030",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00030-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Al-Ahrar Hospital",
            About            = "Experienced Dentist based in Giza governorate with over 18 years of clinical practice.",
        }, "EG-00030-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Khaled Fawzy",
            Gender           = "Male",
            Email            = "dr.khaled.fawzy.giza.ent@ehrs-clinic.eg",
            ContactNumber    = "01510000031",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00031-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Cleopatra Hospital",
            About            = "Experienced ENT Specialist based in Giza governorate with over 24 years of clinical practice.",
        }, "EG-00031-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bassem Shehata",
            Gender           = "Male",
            Email            = "dr.bassem.shehata.giza.uro@ehrs-clinic.eg",
            ContactNumber    = "01010000032",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00032-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Royal Medical Center",
            About            = "Experienced Urology Specialist based in Giza governorate with over 22 years of clinical practice.",
        }, "EG-00032-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nada Khalil",
            Gender           = "Female",
            Email            = "dr.nada.khalil.giza.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01110000033",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00033-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Royal Medical Center",
            About            = "Experienced Ophthalmology Specialist based in Giza governorate with over 25 years of clinical practice.",
        }, "EG-00033-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ramy Khalil",
            Gender           = "Male",
            Email            = "dr.ramy.khalil.giza.rheum@ehrs-clinic.eg",
            ContactNumber    = "01210000034",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00034-MD",
            Area             = "Giza",
            AffiliatedHospital = "Giza Al-Maadi Hospital",
            About            = "Experienced Rheumatology Specialist based in Giza governorate with over 18 years of clinical practice.",
        }, "EG-00034-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sherif Halim",
            Gender           = "Male",
            Email            = "dr.sherif.halim.alexandria.gp@ehrs-clinic.eg",
            ContactNumber    = "01510000035",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00035-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria University Hospital",
            About            = "Experienced General Practitioner (GP) based in Alexandria governorate with over 13 years of clinical practice.",
        }, "EG-00035-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Doaa Zaki",
            Gender           = "Female",
            Email            = "dr.doaa.zaki.alexandria.peds@ehrs-clinic.eg",
            ContactNumber    = "01010000036",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00036-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Nile Medical Center",
            About            = "Experienced Pediatrics Specialist based in Alexandria governorate with over 25 years of clinical practice.",
        }, "EG-00036-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Alaa Rushdi",
            Gender           = "Male",
            Email            = "dr.alaa.rushdi.alexandria.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01110000037",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00037-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Al-Zahraa Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Alexandria governorate with over 9 years of clinical practice.",
        }, "EG-00037-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Mansour",
            Gender           = "Male",
            Email            = "dr.karim.mansour.alexandria.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01210000038",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00038-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria University Hospital",
            About            = "Experienced General Surgery Specialist based in Alexandria governorate with over 24 years of clinical practice.",
        }, "EG-00038-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Amira Naguib",
            Gender           = "Female",
            Email            = "dr.amira.naguib.alexandria.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01510000039",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00039-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria University Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Alexandria governorate with over 8 years of clinical practice.",
        }, "EG-00039-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Saleh",
            Gender           = "Male",
            Email            = "dr.karim.saleh.alexandria.cardio@ehrs-clinic.eg",
            ContactNumber    = "01010000040",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00040-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Al-Helal Medical Complex",
            About            = "Experienced Cardiology Specialist based in Alexandria governorate with over 6 years of clinical practice.",
        }, "EG-00040-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Metwally",
            Gender           = "Male",
            Email            = "dr.ayman.metwally.alexandria.neuro@ehrs-clinic.eg",
            ContactNumber    = "01110000041",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00041-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Central Medical Center",
            About            = "Experienced Neurology Specialist based in Alexandria governorate with over 7 years of clinical practice.",
        }, "EG-00041-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Naguib",
            Gender           = "Female",
            Email            = "dr.reham.naguib.alexandria.vasc@ehrs-clinic.eg",
            ContactNumber    = "01210000042",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00042-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Central Medical Center",
            About            = "Experienced Vascular Specialist based in Alexandria governorate with over 22 years of clinical practice.",
        }, "EG-00042-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Amr Farouk",
            Gender           = "Male",
            Email            = "dr.amr.farouk.alexandria.pulm@ehrs-clinic.eg",
            ContactNumber    = "01510000043",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00043-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria As-Salam International Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Alexandria governorate with over 22 years of clinical practice.",
        }, "EG-00043-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sherif Gohar",
            Gender           = "Male",
            Email            = "dr.sherif.gohar.alexandria.ortho@ehrs-clinic.eg",
            ContactNumber    = "01010000044",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00044-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Ain Shams University Hospital",
            About            = "Experienced Orthopedic Specialist based in Alexandria governorate with over 11 years of clinical practice.",
        }, "EG-00044-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Eman Nasser",
            Gender           = "Female",
            Email            = "dr.eman.nasser.alexandria.derm@ehrs-clinic.eg",
            ContactNumber    = "01110000045",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00045-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Al-Maadi Hospital",
            About            = "Experienced Dermatology Specialist based in Alexandria governorate with over 17 years of clinical practice.",
        }, "EG-00045-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Safwat",
            Gender           = "Male",
            Email            = "dr.karim.safwat.alexandria.intmed@ehrs-clinic.eg",
            ContactNumber    = "01210000046",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00046-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Cairo University Hospital",
            About            = "Experienced Internal Medicine Specialist based in Alexandria governorate with over 8 years of clinical practice.",
        }, "EG-00046-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Shalaby",
            Gender           = "Male",
            Email            = "dr.ayman.shalaby.alexandria.dent@ehrs-clinic.eg",
            ContactNumber    = "01510000047",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00047-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Central Medical Center",
            About            = "Experienced Dentist based in Alexandria governorate with over 15 years of clinical practice.",
        }, "EG-00047-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nada Selim",
            Gender           = "Female",
            Email            = "dr.nada.selim.alexandria.ent@ehrs-clinic.eg",
            ContactNumber    = "01010000048",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00048-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Al-Helal Medical Complex",
            About            = "Experienced ENT Specialist based in Alexandria governorate with over 23 years of clinical practice.",
        }, "EG-00048-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mostafa Hassan",
            Gender           = "Male",
            Email            = "dr.mostafa.hassan.alexandria.uro@ehrs-clinic.eg",
            ContactNumber    = "01110000049",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00049-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Central Medical Center",
            About            = "Experienced Urology Specialist based in Alexandria governorate with over 25 years of clinical practice.",
        }, "EG-00049-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mahmoud Shalaby",
            Gender           = "Male",
            Email            = "dr.mahmoud.shalaby.alexandria.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01210000050",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00050-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Central Medical Center",
            About            = "Experienced Ophthalmology Specialist based in Alexandria governorate with over 6 years of clinical practice.",
        }, "EG-00050-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Amira Mohamed",
            Gender           = "Female",
            Email            = "dr.amira.mohamed.alexandria.rheum@ehrs-clinic.eg",
            ContactNumber    = "01510000051",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00051-MD",
            Area             = "Alexandria",
            AffiliatedHospital = "Alexandria Al-Helal Medical Complex",
            About            = "Experienced Rheumatology Specialist based in Alexandria governorate with over 13 years of clinical practice.",
        }, "EG-00051-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Fady Ramadan",
            Gender           = "Male",
            Email            = "dr.fady.ramadan.aswan.gp@ehrs-clinic.eg",
            ContactNumber    = "01010000052",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00052-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Al-Salam Hospital",
            About            = "Experienced General Practitioner (GP) based in Aswan governorate with over 23 years of clinical practice.",
        }, "EG-00052-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Atef Abdalla",
            Gender           = "Male",
            Email            = "dr.atef.abdalla.aswan.peds@ehrs-clinic.eg",
            ContactNumber    = "01110000053",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00053-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan As-Salam International Hospital",
            About            = "Experienced Pediatrics Specialist based in Aswan governorate with over 18 years of clinical practice.",
        }, "EG-00053-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Heba Omar",
            Gender           = "Female",
            Email            = "dr.heba.omar.aswan.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01210000054",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00054-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Nile Medical Center",
            About            = "Experienced Pediatric Surgery Specialist based in Aswan governorate with over 18 years of clinical practice.",
        }, "EG-00054-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ehab Hamdy",
            Gender           = "Male",
            Email            = "dr.ehab.hamdy.aswan.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01510000055",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00055-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Ain Shams University Hospital",
            About            = "Experienced General Surgery Specialist based in Aswan governorate with over 19 years of clinical practice.",
        }, "EG-00055-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Ali",
            Gender           = "Female",
            Email            = "dr.reham.ali.aswan.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01010000056",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00056-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Nile Medical Center",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Aswan governorate with over 6 years of clinical practice.",
        }, "EG-00056-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Shereen Badawi",
            Gender           = "Female",
            Email            = "dr.shereen.badawi.aswan.cardio@ehrs-clinic.eg",
            ContactNumber    = "01110000057",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00057-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Nile Medical Center",
            About            = "Experienced Cardiology Specialist based in Aswan governorate with over 12 years of clinical practice.",
        }, "EG-00057-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tarek Nasser",
            Gender           = "Male",
            Email            = "dr.tarek.nasser.aswan.neuro@ehrs-clinic.eg",
            ContactNumber    = "01210000058",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00058-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Cairo University Hospital",
            About            = "Experienced Neurology Specialist based in Aswan governorate with over 9 years of clinical practice.",
        }, "EG-00058-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Essam Zaki",
            Gender           = "Male",
            Email            = "dr.essam.zaki.aswan.vasc@ehrs-clinic.eg",
            ContactNumber    = "01510000059",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00059-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Royal Medical Center",
            About            = "Experienced Vascular Specialist based in Aswan governorate with over 19 years of clinical practice.",
        }, "EG-00059-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rania Mohamed",
            Gender           = "Female",
            Email            = "dr.rania.mohamed.aswan.pulm@ehrs-clinic.eg",
            ContactNumber    = "01010000060",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00060-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Cairo University Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Aswan governorate with over 22 years of clinical practice.",
        }, "EG-00060-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ibrahim Ali",
            Gender           = "Male",
            Email            = "dr.ibrahim.ali.aswan.ortho@ehrs-clinic.eg",
            ContactNumber    = "01110000061",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00061-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan University Hospital",
            About            = "Experienced Orthopedic Specialist based in Aswan governorate with over 7 years of clinical practice.",
        }, "EG-00061-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Mansour",
            Gender           = "Male",
            Email            = "dr.ayman.mansour.aswan.derm@ehrs-clinic.eg",
            ContactNumber    = "01210000062",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00062-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Ain Shams University Hospital",
            About            = "Experienced Dermatology Specialist based in Aswan governorate with over 20 years of clinical practice.",
        }, "EG-00062-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Lobna Ramadan",
            Gender           = "Female",
            Email            = "dr.lobna.ramadan.aswan.intmed@ehrs-clinic.eg",
            ContactNumber    = "01510000063",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00063-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Cleopatra Hospital",
            About            = "Experienced Internal Medicine Specialist based in Aswan governorate with over 6 years of clinical practice.",
        }, "EG-00063-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sherif Fawzy",
            Gender           = "Male",
            Email            = "dr.sherif.fawzy.aswan.dent@ehrs-clinic.eg",
            ContactNumber    = "01010000064",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00064-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan University Hospital",
            About            = "Experienced Dentist based in Aswan governorate with over 17 years of clinical practice.",
        }, "EG-00064-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Maged Halim",
            Gender           = "Male",
            Email            = "dr.maged.halim.aswan.ent@ehrs-clinic.eg",
            ContactNumber    = "01110000065",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00065-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Al-Maadi Hospital",
            About            = "Experienced ENT Specialist based in Aswan governorate with over 18 years of clinical practice.",
        }, "EG-00065-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Barakat",
            Gender           = "Female",
            Email            = "dr.manar.barakat.aswan.uro@ehrs-clinic.eg",
            ContactNumber    = "01210000066",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00066-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan As-Salam International Hospital",
            About            = "Experienced Urology Specialist based in Aswan governorate with over 9 years of clinical practice.",
        }, "EG-00066-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tarek Ragab",
            Gender           = "Male",
            Email            = "dr.tarek.ragab.aswan.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01510000067",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00067-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Al-Zahraa Hospital",
            About            = "Experienced Ophthalmology Specialist based in Aswan governorate with over 6 years of clinical practice.",
        }, "EG-00067-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bishoy Ali",
            Gender           = "Male",
            Email            = "dr.bishoy.ali.aswan.rheum@ehrs-clinic.eg",
            ContactNumber    = "01010000068",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00068-MD",
            Area             = "Aswan",
            AffiliatedHospital = "Aswan Dar Al-Shifa Hospital",
            About            = "Experienced Rheumatology Specialist based in Aswan governorate with over 6 years of clinical practice.",
        }, "EG-00068-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mariam Selim",
            Gender           = "Female",
            Email            = "dr.mariam.selim.asyut.gp@ehrs-clinic.eg",
            ContactNumber    = "01110000069",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00069-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut As-Salam International Hospital",
            About            = "Experienced General Practitioner (GP) based in Asyut governorate with over 21 years of clinical practice.",
        }, "EG-00069-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mina Mansour",
            Gender           = "Male",
            Email            = "dr.mina.mansour.asyut.peds@ehrs-clinic.eg",
            ContactNumber    = "01210000070",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00070-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut General Hospital",
            About            = "Experienced Pediatrics Specialist based in Asyut governorate with over 21 years of clinical practice.",
        }, "EG-00070-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Zaki",
            Gender           = "Male",
            Email            = "dr.hassan.zaki.asyut.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01510000071",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00071-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Central Medical Center",
            About            = "Experienced Pediatric Surgery Specialist based in Asyut governorate with over 24 years of clinical practice.",
        }, "EG-00071-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rana Abdalla",
            Gender           = "Female",
            Email            = "dr.rana.abdalla.asyut.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01010000072",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00072-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Cleopatra Hospital",
            About            = "Experienced General Surgery Specialist based in Asyut governorate with over 8 years of clinical practice.",
        }, "EG-00072-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nihal Abdalla",
            Gender           = "Female",
            Email            = "dr.nihal.abdalla.asyut.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01110000073",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00073-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut General Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Asyut governorate with over 24 years of clinical practice.",
        }, "EG-00073-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Aziz",
            Gender           = "Male",
            Email            = "dr.hassan.aziz.asyut.cardio@ehrs-clinic.eg",
            ContactNumber    = "01210000074",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00074-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Dar Al-Shifa Hospital",
            About            = "Experienced Cardiology Specialist based in Asyut governorate with over 13 years of clinical practice.",
        }, "EG-00074-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Heba Amer",
            Gender           = "Female",
            Email            = "dr.heba.amer.asyut.neuro@ehrs-clinic.eg",
            ContactNumber    = "01510000075",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00075-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Al-Helal Medical Complex",
            About            = "Experienced Neurology Specialist based in Asyut governorate with over 13 years of clinical practice.",
        }, "EG-00075-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samir Farouk",
            Gender           = "Male",
            Email            = "dr.samir.farouk.asyut.vasc@ehrs-clinic.eg",
            ContactNumber    = "01010000076",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00076-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Al-Maadi Hospital",
            About            = "Experienced Vascular Specialist based in Asyut governorate with over 19 years of clinical practice.",
        }, "EG-00076-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Wael Mohamed",
            Gender           = "Male",
            Email            = "dr.wael.mohamed.asyut.pulm@ehrs-clinic.eg",
            ContactNumber    = "01110000077",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00077-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut University Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Asyut governorate with over 19 years of clinical practice.",
        }, "EG-00077-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Faten Metwally",
            Gender           = "Female",
            Email            = "dr.faten.metwally.asyut.ortho@ehrs-clinic.eg",
            ContactNumber    = "01210000078",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00078-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Nile Medical Center",
            About            = "Experienced Orthopedic Specialist based in Asyut governorate with over 7 years of clinical practice.",
        }, "EG-00078-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bishoy Ramadan",
            Gender           = "Male",
            Email            = "dr.bishoy.ramadan.asyut.derm@ehrs-clinic.eg",
            ContactNumber    = "01510000079",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00079-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Royal Medical Center",
            About            = "Experienced Dermatology Specialist based in Asyut governorate with over 9 years of clinical practice.",
        }, "EG-00079-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ehab Mohamed",
            Gender           = "Male",
            Email            = "dr.ehab.mohamed.asyut.intmed@ehrs-clinic.eg",
            ContactNumber    = "01010000080",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00080-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Al-Helal Medical Complex",
            About            = "Experienced Internal Medicine Specialist based in Asyut governorate with over 16 years of clinical practice.",
        }, "EG-00080-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Noha Mansour",
            Gender           = "Female",
            Email            = "dr.noha.mansour.asyut.dent@ehrs-clinic.eg",
            ContactNumber    = "01110000081",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00081-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Cairo University Hospital",
            About            = "Experienced Dentist based in Asyut governorate with over 22 years of clinical practice.",
        }, "EG-00081-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Alaa Morsi",
            Gender           = "Male",
            Email            = "dr.alaa.morsi.asyut.ent@ehrs-clinic.eg",
            ContactNumber    = "01210000082",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00082-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut University Hospital",
            About            = "Experienced ENT Specialist based in Asyut governorate with over 22 years of clinical practice.",
        }, "EG-00082-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Alaa Omar",
            Gender           = "Male",
            Email            = "dr.alaa.omar.asyut.uro@ehrs-clinic.eg",
            ContactNumber    = "01510000083",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00083-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Al-Salam Hospital",
            About            = "Experienced Urology Specialist based in Asyut governorate with over 13 years of clinical practice.",
        }, "EG-00083-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Dalia Omar",
            Gender           = "Female",
            Email            = "dr.dalia.omar.asyut.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01010000084",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00084-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Al-Salam Hospital",
            About            = "Experienced Ophthalmology Specialist based in Asyut governorate with over 13 years of clinical practice.",
        }, "EG-00084-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samer Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.samer.abdelrahman.asyut.rheum@ehrs-clinic.eg",
            ContactNumber    = "01110000085",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00085-MD",
            Area             = "Asyut",
            AffiliatedHospital = "Asyut Al-Zahraa Hospital",
            About            = "Experienced Rheumatology Specialist based in Asyut governorate with over 15 years of clinical practice.",
        }, "EG-00085-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hossam Gohar",
            Gender           = "Male",
            Email            = "dr.hossam.gohar.beheira.gp@ehrs-clinic.eg",
            ContactNumber    = "01210000086",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00086-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira As-Salam International Hospital",
            About            = "Experienced General Practitioner (GP) based in Beheira governorate with over 13 years of clinical practice.",
        }, "EG-00086-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mariam Ahmed",
            Gender           = "Female",
            Email            = "dr.mariam.ahmed.beheira.peds@ehrs-clinic.eg",
            ContactNumber    = "01510000087",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00087-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Ain Shams University Hospital",
            About            = "Experienced Pediatrics Specialist based in Beheira governorate with over 13 years of clinical practice.",
        }, "EG-00087-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Omar Hassan",
            Gender           = "Male",
            Email            = "dr.omar.hassan.beheira.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01010000088",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00088-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Dar Al-Shifa Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Beheira governorate with over 9 years of clinical practice.",
        }, "EG-00088-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Maged Mansour",
            Gender           = "Male",
            Email            = "dr.maged.mansour.beheira.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01110000089",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00089-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Cairo University Hospital",
            About            = "Experienced General Surgery Specialist based in Beheira governorate with over 22 years of clinical practice.",
        }, "EG-00089-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Hamdy",
            Gender           = "Female",
            Email            = "dr.manar.hamdy.beheira.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01210000090",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00090-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira University Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Beheira governorate with over 8 years of clinical practice.",
        }, "EG-00090-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Khaled Saad",
            Gender           = "Male",
            Email            = "dr.khaled.saad.beheira.cardio@ehrs-clinic.eg",
            ContactNumber    = "01510000091",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00091-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira General Hospital",
            About            = "Experienced Cardiology Specialist based in Beheira governorate with over 16 years of clinical practice.",
        }, "EG-00091-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Emad Saad",
            Gender           = "Male",
            Email            = "dr.emad.saad.beheira.neuro@ehrs-clinic.eg",
            ContactNumber    = "01010000092",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00092-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Ain Shams University Hospital",
            About            = "Experienced Neurology Specialist based in Beheira governorate with over 9 years of clinical practice.",
        }, "EG-00092-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mariam Saleh",
            Gender           = "Female",
            Email            = "dr.mariam.saleh.beheira.vasc@ehrs-clinic.eg",
            ContactNumber    = "01110000093",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00093-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Al-Wadi Hospital",
            About            = "Experienced Vascular Specialist based in Beheira governorate with over 6 years of clinical practice.",
        }, "EG-00093-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ehab Ramadan",
            Gender           = "Male",
            Email            = "dr.ehab.ramadan.beheira.pulm@ehrs-clinic.eg",
            ContactNumber    = "01210000094",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00094-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Al-Helal Medical Complex",
            About            = "Experienced Chest Specialist (Pulmonology) based in Beheira governorate with over 8 years of clinical practice.",
        }, "EG-00094-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ehab Barakat",
            Gender           = "Male",
            Email            = "dr.ehab.barakat.beheira.ortho@ehrs-clinic.eg",
            ContactNumber    = "01510000095",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00095-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Ain Shams University Hospital",
            About            = "Experienced Orthopedic Specialist based in Beheira governorate with over 24 years of clinical practice.",
        }, "EG-00095-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Saad",
            Gender           = "Female",
            Email            = "dr.reham.saad.beheira.derm@ehrs-clinic.eg",
            ContactNumber    = "01010000096",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00096-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Al-Helal Medical Complex",
            About            = "Experienced Dermatology Specialist based in Beheira governorate with over 10 years of clinical practice.",
        }, "EG-00096-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Walid Aziz",
            Gender           = "Male",
            Email            = "dr.walid.aziz.beheira.intmed@ehrs-clinic.eg",
            ContactNumber    = "01110000097",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00097-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira University Hospital",
            About            = "Experienced Internal Medicine Specialist based in Beheira governorate with over 10 years of clinical practice.",
        }, "EG-00097-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ramy Aziz",
            Gender           = "Male",
            Email            = "dr.ramy.aziz.beheira.dent@ehrs-clinic.eg",
            ContactNumber    = "01210000098",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00098-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Al-Helal Medical Complex",
            About            = "Experienced Dentist based in Beheira governorate with over 13 years of clinical practice.",
        }, "EG-00098-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mona Omar",
            Gender           = "Female",
            Email            = "dr.mona.omar.beheira.ent@ehrs-clinic.eg",
            ContactNumber    = "01510000099",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00099-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Cleopatra Hospital",
            About            = "Experienced ENT Specialist based in Beheira governorate with over 6 years of clinical practice.",
        }, "EG-00099-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Atef Shalaby",
            Gender           = "Male",
            Email            = "dr.atef.shalaby.beheira.uro@ehrs-clinic.eg",
            ContactNumber    = "01010000100",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00100-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Al-Zahraa Hospital",
            About            = "Experienced Urology Specialist based in Beheira governorate with over 19 years of clinical practice.",
        }, "EG-00100-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ehab Saleh",
            Gender           = "Male",
            Email            = "dr.ehab.saleh.beheira.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01110000101",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00101-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Al-Helal Medical Complex",
            About            = "Experienced Ophthalmology Specialist based in Beheira governorate with over 12 years of clinical practice.",
        }, "EG-00101-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nada Nasser",
            Gender           = "Female",
            Email            = "dr.nada.nasser.beheira.rheum@ehrs-clinic.eg",
            ContactNumber    = "01210000102",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00102-MD",
            Area             = "Beheira",
            AffiliatedHospital = "Beheira Cleopatra Hospital",
            About            = "Experienced Rheumatology Specialist based in Beheira governorate with over 15 years of clinical practice.",
        }, "EG-00102-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Adel Mohamed",
            Gender           = "Male",
            Email            = "dr.adel.mohamed.benisuef.gp@ehrs-clinic.eg",
            ContactNumber    = "01510000103",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00103-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Royal Medical Center",
            About            = "Experienced General Practitioner (GP) based in Beni Suef governorate with over 16 years of clinical practice.",
        }, "EG-00103-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Gerges Tantawi",
            Gender           = "Male",
            Email            = "dr.gerges.tantawi.benisuef.peds@ehrs-clinic.eg",
            ContactNumber    = "01010000104",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00104-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Dar Al-Shifa Hospital",
            About            = "Experienced Pediatrics Specialist based in Beni Suef governorate with over 5 years of clinical practice.",
        }, "EG-00104-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Dalia Gohar",
            Gender           = "Female",
            Email            = "dr.dalia.gohar.benisuef.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01110000105",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00105-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Al-Ahrar Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Beni Suef governorate with over 23 years of clinical practice.",
        }, "EG-00105-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Maged Mahmoud",
            Gender           = "Male",
            Email            = "dr.maged.mahmoud.benisuef.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01210000106",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00106-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Nile Medical Center",
            About            = "Experienced General Surgery Specialist based in Beni Suef governorate with over 24 years of clinical practice.",
        }, "EG-00106-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Neveen Gaber",
            Gender           = "Female",
            Email            = "dr.neveen.gaber.benisuef.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01510000107",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00107-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Dar Al-Shifa Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Beni Suef governorate with over 18 years of clinical practice.",
        }, "EG-00107-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Faten Rushdi",
            Gender           = "Female",
            Email            = "dr.faten.rushdi.benisuef.cardio@ehrs-clinic.eg",
            ContactNumber    = "01010000108",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00108-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Nile Medical Center",
            About            = "Experienced Cardiology Specialist based in Beni Suef governorate with over 17 years of clinical practice.",
        }, "EG-00108-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tarek Gohar",
            Gender           = "Male",
            Email            = "dr.tarek.gohar.benisuef.neuro@ehrs-clinic.eg",
            ContactNumber    = "01110000109",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00109-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef General Hospital",
            About            = "Experienced Neurology Specialist based in Beni Suef governorate with over 18 years of clinical practice.",
        }, "EG-00109-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ahmed Shehata",
            Gender           = "Male",
            Email            = "dr.ahmed.shehata.benisuef.vasc@ehrs-clinic.eg",
            ContactNumber    = "01210000110",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00110-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Al-Zahraa Hospital",
            About            = "Experienced Vascular Specialist based in Beni Suef governorate with over 16 years of clinical practice.",
        }, "EG-00110-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Neveen Mohamed",
            Gender           = "Female",
            Email            = "dr.neveen.mohamed.benisuef.pulm@ehrs-clinic.eg",
            ContactNumber    = "01510000111",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00111-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Dar Al-Shifa Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Beni Suef governorate with over 24 years of clinical practice.",
        }, "EG-00111-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Wael Khalil",
            Gender           = "Male",
            Email            = "dr.wael.khalil.benisuef.ortho@ehrs-clinic.eg",
            ContactNumber    = "01010000112",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00112-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Al-Maadi Hospital",
            About            = "Experienced Orthopedic Specialist based in Beni Suef governorate with over 21 years of clinical practice.",
        }, "EG-00112-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Alaa Aziz",
            Gender           = "Male",
            Email            = "dr.alaa.aziz.benisuef.derm@ehrs-clinic.eg",
            ContactNumber    = "01110000113",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00113-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Dar Al-Shifa Hospital",
            About            = "Experienced Dermatology Specialist based in Beni Suef governorate with over 17 years of clinical practice.",
        }, "EG-00113-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Ragab",
            Gender           = "Female",
            Email            = "dr.manar.ragab.benisuef.intmed@ehrs-clinic.eg",
            ContactNumber    = "01210000114",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00114-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Al-Salam Hospital",
            About            = "Experienced Internal Medicine Specialist based in Beni Suef governorate with over 11 years of clinical practice.",
        }, "EG-00114-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nader Fawzy",
            Gender           = "Male",
            Email            = "dr.nader.fawzy.benisuef.dent@ehrs-clinic.eg",
            ContactNumber    = "01510000115",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00115-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Al-Ahrar Hospital",
            About            = "Experienced Dentist based in Beni Suef governorate with over 24 years of clinical practice.",
        }, "EG-00115-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Alaa Tantawi",
            Gender           = "Male",
            Email            = "dr.alaa.tantawi.benisuef.ent@ehrs-clinic.eg",
            ContactNumber    = "01010000116",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00116-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef University Hospital",
            About            = "Experienced ENT Specialist based in Beni Suef governorate with over 14 years of clinical practice.",
        }, "EG-00116-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Noha Ramadan",
            Gender           = "Female",
            Email            = "dr.noha.ramadan.benisuef.uro@ehrs-clinic.eg",
            ContactNumber    = "01110000117",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00117-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Ain Shams University Hospital",
            About            = "Experienced Urology Specialist based in Beni Suef governorate with over 23 years of clinical practice.",
        }, "EG-00117-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Wael Halim",
            Gender           = "Male",
            Email            = "dr.wael.halim.benisuef.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01210000118",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00118-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef Cairo University Hospital",
            About            = "Experienced Ophthalmology Specialist based in Beni Suef governorate with over 19 years of clinical practice.",
        }, "EG-00118-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hossam Rushdi",
            Gender           = "Male",
            Email            = "dr.hossam.rushdi.benisuef.rheum@ehrs-clinic.eg",
            ContactNumber    = "01510000119",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00119-MD",
            Area             = "Beni Suef",
            AffiliatedHospital = "Beni Suef As-Salam International Hospital",
            About            = "Experienced Rheumatology Specialist based in Beni Suef governorate with over 10 years of clinical practice.",
        }, "EG-00119-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samira Ahmed",
            Gender           = "Female",
            Email            = "dr.samira.ahmed.dakahlia.gp@ehrs-clinic.eg",
            ContactNumber    = "01010000120",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00120-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Al-Maadi Hospital",
            About            = "Experienced General Practitioner (GP) based in Dakahlia governorate with over 21 years of clinical practice.",
        }, "EG-00120-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ramy Ahmed",
            Gender           = "Male",
            Email            = "dr.ramy.ahmed.dakahlia.peds@ehrs-clinic.eg",
            ContactNumber    = "01110000121",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00121-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Al-Helal Medical Complex",
            About            = "Experienced Pediatrics Specialist based in Dakahlia governorate with over 14 years of clinical practice.",
        }, "EG-00121-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mostafa Nasser",
            Gender           = "Male",
            Email            = "dr.mostafa.nasser.dakahlia.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01210000122",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00122-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Al-Salam Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Dakahlia governorate with over 5 years of clinical practice.",
        }, "EG-00122-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mariam Abdalla",
            Gender           = "Female",
            Email            = "dr.mariam.abdalla.dakahlia.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01510000123",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00123-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia As-Salam International Hospital",
            About            = "Experienced General Surgery Specialist based in Dakahlia governorate with over 24 years of clinical practice.",
        }, "EG-00123-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ola Mohamed",
            Gender           = "Female",
            Email            = "dr.ola.mohamed.dakahlia.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01010000124",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00124-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Cairo University Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Dakahlia governorate with over 18 years of clinical practice.",
        }, "EG-00124-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tarek Fawzy",
            Gender           = "Male",
            Email            = "dr.tarek.fawzy.dakahlia.cardio@ehrs-clinic.eg",
            ContactNumber    = "01110000125",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00125-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia As-Salam International Hospital",
            About            = "Experienced Cardiology Specialist based in Dakahlia governorate with over 17 years of clinical practice.",
        }, "EG-00125-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rania Saad",
            Gender           = "Female",
            Email            = "dr.rania.saad.dakahlia.neuro@ehrs-clinic.eg",
            ContactNumber    = "01210000126",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00126-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia University Hospital",
            About            = "Experienced Neurology Specialist based in Dakahlia governorate with over 8 years of clinical practice.",
        }, "EG-00126-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Essam Shalaby",
            Gender           = "Male",
            Email            = "dr.essam.shalaby.dakahlia.vasc@ehrs-clinic.eg",
            ContactNumber    = "01510000127",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00127-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Al-Ahrar Hospital",
            About            = "Experienced Vascular Specialist based in Dakahlia governorate with over 21 years of clinical practice.",
        }, "EG-00127-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bassem Ali",
            Gender           = "Male",
            Email            = "dr.bassem.ali.dakahlia.pulm@ehrs-clinic.eg",
            ContactNumber    = "01010000128",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00128-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Al-Helal Medical Complex",
            About            = "Experienced Chest Specialist (Pulmonology) based in Dakahlia governorate with over 8 years of clinical practice.",
        }, "EG-00128-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ghada Farouk",
            Gender           = "Female",
            Email            = "dr.ghada.farouk.dakahlia.ortho@ehrs-clinic.eg",
            ContactNumber    = "01110000129",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00129-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Cairo University Hospital",
            About            = "Experienced Orthopedic Specialist based in Dakahlia governorate with over 21 years of clinical practice.",
        }, "EG-00129-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Emad Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.emad.abdelrahman.dakahlia.derm@ehrs-clinic.eg",
            ContactNumber    = "01210000130",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00130-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Dar Al-Shifa Hospital",
            About            = "Experienced Dermatology Specialist based in Dakahlia governorate with over 19 years of clinical practice.",
        }, "EG-00130-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Gerges Hamdy",
            Gender           = "Male",
            Email            = "dr.gerges.hamdy.dakahlia.intmed@ehrs-clinic.eg",
            ContactNumber    = "01510000131",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00131-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Cairo University Hospital",
            About            = "Experienced Internal Medicine Specialist based in Dakahlia governorate with over 10 years of clinical practice.",
        }, "EG-00131-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Kamal",
            Gender           = "Female",
            Email            = "dr.reham.kamal.dakahlia.dent@ehrs-clinic.eg",
            ContactNumber    = "01010000132",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00132-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Cairo University Hospital",
            About            = "Experienced Dentist based in Dakahlia governorate with over 13 years of clinical practice.",
        }, "EG-00132-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Lotfy",
            Gender           = "Male",
            Email            = "dr.ayman.lotfy.dakahlia.ent@ehrs-clinic.eg",
            ContactNumber    = "01110000133",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00133-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia As-Salam International Hospital",
            About            = "Experienced ENT Specialist based in Dakahlia governorate with over 25 years of clinical practice.",
        }, "EG-00133-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Lotfy",
            Gender           = "Male",
            Email            = "dr.ayman.lotfy.dakahlia.uro@ehrs-clinic.eg",
            ContactNumber    = "01210000134",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00134-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Cairo University Hospital",
            About            = "Experienced Urology Specialist based in Dakahlia governorate with over 7 years of clinical practice.",
        }, "EG-00134-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Ragab",
            Gender           = "Female",
            Email            = "dr.manar.ragab.dakahlia.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01510000135",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00135-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Al-Helal Medical Complex",
            About            = "Experienced Ophthalmology Specialist based in Dakahlia governorate with over 13 years of clinical practice.",
        }, "EG-00135-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ramy Amer",
            Gender           = "Male",
            Email            = "dr.ramy.amer.dakahlia.rheum@ehrs-clinic.eg",
            ContactNumber    = "01010000136",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00136-MD",
            Area             = "Dakahlia",
            AffiliatedHospital = "Dakahlia Central Medical Center",
            About            = "Experienced Rheumatology Specialist based in Dakahlia governorate with over 9 years of clinical practice.",
        }, "EG-00136-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Shalaby",
            Gender           = "Male",
            Email            = "dr.tamer.shalaby.damietta.gp@ehrs-clinic.eg",
            ContactNumber    = "01110000137",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00137-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Cleopatra Hospital",
            About            = "Experienced General Practitioner (GP) based in Damietta governorate with over 9 years of clinical practice.",
        }, "EG-00137-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Ramadan",
            Gender           = "Female",
            Email            = "dr.manar.ramadan.damietta.peds@ehrs-clinic.eg",
            ContactNumber    = "01210000138",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00138-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Central Medical Center",
            About            = "Experienced Pediatrics Specialist based in Damietta governorate with over 18 years of clinical practice.",
        }, "EG-00138-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nader Badawi",
            Gender           = "Male",
            Email            = "dr.nader.badawi.damietta.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01510000139",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00139-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Cairo University Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Damietta governorate with over 18 years of clinical practice.",
        }, "EG-00139-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mahmoud Ramadan",
            Gender           = "Male",
            Email            = "dr.mahmoud.ramadan.damietta.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01010000140",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00140-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Ain Shams University Hospital",
            About            = "Experienced General Surgery Specialist based in Damietta governorate with over 17 years of clinical practice.",
        }, "EG-00140-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ola Selim",
            Gender           = "Female",
            Email            = "dr.ola.selim.damietta.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01110000141",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00141-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta University Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Damietta governorate with over 23 years of clinical practice.",
        }, "EG-00141-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hany Kamal",
            Gender           = "Male",
            Email            = "dr.hany.kamal.damietta.cardio@ehrs-clinic.eg",
            ContactNumber    = "01210000142",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00142-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta University Hospital",
            About            = "Experienced Cardiology Specialist based in Damietta governorate with over 16 years of clinical practice.",
        }, "EG-00142-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Alaa Fawzy",
            Gender           = "Male",
            Email            = "dr.alaa.fawzy.damietta.neuro@ehrs-clinic.eg",
            ContactNumber    = "01510000143",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00143-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Ain Shams University Hospital",
            About            = "Experienced Neurology Specialist based in Damietta governorate with over 22 years of clinical practice.",
        }, "EG-00143-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Ghoneim",
            Gender           = "Female",
            Email            = "dr.reham.ghoneim.damietta.vasc@ehrs-clinic.eg",
            ContactNumber    = "01010000144",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00144-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Al-Helal Medical Complex",
            About            = "Experienced Vascular Specialist based in Damietta governorate with over 20 years of clinical practice.",
        }, "EG-00144-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mostafa Lotfy",
            Gender           = "Male",
            Email            = "dr.mostafa.lotfy.damietta.pulm@ehrs-clinic.eg",
            ContactNumber    = "01110000145",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00145-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Ain Shams University Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Damietta governorate with over 20 years of clinical practice.",
        }, "EG-00145-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mohamed Fawzy",
            Gender           = "Male",
            Email            = "dr.mohamed.fawzy.damietta.ortho@ehrs-clinic.eg",
            ContactNumber    = "01210000146",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00146-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Dar Al-Shifa Hospital",
            About            = "Experienced Orthopedic Specialist based in Damietta governorate with over 17 years of clinical practice.",
        }, "EG-00146-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Mansour",
            Gender           = "Female",
            Email            = "dr.reham.mansour.damietta.derm@ehrs-clinic.eg",
            ContactNumber    = "01510000147",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00147-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Cairo University Hospital",
            About            = "Experienced Dermatology Specialist based in Damietta governorate with over 9 years of clinical practice.",
        }, "EG-00147-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bishoy Ibrahim",
            Gender           = "Male",
            Email            = "dr.bishoy.ibrahim.damietta.intmed@ehrs-clinic.eg",
            ContactNumber    = "01010000148",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00148-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Cleopatra Hospital",
            About            = "Experienced Internal Medicine Specialist based in Damietta governorate with over 23 years of clinical practice.",
        }, "EG-00148-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mohamed Ahmed",
            Gender           = "Male",
            Email            = "dr.mohamed.ahmed.damietta.dent@ehrs-clinic.eg",
            ContactNumber    = "01110000149",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00149-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Ain Shams University Hospital",
            About            = "Experienced Dentist based in Damietta governorate with over 9 years of clinical practice.",
        }, "EG-00149-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ghada Zaki",
            Gender           = "Female",
            Email            = "dr.ghada.zaki.damietta.ent@ehrs-clinic.eg",
            ContactNumber    = "01210000150",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00150-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta General Hospital",
            About            = "Experienced ENT Specialist based in Damietta governorate with over 13 years of clinical practice.",
        }, "EG-00150-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hany Amer",
            Gender           = "Male",
            Email            = "dr.hany.amer.damietta.uro@ehrs-clinic.eg",
            ContactNumber    = "01510000151",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00151-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Al-Zahraa Hospital",
            About            = "Experienced Urology Specialist based in Damietta governorate with over 19 years of clinical practice.",
        }, "EG-00151-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Wael Badawi",
            Gender           = "Male",
            Email            = "dr.wael.badawi.damietta.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01010000152",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00152-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Cleopatra Hospital",
            About            = "Experienced Ophthalmology Specialist based in Damietta governorate with over 13 years of clinical practice.",
        }, "EG-00152-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ola Aziz",
            Gender           = "Female",
            Email            = "dr.ola.aziz.damietta.rheum@ehrs-clinic.eg",
            ContactNumber    = "01110000153",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00153-MD",
            Area             = "Damietta",
            AffiliatedHospital = "Damietta Royal Medical Center",
            About            = "Experienced Rheumatology Specialist based in Damietta governorate with over 7 years of clinical practice.",
        }, "EG-00153-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Atef Ibrahim",
            Gender           = "Male",
            Email            = "dr.atef.ibrahim.faiyum.gp@ehrs-clinic.eg",
            ContactNumber    = "01210000154",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00154-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum General Hospital",
            About            = "Experienced General Practitioner (GP) based in Faiyum governorate with over 16 years of clinical practice.",
        }, "EG-00154-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mostafa Mohamed",
            Gender           = "Male",
            Email            = "dr.mostafa.mohamed.faiyum.peds@ehrs-clinic.eg",
            ContactNumber    = "01510000155",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00155-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum General Hospital",
            About            = "Experienced Pediatrics Specialist based in Faiyum governorate with over 5 years of clinical practice.",
        }, "EG-00155-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rania Nasser",
            Gender           = "Female",
            Email            = "dr.rania.nasser.faiyum.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01010000156",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00156-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum University Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Faiyum governorate with over 24 years of clinical practice.",
        }, "EG-00156-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Abdalla",
            Gender           = "Male",
            Email            = "dr.tamer.abdalla.faiyum.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01110000157",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00157-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Al-Salam Hospital",
            About            = "Experienced General Surgery Specialist based in Faiyum governorate with over 20 years of clinical practice.",
        }, "EG-00157-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samira Khalil",
            Gender           = "Female",
            Email            = "dr.samira.khalil.faiyum.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01210000158",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00158-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Al-Zahraa Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Faiyum governorate with over 19 years of clinical practice.",
        }, "EG-00158-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Gohar",
            Gender           = "Female",
            Email            = "dr.manar.gohar.faiyum.cardio@ehrs-clinic.eg",
            ContactNumber    = "01510000159",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00159-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Al-Wadi Hospital",
            About            = "Experienced Cardiology Specialist based in Faiyum governorate with over 10 years of clinical practice.",
        }, "EG-00159-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Youssef Mansour",
            Gender           = "Male",
            Email            = "dr.youssef.mansour.faiyum.neuro@ehrs-clinic.eg",
            ContactNumber    = "01010000160",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00160-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Al-Maadi Hospital",
            About            = "Experienced Neurology Specialist based in Faiyum governorate with over 8 years of clinical practice.",
        }, "EG-00160-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mohamed Saleh",
            Gender           = "Male",
            Email            = "dr.mohamed.saleh.faiyum.vasc@ehrs-clinic.eg",
            ContactNumber    = "01110000161",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00161-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Cleopatra Hospital",
            About            = "Experienced Vascular Specialist based in Faiyum governorate with over 17 years of clinical practice.",
        }, "EG-00161-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Nasser",
            Gender           = "Female",
            Email            = "dr.manar.nasser.faiyum.pulm@ehrs-clinic.eg",
            ContactNumber    = "01210000162",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00162-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Central Medical Center",
            About            = "Experienced Chest Specialist (Pulmonology) based in Faiyum governorate with over 23 years of clinical practice.",
        }, "EG-00162-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Omar",
            Gender           = "Male",
            Email            = "dr.ayman.omar.faiyum.ortho@ehrs-clinic.eg",
            ContactNumber    = "01510000163",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00163-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Al-Maadi Hospital",
            About            = "Experienced Orthopedic Specialist based in Faiyum governorate with over 24 years of clinical practice.",
        }, "EG-00163-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Youssef Metwally",
            Gender           = "Male",
            Email            = "dr.youssef.metwally.faiyum.derm@ehrs-clinic.eg",
            ContactNumber    = "01010000164",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00164-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum General Hospital",
            About            = "Experienced Dermatology Specialist based in Faiyum governorate with over 16 years of clinical practice.",
        }, "EG-00164-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Eman Hamdy",
            Gender           = "Female",
            Email            = "dr.eman.hamdy.faiyum.intmed@ehrs-clinic.eg",
            ContactNumber    = "01110000165",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00165-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Al-Wadi Hospital",
            About            = "Experienced Internal Medicine Specialist based in Faiyum governorate with over 7 years of clinical practice.",
        }, "EG-00165-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Gerges Badawi",
            Gender           = "Male",
            Email            = "dr.gerges.badawi.faiyum.dent@ehrs-clinic.eg",
            ContactNumber    = "01210000166",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00166-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum University Hospital",
            About            = "Experienced Dentist based in Faiyum governorate with over 18 years of clinical practice.",
        }, "EG-00166-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Fady Omar",
            Gender           = "Male",
            Email            = "dr.fady.omar.faiyum.ent@ehrs-clinic.eg",
            ContactNumber    = "01510000167",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00167-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Ain Shams University Hospital",
            About            = "Experienced ENT Specialist based in Faiyum governorate with over 16 years of clinical practice.",
        }, "EG-00167-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Abeer Halim",
            Gender           = "Female",
            Email            = "dr.abeer.halim.faiyum.uro@ehrs-clinic.eg",
            ContactNumber    = "01010000168",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00168-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Al-Salam Hospital",
            About            = "Experienced Urology Specialist based in Faiyum governorate with over 18 years of clinical practice.",
        }, "EG-00168-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Walid Shehata",
            Gender           = "Male",
            Email            = "dr.walid.shehata.faiyum.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01110000169",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00169-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Royal Medical Center",
            About            = "Experienced Ophthalmology Specialist based in Faiyum governorate with over 24 years of clinical practice.",
        }, "EG-00169-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bishoy Kamal",
            Gender           = "Male",
            Email            = "dr.bishoy.kamal.faiyum.rheum@ehrs-clinic.eg",
            ContactNumber    = "01210000170",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00170-MD",
            Area             = "Faiyum",
            AffiliatedHospital = "Faiyum Cairo University Hospital",
            About            = "Experienced Rheumatology Specialist based in Faiyum governorate with over 18 years of clinical practice.",
        }, "EG-00170-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Selim",
            Gender           = "Female",
            Email            = "dr.reham.selim.sharqia.gp@ehrs-clinic.eg",
            ContactNumber    = "01510000171",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00171-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Royal Medical Center",
            About            = "Experienced General Practitioner (GP) based in Sharqia governorate with over 15 years of clinical practice.",
        }, "EG-00171-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Ahmed",
            Gender           = "Male",
            Email            = "dr.ayman.ahmed.sharqia.peds@ehrs-clinic.eg",
            ContactNumber    = "01010000172",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00172-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Royal Medical Center",
            About            = "Experienced Pediatrics Specialist based in Sharqia governorate with over 19 years of clinical practice.",
        }, "EG-00172-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Halim",
            Gender           = "Male",
            Email            = "dr.ayman.halim.sharqia.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01110000173",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00173-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Cleopatra Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Sharqia governorate with over 15 years of clinical practice.",
        }, "EG-00173-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nada Naguib",
            Gender           = "Female",
            Email            = "dr.nada.naguib.sharqia.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01210000174",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00174-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Dar Al-Shifa Hospital",
            About            = "Experienced General Surgery Specialist based in Sharqia governorate with over 10 years of clinical practice.",
        }, "EG-00174-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Lobna Ramadan",
            Gender           = "Female",
            Email            = "dr.lobna.ramadan.sharqia.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01510000175",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00175-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Al-Wadi Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Sharqia governorate with over 13 years of clinical practice.",
        }, "EG-00175-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ramy Lotfy",
            Gender           = "Male",
            Email            = "dr.ramy.lotfy.sharqia.cardio@ehrs-clinic.eg",
            ContactNumber    = "01010000176",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00176-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Royal Medical Center",
            About            = "Experienced Cardiology Specialist based in Sharqia governorate with over 22 years of clinical practice.",
        }, "EG-00176-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nada Shehata",
            Gender           = "Female",
            Email            = "dr.nada.shehata.sharqia.neuro@ehrs-clinic.eg",
            ContactNumber    = "01110000177",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00177-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Al-Zahraa Hospital",
            About            = "Experienced Neurology Specialist based in Sharqia governorate with over 7 years of clinical practice.",
        }, "EG-00177-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Aziz",
            Gender           = "Male",
            Email            = "dr.ayman.aziz.sharqia.vasc@ehrs-clinic.eg",
            ContactNumber    = "01210000178",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00178-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia As-Salam International Hospital",
            About            = "Experienced Vascular Specialist based in Sharqia governorate with over 22 years of clinical practice.",
        }, "EG-00178-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Kamal",
            Gender           = "Male",
            Email            = "dr.ayman.kamal.sharqia.pulm@ehrs-clinic.eg",
            ContactNumber    = "01510000179",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00179-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia As-Salam International Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Sharqia governorate with over 19 years of clinical practice.",
        }, "EG-00179-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Shaimaa Ibrahim",
            Gender           = "Female",
            Email            = "dr.shaimaa.ibrahim.sharqia.ortho@ehrs-clinic.eg",
            ContactNumber    = "01010000180",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00180-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Central Medical Center",
            About            = "Experienced Orthopedic Specialist based in Sharqia governorate with over 14 years of clinical practice.",
        }, "EG-00180-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mostafa Tantawi",
            Gender           = "Male",
            Email            = "dr.mostafa.tantawi.sharqia.derm@ehrs-clinic.eg",
            ContactNumber    = "01110000181",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00181-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Al-Helal Medical Complex",
            About            = "Experienced Dermatology Specialist based in Sharqia governorate with over 14 years of clinical practice.",
        }, "EG-00181-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Kamal",
            Gender           = "Male",
            Email            = "dr.karim.kamal.sharqia.intmed@ehrs-clinic.eg",
            ContactNumber    = "01210000182",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00182-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Al-Wadi Hospital",
            About            = "Experienced Internal Medicine Specialist based in Sharqia governorate with over 18 years of clinical practice.",
        }, "EG-00182-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Barakat",
            Gender           = "Female",
            Email            = "dr.reham.barakat.sharqia.dent@ehrs-clinic.eg",
            ContactNumber    = "01510000183",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00183-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Dar Al-Shifa Hospital",
            About            = "Experienced Dentist based in Sharqia governorate with over 16 years of clinical practice.",
        }, "EG-00183-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bassem Lotfy",
            Gender           = "Male",
            Email            = "dr.bassem.lotfy.sharqia.ent@ehrs-clinic.eg",
            ContactNumber    = "01010000184",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00184-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Al-Maadi Hospital",
            About            = "Experienced ENT Specialist based in Sharqia governorate with over 13 years of clinical practice.",
        }, "EG-00184-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mostafa Khalil",
            Gender           = "Male",
            Email            = "dr.mostafa.khalil.sharqia.uro@ehrs-clinic.eg",
            ContactNumber    = "01110000185",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00185-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Al-Zahraa Hospital",
            About            = "Experienced Urology Specialist based in Sharqia governorate with over 15 years of clinical practice.",
        }, "EG-00185-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Dalia Ghoneim",
            Gender           = "Female",
            Email            = "dr.dalia.ghoneim.sharqia.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01210000186",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00186-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Al-Ahrar Hospital",
            About            = "Experienced Ophthalmology Specialist based in Sharqia governorate with over 11 years of clinical practice.",
        }, "EG-00186-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hossam Kamal",
            Gender           = "Male",
            Email            = "dr.hossam.kamal.sharqia.rheum@ehrs-clinic.eg",
            ContactNumber    = "01510000187",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00187-MD",
            Area             = "Sharqia",
            AffiliatedHospital = "Sharqia Royal Medical Center",
            About            = "Experienced Rheumatology Specialist based in Sharqia governorate with over 23 years of clinical practice.",
        }, "EG-00187-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mina Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.mina.abdelrahman.gharbia.gp@ehrs-clinic.eg",
            ContactNumber    = "01010000188",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00188-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia Al-Maadi Hospital",
            About            = "Experienced General Practitioner (GP) based in Gharbia governorate with over 8 years of clinical practice.",
        }, "EG-00188-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Heba Ragab",
            Gender           = "Female",
            Email            = "dr.heba.ragab.gharbia.peds@ehrs-clinic.eg",
            ContactNumber    = "01110000189",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00189-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia Al-Helal Medical Complex",
            About            = "Experienced Pediatrics Specialist based in Gharbia governorate with over 16 years of clinical practice.",
        }, "EG-00189-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Walid Saleh",
            Gender           = "Male",
            Email            = "dr.walid.saleh.gharbia.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01210000190",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00190-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia University Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Gharbia governorate with over 22 years of clinical practice.",
        }, "EG-00190-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Amr Lotfy",
            Gender           = "Male",
            Email            = "dr.amr.lotfy.gharbia.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01510000191",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00191-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia General Hospital",
            About            = "Experienced General Surgery Specialist based in Gharbia governorate with over 6 years of clinical practice.",
        }, "EG-00191-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Eman Ragab",
            Gender           = "Female",
            Email            = "dr.eman.ragab.gharbia.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01010000192",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00192-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia Al-Salam Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Gharbia governorate with over 25 years of clinical practice.",
        }, "EG-00192-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Fady Omar",
            Gender           = "Male",
            Email            = "dr.fady.omar.gharbia.cardio@ehrs-clinic.eg",
            ContactNumber    = "01110000193",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00193-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia University Hospital",
            About            = "Experienced Cardiology Specialist based in Gharbia governorate with over 23 years of clinical practice.",
        }, "EG-00193-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samer Kamal",
            Gender           = "Male",
            Email            = "dr.samer.kamal.gharbia.neuro@ehrs-clinic.eg",
            ContactNumber    = "01210000194",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00194-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia As-Salam International Hospital",
            About            = "Experienced Neurology Specialist based in Gharbia governorate with over 19 years of clinical practice.",
        }, "EG-00194-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Amira Zaki",
            Gender           = "Female",
            Email            = "dr.amira.zaki.gharbia.vasc@ehrs-clinic.eg",
            ContactNumber    = "01510000195",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00195-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia General Hospital",
            About            = "Experienced Vascular Specialist based in Gharbia governorate with over 13 years of clinical practice.",
        }, "EG-00195-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Atef Khalil",
            Gender           = "Male",
            Email            = "dr.atef.khalil.gharbia.pulm@ehrs-clinic.eg",
            ContactNumber    = "01010000196",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00196-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia Central Medical Center",
            About            = "Experienced Chest Specialist (Pulmonology) based in Gharbia governorate with over 17 years of clinical practice.",
        }, "EG-00196-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Fady Mohamed",
            Gender           = "Male",
            Email            = "dr.fady.mohamed.gharbia.ortho@ehrs-clinic.eg",
            ContactNumber    = "01110000197",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00197-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia General Hospital",
            About            = "Experienced Orthopedic Specialist based in Gharbia governorate with over 9 years of clinical practice.",
        }, "EG-00197-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sara Metwally",
            Gender           = "Female",
            Email            = "dr.sara.metwally.gharbia.derm@ehrs-clinic.eg",
            ContactNumber    = "01210000198",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00198-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia Al-Maadi Hospital",
            About            = "Experienced Dermatology Specialist based in Gharbia governorate with over 7 years of clinical practice.",
        }, "EG-00198-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Khalil",
            Gender           = "Male",
            Email            = "dr.ayman.khalil.gharbia.intmed@ehrs-clinic.eg",
            ContactNumber    = "01510000199",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00199-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia Ain Shams University Hospital",
            About            = "Experienced Internal Medicine Specialist based in Gharbia governorate with over 24 years of clinical practice.",
        }, "EG-00199-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mostafa Shehata",
            Gender           = "Male",
            Email            = "dr.mostafa.shehata.gharbia.dent@ehrs-clinic.eg",
            ContactNumber    = "01010000200",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00200-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia Cleopatra Hospital",
            About            = "Experienced Dentist based in Gharbia governorate with over 19 years of clinical practice.",
        }, "EG-00200-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ghada Saleh",
            Gender           = "Female",
            Email            = "dr.ghada.saleh.gharbia.ent@ehrs-clinic.eg",
            ContactNumber    = "01110000201",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00201-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia Ain Shams University Hospital",
            About            = "Experienced ENT Specialist based in Gharbia governorate with over 14 years of clinical practice.",
        }, "EG-00201-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mahmoud Morsi",
            Gender           = "Male",
            Email            = "dr.mahmoud.morsi.gharbia.uro@ehrs-clinic.eg",
            ContactNumber    = "01210000202",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00202-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia Nile Medical Center",
            About            = "Experienced Urology Specialist based in Gharbia governorate with over 11 years of clinical practice.",
        }, "EG-00202-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hossam Gohar",
            Gender           = "Male",
            Email            = "dr.hossam.gohar.gharbia.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01510000203",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00203-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia Central Medical Center",
            About            = "Experienced Ophthalmology Specialist based in Gharbia governorate with over 10 years of clinical practice.",
        }, "EG-00203-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rania Zaki",
            Gender           = "Female",
            Email            = "dr.rania.zaki.gharbia.rheum@ehrs-clinic.eg",
            ContactNumber    = "01010000204",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00204-MD",
            Area             = "Gharbia",
            AffiliatedHospital = "Gharbia Central Medical Center",
            About            = "Experienced Rheumatology Specialist based in Gharbia governorate with over 10 years of clinical practice.",
        }, "EG-00204-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ahmed Aziz",
            Gender           = "Male",
            Email            = "dr.ahmed.aziz.ismailia.gp@ehrs-clinic.eg",
            ContactNumber    = "01110000205",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00205-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Cairo University Hospital",
            About            = "Experienced General Practitioner (GP) based in Ismailia governorate with over 24 years of clinical practice.",
        }, "EG-00205-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Atef Ragab",
            Gender           = "Male",
            Email            = "dr.atef.ragab.ismailia.peds@ehrs-clinic.eg",
            ContactNumber    = "01210000206",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00206-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia General Hospital",
            About            = "Experienced Pediatrics Specialist based in Ismailia governorate with over 12 years of clinical practice.",
        }, "EG-00206-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Noha Ragab",
            Gender           = "Female",
            Email            = "dr.noha.ragab.ismailia.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01510000207",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00207-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Cairo University Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Ismailia governorate with over 7 years of clinical practice.",
        }, "EG-00207-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mostafa Gohar",
            Gender           = "Male",
            Email            = "dr.mostafa.gohar.ismailia.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01010000208",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00208-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Al-Zahraa Hospital",
            About            = "Experienced General Surgery Specialist based in Ismailia governorate with over 18 years of clinical practice.",
        }, "EG-00208-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Dalia Ghoneim",
            Gender           = "Female",
            Email            = "dr.dalia.ghoneim.ismailia.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01110000209",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00209-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Al-Helal Medical Complex",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Ismailia governorate with over 25 years of clinical practice.",
        }, "EG-00209-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sara Lotfy",
            Gender           = "Female",
            Email            = "dr.sara.lotfy.ismailia.cardio@ehrs-clinic.eg",
            ContactNumber    = "01210000210",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00210-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Al-Salam Hospital",
            About            = "Experienced Cardiology Specialist based in Ismailia governorate with over 7 years of clinical practice.",
        }, "EG-00210-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mahmoud Mansour",
            Gender           = "Male",
            Email            = "dr.mahmoud.mansour.ismailia.neuro@ehrs-clinic.eg",
            ContactNumber    = "01510000211",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00211-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Al-Maadi Hospital",
            About            = "Experienced Neurology Specialist based in Ismailia governorate with over 24 years of clinical practice.",
        }, "EG-00211-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samer Safwat",
            Gender           = "Male",
            Email            = "dr.samer.safwat.ismailia.vasc@ehrs-clinic.eg",
            ContactNumber    = "01010000212",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00212-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Nile Medical Center",
            About            = "Experienced Vascular Specialist based in Ismailia governorate with over 19 years of clinical practice.",
        }, "EG-00212-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Saleh",
            Gender           = "Female",
            Email            = "dr.manar.saleh.ismailia.pulm@ehrs-clinic.eg",
            ContactNumber    = "01110000213",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00213-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Cleopatra Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Ismailia governorate with over 13 years of clinical practice.",
        }, "EG-00213-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Gerges Ghoneim",
            Gender           = "Male",
            Email            = "dr.gerges.ghoneim.ismailia.ortho@ehrs-clinic.eg",
            ContactNumber    = "01210000214",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00214-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia As-Salam International Hospital",
            About            = "Experienced Orthopedic Specialist based in Ismailia governorate with over 19 years of clinical practice.",
        }, "EG-00214-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.hassan.abdelrahman.ismailia.derm@ehrs-clinic.eg",
            ContactNumber    = "01510000215",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00215-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia General Hospital",
            About            = "Experienced Dermatology Specialist based in Ismailia governorate with over 18 years of clinical practice.",
        }, "EG-00215-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Amer",
            Gender           = "Female",
            Email            = "dr.reham.amer.ismailia.intmed@ehrs-clinic.eg",
            ContactNumber    = "01010000216",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00216-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Royal Medical Center",
            About            = "Experienced Internal Medicine Specialist based in Ismailia governorate with over 5 years of clinical practice.",
        }, "EG-00216-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Shalaby",
            Gender           = "Male",
            Email            = "dr.hassan.shalaby.ismailia.dent@ehrs-clinic.eg",
            ContactNumber    = "01110000217",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00217-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia University Hospital",
            About            = "Experienced Dentist based in Ismailia governorate with over 13 years of clinical practice.",
        }, "EG-00217-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Omar Zaki",
            Gender           = "Male",
            Email            = "dr.omar.zaki.ismailia.ent@ehrs-clinic.eg",
            ContactNumber    = "01210000218",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00218-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia As-Salam International Hospital",
            About            = "Experienced ENT Specialist based in Ismailia governorate with over 21 years of clinical practice.",
        }, "EG-00218-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Abeer Safwat",
            Gender           = "Female",
            Email            = "dr.abeer.safwat.ismailia.uro@ehrs-clinic.eg",
            ContactNumber    = "01510000219",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00219-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Royal Medical Center",
            About            = "Experienced Urology Specialist based in Ismailia governorate with over 10 years of clinical practice.",
        }, "EG-00219-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Essam Naguib",
            Gender           = "Male",
            Email            = "dr.essam.naguib.ismailia.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01010000220",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00220-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Central Medical Center",
            About            = "Experienced Ophthalmology Specialist based in Ismailia governorate with over 20 years of clinical practice.",
        }, "EG-00220-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ehab Aziz",
            Gender           = "Male",
            Email            = "dr.ehab.aziz.ismailia.rheum@ehrs-clinic.eg",
            ContactNumber    = "01110000221",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00221-MD",
            Area             = "Ismailia",
            AffiliatedHospital = "Ismailia Dar Al-Shifa Hospital",
            About            = "Experienced Rheumatology Specialist based in Ismailia governorate with over 15 years of clinical practice.",
        }, "EG-00221-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samira Omar",
            Gender           = "Female",
            Email            = "dr.samira.omar.kafrelsheikh.gp@ehrs-clinic.eg",
            ContactNumber    = "01210000222",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00222-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Al-Ahrar Hospital",
            About            = "Experienced General Practitioner (GP) based in Kafr El Sheikh governorate with over 15 years of clinical practice.",
        }, "EG-00222-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nader Naguib",
            Gender           = "Male",
            Email            = "dr.nader.naguib.kafrelsheikh.peds@ehrs-clinic.eg",
            ContactNumber    = "01510000223",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00223-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Al-Maadi Hospital",
            About            = "Experienced Pediatrics Specialist based in Kafr El Sheikh governorate with over 17 years of clinical practice.",
        }, "EG-00223-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Emad Mahmoud",
            Gender           = "Male",
            Email            = "dr.emad.mahmoud.kafrelsheikh.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01010000224",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00224-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Cairo University Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Kafr El Sheikh governorate with over 7 years of clinical practice.",
        }, "EG-00224-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Amira Gohar",
            Gender           = "Female",
            Email            = "dr.amira.gohar.kafrelsheikh.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01110000225",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00225-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Dar Al-Shifa Hospital",
            About            = "Experienced General Surgery Specialist based in Kafr El Sheikh governorate with over 8 years of clinical practice.",
        }, "EG-00225-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ola Tantawi",
            Gender           = "Female",
            Email            = "dr.ola.tantawi.kafrelsheikh.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01210000226",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00226-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh University Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Kafr El Sheikh governorate with over 22 years of clinical practice.",
        }, "EG-00226-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bassem Aziz",
            Gender           = "Male",
            Email            = "dr.bassem.aziz.kafrelsheikh.cardio@ehrs-clinic.eg",
            ContactNumber    = "01510000227",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00227-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh General Hospital",
            About            = "Experienced Cardiology Specialist based in Kafr El Sheikh governorate with over 11 years of clinical practice.",
        }, "EG-00227-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Doaa Fouad",
            Gender           = "Female",
            Email            = "dr.doaa.fouad.kafrelsheikh.neuro@ehrs-clinic.eg",
            ContactNumber    = "01010000228",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00228-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh As-Salam International Hospital",
            About            = "Experienced Neurology Specialist based in Kafr El Sheikh governorate with over 25 years of clinical practice.",
        }, "EG-00228-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ashraf Ali",
            Gender           = "Male",
            Email            = "dr.ashraf.ali.kafrelsheikh.vasc@ehrs-clinic.eg",
            ContactNumber    = "01110000229",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00229-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Al-Zahraa Hospital",
            About            = "Experienced Vascular Specialist based in Kafr El Sheikh governorate with over 13 years of clinical practice.",
        }, "EG-00229-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Emad Farouk",
            Gender           = "Male",
            Email            = "dr.emad.farouk.kafrelsheikh.pulm@ehrs-clinic.eg",
            ContactNumber    = "01210000230",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00230-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Al-Maadi Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Kafr El Sheikh governorate with over 19 years of clinical practice.",
        }, "EG-00230-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Naguib",
            Gender           = "Female",
            Email            = "dr.manar.naguib.kafrelsheikh.ortho@ehrs-clinic.eg",
            ContactNumber    = "01510000231",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00231-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Nile Medical Center",
            About            = "Experienced Orthopedic Specialist based in Kafr El Sheikh governorate with over 5 years of clinical practice.",
        }, "EG-00231-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Mansour",
            Gender           = "Male",
            Email            = "dr.ayman.mansour.kafrelsheikh.derm@ehrs-clinic.eg",
            ContactNumber    = "01010000232",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00232-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Al-Maadi Hospital",
            About            = "Experienced Dermatology Specialist based in Kafr El Sheikh governorate with over 22 years of clinical practice.",
        }, "EG-00232-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ahmed Barakat",
            Gender           = "Male",
            Email            = "dr.ahmed.barakat.kafrelsheikh.intmed@ehrs-clinic.eg",
            ContactNumber    = "01110000233",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00233-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Ain Shams University Hospital",
            About            = "Experienced Internal Medicine Specialist based in Kafr El Sheikh governorate with over 7 years of clinical practice.",
        }, "EG-00233-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rania Khalil",
            Gender           = "Female",
            Email            = "dr.rania.khalil.kafrelsheikh.dent@ehrs-clinic.eg",
            ContactNumber    = "01210000234",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00234-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Cairo University Hospital",
            About            = "Experienced Dentist based in Kafr El Sheikh governorate with over 8 years of clinical practice.",
        }, "EG-00234-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Naguib",
            Gender           = "Male",
            Email            = "dr.tamer.naguib.kafrelsheikh.ent@ehrs-clinic.eg",
            ContactNumber    = "01510000235",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00235-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Al-Maadi Hospital",
            About            = "Experienced ENT Specialist based in Kafr El Sheikh governorate with over 21 years of clinical practice.",
        }, "EG-00235-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Adel Aziz",
            Gender           = "Male",
            Email            = "dr.adel.aziz.kafrelsheikh.uro@ehrs-clinic.eg",
            ContactNumber    = "01010000236",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00236-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh As-Salam International Hospital",
            About            = "Experienced Urology Specialist based in Kafr El Sheikh governorate with over 20 years of clinical practice.",
        }, "EG-00236-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rania Halim",
            Gender           = "Female",
            Email            = "dr.rania.halim.kafrelsheikh.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01110000237",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00237-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Al-Salam Hospital",
            About            = "Experienced Ophthalmology Specialist based in Kafr El Sheikh governorate with over 17 years of clinical practice.",
        }, "EG-00237-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tarek Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.tarek.abdelrahman.kafrelsheikh.rheum@ehrs-clinic.eg",
            ContactNumber    = "01210000238",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00238-MD",
            Area             = "Kafr El Sheikh",
            AffiliatedHospital = "Kafr El Sheikh Al-Salam Hospital",
            About            = "Experienced Rheumatology Specialist based in Kafr El Sheikh governorate with over 7 years of clinical practice.",
        }, "EG-00238-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Adel Aziz",
            Gender           = "Male",
            Email            = "dr.adel.aziz.luxor.gp@ehrs-clinic.eg",
            ContactNumber    = "01510000239",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00239-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Dar Al-Shifa Hospital",
            About            = "Experienced General Practitioner (GP) based in Luxor governorate with over 21 years of clinical practice.",
        }, "EG-00239-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Yasmin Hassan",
            Gender           = "Female",
            Email            = "dr.yasmin.hassan.luxor.peds@ehrs-clinic.eg",
            ContactNumber    = "01010000240",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00240-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Al-Maadi Hospital",
            About            = "Experienced Pediatrics Specialist based in Luxor governorate with over 14 years of clinical practice.",
        }, "EG-00240-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Fady Saad",
            Gender           = "Male",
            Email            = "dr.fady.saad.luxor.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01110000241",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00241-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Cairo University Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Luxor governorate with over 22 years of clinical practice.",
        }, "EG-00241-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Atef Gaber",
            Gender           = "Male",
            Email            = "dr.atef.gaber.luxor.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01210000242",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00242-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Dar Al-Shifa Hospital",
            About            = "Experienced General Surgery Specialist based in Luxor governorate with over 22 years of clinical practice.",
        }, "EG-00242-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ola Ghoneim",
            Gender           = "Female",
            Email            = "dr.ola.ghoneim.luxor.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01510000243",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00243-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Cleopatra Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Luxor governorate with over 19 years of clinical practice.",
        }, "EG-00243-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Wael Nasser",
            Gender           = "Male",
            Email            = "dr.wael.nasser.luxor.cardio@ehrs-clinic.eg",
            ContactNumber    = "01010000244",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00244-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Al-Helal Medical Complex",
            About            = "Experienced Cardiology Specialist based in Luxor governorate with over 23 years of clinical practice.",
        }, "EG-00244-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hany Shalaby",
            Gender           = "Male",
            Email            = "dr.hany.shalaby.luxor.neuro@ehrs-clinic.eg",
            ContactNumber    = "01110000245",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00245-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Ain Shams University Hospital",
            About            = "Experienced Neurology Specialist based in Luxor governorate with over 6 years of clinical practice.",
        }, "EG-00245-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Amira Kamal",
            Gender           = "Female",
            Email            = "dr.amira.kamal.luxor.vasc@ehrs-clinic.eg",
            ContactNumber    = "01210000246",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00246-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Cleopatra Hospital",
            About            = "Experienced Vascular Specialist based in Luxor governorate with over 17 years of clinical practice.",
        }, "EG-00246-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Naguib",
            Gender           = "Male",
            Email            = "dr.tamer.naguib.luxor.pulm@ehrs-clinic.eg",
            ContactNumber    = "01510000247",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00247-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor General Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Luxor governorate with over 9 years of clinical practice.",
        }, "EG-00247-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Gerges Selim",
            Gender           = "Male",
            Email            = "dr.gerges.selim.luxor.ortho@ehrs-clinic.eg",
            ContactNumber    = "01010000248",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00248-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Dar Al-Shifa Hospital",
            About            = "Experienced Orthopedic Specialist based in Luxor governorate with over 8 years of clinical practice.",
        }, "EG-00248-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ghada Omar",
            Gender           = "Female",
            Email            = "dr.ghada.omar.luxor.derm@ehrs-clinic.eg",
            ContactNumber    = "01110000249",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00249-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Cairo University Hospital",
            About            = "Experienced Dermatology Specialist based in Luxor governorate with over 5 years of clinical practice.",
        }, "EG-00249-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Aziz",
            Gender           = "Male",
            Email            = "dr.tamer.aziz.luxor.intmed@ehrs-clinic.eg",
            ContactNumber    = "01210000250",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00250-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Al-Salam Hospital",
            About            = "Experienced Internal Medicine Specialist based in Luxor governorate with over 7 years of clinical practice.",
        }, "EG-00250-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Atef Gohar",
            Gender           = "Male",
            Email            = "dr.atef.gohar.luxor.dent@ehrs-clinic.eg",
            ContactNumber    = "01510000251",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00251-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Dar Al-Shifa Hospital",
            About            = "Experienced Dentist based in Luxor governorate with over 24 years of clinical practice.",
        }, "EG-00251-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Tantawi",
            Gender           = "Female",
            Email            = "dr.manar.tantawi.luxor.ent@ehrs-clinic.eg",
            ContactNumber    = "01010000252",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00252-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Central Medical Center",
            About            = "Experienced ENT Specialist based in Luxor governorate with over 15 years of clinical practice.",
        }, "EG-00252-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bishoy Fawzy",
            Gender           = "Male",
            Email            = "dr.bishoy.fawzy.luxor.uro@ehrs-clinic.eg",
            ContactNumber    = "01110000253",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00253-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Dar Al-Shifa Hospital",
            About            = "Experienced Urology Specialist based in Luxor governorate with over 25 years of clinical practice.",
        }, "EG-00253-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Fady Ghoneim",
            Gender           = "Male",
            Email            = "dr.fady.ghoneim.luxor.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01210000254",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00254-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor General Hospital",
            About            = "Experienced Ophthalmology Specialist based in Luxor governorate with over 24 years of clinical practice.",
        }, "EG-00254-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rana Abdalla",
            Gender           = "Female",
            Email            = "dr.rana.abdalla.luxor.rheum@ehrs-clinic.eg",
            ContactNumber    = "01510000255",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00255-MD",
            Area             = "Luxor",
            AffiliatedHospital = "Luxor Al-Maadi Hospital",
            About            = "Experienced Rheumatology Specialist based in Luxor governorate with over 12 years of clinical practice.",
        }, "EG-00255-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Hamdy",
            Gender           = "Male",
            Email            = "dr.hassan.hamdy.matruh.gp@ehrs-clinic.eg",
            ContactNumber    = "01010000256",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00256-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Nile Medical Center",
            About            = "Experienced General Practitioner (GP) based in Matruh governorate with over 25 years of clinical practice.",
        }, "EG-00256-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ibrahim Safwat",
            Gender           = "Male",
            Email            = "dr.ibrahim.safwat.matruh.peds@ehrs-clinic.eg",
            ContactNumber    = "01110000257",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00257-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Al-Ahrar Hospital",
            About            = "Experienced Pediatrics Specialist based in Matruh governorate with over 14 years of clinical practice.",
        }, "EG-00257-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nada Mahmoud",
            Gender           = "Female",
            Email            = "dr.nada.mahmoud.matruh.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01210000258",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00258-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Dar Al-Shifa Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Matruh governorate with over 6 years of clinical practice.",
        }, "EG-00258-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samer Gaber",
            Gender           = "Male",
            Email            = "dr.samer.gaber.matruh.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01510000259",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00259-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Al-Wadi Hospital",
            About            = "Experienced General Surgery Specialist based in Matruh governorate with over 18 years of clinical practice.",
        }, "EG-00259-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sara Abdalla",
            Gender           = "Female",
            Email            = "dr.sara.abdalla.matruh.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01010000260",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00260-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Ain Shams University Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Matruh governorate with over 23 years of clinical practice.",
        }, "EG-00260-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samira Zaki",
            Gender           = "Female",
            Email            = "dr.samira.zaki.matruh.cardio@ehrs-clinic.eg",
            ContactNumber    = "01110000261",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00261-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Al-Ahrar Hospital",
            About            = "Experienced Cardiology Specialist based in Matruh governorate with over 10 years of clinical practice.",
        }, "EG-00261-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Morsi",
            Gender           = "Male",
            Email            = "dr.hassan.morsi.matruh.neuro@ehrs-clinic.eg",
            ContactNumber    = "01210000262",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00262-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Cleopatra Hospital",
            About            = "Experienced Neurology Specialist based in Matruh governorate with over 24 years of clinical practice.",
        }, "EG-00262-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Naguib",
            Gender           = "Male",
            Email            = "dr.ayman.naguib.matruh.vasc@ehrs-clinic.eg",
            ContactNumber    = "01510000263",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00263-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Al-Salam Hospital",
            About            = "Experienced Vascular Specialist based in Matruh governorate with over 12 years of clinical practice.",
        }, "EG-00263-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ghada Gohar",
            Gender           = "Female",
            Email            = "dr.ghada.gohar.matruh.pulm@ehrs-clinic.eg",
            ContactNumber    = "01010000264",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00264-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Cairo University Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Matruh governorate with over 13 years of clinical practice.",
        }, "EG-00264-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ahmed Halim",
            Gender           = "Male",
            Email            = "dr.ahmed.halim.matruh.ortho@ehrs-clinic.eg",
            ContactNumber    = "01110000265",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00265-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Al-Maadi Hospital",
            About            = "Experienced Orthopedic Specialist based in Matruh governorate with over 22 years of clinical practice.",
        }, "EG-00265-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sherif Mohamed",
            Gender           = "Male",
            Email            = "dr.sherif.mohamed.matruh.derm@ehrs-clinic.eg",
            ContactNumber    = "01210000266",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00266-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Cairo University Hospital",
            About            = "Experienced Dermatology Specialist based in Matruh governorate with over 16 years of clinical practice.",
        }, "EG-00266-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nihal Saleh",
            Gender           = "Female",
            Email            = "dr.nihal.saleh.matruh.intmed@ehrs-clinic.eg",
            ContactNumber    = "01510000267",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00267-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Ain Shams University Hospital",
            About            = "Experienced Internal Medicine Specialist based in Matruh governorate with over 13 years of clinical practice.",
        }, "EG-00267-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bassem Saleh",
            Gender           = "Male",
            Email            = "dr.bassem.saleh.matruh.dent@ehrs-clinic.eg",
            ContactNumber    = "01010000268",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00268-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Al-Zahraa Hospital",
            About            = "Experienced Dentist based in Matruh governorate with over 17 years of clinical practice.",
        }, "EG-00268-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Atef Omar",
            Gender           = "Male",
            Email            = "dr.atef.omar.matruh.ent@ehrs-clinic.eg",
            ContactNumber    = "01110000269",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00269-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Al-Helal Medical Complex",
            About            = "Experienced ENT Specialist based in Matruh governorate with over 17 years of clinical practice.",
        }, "EG-00269-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nihal Gaber",
            Gender           = "Female",
            Email            = "dr.nihal.gaber.matruh.uro@ehrs-clinic.eg",
            ContactNumber    = "01210000270",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00270-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Al-Maadi Hospital",
            About            = "Experienced Urology Specialist based in Matruh governorate with over 14 years of clinical practice.",
        }, "EG-00270-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mohamed Tantawi",
            Gender           = "Male",
            Email            = "dr.mohamed.tantawi.matruh.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01510000271",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00271-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh Royal Medical Center",
            About            = "Experienced Ophthalmology Specialist based in Matruh governorate with over 5 years of clinical practice.",
        }, "EG-00271-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mahmoud Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.mahmoud.abdelrahman.matruh.rheum@ehrs-clinic.eg",
            ContactNumber    = "01010000272",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00272-MD",
            Area             = "Matruh",
            AffiliatedHospital = "Matruh As-Salam International Hospital",
            About            = "Experienced Rheumatology Specialist based in Matruh governorate with over 14 years of clinical practice.",
        }, "EG-00272-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ola Shalaby",
            Gender           = "Female",
            Email            = "dr.ola.shalaby.minya.gp@ehrs-clinic.eg",
            ContactNumber    = "01110000273",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00273-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Al-Wadi Hospital",
            About            = "Experienced General Practitioner (GP) based in Minya governorate with over 12 years of clinical practice.",
        }, "EG-00273-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tarek Morsi",
            Gender           = "Male",
            Email            = "dr.tarek.morsi.minya.peds@ehrs-clinic.eg",
            ContactNumber    = "01210000274",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00274-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Royal Medical Center",
            About            = "Experienced Pediatrics Specialist based in Minya governorate with over 9 years of clinical practice.",
        }, "EG-00274-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ibrahim Mahmoud",
            Gender           = "Male",
            Email            = "dr.ibrahim.mahmoud.minya.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01510000275",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00275-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Al-Maadi Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Minya governorate with over 19 years of clinical practice.",
        }, "EG-00275-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mariam Selim",
            Gender           = "Female",
            Email            = "dr.mariam.selim.minya.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01010000276",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00276-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Al-Wadi Hospital",
            About            = "Experienced General Surgery Specialist based in Minya governorate with over 9 years of clinical practice.",
        }, "EG-00276-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rana Ragab",
            Gender           = "Female",
            Email            = "dr.rana.ragab.minya.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01110000277",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00277-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Dar Al-Shifa Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Minya governorate with over 18 years of clinical practice.",
        }, "EG-00277-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Walid Nasser",
            Gender           = "Male",
            Email            = "dr.walid.nasser.minya.cardio@ehrs-clinic.eg",
            ContactNumber    = "01210000278",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00278-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Al-Salam Hospital",
            About            = "Experienced Cardiology Specialist based in Minya governorate with over 22 years of clinical practice.",
        }, "EG-00278-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hana Shehata",
            Gender           = "Female",
            Email            = "dr.hana.shehata.minya.neuro@ehrs-clinic.eg",
            ContactNumber    = "01510000279",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00279-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Royal Medical Center",
            About            = "Experienced Neurology Specialist based in Minya governorate with over 10 years of clinical practice.",
        }, "EG-00279-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Maged Kamal",
            Gender           = "Male",
            Email            = "dr.maged.kamal.minya.vasc@ehrs-clinic.eg",
            ContactNumber    = "01010000280",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00280-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Al-Maadi Hospital",
            About            = "Experienced Vascular Specialist based in Minya governorate with over 15 years of clinical practice.",
        }, "EG-00280-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Youssef Halim",
            Gender           = "Male",
            Email            = "dr.youssef.halim.minya.pulm@ehrs-clinic.eg",
            ContactNumber    = "01110000281",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00281-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Central Medical Center",
            About            = "Experienced Chest Specialist (Pulmonology) based in Minya governorate with over 9 years of clinical practice.",
        }, "EG-00281-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ola Shalaby",
            Gender           = "Female",
            Email            = "dr.ola.shalaby.minya.ortho@ehrs-clinic.eg",
            ContactNumber    = "01210000282",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00282-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Cleopatra Hospital",
            About            = "Experienced Orthopedic Specialist based in Minya governorate with over 22 years of clinical practice.",
        }, "EG-00282-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Ahmed",
            Gender           = "Male",
            Email            = "dr.karim.ahmed.minya.derm@ehrs-clinic.eg",
            ContactNumber    = "01510000283",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00283-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Cleopatra Hospital",
            About            = "Experienced Dermatology Specialist based in Minya governorate with over 5 years of clinical practice.",
        }, "EG-00283-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Maged Ghoneim",
            Gender           = "Male",
            Email            = "dr.maged.ghoneim.minya.intmed@ehrs-clinic.eg",
            ContactNumber    = "01010000284",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00284-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Nile Medical Center",
            About            = "Experienced Internal Medicine Specialist based in Minya governorate with over 19 years of clinical practice.",
        }, "EG-00284-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hana Gohar",
            Gender           = "Female",
            Email            = "dr.hana.gohar.minya.dent@ehrs-clinic.eg",
            ContactNumber    = "01110000285",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00285-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Cleopatra Hospital",
            About            = "Experienced Dentist based in Minya governorate with over 25 years of clinical practice.",
        }, "EG-00285-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Omar",
            Gender           = "Male",
            Email            = "dr.karim.omar.minya.ent@ehrs-clinic.eg",
            ContactNumber    = "01210000286",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00286-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Al-Helal Medical Complex",
            About            = "Experienced ENT Specialist based in Minya governorate with over 20 years of clinical practice.",
        }, "EG-00286-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mohamed Morsi",
            Gender           = "Male",
            Email            = "dr.mohamed.morsi.minya.uro@ehrs-clinic.eg",
            ContactNumber    = "01510000287",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00287-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Dar Al-Shifa Hospital",
            About            = "Experienced Urology Specialist based in Minya governorate with over 24 years of clinical practice.",
        }, "EG-00287-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rania Mohamed",
            Gender           = "Female",
            Email            = "dr.rania.mohamed.minya.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01010000288",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00288-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Cairo University Hospital",
            About            = "Experienced Ophthalmology Specialist based in Minya governorate with over 14 years of clinical practice.",
        }, "EG-00288-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nader Khalil",
            Gender           = "Male",
            Email            = "dr.nader.khalil.minya.rheum@ehrs-clinic.eg",
            ContactNumber    = "01110000289",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00289-MD",
            Area             = "Minya",
            AffiliatedHospital = "Minya Al-Salam Hospital",
            About            = "Experienced Rheumatology Specialist based in Minya governorate with over 6 years of clinical practice.",
        }, "EG-00289-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Omar Saleh",
            Gender           = "Male",
            Email            = "dr.omar.saleh.monufia.gp@ehrs-clinic.eg",
            ContactNumber    = "01210000290",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00290-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia As-Salam International Hospital",
            About            = "Experienced General Practitioner (GP) based in Monufia governorate with over 8 years of clinical practice.",
        }, "EG-00290-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Dalia Abdalla",
            Gender           = "Female",
            Email            = "dr.dalia.abdalla.monufia.peds@ehrs-clinic.eg",
            ContactNumber    = "01510000291",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00291-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Al-Salam Hospital",
            About            = "Experienced Pediatrics Specialist based in Monufia governorate with over 17 years of clinical practice.",
        }, "EG-00291-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bassem Fouad",
            Gender           = "Male",
            Email            = "dr.bassem.fouad.monufia.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01010000292",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00292-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Ain Shams University Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Monufia governorate with over 23 years of clinical practice.",
        }, "EG-00292-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Aziz",
            Gender           = "Male",
            Email            = "dr.tamer.aziz.monufia.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01110000293",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00293-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Nile Medical Center",
            About            = "Experienced General Surgery Specialist based in Monufia governorate with over 20 years of clinical practice.",
        }, "EG-00293-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Faten Aziz",
            Gender           = "Female",
            Email            = "dr.faten.aziz.monufia.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01210000294",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00294-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Royal Medical Center",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Monufia governorate with over 6 years of clinical practice.",
        }, "EG-00294-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Ramadan",
            Gender           = "Male",
            Email            = "dr.karim.ramadan.monufia.cardio@ehrs-clinic.eg",
            ContactNumber    = "01510000295",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00295-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Cairo University Hospital",
            About            = "Experienced Cardiology Specialist based in Monufia governorate with over 19 years of clinical practice.",
        }, "EG-00295-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ayman Fouad",
            Gender           = "Male",
            Email            = "dr.ayman.fouad.monufia.neuro@ehrs-clinic.eg",
            ContactNumber    = "01010000296",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00296-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Nile Medical Center",
            About            = "Experienced Neurology Specialist based in Monufia governorate with over 16 years of clinical practice.",
        }, "EG-00296-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Eman Gaber",
            Gender           = "Female",
            Email            = "dr.eman.gaber.monufia.vasc@ehrs-clinic.eg",
            ContactNumber    = "01110000297",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00297-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia General Hospital",
            About            = "Experienced Vascular Specialist based in Monufia governorate with over 17 years of clinical practice.",
        }, "EG-00297-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Adel Nasser",
            Gender           = "Male",
            Email            = "dr.adel.nasser.monufia.pulm@ehrs-clinic.eg",
            ContactNumber    = "01210000298",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00298-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Nile Medical Center",
            About            = "Experienced Chest Specialist (Pulmonology) based in Monufia governorate with over 19 years of clinical practice.",
        }, "EG-00298-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Ramadan",
            Gender           = "Male",
            Email            = "dr.hassan.ramadan.monufia.ortho@ehrs-clinic.eg",
            ContactNumber    = "01510000299",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00299-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia University Hospital",
            About            = "Experienced Orthopedic Specialist based in Monufia governorate with over 6 years of clinical practice.",
        }, "EG-00299-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Shaimaa Badawi",
            Gender           = "Female",
            Email            = "dr.shaimaa.badawi.monufia.derm@ehrs-clinic.eg",
            ContactNumber    = "01010000300",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00300-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Al-Helal Medical Complex",
            About            = "Experienced Dermatology Specialist based in Monufia governorate with over 9 years of clinical practice.",
        }, "EG-00300-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hossam Mohamed",
            Gender           = "Male",
            Email            = "dr.hossam.mohamed.monufia.intmed@ehrs-clinic.eg",
            ContactNumber    = "01110000301",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00301-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Al-Zahraa Hospital",
            About            = "Experienced Internal Medicine Specialist based in Monufia governorate with over 23 years of clinical practice.",
        }, "EG-00301-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hossam Shalaby",
            Gender           = "Male",
            Email            = "dr.hossam.shalaby.monufia.dent@ehrs-clinic.eg",
            ContactNumber    = "01210000302",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00302-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Dar Al-Shifa Hospital",
            About            = "Experienced Dentist based in Monufia governorate with over 9 years of clinical practice.",
        }, "EG-00302-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Shaimaa Abdel-Rahman",
            Gender           = "Female",
            Email            = "dr.shaimaa.abdelrahman.monufia.ent@ehrs-clinic.eg",
            ContactNumber    = "01510000303",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00303-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia University Hospital",
            About            = "Experienced ENT Specialist based in Monufia governorate with over 13 years of clinical practice.",
        }, "EG-00303-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Farouk",
            Gender           = "Male",
            Email            = "dr.tamer.farouk.monufia.uro@ehrs-clinic.eg",
            ContactNumber    = "01010000304",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00304-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Royal Medical Center",
            About            = "Experienced Urology Specialist based in Monufia governorate with over 10 years of clinical practice.",
        }, "EG-00304-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Youssef Ibrahim",
            Gender           = "Male",
            Email            = "dr.youssef.ibrahim.monufia.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01110000305",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00305-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Al-Salam Hospital",
            About            = "Experienced Ophthalmology Specialist based in Monufia governorate with over 5 years of clinical practice.",
        }, "EG-00305-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hana Abdalla",
            Gender           = "Female",
            Email            = "dr.hana.abdalla.monufia.rheum@ehrs-clinic.eg",
            ContactNumber    = "01210000306",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00306-MD",
            Area             = "Monufia",
            AffiliatedHospital = "Monufia Dar Al-Shifa Hospital",
            About            = "Experienced Rheumatology Specialist based in Monufia governorate with over 5 years of clinical practice.",
        }, "EG-00306-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Walid Gohar",
            Gender           = "Male",
            Email            = "dr.walid.gohar.newvalley.gp@ehrs-clinic.eg",
            ContactNumber    = "01510000307",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00307-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley General Hospital",
            About            = "Experienced General Practitioner (GP) based in New Valley governorate with over 9 years of clinical practice.",
        }, "EG-00307-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nader Shehata",
            Gender           = "Male",
            Email            = "dr.nader.shehata.newvalley.peds@ehrs-clinic.eg",
            ContactNumber    = "01010000308",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00308-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Nile Medical Center",
            About            = "Experienced Pediatrics Specialist based in New Valley governorate with over 7 years of clinical practice.",
        }, "EG-00308-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Lobna Safwat",
            Gender           = "Female",
            Email            = "dr.lobna.safwat.newvalley.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01110000309",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00309-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Al-Wadi Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in New Valley governorate with over 21 years of clinical practice.",
        }, "EG-00309-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ibrahim Safwat",
            Gender           = "Male",
            Email            = "dr.ibrahim.safwat.newvalley.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01210000310",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00310-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Al-Helal Medical Complex",
            About            = "Experienced General Surgery Specialist based in New Valley governorate with over 24 years of clinical practice.",
        }, "EG-00310-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mariam Shehata",
            Gender           = "Female",
            Email            = "dr.mariam.shehata.newvalley.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01510000311",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00311-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Al-Maadi Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in New Valley governorate with over 19 years of clinical practice.",
        }, "EG-00311-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Abeer Ibrahim",
            Gender           = "Female",
            Email            = "dr.abeer.ibrahim.newvalley.cardio@ehrs-clinic.eg",
            ContactNumber    = "01010000312",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00312-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley General Hospital",
            About            = "Experienced Cardiology Specialist based in New Valley governorate with over 20 years of clinical practice.",
        }, "EG-00312-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samir Hamdy",
            Gender           = "Male",
            Email            = "dr.samir.hamdy.newvalley.neuro@ehrs-clinic.eg",
            ContactNumber    = "01110000313",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00313-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Nile Medical Center",
            About            = "Experienced Neurology Specialist based in New Valley governorate with over 20 years of clinical practice.",
        }, "EG-00313-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ashraf Mohamed",
            Gender           = "Male",
            Email            = "dr.ashraf.mohamed.newvalley.vasc@ehrs-clinic.eg",
            ContactNumber    = "01210000314",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00314-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Central Medical Center",
            About            = "Experienced Vascular Specialist based in New Valley governorate with over 15 years of clinical practice.",
        }, "EG-00314-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Faten Saad",
            Gender           = "Female",
            Email            = "dr.faten.saad.newvalley.pulm@ehrs-clinic.eg",
            ContactNumber    = "01510000315",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00315-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Central Medical Center",
            About            = "Experienced Chest Specialist (Pulmonology) based in New Valley governorate with over 9 years of clinical practice.",
        }, "EG-00315-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Adel Morsi",
            Gender           = "Male",
            Email            = "dr.adel.morsi.newvalley.ortho@ehrs-clinic.eg",
            ContactNumber    = "01010000316",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00316-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Dar Al-Shifa Hospital",
            About            = "Experienced Orthopedic Specialist based in New Valley governorate with over 17 years of clinical practice.",
        }, "EG-00316-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mina Ragab",
            Gender           = "Male",
            Email            = "dr.mina.ragab.newvalley.derm@ehrs-clinic.eg",
            ContactNumber    = "01110000317",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00317-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Cairo University Hospital",
            About            = "Experienced Dermatology Specialist based in New Valley governorate with over 21 years of clinical practice.",
        }, "EG-00317-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Faten Hamdy",
            Gender           = "Female",
            Email            = "dr.faten.hamdy.newvalley.intmed@ehrs-clinic.eg",
            ContactNumber    = "01210000318",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00318-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Nile Medical Center",
            About            = "Experienced Internal Medicine Specialist based in New Valley governorate with over 8 years of clinical practice.",
        }, "EG-00318-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Emad Ramadan",
            Gender           = "Male",
            Email            = "dr.emad.ramadan.newvalley.dent@ehrs-clinic.eg",
            ContactNumber    = "01510000319",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00319-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Ain Shams University Hospital",
            About            = "Experienced Dentist based in New Valley governorate with over 19 years of clinical practice.",
        }, "EG-00319-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mostafa Aziz",
            Gender           = "Male",
            Email            = "dr.mostafa.aziz.newvalley.ent@ehrs-clinic.eg",
            ContactNumber    = "01010000320",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00320-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Dar Al-Shifa Hospital",
            About            = "Experienced ENT Specialist based in New Valley governorate with over 19 years of clinical practice.",
        }, "EG-00320-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Shereen Aziz",
            Gender           = "Female",
            Email            = "dr.shereen.aziz.newvalley.uro@ehrs-clinic.eg",
            ContactNumber    = "01110000321",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00321-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Nile Medical Center",
            About            = "Experienced Urology Specialist based in New Valley governorate with over 15 years of clinical practice.",
        }, "EG-00321-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Essam Amer",
            Gender           = "Male",
            Email            = "dr.essam.amer.newvalley.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01210000322",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00322-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Royal Medical Center",
            About            = "Experienced Ophthalmology Specialist based in New Valley governorate with over 16 years of clinical practice.",
        }, "EG-00322-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Kamal",
            Gender           = "Male",
            Email            = "dr.tamer.kamal.newvalley.rheum@ehrs-clinic.eg",
            ContactNumber    = "01510000323",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00323-MD",
            Area             = "New Valley",
            AffiliatedHospital = "New Valley Central Medical Center",
            About            = "Experienced Rheumatology Specialist based in New Valley governorate with over 7 years of clinical practice.",
        }, "EG-00323-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rana Ahmed",
            Gender           = "Female",
            Email            = "dr.rana.ahmed.northsinai.gp@ehrs-clinic.eg",
            ContactNumber    = "01010000324",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00324-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Ain Shams University Hospital",
            About            = "Experienced General Practitioner (GP) based in North Sinai governorate with over 8 years of clinical practice.",
        }, "EG-00324-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Farouk",
            Gender           = "Male",
            Email            = "dr.karim.farouk.northsinai.peds@ehrs-clinic.eg",
            ContactNumber    = "01110000325",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00325-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai General Hospital",
            About            = "Experienced Pediatrics Specialist based in North Sinai governorate with over 23 years of clinical practice.",
        }, "EG-00325-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Emad Barakat",
            Gender           = "Male",
            Email            = "dr.emad.barakat.northsinai.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01210000326",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00326-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Dar Al-Shifa Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in North Sinai governorate with over 8 years of clinical practice.",
        }, "EG-00326-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Neveen Gaber",
            Gender           = "Female",
            Email            = "dr.neveen.gaber.northsinai.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01510000327",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00327-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Ain Shams University Hospital",
            About            = "Experienced General Surgery Specialist based in North Sinai governorate with over 6 years of clinical practice.",
        }, "EG-00327-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Noha Abdel-Rahman",
            Gender           = "Female",
            Email            = "dr.noha.abdelrahman.northsinai.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01010000328",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00328-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Al-Maadi Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in North Sinai governorate with over 16 years of clinical practice.",
        }, "EG-00328-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ibrahim Metwally",
            Gender           = "Male",
            Email            = "dr.ibrahim.metwally.northsinai.cardio@ehrs-clinic.eg",
            ContactNumber    = "01110000329",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00329-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Al-Zahraa Hospital",
            About            = "Experienced Cardiology Specialist based in North Sinai governorate with over 9 years of clinical practice.",
        }, "EG-00329-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samira Kamal",
            Gender           = "Female",
            Email            = "dr.samira.kamal.northsinai.neuro@ehrs-clinic.eg",
            ContactNumber    = "01210000330",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00330-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Al-Helal Medical Complex",
            About            = "Experienced Neurology Specialist based in North Sinai governorate with over 8 years of clinical practice.",
        }, "EG-00330-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ehab Barakat",
            Gender           = "Male",
            Email            = "dr.ehab.barakat.northsinai.vasc@ehrs-clinic.eg",
            ContactNumber    = "01510000331",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00331-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Al-Wadi Hospital",
            About            = "Experienced Vascular Specialist based in North Sinai governorate with over 8 years of clinical practice.",
        }, "EG-00331-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Adel Metwally",
            Gender           = "Male",
            Email            = "dr.adel.metwally.northsinai.pulm@ehrs-clinic.eg",
            ContactNumber    = "01010000332",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00332-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Al-Helal Medical Complex",
            About            = "Experienced Chest Specialist (Pulmonology) based in North Sinai governorate with over 18 years of clinical practice.",
        }, "EG-00332-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Eman Morsi",
            Gender           = "Female",
            Email            = "dr.eman.morsi.northsinai.ortho@ehrs-clinic.eg",
            ContactNumber    = "01110000333",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00333-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai University Hospital",
            About            = "Experienced Orthopedic Specialist based in North Sinai governorate with over 24 years of clinical practice.",
        }, "EG-00333-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Adel Ibrahim",
            Gender           = "Male",
            Email            = "dr.adel.ibrahim.northsinai.derm@ehrs-clinic.eg",
            ContactNumber    = "01210000334",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00334-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Al-Ahrar Hospital",
            About            = "Experienced Dermatology Specialist based in North Sinai governorate with over 13 years of clinical practice.",
        }, "EG-00334-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Alaa Badawi",
            Gender           = "Male",
            Email            = "dr.alaa.badawi.northsinai.intmed@ehrs-clinic.eg",
            ContactNumber    = "01510000335",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00335-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Al-Wadi Hospital",
            About            = "Experienced Internal Medicine Specialist based in North Sinai governorate with over 5 years of clinical practice.",
        }, "EG-00335-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mona Saad",
            Gender           = "Female",
            Email            = "dr.mona.saad.northsinai.dent@ehrs-clinic.eg",
            ContactNumber    = "01010000336",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00336-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Cleopatra Hospital",
            About            = "Experienced Dentist based in North Sinai governorate with over 7 years of clinical practice.",
        }, "EG-00336-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Ibrahim",
            Gender           = "Male",
            Email            = "dr.tamer.ibrahim.northsinai.ent@ehrs-clinic.eg",
            ContactNumber    = "01110000337",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00337-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Central Medical Center",
            About            = "Experienced ENT Specialist based in North Sinai governorate with over 21 years of clinical practice.",
        }, "EG-00337-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hossam Fawzy",
            Gender           = "Male",
            Email            = "dr.hossam.fawzy.northsinai.uro@ehrs-clinic.eg",
            ContactNumber    = "01210000338",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00338-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Ain Shams University Hospital",
            About            = "Experienced Urology Specialist based in North Sinai governorate with over 19 years of clinical practice.",
        }, "EG-00338-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Amira Mansour",
            Gender           = "Female",
            Email            = "dr.amira.mansour.northsinai.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01510000339",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00339-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Al-Wadi Hospital",
            About            = "Experienced Ophthalmology Specialist based in North Sinai governorate with over 14 years of clinical practice.",
        }, "EG-00339-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Wael Metwally",
            Gender           = "Male",
            Email            = "dr.wael.metwally.northsinai.rheum@ehrs-clinic.eg",
            ContactNumber    = "01010000340",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00340-MD",
            Area             = "North Sinai",
            AffiliatedHospital = "North Sinai Central Medical Center",
            About            = "Experienced Rheumatology Specialist based in North Sinai governorate with over 6 years of clinical practice.",
        }, "EG-00340-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Mansour",
            Gender           = "Male",
            Email            = "dr.tamer.mansour.portsaid.gp@ehrs-clinic.eg",
            ContactNumber    = "01110000341",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00341-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said General Hospital",
            About            = "Experienced General Practitioner (GP) based in Port Said governorate with over 7 years of clinical practice.",
        }, "EG-00341-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Yasmin Safwat",
            Gender           = "Female",
            Email            = "dr.yasmin.safwat.portsaid.peds@ehrs-clinic.eg",
            ContactNumber    = "01210000342",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00342-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Ain Shams University Hospital",
            About            = "Experienced Pediatrics Specialist based in Port Said governorate with over 20 years of clinical practice.",
        }, "EG-00342-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ashraf Aziz",
            Gender           = "Male",
            Email            = "dr.ashraf.aziz.portsaid.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01510000343",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00343-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Royal Medical Center",
            About            = "Experienced Pediatric Surgery Specialist based in Port Said governorate with over 11 years of clinical practice.",
        }, "EG-00343-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Gerges Khalil",
            Gender           = "Male",
            Email            = "dr.gerges.khalil.portsaid.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01010000344",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00344-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Al-Wadi Hospital",
            About            = "Experienced General Surgery Specialist based in Port Said governorate with over 18 years of clinical practice.",
        }, "EG-00344-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Dalia Ragab",
            Gender           = "Female",
            Email            = "dr.dalia.ragab.portsaid.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01110000345",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00345-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said As-Salam International Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Port Said governorate with over 21 years of clinical practice.",
        }, "EG-00345-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Alaa Mahmoud",
            Gender           = "Male",
            Email            = "dr.alaa.mahmoud.portsaid.cardio@ehrs-clinic.eg",
            ContactNumber    = "01210000346",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00346-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Al-Helal Medical Complex",
            About            = "Experienced Cardiology Specialist based in Port Said governorate with over 17 years of clinical practice.",
        }, "EG-00346-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mahmoud Hassan",
            Gender           = "Male",
            Email            = "dr.mahmoud.hassan.portsaid.neuro@ehrs-clinic.eg",
            ContactNumber    = "01510000347",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00347-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Al-Zahraa Hospital",
            About            = "Experienced Neurology Specialist based in Port Said governorate with over 14 years of clinical practice.",
        }, "EG-00347-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Heba Farouk",
            Gender           = "Female",
            Email            = "dr.heba.farouk.portsaid.vasc@ehrs-clinic.eg",
            ContactNumber    = "01010000348",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00348-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Royal Medical Center",
            About            = "Experienced Vascular Specialist based in Port Said governorate with over 14 years of clinical practice.",
        }, "EG-00348-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Wael Khalil",
            Gender           = "Male",
            Email            = "dr.wael.khalil.portsaid.pulm@ehrs-clinic.eg",
            ContactNumber    = "01110000349",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00349-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said University Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Port Said governorate with over 20 years of clinical practice.",
        }, "EG-00349-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Essam Zaki",
            Gender           = "Male",
            Email            = "dr.essam.zaki.portsaid.ortho@ehrs-clinic.eg",
            ContactNumber    = "01210000350",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00350-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Al-Salam Hospital",
            About            = "Experienced Orthopedic Specialist based in Port Said governorate with over 17 years of clinical practice.",
        }, "EG-00350-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Eman Shalaby",
            Gender           = "Female",
            Email            = "dr.eman.shalaby.portsaid.derm@ehrs-clinic.eg",
            ContactNumber    = "01510000351",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00351-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Al-Wadi Hospital",
            About            = "Experienced Dermatology Specialist based in Port Said governorate with over 7 years of clinical practice.",
        }, "EG-00351-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samir Mahmoud",
            Gender           = "Male",
            Email            = "dr.samir.mahmoud.portsaid.intmed@ehrs-clinic.eg",
            ContactNumber    = "01010000352",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00352-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Ain Shams University Hospital",
            About            = "Experienced Internal Medicine Specialist based in Port Said governorate with over 5 years of clinical practice.",
        }, "EG-00352-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bassem Mohamed",
            Gender           = "Male",
            Email            = "dr.bassem.mohamed.portsaid.dent@ehrs-clinic.eg",
            ContactNumber    = "01110000353",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00353-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Dar Al-Shifa Hospital",
            About            = "Experienced Dentist based in Port Said governorate with over 23 years of clinical practice.",
        }, "EG-00353-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Neveen Metwally",
            Gender           = "Female",
            Email            = "dr.neveen.metwally.portsaid.ent@ehrs-clinic.eg",
            ContactNumber    = "01210000354",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00354-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Cleopatra Hospital",
            About            = "Experienced ENT Specialist based in Port Said governorate with over 25 years of clinical practice.",
        }, "EG-00354-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nader Ragab",
            Gender           = "Male",
            Email            = "dr.nader.ragab.portsaid.uro@ehrs-clinic.eg",
            ContactNumber    = "01510000355",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00355-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Nile Medical Center",
            About            = "Experienced Urology Specialist based in Port Said governorate with over 17 years of clinical practice.",
        }, "EG-00355-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mohamed Amer",
            Gender           = "Male",
            Email            = "dr.mohamed.amer.portsaid.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01010000356",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00356-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Al-Ahrar Hospital",
            About            = "Experienced Ophthalmology Specialist based in Port Said governorate with over 24 years of clinical practice.",
        }, "EG-00356-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ghada Fouad",
            Gender           = "Female",
            Email            = "dr.ghada.fouad.portsaid.rheum@ehrs-clinic.eg",
            ContactNumber    = "01110000357",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00357-MD",
            Area             = "Port Said",
            AffiliatedHospital = "Port Said Central Medical Center",
            About            = "Experienced Rheumatology Specialist based in Port Said governorate with over 18 years of clinical practice.",
        }, "EG-00357-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ibrahim Abdalla",
            Gender           = "Male",
            Email            = "dr.ibrahim.abdalla.qalyubia.gp@ehrs-clinic.eg",
            ContactNumber    = "01210000358",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00358-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Ain Shams University Hospital",
            About            = "Experienced General Practitioner (GP) based in Qalyubia governorate with over 23 years of clinical practice.",
        }, "EG-00358-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samir Shehata",
            Gender           = "Male",
            Email            = "dr.samir.shehata.qalyubia.peds@ehrs-clinic.eg",
            ContactNumber    = "01510000359",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00359-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Central Medical Center",
            About            = "Experienced Pediatrics Specialist based in Qalyubia governorate with over 17 years of clinical practice.",
        }, "EG-00359-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Noha Badawi",
            Gender           = "Female",
            Email            = "dr.noha.badawi.qalyubia.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01010000360",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00360-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Al-Helal Medical Complex",
            About            = "Experienced Pediatric Surgery Specialist based in Qalyubia governorate with over 15 years of clinical practice.",
        }, "EG-00360-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sherif Mohamed",
            Gender           = "Male",
            Email            = "dr.sherif.mohamed.qalyubia.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01110000361",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00361-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Nile Medical Center",
            About            = "Experienced General Surgery Specialist based in Qalyubia governorate with over 21 years of clinical practice.",
        }, "EG-00361-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Doaa Nasser",
            Gender           = "Female",
            Email            = "dr.doaa.nasser.qalyubia.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01210000362",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00362-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Al-Wadi Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Qalyubia governorate with over 16 years of clinical practice.",
        }, "EG-00362-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Saad",
            Gender           = "Female",
            Email            = "dr.reham.saad.qalyubia.cardio@ehrs-clinic.eg",
            ContactNumber    = "01510000363",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00363-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Al-Helal Medical Complex",
            About            = "Experienced Cardiology Specialist based in Qalyubia governorate with over 8 years of clinical practice.",
        }, "EG-00363-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Gohar",
            Gender           = "Male",
            Email            = "dr.tamer.gohar.qalyubia.neuro@ehrs-clinic.eg",
            ContactNumber    = "01010000364",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00364-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Al-Zahraa Hospital",
            About            = "Experienced Neurology Specialist based in Qalyubia governorate with over 10 years of clinical practice.",
        }, "EG-00364-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Mohamed",
            Gender           = "Male",
            Email            = "dr.tamer.mohamed.qalyubia.vasc@ehrs-clinic.eg",
            ContactNumber    = "01110000365",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00365-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Al-Ahrar Hospital",
            About            = "Experienced Vascular Specialist based in Qalyubia governorate with over 25 years of clinical practice.",
        }, "EG-00365-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Lobna Halim",
            Gender           = "Female",
            Email            = "dr.lobna.halim.qalyubia.pulm@ehrs-clinic.eg",
            ContactNumber    = "01210000366",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00366-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Cairo University Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Qalyubia governorate with over 23 years of clinical practice.",
        }, "EG-00366-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Wael Amer",
            Gender           = "Male",
            Email            = "dr.wael.amer.qalyubia.ortho@ehrs-clinic.eg",
            ContactNumber    = "01510000367",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00367-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Al-Salam Hospital",
            About            = "Experienced Orthopedic Specialist based in Qalyubia governorate with over 19 years of clinical practice.",
        }, "EG-00367-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Khaled Kamal",
            Gender           = "Male",
            Email            = "dr.khaled.kamal.qalyubia.derm@ehrs-clinic.eg",
            ContactNumber    = "01010000368",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00368-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Cairo University Hospital",
            About            = "Experienced Dermatology Specialist based in Qalyubia governorate with over 25 years of clinical practice.",
        }, "EG-00368-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Noha Lotfy",
            Gender           = "Female",
            Email            = "dr.noha.lotfy.qalyubia.intmed@ehrs-clinic.eg",
            ContactNumber    = "01110000369",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00369-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia General Hospital",
            About            = "Experienced Internal Medicine Specialist based in Qalyubia governorate with over 16 years of clinical practice.",
        }, "EG-00369-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Gerges Mohamed",
            Gender           = "Male",
            Email            = "dr.gerges.mohamed.qalyubia.dent@ehrs-clinic.eg",
            ContactNumber    = "01210000370",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00370-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Al-Maadi Hospital",
            About            = "Experienced Dentist based in Qalyubia governorate with over 19 years of clinical practice.",
        }, "EG-00370-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ashraf Mahmoud",
            Gender           = "Male",
            Email            = "dr.ashraf.mahmoud.qalyubia.ent@ehrs-clinic.eg",
            ContactNumber    = "01510000371",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00371-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia General Hospital",
            About            = "Experienced ENT Specialist based in Qalyubia governorate with over 16 years of clinical practice.",
        }, "EG-00371-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Noha Mohamed",
            Gender           = "Female",
            Email            = "dr.noha.mohamed.qalyubia.uro@ehrs-clinic.eg",
            ContactNumber    = "01010000372",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00372-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Central Medical Center",
            About            = "Experienced Urology Specialist based in Qalyubia governorate with over 24 years of clinical practice.",
        }, "EG-00372-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Gerges Fawzy",
            Gender           = "Male",
            Email            = "dr.gerges.fawzy.qalyubia.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01110000373",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00373-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Cairo University Hospital",
            About            = "Experienced Ophthalmology Specialist based in Qalyubia governorate with over 23 years of clinical practice.",
        }, "EG-00373-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Emad Mahmoud",
            Gender           = "Male",
            Email            = "dr.emad.mahmoud.qalyubia.rheum@ehrs-clinic.eg",
            ContactNumber    = "01210000374",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00374-MD",
            Area             = "Qalyubia",
            AffiliatedHospital = "Qalyubia Cairo University Hospital",
            About            = "Experienced Rheumatology Specialist based in Qalyubia governorate with over 23 years of clinical practice.",
        }, "EG-00374-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Abeer Nasser",
            Gender           = "Female",
            Email            = "dr.abeer.nasser.qena.gp@ehrs-clinic.eg",
            ContactNumber    = "01510000375",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00375-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Dar Al-Shifa Hospital",
            About            = "Experienced General Practitioner (GP) based in Qena governorate with over 24 years of clinical practice.",
        }, "EG-00375-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Atef Rushdi",
            Gender           = "Male",
            Email            = "dr.atef.rushdi.qena.peds@ehrs-clinic.eg",
            ContactNumber    = "01010000376",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00376-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Al-Salam Hospital",
            About            = "Experienced Pediatrics Specialist based in Qena governorate with over 6 years of clinical practice.",
        }, "EG-00376-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ashraf Omar",
            Gender           = "Male",
            Email            = "dr.ashraf.omar.qena.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01110000377",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00377-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Dar Al-Shifa Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Qena governorate with over 7 years of clinical practice.",
        }, "EG-00377-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Doaa Zaki",
            Gender           = "Female",
            Email            = "dr.doaa.zaki.qena.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01210000378",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00378-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena General Hospital",
            About            = "Experienced General Surgery Specialist based in Qena governorate with over 12 years of clinical practice.",
        }, "EG-00378-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Safwat",
            Gender           = "Female",
            Email            = "dr.manar.safwat.qena.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01510000379",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00379-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Cairo University Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Qena governorate with over 21 years of clinical practice.",
        }, "EG-00379-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mina Morsi",
            Gender           = "Male",
            Email            = "dr.mina.morsi.qena.cardio@ehrs-clinic.eg",
            ContactNumber    = "01010000380",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00380-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Al-Ahrar Hospital",
            About            = "Experienced Cardiology Specialist based in Qena governorate with over 16 years of clinical practice.",
        }, "EG-00380-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hana Ragab",
            Gender           = "Female",
            Email            = "dr.hana.ragab.qena.neuro@ehrs-clinic.eg",
            ContactNumber    = "01110000381",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00381-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Cleopatra Hospital",
            About            = "Experienced Neurology Specialist based in Qena governorate with over 18 years of clinical practice.",
        }, "EG-00381-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ramy Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.ramy.abdelrahman.qena.vasc@ehrs-clinic.eg",
            ContactNumber    = "01210000382",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00382-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena General Hospital",
            About            = "Experienced Vascular Specialist based in Qena governorate with over 25 years of clinical practice.",
        }, "EG-00382-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ramy Mohamed",
            Gender           = "Male",
            Email            = "dr.ramy.mohamed.qena.pulm@ehrs-clinic.eg",
            ContactNumber    = "01510000383",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00383-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Dar Al-Shifa Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Qena governorate with over 8 years of clinical practice.",
        }, "EG-00383-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Eman Fawzy",
            Gender           = "Female",
            Email            = "dr.eman.fawzy.qena.ortho@ehrs-clinic.eg",
            ContactNumber    = "01010000384",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00384-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Al-Maadi Hospital",
            About            = "Experienced Orthopedic Specialist based in Qena governorate with over 13 years of clinical practice.",
        }, "EG-00384-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Badawi",
            Gender           = "Male",
            Email            = "dr.tamer.badawi.qena.derm@ehrs-clinic.eg",
            ContactNumber    = "01110000385",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00385-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Central Medical Center",
            About            = "Experienced Dermatology Specialist based in Qena governorate with over 23 years of clinical practice.",
        }, "EG-00385-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Gaber",
            Gender           = "Male",
            Email            = "dr.tamer.gaber.qena.intmed@ehrs-clinic.eg",
            ContactNumber    = "01210000386",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00386-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Al-Maadi Hospital",
            About            = "Experienced Internal Medicine Specialist based in Qena governorate with over 25 years of clinical practice.",
        }, "EG-00386-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Manar Tantawi",
            Gender           = "Female",
            Email            = "dr.manar.tantawi.qena.dent@ehrs-clinic.eg",
            ContactNumber    = "01510000387",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00387-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Al-Salam Hospital",
            About            = "Experienced Dentist based in Qena governorate with over 24 years of clinical practice.",
        }, "EG-00387-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Saleh",
            Gender           = "Male",
            Email            = "dr.hassan.saleh.qena.ent@ehrs-clinic.eg",
            ContactNumber    = "01010000388",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00388-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Cleopatra Hospital",
            About            = "Experienced ENT Specialist based in Qena governorate with over 25 years of clinical practice.",
        }, "EG-00388-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ramy Farouk",
            Gender           = "Male",
            Email            = "dr.ramy.farouk.qena.uro@ehrs-clinic.eg",
            ContactNumber    = "01110000389",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00389-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Central Medical Center",
            About            = "Experienced Urology Specialist based in Qena governorate with over 25 years of clinical practice.",
        }, "EG-00389-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samira Hamdy",
            Gender           = "Female",
            Email            = "dr.samira.hamdy.qena.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01210000390",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00390-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Al-Wadi Hospital",
            About            = "Experienced Ophthalmology Specialist based in Qena governorate with over 5 years of clinical practice.",
        }, "EG-00390-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Saleh",
            Gender           = "Male",
            Email            = "dr.karim.saleh.qena.rheum@ehrs-clinic.eg",
            ContactNumber    = "01510000391",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00391-MD",
            Area             = "Qena",
            AffiliatedHospital = "Qena Al-Ahrar Hospital",
            About            = "Experienced Rheumatology Specialist based in Qena governorate with over 11 years of clinical practice.",
        }, "EG-00391-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ramy Naguib",
            Gender           = "Male",
            Email            = "dr.ramy.naguib.redsea.gp@ehrs-clinic.eg",
            ContactNumber    = "01010000392",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00392-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Al-Zahraa Hospital",
            About            = "Experienced General Practitioner (GP) based in Red Sea governorate with over 12 years of clinical practice.",
        }, "EG-00392-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sara Saad",
            Gender           = "Female",
            Email            = "dr.sara.saad.redsea.peds@ehrs-clinic.eg",
            ContactNumber    = "01110000393",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00393-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Central Medical Center",
            About            = "Experienced Pediatrics Specialist based in Red Sea governorate with over 14 years of clinical practice.",
        }, "EG-00393-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ibrahim Rushdi",
            Gender           = "Male",
            Email            = "dr.ibrahim.rushdi.redsea.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01210000394",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00394-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea General Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Red Sea governorate with over 15 years of clinical practice.",
        }, "EG-00394-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Amr Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.amr.abdelrahman.redsea.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01510000395",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00395-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Cleopatra Hospital",
            About            = "Experienced General Surgery Specialist based in Red Sea governorate with over 9 years of clinical practice.",
        }, "EG-00395-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mona Zaki",
            Gender           = "Female",
            Email            = "dr.mona.zaki.redsea.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01010000396",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00396-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Al-Ahrar Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Red Sea governorate with over 19 years of clinical practice.",
        }, "EG-00396-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Omar Aziz",
            Gender           = "Male",
            Email            = "dr.omar.aziz.redsea.cardio@ehrs-clinic.eg",
            ContactNumber    = "01110000397",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00397-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Al-Wadi Hospital",
            About            = "Experienced Cardiology Specialist based in Red Sea governorate with over 12 years of clinical practice.",
        }, "EG-00397-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ashraf Morsi",
            Gender           = "Male",
            Email            = "dr.ashraf.morsi.redsea.neuro@ehrs-clinic.eg",
            ContactNumber    = "01210000398",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00398-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Al-Maadi Hospital",
            About            = "Experienced Neurology Specialist based in Red Sea governorate with over 19 years of clinical practice.",
        }, "EG-00398-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rania Ghoneim",
            Gender           = "Female",
            Email            = "dr.rania.ghoneim.redsea.vasc@ehrs-clinic.eg",
            ContactNumber    = "01510000399",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00399-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Al-Helal Medical Complex",
            About            = "Experienced Vascular Specialist based in Red Sea governorate with over 14 years of clinical practice.",
        }, "EG-00399-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Atef Nasser",
            Gender           = "Male",
            Email            = "dr.atef.nasser.redsea.pulm@ehrs-clinic.eg",
            ContactNumber    = "01010000400",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00400-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Al-Wadi Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Red Sea governorate with over 23 years of clinical practice.",
        }, "EG-00400-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ashraf Halim",
            Gender           = "Male",
            Email            = "dr.ashraf.halim.redsea.ortho@ehrs-clinic.eg",
            ContactNumber    = "01110000401",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00401-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Al-Maadi Hospital",
            About            = "Experienced Orthopedic Specialist based in Red Sea governorate with over 17 years of clinical practice.",
        }, "EG-00401-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Doaa Shehata",
            Gender           = "Female",
            Email            = "dr.doaa.shehata.redsea.derm@ehrs-clinic.eg",
            ContactNumber    = "01210000402",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00402-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Ain Shams University Hospital",
            About            = "Experienced Dermatology Specialist based in Red Sea governorate with over 10 years of clinical practice.",
        }, "EG-00402-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tarek Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.tarek.abdelrahman.redsea.intmed@ehrs-clinic.eg",
            ContactNumber    = "01510000403",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00403-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Al-Salam Hospital",
            About            = "Experienced Internal Medicine Specialist based in Red Sea governorate with over 13 years of clinical practice.",
        }, "EG-00403-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mahmoud Kamal",
            Gender           = "Male",
            Email            = "dr.mahmoud.kamal.redsea.dent@ehrs-clinic.eg",
            ContactNumber    = "01010000404",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00404-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Al-Wadi Hospital",
            About            = "Experienced Dentist based in Red Sea governorate with over 22 years of clinical practice.",
        }, "EG-00404-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Dalia Shehata",
            Gender           = "Female",
            Email            = "dr.dalia.shehata.redsea.ent@ehrs-clinic.eg",
            ContactNumber    = "01110000405",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00405-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Nile Medical Center",
            About            = "Experienced ENT Specialist based in Red Sea governorate with over 14 years of clinical practice.",
        }, "EG-00405-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Mansour",
            Gender           = "Male",
            Email            = "dr.hassan.mansour.redsea.uro@ehrs-clinic.eg",
            ContactNumber    = "01210000406",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00406-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Royal Medical Center",
            About            = "Experienced Urology Specialist based in Red Sea governorate with over 19 years of clinical practice.",
        }, "EG-00406-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Gerges Saad",
            Gender           = "Male",
            Email            = "dr.gerges.saad.redsea.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01510000407",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00407-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Ain Shams University Hospital",
            About            = "Experienced Ophthalmology Specialist based in Red Sea governorate with over 7 years of clinical practice.",
        }, "EG-00407-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Rania Safwat",
            Gender           = "Female",
            Email            = "dr.rania.safwat.redsea.rheum@ehrs-clinic.eg",
            ContactNumber    = "01010000408",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00408-MD",
            Area             = "Red Sea",
            AffiliatedHospital = "Red Sea Al-Wadi Hospital",
            About            = "Experienced Rheumatology Specialist based in Red Sea governorate with over 5 years of clinical practice.",
        }, "EG-00408-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nader Ali",
            Gender           = "Male",
            Email            = "dr.nader.ali.sohag.gp@ehrs-clinic.eg",
            ContactNumber    = "01110000409",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00409-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag Cleopatra Hospital",
            About            = "Experienced General Practitioner (GP) based in Sohag governorate with over 21 years of clinical practice.",
        }, "EG-00409-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Abdalla",
            Gender           = "Male",
            Email            = "dr.karim.abdalla.sohag.peds@ehrs-clinic.eg",
            ContactNumber    = "01210000410",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00410-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag Cleopatra Hospital",
            About            = "Experienced Pediatrics Specialist based in Sohag governorate with over 7 years of clinical practice.",
        }, "EG-00410-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hana Shalaby",
            Gender           = "Female",
            Email            = "dr.hana.shalaby.sohag.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01510000411",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00411-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag University Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Sohag governorate with over 15 years of clinical practice.",
        }, "EG-00411-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ibrahim Badawi",
            Gender           = "Male",
            Email            = "dr.ibrahim.badawi.sohag.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01010000412",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00412-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag Al-Salam Hospital",
            About            = "Experienced General Surgery Specialist based in Sohag governorate with over 9 years of clinical practice.",
        }, "EG-00412-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mariam Ragab",
            Gender           = "Female",
            Email            = "dr.mariam.ragab.sohag.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01110000413",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00413-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag As-Salam International Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Sohag governorate with over 9 years of clinical practice.",
        }, "EG-00413-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ola Kamal",
            Gender           = "Female",
            Email            = "dr.ola.kamal.sohag.cardio@ehrs-clinic.eg",
            ContactNumber    = "01210000414",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00414-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag Cairo University Hospital",
            About            = "Experienced Cardiology Specialist based in Sohag governorate with over 24 years of clinical practice.",
        }, "EG-00414-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ahmed Ahmed",
            Gender           = "Male",
            Email            = "dr.ahmed.ahmed.sohag.neuro@ehrs-clinic.eg",
            ContactNumber    = "01510000415",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00415-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag University Hospital",
            About            = "Experienced Neurology Specialist based in Sohag governorate with over 13 years of clinical practice.",
        }, "EG-00415-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hossam Saad",
            Gender           = "Male",
            Email            = "dr.hossam.saad.sohag.vasc@ehrs-clinic.eg",
            ContactNumber    = "01010000416",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00416-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag Ain Shams University Hospital",
            About            = "Experienced Vascular Specialist based in Sohag governorate with over 8 years of clinical practice.",
        }, "EG-00416-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ola Ragab",
            Gender           = "Female",
            Email            = "dr.ola.ragab.sohag.pulm@ehrs-clinic.eg",
            ContactNumber    = "01110000417",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00417-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag Al-Helal Medical Complex",
            About            = "Experienced Chest Specialist (Pulmonology) based in Sohag governorate with over 14 years of clinical practice.",
        }, "EG-00417-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Youssef Ali",
            Gender           = "Male",
            Email            = "dr.youssef.ali.sohag.ortho@ehrs-clinic.eg",
            ContactNumber    = "01210000418",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00418-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag Al-Helal Medical Complex",
            About            = "Experienced Orthopedic Specialist based in Sohag governorate with over 18 years of clinical practice.",
        }, "EG-00418-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Bassem Mohamed",
            Gender           = "Male",
            Email            = "dr.bassem.mohamed.sohag.derm@ehrs-clinic.eg",
            ContactNumber    = "01510000419",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00419-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag Nile Medical Center",
            About            = "Experienced Dermatology Specialist based in Sohag governorate with over 20 years of clinical practice.",
        }, "EG-00419-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Faten Ghoneim",
            Gender           = "Female",
            Email            = "dr.faten.ghoneim.sohag.intmed@ehrs-clinic.eg",
            ContactNumber    = "01010000420",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00420-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag University Hospital",
            About            = "Experienced Internal Medicine Specialist based in Sohag governorate with over 25 years of clinical practice.",
        }, "EG-00420-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Gerges Metwally",
            Gender           = "Male",
            Email            = "dr.gerges.metwally.sohag.dent@ehrs-clinic.eg",
            ContactNumber    = "01110000421",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00421-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag Al-Helal Medical Complex",
            About            = "Experienced Dentist based in Sohag governorate with over 9 years of clinical practice.",
        }, "EG-00421-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samer Hamdy",
            Gender           = "Male",
            Email            = "dr.samer.hamdy.sohag.ent@ehrs-clinic.eg",
            ContactNumber    = "01210000422",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00422-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag University Hospital",
            About            = "Experienced ENT Specialist based in Sohag governorate with over 24 years of clinical practice.",
        }, "EG-00422-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hana Abdalla",
            Gender           = "Female",
            Email            = "dr.hana.abdalla.sohag.uro@ehrs-clinic.eg",
            ContactNumber    = "01510000423",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00423-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag Ain Shams University Hospital",
            About            = "Experienced Urology Specialist based in Sohag governorate with over 10 years of clinical practice.",
        }, "EG-00423-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hassan Shehata",
            Gender           = "Male",
            Email            = "dr.hassan.shehata.sohag.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01010000424",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00424-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag Al-Wadi Hospital",
            About            = "Experienced Ophthalmology Specialist based in Sohag governorate with over 7 years of clinical practice.",
        }, "EG-00424-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mina Ghoneim",
            Gender           = "Male",
            Email            = "dr.mina.ghoneim.sohag.rheum@ehrs-clinic.eg",
            ContactNumber    = "01110000425",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00425-MD",
            Area             = "Sohag",
            AffiliatedHospital = "Sohag University Hospital",
            About            = "Experienced Rheumatology Specialist based in Sohag governorate with over 17 years of clinical practice.",
        }, "EG-00425-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Lobna Mahmoud",
            Gender           = "Female",
            Email            = "dr.lobna.mahmoud.southsinai.gp@ehrs-clinic.eg",
            ContactNumber    = "01210000426",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00426-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Cleopatra Hospital",
            About            = "Experienced General Practitioner (GP) based in South Sinai governorate with over 16 years of clinical practice.",
        }, "EG-00426-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Maged Ibrahim",
            Gender           = "Male",
            Email            = "dr.maged.ibrahim.southsinai.peds@ehrs-clinic.eg",
            ContactNumber    = "01510000427",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00427-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Al-Wadi Hospital",
            About            = "Experienced Pediatrics Specialist based in South Sinai governorate with over 7 years of clinical practice.",
        }, "EG-00427-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ehab Abdalla",
            Gender           = "Male",
            Email            = "dr.ehab.abdalla.southsinai.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01010000428",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00428-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Nile Medical Center",
            About            = "Experienced Pediatric Surgery Specialist based in South Sinai governorate with over 23 years of clinical practice.",
        }, "EG-00428-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Reham Badawi",
            Gender           = "Female",
            Email            = "dr.reham.badawi.southsinai.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01110000429",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00429-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Al-Salam Hospital",
            About            = "Experienced General Surgery Specialist based in South Sinai governorate with over 6 years of clinical practice.",
        }, "EG-00429-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hana Ghoneim",
            Gender           = "Female",
            Email            = "dr.hana.ghoneim.southsinai.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01210000430",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00430-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Dar Al-Shifa Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in South Sinai governorate with over 25 years of clinical practice.",
        }, "EG-00430-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Walid Halim",
            Gender           = "Male",
            Email            = "dr.walid.halim.southsinai.cardio@ehrs-clinic.eg",
            ContactNumber    = "01510000431",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00431-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai As-Salam International Hospital",
            About            = "Experienced Cardiology Specialist based in South Sinai governorate with over 25 years of clinical practice.",
        }, "EG-00431-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mona Farouk",
            Gender           = "Female",
            Email            = "dr.mona.farouk.southsinai.neuro@ehrs-clinic.eg",
            ContactNumber    = "01010000432",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00432-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Central Medical Center",
            About            = "Experienced Neurology Specialist based in South Sinai governorate with over 19 years of clinical practice.",
        }, "EG-00432-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Omar Ragab",
            Gender           = "Male",
            Email            = "dr.omar.ragab.southsinai.vasc@ehrs-clinic.eg",
            ContactNumber    = "01110000433",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00433-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Al-Zahraa Hospital",
            About            = "Experienced Vascular Specialist based in South Sinai governorate with over 6 years of clinical practice.",
        }, "EG-00433-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tarek Mahmoud",
            Gender           = "Male",
            Email            = "dr.tarek.mahmoud.southsinai.pulm@ehrs-clinic.eg",
            ContactNumber    = "01210000434",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00434-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Dar Al-Shifa Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in South Sinai governorate with over 14 years of clinical practice.",
        }, "EG-00434-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Doaa Tantawi",
            Gender           = "Female",
            Email            = "dr.doaa.tantawi.southsinai.ortho@ehrs-clinic.eg",
            ContactNumber    = "01510000435",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00435-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai As-Salam International Hospital",
            About            = "Experienced Orthopedic Specialist based in South Sinai governorate with over 13 years of clinical practice.",
        }, "EG-00435-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Omar Nasser",
            Gender           = "Male",
            Email            = "dr.omar.nasser.southsinai.derm@ehrs-clinic.eg",
            ContactNumber    = "01010000436",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00436-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Al-Maadi Hospital",
            About            = "Experienced Dermatology Specialist based in South Sinai governorate with over 16 years of clinical practice.",
        }, "EG-00436-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Mahmoud Badawi",
            Gender           = "Male",
            Email            = "dr.mahmoud.badawi.southsinai.intmed@ehrs-clinic.eg",
            ContactNumber    = "01110000437",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00437-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Royal Medical Center",
            About            = "Experienced Internal Medicine Specialist based in South Sinai governorate with over 8 years of clinical practice.",
        }, "EG-00437-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Shaimaa Fouad",
            Gender           = "Female",
            Email            = "dr.shaimaa.fouad.southsinai.dent@ehrs-clinic.eg",
            ContactNumber    = "01210000438",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00438-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Ain Shams University Hospital",
            About            = "Experienced Dentist based in South Sinai governorate with over 17 years of clinical practice.",
        }, "EG-00438-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ashraf Fawzy",
            Gender           = "Male",
            Email            = "dr.ashraf.fawzy.southsinai.ent@ehrs-clinic.eg",
            ContactNumber    = "01510000439",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00439-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Dar Al-Shifa Hospital",
            About            = "Experienced ENT Specialist based in South Sinai governorate with over 10 years of clinical practice.",
        }, "EG-00439-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Fady Naguib",
            Gender           = "Male",
            Email            = "dr.fady.naguib.southsinai.uro@ehrs-clinic.eg",
            ContactNumber    = "01010000440",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00440-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Al-Wadi Hospital",
            About            = "Experienced Urology Specialist based in South Sinai governorate with over 21 years of clinical practice.",
        }, "EG-00440-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Yasmin Ahmed",
            Gender           = "Female",
            Email            = "dr.yasmin.ahmed.southsinai.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01110000441",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00441-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Ain Shams University Hospital",
            About            = "Experienced Ophthalmology Specialist based in South Sinai governorate with over 7 years of clinical practice.",
        }, "EG-00441-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Essam Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.essam.abdelrahman.southsinai.rheum@ehrs-clinic.eg",
            ContactNumber    = "01210000442",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00442-MD",
            Area             = "South Sinai",
            AffiliatedHospital = "South Sinai Al-Ahrar Hospital",
            About            = "Experienced Rheumatology Specialist based in South Sinai governorate with over 22 years of clinical practice.",
        }, "EG-00442-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samer Amer",
            Gender           = "Male",
            Email            = "dr.samer.amer.suez.gp@ehrs-clinic.eg",
            ContactNumber    = "01510000443",
            Specialization   = "General Practitioner (GP)",
            MedicalLicense   = "EG-00443-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Nile Medical Center",
            About            = "Experienced General Practitioner (GP) based in Suez governorate with over 7 years of clinical practice.",
        }, "EG-00443-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Amira Ragab",
            Gender           = "Female",
            Email            = "dr.amira.ragab.suez.peds@ehrs-clinic.eg",
            ContactNumber    = "01010000444",
            Specialization   = "Pediatrics Specialist",
            MedicalLicense   = "EG-00444-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Al-Maadi Hospital",
            About            = "Experienced Pediatrics Specialist based in Suez governorate with over 19 years of clinical practice.",
        }, "EG-00444-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Essam Mansour",
            Gender           = "Male",
            Email            = "dr.essam.mansour.suez.pedsurg@ehrs-clinic.eg",
            ContactNumber    = "01110000445",
            Specialization   = "Pediatric Surgery Specialist",
            MedicalLicense   = "EG-00445-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Cairo University Hospital",
            About            = "Experienced Pediatric Surgery Specialist based in Suez governorate with over 16 years of clinical practice.",
        }, "EG-00445-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Ashraf Mahmoud",
            Gender           = "Male",
            Email            = "dr.ashraf.mahmoud.suez.gensurg@ehrs-clinic.eg",
            ContactNumber    = "01210000446",
            Specialization   = "General Surgery Specialist",
            MedicalLicense   = "EG-00446-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Al-Wadi Hospital",
            About            = "Experienced General Surgery Specialist based in Suez governorate with over 24 years of clinical practice.",
        }, "EG-00446-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Neveen Lotfy",
            Gender           = "Female",
            Email            = "dr.neveen.lotfy.suez.obgyn@ehrs-clinic.eg",
            ContactNumber    = "01510000447",
            Specialization   = "Obstetrics and Gynecology Specialist",
            MedicalLicense   = "EG-00447-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez General Hospital",
            About            = "Experienced Obstetrics and Gynecology Specialist based in Suez governorate with over 7 years of clinical practice.",
        }, "EG-00447-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Samir Fouad",
            Gender           = "Male",
            Email            = "dr.samir.fouad.suez.cardio@ehrs-clinic.eg",
            ContactNumber    = "01010000448",
            Specialization   = "Cardiology Specialist",
            MedicalLicense   = "EG-00448-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Al-Ahrar Hospital",
            About            = "Experienced Cardiology Specialist based in Suez governorate with over 5 years of clinical practice.",
        }, "EG-00448-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Tamer Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.tamer.abdelrahman.suez.neuro@ehrs-clinic.eg",
            ContactNumber    = "01110000449",
            Specialization   = "Neurology Specialist",
            MedicalLicense   = "EG-00449-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Cairo University Hospital",
            About            = "Experienced Neurology Specialist based in Suez governorate with over 6 years of clinical practice.",
        }, "EG-00449-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Sara Mohamed",
            Gender           = "Female",
            Email            = "dr.sara.mohamed.suez.vasc@ehrs-clinic.eg",
            ContactNumber    = "01210000450",
            Specialization   = "Vascular Specialist",
            MedicalLicense   = "EG-00450-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Al-Helal Medical Complex",
            About            = "Experienced Vascular Specialist based in Suez governorate with over 25 years of clinical practice.",
        }, "EG-00450-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Karim Fouad",
            Gender           = "Male",
            Email            = "dr.karim.fouad.suez.pulm@ehrs-clinic.eg",
            ContactNumber    = "01510000451",
            Specialization   = "Chest Specialist (Pulmonology)",
            MedicalLicense   = "EG-00451-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Cleopatra Hospital",
            About            = "Experienced Chest Specialist (Pulmonology) based in Suez governorate with over 23 years of clinical practice.",
        }, "EG-00451-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Omar Abdel-Rahman",
            Gender           = "Male",
            Email            = "dr.omar.abdelrahman.suez.ortho@ehrs-clinic.eg",
            ContactNumber    = "01010000452",
            Specialization   = "Orthopedic Specialist",
            MedicalLicense   = "EG-00452-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Al-Salam Hospital",
            About            = "Experienced Orthopedic Specialist based in Suez governorate with over 19 years of clinical practice.",
        }, "EG-00452-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hana Fouad",
            Gender           = "Female",
            Email            = "dr.hana.fouad.suez.derm@ehrs-clinic.eg",
            ContactNumber    = "01110000453",
            Specialization   = "Dermatology Specialist",
            MedicalLicense   = "EG-00453-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Cairo University Hospital",
            About            = "Experienced Dermatology Specialist based in Suez governorate with over 7 years of clinical practice.",
        }, "EG-00453-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Amr Shehata",
            Gender           = "Male",
            Email            = "dr.amr.shehata.suez.intmed@ehrs-clinic.eg",
            ContactNumber    = "01210000454",
            Specialization   = "Internal Medicine Specialist",
            MedicalLicense   = "EG-00454-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Al-Wadi Hospital",
            About            = "Experienced Internal Medicine Specialist based in Suez governorate with over 17 years of clinical practice.",
        }, "EG-00454-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Wael Lotfy",
            Gender           = "Male",
            Email            = "dr.wael.lotfy.suez.dent@ehrs-clinic.eg",
            ContactNumber    = "01510000455",
            Specialization   = "Dentist",
            MedicalLicense   = "EG-00455-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Al-Helal Medical Complex",
            About            = "Experienced Dentist based in Suez governorate with over 8 years of clinical practice.",
        }, "EG-00455-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Nada Zaki",
            Gender           = "Female",
            Email            = "dr.nada.zaki.suez.ent@ehrs-clinic.eg",
            ContactNumber    = "01010000456",
            Specialization   = "ENT Specialist",
            MedicalLicense   = "EG-00456-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez As-Salam International Hospital",
            About            = "Experienced ENT Specialist based in Suez governorate with over 21 years of clinical practice.",
        }, "EG-00456-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Hany Barakat",
            Gender           = "Male",
            Email            = "dr.hany.barakat.suez.uro@ehrs-clinic.eg",
            ContactNumber    = "01110000457",
            Specialization   = "Urology Specialist",
            MedicalLicense   = "EG-00457-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Nile Medical Center",
            About            = "Experienced Urology Specialist based in Suez governorate with over 13 years of clinical practice.",
        }, "EG-00457-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Maged Safwat",
            Gender           = "Male",
            Email            = "dr.maged.safwat.suez.ophthal@ehrs-clinic.eg",
            ContactNumber    = "01210000458",
            Specialization   = "Ophthalmology Specialist",
            MedicalLicense   = "EG-00458-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Al-Zahraa Hospital",
            About            = "Experienced Ophthalmology Specialist based in Suez governorate with over 24 years of clinical practice.",
        }, "EG-00458-MD"));

        list.Add((new Doctor
        {
            FullName         = "Dr. Noha Naguib",
            Gender           = "Female",
            Email            = "dr.noha.naguib.suez.rheum@ehrs-clinic.eg",
            ContactNumber    = "01510000459",
            Specialization   = "Rheumatology Specialist",
            MedicalLicense   = "EG-00459-MD",
            Area             = "Suez",
            AffiliatedHospital = "Suez Al-Zahraa Hospital",
            About            = "Experienced Rheumatology Specialist based in Suez governorate with over 8 years of clinical practice.",
        }, "EG-00459-MD"));

        return list;
    }
}