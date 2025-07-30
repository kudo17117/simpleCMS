using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCMSWeb.Models;

public partial class Employee
{
    [Column("employeeid")]
    public long Employeeid { get; set; }

    [Column("employeeno")]
    public string? EmployeeNo { get; set; }

    [Column("lastname")]
    public string? LastName { get; set; }

    [Column("firstname")]
    public string? FirstName { get; set; }

    [Column("middlename")]
    public string? MiddleName { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("contactno")]
    public string? ContactNo { get; set; }

    [Column("officecode")]
    public string? OfficeCode { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("gender")]
    public string? Gender { get; set; }

    [Column("birthdate")]
    public DateTime? BirthDate { get; set; }

    [Column("hiredate")]
    public DateTime? HireDate { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("designation")]
    public string? Designation { get; set; }

    [Column("isactive")]
    public bool IsActive { get; set; }

    [Column("proftype")]
    public string? ProfType { get; set; }

    [Column("pan")]
    public string? PAN { get; set; }

    [Column("employeetypeid")]
    public int? EmployeeTypeId { get; set; }

    [Column("licenceno")]
    public string? LicenceNo { get; set; }

    [Column("ptr")]
    public string? PTR { get; set; }

    [Column("proffee")]
    public decimal? ProfFee { get; set; }

    [Column("dailyvisitrate")]
    public decimal? DailyVisitRate { get; set; }

    [Column("phic_accreditationno")]
    public string? PhicAccreditationNo { get; set; }

    [Column("photo")]
    public byte[]? Photo { get; set; }

    [Column("employeemasterid")]
    public long? EmployeeMasterId { get; set; }

    [Column("sssno")]
    public string? SSSNo { get; set; }

    [Column("sssregistrationdate")]
    public DateTime? SSSRegistrationDate { get; set; }

    [Column("hdmfno")]
    public string? HDMFNo { get; set; }

    [Column("hdmftrackingno")]
    public string? HDMFTrackingNo { get; set; }

    [Column("hdmfregistrationdate")]
    public DateTime? HDMFRegistrationDate { get; set; }

    [Column("tinno")]
    public string? TINNo { get; set; }

    [Column("philhealthno")]
    public string? PhilHealthNo { get; set; }

    [Column("bloodtype")]
    public string? BloodType { get; set; }

    [Column("spousename")]
    public string? SpouseName { get; set; }

    [Column("resignationdate")]
    public DateTime? ResignationDate { get; set; }

    [Column("retirementdate")]
    public DateTime? RetirementDate { get; set; }

    [Column("permanentaddress")]
    public string? PermanentAddress { get; set; }

    [Column("employmentstatus")]
    public string? EmploymentStatus { get; set; }

    [Column("contactperson")]
    public string? ContactPerson { get; set; }

    [Column("contactphoneno")]
    public string? ContactPhoneNo { get; set; }

    [Column("elementary")]
    public string? Elementary { get; set; }

    [Column("elemyear")]
    public long? ElemYear { get; set; }

    [Column("secondary")]
    public string? Secondary { get; set; }

    [Column("secyear")]
    public long? SecYear { get; set; }

    [Column("college")]
    public string? College { get; set; }

    [Column("collyear")]
    public long? CollYear { get; set; }

    [Column("others1")]
    public string? Others1 { get; set; }

    [Column("others1year")]
    public long? Others1Year { get; set; }

    [Column("others2")]
    public string? Others2 { get; set; }

    [Column("others2year")]
    public long? Others2Year { get; set; }

    [Column("others3")]
    public string? Others3 { get; set; }

    [Column("others3year")]
    public long? Others3Year { get; set; }

    [Column("patientid_employee")]
    public long? PatientIdEmployee { get; set; }

    [Column("extensioname")]
    public string? ExtensioName { get; set; }

    [Column("placeofbirth")]
    public string? PlaceOfBirth { get; set; }

    [Column("civilstatus")]
    public string? CivilStatus { get; set; }

    [Column("citizenship")]
    public string? Citizenship { get; set; }

    [Column("height")]
    public string? Height { get; set; }

    [Column("weight")]
    public string? Weight { get; set; }

    [Column("prcno")]
    public string? PRCNo { get; set; }

    [Column("cs_eligibility")]
    public string? CsEligibility { get; set; }

    [Column("pagibig_id_no")]
    public string? PagIbigIdNo { get; set; }

    [Column("pagibig_coverage")]
    public string? PagIbigCoverage { get; set; }

    [Column("sss_coverage")]
    public string? SSSCoverage { get; set; }

    [Column("residential_address")]
    public string? ResidentialAddress { get; set; }

    [Column("zipcode")]
    public string? Zipcode { get; set; }

    [Column("telephone_no")]
    public string? TelephoneNo { get; set; }

    [Column("permanent_address_zipcode")]
    public string? PermanentAddressZipcode { get; set; }

    [Column("agency_employeeno")]
    public string? AgencyEmployeeNo { get; set; }

    [Column("e_createdbyid")]
    public long? ECreatedById { get; set; }

    [Column("e_datecreated")]
    public DateTime? EDateCreated { get; set; }

    [Column("e_updatedbyid")]
    public long? EUpdatedById { get; set; }

    [Column("e_dateupdated")]
    public DateTime? EDateUpdated { get; set; }

    [Column("graduatestudies")]
    public string? GraduateStudies { get; set; }

    [Column("grad_year")]
    public string? GradYear { get; set; }

    [Column("vocational")]
    public string? Vocational { get; set; }

