namespace QuanLyCuaHangDienTu_UNETI06_TI17A2HN.ViewModels;

public class CustomerProfileViewModel
{
    public string DisplayName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime? RegisteredAt { get; set; }
}
