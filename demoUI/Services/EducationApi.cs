using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using demoUI.Models;

namespace demoUI.Services;

public class ApiResult<T>
{
    public bool Success { get; private init; }
    public T? Value { get; private init; }
    public string? Error { get; private init; }

    public static ApiResult<T> Ok(T? value) => new() { Success = true, Value = value };
    public static ApiResult<T> Fail(string error) => new() { Success = false, Error = error };
}

public class EducationApi
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _http;

    public EducationApi(HttpClient http)
    {
        _http = http;
    }

    // ------------------------------------------------------------------
    // Students
    // ------------------------------------------------------------------
    public Task<ApiResult<ApiPage<StudentDto>>> GetStudentsAsync(
        string? name, int page, int size, string sortBy, string sortOrder, CancellationToken ct = default) =>
        SendAsync<ApiPage<StudentDto>>(HttpMethod.Get,
            Url("Student", ("name", name), ("pageNumber", page), ("pageSize", size),
                ("sortBy", sortBy), ("sortOrder", sortOrder)), ct: ct);

    public Task<ApiResult<StudentDto>> GetStudentAsync(int id, CancellationToken ct = default) =>
        SendAsync<StudentDto>(HttpMethod.Get, $"Student/{id}", ct: ct);

    public Task<ApiResult<StudentDto>> CreateStudentAsync(
        StudentFormModel model, IEnumerable<int> courseIds, CancellationToken ct = default) =>
        SendAsync<StudentDto>(HttpMethod.Post, "Student",
            new { model.Name, model.Email, model.DateOfBirth, CourseIds = courseIds.ToList() }, ct);

    public Task<ApiResult<StudentDto>> UpdateStudentAsync(
        int id, StudentFormModel model, CancellationToken ct = default) =>
        SendAsync<StudentDto>(HttpMethod.Put, $"Student/{id}",
            new { model.Name, model.Email, model.DateOfBirth }, ct);

    public Task<ApiResult<bool>> DeleteStudentAsync(int id, CancellationToken ct = default) =>
        SendAsync<bool>(HttpMethod.Delete, $"Student/{id}", ct: ct);

    // ------------------------------------------------------------------
    // Courses
    // ------------------------------------------------------------------
    public Task<ApiResult<ApiPage<CourseDto>>> GetCoursesAsync(
        string? search, bool? isActive, int page, int size, CancellationToken ct = default) =>
        SendAsync<ApiPage<CourseDto>>(HttpMethod.Get,
            Url("Course", ("search", search), ("isActive", isActive), ("pageNumber", page), ("pageSize", size)), ct: ct);

    public Task<ApiResult<CourseDto>> GetCourseAsync(int id, CancellationToken ct = default) =>
        SendAsync<CourseDto>(HttpMethod.Get, $"Course/{id}", ct: ct);

    public Task<ApiResult<CourseDto>> CreateCourseAsync(CourseFormModel model, CancellationToken ct = default) =>
        SendAsync<CourseDto>(HttpMethod.Post, "Course", model, ct);

    public Task<ApiResult<CourseDto>> UpdateCourseAsync(int id, CourseFormModel model, CancellationToken ct = default) =>
        SendAsync<CourseDto>(HttpMethod.Put, $"Course/{id}", model, ct);

    public Task<ApiResult<bool>> DeleteCourseAsync(int id, CancellationToken ct = default) =>
        SendAsync<bool>(HttpMethod.Delete, $"Course/{id}", ct: ct);

    // ------------------------------------------------------------------
    // Enrollments
    // ------------------------------------------------------------------
    public Task<ApiResult<ApiPage<EnrollmentDto>>> GetEnrollmentsAsync(
        int? studentId, int? courseId, EnrollmentStatus? status, int page, int size, CancellationToken ct = default) =>
        SendAsync<ApiPage<EnrollmentDto>>(HttpMethod.Get,
            Url("Enrollment", ("studentId", studentId), ("courseId", courseId), ("status", status),
                ("pageNumber", page), ("pageSize", size)), ct: ct);

    public Task<ApiResult<EnrollmentDto>> EnrollAsync(int studentId, int courseId, CancellationToken ct = default) =>
        SendAsync<EnrollmentDto>(HttpMethod.Post, "Enrollment", new { studentId, courseId }, ct);

    public Task<ApiResult<EnrollmentDto>> ChangeEnrollmentStatusAsync(
        int id, EnrollmentStatus status, CancellationToken ct = default) =>
        SendAsync<EnrollmentDto>(HttpMethod.Put, $"Enrollment/{id}/status", new { status }, ct);

    // ------------------------------------------------------------------
    // Assessments
    // ------------------------------------------------------------------
    public Task<ApiResult<List<AssessmentDto>>> GetAssessmentsAsync(int courseId, CancellationToken ct = default) =>
        SendAsync<List<AssessmentDto>>(HttpMethod.Get, Url("Assessment", ("courseId", courseId)), ct: ct);

    public Task<ApiResult<AssessmentDto>> CreateAssessmentAsync(
        int courseId, AssessmentFormModel model, CancellationToken ct = default) =>
        SendAsync<AssessmentDto>(HttpMethod.Post, "Assessment",
            new { courseId, model.Name, model.MaxMarks, model.Weightage }, ct);

    public Task<ApiResult<AssessmentDto>> UpdateAssessmentAsync(
        int id, AssessmentFormModel model, CancellationToken ct = default) =>
        SendAsync<AssessmentDto>(HttpMethod.Put, $"Assessment/{id}",
            new { model.Name, model.MaxMarks, model.Weightage }, ct);

    public Task<ApiResult<bool>> DeleteAssessmentAsync(int id, CancellationToken ct = default) =>
        SendAsync<bool>(HttpMethod.Delete, $"Assessment/{id}", ct: ct);

    // ------------------------------------------------------------------
    // Scores and scorecards
    // ------------------------------------------------------------------
    public Task<ApiResult<List<ScoreDto>>> GetScoresAsync(int enrollmentId, CancellationToken ct = default) =>
        SendAsync<List<ScoreDto>>(HttpMethod.Get, Url("Score", ("enrollmentId", enrollmentId)), ct: ct);

    public Task<ApiResult<ScoreDto>> SaveScoreAsync(
        int enrollmentId, int assessmentId, decimal marks, string? remarks, CancellationToken ct = default) =>
        SendAsync<ScoreDto>(HttpMethod.Put, "Score",
            new { enrollmentId, assessmentId, marksObtained = marks, remarks }, ct);

    public Task<ApiResult<bool>> DeleteScoreAsync(int id, CancellationToken ct = default) =>
        SendAsync<bool>(HttpMethod.Delete, $"Score/{id}", ct: ct);

    public Task<ApiResult<StudentScorecardDto>> GetStudentScorecardAsync(int studentId, CancellationToken ct = default) =>
        SendAsync<StudentScorecardDto>(HttpMethod.Get, $"Scorecard/student/{studentId}", ct: ct);

    public Task<ApiResult<EnrollmentScorecardDto>> GetEnrollmentScorecardAsync(int enrollmentId, CancellationToken ct = default) =>
        SendAsync<EnrollmentScorecardDto>(HttpMethod.Get, $"Scorecard/enrollment/{enrollmentId}", ct: ct);

    // ------------------------------------------------------------------
    // Plumbing
    // ------------------------------------------------------------------
    private async Task<ApiResult<T>> SendAsync<T>(
        HttpMethod method, string url, object? body = null, CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(method, url);
            if (body is not null)
                request.Content = JsonContent.Create(body, body.GetType(), options: Json);

            using var response = await _http.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
                return ApiResult<T>.Fail(await ReadErrorAsync(response, ct));

            if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
                return ApiResult<T>.Ok(default);

            var value = await response.Content.ReadFromJsonAsync<T>(Json, ct);
            return ApiResult<T>.Ok(value);
        }
        catch (HttpRequestException)
        {
            return ApiResult<T>.Fail("Cannot reach the API. Check that it is running and the base address is correct.");
        }
        catch (TaskCanceledException)
        {
            return ApiResult<T>.Fail("The request timed out.");
        }
    }

    private static string Url(string path, params (string Key, object? Value)[] query)
    {
        var parts = query
            .Where(q => q.Value is not null && !string.IsNullOrWhiteSpace(Convert.ToString(q.Value, CultureInfo.InvariantCulture)))
            .Select(q => $"{q.Key}={Uri.EscapeDataString(Convert.ToString(q.Value, CultureInfo.InvariantCulture)!)}");

        var queryString = string.Join("&", parts);
        return queryString.Length == 0 ? path : $"{path}?{queryString}";
    }

    // The API answers with { "error": "..." }, a plain string, or ASP.NET validation problem details
    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var text = await response.Content.ReadAsStringAsync(ct);

        try
        {
            using var doc = JsonDocument.Parse(text);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.String)
                return root.GetString() ?? "The request failed.";

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("error", out var error) && error.ValueKind == JsonValueKind.String)
                    return error.GetString()!;

                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                {
                    var messages = errors.EnumerateObject()
                        .SelectMany(p => p.Value.EnumerateArray().Select(v => v.GetString()))
                        .Where(m => !string.IsNullOrWhiteSpace(m));

                    var joined = string.Join(" ", messages);
                    if (joined.Length > 0) return joined;
                }

                if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
                    return title.GetString()!;
            }
        }
        catch (JsonException)
        {
            // not JSON, fall through
        }

        return string.IsNullOrWhiteSpace(text)
            ? $"The request failed ({(int)response.StatusCode})."
            : text;
    }
}