    [Column("voc_year")]
    public long? VocYear { get; set; }

    [Column("elem_degree_course")]
    public string? ElemDegreeCourse { get; set; }

    [Column("sec_degree_course")]
    public string? SecDegreeCourse { get; set; }

    [Column("voc_degree_course")]
    public string? VocDegreeCourse { get; set; }

    [Column("col_degree_course")]
    public string? ColDegreeCourse { get; set; }

    [Column("grad_degree_course")]
    public string? GradDegreeCourse { get; set; }

    [Column("elem_grade_level")]
    public string? ElemGradeLevel { get; set; }

    [Column("sec_grade_level")]
    public string? SecGradeLevel { get; set; }

    [Column("voc_grade_level")]
    public string? VocGradeLevel { get; set; }

    [Column("col_grade_level")]
    public string? ColGradeLevel { get; set; }

    [Column("grad_grade_level")]
    public string? GradGradeLevel { get; set; }

    [Column("elem_units_earned")]
    public string? ElemUnitsEarned { get; set; }

    [Column("sec_units_earned")]
    public string? SecUnitsEarned { get; set; }

    [Column("voc_units_earned")]
    public string? VocUnitsEarned { get; set; }

    [Column("col_units_earned")]
    public string? ColUnitsEarned { get; set; }

    [Column("grad_units_earned")]
    public string? GradUnitsEarned { get; set; }

    [Column("elem_attendancefrom")]
    public string? ElemAttendanceFrom { get; set; }

    [Column("sec_attendancefrom")]
    public string? SecAttendanceFrom { get; set; }

    [Column("voc_attendancefrom")]
    public string? VocAttendanceFrom { get; set; }

    [Column("col_attendancefrom")]
    public string? ColAttendanceFrom { get; set; }

    [Column("grad_attendancefrom")]
    public string? GradAttendanceFrom { get; set; }

    [Column("elem_attendanceto")]
    public string? ElemAttendanceTo { get; set; }

    [Column("sec_attendanceto")]
    public string? SecAttendanceTo { get; set; }

    [Column("voc_attendanceto")]
    public string? VocAttendanceTo { get; set; }

    [Column("col_attendanceto")]
    public string? ColAttendanceTo { get; set; }

    [Column("grad_attendanceto")]
    public string? GradAttendanceTo { get; set; }

    [Column("elem_scholarship")]
    public string? ElemScholarship { get; set; }

    [Column("sec_scholarship")]
    public string? SecScholarship { get; set; }

    [Column("voc_scholarship")]
    public string? VocScholarship { get; set; }

    [Column("col_scholarship")]
    public string? ColScholarship { get; set; }

    [Column("grad_scholarship")]
    public string? GradScholarship { get; set; }

    [Column("elem_academichonors")]
    public string? ElemAcademicHonors { get; set; }

    [Column("sec_academichonors")]
    public string? SecAcademicHonors { get; set; }

    [Column("voc_academichonors")]
    public string? VocAcademicHonors { get; set; }

    [Column("col_academichonors")]
    public string? ColAcademicHonors { get; set; }

    [Column("grad_academichonors")]
    public string? GradAcademicHonors { get; set; }

    // === Spouse details ===
    [Column("spouse_firstname")]
    public string? SpouseFirstName { get; set; }

    [Column("spouse_middlename")]
    public string? SpouseMiddleName { get; set; }

    [Column("mobileno")]
    public string? MobileNo { get; set; }

    [Column("spouse_occupation")]
    public string? SpouseOccupation { get; set; }

    [Column("spouse_employer_bus_name")]
    public string? SpouseEmployerBusName { get; set; }

    [Column("spouse_business_address")]
    public string? SpouseBusinessAddress { get; set; }

    [Column("spouse_telephone_no")]
    public string? SpouseTelephoneNo { get; set; }

    [Column("spouse_fathers_surname")]
    public string? SpouseFathersSurname { get; set; }

    [Column("spousef_first_name")]
    public string? SpouseFFirstName { get; set; }

    [Column("spousef_middle_name")]
    public string? SpouseFMiddleName { get; set; }

    [Column("spouse_mothers_maiden_name")]
    public string? SpouseMothersMaidenName { get; set; }

    [Column("spousem_surname")]
    public string? SpouseMSurname { get; set; }

    [Column("spousem_first_name")]
    public string? SpouseMFirstName { get; set; }

    [Column("spousem_middle_name")]
    public string? SpouseMMiddleName { get; set; }

    [Column("spouse_community_tax_certificate_no")]
    public string? SpouseCommunityTaxCertificateNo { get; set; }

    [Column("spouse_issued_at")]
    public string? SpouseIssuedAt { get; set; }

    [Column("spouse_issued")]
    public DateTime? SpouseIssued { get; set; }

    [Column("spouse_signature")]
    public byte[]? SpouseSignature { get; set; }

    [Column("spouse_date_accomplished")]
    public DateTime? SpouseDateAccomplished { get; set; }

    [Column("spousebirthdate")]
    public DateTime? SpouseBirthDate { get; set; }

    [Column("religion")]
    public string? Religion { get; set; }
        [Column("s2no")]
    public string? S2No { get; set; }

    [Column("specialization_id")]
    public long? SpecializationId { get; set; }
    public Specialization? Specialization { get; set; }
    public virtual EmployeeType? EmployeeType { get; set; }
    public virtual User? User{ get; set; }
    public virtual ICollection<Clinicdoctor>? ClinicDoctors { get; set; }
}
