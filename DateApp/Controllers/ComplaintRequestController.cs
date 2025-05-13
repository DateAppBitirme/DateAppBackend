using DateApp.Data;
using DateApp.Dtos.ComplaintRequestDto;
using DateApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DateApp.Controllers
{
    public class ComplaintRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ComplaintRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllComplaintAndRequests")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var complaintsAndRequests = await _context.ComplaintAndRequests
                .Where(c => c.IsActive && !c.IsDeleted)
                .Select(c => new ComplaintAndRequestDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    IsActive = c.IsActive,
                    UserName = c.User!.UserName
                })
                    .ToListAsync();

                return Ok(complaintsAndRequests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("CreateComplaintOrRequest")]
        public async Task<IActionResult> CreateComplaintOrRequest([FromBody] CreateComplaintRequestDto complaintAndRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var complaintAndRequest = new ComplaintAndRequest
                {
                    Title = complaintAndRequestDto.Title,
                    Description = complaintAndRequestDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    RequestTypeId = complaintAndRequestDto.RequestTypeId,
                    ComplaintTypeId = complaintAndRequestDto.ComplaintTypeId,
                    IsActive = true,
                    IsDeleted = false,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) // Kullanıcının ID'sini al
                };
                await _context.ComplaintAndRequests.AddAsync(complaintAndRequest);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Complaint or request created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
    

    
}