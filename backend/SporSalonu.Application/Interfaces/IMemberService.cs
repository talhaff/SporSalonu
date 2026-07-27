using SporSalonu.Application.DTOs.Member;

namespace SporSalonu.Application.Interfaces;

public interface IMemberService
{
    Task<List<MemberListDto>> GetAllAsync();
    Task<MemberDetailDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateMemberDto dto);
    Task UpdateAsync(int id, UpdateMemberDto dto);
    Task DeleteAsync(int id);
    Task<MemberListDto?> GetByPhoneAsync(string telefon);
}
