using System.ComponentModel.DataAnnotations;

namespace demoUI.Models;

public enum EnrollmentStatus
{
    Active,
    Completed,
    Dropped
}

// ------------------------------------------------------------------
// API responses
// ------------------------------------------------------------------
public class ApiPagination
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class ApiPage<T>
{
    public List<T> Data { get; set; } = new();
    public ApiPagination Pagination { get; set; } = new();
}

public class StudentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public DateOnly? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public int ActiveCourses { get; set; }
}

public class CourseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Title { get; set; } = "";
    public int Credits { get; set; }
    public bool IsActive { get; set; }
}

public class EnrollmentDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = "";
    public string CourseTitle { get; set; } = "";
    public EnrollmentStatus Status { get; set; }
    public DateOnly EnrolledOn { get; set; }
}

public class AssessmentDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Name { get; set; } = "";
    public decimal MaxMarks { get; set; }
    public decimal Weightage { get; set; }
}

public class ScoreDto
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public int AssessmentId { get; set; }
    public string AssessmentName { get; set; } = "";
    public decimal MaxMarks { get; set; }
    public decimal MarksObtained { get; set; }
    public string? Remarks { get; set; }
}

public class AssessmentResultDto
{
    public int AssessmentId { get; set; }
    public string Name { get; set; } = "";
    public decimal MaxMarks { get; set; }
    public decimal Weightage { get; set; }
    public decimal? MarksObtained { get; set; }
    public decimal? WeightedScore { get; set; }
}

public class CourseResultDto
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = "";
    public string CourseTitle { get; set; } = "";
    public EnrollmentStatus Status { get; set; }
    public List<AssessmentResultDto> Assessments { get; set; } = new();
    public decimal FinalPercentage { get; set; }
    public string Grade { get; set; } = "PENDING";
    public bool IsComplete { get; set; }
}

public class StudentScorecardDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public List<CourseResultDto> Courses { get; set; } = new();
    public decimal? OverallPercentage { get; set; }
}

public class EnrollmentScorecardDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public CourseResultDto Course { get; set; } = new();
}

// ------------------------------------------------------------------
// Form models (validated in the browser before anything is sent)
// ------------------------------------------------------------------
public class StudentFormModel
{
    public const string EmailPattern = @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$";

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2 to 100 characters.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Email is required.")]
    [StringLength(150)]
    [RegularExpression(EmailPattern, ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = "";

    public DateOnly? DateOfBirth { get; set; }
}

public class CourseFormModel
{
    [Required(ErrorMessage = "Code is required.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Code must be 2 to 20 characters.")]
    public string Code { get; set; } = "";

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Title must be 2 to 150 characters.")]
    public string Title { get; set; } = "";

    [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10.")]
    public int Credits { get; set; } = 3;

    public bool IsActive { get; set; } = true;
}

public class AssessmentFormModel
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Name must be 2 to 80 characters.")]
    public string Name { get; set; } = "";

    [Range(0.01, 10000, ErrorMessage = "Max marks must be greater than 0.")]
    public decimal MaxMarks { get; set; } = 100;

    [Range(0.01, 100, ErrorMessage = "Weightage must be between 0.01 and 100.")]
    public decimal Weightage { get; set; }
}

// ------------------------------------------------------------------
// Small UI helpers shared by the pages
// ------------------------------------------------------------------
public static class UiHelpers
{
    public static string StatusBadge(EnrollmentStatus status) => status switch
    {
        EnrollmentStatus.Active => "bg-success",
        EnrollmentStatus.Completed => "bg-primary",
        _ => "bg-secondary"
    };

    public static string GradeBadge(string grade) => grade switch
    {
        "A+" or "A" => "bg-success",
        "B" or "C" => "bg-info text-dark",
        "D" => "bg-warning text-dark",
        "F" => "bg-danger",
        _ => "bg-secondary"
    };
}
